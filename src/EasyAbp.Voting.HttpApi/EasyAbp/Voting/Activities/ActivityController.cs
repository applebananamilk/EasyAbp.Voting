using EasyAbp.Voting.Activities.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Activities;

[RemoteService(Name = VotingRemoteServiceConsts.RemoteServiceName)]
[Area(VotingRemoteServiceConsts.ModuleName)]
[Route("api/voting/activities")]
public class ActivityController : VotingController, IActivityAppService
{
    private readonly IActivityAppService _activityAppService;

    public ActivityController(IActivityAppService activityAppService)
    {
        _activityAppService = activityAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public Task<ActivityDto> GetAsync(Guid id)
    {
        return _activityAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("{id}/statistics")]
    public Task<ActivityStatisticsDto> GetStatisticsAsync(Guid id)
    {
        return _activityAppService.GetStatisticsAsync(id);
    }

    [HttpGet]
    [Route("{id}/players/{playerId}")]
    public Task<PlayerDto> GetPlayerAsync(Guid id, Guid playerId)
    {
        return _activityAppService.GetPlayerAsync(id, playerId);
    }

    [HttpGet]
    [Route("{id}/players")]
    public Task<PagedResultDto<PlayerGetListDto>> GetPlayerListAsync(Guid id, PlayerGetListInput input)
    {
        return _activityAppService.GetPlayerListAsync(id, input);
    }

    [HttpGet]
    [Route("{id}/player-rankings")]
    public Task<PagedResultDto<PlayerRankingsDto>> GetPlayerRankingsAsync(Guid id, PlayerRankingsGetInput input)
    {
        return _activityAppService.GetPlayerRankingsAsync(id, input);
    }

    [HttpGet]
    [Route("{id}/lookup")]
    public Task<ListResultDto<PlayerDto>> LookupAsync(Guid id, PlayerLookupInput input)
    {
        return _activityAppService.LookupAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/player/{playerId}/vote")]
    public Task VoteAsync(Guid id, Guid playerId)
    {
        return _activityAppService.VoteAsync(id, playerId);
    }
}
