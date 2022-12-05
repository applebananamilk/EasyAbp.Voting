using EasyAbp.Voting.Admin.Rules.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Admin.Rules;

[RemoteService(Name = VotingAdminRemoteServiceConsts.RemoteServiceName)]
[Area(VotingAdminRemoteServiceConsts.ModuleName)]
[Route("api/voting/admin/activities")]
[ControllerName("ActivityAdmin")]
public class RuleController : VotingAdminController, IRuleAppService
{
    private readonly IRuleAppService _ruleAppService;

    public RuleController(IRuleAppService ruleAppService)
    {
        _ruleAppService = ruleAppService;
    }

    [HttpGet]
    [Route("{activityId}/rule")]
    public Task<ListResultDto<RuleDto>> GetRuleListAsync(Guid activityId)
    {
        return _ruleAppService.GetRuleListAsync(activityId);
    }

    [HttpPut]
    [Route("{activityId}/rule")]
    public Task<ListResultDto<RuleDto>> UpdateRuleAsync(Guid activityId, List<RuleUpdateInput> rules)
    {
        return _ruleAppService.UpdateRuleAsync(activityId, rules);
    }
}
