using System.Globalization;
using System.Numerics;

namespace Realm.Maps;

public static class MatchLog
{
    public static void Write(Action<string>? broadcast, string message)
    {
        if (!GameConfig.DebugChat || broadcast == null)
            return;
        broadcast(message);
    }

    public static string Spawned(string unitType, int id, Vector3 position, int playerIndex)
    {
        return $"Debug: spawned {unitType} id={id} at {FormatPosition(position)} for player {playerIndex}";
    }

    public static string SpawnFailed(string unitType, Vector3 position, int playerIndex)
    {
        return $"Debug: spawn FAILED {unitType} at {FormatPosition(position)} for player {playerIndex}";
    }

    public static string CastleSummary(int blueCastleId, int redCastleId)
    {
        return $"Debug: blue castle id={blueCastleId} red castle id={redCastleId}";
    }

    public static string WaveTick(int laneCount)
    {
        return $"Debug: wave tick, {laneCount} lanes";
    }

    public static string GoldAwarded(float gold, int playerIndex, string deadUnitTypeId)
    {
        return $"Debug: +{FormatNumber(gold)} gold to player {playerIndex} ({deadUnitTypeId})";
    }

    public static string WinFired(MatchWinner winner)
    {
        return $"Debug: win fired {winner}";
    }

    public static string WinCheckSkipped()
    {
        return "Debug: win check skipped, castle id is 0";
    }

    private static string FormatPosition(Vector3 position)
    {
        return $"({FormatNumber(position.X)}, {FormatNumber(position.Y)}, {FormatNumber(position.Z)})";
    }

    private static string FormatNumber(float value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
