using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class WinDecisionTests
{
    [Fact]
    public void Neither_dead_is_none()
    {
        Assert.Equal(MatchWinner.None, WinDecision.FromCastleState(false, false));
    }

    [Fact]
    public void Red_dead_blue_wins()
    {
        Assert.Equal(MatchWinner.Blue, WinDecision.FromCastleState(false, true));
    }

    [Fact]
    public void Blue_dead_red_wins()
    {
        Assert.Equal(MatchWinner.Red, WinDecision.FromCastleState(true, false));
    }

    [Fact]
    public void Both_dead_blue_wins()
    {
        Assert.Equal(MatchWinner.Blue, WinDecision.FromCastleState(true, true));
    }
}
