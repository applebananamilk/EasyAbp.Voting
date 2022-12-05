using System;

namespace EasyAbp.Voting.Players;

[Serializable]
public class PlayerCacheRemoveEto
{
    public Guid ActivityId { get; set; }

    public Guid PlayerId { get; set; }

    public PlayerCacheRemoveEto()
    {

    }

    public PlayerCacheRemoveEto(Guid activityId, Guid playerId)
    {
        ActivityId = activityId;
        PlayerId = playerId;
    }
}
