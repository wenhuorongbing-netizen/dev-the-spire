# beta.135 Runtime Baseline Capture

This lane prepares the first beta.135 runtime baseline evidence packet without
changing gameplay code. It is an evidence-only lane. It does not prove release,
gameplay, save-load, or co-op readiness.

## Scope

Target:

- Spire Plus `v0.1.0-private-beta.135`
- Slay the Spire 2 `0.107.1`
- STS2-RitsuLib `0.4.34`
- Ritsu compatibility branch `0.107.1`
- Expected runtime patch count `168`
- Current Git HEAD and worktree status from the scaffolded repository

Patch-count boundary: current source inventory is 169 migrated RitsuLib patch
classes, but the default beta.135 DLL compiles 168 runtime patch classes because
`Sts1ReplacementPrototype` is behind `REPLACEMENT_PROTOTYPE_ENABLED`. The
runtime baseline must check the default compiled/applied count, not the source
inventory count.

Allowed files are `scripts/**`, `tests/**`, `docs/testing/**`,
`.tools/runtime-evidence/**`, and debug status or validation notes. Do not edit
product logic, `Sts1Events/`, locked source areas, project files, or manifests
for this lane.

## Owner-Run Boundary

Codex prepares the evidence scaffold and offline log checker. The owner starts
the real game through Steam and captures the actual runtime log. Until that log
exists, `startup`, `main-menu`, `screenshot`, and runtime patch application are
`pending-owner-run`.

Do not fabricate:

- `godot.log.after-launch`
- `godot-log-audit.json`
- main-menu screenshot
- runtime patch PASS

## Prepare The Evidence Directory

Run this no-launch command:

```powershell
.\scripts\new-beta135-runtime-baseline-evidence.ps1
```

If `-GameRoot` is omitted, the scaffold and checker both read
`Directory.Build.props` `Sts2Path`; if neither is available, they fail closed
and ask for `-GameRoot`.
For production baseline evidence, use the configured `Directory.Build.props`
game root and the repo default `publish/SpirePlus-v0.1.0-private-beta.135.zip`.
Explicit `-GameRoot` or `-PackageZipPath` overrides that do not match those
canonical locations are allowed only for checker fixtures and are recorded as
`TrustAnchorMode = noncanonical-override-test-only`; they must not be described
as canonical beta.135 package/install proof.

It creates:

```text
.tools/runtime-evidence/beta135-runtime-baseline-YYYYMMDD-HHMMSS/
  command.txt
  preflight.json
  run-manifest.json
  runtime-baseline-notes.md
```

`preflight.json` and `run-manifest.json` record the current Git HEAD,
decorated commit line, branch/upstream, `git status --short --branch`, and dirty
worktree status. That source binding is part of the evidence packet; a later
checker run must fail closed if the manifest no longer matches the current
repository HEAD or worktree status. The checker may record `LastCheckedGit` and
`CurrentGit` for diagnostics, but it must not overwrite the scaffolded `Git`
trust anchor in `run-manifest.json`.
When the owner supplies a log through the scaffold command and the checker
succeeds, the scaffold must preserve the checker-validated package, install, and
Git trust anchors instead of recomputing them after validation.

If the owner already has a captured log, pass it without launching the game:

```powershell
.\scripts\new-beta135-runtime-baseline-evidence.ps1 `
  -GodotLogAfterLaunchPath "<owner-captured>\godot.log.after-launch" `
  -ScreenshotPath "<owner-captured>\main-menu-screenshot.png"
```

## Turnkey Owner Capture Runbook

This is the one-stop owner sequence: a minimal `StartupOnly` smoke captures the
runtime log, then the evidence packet assembles and verifies automatically. Only
the smoke step (Step 2) launches the game; it is `pending-owner-run`. Everything
else is no-launch.

Prerequisites, already confirmed by this lane's no-launch preflight on
2026-06-26 (re-run if anything changed):

- `Directory.Build.props` `Sts2Path` is set to the installed game root
  (`E:\Steam\steamapps\common\Slay the Spire 2`).
- Package `v0.1.0-private-beta.135` is packaged
  (`publish\SpirePlus-v0.1.0-private-beta.135.zip`) and installed under
  `<GameRoot>\mods\EZMicroBalance` with matching DLL/JSON/PCK/README hashes.
