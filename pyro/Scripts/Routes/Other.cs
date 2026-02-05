using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class OtherController : ControllerBase
    {
        private readonly Database _database;

        public OtherController(Database database)
        {
            _database = database;
        }

        // inibe ki van kapcsolva tho
        [HttpGet("waitingroom/api/waitingroom")]
        public async Task<IActionResult> WaitingRoom()
        {
            return NoContent();
        }

        [HttpPost("datarouter/api/v1/public/data")]
        public async Task<IActionResult> DataRouter()
        {
            return NoContent();
        }

        [HttpGet("eulatracking/api/public/agreements/fn/account/{accountId}")]
        public async Task<IActionResult> EulaTracking(string accountId)
        {
            return NoContent();
        }

        [HttpGet("fortnite/api/game/v2/enabled_features")]
        public async Task<IActionResult> EnabledFeatures()
        {
            return Ok(Array.Empty<int>());
        }

        [HttpGet("fortnite/api/v2/versioncheck/{platform}")]
        public async Task<IActionResult> VersionCheck()
        {
            return Ok(new { type = "NO_UPDATE" });
        }
    }
}