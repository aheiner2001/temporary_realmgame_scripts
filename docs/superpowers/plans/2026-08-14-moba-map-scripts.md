# MOBA Map Scripts Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a simple 1-lane MOBA demo in this map repo: minion waves, towers, two basic heroes, gold, fake respawn, destroy the enemy castle to win.

**Architecture:** `CustomMap` in `MOBA/MapScript.cs` is a thin coordinator. Pure logic (config, gold table, waypoints, win decision, coordinate fallbacks) is unit-tested with `dotnet test` and does not reference `IGameAPI`. Systems that call the engine (`TeamSetup`, `MinionWaveSystem`, `HeroSystem`, `GoldSystem`, `WinSystem`) are verified with in-game Play checklists. Do not modify the Realm engine in v1; fake hero revive by spawning a new unit.

**Tech Stack:** C# / .NET 10, Realm `IGameAPI` (`MOBA/lib/Realm.MapAPI.dll`), map compiles to WASI WebAssembly, xUnit for pure-logic tests.

## Global Constraints

- Do not edit `MOBA/terrain.json` (Isabella owns terrain).
- Do not modify the core engine (`Realm.Godot`, `Realm.MapAPI` source) in v1.
- `IGameAPI` stays generic; this repo only *calls* it.
- Keep `CustomMap : IWasmModule` as the map entry type in `MOBA/MapScript.cs`.
- Broadcast exactly `MOBA scripts loaded` from `Initialize`.
- Wave interval `30` seconds, `3` minions per side, hero respawn `8` seconds.
- Player `0` = Blue team `0`. Player `1` = Red team `1`. 1v1 only.
- Hero death uses fake revive (`SpawnUnitForPlayer` at fountain), not `ReviveUnit`.
- Tag every spawned unit with `SetCustomData("ownerPlayer", playerIndex.ToString())`.
- Unit type ids: `melee_minion`, `adventurer`, `armored_dragon`, `castle`, `castle_tower_1`.
- Chat respawn copy: `Hero down. Respawning in 8s.`
- Beginner home page: `docs/superpowers/tasks/README.md` (one checklist file per task). Full code lives in this plan.

## File Structure

**Create**

| File | Responsibility |
|---|---|
| `MOBA.Tests/MOBA.Tests.csproj` | net10.0 xUnit project; compiles pure-logic `.cs` files from `MOBA/` |
| `MOBA.Tests/GameConfigTests.cs` | Config constant tests |
| `MOBA.Tests/CoordinateFallbackTests.cs` | Fallback position tests |
| `MOBA.Tests/WaypointProgressTests.cs` | Lane waypoint index tests |
| `MOBA.Tests/WinDecisionTests.cs` | Castle-dead → winner tests |
| `MOBA.Tests/GoldTableTests.cs` | Kill bounty tests |
| `MOBA/GameConfig.cs` | All match numbers and unit id strings |
| `MOBA/CoordinateFallbacks.cs` | Hardcoded named positions (no `IGameAPI`) |
| `MOBA/LanePaths.cs` | Blue/Red fallback waypoint arrays (no `IGameAPI`) |
| `MOBA/WaypointProgress.cs` | Advance minion along a path (no `IGameAPI`) |
| `MOBA/WinDecision.cs` | `MatchWinner` from castle flags (no `IGameAPI`) |
| `MOBA/GoldTable.cs` | Gold amount from dead unit id / `isHero` (no `IGameAPI`) |
| `MOBA/Lane.cs` | BluePath / RedPath lists |
| `MOBA/OwnerTag.cs` | `ownerPlayer` custom-data helper (`IUnit`) |
| `MOBA/TeamSetup.cs` | Teams, population, spawn castle + tower |
| `MOBA/MinionWaveSystem.cs` | 30s waves + waypoint `AttackMove` |
| `MOBA/WinSystem.cs` | End match once when a castle dies |
| `MOBA/GoldSystem.cs` | Award gold on `OnUnitDied` |
| `MOBA/HeroSystem.cs` | Starting heroes + 8s fake respawn |

**Modify**

| File | Responsibility |
|---|---|
| `MOBA/Coordinates.cs` | `Resolve(api, name)` → named coordinate or fallback |
| `MOBA/MapScript.cs` | Wire systems; `Initialize` / `Update` / events |
| `MOBA/metadata.json` | Stats for minion, two heroes, castle, tower |

**Do not touch:** `MOBA/terrain.json`, `MOBA/WasmEntryPoint.cs`, engine repos.

**Calendar**

| When | Tasks |
|---|---|
| Today | 1 (setup + smoke), 2 (config tests) |
| Day 2 milestone | 3–8 (coords, lane, metadata, buildings, win, waves) |
| Next week demo | 9–10 (gold, heroes/respawn) |

---

### Task 1: Load the map and print a chat message

**Files:**
- Modify: `MOBA/MapScript.cs`
- Checklist: `docs/superpowers/tasks/01-setup-and-smoke.md`

**Interfaces:**
- Consumes: existing `CustomMap : IWasmModule`, `IGameAPI.BroadcastMessage(string)`
- Produces: chat line `MOBA scripts loaded` on match start

- [ ] **Step 1: Watch the three MOBA explainers (once, ~45 min)**

1. https://www.youtube.com/watch?v=ksh7_9NWNqw
2. https://www.youtube.com/watch?v=NquAYMAV1Sg
3. https://www.leagueoflegends.com/en-us/how-to-play/

You only need: two bases, lanes, minion waves, towers, one hero, destroy the nexus. Skip last-hitting, jungle, and ranked.

- [ ] **Step 2: Confirm the engine clone (already in this workspace)**

Engine GitHub: https://github.com/speige/Realm.git  
Local path: `Realm/` next to `MOBA/` (do not commit `Realm/` into the map repo).

You need a **full** checkout, including:

- `Realm/Realm.slnx`
- `Realm/Realm.MapAPI/`
- `Realm/Realm.Godot/project.godot`

If those are missing, from `Realm/` run `git checkout .`, or re-clone:

```bash
git clone https://github.com/speige/Realm.git Realm
```

Prerequisites from the engine repo:

