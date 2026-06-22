# Runtime Monkey Stability Lane

## Purpose

This lane turns the current manual runtime-smoke practice into a repeatable
long-run stability test for Spire Plus. It is meant to catch startup hangs,
black screens, runtime exceptions, bad restore behavior, and log-level drift
before a manual tester spends time on feature matrices.

This is not release proof by itself. A clean monkey lane can support bug
triage, but clicked UI, gameplay, save-load, co-op, and release handoff still
need their own evidence rows.

## Current Source And Tooling State

- Current installed game: `E:\Steam\steamapps\common\Slay the Spire 2`.
- Current runtime dependency: `STS2-RitsuLib v0.4.34` in direct NuGet runtime layout.
- Local Godot editor: `.tools\godot-4.5.1-mono\Godot_v4.5.1-stable_mono_win64`.
- Local recovered game project: `source code\project.godot`.
- Installed RitsuLib includes a local log viewer under
  `mods\STS2-RitsuLib\viewer\`; it is not an unpacker, game-project opener, or
  monkey-runner.

The recovered `source code/` tree is local scratch/reference material. It must
stay ignored, and original Slay the Spire 2 non-art source/resources must not be
copied into tracked files. Record only short signatures, paths, observed
version metadata, and conclusions in docs/tests.
Use only the owner-authorized local install as the recovery source; third-party
dumps or redistributed source/resource bundles are not valid evidence.

Current source snapshot: `source code\release_info.json` matches the installed
`v0.107.1` game identity after the 2026-06-20 GDRE refresh: commit `59260271`,
branch `v0.107.1`, and main assembly hash `-1555940892`. Re-run the source
workspace checker before using it as current-source evidence. If the source and
installed game identity diverge, refresh the local source snapshot first or
treat the recovered project as historical.

Audit the local source workspace without launching Godot or the game:

```powershell
.\scripts\check-local-godot-source-workspace.ps1 `
  -OutFile .tools\runtime-evidence\local-godot-source-workspace-current\workspace-check.json
```

When `-OutFile` is used, the checker emits a machine-readable
`OpenProjectCommand` and `EvidenceUsePolicy` in its JSON report. Treat that
command as local operator guidance only: it does not launch Godot, does not
prove runtime behavior, and does not permit tracked copies of original game
source or extracted resources. The same report also includes an `AutoSlay`
summary and no-launch checks for the recovered game-native simulator signatures:
`AutoSlayer.Start(seed, logFile)`, `NonInteractiveMode.AutoSlayerCheck`,
`NGame.Instance.DebugSeedOverride`, `AutoSlayCardSelector`, Ancient dialogue
handling, event-option selection logging, and event-combat logging. These are
source-contract checks only; game-native monkey proof still requires a launched
AutoSlay-backed packet.

Use `-RequireCurrentSourceSnapshot -FailOnMismatch` when a task requires
current-source parity. The previous beta.93 local state is expected to pass
`source_version_matches_installed_game`, `source_commit_matches_installed_game`,
`source_branch_matches_installed_game`,
`source_main_assembly_hash_matches_installed_game`, and
`source_release_identity_matches_installed_game`. If any of those checks warn or
fail, the recovered project is stale for current-source claims. The checker also
compares the GDRE `Opening file` line to the installed `SlayTheSpire2.pck` path
and reports
`GdreExport.OpeningFileMatchesInstalledGame` plus
`RecoveredSource.OriginMatchesInstalledGamePck`. The report also retains
RitsuLib manifest, variants, selected variant DLL, and compat-target paths and
hashes so runtime packets can bind the local Ritsu state they used. The current
retained GDRE warnings are 18 failed scripts and one parse warning; they are
recovery-quality warnings, not source version, commit, branch, hash, or origin
mismatches. Use `-RequireCleanGdreExport` only when GDRE recovery warnings must
be release-blocking for the task.

## Godot Project Opening

Use the recovered project only locally:

```powershell
& ".\.tools\godot-4.5.1-mono\Godot_v4.5.1-stable_mono_win64\Godot_v4.5.1-stable_mono_win64.exe" --editor ".\source code\project.godot"
```

Do not save generated editor artifacts into tracked paths. Keep `.godot/`,
`source code/`, extracted PCK contents, and downloaded binaries ignored.

## Monkey Lane Shape

The first implementation layer is deliberately conservative:

1. Create a timestamped evidence root under `.tools\runtime-evidence`.
2. Write a deterministic command corpus and `monkey-plan.json`.
   The plan must retain the expected package, game, RitsuLib, compat branch,
   and positive `ExpectedPatchCount` values. The packet checker uses
   `-ExpectedPatchCount` when supplied, otherwise it uses the retained
   `monkey-plan.json` value; either way the current-iteration log must contain
   matching Spire Plus patch-count lines. The plan must also retain
    `RunnerScriptPath` and `RunnerScriptSha256` for
    `scripts\run-spire-plus-monkey-stability.ps1`; the packet checker rejects a
    packet whose retained runner path or hash does not match the current repo
    runner. The plan also retains `CommandCorpusPath` and `CommandCorpusSha256`;
    `command-corpus.txt` must stay under the evidence root and its lines must
    exactly match `monkey-plan.json` `CommandCorpus`. Each iteration must also
    retain `iteration-000N\command.txt` as the single command line used for that
    run. `iteration-result.json` records `CommandFilePath` and
    `CommandFileSha256`; the packet checker binds that path/hash to the
    retained `command.txt` and requires the command text to match
    `monkey-plan.json`, `iteration-result.json`, and `monkey-summary.json`.
    The summary `Results[]` row must retain the same command-file path and
    hash as `iteration-result.json`.
3. For each launched iteration, call `scripts\spire-plus-live-session.ps1` in
   prepare mode with explicit mod isolation and current-run isolation, retaining
   its stdout as `prepare-output.json`.
   `LiveSessionLaunchedProcessId` is launcher evidence from Steam
   `-applaunch`, not a claim that Steam's process id is the game process id.
   The hard game identity is the live-session-selected `SlayTheSpire2` process
   id, start timestamp, and executable path.
4. Wait for `[Startup] Time to main menu` in `godot.log`.
5. During startup, sample the `SlayTheSpire2` process, main-window
   responsiveness, and `godot.log` length/write time. The runner records the
   pre-launch log length and only accepts a main-menu marker from appended or
   reset log content. Before that baseline is captured or the game is launched,
   startup fails if any `SlayTheSpire2` process is already running, because the
   shared `godot.log` can no longer be attributed to the launched process.
   Runtime probe samples must match the live-session-selected game process
   identity by process id, start timestamp, and path; a different process is
   treated as harness contamination rather than gameplay evidence.
   Runtime probe sample timestamps must be parseable, and retained log
   last-write timestamps must be parseable and not later than the sample time
   whenever the log exists. The retained sample array must also be chronological:
   `SampledAt` values cannot go backward, startup/main-menu samples cannot be
   retained after runtime samples, and `LogLengthBytes` must be non-negative and
   nondecreasing whenever `LogExists` is true. `iteration-result.json` must also
   retain `RuntimeProbeSamplesSha256`, and the packet checker recomputes it from
   the retained `runtime-probe-samples.json`. Malformed numeric or boolean
   values in nested observation or probe fields, including sample counts,
   stale-process counts, log lengths, expected process ids, pass flags, process
   health flags, and window responsiveness flags, are packet mismatches rather
   than verifier crashes, string-to-true coercions, or `null`-to-zero/false
   passes.
   Startup also fails if the current process disappears, the window reports
   hung/not responding for
   `-UnresponsiveSampleThreshold` consecutive samples, or the log stops growing
   before main menu for `-NoLogGrowthTimeoutSeconds`.
