using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace pyro.Scripts.Utils
{
    public class RequiresAuthorization : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            filterContext.Result = new UnauthorizedResult();
        }
    }

    public class TokenManager
    {
        private readonly string _secretKey;
        private readonly Database _database;
        public TokenManager(Database database)
        {
            var env = dotenv.net.DotEnv.Read();
            _secretKey = env["JWT_SECRET_KEY"];
            _database = database;
        }
        public async Task<string> GenerateAccessToken(string accountId, string username, string clientId, string deviceId, string grantType, int expires)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Utils.GenerateUuid());
            string base64Uuid = Convert.ToBase64String(bytes);

            var claims = new List<Claim>
            {
                new Claim("sub", accountId),
                new Claim("hours_expire", expires.ToString()),
                new Claim("creation_date", Utils.GetIsoDatetime(DateTime.Now)),
                new Claim("app", "fortnite"),
                new Claim("dvid", deviceId),
                new Claim("clid", clientId),
                new Claim("mver", "false"),
                new Claim("dn", username),
                new Claim("am", grantType),
                new Claim("p", base64Uuid),
                new Claim("iai", accountId),
                new Claim("sec", "1"),
                new Claim("t", "s"),
                new Claim("clsvc", "fortnite"),
                new Claim("ic", "true"),
                new Claim("jti", Utils.GenerateUuid())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(expires),
                signingCredentials: creds
            );

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            await _database.AddToken("accessToken", accountId, tokenStr);

            return tokenStr;
        }


        public async Task<string> GenerateRefreshToken(string accountId, string clientId, string deviceId, string grantType, int expires)
        {
            var claims = new List<Claim>
            {
                new Claim("sub", accountId),
                new Claim("hours_expire", expires.ToString()),
                new Claim("creation_date", Utils.GetIsoDatetime(DateTime.Now)),
                new Claim("dvid", deviceId),
                new Claim("clid", clientId),
                new Claim("am", grantType),
                new Claim("t", "r"),
                new Claim("jti", Utils.GenerateUuid())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(expires),
                signingCredentials: creds
            );

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            await _database.AddToken("refreshToken", accountId, tokenStr);

            return tokenStr;
        }

        public async Task<string> GenerateClientToken(string ipAddress, string clientId, string grantType, int expires)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Utils.GenerateUuid());
            string base64Uuid = Convert.ToBase64String(bytes);

            var claims = new List<Claim>
            {
                new Claim("hours_expire", expires.ToString()),
                new Claim("creation_date", Utils.GetIsoDatetime(DateTime.Now)),
                new Claim("clid", clientId),
                new Claim("mver", "false"),
                new Claim("am", grantType),
                new Claim("p", base64Uuid),
                new Claim("t", "s"),
                new Claim("clsvc", "fortnite"),
                new Claim("ic", "true"),
                new Claim("jti", Utils.GenerateUuid())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(expires),
                signingCredentials: creds
            );

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            await _database.AddClientToken(ipAddress, tokenStr);

            return tokenStr;
        }
    }
}