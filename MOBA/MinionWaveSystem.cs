using System.Numerics;
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class MinionWaveSystem
{
    private IReadOnlyList<Lane> _lanes = Array.Empty<Lane>();
    private int _waveTimerHandle;
    private readonly List<TrackedMinion> _minions = new();

    private sealed class TrackedMinion
    {
        public int UnitId;
        public int WaypointIndex;
        public IReadOnlyList<Vector3> Path = Array.Empty<Vector3>();
    }

    public void Start(IGameAPI api, IReadOnlyList<Lane> lanes)
    {
        _lanes = lanes;
        _waveTimerHandle = api.ScheduleRepeatingTimer(GameConfig.WaveIntervalSeconds);
    }

    public void OnTimerExpired(IGameAPI api, int timerHandle)
    {
        if (timerHandle != _waveTimerHandle)
            return;
        MatchLog.Write(api.BroadcastMessage, MatchLog.WaveTick(_lanes.Count));
        foreach (var lane in _lanes)
        {
            SpawnWave(api, GameConfig.BluePlayerIndex, lane.BluePath);
            SpawnWave(api, GameConfig.RedPlayerIndex, lane.RedPath);
        }
    }

    public void Tick(IGameAPI api, float delta)
    {
        _ = delta;
        for (int i = _minions.Count - 1; i >= 0; i--)
        {
            var minion = _minions[i];
            var unit = api.GetUnitById(minion.UnitId);
            if (unit == null || unit.IsDead)
            {
                _minions.RemoveAt(i);
                continue;
            }

            int next = WaypointProgress.AdvanceIndex(
                unit.Position,
                minion.Path,
                minion.WaypointIndex,
                GameConfig.WaypointReachDistance);
            if (next == minion.WaypointIndex)
                continue;
            minion.WaypointIndex = next;
            unit.AttackMove(minion.Path[next]);
        }
    }

    private void SpawnWave(IGameAPI api, int playerIndex, IReadOnlyList<Vector3> path)
    {
        if (path.Count == 0)
            return;

        Vector3 spawn = path[0];
        int firstDest = path.Count > 1 ? 1 : 0;
        SpawnGroup(api, GameConfig.MeleeMinionUnitId, GameConfig.MinionsPerWave, spawn, playerIndex, path, firstDest);
        SpawnGroup(api, GameConfig.RangedMinionUnitId, GameConfig.RangedMinionsPerWave, spawn, playerIndex, path, firstDest);
    }

    private void SpawnGroup(
        IGameAPI api,
        string unitTypeId,
        int count,
        Vector3 spawn,
        int playerIndex,
        IReadOnlyList<Vector3> path,
        int firstDest)
    {
        for (int i = 0; i < count; i++)
        {
            var unit = api.SpawnUnitForPlayer(unitTypeId, spawn, playerIndex);
            if (unit == null)
            {
                MatchLog.Write(api.BroadcastMessage, MatchLog.SpawnFailed(unitTypeId, spawn, playerIndex));
                continue;
            }
            OwnerTag.Set(unit, playerIndex);
            unit.AttackMove(path[firstDest]);
            _minions.Add(new TrackedMinion
            {
                UnitId = unit.UniqueId,
                WaypointIndex = firstDest,
                Path = path
            });
        }
    }
}
