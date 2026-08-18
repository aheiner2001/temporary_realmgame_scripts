# How to Select and Run Game Modes in the Realm Editor

This guide explains how to select, configure, and run different MOBA demo game modes directly within the **Realm Map Editor**.

---

## 1. Available Game Modes

| Enum Value | Mode Name | Description |
|---|---|---|
| `GameMode.ThreeLanePush` | **3-Lane Standard Push** | Classic 3-lane MOBA push with minion waves, towers, and base destruction victory. |
| `GameMode.ArenaBrawl` | **1v1 / 2v2 Arena Brawl** | Fast-paced single-lane battle with rapid waves, condensed bounds, and bonus gold. |
| `GameMode.ControlPoints` | **Capture & Hold** | 3 neutral capture zones across the map that drain enemy tickets down to 0. |
| `GameMode.BossObjective` | **Boss Objective Lane** | Single-lane push featuring a neutral Jungle Titan that grants team-wide damage buffs. |
| `GameMode.HeroDeathmatch` | **Hero Deathmatch** | Pure hero arena with healing orb pickups. First team to reach 15 hero kills wins. |

---

## 2. How to Select a Game Mode

1. Open **`MOBA/GameConfig.cs`** in your code editor.
2. Locate the **`CurrentMode`** property near the top of the file:

```csharp
public static class GameConfig
{
    // Change this value to whichever mode you want to test:
    public static GameMode CurrentMode = GameMode.ThreeLanePush;
    
    // ...
}
```

3. Set `CurrentMode` to your desired game mode:
   - `GameConfig.CurrentMode = GameMode.ThreeLanePush;`
   - `GameConfig.CurrentMode = GameMode.ArenaBrawl;`
   - `GameConfig.CurrentMode = GameMode.ControlPoints;`
   - `GameConfig.CurrentMode = GameMode.BossObjective;`
   - `GameConfig.CurrentMode = GameMode.HeroDeathmatch;`
4. **Save the file** (`Ctrl+S` on Windows / `Cmd+S` on macOS).

---

## 3. How to Run & Test in the Realm Editor

1. Launch the **Realm** application.
2. In the Map Editor, select **Open Map** (or open your workspace).
3. Select your **`MOBA`** directory (`/Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA`).
4. Click the **Test** button in the top menu bar.
5. The editor will automatically:
   - Compile the C# map scripts to WebAssembly (WASM).
   - Load the terrain, structures, and heroes.
   - Run the game match using the rules configured in `CurrentMode`.

---

## 4. Quick CLI Verification (Optional)

You can verify that the C# code compiles and all unit tests pass before launching the editor:

```bash
# Run the test suite
dotnet test MOBA.Tests/MOBA.Tests.csproj

# Test compile the WASM package
dotnet build MOBA/CustomMap.csproj
```

