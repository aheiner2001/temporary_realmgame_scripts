namespace Realm.Maps;

using System.Numerics;
using Realm.MapAPI;

public class CustomMap : IWasmModule
{
    public void Initialize(IGameAPI api)
    {
        Vector3 start = api.GetPlayerStartLocation(0);
        var unit = api.SpawnUnitForPlayer("adventurer", start, 0);
        if (unit == null)
        {
            api.BroadcastMessage("Spawn failed: adventurer");
            return;
        }

        Vector3 dest = unit.Position + new Vector3(20f, 0f, 0f);
        unit.AttackMove(dest);
        api.PanCameraTo(unit.Position, 0.1f);
        api.BroadcastMessage($"Spawned {unit.UnitId} id={unit.UniqueId} at {unit.Position}");
    }

    public void Update(IGameAPI api, float delta)
    {
    }
}