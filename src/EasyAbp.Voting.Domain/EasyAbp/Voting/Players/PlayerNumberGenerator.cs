using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.Voting.Players;

public class PlayerNumberGenerator : IPlayerNumberGenerator, ITransientDependency
{
    protected IDataFilter DataFilter { get; }
    protected IPlayerRepository PlayerRepository { get; }

    public PlayerNumberGenerator(
        IDataFilter dataFilter,
        IPlayerRepository playerRepository)
    {
        DataFilter = dataFilter;
        PlayerRepository = playerRepository;
    }

    public virtual async Task<int> GenerateAsync(Guid activityId)
    {
        using (DataFilter.Disable<ISoftDelete>())
        {
            if (!await PlayerRepository.AnyAsync(p => p.ActivityId == activityId && p.PlayerNumber.HasValue))
            {
                return 1;
            }

            var queryable = await PlayerRepository.GetQueryableAsync();
            return await PlayerRepository.AsyncExecuter.MaxAsync(
                queryable
                    .Where(p => p.ActivityId == activityId && p.PlayerNumber.HasValue)
                    .Select(p => p.PlayerNumber.Value)
                    ) + 1;
        }
    }

    protected virtual string CalculateLockName(Guid activityId)
    {
        return $"Voting:Activity:{activityId}:Lock:PlayerNumberGenerator";
    }
}
