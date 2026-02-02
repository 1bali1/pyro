using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Linq;

namespace pyro.Scripts.Utils
{
    public class Utils
    {
        // colors
        public const string redColor = "\u001b[31m";
        public const string greenColor = "\u001b[32m";
        public const string resetColor = "\u001b[0m";
        public const string purpleColor = "\u001b[35m";


        // path
        const string profilesPath = "Data/Models/";
        const string configPath = "Config/Config.json";
        const string jsonDataPath = "Data/";

        public static string GenerateUuid() => Guid.NewGuid().ToString().Replace("-", "");
        public static string GetIsoDatetime(DateTime datetime) => datetime.ToString("yyyy-MM-ddTHH:mm:ssZ");

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
            jsonData["emergencynotice"]["news"]["messages"][0]["body"] = emergencynotice["description"];

            jsonData["emergencynoticev2"]["emergencynotices"]["emergencynotices"][0]["title"] = emergencynotice["title"];
            jsonData["emergencynoticev2"]["emergencynotices"]["emergencynotices"][0]["body"] = emergencynotice["description"];

            await UpdateJsonData("contentpages", jsonData);
        }
    }

    public class BackendError
    {
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public List<string> messageVars { get; set; }
        public int numericErrorCode { get; set; }
        public int statusCode { get; set; }

        public BackendError(string code, string message, List<string> vars, int numericCode, int status)
        {
            errorCode = code;
            errorMessage = message;
            messageVars = vars;
            numericErrorCode = numericCode;
            statusCode = status;
        }

        public async Task<ObjectResult> Create(HttpResponse response)
        {
            var error = new ObjectResult(
                new
                {
                    errorCode = errorCode,
                    errorMessage = errorMessage,
                    numericErrorCode = numericErrorCode,
                    originatingService = "any",
                    intent = "prod"
                }
            )
            {
                StatusCode = statusCode
            };

            response.Headers.Append("X-Epic-Error-Name", errorCode);
            response.Headers.Append("X-Epic-Error-Code", numericErrorCode.ToString());    

            return error;
        }

    }
}
