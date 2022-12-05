using EasyAbp.Voting.Admin.Rules.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace EasyAbp.Voting.Admin.Rules;

public interface IRuleAppService : IApplicationService
{
    Task<ListResultDto<RuleDto>> GetRuleListAsync(Guid activityId);

    Task<ListResultDto<RuleDto>> UpdateRuleAsync(Guid activityId, List<RuleUpdateInput> rules);
}
