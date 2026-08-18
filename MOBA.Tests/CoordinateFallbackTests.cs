using System.Numerics;
using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class CoordinateFallbackTests
{
    [Theory]
    [InlineData("BlueFountain", -102.6f, 0f, 105.5f)]
    [InlineData("BlueCastle", -91.6f, 0f, 97.6f)]
    [InlineData("BlueGate", -83.3f, 0f, 91.3f)]
    [InlineData("BlueTower", -59.1f, 0f, 62.8f)]
    [InlineData("MidLane1", 1.3f, 0f, -1.9f)]
    [InlineData("RedTower", 61.7f, 0f, -66.6f)]
    [InlineData("RedGate", 84.6f, 0f, -93.2f)]
    [InlineData("RedCastle", 94.2f, 0f, -101.4f)]
    [InlineData("RedFountain", 102.1f, 0f, -106.1f)]
    public void Known_names_return_spec_positions(string name, float x, float y, float z)
    {
        Assert.Equal(new Vector3(x, y, z), CoordinateFallbacks.Get(name));
    }

    [Fact]
    public void Unknown_name_returns_zero()
    {
        Assert.Equal(Vector3.Zero, CoordinateFallbacks.Get("NotARealPlace"));
    }
}
