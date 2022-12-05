using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Activities.Dtos;

public class PlayerGetListDto : EntityDto<Guid>
{
    public Guid? GroupId { get; set; }

    public int PlayerNumber { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string CoverImage { get; set; }

    public long Votes { get; set; }
}
