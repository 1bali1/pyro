using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class Itemshop : ControllerBase
    {
        private readonly List<string> _keychain;
        private readonly MItemshop _itemshop;

        public Itemshop(List<string> keychain, MItemshop itemshop)
        {
            _keychain = keychain;
            _itemshop = itemshop;
        }

        [HttpGet("fortnite/api/storefront/v2/keychain")]
        public async Task<IActionResult> Keychain()
        {
            return Ok(_keychain);
        }

        [HttpGet("fortnite/api/storefront/v2/catalog"), RequiresAuthorization]
        public async Task<IActionResult> Catalog()
        {
            var shop = GetItemshop();
            return Ok(shop);
        }

        private ShopResponse GetItemshop()
        {
            string date = "9999-01-02T00:00:00.000Z"; // ? auto item shop miatt még lehet hogy piszkálni kell
            var jsonData = new ShopResponse() { expiration = date };

            jsonData.storefronts.Add(new Storefront { name = "BRDailyStorefront" });
            jsonData.storefronts.Add(new Storefront { name = "BRWeeklyStorefront" });
            jsonData.storefronts.Add(new Storefront { name = "BRSpecialFeatured" });

            foreach (var i in _itemshop.items)
            {
                var item = i.Value;
                if (item.items.Count <= 0) continue;

                var itemEntry = new CatalogEntry();

                int storefrontIndex = 0;
                if (i.Key.StartsWith("daily")) itemEntry.sortPriority = -1;
                else
                {
                    itemEntry.meta.TileSize = "Normal";
                    itemEntry.metaInfo[1].value = "Normal";
                }

                if (i.Key.StartsWith("featured")) storefrontIndex = 2;
                else if (i.Key.StartsWith("weekly")) storefrontIndex = 1;

                foreach (var grant in item.items)
                {
                    itemEntry.requirements.Add(new ItemRequirementInfo { requirementType = "DenyOnItemOwnership", requiredId = grant, minQuantity = 1 });
                    itemEntry.itemGrants.Add(new ItemGrantInfo { templateId = grant, quantity = 1 });
                }

                itemEntry.prices = new List<ItemPriceInfo>
                {
                    new ItemPriceInfo
                    {
                        regularPrice = item.price,
                        finalPrice = item.price,
                        basePrice = item.price,
                        saleExpiration = date,
                    }
                };

                string itemGrantJson = JsonConvert.SerializeObject(item.items, Formatting.None);
                string itemStr = $"{itemGrantJson}_{item.price}";
                byte[] hashBytes = SHA1.HashData(Encoding.UTF8.GetBytes(itemStr));
                string itemHash = Convert.ToHexString(hashBytes).ToLower(); itemEntry.devName = itemHash;
                itemEntry.offerId = itemHash;

                // TODO: global offereket berakni, vásárláshoz kell madj

                jsonData.storefronts[storefrontIndex].catalogEntries.Add(itemEntry);
            }

            return jsonData;
        }
    }
}