# Ancient Expansion v2.2 Work Log

## 2026-05-12 - Morvi hardening and Lotha/art blocker review

- Re-audited Morvi against local Core card-play and reward flows.
- Hardened Misprint Press to use `AncientCardHelpers.TryAddGeneratedCardToCombat(...)` so a failed generated-copy add removes the unpiled clone from combat state before returning.
- Hardened Debt Settlement payoff cleanup so `DebtRewardPending` is cleared from freshly read progress only after the payoff reward resolver succeeds.
- Clarified Morvi Debt Settlement English/zhs text to say missing Gold falls back to nonlethal HP.
- Added source guards for Morvi generated-copy cleanup, clone/reentry/Power-card safety, Debt Settlement nonlethal HP fallback, delayed payoff reward UI, and event-art pending status.
- Rechecked local Act 3 Ancient source: `Glory.GetUnlockedAncients(...)` returns `AllAncients.ToList()` with no native extension hook, so any Lotha insertion would need the same narrow Harmony-postfix shape currently used by Urda/Morvi.
- Rechecked local event visuals: `NAncientEventLayout.InitializeVisuals()` loads an Ancient background scene through `AncientEventModel.CreateBackgroundScene()`, and `EventModel.GetAssetPaths(...)` preloads `BackgroundScenePath` for `EventLayoutType.Ancient`. BaseLib exposes `CustomAncientModel.CustomScenePath`, but this pass has no explicit local Morvi/Lotha source art or custom scene file to bind.
- No explicit local source file was found for `EZMicroBalance/images/events/ezmb_morvi.png` or `EZMicroBalance/images/events/ezmb_lotha.png`; no placeholder art, `.import`, or export-preset entry was added.
- Lotha and Vakuu gameplay remain planning-only in this pass. No future Urda blessing, A21-A30, or custom-character content was added.
- Validation:
  - `git status --short --branch`: branch `main...origin/main` with a pre-existing dirty worktree.
  - `git log -1 --oneline --decorate`: `c8bcaa9 (HEAD -> main, origin/main, origin/HEAD) update`.
  - `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: passed, 77 passed, 16 skipped.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `dotnet publish EZMicroBalance.sln`: passed because localization changed; Godot emitted the known nested `source code/project.godot` ignore warning.
  - Post-publish `dotnet test EZMicroBalance.sln --no-build`: passed, 77 passed, 16 skipped.
  - `git diff --check`: passed with CRLF normalization warnings only.
- Release artifact tests were not run because release artifact logic was not changed.
- Live game, save/load, and co-op testing remain pending.

## 2026-05-12 - Morvi/Lotha art direction and next prompt

- Recorded user-approved event-art direction for Morvi and Lotha in `art-direction.md`.
- Target paths are `EZMicroBalance/images/events/ezmb_morvi.png` and `EZMicroBalance/images/events/ezmb_lotha.png`.
- Did not copy unverified temporary image files into active resources; final image bytes still need explicit local source confirmation before export.
- Added `next-development-prompt.md` for the next implementation pass.
- Lotha and Vakuu gameplay remain planning-only; Morvi remains default-off.

## 2026-05-12 - Morvi default-off prototype

- Added `EZMB_ENABLE_MORVI_V22=1` gated Act 2 Morvi registration.
- Added `EZMB_FORCE_MORVI_BLESSING` for focused local testing.
- Added default-off source-backed prototypes for Misprint Press, Open-Book Exam, and Debt Settlement.
- Misprint Press uses Attack/Skill-only generated-copy autoplay with clone/reentry guards and Power-card exclusion.
- Open-Book Exam upgrades one Attack or Skill option in normal Act 2 combat card rewards.
- Debt Settlement grants 75 Gold on selection and adds a `Repay Debt` reward alternative for three Act 2 normal combat rewards; payoff is an upgraded card reward after the third repayment.
- Lotha, Vakuu fight, future six Urda blessings, A21-A30, and custom characters remain unimplemented.
- Live game, save/load, and co-op testing remain pending.

## 2026-05-12 - Urda acceptance hardening only

- Limited this pass to current Urda acceptance/stability work. No Morvi, Lotha, Vakuu, or future Urda blessing gameplay was added.
- Hardened Humus Pact's third payoff so `HumusCompletionPending` is cleared only after payoff resolver success; payoff card generation now happens before optional removals so a no-card fallback cannot consume removals or silently drop the payoff.
- Added/strengthened guards for Humus no `CardReward.OnSkipped`, Humus option localization, Seedbed accept-only counting, future six Urda ids not active, Morvi/Lotha/Vakuu not active, and docs not claiming Urda live/save-load verification.
- Updated local API research with negative evidence for `SavedSpireField<Player,string>` persistence: local Core `Player` serialization uses a fixed `SerializablePlayer` shape and inspected `SavedProperties` usage is card/relic/modifier-oriented, so player-field save/load remains pending live proof.
- `git status --short --branch`: branch `main...origin/main` with a pre-existing dirty worktree.
- `git log -1 --oneline --decorate`: `c8bcaa9 (HEAD -> main, origin/main, origin/HEAD) update`.
- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- `dotnet test EZMicroBalance.sln --no-build`: passed, 75 passed, 16 skipped.
- `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
- `git diff --check`: passed with CRLF normalization warnings only.
- `dotnet publish EZMicroBalance.sln`: not run; this pass did not change resources, localization, export presets, or package logic.
- Live gameplay, save/load, and co-op checks were not run.

