using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace EasyAbp.Voting.Players.Cache;

public class PlayerRankingsVotesSynchronizer : AsyncPeriodicBackgroundWorkerBase
{
    public PlayerRankingsVotesSynchronizer(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<VotingOptions> votingOptions) : base(timer, serviceScopeFactory)
    {
        var options = votingOptions.Value;
        Timer.Period = Convert.ToInt32(options.PlayerRankingsVotesSynchronizerPeriod.TotalMilliseconds);
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var playerRankingsCacheManager = LazyServiceProvider.LazyGetRequiredService<IPlayerRankingsCacheManager>();
        await playerRankingsCacheManager.SaveAsync(workerContext.CancellationToken);
    }
}
