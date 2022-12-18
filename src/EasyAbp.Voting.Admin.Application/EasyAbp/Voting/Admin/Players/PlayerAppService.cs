using EasyAbp.Voting.Activities;
using EasyAbp.Voting.Admin.Permissions;
using EasyAbp.Voting.Admin.Players.Dtos;
using EasyAbp.Voting.Players;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectExtending;

namespace EasyAbp.Voting.Admin.Players;

public class PlayerAppService : CrudAppService<
    Player,
    PlayerDto,
    Guid,
    PlayerGetListInput,
    PlayerCreateInput,
    PlayerUpdateInput>, IPlayerAppService
{
    protected override string GetPolicyName => VotingAdminPermissions.Activities.Default;
    protected override string GetListPolicyName => VotingAdminPermissions.Activities.Default;
    protected override string CreatePolicyName => VotingAdminPermissions.Players.Create;
    protected override string UpdatePolicyName => VotingAdminPermissions.Players.Update;
    protected override string DeletePolicyName => VotingAdminPermissions.Players.Delete;

    protected IActivityRepository ActivityRepository { get; }
    protected PlayerManager PlayerManager { get; }
    protected IPlayerRepository PlayerRepository { get; }

    public PlayerAppService(
        IActivityRepository activityRepository,
        PlayerManager playerManager,
        IPlayerRepository playerRepository) : base(playerRepository)
    {
        ActivityRepository = activityRepository;
        PlayerManager = playerManager;
        PlayerRepository = playerRepository;
    }

    public override async Task<PlayerDto> CreateAsync(PlayerCreateInput input)
    {
        var activity = await ActivityRepository.GetAsync(input.ActivityId);

        var player = await PlayerManager.CreateAsync(
            activity,
            input.GroupId,
            input.UserId,
            input.Name,
            input.Avatar,
            input.CoverImage,
            input.FormContent);

        input.MapExtraPropertiesTo(player);

        await PlayerRepository.InsertAsync(player, true);

        return await MapToGetOutputDtoAsync(player);
    }

    protected override async Task<IQueryable<Player>> CreateFilteredQueryAsync(PlayerGetListInput input)
    {
        return (await base.CreateFilteredQueryAsync(input))
            .Where(p => p.ActivityId == input.ActivityId)
            .WhereIf(input.GroupId.HasValue, p => p.GroupId == input.GroupId.Value)
            .WhereIf(input.PlayerNumber.HasValue, p => p.PlayerNumber == input.PlayerNumber.Value)
            .WhereIf(!input.Name.IsNullOrWhiteSpace(), p => p.Name.Contains(input.Name))
            .WhereIf(input.Status.HasValue, p => p.Status == input.Status.Value);
    }

    public override async Task<PlayerDto> UpdateAsync(Guid id, PlayerUpdateInput input)
    {
        var player = await PlayerRepository.GetAsync(id);

        if (player.UserId != input.UserId)
        {
            await PlayerManager.ChangeUserIdAsync(player, input.UserId);
        }

        if (input.GroupId.HasValue && player.GroupId != input.GroupId)
        {
            await PlayerManager.ChangeGroupAsync(player, input.GroupId.Value);
        }

        player.SetName(input.Name);
        player.SetAvatar(input.Avatar);
        player.SetCoverImage(input.CoverImage);
        player.SetFormContent(input.FormContent);

        input.MapExtraPropertiesTo(player);

        await PlayerRepository.UpdateAsync(player, true);

        return await MapToGetOutputDtoAsync(player);
    }

    [Authorize(VotingAdminPermissions.Players.Review)]
    public virtual async Task ApproveAsync(Guid id)
    {
        var player = await PlayerRepository.GetAsync(id);
        await PlayerManager.ApproveAsync(player);
        await PlayerRepository.UpdateAsync(player, true);
    }

    [Authorize(VotingAdminPermissions.Players.Review)]
    public virtual async Task RejectAsync(Guid id, PlayerRejectInput input)
    {
        var player = await PlayerRepository.GetAsync(id);
        await PlayerManager.RejectAsync(player, input.RejectReason);
        await PlayerRepository.UpdateAsync(player, true);
    }
}
