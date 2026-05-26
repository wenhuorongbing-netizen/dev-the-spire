# Spire Plus Issues
Current target: test-ready manual build, not release-ready. Current package hashes, 2026-05-26:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `5C85C7ADD99A2A06E8999654711763A351DD19A5720FE9BF8EA22412BB78382F` |
| DLL | `E4CA3D5957BECA4FFA552A7A9C68185519D2F873AA97630F84B8F7855E4420CA` |
| PCK | `B538EF54DEC975CB7E4DA91C04D442CCACCC7E1E0088D07C2B750C065FE09BE3` |
| Manifest | `5A3A2FC0E313C15640C223A6CC5D2874881F27B468D1BC7EF4AF602CD7350EFF` |
| README_INSTALL | `45E605958747C7089D5FCBC0BDE86ED0F7E646DE371658A1435B9C33703BB078` |
## Active blockers
- `SERE-TALON/TANX-CLAWS-ROUTING` and `SERE-TALON-VISUAL-IDENTITY` P0 source/package-fixed / live-pending
- `ASCENSION-SELECTOR-LOCALIZATION` P0 source/package-fixed / live-pending: character-select Ascension A11-A20 panels must show localized titles/descriptions, not raw keys like `ascension.LEVEL_20.title`.
- `SOUL-TIDE-BLOCK-TIMING` P0 source/package-fixed / live-pending: Soul Fysh now counts Beckons before turn-end in-hand damage moves them, then converts them into capped Block after its turn; player-turn-start remains a fallback.
- `URDA-SEED-BANK-HOVER` P0 source/package-fixed / live-pending: Seed Bank relic hover now shows a compact stored-card list instead of full card previews that can clip offscreen and spawn unrelated keyword tips.
- `URDA-SEEDBED-PLANTING` P0 source/package-fixed / live-pending: Planting text marks it as non-play/non-discard/non-Exhaust; planted Sprouts add no Rootblight I, and planted Rootblight keeps its master-deck stage.
- `COOP-COMBAT-START-CRASH` P0 source/package-fixed / live-pending: co-op fails closed for unproven Ascension, Ancient, combat, and preview mutations; retest with override env vars unset.
- `COOP-PREVIEW-TOOLS-CRASH` P0 source/package-fixed / live-pending: transform prediction, Crystal Sphere peek, and broader co-op gameplay mutation fail closed; retest two-client combat, transform, reconnect, save-load, and logs.
- `ENEMY-DAMAGE-POLISH` P1 source/package-fixed / live-pending: Decimillipede's strengthened Bulk attack now uses the same source getter polish as Writhe/Constrict, so the visible intent and real damage no longer stay at the old 7-damage value.
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
