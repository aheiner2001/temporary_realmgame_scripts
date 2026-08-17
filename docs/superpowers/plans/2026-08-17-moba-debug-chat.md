# MOBA Debug Chat Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add gated `Debug:` chat so the next Realm TEST shows whether scripts spawned units, fired a wave, awarded gold, or skipped win because a castle id is 0.

**Architecture:** Pure-logic `MatchLog` formatters (unit-tested, no `IGameAPI`) plus `MatchLog.Write(Action<string>, string)` that no-ops when `GameConfig.DebugChat` is false. Call sites pass `api.BroadcastMessage`. Player-facing chat stays ungated. No new match rules.

**Tech Stack:** C# / .NET 10, xUnit (`MOBA.Tests/MOBA.Tests.csproj`), Realm `IGameAPI.BroadcastMessage` at call sites only.

**Spec:** `docs/superpowers/specs/2026-08-17-moba-debug-chat-design.md`

## Global Constraints

- Map-only. Do not change `Realm/`, `IGameAPI`, or `MOBA/lib`.
- Do not edit `MOBA/terrain.json`.
- Do not add heroes, shop, jungle, or engine `ReviveUnit`.
- Do not mock the full `IGameAPI` in tests.
- Do not construct a fake `IGameAPI`. Do not assert Godot UI.
- Player-facing chat stays always on: `MOBA scripts loaded`, guest init lines, `Hero down. Respawning in 8s.`, `SendMessageToPlayer` gold, `Blue wins!` / `Red wins!`.
- Debug lines always start with `Debug:`.
- Positions: invariant `(x, y, z)` with `0.##` (`24` not `24.00`). Not `Vector3.ToString()`.
- Gold amounts in debug text: same `0.##` rule (`+15` not `+15.0`).
- Do not log one debug line per successful minion.
- `{deadUnitId}` in gold debug is the unit type id string, not the numeric unique id.
- `MOBA.Tests` only compiles listed `MOBA/*.cs` files. Add `MatchLog.cs` to that list. Do not add `TeamSetup.cs` / `HeroSystem.cs` / etc. to the test project.
- If `git commit` fails with `unknown option 'trailer'`, use `/usr/bin/git commit` (local wrapper injects `--trailer`; Apple git 2.15 does not support it).

## File Structure

**Create**

| File | Responsibility |
|---|---|
| `MOBA/MatchLog.cs` | Formatters + `Write(Action<string>?, string)` |
| `MOBA.Tests/MatchLogTests.cs` | Exact `Debug:` string tests + Write invokes callback |

**Modify**

| File | Responsibility |
|---|---|
| `MOBA/GameConfig.cs` | `DebugChat = true` |
| `MOBA.Tests/GameConfigTests.cs` | Assert `DebugChat` |
| `MOBA.Tests/MOBA.Tests.csproj` | Compile `MatchLog.cs` |
| `MOBA/TeamSetup.cs` | Spawn ok/fail debug |
| `MOBA/HeroSystem.cs` | Spawn ok/fail debug (start + respawn) |
| `MOBA/MinionWaveSystem.cs` | Wave tick + spawn fail only |
| `MOBA/GoldSystem.cs` | Debug after gold award |
| `MOBA/WinSystem.cs` | Win-fired debug; one-shot castle-id-0 skip |
| `MOBA/MapScript.cs` | Castle id summary after heroes spawn |

**Do not touch:** `MOBA/terrain.json`, `MOBA/WasmEntryPoint.cs`, `MOBA/lib/`, `Realm/`.

---

### Task 1: DebugChat flag and MatchLog formatters

**Files:**
- Create: `MOBA/MatchLog.cs`
- Create: `MOBA.Tests/MatchLogTests.cs`
- Modify: `MOBA/GameConfig.cs`
- Modify: `MOBA.Tests/GameConfigTests.cs`
- Modify: `MOBA.Tests/MOBA.Tests.csproj`

