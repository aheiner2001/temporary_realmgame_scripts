# MOBA Map Scripts Design

Date: 2026-08-14
Status: Approved in conversation; awaiting user review of this written spec
Approach: Map-first demo (engine API additions deferred)

## Goal

Build a **simple MOBA demo** in this map repo that showcases Realm’s editor: a different genre than Age of Empires. Two teams, one lane of minion waves, towers, one hero per player, gold on kills, hero respawn, destroy the enemy castle to win.

This is not League of Legends. A few basic heroes and one lane are enough.

## What Devin asked for (mapped to this spec)

| Supervisor instruction | How this spec fulfills it |
|---|---|
| Script gameplay in the Realm editor for this custom map | All match logic lives in `MOBA/` C# (`MapScript` + small systems) |
| `IGameAPI` stays generic and reusable | Map code only *calls* the API. No engine changes in v1 |
| Map repo is MOBA-specific and not reusable | Waypoints, wave timing, hero loadout, win condition are map-only |
| Missing generic APIs (e.g. `ReviveUnit`) go in the core engine | Deferred: fake revive by spawning a new hero. Add `ReviveUnit` later |
| Keep it simple; still feel like a MOBA; you pick the rules | Rules below. No jungle, items, inhibitors, or 100 heroes |
| Demo that the editor can invent a new genre | One hero per player + waves + respawn + destroy base |
| Terrain is unfinished (Isabella) | Do not edit `terrain.json`. Use hardcoded waypoint fallbacks until named coordinates exist |

## Two repos

```text
Realm engine (clone to press Play; do not modify in v1)
  Realm.Godot      game window
  Realm.MapAPI     generic IGameAPI (SpawnUnit, AttackMove, gold, timers, …)
        ↑
        | map compiles to WebAssembly and runs sandboxed
        |
This repo: MOBA/
  MapScript.cs     match coordinator
  metadata.json    unit stats
  terrain.json     Isabella’s map (read-only for us)
```

The engineer implementing this may not have the engine cloned yet. Task 0 is clone + load this map + print a chat message. Gameplay comes after that.

## Match rules (v1)

### Teams and players

- Two teams: **Blue** and **Red**.
- **1v1 first.** Player index `0` is Blue. Player index `1` is Red.
- Use `SetPlayerTeam` and `SetPlayersAllied` so same-team units do not fight.
- If player 1 is a computer slot, still spawn Red minions, tower, castle, and a hero owned by that player. Do not build a custom bot AI in v1.
- Not locked to 5v5. Extra slots can be added later with the same team functions.

### Map

- **One lane** for v1.
- Three lanes only if Isabella’s terrain later has them, and only after the one-lane demo works.
- Do not sculpt terrain in this project.

### Win / lose

- Each team has one **castle** (`castle` building).
- Destroying the enemy castle wins. Call `TriggerPlayerVictory` for the winning player(s) and `TriggerPlayerDefeat` for the losing player(s).
- Do **not** use `GetCastle(bool isEnemy)` as the source of truth. That API is a two-faction helper and may not match Blue/Red player teams. Spawn both castles in script and store their unit ids.

### Minion waves

- Every **30 seconds**, each team spawns **3 melee minions** at that team’s gate.
- Minions **attack-move** along that team’s waypoint list toward the enemy castle.
- First wave at t = 30s (not immediately on match start), so the player can find their hero.

### Towers

- **One tower per side** on the lane (`castle_tower_1`).
- Towers are buildings owned by the team. Engine combat makes them shoot. No custom tower AI in v1.

### Heroes

- Each player gets **exactly one hero** at their fountain.
- Blue hero unit type: `adventurer` (melee).
- Red hero unit type: `armored_dragon` (ranged), so the two sides are visually distinct.
- Mark both as `IsHero: true` in `metadata.json`.
- No hero picker UI in v1.

### Death and respawn (fake revive)

There is no `ReviveUnit` on `IGameAPI` yet.

When `OnUnitDied` fires and the dead unit `IsHero`:

