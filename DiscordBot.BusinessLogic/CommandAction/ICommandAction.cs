namespace DiscordBot.BusinessLogic.CommandAction
{
    public interface ICommandAction
    {
        public List<ActionData> GetAvailableCommands();
    }
}