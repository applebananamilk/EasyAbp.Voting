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
    protected IActivityCacheItemProvider ActivityCacheItemProvider { get; }

    public CacheExpirationCalculator(
        IClock clock,
        IActivityCacheItemProvider activityCacheItemProvider)
    {
        Clock = clock;
        ActivityCacheItemProvider = activityCacheItemProvider;
    }

    public virtual async Task<TimeSpan> CalculateAsync(Guid activityId)
    {
        var activityCacheItem = await ActivityCacheItemProvider.GetAsync(activityId);

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
