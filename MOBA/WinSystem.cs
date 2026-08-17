using Realm.MapAPI;

namespace Realm.Maps;

public sealed class WinSystem
{
    private readonly int _blueCastleId;
    private readonly int _redCastleId;
    private bool _matchEnded;

    public WinSystem(int blueCastleId, int redCastleId)
    {
        _blueCastleId = blueCastleId;
        _redCastleId = redCastleId;
    }

    public bool HasEnded => _matchEnded;

    public void Check(IGameAPI api)
    {
        if (_matchEnded)
            return;

        var winner = WinDecision.FromCastleState(
            IsDead(api, _blueCastleId),
            IsDead(api, _redCastleId));
        if (winner == MatchWinner.None)
            return;

        _matchEnded = true;
        if (winner == MatchWinner.Blue)
        {
            api.TriggerPlayerVictory(GameConfig.BluePlayerIndex);
            api.TriggerPlayerDefeat(GameConfig.RedPlayerIndex, "Your castle was destroyed.");
            api.BroadcastMessage("Blue wins!");
        }
        else
        {
            api.TriggerPlayerVictory(GameConfig.RedPlayerIndex);
            api.TriggerPlayerDefeat(GameConfig.BluePlayerIndex, "Your castle was destroyed.");
            api.BroadcastMessage("Red wins!");
        }
    }

    private static bool IsDead(IGameAPI api, int id)
    {
        if (id == 0)
            return false;
        var unit = api.GetUnitById(id);
        return unit == null || unit.IsDead;
    }
}
