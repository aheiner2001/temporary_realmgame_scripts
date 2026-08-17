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

The editor/game is easiest to run on Windows. You need **two clones**, not one, and Realm must sit **next to** `MOBA/` (not in some unrelated folder).

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

1. Do **not** copy the Mac `Realm\` folder by hand. A fresh clone on Windows is cleaner.
2. Confirm you see `Realm\Realm.slnx`, `Realm\Realm.MapAPI\`, and `Realm\Realm.Godot\project.godot`.
3. Continue the Steps below: run `Realm.Godot` → **LOAD** `MOBA\` → **TEST**.

LOAD/TEST works as long as you can browse to `MOBA\`. The sibling `Realm\` folder is also what the VS Code “Re-generate Map API” task expects.

## Steps

- [x] Watch the three videos/pages listed in the plan (skip last-hitting / jungle / ranked)
- [x] Confirm the engine is a **full** checkout:
  - You should see `Realm/Realm.slnx`, `Realm/Realm.MapAPI/`, and `Realm/Realm.Godot/project.godot`
  - If those are missing, from `Realm/` run `git checkout .` or re-clone:
  `git clone https://github.com/speige/Realm.git Realm`
- [x] Install **.NET 10** (engine `global.json` wants SDK `10.0.302`). Godot **4.7 Mono** (the team’s editor build — ask Devin if you do not have it)
- [x] Run the Realm editor (`Realm.Godot`). Accept the asset license if prompted
- [x] In the map editor: **LOAD** → pick this folder: `MOBA/` (the one with `terrain.json` and `MapScript.cs`)
- [x] Click **TEST** (launches single-player on the loaded map). You should see Isabella’s terrain. An empty unit list is expected
- [ ] Change `MOBA/MapScript.cs` to the code in the plan (broadcast message)
- [ ] **TEST** again: see `MOBA scripts loaded` in chat
- [ ] Commit as in the plan



## If you are stuck

Ask Devin: “I have `speige/Realm` cloned next to `MOBA/`. Which Godot 4.7 Mono build should I use to run `Realm.Godot`?” Do not spend more than 90 minutes on Godot setup.

## TEST failed: NU1101 / ILCompiler.LLVM

**LOAD working + TEST aborting is expected until WASM packages restore.** TEST runs `dotnet publish` on a temp copy of the map. Those LLVM packages are **not** on nuget.org.

1. This repo now has `MOBA/NuGet.Config` with the `dotnet-experimental` feed.
2. Copy that file into the editor temp workspace (or re-LOAD `MOBA\` so the editor copies it):

```text
C:\Users\User\AppData\Roaming\Godot\app_userdata\Realm.Godot\temp_map_workspace\NuGet.Config
```

1. The machine needs internet the first time TEST compiles.
2. Press **TEST** again. Restore should find `Microsoft.DotNet.ILCompiler.LLVM` and `runtime.win-x64.Microsoft.DotNet.ILCompiler.LLVM`.

If it still fails, from PowerShell (to confirm the feed works):

```powershell
dotnet restore "C:\Users\User\AppData\Roaming\Godot\app_userdata\Realm.Godot\temp_map_workspace\CustomMap.csproj"
```

Paste that output if restore still errors.

## TEST failed: clang / “The system cannot find the path specified”

NuGet worked. TEST is now failing while **linking WASM** with `clang`. It tried:

`C:\Program Files\dotnet\packs\Microsoft.NET.Runtime.WebAssembly.Wasi.Sdk\10.0.10\wasi-sdk\bin\clang`

That path is incomplete. Realm wants **WASI SDK 30** (`bin\clang.exe`). This is a **toolchain install on your PC**, not a change to Devin’s game.

**1. See if the prealpha already has it**

Next to `Realm.Godot.exe` (or in the unzipped prealpha folder), look for:

`wasi_sdk_embedded\bin\clang.exe`

If that file exists, TEST should find it. If TEST still uses the `dotnet\packs\...` path, set the environment variable in step 3 to that `wasi_sdk_embedded` folder.

**2. If** `clang.exe` **is missing, download WASI SDK 30** (same URL the engine’s install script uses)

In PowerShell:

```powershell
$dest = "$env:USERPROFILE\wasi-sdk-30"
New-Item -ItemType Directory -Force -Path $dest | Out-Null
$tar = "$env:TEMP\wasi-sdk-30.tar.gz"
curl.exe -L "https://github.com/WebAssembly/wasi-sdk/releases/download/wasi-sdk-30/wasi-sdk-30.0-x86_64-windows.tar.gz" -o $tar
tar.exe -xf $tar -C $dest --strip-components=1
Test-Path "$dest\bin\clang.exe"
```

That last line must print `True`.

**3. Point TEST at it** (User environment variable, then **restart the prealpha**)

```powershell
[System.Environment]::SetEnvironmentVariable("WASI_SDK_PATH", "$env:USERPROFILE\wasi-sdk-30", "User")
```

Close Realm completely, open it again, LOAD `MOBA`, TEST.

You are not editing `Realm\` or the prealpha source. You are installing `clang` so his editor can compile **your** map.