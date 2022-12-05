using EasyAbp.Voting.Localization;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace EasyAbp.Voting.Admin;

[DependsOn(
    typeof(VotingAdminApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class VotingAdminHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(VotingAdminHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<VotingResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
