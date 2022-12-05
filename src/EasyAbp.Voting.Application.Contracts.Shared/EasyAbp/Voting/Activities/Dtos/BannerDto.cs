using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Activities.Dtos;

public class BannerDto : EntityDto<Guid>
{
    public string Url { get; set; }

    public string Link { get; set; }
}
