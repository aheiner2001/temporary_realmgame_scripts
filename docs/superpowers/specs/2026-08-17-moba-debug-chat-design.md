# MOBA Debug Chat Design

Date: 2026-08-17
Status: Approved in conversation; awaiting user review of this written spec
Approach: `MatchLog` helper + `GameConfig.DebugChat` (default on)

## Goal

While the Realm editor TEST path is blocked on Devin’s engine bug, improve the existing MOBA map scripts so the **next** successful TEST is diagnosable from chat.

This is not new gameplay. Match rules stay as they are (teams, buildings, waves, gold, heroes, fake respawn, castle win).

Success when TEST works: chat shows `Debug:` lines for spawns (or spawn failures), one wave tick per 30s, gold awards, and win. Success without TEST: `dotnet test` covers the flag and the exact message strings.

## Why now

v1 systems already exist in `MOBA/`. The last TEST failure looked like “nothing appeared” with no script-side signal. `BroadcastMessage` already works in this engine when WASM runs. Extra chat is the cheapest probe that does not need a fake `IGameAPI`.

## Constraints

- Map-only. Do not change `Realm/`, `IGameAPI`, or `MOBA/lib`.
- Do not edit `terrain.json`.
- Do not add heroes, shop, jungle, or engine `ReviveUnit`.
- Do not mock the full `IGameAPI` in tests.
- Player-facing chat stays always on. Debug chat is extra and can be turned off for Devin’s demo.

## Architecture

```text
GameConfig.DebugChat  (true now, false before demo)
        │
        ▼
MatchLog.Write(api, message)  → BroadcastMessage only if flag on
        ▲
        │  formatters return the string (unit-tested)
TeamSetup, HeroSystem, MinionWaveSystem, GoldSystem, WinSystem, MapScript
```

`MatchLog` is a static helper in `MOBA/MatchLog.cs`:

- Format methods return `string` (no `IGameAPI`). Tests assert these strings.
- `Write(IGameAPI api, string message)` no-ops when `DebugChat` is false; otherwise `api.BroadcastMessage(message)`.
- Positions use invariant formatting: `(x, y, z)` with up to two decimal places (`24` not `24.00`), not `Vector3.ToString()` (culture-sensitive).
- Gold amounts in debug text use the same rule (`+15` not `+15.0`).

Call sites keep match behavior. They only add `MatchLog.Write(api, MatchLog.…(…))`. Spawn still does not throw on null.

## Always-on vs debug

**Always on** (already exist; do not gate):

- `MOBA scripts loaded` / guest init lines
- `Hero down. Respawning in 8s.`
- `SendMessageToPlayer` `+15 gold` (and other gold amounts) to the killer
- `Blue wins!` / `Red wins!`

**Debug only** (prefix `Debug:`):

| When | Message |
|---|---|
| Building or hero spawn succeeds | `Debug: spawned {unitType} id={id} at (x, y, z) for player {p}` |
| Spawn returns null | `Debug: spawn FAILED {unitType} at (x, y, z) for player {p}` |
| After buildings + heroes in `Initialize` | `Debug: blue castle id={id} red castle id={id}` |
| Wave timer fires | `Debug: wave tick, {n} lanes` |
| A minion spawn in that wave returns null | same `spawn FAILED` line as buildings |
| Gold actually awarded | `Debug: +{gold} gold to player {p} ({deadUnitId})` |
| Win fires | `Debug: win fired Blue` or `Debug: win fired Red` |
| Either castle id is `0` the first time `WinSystem.Check` runs | `Debug: win check skipped, castle id is 0` (log once; still skip treating id `0` as destroyed) |

Do **not** log one debug line per successful minion (3 lanes × 2 sides × 4 minions ≈ 24 lines every 30s). Wave success is the single tick line.

Hero respawn uses the same spawn ok/fail format as match start.

`{deadUnitId}` in the gold line is the unit type id (`melee_minion`, `adventurer`, `castle_tower_1`), not the numeric unique id.

## Error handling

- `SpawnUnitForPlayer` null → debug fail line, continue. Castle/hero ids stay `0` if that spawn failed.
- `WinSystem` already ignores id `0` (not dead). Keep that. Add a one-shot debug skip log so a missing castle is visible.
- Win still fires at most once.
- Invalid killer / no gold → no debug gold line (same as today: skip reward).

## Tests

Add `MOBA.Tests/MatchLogTests.cs` and extend `GameConfigTests`:

- `DebugChat` defaults to `true`
- Each formatter matches the table above for a fixed position `new Vector3(24, 0, 24)` → `(24, 0, 24)`
- Gold formatter uses `15` and `melee_minion`
- Win formatter uses `Blue` / `Red`

Do not construct a fake `IGameAPI`. Do not assert Godot UI.

Existing tests for config, fallbacks, waypoints, gold table, and win decision must still pass.

## How to use after Devin’s fix

1. LOAD `MOBA/` in the new editor, TEST.
2. If WASM ran: look for `Debug:` lines. Missing spawn lines + `spawn FAILED` means scripts ran but units did not. No `Debug:` and no `MOBA scripts loaded` means WASM still did not run.
3. Before the supervisor demo: set `GameConfig.DebugChat = false`.

## Out of scope

- New match features
- Engine API work
- Recording/fake `IGameAPI` harness
- `.gitignore` / `bin`/`obj` cleanup (unless a test cannot run without it)
- Changing lane count or spawn coordinates

## Files

| File | Change |
|---|---|
| `MOBA/GameConfig.cs` | `public const bool DebugChat = true;` |
| `MOBA/MatchLog.cs` | new |
| `MOBA/TeamSetup.cs` | log spawn ok/fail |
| `MOBA/HeroSystem.cs` | log spawn ok/fail |
| `MOBA/MinionWaveSystem.cs` | wave tick + spawn fail only |
| `MOBA/GoldSystem.cs` | log after `AdjustPlayerGold` |
| `MOBA/WinSystem.cs` | win-fired debug; one-shot castle-id-0 skip |
| `MOBA/MapScript.cs` | summary castle ids after setup |
| `MOBA.Tests/GameConfigTests.cs` | assert `DebugChat` |
| `MOBA.Tests/MatchLogTests.cs` | new |
