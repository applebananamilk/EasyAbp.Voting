using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Activities.Dtos;

public class PlayerRankingsGetInput : PagedResultRequestDto
{
    public Guid? GroupId { get; set; }
}
