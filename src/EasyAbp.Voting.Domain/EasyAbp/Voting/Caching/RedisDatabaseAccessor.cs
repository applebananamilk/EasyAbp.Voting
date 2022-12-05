using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Threading;
using Volo.Abp.DependencyInjection;

namespace EasyAbp.Voting.Caching;

public class RedisDatabaseAccessor : IRedisDatabaseAccessor, ISingletonDependency
{
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private readonly RedisCacheOptions _options;

    public IDatabase RedisDatabase { get; private set; }

    public RedisDatabaseAccessor(IOptions<RedisCacheOptions> optionsAccessor)
    {
        _options = optionsAccessor.Value;
        Connect();
    }

    private void Connect()
    {
        if (RedisDatabase != null)
        {
            return;
        }

        _connectionLock.Wait();
        try
        {
            if (RedisDatabase == null)
            {
                ConfigurationOptions redisConfig;
                if (_options.ConfigurationOptions != null)
                {
                    redisConfig = _options.ConfigurationOptions;
                }
                else
                {
                    redisConfig = ConfigurationOptions.Parse(_options.Configuration);
                }
                RedisDatabase = ConnectionMultiplexer.Connect(redisConfig).GetDatabase();
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }
}
