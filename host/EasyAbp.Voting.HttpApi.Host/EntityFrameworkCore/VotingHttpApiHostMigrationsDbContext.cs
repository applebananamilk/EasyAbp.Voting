using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Voting.EntityFrameworkCore;

public class VotingHttpApiHostMigrationsDbContext : AbpDbContext<VotingHttpApiHostMigrationsDbContext>
{
    public VotingHttpApiHostMigrationsDbContext(DbContextOptions<VotingHttpApiHostMigrationsDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureVoting();
    }
}
