using Microsoft.AspNetCore.Mvc;

namespace Pyro.Scripts.Routes
{
    [ApiController]
    public class OtherController : ControllerBase
    {
        [HttpGet("waitingroom/api/waitingroom")]
        public async Task<IActionResult> WaitingRoom()
        {
            return NoContent();
        }
    }
}