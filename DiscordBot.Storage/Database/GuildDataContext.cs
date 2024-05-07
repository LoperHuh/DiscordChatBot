using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.Json.Serialization;

namespace DiscordBot.Storage.Database
{
    public class GuildData
    {
        public GuildData()
        {
        }

        public GuildData(ulong guildID)
        {
            CultureCode = "en";
            UTCOffset = 0;
            GuildID = guildID;
        }


        [JsonIgnore]
        [NotMapped]
        public CultureInfo CultureInfo
        {
            get
            {
                if (cultureInfo == null)
                {
                    cultureInfo = new CultureInfo(CultureCode);
                }

                return cultureInfo;
            }
            set => cultureInfo = value;
        }

        [NotMapped][JsonIgnore] private CultureInfo cultureInfo;

        public int ID { get; set; }
        public ulong GuildID { get; set; }
        public string CultureCode { get; set; }
        public int UTCOffset { get; set; } //Sorry Mumbai
    }
}