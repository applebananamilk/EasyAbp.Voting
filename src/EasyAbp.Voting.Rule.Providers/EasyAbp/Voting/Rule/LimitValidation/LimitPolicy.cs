namespace EasyAbp.Voting.Rule.LimitValidation;

/// <summary>
/// 限制策略
/// </summary>
public enum LimitPolicy
{
    /// <summary>
    /// 按分钟限制
    /// </summary>
    Seconds = 0,

    /// <summary>
    /// 按分钟限制
    /// </summary>
    Minutes = 1,

    /// <summary>
    /// 按小时限制
    /// </summary>
    Hours = 2,

    /// <summary>
    /// 按天限制
    /// </summary>
    Days = 3,
}