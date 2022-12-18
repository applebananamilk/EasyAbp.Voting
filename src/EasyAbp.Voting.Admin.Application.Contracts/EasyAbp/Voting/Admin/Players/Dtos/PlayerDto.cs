using EasyAbp.Voting.Players;
using System;
using Volo.Abp.Application.Dtos;

namespace EasyAbp.Voting.Admin.Players.Dtos;

public class PlayerDto : ExtensibleCreationAuditedEntityDto<Guid>
{
    public Guid ActivityId { get; set; }

    public Guid? GroupId { get; set; }

    public int? PlayerNumber { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string CoverImage { get; set; }

    public string FormContent { get; set; }

    public Status Status { get; set; }

    public string RejectReason { get; set; }

    public int Votes { get; set; }
}
