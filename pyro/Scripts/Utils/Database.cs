using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace pyro.Scripts.Utils
{
    public interface IDatabase
    {
        IMongoCollection<User> users { get; }
    }

    public class Database : IDatabase
    {
        private readonly IMongoDatabase _database;

        public Database(IConfiguration configuration)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("pyro");
        }

        public IMongoCollection<User> users => _database.GetCollection<User>("users");
        public IMongoCollection<Token> tokens => _database.GetCollection<Token>("tokens");

        public async Task<string> CreateUser(string username, string email, string password, long discordUserId)
        {
            email = email.ToLower();

            if (await users.Find(p => p.discordUserId == discordUserId).FirstOrDefaultAsync() != null) return "Már van regisztrált fiókod!";

            if (await users.Find(p => p.username == username).FirstOrDefaultAsync() != null) return "Ez a felhasználónév már foglalt!";

            if (await users.Find(p => p.email == email).FirstOrDefaultAsync() != null) return "Ezzel az e-mail-címmel már regisztrált valaki!";
            
            string emailFilter = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,7}$";

            if (!Regex.IsMatch(email, emailFilter)) return "Helytelen a megadott e-mail formátum!";
        
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            string accountId = Utils.GenerateUuid();

            var athena = await Utils.GetProfileModel("athena");

            var commonCore = await Utils.GetProfileModel("common_core");
            var commonPublic = await Utils.GetProfileModel("common_public");
            var creative = await Utils.GetProfileModel("creative");

            User user = new User
            {
                accountId = accountId,
                discordUserId = discordUserId,
                username = username,
                email = email,
                password = hashedPassword,
                isBanned = false,
                vbucks = 200,
                createdOn = DateTime.Now,
                profiles = new Dictionary<string, BsonDocument>
                {
                    { "athena", athena },
                    { "common_core", commonCore },
                    { "common_public", commonPublic },
                    { "creative", creative }
                },
                friendSystem = new Dictionary<string, object>
                {
                    { "acceptInvites", true },
                    { "mutualPrivacy", "ALL" },
                    { "friends", new Dictionary<string, object>() },
                    { "blockedUsers", new List<string>() }
                }
            };

            foreach (var profileId in user.profiles.Keys)
            {
                var profile = user.profiles[profileId];
                profile["accountId"] = accountId;
                profile["created"] = DateTime.Now;
                profile["updated"] = DateTime.Now;

            }

            await users.InsertOneAsync(user);

            return "Sikeresen regisztráltál!";
        }

        public async Task AddToken(string type, string accountId, string token)
        {
            var userToken = await tokens.Find(p => p.accountId == accountId).FirstOrDefaultAsync();

            if (userToken == null)
            {
                userToken = new Token
                {
                  accountId=accountId,
                  accessToken="",
                  refreshToken=""  
                };
            }

            userToken.GetType().GetProperty(type)?.SetValue(userToken, token);

            await tokens.ReplaceOneAsync(p => p.accountId == accountId, replacement: userToken);
        }
    }
}