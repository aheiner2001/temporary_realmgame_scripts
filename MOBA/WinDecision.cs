namespace Realm.Maps;

public static class WinDecision
{
    public static MatchWinner FromCastleState(bool blueCastleDead, bool redCastleDead)
    {
        if (redCastleDead)
            return MatchWinner.Blue;
        if (blueCastleDead)
            return MatchWinner.Red;
        return MatchWinner.None;
    }
}
