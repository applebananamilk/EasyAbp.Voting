using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Activities.Dtos;

public class ActivityDto : EntityDto<Guid>
{
    public string ActivityName { get; set; }

    public DateTime ActivityStartTime { get; set; }

    public DateTime ActivityEndTime { get; set; }

    public string Introduction { get; set; }

    public string BackgroundMusic { get; set; }

    public string CoverImage { get; set; }

    public string VotesUnit { get; set; }

    public string ContactUs { get; set; }

    public string FormContent { get; set; }

    public List<GroupDto> Groups { get; set; } = new List<GroupDto>();

    public List<BannerDto> Banners { get; set; } = new List<BannerDto>();
}
