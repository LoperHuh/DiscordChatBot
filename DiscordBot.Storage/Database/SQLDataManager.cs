using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Storage.Database
{
    public class SQLDataManager : DbContext
    {
        public DbSet<GuildData> Guilds { get; set; } = null!;

        public SQLDataManager(DbContextOptions<SQLDataManager> options) : base(options)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                throw new Exception($"There is an error trying to connect to sql database", ex);
            }
        }
    }
}