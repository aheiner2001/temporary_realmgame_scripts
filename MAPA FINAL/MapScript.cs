namespace Realm.Maps;

using Realm.MapAPI;

public class CustomMap : IWasmModule
{
    public void Initialize(IGameAPI api)
    {
        api.BroadcastMessage("Map script loaded!");
    }

    public void Update(IGameAPI api, float delta)
    {
    }
}
