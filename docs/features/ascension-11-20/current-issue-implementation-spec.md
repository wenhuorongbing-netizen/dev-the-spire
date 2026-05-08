# Current Issue Implementation Spec

Date: 2026-05-08

## 1. Open Issues Fixed This Pass

- `ISSUE-2026-05-08-ASCENSION-PUBLIC-SELECTION-DEFAULT-ON-FOR-MP-TEST`: make A11-A20 selection default-on for the private-beta multiplayer test candidate while preserving emergency disable switches and conservative progress/gameplay boundaries.
- `ISSUE-2026-05-07-A11-LONG-ROAD-MAP-MARKER-UNWANTED`: keep A11 documented as map width/row geometry only, with no A11-specific marker, icon, or hover tooltip.
- `ISSUE-2026-05-07-A20-MULTIPLAYER-SELECTION-WARNING-MISSING`: add a log-facing warning when host multiplayer selects or starts A20 while Dual King Brands gameplay remains single-player gated; follow-up fixes the host-only lobby case before any client joins.
- `ISSUE-2026-05-07-RELEASE-ARTIFACT-TESTS-DEPEND-ON-IGNORED-PUBLISH-OUTPUT`: make release artifact/package/hash/runtime-smoke tests opt-in with `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1`.
- `ISSUE-2026-05-07-CURRENT-PACKAGE-RUNTIME-SMOKE-STALE`: refresh smoke if local conditions allow; otherwise keep docs explicitly pending and do not claim current-package runtime readiness.
- `ISSUE-2026-05-07-LIVE-COOP-A11-A20-MATRIX-PENDING`: keep live co-op status pending and expand the manual checklist with the required gate/selection/ownership/desync checks.
- `ISSUE-2026-05-07-HANDOFF-GIT-STATUS-HYGIENE`: refresh point-in-time git handoff status so already tracked files are not described as untracked.

## 2. Source Evidence

- `source code/src/Core/Multiplayer/Game/Lobby/StartRunLobby.cs`
  - `UpdateMaxMultiplayerAscension()` computes host cap from `LobbyPlayer.maxMultiplayerAscensionUnlocked`, then clamps `Ascension` to `MaxAscension` through `SyncAscensionChange(MaxAscension)`.
  - `SyncAscensionChange(int ascension)` is the host/single-player selection change surface and broadcasts `LobbyAscensionChangedMessage`.
  - `BeginRunForAllPlayers(...)` calls `UpdatePreferredAscension()` before `BeginRunLocally(...)`.
  - `UpdatePreferredAscension()` writes `ProgressState.PreferredMultiplayerAscension` on host multiplayer.
  - `BeginRunLocally(...)` only clamps single-player Ascension to character `MaxAscension`; multiplayer launches with the lobby value.
- `source code/src/Core/Nodes/Screens/CustomRun/NCustomRunScreen.cs`
  - `OnAscensionPanelLevelChanged()` calls `_lobby.SyncAscensionChange(_ascensionPanel.Ascension)` for host and single-player, not for clients.
  - `MaxAscensionChanged()` calls `_ascensionPanel.SetMaxAscension(_lobby.MaxAscension)`.
  - `AscensionChanged()` mirrors lobby value into `NAscensionPanel`.
- `source code/src/Core/Nodes/Screens/CharacterSelect/NAscensionPanel.cs`
  - `SetMaxAscension(int)` controls arrow bounds and panel visibility.
  - `RefreshAscensionText()` uses `AscensionHelper.GetTitle/GetDescription`, so adding a safe lobby warning UI would require patching UI text or adding another node; no narrow source-supported hint surface was proven this pass.
- `source code/src/Core/Entities/Multiplayer/LobbyPlayer.cs`
  - `LobbyPlayer.maxMultiplayerAscensionUnlocked` is serialized over the lobby protocol.
- `source code/src/Core/Saves/ProgressState.cs`
  - `PreferredMultiplayerAscension` and `MaxMultiplayerAscension` are persisted fields.
  - `ClampAscension(...)` clamps progress values above 10 back to 10.
- `source code/src/Core/Saves/Managers/ProgressSaveManager.cs`
  - `IncrementMultiplayerAscension(SerializableRun run)` only increments `MaxMultiplayerAscension` while it is `< 10`.
- `source code/src/Core/Saves/SerializableRun.cs` and `source code/src/Core/Runs/RunState.cs`
  - run Ascension is stored as an `int` (`SerializableRun.Ascension`, `RunState.AscensionLevel`), which supports development run values above A10 without changing vanilla progress.
