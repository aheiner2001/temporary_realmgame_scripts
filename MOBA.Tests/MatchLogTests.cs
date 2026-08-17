using System.Numerics;
using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class MatchLogTests
{
    private static readonly Vector3 Origin = new(24, 0, 24);

    [Fact]
    public void Spawned_uses_debug_prefix_and_invariant_position()
    {
        Assert.Equal(
            "Debug: spawned castle id=12 at (24, 0, 24) for player 0",
            MatchLog.Spawned("castle", 12, Origin, 0));
    }

    [Fact]
    public void SpawnFailed_has_no_id()
    {
        Assert.Equal(
            "Debug: spawn FAILED castle at (24, 0, 24) for player 0",
            MatchLog.SpawnFailed("castle", Origin, 0));
    }

    [Fact]
    public void CastleSummary_lists_both_ids()
    {
        Assert.Equal(
            "Debug: blue castle id=12 red castle id=34",
            MatchLog.CastleSummary(12, 34));
    }

    [Fact]
    public void WaveTick_uses_lane_count()
    {
        Assert.Equal("Debug: wave tick, 3 lanes", MatchLog.WaveTick(3));
    }

    [Fact]
    public void GoldAwarded_uses_unit_type_id()
    {
        Assert.Equal(
            "Debug: +15 gold to player 0 (melee_minion)",
            MatchLog.GoldAwarded(15f, 0, "melee_minion"));
    }

    [Fact]
    public void WinFired_blue_and_red()
    {
        Assert.Equal("Debug: win fired Blue", MatchLog.WinFired(MatchWinner.Blue));
        Assert.Equal("Debug: win fired Red", MatchLog.WinFired(MatchWinner.Red));
    }

    [Fact]
    public void WinCheckSkipped_is_stable()
    {
        Assert.Equal(
            "Debug: win check skipped, castle id is 0",
            MatchLog.WinCheckSkipped());
    }

    [Fact]
    public void Position_drops_trailing_zeros_keeps_tenths()
    {
        Assert.Equal(
            "Debug: spawned castle id=1 at (24.5, 0, 24) for player 1",
            MatchLog.Spawned("castle", 1, new Vector3(24.5f, 0, 24), 1));
    }

    [Fact]
    public void Write_sends_when_DebugChat_is_on()
    {
        string? got = null;
        MatchLog.Write(msg => got = msg, "Debug: hello");
        Assert.Equal("Debug: hello", got);
    }
}
