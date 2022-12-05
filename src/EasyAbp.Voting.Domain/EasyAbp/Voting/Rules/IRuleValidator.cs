using System.Threading.Tasks;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则验证
/// </summary>
public interface IRuleValidator
{
    /// <summary>
    /// 用于验证此次投票是否符合所有已经启用的规则。
    /// </summary>
    /// <param name="ruleValidationContext">验证规则上下文</param>
    /// <returns></returns>
    Task ValidateAsync(RuleValidationContext ruleValidationContext);
}
