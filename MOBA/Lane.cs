using System.Numerics;
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class Lane
{
    public IReadOnlyList<Vector3> BluePath { get; }
    public IReadOnlyList<Vector3> RedPath { get; }

    public Lane(IReadOnlyList<Vector3> bluePath, IReadOnlyList<Vector3> redPath)
    {
        BluePath = bluePath;
        RedPath = redPath;
    }

    public static Lane FromFallbacks() =>
        new(LanePaths.BlueFromFallbacks(), LanePaths.RedFromFallbacks());

    public static Lane FromFallbacksOffset(float dz) =>
        new(LanePaths.Offset(LanePaths.BlueFromFallbacks(), dz),
            LanePaths.Offset(LanePaths.RedFromFallbacks(), dz));

    public static Lane FromCoordinates(IGameAPI api)
    {
        Vector3[] blue =
        {
            Coordinates.Resolve(api, "BlueGate"),
            Coordinates.Resolve(api, "BlueTower"),
            Coordinates.Resolve(api, "MidLane1"),
            Coordinates.Resolve(api, "RedTower"),
            Coordinates.Resolve(api, "RedCastle")
        };
        Vector3[] red =
        {
            Coordinates.Resolve(api, "RedGate"),
            Coordinates.Resolve(api, "RedTower"),
            Coordinates.Resolve(api, "MidLane1"),
            Coordinates.Resolve(api, "BlueTower"),
            Coordinates.Resolve(api, "BlueCastle")
        };
        return new Lane(blue, red);
    }

    public static IReadOnlyList<Lane> ThreeLanesFromCoordinates(IGameAPI api)
    {
        var mid = FromCoordinates(api);
        return
        [
            OffsetLane(mid, GameConfig.LaneOffset),
            mid,
            OffsetLane(mid, -GameConfig.LaneOffset)
        ];
    }

    private static Lane OffsetLane(Lane lane, float dz) =>
        new(Offset(lane.BluePath, dz), Offset(lane.RedPath, dz));

    private static Vector3[] Offset(IReadOnlyList<Vector3> path, float dz)
    {
        var result = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
            result[i] = new Vector3(path[i].X, path[i].Y, path[i].Z + dz);
        return result;
    }
}
