using EasyAbp.Voting.Activities.Cache;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace EasyAbp.Voting.Caching;

public class CacheExpirationCalculator : ICacheExpirationCalculator, ISingletonDependency
{
    public const int PersistenceTimeMinutes = 120;

    protected IClock Clock { get; }
    protected IActivityCacheItemProvider ActivityCache { get; }

    public CacheExpirationCalculator(
        IClock clock,
        IActivityCacheItemProvider activityCache)
    {
        Clock = clock;
        ActivityCache = activityCache;
    }

    public virtual async Task<TimeSpan> CalculateAsync(Guid activityId)
    {
        var activityCacheItem = await ActivityCache.GetAsync(activityId);

        var interval = activityCacheItem.ActivityEndTime - Clock.Now;
        if (interval < TimeSpan.Zero)
        {
            interval = TimeSpan.FromMinutes(PersistenceTimeMinutes);
        }
        else
        {
            interval.Add(TimeSpan.FromMinutes(PersistenceTimeMinutes));
        }

        return interval;
    }
}
