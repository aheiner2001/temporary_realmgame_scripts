namespace Realm.Maps;

public static class GameConfig
{
    public const int PlayerIndex = 0;
    public const int EnemyTeamIndex = 1;

    public const float WaveIntervalSeconds = 25f;
    public const int WavesTotal = 10;
    public const int MinionsPerWaveBase = 5;
    public const float WaypointReachDistance = 4f;

    public const string AttackerMinionUnitId = "melee_minion";
    public const string FastMinionUnitId = "ranged_minion";
    public const string DefenseCoreUnitId = "castle";
    public const string TowerUnitId = "castle_tower_1";
    public const bool DebugChat = true;
}
