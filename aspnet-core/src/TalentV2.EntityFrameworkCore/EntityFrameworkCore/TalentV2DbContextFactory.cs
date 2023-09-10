using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TalentV2.Configuration;
using TalentV2.Web;

namespace TalentV2.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class TalentV2DbContextFactory : IDesignTimeDbContextFactory<TalentV2DbContext>
    {
        public TalentV2DbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TalentV2DbContext>();
            
            /*
             You can provide an environmentName parameter to the AppConfigurations.Get method. 
             In this case, AppConfigurations will try to read appsettings.{environmentName}.json.
             Use Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") method or from string[] args to get environment if necessary.
             https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#args
             */
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            TalentV2DbContextConfigurer.Configure(builder, configuration.GetConnectionString(TalentV2Consts.ConnectionStringName));

            return new TalentV2DbContext(builder.Options);
        }
    }
}
