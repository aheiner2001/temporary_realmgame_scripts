using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class GoldTableTests
{
    [Fact]
    public void Minion_pays_15()
    {
        Assert.Equal(15f, GoldTable.ForDeadUnit(GameConfig.MeleeMinionUnitId, isHero: false));
        Assert.Equal(15f, GoldTable.ForDeadUnit(GameConfig.RangedMinionUnitId, isHero: false));
    }

    [Fact]
    public void Hero_pays_150()
    {
        Assert.Equal(150f, GoldTable.ForDeadUnit(GameConfig.BlueHeroUnitId, isHero: true));
        Assert.Equal(150f, GoldTable.ForDeadUnit(GameConfig.RedHeroUnitId, isHero: true));
    }

    [Fact]
    public void Tower_pays_100()
    {
        Assert.Equal(100f, GoldTable.ForDeadUnit(GameConfig.TowerUnitId, isHero: false));
    }

    [Fact]
    public void Castle_pays_0()
    {
        Assert.Equal(0f, GoldTable.ForDeadUnit(GameConfig.CastleUnitId, isHero: false));
    }

    [Fact]
    public void Unknown_non_hero_pays_0()
    {
        Assert.Equal(0f, GoldTable.ForDeadUnit("chicken", isHero: false));
    }
}
