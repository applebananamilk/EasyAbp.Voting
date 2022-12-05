using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace EasyAbp.Voting.Admin;

[DependsOn(
    typeof(VotingApplicationContractsSharedModule),
    typeof(AbpAuthorizationModule)
    )]
public class VotingAdminApplicationContractsModule : AbpModule
{

}
