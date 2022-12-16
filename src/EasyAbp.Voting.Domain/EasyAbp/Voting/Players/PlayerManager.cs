using EasyAbp.Voting.Activities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Uow;

namespace EasyAbp.Voting.Players;

public class PlayerManager : DomainService
{
    protected IPlayerRepository PlayerRepository { get; }
    protected IAbpDistributedLock DistributedLock { get; }
    protected IUnitOfWorkManager UnitOfWorkManager { get; }
    protected ILocalEventBus LocalEventBus { get; }
    protected IPlayerNumberGenerator PlayerNumberGenerator { get; }

    public PlayerManager(
        IPlayerRepository playerRepository,
        IAbpDistributedLock distributedLock,
        IUnitOfWorkManager unitOfWorkManager,
        ILocalEventBus localEventBus,
        IPlayerNumberGenerator playerNumberGenerator)
    {
        PlayerRepository = playerRepository;
        DistributedLock = distributedLock;
        UnitOfWorkManager = unitOfWorkManager;
        LocalEventBus = localEventBus;
        PlayerNumberGenerator = playerNumberGenerator;
    }

    public virtual async Task<Player> CreateAsync(
        Activity activity,
        Guid? groupId,
        string userId,
        string name,
        string avatar,
        string coverImage,
        string formContent = null)
    {
        if (!userId.IsNullOrWhiteSpace() && await IsInActivityAsync(activity.Id, userId))
        {
            throw new BusinessException(VotingErrorCodes.PlayerAlreadyExists);
        }

        if (activity.Groups.Any() && !groupId.HasValue)
        {
            throw new BusinessException(VotingErrorCodes.GroupNotSelected);
        }

        if (groupId.HasValue)
        {
            await ValidateGroupAsync(activity, groupId.Value);
        }

        return new Player(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            activity.Id,
            groupId,
            userId,
            name,
            avatar,
            coverImage,
            formContent);
    }

    public virtual Task<bool> IsInActivityAsync(Guid activityId, string userId)
    {
        Check.NotNullOrWhiteSpace(userId, nameof(userId));

        return PlayerRepository.AnyAsync(p => p.ActivityId == activityId && p.UserId == userId);
    }

    protected virtual Task<bool> ValidateGroupAsync(Activity activity, Guid groupId, bool throwOnVerifyFail = true)
    {
        var verificationResult = activity.Groups.Any(p => p.Id == groupId);

        if (!verificationResult && throwOnVerifyFail)
        {
            throw new BusinessException(VotingErrorCodes.InvalidGroup)
                .WithData(nameof(groupId), groupId);
        }

        return Task.FromResult(verificationResult);
    }

    public virtual async Task ChangeGroupAsync(Player player, Guid groupId)
    {
        var activityRepository = LazyServiceProvider.LazyGetRequiredService<IActivityRepository>();

        var activity = await activityRepository.GetAsync(player.ActivityId);

        await ValidateGroupAsync(activity, groupId);

        player.GroupId = groupId;

        // 本地事件能够保证执行顺序
        await LocalEventBus.PublishAsync(new PlayerCacheRemoveEto(player.ActivityId, player.Id));
        await LocalEventBus.PublishAsync(new PlayerCacheAddEto(player.ActivityId, player.Id));
    }

    public virtual async Task ChangeUserIdAsync(Player player, string userId)
    {
        if (userId.IsNullOrWhiteSpace())
        {
            if (await IsInActivityAsync(player.ActivityId, userId))
            {
                throw new BusinessException(VotingErrorCodes.PlayerAlreadyExists);
            }
        }

        player.UserId = userId;
    }

    public virtual async Task ApproveAsync(Player player)
    {
        player.Status = Status.Approved;
        player.RejectReason = null;

        if (!player.PlayerNumber.HasValue)
        {
            await AssignPlayerNumberAsync(player);
        }

        await LocalEventBus.PublishAsync(new PlayerCacheAddEto(player.ActivityId, player.Id));
    }

    protected virtual async Task AssignPlayerNumberAsync(Player player)
    {
        if (!player.PlayerNumber.HasValue)
        {
            await using (var handle = await DistributedLock.TryAcquireAsync(
                CalculateAssignPlayerNumberLockName(player.ActivityId),
                TimeSpan.FromSeconds(3)))
            {
                if (handle != null)
                {
                    using (var uow = UnitOfWorkManager.Begin(requiresNew: true))
                    {
                        player.PlayerNumber = await PlayerNumberGenerator.GenerateAsync(player.ActivityId);

                        await PlayerRepository.UpdateAsync(player);
                        await uow.CompleteAsync();
                    }
                }
                else
                {
                    throw new BusinessException(VotingErrorCodes.AssignPlayerNumberTimeout);
                }
            }
        }
    }

    public virtual async Task RejectAsync(Player player, string rejectReason)
    {
        if (rejectReason.IsNullOrWhiteSpace())
        {
            throw new BusinessException(VotingErrorCodes.RejectReasonNotNull);
        }

        player.Status = Status.Rejected;
        player.RejectReason = rejectReason;

        await LocalEventBus.PublishAsync(new PlayerCacheRemoveEto(player.ActivityId, player.Id));
    }

    protected virtual string CalculateAssignPlayerNumberLockName(Guid activityId)
    {
        return $"Voting:Lock:Activity:{activityId}:AssignPlayerNumber";
    }
}
