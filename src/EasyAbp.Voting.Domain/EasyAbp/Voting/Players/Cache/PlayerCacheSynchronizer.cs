using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace EasyAbp.Voting.Players.Cache;

public class PlayerCacheSynchronizer :
    ILocalEventHandler<EntityUpdatedEventData<Player>>,
    ILocalEventHandler<EntityDeletedEventData<Player>>,
    ILocalEventHandler<PlayerCacheAddEto>,
    ILocalEventHandler<PlayerCacheRemoveEto>, ITransientDependency
{
    protected IPlayerRepository PlayerRepository { get; }
    protected IPlayerListCacheManager PlayerListCacheManager { get; }
    protected IPlayerRankingsCacheManager PlayerRankingsCacheManager { get; }
    protected IPlayerCacheItemProvider PlayerCacheItemProvider { get; }

    public PlayerCacheSynchronizer(
        IPlayerRepository playerRepository,
        IPlayerListCacheManager playerListCacheManager,
        IPlayerRankingsCacheManager playerRankingsCacheManager,
        IPlayerCacheItemProvider playerCacheItemProvider)
    {
        PlayerRepository = playerRepository;
        PlayerListCacheManager = playerListCacheManager;
        PlayerRankingsCacheManager = playerRankingsCacheManager;
        PlayerCacheItemProvider = playerCacheItemProvider;
    }

    public virtual async Task HandleEventAsync(EntityUpdatedEventData<Player> eventData)
    {
        await RemoveCacheAsync(eventData.Entity.Id);
    }

    public virtual async Task HandleEventAsync(EntityDeletedEventData<Player> eventData)
    {
        await RemoveCacheAsync(eventData.Entity.Id);
        await PlayerRankingsCacheManager.RemovePlayerAsync(eventData.Entity);
        await PlayerListCacheManager.RemovePlayerAsync(eventData.Entity);
    }

    public async Task HandleEventAsync(PlayerCacheAddEto eventData)
    {
        var player = await PlayerRepository.FindAsync(p => p.Id == eventData.PlayerId);

        if (player != null)
        {
            await PlayerRankingsCacheManager.AddPlayerAsync(player);
            await PlayerListCacheManager.AddPlayerAsync(player);
        }
    }

    public async Task HandleEventAsync(PlayerCacheRemoveEto eventData)
    {
        var player = await PlayerRepository.FindAsync(p => p.Id == eventData.PlayerId);

        if (player != null)
        {
            await PlayerRankingsCacheManager.RemovePlayerAsync(player);
            await PlayerListCacheManager.RemovePlayerAsync(player);
        }
    }

    protected virtual async Task RemoveCacheAsync(Guid playerId)
    {
        await PlayerCacheItemProvider.RemoveAsync(playerId);
    }
}
