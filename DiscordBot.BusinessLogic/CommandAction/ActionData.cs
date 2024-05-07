namespace DiscordBot.BusinessLogic.CommandAction;

public class ActionData
{
    public ActionData(string commandString, string description, Func<string, Task<MessageHandleResult>> command)
    {
        CommandString = commandString;
        Description = description;
        Command = command;
    }

    public string CommandString { get; }
    public string Description { get; }
    public Func<string, Task<MessageHandleResult>> Command { get; }
}