6. Optionally send one short DevConsole command from the corpus.
7. During the post-command window, sample process/window/log health again.
   Process disappearance or a hung/not-responding window fails the iteration;
   post-main-menu log growth is recorded as telemetry but not required because
   an idle main menu can legitimately stop writing.
8. Before launch, retain the shared log as `godot.log.before` in the iteration
   folder. If no pre-launch log exists, retain an explicit zero-byte file.
9. After observation, copy the full shared log to `godot.log.after-launch`, then
   write `godot.log.current-iteration` as the exact byte slice after
   `godot.log.before`. The retained `LogScanOffsetBytes` must equal the retained
   before-log byte length; log reset/truncation is invalid packet evidence unless
   the before snapshot is genuinely zero bytes.
10. Run `scripts\audit-godot-log.ps1` on `godot.log.current-iteration` and
   retain `godot-log-audit.json`. If the current-iteration slice cannot be
   written, do not write the canonical audit from the full log; any full-log
   diagnostic audit must use `godot-log-after-launch-audit.json`.
11. Run `scripts\check-sts1-enabled-mode-runtime-log.ps1` for the requested
   Off/CanaryOnly/AdditiveBatch1 mode and retain `sts1-mode-log-check.json`.
   This uses `godot.log.current-iteration` as truth, not the evidence folder
   name or older log content. Malformed numeric length/count values and boolean
   pass/clean flags in retained audit or StS1 verifier JSON must fail the packet
   check without aborting it.
12. Restore the live session with `-StopGameOnRestore` and
   `-PreserveNewCurrentRunsOnRestore`, retaining `session-state.json` and
   `restore-state.json` inside the same `iteration-####` directory. A clean
   packet must bind both files from `iteration-result.json` by canonical
   path and SHA256, prove restore schema version 1, prove the selected game
   process was stopped, close restored mod/current-run counts against the
   moved lists from `session-state.json`, record any preserved new current-run
   manifest by path and SHA256 when its count is positive, require
   post-restore SlayTheSpire2 and Godot process counts and id arrays to be
   zero, and require restored settings hashes to match the retained
   pre-prepare hashes. `settings.save.backup` may be absent before prepare, but
   that absence must be recorded in `session-state.json`, restored by deleting
   any test-created backup, and closed in `restore-state.json` with
   `SettingsBackupExistsAfterRestore = false`.
13. Write `iteration-result.json` and a root `monkey-summary.json`. A clean
    batch must use the exact `1..N` iteration set once in both
    `monkey-plan.json` `PlannedCommands[].Iteration` and `monkey-summary.json`
    `Results[].Iteration`; duplicate, missing, non-positive, or out-of-range
    iteration ids fail the packet check. The summary restore counters
    `LiveSessionRestoreItemCountMismatchCount`,
    `LiveSessionPreservedCurrentRunManifestMissingCount`,
    `LiveSessionRestoreLeakCount`, `LiveSessionRestoreHashMismatchCount`, and
    `LiveSessionSelectedProcessNotStoppedCount` must be zero for a clean packet
    and must match the corresponding `Results[].FailureReasonCodes`
    aggregation.

The lane fails an iteration on:

- main-menu timeout;
- `SlayTheSpire2` disappearing during startup or post-command observation;
- any pre-existing `SlayTheSpire2` process observed during startup or
  post-command observation;
- missing `prepare-output.json`, missing Steam launch metadata, failed selected
  game process attribution, or runtime probe PID/start/path mismatch against the
  live-session-selected game process;
- missing or hash-mismatched retained runtime probe samples;
- the game window reporting hung or not responding for the configured
  consecutive-sample threshold;
- `godot.log` not growing before main menu for the configured no-growth
  timeout;
- missing `godot.log.before`, or missing/empty `godot.log.after-launch` or
  `godot.log.current-iteration`; a zero-byte before snapshot is valid only when
  no pre-launch log existed;
- `audit-godot-log.ps1` release-blocking signature hits;
- package/game/RitsuLib/compat branch/patch-count expectation mismatch;
- actual StS1 mode verifier mismatch;
- DevConsole command failure when commands are enabled;
- missing, escaped, hash-mismatched, or malformed retained `session-state.json`
  / `restore-state.json`, restore item-count/hash mismatch, missing preserved
  current-run manifest binding, post-restore process leak, selected-game-process
  stop failure, or failed restore.

This catches hangs and bad startup/runtime logs without depending on a fragile
uncontrolled click bot. Later layers can add true random UI input after the
main-menu/start-run path is stable and after window-focus safeguards are proven.

## Game-Native AutoSlay Batch Lane

The recovered game source contains a separate built-in simulator under
`source code\src\Core\AutoSlay\AutoSlayer.cs`. Use only short signatures from
that ignored local source tree as planning evidence; do not copy original game
source or resources into tracked files. The relevant static signatures are:

- `AutoSlayer.Start(seed, logFile)` opens an optional AutoSlay log, marks the
  run active, and starts the async simulator.
- `NonInteractiveMode.AutoSlayerCheck = () => IsActive` marks the simulator as
  the game's non-interactive mode.
- `NGame.Instance.DebugSeedOverride = seed` and
  `CardSelectCmd.UseSelector(new AutoSlayCardSelector(_random))` make the run
  deterministic for a retained seed.
- `source code\src\Core\AutoSlay\Handlers\Rooms\EventRoomHandler.cs` includes
  event-room handling signatures such as `Detected Ancient event, clicking through dialogue`,
  `Selecting event option:`, `Event triggered combat`, and `Event combat started`.

Current `scripts\run-spire-plus-monkey-stability.ps1` lane is not
AutoSlay-backed. It launches through `scripts\spire-plus-live-session.ps1`,
waits for main menu, samples process/window/log health, and optionally sends
DevConsole commands. Do not count a packet from that lane as game-native
AutoSlay proof.

A future AutoSlay-backed batch packet must retain all of the following before it
can close a game-native monkey proof row:

- top-level `autoslay-plan.json` and `autoslay-summary.json` with
  `SchemaVersion: 1`, `RunnerKind: GameNativeAutoSlay`, and retained batch
  metadata before any per-run artifacts can be trusted. `autoslay-summary.json`
  top-level `RunnerKind`, `Sts1EventMode`, package/game/Ritsu targets,
  `ExpectedPatchCount`, and `ExpectedAncientIds` must match `autoslay-plan.json`.
  `autoslay-summary.json` `Passed` and `FailedRuns` must match the aggregation of `Runs[]` `Passed`,
  `FailureReasonCodes`, and `HangSignals`; top-level green fields are not
  accepted as proof unless the rows are also clean. All retained AutoSlay JSON
  boolean fields, including pass flags, source-workspace policy flags,
  observation health fields, runtime-probe process/window flags, audit `Clean`,
  and StS1 verifier flags, must be native JSON booleans. String, null, blank, or
  otherwise malformed boolean values are packet mismatches, not proof.
  `Seeds`, `ExpectedAncientIds`, `Runs`, `FailureReasonCodes`, and
  `HangSignals` must be native JSON arrays; scalar strings or nulls are
  malformed retained evidence, not empty signal sets;
