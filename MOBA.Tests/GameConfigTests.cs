using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class GameConfigTests
{
    [Fact]
    public void Match_numbers_match_the_spec()
    {
        Assert.Equal(0, GameConfig.BluePlayerIndex);
        Assert.Equal(1, GameConfig.RedPlayerIndex);
        Assert.Equal(0, GameConfig.BlueTeamIndex);
        Assert.Equal(1, GameConfig.RedTeamIndex);
        Assert.Equal(30f, GameConfig.WaveIntervalSeconds);
        Assert.Equal(3, GameConfig.MinionsPerWave);
        Assert.Equal(1, GameConfig.RangedMinionsPerWave);
        Assert.Equal(8f, GameConfig.HeroRespawnSeconds);
        Assert.Equal(4f, GameConfig.WaypointReachDistance);
        Assert.Equal(15f, GameConfig.MinionKillGold);
        Assert.Equal(150f, GameConfig.HeroKillGold);
        Assert.Equal(100f, GameConfig.TowerKillGold);
        Assert.Equal("melee_minion", GameConfig.MeleeMinionUnitId);
        Assert.Equal("ranged_minion", GameConfig.RangedMinionUnitId);
        Assert.Equal("adventurer", GameConfig.BlueHeroUnitId);
        Assert.Equal("armored_dragon", GameConfig.RedHeroUnitId);
        Assert.Equal("castle", GameConfig.CastleUnitId);
        Assert.Equal("castle_tower_1", GameConfig.TowerUnitId);
        Assert.True(GameConfig.DebugChat);
    }
}
