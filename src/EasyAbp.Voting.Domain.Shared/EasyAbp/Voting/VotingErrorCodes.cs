namespace EasyAbp.Voting;

public static class VotingErrorCodes
{
    public const string ActivityNotFound = "Voting:ActivityNotFound";
    public const string ActivityStartTimeNotGtActivityEndTime = "Voting:ActivityStartTimeNotGtActivityEndTime";
    public const string ActivityNotStarted = "Voting:ActivityNotStarted";
    public const string ActivityHasEnded = "Voting:ActivityHasEnded";
    public const string RejectReasonNotNull = "Voting:RejectReasonNotNull";
    public const string PlayerAlreadyExists = "Voting:PlayerAlreadyExists";
    public const string GroupNotSelected = "Voting:GroupNotSelected";
    public const string InvalidGroup = "Voting:InvalidGroup";
    public const string AssignPlayerNumberTimeout = "Voting:AssignPlayerNumberTimeout";
    public const string PlayerInfoException = "Voting:PlayerInfoException";
    public const string RuleNameNotFound = "Voting:RuleNameNotFound";
    public const string GroupCannotDelete = "Voting:GroupCannotDelete";
}
