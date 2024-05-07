using System.Globalization;
using DiscordBot.Storage.Database;

namespace DiscordBot.BusinessLogic.CommandAction
{
    public class GuildSettingsAction : ICommandAction
    {
        private IGuildDataProvider _guildDataProvider;
        private Localizator _localizator;
        private ulong _guild;

        public GuildSettingsAction(IGuildDataProvider guildDataProvider, Localizator localizator, ulong guild)
        {
            _localizator = localizator;
            _guildDataProvider = guildDataProvider;
            _guild = guild;
        }

        public List<(string command, string description)> GetDescription()
        {
            return new List<(string command, string description)>
            {
                ("SetTZ", "SettingsGetTimeZone"),
                ("GetTZ", "SettingsGetTimeZone"),
                ("SetLocale", "SettingsSetCurrentLocale"),
                ("GetLocale", "SettingsGetCurrentLocale")
            };
        }

        public List<ActionData> GetAvailableCommands()
        {
            return new List<ActionData>
            {
                new ("SetTZ", string.Empty, SetTimezone),
                new ("GetTZ", string.Empty, GetTimezone),
                new ("SetLocale", string.Empty, SetLocale),
                new ("GetLocale", string.Empty, GetLocale)
            };
        }

        private async Task<MessageHandleResult> SetTimezone(string input)
        {
            var output = input;
            if (int.TryParse(input, out int offset))
            {
                _guildDataProvider.SetGuildTimezone(_guild, offset);
            }
            else
            {
                output = _localizator.Localize("ParsingError");
            }

            return new MessageHandleResult(output, true, true);
        }

        private async Task<MessageHandleResult> GetTimezone(string input)
        {
            return new MessageHandleResult($"UTC {_guildDataProvider.GetGuildTimezone(_guild)}");
        }

        private async Task<MessageHandleResult> SetLocale(string input)
        {
            var output = string.Empty;
            if (string.IsNullOrEmpty(input))
            {
                try
                {
                    _guildDataProvider.SetGuildCulture(_guild, input);
                    output = _localizator.Localize("SettingsLocaleSettedtCurrentLocale");
                }
                catch (CultureNotFoundException e)
                {
                    output = _localizator.Localize("ParsingError");
                }
            }
            else
            {
                output = _localizator.Localize("ParsingError");
            }

            return new MessageHandleResult(output);
        }

        private async Task<MessageHandleResult> GetLocale(string input)
        {
            return new MessageHandleResult(
                $"Locale is {_guildDataProvider.GetGuildCulture(_guild).TwoLetterISOLanguageName}");
        }
    }
}