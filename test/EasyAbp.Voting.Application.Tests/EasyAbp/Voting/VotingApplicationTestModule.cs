using Volo.Abp.Modularity;

namespace EasyAbp.Voting.EasyAbp.Voting;

[DependsOn(
    typeof(VotingApplicationModule),
    typeof(VotingDomainTestModule)
    )]
public class VotingApplicationTestModule : AbpModule
{

}
