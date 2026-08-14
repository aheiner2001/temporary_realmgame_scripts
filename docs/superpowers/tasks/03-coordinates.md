# Task 3 — Coordinate fallbacks

**When:** Day 2  
**Plan:** Task 3 in `docs/superpowers/plans/2026-08-14-moba-map-scripts.md`  
**Done when:** `dotnet test ... --filter CoordinateFallbackTests` passes

## Skills you are learning

- Named places on a map (`BlueFountain`, `MidLane1`, …)
- Hardcoded fallbacks until Isabella adds named boxes in terrain

## Steps

- [ ] Add the test file and Compile include from the plan
- [ ] Run tests; confirm **fail**
- [ ] Create `MOBA/CoordinateFallbacks.cs`
- [ ] Run tests; confirm **pass**
- [ ] Commit as in the plan

## Later

If units spawn in rocks or underground, change **only** these numbers, then re-run this test.
