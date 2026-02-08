using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class ContentPages : ControllerBase
    {
        private readonly object _contentPages;

        public ContentPages(object contentPages)
        {
            _contentPages = contentPages;
        }

        [HttpGet("content/api/pages/fortnite-game"), RequiresAuthorization]
        public async Task<IActionResult> GetContentPages()
        {
            return Ok(_contentPages);
        }
    }
}