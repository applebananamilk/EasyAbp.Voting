using EasyAbp.Voting.Activities;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace EasyAbp.Voting.Admin.Activities.Dtos;

public class BannerCreateUpdateInput
{
    [Required]
    [DynamicStringLength(typeof(BannerConsts), nameof(BannerConsts.MaxUrlLength))]
    public string Url { get; set; }

    [DynamicStringLength(typeof(BannerConsts), nameof(BannerConsts.MaxUrlLength))]
    public string Link { get; set; }
}
