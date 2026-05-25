# Spire Plus To Review
Current queue for user manual testing. Full pre-slim implementation history is archived at `docs/archive/feature-audits/toreview-pre-slim-20260518.md`.
Current test package: `publish/SpirePlus-v0.1.0-private-beta.0.zip`.

Current package hashes:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `2EAC08531559C7871497741F5827705A3B9DB0EC60AF69A1C485AB6F9B4A3006` |
| DLL | `C8A8862AB427CD49BB77E7D90CB299A07276B768F5DB6C34BBAFB363F06DD6F1` |
| PCK | `30312BE1E2723A1F7C1A617CAD45F9E2313C567EFE391378424B39CA330039A2` |
| Manifest | `C2FB53C13AE099080AC71FF7EE2A1F217A2586549A9152DAFE0EBF512EF42FF6` |
| README_INSTALL | `33263ACDEEE8F46DD89FFCF649A259B190805C992F743BC3DC07F716FD212FAA` |

Latest no-game validation snapshot: build passed, default tests `273 passed / 20 skipped`, opt-in artifact tests `293 passed / 0 skipped`, website syntax checks passed, format passed, installed-package check passed, and `git diff --check` passed. After the Elite Root source-safety refresh, the current-package loader row is pending again because the ZIP/DLL hash changed without opening the game. The release verifier fails closed with 19 pending live/manual rows.

