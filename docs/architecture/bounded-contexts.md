# Bounded Contexts

This is the release-candidate boundary map for the single `Spire Plus / EZMicroBalance` mod.

| Context | Responsibility | Public surface | Internal details | Dependencies | Risk |
| --- | --- | --- | --- | --- | --- |
| AncientRewardRebalance | Vanilla Ancient reward changes and player text. | Relic/card behavior, reward alternatives, localization. | `EZMicroBalanceCode/Ancients/Patches/`. | Reward, relic, card, save APIs. | Medium-high; reward reentry and save/load. |
| AncientExpansionUrda | Urda blessings, Root Eyes, Seed Bank, Seedbed, Rooted Route. | Ancient options, marker relics, map preview, deck changes. | `Ancients/Expansion/Urda/`. | Run map, card reward, event, relic inventory UI. | High; map, reward, save, co-op. |
| AncientExpansionMorvi | Morvi blessings and debt/card state. | Ancient options, marker relics, powers/cards. | `Ancients/Expansion/Morvi/`. | Combat hooks, card piles, player state. | High; freeze reports and save/load. |
| AncientExpansionLotha | Lotha blessings and Death Reprieve lifecycle. | Ancient options, marker relics, powers. | `Ancients/Expansion/Lotha/`. | Damage/death hooks, card cost hooks, deck mirror. | High; lethal path and save/load. |
| AncientExpansionVakuu | Hidden fight, dedicated enemy/scene, post-fight choices. | Env-gated fight option and combat. | `Ancients/Expansion/Vakuu/`. | Event room, combat room, reward flow. | Very high; black screen, death, save, co-op. |
| AscensionCore | A11-A20 selection, map markers, combat modifiers, boss seals. | Ascension UI, map hover, combat powers, rewards. | `EZMicroBalanceCode/Ascension/`. | Lobby, map, combat, reward, boss flow. | High; map traversal and co-op. |
| RootDeck | Rootblight/Sprout cards and combat-end state. | Player deck/status cards and notices. | `Ascension/Cards`, `Ascension/Combat`. | Card model, piles, combat lifecycle. | High; save/load and combat-end behavior. |
| PreviewTools | Preview-only helpers inside Spire Plus. | Crystal Sphere peek and transform preview. | `EZMicroBalanceCode/Preview/`. | UI nodes, transformation RNG. | Medium; RNG purity and live result match. |
| ReleaseEvidence | Package, hash, CI, live/manual proof governance. | Docs, scripts, tests, workflows. | `docs/`, `scripts/`, `.github/`, tests. | Filesystem, GitHub runner, local game paths. | High for release decisions. |

## Dependency Direction

- Patches should be thin adapters into feature services.
- Feature services should own gameplay decisions and avoid direct Godot node dependencies.
- UI code should present state and accept user intent; it should not mutate gameplay without a service boundary.
- Saved state should be explicit through existing save/deck/run fields when behavior must survive reload.
- Preview tools must stay isolated under `EZMicroBalanceCode/Preview/` and must not mutate rewards, reveal cells, create real cards, or advance committed RNG.

## Current Priority

The highest-value next refactors are behavior seams for Root Eyes, Banner/Firemark, and Vakuu flow. These are the places where source tests can move away from snippet-only assertions toward deterministic policy tests.
