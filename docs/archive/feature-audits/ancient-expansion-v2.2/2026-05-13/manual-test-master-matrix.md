# Manual Test Master Matrix

Reviewed source baseline: `a2183ee`. Existing automated/source guards are useful but do not close live gameplay rows. Status values here are planning status, not pass claims.

| Tier | Test row | Current status | Evidence required | Notes |
| --- | --- | --- | --- | --- |
| 0 Environment | Clean BaseLib + EZMB only | Existing controlled smoke evidence, refresh required | Installed mod list, clean main-menu load, `godot.log` | Re-run after final package refresh. |
| 0 Environment | Installed hash check | Existing handoff hashes, refresh required after packaging | DLL/manifest/PCK/zip SHA256 | Current docs-only audit does not publish. |
| 0 Environment | Clean log audit | Existing logs partial; refresh required | `audit-godot-log.ps1` output or manual grep | Include Windows and Mac path variants. |
| 0 Environment | SavedSpireField count | Source says 16; older 13-field smoke stale | Log/source evidence matching current source | Must account for MorviStateKey plus Urda/Morvi deck mirror keys. |
| 0 Environment | Windows/macOS command variants | Windows documented; Mac not live-proven | Tester commands and results | See platform matrix. |
| 1 Single-player smoke | A0 first combat | Existing smoke evidence in handoff | Clean log, first combat entry/exit | Re-run after final artifact refresh. |
| 1 Single-player smoke | A10 first combat | Existing smoke evidence in handoff | Clean log, first combat entry/exit | Re-run after final artifact refresh. |
| 1 Single-player smoke | A20 first combat | Existing smoke evidence in handoff | Clean log, selector/first combat evidence | Does not prove full A20 boss flow. |
| 1 Single-player smoke | Mod Settings | Existing normal Steam evidence | Screenshot/log note | Re-run if package changes. |
| 1 Single-player smoke | No errors | Partial | Clean `godot.log` | Must include BaseLib + EZMB only. |
| 2 Active Urda | Seedbed | Pending | Select Urda Seedbed, accept 1-4 alternatives, verify max HP, deck, upgraded first Seedling, no reroll exploit | Include save/load during reward screen. |
| 2 Active Urda | Humus Pact | Pending | Three composts, gold gains, removal selector, upgraded payoff reward, pending clears once | Include resolver cancel/fail and save/load. |
| 2 Active Urda | Molting / Withered Husk | Pending | Starter removal, two Husks, exhaust block, Act 2 cleanup | Include deck save/load before Act 2. |
| 2 Active Urda | Moss Map | Pending | First Act 1 Monster/Event/Shop/Elite/Rest rewards exactly once | Include save/reload before and after room entry. |
| 2 Active Urda | Disable Urda gate | Pending | `EZMB_DISABLE_URDA=1` hides Urda; no hooks fire | Compare Act 1 Ancient offers. |
| 3 Ancient reward rebalance | Velvet Choker | Pending | Soft-limit/cost behavior, x-cost behavior, reset timing | Include save/load if card/relic state involved. |
| 3 Ancient reward rebalance | Distinguished Cape | Pending | Event option/pickup behavior | Verify current text/UI. |
| 3 Ancient reward rebalance | Prismatic Gem | Pending | Normal reward counter, preview/hint, reward behavior | Reward screen save/load required. |
| 3 Ancient reward rebalance | Quality Flame | Pending | Exhaust/draw behavior | Include combat log/visual. |
| 3 Ancient reward rebalance | Pumpkin Candle vanilla | Pending | No unintended change | Regression row. |
| 3 Ancient reward rebalance | Save/load rows | Pending | Save/load sensitive Ancient reward cases | Do not claim release-ready without closure/disposition. |
| 4 Ascension | A11 natural traversal | Pending | Route-click first node, natural Act 1/2/3 traversal, boss reachability | Existing spot checks are not full proof. |
| 4 Ascension | A12 Firemark variety/preview | Pending | Map hover/preview, varied first marks across seeds, combat effect, reward | Use diagnostics. |
| 4 Ascension | A13 Fission diagnostics | Pending | Diagnostics show chance/source/eligible/roll/applied; player sees applied enchantment when true | Explain no-show cases. |
| 4 Ascension | A14/A15/A18 Rootblight/Blight Sprout | Pending | Starter/combat-end notices, art hover, deck counts, play/downgrade/purge | Include save/load. |
| 4 Ascension | A16 Banner variety/preview | Pending | Map hover/preview, varied banner types, combat effects, rewards | Use diagnostics. |
| 4 Ascension | A19 Boss Seal preview | Pending | Boss hover, combat notice, each high-risk boss behavior | Include Knowledge Demon, Kaiser Crab, Test Subject. |
| 4 Ascension | A20 second boss/intermission/brand | Pending | Boss1 reward/intermission, second boss transition, Brand metadata/effect, clean log | Single-player first; multiplayer downgraded/unverified. |
| 5 Multiplayer | A10 control | Pending | Two-client Steam host/client logs, route/combat/save quit | Baseline before A11-A20. |
| 5 Multiplayer | A11 default | Pending | Selection propagation, route traversal, save/quit, no black screen | Capture both logs. |
| 5 Multiplayer | A12 | Pending | Firemark map/combat/reward consistency | Host/client map metadata must match. |
| 5 Multiplayer | A14 | Pending | Rootblight/Blight Sprout owner/deck consistency | Host/client deck and logs. |
| 5 Multiplayer | A16 | Pending | Banner map/combat/reward consistency | Host/client map metadata must match. |
| 5 Multiplayer | A20 | Pending | Warning/downgrade behavior, no unsupported second-boss co-op claim | Do not claim full co-op A20. |
| 5 Multiplayer | Save/quit | Pending | Host/client save and resume, no state loss/desync | Include Urda/Rootblight if supported. |
| 5 Multiplayer | Host/client logs | Pending | ModelDb hash, mod list, mismatch diagnostics when applicable | Required for "version differs" reports. |
| 5 Multiplayer | Ownership/desync | Pending | Gold/deck/HP/reward changes apply once to correct player | Highest risk for Urda. |
| 6 Future v2.2 | Morvi not active by default | Source-backed, live spot pending | Default run shows no Morvi unless env enabled | Do not promote. |
| 6 Future v2.2 | Lotha not active | Source-backed | No Lotha offer/source/assets in default run | Planning only. |
| 6 Future v2.2 | Vakuu Fight not active | Source-backed | Existing Vakuu reward patches only; no fight | Planning only. |
| 6 Future v2.2 | Source API blockers documented | This audit created blocker docs | Read audit package before implementation | Required before future milestone. |

## Release Gate Summary

Private beta release cannot be claimed until Tier 0-5 either pass or have an explicit release-note disposition approved. Future v2.2 Tier 6 items must remain inactive by default.
