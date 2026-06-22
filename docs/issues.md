# Spire Plus Issues - Current target: test-ready manual build, not release-ready. Current package hashes, 2026-06-22 beta.122:
| Artifact | SHA256 |
| --- | --- |
| ZIP | `2D9752CBEB98A358D75DF85B2189F61DA3B116A1619A329C2959CB88BB964D98` |
| DLL | `C6355689524B5A9E680531E7DFFD989B07460BAA6CC4204A81A4719DA881148D` |
| PCK | `B9D4BB9543A6CC0089598533C13ED56D045BFCEA55A0900D246F49F5F5C6325F` |
| Manifest | `D75A2417583C3B8E0926730AA42295861B99A5FBD2C85006F82B82E3654F4FDB` |
| README_INSTALL | `7280953E68CCA9E208D71D588E52ED889A8212660BEF910AFE253178E5BAB171` |
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
- `TANX-CLAWS-MAUL-TUNING` P2 source-fixed / live-pending: Tanx Claws now creates upgraded Maul+ / 鎾曞挰+; live pickup proof still pending.
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
- `RITSULIB-RUNTIME-SMOKE` P0 runtime-smoke current / gameplay pending.
  beta.122 package parity, runtime preflight 28 / 0, source-workspace validation 57 / 0, and clicked UI smoke packet verification 1621 / 0 are current for the 127-patch source state.
  Evidence: `.tools/runtime-evidence/monkey-stability-beta122-20260622-230109/`; beta.99/beta.96/beta.93 and earlier smokes are previous-version or previous-package context.
  Enabled-mode proof, gameplay, save-load, co-op, QA, clean-worktree recapture, and handoff remain blocked.
## Manual Proof Gates: `ANCIENT-CLICKED-UI/LIVE-GAMEPLAY`: beta.122 Ancient smoke covers Urda/Morvi/Lotha/normal Vakuu; gated Vakuu fight, hover/readability, A11-A20, save-load, co-op, gameplay proof remain pending; use scripts/collect-ancient-ui-evidence.ps1.
- `A19-A20-DEDICATED-BOSS-ABILITIES`: fill the per-Boss checklist, logs, and notes; source guards alone cannot close it.
- `SAVE-LOAD/VAKUU-FIGHT-LIVE` / `CO-OP`: prove save/load, Vakuu victory/death/no-black-screen, multiplayer Ancient/Ascension behavior, Root Eyes, Rootblight, reconnect, and preview tools.
## Issue detail links: `docs/issues/ancient-expansion-v2.2.md`, `docs/issues/urda.md`, `docs/issues/waiting-tests.md`, `docs/issues/v3.3-design-review.md`; retest rows in `docs/toreview.md`.
