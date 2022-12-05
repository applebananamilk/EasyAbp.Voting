using System;
using System.Threading.Tasks;

namespace EasyAbp.Voting.Players;

public interface IPlayerNumberGenerator
{
    Task<int> GenerateAsync(Guid activityId);
}