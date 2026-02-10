using Discord;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
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
        public const string blueColor = "\u001b[94m";

        // path
        const string profilesPath = "Data/Models/";
        const string configPath = "Config/";
        const string jsonDataPath = "Data/";
        public const string cloudstoragePath = "Cloudstorage/";

        public static string seasonName = "s15";
        public static int seasonNumber = int.Parse(seasonName.Replace("s", ""));
        public static List<ulong> owners = new List<ulong>();

        public static string GenerateUuid() => Guid.NewGuid().ToString().Replace("-", "");
        public static string GetIsoDatetime(DateTime datetime) => datetime.ToString("yyyy-MM-ddTHH:mm:ssZ");

        public async static Task<BsonDocument> GetProfileModel(string name)
        {
            string json = await File.ReadAllTextAsync(profilesPath + name + ".json");

            return BsonSerializer.Deserialize<BsonDocument>(json);
        }

        public async static Task<JObject> GetConfig(string name)
        {
            string json = await File.ReadAllTextAsync(configPath + name + ".json");

            JObject config = JObject.Parse(json);
            return config;
        }

        public async static Task<T> GetJsonData<T>(string name)
        {
            string json = await File.ReadAllTextAsync(jsonDataPath + name + ".json");
            
            T content = JsonConvert.DeserializeObject<T>(json)!;

            return content;
        }
        public async static Task UpdateJsonData(string name, JObject data)
        {
            await File.WriteAllTextAsync(jsonDataPath + name + ".json", data.ToString());
        }

        public static EmbedBuilder CreateEmbed(string title, string description, string footer, Discord.WebSocket.SocketUser? user = null, Color? color = null)
        {
            Color embedColor = Color.DarkBlue;
            if(color != null) embedColor = (Color)color;

            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(embedColor)
                .WithFooter(f => f.WithText($"Pyro • {footer}").WithIconUrl("https://github.com/1bali1/pyro/raw/main/pyro/Assets/banner.png"))
                .WithCurrentTimestamp();

            if(user != null)
            {
                embed.WithAuthor(author => author.WithName(user.GlobalName));
                string avatarUrl = user.GetAvatarUrl();
                if(avatarUrl == null) avatarUrl = user.GetDefaultAvatarUrl();
                embed.Author.WithIconUrl(avatarUrl);
                
            }

            return embed;
            
        }

        // TODO: talán tokenezni
        public async Task LoadConfig()
        {
            JObject config = await GetConfig("config");
            JObject jsonData = await GetJsonData<JObject>("contentpages");

            // news
            var news = config.GetValue("news");

            jsonData["savetheworldnews"]!["news"]!["messages"] = news;
            jsonData["creativenews"]!["news"]!["messages"] = news;
            jsonData["battleroyalenews"]!["news"]!["messages"] = news;
            jsonData["battleroyalenewsv2"]!["news"]!["motds"] = news;

            // emergency notice
            var emergencynotice = config.GetValue("emergencynotice")!;

            jsonData["emergencynotice"]!["news"]!["messages"]![0]!["title"] = emergencynotice["title"];
            jsonData["emergencynotice"]!["news"]!["messages"]![0]!["body"] = emergencynotice["description"];

            jsonData["emergencynoticev2"]!["emergencynotices"]!["emergencynotices"]![0]!["title"] = emergencynotice["title"];
            jsonData["emergencynoticev2"]!["emergencynotices"]!["emergencynotices"]![0]!["body"] = emergencynotice["description"];

            owners = config.GetValue("owners")!.Values<ulong>().ToList();

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
