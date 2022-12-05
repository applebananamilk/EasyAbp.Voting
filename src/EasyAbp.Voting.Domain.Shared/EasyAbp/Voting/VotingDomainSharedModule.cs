using EasyAbp.Voting.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace EasyAbp.Voting;

[DependsOn(
    typeof(AbpValidationModule)
)]
public class VotingDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<VotingDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<VotingResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("EasyAbp/Voting/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("Voting", typeof(VotingResource));
        });
    }
}
