using EasyAbp.Voting.Activities;
using EasyAbp.Voting.Players;
using EasyAbp.Voting.Rules;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Voting.EntityFrameworkCore;

[ConnectionStringName(VotingDbProperties.ConnectionStringName)]
public interface IVotingDbContext : IEfCoreDbContext
{
    DbSet<Activity> Activities { get; }

    DbSet<Player> Players { get; }

    DbSet<PlayerVotes> PlayerVotes { get; }

    DbSet<Rule> Rules { get; }
}