- top-level `autoslay-summary.json` `AncientIdCounts` keyed by normalized
  Ancient id, with non-negative integer counts that exactly match the
  aggregation of per-run `Runs[].AncientId` values and give every requested
  target Ancient id a positive count; extra zero-count Ancient ids are not
  allowed because the retained count map must be the exact run aggregation;
- the exact launcher or mod hook that calls `AutoSlayer.Start(seed, logFile)`,
  retained as a hashed launcher/provenance artifact plus `LauncherKind`,
  `LauncherPath`, `LauncherSha256`, `HookId`, `HookAssembly`, and
  `InvocationCommand`;
- one `run-result.json` per seed with `SchemaVersion: 1`, `Launch: true`,
  `RunnerKind: GameNativeAutoSlay`, invocation text, process id, process start
  time, process path, parseable start/end timestamps where start is not later than end,
  `Passed: true`, empty `FailureReasonCodes` and
  `HangSignals`, exit code, stale-process count, `EventKind: Ancient`,
  `AncientId`, `RuntimeProbeSamplesPath`, clean `MainMenuObservation`,
  clean `RuntimeObservation` with `LogGrew: true` and
  `NoLogGrowthTimeoutExceeded: false`, and the retained per-run before,
  after-launch, and current-iteration Godot log paths, byte lengths, and
  SHA256 hashes;
- each `autoslay-summary.json` `Runs[]` row must retain `RunResultPath` and
  `RunResultSha256`, and the hash must match that seed's retained
  `run-result.json` bytes before the run-result data is trusted. `RunResultPath`
  must resolve exactly to the top-level per-seed path
  `run-####/run-result.json`; nested or shadow `run-####` directories are not
  proof packets;
- one retained `runtime-probe-samples.json` per seed with `Phase`, `SampledAt`,
  `LogExists`, `LogLengthBytes`, retained `LogLastWriteTimeUtc`, `ProcessId`,
  `ProcessStartTimeUtc`, `ProcessPath`, `ExpectedGameProcessId`,
  `ExpectedGameProcessStartTimeUtc`, `ExpectedGameProcessPath`,
  process-id/start/path match booleans, `ProcessIdentityMatchesExpected`,
  `ProcessObserved`,
  `MainWindowObserved`, `HungWindow`, `Responding`, and
  `StaleProcessCount`, `CurrentProcessCount`,
  `UnknownStartTimeProcessCount`, and `AmbiguousCurrentProcessCount` fields, at
  least one `main-menu` phase sample, at least one `runtime` phase sample,
  stable positive process id/start/path binding that matches the per-seed
  `run-result.json` `ProcessId`, `ProcessStartTimeUtc`, and `ProcessPath`,
  process and main-window observations, no hung-window samples, no
  `Responding=false` samples, `StaleProcessCount: 0`,
  `UnknownStartTimeProcessCount: 0`, and `AmbiguousCurrentProcessCount: 0` on
  every sample; `SampledAt` must be parseable, and `LogLastWriteTimeUtc` must
  be parseable and not later than `SampledAt` whenever `LogExists` is true; the
  retained sample array must be chronological, with nondecreasing `SampledAt`
  values, main-menu samples before runtime samples, and non-negative,
  nondecreasing `LogLengthBytes` values while the log exists; the file must
  resolve exactly to `run-####/runtime-probe-samples.json`, and
  `RuntimeProbeSamplesSha256` must be retained in both `run-result.json` and
  `autoslay-summary.json` and match the retained file;
- the seed, AutoSlay log path, exit code, Ancient id, ordered
  start/event/Ancient-dialogue/event-option/completion markers, with
  `AutoSlayLogSha256` bound to the retained log file; the sidecar log path must
  resolve exactly to `run-####/autoslay.log`;
- a retained `check-local-godot-source-workspace.ps1 -OutFile` report with
  schema version, creation time, repo/source/game roots, no-launch policy flags,
  passing AutoSlay source-contract checks, and
  `RecoveredSource.MatchesInstalledGame` and
  `RecoveredSource.OriginMatchesInstalledGamePck` true for the installed game
  under test, plus RitsuLib manifest/variants/selected-DLL hashes;
- the same package, game version, RitsuLib version, compat branch, and positive
  `ExpectedPatchCount` in `autoslay-plan.json` as the explicit verifier targets,
  plus `godot.log.before`, `godot.log.after-launch`,
  `godot.log.current-iteration`, `godot-log-audit.json`, and
  `sts1-mode-log-check.json` bindings required by the current runtime packet
  checker, including StS1 mode report recomputation from the retained current
  slice plus audit. Every standard per-seed artifact path must resolve exactly
  to the top-level `run-####/<standard-file>` path, including
  `godot.log.before`, `godot.log.after-launch`,
  `godot.log.current-iteration`, `godot-log-audit.json`, and
  `sts1-mode-log-check.json`;
- observed ordered event-room lines in both the AutoSlay sidecar log and the
  current Godot log slice proving Ancient dialogue/options were traversed, not
  only main-menu startup;
- a clear statement that the local recovered source snapshot matched the
  installed game version when the AutoSlay invocation path was derived.

After the packet is captured, verify it with:

```powershell
.\scripts\check-spire-plus-autoslay-packet.ps1 `
  -EvidenceDir "<evidence>" `
  -MinRuns 1000 `
  -ExpectedAncientIds VAKUU,URDA,MORVI,LOTHA `
  -ExpectedPackageVersion v0.1.0-private-beta.113 `
  -ExpectedGameVersion v0.107.1 `
  -ExpectedRitsuLibVersion 0.4.34 `
  -ExpectedRitsuCompatBranch 0.107.1 `
  -ExpectedPatchCount 64 `
  -OutFile "<evidence>\autoslay-packet-check.json" `
  -FailOnMismatch
```

Pass multiple expected Ancient ids as a comma-separated `-ExpectedAncientIds`
value; the verifier splits those tokens so process-launched test wrappers can
exercise the same target-coverage checks as an interactive PowerShell run.
Target coverage compares expected, plan, summary, and traversed Ancient ids
case-insensitively after normalizing them to uppercase, while per-run
`run-result.json` and `autoslay-summary.json` `AncientId` values must still
match each other exactly.
In `-FailOnMismatch` proof mode, `-ExpectedAncientIds` is required. A proof packet
must also retain the same target set in `autoslay-plan.json`
`ExpectedAncientIds` and `autoslay-summary.json` `ExpectedAncientIds`;
summary-only target coverage is not sufficient.
It must also retain a positive `autoslay-plan.json` `ExpectedPatchCount` that
matches `-ExpectedPatchCount`; the current-log patch marker check uses the
retained plan count, so a stale plan cannot be hidden by the verifier command.
`autoslay-summary.json` must retain `AncientIdCounts` whose normalized keys and
non-negative integer values match `Runs[].AncientId` aggregation exactly, whose
total equals the retained run count, and whose value for each requested target
Ancient id is greater than zero. Extra zero-count keys still fail because the
map must not claim an Ancient id that never appeared in `Runs[]`. The same
summary binding applies to batch status: `Passed` and `FailedRuns` must be
recomputed from `Runs[]` rather than trusted as standalone counters. Each requested
Ancient id must have sidecar and current-log traversal proof whose ordered
`Selecting event option: <AncientId>` line is bound to that same id after the
event-room and Ancient-dialogue markers, not merely present somewhere else in
the retained log slice.
Omitting the target set fails `expected_ancient_ids_required_for_proof_mode`.
Do not combine `-AllowMissingEventTraversal` with `-FailOnMismatch`; proof-mode
verification fails `allow_missing_event_traversal_not_proof_mode` so a parser
fixture bypass cannot be mistaken for game-native event traversal proof.

