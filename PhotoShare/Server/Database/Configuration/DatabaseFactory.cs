using Microsoft.EntityFrameworkCore;
using PhotoShare.Server.Database.Configuration.Models;

namespace PhotoShare.Server.Database.Configuration
{
    public static class DatabaseFactory
    {
        public static DbContextOptionsBuilder AddCorrectDatabase(this DbContextOptionsBuilder builder, IConfiguration config)
        {
            var efConfig = config.GetSection("EntityFramework");
            if (!Enum.TryParse<DatabaseType>(efConfig.GetValue<string>("Type"), out var dbType)) throw new ArgumentException($"Database type {efConfig.GetValue<string>("Type")} not found");
            switch (dbType)
            {
                case DatabaseType.SQLite:
                    return AddSqliteDatabase(builder,efConfig);
                case DatabaseType.None:
                default:
                    throw new ArgumentException("No Database type was selected");
            }
        }

        private static DbContextOptionsBuilder AddSqliteDatabase( DbContextOptionsBuilder builder, IConfigurationSection config)
        {
            var configModel = config.GetSection("Config").Get<SQLiteConfiguration>();
            builder.UseSqlite($"Data Source={Environment.CurrentDirectory}/{configModel.DataSource}");
            return builder;
        }
    }
}
