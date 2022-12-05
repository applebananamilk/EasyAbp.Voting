using EasyAbp.Voting.Activities.Cache;
using EasyAbp.Voting.Players.Cache;
using EasyAbp.Voting.Rules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace EasyAbp.Voting;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpBackgroundWorkersModule),
    typeof(VotingDomainSharedModule)
)]
public class VotingDomainModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var definitionProviders = new List<Type>();
        var validationProvider = new List<Type>();

        context.Services.OnRegistred(context =>
        {
            if (typeof(IRuleDefinitionProvider).IsAssignableFrom(context.ImplementationType))
            {
                definitionProviders.Add(context.ImplementationType);
            }
            if (typeof(IRuleValidationProvider).IsAssignableFrom(context.ImplementationType))
            {
                validationProvider.Add(context.ImplementationType);
            }
        });

        Configure<RuleOptions>(options =>
        {
            options.DefinitionProviders.AddIfNotContains(definitionProviders);
            options.ValidationProviders.AddIfNotContains(validationProvider);
        });
    }

    public override void OnApplicationInitialization(Volo.Abp.ApplicationInitializationContext context)
    {
        context.AddBackgroundWorkerAsync<ActivityViewsCacheSynchronizer>();
        context.AddBackgroundWorkerAsync<PlayerRankingsVotesSynchronizer>();
    }
}