This verifier is no-launch only. It rejects packets that do not identify
`GameNativeAutoSlay`, do not record structured launcher/mod-hook provenance for
`AutoSlayer.Start(seed, logFile)`, do not bind the retained
`check-local-godot-source-workspace.ps1` report, schema fields, policy flags,
and source-version summary, omit the explicit package/game/Ritsu/patch target
switches, duplicate or drop planned seeds, place any per-seed artifact outside
that seed's top-level `run-####` folder or under a nested shadow `run-####`
folder, lack per-seed run-result JSON or matching `RunResultSha256`, clean
pass/failure state, before/after/current Godot log length/SHA256 metadata,
`RuntimeProbeSamplesPath`, clean `MainMenuObservation` and
`RuntimeObservation` records including runtime `LogGrew: true`, parseable
ordered run-result timestamps, `main-menu` and `runtime` probe phases, probe
sample parseable `SampledAt`, `LogExists`, `LogLengthBytes`, and retained
parseable `LogLastWriteTimeUtc` fields when the log exists, with no log
last-write timestamp later than the sample timestamp, retained probe samples in
nondecreasing `SampledAt` order, `main-menu` phases before `runtime` phases,
non-negative and nondecreasing log lengths while `LogExists=true`, runtime sample
`LogLengthBytes` growth beyond
`RuntimeObservation.LogInitialLengthBytes`, unknown process start-time counts,
ambiguous current-process counts, before/after/current Godot log-slice proof,
clean audit recomputation, StS1 mode binding, `EventKind: Ancient` /
`AncientId`, required proof-mode `-ExpectedAncientIds` plan, summary, and
traversed-id coverage, or ordered event-room traversal markers such as
`Entering Event room`, `Detected Ancient event, clicking through dialogue`,
and `Selecting event option: <AncientId>`. Malformed numeric fields are treated
as failed checks rather than verifier crashes, so hand-edited JSON remains
diagnosable as rejected evidence. Use a smaller
`-MinRuns` only for temporary parser or fixture tests, and never set it to 0 or
a negative value; a real game-native monkey proof should use the intended proof
count plus the intended target Ancient id coverage. A single-seed fixture packet is not batch proof; the verifier must fail when `-MinRuns` is higher than the retained plan and summary
run count, and it must fail when any requested `-ExpectedAncientIds` value is
missing from the retained traversed Ancient ids.
The verifier reruns `check-sts1-enabled-mode-runtime-log.ps1` against each
retained current Godot log and audit, then requires retained StS1 verifier
`Mismatches` and `Checks` to match the recomputed report. If explicit
package/game/Ritsu target switches are omitted, this recompute uses
`autoslay-plan.json` `PackageVersion`, `GameVersion`, `RitsuLibVersion`, and
`RitsuCompatBranch`; proof-mode still requires the explicit switches.
Omitting the current package/game/Ritsu/patch target switches fails
`expected_package_version_parameter_provided`,
`expected_game_version_parameter_provided`,
`expected_ritsu_lib_version_parameter_provided`,
`expected_ritsu_compat_branch_parameter_provided`, and
`expected_patch_count_parameter_provided`.

If a launched AutoSlay batch fails, triage the retained packet without launching
anything:

```powershell
.\scripts\analyze-spire-plus-runtime-failure.ps1 `
  -EvidenceDir "<evidence>" `
  -OutFile "<evidence>\runtime-failure-analysis.json"
```

For failed direct smoke evidence roots, the analyzer recognizes
`direct-smoke-summary.json` as a `DirectSmoke` target even when the packet is
incomplete. A failed or dirty DirectSmoke summary without retained
`godot.log.current-iteration` or `godot-log-audit.json` fails closed as
`RuntimeHarness` evidence before owner routing. When those artifacts are
present, it binds the audit back to the current slice and routes previous package
dirty-audit signatures such as
`dependency patch failure` and `[ERROR] [previous package]` to `PackageRuntimeDrift`, not to
StS1 event source. Nonzero DirectSmoke mode or packet verifier mismatch counts
are reported as `direct_smoke_verifier_mismatch` under `PackageRuntimeDrift`.
The JSON report's `dependency patch failures` array records
patch-level details such as `AdjustCustomMessageKeys::Fuckery()` undefined
target-method failures, `NRelicCollectionCategory::LoadRelics` instruction
matcher failures, and the previous package applied/failed patch summary so dependency
compatibility work can start from the exact failed patch signatures. Descriptive
startup text that merely mentions
`SPIREPLUS_ALLOW_UNVERIFIED_COOP_*` must not count as an enabled co-op override;
only explicit `coop_*override_enabled` runtime markers should produce that
blocker.

For `GameNativeAutoSlay`, the analyzer reads `autoslay-summary.json`, each
per-seed `run-result.json`, `runtime-probe-samples.json`, and sidecar log. It
requires `autoslay-summary.json` `Runs[].RunResultPath` to resolve under the
evidence root, and requires `autoslay-summary.json` `Runs[].RunResultSha256` to match the retained per-seed `run-result.json`. The AutoSlay summary target and
per-seed `run-result.json` must also agree on `RunnerKind: GameNativeAutoSlay`
before run-result fields can drive owner routing. It also requires retained summary row `Passed`, `FailureReasonCodes`, and
`HangSignals` to match the retained `run-result.json` before owner routing, and
requires retained signal fields to be native JSON arrays rather than scalar
strings.
It also rejects top-level AutoSlay analyzer summary-plan batch metadata drift:
`autoslay-summary.json` `RunnerKind`, `Sts1EventMode`, package/game/Ritsu
targets, `ExpectedPatchCount`, and normalized `ExpectedAncientIds` must match
`autoslay-plan.json` before per-seed run/log artifacts can route source
ownership; missing or blank target fields are invalid even when both files omit
the same value, and non-positive `ExpectedPatchCount` or empty
`ExpectedAncientIds` targets are invalid retained evidence.
It refuses to route source ownership from `godot.log.current-iteration` unless
`godot.log.before` and `godot.log.after-launch` prove the current slice by exact
bytes and the run-result before/after/current Godot log byte-length/SHA256
metadata matches the retained files. Malformed numeric, boolean, or array-shape evidence
fields are treated as failed harness evidence checks with sentinel values rather
than analyzer crashes, string-to-true coercions, null-to-false passes, or
gameplay-owner signals. It rejects GameNativeAutoSlay
`RunResultPath` escapes, root/shared `run-result.json` paths that do not resolve
exactly to `run-####/run-result.json`, and root/shared GameNativeAutoSlay Godot logs, runtime
probe samples, audit JSON, and StS1 reports with
`RuntimeHarness` blockers before using those files for source routing. It
also treats retained AutoSlay path fields that are empty, malformed, missing on
disk, or not retained under the per-seed run directory as untrusted before owner
routing. It appends AutoSlay sidecar text to log-derived owner routing only when
`autoslay.log` is retained in the per-seed run directory and
`AutoSlayLogSha256` matches. It also reports missing launcher invocation,
missing, hash-mismatched, or unhealthy runtime probe samples, missing `main-menu` / `runtime`
probe phases, invalid probe sample timestamps, invalid or reversed run-result timestamps,
missing or unhealthy `MainMenuObservation` / `RuntimeObservation`,
runtime probe `LogLengthBytes` drift from `RuntimeObservation.LogGrew`,
`EventKind: Ancient` / `AncientId`,
sidecar log, completion/failure marker, or ordered Ancient event traversal as
`RuntimeHarness` evidence defects first. Those defects clear AutoSlay run,
probe, sidecar, or current-log trust before owner routing, so retained
`process_unresponsive`, `command_ack_missing`, and unclassified failure codes
stay with `RuntimeHarness` until the traversal packet is trustworthy. This makes failed AutoSlay packets
useful for diagnosis, but it still does not turn a failed or source-only packet
into gameplay proof.

