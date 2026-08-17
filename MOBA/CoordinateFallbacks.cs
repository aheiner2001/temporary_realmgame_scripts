using System.Numerics;

namespace Realm.Maps;

public static class CoordinateFallbacks
{
    public static Vector3 Get(string name)
    {
        return name switch
        {
            "BlueFountain" => new Vector3(16, 0, 16),
            "BlueCastle" => new Vector3(24, 0, 24),
            "BlueGate" => new Vector3(32, 0, 32),
            "BlueTower" => new Vector3(48, 0, 48),
            "MidLane1" => new Vector3(64, 0, 64),
            "RedTower" => new Vector3(80, 0, 80),
            "RedGate" => new Vector3(96, 0, 96),
            "RedCastle" => new Vector3(104, 0, 104),
            "RedFountain" => new Vector3(112, 0, 112),
            _ => Vector3.Zero
        };
    }
}
