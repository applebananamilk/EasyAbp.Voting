using System;

namespace EasyAbp.Voting.Players.Cache;

[Serializable]
public class PlayerCacheItem
{
    public Guid Id { get; set; }

    public Guid ActivityId { get; set; }

    public Guid? GroupId { get; set; }

    public string UserId { get; set; }

    public int PlayerNumber { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string CoverImage { get; set; }

    public string FormContent { get; set; }

    public Status Status { get; set; }

    public static string CalculateCacheKey(Guid playerId)
    {
        return $"Voting:Activity:Players:{playerId}";
    }
}
