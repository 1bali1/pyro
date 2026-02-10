using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class ContentPages : ControllerBase
    {
        private readonly dynamic _contentPages;

        public ContentPages(dynamic contentPages)
        {
            _contentPages = contentPages;
        }

        [HttpGet("content/api/pages/fortnite-game")]
        public async Task<IActionResult> GetContentPages()
        {
            return Content(_contentPages, "application/json");
        }
    }
}