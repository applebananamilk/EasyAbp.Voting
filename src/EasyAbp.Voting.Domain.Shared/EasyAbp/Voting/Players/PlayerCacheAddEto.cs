using System;

namespace EasyAbp.Voting.Players;

[Serializable]
public class PlayerCacheAddEto
{
    public Guid ActivityId { get; set; }

    public Guid PlayerId { get; set; }

    public PlayerCacheAddEto()
    {

    }

    public PlayerCacheAddEto(Guid activityId, Guid playerId)
    {
        ActivityId = activityId;
        PlayerId = playerId;
    }
}
