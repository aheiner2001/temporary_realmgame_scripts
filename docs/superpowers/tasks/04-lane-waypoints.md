# Task 4 — Lane waypoints

**When:** Day 2  
**Plan:** Task 4 in `docs/superpowers/plans/2026-08-14-moba-map-scripts.md`  
**Done when:** `dotnet test ... --filter WaypointProgressTests` passes, and `Coordinates.Resolve` exists

## Skills you are learning

- A lane is just a list of points
- Minions advance to the next point when they get close (4 units)

## Steps

- [ ] Add tests + Compile includes from the plan (`WaypointProgress`, `LanePaths` only — not `Lane.cs`)
- [ ] Run tests; confirm **fail**
- [ ] Create `WaypointProgress.cs`, `LanePaths.cs`, `Lane.cs`, and fill in `Coordinates.cs`
- [ ] Run tests; confirm **pass**
- [ ] Commit as in the plan

## Why two lane files?

`Lane.cs` talks to `IGameAPI`. Tests cannot compile that without the engine DLL. `LanePaths.cs` is the same path data with no engine call.
