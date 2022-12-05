using EasyAbp.Voting.Localization;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace EasyAbp.Voting;

[DependsOn(
    typeof(VotingApplicationContractsModule),
    typeof(AbpAspNetCoreMvcModule))]
public class VotingHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(VotingHttpApiModule).Assembly);
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
