using EasyAbp.Voting.Rules;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Timing;

namespace EasyAbp.Voting.Rule.Providers;

public class LimitVotingTimeRuleValidationProvider : RuleValidationProvider
{
    public const string ProviderName = "LimitVotingTime";
    public const string StartTime = nameof(StartTime);
    public const string EndTime = nameof(EndTime);

    public LimitVotingTimeRuleValidationProvider(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }

    public override string Name => ProviderName;

    public override Task ValidateAsync(RuleValidationContext context, CancellationToken cancellationToken = default)
    {
        var startTime = TimeOnly.Parse(context.GetProperty<string>(StartTime));
        var endTime = TimeOnly.Parse(context.GetProperty<string>(EndTime));

        if (startTime > endTime)
        {
            return Task.CompletedTask;
        }

        var clock = LazyServiceProvider.LazyGetRequiredService<IClock>();
        var currentTime = TimeOnly.FromDateTime(clock.Now);

        if (currentTime > startTime && currentTime < endTime)
        {
            throw new BusinessException("Voting.Rule:CurrentTimeCannotVote")
                .WithData(nameof(startTime), startTime)
                .WithData(nameof(endTime), endTime);
        }

        return Task.CompletedTask;
    }
}
