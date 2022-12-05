using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Activities.Dtos;

public class PlayerGetListInput : PagedResultRequestDto
{
    public Guid? GroupId { get; set; }
}
