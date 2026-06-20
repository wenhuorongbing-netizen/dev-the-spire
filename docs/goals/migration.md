# RitsuLib Migration Goal

## Current Target

Date: 2026-06-20

Active branch target: GitHub `main`

Current package target: Spire Plus `v0.1.0-private-beta.91`

Installed game target: Slay the Spire 2 `v0.107.1`

Runtime dependency target: official `STS2-RitsuLib` `v0.4.28` with `lib/0.107.1`

Owner update, 2026-06-20: the owner approved the final dependency migration. The
active implementation target is no remaining Spire Plus code, project, manifest,
package, release-handoff, or guard dependency on BaseLib. RitsuLib remains the
only shared runtime framework dependency for Spire Plus. Historical BaseLib
evidence may stay in archives or explicit history sections, but current setup,
package, and validation claims must not require BaseLib.

## Final BaseLib Removal Success Criteria

- `EZMicroBalance.csproj` has no `Alchyr.Sts2.BaseLib` package reference and no
  BaseLib runtime export reference.
- `EZMicroBalance.json` declares `STS2-RitsuLib` only as the runtime framework
  dependency and its description no longer says BaseLib is required.
- `EZMicroBalanceCode/**/*.cs` has no `BaseLib` namespace usage, BaseLib config
  registration, BaseLib attributes, or BaseLib abstract model inheritance.
- Spire Plus custom cards, relics, powers, enchantments, monsters, encounters,
  and Ancients are based on RitsuLib templates or game-native models and are
  explicitly registered through RitsuLib content registration.
- Mod Settings are registered through RitsuLib settings APIs and preserve the
  existing preview-tool settings defaults.
- Current release/setup docs and automated guards describe RitsuLib-only runtime
  dependency requirements for the new beta.91 package; old BaseLib evidence is
  clearly historical.
- Validation after the migration includes at least build, focused guard tests,
  format, diff-check, patch inventory, batch classifier, publish, package
  refresh, installed package parity, and the relevant release artifact guards.

Use these files as the current source of truth before acting on this goal:

- `PROJECT_STATE.md`
- `docs/reviews/current-validation.md`
- `docs/features/sts1-events/status-board.md`
- `docs/features/ritsulib-migration/next-overnight-run.md`
- `docs/test-ready-development-goal.md`

The previous long-form contents of this file were mojibake-heavy prompt notes and are archived at
`docs/archive/legacy-planning/migration-goal-mojibake-20260618.md`. Keep this active file compact,
current, and action-oriented.

## Current Conclusion

Current code, manifest, package, current setup docs, local game-source snapshot, and controlled loader
proof have moved to the beta.91 RitsuLib-only target: `EZMicroBalance.csproj`
has no `Alchyr.Sts2.BaseLib` reference, `EZMicroBalance.json` declares only
`STS2-RitsuLib`, the current package artifacts/hash docs are refreshed for
`v0.1.0-private-beta.91`, the local ignored `source code/` snapshot matches
the installed `v0.107.1` game after GDRE recovery, and the local runtime uses
official `STS2-RitsuLib` `v0.4.28` / `lib/0.107.1`. Off proof under
`.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/`
reached main menu with exactly `STS2-RitsuLib` and `EZMicroBalance` loaded,
clean audit, and Off packet verifier 43 / 0. AdditiveBatch1 proof under
`.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/`
reached main menu, registered 10 event types through 14 calls, audited clean,
passed enabled-mode verifier 31 / 0, and passed packet verifier 61 / 0. The
migration is not release-ready. Gameplay, Mod Settings UI page refresh, event
screenshots, save-load, image/render, replacement functional proof,
co-op/fail-closed proof, independent QA, and tester-package handoff remain
pending. Worktree and pushed-HEAD status must be recaptured before any later
handoff.

## Status

