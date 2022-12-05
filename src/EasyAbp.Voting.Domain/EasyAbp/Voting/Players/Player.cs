using EasyAbp.Voting.Activities;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace EasyAbp.Voting.Players;

public class Player : FullAuditedAggregateRoot<Guid>
{
    public virtual Guid ActivityId { get; protected set; }

    public virtual Guid? GroupId { get; protected internal set; }

    public virtual string UserId { get; protected internal set; }

    public virtual int? PlayerNumber { get; protected internal set; }

    public virtual string Name { get; protected set; }

    public virtual string Avatar { get; protected set; }

    public virtual string CoverImage { get; protected set; }

    public virtual string FormContent { get; protected set; }

    // TODO 自行报名的扩展性
    public virtual Status Status { get; protected internal set; }

    public virtual string RejectReason { get; protected internal set; }

    public virtual PlayerVotes PlayerVotes { get; protected set; }

    protected Player()
    {
    }

    internal Player(
        Guid id,
        Guid activityId,
        Guid? groupId,
        string userId,
        string name,
        string avatar,
        string coverImage,
        string formContent)
        : base(id)
    {
        ActivityId = activityId;
        GroupId = groupId;
        UserId = userId;
        SetName(name);
        SetAvatar(avatar);
        SetCoverImage(coverImage);
        SetFormContent(formContent);

        Status = Status.PendingReview;
        PlayerVotes = new PlayerVotes(Id, 0);
    }

    public virtual void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name));
    }

    public virtual void SetAvatar(string avatar)
    {
        Avatar = Check.NotNullOrWhiteSpace(avatar, nameof(avatar));
    }

    public virtual void SetCoverImage(string coverImage)
    {
        CoverImage = Check.NotNullOrWhiteSpace(coverImage, nameof(coverImage));
    }

    public virtual void SetFormContent(string formContent)
    {
        if (formContent.IsNullOrWhiteSpace())
        {
            formContent = ActivityConsts.DefaultFormContent;
        }

        FormContent = formContent;
    }
}