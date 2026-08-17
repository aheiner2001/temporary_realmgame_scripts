using System.Numerics;
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class TeamSetup
{
    public int BlueCastleId { get; private set; }
    public int RedCastleId { get; private set; }

    public void Apply(IGameAPI api, Lane lane)
    {
        _ = lane;
        api.SetPlayerTeam(GameConfig.BluePlayerIndex, GameConfig.BlueTeamIndex);
        api.SetPlayerTeam(GameConfig.RedPlayerIndex, GameConfig.RedTeamIndex);
        api.SetPlayersAllied(GameConfig.BluePlayerIndex, GameConfig.RedPlayerIndex, false);
        api.SetPlayerMaxPopulation(GameConfig.BluePlayerIndex, 200);
        api.SetPlayerMaxPopulation(GameConfig.RedPlayerIndex, 200);

        var blueCastle = Spawn(api, GameConfig.CastleUnitId, Coordinates.Resolve(api, "BlueCastle"), GameConfig.BluePlayerIndex);
        var redCastle = Spawn(api, GameConfig.CastleUnitId, Coordinates.Resolve(api, "RedCastle"), GameConfig.RedPlayerIndex);
        BlueCastleId = blueCastle?.UniqueId ?? 0;
        RedCastleId = redCastle?.UniqueId ?? 0;

        Spawn(api, GameConfig.TowerUnitId, Coordinates.Resolve(api, "BlueTower"), GameConfig.BluePlayerIndex);
        Spawn(api, GameConfig.TowerUnitId, Coordinates.Resolve(api, "RedTower"), GameConfig.RedPlayerIndex);
    }

    private static IUnit? Spawn(IGameAPI api, string unitTypeId, Vector3 position, int playerIndex)
    {
        var unit = api.SpawnUnitForPlayer(unitTypeId, position, playerIndex);
        if (unit != null)
            OwnerTag.Set(unit, playerIndex);
        return unit;
    }
}
