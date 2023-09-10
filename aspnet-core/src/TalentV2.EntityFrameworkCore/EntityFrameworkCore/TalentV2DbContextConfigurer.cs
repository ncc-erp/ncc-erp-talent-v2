using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace TalentV2.EntityFrameworkCore
{
    public static class TalentV2DbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<TalentV2DbContext> builder, string connectionString)
        {
            builder.UseNpgsql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<TalentV2DbContext> builder, DbConnection connection)
        {
            builder.UseNpgsql(connection);
        }
    }
}
