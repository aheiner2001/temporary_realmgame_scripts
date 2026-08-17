using System.Numerics;

namespace Realm.Maps;

public static class WaypointProgress
{
    public static int AdvanceIndex(
        Vector3 position,
        IReadOnlyList<Vector3> path,
        int currentIndex,
        float reachDistance)
    {
        if (path.Count == 0)
            return 0;
        if (currentIndex >= path.Count - 1)
            return path.Count - 1;
        if (currentIndex < 0)
            currentIndex = 0;
        if (Vector3.Distance(position, path[currentIndex]) <= reachDistance)
            return currentIndex + 1;
        return currentIndex;
    }
}
