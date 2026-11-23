# Pink Coin Progression — Slay the Spire 2

A companion mod for an existing Infinite Run mod. **This repository does not implement Infinite Run.**

## Status

**Core progression implementation (C#) committed; game integration is not yet implemented or tested.** This is not a playable mod release. See `PinkCoinProgression/INTEGRATION.md` for the required hooks.

## Rules

- On each uniquely completed Act boss, earn `2 * (floor((act-1)/3)+1)` permanent Pink Coins.
- Per character, `max_coins` is a persistent balance and `current_coins` resets to `max_coins` when the starting merchant opens.
- Starting card purchases deduct only `current_coins`, using `floor(normal card price / 10)`; discounts never apply to this merchant. All eligible character cards are infinitely stocked.
- Permanent upgrades deduct the same cost from both balances, and persist between runs.
- Upgrade prices use `floor(base * multiplier^purchases + 0.5)` for positive values, computed with decimal arithmetic.
- Upgrade caps and formulas are defined in source.

See the integration guide for the Godot merchant UI, run/save lifecycle, command-based card rewards, Rest Site and regular Shop patches, and multiplayer requirements.
