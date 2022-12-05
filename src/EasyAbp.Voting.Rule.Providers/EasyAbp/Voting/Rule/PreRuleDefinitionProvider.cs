using EasyAbp.Voting.Rule.LimitValidation;
using EasyAbp.Voting.Rule.Localization;
using EasyAbp.Voting.Rule.Providers;
using EasyAbp.Voting.Rules;
using System;
using Volo.Abp.Localization;

namespace EasyAbp.Voting.Rule;

public class PreRuleDefinitionProvider : RuleDefinitionProvider
{
    public override void Define(IRuleDefinitionContext context)
    {
        context
            .AddDefinition(
                name: LimitVotingTimeRuleValidationProvider.ProviderName,
                displayName: L(LimitVotingTimeRuleValidationProvider.ProviderName))
            .WithProperty(
                propertyName: LimitVotingTimeRuleValidationProvider.StartTime,
                defaultValue: DateTime.Parse("00:00:00"))
            .WithProperty(
                propertyName: LimitVotingTimeRuleValidationProvider.EndTime,
                defaultValue: DateTime.Parse("06:00:00"));

        context
            .AddDefinition(
                name: LimitVotesNumberRuleValidationProvider.ProviderName,
                displayName: L(LimitVotesNumberRuleValidationProvider.ProviderName))
            .WithProperty(
                propertyName: LimitVotesNumberRuleValidationProvider.LimitPolicy,
                defaultValue: LimitPolicy.Days)
            .WithProperty(
                propertyName: LimitVotesNumberRuleValidationProvider.Number,
                defaultValue: 3);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<VotingRuleResource>(name);
    }
}
