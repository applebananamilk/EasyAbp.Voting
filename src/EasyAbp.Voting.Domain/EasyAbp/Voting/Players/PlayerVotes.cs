using System;
using Volo.Abp.Domain.Entities;

namespace EasyAbp.Voting.Players;

public class PlayerVotes : Entity
{
    public virtual Guid PlayerId { get; protected set; }

    public virtual long Votes { get; protected set; }

    protected PlayerVotes()
    {

    }

    internal PlayerVotes(Guid playerId, long votes)
    {
        PlayerId = playerId;
        Votes = votes;
    }

    public virtual void Update(long votes)
    {
        if (votes < 0)
        {
            throw new ArgumentException("votes < 0", nameof(votes));
        }

        Votes = votes;
    }

    public override object[] GetKeys()
    {
        return new object[] { PlayerId };
    }
}
