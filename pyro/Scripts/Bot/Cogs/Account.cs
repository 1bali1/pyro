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

        public Account(Database database){
            _database = database;
        }

        [SlashCommand("create", "Create a Pyro account!")]
        public async Task AccountCreate()
        {
            await Context.Interaction.RespondWithModalAsync<CreateAccountModal>("accountCreateModal");
        }

        [ModalInteraction("accountCreateModal", ignoreGroupNames: true)]
        public async Task HandleAccountModal(CreateAccountModal modal)
        {
            await Context.Interaction.DeferAsync();

            string response = await _database.CreateUser(modal.username, modal.email, modal.password, Context.User.Id);
            var embed = Utils.Utils.CreateEmbed("Registration", response, "Account", Context.User);

            await Context.Interaction.FollowupAsync(embed: embed.Build());
        }

        [SlashCommand("ban", "Ban a Pyro account!")]
        public async Task AccountBan(Discord.WebSocket.SocketUser user)
        {
            await Context.Interaction.DeferAsync();

            var embed = Utils.Utils.CreateEmbed("User ban", "You have successfully banned the user!", "Account", Context.User);

            if (!Utils.Utils.owners.Contains(Context.User.Id))
            {
                embed.Description = "You don't have enough permissions to use this command!";
                await Context.Interaction.FollowupAsync(embed: embed.Build());
                return;
            }

            var account = await _database.users.Find(p => p.discordUserId == user.Id).FirstOrDefaultAsync();

            if(account == null)
            {
                embed.Description = $"{user.GlobalName} doesn't have a registered account!";
                await Context.Interaction.FollowupAsync(embed: embed.Build());
                return;
            }

            await _database.users.UpdateOneAsync(p => p.discordUserId == user.Id, Builders<User>.Update.Set(u => u.isBanned, true));

            await Context.Interaction.FollowupAsync(embed: embed.Build());
        }

        [SlashCommand("unban", "Unban a Pyro account!")]
        public async Task AccountUnban(Discord.WebSocket.SocketUser user)
        {
            await Context.Interaction.DeferAsync();

            var embed = Utils.Utils.CreateEmbed("Unban user", "You have successfully unbanned the account!", "Account", Context.User);

            if (!Utils.Utils.owners.Contains(Context.User.Id))
            {
                embed.Description = "You don't have enough permissions to use this command!";
                await Context.Interaction.FollowupAsync(embed: embed.Build());
                return;
            }

            var account = await _database.users.Find(p => p.discordUserId == user.Id).FirstOrDefaultAsync();

            if(account == null)
            {
                embed.Description = $"{user.GlobalName} doesn't have a registered account!";
                await Context.Interaction.FollowupAsync(embed: embed.Build());
                return;
            }

            await _database.users.UpdateOneAsync(p => p.discordUserId == user.Id, Builders<User>.Update.Set(u => u.isBanned, false));

            await Context.Interaction.FollowupAsync(embed: embed.Build());
        }

        [SlashCommand("full-locker", "Give someone a full locker!")]
        public async Task GiveFullLocker(Discord.WebSocket.SocketUser user)
        {
            await Context.Interaction.DeferAsync();

            var embed = Utils.Utils.CreateEmbed("Give full locker", $"You have successfully given {user.Mention} a full locker!", "Account", Context.User);

            if (!Utils.Utils.owners.Contains(Context.User.Id))
            {
                embed.Description = "You don't have enough permissions to use this command!";
                await Context.Interaction.FollowupAsync(embed: embed.Build());
                return;
            }

            var account = await _database.users.Find(p => p.discordUserId == user.Id).FirstOrDefaultAsync();

            if(account == null)
            {
                embed.Description = $"{user.GlobalName} doesn't have a registered account!";
                await Context.Interaction.FollowupAsync(embed: embed.Build());
                return;
            }
            
            var profile = account.profiles["athena"];
            var loadout = profile["items"]["default-loadout"];

            string json = await File.ReadAllTextAsync("Data/athenaitems.json"); 
            BsonDocument content = BsonDocument.Parse(json);

            profile["items"] = content;
            profile["items"]["default-loadout"] = loadout;

            await _database.users.UpdateOneAsync(p => p.discordUserId == user.Id, Builders<User>.Update.Set("profiles.athena.items", profile["items"]));

            await Context.Interaction.FollowupAsync(embed: embed.Build());

        }
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