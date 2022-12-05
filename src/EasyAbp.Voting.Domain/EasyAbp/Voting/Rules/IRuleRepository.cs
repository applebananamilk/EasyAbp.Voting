using System;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.Voting.Rules;

/// <summary>
/// 规则仓储接口
/// </summary>
public interface IRuleRepository : IRepository<Rule, Guid>
{

}
