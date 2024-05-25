using Discord;
using Discord.WebSocket;
using DiscordBot.BusinessLogic;
using DiscordBot.BusinessLogic.CommandAction;
using DiscordBot.Storage.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Bootstrap;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddService
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddLogging(configure => configure.AddConsole())
            .AddTransient<IGuildDataProvider, DataManager>()
            .AddTransient<GuildEnvironmentFactory>()
            .AddSingleton<DiscordSocketConfig>(_ => new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            })
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<GuildMessageReceiver>()
            .AddSingleton<OpenAI_API.OpenAIAPI>(_ =>
            {
                return new OpenAI_API.OpenAIAPI(configuration.GetGptToken());
            })
            .AddDbContext<SQLDataManager>(options =>
            {
                options.UseNpgsql(configuration.GetDbConnectionString());
            });
    }
}