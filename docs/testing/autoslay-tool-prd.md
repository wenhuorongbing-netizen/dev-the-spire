# AutoSlay Tool PRD (headless game-native smoke runner)

Status: implemented (DEBUG track). Owner runs the launcher; this doc is the spec
plus self-review notes. This lane is a smoke runner, not release proof.

## Goal

Provide a fully-automatic, headless, game-native AutoSlay test tool for Spire
Plus that:

1. Re-enables the game's built-in `MegaCrit.Sts2.Core.AutoSlay.AutoSlayer` from
   the Spire Plus mod, WITHOUT modifying the game install, default-OFF and
   env-gated.
2. Ships a PowerShell launcher that sets the env gate, launches the game with
   forwarded args, waits for AutoSlay to finish (log markers + process exit),
   and captures structured evidence under `.tools/runtime-evidence/`.

Non-goals this cycle: full multi-run AutoSlay proof packet (per-seed `run-*`
dirs, runtime-probe PID attribution, ancient-id coverage) demanded by
`scripts/check-spire-plus-autoslay-packet.ps1` in `-FailOnMismatch` proof mode.
The launcher emits the canonical top-level artifact shapes so that checker can
parse them, but a full proof-mode PASS needs the additional multi-run/coverage
fields and is out of scope for a single smoke run.

## Background (verified against local source + RitsuLib 0.4.34)

- `AutoSlayer.Start(string seed, string? logFile = null)` (in
  `source code/src/Core/AutoSlay/AutoSlayer.cs`) plays a full run, logs via
  `AutoSlayLog`, and calls `NGame.Instance.GetTree().Quit(exitCode)` to
  auto-quit. `Start` is `public`. `AutoSlayer.PlayMainMenuAsync` drives from the
  main menu (it clicks Singleplayer -> character select -> confirm), so the main
  menu must already be loaded when `Start` runs.
- Normal trigger is `NGame.cs:296`:
  `if (!IsReleaseGame() && CommandLineHelper.HasArg("autoslay")) { ... new AutoSlayer().Start(seed, logFile); }`.
- `NGame.IsReleaseGame()` (`NGame.cs:362`) is hardcoded `return true` on the
  Steam build, so the built-in trigger never fires. Patching that method is
  timing-fragile because the check runs during NGame init, possibly before the
  mod's Harmony patches apply.
- `AutoSlayLog` markers we key on:
  - start:     `Starting run with seed=<seed>`
  - completed: `Run completed successfully with seed=<seed>`
  - failed:    `Run failed with seed=<seed>`
- Headless: `NGame.cs:372` checks `DisplayServer.GetName() == "headless"` and
  skips display settings; the game supports `--headless`.

### RitsuLib lifecycle API (decisive)

RitsuLib 0.4.34 exposes a lifecycle event bus on `STS2RitsuLib.RitsuLibFramework`:

- `static IDisposable SubscribeLifecycleOnce<TEvent>(Action<TEvent> handler, bool replayCurrentState = true)`
- `static IDisposable SubscribeLifecycle<TEvent>(Action<TEvent> handler, bool replayCurrentState = true)`

Event payloads include `STS2RitsuLib.MainMenuReadyEvent` and
`STS2RitsuLib.GameReadyEvent`, both implementing
`IReplayableFrameworkLifecycleEvent`. With `replayCurrentState: true`, if the
event already fired before the mod subscribed, the handler is invoked
immediately on subscribe. This removes the patch-timing risk entirely.

## Decision: hook approach

Use the PREFERRED mod-driven approach (no game-install change, no fragile
patch-timing):

- In `MainFile.Initialize()`, after RitsuLib bootstrap, call
  `SpirePlusAutoSlayDriver.TryArm(...)`.
- `TryArm` checks the env gate (`SPIREPLUS_ENABLE_AUTOSLAY` truthy). If unset =>
  return immediately, register nothing, zero normal-play impact.
- If armed, it subscribes via
  `RitsuLibFramework.SubscribeLifecycleOnce<MainMenuReadyEvent>(_ => new AutoSlayer().Start(seed, logFile), replayCurrentState: true)`.
  `MainMenuReadyEvent` guarantees the main menu is loaded, which is exactly the
  precondition `AutoSlayer.PlayMainMenuAsync` needs.

Why not patch `IsReleaseGame()`: timing fragility (documented above). The
mod-driven path with `replayCurrentState: true` is timing-safe and is the
research-preferred option.

### Default-OFF + testability split

`SubscribeLifecycleOnce` / `AutoSlayer` reference RitsuLib + game assemblies that
the unit-test project does not load (the test project compiles selected mod
`.cs` files only). So the gating + seed/log resolution is extracted into a pure,
dependency-free class:

- `SpirePlusAutoSlayGate` (no game/RitsuLib using-directives): reads env vars,
  resolves seed (env `SPIREPLUS_AUTOSLAY_SEED` or `--seed` CLI) and log path (env
  `SPIREPLUS_AUTOSLAY_LOG` or `--log-file` CLI), exposes `IsEnabled` and
  `Resolve(...)`. Unit-tested directly.
- `SpirePlusAutoSlayDriver` (references RitsuLib + `AutoSlayer`): thin wiring that
  calls the gate, then subscribes the lifecycle handler. Source-guarded by test.

The CLI fallback uses `MegaCrit.Sts2.Core.Helpers.CommandLineHelper` inside the
driver; the gate accepts an injectable CLI resolver so it stays game-free for
tests.