**Interfaces:**
- Consumes: `GameConfig` constants; `MatchWinner` from `MOBA/WinDecision.cs` (already compiled by tests)
- Produces:
  - `GameConfig.DebugChat` (`public const bool`, default `true`)
  - `MatchLog.Write(Action<string>? broadcast, string message)` — no-op if `!DebugChat` or `broadcast` is null; otherwise `broadcast(message)`
  - `MatchLog.Spawned(string unitType, int id, Vector3 position, int playerIndex)`
  - `MatchLog.SpawnFailed(string unitType, Vector3 position, int playerIndex)`
  - `MatchLog.CastleSummary(int blueCastleId, int redCastleId)`
  - `MatchLog.WaveTick(int laneCount)`
  - `MatchLog.GoldAwarded(float gold, int playerIndex, string deadUnitTypeId)`
  - `MatchLog.WinFired(MatchWinner winner)`
  - `MatchLog.WinCheckSkipped()`

- [ ] **Step 1: Write the failing tests**

In `MOBA.Tests/GameConfigTests.cs`, add this assert inside `Match_numbers_match_the_spec` (after the existing `TowerUnitId` assert):

```csharp
        Assert.True(GameConfig.DebugChat);
```

Replace `MOBA.Tests/MatchLogTests.cs` with:

```csharp
using System.Numerics;
using Realm.Maps;
using Xunit;

namespace Realm.Maps.Tests;

public class MatchLogTests
{
    private static readonly Vector3 Origin = new(24, 0, 24);

    [Fact]
    public void Spawned_uses_debug_prefix_and_invariant_position()
    {
        Assert.Equal(
            "Debug: spawned castle id=12 at (24, 0, 24) for player 0",
            MatchLog.Spawned("castle", 12, Origin, 0));
    }

    [Fact]
    public void SpawnFailed_has_no_id()
    {
        Assert.Equal(
            "Debug: spawn FAILED castle at (24, 0, 24) for player 0",
            MatchLog.SpawnFailed("castle", Origin, 0));
    }

    [Fact]
    public void CastleSummary_lists_both_ids()
    {
        Assert.Equal(
            "Debug: blue castle id=12 red castle id=34",
            MatchLog.CastleSummary(12, 34));
    }

    [Fact]
    public void WaveTick_uses_lane_count()
    {
        Assert.Equal("Debug: wave tick, 3 lanes", MatchLog.WaveTick(3));
    }

    [Fact]
    public void GoldAwarded_uses_unit_type_id()
    {
        Assert.Equal(
            "Debug: +15 gold to player 0 (melee_minion)",
            MatchLog.GoldAwarded(15f, 0, "melee_minion"));
    }

    [Fact]
    public void WinFired_blue_and_red()
    {
        Assert.Equal("Debug: win fired Blue", MatchLog.WinFired(MatchWinner.Blue));
        Assert.Equal("Debug: win fired Red", MatchLog.WinFired(MatchWinner.Red));
    }

    [Fact]
    public void WinCheckSkipped_is_stable()
    {
        Assert.Equal(
            "Debug: win check skipped, castle id is 0",
            MatchLog.WinCheckSkipped());
    }

    [Fact]
    public void Position_drops_trailing_zeros_keeps_tenths()
    {
        Assert.Equal(
            "Debug: spawned castle id=1 at (24.5, 0, 24) for player 1",
            MatchLog.Spawned("castle", 1, new Vector3(24.5f, 0, 24), 1));
    }

    [Fact]
    public void Write_sends_when_DebugChat_is_on()
    {
        string? got = null;
        MatchLog.Write(msg => got = msg, "Debug: hello");
        Assert.Equal("Debug: hello", got);
    }
}
```

In `MOBA.Tests/MOBA.Tests.csproj`, add this line with the other `Compile Include` entries:

```xml
    <Compile Include="..\MOBA\MatchLog.cs" Link="MatchLog.cs" />
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter "FullyQualifiedName~MatchLogTests|FullyQualifiedName~GameConfigTests"`

Expected: FAIL — `DebugChat` does not exist; `MatchLog` type does not exist (CS0103 / CS0117).

- [ ] **Step 3: Write minimal implementation**

Add this field to `MOBA/GameConfig.cs` with the other consts (after `TowerUnitId`):

```csharp
    public const bool DebugChat = true;
```

Create `MOBA/MatchLog.cs`:

```csharp
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
```

