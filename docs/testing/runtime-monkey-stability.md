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
- Current runtime dependency: `STS2-RitsuLib v0.4.24` with `lib\0.107.0`.
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

Current caution: `source code\release_info.json` currently reports `v0.106.0`
on this machine, while the installed game is `v0.107.0`. Check that file before
using the recovered project as current-source evidence. If it does not match the
installed game version, refresh the local source snapshot first or treat it as
historical.

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

Use `-RequireCurrentSourceSnapshot -FailOnMismatch` only when a task requires
current-source parity. The current local state is expected to report
`source_version_matches_installed_game`, `source_commit_matches_installed_game`,
`source_branch_matches_installed_game`,
`source_main_assembly_hash_matches_installed_game`, and
`source_release_identity_matches_installed_game` as warnings until `source code/`
is refreshed from the installed `v0.107.0` package. The checker also compares the
GDRE `Opening file` line to the installed `SlayTheSpire2.pck` path and reports
`GdreExport.OpeningFileMatchesInstalledGame` plus
`RecoveredSource.OriginMatchesInstalledGamePck`. The report also retains
RitsuLib manifest, variants, selected variant DLL, and compat-target paths and
hashes so runtime packets can bind the local Ritsu state they used. GDRE origin
mismatch and the current GDRE export's failed-script / parse-error counts are
warnings by default; use `-RequireCurrentSourceSnapshot` or
`-RequireCleanGdreExport` only when those must be release-blocking for the task.

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
   name or older log content.
12. Restore the live session with `-StopGameOnRestore` and
   `-PreserveNewCurrentRunsOnRestore`.
13. Write `iteration-result.json` and a root `monkey-summary.json`.

The lane fails an iteration on:

- main-menu timeout;
- `SlayTheSpire2` disappearing during startup or post-command observation;
- any pre-existing `SlayTheSpire2` process observed during startup or
  post-command observation;
- missing `prepare-output.json`, missing Steam launch metadata, failed selected
  game process attribution, or runtime probe PID/start/path mismatch against the
  live-session-selected game process;
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
- failed restore.

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

- the exact launcher or mod hook that calls `AutoSlayer.Start(seed, logFile)`,
  retained as a hashed launcher/provenance artifact plus `LauncherKind`,
  `LauncherPath`, `LauncherSha256`, `HookId`, `HookAssembly`, and
  `InvocationCommand`;
- one `run-result.json` per seed with `SchemaVersion: 1`, `Launch: true`,
  `RunnerKind: GameNativeAutoSlay`, invocation text, process id,
  parseable start/end timestamps where start is not later than end,
  `Passed: true`, empty `FailureReasonCodes` and
  `HangSignals`, exit code, stale-process count, `EventKind: Ancient`,
  `AncientId`, `RuntimeProbeSamplesPath`, clean `MainMenuObservation`,
  clean `RuntimeObservation` with `LogGrew: true` and
  `NoLogGrowthTimeoutExceeded: false`, and the retained per-run before,
  after-launch, and current-iteration Godot log paths, byte lengths, and
  SHA256 hashes;
- one retained `runtime-probe-samples.json` per seed with `Phase`, `SampledAt`,
  `LogExists`, `LogLengthBytes`, retained `LogLastWriteTimeUtc`, `ProcessId`,
  `ProcessObserved`, `MainWindowObserved`, `HungWindow`, `Responding`, and
  `StaleProcessCount`, `CurrentProcessCount`,
  `UnknownStartTimeProcessCount`, and `AmbiguousCurrentProcessCount` fields, at
  least one `main-menu` phase sample, at least one `runtime` phase sample,
  stable positive process id binding that matches the per-seed `run-result.json`
  `ProcessId`, process and main-window observations, no hung-window samples, no
  `Responding=false` samples, `StaleProcessCount: 0`,
  `UnknownStartTimeProcessCount: 0`, and `AmbiguousCurrentProcessCount: 0` on
  every sample; the file must be retained inside the same per-seed `run-####`
  directory as that seed's `run-result.json`;
- the seed, AutoSlay log path, exit code, Ancient id, ordered
  start/event/Ancient-dialogue/event-option/completion markers, with
  `AutoSlayLogSha256` bound to the retained log file; the sidecar log path must
  stay inside the same per-seed `run-####` evidence directory as that seed's
  `run-result.json`;
