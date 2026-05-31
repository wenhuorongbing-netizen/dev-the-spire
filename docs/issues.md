# Spire Plus Issues - Current target: test-ready manual build, not release-ready. Current package hashes, 2026-05-31:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `9F0656DEDF57598D3A26743FD1A3061E669EF21354D6E12A6AA4C4AF44796894` |
| DLL | `D65E7AE135A1D49F1403F96B29FE800A840E55D496480E380558AD2EE1211766` |
| PCK | `EEB90DA21DC66E24B78183374A3064D58F8ACCB346DE200E58BAA283BC7887CA` |
| Manifest | `47658D7593FE8DF23C4E5B3413CA53B1C7F1B779109A42A8B65E381C4C0FEDAC` |
| README_INSTALL | `C99C7B4146F4CEE49B661F1839E1E68EB414E2F7805DD4C66853A4D3D0D88D8C` |
## Active blockers
- `SERE-TALON/TANX-CLAWS-ROUTING` and `SERE-TALON-VISUAL-IDENTITY` P0 source/package-fixed / live-pending
- `ASCENSION-SELECTOR-LOCALIZATION` P0 source/package-fixed / live-pending: character-select Ascension A11-A20 panels must show localized titles/descriptions, not raw keys like `ascension.LEVEL_20.title`.
- `SOUL-TIDE-BLOCK-TIMING` P0 source/package-fixed / live-pending: user still saw only Artifact; pending Block now grants at next player start and enemy-turn-end no longer consumes it early.
- `URDA-SEED-BANK-HOVER` P0 source/package-fixed / live-pending: Seed Bank relic hover now shows a compact stored-card list instead of full card previews that can clip offscreen and spawn unrelated keyword tips.
- `URDA-SEEDBED-PLANTING` P0 source/package-fixed / live-pending: Planting text marks it as non-play/non-discard/non-Exhaust; planted Sprouts add no Rootblight I, and planted Rootblight keeps its master-deck stage.
- `COOP-COMBAT-START-CRASH` P0 source/package-fixed / live-pending: co-op fails closed for unproven shared-state mutations; local UI-only preview tools are exempt.
- `COOP-PREVIEW-TOOLS-CRASH` P0 source/package-fixed / live-pending: Crystal Sphere and transform preview now run in co-op as local UI only; no choices, rewards, or real RNG calls.
- `ENEMY-DAMAGE-POLISH` P1 source/package-fixed / live-pending: strengthened Bulk now uses the same getter polish as Writhe/Constrict, so visible intent and real damage no longer stay at 7.
- `ROOTBLIGHT-STARTER-MISSING` / `BOSS-BLIGHT-SPROUT-PLAYED-STILL-ROOTBLIGHT` P1 source-fixed / live-pending: combat start retries A14 starter Rootblight before growth marking; played/planted Blight Sprouts still do not grow.
- `TANX-CLAWS-MAUL-TUNING` P2 source-fixed / live-pending: Tanx Claws now creates upgraded Maul+ / 撕咬+; live pickup proof still pending.
- `QUEEN-BOSS-SEAL-RUNTIME` P0 source/package-fixed / live-pending: Royal Decree now skips un-enchantable Bound cards before `CardCmd.Enchant`; retest Queen/Royal Decree and capture `godot.log`.
- `URDA-PROTOTYPE` P0 open / live-pending: default-on with eleven source-backed ids, including Elite Root; live gameplay and save/load proof remain pending.
- `MORVI/LOTHA/VAKUU` P0/P1 live-pending: default-on morvi_forbidden_loan/lotha_death_reprieve need gameplay proof; hidden Vakuu fight needs victory, death/failure, save/load, and no-black-screen proof.
## User manual-test findings, 2026-05-22/24
- `HUSK-CARD-BEHAVIOR`, `ROOT-SIGHT-ENCOUNTER-POOL`, `FIREMARK-HEAL/TEXT` P1: check Husk block, Root Eyes previews, and Act Constant-Heal thresholds.
- `UNKNOWN-EVENT-PREVIEW-READABILITY`, `WATERFALL-BOSS-SEAL` P1: check event hover and Waterfall Boss.
- `HOURGLASS-BOSS-SEAL-DESIGN`, `QUEEN-BOSS-SEAL-WEAKNESS`, `FIREMARK-OVERFLOW/FORGE-ARMOR`, `BANNER-ROOM-PREVIEW` P1: check Time Sand, Royal Decree, Firemark secondary-target explanation, and Banners.
- `ROOT-EYES-CONFLICTS-COOP`, `PREVIEW-TOOLS-REWARD-HOOKS`, `SEAL-BANNER-VISIBILITY`, `V33-DESIGN-PASS` P1: check co-op Root Eyes, preview rewards, seal visibility, and v3.3 designs.
- `FISSION-EXHAUST-TRIGGERS` P1 source/package-fixed / live-pending: canonical Exhaust path guarded through Fission keyword, `CardCmd.Exhaust(...)`, `AfterCardExhausted(...)`, and Drum/Howl/power/relic listeners.
- Strict source/BaseLib audit, 2026-05-20: `STRICT-AUDIT-LIVE-EVIDENCE`, `STRICT-AUDIT-VAKUU-FIGHT` P0 open; `STRICT-AUDIT-VAKUU-CULTURE-SAVE`, `STRICT-AUDIT-PATCH-SURFACE`, `STRICT-AUDIT-EVIDENCE-LOG` P1/P2 source-fixed / live-pending.
## Engineering governance blockers
- `GOV-WIP-SPLIT` P0 source-fixed for the committed baseline; current local Runtime Proof + Governance Closure worktree is dirty with preserved shared edits and new diagnostics/doc/test updates.
- `GOV-CI-FIRST-RUN` P2 pending: self-hosted lane exists, but first run evidence is missing; 2026-05-26 API check found 0 completed `Full Local Validation` runs.
- `DOC-CONFLICT-GOVERNANCE` P2 source-fixed: active release audit no longer pins a stale dirty-worktree snapshot; final release handoff must still recapture current status.
- `PLATFORM-PACKAGE-CHECKS` P2 tooling-ready / tester-pending: Windows/macOS package checker docs and scripts exist; cross-machine live package parity still needs tester evidence.
- `STS1EVENTS-NULL-SAFETY-WARNINGS` P1 open: 89 nullable warnings are tracked as Sts1Events-only staging debt (`CS8604` = 54, `CS8602` = 34, `CS8625` = 1); see `docs/issues/ISSUE-2026-05-31-STS1EVENTS-NULL-SAFETY-WARNINGS.md`.
- `REFACTOR-PHASE0-1-VALIDATION` P1 historical done; current no-game validation is 0 build errors / 89 warnings and 461 passed / 0 failed / 21 skipped / 482 total. Runtime smoke remains blocked.
  Phase 2 patch adapter rule started (checklist drafted). StS1Events feature-gated (default Off, guard tests active). Combat events now declare `IsShared`. See `docs/reviews/overnight-run-20260529.md`.
