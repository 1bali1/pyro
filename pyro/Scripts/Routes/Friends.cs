using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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

            foreach (var key in friendSystem.friends.Keys)
            {
                var friend = friendSystem.friends[key];

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

        [HttpPost("friends/api/v1/{sentFromId}/friends/{sentToId}"), RequiresAuthorization]
        public async Task<IActionResult> AddFriend(string sentFromId, string sentToId)
        {
            var sentFrom = HttpContext.Items["user"] as User;
            if(sentFrom!.accountId != sentFromId)
            {
                var error = await new BackendError("errors.com.epicgames.modules.profiles.operation_forbidden", "You can't do that!", [], 12813, 400).Create(Response);
                return error;
            }

            var sentTo = await _database.users.Find(p => p.accountId == sentToId).FirstOrDefaultAsync();

            if(sentTo == null)
            {
                var error = await new BackendError("errors.com.epicgames.modules.profiles.operation_forbidden", "User account not found!", [], 12813, 400).Create(Response);
                return error;
            }

            Friend sentFromFriendObj;
            sentTo.friendSystem.friends.TryGetValue(sentFromId, out sentFromFriendObj!);

            Friend sentToFriendObj;
            sentFrom.friendSystem.friends.TryGetValue(sentToId, out sentToFriendObj!);

            if (sentFromFriendObj != null && (sentFromFriendObj.blocked || !sentTo.friendSystem.acceptInvites))
            {
                var error = await new BackendError("errors.com.epicgames.friends.cannot_friend_due_to_target_settings", $"{sentTo.username} doesn't accept friend invites!", [], 14120, 400).Create(Response);
                return error;
            }

            if(sentFromFriendObj != null && ((sentFromFriendObj.status == "PENDING" && sentFromFriendObj.direction == "INBOUND") || sentFromFriendObj.status == "ACCEPTED"))
            {
                var error = await new BackendError("errors.com.epicgames.friends.friend_request_already_sent", $"You've already sent a request to {sentTo.username}, or you are already friends!", [], 14014, 400).Create(Response);
                return error;
            }

            string status = "PENDING";
            if(sentFromFriendObj == null && sentToFriendObj == null)
            {
                sentFromFriendObj = new Friend
                {
                    accountId = sentFromId,
                    nickname = "",
                    note = "",
                    status = status,
                    direction = "INBOUND",
                    favorite = false,
                    blocked = false,
                    created = Utils.Utils.GetIsoDatetime(DateTime.Now)
                };
                sentToFriendObj = new Friend
                {
                    accountId = sentToId,
                    nickname = "",
                    note = "",
                    status = status,
                    direction = "OUTBOUND",
                    favorite = false,
                    blocked = false,
                    created = Utils.Utils.GetIsoDatetime(DateTime.Now)
                };
            }
            else
            {
                status = "ACCEPTED";
                sentFromFriendObj!.status = status;
                sentToFriendObj!.status = status;
            }

            await _database.users.UpdateOneAsync(p => p.accountId == sentFromId, Builders<User>.Update.Set($"friendSystem.friends.{sentToId}", sentToFriendObj));
            await _database.users.UpdateOneAsync(p => p.accountId == sentToId, Builders<User>.Update.Set($"friendSystem.friends.{sentFromId}", sentFromFriendObj));

            // TODO: xmpp implementálása: presence + friend notification

            return NoContent();
        }
    }
}