1. Broadcast a short message (`"Hero down. Respawning in 8s."`).
2. Start an 8 second timer for that player.
3. On timer: `SpawnUnitForPlayer` a new copy of that player’s hero type at their fountain.

Gold lives on the **player** (`GetPlayerGold` / `AdjustPlayerGold`), not on the unit, so it survives respawn.

Follow-up (not v1): add `ReviveUnit` to `Realm.MapAPI` in the engine repo and switch `HeroSystem` to call it.

### Gold

| Event | Gold to the killing player |
|---|---|
| Kill a minion | +15 |
| Kill a hero | +150 |
| Kill a tower | +100 |
| Kill a castle | +0 (match is ending) |

- Starting gold: 0.
- No passive gold tick in v1.
- No item shop in v1.
- If the killer id is missing or invalid, skip the reward (do not crash).

### Out of scope (v1)

Jungle, dragon/baron, wards, fog mastery, item shop, inhibitors, super minions, last-hitting tutorials, 5-role team comps, 3-lane routing, custom bot AI, replay, cosmetics, engine `ReviveUnit`.

## Numbers (single source of truth)

These live in `GameConfig.cs`. Do not scatter magic numbers.

```csharp
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

## Coordinates

`terrain.json` currently has **zero** named coordinates and **zero** placed units. Isabella may add named boxes later.

`Coordinates.cs` exposes named lookups. Prefer `api.HasCoordinate(name)` when present; otherwise use hardcoded fallbacks.

Required names:

| Name | Role |
|---|---|
| `BlueFountain` | Hero spawn / respawn |
| `BlueGate` | Blue minion spawn |
| `BlueTower` | Blue tower |
| `BlueCastle` | Blue base |
| `MidLane1` | Middle waypoint |
| `RedGate` | Red minion spawn |
| `RedTower` | Red tower |
| `RedCastle` | Red base |
| `RedFountain` | Red hero spawn / respawn |

Fallback world positions (128×128 map; replace after walking the map in the editor and reading a unit’s `Position`):

| Name | Fallback (x, y, z) |
|---|---|
| `BlueFountain` | (16, 0, 16) |
| `BlueCastle` | (24, 0, 24) |
| `BlueGate` | (32, 0, 32) |
| `BlueTower` | (48, 0, 48) |
| `MidLane1` | (64, 0, 64) |
| `RedTower` | (80, 0, 80) |
| `RedGate` | (96, 0, 96) |
| `RedCastle` | (104, 0, 104) |
| `RedFountain` | (112, 0, 112) |

Blue minion path: `BlueGate` → `BlueTower` → `MidLane1` → `RedTower` → `RedCastle`  
Red minion path: `RedGate` → `RedTower` → `MidLane1` → `BlueTower` → `BlueCastle`

Y is height. If units spawn underground or float, set Y from the first successful `IUnit.Position.Y` on the map rather than guessing.

## Files

`MapScript.cs` is a thin coordinator. It must not become a god file.

| File | Responsibility |
|---|---|
| `MOBA/MapScript.cs` | `Initialize` / `Update`: construct systems, subscribe events, tick minion waypoints, ask `WinSystem` |
| `MOBA/GameConfig.cs` | Constants listed above |
| `MOBA/Coordinates.cs` | Resolve named region → `Vector3` (API coordinate or fallback) |
| `MOBA/Lane.cs` | One lane: blue path and red path as `Vector3` lists |
| `MOBA/TeamSetup.cs` | Teams, allies, spawn castle/tower/hero per side |
| `MOBA/MinionWaveSystem.cs` | Repeating 30s timer; spawn 3 minions per side; track waypoint index |
| `MOBA/HeroSystem.cs` | Track living hero per player; schedule fake respawn |
| `MOBA/GoldSystem.cs` | `OnUnitDied` → gold to killer’s owner |
| `MOBA/WinSystem.cs` | If stored castle id is dead → end match once |
| `MOBA/metadata.json` | Stats for minion, heroes, castle, tower |
| `MOBA/terrain.json` | Do not edit |

### `metadata.json` stats to set

Reuse existing models. Add `melee_minion` as a new custom unit.

| UnitId | Model | IsHero | MaxHp | Damage | Range | Speed | GoldBounty | Notes |
|---|---|---|---|---|---|---|---|---|
| `melee_minion` | `adventurer.glb` | false | 80 | 8 | 1.5 | 3.5 | 15 | New unit. AttackType `melee`. Armor 0 |
| `adventurer` | `adventurer.glb` | true | 400 | 25 | 1.5 | 4.0 | 150 | Blue hero. AttackType `melee`. Armor 2 |
| `armored_dragon` | `armored_dragon.glb` | true | 350 | 20 | 8 | 4.0 | 150 | Red hero. AttackType `ranged`. Armor 2 |
| `castle` | `castle.glb` | false | 3000 | 0 | 0 | 0 | 0 | AttackType `none`. Armor 5. Building |
| `castle_tower_1` | `castle_tower_1.glb` | false | 1200 | 40 | 12 | 0 | 100 | AttackType `ranged`. Armor 5. Building |

Exact JSON property names must match the map schema (`MaxHp`, `Damage`, `Range`, `Armor`, `Speed`, `AttackType`, `IsHero`, `GoldBounty`, `ModelPath`, `PathingType`). Buildings keep `PathingType` 32. Ground units keep `PathingType` 9.

## Interfaces (how systems talk)

The coordinator owns one instance of each system. Systems receive `IGameAPI`; they do not look up globals.

```csharp
public sealed class Lane
{
    public IReadOnlyList<Vector3> BluePath { get; }
    public IReadOnlyList<Vector3> RedPath { get; }
    public static Lane FromCoordinates(IGameAPI api);
}

