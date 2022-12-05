using EasyAbp.Voting.Admin.Players.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace EasyAbp.Voting.Admin.Players;

public interface IPlayerAppService : ICrudAppService<
    PlayerDto,
    Guid,
    PlayerGetListInput,
    PlayerCreateInput,
    PlayerUpdateInput>
{
    Task ApproveAsync(Guid id);

    Task RejectAsync(Guid id, PlayerRejectInput input);
}
