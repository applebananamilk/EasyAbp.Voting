using AutoMapper;
using EasyAbp.Voting.Activities.Cache;
using EasyAbp.Voting.Activities.Dtos;
using EasyAbp.Voting.Players.Cache;

namespace EasyAbp.Voting;

public class VotingApplicationAutoMapperProfile : Profile
{
    public VotingApplicationAutoMapperProfile()
    {
        CreateMap<ActivityCacheItem, ActivityDto>();
        CreateMap<GroupCacheItem, GroupDto>();
        CreateMap<BannerCacheItem, BannerDto>();

        CreateMap<PlayerCacheItem, PlayerDto>();
        CreateMap<PlayerCacheItem, PlayerGetListDto>();
        CreateMap<PlayerCacheItem, PlayerRankingsDto>();
    }
}