public sealed class TeamSetup
{
    public int BlueCastleId { get; }
    public int RedCastleId { get; }
    public void Apply(IGameAPI api, Lane lane);
    // Every spawned unit calls unit.SetCustomData("ownerPlayer", playerIndex.ToString())
}

public sealed class MinionWaveSystem
{
    public void Start(IGameAPI api, Lane lane);
    public void OnTimerExpired(IGameAPI api, int timerHandle);
    public void Tick(IGameAPI api, float delta); // advance waypoints
}

public sealed class HeroSystem
{
    public void SpawnStartingHeroes(IGameAPI api);
    public void OnUnitDied(IGameAPI api, int unitId, int killerId);
    public void OnTimerExpired(IGameAPI api, int timerHandle);
}

public sealed class GoldSystem
{
    public void OnUnitDied(IGameAPI api, int deadUnitId, int killerId);
}

public sealed class WinSystem
{
    public WinSystem(int blueCastleId, int redCastleId);
    public void Check(IGameAPI api); // call from Update and OnUnitDied
}
```

`MapScript.Initialize` order:

1. Broadcast `"MOBA scripts loaded"`.
2. `Lane.FromCoordinates(api)`.
3. `TeamSetup.Apply` (teams + buildings + starting heroes).
4. `WinSystem` constructed with the two castle ids.
5. `MinionWaveSystem.Start` (schedule repeating 30s timer).
6. Subscribe `OnUnitDied`, `OnTimerExpired`.

`MapScript.Update`:

1. `MinionWaveSystem.Tick`.
2. `WinSystem.Check`.

## Data flow

```text
Initialize
  → chat "MOBA scripts loaded"
  → resolve coordinates
  → set player 0 Blue, player 1 Red, allied within team
  → spawn Blue/Red castle, tower, hero
  → start repeating wave timer

OnTimerExpired (wave handle)
  → spawn 3 melee_minion for Blue at BlueGate, 3 for Red at RedGate
  → AttackMove each toward first waypoint

Update
  → if a tracked minion is within 4 units of its current waypoint,
      AttackMove to the next (last waypoint is enemy castle)
  → if a castle id is dead, end match once

OnUnitDied
  → GoldSystem: AdjustPlayerGold on killer owner
  → HeroSystem: if dead IsHero, schedule 8s respawn at fountain
  → WinSystem.Check