- `EZMicroBalanceCode/Ascension/AscensionSelectionPatches.cs`
  - current selector patch expands only `StartRunLobby` single-player and host multiplayer paths.
  - `UpdatePreferredAscension` prefix skips A11-A20 preferred-progress writes.
  - A20 downgrade warning should use the host multiplayer selection surface and A20 level gate, not current `Players.Count > 1`, because a host-only lobby can select A20 before any client joins.
- `EZMicroBalanceCode/Ascension/AscensionFeatureGate.cs`
  - `IsPublicSelectionEnabled` previously required `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1`; for the private-beta multiplayer test candidate this should become default-on and only be disabled by `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1`.
  - `PublicGateEnvironmentVariable` remains defined for compatibility/docs/history, but it must no longer be required by the `IsPublicSelectionEnabled` predicate.
  - `IsMultiplayerSelectionDisabled` remains tied only to `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`, so single-player A11-A20 selection can stay available while host-multiplayer expansion is disabled for comparison.
  - `IsDualKingBrandsSinglePlayerEnabled(IRunState)` requires `runState.Players.Count == 1`, so host multiplayer A20 selection can outpace A20 gameplay support.
- `EZMicroBalanceCode/Ascension/AscensionMapUiPatches.cs` and `EZMicroBalanceCode/Ascension/AscensionNodeMetadata.cs`
  - A12 Firemark, A16 Banner, A17 Deep Branch, and A19/A20 boss hovers are still separate metadata paths.
  - There is no `LongRoad` metadata path in active source.
- `tests/EZMicroBalance.Tests/**`
  - release artifact tests currently read ignored `publish/` zip/staging files and installed DLL/PCK artifacts directly.

Secondary reference checked: `https://glitchedreme.github.io/SlayTheSpire2ModdingTutorials/index.html` only as orientation for BaseLib/RitsuLib/save/debug sections; local Core source remains the implementation authority.

## 3. Safe Implementation Plan

1. Add an A20 host-multiplayer warning helper in `AscensionSelectionPatches`.
   - Trigger on host `SyncAscensionChange` when selected Ascension is A20+.
   - Trigger again when a host multiplayer A20 run starts.
   - Do not require `lobby.Players.Count > 1`; the warning should also fire in a host-only multiplayer lobby before a client joins.
   - Use `MainFile.Logger.Warn` only; no UI node patch this pass because a safe lobby hint surface is not proven.
   - Do not change progress, multiplayer packets, run state, A20 gameplay gates, or client behavior.
2. Flip A11-A20 public selection to default-on for this multiplayer test candidate.
   - Change `AscensionFeatureGate.IsPublicSelectionEnabled` to return true unless `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` is set.
   - Keep `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` accepted as a no-op legacy-compatible opt-in variable for testers who already set it.
   - Keep `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` scoped to host-multiplayer selector expansion only.
   - Do not alter `DebugLevel`, A11-A20 progress-write skip behavior, or A20 Dual King Brands single-player gameplay gate.
3. Add source guards for:
   - `IsPublicSelectionEnabled` no longer requiring `IsTruthy(Environment.GetEnvironmentVariable(PublicGateEnvironmentVariable))`;
   - emergency disable variable still controlling public selection;
   - multiplayer-only disable variable still present and scoped;
   - warning text and warning call sites;
   - A20 gameplay remaining single-player gated;
   - A11 Long Road marker/hover text staying absent while A12/A16/A17/A19/A20 indicator paths remain.
4. Add a small xUnit-compatible release artifact gate:
   - `ReleaseArtifactFactAttribute` skips tests unless `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1` is set.
   - Gate only tests that depend on ignored `publish/`, staging/versioned zip, installed DLL/PCK, or local smoke logs.
   - When the env var is set, missing artifacts continue to fail.
5. Refresh current-facing docs:
   - changelog A11 wording;
   - checklist/manual matrix A20 co-op warning and pending status;
   - test plan release-artifact command order;
   - release checklist, handoff, dev environment, completion audit, issues, api research, and work log with truthful runtime/co-op pending state.
   - Replace stale 9-SavedSpireFields wording with the refreshed 12-field smoke result.
   - Keep handoff git status as a point-in-time local snapshot and avoid describing tracked files as untracked.
   - State that A11-A20 selection is now default-on in this private-beta multiplayer test candidate.
   - State `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1`, `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`, and legacy-compatible `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` behavior consistently.
   - Add `docs/features/ascension-11-20/multiplayer-test-runbook.md` with multiplayer setup, Windows env var commands, exact matrix, and result template.

## 4. Tests To Add Or Update