## Commands

Dry-run broad Ancient plan, no game launch:

```powershell
.\scripts\run-spire-plus-monkey-stability.ps1 `
  -Iterations 1000 `
  -Scenario AncientUiPlusVakuuFight `
  -CommandSelectionMode RoundRobin
```

Dry-run focused Vakuu forced fight-option plan, no game launch:

```powershell
.\scripts\run-spire-plus-monkey-stability.ps1 `
  -Iterations 1000 `
  -Scenario VakuuFightSmoke `
  -CommandSelectionMode RoundRobin
```

Small controlled smoke, with only STS2-RitsuLib and Spire Plus enabled:

```powershell
.\scripts\run-spire-plus-monkey-stability.ps1 `
  -Iterations 5 `
  -Launch `
  -MoveOtherMods `
  -MoveCurrentRuns `
  -MainMenuTimeoutSeconds 240 `
  -NoLogGrowthTimeoutSeconds 120 `
  -ObservationIntervalSeconds 2 `
  -PostCommandSeconds 20
```

Use `-Scenario StartupOnly` or `-NoDevConsoleCommands` when testing startup
stability only. Named scenarios provide safer defaults than long ad hoc command
lines:

- `AncientUiSmoke`: round-robin Urda, Morvi, Lotha, and normal Vakuu Ancient
  UI setup commands.
- `VakuuFightSmoke`: focused `spireplus_test_ancient VAKUU confirm fight`
  coverage for force-fight gate arming and hidden fight-option UI setup.
- `AncientUiPlusVakuuFight`: Urda, Morvi, Lotha, normal Vakuu, and gated Vakuu
  fight in one balanced corpus.
- `StartupOnly`: no DevConsole commands; startup/watchdog/log audit only.

The default command selection mode is `RoundRobin`, so a 1000-iteration plan has
balanced command coverage. Use `-CommandSelectionMode Random` only when random
distribution is intentional, and keep `-RandomSeed` in the retained plan. Use
`-CommandCorpus` or `-CommandCorpusFile` to pass a focused custom command set
for future preview-tool or StS1 probes.

By default, the runner derives expected package version from root
`EZMicroBalance.json`, game version from `<GameRoot>\release_info.json`, and
RitsuLib version/compat branch from the installed `STS2-RitsuLib` manifest and
variant config. Override those parameters only when intentionally auditing an
older package.

After a launched run, verify the retained packet without launching anything:

```powershell
.\scripts\check-spire-plus-runtime-monkey-packet.ps1 `
  -EvidenceDir .tools\runtime-evidence\<monkey-stability-dir> `
  -ExpectedIterations 5 `
  -ExpectedPackageVersion v0.1.0-private-beta.113 `
  -ExpectedGameVersion 0.107.1 `
  -ExpectedRitsuLibVersion 0.4.34 `
  -ExpectedRitsuCompatBranch 0.107.1 `
  -ExpectedPatchCount 64 `
  -FailOnMismatch
```

In `-FailOnMismatch` proof mode, the current package/game/Ritsu/patch target
switches are required even though non-proof diagnostics may fall back to retained
`monkey-plan.json` values. Omitting those switches fails
`expected_package_version_parameter_provided`,
`expected_game_version_parameter_provided`,
`expected_ritsu_lib_version_parameter_provided`,
`expected_ritsu_compat_branch_parameter_provided`, and
`expected_patch_count_parameter_provided`.

Add `-RequireCurrentSourceSnapshot` only when the packet is being used as
current-source proof. That mode fails if the retained source-workspace report
does not have `RecoveredSource.MatchesInstalledGame`,
`RecoveredSource.OriginMatchesInstalledGamePck`, and
`EvidenceUsePolicy.AuthorizedSourceOriginVerified` all true.

If the packet fails, triage it without launching anything:

```powershell
.\scripts\analyze-spire-plus-runtime-failure.ps1 `
  -EvidenceDir .tools\runtime-evidence\<monkey-stability-dir> `
  -OutFile .tools\runtime-evidence\<monkey-stability-dir>\runtime-failure-analysis.json
```

Use the analyzer's top-level `TriageDisposition` before changing gameplay
source. `HarnessEvidenceInvalid` means the retained packet is not trustworthy
enough for source ownership; fix the runner/evidence first. `PackageRuntimeDrift`
means inspect installed package/API compatibility before gameplay source.
`GameplayOwnerAction` means the packet is sufficiently bound to start from
`GameplayBlockingFindings`, owner areas, and `RecommendedNextActions`.

Do not run the packet checker with `-FailOnMismatch` against dry-run-only
folders. Dry-run folders intentionally contain `monkey-plan.json` but no
`monkey-summary.json`, iteration logs, or runtime audit files.

The packet checker recomputes command, scenario-tag, and owner-area counts from
the retained plan and summary, then binds each retained
`iteration-result.json` back to the matching `monkey-plan.json`
`PlannedCommands` row and `monkey-summary.json` `Results` row by iteration
number. The binding includes the root `Scenario`, `CommandSelectionMode`,
command, command index, scenario tag, owner area, command acknowledgement
pattern, required flag, pass state, and acknowledgement result. Summary
`Results` rows must retain the same command acknowledgement pattern and required
flag as `iteration-result.json`, and `CommandAckRequired` must equal whether a
`CommandAckPattern` is retained. For known built-in commands, the retained
`ScenarioTag`, `OwnerArea`, and `CommandAckPattern` must also match the runner's
canonical command classification. Known commands with canonical acknowledgement
patterns must keep `CommandAckRequired=true` and a non-empty pattern. When a
command acknowledgement is required, the checker replays the retained pattern
against `godot.log.current-iteration`; `CommandAckObserved=true` in JSON is not
enough without the source-backed log line. `VakuuFightSmoke` packets must contain
only `vakuu-fight` planned iterations. A 1000-iteration
`AncientUiPlusVakuuFight` round-robin packet must contain exactly 200
`vakuu-fight` planned iterations.
The current `vakuu-fight` command proves only the forced fight option is shown
when the copied log contains `[SPIREPLUS-EVIDENCE] VakuuFight fight_option_shown`.
It is not child-combat proof unless later evidence also contains `fight_started`
and `child_combat_room_entered`.

