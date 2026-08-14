# Task 5 — Unit stats

**When:** Day 2  
**Plan:** Task 5 in `docs/superpowers/plans/2026-08-14-moba-map-scripts.md`  
**Done when:** `python3` JSON check prints `ok` and the map still loads

## Skills you are learning

- Stats live in `metadata.json` (HP, damage, range)
- Behavior lives in C#
- Reuse existing models (`adventurer.glb`, `castle.glb`, …)

## Steps

- [ ] Replace `CustomUnits` and add `melee_minion` using the plan’s JSON
- [ ] Add stats to `castle` and `castle_tower_1` only
- [ ] Run: `python3 -c "import json; json.load(open('MOBA/metadata.json')); print('ok')"`
- [ ] Play: map still loads
- [ ] Commit as in the plan

## Do not

Edit `terrain.json`. Do not change unrelated buildings.
