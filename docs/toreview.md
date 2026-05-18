# Spire Plus To Review

Current queue for user manual testing. The full pre-slim implementation history is archived at `docs/archive/feature-audits/toreview-pre-slim-20260518.md`.

Current test package: `publish/SpirePlus-v0.1.0-private-beta.0.zip`.

Current package hashes:

| Artifact | SHA256 |
| --- | --- |
| ZIP | `5AEE65325C4248E8BFB86268E360E24BB68B428B2CBC6CDA96F8F86DA483228A` |
| DLL | `54524355D5F6986A017E06E7F3EC996BF5B4F8A23870B518E40D38AB91EF1096` |
| PCK | `9547FD17CEAC9719A3BA044A9E47D65A7F3F942C248559136D380EFD75AB2B86` |
| Manifest | `659943569D01C1DDD8B5C351D763497F7FEE513AD0BB84903D05B69F8DBD1AB2` |
| README_INSTALL | `C9F19363848AEECD4B763BFF7BB2B75980A90BFE22358ACEC8FF5E9E5C129CE4` |

Latest no-game validation passed: build, focused guards, full tests `187 passed / 18 skipped`, format, diff-check, publish, package rebuild, artifact tests `205 passed / 0 skipped`, and installed package hash check.

## Retest Queue

| ID | Area | What changed | Manual proof needed |
| --- | --- | --- | --- |
| URDA-ROOT-EYES | Urda | Root Eyes uses a visible relic counter, selectable map nodes, forked preview RNG, saved markers, entry-time commitment for Monster/Unknown/Elite, preview-record consumption after entry, duplicate-reservation avoidance, post-hook Unknown filtering, stale-preview cleanup/refund, and map-screen null guard. | Click relic, select multiple valid future nodes, verify hover title/icon, enter marked rooms in order and out of order, then save/load before entry. |
| URDA-SEED-BANK | Urda | Seed Bank stores cards on the relic, exposes stored card hover tips, supports relic-click extraction, preserves seeds if deck-add fails, and refreshes stale storage. | Store cards, hover relic, click extract, verify chosen cards enter deck, verify Boss transition does not hang. |
| URDA-TRIAL-HUMUS | Urda | Trial Branch, Seedbed, Humus Pact, Moss Map, Rooted Route, After Rain, Molting, and Shallow-Root Relic have source guards, visible option relics, and normal relic hover text. | Check event text, relic hover text, card reward alternatives, map markers, and combat results. |
| MORVI-REWARDS | Morvi | All Morvi choices now use visible option relics with relic hover text. Debt and Overdraft counters are Buff counters, Forbidden Loan hides when no card is eligible, and failed selection refreshes choices. | Test Forbidden Loan, Misprint Press, Red Ink, Overdue Library, Blueprint Proof, Paperstorm, Open Book, Debt Settlement. |
| LOTHA-REWARDS | Lotha | All Lotha choices now use visible option relics with relic hover text, source-split card rules, transient state recovery, and Mirror Rebuttal candidate gating. | Test each Lotha blessing in combat, especially Single Sentence, Death Reprieve, Public Evidence, and Mirror Rebuttal. |
| VAKUU-FIGHT | Vakuu | Vakuu fight has a visible fight relic, dedicated encounter scene/monster, no-normal-reward resume path, fallback exit path, and non-Vakuu victory relic choices. | Enable the gate, start the fight, win, confirm no black screen, choose reward, test failure/death and save/load. |
| ASCENSION-A11-A20 | Ascension | A11 map geometry, A12 Firemarked Elites, A13 Fission rewards, A16 Banners, A17 Deep Branch, A19 seals, A20 brand path, and Rootblight have current source guards. Fission keeps upgrade-only reward relic changes; Soul Tide counts Beckon before Core flushes the hand; Boiling Critical no longer promises equal Block on the shared tooltip. | Play A11-A20 paths, verify map markers/hovers, Fission reward display and pickup, Soul Fysh/Soul Tide timing, Waterfall Giant/Boiling Critical tooltip and timing, combat powers, Rootblight timing, save/load, and co-op boundaries. |
| ANCIENT-UI-ART | UI/art | Ancient clicked screens, option relic icons, map/run-history icons, card/power/relic art paths, and package resources are guarded. | Capture clicked Ancient UI and hover screenshots for Urda, Morvi, Lotha, and gated Vakuu. |

Do not close these rows from source review alone. Close only after the matching live manual proof exists.
