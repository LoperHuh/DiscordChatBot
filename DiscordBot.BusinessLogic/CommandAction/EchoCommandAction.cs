using Discord.WebSocket;

namespace DiscordBot.BusinessLogic.CommandAction;

public class EchoCommandAction : ICommandAction
{
    private Localizator _localizator;

    public EchoCommandAction(Localizator localizator)
    {
        _localizator = localizator;
    }

    public List<ActionData> GetAvailableCommands()
    {
        return new List<ActionData>
        {
            new("echo", _localizator.Localize("EchoDefaultResponse"), Respond),
        };
    }

    public async Task<MessageHandleResult> Respond(string input)
    {
        string respond;
        if (string.IsNullOrWhiteSpace(input))
        {
            respond = _localizator.Localize("EchoDefaultResponse");
        }
        else
        {
            respond = input;
        }

        return new MessageHandleResult(respond);
    }
}