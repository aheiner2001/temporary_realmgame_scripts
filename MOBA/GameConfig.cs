namespace Realm.Maps;

public enum GameMode
{
    ThreeLanePush = 0,
    ArenaBrawl = 1,
    ControlPoints = 2,
    BossObjective = 3,
    HeroDeathmatch = 4
}

public static class GameConfig
{
    public static GameMode CurrentMode = GameMode.ThreeLanePush;

    public const int BluePlayerIndex = 0;
    public const int RedPlayerIndex = 1;
    public const int BlueTeamIndex = 0;
    public const int RedTeamIndex = 1;

    public const float WaveIntervalSeconds = 30f;
    public const int MinionsPerWave = 3;
    public const int RangedMinionsPerWave = 1;
    public const float HeroRespawnSeconds = 8f;
    public const float WaypointReachDistance = 4f;
    public const float LaneOffset = 12f;

    public const float MinionKillGold = 15f;
    public const float HeroKillGold = 150f;
    public const float TowerKillGold = 100f;

    public const string MeleeMinionUnitId = "melee_minion";
    public const string RangedMinionUnitId = "ranged_minion";
    public const string BlueHeroUnitId = "adventurer";
    public const string RedHeroUnitId = "armored_dragon";
    public const string CastleUnitId = "castle";
    public const string TowerUnitId = "castle_tower_1";
    public const bool DebugChat = true;
}
