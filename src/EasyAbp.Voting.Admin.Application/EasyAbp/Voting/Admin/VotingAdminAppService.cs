using EasyAbp.Voting.Localization;
using Volo.Abp.Application.Services;

namespace EasyAbp.Voting.Admin;

public abstract class VotingAdminAppService : ApplicationService
{
    protected VotingAdminAppService()
    {
        LocalizationResource = typeof(VotingResource);
        ObjectMapperContext = typeof(VotingAdminApplicationModule);
    }
}
