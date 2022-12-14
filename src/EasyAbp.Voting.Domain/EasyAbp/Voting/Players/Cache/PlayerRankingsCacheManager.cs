using EasyAbp.Voting.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Linq;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace EasyAbp.Voting.Players.Cache;

public class PlayerRankingsCacheManager : IPlayerRankingsCacheManager, ISingletonDependency
{
    protected IDatabase Redis { get; }
    protected IRedisCacheKeyNormalizer KeyNormalizer { get; }
    protected ICacheExpirationCalculator CacheExpirationCalculator { get; }
    protected IAbpDistributedLock DistributedLock { get; }
    protected IMemoryCache MemoryCache { get; }
    protected IClock Clock { get; }
    protected IPlayerCacheItemProvider PlayerCacheItemProvider { get; }
    protected ILogger<PlayerRankingsCacheManager> Logger { get; }
    protected IUnitOfWorkManager UnitOfWorkManager { get; }
    protected IExceptionNotifier ExceptionNotifier { get; }
    protected VotingOptions VotingOptions { get; }
    protected IPlayerRepository PlayerRepository { get; }

    public PlayerRankingsCacheManager(
        IRedisDatabaseAccessor redisDatabaseAccessor,
        IRedisCacheKeyNormalizer keyNormalizer,
        ICacheExpirationCalculator cacheExpirationCalculator,
        IAbpDistributedLock distributedLock,
        IMemoryCache memoryCache,
        IClock clock,
        IPlayerCacheItemProvider playerCacheItemProvider,
        IOptions<VotingOptions> votingOptionAccessor,
        ILogger<PlayerRankingsCacheManager> logger,
        IUnitOfWorkManager unitOfWorkManager,
        IExceptionNotifier exceptionNotifier,
        IPlayerRepository playerRepository)
    {
        Redis = redisDatabaseAccessor.RedisDatabase;
        KeyNormalizer = keyNormalizer;
        CacheExpirationCalculator = cacheExpirationCalculator;
        DistributedLock = distributedLock;
        MemoryCache = memoryCache;
        Clock = clock;
        PlayerCacheItemProvider = playerCacheItemProvider;
        Logger = logger;
        UnitOfWorkManager = unitOfWorkManager;
        ExceptionNotifier = exceptionNotifier;
        VotingOptions = votingOptionAccessor.Value;
        PlayerRepository = playerRepository;
    }

    public virtual async Task AddPlayerAsync(Player player)
    {
        await EnsureLoadedAsync(player.ActivityId);

        await InternalAddAsync(player.ActivityId, player.Id, player.GroupId, player.PlayerVotes.Votes);
    }

    protected virtual async Task InternalAddAsync(
        Guid activityId,
        Guid playerId,
        Guid? groupId,
        long votes)
    {
        var keys = NormalizeWriteKeys(activityId, groupId);

        var tasks = keys.Select(key => Redis.SortedSetAddAsync(
            key: key,
            member: NormalizeMember(playerId),
            score: votes,
            when: When.NotExists
            )
        ).ToList<Task>();

        tasks.Add(CreateWriteTotalVotesTaskAsync(activityId, votes));

        await Task.WhenAll(tasks);
    }

    public virtual async Task VoteAsync(Guid activityId, Guid playerId, long votes)
    {
        await EnsureLoadedAsync(activityId);

        var readKey = NormalizeReadKey(activityId, null);

        var rankIndex = await Redis.SortedSetRankAsync(readKey, NormalizeMember(playerId));

        if (rankIndex != null)
        {
            var playerCacheItem = await PlayerCacheItemProvider.GetAsync(playerId);

            if (playerCacheItem.Status == Status.Approved)
            {
                await InternalAddAsync(
                    playerCacheItem.ActivityId,
                    playerCacheItem.Id,
                    playerCacheItem.GroupId,
                    votes);

                await MarkAsync(activityId, playerId);
                return;
            }
        }

        throw new UserFriendlyException(message: "error");
    }

