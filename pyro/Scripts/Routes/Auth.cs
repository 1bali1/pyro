using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class Auth : ControllerBase
    {
        private readonly Database _database;
        private readonly string[] allowedGrantTypes = ["password", "refresh", "client_credentials"];

        public Auth(Database database)
        {
            _database = database;
        }

        [HttpPost("account/api/oauth/token")]
        public async Task<IActionResult> OAuth()
        {
            var data = Request.Form;
            var headers = Request.Headers;
            string[] ?clientId = null;

            try
            {
                string authorization = headers["authorization"]!;
                byte[] decodedBytes = Convert.FromBase64String(authorization.Split(":")[0]);
                string decodedString = Encoding.UTF8.GetString(decodedBytes);
                clientId = decodedString.Split(":", 1);

                if (clientId == null) throw new Exception();
            }
            catch (Exception)
            {
                var error = await new BackendError("errors.com.epicgames.common.oauth.invalid_client", "Hiányzó 'authorization' header! Kérlek ellenőrizd a kérelmedet!", [], 1011, 400).Create(Response);
                return error;
            }

            string grantType = data["grant_type"]!;

            if (!allowedGrantTypes.Contains(grantType))
            {
                var error = await new BackendError("errors.com.epicgames.common.oauth.invalid_request", "Hiányzik a 'grant_type' paraméter!", [], 1011, 400).Create(Response);
                return error;
            }

            User ?user = null;
            
            switch (grantType)
            {
                case "password":
                    StringValues email;
                    StringValues password;
                    data.TryGetValue("username", out email);
                    data.TryGetValue("password", out password);

                    if(StringValues.IsNullOrEmpty(email) || StringValues.IsNullOrEmpty(password))
                    {
                        // TODO: error
                        return Forbid();
                    }
                    // TODO: db query
                    
                    break;
                default: break;
            }

            return Ok();
        }
    }
}