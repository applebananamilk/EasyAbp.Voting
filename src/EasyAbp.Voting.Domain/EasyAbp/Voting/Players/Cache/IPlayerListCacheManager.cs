using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyAbp.Voting.Players.Cache;

public interface IPlayerListCacheManager
{
    Task AddPlayerAsync(Player player);

    Task<int> GetCountAsync(Guid activityId, Guid? groupId);

    Task<List<PlayerSortedSetEntry>> GetListAsync(Guid activityId, Guid? groupId, int skipCount, int maxResultCount, bool newest = false);

    Task<List<PlayerSortedSetEntry>> LookupAsync(Guid activityId, Guid? groupId, string nameOrPlayerNumber);

    Task RemovePlayerAsync(Player player);
}
