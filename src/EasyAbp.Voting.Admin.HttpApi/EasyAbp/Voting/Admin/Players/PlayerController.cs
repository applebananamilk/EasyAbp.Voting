using EasyAbp.Voting.Admin.Players.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Admin.Players;

[RemoteService(Name = VotingAdminRemoteServiceConsts.RemoteServiceName)]
[Area(VotingAdminRemoteServiceConsts.ModuleName)]
[Route("api/voting/admin/players")]
[ControllerName("PlayerAdmin")]
public class PlayerController : VotingAdminController, IPlayerAppService
{
    private readonly IPlayerAppService _playerAppService;

    public PlayerController(IPlayerAppService playerAppService)
    {
        _playerAppService = playerAppService;
    }

    [HttpPost]
    public Task<PlayerDto> CreateAsync(PlayerCreateInput input)
    {
        return _playerAppService.CreateAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public Task<PlayerDto> GetAsync(Guid id)
    {
        return _playerAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<PlayerDto>> GetListAsync(PlayerGetListInput input)
    {
        return _playerAppService.GetListAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public Task<PlayerDto> UpdateAsync(Guid id, PlayerUpdateInput input)
    {
        return _playerAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _playerAppService.DeleteAsync(id);
    }

    [HttpPost]
    [Route("{id}/approve")]
    public Task ApproveAsync(Guid id)
    {
        return _playerAppService.ApproveAsync(id);
    }

    [HttpPost]
    [Route("{id}/reject")]
    public Task RejectAsync(Guid id, PlayerRejectInput input)
    {
        return _playerAppService.RejectAsync(id, input);
    }
}
