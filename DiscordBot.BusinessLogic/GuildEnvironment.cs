using Discord.WebSocket;
using DiscordBot.BusinessLogic.CommandAction;
using DiscordBot.BusinessLogic.CommandAction.ChatGPT;
using DiscordBot.BusinessLogic.Extensions;
using DiscordBot.Storage.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenAI_API;

namespace DiscordBot.BusinessLogic;

public class GuildEnvironment
{
    private Dictionary<string, Func<string, Task<MessageHandleResult>>> actionDictionary = new();
    private ulong _guildId;
    private ILogger _logger;
    private Localizator _localizator;
    private ServiceProvider _serviceProvider;

    public GuildEnvironment(IGuildDataProvider guildDataProvider, OpenAIAPI chatGptApi,
        ILogger<GuildEnvironment> logger, ulong guildId)
    {
        BuildDI(guildDataProvider, chatGptApi, guildId);
        BuildActionDictionary(_serviceProvider.GetServices<ICommandAction>().ToList());
    }

    private void BuildDI(IGuildDataProvider guildDataProvider, OpenAIAPI chatGptApi, ulong guildId)
    {
        var localizator = new Localizator(guildDataProvider.GetGuildCulture(guildId));
        var services = new ServiceCollection()
            .AddSingleton(guildDataProvider)
            .AddSingleton(chatGptApi)
            .AddSingleton<EchoCommandAction>()
            .AddSingleton<Localizator>(localizator)
            .AddTransient<ICommandAction, ChatGPTAction>()
            .AddTransient<ICommandAction, GuildSettingsAction>(_ =>
                new GuildSettingsAction(guildDataProvider, localizator, guildId));
        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task<MessageHandleResult> HandleMessageAsync(SocketMessage socketMessage)
    {
        var messageContent = socketMessage.Content;
        var commandContent = StringParserHelper.ParseMessage(messageContent);
        if (!actionDictionary.ContainsKey(commandContent.Command))
        {
            return new MessageHandleResult();
        }
        //TODO: обернуть в скоуп и доп зависимостью прокинуть guid канала
        var messageAction = actionDictionary[commandContent.Command];
        var respondTask = messageAction(commandContent.Content);
        using (socketMessage.Channel.EnterTypingState())
        {
            var output = await respondTask;
            await PrintTextAsync(socketMessage, output.Message); 
        }

        return respondTask.Result;
    }

    private async Task PrintTextAsync(SocketMessage socketMessage, string text)
    {
        List<string> chunks = StringParserHelper.Split(text, 2000).ToList();
        for (int i = 0; i < chunks.Count; i++)
        {
            await socketMessage.Channel.SendMessageAsync(chunks[i]);
        }
    }

    private void BuildActionDictionary(List<ICommandAction> commandActions)
    {
        foreach (var commandAction in commandActions)
        {
            foreach (var commandData in commandAction.GetAvailableCommands())
            {
                if (actionDictionary.ContainsKey(commandData.CommandString))
                {
                    _logger.LogWarning(
                        $"Trying to add command which is already exist. Command: {commandData.CommandString}");
                    continue;
                }

                actionDictionary.Add(commandData.CommandString.ToLower(), commandData.Command);
            }

            //foreach (var commandDescription in commandAction.GetDescription())
            //{
            //    if (actionDescriptionDictionary.ContainsKey(commandDescription.command))
            //    {
            //        BotLogger.LogWarning(
            //            $"Trying to add command which is already exist. Command: {commandDescription.command}");
            //        continue;
            //    }
            //    actionDescriptionDictionary.Add(commandDescription.command, commandDescription.description);
            //}
        }
    }
}