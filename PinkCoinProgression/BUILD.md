# Build and integration status

This is a **loader scaffold**, not a playable mod. The loader entry point is based on community references and has not been verified against your installed game version.

## Prerequisites

- .NET SDK 9
- Your legitimately installed game's `sts2.dll` (do not commit or redistribute it)
- Current game-version decompilation or API reference

## Build

From `PinkCoinProgression/`, use:

```powershell
dotnet build PinkCoinProgression.Game.csproj -c Release -p:Sts2AssemblyPath="C:\path\to\sts2.dll"
```

This intentionally fails without the real game assembly. The earlier `PinkCoinProgression.csproj` remains a game-independent core project.

For local installation after compiling, place the compiled DLL and `PinkCoinProgression.json` together in a mod folder according to the current loader's conventions. No installable release is claimed yet.

## Before gameplay implementation

Inspect your installed game version and verify the mod loader entrypoint, native merchant scene, character selection transition, boss completion event, save lifecycle, Rest Site actions, card grant commands, regular Shop pricing and multiplayer authority. The actual gameplay patches and custom shop UI are **not** yet written.

## References

- https://slaythespire.wiki.gg/wiki/Modding_Tutorials
- https://github.com/Alchyr/ModTemplate-StS2
- https://github.com/fresh-milkshake/Modding-Tutorial
- https://github.com/ColeG24/StsMod/blob/main/sts2-modding-reference.md
