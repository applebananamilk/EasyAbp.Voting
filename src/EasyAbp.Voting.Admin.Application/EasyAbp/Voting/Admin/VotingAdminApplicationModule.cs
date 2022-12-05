using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace EasyAbp.Voting.Admin;

[DependsOn(
    typeof(VotingDomainModule),
    typeof(VotingAdminApplicationContractsModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpAutoMapperModule)
    )]
public class VotingAdminApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<VotingAdminApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<VotingAdminApplicationModule>();
        });
    }
}
