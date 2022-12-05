using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyAbp.Voting.Players.Cache;

public interface IPlayerCacheItemProvider
{
    Task<PlayerCacheItem> GetAsync(Guid playerId);

    Task<List<PlayerCacheItem>> GetManyAsync(IEnumerable<Guid> playerIds);

    Task RemoveAsync(Guid playerId);
}
