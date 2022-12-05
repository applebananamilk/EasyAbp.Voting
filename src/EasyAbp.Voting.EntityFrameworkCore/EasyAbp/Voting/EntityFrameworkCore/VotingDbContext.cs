using EasyAbp.Voting.Activities;
using EasyAbp.Voting.Players;
using EasyAbp.Voting.Rules;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Voting.EntityFrameworkCore;

[ConnectionStringName(VotingDbProperties.ConnectionStringName)]
public class VotingDbContext : AbpDbContext<VotingDbContext>, IVotingDbContext
{
    public DbSet<Activity> Activities { get; set; }

    public DbSet<Player> Players { get; set; }

    public DbSet<PlayerVotes> PlayerVotes { get; set; }

    public DbSet<Rule> Rules { get; set; }

    public VotingDbContext(DbContextOptions<VotingDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureVoting();
    }
}
