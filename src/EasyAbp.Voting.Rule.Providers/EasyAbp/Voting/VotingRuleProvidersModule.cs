using EasyAbp.Voting.Rule.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.Voting;

[DependsOn(
    typeof(VotingDomainModule)
    )]
public class VotingRuleProvidersModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<VotingRuleProvidersModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<VotingRuleResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("EasyAbp/Voting/Rule/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Voting.Rule", typeof(VotingRuleResource));
        });
    }
}
