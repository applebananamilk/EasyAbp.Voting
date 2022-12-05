using EasyAbp.Voting.Caching;
using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Timing;

namespace EasyAbp.Voting.Players.Cache;

public class PlayerListCacheManager : IPlayerListCacheManager, ISingletonDependency
{
    protected IDatabase Redis { get; }
    protected IRedisCacheKeyNormalizer KeyNormalizer { get; }
    protected ICacheExpirationCalculator CacheExpirationCalculator { get; }
    protected IAbpDistributedLock DistributedLock { get; }
    protected IMemoryCache MemoryCache { get; }
    protected IClock Clock { get; }
    protected IPlayerRepository PlayerRepository { get; }

    public PlayerListCacheManager(
        IRedisDatabaseAccessor redisDatabaseAccessor,
        IRedisCacheKeyNormalizer keyNormalizer,
        ICacheExpirationCalculator cacheExpirationCalculator,
        IAbpDistributedLock distributedLock,
        IMemoryCache memoryCache,
        IClock clock,
        IPlayerRepository playerRepository)
    {
        Redis = redisDatabaseAccessor.RedisDatabase;
        KeyNormalizer = keyNormalizer;
        CacheExpirationCalculator = cacheExpirationCalculator;
        DistributedLock = distributedLock;
        MemoryCache = memoryCache;
        Clock = clock;
        PlayerRepository = playerRepository;

    }
    public async Task AddPlayerAsync(Player player)
    {
        await EnsureLoadedAsync(player.ActivityId);

        await InternalAddAsync(player.ActivityId, player.Id, player.GroupId, player.Name, player.PlayerNumber.Value);
    }

    protected virtual async Task InternalAddAsync(
        Guid activityId,
        Guid playerId,
        Guid? groupId,
        string name,
        int playerNumber)
    {
        var keys = NormalizeWriteKeys(activityId, groupId);

        var tasks = keys.Select(key => Redis.SortedSetAddAsync(
            key: key,
            member: NormalizeMember(playerId, name, playerNumber),
            score: playerNumber,
            when: When.NotExists
            )
        );

        await Task.WhenAll(tasks);
    }

    public async Task<int> GetCountAsync(Guid activityId, Guid? groupId)
    {
        await EnsureLoadedAsync(activityId);

        var key = NormalizeReadKey(activityId, groupId);

        return (int)await Redis.SortedSetLengthAsync(key);
    }

    public async Task<List<PlayerSortedSetEntry>> GetListAsync(Guid activityId, Guid? groupId, int skipCount, int maxResultCount)
    {
        await EnsureLoadedAsync(activityId);

        var key = NormalizeReadKey(activityId, groupId);

        var sortedSetEntries = await Redis.SortedSetRangeByRankWithScoresAsync(
            key: key,
            start: skipCount,
            stop: skipCount + maxResultCount,
            order: Order.Ascending
            );

        var playerSortedSetEntries = sortedSetEntries.Select(Map);

        return playerSortedSetEntries.ToList();
    }

    public async Task<List<PlayerSortedSetEntry>> LookupAsync(Guid activityId, Guid? groupId, string nameOrPlayerNumber)
    {
        List<PlayerSortedSetEntry> playerSortedSetEntries = new();

        if (nameOrPlayerNumber.IsNullOrWhiteSpace())
        {
            return playerSortedSetEntries;
        }

        await EnsureLoadedAsync(activityId);

        var key = NormalizeReadKey(activityId, groupId);

        await foreach (var sortedSetEntry in Redis.SortedSetScanAsync(key: key, pattern: $"*{nameOrPlayerNumber}*"))
        {
            playerSortedSetEntries.Add(Map(sortedSetEntry));
        };

        return playerSortedSetEntries;
    }

    public async Task RemovePlayerAsync(Player player)
    {
        var keys = NormalizeWriteKeys(player.ActivityId, player.GroupId);

        var tasks = keys.Select(key => Redis.SortedSetRemoveAsync(
          key: key,
          member: NormalizeMember(player.Id, player.Name, player.PlayerNumber.Value)
          )
        );

        await Task.WhenAll(tasks);
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
                    MemoryCache.Set(NormalizeReadKey(activityId), DateTime.Now, new MemoryCacheEntryOptions { AbsoluteExpiration = Clock.Now.Add(timeSpan.Value) });
                }
                return;
            }

            var playerQueryable = await PlayerRepository.GetQueryableAsync();

            var players = await PlayerRepository.AsyncExecuter.ToListAsync(
                playerQueryable
                    .Where(p => p.ActivityId == activityId && p.Status == Status.Approved)
                    .Select(p => new { p.ActivityId, p.Id, p.GroupId, p.Name, PlayerNumber = p.PlayerNumber.Value })
                );

            foreach (var player in players)
            {
                await InternalAddAsync(player.ActivityId, player.Id, player.GroupId, player.Name, player.PlayerNumber);
            }

            var expiry = await CacheExpirationCalculator.CalculateAsync(activityId);

            var expireTasks = new List<Task>();
            expireTasks.Add(Redis.KeyExpireAsync(NormalizeReadKey(activityId), expiry));

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
        return $"Voting:{activityId}:Lock:EnsureLoadedPlayers";
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
            key = $"Voting:Activity:{activityId}:Group:{groupId.Value}:Players";
        }
        else
        {
            key = $"Voting:Activity:{activityId}:Players";
        }

        return KeyNormalizer.NormalizeKey<string>(key);
    }

    protected virtual string NormalizeMember(Guid playerId, string playerName, int playerNumber)
    {
        return $"{playerId}:{playerName}:{playerNumber}";
    }

    protected virtual PlayerSortedSetEntry Map(SortedSetEntry sortedSetEntry)
    {
        var playerId = Guid.Parse(sortedSetEntry.Element.ToString().Split(':')[0]);
        var score = (long)sortedSetEntry.Score;

        return new PlayerSortedSetEntry(playerId, score);
    }
}
