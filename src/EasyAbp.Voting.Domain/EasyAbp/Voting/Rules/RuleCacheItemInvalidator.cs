using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus.Distributed;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则缓存项同步修改及时失效。
/// </summary>
public class RuleCacheItemInvalidator : IDistributedEventHandler<EntityUpdatedEventData<Rule>>, ITransientDependency
{
    protected IDistributedCache<RuleCacheItem> DistributedCache { get; }

    public RuleCacheItemInvalidator(IDistributedCache<RuleCacheItem> distributedCache)
    {
        DistributedCache = distributedCache;
    }

    public virtual Task HandleEventAsync(EntityUpdatedEventData<Rule> eventData)
    {
        return DistributedCache.RemoveAsync(
            RuleCacheItem.CalculateCacheKey(eventData.Entity.ActivityId)
            );
    }
}
