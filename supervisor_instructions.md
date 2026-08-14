

For your next task, I'd like you to work on the scripts for a custom map. Not sure if you've ever played a MOBA (like League of Legends). If not, I can send you a WC3 map that's similar to experiment with.
This is an unfinished terrain (Isabella is working on it). I'd like you to use the Realm editor to code the scripts for the gameplay.

my question(
Should these scripts be specific to this map, or built more generically for reuse in future maps? Also, should the player count be fixed (e.g., 5v5), or should it be customizable/scalable for different lobby sizes?)
Devin Garner  [9:45 AM]
The core game engine exposes a set of APIs as IGameAPI. It has things like 'MoveUnit'. Those should stay generic & reusable for any map.

The map itself should be in a completely different github repo & should be specific to a MOBA & does not need to be reusable. This would be the logic for the specific walking path of the minions.

If there's a missing generic API, like 'ReviveUnit' you should add it to the core engine.

Both are c#, but there's a sandbox layer to restrict what mods can do to prevent hacking. The map compiles from c# to WebAssembly & the core engine runs it through a WebAssembly VirtualMachine.
[9:50 AM]I don't have specific requirements on how the MOBA itself works, it's meant to be a demo to showcase what the editor is capable of. Real League of Legends has super complex hero design & there are over a hundred heroes. You can do just a few & they can be relatively basic design. You can pick the overall game rules, how many minions per spawn, what spawn interval, how many max players, etc. I'd say keep it relatively simple & make sure it still feels like a MOBA. However, I'm sure DOTA & LoL use different minion wave logic, so it's not like there's a "correct" perfect formula you have to follow.

Have you played LoL or a similar game before? If not, maybe watch a YT video to see how it works.
[9:51 AM]Even though the base Realm game is essentially Age of Empires, the editor is supposed to allow modders to make any random game, like an arcade of maps with completely different rules. So, the demo is to show that you can invent a brand new genre inside the game.
[9:56 AM]I refactored the game data files, so the old .zip I sent won't work well with the latest code. Here's an update base version to start with.Zip