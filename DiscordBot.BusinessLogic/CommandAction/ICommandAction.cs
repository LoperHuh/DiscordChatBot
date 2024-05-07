using Discord.WebSocket;

namespace DiscordBot.BusinessLogic.CommandAction
{
    public interface ICommandAction
    {
        public List<ActionData> GetAvailableCommands();
    }
}