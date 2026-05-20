# Spire Plus To Review

Current queue for user manual testing. The full pre-slim implementation history is archived at `docs/archive/feature-audits/toreview-pre-slim-20260518.md`.

Current test package: `publish/SpirePlus-v0.1.0-private-beta.0.zip`.

Current package hashes:

| Artifact | SHA256 |
| --- | --- |
| ZIP | `921EEB0468E9D9110405C4AADD958A64E6DED5D648A6DA8346CFD6B2A9956F91` |
| DLL | `CCF61C5E9B69C53C9DFF5E61F0C87913B54A8D1486F97000CBD6125D90903DE4` |
| PCK | `3CDB72F1225FF2492F536091772979983653865F2902E2B485BBCB16B4FD1392` |
| Manifest | `A41EBF8ABEDCFC09DBB02CB655D7E50465888065ABA77F8EF087E87206F276CF` |
| README_INSTALL | `BA885193452EBA22A78433304F383A87A0830FA5E935A20B63BBAA08ABEBB906` |

Latest no-game validation passed: Spire Plus build/test `232 passed / 18 skipped`, format, diff-check, Spire Plus publish/package, artifact tests `250 passed / 0 skipped`, and local game-root package copy/hash check.

## Retest Queue

| ID | Area | What changed | Manual proof needed |
| --- | --- | --- | --- |
| URDA-ROOT-EYES | Urda | Root Eyes uses a visible relic counter, selectable map nodes, forked preview RNG, saved markers, entry-time commitment for Monster/Unknown/Elite, preview-record consumption after entry, duplicate-reservation avoidance, post-hook Unknown filtering, stale-preview cleanup/refund, and map-screen null guard. | Click relic, select multiple valid future nodes, verify hover title/icon, enter marked rooms in order and out of order, then save/load before entry. |
| URDA-SEED-BANK | Urda | Seed Bank stores cards on the relic, exposes stored card hover tips, supports relic-click extraction, preserves seeds if deck-add fails, and refreshes stale storage. | Store cards, hover relic, click extract, verify chosen cards enter deck, verify Boss transition does not hang. |
| URDA-TRIAL-HUMUS | Urda | Trial Branch, Seedbed, Humus Pact, Moss Map, Rooted Route, After Rain, Molting, and Shallow-Root Relic have source guards, visible option relics, and normal relic hover text. | Check event text, relic hover text, card reward alternatives, map markers, and combat results. |
| MORVI-REWARDS | Morvi | All Morvi choices now use visible option relics with relic hover text. Debt and Overdraft counters are Buff counters, Forbidden Loan hides when no card is eligible, and failed selection refreshes choices. | Test Forbidden Loan, Misprint Press, Red Ink, Overdue Library, Blueprint Proof, Paperstorm, Open Book, Debt Settlement. |
| LOTHA-REWARDS | Lotha | All Lotha choices now use visible option relics with relic hover text, source-split card rules, transient state recovery, and Mirror Rebuttal candidate gating. | Test each Lotha blessing in combat, especially Single Sentence, Death Reprieve, Public Evidence, and Mirror Rebuttal. |
| VAKUU-FIGHT | Vakuu | Vakuu fight has a visible fight relic, dedicated encounter scene/monster, no-normal-reward resume path, fallback exit path, and non-Vakuu victory relic choices. | Enable the gate, start the fight, win, confirm no black screen, choose reward, test failure/death and save/load. |
| MANUAL-20260519-COUNTERS-PEEK-TAGS | UI/tooltips | Vakuu combat powers use visible counter amounts. Claws is a pickup choice, not a lasting counter relic. Crystal Sphere preview is now inside Spire Plus; Quiet Echo and Deferred Verdict wording stays in the manual text pass. | Inspect Vakuu power icons/counts, Claws pickup choice, Spire Plus Crystal Sphere preview, Quiet Echo, and Deferred Verdict wording in game. |
| ASCENSION-A11-A20 | Ascension | A11 map geometry, A12 Firemarked Elites, A13 Fission rewards, A16 Banners, A17 Deep Branch, A19 seals, A20 brand path, and Rootblight have current source guards. Fission keeps upgrade-only reward relic changes; Soul Tide counts Beckon before Core flushes the hand; Boiling Critical no longer promises equal Block on the shared tooltip. | Play A11-A20 paths, verify map markers/hovers, Fission reward display and pickup, Soul Fysh/Soul Tide timing, Waterfall Giant/Boiling Critical tooltip and timing, combat powers, Rootblight timing, save/load, and co-op boundaries. |
| MANUAL-20260519-SEEDBED | Urda | Seedbed text now explains the plant-bed capacity directly, option hover uses the card hover only, and nested Root/Rootblight/Status card hover spam is removed. | Check Seedbed option hover and card hover in Chinese/English. Confirm the tooltip is readable and no longer hides the choice list. |
| MANUAL-20260519-MOLTING | Urda | Molting now creates `Withered Husk` as an Ethereal + Exhaust curse card, with matching English/Chinese option and relic text. The card still gives 3 Block when played. | Pick Molting, inspect reward preview, draw/play the generated card, and confirm it behaves like a temporary curse-style buffer. |
| MANUAL-20260519-ACT-VALUES | Ascension | Firemark and Banner map hovers now fill descriptions with current-act values instead of `Act 1/2/3` slash tables. | Check A12 and A16 map hovers in Act 1, Act 2, and Act 3. Confirm each hover only shows the current act's numbers. |
| MANUAL-20260519-BANNER-FIREMARK | Ascension | Shieldwall now grants ally protection after the enemy turn, Firemark and banner powers show their current amount when Core supports a counter, and Forge Armor tracks generated Molten Armor so unrelated Block no longer hides the shatter skip. Molten Armor tooltip explains the skip window. | Test Shieldwall in multi-enemy fights and Forge Armor firemark elites. Confirm protection persists into the useful turn, visible numbers match the current Act, and shattering generated armor skips the next armor gain. |
| MANUAL-20260520-EVIDENCE-LOG | Evidence | `ReleaseEvidenceLog` now emits opt-in markers for the highest-risk runtime surfaces: Vakuu fight return paths, Root Eyes, Seed Bank, Rootblight, preview tools, A20 map/combat paths, and co-op gates. | Launch with `EZMB_RELEASE_EVIDENCE_LOG=1`, exercise those paths, and attach the resulting `godot.log` snippets before closing live proof rows. |
| ANCIENT-UI-ART | UI/art | Ancient clicked screens, option relic icons, map/run-history icons, card/power/relic art paths, and package resources are guarded. | Capture clicked Ancient UI and hover screenshots for Urda, Morvi, Lotha, and gated Vakuu. |
| GOV-CI-FULL-LANE | Engineering | Added `.github/workflows/full-local-validation.yml` and `scripts/ci-full-validation.ps1` for self-hosted Windows full no-game validation with explicit `STS2_PATH` and `GODOT_PATH`. | Run the workflow on the self-hosted runner once and attach the run log before treating CI as full validation evidence. |

Do not close these rows from source review alone. Close only after the matching live manual proof exists.
