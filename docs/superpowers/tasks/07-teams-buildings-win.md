# Task 7 — Teams, buildings, win

**When:** Day 2  
**Plan:** Task 7 in `docs/superpowers/plans/2026-08-14-moba-map-scripts.md`  
**Done when:** Two castles and two towers spawn; destroying the Red castle shows `Blue wins!`

## Skills you are learning

- `Initialize` sets up the match once
- `SetPlayerTeam` / spawn / `SetCustomData("ownerPlayer", ...)`
- End the match **once** (`HasEnded`)

## Steps

- [ ] Create `OwnerTag.cs`, `TeamSetup.cs`, `WinSystem.cs` from the plan
- [ ] Replace `MapScript.cs` and add the three stubs (`MinionWaveSystem`, `HeroSystem`, `GoldSystem`)
- [ ] If `OnUnitDied` does not compile, use the `(int, int)` handler in the plan
- [ ] Play: two castles, two towers, chat still works
- [ ] Play: destroy Red castle → `Blue wins!` once
- [ ] Commit as in the plan

## If units spawn in the wrong place

Change numbers in `CoordinateFallbacks.cs` only, then re-run Task 3 tests.