## 2026-05-12

Urda stabilization pass.

- Reviewed current Urda code against v2.2 docs, Urda docs, issue docs, local Core source, BaseLib docs, and the tutorial index as secondary orientation.
- Confirmed current reviewed HEAD before edits: `c8bcaa9 (HEAD -> main, origin/main, origin/HEAD) update`.
- Confirmed the worktree already had unrelated modified files before this pass.
- Removed the Humus Pact dependency on a global `CardReward.OnSkipped` postfix because local Core source shows skipped reward finalization can occur during reward-set abandonment or room exit.
- Added an explicit Humus Pact reward alternative and moved third-trigger removal/payoff resolution to `AfterRewardTaken`.
- Guarded Seedbed so it only counts accepted choices, is not offered when max HP cannot safely pay the cost, and uses `SetMaxHp` for the completion bonus so the documented +10 max HP does not also heal.
- Added Humus Pact reward-option localization and source/localization guards.
- Kept Morvi, Lotha, Vakuu fight, and the six future Urda blessings out of active source.
- `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
- Validation for this pass:
  - `dotnet build EZMicroBalance.sln`: passed with 0 warnings and 0 errors.
  - `dotnet test EZMicroBalance.sln --no-build`: passed, 74 passed, 16 skipped.
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`: passed.
  - `git diff --check`: passed with CRLF normalization warnings only.
  - `dotnet publish EZMicroBalance.sln`: passed because localization/resources changed. Godot emitted the known nested `source code/project.godot` ignore warning during export.
- Release artifact tests were not run because release artifact logic was not changed.
- Live gameplay, save/load, and co-op checks are still pending for this pass.

Planning ingestion pass.

- Read `PROJECT_STATE.md`, `AGENTS.md`, docs indexes, current Urda docs, Urda source files, and the user-provided v2.2 prompt/addendum.
- Confirmed current reviewed HEAD before edits: `c8bcaa9 (HEAD -> main, origin/main, origin/HEAD) update`.
- Confirmed the worktree already had unrelated modified files before this docs pass.
- Created a planning-only v2.2 feature folder and compact issue file.
- Did not implement Morvi, Lotha, Vakuu fight, new Urda blessings, Ascension, Rootblight, Boss Seal, Fission, or multiplayer gameplay.
- Did not publish/package.
