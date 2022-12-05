using System;
using System.Collections.Generic;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则缓存项
/// </summary>
[Serializable]
public class RuleCacheItem
{
    /// <summary>
    /// 规则定义列表
    /// </summary>
    public List<RuleDefinitionWithOnOff> Rules { get; set; }

    public RuleCacheItem()
    {
        Rules = new List<RuleDefinitionWithOnOff>();
    }

    public RuleCacheItem(List<RuleDefinitionWithOnOff> rules)
    {
        Rules = rules;
    }

    public static string CalculateCacheKey(Guid activityId)
    {
        return $"Voting:Activity:{activityId}:Rule";
    }
}