    protected virtual Task CreateWriteTotalVotesTaskAsync(Guid activityId, long votes)
    {
        return Redis.StringIncrementAsync(NormalizeTotalVotesKey(activityId), votes);
    }

    public virtual async Task<long> GetTotalVotesAsync(Guid activityId)
    {
        await EnsureLoadedAsync(activityId);

        return (long)await Redis.StringGetAsync(NormalizeTotalVotesKey(activityId));
    }

    public async Task<PlayerSortedSetEntry> GetAsync(Guid activityId, Guid playerId)
    {
        await EnsureLoadedAsync(activityId);

        var key = NormalizeReadKey(activityId, null);
        var member = NormalizeMember(playerId);

        var score = await Redis.SortedSetScoreAsync(key, member);

        return new PlayerSortedSetEntry(playerId, (long)(score ?? 0));
    }

    public virtual async Task<List<PlayerSortedSetEntry>> GetManyAsync(Guid activityId, IEnumerable<Guid> playerIds)
    {
        await EnsureLoadedAsync(activityId);

        var key = NormalizeReadKey(activityId, null);
        var members = playerIds.Select(playerId => NormalizeMember(playerId));

        var tasks = members.Select(member => Redis.SortedSetScoreAsync(key, member)).ToList();

        var scores = await Task.WhenAll(tasks);

        return playerIds
            .Select((playerId, i) => new PlayerSortedSetEntry(playerId, (long)(scores[i] ?? 0)))
            .ToList();
    }

    public virtual async Task<long> GetCountAsync(Guid activityId, Guid? groupId)
    {
        await EnsureLoadedAsync(activityId);

        var key = NormalizeReadKey(activityId, groupId);

        return (int)await Redis.SortedSetLengthAsync(key);
    }

    public virtual async Task<List<PlayerSortedSetEntry>> GetListAsync(Guid activityId, Guid? groupId, int skipCount, int maxResultCount)
    {
        await EnsureLoadedAsync(activityId);

        var key = NormalizeReadKey(activityId, groupId);

        var sortedSetEntries = await Redis.SortedSetRangeByRankWithScoresAsync(
            key: key,
            start: skipCount,
            stop: skipCount + maxResultCount,
            order: Order.Descending
            );

        var playerSortedSetEntries = sortedSetEntries.Select(Map);

        return playerSortedSetEntries.ToList();
    }

    public virtual async Task RemovePlayerAsync(Player player)
    {
        // FIX 是否加锁，在移除期间禁止投票。如果加锁是否降低很多性能
        var keys = NormalizeWriteKeys(player.ActivityId, player.GroupId);

        var tasks = keys.Select(key => Redis.SortedSetRemoveAsync(
          key: key,
          member: NormalizeMember(player.Id)
          )
        ).ToList<Task>();

        var playerSortedSetEntry = await GetAsync(player.ActivityId, player.Id);

        tasks.Add(Redis.StringDecrementAsync(NormalizeTotalVotesKey(player.ActivityId), playerSortedSetEntry.Score));

        var markInfo = await GetMarkOrNullAsync(player.ActivityId, player.Id);
        var removeMarkResult = false;

        await Task.WhenAll(tasks);
        try
        {
            try
            {
                if (markInfo != null)
                {
                    await RemoveMarkAsync(markInfo);
                    removeMarkResult = true;
                }
            }
            catch (Exception ex)
            {
                await ExceptionNotifier.NotifyAsync(new ExceptionNotificationContext(ex, LogLevel.Error, false));
            }

            await PlayerRepository.UpdateVotesAsync(playerSortedSetEntry.PlayerId, playerSortedSetEntry.Score);
        }
        catch (Exception)
        {
            if (markInfo != null && removeMarkResult)
            {
                await RestoreMarkAsync(markInfo);
            }

            if (playerSortedSetEntry != null)
            {
                await InternalAddAsync(player.ActivityId, player.Id, player.GroupId, playerSortedSetEntry.Score);
            }

            throw;
        }
    }

    protected const string DistributedLockName = "Voting_PersistentPlayerRankingsVotes";

    public virtual async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await using var handle = await DistributedLock.TryAcquireAsync(DistributedLockName);

