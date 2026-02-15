using System.Text.Json.Nodes;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ZstdSharp.Unsafe;

namespace pyro.Scripts.Utils
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public string accountId { get; set; } = default!;
        public ulong discordUserId { get; set; } = default!; // most nem tom fejlből
        public string username { get; set; } = default!;
        public string email { get; set; } = default!;
        public string password { get; set; } = default!;
        public bool isBanned { get; set; }
        public int vbucks { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime? lastDailyVBuckClaim { get; set; } = null;
        public Dictionary<string, BsonDocument> profiles { get; set; } = default!;
        public FriendSystem friendSystem { get; set; } = default!;
    }

    public class FriendSystem
    {
        public bool acceptInvites { get; set; } = true;
        public string mutualPrivacy { get; set; } = "ALL";
        public Dictionary<string, Friend> friends { get; set; } = new();
        public List<string> blockedUsers { get; set; } = new();
    }

    public class Friend
    {
        public string accountId { get; set; } = default!;
        public string nickname { get; set; } = default!;
        public string note { get; set; } = default!;
        public string status { get; set; } = default!; // ACCEPTED - PENDING
        public string direction { get; set; } = default!; // INBOUND - OUTBOUND
        public bool favorite { get; set; }
        public bool blocked { get; set; }
        public string created { get; set; } = default!;
    }

    public class UserToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public string accountId { get; set; } = default!;
        public string accessToken { get; set; } = default!;
        public string refreshToken { get; set; } = default!;
    }

    public class ClientToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public string ipAddress { get; set; } = default!;
        public string clientToken { get; set; } = default!;
    }

    public class MItemshop
    {
        public AutoItemshop autoItemshop { get; set; } = default!;
        public Dictionary<string, ShopItem> items { get; set; } = default!;
    }
    public class AutoItemshop
    {
        public bool enabled { get; set; }
        public List<string> blacklistedItems { get; set; } = default!;
        public List<Bundle> bundles { get; set; } = default!;
        public Dictionary<string, Dictionary<string, int>> prices { get; set; } = default!;
    }

    public class Bundle
    {
        public List<string> items { get; set; } = default!;
        public int price { get; set; }
    }

    public class ShopItem
    {
        public List<string> items { get; set; } = default!;
        public int price { get; set; }
    }
    public class CatalogEntry
    {
        public string devName { get; set; } = "";
        public string offerId { get; set; } = "";
        public List<string> fulfillmentIds { get; set; } = new();
        public int dailyLimit { get; set; } = -1;
        public int weeklyLimit { get; set; } = -1;
        public int monthlyLimit { get; set; } = -1;
        public List<string> categories { get; set; } = new();
        public List<ItemPriceInfo> prices { get; set; } = new();
        public ItemMetaData meta { get; set; } = new();
        public string matchFilter { get; set; } = "";
        public int filterWeight { get; set; } = 0;
        public List<string> appStoreId { get; set; } = new();
        public List<ItemRequirementInfo> requirements { get; set; } = new();
        public string offerType { get; set; } = "StaticPrice";
        public ItemGiftInfo giftInfo { get; set; } = new();
        public bool refundable { get; set; } = false;
        public List<MetaInfoItem> metaInfo { get; set; } = new()
        {
            new MetaInfoItem { key = "SectionId", value = "Featured" },
            new MetaInfoItem { key = "TileSize", value = "Small" }
        };
        public string displayAssetPath { get; set; } = "";
        public List<ItemGrantInfo> itemGrants { get; set; } = new();
        public int sortPriority { get; set; } = 0;
        public int catalogGroupPriority { get; set; } = 0;
    }

    public class ItemPriceInfo
    {
        public string currencyType { get; set; } = "MtxCurrency";
        public string currencySubType { get; set; } = "";
        public int regularPrice { get; set; } = 0;
        public int finalPrice { get; set; } = 0;
        public string saleExpiration { get; set; } = "9999-12-02T01:12:00Z";
        public int basePrice { get; set; } = 0;
    }

    public class ItemMetaData
    {
        public string SectionId { get; set; } = "Featured";
        public string TileSize { get; set; } = "Small";
    }

    public class ItemGiftInfo
    {
        public bool bIsEnabled { get; set; } = true;
        public string forcedGiftBoxTemplateId { get; set; } = "";
        public List<object> purchaseRequirements { get; set; } = new();
        public List<object> giftRecordIds { get; set; } = new();
    }

    public class MetaInfoItem
    {
        public string key { get; set; } = "";
        public string value { get; set; } = "";
    }

    public class ItemRequirementInfo
    {
        public string requirementType { get; set; } = "";
        public string requiredId { get; set; } = "";
        public int minQuantity { get; set; } = 1;
    }

    public class ItemGrantInfo
    {
        public string templateId { get; set; } = "";
        public int quantity { get; set; } = 1;
    }
    public class ShopResponse
    {
        public int refreshIntervalHrs { get; set; } = 24;
        public int dailyPurchaseHrs { get; set; } = 24;
        public string expiration { get; set; } = "";
        public List<Storefront> storefronts { get; set; } = new();
    }

    public class Storefront
    {
        public string name { get; set; } = "";
        public List<CatalogEntry> catalogEntries { get; set; } = new();
    }
}