## Retest Queue
| ID | Area | What changed | Manual proof needed |
| --- | --- | --- | --- |
| URDA-ROOT-EYES | Urda | Root Eyes selects future Monster/Unknown/Elite nodes, stores concrete previews, shows stacked marker hovers, and gates co-op queue mutation. | Click the relic, select shared-marker nodes, enter marked rooms in order and out of order, then save/load before entry. |
| URDA-SEED-BANK | Urda | Seed Bank stores cards on the relic, exposes hover tips, supports relic-click extraction, and preserves seeds if deck-add fails. | Store cards, hover the relic, click extract, verify cards enter deck, and verify Boss transition does not hang. |
| URDA-TRIAL-HUMUS | Urda | Trial Branch, Seedbed, Humus Pact, Moss Map, Rooted Route, After Rain, Molting, Shallow-Root Relic, and Elite Root have source guards and visible relic hovers. | Check event text, relic hover text, card reward alternatives, map markers, and combat results. |
| URDA-ELITE-ROOT | Urda | New first-tier Urda option relic: after each Elite combat, heal 10 HP. | Pick Elite Root, fight a normal Elite and a Firemarked Elite if possible, and confirm each victory heals up to 10 HP. |
| MORVI-REWARDS | Morvi | Morvi choices use visible option relics; Debt and Overdraft counters are Buff counters; failed selections refresh choices. | Test Forbidden Loan, Misprint Press, Red Ink, Overdue Library, Blueprint Proof, Paperstorm, Open Book, Debt Settlement. |
| LOTHA-REWARDS | Lotha | Lotha choices use visible option relics with source-split card rules, transient state recovery, and Mirror Rebuttal handling. | Test each blessing, especially Single Sentence, Death Reprieve, Public Evidence, and Mirror Rebuttal. |
| VAKUU-FIGHT | Vakuu | Vakuu fight has a visible fight relic, dedicated encounter scene/monster, no-normal-reward resume path, and fallback exit path. | Enable the gate, start the fight, win, confirm no black screen, choose reward, test failure/death and save/load. |
| MANUAL-20260519-COUNTERS-PEEK-TAGS | UI/tooltips | Vakuu combat counters are visible. Vakuu's Sere Talon / 瓦库原初之爪 offers 4 Curses, then adds the chosen Curse, 2 Wish, and 1 Wish+; Tanx Claws / 坦克斯利爪 transforms cards into Maul+ / 撕咬+. | Inspect Vakuu's Sere Talon pickup, Tanx Claws pickup, relic-bar art, Crystal Sphere preview, Quiet Echo, and Deferred Verdict text. |
| MANUAL-20260524-SERE-TALON-ART | Ancient rewards / relic art | Sere Talon routes to Spire Plus-owned icon art and no longer uses Tanx Claws art. | Pick up Vakuu's Sere Talon and verify event option, relic bar, inspect screen, hover title/text, and log routes. |
| MANUAL-20260524-SERE-TALON-TANX-CLAWS-REPORT | Ancient rewards / relic art | Treat any green Tanx Claws art on Vakuu's Sere Talon as our display/package route problem until live proof says otherwise. | If the effect is curse choice + 2 Wish + 1 Wish+ but art/title is Tanx Claws, capture `godot.log` route lines and the surface that bypassed the patch. |
| ASCENSION-A11-A20 | Ascension | A11 map geometry, A12 Firemarked Elites, A13 Fission, A16 Banners, A17 Deep Branch, A19 dedicated abilities, A20 Branded Form, and Rootblight have guards. | Play A11-A20 paths, verify hovers, rewards, boss abilities, Rootblight timing, save/load, and co-op boundaries. |
| MANUAL-20260519-SEEDBED | Urda | Seedbed catches later Blight Sprouts and temporary Status/Curse cards, skips Rootblight and Withered Husk, and grants a Husk per planted card. | Check Seedbed option/card hover in EN/ZHS and combat planting behavior. |
| MANUAL-20260523-SEEDBED-REWARD-REENTRY | Urda | Seedbed reward alternatives have a per-reward handled guard. | Choose the Seedbed alternative once, then try rapid repeat clicks and save/load around the reward screen. |
| MANUAL-20260519-MOLTING | Urda | Molting creates `Withered Husk` as an Ethereal + Exhaust curse card; exhausted Husk gives 3 Block. | Pick Molting, inspect the preview, draw/play or let the card expire, and confirm timing. |
| MANUAL-20260519-ACT-VALUES | Ascension | Firemark and Banner map hovers fill descriptions with current-act values instead of slash tables. | Check A12 and A16 map hovers in Acts 1, 2, and 3. |
| MANUAL-20260519-BANNER-FIREMARK | Ascension | Shieldwall grants protection after enemy turns; Firemarked Elites use one host plus one-target overflow; Forge Armor starts on player turns. | Test multi-enemy Firemarked Elites and verify overflow, host death, act values, heal threshold, and Forge Armor skip timing. |
| MANUAL-20260522-V33-DESIGN | Ancient rewards | v3.3 source pass: Vakuu contract choices/Cash Out/Blood Debt settlement, Closed Court split resource turns, Mirror Rebuttal, After Rain, Seedbed, and shorter Lotha text. | Test each changed choice in EN/ZHS; for Vakuu verify Cash Out, three-lock win, reward choices, Blood Debt settlement, and no black screen. |
| MANUAL-20260522-BOSS-SEALS | Ascension | A19/A20 use v4.1 boss-specific mechanics: Martyr Oath, Ink Return, Plating Wake, Soul Tide, Unweakenable, Claw Calibration, Marginal Note, Escape Fatigue, Time Sand Reflow, Royal Decree, and Experimental Record. | Test every A19/A20 boss, including intent changes, notices, save/load, and no black screen. |
| MANUAL-20260523-WEBSITE-A19A20 | Website | Public site Ascension data and copied localization use v4.1 boss dedicated abilities / Branded Form and current package metadata. | Render the website in Chinese and English and inspect A19/A20 cards plus zip metadata. |
| MANUAL-20260522-ROOT-EYES-CONFLICTS | Urda / multiplayer | Root Eyes can share nodes with Firemark, Banner, and Deep Branch markers via hover stacks and a small badge. | Select future nodes near other markers; confirm previews and original marker text both remain readable. |
| MANUAL-20260522-PREVIEW-TOOLS | Preview tools | Crystal Sphere peek remains UI-mask-only; transform preview uses a forked transformation RNG snapshot. | Test Crystal Sphere peek on/off, transform preview, Prismatic Gem with reward modifiers, and multiplayer warning/gate paths. |
| MANUAL-20260522-LOCALIZATION-QA | Localization | EN/ZHS localization has matching keys, rich-text tags, dynamic vars, and cleaned wording. | Check Ancient choices, relic hovers, Ascension hovers, card/status text, and map/combat markers in both languages. |
| MANUAL-20260522-SEAL-INDICATORS | Ascension / UI | Dedicated ability and Branded Form effects have readable hovers and visible counters. | Start A19/A20 fights and check every boss power row. |
| MANUAL-20260522-DEDICATED-ABILITY-REROLL | Ascension / Ancient UI | Boss descriptions spell out concrete values; first-layer Ancient reward screens have a dice-style one-use reroll. | Check boss hovers and click the Ancient reroll once; confirm it cannot be used twice. |
| MANUAL-20260520-EVIDENCE-LOG | Evidence | `ReleaseEvidenceLog` emits opt-in markers for high-risk runtime surfaces. | Launch with `SPIREPLUS_RELEASE_EVIDENCE_LOG=1`, exercise paths, and attach `godot.log` snippets. |
| ANCIENT-UI-ART | UI/art | Ancient clicked screens, option relic icons, map/run-history icons, and package resources are guarded. | Capture clicked Ancient UI and hover screenshots for Urda, Morvi, Lotha, and gated Vakuu. |
| GOV-CI-FULL-LANE | Engineering | Added self-hosted Windows full no-game validation workflow and script. | Run it once and attach the workflow log before treating CI as full validation evidence. |

Do not close these rows from source review alone. Close only after matching live manual proof exists.

