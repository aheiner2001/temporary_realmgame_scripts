using Realm.MapAPI;

namespace Realm.Maps;

public sealed class GoldSystem
{
    public void OnUnitDied(IGameAPI api, int deadUnitId, int killerId)
    {
        var killer = api.GetUnitById(killerId);
        if (killer == null || !OwnerTag.TryGet(killer, out int playerIndex))
            return;

        var dead = api.GetUnitById(deadUnitId);
        if (dead == null)
            return;

        float gold = GoldTable.ForDeadUnit(dead.UnitId, dead.IsHero);
        if (gold <= 0f)
            return;

        api.AdjustPlayerGold(playerIndex, gold);
        api.SendMessageToPlayer(playerIndex, $"+{gold} gold");
    }
}
