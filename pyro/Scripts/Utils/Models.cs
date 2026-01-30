using System.Text.Json.Nodes;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace pyro.Scripts.Utils
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public string accountId { get; set; } = default!;
        public long discordUserId { get; set; } = default!; // most nem tom fejlből
        public string username { get; set; } = default!;
        public string email { get; set; } = default!;
        public string password { get; set; } = default!;
        public bool isBanned { get; set; }
        public int vbucks { get; set; }
        public DateTime createdOn { get; set; }
        public Dictionary<string, BsonDocument> profiles { get; set; } = default!;
        public Dictionary<string, object> friendSystem { get; set; } = default!;
    }
    
    public class Token
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public string accountId { get; set; } = default!;
        public string accessToken { get; set; } = default!;
        public string refreshToken { get; set; } = default!;
    }
}