namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则定义提供者
/// </summary>
public interface IRuleDefinitionProvider
{
    /// <summary>
    /// 规则定义
    /// </summary>
    /// <param name="context">上下文</param>
    void Define(IRuleDefinitionContext context);
}
