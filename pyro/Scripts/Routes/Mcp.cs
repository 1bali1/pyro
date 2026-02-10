using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class Mcp : ControllerBase
    {
        private static readonly string[] allowedOperations = [
            "QueryProfile",
            "ClientQuestLogin",
            "GetMcpTimeForLogin",
            "RefreshExpeditions",
            "BulkEquipBattleRoyaleCustomization",
            "SetMtxPlatform",
            "SetHardcoreModifier",
            "IncrementNamedCounterStat",
            "SetAffiliateName",
            "EquipBattleRoyaleCustomization",
            "SetCosmeticLockerSlot",
            "SetItemFavoriteStatusBatch",
            "MarkItemSeen"
        ];

        private static readonly string[] allowedProfiles = ["athena", "common_core", "common_public", "creative", "collections"];

        // TODO: update dolgok
        // TODO: hiányzó profile: collections

        [HttpPost("/fortnite/api/game/v2/profile/{accountId}/client/{operation}"), RequiresAuthorization]
        public async Task<IActionResult> McpProfile(string accountId, string operation)
        {
            var query = Request.Query;
            string profileId = query["profileId"]!;

            if (!allowedOperations.Contains(operation) || !allowedProfiles.Contains(profileId))
            {
                var error = await new BackendError("errors.com.epicgames.modules.profiles.operation_not_found", "There's no such operation!", [], 12813, 400).Create(Response);
                return error;
            }

            var user = HttpContext.Items["user"] as User;

            var profile = user!.profiles[profileId];

            if(profile == null)
            {
                var error = await new BackendError("errors.com.epicgames.modules.profiles.operation_not_found", "There's no such operation!", [], 12813, 400).Create(Response);
                return error;
            }

            int profileRvn = profile["rvn"].AsInt32;
            int commandRvn = profile["commandRevision"].AsInt32;

            if(profileRvn == commandRvn) profileRvn++;

            if(profileId == "athena") profile["stats"]["seasonNum"] = Utils.Utils.seasonNumber;
            if(profileId == "common_core")
            {
                profile["items"]["Currency:MtxPurchased"]["quantity"] = user.vbucks;
            }
            
            var changes = new List<object>();

            changes.Add(new
            {
                profile = BsonTypeMapper.MapToDotNetValue(profile),
                changeType = "fullProfileUpdate"
            });

            var returnData = new
            {
                profileRevision = profileRvn,
                profileId = profileId,
                profileChangesBaseRevision = profileRvn,
                profileChanges = changes,
                profileCommandRevision = commandRvn,
                serverTime = Utils.Utils.GetIsoDatetime(DateTime.Now),
                multiUpdate = Array.Empty<int>(),
                responseVersion = 1
            };

            return Ok(returnData);
        }

        // ! lehet hogy get
        [HttpGet("/fortnite/api/game/v2/profile/{accountId}/dedicated_server/{operation}"), RequiresAuthorization] // ? TODO: gs auth
        public async Task<IActionResult> DedicatedProfile(string accountId, string operation)
        {
            return Ok();
        }
    }
}