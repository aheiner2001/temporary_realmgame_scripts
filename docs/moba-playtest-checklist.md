# MOBA playtest checklist

Use this after **LOAD `MOBA/` → TEST** in Realm Rev_7 (or newer).  
Setup: [how-to-run-realm-and-test-moba.md](how-to-run-realm-and-test-moba.md)

**How to use:** Run each test once per session. Mark **Pass / Fail / N/A** and write one line of notes. Share the filled checklist with Devin.

---

## Before you play

- [ ] `git pull` on the map repo
- [ ] LOAD folder is `...\MOBA\` (has `MapScript.cs` + `terrain.json`)
- [ ] Using **Rev_7** (or newer), not the old pre-alpha zip
- [ ] WASM compile finished with **no red errors**
- [ ] `dotnet test MOBA.Tests\MOBA.Tests.csproj` → **34 passed** (optional, on any PC)

---

## Core smoke (scripts ran)

| # | Test | Pass? | Notes |
|---|---|---|---|
| 1 | Chat shows **`MOBA scripts loaded`** | | |
| 2 | Chat shows **`Guest WasmModule initialized successfully`** | | |
| 3 | Chat shows **`Debug: spawned castle…`** (both teams) | | |
| 4 | Chat shows **`Debug: blue castle id=… red castle id=…`** | | |
| 5 | **No** `Debug: spawn FAILED` lines at match start | | |

If #1 fails → WASM/compile problem (not gameplay). Stop and fix compile first.

---

## Map & units visible

| # | Test | Pass? | Notes |
|---|---|---|---|
| 6 | You see **terrain** (not a black screen) | | |
| 7 | You can find **castles/towers** on the map (minimap dots help) | | |
| 8 | You see **two heroes** (adventurer + armored_dragon) near team bases | | |
| 9 | Units are on **walkable ground**, not stuck in a river/cliff | | Often **Fail** until coordinates match Isabella’s terrain |

---

## MOBA behavior (demo rules)

| # | Test | Pass? | Notes |
|---|---|---|---|
| 10 | After **~30 seconds**, chat shows **`Debug: wave tick, 3 lanes`** | | |
| 11 | **New minions** appear after the wave tick (more units over time) | | Expected — causes lag on weak PCs |
| 12 | Minions **move toward the enemy side** without you ordering them | | They use attack-move on waypoints |
| 13 | **Combat happens** (e.g. “under attack” messages, units die) | | |
| 14 | Killing a minion gives **`+15 gold`** (or debug gold line) to the killer’s player | | |
| 15 | **Destroy enemy castle** → **`Blue wins!`** or **`Red wins!`** and match ends | | Hardest test; may take several minutes |

---

## Known v1 limitations (not always Fail)

These are **expected in the current demo**, not necessarily bugs:

| What you might see | Why |
|---|---|
| **Lag** after 1–2 minutes | 3 lanes × 2 teams × 4 minions every 30s = many units |
| **Can select every unit** in TEST | Editor TEST often gives full control; not a polished 1-hero MOBA UI yet |
| **Both teams look “red” or you control everyone** | Team colors / player lock are engine/UI; scripts set player 0 = Blue, player 1 = Red |
| **Spawned in a river / wrong place** | Fallback coordinates `(16,16)…(112,112)` don’t match real terrain yet |
| **`Debug: wave tick, 3 lanes`** | Code spawns 3 parallel lanes (spec wanted 1 lane first; 3 lanes were added later) |
| **Hero doesn’t auto-attack** | You may need to order attacks manually; minions auto-path |

---

## If TEST isn’t showing what we built

Stop at the **first** broken step:

1. **Wrong folder?** → LOAD `MOBA\`, not `MOBA.Tests` or Realm zip  
2. **`git pull`** → LOAD again → TEST  
3. **Compile failed?** → paste full WASM console  
4. **`Bypassing compilation…`** → save `MapScript.cs`, reload, TEST  
5. **No chat lines** → WASM didn’t run (Step 3)  
6. **`spawn FAILED`** → ask Devin if unit ids changed  
7. **Chat OK, no models** → pan to corners; check spawn positions  
8. **`dotnet test`** → 34 pass means logic OK, editor/spawn issue  

---

## Your next step: align spawns with the real map

**Yes — screenshot + coordinates is the right next move.**

Isabella’s terrain is real; our script uses **guessed numbers** in `MOBA/CoordinateFallbacks.cs` until named boxes exist in terrain.

**Do this:**

1. In the **editor** (LOAD only, no TEST), pan to where Blue base / Red base / lane **should** be.
2. Note positions from the editor (or place named coordinate boxes if the editor supports them — ask Devin/Isabella).
3. Send Devin (or your AI session):
   - **Screenshot** of the map (top-down if possible)
   - **List:** Blue fountain, Blue castle, Blue tower, lane mid, Red mirror — as `(x, y, z)` or editor labels
4. We update **`CoordinateFallbacks.cs`** only (not `terrain.json`) so castles/heroes/minions spawn on land.

Example message:

> “Heroes spawned in the river at fallback `(16,0,16)`. Blue base should be at `(???, 0, ???)` — see attached screenshot.”

---

## Internship next steps (learning + AI-assisted dev)

**This week — prove the demo**

1. Fill out this checklist once; send results to Devin.  
2. Screenshot + coordinate list for spawn fixes.  
3. Record a **30–60s clip**: waves walking, one fight, castle win (if you get there).

**Learn the split**

- **`MOBA/`** = your assignment (rules, waves, win, stats).  
- **Realm engine** = only when Devin asks for new APIs (`IGameAPI`).

**Use AI well**

- Ask AI to **explain** chat lines and which file owns each behavior (`MapScript`, `MinionWaveSystem`, etc.).  
- Ask AI to **change one thing at a time** (e.g. only fallbacks, or only wave interval).  
- **You** run TEST and report Pass/Fail — AI can’t see your game.

**Before supervisor demo**

- Set `GameConfig.DebugChat = false` in `MOBA/GameConfig.cs`.  
- Confirm spawns on land after coordinate fix.  
- Optional: ask Devin if you should **drop to 1 lane** temporarily to reduce lag.

**Questions for Devin**

1. Should TEST lock me to **one hero**, or is full select OK for now?  
2. Are unit ids `castle`, `adventurer`, `melee_minion` still correct in Rev_7 data?  
3. When will Isabella add **named coordinates** on terrain so we stop using fallbacks?

---

## Quick reference — what “good” looks like

```
MOBA scripts loaded
Guest Initialize started
Debug: spawned castle id=… at (…) for player 0
Debug: spawned castle id=… at (…) for player 1
… towers, heroes …
Debug: blue castle id=12 red castle id=34
Guest WasmModule initialized successfully
… wait 30s …
Debug: wave tick, 3 lanes
… fight …
Blue wins!   (or Red wins!)
```
