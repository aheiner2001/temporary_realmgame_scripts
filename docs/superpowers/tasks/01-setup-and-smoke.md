# Task 1 — Setup and smoke

**When:** Today  
**Plan:** Task 1 in `docs/superpowers/plans/2026-08-14-moba-map-scripts.md`  
**Done when:** Playing the map shows chat `MOBA scripts loaded`

## Skills you are learning

- What a MOBA *is* (lanes, minions, towers, one hero, destroy the base)
- This map repo vs the engine repo
- Load `MOBA/` in the Realm editor and see your C# run

## Layout (this workspace)

```text
temporary_realmgame_scripts/
  MOBA/          ← your map (this assignment)
  Realm/         ← engine clone of https://github.com/speige/Realm.git
    Realm.Godot      game + editor
    Realm.MapAPI     generic IGameAPI source
    Realm.slnx       open this in an IDE
    MapTemplate      boilerplate for new maps
```

Do **not** commit the `Realm/` folder into the map git repo. It is a separate GitHub project.

## Moving to a Windows computer

The editor/game is easiest to run on Windows. You need **two clones**, not one, and Realm must sit **next to `MOBA/`** (not in some unrelated folder).

```text
temporary_realmgame_scripts\
  MOBA\          ← from this map repo
  docs\
  Realm\         ← clone the engine here (second git repo)
```

On the Windows machine:

1. Install Git (enable **Git LFS** if asked), **.NET 10 SDK**, and Godot **4.7 Mono** (ask Devin for the team’s editor build if you do not have an installer).
2. Clone **this map repo** (the one that contains `MOBA/` and `docs/`).
3. Inside that folder, clone the engine as `Realm` (this does **not** duplicate the map repo):

```powershell
cd temporary_realmgame_scripts
git clone https://github.com/speige/Realm.git Realm
```

4. Do **not** copy the Mac `Realm\` folder by hand. A fresh clone on Windows is cleaner.
5. Confirm you see `Realm\Realm.slnx`, `Realm\Realm.MapAPI\`, and `Realm\Realm.Godot\project.godot`.
6. Continue the Steps below: run `Realm.Godot` → **LOAD** `MOBA\` → **TEST**.

LOAD/TEST works as long as you can browse to `MOBA\`. The sibling `Realm\` folder is also what the VS Code “Re-generate Map API” task expects.

## Steps

- [x] Watch the three videos/pages listed in the plan (skip last-hitting / jungle / ranked)
- [x] Confirm the engine is a **full** checkout:
  - You should see `Realm/Realm.slnx`, `Realm/Realm.MapAPI/`, and `Realm/Realm.Godot/project.godot`
  - If those are missing, from `Realm/` run `git checkout .` or re-clone:
  `git clone https://github.com/speige/Realm.git Realm`
- [ ] Install **.NET 10** (engine `global.json` wants SDK `10.0.302`). Godot **4.7 Mono** (the team’s editor build — ask Devin if you do not have it)
- [ ] Run the Realm editor (`Realm.Godot`). Accept the asset license if prompted
- [ ] In the map editor: **LOAD** → pick this folder: `MOBA/` (the one with `terrain.json` and `MapScript.cs`)
- [ ] Click **TEST** (launches single-player on the loaded map). You should see Isabella’s terrain. An empty unit list is expected
- [ ] Change `MOBA/MapScript.cs` to the code in the plan (broadcast message)
- [ ] **TEST** again: see `MOBA scripts loaded` in chat
- [ ] Commit as in the plan



## If you are stuck

Ask Devin: “I have `speige/Realm` cloned next to `MOBA/`. Which Godot 4.7 Mono build should I use to run `Realm.Godot`?” Do not spend more than 90 minutes on Godot setup.