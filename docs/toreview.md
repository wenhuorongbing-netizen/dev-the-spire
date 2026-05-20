# Spire Plus To Review

Current queue for user manual testing. The full pre-slim implementation history is archived at `docs/archive/feature-audits/toreview-pre-slim-20260518.md`.

Current test package: `publish/SpirePlus-v0.1.0-private-beta.0.zip`.

Current package hashes:

| Artifact | SHA256 |
| --- | --- |
| ZIP | `B19620D8D8A15D5B96208D3DE8C3B372BCA0874E076DD2DEBEDE09422FF28BD2` |
| DLL | `A1D86D01E57E0F58617ACA23EA8094B1AF35F525E3254007DE3675A1289B8159` |
| PCK | `073CAF976C91D9E6CEA39FA90FB5A6417E66CD5E12DED5EDD8169C892A0F0538` |
| Manifest | `659943569D01C1DDD8B5C351D763497F7FEE513AD0BB84903D05B69F8DBD1AB2` |
| README_INSTALL | `C9F19363848AEECD4B763BFF7BB2B75980A90BFE22358ACEC8FF5E9E5C129CE4` |

Latest no-game validation passed: Spire Plus build/test `202 passed / 18 skipped`, Future Peek build/test `8 passed`, format, diff-check, Spire Plus publish/package, Future Peek publish, artifact tests `220 passed / 0 skipped`, and local game-root package copy/hash check.

## Retest Queue

| ID | Area | What changed | Manual proof needed |
| --- | --- | --- | --- |
| URDA-ROOT-EYES | Urda | Root Eyes uses a visible relic counter, selectable map nodes, forked preview RNG, saved markers, entry-time commitment for Monster/Unknown/Elite, preview-record consumption after entry, duplicate-reservation avoidance, post-hook Unknown filtering, stale-preview cleanup/refund, and map-screen null guard. | Click relic, select multiple valid future nodes, verify hover title/icon, enter marked rooms in order and out of order, then save/load before entry. |
| URDA-SEED-BANK | Urda | Seed Bank stores cards on the relic, exposes stored card hover tips, supports relic-click extraction, preserves seeds if deck-add fails, and refreshes stale storage. | Store cards, hover relic, click extract, verify chosen cards enter deck, verify Boss transition does not hang. |
| URDA-TRIAL-HUMUS | Urda | Trial Branch, Seedbed, Humus Pact, Moss Map, Rooted Route, After Rain, Molting, and Shallow-Root Relic have source guards, visible option relics, and normal relic hover text. | Check event text, relic hover text, card reward alternatives, map markers, and combat results. |
| MORVI-REWARDS | Morvi | All Morvi choices now use visible option relics with relic hover text. Debt and Overdraft counters are Buff counters, Forbidden Loan hides when no card is eligible, and failed selection refreshes choices. | Test Forbidden Loan, Misprint Press, Red Ink, Overdue Library, Blueprint Proof, Paperstorm, Open Book, Debt Settlement. |
| LOTHA-REWARDS | Lotha | All Lotha choices now use visible option relics with relic hover text, source-split card rules, transient state recovery, and Mirror Rebuttal candidate gating. | Test each Lotha blessing in combat, especially Single Sentence, Death Reprieve, Public Evidence, and Mirror Rebuttal. |
| VAKUU-FIGHT | Vakuu | Vakuu fight has a visible fight relic, dedicated encounter scene/monster, no-normal-reward resume path, fallback exit path, and non-Vakuu victory relic choices. | Enable the gate, start the fight, win, confirm no black screen, choose reward, test failure/death and save/load. |
| MANUAL-20260519-COUNTERS-PEEK-TAGS | UI/tooltips | Vakuu combat powers use visible counter amounts. Claws is a pickup choice, not a lasting counter relic. Crystal Sphere preview is isolated in `EZFuturePeek`; Quiet Echo and Deferred Verdict wording stays in the manual text pass. | Inspect Vakuu power icons/counts, Claws pickup choice, `EZFuturePeek` Crystal Sphere preview, Quiet Echo, and Deferred Verdict wording in game. |
| ASCENSION-A11-A20 | Ascension | A11 map geometry, A12 Firemarked Elites, A13 Fission rewards, A16 Banners, A17 Deep Branch, A19 seals, A20 brand path, and Rootblight have current source guards. Fission keeps upgrade-only reward relic changes; Soul Tide counts Beckon before Core flushes the hand; Boiling Critical no longer promises equal Block on the shared tooltip. | Play A11-A20 paths, verify map markers/hovers, Fission reward display and pickup, Soul Fysh/Soul Tide timing, Waterfall Giant/Boiling Critical tooltip and timing, combat powers, Rootblight timing, save/load, and co-op boundaries. |
| MANUAL-20260519-SEEDBED | Urda | Seedbed text now explains the plant-bed capacity directly, option hover uses the card hover only, and nested Root/Rootblight/Status card hover spam is removed. | Check Seedbed option hover and card hover in Chinese/English. Confirm the tooltip is readable and no longer hides the choice list. |
| MANUAL-20260519-MOLTING | Urda | Molting now creates `Withered Husk` as an Ethereal + Exhaust curse card, with matching English/Chinese option and relic text. The card still gives 3 Block when played. | Pick Molting, inspect reward preview, draw/play the generated card, and confirm it behaves like a temporary curse-style buffer. |
| MANUAL-20260519-ACT-VALUES | Ascension | Firemark and Banner map hovers now fill descriptions with current-act values instead of `Act 1/2/3` slash tables. | Check A12 and A16 map hovers in Act 1, Act 2, and Act 3. Confirm each hover only shows the current act's numbers. |
| MANUAL-20260519-BANNER-FIREMARK | Ascension | Shieldwall now grants ally protection after the enemy turn, its bannerbearer status shows the current Block amount, and Forge Armor tracks only generated armor so unrelated Block no longer blocks the shatter skip. Molten Armor tooltip explains the skip window. | Test Shieldwall in multi-enemy fights and Forge Armor firemark elites. Confirm protection persists into the useful turn and shattering generated armor skips the next armor gain. |
| ANCIENT-UI-ART | UI/art | Ancient clicked screens, option relic icons, map/run-history icons, card/power/relic art paths, and package resources are guarded. | Capture clicked Ancient UI and hover screenshots for Urda, Morvi, Lotha, and gated Vakuu. |
| GOV-CI-FULL-LANE | Engineering | Added `.github/workflows/full-local-validation.yml` and `scripts/ci-full-validation.ps1` for self-hosted Windows full no-game validation with explicit `STS2_PATH` and `GODOT_PATH`. | Run the workflow on the self-hosted runner once and attach the run log before treating CI as full validation evidence. |

Do not close these rows from source review alone. Close only after the matching live manual proof exists.
