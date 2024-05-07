using System.Globalization;

namespace DiscordBot.Storage.Database
{
    public interface IGuildDataProvider
    {
        public CultureInfo GetGuildCulture(ulong guildID);
        public int GetGuildTimezone(ulong guildID);
        public void SetGuildTimezone(ulong guildID,int utcOffset);
        public void SetGuildCulture(ulong guildID, string cultureCode);
    }
}