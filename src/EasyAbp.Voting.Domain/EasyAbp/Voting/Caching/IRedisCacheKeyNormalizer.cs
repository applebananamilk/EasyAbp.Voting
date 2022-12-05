namespace EasyAbp.Voting.Caching;

public interface IRedisCacheKeyNormalizer
{
    string NormalizeKey<TCacheItem>(string key) where TCacheItem : class;
}
