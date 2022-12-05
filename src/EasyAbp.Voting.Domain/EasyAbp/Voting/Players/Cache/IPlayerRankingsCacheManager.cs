using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EasyAbp.Voting.Players.Cache;

public interface IPlayerRankingsCacheManager
{
    Task AddPlayerAsync(Player player);

    Task VoteAsync(Guid activityId, Guid playerId, long votes);

    Task<long> GetTotalVotesAsync(Guid activityId);

    Task<PlayerSortedSetEntry> GetAsync(Guid activityId, Guid playerId);

    Task<List<PlayerSortedSetEntry>> GetManyAsync(Guid activityId, IEnumerable<Guid> playerIds);

    Task<long> GetCountAsync(Guid activityId, Guid? groupId);

    Task<List<PlayerSortedSetEntry>> GetListAsync(Guid activityId, Guid? groupId, int skipCount, int maxResultCount);

    Task RemovePlayerAsync(Player player);

    Task SaveAsync(CancellationToken cancellationToken = default);
}
