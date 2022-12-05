using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace EasyAbp.Voting.Activities.Cache;

public class ActivityViewsCacheSynchronizer : AsyncPeriodicBackgroundWorkerBase
{
    public ActivityViewsCacheSynchronizer(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<VotingOptions> votingOptions) : base(timer, serviceScopeFactory)
    {
        var options = votingOptions.Value;
        Timer.Period = Convert.ToInt32(options.ActivityViewsCacheSynchronizerPeriod.TotalMilliseconds);
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var activityViewsCacheManager = LazyServiceProvider.LazyGetRequiredService<IActivityViewsCacheManager>();
        await activityViewsCacheManager.SaveAsync(workerContext.CancellationToken);
    }
}