## Env contract (all default-OFF)

| Var | Meaning |
| --- | --- |
| `SPIREPLUS_ENABLE_AUTOSLAY` | `1`/`true`/`yes`/`on` arms the driver. Unset/other => disabled. |
| `SPIREPLUS_AUTOSLAY_SEED` | Seed override. Falls back to `--seed` CLI, else a random seed. |
| `SPIREPLUS_AUTOSLAY_LOG` | autoslay.log path. Falls back to `--log-file` CLI, else null (no file log). |

## Launcher: `scripts/run-spire-plus-autoslay.ps1`

Params: `-Seed` (default fixed reproducible seed `SPIREPLUS-AUTOSLAY-0001`),
`-Headless` (default `$true`), `-GameRoot`/`-SteamExe` (default to
`spire-plus-live-session.ps1` values; Sts2 path also in `Directory.Build.props`),
`-EvidenceRoot`, `-TimeoutMinutes` (default 30), `-DirectExe` (force the
direct-exe launch fallback), `-SteamUserId`.

Behavior:

1. Resolve evidence dir `.tools/runtime-evidence/autoslay-<yyyyMMdd-HHmmss>/` and
   a per-seed `run-<seed-slug>/` subdir.
2. Set env `SPIREPLUS_ENABLE_AUTOSLAY=1`, `SPIREPLUS_AUTOSLAY_SEED=<seed>`,
   `SPIREPLUS_AUTOSLAY_LOG=<runDir>\autoslay.log` for the child process.
3. Launch. Default Steam forwarding:
   `& $SteamExe -applaunch 2868840 --headless autoslay --seed <seed> --log-file <runDir>\autoslay.log`
   (args after the appId forward to the game; `--headless` + `autoslay` keep
   compatibility with the built-in CLI path even though the mod hook is what
   actually arms AutoSlay). Direct-exe fallback (`-DirectExe` or no Steam exe):
   `& <GameRoot>\SlayTheSpire2.exe --headless autoslay --seed <seed> --log-file <runDir>\autoslay.log`.
4. Wait up to `-TimeoutMinutes` for EITHER the autoslay.log completion marker
   (`Run completed successfully with seed=<seed>`) / failure marker
   (`Run failed with seed=<seed>`) OR the launched game process exit. Hard
   timeout enforced; on timeout, attempt to stop the launched process.
5. Capture into the run dir: `autoslay.log`, `godot.log` (copied from
   `%APPDATA%\SlayTheSpire2\logs\godot.log`), `run-result.json`,
   `autoslay-launcher-proof.json`; and into the evidence dir: `autoslay-plan.json`,
   `autoslay-summary.json`. Also run `audit-godot-log.ps1` for a clean log audit.
6. Report PASS (completion marker seen + exit 0 + clean audit) or BLOCKED (exact
   failure + the autoslay.log/godot.log paths).

Process attribution reuses the start-time + single-process technique from
`run-spire-plus-monkey-stability.ps1` / `spire-plus-live-session.ps1`.

### Emitted artifact shapes (checker-compatible top level)

- `autoslay-plan.json`: `SchemaVersion=1`, `RunnerKind="GameNativeAutoSlay"`,
  `Invocation`/`InvocationCommand` containing the literal substring
  `AutoSlayer.Start(seed, logFile)`, `LauncherKind`, `LauncherPath`,
  `LauncherSha256`, `HookId`, `HookAssembly`, `Seeds` (array), `Sts1EventMode`,
  `ExpectedPatchCount`, package/game/ritsu versions.
- `autoslay-summary.json`: `SchemaVersion=1`, `Passed`, `TotalRuns`,
  `FailedRuns`, `Runs[]` (seed + per-run result/log paths).
- `run-result.json` (per seed): `SchemaVersion=1`, `Launch=true`,
  `RunnerKind="GameNativeAutoSlay"`, `Invocation`/`InvocationCommand` with the
  `AutoSlayer.Start(seed, logFile)` literal, `Seed`, `Passed`, `ExitCode`,
  `FailureReasonCodes` (array), `HangSignals` (array), floors reached, process
  id/path/start time, log paths.
- `autoslay-launcher-proof.json`: the exact launch file path + argument list +
  env gate values + resolved hook command, for audit.

## Self-review / risks to verify on first real run

1. Headless compatibility of AutoSlay handlers: AutoSlayer drives Godot UI nodes
   via `UiHelper.Click`. The devs ship this as headless smoke, but headless click
   routing is the single biggest unknown. If a handler hangs, the watchdog/run
   timeout fails the run and the launcher reports BLOCKED with the autoslay.log.
2. Steam arg forwarding: `-applaunch <appId> <args...>` forwarding is assumed.
   If args do not reach the game (no `--headless`/autoslay in godot.log), use
   `-DirectExe`. The mod hook does not require the CLI args (it is env-gated), so
   even if forwarding drops args, the env gate still arms AutoSlay; `--headless`
   is the part that matters for true headless and is the main forwarding risk.
3. `AutoSlayer.Start` callability/timing: confirmed `public`; armed on
   `MainMenuReadyEvent` so the menu precondition holds. `replayCurrentState:true`
   covers the already-fired case.
4. Single-process attribution: a stale `SlayTheSpire2` process poisons the shared
   `godot.log`. The launcher refuses ambiguous attribution and reports BLOCKED.
5. Default-OFF proof: guard test asserts the gate is disabled when the env var is
   unset and that the driver wiring is behind the gate.
