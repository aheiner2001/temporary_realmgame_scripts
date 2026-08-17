namespace Realm.Maps;

using Realm.MapAPI;

public class CustomMap : IWasmModule
{
    private IGameAPI? _api;
    private MinionWaveSystem? _waves;
    private HeroSystem? _heroes;
    private GoldSystem? _gold;
    private WinSystem? _win;

    public void Initialize(IGameAPI api)
    {
        _api = api;
        api.BroadcastMessage("MOBA scripts loaded");
        api.BroadcastMessage("Guest Initialize started");

        var lanes = Lane.ThreeLanesFromCoordinates(api);
        var setup = new TeamSetup();
        setup.Apply(api, lanes[1]);
        _win = new WinSystem(setup.BlueCastleId, setup.RedCastleId);

        _waves = new MinionWaveSystem();
        _waves.Start(api, lanes);

        _gold = new GoldSystem();
        _heroes = new HeroSystem();
        _heroes.SpawnStartingHeroes(api);

        MatchLog.Write(api.BroadcastMessage, MatchLog.CastleSummary(setup.BlueCastleId, setup.RedCastleId));

        api.OnUnitDied += OnUnitDied;
        api.OnTimerExpired += OnTimerExpired;
        api.BroadcastMessage("Guest WasmModule initialized successfully");
    }

    public void Update(IGameAPI api, float delta)
    {
        _waves?.Tick(api, delta);
        _win?.Check(api);
        if (_win?.HasEnded == true)
            _heroes?.NotifyMatchEnded();
    }

    private void OnUnitDied(IUnit victim, IUnit? killer)
    {
        if (_api == null)
            return;
        int deadId = victim?.UniqueId ?? 0;
        int killerId = killer?.UniqueId ?? 0;
        _gold?.OnUnitDied(_api, deadId, killerId);
        _heroes?.OnUnitDied(_api, deadId, killerId);
        _win?.Check(_api);
        if (_win?.HasEnded == true)
            _heroes?.NotifyMatchEnded();
    }

    private void OnTimerExpired(int timerHandle)
    {
        if (_api == null)
            return;
        _waves?.OnTimerExpired(_api, timerHandle);
        _heroes?.OnTimerExpired(_api, timerHandle);
    }
}
