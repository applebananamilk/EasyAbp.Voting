using EasyAbp.Voting.Activities;
using EasyAbp.Voting.Activities.Dtos;
using EasyAbp.Voting.Admin.Activities.Dtos;
using EasyAbp.Voting.Admin.Permissions;
using EasyAbp.Voting.Players;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.Voting.Admin.Activities;

public class ActivityAppService : CrudAppService<
    Activity,
    ActivityDto,
    ActivityGetListDto,
    Guid,
    ActivityGetListInput,
    ActivityCreateInput,
    ActivityUpdateInput>, IActivityAppService
{
    protected override string GetPolicyName => VotingAdminPermissions.Activities.Default;
    protected override string GetListPolicyName => VotingAdminPermissions.Activities.Default;
    protected override string CreatePolicyName => VotingAdminPermissions.Activities.Create;
    protected override string UpdatePolicyName => VotingAdminPermissions.Activities.Update;
    protected override string DeletePolicyName => VotingAdminPermissions.Activities.Delete;

    protected IActivityRepository ActivityRepository { get; }

    public ActivityAppService(IActivityRepository activityRepository) : base(activityRepository)
    {
        ActivityRepository = activityRepository;
    }

    public override async Task<ActivityDto> CreateAsync(ActivityCreateInput input)
    {
        await CheckCreatePolicyAsync();

        var activity = new Activity(
            GuidGenerator.Create(),
            input.ActivityName,
            Clock.Normalize(input.ActivityStartTime),
            Clock.Normalize(input.ActivityEndTime),
            input.CoverImage,
            input.VotesUnit);

        await ActivityRepository.InsertAsync(activity, autoSave: true);

        return await MapToGetOutputDtoAsync(activity);
    }

    protected override async Task<IQueryable<Activity>> CreateFilteredQueryAsync(ActivityGetListInput input)
    {
        return (await base.CreateFilteredQueryAsync(input))
            .WhereIf(!input.ActivityName.IsNullOrWhiteSpace(), p => p.ActivityName.Contains(input.ActivityName))
            .WhereIf(input.ActivityStartTime.HasValue, p => p.ActivityStartTime >= input.ActivityStartTime.Value)
            .WhereIf(input.ActivityEndTime.HasValue, p => p.ActivityEndTime <= input.ActivityEndTime.Value)
            .WhereIf(input.IsDraft.HasValue, p => p.IsDraft == input.IsDraft.Value);
    }

    public override async Task<ActivityDto> UpdateAsync(Guid id, ActivityUpdateInput input)
    {
        await CheckUpdatePolicyAsync();

        var activity = await ActivityRepository.GetAsync(id);

        activity.SetActivityName(input.ActivityName);
        activity.SetActivityTime(Clock.Normalize(input.ActivityStartTime), Clock.Normalize(input.ActivityEndTime));
        activity.Introduction = input.Introduction;
        activity.BackgroundMusic = input.BackgroundMusic;
        activity.SetCoverImage(input.CoverImage);
        activity.SetVotesUnit(input.VotesUnit);
        activity.ContactUs = input.ContactUs;
        activity.SetFormContent(input.FormContent);

        await ActivityRepository.UpdateAsync(activity, autoSave: true);

        return await MapToGetOutputDtoAsync(activity);
    }

    [Authorize(VotingAdminPermissions.Activities.Publish)]
    public virtual async Task PublishAsync(Guid id)
    {
        var activity = await ActivityRepository.GetAsync(id);

        activity.Publish();

        await ActivityRepository.UpdateAsync(activity, autoSave: true);
    }

    [Authorize(VotingAdminPermissions.Activities.Publish)]
    public virtual async Task UnpublishAsync(Guid id)
    {
        var activity = await ActivityRepository.GetAsync(id);

        activity.Publish(false);

        await ActivityRepository.UpdateAsync(activity, autoSave: true);
    }

    [Authorize(VotingAdminPermissions.Activities.Update)]
    public async Task<ActivityDto> AddGroupAsync(Guid id, GroupCreateUpdateInput input)
    {
        var activity = await ActivityRepository.GetAsync(id);

        activity.AddGroup(
            GuidGenerator.Create(),
            input.Name,
            input.Description);

        await ActivityRepository.UpdateAsync(activity, autoSave: true);

        return await MapToGetOutputDtoAsync(activity);
    }

    [Authorize(VotingAdminPermissions.Activities.Update)]
    public virtual async Task<ActivityDto> UpdateGroupAsync(Guid id, Guid groupId, GroupCreateUpdateInput input)
    {
        var activity = await ActivityRepository.GetAsync(id);

        activity.UpdateGroup(groupId, input.Name, input.Description);

        await ActivityRepository.UpdateAsync(activity, autoSave: true);

        return await MapToGetOutputDtoAsync(activity);
    }

    [Authorize(VotingAdminPermissions.Activities.Update)]
    public virtual async Task<ActivityDto> DeleteGroupAsync(Guid id, Guid groupId)
    {
        var activity = await ActivityRepository.GetAsync(id);

        await CheckGroupHasPlayerAsync(id, groupId);

        activity.RemoveGroup(groupId);

        await ActivityRepository.UpdateAsync(activity, autoSave: true);

        return await MapToGetOutputDtoAsync(activity);
    }

    protected virtual async Task CheckGroupHasPlayerAsync(Guid id, Guid groupId)
    {
        var playerRepository = LazyServiceProvider.LazyGetRequiredService<IPlayerRepository>();

        if (await playerRepository.AnyAsync(p => p.ActivityId == id && p.GroupId == groupId))
        {
            throw new BusinessException(VotingErrorCodes.GroupCannotDelete);
        }
    }

    [Authorize(VotingAdminPermissions.Activities.Update)]
    public virtual async Task<ActivityDto> AddBannerAsync(Guid id, BannerCreateUpdateInput input)
    {
        var activity = await ActivityRepository.GetAsync(id);

        activity.AddBanner(
            GuidGenerator.Create(),
            input.Url,
            input.Link);

        await ActivityRepository.UpdateAsync(activity, autoSave: true);

        return await MapToGetOutputDtoAsync(activity);
    }

    [Authorize(VotingAdminPermissions.Activities.Update)]
    public virtual async Task<ActivityDto> UpdateBannerAsync(Guid id, Guid bannerId, BannerCreateUpdateInput input)
    {
        var activity = await ActivityRepository.GetAsync(id);

        activity.UpdateBanner(bannerId, input.Url, input.Link);

        await ActivityRepository.UpdateAsync(activity, autoSave: true);

        return await MapToGetOutputDtoAsync(activity);
    }

    [Authorize(VotingAdminPermissions.Activities.Update)]
    public virtual async Task<ActivityDto> DeleteBannerAsync(Guid id, Guid bannerId)
    {
        var activity = await ActivityRepository.GetAsync(id);

        activity.RemoveBanner(bannerId);

        await ActivityRepository.UpdateAsync(activity, autoSave: true);

        return await MapToGetOutputDtoAsync(activity);
    }
}
