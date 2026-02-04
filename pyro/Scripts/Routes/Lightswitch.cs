using Microsoft.AspNetCore.Mvc;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class Lightswitch : ControllerBase
    {
        [HttpGet("lightswitch/api/service/bulk/status")]
        public async Task<IActionResult> LightswitchStatus()
        {
            var data = new
            {
                serviceInstanceId = "fortnite",
                status = "UP",
                message = "Pyro backend fut...",
                maintenanceUri = "null",
                overrideCatalogIds = Array.Empty<int>(),
                allowedActions = new []{"PLAY", "DOWNLOAD"},
                banned = false,
                launcherInfoDTO = new
                {
                    appName = "Fortnite",
                    catalogItemId = "",
                    @namespace = "fn"
                }
            };
            return Ok(new []{data});
        }
    }
}