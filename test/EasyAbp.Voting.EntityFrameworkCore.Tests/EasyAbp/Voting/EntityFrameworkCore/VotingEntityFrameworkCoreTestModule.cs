using EasyAbp.Voting.EasyAbp.Voting;
using EasyAbp.Voting.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.Modularity;

namespace EasyAbp.Voting.EasyAbp.Voting.EntityFrameworkCore;

[DependsOn(
    typeof(VotingTestBaseModule),
    typeof(VotingEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
    )]
public class VotingEntityFrameworkCoreTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var sqliteConnection = CreateDatabaseAndGetConnection();

        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(abpDbContextConfigurationContext =>
            {
                abpDbContextConfigurationContext.DbContextOptions.UseSqlite(sqliteConnection);
            });
        });
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        new VotingDbContext(
            new DbContextOptionsBuilder<VotingDbContext>().UseSqlite(connection).Options
        ).GetService<IRelationalDatabaseCreator>().CreateTables();

        return connection;
    }
}
