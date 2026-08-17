using System.Numerics;
using Realm.MapAPI;

namespace Realm.Maps;

public static class Coordinates
{
    public static Vector3 Resolve(IGameAPI api, string name)
    {
        if (api.HasCoordinate(name))
        {
            Vector3 min = api.GetCoordinateMin(name);
            Vector3 max = api.GetCoordinateMax(name);
            return (min + max) * 0.5f;
        }

        return CoordinateFallbacks.Get(name);
    }
}
