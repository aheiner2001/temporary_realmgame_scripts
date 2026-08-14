# Task checklists (do these in order)

Full code for every step is in:

`docs/superpowers/plans/2026-08-14-moba-map-scripts.md`

Open **one** task file below. Finish it. Commit. Then open the next.

| When | File | What “done” looks like |
|---|---|---|
| Today | [01-setup-and-smoke.md](01-setup-and-smoke.md) | Map plays; chat says `MOBA scripts loaded` |
| Today | [02-game-config.md](02-game-config.md) | `dotnet test` passes for config numbers |
| Day 2 | [03-coordinates.md](03-coordinates.md) | Fallback positions unit-tested |
| Day 2 | [04-lane-waypoints.md](04-lane-waypoints.md) | Minion path math unit-tested |
| Day 2 | [05-unit-stats.md](05-unit-stats.md) | metadata.json has HP/damage; map still loads |
| Day 2 | [06-win-decision.md](06-win-decision.md) | Castle-dead → winner unit-tested |
| Day 2 | [07-teams-buildings-win.md](07-teams-buildings-win.md) | Two castles + towers; destroy castle to win |
| Day 2 | [08-minion-waves.md](08-minion-waves.md) | **Show this:** waves walk the lane |
| Next week | [09-gold.md](09-gold.md) | `+15 gold` on minion kill |
| Next week | [10-heroes-respawn.md](10-heroes-respawn.md) | **Devin demo:** hero + 8s respawn |

Engine repo: [speige/Realm](https://github.com/speige/Realm.git), cloned as `Realm/` next to `MOBA/`. If you get stuck more than 90 minutes on Godot, ask Devin which 4.7 Mono build to run.
