using System;
using System.Threading.Tasks;

namespace EasyAbp.Voting.Activities.Cache;

public interface IActivityCacheItemProvider
{
    Task<ActivityCacheItem> GetAsync(Guid id);

    Task RemoveAsync(Guid id);
}
