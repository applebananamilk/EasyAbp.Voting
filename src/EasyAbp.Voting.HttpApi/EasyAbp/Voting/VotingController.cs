using EasyAbp.Voting.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace EasyAbp.Voting;

public abstract class VotingController : AbpControllerBase
{
    protected VotingController()
    {
        LocalizationResource = typeof(VotingResource);
    }
}
