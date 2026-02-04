using Discord.Interactions;
using Discord.WebSocket;
using dotenv.net;
using Newtonsoft.Json.Linq;
using pyro.Scripts.Utils;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<Database>();
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
builder.Services.AddHostedService<Bot>();

var keychain = await Utils.GetJsonData<List<string>>("keychain");
builder.Services.AddSingleton(keychain);

var utils = new Utils();

await utils.LoadNews();
await utils.LoadEmergencyNotice();

var app = builder.Build();

app.MapControllers();
app.UseMiddleware<Logger>();

await app.RunAsync();