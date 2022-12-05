using System.Collections.Generic;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则定义管理
/// </summary>
public interface IRuleDefinitionManager
{
    /// <summary>
    /// 获取所有规则定义
    /// </summary>
    /// <returns></returns>
    IReadOnlyList<RuleDefinition> GetAll();

    /// <summary>
    /// 获取指定规则定义
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    RuleDefinition Get(string name);
}