- .NET SDK **10.0.x** (`Realm/global.json` pins `10.0.302`)
- Godot **4.7 Mono** (CI uses 4.7.0; the team may give you a modded editor build)

Do not spend more than 90 minutes stuck on Godot. Ask Devin which editor binary to run.

**Windows machine (recommended for Play):** clone **two** repos. Put the engine **next to `MOBA/`**, not in an unrelated folder.

```powershell
cd temporary_realmgame_scripts
git clone https://github.com/speige/Realm.git Realm
```

Layout must be:

```text
temporary_realmgame_scripts\
  MOBA\
  docs\
  Realm\     ← this clone, a separate git repo. Do not commit it into the map repo.
```

Also install Git (with Git LFS), .NET 10 SDK, and Godot 4.7 Mono. Do not copy the Mac `Realm` folder; clone fresh on Windows. Then LOAD `MOBA\` in the editor and TEST.

- [ ] **Step 3: LOAD `MOBA/` in the Realm map editor and TEST**

1. Run `Realm.Godot` (accept the asset license if prompted).
2. In the map editor, click **LOAD** and choose the `MOBA/` folder (the one with `terrain.json` and `MapScript.cs`).
3. Click **TEST** (single-player on the current editor map).

You should see Isabella’s terrain. An empty unit list is expected. The editor copies the map into a temp workspace and hot-reloads from there; edit files in `MOBA/` (or the temp workspace VS Code opens) so TEST picks up script changes.

- [ ] **Step 4: Write the smoke change in `MOBA/MapScript.cs`**

Replace the file with:

```csharp
namespace Realm.Maps;

using Realm.MapAPI;

public class CustomMap : IWasmModule
{
    public void Initialize(IGameAPI api)
    {
        api.BroadcastMessage("MOBA scripts loaded");
    }

    public void Update(IGameAPI api, float delta)
    {
    }
}
```

- [ ] **Step 5: Play-test (this task has no `dotnet test`)**

Run: Play the map in Realm.

Expected: chat/broadcast shows `MOBA scripts loaded`.

If nothing appears: you did not LOAD this `MOBA/` folder, or WASM did not rebuild after the script change. Click **TEST** again after saving. Breakpoints: `MOBA/.vscode/launch.json` → **Attach to Realm Game Host** (`processName`: `Realm.Godot`).

To refresh `MOBA/lib/Realm.MapAPI.dll` from engine source (only if the API changed):

```bash
dotnet build Realm/Realm.MapAPI/Realm.MapAPI.csproj
cp Realm/Realm.MapAPI/bin/Debug/net10.0/Realm.MapAPI.* MOBA/lib/
```

Windows (PowerShell):

```powershell
dotnet build Realm\Realm.MapAPI\Realm.MapAPI.csproj
Copy-Item Realm\Realm.MapAPI\bin\Debug\net10.0\Realm.MapAPI.* MOBA\lib\ -Force
```

- [ ] **Step 6: Commit**

```bash
git add MOBA/MapScript.cs
git commit -m "feat: broadcast MOBA scripts loaded on map init"
```

---

### Task 2: GameConfig + test project

**Files:**
- Create: `MOBA.Tests/MOBA.Tests.csproj`
- Create: `MOBA.Tests/GameConfigTests.cs`
- Create: `MOBA/GameConfig.cs`
- Checklist: `docs/superpowers/tasks/02-game-config.md`

**Interfaces:**
- Consumes: nothing
- Produces: `Realm.Maps.GameConfig` public consts listed in the spec

- [ ] **Step 1: Write the failing test**

Create `MOBA.Tests/MOBA.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.3">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <Compile Include="..\MOBA\GameConfig.cs" Link="GameConfig.cs" />
  </ItemGroup>
