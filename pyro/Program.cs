using dotenv.net;
using pyro.Scripts.Utils;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<Database>();

var utils = new Utils();

await utils.LoadNews();
await utils.LoadEmergencyNotice();

var app = builder.Build();

app.MapControllers();
app.UseMiddleware<Logger>();

await app.RunAsync();