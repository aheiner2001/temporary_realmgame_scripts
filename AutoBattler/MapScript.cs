namespace Realm.Maps;

using Realm.MapAPI;

public class CustomMap : IWasmModule
{
    public void Initialize(IGameAPI api)
    {
        api.BroadcastMessage("AutoBattler script loaded!");
    }

    public void Update(IGameAPI api, float delta)
    {
    }
}
