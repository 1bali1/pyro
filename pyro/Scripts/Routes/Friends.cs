using Microsoft.AspNetCore.Mvc;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Routes
{
    [ApiController]
    public class Friends : ControllerBase
    {
        private readonly Database _database;
        public Friends(Database database)
        {
            _database = database;
        }

        // ? nem biztos, hogy kér le paramból
        [HttpGet("friends/api/v1/{accountId}/settings"), RequiresAuthorization]
        public async Task<IActionResult> FriendSettings(string accountId)
        {
            var user = HttpContext.Items["user"] as User;

            return Ok(new
            {
                acceptInvites = user!.friendSystem.acceptInvites ? "public" : "private",
                mutualPrivacy = user.friendSystem.mutualPrivacy
            });
        }

        // ? nem biztos, hogy kér le paramból
        [HttpGet("friends/api/v1/{accountId}/summary"), RequiresAuthorization]
        public async Task<IActionResult> FriendSummary(string accountId)
        {
            var user = HttpContext.Items["user"] as User;
            var friendSystem = user!.friendSystem;

            var outgoing = new List<object>();
            var incoming = new List<object>();
            var friends = new List<object>();
            var blockedUsers = new List<object>();

            foreach (var friend in friendSystem.friends)
            {
                var friendObj = new Dictionary<string, object>
                {
                    { "accountId", friend.accountId },
                    { "mutual", 0 },
                    { "favorite", friend.favorite },
                    { "created", friend.created }
                };

                if (friend.blocked) { blockedUsers.Add(friendObj); }
                else if (friend.status == "ACCEPTED")
                {
                    friendObj["note"] = friend.note;
                    friendObj["alias"] = friend.nickname;
                    friends.Add(friendObj);
                }
                else if (friend.direction == "INBOUND") { incoming.Add(friendObj); }
                else if (friend.direction == "OUTBOUND") { outgoing.Add(friendObj); }
            }

            return Ok(new
            {
                friends = friends,
                incoming = incoming,
                outgoing = outgoing,
                blocklist = blockedUsers,
                suggested = Array.Empty<string>(),
                settings = new
                {
                    acceptInvites = user!.friendSystem.acceptInvites ? "public" : "private"
                }
            });
        }
    }
}