using DiscordBot.Storage.Database;
using Microsoft.Extensions.Logging;

namespace DiscordBot.BusinessLogic;

public class GuildEnvironmentFactory
{
    private IGuildDataProvider _guildDataProvider;
    private ILogger<GuildEnvironment> _logger;
    private OpenAI_API.OpenAIAPI _chatGpt;

    public GuildEnvironmentFactory(IGuildDataProvider guildDataProvider, OpenAI_API.OpenAIAPI chatGpt,
        ILogger<GuildEnvironment> logger)
    {
        _guildDataProvider = guildDataProvider;
        _logger = logger;
        _chatGpt = chatGpt;
    }
    
    public GuildEnvironment Create(ulong guildId)
    {
        return new GuildEnvironment(_guildDataProvider, _chatGpt, _logger, guildId);
    }
}