| Area | Current state | Evidence / notes |
| --- | --- | --- |
| Upstream RitsuLib target | Pass | Rechecked on 2026-06-20: the official GitHub releases page marks `0.4.28` as Latest and its dev-build entry says the current stable line is `0.4.28`; `dotnet list EZMicroBalance.csproj package --outdated --include-transitive` reported no `STS2.RitsuLib` update, only transitive `System.IO.Hashing` 10.0.9. |
| Current game update target | Pass | Rechecked on 2026-06-20: the official Steam news page for Slay the Spire 2 still names Major Update #2 as `v0.107.1`; local `release_info.json` matches `v0.107.1`, commit `59260271`, branch `v0.107.1`, and main assembly hash `-1555940892`. |
| RitsuLib install | Pass | `STS2-RitsuLib` `v0.4.28` is installed with `lib/0.107.1`; previous `v0.4.24` install was backed up before replacement. |
| Current game source snapshot | Pass | `source code/` was recovered from installed `v0.107.1` with GDRE Tools `v2.5.0`; checker passed 58 checks / 0 mismatches against installed version/commit/branch/main assembly hash and RitsuLib `0.4.28` / compat `0.107.1`. |
| Root cause history | Resolved for loader | The beta.84 Off failure was Spire Plus API drift, including `EctoplasmGoldGatePatch` and getter-target drift, not missing BaseLib/RitsuLib. |
| beta.85 Off loader proof | Historical pass | `v0.107.0` beta.85 package runtime proof reached main menu with 25/25 Spire Plus patches and clean audit. Treat it as previous-package/game-version loader context. |
| beta.85 CanaryOnly proof | Historical pass | Previous-package loader proof only: 4 event types / 6 registration calls. |
| beta.85 AdditiveBatch1 proof | Historical fail | Previous-package mismatch: 13/14 registration calls because the installed package/source shape was stale. |
| beta.87 build/publish/package | Pass | `dotnet build`, `dotnet publish`, package creation, and installed package parity passed for `v0.1.0-private-beta.87`. |
| beta.87 AdditiveBatch1 proof | Retained loader/registration pass | `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/` reached main menu on `v0.107.0` with BaseLib, RitsuLib, and Spire Plus loaded; 25/25 patches; 30 SavedSpireFields; 10 event types / 14 registration calls; clean audit; retained log verifier 31/0 and packet verifier 52/0. Recapture before treating it as current `v0.107.1` runtime evidence. |
| v0.107.1 AdditiveBatch1 recapture | Failed clean-loader gate | `.tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/` reached main menu in 33.81s, loaded Spire Plus `v0.1.0-private-beta.87`, selected RitsuLib `0.4.24` compat branch `0.107.0`, applied 25/25 Spire Plus patches, and matched AdditiveBatch1 10 event types / 14 calls with exact act/shared tuple parity. It is not passing proof: BaseLib `v3.2.1` logged `Applied 241 patches successfully, 2 failed`, audit found 3 BaseLib patch-failure hits and 2 Godot ERROR lines, enabled-mode verifier mismatches=2, packet verifier mismatches=1. |
| beta.91 RitsuLib-only build/publish/package | Pass | `dotnet build`, `dotnet publish`, package creation, and installed package parity passed for `v0.1.0-private-beta.91`; the package declares only `STS2-RitsuLib >= 0.4.28` as its shared runtime framework dependency. |
| beta.91 RitsuLib-only Off loader proof | Pass | `.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/` reached main menu on `v0.107.1`, loaded exactly `RitsuLib [STS2-RitsuLib] (0.4.28)` and `Spire Plus [EZMicroBalance] (v0.1.0-private-beta.91)`, applied 25/25 Spire Plus patches, audited clean, and passed Off log verifier 21 / 0 plus packet verifier 43 / 0. |
| beta.91 RitsuLib-only AdditiveBatch1 proof | Pass | `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/` reached main menu on `v0.107.1`, loaded exactly RitsuLib and Spire Plus, registered 10 event types through 14 calls, audited clean, passed enabled-mode verifier 31 / 0, and passed packet verifier 61 / 0. |
| beta.88 AdditiveBatch1 proof | Historical loader/registration pass | `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/` reached main menu on `v0.107.1` with BaseLib `v3.3.0`, RitsuLib `0.4.24`, and Spire Plus loaded; 25/25 patches; 10 event types / 14 registration calls; clean audit; retained log verifier 31/0; packet verifier 0 mismatches. This is previous BaseLib-backed package context only, not beta.91 proof. |
| Tests | Current no-game pass | Recapture `git log -1 --oneline --decorate` and `git status --short --branch` at the start of each continuation and immediately before handoff; older run-start hashes are historical notes, not current status. The beta.90 follow-up passed build 0 warnings / 0 errors, release/package artifact guards, Ancient/Ascension/save-state guards, ReleaseEvidenceGateTests, migration/docs/governance guards, format, diff-check, patch inventory, and batch classifier. RuntimeMonkey analyzer/packet checker subsets timed out in this continuation and were not counted as pass evidence. |
| Opt-in artifact subset | Current pass | Release/package/source opt-in checks passed with `SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS=1` and the local `STS2_PATH`; current installed/staging/versioned/zip hashes match beta.91. |
| StS1 static/governance lanes | Current pass | Current-doc claims 1316/0 after beta.91 RitsuLib-only doc cleanup, beta.88 doc alignment, AutoSlay proof-mode command scan-scope/presence/quoted-path recognizer, `-ExpectedAncientIds` target coverage, AutoSlay summary/analyzer binding hardening, runtime-monkey plan/result/summary binding hardening, runtime-monkey packet native-array-shape hardening including `CommandCorpus`, runtime-monkey analyzer summary array-shape, launcher-provenance, artifact-trust log-owner closure, and AutoSlay/runtime-monkey proof-mode current-target parameter hardening, AutoSlay exact top-level standard artifact path hardening, runtime-monkey live-session child EvidenceDir binding hardening, runtime-monkey summary prepare-output path/hash binding hardening, analyzer noncanonical-path checks, probe process identity checks, malformed AutoSlay retained-path rejection, direct-smoke analyzer hardening, and live-session restore-state hardening; runtime preflight 27/0 for local `v0.107.1`; retained AdditiveBatch1 packet verifier 62/0; static suite 15/0; static-file hygiene 12/0. |
| Batch 4a/4b migration | Source-level complete | Current patch inventory records 25 migrated `IPatchMethod` patch classes, 146 remaining raw `HarmonyPatch` declarations, and 171 tracked patch units total. |
| Batch 4c migration | Proposal only / static review recaptured | 2026-06-18 recapture confirmed 10 low-risk candidates, no forbidden high-risk categories, and no migration performed. Owner approval is still required before any migration work. |
| Mod Settings UI scaffold | Historical template / live pending | No-launch scaffold at `.tools/runtime-evidence/mod-settings-current-display-20260618-223145/` was captured for package `v0.1.0-private-beta.87`; treat it as a template only and refresh it for beta.91 before using it as current UI evidence. This is not screenshot, log/audit, or gameplay proof. |
| Manual-test handoff scaffold | Historical template / live pending | No-launch handoff scaffold refreshed at `.tools/runtime-evidence/manual-test-handoff-20260619-120202/` on pushed HEAD `2400ec4b`; generated `handoff-summary.json` recorded 21 required live rows, 21 expected pending failures, 0 warnings, package ZIP `D547847874919EE923E2281A495D5389BAB22BBDB9F1090DC57B77033668A36D`, and git handoff metadata with `GitHeadMatchesUpstream=true`. This is a beta.88-era template scaffold only; current beta.91 handoff must use the package hashes in `docs/private-beta-verification-handoff.md` / `docs/release-evidence-status.md` and recapture HEAD/worktree status. No game was launched. |
| Manual proof | Pending | Gameplay, clicked UI, save-load, image rendering, replacement behavior, co-op/fail-closed behavior, independent QA, and tester handoff are still open. |

