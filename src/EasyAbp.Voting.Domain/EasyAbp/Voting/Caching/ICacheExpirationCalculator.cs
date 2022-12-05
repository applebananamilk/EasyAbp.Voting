using System;
using System.Threading.Tasks;

namespace EasyAbp.Voting.Caching;

public interface ICacheExpirationCalculator
{
    Task<TimeSpan> CalculateAsync(Guid activityId);
}
