using EasyAbp.Voting.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace EasyAbp.Voting.Admin.Permissions;

public class VotingAdminPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var votingGroup = context.AddGroup(VotingAdminPermissions.GroupName, L("Permission:Voting"));

        var activities = votingGroup.AddPermission(VotingAdminPermissions.Activities.Default, L("Permission:Activities"));
        activities.AddChild(VotingAdminPermissions.Activities.Create, L("Permission:Create"));
        activities.AddChild(VotingAdminPermissions.Activities.Update, L("Permission:Update"));
        activities.AddChild(VotingAdminPermissions.Activities.Delete, L("Permission:Delete"));
        activities.AddChild(VotingAdminPermissions.Activities.Publish, L("Permission:ActivityPublish"));
        activities.AddChild(VotingAdminPermissions.Players.Create, L("Permission:PlayerCreate"));
        activities.AddChild(VotingAdminPermissions.Players.Update, L("Permission:PlayerUpdate"));
        activities.AddChild(VotingAdminPermissions.Players.Delete, L("Permission:PlayerDelete"));
        activities.AddChild(VotingAdminPermissions.Players.Review, L("Permission:PlayerReview"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<VotingResource>(name);
    }
}
