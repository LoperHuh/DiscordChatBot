using Discord.WebSocket;
using DiscordBot.BusinessLogic.Extensions;
using Microsoft.Extensions.Logging;

namespace DiscordBot.BusinessLogic.CommandAction
{
    public class GuildMessageReceiver
    {
        private readonly ILogger<GuildMessageReceiver> _logger;

        private Dictionary<ulong, GuildEnvironment> _guildByGuid = new Dictionary<ulong, GuildEnvironment>();

        private IServiceProvider _serviceProvider;
        private GuildEnvironmentFactory _guildEnvironmentFactory;

        public GuildMessageReceiver(DiscordSocketClient client, IServiceProvider serviceProvider,
            GuildEnvironmentFactory guildEnvironmentFactory)
        {
            _serviceProvider = serviceProvider;
            client.MessageReceived += ClientOnMessageReceived;
            _guildEnvironmentFactory = guildEnvironmentFactory;
        }

        private async Task ClientOnMessageReceived(SocketMessage socketMessage)
        {
            if (socketMessage.Author.IsBot)
                return;
            string message = socketMessage.Content;
            if (string.IsNullOrWhiteSpace(message))
                return;
            if (!StringParserHelper.IsCommand(message))
                return;

            ulong guildID = (socketMessage.Channel as SocketGuildChannel).Guild.Id;
            if (!_guildByGuid.ContainsKey(guildID))
            {
                _guildByGuid.Add(guildID, _guildEnvironmentFactory.Create(guildID));
            }

            var guild = _guildByGuid[guildID];
            var result = await guild.HandleMessageAsync(socketMessage);

            if (result.RestartRequired)
            {
                _guildByGuid.Remove(guildID);
            }
        }
    }
}