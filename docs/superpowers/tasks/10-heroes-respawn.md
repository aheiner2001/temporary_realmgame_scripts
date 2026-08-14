# Task 10 — Heroes and respawn (supervisor demo)

**When:** Next week  
**Plan:** Task 10 in `docs/superpowers/plans/2026-08-14-moba-map-scripts.md`  
**Done when:** You have a hero; dying respawns it at the fountain in 8s; destroying the castle still wins

## Skills you are learning

- One hero per player (the genre shift vs Age of Empires)
- Fake revive: wait, then `SpawnUnitForPlayer` again (no engine `ReviveUnit` yet)
- Do not respawn after the match has ended

## Steps

- [ ] Replace `HeroSystem.cs` with the plan code
- [ ] Call `SpawnStartingHeroes` from `Initialize`
- [ ] `dotnet test` still passes
- [ ] Play the full checklist in the plan (hero, waves, respawn, gold, win)
- [ ] Commit as in the plan

## Demo script for Devin

1. Chat loads  
2. You control a hero  
3. Minions wave  
4. Die → come back in 8s  
5. Kill their castle → win  

That is the assignment.
