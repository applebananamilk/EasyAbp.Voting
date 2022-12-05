using Volo.Abp.Localization;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则定义上下文
/// </summary>
public interface IRuleDefinitionContext
{
    /// <summary>
    /// 添加规则定义
    /// </summary>
    /// <param name="name">规则名称Key</param>
    /// <param name="displayName">显示名称</param>
    /// <param name="description">描述</param>
    /// <returns></returns>
    RuleDefinition AddDefinition(
        string name,
        ILocalizableString displayName = null,
        ILocalizableString description = null);
}
