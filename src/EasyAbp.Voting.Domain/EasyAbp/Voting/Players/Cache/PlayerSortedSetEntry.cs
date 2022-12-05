using System;

namespace EasyAbp.Voting.Players.Cache;

public class PlayerSortedSetEntry
{
    public Guid PlayerId { get; }

    public long Score { get; }

    public PlayerSortedSetEntry(Guid playerId, long score)
    {
        PlayerId = playerId;
        Score = score;
    }
}
