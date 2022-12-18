using EasyAbp.Voting.Players;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace EasyAbp.Voting.Admin.Players.Dtos;

public class PlayerUpdateInput : ExtensibleObject
{
    public Guid? GroupId { get; set; }

    public virtual string UserId { get; set; }

    [Required]
    [DynamicStringLength(typeof(PlayerConsts), nameof(PlayerConsts.MaxNameLength))]
    public string Name { get; set; }

    [Required]
    public string Avatar { get; set; }

    [Required]
    public string CoverImage { get; set; }

    public string FormContent { get; set; }
}
