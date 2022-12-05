using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Localization;

namespace EasyAbp.Voting.Rules;

public class RuleDefinitionContext : IRuleDefinitionContext
{
    /// <summary>
    /// 规则定义
    /// </summary>
    public IReadOnlyList<RuleDefinition> RuleDefinitions => _ruleDefinitions.ToImmutableList();
    private readonly List<RuleDefinition> _ruleDefinitions;

    public RuleDefinitionContext()
    {
        _ruleDefinitions = new List<RuleDefinition>();
    }

    public RuleDefinition AddDefinition(
        string name,
        ILocalizableString displayName = null,
        ILocalizableString description = null)
    {
        if (_ruleDefinitions.Any(p => p.Name == name))
        {
            throw new AbpException("已经存在一个相同名称的规则: " + name);
        }

        var rule = new RuleDefinition(
           name,
           displayName,
           description);

        _ruleDefinitions.Add(rule);
        return rule;
    }
}
