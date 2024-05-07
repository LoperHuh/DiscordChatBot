using Microsoft.Extensions.Configuration;

namespace DiscordBot.Bootstrap;

public static class ConfigurationExtensions
{
    public static string GetDbConnectionString(this IConfiguration configuration) =>
        Environment.GetEnvironmentVariable("DbDiscordBotConnection") ??
        throw new ArgumentNullException("DbDiscordBotConnection");

    public static string GetGptToken(this IConfiguration configuration) =>
        Environment.GetEnvironmentVariable("GPTToken") ?? throw new ArgumentNullException("GPTToken");

    public static string GetDiscordToken(this IConfiguration configuration) =>
        Environment.GetEnvironmentVariable("DiscordToken") ?? throw new ArgumentNullException("DiscordToken");
}