The launched packet checker requires `MainMenuObservation` and
`RuntimeObservation` in each `iteration-result.json`. These records include
process-observed, process-exited, stale-process, hung-window, log-observed,
`RuntimeLogGrowthRequired`, log-length, main-menu/runtime
`NoLogGrowthTimeoutExceeded=false` state, and command-bearing runtime
`LogGrew=true`. Startup-only or no-command observations do not require idle
main-menu log growth. The checker also requires the retained
`iteration-result.json` `RuntimeProbeSamplesPath`,
`RuntimeProbeSamplesSha256`, `GameProcessId`, `GameProcessStartTimeUtc`, and
`GameProcessPath` to match the single positive process identity observed by
`runtime-probe-samples.json` and the live-session-selected game process.
`RuntimeProbeSamplesSha256` must match the retained standard
`runtime-probe-samples.json` file before probe telemetry is trusted. Runtime
probe `LogLengthBytes` must prove required post-command growth and must not
exceed either recorded `GodotLogAfterLaunchLengthBytes` or retained
`godot.log.after-launch` bytes for any retained sample whose `LogExists` is
true.
`sts1-mode-log-check.json` to match the plan's `Sts1EventMode` and bind its
`LogPath`, `LogLength`, and `LogSha256` to `godot.log.current-iteration`.
The packet checker reruns `check-sts1-enabled-mode-runtime-log.ps1` against
that same retained current slice and audit, then requires the retained
StS1 verifier mismatches and checks to match the recomputed report. When the
packet-check command omits explicit package/game/Ritsu target switches, the
checker uses the retained `monkey-plan.json` target values for this recompute
so real runner packets remain self-contained. It also
requires exact Spire Plus patch-count lines from `godot.log.current-iteration`,
probe sample paths, probe sample SHA256 binding, and sliced-log paths that
point to the retained standard files inside the current iteration folder,
and no `../` or absolute-path escape from `iteration-result.json` log/probe
path fields. The packet checker rejects `iteration-result.json` log/probe paths that resolve outside the current `iteration-####` directory.
`godot.log.before` path/length/SHA256 binding, `godot.log.after-launch`
path/length/SHA256 binding, `LogScanOffsetBytes` equal to the retained
before-log byte length, `godot.log.before` as a byte prefix of
`godot.log.after-launch`, a `godot.log.current-iteration` byte slice that
matches `godot.log.after-launch` after that before-log prefix, command
acknowledgement patterns that match known built-in command regexes and that
retained slice when required, a `godot-log-audit.json` whose scanned `Path`,
`Length`, and `Sha256` bind to the retained current-iteration slice and whose
signature counts match a packet-checker recomputation from that slice, no raw probe sample with
`Responding=false`, and no probe sample or observation with
`StaleProcessCount > 0`. It also requires the per-iteration
  `session-state.json` and `restore-state.json` files, validates that both
  states belong to the same iteration directory, validates result-file SHA256
  bindings, and rejects malformed restore timestamps, stopped process fields,
  scalar or missing moved-item/process-id arrays, restored item count
  mismatches, post-restore SlayTheSpire2/Godot process leaks,
  preserved-current-run manifest drift, or settings hashes/recorded backup
  existence that do not match the retained pre-prepare session state.
A clean packet means those signals stayed healthy for both sampled windows;
the retained `runtime-probe-samples.json` must include `StartupMainMenu` and
`PostCommandRuntime` samples, and those phase counts must match
`MainMenuObservation.Samples` and `RuntimeObservation.Samples`. For
command-bearing iterations, the `PostCommandRuntime` samples' `LogLengthBytes`
must also prove the `RuntimeObservation.LogGrew` claim by exceeding
`RuntimeObservation.LogInitialLengthBytes` while staying within the retained
`godot.log.after-launch` byte ceiling. It still does not prove deeper gameplay
behavior.

Current packet schema is `HangProbeSchemaVersion = 1`.

- `monkey-plan.json` records `Scenario`, `CommandSelectionMode`,
  `CommandCorpusSource`, `CommandScenarioMatrix`, `ProcessProbe`,
  `LogGrowthProbe`, `CommandAckPatterns`, `SourceWorkspaceCheckPath`,
  `SourceWorkspaceCheckSha256`, and `SourceWorkspace` source-snapshot
  disposition. The packet verifier parses the retained source-workspace report,
  requires `Passed=true` with no mismatches, checks the evidence-use policy
  flags, and compares the report to the plan summary. The source-workspace
  check is no-launch evidence binding for API/source triage; it is not gameplay
  or runtime proof.
- Each iteration retains `prepare-output.json` from
  `scripts\spire-plus-live-session.ps1 -Mode Prepare -Launch`. The packet
  checker binds it to `LiveSessionPrepareOutputPath` /
  `LiveSessionPrepareOutputSha256`, requires Steam `-applaunch 2868840`
  metadata, requires PID attribution to pass, and compares the selected game
  process id, start timestamp, executable path, and empty pre-launch
  `SlayTheSpire2` process set back to `iteration-result.json`.
  `prepare-output.json`, `session-state.json`, and `restore-state.json`
  `EvidenceDir` fields must resolve to the same current `iteration-####`
  directory; live-session child files from another iteration or a shadow
  directory are not valid packet evidence.
- Each `iteration-result.json` records `GameProcessId`,
  `GameProcessStartTimeUtc`, `GameProcessPath`, `MainWindowObserved`,
  `MainMenuDetectedAt`, `MainMenuElapsedSeconds`, `PreLaunchLogLengthBytes`,
  `MinimumProcessStartTimeUtc`, `LogScanOffsetBytes`,
  `CurrentIterationLogPath`, `CurrentIterationLogCopied`, `ScenarioTag`, `OwnerArea`,
  `CommandSelectionMode`, `LogInitialLengthBytes`,
  `LogFinalLengthBytes`, `LastLogGrowthAt`, `MaxSecondsWithoutLogGrowth`,
  `MaxConsecutiveUnresponsiveSamples`, `StaleProcessObserved`,
  `StaleProcessCount`, `StartupLogProbePassed`,
  `PostCommandLogProbePassed`, `CommandAckRequired`, `CommandAckPattern`,
  `CommandAckObserved`, `LiveSessionPrepareOutputPath`,
  `LiveSessionPrepareOutputSha256`, `LiveSessionSessionStatePath`,
  `LiveSessionSessionStateSha256`, `LiveSessionRestoreStatePath`,
  `LiveSessionRestoreStateSha256`, `LiveSessionLaunchedAt`,
  `LiveSessionPidAttributionPassed`, `LiveSessionSelectedGameProcessId`,
  `LiveSessionSelectedGameProcessStartTimeUtc`,
  `LiveSessionSelectedGameProcessPath`,
  `GameProcessStartTimeAfterLiveSessionLaunch`,
  `GameProcessIdMatchesLiveSession`,
  `GameProcessStartTimeMatchesLiveSession`, `GameProcessPathMatchesLiveSession`,
  restore schema/count/process/settings fields, `ResponsivenessProbePassed`,
  current-slice offset binding, `HangSignals`, and `FailureReasonCodes`.
- Each iteration retains `runtime-probe-samples.json` with the sampled
  process/window/log records, including `Phase`, parseable `SampledAt`,
  `LogExists`, `LogLengthBytes`, parseable `LogLastWriteTimeUtc` when the log
  exists, `ProcessId`, `ProcessStartTimeUtc`, `ProcessPath`, expected game
  process identity, process-id/start/path match booleans,
  stale/unknown/ambiguous process counts, window state, and responsiveness
  state. `iteration-result.json` must retain `RuntimeProbeSamplesPath` and
  `RuntimeProbeSamplesSha256`, and that hash must match the retained
  per-iteration `runtime-probe-samples.json` before the samples can support
  owner routing. The retained probe file must be a native JSON array; scalar,
  object, string, missing, or null probe evidence is malformed rather than a
  single-sample shortcut. `LogLastWriteTimeUtc` must not be later than `SampledAt`, `SampledAt`
  values must be nondecreasing in retained order, and `LogLengthBytes` must be
  non-negative and nondecreasing whenever `LogExists=true`. Runtime monkey phases are
  `StartupMainMenu` and `PostCommandRuntime`; the packet checker rejects
  unknown phase values, missing phase coverage, and phase-count drift versus
  the retained observation sample counts.
