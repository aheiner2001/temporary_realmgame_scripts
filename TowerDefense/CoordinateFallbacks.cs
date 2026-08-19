using System.Numerics;

namespace Realm.Maps;

public static class CoordinateFallbacks
{
    public static Vector3 Get(string name)
    {
        return name switch
        {
            "BlueFountain" => new Vector3(-102.6f, 0f, 105.5f),
            "BlueCastle" => new Vector3(-91.6f, 0f, 97.6f),
            "BlueGate" => new Vector3(-83.3f, 0f, 91.3f),
            "BlueTower" => new Vector3(-59.1f, 0f, 62.8f),
            "MidLane1" => new Vector3(1.3f, 0f, -1.9f),
            "RedTower" => new Vector3(61.7f, 0f, -66.6f),
            "RedGate" => new Vector3(84.6f, 0f, -93.2f),
            "RedCastle" => new Vector3(94.2f, 0f, -101.4f),
            "RedFountain" => new Vector3(102.1f, 0f, -106.1f),
            _ => Vector3.Zero
        };
    }
}
