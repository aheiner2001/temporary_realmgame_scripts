using System.Numerics;
using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class CoordinateFallbackTests
{
    [Theory]
    [InlineData("BlueFountain", 16f, 0f, 16f)]
    [InlineData("BlueCastle", 24f, 0f, 24f)]
    [InlineData("BlueGate", 32f, 0f, 32f)]
    [InlineData("BlueTower", 48f, 0f, 48f)]
    [InlineData("MidLane1", 64f, 0f, 64f)]
    [InlineData("RedTower", 80f, 0f, 80f)]
    [InlineData("RedGate", 96f, 0f, 96f)]
    [InlineData("RedCastle", 104f, 0f, 104f)]
    [InlineData("RedFountain", 112f, 0f, 112f)]
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
