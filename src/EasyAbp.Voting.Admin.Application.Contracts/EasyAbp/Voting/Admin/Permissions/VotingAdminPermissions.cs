using Volo.Abp.Reflection;

namespace EasyAbp.Voting.Admin.Permissions;

public class VotingAdminPermissions
{
    public const string GroupName = "VotingAdmin";

    public static class Activities
    {
        public const string Default = GroupName + ".Activity";
        public const string Create = GroupName + ".Create";
        public const string Update = GroupName + ".Update";
        public const string Delete = GroupName + ".Delete";
        public const string Publish = GroupName + ".Publish";
    }

    public static class Players
    {
        private const string Default = Activities.Default + ".Player";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string Review = Default + ".Review";
    }

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(VotingAdminPermissions));
    }
}
