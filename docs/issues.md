# Spire Plus Issues
Current target: test-ready manual build, not release-ready. Current package hashes, 2026-05-26:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `CA19F9CC83F3F65581D10F1066C04B8E58E812DE07E1DBA7ACA6FEB4D22F693A` |
| DLL | `7C894354725DC2DD789245A35DE73F707CD6E82CE54F82B9287B1CB98B0DB970` |
| PCK | `14BA8D3A135A154C68512B83E59BF4DBF7117320A5C68EDE1E56640245D701F0` |
| Manifest | `91924EFCF444F0DC495A8B16CA171AA58B923517FC283CC553C2EC6408EA7A7F` |
| README_INSTALL | `174715C398A6DB6D91ABDFCE294A89B268E2B00AE359FC170C73B72259F1DACD` |
## Active blockers
- `SERE-TALON/TANX-CLAWS-ROUTING` and `SERE-TALON-VISUAL-IDENTITY` P0 source/package-fixed / live-pending
- `ASCENSION-SELECTOR-LOCALIZATION` P0 source/package-fixed / live-pending: character-select Ascension A11-A20 panels must show localized titles/descriptions, not raw keys like `ascension.LEVEL_20.title`.
- `SOUL-TIDE-BLOCK-TIMING` P0 source/package-fixed / live-pending: Soul Fysh's Artifact-without-Block report is source-fixed; pending Beckons now convert to capped Block at enemy turn end, with player-turn-start fallback for unusual flow.
- `URDA-SEED-BANK-HOVER` P0 source/package-fixed / live-pending: Seed Bank relic hover now shows a compact stored-card list instead of full card previews that can clip offscreen and spawn unrelated keyword tips.
- `URDA-SEEDBED-PLANTING` P0 source/package-fixed / live-pending:
  - Seedbed explains Planting / 种下 as a non-play, non-discard, non-exhaust handling action.
  - Planted Blight Sprouts are handled and add no Rootblight I after combat.
  - Planted Rootblight freezes this combat's end check only and stays in the master deck at the same stage; it does not improve, worsen, split, downgrade, cleanse, or get removed.
- `COOP-COMBAT-START-CRASH` P0 source/package-fixed / live-pending:
  - User log ended during multiplayer combat startup for `CUBEX_CONSTRUCT_NORMAL`, with no managed stack. Treat this as latest Spire Plus crash evidence even if the in-game manifest text still shows an older beta label.
  - Source fix: co-op fails closed before proof for A11-A20 selection/gameplay, Ascension map/rewards, Ancient offers/selections/run hooks, Urda reward alternatives, combat hooks, and preview tools.
  - Retest must use the newly packaged build and leave `SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY`, `SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS`, and `SPIREPLUS_ALLOW_UNVERIFIED_COOP_PREVIEW_TOOLS` unset unless deliberately debugging.
- `COOP-PREVIEW-TOOLS-CRASH` P0 source/package-fixed / live-pending:
  - User log `godot2026-05-25T08.54.22.log` is latest crash evidence despite the stale beta label. It shows co-op A20, A14 Rootblight, Urda/Seed Bank, A13 Fission, reconnect, and `Aroma of Chaos` transform sync.
  - Source fix: transform prediction, Crystal Sphere peek, and broader Spire Plus co-op gameplay mutation fail closed by default.
  - Retest needs two-client combat start, transform event, Crystal Sphere if available, reconnect/save-load, and both clients' logs.
- `ENEMY-DAMAGE-POLISH` P1 open / user-regressed: strengthened Decimillipede still attacks for 7; recheck Decimillipede damage polish against intent and real damage.
- `TANX-CLAWS-MAUL-TUNING` P2 source-fixed / live-pending: Tanx Claws now creates upgraded Maul+ / 撕咬+; live pickup proof still pending.
- `QUEEN-BOSS-SEAL-RUNTIME` P0 open: user reports the Queen/Royal Decree fight cannot run correctly; reproduce on the current package and capture `godot.log`.
- `URDA-PROTOTYPE` P0 open / live-pending: default-on with eleven source-backed ids, including Elite Root; live gameplay and save/load proof remain pending.
- `MORVI/LOTHA/VAKUU` P0/P1 live-pending: default-on morvi_forbidden_loan/lotha_death_reprieve need gameplay proof; hidden Vakuu fight needs victory, death/failure, save/load, and no-black-screen proof.
## User manual-test findings, 2026-05-22/24
- `HUSK-CARD-BEHAVIOR`, `ROOT-SIGHT-ENCOUNTER-POOL`, `FIREMARK-HEAL/TEXT` P1: check Husk block, Root Eyes previews, and Act Constant-Heal thresholds.
- `UNKNOWN-EVENT-PREVIEW-READABILITY`, `ROOTBLIGHT-STARTER-MISSING`, `BOSS-BLIGHT-SPROUT-PLAYED-STILL-ROOTBLIGHT`, `WATERFALL-BOSS-SEAL` P1: check event hover, Rootblight starter deck, Boss Blight Sprout play prevention, and Waterfall Boss.
- `HOURGLASS-BOSS-SEAL-DESIGN`, `QUEEN-BOSS-SEAL-WEAKNESS`, `FIREMARK-OVERFLOW/FORGE-ARMOR`, `BANNER-ROOM-PREVIEW` P1: check Time Sand, Royal Decree, Firemark secondary-target explanation, and Banners.
- `ROOT-EYES-CONFLICTS-COOP`, `PREVIEW-TOOLS-REWARD-HOOKS`, `SEAL-BANNER-VISIBILITY`, `V33-DESIGN-PASS` P1: check co-op Root Eyes, preview rewards, seal visibility, and v3.3 designs.
- `FISSION-EXHAUST-TRIGGERS` P1 open: Fission cards must count as real Exhaust when they leave play; acceptance is the canonical exhaust path, updated intent/text, and one card-owned plus one relic/power-style listener test.
- Strict source/BaseLib audit, 2026-05-20: `STRICT-AUDIT-LIVE-EVIDENCE`, `STRICT-AUDIT-VAKUU-FIGHT` P0 open; `STRICT-AUDIT-VAKUU-CULTURE-SAVE`, `STRICT-AUDIT-PATCH-SURFACE`, `STRICT-AUDIT-EVIDENCE-LOG` P1/P2 source-fixed / live-pending.
## Engineering governance blockers: `GOV-WIP-SPLIT` P0 source-fixed; current dirty worktree is intentionally batch-classified with 0 unclassified entries. `GOV-CI-FIRST-RUN`, `DOC-CONFLICT-GOVERNANCE`, and `PLATFORM-PACKAGE-CHECKS` P2 pending.
## Manual Proof Gates
- `ANCIENT-CLICKED-UI/LIVE-GAMEPLAY`: capture UI screenshots/logs and test Ancient choices, A11-A20, Rootblight, Root Eyes, Seed Bank, Morvi, Lotha, and Vakuu. Use scripts/collect-ancient-ui-evidence.ps1.
- `A19-A20-DEDICATED-BOSS-ABILITIES`: fill the per-Boss checklist, logs, and notes; source guards alone cannot close it.
- `SAVE-LOAD/VAKUU-FIGHT-LIVE` / `CO-OP`: prove save/load, Vakuu victory/death/no-black-screen, multiplayer Ancient/Ascension behavior, Root Eyes, Rootblight, reconnect, and preview tools.
## Issue detail links: `docs/issues/ancient-expansion-v2.2.md`, `docs/issues/urda.md`, `docs/issues/waiting-tests.md`, `docs/issues/v3.3-design-review.md`; retest rows in `docs/toreview.md`.
