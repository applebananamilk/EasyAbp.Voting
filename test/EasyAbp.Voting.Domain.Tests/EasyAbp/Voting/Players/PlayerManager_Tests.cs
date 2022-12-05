using EasyAbp.Voting.Players;

namespace EasyAbp.Voting.EasyAbp.Voting.Players;

public class PlayerManager_Tests : VotingDomainTestBase
{
    private readonly PlayerManager _playerManager;

    public PlayerManager_Tests()
    {
        _playerManager = GetRequiredService<PlayerManager>();
    }
}
