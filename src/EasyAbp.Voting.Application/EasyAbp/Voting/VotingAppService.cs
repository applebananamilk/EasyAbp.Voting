using EasyAbp.Voting.Localization;
using Volo.Abp.Application.Services;

namespace EasyAbp.Voting;

public abstract class VotingAppService : ApplicationService
{
    protected VotingAppService()
    {
        LocalizationResource = typeof(VotingResource);
        ObjectMapperContext = typeof(VotingApplicationModule);
    }
}
