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
- Current runtime dependency: `STS2-RitsuLib v0.4.16` with `lib\0.107.0`.
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
source or extracted resources.

Use `-RequireCurrentSourceSnapshot -FailOnMismatch` only when a task requires
current-source parity. The current local state is expected to report
`source_version_matches_installed_game` as a warning until `source code/` is
refreshed from the installed `v0.107.0` package. The checker also warns, by
default, about the current GDRE export's failed-script and parse-error counts;
use `-RequireCleanGdreExport` only when those must be release-blocking for the
task.

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
   prepare mode with explicit mod isolation and current-run isolation.
4. Wait for `[Startup] Time to main menu` in `godot.log`.
5. During startup, sample the `SlayTheSpire2` process, main-window
   responsiveness, and `godot.log` length/write time. The runner records the
   pre-launch log length and only accepts a main-menu marker from appended or
   reset log content. It also ignores `SlayTheSpire2` processes that started
   before the iteration. Startup fails if the current process disappears, the
   window reports hung/not responding for `-UnresponsiveSampleThreshold`
   consecutive samples, or the log stops growing before main menu for
   `-NoLogGrowthTimeoutSeconds`.
6. Optionally send one short DevConsole command from the corpus.
7. During the post-command window, sample process/window/log health again.
   Process disappearance or a hung/not-responding window fails the iteration;
   post-main-menu log growth is recorded as telemetry but not required because
   an idle main menu can legitimately stop writing.
8. Copy the full `godot.log` to the iteration folder as forensic context, then
   write `godot.log.current-iteration` from the accepted pre-launch scan offset
   so stale appended log content cannot satisfy or fail the current iteration.
9. Run `scripts\audit-godot-log.ps1` on `godot.log.current-iteration` and
   retain `godot-log-audit.json`.
10. Run `scripts\check-sts1-enabled-mode-runtime-log.ps1` for the requested
   Off/CanaryOnly/AdditiveBatch1 mode and retain `sts1-mode-log-check.json`.
   This uses `godot.log.current-iteration` as truth, not the evidence folder
   name or older log content.
11. Restore the live session with `-StopGameOnRestore` and
   `-PreserveNewCurrentRunsOnRestore`.
12. Write `iteration-result.json` and a root `monkey-summary.json`.

The lane fails an iteration on:

- main-menu timeout;
- `SlayTheSpire2` disappearing during startup or post-command observation;
- the game window reporting hung or not responding for the configured
  consecutive-sample threshold;
- `godot.log` not growing before main menu for the configured no-growth
  timeout;
- missing or empty `godot.log`;
- `audit-godot-log.ps1` release-blocking signature hits;
- package/game/RitsuLib/compat branch/patch-count expectation mismatch;
- actual StS1 mode verifier mismatch;
- DevConsole command failure when commands are enabled;
- failed restore.

This catches hangs and bad startup/runtime logs without depending on a fragile
uncontrolled click bot. Later layers can add true random UI input after the
main-menu/start-run path is stable and after window-focus safeguards are proven.

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
  -ExpectedPackageVersion v0.1.0-private-beta.86 `
  -ExpectedPatchCount 25 `
  -FailOnMismatch
```

If the packet fails, triage it without launching anything:

```powershell
.\scripts\analyze-spire-plus-runtime-failure.ps1 `
  -EvidenceDir .tools\runtime-evidence\<monkey-stability-dir> `
  -OutFile .tools\runtime-evidence\<monkey-stability-dir>\runtime-failure-analysis.json
```

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
`CommandAckPattern` must also match the runner's canonical command pattern.
When a command acknowledgement is required, the checker replays the retained
pattern against `godot.log.current-iteration`; `CommandAckObserved=true` in JSON
is not enough without the source-backed log line. `VakuuFightSmoke` packets must
contain only `vakuu-fight` planned iterations. A 1000-iteration
`AncientUiPlusVakuuFight` round-robin packet must contain exactly 200
`vakuu-fight` planned iterations.
The current `vakuu-fight` command proves only the forced fight option is shown
when the copied log contains `[SPIREPLUS-EVIDENCE] VakuuFight
fight_option_shown`. It is not child-combat proof unless later evidence also
contains `fight_started` and `child_combat_room_entered`.

The launched packet checker requires `MainMenuObservation` and
`RuntimeObservation` in each `iteration-result.json`. These records include
process-observed, process-exited, hung-window, log-observed, log-length, and
max-no-growth counters. It also requires the retained
`sts1-mode-log-check.json`, exact Spire Plus patch-count lines from
`godot.log.current-iteration`, probe sample paths and sliced-log paths that
point to the retained standard files inside the current iteration folder,
`LogScanOffsetBytes` within the copied full log, a `godot.log.current-iteration`
slice that matches `godot.log.after-launch` from that offset, command
acknowledgement patterns that match known built-in command regexes and that
retained slice when required, and no raw probe sample with `Responding=false`.
A clean packet means those signals stayed healthy for the sampled windows; it
still does not prove deeper gameplay behavior.

Current packet schema is `HangProbeSchemaVersion = 1`.

- `monkey-plan.json` records `Scenario`, `CommandSelectionMode`,
  `CommandCorpusSource`, `CommandScenarioMatrix`, `ProcessProbe`,
  `LogGrowthProbe`, and `CommandAckPatterns`.
- Each `iteration-result.json` records `GameProcessId`,
  `GameProcessStartTimeUtc`, `MainWindowObserved`, `MainMenuDetectedAt`,
  `MainMenuElapsedSeconds`, `PreLaunchLogLengthBytes`,
  `MinimumProcessStartTimeUtc`, `LogScanOffsetBytes`,
  `CurrentIterationLogPath`, `CurrentIterationLogCopied`, `ScenarioTag`, `OwnerArea`,
  `CommandSelectionMode`, `LogInitialLengthBytes`,
  `LogFinalLengthBytes`, `LastLogGrowthAt`, `MaxSecondsWithoutLogGrowth`,
  `MaxConsecutiveUnresponsiveSamples`, `StartupLogProbePassed`,
  `PostCommandLogProbePassed`, `CommandAckRequired`, `CommandAckPattern`,
  `CommandAckObserved`,
  `ResponsivenessProbePassed`, current-slice offset binding, `HangSignals`, and
  `FailureReasonCodes`.
- Each iteration retains `runtime-probe-samples.json` with the sampled
  process/window/log records.
- `monkey-summary.json` records `FailedIterationIds`, `FailureReasonCounts`,
  `ProcessExitCount`, `UnresponsiveIterationCount`, `LogStallIterationCount`,
  `CommandAckMissingCount`, `CommandCounts`, `ScenarioTagCounts`,
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

The triage analyzer maps retained signals to owner areas. It reads
`godot.log.current-iteration` first when present, falls back to the full copied
`godot.log.after-launch`, and records the planned `OwnerAreaHint` separately
from `OwnerAreaFromLog` and `OwnerAreaFromCommand`. When `LogScanOffsetBytes` is
available, the analyzer validates the retained current-iteration slice against
the full copied log and reports a `RuntimeHarness` blocker if they disagree; for
owner routing, a valid offset-derived slice is preferred over a stale retained
slice. If `iteration-result.json` is missing or invalid, `monkey-summary.json`
may still provide a fallback row for routing, but the analyzer reports a
`RuntimeHarness` blocker because summary data does not replace the canonical
per-iteration artifact. Empty retained JSON arrays such as `Mismatches`,
`FailureReasonCodes`, `HangSignals`, and `SignatureHits` are treated as empty
signal sets before owner routing. For hung processes, unclassified retained failures, audit hits, Spire
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
