using EasyAbp.Voting.EasyAbp.Voting.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace EasyAbp.Voting.EasyAbp.Voting;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(VotingEntityFrameworkCoreTestModule)
    )]
public class VotingDomainTestModule : AbpModule
{

}
