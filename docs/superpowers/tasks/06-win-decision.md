# Task 6 — Win decision

**When:** Day 2  
**Plan:** Task 6 in `docs/superpowers/plans/2026-08-14-moba-map-scripts.md`  
**Done when:** `dotnet test ... --filter WinDecisionTests` passes

## Skills you are learning

- Pure functions are easy to test: flags in, winner out
- The engine call (`TriggerPlayerVictory`) comes in the next task

## Steps

- [ ] Add `WinDecisionTests.cs` from the plan
- [ ] Run tests; confirm **fail**
- [ ] Create `MOBA/WinDecision.cs`
- [ ] Run tests; confirm **pass**
- [ ] Commit as in the plan

## Rule

Red castle dead → Blue wins. Blue castle dead → Red wins. Both dead → Blue wins. Neither → none.
