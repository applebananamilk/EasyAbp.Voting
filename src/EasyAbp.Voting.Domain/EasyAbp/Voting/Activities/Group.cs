using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace EasyAbp.Voting.Activities;

public class Group : Entity<Guid>
{
    public virtual Guid ActivityId { get; protected set; }

    public virtual string Name { get; protected set; }

    public virtual string Description { get; protected set; }

    protected Group()
    {

    }

    public Group(
        Guid id,
        Guid activityId,
        string name,
        string description = null)
        : base(id)
    {
        ActivityId = activityId;
        SetName(name);
        SetDescription(description);
    }

    public virtual void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), GroupConsts.MaxNameLength);
    }

    public virtual void SetDescription(string description)
    {
        Description = Check.Length(description, nameof(description), GroupConsts.MaxDescriptionLength);
    }
}
