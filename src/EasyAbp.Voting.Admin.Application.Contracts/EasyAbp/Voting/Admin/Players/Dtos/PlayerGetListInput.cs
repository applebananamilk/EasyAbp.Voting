using EasyAbp.Voting.Players;
using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Admin.Players.Dtos;

public class PlayerGetListInput : PagedAndSortedResultRequestDto
{
    public Guid ActivityId { get; set; }

    public Guid? GroupId { get; set; }

    public int? PlayerNumber { get; set; }

    public string Name { get; set; }

    public Status? Status { get; set; }
}
