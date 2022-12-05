using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace EasyAbp.Voting.Activities;

public class Banner : Entity<Guid>
{
    public virtual Guid ActivityId { get; protected set; }

    public virtual string Url { get; protected set; }

    public virtual string Link { get; protected set; }

    protected Banner()
    {

    }

    public Banner(
        Guid id,
        Guid activityId,
        string url,
        string link = null)
        : base(id)
    {
        ActivityId = activityId;
        SetUrl(url);
        SetLink(link);
    }

    public virtual void SetUrl(string url)
    {
        Url = Check.NotNullOrWhiteSpace(url, nameof(url), BannerConsts.MaxUrlLength);
    }

    public virtual void SetLink(string link)
    {
        Link = Check.Length(link, nameof(link), BannerConsts.MaxLinkLength);
    }
}
