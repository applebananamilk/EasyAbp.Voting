using System.Threading.Tasks;

namespace EasyAbp.Voting.Rule.LimitValidation;

/// <summary>
/// 功能限制使用检测器。
/// </summary>
/// <remarks>用于在规定时间内，不能超过指定Limit的次数，数量等某一指标。</remarks>
public interface IRequiresLimitChecker
{
    Task ProcessAsync(RequiresLimitContext context);
}
