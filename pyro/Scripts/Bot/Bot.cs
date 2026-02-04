using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

public class Bot : BackgroundService
{
    private readonly DiscordSocketClient _bot;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _config;

    public Bot(DiscordSocketClient bot, InteractionService interactions, IServiceProvider services, IConfiguration config)
    {
        _bot = bot;
        _interactions = interactions;
        _services = services;
        _config = config;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _bot.Log += Log;

        _bot.InteractionCreated += async interaction => {
            var ctx = new SocketInteractionContext(_bot, interaction);
            await _interactions.ExecuteCommandAsync(ctx, _services);
        };

        _bot.Ready += async () =>
        {
            await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _interactions.RegisterCommandsGloballyAsync();
        };

        await _bot.LoginAsync(TokenType.Bot, _config["BOT_TOKEN"]);
        await _bot.StartAsync();

        await Task.Delay(-1);
    }

    // TODO: custom logger
    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}