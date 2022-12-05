using System.Threading.Tasks;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则验证提供者接口
/// </summary>
public interface IRuleValidationProvider
{
    /// <summary>
    /// 名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 验证
    /// </summary>
    /// <param name="context">规则上下文</param>
    /// <returns></returns>
    Task ValidateAsync(RuleValidationContext context);
}
