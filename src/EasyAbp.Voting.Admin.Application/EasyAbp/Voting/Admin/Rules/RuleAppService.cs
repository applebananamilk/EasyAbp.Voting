using EasyAbp.Voting.Admin.Permissions;
using EasyAbp.Voting.Admin.Rules.Dtos;
using EasyAbp.Voting.Rules;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Admin.Rules;

public class RuleAppService : VotingAdminAppService, IRuleAppService
{
    protected RuleManager RuleManager { get; }

    public RuleAppService(RuleManager ruleManager)
    {
        RuleManager = ruleManager;
    }

    [Authorize(VotingAdminPermissions.Activities.Default)]
    public virtual async Task<ListResultDto<RuleDto>> GetRuleListAsync(Guid activityId)
    {
        var rules = await RuleManager.GetRuleListAsync(activityId);

        return new ListResultDto<RuleDto>(
            ObjectMapper.Map<List<RuleDefinitionWithOnOff>, List<RuleDto>>(rules)
            );
    }

    [Authorize(VotingAdminPermissions.Activities.Update)]
    public virtual async Task<ListResultDto<RuleDto>> UpdateRuleAsync(Guid activityId, List<RuleUpdateInput> rules)
    {
        await RuleManager.UpdateRuleListAsync(
            activityId,
            ObjectMapper.Map<List<RuleUpdateInput>, List<RuleDefinitionWithOnOff>>(rules)
          );

        return await GetRuleListAsync(activityId);
    }
}