- Versions match: game `0.107.1`, RitsuLib `0.4.34` (direct-NuGet layout),
  package `v0.1.0-private-beta.135`; STS2-RitsuLib present under
  `<GameRoot>\mods\STS2-RitsuLib`.

Re-confirm any time with these no-launch checks (both must exit 0):

```powershell
.\scripts\check-sts1-runtime-preflight.ps1 -FailOnMismatch
.\scripts\check-installed-spire-plus-package.ps1 `
  -ModDirectory "E:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance" `
  -GameRoot "E:\Steam\steamapps\common\Slay the Spire 2" `
  -ExpectedPackageVersion v0.1.0-private-beta.135
```

### Step 1 — (Optional) preview the empty scaffold (no launch)

This step is optional and only previews what the packet will contain. Skip it if
you want the shortest path; Step 3 creates the real evidence directory in one go.

```powershell
.\scripts\new-beta135-runtime-baseline-evidence.ps1
```

It prints `runtime_baseline_scaffold status=pending-owner-run evidence_dir=<DIR>`
and writes `command.txt` / `preflight.json` / `run-manifest.json` /
`runtime-baseline-notes.md` into a fresh timestamped `<DIR>`. Do NOT later point
Step 3's `-GodotLogAfterLaunchPath` at this same `<DIR>`: the assembler refuses to
reuse a non-empty evidence directory (it fails closed rather than overwrite). For
the real packet, let Step 3 create its own directory.

### Step 2 — Capture the runtime log (OWNER-RUN: launches the game)

> `pending-owner-run`. This is the only step that starts Slay the Spire 2. Run it
> yourself; the dev lane must not launch the game.

`StartupOnly` sends no DevConsole commands; it launches with only STS2-RitsuLib
and Spire Plus enabled, waits for the main menu, watches for startup hangs / log
stalls / unresponsive window, then restores the session. Isolate the game's
shared `godot.log` first by closing any running Slay the Spire 2 instance.

```powershell
.\scripts\run-spire-plus-monkey-stability.ps1 `
  -Iterations 1 `
  -Scenario StartupOnly `
  -NoDevConsoleCommands `
  -Launch `
  -MoveOtherMods `
  -MoveCurrentRuns `
  -MainMenuTimeoutSeconds 240 `
  -NoLogGrowthTimeoutSeconds 120 `
  -ObservationIntervalSeconds 2 `
  -ExpectedPackageVersion v0.1.0-private-beta.135 `
  -ExpectedGameVersion 0.107.1 `
  -ExpectedRitsuLibVersion 0.4.34 `
  -ExpectedRitsuCompatBranch 0.107.1
```

Without `-Launch` the runner is a dry run (it prints
`Status: planned … Re-run with -Launch to start Steam sessions.` and exits without
touching Steam) — use that to preview the plan safely.

The launched run writes its own evidence root under `.tools\runtime-evidence\`
(`monkey-stability-YYYYMMDD-HHMMSS\iteration-0001\`). The log to feed forward is
that iteration's full post-launch log:

```text
.tools\runtime-evidence\monkey-stability-YYYYMMDD-HHMMSS\iteration-0001\godot.log.after-launch
```

Call it `<CAPTURED-LOG>`. If you also grab a main-menu screenshot, call it
`<CAPTURED-SHOT>` (PNG). A screenshot is optional; the baseline can pass on log
markers alone.

> Patch-count note: the `StartupOnly` smoke is only the LOG SOURCE here. Do not
> reuse the runtime-monkey packet checker's historical `-ExpectedPatchCount`
> values (`25` runner default, `144`, or `152` from beta.128) for this baseline.
> The beta.135 baseline lane uses `-ExpectedPatchCount 168` (Step 4), which the
> checker self-test confirms (`expected=168 applied=168 total=168`). If you later
> also want a monkey packet, derive its `-ExpectedPatchCount` fresh from the
> captured `ModPatcher applied N patches (N registered)` log line.

### Step 3 — Assemble + verify the evidence packet (no launch)

Run the assembler ONCE with the captured log (and optional screenshot). With no
`-EvidenceDir`, it creates a fresh timestamped `.tools\runtime-evidence\beta135-runtime-baseline-*`
directory, copies the artifacts into it, then — because a log was provided —
automatically runs `check-beta135-runtime-baseline-log.ps1` (with
`-ExpectedPatchCount 168` and the configured game root/zip) and refreshes
`run-manifest.json`:

```powershell
.\scripts\new-beta135-runtime-baseline-evidence.ps1 `
  -GodotLogAfterLaunchPath "<CAPTURED-LOG>" `
  -ScreenshotPath "<CAPTURED-SHOT>"   # omit this line if no screenshot
