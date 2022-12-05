using EasyAbp.Voting.Activities.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EasyAbp.Voting.Activities;

public interface IActivityAppService : IApplicationService
{
    Task<ActivityDto> GetAsync(Guid id);

    Task<ActivityStatisticsDto> GetStatisticsAsync(Guid id);

    Task<PlayerDto> GetPlayerAsync(Guid id, Guid playerId);

    Task<PagedResultDto<PlayerGetListDto>> GetPlayerListAsync(Guid id, PlayerGetListInput input);

    Task<PagedResultDto<PlayerRankingsDto>> GetPlayerRankingsAsync(Guid id, PlayerRankingsGetInput input);

    Task<ListResultDto<PlayerDto>> LookupAsync(Guid id, PlayerLookupInput input);

    Task VoteAsync(Guid id, Guid playerId);
}
