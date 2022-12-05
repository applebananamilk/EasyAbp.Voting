using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace EasyAbp.Voting;

[DependsOn(
    typeof(VotingApplicationContractsSharedModule),
    typeof(AbpAuthorizationModule)
    )]
public class VotingApplicationContractsModule : AbpModule
{

}
