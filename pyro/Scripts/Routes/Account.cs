using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MongoDB.Driver;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class Account : ControllerBase
    {
        private readonly Database _database;

        public Account(Database database)
        {
            _database = database;
        }

        [HttpGet("/account/api/public/account"), RequiresAuthorization]
        public async Task<IActionResult> QueryAccountGet()
        {
            var query = Request.Query;

            StringValues accountId;
            query.TryGetValue("accountId", out accountId);

            var user = await _database.users.Find(p => p.accountId == (string)accountId!).FirstOrDefaultAsync();

            if (user == null)
            {
                var error = await new BackendError("errors.com.epicgames.common.oauth.invalid_client", "A fiók nem található!", [], 1011, 404).Create(Response);
                return error;
            }

            var returnData = new[]
            {
                new
                {
                    id = user.accountId,
                    displayName = user.username,
                    externalAuths = new {}
                }
            };

            return Ok(returnData);
        }

        [HttpGet("account/api/public/account/{accountId}"), RequiresAuthorization]
        public async Task<IActionResult> GetAccount(string accountId)
        {
            var user = HttpContext.Items["user"] as User;
            string redactedEmail = $"******@{user!.email.Split('@')[0]}";

            var returnData = new
            {
                id = user.accountId,
                displayName = user.username,
                name = "Pyro",
                email = redactedEmail,
                failedLoginAttempts = 0,
                lastLogin = Utils.Utils.GetIsoDatetime(DateTime.Now),
                numberOfDisplayNameChanges = 0,
                ageGroup = "UNKNOWN",
                headless = false,
                country = "US",
                lastName = "Pyro",
                preferredLanguage = "en",
                canUpdateDisplayName = false,
                tfaEnabled = false,
                emailVerified = true,
                minorVerified = false,
                minorStatus = "NOT_MINOR",
                cabinedMode = false,
                hasHashedEmail = false,
            };

            return Ok(returnData);
        }

        [HttpGet("account/api/public/account/{accountId}/externalAuths")]
        public async Task<IActionResult> AccountExternalAuths(string accountId)
        {
            return Ok(Array.Empty<int>());
        }

        [HttpPost("api/v1/user/setting"), RequiresAuthorization]
        public async Task<IActionResult> UserSetting()
        {
            return Ok(new {});
        }
    }
}