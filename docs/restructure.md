# Restructure Boundary

Status: current planning boundary for documentation/source cleanup. This file is
not a release-readiness claim and must not override `PROJECT_STATE.md`,
`docs/test-ready-development-goal.md`, or current command output.

## Current Target

Current package/runtime target is Spire Plus `v0.1.0-private-beta.119` on Slay
the Spire 2 `v0.107.1` with `STS2-RitsuLib` `0.4.34` in direct NuGet runtime layout
runtime variant. `EZMicroBalance.csproj`, `EZMicroBalance.json`, package
contents, and current runtime proof must remain RitsuLib-only unless the owner
explicitly approves a new dependency decision. The previous package is previous-package or other-mod local context only, not a current Spire Plus dependency.

External version checks are time-sensitive. As of the 2026-06-22 cleanup pass,
the current public evidence to recheck is:

- RitsuLib releases and manifest:
  `https://github.com/BAKAOLC/STS2-RitsuLib/releases` and
  `https://github.com/BAKAOLC/STS2-RitsuLib/blob/main/mod_manifest.json`.
- Slay the Spire 2 main-branch patch:
  `https://steamdb.info/patchnotes/23811903/`.

## Evidence Rules

- Read `PROJECT_STATE.md` first, then `docs/README.md`,
  `docs/test-ready-development-goal.md`, `docs/issues.md`, and
  `docs/review.md`.
- Treat local `source code/` as ignored source evidence only after
  `scripts\check-local-godot-source-workspace.ps1 -RequireCurrentSourceSnapshot`
  passes with the current package, game, RitsuLib, and compat-branch targets.
- Do not use historical `v0.106.1`, `v0.107.0`, previous package beta.88, or
  beta.90 rows as current runtime proof.
- Previous beta.93 Off/AdditiveBatch1 logs prove loader, patch application, and
  registration shape only. Gameplay, clicked UI, save-load, replacement,
  multiplayer, QA, and tester handoff still require their own evidence.
- Use `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md` for current StS1 event guidance; beta.93 proves only previous-package RitsuLib-only `v0.107.1` Off and AdditiveBatch1 loader/registration behavior, beta.85/beta.87/beta.88/beta.90 rows remain previous-context evidence, and CanaryOnly gameplay/runtime, save-load, replacement, multiplayer, QA, handoff, and release-ready proof remain pending or blocked.

## Cleanup Boundaries

- Keep the stable technical id, project, resource folder, code folder, install
  folder, DLL, PCK, and saved-field namespace as `EZMicroBalance`.
- Use `Spire Plus` for player-facing docs, tester instructions, website copy,
  and release notes.
- Do not copy uploaded ZIP/DLL/PCK packages, downloaded dependencies, local
  tool binaries, `.tools/`, `.godot/`, `source code/`, `bin/`, `obj/`, or
  `publish/` outputs into tracked release source.
- Do not wholesale delete ignored files. Use targeted cleanup scripts and keep
  runtime evidence, art provenance, GDRE/Godot/ILSpy tooling, package output,
  and local source snapshots unless a focused cleanup proves they are stale.
- Archive useful historical planning or prompt material under `docs/archive/`
  instead of leaving prompt dumps in active reading paths.
- Keep active docs compact. If a document becomes an old packet, replace the
  active file with a short boundary stub and point to the archive.

## Refactor Sequence

1. Docs-only cleanup and stale-claim guards.
2. Move-only source or test refactors with no behavior change.
3. Guard/test path updates after move-only changes.
4. Low-risk RitsuLib API adoption where it reduces current code complexity.
5. High-risk run/map/reward/save/multiplayer patch migration one feature at a
   time, only with current source evidence and validation.

Do not combine behavior changes, package version bumps, broad file moves, and
runtime dependency changes in one slice.

## Validation

Docs/test-guard cleanup should run the touched focused tests, current-doc
claims guard, static-file hygiene, source-workspace check when dependency or
source evidence is discussed, format verification, `git diff --check`, and
worktree batch classification. Code/config changes also run `dotnet build`.
Resource, localization, manifest, export, package, or tester-handoff changes
also run publish, package refresh, and the appropriate package/artifact guards.