</Project>
```

If `dotnet test` errors on `net10.0`, change `TargetFramework` to `net8.0`. These files do not need WASM.

Create `MOBA.Tests/GameConfigTests.cs`:

```csharp
using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class GameConfigTests
{
    [Fact]
    public void Match_numbers_match_the_spec()
    {
        Assert.Equal(0, GameConfig.BluePlayerIndex);
        Assert.Equal(1, GameConfig.RedPlayerIndex);
        Assert.Equal(0, GameConfig.BlueTeamIndex);
        Assert.Equal(1, GameConfig.RedTeamIndex);
        Assert.Equal(30f, GameConfig.WaveIntervalSeconds);
        Assert.Equal(3, GameConfig.MinionsPerWave);
        Assert.Equal(8f, GameConfig.HeroRespawnSeconds);
        Assert.Equal(4f, GameConfig.WaypointReachDistance);
        Assert.Equal(15f, GameConfig.MinionKillGold);
        Assert.Equal(150f, GameConfig.HeroKillGold);
        Assert.Equal(100f, GameConfig.TowerKillGold);
        Assert.Equal("melee_minion", GameConfig.MeleeMinionUnitId);
        Assert.Equal("adventurer", GameConfig.BlueHeroUnitId);
        Assert.Equal("armored_dragon", GameConfig.RedHeroUnitId);
        Assert.Equal("castle", GameConfig.CastleUnitId);
        Assert.Equal("castle_tower_1", GameConfig.TowerUnitId);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter GameConfigTests -v n`

Expected: FAIL compiling with `The type or namespace name 'GameConfig' could not be found` or `GameConfig.cs could not be found`.

- [ ] **Step 3: Write minimal implementation**

Create `MOBA/GameConfig.cs`:

```csharp
namespace Realm.Maps;

public static class GameConfig
{
    public const int BluePlayerIndex = 0;
    public const int RedPlayerIndex = 1;
    public const int BlueTeamIndex = 0;
    public const int RedTeamIndex = 1;

    public const float WaveIntervalSeconds = 30f;
    public const int MinionsPerWave = 3;
    public const float HeroRespawnSeconds = 8f;
    public const float WaypointReachDistance = 4f;

    public const float MinionKillGold = 15f;
    public const float HeroKillGold = 150f;
    public const float TowerKillGold = 100f;

    public const string MeleeMinionUnitId = "melee_minion";
    public const string BlueHeroUnitId = "adventurer";
    public const string RedHeroUnitId = "armored_dragon";
    public const string CastleUnitId = "castle";
    public const string TowerUnitId = "castle_tower_1";
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter GameConfigTests -v n`

Expected: PASS (`Passed: 1`)

- [ ] **Step 5: Commit**

```bash
git add MOBA/GameConfig.cs MOBA.Tests/MOBA.Tests.csproj MOBA.Tests/GameConfigTests.cs
git commit -m "feat: add MOBA GameConfig and unit test project"
```

---

### Task 3: Coordinate fallbacks

**Files:**
- Create: `MOBA/CoordinateFallbacks.cs`
- Create: `MOBA.Tests/CoordinateFallbackTests.cs`
- Modify: `MOBA.Tests/MOBA.Tests.csproj` (add Compile include)
- Checklist: `docs/superpowers/tasks/03-coordinates.md`

**Interfaces:**
- Consumes: nothing (no `IGameAPI`)
- Produces: `CoordinateFallbacks.Get(string name) -> Vector3`

- [ ] **Step 1: Write the failing test**

Add to `MOBA.Tests/MOBA.Tests.csproj` inside the existing `ItemGroup` of Compile includes:

```xml
<Compile Include="..\MOBA\CoordinateFallbacks.cs" Link="CoordinateFallbacks.cs" />
```

Create `MOBA.Tests/CoordinateFallbackTests.cs`:

```csharp
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
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter CoordinateFallbackTests -v n`

Expected: FAIL with `CoordinateFallbacks` not found.

- [ ] **Step 3: Write minimal implementation**

Create `MOBA/CoordinateFallbacks.cs`:

```csharp
using System.Numerics;

namespace Realm.Maps;

public static class CoordinateFallbacks
{
    public static Vector3 Get(string name)
    {
        return name switch
        {
            "BlueFountain" => new Vector3(16, 0, 16),
            "BlueCastle" => new Vector3(24, 0, 24),
            "BlueGate" => new Vector3(32, 0, 32),
            "BlueTower" => new Vector3(48, 0, 48),
            "MidLane1" => new Vector3(64, 0, 64),
            "RedTower" => new Vector3(80, 0, 80),
            "RedGate" => new Vector3(96, 0, 96),
            "RedCastle" => new Vector3(104, 0, 104),
            "RedFountain" => new Vector3(112, 0, 112),
            _ => Vector3.Zero
        };
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter CoordinateFallbackTests -v n`

Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add MOBA/CoordinateFallbacks.cs MOBA.Tests/CoordinateFallbackTests.cs MOBA.Tests/MOBA.Tests.csproj
git commit -m "feat: add hardcoded MOBA coordinate fallbacks"
```

---

### Task 4: Waypoint progress + Lane

**Files:**
- Create: `MOBA/WaypointProgress.cs`
- Create: `MOBA/LanePaths.cs`
- Create: `MOBA/Lane.cs`
- Create: `MOBA.Tests/WaypointProgressTests.cs`
- Modify: `MOBA/Coordinates.cs`
- Modify: `MOBA.Tests/MOBA.Tests.csproj`
- Checklist: `docs/superpowers/tasks/04-lane-waypoints.md`

**Interfaces:**
- Consumes: `CoordinateFallbacks.Get`, later `IGameAPI.HasCoordinate` / `GetCoordinate`
- Produces:
  - `WaypointProgress.AdvanceIndex(Vector3 position, IReadOnlyList<Vector3> path, int currentIndex, float reachDistance) -> int`
  - `LanePaths.BlueFromFallbacks()` / `RedFromFallbacks()`
  - `Lane` with `IReadOnlyList<Vector3> BluePath`, `RedPath`
  - `Lane.FromFallbacks()` and `Lane.FromCoordinates(IGameAPI api)`
  - `Coordinates.Resolve(IGameAPI api, string name) -> Vector3`

- [ ] **Step 1: Write the failing tests**

Add Compile includes (do **not** include `Lane.cs` or `Coordinates.cs`; those need `IGameAPI`):

```xml
<Compile Include="..\MOBA\WaypointProgress.cs" Link="WaypointProgress.cs" />
<Compile Include="..\MOBA\LanePaths.cs" Link="LanePaths.cs" />
```

Create `MOBA.Tests/WaypointProgressTests.cs`:

```csharp
using System.Numerics;
using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class WaypointProgressTests
{
    private static readonly Vector3[] Path =
    {
        new(0, 0, 0),
        new(10, 0, 0),
        new(20, 0, 0)
    };

    [Fact]
    public void Empty_path_returns_zero()
    {
        Assert.Equal(0, WaypointProgress.AdvanceIndex(Vector3.Zero, Array.Empty<Vector3>(), 0, 4f));
    }

    [Fact]
    public void Far_from_current_keeps_index()
    {
        Assert.Equal(0, WaypointProgress.AdvanceIndex(new Vector3(10, 0, 0), Path, 0, 4f));
    }

    [Fact]
    public void Near_current_advances()
    {
        Assert.Equal(1, WaypointProgress.AdvanceIndex(new Vector3(0, 0, 0), Path, 0, 4f));
    }

    [Fact]
    public void Last_index_does_not_pass_end()
    {
        Assert.Equal(2, WaypointProgress.AdvanceIndex(new Vector3(20, 0, 0), Path, 2, 4f));
    }

    [Fact]
    public void Fallback_lane_has_five_waypoints_each_side()
    {
        var blue = LanePaths.BlueFromFallbacks();
        var red = LanePaths.RedFromFallbacks();
        Assert.Equal(5, blue.Length);
        Assert.Equal(5, red.Length);
        Assert.Equal(CoordinateFallbacks.Get("BlueGate"), blue[0]);
        Assert.Equal(CoordinateFallbacks.Get("RedCastle"), blue[4]);
        Assert.Equal(CoordinateFallbacks.Get("RedGate"), red[0]);
        Assert.Equal(CoordinateFallbacks.Get("BlueCastle"), red[4]);
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter WaypointProgressTests -v n`

Expected: FAIL with `WaypointProgress` / `LanePaths` not found.

- [ ] **Step 3: Write minimal implementation**

Create `MOBA/WaypointProgress.cs`:

```csharp
using System.Numerics;

namespace Realm.Maps;

public static class WaypointProgress
{
    public static int AdvanceIndex(
        Vector3 position,
        IReadOnlyList<Vector3> path,
        int currentIndex,
        float reachDistance)
    {
        if (path.Count == 0)
            return 0;
        if (currentIndex >= path.Count - 1)
            return path.Count - 1;
        if (currentIndex < 0)
            currentIndex = 0;
        if (Vector3.Distance(position, path[currentIndex]) <= reachDistance)
            return currentIndex + 1;
        return currentIndex;
    }
}
```

Create `MOBA/LanePaths.cs` (no `IGameAPI`, so tests can compile it):

```csharp
using System.Numerics;

namespace Realm.Maps;

public static class LanePaths
{
    public static Vector3[] BlueFromFallbacks() =>
    [
        CoordinateFallbacks.Get("BlueGate"),
        CoordinateFallbacks.Get("BlueTower"),
        CoordinateFallbacks.Get("MidLane1"),
        CoordinateFallbacks.Get("RedTower"),
        CoordinateFallbacks.Get("RedCastle")
    ];

    public static Vector3[] RedFromFallbacks() =>
    [
        CoordinateFallbacks.Get("RedGate"),
        CoordinateFallbacks.Get("RedTower"),
        CoordinateFallbacks.Get("MidLane1"),
        CoordinateFallbacks.Get("BlueTower"),
        CoordinateFallbacks.Get("BlueCastle")
    ];
}
```

Create `MOBA/Lane.cs`:

```csharp
using System.Numerics;
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class Lane
{
    public IReadOnlyList<Vector3> BluePath { get; }
    public IReadOnlyList<Vector3> RedPath { get; }

    public Lane(IReadOnlyList<Vector3> bluePath, IReadOnlyList<Vector3> redPath)
    {
        BluePath = bluePath;
        RedPath = redPath;
    }

    public static Lane FromFallbacks() =>
        new(LanePaths.BlueFromFallbacks(), LanePaths.RedFromFallbacks());

    public static Lane FromCoordinates(IGameAPI api)
    {
        Vector3[] blue =
        {
            Coordinates.Resolve(api, "BlueGate"),
            Coordinates.Resolve(api, "BlueTower"),
            Coordinates.Resolve(api, "MidLane1"),
            Coordinates.Resolve(api, "RedTower"),
            Coordinates.Resolve(api, "RedCastle")
        };
        Vector3[] red =
        {
            Coordinates.Resolve(api, "RedGate"),
            Coordinates.Resolve(api, "RedTower"),
            Coordinates.Resolve(api, "MidLane1"),
            Coordinates.Resolve(api, "BlueTower"),
            Coordinates.Resolve(api, "BlueCastle")
        };
        return new Lane(blue, red);
    }
}
```

Replace `MOBA/Coordinates.cs` with:

```csharp
using System.Numerics;
using Realm.MapAPI;

namespace Realm.Maps;

public static class Coordinates
{
    public static Vector3 Resolve(IGameAPI api, string name)
    {
        if (api.HasCoordinate(name))
            return api.GetCoordinate(name).Center;
        return CoordinateFallbacks.Get(name);
    }
}
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter WaypointProgressTests -v n`

Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add MOBA/WaypointProgress.cs MOBA/LanePaths.cs MOBA/Lane.cs MOBA/Coordinates.cs MOBA.Tests/WaypointProgressTests.cs MOBA.Tests/MOBA.Tests.csproj
git commit -m "feat: add lane waypoints and coordinate resolve"
```

---

### Task 5: Unit stats in metadata.json

**Files:**
- Modify: `MOBA/metadata.json` (`CustomUnits` adventurer/armored_dragon, add `melee_minion`; `CustomBuildings` castle and castle_tower_1)
- Checklist: `docs/superpowers/tasks/05-unit-stats.md`

**Interfaces:**
- Consumes: existing models `adventurer.glb`, `armored_dragon.glb`, `castle.glb`, `castle_tower_1.glb`
- Produces: engine-readable stats for those unit ids

- [ ] **Step 1: Replace the two CustomUnits entries and add melee_minion**

In `MOBA/metadata.json`, replace the `CustomUnits` array with:

```json
  "CustomUnits": [
    {
      "UnitId": "adventurer",
      "Name": "adventurer",
      "Description": "Blue melee hero",
      "PathingType": 9,
      "ModelPath": "adventurer.glb",
      "MaxHp": 400,
      "Damage": 25,
      "Range": 1.5,
      "Armor": 2,
      "Speed": 4.0,
      "AttackType": "melee",
      "IsHero": true,
      "GoldBounty": 150
    },
    {
      "UnitId": "armored_dragon",
      "Name": "armored_dragon",
      "Description": "Red ranged hero",
      "PathingType": 9,
      "ModelPath": "armored_dragon.glb",
      "MaxHp": 350,
      "Damage": 20,
      "Range": 8,
      "Armor": 2,
      "Speed": 4.0,
      "AttackType": "ranged",
      "IsHero": true,
      "GoldBounty": 150
    },
    {
      "UnitId": "melee_minion",
      "Name": "melee_minion",
      "Description": "Lane minion",
      "PathingType": 9,
      "ModelPath": "adventurer.glb",
      "MaxHp": 80,
      "Damage": 8,
      "Range": 1.5,
      "Armor": 0,
      "Speed": 3.5,
      "AttackType": "melee",
      "IsHero": false,
      "GoldBounty": 15
    }
  ],
```

- [ ] **Step 2: Add stats to castle and castle_tower_1**

Replace the `castle` object with:

```json
    {
      "UnitId": "castle",
      "Name": "castle",
      "Description": "Team base. Destroy to win.",
      "PathingType": 32,
      "ModelPath": "castle.glb",
      "MaxHp": 3000,
      "Damage": 0,
      "Range": 0,
      "Armor": 5,
      "Speed": 0,
      "AttackType": "none",
      "IsHero": false,
      "GoldBounty": 0
    },
```

Replace the `castle_tower_1` object with:

```json
    {
      "UnitId": "castle_tower_1",
      "Name": "castle_tower_1",
      "Description": "Lane tower",
      "PathingType": 32,
      "ModelPath": "castle_tower_1.glb",
      "MaxHp": 1200,
      "Damage": 40,
      "Range": 12,
      "Armor": 5,
      "Speed": 0,
      "AttackType": "ranged",
      "IsHero": false,
      "GoldBounty": 100
    },
```

Leave every other building unchanged.

- [ ] **Step 3: Validate JSON**

Run: `python3 -c "import json; json.load(open('MOBA/metadata.json')); print('ok')"`

Expected: `ok`

- [ ] **Step 4: Play-test**

Play the map. No new units required yet. Confirm the map still loads (bad JSON would fail load).

Expected: map loads; chat still shows `MOBA scripts loaded`.

- [ ] **Step 5: Commit**

```bash
git add MOBA/metadata.json
git commit -m "feat: add MOBA hero, minion, castle, and tower stats"
```

---

### Task 6: Win decision (pure)

**Files:**
- Create: `MOBA/WinDecision.cs`
- Create: `MOBA.Tests/WinDecisionTests.cs`
- Modify: `MOBA.Tests/MOBA.Tests.csproj`
- Checklist: `docs/superpowers/tasks/06-win-decision.md`

**Interfaces:**
- Consumes: nothing
- Produces: `enum MatchWinner { None, Blue, Red }` and `WinDecision.FromCastleState(bool blueCastleDead, bool redCastleDead)`

- [ ] **Step 1: Write the failing test**

Add:

```xml
<Compile Include="..\MOBA\WinDecision.cs" Link="WinDecision.cs" />
```

Create `MOBA.Tests/WinDecisionTests.cs`:

```csharp
using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class WinDecisionTests
{
    [Fact]
    public void Neither_dead_is_none()
    {
        Assert.Equal(MatchWinner.None, WinDecision.FromCastleState(false, false));
    }

    [Fact]
    public void Red_dead_blue_wins()
    {
        Assert.Equal(MatchWinner.Blue, WinDecision.FromCastleState(false, true));
    }

    [Fact]
    public void Blue_dead_red_wins()
    {
        Assert.Equal(MatchWinner.Red, WinDecision.FromCastleState(true, false));
    }

    [Fact]
    public void Both_dead_blue_wins()
    {
        Assert.Equal(MatchWinner.Blue, WinDecision.FromCastleState(true, true));
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter WinDecisionTests -v n`

Expected: FAIL with `WinDecision` not found.

- [ ] **Step 3: Write minimal implementation**

Create `MOBA/WinDecision.cs`:

```csharp
namespace Realm.Maps;

public enum MatchWinner
{
    None,
    Blue,
    Red
}

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
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter WinDecisionTests -v n`

Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add MOBA/WinDecision.cs MOBA.Tests/WinDecisionTests.cs MOBA.Tests/MOBA.Tests.csproj
git commit -m "feat: add castle win decision"
```

---

### Task 7: Team setup, owner tags, win system, wire Initialize

**Files:**
- Create: `MOBA/OwnerTag.cs`
- Create: `MOBA/TeamSetup.cs`
- Create: `MOBA/WinSystem.cs`
- Modify: `MOBA/MapScript.cs`
- Checklist: `docs/superpowers/tasks/07-teams-buildings-win.md`

**Interfaces:**
- Consumes: `GameConfig`, `Coordinates.Resolve`, `Lane.FromCoordinates`, `WinDecision.FromCastleState`, `IGameAPI.SetPlayerTeam`, `SetPlayersAllied`, `SetPlayerMaxPopulation`, `SpawnUnitForPlayer`, `TriggerPlayerVictory`, `TriggerPlayerDefeat`, `BroadcastMessage`, `GetUnitById`
- Produces:
  - `OwnerTag.Key == "ownerPlayer"`; `OwnerTag.Set(IUnit, int)`; `OwnerTag.TryGet(IUnit, out int)`
  - `TeamSetup.BlueCastleId`, `RedCastleId`, `Apply(IGameAPI api, Lane lane)`
  - `WinSystem.HasEnded`, `Check(IGameAPI api)`
  - Castles and towers exist after `Initialize`

Heroes are **not** spawned in this task (`HeroSystem` is Task 10). `TeamSetup.Apply` does teams + buildings only.

- [ ] **Step 1: Write OwnerTag, TeamSetup, WinSystem**

Create `MOBA/OwnerTag.cs`:

```csharp
using Realm.MapAPI;

namespace Realm.Maps;

public static class OwnerTag
{
    public const string Key = "ownerPlayer";

    public static void Set(IUnit unit, int playerIndex)
    {
        unit.SetCustomData(Key, playerIndex.ToString());
    }

    public static bool TryGet(IUnit unit, out int playerIndex)
    {
        playerIndex = 0;
        if (!unit.HasCustomData(Key))
            return false;
        return int.TryParse(unit.GetCustomData(Key), out playerIndex);
    }
}
```

Create `MOBA/TeamSetup.cs`:

```csharp
using System.Numerics;
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class TeamSetup
{
    public int BlueCastleId { get; private set; }
    public int RedCastleId { get; private set; }

    public void Apply(IGameAPI api, Lane lane)
    {
        api.SetPlayerTeam(GameConfig.BluePlayerIndex, GameConfig.BlueTeamIndex);
        api.SetPlayerTeam(GameConfig.RedPlayerIndex, GameConfig.RedTeamIndex);
        api.SetPlayersAllied(GameConfig.BluePlayerIndex, GameConfig.RedPlayerIndex, false);
        api.SetPlayerMaxPopulation(GameConfig.BluePlayerIndex, 200);
        api.SetPlayerMaxPopulation(GameConfig.RedPlayerIndex, 200);

        var blueCastle = Spawn(api, GameConfig.CastleUnitId, Coordinates.Resolve(api, "BlueCastle"), GameConfig.BluePlayerIndex);
        var redCastle = Spawn(api, GameConfig.CastleUnitId, Coordinates.Resolve(api, "RedCastle"), GameConfig.RedPlayerIndex);
        BlueCastleId = blueCastle?.UniqueId ?? 0;
        RedCastleId = redCastle?.UniqueId ?? 0;

        Spawn(api, GameConfig.TowerUnitId, Coordinates.Resolve(api, "BlueTower"), GameConfig.BluePlayerIndex);
        Spawn(api, GameConfig.TowerUnitId, Coordinates.Resolve(api, "RedTower"), GameConfig.RedPlayerIndex);
    }

    private static IUnit? Spawn(IGameAPI api, string unitTypeId, Vector3 position, int playerIndex)
    {
        var unit = api.SpawnUnitForPlayer(unitTypeId, position, playerIndex);
        if (unit != null)
            OwnerTag.Set(unit, playerIndex);
        return unit;
    }
}
```

Create `MOBA/WinSystem.cs`:

```csharp
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
```

- [ ] **Step 2: Wire MapScript**

Replace `MOBA/MapScript.cs` with:

```csharp
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

        var lane = Lane.FromCoordinates(api);
        var setup = new TeamSetup();
        setup.Apply(api, lane);
        _win = new WinSystem(setup.BlueCastleId, setup.RedCastleId);

        api.OnUnitDied += OnUnitDied;
        api.OnTimerExpired += OnTimerExpired;
    }

    public void Update(IGameAPI api, float delta)
    {
        _waves?.Tick(api, delta);
        _win?.Check(api);
        if (_win?.HasEnded == true)
            _heroes?.NotifyMatchEnded();
    }

    private void OnUnitDied(IUnit victim, IUnit killer)
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
```

`MinionWaveSystem`, `HeroSystem`, and `GoldSystem` are referenced but not created yet. Add three stub types so this compiles.

Create `MOBA/MinionWaveSystem.cs`:

```csharp
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class MinionWaveSystem
{
    public void Start(IGameAPI api, Lane lane) { }

    public void OnTimerExpired(IGameAPI api, int timerHandle) { }

    public void Tick(IGameAPI api, float delta) { }
}
```

Create `MOBA/HeroSystem.cs`:

```csharp
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class HeroSystem
{
    public void SpawnStartingHeroes(IGameAPI api) { }

    public void OnUnitDied(IGameAPI api, int unitId, int killerId) { }

    public void OnTimerExpired(IGameAPI api, int timerHandle) { }

    public void NotifyMatchEnded() { }
}
```

Create `MOBA/GoldSystem.cs`:

```csharp
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class GoldSystem
{
    public void OnUnitDied(IGameAPI api, int deadUnitId, int killerId) { }
}
```

`OnUnitDied` in `Realm/Realm.MapAPI/IGameAPI.cs` is `event Action<IUnit, IUnit?>?` (victim, optional killer). Use the `(IUnit victim, IUnit killer)` handler above. If a future API drops the `IUnit` args, fall back to:

```csharp
private void OnUnitDied(int deadId, int killerId)
{
    if (_api == null)
        return;
    _gold?.OnUnitDied(_api, deadId, killerId);
    _heroes?.OnUnitDied(_api, deadId, killerId);
    _win?.Check(_api);
    if (_win?.HasEnded == true)
        _heroes?.NotifyMatchEnded();
}
```

- [ ] **Step 3: Play-test buildings**

Run: Play the map.

Expected:

- Chat: `MOBA scripts loaded`
- Two castles visible near fallback corners (or named coords if Isabella added them)
- Two towers on the diagonal between them
- If they spawn in a rock or underground, read a unit `Position` in the debugger and update **only** `CoordinateFallbacks.cs` numbers, then re-run Task 3 tests

- [ ] **Step 4: Play-test win**

Select the Red castle (or use debug kill if you have one) and destroy it.

Expected: `Blue wins!` and the match ends. Doing it again must not double-fire.

- [ ] **Step 5: Commit**

```bash
git add MOBA/OwnerTag.cs MOBA/TeamSetup.cs MOBA/WinSystem.cs MOBA/MapScript.cs MOBA/MinionWaveSystem.cs MOBA/HeroSystem.cs MOBA/GoldSystem.cs
git commit -m "feat: spawn team buildings and end match when a castle dies"
```

---

### Task 8: Minion waves (day-2 milestone)

**Files:**
- Modify: `MOBA/MinionWaveSystem.cs` (replace stub)
- Modify: `MOBA/MapScript.cs` (`Initialize` starts waves)
- Checklist: `docs/superpowers/tasks/08-minion-waves.md`

**Interfaces:**
- Consumes: `Lane`, `WaypointProgress.AdvanceIndex`, `GameConfig.WaveIntervalSeconds`, `MinionsPerWave`, `MeleeMinionUnitId`, `IGameAPI.ScheduleRepeatingTimer`, `SpawnUnitForPlayer`, `IUnit.AttackMove`
- Produces: working `MinionWaveSystem.Start(IGameAPI api, Lane lane)`, `OnTimerExpired`, `Tick`

- [ ] **Step 1: Implement MinionWaveSystem**

Replace `MOBA/MinionWaveSystem.cs` with:

```csharp
using System.Numerics;
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class MinionWaveSystem
{
    private Lane? _lane;
    private int _waveTimerHandle;
    private readonly List<TrackedMinion> _minions = new();

    private sealed class TrackedMinion
    {
        public int UnitId;
        public int WaypointIndex;
        public IReadOnlyList<Vector3> Path = Array.Empty<Vector3>();
    }

    public void Start(IGameAPI api, Lane lane)
    {
        _lane = lane;
        _waveTimerHandle = api.ScheduleRepeatingTimer(GameConfig.WaveIntervalSeconds);
    }

    public void OnTimerExpired(IGameAPI api, int timerHandle)
    {
        if (_lane == null || timerHandle != _waveTimerHandle)
            return;
        SpawnWave(api, GameConfig.BluePlayerIndex, _lane.BluePath);
        SpawnWave(api, GameConfig.RedPlayerIndex, _lane.RedPath);
    }

    public void Tick(IGameAPI api, float delta)
    {
        for (int i = _minions.Count - 1; i >= 0; i--)
        {
            var minion = _minions[i];
            var unit = api.GetUnitById(minion.UnitId);
            if (unit == null || unit.IsDead)
            {
                _minions.RemoveAt(i);
                continue;
            }

            int next = WaypointProgress.AdvanceIndex(
                unit.Position,
                minion.Path,
                minion.WaypointIndex,
                GameConfig.WaypointReachDistance);
            if (next == minion.WaypointIndex)
                continue;
            minion.WaypointIndex = next;
            unit.AttackMove(minion.Path[next]);
        }
    }

    private void SpawnWave(IGameAPI api, int playerIndex, IReadOnlyList<Vector3> path)
    {
        if (path.Count == 0)
            return;

        Vector3 spawn = path[0];
        int firstDest = path.Count > 1 ? 1 : 0;
        for (int i = 0; i < GameConfig.MinionsPerWave; i++)
        {
            var unit = api.SpawnUnitForPlayer(GameConfig.MeleeMinionUnitId, spawn, playerIndex);
            if (unit == null)
                continue;
            OwnerTag.Set(unit, playerIndex);
            unit.AttackMove(path[firstDest]);
            _minions.Add(new TrackedMinion
            {
                UnitId = unit.UniqueId,
                WaypointIndex = firstDest,
                Path = path
            });
        }
    }
}
```

- [ ] **Step 2: Start waves from Initialize**

In `MOBA/MapScript.cs` `Initialize`, after `_win = new WinSystem(...)` add:

```csharp
        _waves = new MinionWaveSystem();
        _waves.Start(api, lane);
```

- [ ] **Step 3: Run pure tests (still pass)**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj -v n`

Expected: PASS (all existing tests)

- [ ] **Step 4: Play-test waves (day-2 milestone)**

Run: Play the map and wait 30 seconds.

Expected:

- 3 Blue minions spawn at BlueGate and attack-move toward Red castle
- 3 Red minions spawn at RedGate and attack-move toward Blue castle
- They fight if they meet
- Towers damage enemy minions that get in range
- Destroying a castle still wins

This is the **2-day slice**. You can show this to Devin even without heroes.

- [ ] **Step 5: Commit**

```bash
git add MOBA/MinionWaveSystem.cs MOBA/MapScript.cs
git commit -m "feat: spawn minion waves that attack-move down the lane"
```

---

### Task 9: Gold table + GoldSystem

**Files:**
- Create: `MOBA/GoldTable.cs`
- Create: `MOBA.Tests/GoldTableTests.cs`
- Modify: `MOBA/GoldSystem.cs`
- Modify: `MOBA/MapScript.cs` (construct `GoldSystem`)
- Modify: `MOBA.Tests/MOBA.Tests.csproj`
- Checklist: `docs/superpowers/tasks/09-gold.md`

**Interfaces:**
- Consumes: `GameConfig` gold constants and unit ids; `OwnerTag.TryGet`; `IGameAPI.GetUnitById`, `AdjustPlayerGold`, `SendMessageToPlayer`
- Produces: `GoldTable.ForDeadUnit(string unitId, bool isHero) -> float`; working `GoldSystem.OnUnitDied`

- [ ] **Step 1: Write the failing test**

Add:

```xml
<Compile Include="..\MOBA\GoldTable.cs" Link="GoldTable.cs" />
```

Create `MOBA.Tests/GoldTableTests.cs`:

```csharp
using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class GoldTableTests
{
    [Fact]
    public void Minion_pays_15()
    {
        Assert.Equal(15f, GoldTable.ForDeadUnit(GameConfig.MeleeMinionUnitId, isHero: false));
    }

    [Fact]
    public void Hero_pays_150()
    {
        Assert.Equal(150f, GoldTable.ForDeadUnit(GameConfig.BlueHeroUnitId, isHero: true));
        Assert.Equal(150f, GoldTable.ForDeadUnit(GameConfig.RedHeroUnitId, isHero: true));
    }

    [Fact]
    public void Tower_pays_100()
    {
        Assert.Equal(100f, GoldTable.ForDeadUnit(GameConfig.TowerUnitId, isHero: false));
    }

    [Fact]
    public void Castle_pays_0()
    {
        Assert.Equal(0f, GoldTable.ForDeadUnit(GameConfig.CastleUnitId, isHero: false));
    }

    [Fact]
    public void Unknown_non_hero_pays_0()
    {
        Assert.Equal(0f, GoldTable.ForDeadUnit("chicken", isHero: false));
    }
}
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter GoldTableTests -v n`

Expected: FAIL with `GoldTable` not found.

- [ ] **Step 3: Write GoldTable and GoldSystem**

Create `MOBA/GoldTable.cs`:

```csharp
namespace Realm.Maps;

public static class GoldTable
{
    public static float ForDeadUnit(string unitId, bool isHero)
    {
        if (unitId == GameConfig.CastleUnitId)
            return 0f;
        if (isHero)
            return GameConfig.HeroKillGold;
        if (unitId == GameConfig.TowerUnitId)
            return GameConfig.TowerKillGold;
        if (unitId == GameConfig.MeleeMinionUnitId)
            return GameConfig.MinionKillGold;
        return 0f;
    }
}
```

Replace `MOBA/GoldSystem.cs` with:

```csharp
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class GoldSystem
{
    public void OnUnitDied(IGameAPI api, int deadUnitId, int killerId)
    {
        var killer = api.GetUnitById(killerId);
        if (killer == null || !OwnerTag.TryGet(killer, out int playerIndex))
            return;

        var dead = api.GetUnitById(deadUnitId);
        if (dead == null)
            return;

        float gold = GoldTable.ForDeadUnit(dead.UnitId, dead.IsHero);
        if (gold <= 0f)
            return;

        api.AdjustPlayerGold(playerIndex, gold);
        api.SendMessageToPlayer(playerIndex, $"+{gold} gold");
    }
}
```

In `MapScript.Initialize`, after constructing `_win`, add:

```csharp
        _gold = new GoldSystem();
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter GoldTableTests -v n`

Expected: PASS

- [ ] **Step 5: Play-test gold**

Run: Play, wait for waves, let minions die.

Expected: the player who got the last hit sees `+15 gold` (or the killer’s owner). Invalid killer does not crash.

- [ ] **Step 6: Commit**

```bash
git add MOBA/GoldTable.cs MOBA/GoldSystem.cs MOBA/MapScript.cs MOBA.Tests/GoldTableTests.cs MOBA.Tests/MOBA.Tests.csproj
git commit -m "feat: award gold to the killing player"
```

---

### Task 10: Heroes and fake respawn (supervisor demo)

**Files:**
- Modify: `MOBA/HeroSystem.cs` (replace stub)
- Modify: `MOBA/MapScript.cs` (`SpawnStartingHeroes`)
- Checklist: `docs/superpowers/tasks/10-heroes-respawn.md`

**Interfaces:**
- Consumes: `GameConfig` hero ids and `HeroRespawnSeconds`; `Coordinates.Resolve`; `IGameAPI.IsPlayerActive`, `SpawnUnitForPlayer`, `ScheduleTimer`, `BroadcastMessage`
- Produces: working `HeroSystem.SpawnStartingHeroes`, `OnUnitDied`, `OnTimerExpired`, `NotifyMatchEnded`

- [ ] **Step 1: Implement HeroSystem**

Replace `MOBA/HeroSystem.cs` with:

```csharp
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class HeroSystem
{
    private readonly Dictionary<int, int> _heroUnitByPlayer = new();
    private readonly Dictionary<int, int> _respawnTimerToPlayer = new();
    private bool _matchEnded;

    public void SpawnStartingHeroes(IGameAPI api)
    {
        TrySpawn(api, GameConfig.BluePlayerIndex, GameConfig.BlueHeroUnitId, "BlueFountain");
        TrySpawn(api, GameConfig.RedPlayerIndex, GameConfig.RedHeroUnitId, "RedFountain");
    }

    public void OnUnitDied(IGameAPI api, int unitId, int killerId)
    {
        foreach (var pair in _heroUnitByPlayer)
        {
            if (pair.Value != unitId)
                continue;

            api.BroadcastMessage("Hero down. Respawning in 8s.");
            int handle = api.ScheduleTimer(GameConfig.HeroRespawnSeconds);
            _respawnTimerToPlayer[handle] = pair.Key;
            return;
        }
    }

    public void OnTimerExpired(IGameAPI api, int timerHandle)
    {
        if (!_respawnTimerToPlayer.TryGetValue(timerHandle, out int playerIndex))
            return;
        _respawnTimerToPlayer.Remove(timerHandle);
        if (_matchEnded)
            return;

        string unitType = playerIndex == GameConfig.BluePlayerIndex
            ? GameConfig.BlueHeroUnitId
            : GameConfig.RedHeroUnitId;
        string fountain = playerIndex == GameConfig.BluePlayerIndex
            ? "BlueFountain"
            : "RedFountain";
        TrySpawn(api, playerIndex, unitType, fountain);
    }

    public void NotifyMatchEnded()
    {
        _matchEnded = true;
    }

    private void TrySpawn(IGameAPI api, int playerIndex, string unitTypeId, string fountainName)
    {
        if (!api.IsPlayerActive(playerIndex))
            return;

        var unit = api.SpawnUnitForPlayer(
            unitTypeId,
            Coordinates.Resolve(api, fountainName),
            playerIndex);
        if (unit == null)
            return;
        OwnerTag.Set(unit, playerIndex);
        _heroUnitByPlayer[playerIndex] = unit.UniqueId;
    }
}
```

If Red is a computer slot and `IsPlayerActive` is false, Red will have no hero. If that happens in Play, remove the `IsPlayerActive` check for v1 so both sides always get a hero (minions already spawn for player 1). Spec prefers skip when inactive; Play-test decides. Default: keep the check. If Red has no hero and you need the demo, delete the `IsPlayerActive` guard only.

- [ ] **Step 2: Call SpawnStartingHeroes from Initialize**

In `MOBA/MapScript.cs` `Initialize`, after `_gold = new GoldSystem();` add:

```csharp
        _heroes = new HeroSystem();
        _heroes.SpawnStartingHeroes(api);
```

- [ ] **Step 3: Run all unit tests**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj -v n`

Expected: PASS

- [ ] **Step 4: Play-test the supervisor demo**

Expected:

1. Chat `MOBA scripts loaded`
2. You control an `adventurer` at Blue fountain
3. Red has an `armored_dragon` at Red fountain (if that slot is active)
4. Waves still spawn at 30s
5. Kill your hero → `Hero down. Respawning in 8s.` → new hero at fountain ~8s later
6. Gold still awards; gold is on the player so it survives respawn
7. Destroy enemy castle → that team wins
8. After win, a pending respawn must **not** spawn a new hero

- [ ] **Step 5: Commit**

```bash
git add MOBA/HeroSystem.cs MOBA/MapScript.cs
git commit -m "feat: spawn heroes and fake-revive after 8 seconds"
```

---

## Self-review

**Spec coverage**

| Spec item | Task |
|---|---|
| Clone engine / map loads / chat | 1 |
| GameConfig numbers | 2 |
| Coordinate fallbacks + Resolve | 3–4 |
| Lane paths | 4 |
| metadata stats | 5 |
| Teams, castle, tower | 7 |
| Win/lose once | 6–7 |
| Minion waves 30s × 3 | 8 |
| Gold on kill | 9 |
| Heroes + 8s fake respawn | 10 |
| No terrain.json edits | all |
| No engine ReviveUnit in v1 | 10 (spawn instead) |
| ownerPlayer tag | 7–10 |

**Placeholder scan:** No TBD. Event signature fork is an explicit compile-time branch in Task 7.

**Type consistency:** `MinionWaveSystem.Start(IGameAPI, Lane)` matches the spec. `TeamSetup.Apply` does buildings only; `HeroSystem.SpawnStartingHeroes` is called from `MapScript` (spec listed both). `WinSystem.HasEnded` + `HeroSystem.NotifyMatchEnded` implement “do not respawn after match end.”

## After the last task

Optional extra time only (not required for Devin’s demo): leaderboard of gold/kills, countdown label before first wave, third hero, 3 lanes if Isabella’s coordinates exist, real `ReviveUnit` in the engine repo.
