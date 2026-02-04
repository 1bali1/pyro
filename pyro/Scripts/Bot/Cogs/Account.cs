using Discord;
using Discord.Interactions;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using pyro.Scripts.Utils;

namespace pyro.Scripts.Bot.Cogs
{
    [Group("account", "Fiókoddal kapcsolatos parancsok")]
    public class Account : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Database _database;

        public Account(Database database){
            _database = database;
        }

        [SlashCommand("create", "Hozz létre egy Pyro fiókot!")]
        public async Task AccountCreate()
        {
            await Context.Interaction.RespondWithModalAsync<CreateAccountModal>("accountCreateModal");
        }

        [ModalInteraction("accountCreateModal", ignoreGroupNames: true)]
        public async Task HandleAccountModal(CreateAccountModal modal)
        {
            await Context.Interaction.DeferAsync();

            string response = await _database.CreateUser(modal.username, modal.email, modal.password, Context.User.Id);
            var embed = Utils.Utils.CreateEmbed("Regisztráció", response, "Fiók", Context.User);

            await Context.Interaction.FollowupAsync(embed: embed);
        }

        [SlashCommand("ban", "Tilts ki egy Pyro fiókot!")]
        public async Task AccountBan(Discord.WebSocket.SocketUser user)
        {
            await Context.Interaction.DeferAsync();

            var embed = Utils.Utils.CreateEmbed("Felhasználó kitiltása", "Sikeresen kitiltottad a felhasználót!", "Fiók", Context.User);

            if (!Utils.Utils.owners.Contains(Context.User.Id))
            {
                embed = Utils.Utils.CreateEmbed("Felhasználó kitiltása", "Nincs jogosultságod a parancs használatához!", "Fiók", Context.User);
                await Context.Interaction.FollowupAsync(embed: embed);
                return;
            }

            var account = await _database.users.Find(p => p.discordUserId == user.Id).FirstOrDefaultAsync();

            if(account == null)
            {
                embed = Utils.Utils.CreateEmbed("Felhasználó kitiltása", $"{user.GlobalName} nem rendelkezik regisztrált fiókkal!", "Fiók", Context.User);
                await Context.Interaction.FollowupAsync(embed: embed);
                return;
            }

            await _database.users.UpdateOneAsync(p => p.discordUserId == user.Id, Builders<User>.Update.Set(u => u.isBanned, true));

            await Context.Interaction.FollowupAsync(embed: embed);
        }

        [SlashCommand("unban", "Oldd fel valakinek a kitiltását!")]
        public async Task AccountUnban(Discord.WebSocket.SocketUser user)
        {
            await Context.Interaction.DeferAsync();

            var embed = Utils.Utils.CreateEmbed("Kitiltás feloldása", "Sikeresen feloldottad a kitiltást!", "Fiók", Context.User);

            if (!Utils.Utils.owners.Contains(Context.User.Id))
            {
                embed = Utils.Utils.CreateEmbed("Kitiltás feloldása", "Nincs jogosultságod a parancs használatához!", "Fiók", Context.User);
                await Context.Interaction.FollowupAsync(embed: embed);
                return;
            }

            var account = await _database.users.Find(p => p.discordUserId == user.Id).FirstOrDefaultAsync();

            if(account == null)
            {
                embed = Utils.Utils.CreateEmbed("Kitiltás feloldása", $"{user.GlobalName} nem rendelkezik regisztrált fiókkal!", "Fiók", Context.User);
                await Context.Interaction.FollowupAsync(embed: embed);
                return;
            }

            await _database.users.UpdateOneAsync(p => p.discordUserId == user.Id, Builders<User>.Update.Set(u => u.isBanned, false));

            await Context.Interaction.FollowupAsync(embed: embed);
        }
    }
    
    public class CreateAccountModal : IModal
    {
        public string Title { get; } = "Fiók létrehozása";

        [InputLabel("E-mail-cím")]
        [ModalTextInput("email", Discord.TextInputStyle.Short, "Írd be az e-mail-címedet!", minLength: 5, maxLength: 50)]
        required public string email { get; set; }

        [InputLabel("Játékos név")]
        [ModalTextInput("username", Discord.TextInputStyle.Short, "Írd be, hogy milyen nevet szeretnél!", minLength: 4, maxLength: 24)]
        required public string username { get; set; }

        [InputLabel("Jelszó")]
        [ModalTextInput("password", Discord.TextInputStyle.Short, "Írj be egy erős jelszavat!", minLength: 6, maxLength: 35)]
        required public string password { get; set; }
    }
}