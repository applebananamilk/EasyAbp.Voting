using EasyAbp.Voting.Caching;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Uow;

namespace EasyAbp.Voting.Activities.Cache;

public class ActivityViewsCacheManager : IActivityViewsCacheManager, ISingletonDependency
{
    protected IDatabase Redis { get; }
    protected IUnitOfWorkManager UnitOfWorkManager { get; }
    protected IAbpDistributedLock DistributedLock { get; }
    protected IActivityRepository ActivityRepository { get; }
    protected IRedisCacheKeyNormalizer KeyNormalizer { get; }

    public ActivityViewsCacheManager(
        IRedisDatabaseAccessor redisDatabaseAccessor,
        IUnitOfWorkManager unitOfWorkManager,
        IAbpDistributedLock distributedLock,
        IActivityRepository activityRepository,
        IRedisCacheKeyNormalizer keyNormalizer)
    {
        Redis = redisDatabaseAccessor.RedisDatabase;
        UnitOfWorkManager = unitOfWorkManager;
        DistributedLock = distributedLock;
        ActivityRepository = activityRepository;
        KeyNormalizer = keyNormalizer;
    }

    public async Task IncrementAsync(Guid activityId)
    {
        await EnsureCacheLoadedAsync(activityId);

        await Redis.HashIncrementAsync(
            NormalizeKey(),
            NormalizeMember(activityId)
            );
    }

    public async Task<long> GetAsync(Guid activityId)
    {
        await EnsureCacheLoadedAsync(activityId);

        return (long)await Redis.HashGetAsync(
            NormalizeKey(),
            NormalizeMember(activityId)
            );
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await using var handle = await DistributedLock.TryAcquireAsync("Voting_PersistentActivityViews");

        if (handle != null)
        {
            using var uow = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            var hashEntries = await Redis.HashGetAllAsync(NormalizeKey());
            var activityViewsItems = hashEntries.Select(p => new { Id = Guid.Parse(p.Name), Views = (long)p.Value });

            if (activityViewsItems.Any())
            {
                var activityIds = activityViewsItems.Select(x => x.Id);
                var activities = await ActivityRepository.GetListAsync(p => activityIds.Contains(p.Id));

                if (activities.Any())
                {
                    foreach (var activity in activities)
                    {
                        var activityViewsItem = activityViewsItems.FirstOrDefault(p => p.Id == activity.Id);

                        if (activityViewsItem != null && activityViewsItem.Views != activity.Views)
                        {
                            activity.SetViews(activityViewsItem.Views);
                            await ActivityRepository.UpdateAsync(activity, autoSave: true);
                        }
                    }
                }
            }

            await uow.CompleteAsync();
        }
        else
        {
            // wait?
        }
    }

    protected virtual async Task EnsureCacheLoadedAsync(Guid activityId)
    {
        if (!await Redis.HashExistsAsync(NormalizeKey(), NormalizeMember(activityId)))
        {
            var activity = await ActivityRepository.GetAsync(activityId);

            await Redis.HashIncrementAsync(
                NormalizeKey(),
                NormalizeMember(activityId),
                activity.Views
                );

            await Redis.KeyExpireAsync(NormalizeKey(), TimeSpan.FromDays(10));
        }
    }

    protected virtual string NormalizeKey()
    {
        return KeyNormalizer.NormalizeKey<string>("Voting:Activity:Views");
    }

    protected virtual string NormalizeMember(Guid activityId)
    {
        return activityId.ToString();
    }
}
