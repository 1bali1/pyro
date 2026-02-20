using Discord.Interactions;
using Discord.WebSocket;
using dotenv.net;
using Newtonsoft.Json;
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

var itemshop = await Utils.GetConfig("itemshop");
builder.Services.AddSingleton<MItemshop>(itemshop.ToObject<MItemshop>()!);

var contentPages = await Utils.GetJsonData<object>("contentpages"); // ! ez még lehet probléma lesz
contentPages = JsonConvert.SerializeObject(contentPages);
builder.Services.AddSingleton(contentPages);

var utils = new Utils();

await utils.LoadConfig();

var app = builder.Build();

app.MapControllers();

if (DotEnv.Read()["MUTE_ACCESS_LOG"] != "true") app.UseMiddleware<Logger>();

await app.RunAsync();