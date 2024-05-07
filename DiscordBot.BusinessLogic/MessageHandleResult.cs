namespace DiscordBot.BusinessLogic;

public struct MessageHandleResult
{
    public string Message { get; }
    public bool Success { get; }
    public bool RestartRequired { get; }

    public MessageHandleResult() : this(string.Empty, false)
    {
    }

    public MessageHandleResult(string message, bool success = true, bool restartRequired = false)
    {
        Message = message;
        Success = success;
        RestartRequired = restartRequired;
    }
}