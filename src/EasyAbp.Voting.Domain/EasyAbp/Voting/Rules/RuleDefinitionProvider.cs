using Volo.Abp.DependencyInjection;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则定义提供者
/// </summary>
public abstract class RuleDefinitionProvider : IRuleDefinitionProvider, ITransientDependency
{
    /// <inheritdoc/>
    public abstract void Define(IRuleDefinitionContext context);
}
