using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;
namespace pyro.Scripts.Utils
{
    class Utils
    {
        const string profilesPath = "Data/Models/";
        const string configPath = "Config/Config.json";
        const string jsonDataPath = "Data/";
        public static string GenerateUuid() => Guid.NewGuid().ToString().Replace("-", "");

        public async static Task<BsonDocument> GetProfileModel(string name)
        {
            string json = await File.ReadAllTextAsync(profilesPath + name + ".json");

            return BsonSerializer.Deserialize<BsonDocument>(json);
        }

        public async static Task<JObject> GetConfig()
        {
            string json = await File.ReadAllTextAsync(configPath);

            JObject config = JObject.Parse(json);
            return config;
        }

        public async static Task<JObject> GetJsonData(string name)
        {
            string json = await File.ReadAllTextAsync(jsonDataPath + name + ".json");

            JObject content = JObject.Parse(json);
            return content;
        }
        public async static Task UpdateJsonData(string name, JObject data)
        {
            await File.WriteAllTextAsync(jsonDataPath + name + ".json", data.ToString());
        }

        // TODO: talán tokenezni
        public async Task LoadNews()
        {
            JObject config = await GetConfig();
            JObject jsonData = await GetJsonData("contentpages");

            var news = config.GetValue("news");

            jsonData["savetheworldnews"]["news"]["messages"] = news;
            jsonData["creativenews"]["news"]["messages"] = news;
            jsonData["battleroyalenews"]["news"]["messages"] = news;

            await UpdateJsonData("contentpages", jsonData);
        }

        public async Task LoadEmergencyNotice()
        {
            JObject config = await GetConfig(); 
            JObject jsonData = await GetJsonData("contentpages");

            var emergencynotice = config.GetValue("emergencynotice")!;

            jsonData["emergencynotice"]["news"]["messages"][0]["title"] = emergencynotice["title"];
            jsonData["emergencynotice"]["news"]["messages"][0]["body"] = emergencynotice["title"];

            jsonData["emergencynoticev2"]["emergencynotices"]["emergencynotices"][0]["title"] = emergencynotice["title"];
            jsonData["emergencynoticev2"]["emergencynotices"]["emergencynotices"][0]["body"] = emergencynotice["title"];

            await UpdateJsonData("contentpages", jsonData);
        }
    }
}
