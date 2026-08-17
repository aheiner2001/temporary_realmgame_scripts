namespace Realm.Maps;

public static class GoldTable
{
    public static float ForDeadUnit(string unitId, bool isHero)
    {
        if (unitId == GameConfig.CastleUnitId)
            return 0f;
        if (isHero)
            return GameConfig.HeroKillGold;
        if (unitId == GameConfig.TowerUnitId)
            return GameConfig.TowerKillGold;
        if (unitId == GameConfig.MeleeMinionUnitId || unitId == GameConfig.RangedMinionUnitId)
            return GameConfig.MinionKillGold;
        return 0f;
    }
}
