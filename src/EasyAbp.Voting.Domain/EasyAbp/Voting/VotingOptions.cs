using System;

namespace EasyAbp.Voting;

public class VotingOptions
{
    public TimeSpan ActivityViewsCacheSynchronizerPeriod { get; set; }

    public TimeSpan PlayerRankingsVotesSynchronizerPeriod { get; set; }

    public VotingOptions()
    {
        ActivityViewsCacheSynchronizerPeriod = TimeSpan.FromMinutes(5);
        PlayerRankingsVotesSynchronizerPeriod = TimeSpan.FromMinutes(3);
    }
}
