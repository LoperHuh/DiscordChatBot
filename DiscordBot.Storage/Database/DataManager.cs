using System.Globalization;

namespace DiscordBot.Storage.Database
{
    public class DataManager : IGuildDataProvider
    {
        private Dictionary<ulong, GuildData> guildDataDict = new Dictionary<ulong, GuildData>();
        private SQLDataManager _sqlDataManager;
        public DataManager(SQLDataManager sqlDataManager)
        {
            _sqlDataManager = sqlDataManager;
            LoadGuildsData();
        }

        public CultureInfo GetGuildCulture(ulong guildID)
        {
            return EnsureDataCreated(guildID).CultureInfo;
        }

        public void SetGuildTimezone(ulong guildID, int utcOffset)
        {
            GuildData guildData = EnsureDataCreated(guildID);
            guildData.UTCOffset = utcOffset;
            AddOrUpdateEntryToDb(guildData);
        }

        public void SetGuildCulture(ulong guildID, string CultureCode)
        {
            GuildData guildData = EnsureDataCreated(guildID);
            guildData.CultureInfo = new CultureInfo(CultureCode);
            guildData.CultureCode = CultureCode;
            AddOrUpdateEntryToDb(guildData);
        }

        public int GetGuildTimezone(ulong guildID)
        {
            return EnsureDataCreated(guildID).UTCOffset;
        }


        private GuildData EnsureDataCreated(ulong guildID)
        {
            if (guildDataDict.ContainsKey(guildID))
                return guildDataDict[guildID];
            GuildData guildData = new GuildData(guildID);
            AddOrUpdateEntryToDb(guildData);
            guildDataDict.Add(guildID, guildData);
            return guildData;
        }

        private void LoadGuildsData()
        {
            foreach (var guildData in _sqlDataManager.Guilds)
            {
                if (!guildDataDict.ContainsKey(guildData.GuildID))
                {
                    guildDataDict.Add(guildData.GuildID, guildData);
                }
                else
                {
                    guildDataDict[guildData.GuildID] = (guildData);
                }
            }
        }

        private void AddOrUpdateEntryToDb(GuildData guildData)
        {
            var existingGuild = _sqlDataManager.Guilds.FirstOrDefault(gulid => gulid.GuildID == guildData.GuildID);
            if(existingGuild != null)
            {
                existingGuild = guildData;
            }
            else
            {
                _sqlDataManager.Guilds.Add(guildData);
            }
            _sqlDataManager.SaveChanges();
        }
    }
}