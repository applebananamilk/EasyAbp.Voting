using System;
using Volo.Abp;
using Volo.Abp.Data;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则验证上下文
/// </summary>
public class RuleValidationContext : IHasExtraProperties
{
    /// <summary>
    /// 活动Id
    /// </summary>
    public Guid ActivityId { get; }

    /// <summary>
    /// 当前选手Id
    /// </summary>
    public Guid PlayerId { get; }

    /// <summary>
    /// 票数
    /// </summary>
    public int Votes { get; }

    /// <summary>
    /// 额外属性
    /// </summary>
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = new ExtraPropertyDictionary();

    internal RuleValidationContext(Guid activityId, Guid playerId, int votes)
    {
        ActivityId = activityId;
        PlayerId = playerId;
        Votes = votes;
    }

    /// <summary>
    /// 设置扩展属性
    /// </summary>
    /// <param name="externalExtraProperties"></param>
    /// <returns></returns>
    internal RuleValidationContext SetExtraProperties(ExtraPropertyDictionary externalExtraProperties)
    {
        foreach (var property in externalExtraProperties)
        {
            ExtraProperties[property.Key] = externalExtraProperties[property.Key];
        }

        return this;
    }

    /// <summary>
    /// 清除扩展属性
    /// </summary>
    /// <param name="externalExtraProperties"></param>
    /// <returns></returns>
    internal RuleValidationContext ClearExtraProperties(ExtraPropertyDictionary externalExtraProperties)
    {
        foreach (var property in externalExtraProperties)
        {
            ExtraProperties.Remove(property.Key);
        }

        return this;
    }

    /// <summary>
    /// 创建验证上下文
    /// </summary>
    /// <param name="activityId">活动Id</param>
    /// <param name="playerId">选手Id</param>
    /// <param name="votes">票数</param>
    /// <returns></returns>
    public static RuleValidationContext Create(Guid activityId, Guid playerId, int votes)
    {
        if (votes < 0)
        {
            throw new ArgumentException("votes < 0", nameof(votes));
        }

        return new RuleValidationContext(activityId, playerId, votes);
    }

    /// <summary>
    /// 设置扩展属性，用于<see cref="RuleValidationProvider"/>内部传参。
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public RuleValidationContext SetExtraProperty(string name, object value)
    {
        if (ExtraProperties.ContainsKey(name))
        {
            throw new ArgumentException($"已有额外属性：{name}");
        }

        ExtraProperties[name] = value;

        return this;
    }
}
