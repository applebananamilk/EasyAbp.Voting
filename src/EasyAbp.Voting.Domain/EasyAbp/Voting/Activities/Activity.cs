using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace EasyAbp.Voting.Activities;

public class Activity : FullAuditedAggregateRoot<Guid>
{
    public virtual string ActivityName { get; protected set; }

    public virtual DateTime ActivityStartTime { get; protected set; }

    public virtual DateTime ActivityEndTime { get; protected set; }

    public virtual string Introduction { get; set; }

    public virtual string BackgroundMusic { get; set; }

    public virtual string CoverImage { get; protected set; }

    /// <summary>
    /// 票数单位名称（例如：票、魅力值）
    /// </summary>
    public virtual string VotesUnit { get; protected set; }

    public virtual string ContactUs { get; set; }

    public virtual bool IsDraft { get; protected set; }

    public virtual long Views { get; protected set; }

    public virtual string FormContent { get; protected set; }

    public virtual ICollection<Group> Groups { get; protected set; }

    public virtual ICollection<Banner> Banners { get; protected set; }

    protected Activity()
    {
    }

    public Activity(
        Guid id,
        string activityName,
        DateTime activityStartTime,
        DateTime activityEndTime,
        string coverImage,
        string votesUnit)
        : base(id)
    {
        SetActivityName(activityName);
        SetActivityTime(activityStartTime, activityEndTime);
        SetCoverImage(coverImage);
        SetVotesUnit(votesUnit);
        Publish(false);

        Groups = new Collection<Group>();
        Banners = new Collection<Banner>();
    }

    public virtual void SetActivityName(string activityName)
    {
        ActivityName = Check.NotNullOrWhiteSpace(activityName, nameof(activityName), ActivityConsts.MaxActivityNameLength);
    }

    public virtual void SetCoverImage(string coverImage)
    {
        CoverImage = Check.NotNullOrWhiteSpace(coverImage, nameof(coverImage));
    }

    public virtual void SetVotesUnit(string votesUnit)
    {
        VotesUnit = Check.NotNullOrWhiteSpace(votesUnit, nameof(votesUnit), ActivityConsts.MaxVotesUnitLength);
    }

    public virtual void SetActivityTime(DateTime startTime, DateTime endTime)
    {
        if (startTime > endTime)
        {
            throw new BusinessException(VotingErrorCodes.ActivityStartTimeNotGtActivityEndTime);
        }

        ActivityStartTime = startTime;
        ActivityEndTime = endTime;
    }

    internal virtual void SetViews(long views)
    {
        if (views < 0)
        {
            throw new ArgumentException("views < 0", nameof(views));
        }

        Views = views;
    }

    public virtual void SetFormContent(string formContent)
    {
        if (formContent.IsNullOrWhiteSpace())
        {
            formContent = ActivityConsts.DefaultFormContent;
        }

        FormContent = formContent;
    }

    public virtual void AddGroup(Guid groupId, string groupName, string description = null)
    {
        Groups.Add(new Group(groupId, Id, groupName, description));
    }

    public virtual Group GetGroup(Guid groupId)
    {
        var group = Groups.SingleOrDefault(p => p.Id == groupId);

        if (group is null)
        {
            throw new EntityNotFoundException(typeof(Group), groupId);
        }

        return group;
    }

    public virtual void UpdateGroup(Guid groupId, string name, string description = null)
    {
        var group = GetGroup(groupId);

        group.SetName(name);
        group.SetDescription(description);
    }

    public virtual void RemoveGroup(Guid groupId)
    {
        var group = GetGroup(groupId);

        Groups.Remove(group);
    }

    public virtual void AddBanner(Guid bannerId, string url, string link = null)
    {
        Banners.Add(new Banner(bannerId, Id, url, link));
    }

    public virtual Banner GetBanner(Guid bannerId)
    {
        var banner = Banners.SingleOrDefault(p => p.Id == bannerId);

        if (banner is null)
        {
            throw new EntityNotFoundException(typeof(Banner), bannerId);
        }

        return banner;
    }

    public virtual void UpdateBanner(Guid bannerId, string url, string link = null)
    {
        var banner = GetBanner(bannerId);

        banner.SetUrl(url);
        banner.SetLink(link);
    }

    public virtual void RemoveBanner(Guid bannerId)
    {
        var banner = GetBanner(bannerId);

        Banners.Remove(banner);
    }

    public void Publish(bool isPublish = true)
    {
        IsDraft = !isPublish;
    }
}
