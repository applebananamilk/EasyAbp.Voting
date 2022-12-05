using EasyAbp.Voting.Activities.Dtos;
using EasyAbp.Voting.Admin.Activities.Dtos;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace EasyAbp.Voting.Admin.Activities;

public interface IActivityAppService : ICrudAppService<
    ActivityDto,
    ActivityGetListDto,
    Guid,
    ActivityGetListInput,
    ActivityCreateInput,
    ActivityUpdateInput>
{
    Task PublishAsync(Guid id);

    Task UnpublishAsync(Guid id);

    Task<ActivityDto> AddGroupAsync(Guid id, GroupCreateUpdateInput input);

    Task<ActivityDto> UpdateGroupAsync(Guid id, Guid groupId, GroupCreateUpdateInput input);

    Task<ActivityDto> DeleteGroupAsync(Guid id, Guid groupId);

    Task<ActivityDto> AddBannerAsync(Guid id, BannerCreateUpdateInput input);

    Task<ActivityDto> UpdateBannerAsync(Guid id, Guid bannerId, BannerCreateUpdateInput input);

    Task<ActivityDto> DeleteBannerAsync(Guid id, Guid bannerId);
}
