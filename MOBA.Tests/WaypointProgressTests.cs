using System.Numerics;
using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class WaypointProgressTests
{
    private static readonly Vector3[] Path =
    {
        new(0, 0, 0),
        new(10, 0, 0),
        new(20, 0, 0)
    };

    [Fact]
    public void Empty_path_returns_zero()
    {
        Assert.Equal(0, WaypointProgress.AdvanceIndex(Vector3.Zero, Array.Empty<Vector3>(), 0, 4f));
    }

    [Fact]
    public void Far_from_current_keeps_index()
    {
        Assert.Equal(0, WaypointProgress.AdvanceIndex(new Vector3(10, 0, 0), Path, 0, 4f));
    }

    [Fact]
    public void Near_current_advances()
    {
        Assert.Equal(1, WaypointProgress.AdvanceIndex(new Vector3(0, 0, 0), Path, 0, 4f));
    }

    [Fact]
    public void Last_index_does_not_pass_end()
    {
        Assert.Equal(2, WaypointProgress.AdvanceIndex(new Vector3(20, 0, 0), Path, 2, 4f));
    }

    [Fact]
    public void Fallback_lane_has_five_waypoints_each_side()
    {
        var blue = LanePaths.BlueFromFallbacks();
        var red = LanePaths.RedFromFallbacks();
        Assert.Equal(5, blue.Length);
        Assert.Equal(5, red.Length);
        Assert.Equal(CoordinateFallbacks.Get("BlueGate"), blue[0]);
        Assert.Equal(CoordinateFallbacks.Get("RedCastle"), blue[4]);
        Assert.Equal(CoordinateFallbacks.Get("RedGate"), red[0]);
        Assert.Equal(CoordinateFallbacks.Get("BlueCastle"), red[4]);
    }
}
