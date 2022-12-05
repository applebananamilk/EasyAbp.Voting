using EasyAbp.Voting.Activities;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace EasyAbp.Voting.Admin.Activities.Dtos;

public class ActivityCreateInput
{
    [Required]
    [DynamicStringLength(typeof(ActivityConsts), nameof(ActivityConsts.MaxActivityNameLength))]
    public string ActivityName { get; set; }

    [Required]
    public DateTime ActivityStartTime { get; set; }

    [Required]
    public DateTime ActivityEndTime { get; set; }

    [Required]
    public string CoverImage { get; set; }

    [Required]
    [DynamicStringLength(typeof(ActivityConsts), nameof(ActivityConsts.MaxVotesUnitLength))]
    public string VotesUnit { get; set; }
}
