using EasyAbp.Voting.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace EasyAbp.Voting.Players.Cache;

public class PlayerCacheItemProvider : IPlayerCacheItemProvider, ISingletonDependency
{
    protected IDatabase Redis { get; }
    protected IRedisCacheKeyNormalizer KeyNormalizer { get; }
    protected ICacheExpirationCalculator CacheExpirationCalculator { get; }
    protected IJsonSerializer JsonSerializer { get; }
    protected IPlayerRepository PlayerRepository { get; }

    public PlayerCacheItemProvider(
        IRedisDatabaseAccessor redisDatabaseAccessor,
        IRedisCacheKeyNormalizer keyNormalizer,
        ICacheExpirationCalculator cacheExpirationCalculator,
        IJsonSerializer jsonSerializer,
        IPlayerRepository playerRepository)
    {
        Redis = redisDatabaseAccessor.RedisDatabase;
        KeyNormalizer = keyNormalizer;
        CacheExpirationCalculator = cacheExpirationCalculator;
        JsonSerializer = jsonSerializer;
        PlayerRepository = playerRepository;
    }

    public virtual async Task<PlayerCacheItem> GetAsync(Guid playerId)
    {
        var key = NormalizeKey(PlayerCacheItem.CalculateCacheKey(playerId));

        string value = await Redis.StringGetAsync(key);

        if (value == null)
        {
            var player = await PlayerRepository.FindAsync(playerId);

            if (player == null || player?.Status != Status.Approved)
            {
                throw new BusinessException(VotingErrorCodes.PlayerInfoException);
            }

            value = JsonSerializer.Serialize(player);

            await Redis.StringSetAsync(
                key: key,
                value: value,
                expiry: await CacheExpirationCalculator.CalculateAsync(player.ActivityId)
                    + CalculateRandomExpiry());
        }

        return JsonSerializer.Deserialize<PlayerCacheItem>(value);
    }

    public virtual async Task<List<PlayerCacheItem>> GetManyAsync(IEnumerable<Guid> playerIds)
    {
        var items = playerIds.Distinct().Select(p => new
        {
            PlayerId = p,
            Task = Redis.StringGetAsync(NormalizeKey(PlayerCacheItem.CalculateCacheKey(p)))
        });

        var tasks = items.Select(p => p.Task).ToArray();

        await Task.WhenAll(tasks);

        List<PlayerCacheItem> playerCacheItems = new(playerIds.Count());
        List<Guid> notExists = new();

        foreach (var item in items)
        {
            if (!item.Task.Result.IsNull)
            {
                playerCacheItems.Add(JsonSerializer.Deserialize<PlayerCacheItem>(item.Task.Result));
            }
            else
            {
                notExists.Add(item.PlayerId);
            }
        }

        if (notExists.Any())
        {
            var players = await PlayerRepository.GetListAsync(p => notExists.Contains(p.Id));

            var time = await CacheExpirationCalculator.CalculateAsync(players.First().ActivityId);

            var createCacheTasks = players.Select(player =>
            {
                string value = JsonSerializer.Serialize(player);

                playerCacheItems.Add(JsonSerializer.Deserialize<PlayerCacheItem>(value));

                return Redis.StringSetAsync(
                    key: NormalizeKey(PlayerCacheItem.CalculateCacheKey(player.Id)),
                    value: value,
                    expiry: time + CalculateRandomExpiry()
                    );
            });

            await Task.WhenAll(createCacheTasks);
        }

        return playerCacheItems;
    }

    public virtual async Task RemoveAsync(Guid playerId)
    {
        var key = NormalizeKey(PlayerCacheItem.CalculateCacheKey(playerId));

        await Redis.KeyDeleteAsync(key);
    }

    protected virtual TimeSpan CalculateRandomExpiry()
    {
        return TimeSpan.FromMinutes(RandomHelper.GetRandom(1, 60));
    }

    protected virtual string NormalizeKey(string key)
    {
        return KeyNormalizer.NormalizeKey<PlayerCacheItem>(key);
    }
}
