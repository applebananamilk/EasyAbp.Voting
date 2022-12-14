using EasyAbp.Voting.Caching;
using StackExchange.Redis;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace EasyAbp.Voting.Rule.LimitValidation;

// TODO 不是精准的控制。因为命令是分开的。如果要精准。需要使用Lua脚本。
public class RedisRequiresLimitChecker : IRequiresLimitChecker, ISingletonDependency
{
    protected IDatabase Redis { get; }
    protected IRedisCacheKeyNormalizer KeyNormalizer { get; }
    protected IClock Clock { get; }

    public RedisRequiresLimitChecker(
        IRedisDatabaseAccessor redisDatabaseAccessor,
        IRedisCacheKeyNormalizer keyNormalizer,
        IClock clock)
    {
        Redis = redisDatabaseAccessor.RedisDatabase;
        KeyNormalizer = keyNormalizer;
        Clock = clock;
    }

    public async Task ProcessAsync(RequiresLimitContext context)
    {
        if (await InternalCheckAsync(context))
        {
            await InternalProcessAsync(context);
        }
        else
        {
            await OnFailedAsync(context);
        }
    }

    protected virtual async Task<bool> InternalCheckAsync(RequiresLimitContext context)
    {
        if (context.Increments > context.Limit)
        {
            return false;
        }

        var key = NormalizeKey(context.LimitKey);

        if (await Redis.KeyExistsAsync(key))
        {
            var result = await Redis.StringGetAsync(key);

            return (int)result + context.Increments <= context.Limit;
        }
        else
        {
            return true;
        }
    }

    protected virtual async Task InternalProcessAsync(RequiresLimitContext context)
    {
        var key = NormalizeKey(context.LimitKey);

        if (await Redis.KeyExistsAsync(key))
        {
            await Redis.StringIncrementAsync(
                key,
                context.Increments
                );
        }
        else
        {
            await Redis.StringSetAsync(
                key,
                context.Increments,
                context.GetEffectTimeSpan(Clock),
                When.NotExists
                );
        }
    }

    // TODO 应该抽象出一个接口？
    protected virtual string NormalizeKey(string limitKey)
    {
        return KeyNormalizer.NormalizeKey<string>("RuleLimitKey:" + limitKey);
    }

    // TODO 是否多余？
    protected virtual async Task OnFailedAsync(RequiresLimitContext context)
    {
        foreach (var handler in context.FailedHandlers)
        {
            await handler.Invoke();
        }
    }
}
