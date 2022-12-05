using EasyAbp.Voting.Players;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Validation;

namespace EasyAbp.Voting.Admin.Players.Dtos;

public class PlayerRejectInput
{
    [Required]
    [DynamicStringLength(typeof(PlayerConsts), nameof(PlayerConsts.MaxRejectReasonLength))]
    public string RejectReason { get; set; }
}
