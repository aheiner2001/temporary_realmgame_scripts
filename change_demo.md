# How to Select and Run Game Modes in the Realm Editor

This document explains how to switch between different MOBA demo game modes and test them directly in the **Realm Map Editor**.

---

## 1. Available Game Modes


| Mode Enum | Game Mode Name            | Description                                                                           |
| --------- | ------------------------- | ------------------------------------------------------------------------------------- |
|           | **3-Lane Standard Push**  | Classic 3-lane MOBA push with minions, towers, and base destruction victory.          |
|           | **1v1 / 2v2 Arena Brawl** | Fast-paced single-lane battle with rapid waves, condensed bounds, and bonus gold.     |
|           | **Capture & Hold**        | 3 neutral capture zones across the map that drain enemy tickets down to 0.            |
|           | **Boss Objective Lane**   | Single-lane push featuring a neutral Jungle Titan that grants team-wide damage buffs. |
|           | **Hero Deathmatch**       | Pure hero coliseum with healing orb pickups. First team to reach 15 hero kills wins.  |


---



## 2. How to Select a Game Mode

1. Open the file **** in your code editor.
2. Locate the **** property near the top (around line 14):
3. Change  to your desired mode:
  - 
  ##  - 
  - 
4. **Save** the file ( or ).

---



## 3. How to Run & Test from the Realm Editor

1. Open the **Realm** application.
2. In the Map Editor, select **Open Map** (or open your workspace folder):
  - Choose the **** folder ().
3. Click the **Test** button in the top menu bar.
4. The editor will automatically:
  - Compile the C# map scripts to a sandboxed **WebAssembly (WASM)** binary.
  - Load the terrain, structures, and heroes.
  - Run the game match according to the rules of the selected .

---



## 4. Verifying Build & Tests via Terminal (Optional)

You can verify that the C# code compiles and all unit tests pass at any time before running the editor:

  Determining projects to restore...
  All projects are up-to-date for restore.
  MOBA.Tests -> /Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA.Tests/bin/Debug/net10.0/MOBA.Tests.dll
Test run for /Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA.Tests/bin/Debug/net10.0/MOBA.Tests.dll (.NETCoreApp,Version=v10.0)
VSTest version 18.0.2 (arm64)

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    34, Skipped:     0, Total:    34, Duration: 35 ms - MOBA.Tests.dll (net10.0)
  Determining projects to restore...
  All projects are up-to-date for restore.
/Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA/WasmEntryPoint.cs(31,42): warning IL2026: Using member 'System.Reflection.Assembly.GetTypes()' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. Types might be removed. [/Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA/CustomMap.csproj]
/Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA/WasmEntryPoint.cs(35,55): warning IL2072: 'type' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicParameterlessConstructor' in call to 'System.Activator.CreateInstance(Type)'. The return value of method 'System.Collections.IEnumerator.Current.get' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to. [/Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA/CustomMap.csproj]
  CustomMap -> /Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA/bin/Debug/net10.0/wasi-wasm/CustomMap.dll

Build succeeded.

/Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA/WasmEntryPoint.cs(31,42): warning IL2026: Using member 'System.Reflection.Assembly.GetTypes()' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. Types might be removed. [/Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA/CustomMap.csproj]
/Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA/WasmEntryPoint.cs(35,55): warning IL2072: 'type' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicParameterlessConstructor' in call to 'System.Activator.CreateInstance(Type)'. The return value of method 'System.Collections.IEnumerator.Current.get' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to. [/Users/aaronheiner/Sandbox/temporary_realmgame_scripts/MOBA/CustomMap.csproj]
    2 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.44