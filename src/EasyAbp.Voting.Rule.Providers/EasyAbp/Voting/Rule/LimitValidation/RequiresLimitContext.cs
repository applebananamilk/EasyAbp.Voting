using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Timing;

namespace EasyAbp.Voting.Rule.LimitValidation;

public class RequiresLimitContext
{
    /// <summary>
    /// 功能限制Key。
    /// </summary>
    public string LimitKey { get; }

    /// <summary>
    /// 限制时长
    /// </summary>
    public int Interval { get; }

    /// <summary>
    /// 功能限制时间策略
    /// </summary>
    public LimitPolicy Policy { get; }

    /// <summary>
    /// 限制数量
    /// </summary>
    public int Limit { get; }

    /// <summary>
    /// 增加数量
    /// </summary>
    public int Increments { get; }

    public List<Func<Task>> FailedHandlers { get; } = new();

    /// <summary>
    /// 启用每天重置
    /// </summary>
    private bool _enableDayReset;

    private RequiresLimitContext(
        string limitKey,
        int interval,
        LimitPolicy limitPolicy,
        int limit,
        int increments)
    {
        LimitKey = limitKey;
        Interval = interval;
        Policy = limitPolicy;
        Limit = limit;
        Increments = increments;
    }

    /// <summary>
    /// 创建限制上下文
    /// </summary>
    /// <param name="limitKey">唯一Key</param>
    /// <param name="interval">限制时间间隔</param>
    /// <param name="limitPolicy">限制时间策略</param>
    /// <param name="limit">限量</param>
    /// <param name="increments">此次增量</param>
    /// <remarks>
    /// 例如：<br/>
    /// 每1(interval)分钟(limitPolicy)，最多投5(limit)票。本次投3(increments)票。<br/>
    /// 如果投票成功利用唯一Key记录加3(increments)，投票失败调用OnFailed。<br/>
    /// </remarks>
    public static RequiresLimitContext Create(
        string limitKey,
        int interval,
        LimitPolicy limitPolicy,
        int limit,
        int increments)
    {
        Check.NotNullOrWhiteSpace(limitKey, nameof(limitKey));
        Check.Range(interval, nameof(interval), 1);
        Check.Range(limit, nameof(limit), 1);
        Check.Range(increments, nameof(increments), 1);

        return new RequiresLimitContext(limitKey, interval, limitPolicy, limit, increments);
    }

    /// <summary>
    /// 启用每天重置之后，限制时间最大不能超过0点。
    /// </summary>
    public RequiresLimitContext EnableDayReset(bool enableDayReset)
    {
        _enableDayReset = enableDayReset;
        return this;
    }

    public TimeSpan GetEffectTimeSpan(IClock clock)
    {
        var now = clock.Now;
        return Policy switch
        {
            LimitPolicy.Seconds => CalculateExpiry(now, now.AddSeconds(Interval)) - now,
            LimitPolicy.Minutes => CalculateExpiry(now, now.AddMinutes(Interval)) - now,
            LimitPolicy.Hours => CalculateExpiry(now, now.AddHours(Interval)) - now,
            LimitPolicy.Days => CalculateExpiry(now, now.AddDays(Interval)) - now,
            _ => throw new NotImplementedException(),
        };
    }

    private DateTime CalculateExpiry(DateTime now, DateTime interval)
    {
        if (!_enableDayReset)
        {
            return interval;
        }

        var tomorrow = now.AddDays(1);
        tomorrow = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0);
        return interval > tomorrow ? tomorrow : interval;
    }

    public RequiresLimitContext OnFailed(Func<Task> handler)
    {
        FailedHandlers.Add(handler);
        return this;
    }
}