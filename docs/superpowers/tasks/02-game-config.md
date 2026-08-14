# Task 2 — GameConfig

**When:** Today  
**Plan:** Task 2 in `docs/superpowers/plans/2026-08-14-moba-map-scripts.md`  
**Done when:** `dotnet test MOBA.Tests/MOBA.Tests.csproj --filter GameConfigTests` passes

## Skills you are learning

- TDD: write a failing test, then the code
- Put match numbers in one file so you do not hunt magic numbers later

## Steps

- [ ] Create the test project + `GameConfigTests.cs` from the plan
- [ ] Run the test; confirm it **fails**
- [ ] Create `MOBA/GameConfig.cs` from the plan
- [ ] Run the test; confirm it **passes**
- [ ] Commit as in the plan

## Tip

If `net10.0` is missing, the plan says to switch the test project to `net8.0`.
