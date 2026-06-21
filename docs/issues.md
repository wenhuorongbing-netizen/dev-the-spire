# Spire Plus Issues - Current target: test-ready manual build, not release-ready. Current package hashes, 2026-06-21 beta.96:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `6E313D383E49B750E3C5809E92D7795CC5E196B5A7511707D2AB4357E24D4265` |
| DLL | `28D9AA1632B783CE34BC4D4174C5A84DEB26FD74947529656A71757BF660309F` |
| PCK | `549FD8B2A90B2AF74F8D6C591107F423588EFD868A61D1C901585E6FE188D20C` |
| Manifest | `A752A38EFF068FDB75B629D4A0DC92153D115EFD76D369B406E3D7077E4E4593` |
| README_INSTALL | `C8171889B7B03E177CAC6428E4CCA3873BEEDB054180A10A7E6703DEBC72BDFE` |
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
- Strict source/previous package audit, 2026-05-20: `STRICT-AUDIT-LIVE-EVIDENCE`, `STRICT-AUDIT-VAKUU-FIGHT` P0 open; `STRICT-AUDIT-VAKUU-CULTURE-SAVE`, `STRICT-AUDIT-PATCH-SURFACE`, `STRICT-AUDIT-EVIDENCE-LOG` P1/P2 source-fixed / live-pending.
## Engineering governance blockers
- `GOV-WIP-SPLIT` P0 source-fixed for the committed baseline; read latest pushed migration HEAD from `git log -1 --oneline --decorate`, and recapture worktree status before final handoff.
- `GOV-CI-FIRST-RUN` P2 pending: self-hosted lane exists, but first run evidence is missing; 2026-05-26 API check found 0 completed `Full Local Validation` runs.
- `DOC-CONFLICT-GOVERNANCE` P2 source-fixed: active release audit no longer pins a stale dirty-worktree snapshot; final release handoff must still recapture current status.
- `PLATFORM-PACKAGE-CHECKS` P2 tooling-ready / tester-pending: Windows/macOS package checker docs and scripts exist; cross-machine live package parity still needs tester evidence.
- `STS1EVENTS-NULL-SAFETY-WARNINGS` P1 source-fixed / runtime-open: current mod-project forced build is 0 errors / 0 warnings; live Sts1Events proof remains pending.
- `REFACTOR-PHASE0-1-VALIDATION` P1 current: beta.93 build/source checks pass; retained split no-build lane passed 475 / 0 / 21 / 496. Phase 2 patch adapter checklist drafted; StS1Events default Off, guards active, combat events declare `IsShared`.
- `RITSULIB-RUNTIME-SMOKE` P0 loader fixed / gameplay pending: beta.93 Off and AdditiveBatch1 proof is clean on `v0.107.1` with RitsuLib `v0.4.31` / `lib\0.107.1`.
  Only STS2-RitsuLib is the shared runtime dependency; earlier smokes are previous-version or previous-package context.
  previous beta.93 RitsuLib-only AdditiveBatch1 loader/registration proof exists at `.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/`.
  Current Spire Plus package no longer depends on previous package. Gameplay/UI/save-load/co-op, QA, clean-worktree recapture, and handoff remain blocked.
## Manual Proof Gates: `ANCIENT-CLICKED-UI/LIVE-GAMEPLAY` needs screenshots/logs for Ancient choices, A11-A20, Rootblight, Root Eyes, Seed Bank, Morvi, Lotha, and Vakuu; use scripts/collect-ancient-ui-evidence.ps1.
- `A19-A20-DEDICATED-BOSS-ABILITIES`: fill the per-Boss checklist, logs, and notes; source guards alone cannot close it.
- `SAVE-LOAD/VAKUU-FIGHT-LIVE` / `CO-OP`: prove save/load, Vakuu victory/death/no-black-screen, multiplayer Ancient/Ascension behavior, Root Eyes, Rootblight, reconnect, and preview tools.
## Issue detail links: `docs/issues/ancient-expansion-v2.2.md`, `docs/issues/urda.md`, `docs/issues/waiting-tests.md`, `docs/issues/v3.3-design-review.md`; retest rows in `docs/toreview.md`.
