using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace EasyAbp.Voting.Rules;

public class RuleValidator : IRuleValidator, ITransientDependency
{
    protected ILogger<RuleValidator> Logger { get; }
    protected RuleManager RuleManager { get; }
    protected IDistributedCache<RuleCacheItem> DistributedCache { get; }
    protected IRuleValidationProviderManager ValidationProviderManager { get; }
    protected IClock Clock { get; }

    public RuleValidator(
        ILogger<RuleValidator> logger,
        RuleManager ruleManager,
        IDistributedCache<RuleCacheItem> distributedCache,
        IRuleValidationProviderManager validationProviderManager,
        IClock clock)
    {
        Logger = logger;
        RuleManager = ruleManager;
        DistributedCache = distributedCache;
        ValidationProviderManager = validationProviderManager;
        Clock = clock;
    }

    public virtual async Task ValidateAsync(RuleValidationContext ruleValidationContext)
    {
        var ruleCacheItem = await DistributedCache.GetOrAddAsync(
            RuleCacheItem.CalculateCacheKey(ruleValidationContext.ActivityId),
            async () =>
            {
                var rules = await RuleManager.GetRuleListAsync(ruleValidationContext.ActivityId);
                return new RuleCacheItem(rules);
            },
            () => new DistributedCacheEntryOptions { AbsoluteExpiration = Clock.Now.AddDays(1) });

        var providers = ValidationProviderManager.Providers;

        var stopwatch = Stopwatch.StartNew();

        foreach (var rule in ruleCacheItem.Rules)
        {
            if (rule.IsEnabled)
            {
                var provider = providers.Find(p => p.Name == rule.Name);

                if (provider != null)
                {
                    stopwatch.Start();

                    ruleValidationContext.SetExtraProperties(rule.ExtraProperties);
                    await provider.ValidateAsync(ruleValidationContext);
                    ruleValidationContext.ClearExtraProperties(rule.ExtraProperties);

                    stopwatch.Stop();
                    Logger.LogDebug($"执行规则：{provider.Name}，耗时：{stopwatch.Elapsed.TotalMilliseconds} ms！");
                    stopwatch.Reset();
                }
            }
        }
    }
}
