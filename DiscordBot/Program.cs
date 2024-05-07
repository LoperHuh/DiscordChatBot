using Discord;
using Discord.WebSocket;
using DiscordBot.Bootstrap;
using DiscordBot.BusinessLogic.CommandAction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot
{
    class Program
    {
        private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private GuildMessageReceiver guildMessageReceiver;
        private DiscordSocketClient client;
        private ILogger _logger;

        static void Main(string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        private static IConfiguration GetConfiguration() => new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        private async Task MainAsync()
        {
            var configurationRoot = GetConfiguration();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(configurationRoot)
                .AddService(configurationRoot)
                .BuildServiceProvider();
            _logger = serviceProvider.GetService<ILogger<Program>>()!;
            client = serviceProvider.GetService<DiscordSocketClient>()!;
            client.Log += arg => LogAsync(arg, _logger);
            guildMessageReceiver = serviceProvider.GetService<GuildMessageReceiver>()!;
            await client.LoginAsync(TokenType.Bot, configurationRoot.GetDiscordToken());
            await client.StartAsync();
            _shutdownEvent.WaitOne();
        }

        private Task ClientOnMessageReceived(SocketMessage arg)
        {
            if (!arg.Author.IsBot)
                arg.Channel.SendMessageAsync(arg.Content);
            return Task.CompletedTask;
        }
        
        private static async Task LogAsync(LogMessage message, ILogger logger)
        {
            var severity = message.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Debug => LogLevel.Debug,
                _ => LogLevel.Information
            };
            logger.Log(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
            await Task.CompletedTask;
        }
    }
}