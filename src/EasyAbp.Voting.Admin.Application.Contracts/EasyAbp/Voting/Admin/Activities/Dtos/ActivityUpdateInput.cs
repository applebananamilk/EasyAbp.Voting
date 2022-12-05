using EasyAbp.Voting.Activities;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace EasyAbp.Voting.Admin.Activities.Dtos;

public class ActivityUpdateInput
{
    [Required]
    public string ActivityName { get; set; }

    public DateTime ActivityStartTime { get; set; }

    public DateTime ActivityEndTime { get; set; }

    public string Introduction { get; set; }

    public string BackgroundMusic { get; set; }

    [Required]
    public string CoverImage { get; set; }

    [Required]
    [DynamicStringLength(typeof(ActivityConsts), nameof(ActivityConsts.MaxVotesUnitLength))]
    public string VotesUnit { get; set; }

    public string ContactUs { get; set; }

    public string FormContent { get; set; }
}
