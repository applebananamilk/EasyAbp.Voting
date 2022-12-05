using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace EasyAbp.Voting;

[DependsOn(
    typeof(VotingDomainModule),
    typeof(VotingApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class VotingApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<VotingApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<VotingApplicationModule>();
        });
    }
}