- `monkey-summary.json` records `FailedIterationIds`, `FailureReasonCounts`,
  `ProcessExitCount`, `LiveSessionBindingMissingCount`,
  `LiveSessionRestoreItemCountMismatchCount`, `LiveSessionRestoreLeakCount`,
  `LiveSessionRestoreHashMismatchCount`,
  `LiveSessionSelectedProcessNotStoppedCount`, `UnresponsiveIterationCount`,
  `LogStallIterationCount`, `StaleProcessObservedCount`,
  `CommandAckMissingCount`, `CommandCounts`,
  `ScenarioTagCounts`,
  `OwnerAreaCounts`, `VakuuFightIterationCount`, `MaxMainMenuElapsedSeconds`,
  `MaxSecondsWithoutLogGrowth`, and `MaxConsecutiveUnresponsiveSamples`.
  Top-level `monkey-summary.json` batch metadata for `Scenario`,
  `CommandSelectionMode`, `Sts1EventMode`, expected package/game/Ritsu targets,
  and `ExpectedPatchCount` must match the retained `monkey-plan.json`.
  `CommandCorpus`, `PlannedCommands`, `Results[]`, `FailedIterationIds`,
  `FailureReasonCodes`, `HangSignals`, `LiveSessionPreLaunchSlayProcessIds`,
  `PreLaunchSlayProcessIds`, and retained `runtime-probe-samples.json` must be
  native JSON arrays; scalar, object, string, missing, or null retained values
  are malformed evidence.
  `FailedIterationIds` entries must be positive integers; malformed, null,
  non-positive, or overflow entries fail closed as `RuntimeHarness` evidence
  defects before summary-directed owner routing is trusted.
  Top-level failed-iteration ids, failure-reason maps, process-exit,
  main-window, live-session-binding, log-missing, unresponsive, stale-process,
  log-stall, and command-ack counters must match the aggregation recomputed
  from `Results[]` before a summary can be trusted. Malformed or overflow
  `FailureReasonCounts` values and malformed boolean pass fields are
  runtime-harness evidence defects, not analyzer crashes or owner-routing proof.
  Summary max telemetry must match the maximum values recomputed from
  `Results[]`; stale or hand-edited max values fail packet verification.
  Each `Results[]` live-session prepare-output path/SHA256 field must match the
  corresponding `iteration-result.json` `LiveSessionPrepareOutputPath` and
  `LiveSessionPrepareOutputSha256` field before launch evidence can be trusted.
  Each `Results[]` runtime-probe path/SHA256 field must match the corresponding
  `iteration-result.json` `RuntimeProbeSamplesPath` and
  `RuntimeProbeSamplesSha256` field before probe evidence can be trusted.
  Each `Results[]` live-session state path/SHA256 field must match the
  corresponding `iteration-result.json` `LiveSessionSessionStatePath`,
  `LiveSessionSessionStateSha256`, `LiveSessionRestoreStatePath`, and
  `LiveSessionRestoreStateSha256` field before restore evidence can be trusted.
  Each `Results[]` row must also retain empty `FailureReasonCodes` and
  `HangSignals` for a clean packet, and those arrays must match the canonical
  `iteration-result.json` row for the same iteration.

The built-in `spireplus_test_ancient ... confirm` commands require the
source-backed acknowledgement line from
`SpirePlusAncientLiveTestConsoleCmd.RunSetup.cs`: normal Ancient setup commands
must show the unsaved live-test run starting for the requested Ancient. The
`spireplus_test_ancient VAKUU confirm fight` command is stricter: it proves the
forced fight-option setup only with the `VakuuFightService.Entry.cs` release
evidence line `[SPIREPLUS-EVIDENCE] VakuuFight fight_option_shown`, not with the
generic unsaved live-test setup line.

