# MOBA Modding & Game Mode Cheat Sheet

Quick reference for configuring and editing each game mode, unit stats, spawners, and mechanics in the **MOBA** custom map.

---

## 🎮 1. How to Select Your Active Game Mode

Open **`MOBA/GameConfig.cs`** and change `CurrentMode`:

```csharp
public static class GameConfig
{
    // Options: ThreeLanePush, ArenaBrawl, ControlPoints, BossObjective, HeroDeathmatch
    public static GameMode CurrentMode = GameMode.ArenaBrawl;
}
```

---

## 🛠️ 2. How to Edit Each Game Mode

### Mode 1: 3-Lane Standard Push (`GameMode.ThreeLanePush`)
*Classic 3-lane push with Top, Mid, and Bot minion waves.*
- **Number of Lanes & Offsets**: Open **`MOBA/Lane.cs`** → `ThreeLanesFromCoordinates()` and `GameConfig.LaneOffset`.
- **Wave Frequency & Count**: Open **`MOBA/GameConfig.cs`** → `WaveIntervalSeconds` (e.g. `30f`), `MinionsPerWave` (`3`), `RangedMinionsPerWave` (`1`).
- **Lane Waypoints**: Open **`MOBA/LanePaths.cs`** → edit coordinates in `BlueFromFallbacks()` and `RedFromFallbacks()`.
- **Win Condition**: Destroying the enemy nexus base (**`MOBA/WinSystem.cs`**).

---

### Mode 2: 1v1 / 2v2 Single-Lane Arena (`GameMode.ArenaBrawl`)
*Fast-paced single center-lane brawl with rapid waves and extra gold.*
- **Make It Single Lane**: Automatically activates when `CurrentMode = GameMode.ArenaBrawl` (**`MOBA/MapScript.cs`** loads 1 center lane instead of 3).
- **Faster Waves**: Open **`MOBA/GameConfig.cs`** → set `WaveIntervalSeconds = 15f;` or `20f;`.
- **Boost Gold Income**: Open **`MOBA/GameConfig.cs`** → increase `MinionKillGold`, `HeroKillGold`, or `TowerKillGold`.
- **Hero Stats**: Open **`MOBA/metadata.json`** → search for `"adventurer"` or `"armored_dragon"` to tweak starting HP/Armor/Damage for arena fights.

---

### Mode 3: Capture & Hold / Control Points (`GameMode.ControlPoints`)
*Capture neutral zones on the map to drain enemy life tickets.*
- **Control Point Positions**: Define custom point coordinates in **`MOBA/Coordinates.cs`** or **`MOBA/CoordinateFallbacks.cs`**.
- **Ticket Drain Rate & Capture Speed**: In **`MOBA/MapScript.cs`** (or your custom point system), use `api.GetUnitsInRadius(pointPos, radius)` in `Update()` to detect team control and subtract tickets.
- **Victory Condition**: Trigger `api.TriggerVictory()` or `api.TriggerDefeat()` when tickets reach 0.

---

### Mode 4: Boss Objective Lane (`GameMode.BossObjective`)
*Neutral jungle boss that grants team-wide damage buffs when slain.*
- **Boss Spawn Location & Timer**: Spawn neutral boss entity using `api.SpawnUnit("turtle", bossPosition, true)` on a repeating timer (`api.ScheduleRepeatingTimer(180f)`).
- **Boss Buff Effect**: Register buff modifier in `Initialize()` via `api.RegisterBuffModifier("boss_buff", "Attack", true, 0.35f)`.
- **Awarding the Buff**: In `OnUnitDied()` in **`MOBA/MapScript.cs`**, check if `victim.UnitId == "turtle"` and call `unit.AddBuff("boss_buff", 60f)` on the killer team.

---

### Mode 5: Hero Deathmatch / Brawler (`GameMode.HeroDeathmatch`)
*Coliseum battle with healing pickups where the first team to reach 15 kills wins.*
- **Target Kill Goal**: Set a target kill count (e.g., 15 kills).
- **Tracking Kills**: In **`MOBA/HeroSystem.cs`** or **`MOBA/MapScript.cs`** → `OnUnitDied()` increments player/enemy kill counters.
- **Fast Respawns**: Set `GameConfig.HeroRespawnSeconds = 3f;` or `5f;` for rapid respawning back into the arena.
- **Health Pickups**: In `Update()`, check `api.GetUnitsInRadius(pickupPos, 3f)` and restore health (`unit.Health += 50f`).

---

## 📁 3. Core Files Quick Index

| File | What It Controls |
|---|---|
| **`MOBA/GameConfig.cs`** | Active mode, player indexes, wave timers, minion counts, gold values, respawn delay |
| **`MOBA/metadata.json`** | Unit & building baseline stats (HP, Damage, Speed, Range, Armor, Gold Bounty) |
| **`MOBA/MapScript.cs`** | Main match initialization, tick loop, death listeners, mode routing |
| **`MOBA/HeroSystem.cs`** | Hero spawning, death tracking, revival queue |
| **`MOBA/MinionWaveSystem.cs`**| Minion wave timers, group spawning, and waypoint marching logic |
| **`MOBA/LanePaths.cs`** | X, Y, Z waypoint coordinates for minion travel |
| **`MOBA/GoldSystem.cs`** | Gold distribution and kill bounties |
| **`MOBA/WinSystem.cs`** | Base destruction check and victory/defeat sequence |

---

## ⚡ 4. Testing Your Changes
Whenever you make a change, simply save the file and click **Test** in the Realm Editor!
