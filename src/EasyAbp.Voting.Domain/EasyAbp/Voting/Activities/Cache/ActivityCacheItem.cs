using System;
using System.Collections.Generic;

namespace EasyAbp.Voting.Activities.Cache;

[Serializable]
public class ActivityCacheItem
{
    public Guid Id { get; set; }

    public string ActivityName { get; set; }

    public DateTime ActivityStartTime { get; set; }

    public DateTime ActivityEndTime { get; set; }

    public string Introduction { get; set; }

    public string BackgroundMusic { get; set; }

    public string CoverImage { get; set; }

    public string VotesUnit { get; set; }

    public string ContactUs { get; set; }

    public bool IsDraft { get; set; }

    public string FormContent { get; set; }

    public List<GroupCacheItem> Groups { get; set; } = new();

    public List<BannerCacheItem> Banners { get; set; } = new();

    public static string CalculateCacheKey(Guid activityId)
    {
        return $"Voting:Activity:{activityId}";
    }
}

[Serializable]
public class GroupCacheItem
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}

[Serializable]
public class BannerCacheItem
{
    public Guid Id { get; set; }

    public string Url { get; set; }

    public string Link { get; set; }
}
