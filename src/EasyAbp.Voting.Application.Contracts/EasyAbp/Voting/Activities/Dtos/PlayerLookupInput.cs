using System;

namespace EasyAbp.Voting.Activities.Dtos;

public class PlayerLookupInput
{
    public Guid? GroupId { get; set; }

    public string NameOrPlayerNumber { get; set; }
}
