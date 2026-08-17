using System.Numerics;
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class HeroSystem
{
    private readonly Dictionary<int, int> _heroUnitByPlayer = new();
    private readonly Dictionary<int, int> _respawnTimerToPlayer = new();
    private bool _matchEnded;

    public void SpawnStartingHeroes(IGameAPI api)
    {
        TrySpawn(api, GameConfig.BluePlayerIndex, GameConfig.BlueHeroUnitId, "BlueFountain");
        TrySpawn(api, GameConfig.RedPlayerIndex, GameConfig.RedHeroUnitId, "RedFountain");
    }

    public void OnUnitDied(IGameAPI api, int unitId, int killerId)
    {
        _ = killerId;
        foreach (var pair in _heroUnitByPlayer)
        {
            if (pair.Value != unitId)
                continue;

            api.BroadcastMessage("Hero down. Respawning in 8s.");
            int handle = api.ScheduleTimer(GameConfig.HeroRespawnSeconds);
            _respawnTimerToPlayer[handle] = pair.Key;
            return;
        }
    }

    public void OnTimerExpired(IGameAPI api, int timerHandle)
    {
        if (!_respawnTimerToPlayer.TryGetValue(timerHandle, out int playerIndex))
            return;
        _respawnTimerToPlayer.Remove(timerHandle);
        if (_matchEnded)
            return;

        string unitType = playerIndex == GameConfig.BluePlayerIndex
            ? GameConfig.BlueHeroUnitId
            : GameConfig.RedHeroUnitId;
        string fountain = playerIndex == GameConfig.BluePlayerIndex
            ? "BlueFountain"
            : "RedFountain";
        TrySpawn(api, playerIndex, unitType, fountain);
    }

    public void NotifyMatchEnded()
    {
        _matchEnded = true;
    }

    private void TrySpawn(IGameAPI api, int playerIndex, string unitTypeId, string fountainName)
    {
        Vector3 position = Coordinates.Resolve(api, fountainName);
        var unit = api.SpawnUnitForPlayer(unitTypeId, position, playerIndex);
        if (unit == null)
        {
            MatchLog.Write(api.BroadcastMessage, MatchLog.SpawnFailed(unitTypeId, position, playerIndex));
            return;
        }

        OwnerTag.Set(unit, playerIndex);
        _heroUnitByPlayer[playerIndex] = unit.UniqueId;
        MatchLog.Write(api.BroadcastMessage, MatchLog.Spawned(unitTypeId, unit.UniqueId, position, playerIndex));
    }
}