Current beta.85/beta.86 loader proof remains previous-package/game-version context, retained beta.87 AdditiveBatch1 loader/registration proof exists under `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/`, beta.88 `v0.107.1` clean-loader proof exists under `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/` as previous BaseLib-backed package context, and beta.90 RitsuLib-only proof is previous package context. Current beta.91 RitsuLib-only AdditiveBatch1 proof is `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/`. The BaseLib `v3.2.1` patch failures in `.tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/` are root-cause history.

## Boundaries

- Do not claim private beta, live gameplay, or release readiness from loader/registration proof.
- Do not use `AllDraft` or `Replacement` as a tester/release path without owner approval and fresh targeted validation.
- Do not perform Batch 4c migration without explicit owner approval.
- Do not implement Ascension 21-30 or custom character work.
- Do not bump compile package, manifest minimums, or dependency minimums again unless an owner-approved package pass requires it.
- Prefer RitsuLib, local game command APIs, and template-supported APIs; keep Harmony patches narrow.
- Treat the previous `v0.106.1` Off/CanaryOnly/AdditiveBatch1 smokes as historical loader evidence only.

## Next Actions

1. Refresh Mod Settings UI proof for the current Spire Plus display-name package; the current no-launch scaffold is prepared at `.tools/runtime-evidence/mod-settings-current-display-20260618-223145/`, and it must be rerun with `-Capture List` / `-Capture Page` only after manually opening the relevant UI.
2. Use `.tools/runtime-evidence/manual-test-handoff-20260619-120202/TESTER_START_HERE.md` only as a historical live-row template, or regenerate a beta.91 no-launch scaffold before filling rows with live files for any pass claim.
3. Capture Canary gameplay proof for Big Fish, Golden Idol, The Lab, and Divine Fountain only after recapturing any required current-version CanaryOnly loader packet or explicitly accepting the retained previous-package context for that narrow purpose.
4. Capture AdditiveBatch1 gameplay proof; beta.91 loader/registration proof is clean and package-matched, but event gameplay is still unproven.
5. Capture save-load and image/render proof for event and replacement surfaces.
6. Verify multiplayer fail-closed behavior and any owner-approved two-client diagnostics.
7. Record an owner decision for Batch 4c. The candidate list has static-review coverage; do not migrate unless the owner approves the scope.
8. Recapture git status, pushed HEAD, and validation status before any later handoff.

