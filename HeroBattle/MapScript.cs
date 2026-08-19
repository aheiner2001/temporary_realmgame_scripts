namespace Realm.Maps;

using Realm.MapAPI;
using System.Numerics;
using System.Collections.Generic;

public class CustomMap : IWasmModule
{
    private IGameAPI? _api;
    private readonly HashSet<int> _p1LivingUnits = new();
    private readonly HashSet<int> _p2LivingUnits = new();
    private bool _gameOver;

    public void Initialize(IGameAPI api)
    {
        _api = api;
        api.BroadcastMessage("1v1 Hero Battle loaded!");

        api.SetPlayerTeam(GameConfig.Player1Index, 0);
        api.SetPlayerTeam(GameConfig.Player2Index, 1);
        api.SetPlayersAllied(GameConfig.Player1Index, GameConfig.Player2Index, false);

        var p1Pos = Coordinates.Resolve(api, "BlueFountain");
        var p2Pos = Coordinates.Resolve(api, "RedFountain");

        // Spawn P1 Hero + 5 Units
        var h1 = api.SpawnUnitForPlayer(GameConfig.Player1HeroId, p1Pos, GameConfig.Player1Index);
        if (h1 != null) _p1LivingUnits.Add(h1.UniqueId);

        for (int i = 0; i < GameConfig.UnitsPerPlayer; i++)
        {
            var u = api.SpawnUnitForPlayer(GameConfig.UnitTypeId, p1Pos + new Vector3(i * 1.5f, 0, 0), GameConfig.Player1Index);
            if (u != null) _p1LivingUnits.Add(u.UniqueId);
        }

        // Spawn P2 Hero + 5 Units
        var h2 = api.SpawnUnitForPlayer(GameConfig.Player2HeroId, p2Pos, GameConfig.Player2Index);
        if (h2 != null) _p2LivingUnits.Add(h2.UniqueId);

        for (int i = 0; i < GameConfig.UnitsPerPlayer; i++)
        {
            var u = api.SpawnUnitForPlayer(GameConfig.UnitTypeId, p2Pos + new Vector3(i * 1.5f, 0, 0), GameConfig.Player2Index);
            if (u != null) _p2LivingUnits.Add(u.UniqueId);
        }

        api.OnUnitDied += OnUnitDied;
    }

    public void Update(IGameAPI api, float delta)
    {
    }

    private void OnUnitDied(IUnit victim, IUnit? killer)
    {
        if (_api == null || _gameOver || victim == null) return;

        _p1LivingUnits.Remove(victim.UniqueId);
        _p2LivingUnits.Remove(victim.UniqueId);

        if (_p1LivingUnits.Count == 0)
        {
            _gameOver = true;
            _api.BroadcastMessage("Player 2 Wins! Player 1's hero and squad were eliminated.");
        }
        else if (_p2LivingUnits.Count == 0)
        {
            _gameOver = true;
            _api.BroadcastMessage("Player 1 Wins! Player 2's hero and squad were eliminated.");
        }
    }
}
