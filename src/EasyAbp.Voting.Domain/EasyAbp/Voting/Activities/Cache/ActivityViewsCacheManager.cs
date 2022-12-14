using EasyAbp.Voting.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
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

    public async Task IncrementAsync(Guid activityId, int number = 1)
    {
        Check.Range(number, nameof(number), 1);

        await EnsureCacheLoadedAsync(activityId);

        await Redis.HashIncrementAsync(
            NormalizeKey(),
            NormalizeMember(activityId),
            number);
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

            var activityViewsItems = await GetActivityViewsCacheItemsAsync();

            if (activityViewsItems.Any())
            {
                var activityIds = activityViewsItems.Select(x => x.ActivityId);
                var activities = await ActivityRepository.GetListAsync(p => activityIds.Contains(p.Id));

                if (activities.Any())
                {
                    foreach (var activity in activities)
                    {
                        var activityViewsItem = activityViewsItems.FirstOrDefault(p => p.ActivityId == activity.Id);

                        if (activityViewsItem != null && activityViewsItem.Views != activity.Views)
                        {
                            activity.SetViews(activityViewsItem.Views);

                            await ActivityRepository.UpdateAsync(activity, autoSave: true);

                            await RemoveCacheItemAsync(activity.Id);
                        }
                    }
                }
            }

            // 不提交不会发送事件
            // 这样的话修改浏览量就不会导致活动缓存失效
            //await uow.CompleteAsync();
        }
        else
        {
            // wait?
        }
    }

    protected virtual async Task<List<ActivityViewsCacheItem>> GetActivityViewsCacheItemsAsync()
    {
        var hashEntries = await Redis.HashGetAllAsync(NormalizeKey());

        var activityViewsItems = hashEntries
            .Select(p => new ActivityViewsCacheItem(Guid.Parse(p.Name), (long)p.Value));

        return activityViewsItems.ToList();
    }

    protected virtual Task RemoveCacheItemAsync(Guid activityId)
    {
        return Redis.HashDeleteAsync(NormalizeKey(), NormalizeMember(activityId));
    }

    protected virtual async Task EnsureCacheLoadedAsync(Guid activityId)
    {
        if (!await Redis.HashExistsAsync(NormalizeKey(), NormalizeMember(activityId)))
        {
            var activityQuery = await ActivityRepository.GetQueryableAsync();

            var activityViews = await ActivityRepository.AsyncExecuter.SingleOrDefaultAsync(
                activityQuery.Where(p => p.Id == activityId && !p.IsDraft).Select(p => new { p.Id, p.Views })
                );

            if (activityViews == null)
            {
                throw new BusinessException(VotingErrorCodes.ActivityNotFound);
            }

            await Redis.HashIncrementAsync(
                NormalizeKey(),
                NormalizeMember(activityId),
                activityViews.Views
                );
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

    public class ActivityViewsCacheItem
    {
        public Guid ActivityId { get; }

        public long Views { get; }

        public ActivityViewsCacheItem(Guid activityId, long views)
        {
            ActivityId = activityId;
            Views = views;
        }
    }
}
