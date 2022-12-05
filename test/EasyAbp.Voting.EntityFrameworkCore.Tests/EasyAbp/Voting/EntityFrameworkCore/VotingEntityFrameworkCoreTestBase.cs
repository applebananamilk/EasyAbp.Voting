using EasyAbp.Voting.EasyAbp.Voting;

namespace EasyAbp.Voting.EasyAbp.Voting.EntityFrameworkCore;

/* This class can be used as a base class for EF Core integration tests,
 * while SampleRepository_Tests uses a different approach.
 */
public abstract class VotingEntityFrameworkCoreTestBase : VotingTestBase<VotingEntityFrameworkCoreTestModule>
{

}
