using Realm.MapAPI;

namespace Realm.Maps;

public static class OwnerTag
{
    public const string Key = "ownerPlayer";

    public static void Set(IUnit unit, int playerIndex)
    {
        unit.SetCustomData(Key, playerIndex.ToString());
    }

    public static bool TryGet(IUnit unit, out int playerIndex)
    {
        playerIndex = 0;
        if (!unit.HasCustomData(Key))
            return false;
        return int.TryParse(unit.GetCustomData(Key), out playerIndex);
    }
}
