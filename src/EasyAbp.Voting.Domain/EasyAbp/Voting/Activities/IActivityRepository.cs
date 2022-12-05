using System;
using Volo.Abp.Domain.Repositories;

namespace EasyAbp.Voting.Activities;

public interface IActivityRepository : IRepository<Activity, Guid>
{

}
