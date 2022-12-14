using EasyAbp.Voting.Rule.LimitValidation;
using EasyAbp.Voting.Rule.Localization;
using EasyAbp.Voting.Rules;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Users;

namespace EasyAbp.Voting.Rule.Providers;

public class LimitVotesNumberRuleValidationProvider : RuleValidationProvider
{
    public const string ProviderName = "LimitVotesNumber";
    public const string LimitPolicy = nameof(LimitPolicy);
    public const string Number = nameof(Number);

    public LimitVotesNumberRuleValidationProvider(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }

    public override string Name => ProviderName;

    public override async Task ValidateAsync(RuleValidationContext context, CancellationToken cancellationToken = default)
    {
        var number = context.GetProperty<int>(Number);
        var limitPolicy = (LimitPolicy)context.GetProperty<int>(LimitPolicy);

        var requiresLimitChecker = LazyServiceProvider.LazyGetRequiredService<IRequiresLimitChecker>();
        var localizer = LazyServiceProvider.LazyGetRequiredService<IStringLocalizer<VotingRuleResource>>();

        var requiresLimitContext = RequiresLimitContext
            .Create(NormalizeKey(context), 1, limitPolicy, number, context.Votes)
            .OnFailed(
                () => throw new BusinessException($"Voting.Rule:LimitVotesNumber")
                    .WithData(nameof(limitPolicy), localizer[limitPolicy.ToString()])
                    .WithData(nameof(number), number)
                );

        await requiresLimitChecker.ProcessAsync(requiresLimitContext);
    }

    protected virtual string NormalizeKey(RuleValidationContext context)
    {
        return $"{ProviderName}:UserId:{CurrentUser.GetId()}:PlayerId:{context.PlayerId}";
    }
}
