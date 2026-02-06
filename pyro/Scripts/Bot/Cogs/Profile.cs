using System.Net.WebSockets;
using Discord;
using Discord.Interactions;
using MongoDB.Driver;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Bot.Cogs
{
    [Group("profile", "Commands related to profiles")]
    public class Other : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Database _database;
        private static readonly string iconUrl = "https://fortnite-api.com/images/cosmetics/br/{characterId}/icon.png";

        public Other(Database database)
        {
            _database = database;
        }

        [SlashCommand("discord", "View someone's Pyro profile by their Discord user ID!")]
        public async Task ProfileByDiscordId(Discord.WebSocket.SocketUser? user = null)
        {
            await Context.Interaction.DeferAsync();

            if (user == null) user = Context.User;

            var embed = Utils.Utils.CreateEmbed("Profile", $"{user.Mention}'s Pyro account", "Profile", Context.User);

            var account = await _database.users.Find(p => p.discordUserId == user.Id).FirstOrDefaultAsync();

            if (account == null)
            {
                embed.Description = $"{user.Mention} doesn't have a registered account!";
                await Context.Interaction.FollowupAsync(embed: embed.Build());
                return;
            }

            string favCharacter = account.profiles["athena"]["stats"]["attributes"]["favorite_character"].AsString;

            if (string.IsNullOrEmpty(favCharacter)) { favCharacter = iconUrl.Replace("{characterId}", "CID_A_402_Athena_Commando_F_RebirthFresh"); }
            else { favCharacter = iconUrl.Replace("{characterId}", favCharacter); }

            embed.WithThumbnailUrl(favCharacter);
            
            embed.AddField(new EmbedFieldBuilder { Name = "Name", Value = account.username });
            embed.AddField(new EmbedFieldBuilder { Name = "V-Bucks", Value = account.vbucks });
            embed.AddField(new EmbedFieldBuilder { Name = "Friends", Value = 0 }); // TODO: firends model + fix 
            embed.AddField(new EmbedFieldBuilder { Name = "Is banned", Value = account.isBanned ? "Yes" : "No" });
            embed.AddField(new EmbedFieldBuilder { Name = "Creation date", Value = account.createdOn });

            await Context.Interaction.FollowupAsync(embed: embed.Build());
        }

        [SlashCommand("name", "View someone's profile by their player name!")]
        public async Task ProfileByName(string username)
        {
            await Context.Interaction.DeferAsync();

            var embed = Utils.Utils.CreateEmbed("Profile", $"{username}'s Pyro account", "Profile", Context.User);
            var account = await _database.users.Find(p => p.username == username).FirstOrDefaultAsync();

            if (account == null)
            {
                embed.Description = $"There's no account named {username}!";
                await Context.Interaction.FollowupAsync(embed: embed.Build());
                return;
            }

            embed.Description = $"<@{account.discordUserId}>'s Pyro account";

            string favCharacter = account.profiles["athena"]["stats"]["attributes"]["favorite_character"].AsString;

            if (string.IsNullOrEmpty(favCharacter)) { favCharacter = iconUrl.Replace("{characterId}", "CID_A_402_Athena_Commando_F_RebirthFresh"); }
            else { favCharacter = iconUrl.Replace("{characterId}", favCharacter); }

            embed.WithThumbnailUrl(favCharacter);
            
            embed.AddField(new EmbedFieldBuilder { Name = "Name", Value = account.username });
            embed.AddField(new EmbedFieldBuilder { Name = "V-Bucks", Value = account.vbucks });
            embed.AddField(new EmbedFieldBuilder { Name = "Friends", Value = 0 }); // TODO: firends model + fix 
            embed.AddField(new EmbedFieldBuilder { Name = "Is banned", Value = account.isBanned ? "Yes" : "No" });
            embed.AddField(new EmbedFieldBuilder { Name = "Creation date", Value = account.createdOn });

            await Context.Interaction.FollowupAsync(embed: embed.Build());
        }
    }
}