- [ ] **Step 4: Run tests to verify they pass**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj`

Expected: PASS (all existing tests plus new ones).

- [ ] **Step 5: Commit**

```bash
git add MOBA/GameConfig.cs MOBA/MatchLog.cs MOBA.Tests/GameConfigTests.cs MOBA.Tests/MatchLogTests.cs MOBA.Tests/MOBA.Tests.csproj
git commit -m "feat: add MatchLog debug chat formatters"
```

---

### Task 2: Log building, hero, and minion spawns

**Files:**
- Modify: `MOBA/TeamSetup.cs`
- Modify: `MOBA/HeroSystem.cs`
- Modify: `MOBA/MinionWaveSystem.cs`

**Interfaces:**
- Consumes: `MatchLog.Write`, `MatchLog.Spawned`, `MatchLog.SpawnFailed`, `MatchLog.WaveTick` from Task 1
- Produces: debug chat on castle/tower/hero spawn ok or fail; one `wave tick` line per timer; minion fail lines only

- [ ] **Step 1: Wire TeamSetup spawn logging**

Replace `Spawn` in `MOBA/TeamSetup.cs` with:

```csharp
    private static IUnit? Spawn(IGameAPI api, string unitTypeId, Vector3 position, int playerIndex)
    {
        var unit = api.SpawnUnitForPlayer(unitTypeId, position, playerIndex);
        if (unit == null)
        {
            MatchLog.Write(api.BroadcastMessage, MatchLog.SpawnFailed(unitTypeId, position, playerIndex));
            return null;
        }

        OwnerTag.Set(unit, playerIndex);
        MatchLog.Write(api.BroadcastMessage, MatchLog.Spawned(unitTypeId, unit.UniqueId, position, playerIndex));
        return unit;
    }
```

Leave `Apply` unchanged.

- [ ] **Step 2: Wire HeroSystem spawn logging**

Replace `TrySpawn` in `MOBA/HeroSystem.cs` with:

```csharp
    private void TrySpawn(IGameAPI api, int playerIndex, string unitTypeId, string fountainName)
    {
        Vector3 position = Coordinates.Resolve(api, fountainName);
        var unit = api.SpawnUnitForPlayer(unitTypeId, position, playerIndex);
        if (unit == null)
        {
            MatchLog.Write(api.BroadcastMessage, MatchLog.SpawnFailed(unitTypeId, position, playerIndex));
            return;
        }

        OwnerTag.Set(unit, playerIndex);
        _heroUnitByPlayer[playerIndex] = unit.UniqueId;
        MatchLog.Write(api.BroadcastMessage, MatchLog.Spawned(unitTypeId, unit.UniqueId, position, playerIndex));
    }
```

Do not gate `Hero down. Respawning in 8s.`

- [ ] **Step 3: Wire MinionWaveSystem tick + fail logging**

Replace `OnTimerExpired` in `MOBA/MinionWaveSystem.cs` with:

```csharp
    public void OnTimerExpired(IGameAPI api, int timerHandle)
    {
        if (timerHandle != _waveTimerHandle)
            return;
        MatchLog.Write(api.BroadcastMessage, MatchLog.WaveTick(_lanes.Count));
        foreach (var lane in _lanes)
        {
            SpawnWave(api, GameConfig.BluePlayerIndex, lane.BluePath);
            SpawnWave(api, GameConfig.RedPlayerIndex, lane.RedPath);
        }
    }
```

Replace the loop body in `SpawnGroup` with:

```csharp
        for (int i = 0; i < count; i++)
        {
            var unit = api.SpawnUnitForPlayer(unitTypeId, spawn, playerIndex);
            if (unit == null)
            {
                MatchLog.Write(api.BroadcastMessage, MatchLog.SpawnFailed(unitTypeId, spawn, playerIndex));
                continue;
            }
            OwnerTag.Set(unit, playerIndex);
            unit.AttackMove(path[firstDest]);
            _minions.Add(new TrackedMinion
            {
                UnitId = unit.UniqueId,
                WaypointIndex = firstDest,
                Path = path
            });
        }
```

Do not add a debug line when a minion spawn succeeds.

- [ ] **Step 4: Run tests**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj`

Expected: PASS. These files are not in the test project; this is a regression check.

- [ ] **Step 5: Commit**

```bash
git add MOBA/TeamSetup.cs MOBA/HeroSystem.cs MOBA/MinionWaveSystem.cs
git commit -m "feat: debug-log building, hero, and failed minion spawns"
```

---

### Task 3: Log gold, win, and castle summary