## Validation Snapshot

Current beta.91 RitsuLib-only validation commands and dependency/source checks:

```text
git log -1 --oneline --decorate
git status --short --branch
dotnet list EZMicroBalance.csproj package --include-transitive
dotnet list EZMicroBalance.csproj package --outdated --include-transitive
$hits = git grep -n "BaseLib" -- EZMicroBalanceCode EZMicroBalance.csproj EZMicroBalance.json; if ($LASTEXITCODE -eq 0) { $hits; exit 1 } elseif ($LASTEXITCODE -eq 1) { 'No BaseLib references in active code/project/manifest.'; exit 0 } else { exit $LASTEXITCODE }
scripts/check-local-godot-source-workspace.ps1 -SourceRoot 'source code' -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2' -ExpectedGameVersion 'v0.107.1' -ExpectedRitsuLibVersion '0.4.28' -ExpectedRitsuCompatBranch '0.107.1' -RequireCurrentSourceSnapshot -FailOnMismatch
dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false
scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~ReleaseSafetyExpandedGuardTests" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

Current beta.91 package/runtime refresh commands retained from the versioned package pass:

```text
dotnet publish EZMicroBalance.sln -m:1
scripts/package-spire-plus.ps1 -GameRoot 'E:\Steam\steamapps\common\Slay the Spire 2'
scripts/check-installed-spire-plus-package.ps1 -ModDirectory 'E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance'
scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch
scripts/check-sts1-enabled-mode-runtime-log.ps1 -Mode AdditiveBatch1 -LogPath .tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/godot.log.current-iteration -AuditPath .tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/godot-log-audit.json -ExpectedPackageVersion v0.1.0-private-beta.91 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.28 -ExpectedGameVersion 0.107.1 -OutFile .tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/sts1-enabled-mode-report.json -FailOnMismatch
scripts/check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir .tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620 -LogFileName godot.log.current-iteration -ExpectedPackageVersion v0.1.0-private-beta.91 -ExpectedRitsuCompatBranch 0.107.1 -ExpectedRitsuLibVersion 0.4.28 -ExpectedGameVersion 0.107.1 -OutFile .tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/runtime-evidence-packet-check.json -FailOnMismatch
```

Historical beta.87 migration pass and follow-up no-game recapture commands:

```text
scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch
scripts/check-sts1-event-static-suite.ps1
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~RuntimeFailureAnalyzer|FullyQualifiedName~RuntimeMonkeyPacketChecker|FullyQualifiedName~GameNativeAutoSlayPacketVerifier|FullyQualifiedName~RuntimeMonkeyDocs" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~LothaLocalizationHoverAndRichTextAreReadable" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
scripts/generate-patch-inventory.ps1 -Check
scripts/report-worktree-batches.ps1 -FailOnUnclassified
```

Additional June 19 `v0.107.1` no-launch drift and handoff-scaffold follow-up:

```text
scripts/prepare-current-manual-test-handoff.ps1
scripts/check-installed-spire-plus-package.ps1 -ModDirectory 'E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance'
scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch
dotnet build EZMicroBalance.sln -m:1 --no-incremental -p:UseSharedCompilation=false
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~MultiplayerVersionMismatchDiagnosticsExposeModelHashHandshakeWithoutBypass" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~Sts1RuntimeEvidencePacketVerifier" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --filter "FullyQualifiedName~SimplifiedChineseLocalizationContainsNo" --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch
scripts/check-sts1-event-static-suite.ps1
scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch
scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch
scripts/check-sts1-v20-final-gate-overlay.ps1 -FailOnMismatch
scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch
```

Current-version failed loader recapture attempt, retained as root-cause history:

```text
scripts/check-installed-spire-plus-package.ps1 -ModDirectory 'E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance'
scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch
direct AdditiveBatch1 launch with temporary steam_appid.txt and SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1 -> .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/
scripts/audit-godot-log.ps1 -Path .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/godot.log.current-iteration -OutFile .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/godot-log-audit.json -FailOnHit
scripts/check-sts1-enabled-mode-runtime-log.ps1 -Mode AdditiveBatch1 -LogPath .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/godot.log.current-iteration -AuditPath .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/godot-log-audit.json -ExpectedPackageVersion v0.1.0-private-beta.87 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.24 -ExpectedGameVersion 0.107.1 -OutFile .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/enabled-mode-log-check.json -FailOnMismatch
scripts/check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309 -ExpectedPackageVersion v0.1.0-private-beta.87 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.24 -ExpectedGameVersion 0.107.1 -OutFile .tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/runtime-evidence-packet-check.json -FailOnMismatch
```

That attempt is a blocker packet, not passing proof: main menu and StS1 registration shape passed, but the clean-audit gate failed on BaseLib patch failures.

Pause-safe analyzer follow-up: `analyze-spire-plus-runtime-failure.ps1` now treats this failed direct smoke evidence root as a `DirectSmoke` target via `direct-smoke-summary.json`, `godot.log.current-iteration`, and `godot-log-audit.json`. The retained `.tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/` packet now triages as `PackageRuntimeDrift` with 1 analyzed target, 2 package blockers, 0 harness blockers, and 0 gameplay blockers; `BaseLibPatchFailures` pinpoints `AdjustCustomMessageKeys::Fuckery()` as an undefined target-method failure, `NRelicCollectionCategory::LoadRelics` as an instruction matcher failure, and the 241-applied / 2-failed BaseLib patch summary; the analyzer no longer turns explanatory `SPIREPLUS_ALLOW_UNVERIFIED_COOP_*` startup text into a false co-op override blocker.

Current-version clean loader recapture:

```text
direct AdditiveBatch1 clean-log launch with temporary steam_appid.txt and SPIREPLUS_STS1_EVENT_MODE=AdditiveBatch1 -> .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/
scripts/check-sts1-enabled-mode-runtime-log.ps1 -Mode AdditiveBatch1 -LogPath .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/godot.log.current-iteration -AuditPath .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/godot-log-audit.json -ExpectedPackageVersion v0.1.0-private-beta.88 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.24 -ExpectedGameVersion 0.107.1 -OutFile .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/sts1-enabled-mode-report.json -FailOnMismatch
scripts/check-sts1-runtime-evidence-packet.ps1 -Mode AdditiveBatch1 -EvidenceDir .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937 -LogFileName godot.log.current-iteration -ExpectedPackageVersion v0.1.0-private-beta.88 -ExpectedRitsuCompatBranch 0.107.0 -ExpectedRitsuLibVersion 0.4.24 -ExpectedGameVersion 0.107.1 -OutFile .tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/sts1-runtime-evidence-packet.json -FailOnMismatch
```

The direct beta.88 AdditiveBatch1 smoke is loader/registration proof only. It does not close
gameplay, UI, save-load, co-op, QA, release, or handoff gates.
