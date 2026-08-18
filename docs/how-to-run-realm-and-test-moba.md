# How to run Realm and test the MOBA map

Two separate projects:

| Piece | What it is | Repo / download |
|---|---|---|
| **Realm** | Game + map editor | [speige/Realm](https://github.com/speige/Realm) or a release zip |
| **MOBA** | Your map (scripts, terrain, stats) | This repo → `MOBA/` folder |

You run **Realm**, then **LOAD** the `MOBA/` folder. You do not run `MOBA/` by itself.

Recommended layout on Windows:

```text
temporary_realmgame_scripts\
  MOBA\          ← this map repo
  docs\
  Realm\         ← engine clone (optional; see Option B below)
```

Do **not** commit `Realm/` into the map git repo.

---

## Option A — Pre-built release (easiest; test your map)

Use this to **LOAD → TEST** your MOBA scripts. No GitHub clone required.

1. Download **Rev_7** (or latest pre-alpha):  
   https://github.com/speige/Realm/releases/tag/0.0.1_Pre-Alpha_Rev_7  
   File: `Realm-Godot-Windows-x64_0.0.1_Pre-Alpha_Rev_7.7z`
2. Unzip anywhere and run **`Realm.Godot.exe`**
3. Accept the asset license if prompted
4. In the map editor: **LOAD** → `temporary_realmgame_scripts\MOBA\`  
   (must contain `MapScript.cs` and `terrain.json`)
5. Click **TEST**

**Pull latest map code first:**

```powershell
cd path\to\temporary_realmgame_scripts
git pull
```

Then LOAD `MOBA\` again (the editor copies the map to a temp folder).

---

## Option B — Run from GitHub source (engine / API work)

Use this when you need to **change `IGameAPI`** or other engine code, not for everyday map testing.

### Prerequisites (Windows)

- **Git** (enable **Git LFS** if asked)
- **.NET 10 SDK** (engine wants **10.0.302+**): https://dotnet.microsoft.com/download/dotnet/10.0
- **Godot 4.7 .NET** (C# build, not plain GDScript): https://godotengine.org/download  
  If the standard 4.7 .NET editor fails, ask Devin for the team’s exact Godot build.

For **map TEST** (WASM compile), you may also need **WASI SDK 30** — see [01-setup-and-smoke.md](superpowers/tasks/01-setup-and-smoke.md) (clang / WASI section).

### Clone the engine next to MOBA

```powershell
cd path\to\temporary_realmgame_scripts
git clone --recursive https://github.com/speige/Realm.git Realm
cd Realm
git checkout 0.0.1_Pre-Alpha_Rev_7
```

Confirm these exist:

- `Realm\Realm.slnx`
- `Realm\Realm.MapAPI\`
- `Realm\Realm.Godot\project.godot`

### Build C# projects

```powershell
cd Realm
dotnet build Realm.slnx
```

First build may take several minutes (NuGet restore).

### Open in Godot

1. Launch **Godot 4.7 .NET**
2. **Import** or **Open** → `Realm\Realm.Godot\project.godot`
3. Let Godot build the C# project when prompted
4. Accept the **asset license** if prompted (downloads default assets)

### Test your map (same as the zip)

1. **LOAD** → `..\MOBA\` (sibling of `Realm\`, not inside it)
2. **TEST**

---

## Changing engine APIs (`IGameAPI`)

| Task | Where |
|---|---|
| Edit API | `Realm\Realm.MapAPI\IGameAPI.cs` |
| Build | `dotnet build Realm.MapAPI` |
| Refresh map DLL | Copy output into `MOBA\lib\` (engine docs also mention `MapTemplate\lib\`) |
| Map gameplay | Stay in **`MOBA/`** — call the API; do not put MOBA rules in the engine |

Engine changes belong in **speige/Realm** (PR when Devin asks). Do not commit `Realm/` into this map repo.

---

## What you should see on TEST

1. **Terrain** (ground/hills) after LOAD — fog “no units placed” dialog is OK; click **Okay**
2. WASM compile window finishes without red errors (first TEST can take **2–10+ minutes**)
3. Chat lines (on-screen text):
   - `MOBA scripts loaded`
   - `Guest WasmModule initialized successfully`
   - `Debug: spawned …` / `Debug: blue castle id=…` (while `GameConfig.DebugChat` is `true`)
4. Units near **map corners** — pan the camera if the view looks empty
5. After ~30s: `Debug: wave tick, 3 lanes`

**If spawn fails:** `Debug: spawn FAILED …` — unit type ids may have changed in Devin’s new data; ask which ids to use.

**If no script chat at all:** WASM did not run — check the compile console, not the map scripts.

---

## Quick checks

```powershell
# Map logic (no editor)
dotnet test MOBA.Tests\MOBA.Tests.csproj
# Expect: 34 passed
```

| Symptom | Likely cause |
|---|---|
| No terrain | Wrong folder (use `MOBA\`, not `MOBA.Tests\`) |
| Long LOAD | Large `terrain.json` (~970 KB) — normal |
| Long first TEST | WASM compile — normal if console is still updating |
| `Bypassing compilation using existing WASM binary` | Stale WASM — save `MapScript.cs`, LOAD `MOBA\` again, TEST |
| `dotnet publish failed` / `error CS` | Paste full compile log |
| Chat but no units | Pan to corners; look for `spawn FAILED` |

---

## Zip vs source — which to use

| Goal | Use |
|---|---|
| Test MOBA scripts, demo to Devin | **Option A** (release zip) |
| Add `ReviveUnit`, edit `IGameAPI` | **Option B** (GitHub clone + Godot .NET) |

If setup from source fails after ~90 minutes, ask Devin:  
“I cloned `speige/Realm` at `0.0.1_Pre-Alpha_Rev_7`, .NET 10 + Godot 4.7 .NET installed — which Godot build and open steps should I use?”

More WASM/toolchain detail: [superpowers/tasks/01-setup-and-smoke.md](superpowers/tasks/01-setup-and-smoke.md)

---

## Playtest checklist (15 tests)

After TEST works, work through: **[moba-playtest-checklist.md](moba-playtest-checklist.md)**  
Includes what’s normal vs broken, troubleshooting order, and next steps (screenshots + coordinates).
