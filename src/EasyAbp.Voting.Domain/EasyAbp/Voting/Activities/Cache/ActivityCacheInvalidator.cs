using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace EasyAbp.Voting.Activities.Cache;

public class ActivityCacheInvalidator :
    ILocalEventHandler<EntityUpdatedEventData<Activity>>,
    ILocalEventHandler<EntityDeletedEventData<Activity>>,
    ITransientDependency
{
    public IActivityCacheItemProvider ActivityCacheItemProvider { get; }

    public ActivityCacheInvalidator(IActivityCacheItemProvider activityCacheItemProvider)
    {
        ActivityCacheItemProvider = activityCacheItemProvider;
    }

    public virtual Task HandleEventAsync(EntityUpdatedEventData<Activity> eventData)
    {
        return RemoveCacheAsync(eventData.Entity);
    }

    public virtual Task HandleEventAsync(EntityDeletedEventData<Activity> eventData)
    {
        return RemoveCacheAsync(eventData.Entity);
    }

    protected virtual Task RemoveCacheAsync(Activity activity)
    {
        return ActivityCacheItemProvider.RemoveAsync(activity.Id);
    }
}
