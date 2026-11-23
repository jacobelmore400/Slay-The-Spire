# Integration checklist — not yet a playable mod

The `Core/` code is game-independent logic, **not** a compiled/installable Slay the Spire 2 plugin. No game hooks have been guessed. Before shipping, inspect the exact installed `sts2.dll` and game UI scenes, then implement and test the following adapter.

1. **Mod loader**: create a game-targeted Godot/.NET 9 project, manifest, `[ModInitializer]`, and Harmony patches matching the installed game version.
2. **Save model**: persist `CharacterProgress` per character; load before entering a run. Keep the set of boss-completion IDs stable across reloads (run ID + Act + boss encounter ID). Use transactional saving to avoid duplicate rewards.
3. **Boss completion**: hook authoritative boss death/completion (not merely room entry) and call `TryAwardBoss` once, using Infinite Run's actual 1-based Act index when available. Reward 2 * Chapter. Do not implement Infinite Run itself.
4. **Starting merchant**: open exactly once after character select; set `current_coins = max_coins`; reuse native merchant card visual/hover/zoom/price layout, tint coin icon pink, show every eligible card in the selected character's pool with infinite stock, no discounts, and `floor(normal price/10)`. Use native commands to add cards to the run deck, not direct model mutation. Add top-left Shop/Upgrades bookmark navigation.
5. **Upgrade UI**: group Player, Rest Sites, Shop; show each level, exact next cost, and MAX at cap; purchases debit both balances and persist immediately. Disallow unaffordable purchases. Keep unlimited Max HP.
6. **Player effects**: apply HP at run initialization, draw per turn, maximum Energy, and Plating at each combat start. Respect native save/reload and multiplayer replication.
7. **Rest Sites**: +5 percentage points per healing level (base 30%, maximum 100%); use native Rest/Mend healing semantics. Smith bonus rolls once per Smith and chooses distinct eligible cards, excluding the manually upgraded card. Extra-action budget rolls once on entry, displays remaining actions, and never repeats a unique action.
8. **Regular shops**: +1 removal opportunity per level, capped at +10; discounts by category, 1% per level up to 100%, never applied to Pink Coin starting merchant.
9. **Testing**: reload mid-run, reload after a boss kill, multiplayer, Infinite Run Act 4+, starter cards/rarities, card hover, Smith with insufficient eligible targets, Miniature Tent, Rest/Mend interactions, discounted free purchases.

## Important implementation decisions

- Pink Coins earned mid-run increase **permanent** `max_coins` only; `current_coins` is not replenished.
- Card-pool membership, base card shop price, and game-specific rounding of discounted gold prices must be verified in game.
- Core currently models Rest healing percentage but does not directly manipulate HP. With healing capped at 100%, there is no upgrade-generated overheal; ordinary game healing semantics apply.
- A gameplay patch should use game-native commands for card additions, upgrades, healing, currency changes, and combat powers, preserving multiplayer and previews.
- The C# core's `TryBuyStartingCard` callback assumes an authoritative atomic grant; do not use it directly with asynchronous UI selection without a transaction.
- This repository intentionally has **no Infinite Run source**.

## Reference

- https://github.com/ColeG24/StsMod/blob/main/sts2-modding-reference.md
- https://github.com/fresh-milkshake/Modding-Tutorial
