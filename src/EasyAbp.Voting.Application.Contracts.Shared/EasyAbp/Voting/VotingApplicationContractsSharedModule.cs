using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace EasyAbp.Voting;

[DependsOn(
    typeof(VotingDomainSharedModule),
    typeof(AbpDddApplicationContractsModule)
    )]
public class VotingApplicationContractsSharedModule : AbpModule
{
}