- `RITSULIB-RUNTIME-SMOKE` P0 hard blocked: `STS2-RitsuLib` missing at checked D/E mod paths; E-drive BaseLib and Spire Plus exist; active `godot.log` is missing.
  Batch 4c, high-risk migration, Off/CanaryOnly runtime claims, live-ready, and release-ready stay blocked until install + fresh `godot.log`. See `docs/reviews/current-validation.md` and `docs/features/ritsulib-migration/runtime-smoke-checklist.md`.
## Manual Proof Gates
- `ANCIENT-CLICKED-UI/LIVE-GAMEPLAY`: capture UI screenshots/logs and test Ancient choices, A11-A20, Rootblight, Root Eyes, Seed Bank, Morvi, Lotha, and Vakuu. Use scripts/collect-ancient-ui-evidence.ps1.
- `A19-A20-DEDICATED-BOSS-ABILITIES`: fill the per-Boss checklist, logs, and notes; source guards alone cannot close it.
- `SAVE-LOAD/VAKUU-FIGHT-LIVE` / `CO-OP`: prove save/load, Vakuu victory/death/no-black-screen, multiplayer Ancient/Ascension behavior, Root Eyes, Rootblight, reconnect, and preview tools.
## Issue detail links: `docs/issues/ancient-expansion-v2.2.md`, `docs/issues/urda.md`, `docs/issues/waiting-tests.md`, `docs/issues/v3.3-design-review.md`; retest rows in `docs/toreview.md`.
