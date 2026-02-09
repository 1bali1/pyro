using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace pyro.Scripts.Utils
{
    public interface IDatabase
    {
        IMongoCollection<User> users { get; }
        IMongoCollection<UserToken> tokens { get; }
        IMongoCollection<ClientToken> clientTokens { get; }
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
        public IMongoCollection<UserToken> tokens => _database.GetCollection<UserToken>("userTokens");
        public IMongoCollection<ClientToken> clientTokens => _database.GetCollection<ClientToken>("clientTokens");

        public async Task<string> CreateUser(string username, string email, string password, ulong discordUserId)
        {
            email = email.ToLower();

            if (await users.Find(p => p.discordUserId == discordUserId).FirstOrDefaultAsync() != null) return "You already have a registered account!";

            if (await users.Find(p => p.username == username).FirstOrDefaultAsync() != null) return "This username is already taken!";

            if (await users.Find(p => p.email == email).FirstOrDefaultAsync() != null) return "Someone has already registered with this email address!";
            
            string emailFilter = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,7}$";

            if (!Regex.IsMatch(email, emailFilter)) return "The email format provided is incorrect.!";
        
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
                friendSystem = new FriendSystem()
            };

            foreach (var profileId in user.profiles.Keys)
            {
                var profile = user.profiles[profileId];
                profile["accountId"] = accountId;
                profile["created"] = Utils.GetIsoDatetime(DateTime.Now);
                profile["updated"] = Utils.GetIsoDatetime(DateTime.Now);

            }

            await users.InsertOneAsync(user);

            return "You have successfully registered your account!";
        }

        public async Task AddToken(string type, string accountId, string token)
        {
            var userToken = await tokens.Find(p => p.accountId == accountId).FirstOrDefaultAsync();

            if (userToken == null)
            {
                userToken = new UserToken
                {
                  accountId=accountId,
                  accessToken="",
                  refreshToken=""  
                };
                await tokens.InsertOneAsync(userToken);
            }

            userToken.GetType().GetProperty(type)?.SetValue(userToken, token);

            await tokens.ReplaceOneAsync(p => p.accountId == accountId, replacement: userToken);
            
        }

        public async Task AddClientToken(string ipAddress, string token)
        {
            var clientToken = await clientTokens.Find(p => p.ipAddress == ipAddress).FirstOrDefaultAsync();

            if (clientToken == null)
            {
                clientToken = new ClientToken
                {
                  ipAddress=ipAddress,
                  clientToken="" 
                };
            }

            clientToken.GetType().GetProperty("clientToken")?.SetValue(clientToken, token);

            await clientTokens.ReplaceOneAsync(p => p.ipAddress == ipAddress, replacement: clientToken);
        }
    }
}