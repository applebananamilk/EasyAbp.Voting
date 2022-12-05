using EasyAbp.Voting.Activities.Dtos;
using EasyAbp.Voting.Admin.Activities.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Admin.Activities;

[RemoteService(Name = VotingAdminRemoteServiceConsts.RemoteServiceName)]
[Area(VotingAdminRemoteServiceConsts.ModuleName)]
[Route("api/voting/admin/activities")]
[ControllerName("ActivityAdmin")]
public class ActivityController : VotingAdminController, IActivityAppService
{
    private readonly IActivityAppService _activityAppService;

    public ActivityController(IActivityAppService activityAppService)
    {
        _activityAppService = activityAppService;
    }

    [HttpPost]
    public Task<ActivityDto> CreateAsync(ActivityCreateInput input)
    {
        return _activityAppService.CreateAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public Task<ActivityDto> GetAsync(Guid id)
    {
        return _activityAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<ActivityGetListDto>> GetListAsync(ActivityGetListInput input)
    {
        return _activityAppService.GetListAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public Task<ActivityDto> UpdateAsync(Guid id, ActivityUpdateInput input)
    {
        return _activityAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _activityAppService.DeleteAsync(id);
    }

    [HttpPost]
    [Route("{id}/publish")]
    public Task PublishAsync(Guid id)
    {
        return _activityAppService.PublishAsync(id);
    }

    [HttpPost]
    [Route("{id}/unpublish")]
    public Task UnpublishAsync(Guid id)
    {
        return _activityAppService.UnpublishAsync(id);
    }

    [HttpPost]
    [Route("{id}/groups")]
    public Task<ActivityDto> AddGroupAsync(Guid id, GroupCreateUpdateInput input)
    {
        return _activityAppService.AddGroupAsync(id, input);
    }

    [HttpPut]
    [Route("{id}/groups/{groupId}")]
    public Task<ActivityDto> UpdateGroupAsync(Guid id, Guid groupId, GroupCreateUpdateInput input)
    {
        return _activityAppService.UpdateGroupAsync(id, groupId, input);
    }

    [HttpDelete]
    [Route("{id}/groups/{groupId}")]
    public Task<ActivityDto> DeleteGroupAsync(Guid id, Guid groupId)
    {
        return _activityAppService.DeleteGroupAsync(id, groupId);
    }

    [HttpPost]
    [Route("{id}/banners")]
    public Task<ActivityDto> AddBannerAsync(Guid id, BannerCreateUpdateInput input)
    {
        return _activityAppService.AddBannerAsync(id, input);
    }

    [HttpPut]
    [Route("{id}/banners/{bannerId}")]
    public Task<ActivityDto> UpdateBannerAsync(Guid id, Guid bannerId, BannerCreateUpdateInput input)
    {
        return _activityAppService.UpdateBannerAsync(id, bannerId, input);
    }

    [HttpDelete]
    [Route("{id}/banners/{bannerId}")]
    public Task<ActivityDto> DeleteBannerAsync(Guid id, Guid bannerId)
    {
        return _activityAppService.DeleteBannerAsync(id, bannerId);
    }
}
