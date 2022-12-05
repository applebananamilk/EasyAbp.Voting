using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace EasyAbp.Voting.Activities.Cache;

public class ActivityCacheItemProvider : IActivityCacheItemProvider, ISingletonDependency
{
    protected IDistributedCache<ActivityCacheItem> DistributedCache { get; }
    protected IActivityRepository ActivityRepository { get; }
    protected IJsonSerializer JsonSerializer { get; }

    public ActivityCacheItemProvider(
        IDistributedCache<ActivityCacheItem> distributedCache,
        IActivityRepository activityRepository,
        IJsonSerializer jsonSerializer)
    {
        DistributedCache = distributedCache;
        ActivityRepository = activityRepository;
        JsonSerializer = jsonSerializer;
    }

    public virtual async Task<ActivityCacheItem> GetAsync(Guid id)
    {
        string key = ActivityCacheItem.CalculateCacheKey(id);

        var cacheItem = await DistributedCache.GetAsync(key);

        if (cacheItem == null)
        {
            // TODO 状态是否单独缓存，查询失败结果是否需要缓存
            var activity = await ActivityRepository.FindAsync(p => p.Id == id && !p.IsDraft);

            if (activity == null)
            {
                throw new BusinessException(VotingErrorCodes.ActivityNotFound);
            }

            string value = JsonSerializer.Serialize(activity);
            cacheItem = JsonSerializer.Deserialize<ActivityCacheItem>(value);

            await DistributedCache.SetAsync(
                key,
                cacheItem,
                new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromHours(1) }
                );
        }

        return cacheItem;
    }

    public virtual async Task RemoveAsync(Guid id)
    {
        await DistributedCache.RemoveAsync(ActivityCacheItem.CalculateCacheKey(id));
    }
}