- a retained `check-local-godot-source-workspace.ps1 -OutFile` report with
  schema version, creation time, repo/source/game roots, no-launch policy flags,
  passing AutoSlay source-contract checks, and
  `RecoveredSource.MatchesInstalledGame` and
  `RecoveredSource.OriginMatchesInstalledGamePck` true for the installed game
  under test, plus RitsuLib manifest/variants/selected-DLL hashes;
- the same package, game version, RitsuLib version, compat branch, patch-count,
  `godot.log.before`, `godot.log.after-launch`, `godot.log.current-iteration`,
  `godot-log-audit.json`, and `sts1-mode-log-check.json` bindings required by
  the current runtime packet checker, with every per-seed artifact retained in
  that seed's `run-####` directory;
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
  -ExpectedPackageVersion v0.1.0-private-beta.87 `
  -ExpectedGameVersion 0.107.0 `
  -ExpectedRitsuLibVersion 0.4.24 `
  -ExpectedRitsuCompatBranch 0.107.0 `
  -ExpectedPatchCount 25 `
  -OutFile "<evidence>\autoslay-packet-check.json" `
  -FailOnMismatch
```

Pass multiple expected Ancient ids as a comma-separated `-ExpectedAncientIds`
value; the verifier splits those tokens so process-launched test wrappers can
exercise the same target-coverage checks as an interactive PowerShell run. A
proof packet must also retain the same target set in `autoslay-plan.json`
`ExpectedAncientIds`; summary-only target coverage is not sufficient.

This verifier is no-launch only. It rejects packets that do not identify
`GameNativeAutoSlay`, do not record structured launcher/mod-hook provenance for
`AutoSlayer.Start(seed, logFile)`, do not bind the retained
`check-local-godot-source-workspace.ps1` report, schema fields, policy flags,
and source-version summary, omit the explicit package/game/Ritsu/patch target
switches, duplicate or drop planned seeds, place any per-seed artifact outside
that seed's `run-####` folder, lack per-seed run-result JSON, clean
pass/failure state, before/after/current Godot log length/SHA256 metadata,
`RuntimeProbeSamplesPath`, clean `MainMenuObservation` and
`RuntimeObservation` records including runtime `LogGrew: true`, parseable
ordered run-result timestamps, `main-menu` and `runtime` probe phases, probe
sample `SampledAt`, `LogExists`, `LogLengthBytes`, and retained
`LogLastWriteTimeUtc` fields, runtime sample `LogLengthBytes` growth beyond
`RuntimeObservation.LogInitialLengthBytes`, unknown process start-time counts,
ambiguous current-process counts, before/after/current Godot log-slice proof,
clean audit recomputation, StS1 mode binding, `EventKind: Ancient` /
`AncientId`, requested `-ExpectedAncientIds` plan and summary coverage, or ordered event-room
traversal markers such as `Entering Event room`, `Detected Ancient event,
clicking through dialogue`, and `Selecting event option:`. Use a smaller
`-MinRuns` only for temporary parser or fixture tests; a real game-native
monkey proof should use the intended proof count plus the intended target
Ancient id coverage. A single-seed fixture packet is not batch proof; the verifier must fail when `-MinRuns` is higher than the retained plan and summary
run count, and it must fail when any requested `-ExpectedAncientIds` value is
missing from the retained summary runs.

If a launched AutoSlay batch fails, triage the retained packet without launching
anything:

```powershell
.\scripts\analyze-spire-plus-runtime-failure.ps1 `
  -EvidenceDir "<evidence>" `
  -OutFile "<evidence>\runtime-failure-analysis.json"
```

For `GameNativeAutoSlay`, the analyzer reads `autoslay-summary.json`, each
per-seed `run-result.json`, `runtime-probe-samples.json`, and sidecar log. It
refuses to route source ownership from `godot.log.current-iteration` unless
`godot.log.before` and `godot.log.after-launch` prove the current slice by exact
bytes and the run-result before/after/current Godot log byte-length/SHA256
metadata matches the retained files. It appends AutoSlay sidecar text to
log-derived owner routing only when `autoslay.log` is retained in the per-seed
run directory and `AutoSlayLogSha256` matches. It also reports missing launcher
invocation, missing or unhealthy runtime probe samples, missing `main-menu` /
`runtime` probe phases, invalid or reversed run-result timestamps, missing or
unhealthy `MainMenuObservation` / `RuntimeObservation`, runtime probe
`LogLengthBytes` drift from `RuntimeObservation.LogGrew`, `EventKind: Ancient`
/ `AncientId`, sidecar log, completion/failure marker, or ordered Ancient event
traversal as `RuntimeHarness` evidence defects first. This makes failed
AutoSlay packets useful for diagnosis, but it still does not turn a failed or
source-only packet into gameplay proof.

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

