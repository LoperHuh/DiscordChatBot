using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OpenAI_API.Chat;
using OpenAI_API.Models;

namespace DiscordBot.BusinessLogic.CommandAction.ChatGPT
{
    public class ChatGPTAction : ICommandAction
    {
        private Dictionary<ulong, Conversation> _conversationPerChannel = new();
        private OpenAI_API.OpenAIAPI _bot;
        private Localizator _localizator;

        public ChatGPTAction(OpenAI_API.OpenAIAPI openAIAPI, Localizator localizator)
        {
            _localizator = localizator;
            _bot = openAIAPI;
        }

        public List<(string command, string description)> GetDescription()
        {
            return new List<(string command, string description)>
                { ("ai", "AIChatDescription") };
        }

        public List<ActionData> GetAvailableCommands()
        {
            return new List<ActionData> { new ActionData("ai", "AIChatDescription", AIMessage) };
        }

        private async Task<MessageHandleResult> AIMessage(string input)
        {
            ulong _guildId = 1; // TODO: нужно будет для каждого канала сделать свой экземпляр Conversation;
            string response = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return new MessageHandleResult();
                }

                Conversation conversation = GetConversation(_guildId);
                conversation.AppendUserInput(input);
                response = await conversation.GetResponseFromChatbotAsync();
                response = "Токенов затрачено:" + conversation.MostRecentApiResult.Usage.TotalTokens +
                           Environment.NewLine + response;
            }
            catch (Exception exception)
            {
                string errorMessageText;
                try
                {
                    var pattern = @"^.*(?={)";
                    var filteredMessage = Regex.Replace(exception.Message, pattern, "");
                    var errorBody = JsonConvert.DeserializeObject<Dictionary<string, Object>>(filteredMessage);
                    var parsedValues =
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(errorBody["error"].ToString());
                    errorMessageText = _localizator.Localize(parsedValues["code"]) + Environment.NewLine +
                                       parsedValues["message"];
                }
                catch (Exception ex)
                {
                    errorMessageText =
                        $"Can't parse error due to exception: {ex.Message} {Environment.NewLine} FullMessage from chatGPT: {exception.Message}";
                }

                errorMessageText += Environment.NewLine + "Контекст диалога будет сброшен. Отправь сообщение заново!";
                response = errorMessageText;
                GetNewConversation(_guildId);
            }
            
            return new MessageHandleResult(response);
        }

        private Conversation GetConversation(ulong guid)
        {
            if (_conversationPerChannel.ContainsKey(guid))
            {
                return _conversationPerChannel[guid];
            }

            return GetNewConversation(guid);
        }

        private Conversation GetNewConversation(ulong guid)
        {
            var chatRequest = new ChatRequest();
            chatRequest.Model= new Model("gpt-4o");
            var conversation = _bot.Chat.CreateConversation(chatRequest);
            if (_conversationPerChannel.ContainsKey(guid))
            {
                _conversationPerChannel[guid] = conversation;
            }
            else
            {
                _conversationPerChannel.Add(guid, conversation);
            }

            return conversation;
        }
    }
}