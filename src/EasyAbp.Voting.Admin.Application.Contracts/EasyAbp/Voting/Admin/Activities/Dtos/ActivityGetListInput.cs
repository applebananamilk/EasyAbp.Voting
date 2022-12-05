using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Admin.Activities.Dtos;

public class ActivityGetListInput : PagedAndSortedResultRequestDto
{
    public string ActivityName { get; set; }

    public DateTime? ActivityStartTime { get; set; }

    public DateTime? ActivityEndTime { get; set; }

    public bool? IsDraft { get; set; }
}
