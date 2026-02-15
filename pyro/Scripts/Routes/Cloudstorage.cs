
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class Cloudstorage : ControllerBase
    {
        [HttpGet("fortnite/api/cloudstorage/system"), RequiresAuthorization]
        public async Task<IActionResult> CloudstorageSystem()
        {
            var cloudFiles = new List<dynamic>();

            foreach (string item in Directory.GetFiles(Utils.Utils.cloudstoragePath))
            {
                string name = item.Replace(Utils.Utils.cloudstoragePath, "");
                if (!name.EndsWith(".ini")) continue;
                
                string path = Utils.Utils.cloudstoragePath + name;
                var content = await System.IO.File.ReadAllTextAsync(path);

                cloudFiles.Add(new
                {
                    uniqueFilename = name,
                    filename = name,
                    hash = GetHash(content, "sha1"),
                    hash256 = GetHash(content, "sha256"),
                    length = content.Length,
                    contentType = "application/octet-stream",
                    uploaded = DateTime.Now,
                    storageType = "S3",
                    storageIds = new {},
                    doNotCache = true
                });
            }

            return Ok(cloudFiles);
        }

        [HttpGet("fortnite/api/cloudstorage/system/{filename}")]
        public async Task<IActionResult> CloudstorageSpec(string filename, IWebHostEnvironment env)
        {
            string name = Path.GetFileName(filename);
            string path = Path.Combine(env.ContentRootPath, Utils.Utils.cloudstoragePath, name);

            if(!System.IO.File.Exists(path)) return NotFound();

            return PhysicalFile(path, "application/octet-stream");
        }

        [HttpGet("fortnite/api/cloudstorage/system/config"), RequiresAuthorization]
        public async Task<IActionResult> CloudstorageConfig()
        {
            return Ok(Array.Empty<int>());
        }

        private string GetHash(string data, string algo)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(data);
            byte[] hashBytes;

            if (algo == "sha1")
            {
                hashBytes = SHA1.HashData(inputBytes);
            }
            else
            {
                hashBytes = SHA256.HashData(inputBytes);
            }

            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
}