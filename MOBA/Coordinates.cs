using System.Numerics;
using Realm.MapAPI;

namespace Realm.Maps;

public static class Coordinates
{
    public static Vector3 Resolve(IGameAPI api, string name)
    {
        if (api.HasCoordinate(name))
            return api.GetCoordinate(name).Center;
        return CoordinateFallbacks.Get(name);
    }
}
