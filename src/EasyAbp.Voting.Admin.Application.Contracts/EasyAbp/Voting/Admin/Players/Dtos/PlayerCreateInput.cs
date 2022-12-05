using System;

namespace EasyAbp.Voting.Admin.Players.Dtos;

public class PlayerCreateInput : PlayerUpdateInput
{
    public Guid ActivityId { get; set; }
}
