using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.Voting.Caching;

public class RedisCacheKeyNormalizer : IRedisCacheKeyNormalizer, ISingletonDependency
{
    protected IDistributedCacheKeyNormalizer KeyNormalizer { get; }

    public RedisCacheKeyNormalizer(IDistributedCacheKeyNormalizer keyNormalizer)
    {
        KeyNormalizer = keyNormalizer;
    }

    public virtual string NormalizeKey<TCacheItem>(string key) where TCacheItem : class
    {
        return KeyNormalizer.NormalizeKey(
            new DistributedCacheKeyNormalizeArgs(
                key,
                CacheNameAttribute.GetCacheName(typeof(TCacheItem)),
                false));
    }
}
