using EasyAbp.Voting.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Voting.Players;

public class PlayerRepository : EfCoreRepository<IVotingDbContext, Player, Guid>, IPlayerRepository
{
    public PlayerRepository(IDbContextProvider<IVotingDbContext> dbContextProvider) : base(dbContextProvider)
    {

    }


    public override async Task<IQueryable<Player>> WithDetailsAsync()
    {
        return (await base.WithDetailsAsync())
            .Include(p => p.PlayerVotes);
    }

    public async Task UpdateVotesAsync(Guid id, long votes, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var playerVotes = await dbContext.PlayerVotes
            .SingleOrDefaultAsync(p => p.PlayerId == id, cancellationToken: cancellationToken);

        if (playerVotes != null)
        {
            playerVotes.Update(votes);
        }
    }
}
