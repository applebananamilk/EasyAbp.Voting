using EasyAbp.Voting.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Voting.Rules;

public class RuleRepository : EfCoreRepository<IVotingDbContext, Rule, Guid>, IRuleRepository
{
    public RuleRepository(IDbContextProvider<IVotingDbContext> dbContextProvider) : base(dbContextProvider)
    {

    }
}
