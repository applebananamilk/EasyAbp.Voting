using EasyAbp.Voting.Activities;
using EasyAbp.Voting.Players;
using EasyAbp.Voting.Rules;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace EasyAbp.Voting.EntityFrameworkCore;

[DependsOn(
    typeof(VotingDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class VotingEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<VotingDbContext>(options =>
        {
            options.AddRepository<Activity, ActivityRepository>();
            options.AddRepository<Player, PlayerRepository>();
            options.AddRepository<Rule, RuleRepository>();
        });
    }
}
