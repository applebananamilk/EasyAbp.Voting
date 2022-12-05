namespace EasyAbp.Voting.Activities;

public static class ActivityConsts
{
    public static int MaxActivityNameLength { get; set; } = 128;

    public static int MaxVotesUnitLength { get; set; } = 32;

    public static string DefaultFormContent { get; set; } = "{}";
}
