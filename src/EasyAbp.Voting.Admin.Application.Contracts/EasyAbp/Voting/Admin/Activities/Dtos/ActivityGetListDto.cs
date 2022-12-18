using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Admin.Activities.Dtos;

public class ActivityGetListDto : ExtensibleFullAuditedEntityDto<Guid>
{
    public string ActivityName { get; set; }

    public DateTime ActivityStartTime { get; set; }

    public DateTime ActivityEndTime { get; set; }

    public string CoverImage { get; set; }

    public bool IsDraft { get; set; }

    public int Views { get; set; }
}
