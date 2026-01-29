using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace pyro.Scripts.Utils
{
    class Utils
    {
        const string profilesPath = "Data/Models/";
        public static string GenerateUuid()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public async static Task<BsonDocument> GetProfileModel(string name)
        {
            string json = await File.ReadAllTextAsync(profilesPath + name + ".json");

            return BsonSerializer.Deserialize<BsonDocument>(json);
        }
    }
}