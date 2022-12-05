using EasyAbp.Voting.Activities.Cache;
using EasyAbp.Voting.Activities.Dtos;
using EasyAbp.Voting.Players;
using EasyAbp.Voting.Players.Cache;
using EasyAbp.Voting.Rules;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Activities;

public class ActivityAppService : VotingAppService, IActivityAppService
{
    protected IActivityViewsCacheManager ActivityViewsCacheManager { get; }
    protected IPlayerListCacheManager PlayerListCacheManager { get; }
    protected IPlayerRankingsCacheManager PlayerRankingsCacheManager { get; }
    protected IPlayerCacheItemProvider PlayerCacheItemProvider { get; }
    protected IActivityCacheItemProvider ActivityCacheItemProvider { get; }

    public ActivityAppService(
        IActivityViewsCacheManager activityViewsCacheManager,
        IPlayerListCacheManager playerListCacheManager,
        IPlayerRankingsCacheManager playerRankingsCacheManager,
        IPlayerCacheItemProvider playerCacheItemProvider,
        IActivityCacheItemProvider activityCacheItemProvider)
    {
        ActivityViewsCacheManager = activityViewsCacheManager;
        PlayerListCacheManager = playerListCacheManager;
        PlayerRankingsCacheManager = playerRankingsCacheManager;
        PlayerCacheItemProvider = playerCacheItemProvider;
        ActivityCacheItemProvider = activityCacheItemProvider;
    }

    public virtual async Task<ActivityDto> GetAsync(Guid id)
    {
        var activityCacheItem = await ActivityCacheItemProvider.GetAsync(id);

        await ActivityViewsCacheManager.IncrementAsync(id);

        return ObjectMapper.Map<ActivityCacheItem, ActivityDto>(activityCacheItem);
    }

    public virtual async Task<ActivityStatisticsDto> GetStatisticsAsync(Guid id)
    {
        var statisticsDto = new ActivityStatisticsDto();

        statisticsDto.Views = await ActivityViewsCacheManager.GetAsync(id);
        statisticsDto.TotalVotes = await PlayerRankingsCacheManager.GetTotalVotesAsync(id);
        statisticsDto.PlayerCount = await PlayerListCacheManager.GetCountAsync(id, null);

        return statisticsDto;
    }

    public virtual async Task<PlayerDto> GetPlayerAsync(Guid id, Guid playerId)
    {
        var playerCacheItem = await PlayerCacheItemProvider.GetAsync(playerId);

        if (playerCacheItem.Status != Status.Approved)
        {
            throw new BusinessException(VotingErrorCodes.PlayerInfoException);
        }

        var playerDto = ObjectMapper.Map<PlayerCacheItem, PlayerDto>(playerCacheItem);

        playerDto.Votes = (await PlayerRankingsCacheManager.GetAsync(playerDto.ActivityId, playerDto.Id)).Score;

        return playerDto;
    }

    public virtual async Task<PagedResultDto<PlayerGetListDto>> GetPlayerListAsync(Guid id, PlayerGetListInput input)
    {
        var totalCount = await PlayerListCacheManager.GetCountAsync(id, input.GroupId);

        var items = new List<PlayerGetListDto>();

        if (totalCount > 0)
        {
            var playerEntries = await PlayerListCacheManager.GetListAsync(id,
                input.GroupId,
                input.SkipCount,
                input.MaxResultCount);

            var playerCacheItems = await PlayerCacheItemProvider.GetManyAsync(playerEntries.Select(p => p.PlayerId));

            items = ObjectMapper.Map<List<PlayerCacheItem>, List<PlayerGetListDto>>(
                playerCacheItems
                );

            var playerRankingsEntries = await PlayerRankingsCacheManager.GetManyAsync(id, items.Select(p => p.Id));

            foreach (var item in items)
            {
                var entry = playerRankingsEntries.Find(p => p.PlayerId == item.Id);
                if (entry != null)
                {
                    item.Votes = entry.Score;
                }
            }
        }

        return new PagedResultDto<PlayerGetListDto>(totalCount, items);
    }

    public virtual async Task<PagedResultDto<PlayerRankingsDto>> GetPlayerRankingsAsync(Guid id, PlayerRankingsGetInput input)
    {
        var totalCount = await PlayerRankingsCacheManager.GetCountAsync(id, input.GroupId);

        var items = new List<PlayerRankingsDto>();

        if (totalCount > 0)
        {
            var playerRankingsEntries = await PlayerRankingsCacheManager.GetListAsync(
                 id,
                 input.GroupId,
                 input.SkipCount,
                 input.MaxResultCount);

            var playerCacheItems = await PlayerCacheItemProvider.GetManyAsync(
                playerRankingsEntries.Select(p => p.PlayerId)
                );

            items = ObjectMapper.Map<List<PlayerCacheItem>, List<PlayerRankingsDto>>(
                playerCacheItems
            );

            foreach (var item in items)
            {
                var entry = playerRankingsEntries.Find(p => p.PlayerId == item.Id);
                if (entry != null)
                {
                    item.Votes = entry.Score;
                }
            }
        }

        return new PagedResultDto<PlayerRankingsDto>(totalCount, items);
    }

    public virtual async Task<ListResultDto<PlayerDto>> LookupAsync(Guid id, PlayerLookupInput input)
    {
        var playerEntries = await PlayerListCacheManager.LookupAsync(id, input.GroupId, input.NameOrPlayerNumber);

        var items = new List<PlayerDto>();

        if (playerEntries.Count > 0)
        {
            var playerCacheItems = await PlayerCacheItemProvider.GetManyAsync(playerEntries.Select(p => p.PlayerId));

            items = ObjectMapper.Map<List<PlayerCacheItem>, List<PlayerDto>>(playerCacheItems);

            var playerRankingsEntries = await PlayerRankingsCacheManager.GetManyAsync(id, items.Select(p => p.Id));

            foreach (var item in items)
            {
                var entry = playerRankingsEntries.Find(p => p.PlayerId == item.Id);
                if (entry != null)
                {
                    item.Votes = entry.Score;
                }
            }
        }

        return new ListResultDto<PlayerDto>(
            items
            );
    }

    [Authorize]
    public virtual async Task VoteAsync(Guid id, Guid playerId)
    {
        int votes = 1;

        var activityCacheItem = await ActivityCacheItemProvider.GetAsync(id);

        if (activityCacheItem.ActivityStartTime > Clock.Now)
        {
            throw new BusinessException(VotingErrorCodes.ActivityNotStarted);
        }

        if (activityCacheItem.ActivityEndTime < Clock.Now)
        {
            throw new BusinessException(VotingErrorCodes.ActivityHasEnded);
        }

        var ruleValidator = LazyServiceProvider.LazyGetRequiredService<IRuleValidator>();
        var ruleValidationContext = RuleValidationContext.Create(id, playerId, votes: votes);
        await ruleValidator.ValidateAsync(ruleValidationContext);

        await PlayerRankingsCacheManager.VoteAsync(id, playerId, votes);
    }
}