```

Minion owner: Blue minions `SpawnUnitForPlayer(..., BluePlayerIndex)`, Red minions `SpawnUnitForPlayer(..., RedPlayerIndex)`.

`IUnit` has no owner getter. Every spawn must tag the unit:

```csharp
unit.SetCustomData("ownerPlayer", playerIndex.ToString());
```

`GoldSystem` reads the killer with `GetUnitById(killerId)`, then `GetCustomData("ownerPlayer")`. If the killer is null or the key is missing, skip gold. Do not infer team from the victim.

## Error handling

- Missing named coordinate: use the fallback table. Do not throw.
- `GetUnitById` returns null (already removed): drop that minion/hero from tracking. Do not throw.
- `OnUnitDied` with invalid killer: skip gold.
- `WinSystem` fires victory/defeat **once**. A boolean `matchEnded` prevents double triggers.
- Inactive player slot: skip spawning a hero for that index; still spawn that side’s castle, tower, and minions so the lane works in single-player.
- Respawn while match ended: do not spawn.

## Testing (Play checklists, not a unit-test runner)

WASM map scripts are not set up for `dotnet test` in this repo. Each task is verified in-game.

**Task 0 — map loads**

- Chat shows `MOBA scripts loaded`.

**Task 1 — spawn smoke**

- An `adventurer` appears at Blue fountain (or fallback).

**Task 2 — waves**

- At ~30s, 3 Blue and 3 Red minions appear and walk the lane toward each other.

**Task 3 — win**

- Castles exist. Destroying the Red castle ends the match with a Blue win.

**Task 4 — towers**

- Each side has a tower. Minions take damage as they approach it.

**Task 5 — heroes and respawn**

- Each active player has a hero. Killing a hero prints the respawn message; a new hero appears at the fountain ~8s later.

**Task 6 — gold**

- Killing a minion increases that player’s gold (chat or leaderboard).

## Calendar and learning

### Skills this assignment teaches

1. Run a custom Realm map.
2. Game loop: `Initialize`, `Update`, timers, `OnUnitDied`.
3. Call `IGameAPI` (spawn, attack-move, teams, gold, victory).
4. Data vs code: `metadata.json` stats vs C# behavior.
5. Named coordinates vs hardcoded fallbacks.
6. Later: add a generic engine API if still needed.

### Videos (~45 minutes, once)

- https://www.youtube.com/watch?v=ksh7_9NWNqw (MOBA basics; ignore mobile-specific UI)
- https://www.youtube.com/watch?v=NquAYMAV1Sg (Summoner's Rift layout)
- https://www.leagueoflegends.com/en-us/how-to-play/ (Nexus, turrets, minions)

Do not study last-hitting, jungle pathing, or ranked roles.

### Time (today + next week)

| When | Deliverable |
|---|---|
| Today | Videos + clone engine + map loads + chat message + one spawned unit |
| Day 2 (2-day milestone) | One lane, waves, two castles, destroy castle to win |
| Next week (supervisor demo) | Towers, two hero types, 8s respawn, gold, Blue/Red 1v1 |
| Extra time only | Third hero, 3 lanes if terrain ready, tiny shop, real `ReviveUnit` |

Two calendar days is **not** enough for the full demo if the person is new to MOBAs and has not run the editor yet. Today plus next week **is** enough for the demo above.

## Follow-up (not this plan)

1. If Devin wants a real revive: add `ReviveUnit` to `IGameAPI` in the engine, then replace fake respawn in `HeroSystem`.
2. When Isabella adds named coordinates, keep the same names in the table above and stop using fallbacks for names that exist.
3. Scale player count by assigning more slot indices to BlueTeam/RedTeam with the same `TeamSetup` functions.

## Non-goals for this spec

- Editing Godot, WASM bindgen, or lobby code.
- A reusable “MOBA engine” inside MapAPI.
- Perfect balance.
- Waiting for finished terrain before writing scripts.
