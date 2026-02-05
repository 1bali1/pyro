using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MongoDB.Driver;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class Auth : ControllerBase
    {
        private readonly Database _database;
        private readonly string[] allowedGrantTypes = ["password", "refresh", "client_credentials"];
        private const int clientExpireTime = 5; // óra 
        private const int accessExpireTime = 10;
        private const int refreshExpireTime = 40;

        public Auth(Database database)
        {
            _database = database;
        }

        [HttpPost("account/api/oauth/token")]
        public async Task<IActionResult> OAuth()
        {
            var data = Request.Form;
            var headers = Request.Headers;
            string ?clientId = null;

            try
            {
                string authorization = headers["Authorization"]!;
                byte[] decodedBytes = Convert.FromBase64String(authorization.Split(" ")[1]);
                string decodedString = Encoding.UTF8.GetString(decodedBytes);
                clientId = decodedString.Split(":", 1)[0];

                if (clientId == null) throw new Exception();
            }
            catch (Exception)
            {
                var error = await new BackendError("errors.com.epicgames.common.oauth.invalid_client", "Missing 'authorization' header! Please check your request!", [], 1011, 400).Create(Response);
                return error;
            }

            string grantType = data["grant_type"]!;

            if (!allowedGrantTypes.Contains(grantType))
            {
                var error = await new BackendError("errors.com.epicgames.common.oauth.invalid_request", "The 'grant_type' parameter is missing!", [], 1011, 400).Create(Response);
                return error;
            }

            User ?user = null;
            var tokenManager = new TokenManager(_database);

            switch (grantType)
            {
                case "password":
                    StringValues email;
                    StringValues password;
                    data.TryGetValue("username", out email);
                    data.TryGetValue("password", out password);

                    if(StringValues.IsNullOrEmpty(email) || StringValues.IsNullOrEmpty(password))
                    {
                        var error = await new BackendError("errors.com.epicgames.common.oauth.invalid_request", "Email address or password is missing!", [], 1011, 400).Create(Response);
                        return error;
                    }
                    
                    user = await _database.users.Find(p => p.email == (string)email!).FirstOrDefaultAsync();

                    if(user == null)
                    {
                        var error = await new BackendError("errors.com.epicgames.account.invalid_account_credentials", "The email or password you entered is incorrect!", [], 1011, 401).Create(Response);
                        return error;
                    }

                    if(!BCrypt.Net.BCrypt.Verify((string)password!, user.password))
                    {
                        var error = await new BackendError("errors.com.epicgames.account.invalid_account_credentials", "The email or password you entered is incorrect!", [], 1011, 401).Create(Response);
                        return error;
                    }
                    
                    break;

                case "refresh":
                    // TODO: token frissítése
                    Console.WriteLine("Refresh");
                    break;

                case "client_credentials":
                    string token = await tokenManager.GenerateClientToken(Request.Host.ToString(), clientId, grantType, clientExpireTime);
                    var expiresAt = Utils.Utils.GetIsoDatetime(DateTime.Now.AddHours(clientExpireTime));

                    var clientData = new
                    {
                        access_token = $"eg1~{token}",
                        expires_in = clientExpireTime*3600,
                        expires_at = expiresAt,
                        internal_client = true,
                        client_service = "fortnite"
                    };
                    return Ok(clientData);

                default: break;
            }

            if(user == null)
            {
                var error = await new BackendError("errors.com.epicgames.account.invalid_account_credentials", "An error occurred while logging in!", [], 1011, 401).Create(Response);
                return error;
            }

            if (user.isBanned)
            {
                var error = await new BackendError("errors.com.epicgames.common.oauth.account_forbidden", "You are banned from the backend!", [], 1012, 401).Create(Response);
                return error;
            }

            string deviceId = Utils.Utils.GenerateUuid();

            string accessToken = await tokenManager.GenerateAccessToken(user.accountId, user.username, clientId, deviceId, grantType, accessExpireTime);
            string refreshToken = await tokenManager.GenerateRefreshToken(user.accountId, clientId, deviceId, grantType, refreshExpireTime);

            var returnData = new
            {
                access_token = $"eg1~{accessToken}",
                expires_in = accessExpireTime*3600,
                expires_at = Utils.Utils.GetIsoDatetime(DateTime.Now.AddHours(accessExpireTime)),
                token_type = "bearer",
                refresh_token = $"eg1~{refreshToken}",
                refresh_expires = refreshExpireTime*3600,
                refresh_expires_at = Utils.Utils.GetIsoDatetime(DateTime.Now.AddHours(refreshExpireTime)),
                account_id = user.accountId,
                client_id = clientId,
                internal_client = true,
                client_service = "fortnite",
                displayName = user.username,
                app = "fortnite",
                in_app_id = user.accountId,
                device_id = deviceId
            
            };

            return Ok(returnData);
        }

        [HttpGet("account/api/oauth/verify"), RequiresAuthorization]
        public async Task<IActionResult> OAuthVerify()
        {
            var user = HttpContext.Items["user"] as User;
            var headers = Request.Headers;

            StringValues authorization;
            headers.TryGetValue("Authorization", out authorization);

            string encodedToken = authorization.ToString().Split(" ")[1].Replace("eg1~", "");
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(encodedToken);

            DateTime expiresAt;
            DateTime.TryParse(decodedToken.Claims.First(claim => claim.Type == "creation_date").Value, out expiresAt);

            var returnData = new
            {
                token = encodedToken,
                session_id = decodedToken.Claims.First(claim => claim.Type == "jti"),
                token_type = "bearer",
                client_id = decodedToken.Claims.First(claim => claim.Type == "clid"),
                internal_client = true,
                client_service = "fortnite",
                account_id = user!.accountId,
                expires_in = (expiresAt - DateTime.Now).Seconds,
                expires_at = expiresAt,
                auth_method = decodedToken.Claims.First(claim => claim.Type == "am"),
                display_name = user.username,
                app = "fortnite",
                in_app_id = user.accountId,
                device_id = decodedToken.Claims.First(claim => claim.Type == "dvid"),
            };

            return Ok(returnData);
        }
    }
}