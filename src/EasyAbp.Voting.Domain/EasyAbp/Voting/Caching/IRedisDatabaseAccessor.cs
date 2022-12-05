using StackExchange.Redis;

namespace EasyAbp.Voting.Caching;

public interface IRedisDatabaseAccessor
{
    public IDatabase RedisDatabase { get; }
}
