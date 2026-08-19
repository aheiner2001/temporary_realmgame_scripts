namespace Realm.Maps;

using Realm.MapAPI;
using System.Numerics;
using System.Collections.Generic;

public class CustomMap : IWasmModule
{
    private IGameAPI? _api;
    private int _coreUnitId;
    private int _currentWave = 0;
    private int _waveTimerHandle;
    private bool _gameOver;

    public void Initialize(IGameAPI api)
    {
        _api = api;
        api.BroadcastMessage("Tower Defense script loaded!");

        // Setup teams and base
        api.SetPlayerTeam(GameConfig.PlayerIndex, 0);
        api.SetPlayerTeam(GameConfig.EnemyTeamIndex, 1);
        api.SetPlayersAllied(GameConfig.PlayerIndex, GameConfig.EnemyTeamIndex, false);

        var corePos = Coordinates.Resolve(api, "BlueCastle");
        var core = api.SpawnUnitForPlayer(GameConfig.DefenseCoreUnitId, corePos, GameConfig.PlayerIndex);
        _coreUnitId = core?.UniqueId ?? 0;

        // Spawn initial defense towers
        api.SpawnUnitForPlayer(GameConfig.TowerUnitId, Coordinates.Resolve(api, "BlueTower"), GameConfig.PlayerIndex);

        _waveTimerHandle = api.ScheduleRepeatingTimer(GameConfig.WaveIntervalSeconds);
        api.OnUnitDied += OnUnitDied;
        api.OnTimerExpired += OnTimerExpired;
    }

    public void Update(IGameAPI api, float delta)
    {
    }

    private void OnUnitDied(IUnit victim, IUnit? killer)
    {
        if (_api == null || _gameOver) return;
        if (victim?.UniqueId == _coreUnitId)
        {
            _gameOver = true;
            _api.BroadcastMessage("DEFEAT: The Defense Core has been destroyed!");
        }
    }

    private void OnTimerExpired(int timerHandle)
    {
        if (_api == null || _gameOver || timerHandle != _waveTimerHandle) return;

        _currentWave++;
        if (_currentWave > GameConfig.WavesTotal)
        {
            _gameOver = true;
            _api.BroadcastMessage("VICTORY: All waves survived!");
            return;
        }

        _api.BroadcastMessage($"Wave {_currentWave}/{GameConfig.WavesTotal} incoming!");
        var spawnPos = Coordinates.Resolve(_api, "RedCastle");
        var targetPos = Coordinates.Resolve(_api, "BlueCastle");

        for (int i = 0; i < GameConfig.MinionsPerWaveBase + _currentWave; i++)
        {
            var unit = _api.SpawnUnitForPlayer(GameConfig.AttackerMinionUnitId, spawnPos, GameConfig.EnemyTeamIndex);
            unit?.AttackMove(targetPos);
        }
    }
}
