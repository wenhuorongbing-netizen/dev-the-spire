# Release Traceability Matrix

This matrix maps player-visible promises to source, guard, and evidence state. It intentionally keeps live rows open.

| Area | Player promise | Source evidence | Guard evidence | Manual/live state | Release stance |
| --- | --- | --- | --- | --- | --- |
| Package | Installable `Spire Plus` zip contains `EZMicroBalance` manifest, DLL, PCK, and install notes. | `EZMicroBalance.json`, `publish/package-staging/EZMicroBalance/` | Release artifact tests, hash docs | Package parity and beta.123 clicked UI smoke exist; gameplay proof pending | Manual-test package only |
| Ancient reward rebalance | Core Ancient rewards have revised tradeoffs and visible text. | `EZMicroBalanceCode/Ancients/Patches/` | `AncientBehaviorGuardTests`, release coverage guards | Reward gameplay pending | Manual-test candidate |
| Urda | Eleven blessings, marker relics, Root Eyes, Seed Bank, Trial Branch, Rooted Route, Seedbed. | `EZMicroBalanceCode/Ancients/Expansion/Urda/` | Urda and Ancient expansion guards | Beta.123 forced clicked UI smoke exists; Root Eyes, Seed Bank, hover/readability, save/load, co-op pending | Manual-test candidate |
| Morvi | Eight blessings with debt, misprint, Red Ink, Open Book, Blueprint Proof, Overdue Library. | `EZMicroBalanceCode/Ancients/Expansion/Morvi/` | Morvi v2.2 guards | Live gameplay, freeze reports, save/load, co-op pending | Manual-test candidate |
| Lotha | Eight blessings with verdict, mirror, public evidence, death reprieve. | `EZMicroBalanceCode/Ancients/Expansion/Lotha/` | Lotha and save-risk guards | Lethal path, save/load, co-op pending | Manual-test candidate |
| Vakuu normal Ancient | Standard Vakuu option behavior remains available. | `EZMicroBalanceCode/Ancients/Patches/`, `VakuuFightPatch.cs` | Release and Vakuu guards | Beta.123 normal clicked UI smoke exists; gated fight-option UI pending | Manual-test candidate |
| Vakuu fight | Gated fight uses a dedicated enemy/scene and returns after victory. | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/` | Vakuu source guards | Victory/no-black-screen, failure/death, save/load, co-op pending | Hidden by default |
| A11-A20 | Higher Ascension levels add map, combat, reward, boss, and root systems. | `EZMicroBalanceCode/Ascension/` | Ascension guards, A11 geometry tests | Natural traversal, combat, save/load, co-op pending | Development-test surface |
| Rootblight | Rootblight/Sprout state is capped and source-hardened. | `EZMicroBalanceCode/Ascension/Cards/`, `Ascension/Combat/` | Rootdeck and release guards | Visual, combat-end, save/load, co-op pending | Manual-test candidate |
| Preview tools | Crystal Sphere peek and transform preview are preview-only helpers inside Spire Plus. | `EZMicroBalanceCode/Preview/` | `PreviewToolsGuardTests` | Crystal Sphere and transform live proof pending | Integrated into Spire Plus |
| Website | Public claims match implemented and proven features without implying release readiness. | `website/`, `.github/workflows/spire-plus-site.yml`, archived `.tools/archive/local-website-preview-20260516/` for historical comparison | Website claim audit and governance guards | Public-info source active; live game proof still pending | May describe manual-test package only |
| Co-op | Multiplayer support is safe where advertised. | `StartRunLobby`, `CombatStateSynchronizer`, mod selection diagnostics | Source/diagnostic guards | Two-client proof pending | Do not advertise full support |

## Closure Rule

An area can move from manual-test candidate to release candidate only when this matrix has source evidence, guard evidence, live proof, and release evidence for the same player promise.
