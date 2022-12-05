namespace EasyAbp.Voting;

public static class VotingDbProperties
{
    public static string DbTablePrefix { get; set; } = "Voting";

    public static string DbSchema { get; set; } = null;

    public const string ConnectionStringName = "Voting";
}