```

On success it prints
`runtime_baseline_scaffold status=runtime-baseline-log-markers-checked evidence_dir=<EVIDENCE-DIR>`.
Copy that reported `<EVIDENCE-DIR>` for the optional re-check below. If the log
markers do not pass, the assembler surfaces the checker failure and the status is
not `runtime-baseline-log-markers-checked`.

### Step 4 — (Optional) re-verify (no launch; idempotent)

Step 3 already ran the checker. To re-verify the assembled packet at any later
time, point `-EvidenceDir` at the directory Step 3 reported:

```powershell
.\scripts\check-beta135-runtime-baseline-log.ps1 `
  -EvidenceDir "<EVIDENCE-DIR>" `
  -LogPath "<EVIDENCE-DIR>\godot.log.after-launch" `
  -ExpectedPackageVersion v0.1.0-private-beta.135 `
  -ExpectedGameVersion 0.107.1 `
  -ExpectedRitsuLibVersion 0.4.34 `
  -ExpectedRitsuCompatBranch 0.107.1 `
  -ExpectedPatchCount 168 `
  -GameRoot "E:\Steam\steamapps\common\Slay the Spire 2" `
  -FailOnMismatch
```

`status=pass` with all `check … status=pass` rows means the beta.135 runtime
baseline markers are present. This remains marker-only evidence
(`LogOriginProofStatus = marker-only-origin-not-proven-by-offline-checker`) and a
production baseline claim still requires `TrustAnchorMode = canonical-configured`
(default game root + repo `publish\` zip). It does NOT prove release, gameplay,
save-load, or co-op readiness.

## Owner Smoke Command

The owner should:

1. Enable only STS2-RitsuLib and Spire Plus / `EZMicroBalance`.
2. Start Slay the Spire 2 through Steam.
3. Wait for the main menu.
4. Save the post-launch log as `godot.log.after-launch` in the evidence
   directory.
5. Save `main-menu-screenshot.png` if screenshot capture is stable.
6. Run:

```powershell
.\scripts\check-beta135-runtime-baseline-log.ps1 `
  -EvidenceDir "<evidence-dir>" `
  -LogPath "<evidence-dir>\godot.log.after-launch" `
  -ExpectedPackageVersion v0.1.0-private-beta.135 `
  -ExpectedGameVersion 0.107.1 `
  -ExpectedRitsuLibVersion 0.4.34 `
  -ExpectedRitsuCompatBranch 0.107.1 `
  -ExpectedPatchCount 168 `
  -GameRoot "<Steam>\steamapps\common\Slay the Spire 2" `
  -FailOnMismatch
```

The checker writes:

```text
godot-log-audit.json
runtime-baseline-log-check.json
run-manifest.json
```

Evidence directories must stay under `.tools/runtime-evidence/`, must not reuse
non-empty directories with `-Force`, and must not pass through reparse points or
use a reparse-point log/artifact leaf. Any copied owner log or screenshot source path
must also avoid reparse points across the full existing path chain, including
source directories outside the repository. A successful owner-log check records
the manifest status as `runtime-baseline-log-markers-checked`.

It checks version markers, the runtime log's discovered mod manifest id set
(`EZMicroBalance`, `STS2-RitsuLib`, and no extra ids), RitsuLib/Spire Plus load
markers, absence of retired BaseLib markers, main-menu marker, runtime patch
summary, retained audit cleanliness, native audit array/bool/int shape, and
release-blocking signatures such as
`MissingMethodException`, `TypeLoadException`, `TargetInvocationException`,
loader failure, initializer exception, and release-blocking `ERROR` lines.
The retained audit must use schema v2, keep a non-empty named signature vector,
keep the current complete audit signature name set, and bind `Path`, `Length`, and `Sha256` to the same `godot.log.after-launch` that the checker scanned.
The checker refuses direct log-only evidence: non-self-test runs must have the
scaffold-created `run-manifest.json`, `OwnerRunRequired`, `DoesNotLaunchGame`,
the expected package/game/Ritsu/patch targets, the scaffolded Git HEAD/worktree
binding, the beta.135 package zip hash record, and hash-bound installed-file records for `EZMicroBalance.dll`,
`EZMicroBalance.json`, `EZMicroBalance.pck`, `STS2-RitsuLib.dll`, and
`STS2-RitsuLib.xml`. The checker recomputes the current package zip and required
installed-file path, length, and SHA256 values, then compares them with the
manifest records. It also recomputes the current Git source binding and compares
it with the scaffolded manifest without repairing the scaffolded `Git` record;
stale HEADs, stale dirty-state claims, or stale status lines fail
`scaffold_manifest_git_head_bound` on every run until a fresh scaffold is
captured. Installed-file checks are rooted in `-GameRoot`, or in
`Directory.Build.props` `Sts2Path` when `-GameRoot` is omitted, not in
manifest-provided install directories; fabricated manifest records with plausible
hash-shaped strings must fail.
The checker also binds `GameRootAnchorMode`, `PackageAnchorMode`, and
`TrustAnchorMode` so synthetic override roots are visible as test-only rather
than silently becoming canonical package/install anchors.
The checker also refreshes `run-manifest.json` with the retained log, audit, and
checker-report hashes after those scaffold/package binding checks run. This is
still marker-only evidence; the offline checker records
`LogOriginProofStatus = marker-only-origin-not-proven-by-offline-checker` and
must not be treated as proof that the log originated from a real Steam session.
`verify-spire-plus-release-evidence.ps1` explicitly rejects release required
rows that point at this marker-only baseline manifest or copy its marker-only
or no-launch owner-run boundary fields. Release pass rows must keep the positive
`EvidenceBoundary = live-release-row-required` provenance marker, must set
`LogOriginProofStatus = owner-live-release-log`, and must include a filled
`log-origin-note.md` with `LogOriginProofStatus: owner-live-release-log`, a
non-empty `Source:` line, and a non-empty `Log files:` line for that row's own
live release session. `Source:` and `Log files:` must not be placeholders such
as `TBD`, `TODO`, `unknown`, `none`, `N/A`, `pending`, or `-`. The note must not
reference beta.135 runtime baseline artifacts, marker-only checks, no-launch
owner-run scaffolds, or `godot.log.after-launch`. Renaming a baseline
`godot.log.after-launch` to a release row `godot.log` without that owner/live
origin proof must fail even when `run-manifest.json` and baseline sentinel files
are absent. The verifier also
rejects release rows that retain baseline scaffold sentinel files such as
`runtime-baseline-log-check.json`, `runtime-baseline-notes.md`,
`godot.log.after-launch`, or `preflight.json`, even when `run-manifest.json` is
missing. If a `run-manifest.json` is present, it must have both
`EvidenceKind = release-evidence` and
`EvidenceBoundary = live-release-row-required` provenance and must not contain
beta.135 baseline-specific record fields or paths such as
`GodotLogAfterLaunchRecord`, `RuntimeBaselineLogCheckRecord`,
`runtime-baseline-log-check.json`, or `godot.log.after-launch`.

## Static Self-Test

The log checker has a no-launch self-test using an embedded sample log:

```powershell
.\scripts\check-beta135-runtime-baseline-log.ps1 -SelfTest -FailOnMismatch
```

This only proves checker logic on a sample log. It does not prove beta.135
runtime behavior.

## Exit State

Before the owner-run log exists, the correct status is `pending-owner-run`.
After the owner supplies the log, the evidence can only claim runtime baseline
startup/main-menu markers if all checker rows pass and the retained audit is
clean, and only as production beta.135 baseline evidence when
`TrustAnchorMode = canonical-configured`. A packet with
`TrustAnchorMode = noncanonical-override-test-only` is fixture/checker evidence
only even when `BaselineLogCheckStatus = pass`. It still must not claim release,
gameplay, save-load, co-op readiness, or offline proof of log origin.
