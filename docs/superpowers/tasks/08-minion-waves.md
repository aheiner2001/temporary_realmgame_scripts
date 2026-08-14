# Task 8 — Minion waves (2-day milestone)

**When:** End of day 2 — this is what you can show  
**Plan:** Task 8 in `docs/superpowers/plans/2026-08-14-moba-map-scripts.md`  
**Done when:** At ~30s, 3 minions per side spawn and walk toward the enemy castle

## Skills you are learning

- Timers: `ScheduleRepeatingTimer(30)`
- `AttackMove` down a waypoint list
- `Update` ticks to advance to the next point

## Steps

- [ ] Replace `MinionWaveSystem.cs` with the plan code
- [ ] Call `_waves.Start(api, lane)` from `Initialize`
- [ ] `dotnet test MOBA.Tests/MOBA.Tests.csproj` still passes
- [ ] Play, wait 30s: 3 vs 3 minions march and fight
- [ ] Destroying a castle still wins
- [ ] Commit as in the plan

## Show Devin

“Waves spawn, walk the lane, and you win by killing the castle.” Heroes can wait until next week.
