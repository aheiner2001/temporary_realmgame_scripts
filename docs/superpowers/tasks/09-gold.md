# Task 9 — Gold

**When:** Next week  
**Plan:** Task 9 in `docs/superpowers/plans/2026-08-14-moba-map-scripts.md`  
**Done when:** Killing a minion shows `+15 gold` for the killer’s player

## Skills you are learning

- `OnUnitDied` events
- Gold lives on the **player**, not the unit (so respawn does not wipe it)
- `ownerPlayer` custom data tells you who to pay

## Steps

- [ ] Add `GoldTableTests.cs`; run; confirm **fail**
- [ ] Create `GoldTable.cs` and implement `GoldSystem.cs`
- [ ] Construct `_gold = new GoldSystem()` in `Initialize`
- [ ] Tests pass
- [ ] Play: minion death → `+15 gold`
- [ ] Commit as in the plan

## Amounts

Minion 15, hero 150, tower 100, castle 0. Missing killer → skip, do not crash.