The triage analyzer maps retained signals to owner areas. It records the planned
`OwnerAreaHint` separately from `OwnerAreaFromLog` and `OwnerAreaFromCommand`.
Runtime monkey analysis also records `RuntimeMonkeyRunArtifactsTrustedForOwner`,
`RuntimeMonkeyProbeArtifactTrustedForOwner`, and `LogTextTrustedForOwner` so
retained reports show which evidence layer was allowed to support owner routing.
It treats `monkey-summary.json` summary counter mismatch versus `Results[]`
aggregation as a `RuntimeHarness` blocker and clears runtime-monkey run/log
trust before owner routing.
It also treats scalar, object, string, missing, or null
`monkey-summary.json` `Results` / `FailedIterationIds` shapes as summary
defects before owner routing; the analyzer may normalize PowerShell values for
iteration discovery, but malformed retained JSON shape remains a
`RuntimeHarness` finding.
It also treats `monkey-summary.json` `Results[]` row mismatch versus canonical
`iteration-result.json` fields as a `RuntimeHarness` blocker before owner
routing.
It also treats top-level `monkey-summary.json` batch metadata drift versus
`monkey-plan.json` as a `RuntimeHarness` blocker before owner routing; missing
or blank target fields such as `Sts1EventMode`, package/game/Ritsu targets, or
non-positive `ExpectedPatchCount` are invalid even when both files omit the
same value.
When `monkey-plan.json` is retained, the analyzer also treats
`PlannedCommands` row mismatch versus canonical `iteration-result.json` fields
as a `RuntimeHarness` blocker before owner routing.
Runtime monkey batch analysis with `monkey-summary.json` requires a parseable
`monkey-plan.json` with `PlannedCommands`; a missing or malformed batch plan
is a `RuntimeHarness` blocker before owner routing.
For runtime monkey packets, it treats missing `RuntimeProbeSamplesPath`,
missing or hash-mismatched `RuntimeProbeSamplesSha256`,
missing/invalid `runtime-probe-samples.json`, invalid probe timestamps, missing
phase coverage, and phase-count or runtime log-growth timeline drift as `RuntimeHarness` blockers
before source ownership routing. Those probe defects
clear runtime-monkey run/probe/log trust, so retained `command_ack_missing` or
unclassified `iteration-result.json` failure codes stay under `RuntimeHarness`
until the packet evidence is valid. It also rejects probe `LogLengthBytes`
values that exceed recorded or retained `godot.log.after-launch` bytes, and
`iteration-result.json` log or probe paths that resolve outside the current
`iteration-####` directory or to shadow/nonstandard files under that directory,
and it does not use those escaped or noncanonical files for log-derived,
audit-derived, or probe-derived owner routing. Retained probe samples must also
bind to the `iteration-result.json` game process id, start time, and path, and
to the live-session-selected process identity when those fields are present.
Stale-process, unknown-start-time, ambiguous-current-process, non-single-current-process,
or process-identity defects make the runtime monkey log text untrusted for
owner routing.
When `godot.log.current-iteration` exists, the analyzer requires
`godot.log.before`, `godot.log.after-launch`, and `LogScanOffsetBytes`;
otherwise it reports a `RuntimeHarness` blocker and does not route ownership
from the unbound current slice. When the binding files are present, the analyzer
requires `LogScanOffsetBytes` to equal the before-log byte length, requires
`godot.log.before` to be a byte prefix of `godot.log.after-launch`, and requires
`godot.log.current-iteration` to match the after-launch bytes after that prefix.
It also requires `iteration-result.json` before/after/current log length and
SHA256 metadata to match the retained files. Any slice or metadata mismatch is
a `RuntimeHarness` blocker and the log is not trusted for owner routing. If
`iteration-result.json` is missing or invalid, `monkey-summary.json` may still
provide a fallback row for routing, but the analyzer reports a `RuntimeHarness`
blocker because summary data does not replace the canonical per-iteration
artifact. Empty retained JSON arrays such as `Mismatches`, `FailureReasonCodes`,
`HangSignals`, and `SignatureHits` are treated as empty signal sets before owner
routing. If `Passed=false` has no retained failure code, hang signal, audit hit,
or other blocking harness finding, the analyzer emits
`iteration_failed_without_failure_signal`. Invalid or empty
`godot-log-audit.json` files are `RuntimeHarness` blockers because audit
evidence cannot be trusted. Valid audit JSON is still not owner-routing
evidence until its scanned `Path`, `Length`, and `Sha256` bind to
`godot.log.current-iteration` and a fresh analyzer-side
`audit-godot-log.ps1` recomputation agrees with the retained signature counts.
Stale or hand-assembled audit JSON is reported as a `RuntimeHarness` blocker,
and its signature hits are ignored for feature ownership.
During GameNativeAutoSlay analysis, if run, probe, sidecar, audit, or StS1
artifact trust is revoked after initial slice binding, the analyzer clears
`LogTextTrustedForOwner` and `OwnerAreaFromLog` before retained failure signals
are routed.
For GameNativeAutoSlay summaries, retained `Runs` must be a non-empty native
JSON array. If the array shape is malformed, the analyzer still inspects
retained `run-*` directories when present, but records an
`autoslay_summary_shape_invalid` `RuntimeHarness` blocker and does not trust
AutoSlay run or probe artifacts for owner routing.
It also recomputes summary `Passed`, `TotalRuns`, `FailedRuns`, and
`AncientIdCounts` from retained `Runs[]`; stale or hand-edited aggregate values
record `autoslay_summary_counter_mismatch` before any AutoSlay owner routing is
trusted.
The analyzer also requires the AutoSlay launcher/mod-hook provenance from
`autoslay-plan.json` and each `run-result.json` to bind to the same retained
launcher proof artifact. `LauncherKind`, `LauncherPath`, `LauncherSha256`,
`HookId`, `HookAssembly`, and `InvocationCommand` must be present, the launcher
path must stay under the evidence directory, the retained file hash must match,
and `InvocationCommand` must call `AutoSlayer.Start(seed, logFile)`. Missing or
stale launcher provenance records `autoslay_launcher_provenance_mismatch` as a
`RuntimeHarness` blocker and disables AutoSlay run, probe, sidecar, audit, StS1,
and log owner routing for that seed.
When a retained `sts1-mode-log-check.json` exists, the analyzer applies the
same trust rule: the report's `Mode` must bind to the retained run plan
`Sts1EventMode`, its `LogPath`, `LogLength`, and `LogSha256` must bind to
`godot.log.current-iteration`, and a fresh analyzer-side
`check-sts1-enabled-mode-runtime-log.ps1` recomputation from the retained
current log plus `godot-log-audit.json` must match the retained `Mismatches`
and `Checks`. Missing, stale, hand-edited, or unrecomputable StS1 reports are
`RuntimeHarness` blockers and set `Sts1ModeLogCheckTrustedForOwner=false`;
only trusted analyzer-side recomputed mismatches or failed checks, with trusted
audit evidence, are routed to `Sts1Events`. A retained `sts1_mode_mismatch`
failure code by itself remains `RuntimeHarness` evidence.
Current no-launch restore hardening binds `iteration-result.json` to retained
`session-state.json` and `restore-state.json` paths/hashes, requires restored
mod/current-run counts to close over the prepared moved lists, requires any
preserved test-created current-run manifest to be path/hash bound, requires
post-restore SlayTheSpire2/Godot process counts and id arrays to be zero,
models an originally absent `settings.save.backup` as absent-after-restore
instead of a hash mismatch, and routes restore transaction drift to
`RuntimeHarness` or `LiveSessionRestore` before gameplay owner routing. This
still needs a fresh live runtime packet after same-repo validation/runtime lanes
are unpaused.
For hung processes, unclassified retained failures, audit hits, Spire
Plus error/exception hits, and co-op override failures, explicit log-derived
package/runtime drift, StS1, preview-tool, or multiplayer-policy signatures take
precedence over the planned command owner. Package/runtime drift classification
is reserved for actual mismatch/error signals such as type-load, missing-method,
or expectation-drift lines, not normal startup package markers. `PreviewTools`
is reserved for specific Crystal Sphere, Transform Preview, Future Peek,
`PreviewTransform`, `PreviewCrystalSphere`, `[Spire Plus] Preview`, or
local-UI-only preview-tool evidence; generic map preview text such as Root Sight
remains under its feature owner. Command-ack failures and Vakuu command failures
still preserve the planned/command owner unless the log is the only useful
source.

Current owner areas
include `RuntimeStartup`, `RuntimeCrash`, `RuntimeHarness`,
`RuntimeLogAudit`, `PackageRuntimeDrift`, `Sts1Events`, `LiveSessionRestore`,
`DevConsoleHarness`, `Ancients.Vakuu`, `Ancients.Morvi`, `Ancients.Lotha`,
`Ancients.Urda`, `Ascension11To20`, `PreviewTools`, `MultiplayerPolicy`, and
`Runtime.Unknown`.
More specific rows such as `Ancients.Vakuu.FightOptionSetup`,
`Ancients.Vakuu.ChildCombatResume`,
`Ancients.Morvi.CardPlayState`, `Ancients.Lotha.CardPlayState`,
`Ancients.Urda.MapSaveState`, and `Ascension11To20.Rootblight` are used when
command/log text is specific enough. Each finding records severity, confidence,
evidence files, rationale, and the next investigation step.

## Triage Workflow

When an iteration fails:

1. Open that iteration's `iteration-result.json`.
2. Check whether failure is timeout, no log growth before main menu, missing
   process, hung window, command failure, audit hit, or restore failure.
3. Inspect `godot.log.current-iteration`, `godot.log.after-launch`, and
   `godot-log-audit.json`.
4. If the log points at Spire Plus source, add or tighten a source guard before
   changing behavior.
5. If the log points at package/source drift, refresh package evidence before
   claiming a runtime fix.
6. If the failure is a hang with no useful log, rerun the same seed and command
   corpus with fewer iterations and screen capture/preflight enabled.

## Relationship To Existing Lanes

- `scripts\ci-full-validation.ps1` stays the no-game build/test/package lane.
- `scripts\spire-plus-live-session.ps1` remains the restore-safe live-session
  primitive.
- `scripts\audit-godot-log.ps1` remains the canonical log signature scanner.
- `scripts\check-spire-plus-runtime-monkey-packet.ps1` verifies launched monkey
  packet shape after the runner has produced retained evidence.
- StS1 enabled-mode proof still requires
  `scripts\check-sts1-enabled-mode-runtime-log.ps1` and
  `scripts\check-sts1-runtime-evidence-packet.ps1`.
- Manual release evidence still requires
  `scripts\verify-spire-plus-release-evidence.ps1`.

The monkey lane is a stability amplifier, not a replacement for those gates.
