# Spire Plus Issues
Current target: test-ready manual build, not release-ready. Current package hashes, 2026-05-26:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `18D28D7AFDB176E9C7A96E3CA99C40EBD68FAC04E7766C4E07D48F740E9F6D56` |
| DLL | `4D665947B042E57A1FDAF81E2682C8DEEE73DC1F19AB8DD060CCBA41B9DF9FC9` |
| PCK | `A8158CC39EB9D4B68FCF79B178829B1E0F18054E3E0774BE3A69F8B0AE383790` |
| Manifest | `1CAB72884A37FC1C7F28533A24EEA96A3D8D4BF640688D22E3B8A1D1D6F1AD2F` |
| README_INSTALL | `B453ED1511A45FEE57A0CAB9B8A004581B859F89963F32E216559F290FB511BD` |
## Active blockers
- `SERE-TALON/TANX-CLAWS-ROUTING` and `SERE-TALON-VISUAL-IDENTITY` P0 source/package-fixed / live-pending
- `ASCENSION-SELECTOR-LOCALIZATION` P0 source/package-fixed / live-pending: character-select Ascension A11-A20 panels must show localized titles/descriptions, not raw keys like `ascension.LEVEL_20.title`.
- `SOUL-TIDE-BLOCK-TIMING` P0 source/package-fixed / live-pending: Soul Fysh now counts Beckons before turn-end in-hand damage moves them, then converts them into capped Block after its turn; player-turn-start remains a fallback.
- `URDA-SEED-BANK-HOVER` P0 source/package-fixed / live-pending: Seed Bank relic hover now shows a compact stored-card list instead of full card previews that can clip offscreen and spawn unrelated keyword tips.
- `URDA-SEEDBED-PLANTING` P0 source/package-fixed / live-pending: Planting text marks it as non-play/non-discard/non-Exhaust; planted Sprouts add no Rootblight I, and planted Rootblight keeps its master-deck stage.
- `COOP-COMBAT-START-CRASH` P0 source/package-fixed / live-pending: co-op fails closed for unproven Ascension, Ancient, combat, and preview mutations; retest with override env vars unset.
- `COOP-PREVIEW-TOOLS-CRASH` P0 source/package-fixed / live-pending: transform prediction, Crystal Sphere peek, and broader co-op gameplay mutation fail closed; retest two-client combat, transform, reconnect, save-load, and logs.
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
- `FISSION-EXHAUST-TRIGGERS` P1 source/package-fixed / live-pending: canonical Exhaust path guarded through Fission keyword, `CardCmd.Exhaust(...)`, `AfterCardExhausted(...)`, and Drum/Howl/power/relic listeners.
- Strict source/BaseLib audit, 2026-05-20: `STRICT-AUDIT-LIVE-EVIDENCE`, `STRICT-AUDIT-VAKUU-FIGHT` P0 open; `STRICT-AUDIT-VAKUU-CULTURE-SAVE`, `STRICT-AUDIT-PATCH-SURFACE`, `STRICT-AUDIT-EVIDENCE-LOG` P1/P2 source-fixed / live-pending.
## Engineering governance blockers: `GOV-WIP-SPLIT` P0 source-fixed; current dirty worktree is intentionally batch-classified with 0 unclassified entries. `GOV-CI-FIRST-RUN`, `DOC-CONFLICT-GOVERNANCE`, and `PLATFORM-PACKAGE-CHECKS` P2 pending.
## Manual Proof Gates
- `ANCIENT-CLICKED-UI/LIVE-GAMEPLAY`: capture UI screenshots/logs and test Ancient choices, A11-A20, Rootblight, Root Eyes, Seed Bank, Morvi, Lotha, and Vakuu. Use scripts/collect-ancient-ui-evidence.ps1.
- `A19-A20-DEDICATED-BOSS-ABILITIES`: fill the per-Boss checklist, logs, and notes; source guards alone cannot close it.
- `SAVE-LOAD/VAKUU-FIGHT-LIVE` / `CO-OP`: prove save/load, Vakuu victory/death/no-black-screen, multiplayer Ancient/Ascension behavior, Root Eyes, Rootblight, reconnect, and preview tools.
## Issue detail links: `docs/issues/ancient-expansion-v2.2.md`, `docs/issues/urda.md`, `docs/issues/waiting-tests.md`, `docs/issues/v3.3-design-review.md`; retest rows in `docs/toreview.md`.
