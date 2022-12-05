using System;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities.Auditing;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则
/// </summary>
public class Rule : AuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 活动Id
    /// </summary>
    public virtual Guid ActivityId { get; protected set; }

    /// <summary>
    /// 规则唯一名称。
    /// </summary>
    public virtual string Name { get; protected set; }

    /// <summary>
    /// 该规则是否启用，默认为false。
    /// </summary>
    public virtual bool IsEnabled { get; set; }

    /// <summary>
    /// 扩展属性，用于存储规则设置。
    /// </summary>
    /// <remarks>此处重写只为显示出此属性。</remarks>
    public override ExtraPropertyDictionary ExtraProperties
    {
        get => base.ExtraProperties;
        protected set => base.ExtraProperties = value;
    }

    protected Rule()
    {
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public Rule(
        Guid id,
        Guid activityId,
        string name,
        bool isEnabled) : base(id)
    {
        ActivityId = activityId;
        Name = name;
        IsEnabled = isEnabled;
    }
}
