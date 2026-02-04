using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class Itemshop : ControllerBase
    {
        private readonly List<string> _keychain;

        public Itemshop(List<string> keychain)
        {
            _keychain = keychain;
        }

        [HttpGet("fortnite/api/storefront/v2/keychain")]
        public async Task<IActionResult> Keychain()
        {
            return Ok(_keychain);
        }
    }
}