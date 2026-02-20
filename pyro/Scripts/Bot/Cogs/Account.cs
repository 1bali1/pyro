using Discord.Interactions;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Bot.Cogs
{
    [Group("account", "Commands related to your account")]
    public class Account : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Database _database;

        public Account(Database database)
        {
            _database = database;
        }

        [SlashCommand("create", "Create a Pyro account!")]
        public async Task AccountCreate()
        {
            await RespondWithModalAsync<CreateAccountModal>("accountCreateModal");
        }

        [ModalInteraction("accountCreateModal", ignoreGroupNames: true)]
        public async Task HandleAccountModal(CreateAccountModal modal)
        {
            await DeferAsync();

            string response = await _database.CreateUser(modal.username, modal.email, modal.password, Context.User.Id);
            var embed = Utils.Utils.CreateEmbed("Registration", response, "Account", Context.User);

            await FollowupAsync(embed: embed.Build());
        }

        [SlashCommand("ban", "Ban a Pyro account!")]
        public async Task AccountBan(Discord.WebSocket.SocketUser user)
        {
            await DeferAsync();

            var embed = Utils.Utils.CreateEmbed("User ban", "You have successfully banned the user!", "Account", Context.User);

            if (!Utils.Utils.owners.Contains(Context.User.Id))
            {
                embed.Description = "You don't have enough permissions to use this command!";
                await FollowupAsync(embed: embed.Build());
                return;
            }

            var account = await _database.users.Find(p => p.discordUserId == user.Id).FirstOrDefaultAsync();

            if (account == null)
            {
                embed.Description = $"{user.GlobalName} doesn't have a registered account!";
                await FollowupAsync(embed: embed.Build());
                return;
            }

            await _database.users.UpdateOneAsync(p => p.discordUserId == user.Id, Builders<User>.Update.Set(u => u.isBanned, true));

            await FollowupAsync(embed: embed.Build());
        }

        [SlashCommand("unban", "Unban a Pyro account!")]
        public async Task AccountUnban(Discord.WebSocket.SocketUser user)
        {
            await DeferAsync();

            var embed = Utils.Utils.CreateEmbed("Unban user", "You have successfully unbanned the account!", "Account", Context.User);

            if (!Utils.Utils.owners.Contains(Context.User.Id))
            {
                embed.Description = "You don't have enough permissions to use this command!";
                await FollowupAsync(embed: embed.Build());
                return;
            }

            var account = await _database.users.Find(p => p.discordUserId == user.Id).FirstOrDefaultAsync();

            if (account == null)
            {
                embed.Description = $"{user.GlobalName} doesn't have a registered account!";
                await FollowupAsync(embed: embed.Build());
                return;
            }

            await _database.users.UpdateOneAsync(p => p.discordUserId == user.Id, Builders<User>.Update.Set(u => u.isBanned, false));

            await FollowupAsync(embed: embed.Build());
        }

        [SlashCommand("full-locker", "Give someone a full locker!")]
        public async Task GiveFullLocker(Discord.WebSocket.SocketUser user)
        {
            await DeferAsync();

            var embed = Utils.Utils.CreateEmbed("Give full locker", $"You have successfully given {user.Mention} a full locker!", "Account", Context.User);

            if (!Utils.Utils.owners.Contains(Context.User.Id))
            {
                embed.Description = "You don't have enough permissions to use this command!";
                await FollowupAsync(embed: embed.Build());
                return;
            }

            var account = await _database.users.Find(p => p.discordUserId == user.Id).FirstOrDefaultAsync();

            if (account == null)
            {
                embed.Description = $"{user.GlobalName} doesn't have a registered account!";
                await FollowupAsync(embed: embed.Build());
                return;
            }

            var profile = account.profiles["athena"];
            var loadout = profile["items"]["default-loadout"];

            string json = await File.ReadAllTextAsync("Data/athenaitems.json");
            BsonDocument content = BsonDocument.Parse(json);

            profile["items"] = content;
            profile["items"]["default-loadout"] = loadout;

            await _database.users.UpdateOneAsync(p => p.discordUserId == user.Id, Builders<User>.Update.Set("profiles.athena.items", profile["items"]));

            await FollowupAsync(embed: embed.Build());

        }

        [SlashCommand("delete", "Delete your Pyro account!")]
        public async Task DeleteAccount()
        {
            var embed = Utils.Utils.CreateEmbed("Delete account", "You have successfully deleted your account!", "Account", Context.User);

            var account = await _database.users.Find(p => p.discordUserId == Context.User.Id).FirstOrDefaultAsync();

            if (account == null || account.isBanned)
            {
                embed.Description = "You don't have a registered Pyro account, or you are banned!";
                await RespondAsync(embed: embed.Build());
                return;
            }
            
            await RespondWithModalAsync<DeleteAccountModal>("deleteAccountModal");
        }

        [ModalInteraction("deleteAccountModal", ignoreGroupNames: true)]
        public async Task HandleAccountDelete(DeleteAccountModal modal)
        {
            await DeferAsync();

            var embed = Utils.Utils.CreateEmbed("Delete account", "You have successfully deleted your account!", "Account", Context.User);

            if (modal.confirmation != "CONFIRM")
            {
                embed.Description = "Account deletion cancelled!";
                await FollowupAsync(embed: embed.Build());
                return;
            }

            var account = await _database.users.Find(p => p.discordUserId == Context.User.Id).FirstOrDefaultAsync();

            if (account != null && !account.isBanned) await _database.users.DeleteOneAsync(p => p.discordUserId == Context.User.Id);
            else embed.Description = "There was an error while trying to delete your account!";

            await FollowupAsync(embed: embed.Build());
            return;
        }

        [SlashCommand("add-vbucks", description: "Add vbucks to a user's balance!")]
        public async Task AccountAddVbucks(Discord.WebSocket.SocketUser user, [MaxValue(99999)] [MinValue(1)] int amount)
        {
            await DeferAsync();

            var embed = Utils.Utils.CreateEmbed("Increase vbucks", $"You have successfully increased {user.Mention}'s vbuck balance by {amount}!", "Account", Context.User);

            if (!Utils.Utils.owners.Contains(Context.User.Id))
            {
                embed.Description = "You don't have enough permissions to use this command!";
                await FollowupAsync(embed: embed.Build());
                return;
            }

            var account = await _database.users.Find(p => p.discordUserId == user.Id).FirstOrDefaultAsync();

            if (account == null)
            {
                embed.Description = $"{user.GlobalName} doesn't have a registered account!";
                await FollowupAsync(embed: embed.Build());
                return;
            }

            await _database.users.UpdateOneAsync(p => p.discordUserId == user.Id, Builders<User>.Update.Inc("vbucks", amount));
            
            await FollowupAsync(embed: embed.Build()); 
        }

        [SlashCommand("remove-vbucks", description: "Remove vbucks from a user's balance!")]
        public async Task AccountRemoveVbucks(Discord.WebSocket.SocketUser user, [MaxValue(99999)] [MinValue(1)] int amount)
        {
            await DeferAsync();

            var embed = Utils.Utils.CreateEmbed("Decrease vbucks", $"You have successfully decreased {user.Mention}'s vbuck balance by {amount}!", "Account", Context.User);

            if (!Utils.Utils.owners.Contains(Context.User.Id))
            {
                embed.Description = "You don't have enough permissions to use this command!";
                await FollowupAsync(embed: embed.Build());
                return;
            }

            var account = await _database.users.Find(p => p.discordUserId == user.Id).FirstOrDefaultAsync();

            if (account == null)
            {
                embed.Description = $"{user.GlobalName} doesn't have a registered account!";
                await FollowupAsync(embed: embed.Build());
                return;
            }

            await _database.users.UpdateOneAsync(p => p.discordUserId == user.Id, Builders<User>.Update.Inc("vbucks", -amount));

            await FollowupAsync(embed: embed.Build()); 
        }
    }

    public class DeleteAccountModal : IModal
    {
        public string Title { get; } = "Confirmation";

        [InputLabel("Are you sure?")]
        [ModalTextInput("confirmationInput", Discord.TextInputStyle.Short, "Enter the text 'CONFIRM' to delete your account!")]
        required public string confirmation { get; set; }

    }
    public class CreateAccountModal : IModal
    {
        public string Title { get; } = "Create an account";

        [InputLabel("Email address")]
        [ModalTextInput("email", Discord.TextInputStyle.Short, "Enter your email address!", minLength: 5, maxLength: 50)]
        required public string email { get; set; }

        [InputLabel("Player name")]
        [ModalTextInput("username", Discord.TextInputStyle.Short, "Enter the name you want!", minLength: 4, maxLength: 24)]
        required public string username { get; set; }

        [InputLabel("Password")]
        [ModalTextInput("password", Discord.TextInputStyle.Short, "Enter a strong password!", minLength: 6, maxLength: 35)]
        required public string password { get; set; }
    }
}