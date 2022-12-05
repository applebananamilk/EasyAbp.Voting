using Volo.Abp.Collections;

namespace EasyAbp.Voting.Rules;

public class RuleOptions
{
    /// <summary>
    /// 规则定义集合
    /// </summary>
    public ITypeList<IRuleDefinitionProvider> DefinitionProviders { get; }

    /// <summary>
    /// 规则验证集合
    /// </summary>
    public ITypeList<IRuleValidationProvider> ValidationProviders { get; }

    public RuleOptions()
    {
        DefinitionProviders = new TypeList<IRuleDefinitionProvider>();
        ValidationProviders = new TypeList<IRuleValidationProvider>();
    }
}
