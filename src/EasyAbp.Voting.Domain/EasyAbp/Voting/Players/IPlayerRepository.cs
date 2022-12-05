using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.Voting.Players;

public interface IPlayerRepository : IRepository<Player, Guid>
{
    Task UpdateVotesAsync(Guid id, long votes, CancellationToken cancellationToken = default);
}