- `tests/EZMicroBalance.Tests/ReleaseArtifactFactAttribute.cs`: new opt-in Fact attribute for artifact/runtime-smoke tests.
- `tests/EZMicroBalance.Tests/AncientBehaviorGuardTests.cs`: gate private-beta zip test.
- `tests/EZMicroBalance.Tests/ReleaseArtifactTests.cs`: gate publish/installed/API artifact tests that are not clean-clone friendly.
- `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs`: gate staging/versioned/hash/handoff artifact tests and add normal source/docs guards for release-artifact gate documentation.
- `tests/EZMicroBalance.Tests/ReleaseSafetyExpandedGuardTests.cs`: gate PCK/package/hash/recent-smoke tests.
- `tests/EZMicroBalance.Tests/AscensionFeatureGuardTests.cs`: require A20 multiplayer downgrade warning source.
- `tests/EZMicroBalance.Tests/AscensionFeatureGuardTests.cs`: guard that the A20 warning predicate does not regress to `lobby.Players.Count > 1`.
- `tests/EZMicroBalance.Tests/AscensionFeatureGuardTests.cs`: guard that public selection is default-on unless the disable env var is set.
- `tests/EZMicroBalance.Tests/ReleaseCoverageGuardTests.cs` and related docs guards: update expected current-facing wording from default-disabled to default-on private-beta multiplayer testing with explicit disable switches.

## 5. Docs To Update

- `docs/mod-changelog.md`
- `docs/issues.md`
- `docs/test-plan.md`
- `docs/dev-environment.md`
- `docs/release-checklist.md`
- `docs/private-beta-verification-handoff.md`
- `docs/features/ancients-rework-v4/completion-audit.md`
- `docs/features/ascension-11-20/api-research.md`
- `docs/features/ascension-11-20/implementation-plan.md`
- `docs/features/ascension-11-20/development-checklist-v2.md`
- `docs/features/ascension-11-20/manual-test-checklist.md`
- `docs/features/ascension-11-20/multiplayer-test-runbook.md`
- `docs/features/ascension-11-20/work-log.md`

## 6. Runtime And Manual Checks Still Pending

- Normal Steam-client Mod Settings verification remains pending.
- Live Ancient reward feature matrix remains pending.
- Current-package controlled `--force-steam off` smoke was refreshed after this publish and reported 12 SavedSpireFields; repeat it after future source/package/BaseLib changes.
- Live co-op A11-A20 matrix remains pending:
  - gate off via `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1`: A1-A10 unchanged;
  - default-on with no env var: host selects A11-A20;
  - legacy `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` remains accepted but no longer required;
  - disable multiplayer selection env var restores vanilla cap;
  - client join does not clamp host A11-A20 selection;
  - A11 co-op map wider/longer with no A11 marker/tooltip;
  - A12/A16 markers visible;
  - A14/A15/A18 ownership is per-player;
  - A20 warning/downgrade is visible/logged on host-only selection and again on run start after a client joins;
  - `godot.log` has no desync/checksum/ownership warnings.

## 7. Rollback Plan

- Revert only `AscensionSelectionPatches` warning additions or the host-only predicate relaxation and the related source guard snippets if the warning path causes issues.
- Revert only the `IsPublicSelectionEnabled` predicate to require `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` again if default-on selection blocks immediate multiplayer testing, keeping `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` documented as the emergency off switch.
- Remove `ReleaseArtifactFactAttribute` and restore affected `[ReleaseArtifactFact]` tests to `[Fact]` if the project decides normal tests should always require package artifacts.
- Docs can be reverted independently; no manifest id, save/progress schema, A20 gameplay gate, or packaged official assets are changed by this pass.

## 8. Acceptance Checklist

- [x] A11 docs/changelog say width +1, Act 1 +1 row, Act 2 +1 row, Act 3 +2 rows, and no A11 marker/icon/hover tooltip.
- [x] A12 Firemark, A16 Banner, A17 Deep Branch, A19 Royal Seal, and A20 Brand indicators remain documented and guarded.
- [x] Host multiplayer A20 selection/start logs a clear development-testing downgrade warning, including host-only selection before a client joins.
- [x] A20 Dual King Brands gameplay remains single-player gated pending live co-op proof.
- [x] A11-A20 selection is default-on with no env var, while `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1` restores vanilla A1-A10 for comparison.
- [x] `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1` disables only host-multiplayer A11-A20 selection while leaving single-player A11-A20 available.
- [x] `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` remains documented as legacy-compatible and no longer required.
- [x] Normal `dotnet test EZMicroBalance.sln --no-build` does not require ignored release package artifacts.
- [x] `EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build` runs the package/hash/runtime artifact checks and fails clearly if artifacts are missing.
- [x] Build/test/format/diff checks are run and documented.
- [x] Publish and runtime smoke are run if safe; otherwise pending status is explicit.
- [x] No release-ready or private-beta-ready claim is made without current runtime and live manual evidence.
