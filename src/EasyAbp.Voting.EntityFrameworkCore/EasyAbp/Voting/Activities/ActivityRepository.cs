using EasyAbp.Voting.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace EasyAbp.Voting.Activities;

public class ActivityRepository : EfCoreRepository<IVotingDbContext, Activity, Guid>, IActivityRepository
{
    public ActivityRepository(IDbContextProvider<IVotingDbContext> dbContextProvider) : base(dbContextProvider)
    {

    }

    public override async Task<IQueryable<Activity>> WithDetailsAsync()
    {
        return (await base.WithDetailsAsync())
            .Include(p => p.Groups)
            .Include(p => p.Banners);
    }
}
