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

        [HttpGet("createacc")]
        public async Task<IActionResult> TestCreateAccount()
        {
            string result = await _database.CreateUser("bali", "test@test.test", "xdxdxd", 100000000000000);
            return Ok(result);
        }
    }
}