**Files:**
- Modify: `MOBA/GoldSystem.cs`
- Modify: `MOBA/WinSystem.cs`
- Modify: `MOBA/MapScript.cs`

**Interfaces:**
- Consumes: `MatchLog.Write`, `MatchLog.GoldAwarded`, `MatchLog.WinFired`, `MatchLog.WinCheckSkipped`, `MatchLog.CastleSummary` from Task 1; `TeamSetup.BlueCastleId` / `RedCastleId`
- Produces: debug gold after a real award; `Debug: win fired Blue|Red` when the match ends; one-shot skip log if a castle id is 0; init summary of castle ids

- [ ] **Step 1: Wire GoldSystem**

Replace `OnUnitDied` in `MOBA/GoldSystem.cs` with:

```csharp
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
        MatchLog.Write(api.BroadcastMessage, MatchLog.GoldAwarded(gold, playerIndex, dead.UnitId));
    }
```

Keep `SendMessageToPlayer` ungated. Do not write debug gold when the method returns early.

- [ ] **Step 2: Wire WinSystem**

Replace `MOBA/WinSystem.cs` with:

```csharp
using Realm.MapAPI;

namespace Realm.Maps;

public sealed class WinSystem
{
    private readonly int _blueCastleId;
    private readonly int _redCastleId;
    private bool _matchEnded;
    private bool _loggedMissingCastle;

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

        if ((_blueCastleId == 0 || _redCastleId == 0) && !_loggedMissingCastle)
        {
            _loggedMissingCastle = true;
            MatchLog.Write(api.BroadcastMessage, MatchLog.WinCheckSkipped());
        }

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

        MatchLog.Write(api.BroadcastMessage, MatchLog.WinFired(winner));
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

Keep `Blue wins!` / `Red wins!` ungated. Id `0` still does not count as destroyed.

- [ ] **Step 3: Wire MapScript castle summary**

In `MOBA/MapScript.cs` `Initialize`, after `_heroes.SpawnStartingHeroes(api);` and before `api.OnUnitDied += OnUnitDied;`, add:

```csharp
        MatchLog.Write(api.BroadcastMessage, MatchLog.CastleSummary(setup.BlueCastleId, setup.RedCastleId));
```

Do not gate `MOBA scripts loaded` or the guest init lines.

- [ ] **Step 4: Run tests**

Run: `dotnet test MOBA.Tests/MOBA.Tests.csproj`

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add MOBA/GoldSystem.cs MOBA/WinSystem.cs MOBA/MapScript.cs
git commit -m "feat: debug-log gold, win, and castle ids"
```

---

## After the last task

When Devin’s editor works: LOAD `MOBA/` → TEST → look for `Debug:` lines.

- `MOBA scripts loaded` but no `Debug:` → WASM ran an old binary, or `DebugChat` is false.
- `Debug: spawn FAILED …` → scripts ran; engine rejected the unit type or position.
- `Debug: spawned castle id=…` → units should exist; pan to fallback corners if the camera is empty.
- No `MOBA scripts loaded` → WASM still did not run (engine bug, not this change).

Before the supervisor demo: set `GameConfig.DebugChat = false` in a separate commit.

## Self-review

**Spec coverage**

| Spec item | Task |
|---|---|
| `GameConfig.DebugChat` default true | 1 |
| Formatters + Write; no IGameAPI in tests | 1 (`Write(Action<string>?)` is the testable form of `Write(api, message)`) |
| Spawn ok/fail buildings | 2 `TeamSetup` |
| Spawn ok/fail heroes including respawn | 2 `HeroSystem.TrySpawn` |
| Wave tick, not per successful minion | 2 `MinionWaveSystem` |
| Minion spawn FAILED | 2 `SpawnGroup` |
| Gold debug after award; skip if no gold | 3 `GoldSystem` |
| Win fired debug; always-on Blue/Red wins | 3 `WinSystem` |
| Castle id 0 skip once; id 0 not dead | 3 `WinSystem` |
| Init castle summary | 3 `MapScript` |
| Always-on player chat unchanged | 2–3 (not gated) |
| No engine / terrain / new features | all |

**Placeholder scan:** No TBD. Full files/methods inlined.

**Type consistency:** `Write(Action<string>? broadcast, string message)`; `WinFired(MatchWinner)`; gold uses `dead.UnitId` (type string, same as `GoldTable.ForDeadUnit`).
