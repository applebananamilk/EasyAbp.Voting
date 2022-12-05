using AutoMapper;
using EasyAbp.Voting.Activities;
using EasyAbp.Voting.Activities.Dtos;
using EasyAbp.Voting.Admin.Activities.Dtos;
using EasyAbp.Voting.Admin.Players.Dtos;
using EasyAbp.Voting.Admin.Rules.Dtos;
using EasyAbp.Voting.Players;
using EasyAbp.Voting.Rules;

namespace EasyAbp.Voting.Admin;

public class VotingAdminApplicationAutoMapperProfile : Profile
{
    public VotingAdminApplicationAutoMapperProfile()
    {
        CreateMap<Activity, ActivityDto>();
        CreateMap<Activity, ActivityGetListDto>();
        CreateMap<Group, GroupDto>();
        CreateMap<Banner, BannerDto>();

        CreateMap<Player, PlayerDto>();

        CreateMap<RuleDefinitionWithOnOff, RuleDto>();
        CreateMap<RuleUpdateInput, RuleDefinitionWithOnOff>();
    }
}
