using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EasyAbp.Voting.EntityFrameworkCore;

public class VotingHttpApiHostMigrationsDbContextFactory : IDesignTimeDbContextFactory<VotingHttpApiHostMigrationsDbContext>
{
    public VotingHttpApiHostMigrationsDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<VotingHttpApiHostMigrationsDbContext>()
            .UseNpgsql(configuration.GetConnectionString("Voting"));

        return new VotingHttpApiHostMigrationsDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