        if (handle != null)
        {
            var markInfos = await GetMarkInfoListAsync();

            if (!markInfos.Any())
            {
                return;
            }

            using var uow = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

            var resolvedMarkInfos = markInfos.Select(p => new { SourceMarkInfo = p, ActivityId = p.ResolveActivityId(), PlayerId = p.ResolvePlayerId() });

            // 按活动Id分组处理
            foreach (var activityMarkInfoGroup in resolvedMarkInfos.GroupBy(p => p.ActivityId))
            {
                var updateCount = 0;

                // 获取被标记的选手
                var playerIds = activityMarkInfoGroup.Select(p => p.PlayerId);

                var playerQuery = await PlayerRepository.GetQueryableAsync();

                var asyncExecuter = uow.ServiceProvider.GetRequiredService<IAsyncQueryableExecuter>();
                var players = await asyncExecuter.ToListAsync(
                    playerQuery.Where(p => playerIds.Contains(p.Id)).Select(p => new { p.Id, p.PlayerVotes.Votes })
                    );

                // 获取选手现有票数
                var playerSortedSetEntries = await GetManyAsync(activityMarkInfoGroup.Key, playerIds);

                foreach (var markInfo in activityMarkInfoGroup)
                {
                    var player = players.Find(p => p.Id == markInfo.PlayerId);
                    var playerSortedSetEntry = playerSortedSetEntries.Find(p => p.PlayerId == markInfo.PlayerId);
                    if (player != null && playerSortedSetEntry != null)
                    {
                        try
                        {
                            if (player.Votes != playerSortedSetEntry.Score)
                            {
                                await RemoveMarkAsync(markInfo.SourceMarkInfo);

                                await PlayerRepository.UpdateVotesAsync(player.Id, playerSortedSetEntry.Score);
                                updateCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            await RestoreMarkAsync(markInfo.SourceMarkInfo);

                            await ExceptionNotifier.NotifyAsync(
                               new ExceptionNotificationContext(ex, LogLevel.Error)
                                );
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

    protected virtual async Task EnsureLoadedAsync(Guid activityId)
    {
        if (MemoryCache.TryGetValue<DateTime>(NormalizeReadKey(activityId), out var _))
        {
            return;
        };

        await using var handle = await DistributedLock.TryAcquireAsync(GetEnsureLoadedLockName(activityId), timeout: TimeSpan.FromSeconds(5));

        if (handle != null)
        {
            if (await Redis.KeyExistsAsync(NormalizeReadKey(activityId)))
            {
                var timeSpan = await Redis.KeyTimeToLiveAsync(NormalizeReadKey(activityId));
                if (timeSpan.HasValue)
                {
                    MemoryCache.Set(NormalizeReadKey(activityId), Clock.Now, new MemoryCacheEntryOptions { AbsoluteExpiration = Clock.Now.Add(timeSpan.Value) });
                }
                return;
            }

            var playerQueryable = await PlayerRepository.GetQueryableAsync();

            var players = await PlayerRepository.AsyncExecuter.ToListAsync(
                playerQueryable
                    .Where(p => p.ActivityId == activityId && p.Status == Status.Approved)
                    .Select(p => new { p.ActivityId, p.Id, p.GroupId, p.Name, p.PlayerVotes.Votes })
                );

            foreach (var player in players)
            {
                await InternalAddAsync(player.ActivityId, player.Id, player.GroupId, player.Votes);
            }

            var expiry = await CacheExpirationCalculator.CalculateAsync(activityId);

            var expireTasks = new List<Task>();
            expireTasks.Add(Redis.KeyExpireAsync(NormalizeReadKey(activityId), expiry));
            expireTasks.Add(Redis.KeyExpireAsync(NormalizeTotalVotesKey(activityId), expiry));

            foreach (var playersG in players.GroupBy(p => p.GroupId))
            {
                expireTasks.Add(Redis.KeyExpireAsync(NormalizeReadKey(activityId, playersG.Key), expiry));
            }

            await Task.WhenAll(expireTasks);
        }
        else
        {
            throw new UserFriendlyException(message: "Loading...");
        }
    }

    protected virtual string GetEnsureLoadedLockName(Guid activityId)
    {
        return $"Voting:{activityId}:Lock:EnsureLoadedPlayerRankings";
    }

    protected virtual IEnumerable<string> NormalizeWriteKeys(Guid activityId, Guid? groupId)
    {
        var keys = new List<string>(2);

        if (groupId.HasValue)
        {
            keys.Add(NormalizeReadKey(activityId, groupId.Value));
        }
        keys.Add(NormalizeReadKey(activityId));

        return keys;
    }

    protected virtual string NormalizeReadKey(Guid activityId, Guid? groupId = null)
    {
        string key;

        if (groupId.HasValue)
        {
            key = $"Voting:Activity:{activityId}:Group:{groupId.Value}:PlayerRankings";
        }
        else
        {
            key = $"Voting:Activity:{activityId}:PlayerRankings";
        }

        return KeyNormalizer.NormalizeKey<string>(key);
    }

    protected virtual string NormalizeTotalVotesKey(Guid activityId)
    {
        string key = $"Voting:Activity:{activityId}:PlayerRankings:TotalVotes";
        return KeyNormalizer.NormalizeKey<string>(key);
    }

    protected virtual string NormalizeMember(Guid playerId)
    {
        return $"{playerId}";
    }

    protected virtual PlayerSortedSetEntry Map(SortedSetEntry sortedSetEntry)
    {
        var playerId = Guid.Parse(sortedSetEntry.Element);
        var score = (long)sortedSetEntry.Score;

        return new PlayerSortedSetEntry(playerId, score);
    }

    #region Mark
    protected virtual Task MarkAsync(Guid activityId, Guid playerId)
    {
        return Redis.HashSetAsync(
            NormalizeMarkKey(),
            NormalizeMarkMember(activityId, playerId),
            Clock.Now.ToString()
            );
    }

    protected virtual async Task<MarkInfo> GetMarkOrNullAsync(Guid activityId, Guid playerId)
    {
        var member = NormalizeMarkMember(activityId, playerId);
        var redisValue = await Redis.HashGetAsync(
            NormalizeMarkKey(),
            member
            );

        if (redisValue.IsNull)
        {
            return null;
        }

        return new MarkInfo { MarkCacheKey = member, MarkExpires = DateTime.Parse(redisValue) };
    }

    protected virtual Task RemoveMarkAsync(MarkInfo markInfo)
    {
        return Redis.HashDeleteAsync(
            NormalizeMarkKey(),
            markInfo.MarkCacheKey
            );
    }

    protected virtual async Task<IEnumerable<MarkInfo>> GetMarkInfoListAsync()
    {
        var entries = await Redis.HashGetAllAsync(
             NormalizeMarkKey()
             );

        return entries.Select(p => new MarkInfo { MarkCacheKey = p.Name, MarkExpires = DateTime.Parse(p.Value.ToString()) });
    }

    protected virtual async Task RestoreMarkAsync(MarkInfo markInfo)
    {
        var activityId = markInfo.ResolveActivityId();
        var playerId = markInfo.ResolvePlayerId();

        await Redis.HashSetAsync(
            NormalizeMarkKey(),
            NormalizeMarkMember(activityId, playerId),
            markInfo.MarkExpires.ToString()
            );
    }

    protected virtual string NormalizeMarkKey()
    {
        return "Voting:PlayerRankings:Persistent:Mark";
    }

    protected virtual string NormalizeMarkMember(Guid activityId, Guid playerId)
    {
        return $"ActivityId:{activityId}:PlayerId:{playerId}";
    }
    #endregion
}

public class MarkInfo
{
    public string MarkCacheKey { get; set; }

    public DateTime MarkExpires { get; set; }

    public Guid ResolveActivityId()
    {
        return Guid.Parse(MarkCacheKey.Split(':')[1]);
    }

    public Guid ResolvePlayerId()
    {
        return Guid.Parse(MarkCacheKey.Split(':')[3]);
    }
}