using System;
using System.Threading;
using System.Threading.Tasks;

namespace EasyAbp.Voting.Activities.Cache;

public interface IActivityViewsCacheManager
{
    Task IncrementAsync(Guid activityId);

    Task<long> GetAsync(Guid activityId);

    Task SaveAsync(CancellationToken cancellationToken = default);
}
