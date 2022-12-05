using EasyAbp.Voting.Activities;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace EasyAbp.Voting.Admin.Activities.Dtos;

public class GroupCreateUpdateInput
{
    [Required]
    [DynamicStringLength(typeof(GroupConsts), nameof(GroupConsts.MaxNameLength))]
    public string Name { get; set; }

    [DynamicStringLength(typeof(GroupConsts), nameof(GroupConsts.MaxDescriptionLength))]
    public string Description { get; set; }
}
