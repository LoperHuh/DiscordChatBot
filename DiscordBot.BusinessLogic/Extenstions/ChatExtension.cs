using System.Globalization;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.BusinessLogic.Extensions
{
    public static class ChatExtension
    {
        public static async Task<IUserMessage> SendLocalizedMessage(this SocketMessage input, string localizationKey,
            CultureInfo culture, object[]? args = null)
        {
            return await SendLocalizedMessage(input.Channel, localizationKey, culture, args);
        }

        public static async Task<IUserMessage> SendLocalizedMessage(this IMessageChannel Channel,
            string localizationKey, CultureInfo culture,
            object[]? args = null)
        {
            string localizedText = LocalizatorLegacy.Localize(localizationKey, culture);
            if (args != null)
            {
                localizedText = String.Format(localizedText, args);
            }

            return await Channel.SendMessageAsync(localizedText);
        }
    }
}