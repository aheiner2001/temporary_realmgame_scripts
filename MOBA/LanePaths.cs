using System.Numerics;

namespace Realm.Maps;

public static class LanePaths
{
    public static Vector3[] BlueFromFallbacks() =>
    [
        CoordinateFallbacks.Get("BlueGate"),
        CoordinateFallbacks.Get("BlueTower"),
        CoordinateFallbacks.Get("MidLane1"),
        CoordinateFallbacks.Get("RedTower"),
        CoordinateFallbacks.Get("RedCastle")
    ];

    public static Vector3[] RedFromFallbacks() =>
    [
        CoordinateFallbacks.Get("RedGate"),
        CoordinateFallbacks.Get("RedTower"),
        CoordinateFallbacks.Get("MidLane1"),
        CoordinateFallbacks.Get("BlueTower"),
        CoordinateFallbacks.Get("BlueCastle")
    ];

    public static Vector3[] Offset(Vector3[] path, float dz)
    {
        var result = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
            result[i] = new Vector3(path[i].X, path[i].Y, path[i].Z + dz);
        return result;
    }
}
