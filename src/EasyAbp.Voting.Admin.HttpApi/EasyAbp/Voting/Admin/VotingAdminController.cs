using EasyAbp.Voting.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace EasyAbp.Voting.Admin;

public abstract class VotingAdminController : AbpControllerBase
{
    protected VotingAdminController()
    {
        LocalizationResource = typeof(VotingResource);
    }
}