Small controlled smoke, with only BaseLib, RitsuLib, and Spire Plus enabled:

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
  -ExpectedPackageVersion v0.1.0-private-beta.87 `
  -ExpectedGameVersion 0.107.0 `
  -ExpectedRitsuLibVersion 0.4.24 `
  -ExpectedRitsuCompatBranch 0.107.0 `
  -ExpectedPatchCount 25 `
  -FailOnMismatch
```

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
`iteration-result.json` `GameProcessId`, `GameProcessStartTimeUtc`, and
`GameProcessPath` to match the single positive process identity observed by
`runtime-probe-samples.json` and the live-session-selected game process.
`sts1-mode-log-check.json` to match the plan's `Sts1EventMode` and bind its
`LogPath`, `LogLength`, and `LogSha256` to `godot.log.current-iteration`, exact Spire Plus patch-count lines from
`godot.log.current-iteration`, probe sample paths and sliced-log paths that
point to the retained standard files inside the current iteration folder,
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
`StaleProcessCount > 0`.
A clean packet means those signals stayed healthy for both sampled windows;
the retained `runtime-probe-samples.json` must include `StartupMainMenu` and
`PostCommandRuntime` samples, and those phase counts must match
`MainMenuObservation.Samples` and `RuntimeObservation.Samples`. For
command-bearing iterations, the `PostCommandRuntime` samples' `LogLengthBytes`
must also prove the `RuntimeObservation.LogGrew` claim by exceeding
`RuntimeObservation.LogInitialLengthBytes`. It still does not prove deeper
gameplay behavior.

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
  `LiveSessionPrepareOutputSha256`, `LiveSessionLaunchedAt`,
  `LiveSessionPidAttributionPassed`, `LiveSessionSelectedGameProcessId`,
  `LiveSessionSelectedGameProcessStartTimeUtc`,
  `LiveSessionSelectedGameProcessPath`,
  `GameProcessStartTimeAfterLiveSessionLaunch`,
  `GameProcessIdMatchesLiveSession`,
  `GameProcessStartTimeMatchesLiveSession`, `GameProcessPathMatchesLiveSession`,
  `ResponsivenessProbePassed`, current-slice offset binding, `HangSignals`, and
  `FailureReasonCodes`.
- Each iteration retains `runtime-probe-samples.json` with the sampled
  process/window/log records, including `Phase`, `ProcessId`,
  `ProcessStartTimeUtc`, `ProcessPath`, expected game process identity,
  process-id/start/path match booleans, stale/unknown/ambiguous process counts,
  window state, and responsiveness state. Runtime monkey phases are
  `StartupMainMenu` and `PostCommandRuntime`; the packet checker rejects
  unknown phase values, missing phase coverage, and phase-count drift versus
  the retained observation sample counts.
- `monkey-summary.json` records `FailedIterationIds`, `FailureReasonCounts`,
  `ProcessExitCount`, `LiveSessionBindingMissingCount`,
  `UnresponsiveIterationCount`, `LogStallIterationCount`,
  `StaleProcessObservedCount`, `CommandAckMissingCount`, `CommandCounts`,
  `ScenarioTagCounts`,
  `OwnerAreaCounts`, `VakuuFightIterationCount`, `MaxMainMenuElapsedSeconds`,
  and `MaxSecondsWithoutLogGrowth`.

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
For runtime monkey packets, it treats missing `RuntimeProbeSamplesPath`,
missing/invalid `runtime-probe-samples.json`, missing phase coverage, and
phase-count or runtime log-growth timeline drift as `RuntimeHarness` blockers
before source ownership routing.
When `godot.log.current-iteration` exists, the analyzer requires
`godot.log.before`, `godot.log.after-launch`, and `LogScanOffsetBytes`;
otherwise it reports a `RuntimeHarness` blocker and does not route ownership
from the unbound current slice. When the binding files are present, the analyzer
requires `LogScanOffsetBytes` to equal the before-log byte length, requires
`godot.log.before` to be a byte prefix of `godot.log.after-launch`, and requires
`godot.log.current-iteration` to match the after-launch bytes after that prefix.
Any mismatch is a `RuntimeHarness` blocker and the log is not trusted for owner
routing. If
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
