# StS1 Event Port Strict Audit v20 - beta.92 / v0.107.1 RitsuLib-Only Current Loader Truth, June Dev Spec, Subagent, and Overnight Run

日期：2026-06-18
项目：`dev-the-spire` / `Spire Plus` / technical id `EZMicroBalance`
审查对象：助理关于“将《杀戮尖塔 1》事件迁移到 StS2 Mod”的当前工作状态。
最新证据基线：GitHub 当前 `README.md`、`docs/reviews/current-validation.md`、`docs/features/sts1-events/status-board.md`。

---

## 0. 总结论

**未完成。**

当前项目相比 v19 又有实质进展：beta.92 在 StS2 `v0.107.1` + STS2-RitsuLib `v0.4.29` 下的 **RitsuLib-only Off loader proof 和 AdditiveBatch1 enabled-mode loader/registration proof 已 clean**，并且当前 Spire Plus 交付面不再依赖 BaseLib。beta.85/beta.87/beta.88/beta.90 证据只保留为 previous-package、previous-game-version 或 previous-dependency context。

但这仍不是 StS1 runtime parity。当前仍缺：

```text
4 canary event gameplay proof
6 simple batch gameplay proof
save/load proof
EN/ZHS runtime render proof
image/license/render proof
ReplacementPrototype functional proof
multiplayer/fail-closed proof
independent QA pass
release handoff proof
```

当前最准确状态：

```text
Source/test/static guard: strong progress
Retained beta.85 default-Off loader: previous-package/game-version pass
Retained beta.85 CanaryOnly enabled-mode loader: previous-package/game-version pass
Retained beta.87 AdditiveBatch1 enabled-mode loader/registration: previous-package/game-version pass
Current beta.92 RitsuLib-only Off/AdditiveBatch1 loader/registration: pass for v0.107.1 with STS2-RitsuLib v0.4.29 and no Spire Plus BaseLib dependency
Gameplay parity: blocked / unverified
Release-ready/live-ready: no
Full StS1 experience: no
```

禁止写：

```text
All tasks complete
All StS1 events complete
Full parity
Gameplay-ready
Release-ready
和杀戮尖塔1完全一样
```

### 0.1 Coordination pause boundary

While the same-repository migration validation lane is active, this event goal must not start new `dotnet build`, `dotnet test`, `dotnet publish`, package/release-evidence validation, game/runtime smoke, staging, commit, or push processes from this thread.

Allowed work during the pause is read-only/static checking, documentation/guard alignment, and no-resource/no-code governance cleanup that does not require build, publish, package, or version-bump validation.

Runtime, gameplay, QA, build/test/publish, package/release-evidence, staging, commit, and push instructions below apply only after the coordination pause is explicitly lifted. During the coordination pause, do not treat static or source-only work as closing runtime gates.

### 0.2 Current beta.92 override

As of 2026-06-21, current loader truth is beta.92 on Slay the Spire 2 `v0.107.1`: `.tools/runtime-evidence/v01071-beta92-ritsulib0429-off-direct-20260621/` and `.tools/runtime-evidence/v01071-beta92-ritsulib0429-additivebatch1-direct-20260621/` reached main menu with exactly STS2-RitsuLib `v0.4.29` and Spire Plus `v0.1.0-private-beta.92` loaded, no Spire Plus BaseLib dependency, 25/25 Spire Plus patches applied, clean audits, Off packet verifier 43 / 0, AdditiveBatch1 enabled-mode verifier 31 / 0, and AdditiveBatch1 packet verifier 61 / 0. Treat beta.85, beta.86, beta.87, beta.88, and beta.90 loader lines below as retained previous-package or previous-dependency context unless a line explicitly names beta.92 evidence.

### 0.3 Historical beta.88 previous BaseLib-backed context

As of 2026-06-19, beta.88 was the previous BaseLib-backed loader truth on Slay the Spire 2 `v0.107.1`: `.tools/runtime-evidence/v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937/` reached main menu with BaseLib `v3.3.0`, STS2-RitsuLib `v0.4.24`, Spire Plus `v0.1.0-private-beta.88`, 25/25 Spire Plus patches applied, AdditiveBatch1 10 event types / 14 registration calls, clean audit, enabled-mode verifier 31 / 0, and runtime packet verifier 0 mismatches. It is now superseded by the beta.92 RitsuLib-only active loader truth above. Treat beta.85, beta.86, beta.87, beta.88, and beta.90 loader lines below as retained previous-package, previous-game-version, or previous-dependency context unless a line explicitly names beta.92 evidence.

This beta.88 packet is loader/registration proof only. It still does not close event gameplay, clicked UI, save-load, EN/ZHS runtime render, image/license/render, replacement functional behavior, multiplayer/fail-closed, independent QA, game-native AutoSlay batch proof, release, or tester handoff gates.

Earlier pause-safe static checkpoint after the runtime-monkey AutoSlay boundary/source-contract, packet-verifier, and analyzer hardening pass: `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 1025 checks / 0 mismatches after the v20 subagent coverage, status-board header, current-doc summary, optional no-launch preflight script, test-plan preflight prerequisite, read-only preflight guards, PROJECT_STATE static-summary alignment, active current-guidance route alignment, historical-review current-route alignment, v20 hard-stop report, v20 O76-O84 final-gate overlay, tuple-aware enabled-mode log verifier guards, CanaryOnly current-pass guard, repo-manifest runtime-preflight drift guard, beta.86 AdditiveBatch1 doc alignment, retained-loader subagent split, current pause-state snapshot alignment, current diff-check wording, retained current-slice binding guards, game-native AutoSlay source-contract boundary, game-native AutoSlay packet-verifier run-result routing/current-slice/audit/StS1-mode/patch-count/launcher/AncientId docs/scripts, AutoSlay runtime-failure analyzer run-result routing, AutoSlay probe phase/timestamp packet/analyzer guards, and runtime `RuntimeLogGrowthRequired` / command-bearing `LogGrew` / no-log-growth-timeout guards were guarded. The then beta.86-target read-only `scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch` returned 27 checks / 0 mismatches because both the repo and installed `EZMicroBalance.json` reported `v0.1.0-private-beta.87`; `scripts/check-local-godot-source-workspace.ps1` returned 50 checks / 0 mismatches with all AutoSlay contract checks passing and 3 retained warnings (`source version=v0.106.0 installed version=v0.107.0`, GDRE failed scripts=18, GDRE parse errors=1); the retained previous synthetic rich AutoSlay packet checkpoint returned 171 checks / 0 mismatches through `scripts/check-spire-plus-autoslay-packet.ps1` before the later probe phase/timestamp guard additions; `scripts/check-sts1-event-static-suite.ps1` returned 15 static steps / 0 suite failures with the known 33-key localization gap; `scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 12 checks / 0 mismatches; `scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 534 checks / 0 mismatches; `scripts/check-sts1-v20-final-gate-overlay.ps1 -FailOnMismatch` returned 29 checks / 0 mismatches; `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 70 checks / 0 mismatches; focused `git diff --check --` on the touched event-governance docs and guard script exited 0 with only CRLF warnings. This is historical static/preflight evidence and does not itself close gameplay, save/load, replacement, multiplayer, QA, game-native AutoSlay batch proof, release, or handoff gates.

Latest beta.92 governance checkpoint after the gate-ledger/current-doc cleanup: `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 1324 checks / 0 mismatches; `scripts/check-sts1-event-static-suite.ps1` returned 15 static steps / 0 suite failures; `scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 12 checks / 0 mismatches; `scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 535 checks / 0 mismatches; and `scripts/check-local-godot-source-workspace.ps1 -RequireCurrentSourceSnapshot -ExpectedPackageVersion v0.1.0-private-beta.92 -ExpectedRitsuLibVersion 0.4.29 -ExpectedRitsuCompatBranch 0.107.1 -FailOnMismatch` returned 60 checks / 0 mismatches with two retained GDRE warnings. This is static/no-launch evidence only and does not close gameplay, save-load, replacement, multiplayer, QA, game-native AutoSlay batch proof, release, or handoff gates.

Latest pause-safe AutoSlay target-coverage follow-up: `check-spire-plus-autoslay-packet.ps1` now accepts comma-separated `-ExpectedAncientIds` values, requires `-ExpectedAncientIds` in `-FailOnMismatch` proof mode, requires the same target set in `autoslay-plan.json` `ExpectedAncientIds`, and fails a retained packet when summary runs or traversed sidecar-plus-current-log event evidence do not cover every requested Ancient id. Minimal no-launch packet probes confirmed a matching `VAKUU,URDA` plan target set passes the new `plan_expected_ancient_ids_*` checks, an omitted `URDA` plan target fails `plan_expected_ancient_ids_match_parameter`, a missing proof-mode target switch fails `expected_ancient_ids_required_for_proof_mode`, and a matching `VAKUU,URDA` plan with only `VAKUU` observed fails `summary_expected_ancient_ids_observed` with `ExpectedAncientIds missing=URDA`; a target Ancient id must also be bound to `Selecting event option: <AncientId>` in both the AutoSlay sidecar and current-iteration Godot log before it counts as traversed proof. This improves future game-native monkey proof quality but remains static/verifier evidence only; it still does not close gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe AutoSlay AncientId normalization/proof-mode/summary-count follow-up: `check-spire-plus-autoslay-packet.ps1` now normalizes expected, plan, summary, and traversed Ancient ids to uppercase for target-coverage comparison so future game-native packets are not rejected solely because the native/log packet uses `Urda` while the proof command uses `URDA`. The per-run `run-result.json` versus `autoslay-summary.json` AncientId self-consistency check remains exact; proof packets must retain `autoslay-summary.json` `AncientIdCounts` whose normalized keys and non-negative integer values exactly match `Runs[].AncientId` aggregation, give every requested target Ancient id a positive count, reject extra zero-count Ancient ids that never appeared in `Runs[]`, reject non-positive `-MinRuns`, and select the retained Ancient id inside the ordered event-room sequence instead of only mentioning it elsewhere in the log slice; `-AllowMissingEventTraversal` is guarded as incompatible with `-FailOnMismatch` proof-mode verification; and active current-doc proof commands now require exact current beta.92 target values for `-MinRuns 1000`, exact `-ExpectedAncientIds VAKUU,URDA,MORVI,LOTHA`, package/game/Ritsu/patch switches, retained `-OutFile`, and no `-AllowMissingEventTraversal` bypass. `RuntimeMonkeyStabilityGuardTests` now includes no-launch verifier fixtures for mixed-case plan/summary/run-result Ancient ids, proof-mode failure for `-AllowMissingEventTraversal`, mismatched `AncientIdCounts`, extra zero-count AncientId summary keys, non-positive `MinRuns`, and stale pre-run selection before a wrong actual Ancient selection, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks and current-doc claims 1148 / 0. This remains verifier/test hardening only and does not close game-native AutoSlay batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe AutoSlay summary batch-metadata binding follow-up: `check-spire-plus-autoslay-packet.ps1` now requires top-level `autoslay-summary.json` `RunnerKind`, `Sts1EventMode`, package/game/Ritsu targets, `ExpectedPatchCount`, and `ExpectedAncientIds` to match retained `autoslay-plan.json` before batch proof can be trusted. `RuntimeMonkeyStabilityGuardTests.GameNativeAutoSlay` has a no-launch stale-summary batch-metadata fixture, and `docs/testing/runtime-monkey-stability.md` documents the rule, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1241 / 0, static suite 15 / 0, static-file hygiene 12 / 0, `git diff --check` with CRLF warnings only, batch classifier 25 dirty / 0 unclassified, and a synthetic no-launch AutoSlay packet proving matching metadata passes while stale summary package/patch/Ancient-target metadata fails. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe AutoSlay analyzer summary-plan binding follow-up: `analyze-spire-plus-runtime-failure.ps1` now treats top-level `autoslay-summary.json` batch metadata drift versus retained `autoslay-plan.json` as a `RuntimeHarness` blocker and clears AutoSlay run/log trust before owner routing. `RuntimeMonkeyStabilityGuardTests` has a no-launch stale-summary-plan AutoSlay analyzer fixture, and `docs/testing/runtime-monkey-stability.md` documents the rule, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1264 / 0, static suite 15 / 0, static-file hygiene 12 / 0, `git diff --check` with CRLF warnings only, batch classifier 18 dirty / 0 unclassified, and a synthetic no-launch analyzer probe proving stale summary package/patch/Ancient-target metadata routes to `HarnessEvidenceInvalid` with AutoSlay run trust cleared. No build, test, publish, package/release-evidence validation, runtime smoke, game-native AutoSlay/monkey batch, gameplay, staging, commit, or push was run in this follow-up.

Latest pause-safe runtime-monkey packet native-array-shape follow-up: `check-spire-plus-runtime-monkey-packet.ps1` now fails closed when retained `monkey-plan.json` `CommandCorpus` / `PlannedCommands`, `monkey-summary.json` `Results[]` / `FailedIterationIds`, per-iteration result signal arrays, live-session process-id arrays, prepare-output process-id arrays, summary signal arrays, or retained `runtime-probe-samples.json` are scalar/object/string evidence instead of native JSON arrays. `RuntimeMonkeyStabilityGuardTests.PacketArrayShape` adds a no-launch malformed per-iteration fixture, and `docs/testing/runtime-monkey-stability.md` documents the retained-array requirement, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1264 / 0, static suite 15 / 0, static-file hygiene 12 / 0, `git diff --check` with CRLF warnings only, and batch classifier 16 dirty / 0 unclassified. No build, test, publish, package/release-evidence validation, runtime smoke, game-native AutoSlay/monkey batch, gameplay, staging, commit, or push was run in this follow-up.

Latest pause-safe runtime-failure analyzer summary array-shape follow-up: `analyze-spire-plus-runtime-failure.ps1` now records `RuntimeHarness` blockers and clears owner-routing trust when retained runtime-monkey `monkey-summary.json` `Results` / `FailedIterationIds` shapes are not native JSON arrays, and when GameNativeAutoSlay `autoslay-summary.json` `Runs` is not a native JSON array. `RuntimeMonkeyStabilityGuardTests.AnalyzerArrayShape` has no-launch fixtures for both malformed summary shapes, and `docs/testing/runtime-monkey-stability.md` documents the analyzer trust boundary, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed after the later launcher-provenance guard alignment: parser checks, current-doc claims 1264 / 0, static suite 15 / 0, static-file hygiene 12 / 0, `git diff --check` with CRLF warnings only, and batch classifier 16 dirty / 0 unclassified. No build, test, publish, package/release-evidence validation, runtime smoke, game-native AutoSlay/monkey batch, gameplay, staging, commit, or push was run in this follow-up.

Latest pause-safe AutoSlay analyzer launcher-provenance follow-up: `analyze-spire-plus-runtime-failure.ps1` now records `autoslay_launcher_provenance_mismatch` as a `RuntimeHarness` blocker when GameNativeAutoSlay `autoslay-plan.json` and `run-result.json` do not retain matching launcher/mod-hook provenance, a retained launcher proof artifact under the evidence directory, a matching `LauncherSha256`, and an `InvocationCommand` that calls `AutoSlayer.Start(seed, logFile)`. `RuntimeFailureAnalyzerRejectsGameNativeAutoSlayLauncherProvenanceDrift` is the no-launch fixture for stale launcher proof hash routing, and `docs/testing/runtime-monkey-stability.md` documents the analyzer trust boundary, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1264 / 0, static suite 15 / 0, static-file hygiene 12 / 0, `git diff --check` with CRLF warnings only, and batch classifier 16 dirty / 0 unclassified. No build, test, publish, package/release-evidence validation, runtime smoke, game-native AutoSlay/monkey batch, gameplay, staging, commit, or push was run in this follow-up.

Latest pause-safe AutoSlay analyzer trust-closure follow-up: `analyze-spire-plus-runtime-failure.ps1` now clears `LogTextTrustedForOwner` and `OwnerAreaFromLog` for GameNativeAutoSlay evidence when final run, probe, sidecar, audit, or StS1 artifact trust is revoked after the initial slice binding. The existing no-launch AutoSlay analyzer fixture now asserts stale sidecar, audit, and StS1 evidence cannot leave log-derived owner routing trusted, and `docs/testing/runtime-monkey-stability.md` documents that closure. The current-doc checker still requires active current docs plus `docs/testing/runtime-monkey-stability.md` to retain at least one recognized `check-spire-plus-autoslay-packet.ps1 -FailOnMismatch` proof command, keeps the quoted-path recognizer guard, keeps the AutoSlay analyzer summary-plan missing-target-field guard, keeps AutoSlay and runtime-monkey proof-mode current-target parameter checks, keeps AutoSlay packet exact top-level standard per-seed artifact path checks, keeps runtime-monkey live-session child EvidenceDir binding checks, and now keeps runtime-monkey summary prepare-output path/hash binding checks plus no-launch fixtures that drift those fields. Pause-safe checks passed: parser checks and current-doc claims 1314 / 0. No build, test, publish, package/release-evidence validation, runtime smoke, game-native AutoSlay/monkey batch, gameplay, staging, commit, or push was run in this follow-up.

Latest pause-safe runtime-monkey iteration coverage follow-up: `check-spire-plus-runtime-monkey-packet.ps1` now requires `monkey-plan.json` `PlannedCommands[].Iteration` and `monkey-summary.json` `Results[].Iteration` to be positive, unique, and to cover the exact resolved `1..N` iteration set. This prevents a duplicate row, such as two `Iteration: 1` entries with missing `Iteration: 2`, from satisfying a batch-count proof. `RuntimeMonkeyStabilityGuardTests` now has a no-launch duplicate/missing iteration fixture, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1152 / 0, static suite 15 / 0, static-file hygiene 12 / 0, `git diff --check` with CRLF warnings only, batch classifier 0 unclassified, and a synthetic no-launch packet proving the four new iteration-coverage checks fail on the forged duplicate/missing packet. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey patch-count binding follow-up: `check-spire-plus-runtime-monkey-packet.ps1` now requires a positive retained `monkey-plan.json` `ExpectedPatchCount`, requires it to match `-ExpectedPatchCount` when that switch is supplied, and otherwise uses the retained plan value as the effective expected patch count for `godot.log.current-iteration` checks. This prevents omitted verifier parameters from bypassing Spire Plus patch-count proof when the packet itself records the target. `RuntimeMonkeyStabilityGuardTests` now has a no-launch omitted-parameter/stale-plan patch-count fixture, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1156 / 0, static suite 15 / 0, static-file hygiene 12 / 0, `git diff --check` with CRLF warnings only, batch classifier 0 unclassified, and a synthetic no-launch packet proving the retained plan value drives patch-count failure when it disagrees with the retained log. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey runner binding follow-up: `run-spire-plus-monkey-stability.ps1` now writes `RunnerScriptPath` and `RunnerScriptSha256` into `monkey-plan.json`, and `check-spire-plus-runtime-monkey-packet.ps1` rejects packets whose retained runner path or SHA256 does not match the current repo runner. This prevents a future monkey packet from proving only a plan-shaped JSON document without proving which controlled runner produced it. `RuntimeMonkeyStabilityGuardTests` now has a no-launch stale-runner-hash fixture, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks and current-doc claims 1164 / 0. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey command-corpus binding follow-up: `run-spire-plus-monkey-stability.ps1` now writes `command-corpus.txt` before `monkey-plan.json` and records `CommandCorpusPath` plus `CommandCorpusSha256`; `check-spire-plus-runtime-monkey-packet.ps1` requires the retained file to stay under the evidence root, match its hash, and have lines exactly matching `monkey-plan.json` `CommandCorpus`. This prevents a future monkey packet from using one command corpus in its plan while retaining a different command file beside it. `RuntimeMonkeyStabilityGuardTests` now has a no-launch stale-command-corpus fixture, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1164 / 0, and a synthetic no-launch packet proving matching corpus evidence passes while a tampered file fails hash/content checks. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey per-iteration command-file binding follow-up: `check-spire-plus-runtime-monkey-packet.ps1` now requires every launched iteration to retain `iteration-000N/command.txt` as a single command line and checks that it matches the corresponding `monkey-plan.json` `PlannedCommands`, `iteration-result.json`, and `monkey-summary.json` command fields. This prevents a future monkey packet from passing with JSON rows for one command while retaining a different per-iteration command file. `RuntimeMonkeyStabilityGuardTests` now has a no-launch stale-iteration-command fixture, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1168 / 0, and a synthetic no-launch packet proving matching per-iteration command evidence passes while a tampered file fails plan/result/summary checks. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey per-iteration command-file hash binding follow-up: `run-spire-plus-monkey-stability.ps1` now records `CommandFilePath` and `CommandFileSha256` in each `iteration-result.json`, and `check-spire-plus-runtime-monkey-packet.ps1` requires that path to point to the retained `iteration-000N/command.txt` file and that hash to match the retained file. This closes the remaining gap where a future packet could retain a command file without proving the runtime result was bound to that exact file. `RuntimeMonkeyStabilityGuardTests` now asserts the clean path/hash pass and that a tampered command file fails the retained-file SHA256 check, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1172 / 0, and a synthetic no-launch packet proving matching command-file path/hash evidence passes while a tampered file fails hash and plan/result/summary checks. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey summary command-file hash binding follow-up: `check-spire-plus-runtime-monkey-packet.ps1` now requires `monkey-summary.json` `Results[]` command-file path and SHA256 to match the corresponding `iteration-result.json` fields. This prevents a future batch summary from drifting away from the per-iteration command artifact while still looking complete at the batch level. `RuntimeMonkeyStabilityGuardTests` now has a no-launch stale-summary-command-file-hash fixture, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1176 / 0, and a synthetic no-launch packet proving matching summary path/hash evidence passes while a tampered summary hash fails. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey summary batch-metadata binding follow-up: `run-spire-plus-monkey-stability.ps1` now writes top-level `monkey-summary.json` batch metadata for scenario, command-selection mode, StS1 mode, expected package/game/Ritsu targets, and expected patch count; `check-spire-plus-runtime-monkey-packet.ps1` requires those fields to match `monkey-plan.json`; `RuntimeMonkeyStabilityGuardTests` has a no-launch stale-summary batch-metadata fixture; and `docs/testing/runtime-monkey-stability.md` documents the rule, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1241 / 0, static suite 15 / 0, static-file hygiene 12 / 0, `git diff --check` with CRLF warnings only, and batch classifier 20 dirty / 0 unclassified. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey analyzer summary-plan binding follow-up: `analyze-spire-plus-runtime-failure.ps1` now treats top-level `monkey-summary.json` batch metadata drift versus retained `monkey-plan.json` as a `RuntimeHarness` blocker and clears runtime-monkey run/log trust before owner routing. `RuntimeMonkeyStabilityGuardTests` has a no-launch stale-summary-plan analyzer fixture, and `docs/testing/runtime-monkey-stability.md` documents the rule, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1241 / 0, static suite 15 / 0, static-file hygiene 12 / 0, `git diff --check` with CRLF warnings only, batch classifier 23 dirty / 0 unclassified, and a synthetic no-launch analyzer probe proving stale summary scenario/patch-count metadata routes to `HarnessEvidenceInvalid` with runtime-monkey run/log trust cleared. This remains analyzer/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey restore summary counter binding follow-up: `check-spire-plus-runtime-monkey-packet.ps1` now requires `monkey-summary.json` restore aggregate counters for item-count mismatch, preserved-current-run manifest missing, restore leak, restore hash mismatch, and selected-process-not-stopped to be zero for a clean packet and to match `Results[].FailureReasonCodes` aggregation. This prevents a future game-native monkey batch from retaining clean per-iteration restore evidence while the batch summary reports stale or contradictory restore counters. `RuntimeMonkeyStabilityGuardTests` now has a no-launch stale-restore-summary-counter fixture, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1180 / 0, and a synthetic no-launch packet proving matching restore summary counters pass while a tampered restore leak counter fails both zero and result-aggregation checks. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey summary max telemetry binding follow-up: `check-spire-plus-runtime-monkey-packet.ps1` now recomputes `monkey-summary.json` `MaxMainMenuElapsedSeconds`, `MaxSecondsWithoutLogGrowth`, and `MaxConsecutiveUnresponsiveSamples` from `Results[]` and rejects stale batch-level telemetry. This prevents a future game-native monkey batch from retaining trustworthy per-iteration timings while the batch summary underreports or overreports stability indicators. `RuntimeMonkeyStabilityGuardTests` now has a no-launch stale-summary-max-telemetry fixture, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1185 / 0, and a synthetic no-launch packet proving matching max telemetry passes while a tampered `MaxSecondsWithoutLogGrowth` fails result-aggregation binding. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey analyzer missing-plan binding follow-up: `analyze-spire-plus-runtime-failure.ps1` now treats runtime-monkey batch EvidenceDir with `monkey-summary.json` but no parseable `monkey-plan.json` `PlannedCommands` as a `RuntimeHarness` blocker and clears runtime-monkey run/log trust before owner routing. `RuntimeMonkeyStabilityGuardTests` has a no-launch missing-plan analyzer fixture, and `docs/testing/runtime-monkey-stability.md` documents the rule, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1241 / 0, static suite 15 / 0, static-file hygiene 12 / 0, batch classifier 18 dirty / 0 unclassified, and a synthetic no-launch analyzer probe proving the missing-plan signal routes to `HarnessEvidenceInvalid` with runtime-monkey run/log trust cleared. This remains analyzer/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey analyzer plan-result binding follow-up: `analyze-spire-plus-runtime-failure.ps1` now treats retained `monkey-plan.json` `PlannedCommands` row drift versus canonical `iteration-result.json` command, command index, command-selection mode, scenario, owner, and acknowledgement pattern fields as a `RuntimeHarness` blocker and clears runtime-monkey run/log trust before owner routing. `RuntimeMonkeyStabilityGuardTests` has a no-launch stale-plan-result analyzer fixture, and `docs/testing/runtime-monkey-stability.md` documents the rule, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks and current-doc claims 1241 / 0. This remains analyzer/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey analyzer summary-result binding follow-up: `analyze-spire-plus-runtime-failure.ps1` now treats `monkey-summary.json` `Results[]` row drift versus canonical `iteration-result.json` command, owner, failure/hang, runtime-probe, and live-session binding fields as a `RuntimeHarness` blocker and clears runtime-monkey run/log trust before owner routing. `RuntimeMonkeyStabilityGuardTests` has a no-launch stale-summary-result analyzer fixture, and `docs/testing/runtime-monkey-stability.md` documents the rule, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks and current-doc claims 1241 / 0. This remains analyzer/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey analyzer summary-counter binding follow-up: `analyze-spire-plus-runtime-failure.ps1` now treats `monkey-summary.json` top-level counter drift versus `Results[]` aggregation as a `RuntimeHarness` blocker and clears runtime-monkey run/log trust before owner routing. `RuntimeMonkeyStabilityGuardTests` has a no-launch stale-summary counter-drift analyzer fixture, and `docs/testing/runtime-monkey-stability.md` documents the rule, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks and current-doc claims 1241 / 0. This remains analyzer/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey summary failure-counter binding follow-up: `check-spire-plus-runtime-monkey-packet.ps1` now requires `monkey-summary.json` top-level failed-iteration ids, failure-reason maps, process-exit, main-window, live-session-binding, log-missing, unresponsive, stale-process, log-stall, and command-ack counters to match the aggregation recomputed from `Results[]`. `RuntimeMonkeyStabilityGuardTests` has a no-launch stale-summary failure-counter fixture, and `docs/testing/runtime-monkey-stability.md` documents the rule, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks and current-doc claims 1241 / 0. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey summary runtime-probe binding follow-up: `check-spire-plus-runtime-monkey-packet.ps1` now requires `monkey-summary.json` `Results[]` `RuntimeProbeSamplesPath` and `RuntimeProbeSamplesSha256` to match the canonical `iteration-result.json` fields. `RuntimeMonkeyStabilityGuardTests` has a no-launch stale-summary runtime-probe fixture, and `docs/testing/runtime-monkey-stability.md` documents the rule, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks and current-doc claims 1241 / 0. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey summary live-session state binding follow-up: `check-spire-plus-runtime-monkey-packet.ps1` now requires `monkey-summary.json` `Results[]` `LiveSessionSessionStatePath`, `LiveSessionSessionStateSha256`, `LiveSessionRestoreStatePath`, and `LiveSessionRestoreStateSha256` to match the canonical `iteration-result.json` fields. `RuntimeMonkeyStabilityGuardTests` has a no-launch stale-summary live-session state fixture, and `docs/testing/runtime-monkey-stability.md` documents the rule, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks and current-doc claims 1241 / 0 after the later runtime-probe summary binding guards. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe AutoSlay analyzer summary signal binding follow-up: `analyze-spire-plus-runtime-failure.ps1` now marks AutoSlay run artifacts untrusted for owner routing when retained `autoslay-summary.json` `Runs[]` `Passed`, `FailureReasonCodes`, or `HangSignals` disagrees with the hash-bound `run-result.json`. `RuntimeMonkeyStabilityGuardTests` has a no-launch summary-signal mismatch fixture, and `check-sts1-event-current-doc-claims.ps1` guards the docs/script/test contract, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks and current-doc claims 1241 / 0 after the later runtime-monkey summary runtime-probe and live-session state binding guards. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe AutoSlay run-result hash binding follow-up: `check-sts1-event-current-doc-claims.ps1` now guards that `docs/testing/runtime-monkey-stability.md`, `check-spire-plus-autoslay-packet.ps1`, and `RuntimeMonkeyStabilityGuardTests.GameNativeAutoSlay` retain `autoslay-summary.json` `RunResultSha256` binding to the retained `run-result.json` bytes. The underlying packet verifier and fixture already reject a tampered retained run-result file, but that guard was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks and current-doc claims 1241 / 0 after the later analyzer owner-routing and runtime-monkey summary artifact guards. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe AutoSlay summary problem-row binding follow-up: `check-spire-plus-autoslay-packet.ps1` now requires `autoslay-summary.json` `FailedRuns` to match the count of `Runs[]` rows with `Passed=false`, non-empty `FailureReasonCodes`, or non-empty `HangSignals`. `RuntimeMonkeyStabilityGuardTests.GameNativeAutoSlay` now has no-launch fixture coverage for stale top-level green summaries with row-level failure/hang signals, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks and current-doc claims 1241 / 0 after the later run-result hash, analyzer signal, and runtime-monkey summary artifact guards. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Latest pause-safe runtime-monkey summary failure/hang signal binding follow-up: `check-spire-plus-runtime-monkey-packet.ps1` now requires every `monkey-summary.json` `Results[]` row to retain empty `FailureReasonCodes` and `HangSignals` for a clean packet and to match the corresponding canonical `iteration-result.json` arrays. This prevents a future game-native monkey batch from keeping clean per-iteration evidence while summary rows carry stale failure or hang signals. `RuntimeMonkeyStabilityGuardTests` now has a no-launch stale-summary-failure/hang fixture, but it was not executed through `dotnet test` in this thread because the shared validation lane is paused. Pause-safe checks passed: parser checks, current-doc claims 1241 / 0 after the later AutoSlay summary problem-row, run-result hash, analyzer signal, and runtime-monkey summary artifact binding guards, and a synthetic no-launch packet proving matching empty summary signals pass while tampered summary failure/hang arrays fail empty and match checks. This remains verifier/test hardening only and does not close game-native AutoSlay/monkey batch proof, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

Pause-safe direct-smoke analyzer follow-up: `analyze-spire-plus-runtime-failure.ps1` now treats failed direct smoke roots with `direct-smoke-summary.json`, bound `godot.log.current-iteration`, and `godot-log-audit.json` as `DirectSmoke` targets. Rerunning it against `.tools/runtime-evidence/v01071-beta87-additive-batch1-direct-20260619-102309/` reports `PackageRuntimeDrift`, 1 analyzed target, 2 package blockers, 0 harness blockers, and 0 gameplay blockers, so the then BaseLib-backed Slay the Spire 2 `v0.107.1` dirty audit is routed to dependency/runtime compatibility instead of StS1 event gameplay source. The report's `BaseLibPatchFailures` field now pinpoints `AdjustCustomMessageKeys::Fuckery()` as an undefined target-method failure, `NRelicCollectionCategory::LoadRelics` as an instruction matcher failure, and the 241-applied / 2-failed BaseLib patch summary. This is analyzer/test hardening only and still does not close clean-loader, gameplay, save-load, replacement, multiplayer, QA, game-native AutoSlay batch proof, release, or handoff gates.

Shared validation update from the migration lane, 2026-06-18: fresh beta.85 CanaryOnly proof under `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` reached main menu, audited clean, and passed retained log/packet verifiers with 4 event types / 6 registration calls. The beta.85 AdditiveBatch1 evidence under `.tools/runtime-evidence/v01070-beta85-additive-batch1-20260617-233759/` remains root-cause history for the 13/14 package/source-shape drift. The beta.86 direct AdditiveBatch1 proof under `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/` reached main menu, audited clean, reported Spire Plus `v0.1.0-private-beta.87`, RitsuLib `0.4.24`, compat branch `0.107.0`, 25/25 Spire Plus patches, 30 SavedSpireFields, 10 event types / 14 registration calls, exact act/shared tuple parity including The Cleric in Overgrowth and Underdocks, retained log verifier 21 / 0, and packet verifier 45 / 0. Treat O25 and O33 as loader/registration proof only, and do not treat either as gameplay, save-load, render, replacement, multiplayer, QA, release, or handoff evidence.

Additional shared validation update from the `d2ff20f5` test-split follow-up, 2026-06-18: `ReleaseEvidenceGateTests` manual evidence template tests were extracted into a partial test file, `RuntimeMonkeyStabilityGuardTests` added coverage for log-derived owner routing, and the shared validation lane reported `dotnet build EZMicroBalance.sln -m:1 --no-incremental` at 0 warnings / 0 errors plus focused `RuntimeMonkeyStabilityGuardTests` and `ReleaseEvidenceGateTests` at 19 passed / 0 failed / 0 skipped / 19 total. This event thread did not run that validation. It does not close publish, package/release-evidence validation, runtime smoke, gameplay, save-load, replacement, multiplayer, QA, release, or handoff gates.

---

## 1. 当前证据重建

### 1.1 项目边界

`Spire Plus` 仍是唯一 active private-beta deliverable。
technical manifest id、项目/资源/兼容路径仍保持 `EZMicroBalance`。

必须保持：

```text
EZMicroBalanceCode/
EZMicroBalance/
EZMicroBalance.json
EZMicroBalance.dll
EZMicroBalance.pck
```

红线：

1. 不原地改 manifest id。
2. 不提交原版游戏资产。
3. 不提交大段反编译代码。
4. StS1 原图若无授权，不进入 tracked/public files。
5. 无可再分发 event art 时，只能采用：
   - owner-provided licensed art；
   - local extraction hash proof；
   - generated replacement art；
   - non-parity placeholder。

---

## 2. 当前 source / static / test 状态

当前 validation 记录：

```text
dotnet build EZMicroBalance.sln -m:1 --no-incremental: PASS, 0 warnings / 0 errors
ReleaseEvidenceGateTests: 9 passed / 0 failed / 0 skipped
Complementary no-build test-project lane: 480 passed / 0 failed / 39 skipped / 519 total
Current retained split coverage after beta.92 migration validation: 475 passed / 0 failed / 21 skipped / 496 total
Opt-in installed-artifact lane: 67 passed / 0 failed / 2 skipped / 69 total
Static suite: 15 static steps / 0 suite failures
current-doc-claims: 1324 checks / 0 mismatches
runtime-preflight: 27 checks / 0 mismatches (local v0.107.1 / beta.92 target; read-only source/prereq only)
static-file-hygiene: 12 checks / 0 mismatches
v19 gate ledger: 535 checks / 0 mismatches
v20 final-gate overlay: 29 checks / 0 mismatches
v19 subagent coverage: 70 checks / 0 mismatches
git diff --check: exit 0 with CRLF normalization warnings only; no whitespace errors
source workspace: 60 checks / 0 mismatches against installed v0.107.1 and STS2-RitsuLib 0.4.29 / compat 0.107.1, with two retained GDRE warnings
d2ff20f5 focused shared follow-up: build 0 warnings / 0 errors; RuntimeMonkeyStabilityGuardTests plus ReleaseEvidenceGateTests 19 passed / 0 failed / 0 skipped / 19 total; no publish/package/runtime/gameplay/QA/handoff closure
```

严格解释：

- Source/test/static guard 层面可以算强进展。
- `0 warnings / 0 errors` 已保留到 beta.92 RitsuLib-only migration 证据。
- skipped tests 已解释为 release-artifact/runtime/local-source gating。
- 这些仍然不等于 gameplay proof。
- 当前文档明确说没有 gameplay、clicked UI、save-load、co-op、event encounter、replacement、independent QA、release handoff proof。

---

## 3. 当前 runtime / loader 状态

### 3.1 已经通过的 loader 部分

Retained beta.85 Off proof（previous-package/game-version context）：

```text
v0.107.0
RitsuLib 0.4.24 / compat branch 0.107.0
Spire Plus v0.1.0-private-beta.85
25/25 Spire Plus ModPatcher patches applied
StS1Events default Off
main menu reached
godot-log-audit clean with 0 blocking signature hits
installed beta.85 package parity passed
```

严格解释：

- v19 的 `v0.107 Off smoke red` 已被 beta.85 Off proof superseded。
- beta.92 RitsuLib-only Off loader proof 是当前通过的 Off loader 证据；beta.85 default-Off proof 只作为 previous-package/game-version context 保留。
- 这只证明 Off path，不证明 CanaryOnly、AdditiveBatch1、gameplay、save/load、replacement、multiplayer、QA。

Retained beta.85 CanaryOnly enabled-mode loader proof（previous-package/game-version context）：

```text
v0.107.0
RitsuLib 0.4.24 / compat branch 0.107.0
Spire Plus v0.1.0-private-beta.85
StS1Events CanaryOnly mode
4 event types / 6 registration calls
main menu reached
godot-log-audit clean with 0 blocking signature hits
retained enabled-mode log/packet verifiers passed with 0 mismatches
tuple-aware copied-log dry-run returned 21 checks / 0 mismatches
```

严格解释：

- Retained beta.85 CanaryOnly loader registration proof remains previous-package/game-version loader context for `O25` and loader-packet `O39`; recapture current CanaryOnly before broader current-runtime claims.
- It still does not prove Big Fish, Golden Idol, The Lab, or Divine Fountain gameplay, result state, save/load, EN/ZHS render, image/license/render, replacement, multiplayer, QA, or handoff readiness.
- Do not derive AdditiveBatch1 proof from CanaryOnly proof.

Current beta.92 RitsuLib-only AdditiveBatch1 enabled-mode loader proof：

```text
v0.107.1
STS2-RitsuLib 0.4.29 / compat branch 0.107.1
Spire Plus v0.1.0-private-beta.92
StS1Events AdditiveBatch1 mode
10 event types / 14 registration calls
main menu reached
godot-log-audit clean with 0 blocking signature hits
enabled-mode log verifier 31 / 0
runtime packet verifier 61 / 0
```

严格解释：

- Current beta.92 RitsuLib-only AdditiveBatch1 loader registration proof can be treated as current-pass for `O33`.
- It still does not prove event encounter gameplay, result state, save/load, EN/ZHS render, image/license/render, replacement functional behavior, multiplayer, QA, or handoff readiness.

### 3.2 仍未通过的部分

当前 validation 明确说：

```text
Retained beta.85 Off, retained beta.85 CanaryOnly, retained beta.87 AdditiveBatch1, previous BaseLib-backed beta.88 AdditiveBatch1, and current beta.92 AdditiveBatch1 loader proof must not be extended to:
gameplay
save-load
replacement
multiplayer
QA
```

所以当前仍 blocked/pending：

```text
Big Fish UI/gameplay/result proof
Golden Idol UI/gameplay/result proof
The Lab UI/gameplay/result proof
Divine Fountain UI/gameplay/result proof
6 simple batch event proofs
save/load
EN/ZHS runtime render
image/license/render
replacement functional proof
multiplayer/fail-closed
QA/Red-Team
```

---

## 4. 当前 StS1 event source 改进

已推进的 source/static 改进：

```text
Divine Fountain:
- now overrides IsAllowed(IRunState)
- requires every run participant to have at least one curse
- guarded by DivineFountainRequiresEveryPlayerToHaveACurse

Big Fish:
- now uses wiki-aligned Box option identity
- EN/ZHS localization keys aligned
- guarded by BigFishUsesBoxOptionName

Golden Idol:
- now uses Outrun / Smash / Hide trap branch identities and values
- still marks random-relic Take reward as non-parity substitute
- missing Golden Idol relic model remains a parity gap

The Lab:
- now has only Open option
- unused Leave EN/ZHS keys removed
- source keeps 3-potion / A15+ 2-potion split
- guarded by TheLabHasOnlyOpenOption

Simple batch:
- Old Beggar, Shining Light, Golden Shrine, The Cleric have source/localization/doc guard coverage for current AdditiveBatch1 contracts
```

严格解释：

- 这些是好的 source/static parity improvements。
- 它们仍不是 runtime render/gameplay/save-load proof。
- Golden Idol 仍存在关键 non-parity gap：没有 Golden Idol relic model，Take 仍是 random relic substitute。

---

## 5. 当前 localization 状态

当前状态：

```text
EN/ZHS resource file key count: improved / guarded
ZHS placeholders: claimed 0 in status-board
But localization-source-gap-scan records 33 source-referenced StS1 result-page keys missing from both EN and ZHS
33-key localization gap is known/non-failing until intentionally closed in a versioned resource pass
```

严格解释：

- “0 placeholder” 不能等于 “runtime localization complete”。
- 33 missing source-referenced result-page keys must remain open.
- EN/ZHS render screenshots are still required.
- Missing-key scan and runtime UI screenshots must be gate conditions.
- Fixing `STS1_GOLDEN_IDOL.pages.LEAVE.description` only removes the direct localization missing-key blocker; it does not prove gameplay behavior or replace the enabled-mode log verifier/runtime evidence packet.

---

## 6. 当前 count matrix

Current and historical numbers must be kept separate.

Current basis includes:

```text
Public wiki baseline: 52
Canonical rows: 54
Runtime registry entries: 50
Model files: 48
Compiling models: 47
RegisterAll calls: 57 current source/static calls
AdditiveBatch1 calls: 14 current source/static calls / 10 event types
Current enabled-mode runtime counts: retained beta.85 CanaryOnly 4 event types / 6 registration calls pass as previous-package/game-version context; beta.92 RitsuLib-only AdditiveBatch1 10 event types / 14 registration calls pass as current loader/registration proof. The beta.85 AdditiveBatch1 13/14 `Sts1TheCleric` mismatch remains root-cause history for stale package/source shape.
```

Strict rule:

```text
Never equate registry entries, model files, or registration calls with full StS1 event completion.
```

---

## 7. Target definition

The actual target remains StS1-like event experience:

```text
unknown-room event pool
correct act bucket
shared / semi-common / exclusive membership
correct options and page flow
locked option conditions
reward/card/relic/curse/potion/gold/HP/max HP effects
Ascension 15 deltas
EN/ZHS runtime text and layout
event images or documented non-parity placeholders
save/load stability
multiplayer / IsShared safety
default Off
ReplacementPrototype functional proof
independent QA
```

StS1 events must be judged by gameplay behavior, not by source count.

---

## 8. Strict gap analysis

| Area | Current status | Verdict |
|---|---|---|
| Build/test/static | Strong progress | Pass for source/static only |
| beta.85 Off loader | Clean | Pass for default-Off only |
| CanaryOnly enabled-mode | Retained beta.85 proof | Previous-package/game-version loader proof only |
| AdditiveBatch1 enabled-mode | Current beta.92 RitsuLib-only direct proof: 10 event types / 14 calls | Loader/registration proof only |
| Canary gameplay | Missing | Blocked |
| Simple batch gameplay | Missing | Blocked |
| Save/load | Missing | Blocked |
| EN/ZHS runtime render | Missing | Blocked |
| Image/license/render | Missing | Blocked |
| ReplacementPrototype | Source-gated only | Blocked |
| Multiplayer/fail-closed | Missing runtime proof | Blocked |
| Combat events | Missing encounter models | Blocked |
| Temporary substitutes | Still non-parity | Must remain flagged |
| QA/Red-Team | No independent gameplay pass | Blocked |
| Release-ready | No | Blocked |

---

## 9. Management decision

Decision:

```text
Continue optimization + limited advancement.
Optimization remains priority.
```

### 9.1 Continue optimizing

Priority optimization:

```text
- protect beta.85 Off loader clean state
- preserve retained beta.85 CanaryOnly loader proof as previous-package/game-version context
- preserve current beta.92 AdditiveBatch1 loader/registration proof
- close or track 33 localization result-page key gaps
- keep zero-warning build
- maintain static-suite guards
- keep count matrix current
- define image/license plan
- update status-board and gate ledger without overclaims
```

### 9.2 Limited advancement

After enabled-mode loader proof:

```text
4 canary runtime proof:
- Big Fish
- Golden Idol
- The Lab
- Divine Fountain

6 simple batch runtime proof:
- Purifier
- Upgrade Shrine
- Golden Shrine
- The Cleric
- Old Beggar / Pleading Vagrant
- Shining Light
```

### 9.3 Pause broader expansion

Pause:

```text
broad Phase 2/3/4 expansion
combat full implementation
custom UI full parity
full parity claim
release-ready claim
commit/push without exact evidence-supported scope
```

---

## 10. June / Next Monthly Dev Spec

目标名称：

```text
StS1 Event Port Prototype Batch 1 - beta.92 RitsuLib-Only Loader Foundation
```

Month-end Go / No-Go:

1. Build: 0 errors / 0 warnings with saved log.
2. Test matrix:
   - ReleaseEvidenceGateTests pass,
   - complementary no-build lane pass,
   - installed-artifact lane pass,
   - static suite pass.
3. Skipped tests explained by release-artifact/runtime/local-source gating.
4. Static suite keeps 0 failures.
5. current-doc-claims, gate-ledger, subagent coverage, static-file hygiene all pass.
6. Worktree state clean or owner-approved dirty scope.
7. beta.85 Off loader clean proof retained.
8. beta.85 CanaryOnly loader proof captured:
   - 4 event types / 6 registration calls.
9. beta.92 RitsuLib-only AdditiveBatch1 loader proof captured:
   - 10 event types / 14 registration calls.
10. AdditiveAllDraft remains unsafe-only.
11. ReplacementPrototype remains debug + unsafe-only.
12. Count matrix updated and Red-Team reviewed.
13. 33 localization source-key gaps either closed or explicitly deferred with owner acceptance.
14. Four canary events runtime verified:
   - screenshots,
   - result logs,
   - pre/post state,
   - save/load,
   - EN/ZHS render,
   - image/license/render decision.
15. Six simple batch events runtime verified.
16. ReplacementPrototype functional proof:
   - unknown rooms only draw StS1 candidates,
   - act bucket correct,
   - event bag/no-repeat proof,
   - save/load proof.
17. Multiplayer/fail-closed runtime proof.
18. Combat blockers current.
19. Temporary substitutes remain non-parity.
20. Independent QA/Red-Team pass/fail by gate.
21. current-validation, status-board, monthly review, handoff docs updated.
22. No commit/push unless exact scope is evidence-supported.

---

## 11. Mandatory Overnight Run v20

The assistant may stop only if:

```text
A. O0-O84 all GREEN
B. HARD STOP BLOCKER REPORT written
```

Hard Stop report must include:

```text
exact gate id
blocker reason
evidence path
attempted actions
owner action
why continuation is impossible in current environment
```

Hard Stop is a pause condition, not completion.

### 11.1 Do not stop merely because

```text
build passes
tests pass
static suite passes
Off loader is clean
source files exist
status-board updated
canonical matrix exists
hard-stop report exists
all code-side work complete
```

### 11.2 O0-O84 gates

| Gate | Requirement |
|---|---|
| O0 | Worktree snapshot: branch, HEAD, diff, dirty files |
| O1 | Full build exit code 0 |
| O2 | Zero-warning proof |
| O3 | Full test matrix exit code 0 |
| O4 | Test count reconciliation |
| O5 | Skipped-test explanation |
| O6 | Static suite pass |
| O7 | current-doc-claims pass |
| O8 | gate-ledger pass |
| O9 | subagent coverage pass |
| O10 | static-file hygiene pass |
| O11 | Format check pass |
| O12 | Diff check pass |
| O13 | Patch inventory check pass |
| O14 | Worktree batch classification pass |
| O15 | Dirty-worktree owner decision |
| O16 | Status-board no false/generic Done |
| O17 | Canonical matrix complete |
| O18 | Count reconciliation Red-Team reviewed |
| O19 | Act mapping guard pass |
| O20 | Feature gate tests pass |
| O21 | Off=0 source guard proof |
| O22 | CanaryOnly=4 source guard proof |
| O23 | AdditiveBatch1 source guard proof |
| O24 | AdditiveAllDraft unsafe-only proof |
| O25 | ReplacementPrototype debug/unsafe-only proof |
| O26 | beta.92 package parity proof |
| O27 | beta.92 package SHA recorded |
| O28 | RitsuLib-only Spire Plus path report |
| O29 | Active godot.log archived |
| O30 | beta.85 Off loader audit clean |
| O31 | Off runtime proof: 0 StS1 registrations |
| O32 | beta.85 CanaryOnly loader audit clean |
| O33 | CanaryOnly runtime proof: 4 event types / 6 registration calls |
| O34 | beta.92 AdditiveBatch1 loader audit clean |
| O35 | AdditiveBatch1 runtime proof: 10 event types / 14 registration calls |
| O36 | 33 localization source-key gap ledger current |
| O37 | Localization gaps closed or owner-deferred |
| O38 | Canary code review clean |
| O39 | Big Fish screenshot/result log/pre-post state |
| O40 | Golden Idol screenshot/result log/pre-post state |
| O41 | Lab screenshot/result log/pre-post state |
| O42 | Divine Fountain screenshot/result log/pre-post state |
| O43 | Canary save/load proof |
| O44 | Canary EN/ZHS render proof |
| O45 | Canary image/license/render proof |
| O46 | Big Fish Box UI/render proof |
| O47 | Golden Idol relic substitute clearly non-parity or fixed |
| O48 | Golden Idol trap branch render proof |
| O49 | Lab Open-only runtime render proof |
| O50 | Divine Fountain curse-prerequisite natural-pool proof |
| O51 | Simple batch exact spec Red-Team pass |
| O52 | Simple batch code review clean |
| O53 | Purifier runtime proof |
| O54 | Upgrade Shrine runtime proof |
| O55 | Golden Shrine runtime proof |
| O56 | The Cleric runtime proof |
| O57 | Old Beggar / Pleading Vagrant runtime proof |
| O58 | Shining Light runtime proof |
| O59 | Simple batch save/load proof where applicable |
| O60 | Simple batch EN/ZHS render proof |
| O61 | Simple batch image/license/render proof |
| O62 | Replacement source guard pass |
| O63 | Replacement functional proof: unknown rooms only draw StS1 candidates |
| O64 | Replacement Act bucket proof |
| O65 | Event bag / visited ids / no-repeat proof |
| O66 | Replacement save/load proof |
| O67 | Multiplayer fail-closed or verified proof |
| O68 | IsShared matrix current |
| O69 | Combat blocker report current |
| O70 | Temporary substitutes matrix current |
| O71 | Content parity gap matrix current |
| O72 | Asset/license decision current |
| O73 | ZHS render screenshots attached |
| O74 | Independent QA/Red-Team report complete |
| O75 | QA does not self-approve implementation |
| O76 | current-validation updated |
| O77 | status-board updated |
| O78 | monthly review updated |
| O79 | handoff docs updated |
| O80 | owner actions listed |
| O81 | no unsupported commit/push |
| O82 | release-ready claim absent unless gates pass |
| O83 | final summary states blocked gates honestly |
| O84 | next-run start point lists unresolved gates only |

---

## 12. Required Subagents

Subagents are mandatory. Implementation agents cannot approve their own work.

1. **BuildGate / Repo Health**
   - build/test/static/format/diff/patch/worktree evidence,
   - skipped-test explanation,
   - zero-warning proof.

2. **Runtime Environment Bootstrap**
   - beta.92 package,
   - STS2-RitsuLib v0.4.29,
   - EZMicroBalance install,
   - godot.log,
   - loader audit.

3. **Enabled-Mode Loader Subagent**
   - CanaryOnly loader proof,
   - AdditiveBatch1 loader proof,
   - enabled log audit.

4. **Wiki Parity Spec Auditor**
   - 52 public events,
   - 54 canonical rows,
   - exact options,
   - A15 deltas,
   - semi-common membership.

5. **StS2 Source/API Auditor**
   - EventModel,
   - ActModel,
   - RitsuLib,
   - card/relic/potion/gold/HP/save/replacement APIs.

6. **Feature Gate / Registration Engineer**
   - Off,
   - CanaryOnly,
   - AdditiveBatch1,
   - AdditiveAllDraft,
   - ReplacementPrototype.

7. **Canary Gameplay Subagent**
   - Big Fish,
   - Golden Idol,
   - Lab,
   - Divine Fountain runtime proof.

8. **Simple Batch Gameplay Subagent**
   - Purifier,
   - Upgrade Shrine,
   - Golden Shrine,
   - The Cleric,
   - Old Beggar/Pleading Vagrant,
   - Shining Light runtime proof.

9. **Localization Gap Closure Subagent**
   - 33 result-page key gaps,
   - EN/ZHS resources,
   - missing-key scan,
   - runtime render proof.

10. **Asset + Image Subagent**
    - image/license plan,
    - local extraction hash proof,
    - generated placeholders,
    - render screenshots.

11. **Event Pool / RNG / Save Subagent**
    - replacement pool,
    - seeded unknown rooms,
    - event bag,
    - visited ids,
    - save/load.

12. **Multiplayer / IsShared Subagent**
    - per-event IsShared,
    - combat true,
    - fail-closed multiplayer proof.

13. **Content Parity Subagent**
    - Bite,
    - face relics,
    - Golden/Bloody Idol,
    - Parasite/Madness,
    - combat encounter models,
    - temporary substitutes.

14. **QA / Red-Team Subagent**
    - independent pass/fail by gate,
    - no implementation.

15. **Release Documentation Subagent**
    - status-board,
    - current-validation,
    - monthly review,
    - handoff,
    - release evidence,
    - owner actions.

---

## 13. Direct instruction to assistant

```text
当前状态不能标完成。

最新证据显示 beta.92 已经完成 RitsuLib-only loader/registration 迁移：Off proof 和 AdditiveBatch1 proof 都在 StS2 `v0.107.1` + STS2-RitsuLib `0.4.29` 下 clean，25/25 patches applied，AdditiveBatch1 10 event types / 14 registration calls，enabled-mode verifier 31 / 0，packet verifier 61 / 0。Retained beta.85 CanaryOnly verifiers 仍只证明 previous-package/game-version 的 4 event types / 6 registration calls。source/test/static 也有强进展：build 0/0，retained split coverage 475 passed / 0 failed / 21 skipped / 496 total，static suite 15 steps / 0 failures，current-doc claims 1324 / 0。

但是 beta.92 Off/AdditiveBatch1 loader proof 和 retained beta.85 CanaryOnly loader proof 都不能外推到 gameplay、save-load、replacement、multiplayer、image/render 或 QA。beta.85 AdditiveBatch1 13/14 mismatch 现在只保留为 package/source-shape drift 的历史 root-cause 证据。当前 runtime parity 仍未完成，因为还没有 event encounter gameplay、save-load、render、replacement、multiplayer、independent QA 或 handoff proof。

coordination pause 解除后，继续 Mandatory Overnight Run v20。只能在 O0-O84 全绿后停止。若当前环境无法完成某 gate，写 HARD STOP BLOCKER REPORT，但 blocked gate 不得标完成。

优先级：
1. 保持 beta.92 RitsuLib-only Off clean proof。
2. 保留 beta.85 CanaryOnly loader proof as previous-package/game-version context：4 event types / 6 registration calls。
3. 保持 beta.92 RitsuLib-only AdditiveBatch1 loader proof：10 event types / 14 registration calls。
4. 处理或 owner-defer 33 个 localization result-page key gaps。
5. 做 4 canary gameplay：screenshots、result logs、pre/post state、save/load、EN/ZHS、image/license/render。
6. 做 6 simple batch gameplay。
7. 功能性证明 ReplacementPrototype。
8. Combat events 在 encounter models 完成前继续 blocked。
9. Temporary substitutes 必须继续标 non-parity。
10. 启动 subagents，QA/Red-Team 必须独立验收。
11. 不要 commit/push，除非 validation evidence 支持本次准确 scope。
```

---

## 14. Red lines

- 不要把 source/test/static pass 当 gameplay completion。
- 不要把 beta.92 Off/AdditiveBatch1 loader proof 或 retained beta.85 CanaryOnly loader proof 外推成 gameplay proof。
- 不要把 registry count 当 StS1 experience。
- 不要把 hard-stop report 当 completion。
- 不要在相关 enabled-mode 和 gameplay 证据 clean 前推进 gameplay claims。
- 不要在 verified scope 绿之前扩大到 broad Phase。
结论：**当前仍未完成**，但审查口径要更新到最新 beta.92 状态：**v0.107.1 的 RitsuLib-only Off 和 AdditiveBatch1 loader/registration proof 已通过，当前 Spire Plus 交付面不再依赖 BaseLib；现在真正阻塞点转移到 gameplay、save/load、replacement、image/render、multiplayer、独立 QA 和 handoff。**

新版完整审查内容已内联在本文件；不要依赖 sandbox-only 下载链接作为当前证据。

## 1. 当前严格审核结论

当前可以认可的进展：

| 模块                      | 审核结论                                                                                                                                    |
| ----------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| Build                   | 最新记录为 `0 warnings / 0 errors`                                                                                                           |
| Test matrix             | beta.92 retained split coverage 记录为 `475 passed / 0 failed / 21 skipped / 496 total`                                                  |
| Installed artifact lane | `67 passed / 0 failed / 2 skipped / 69 total`                                                                                           |
| Static suite            | `15 static steps / 0 suite failures`                                                                                                    |
| Current doc claims      | 后续静态检查最高记录为 `1324 checks / 0 mismatches` |
| beta.92 Off loader      | **clean**：v0.107.1 + STS2-RitsuLib 0.4.29，25/25 patches，RitsuLib-only package                                                               |
| beta.85 CanaryOnly loader | **clean**：retained verifiers 证明 4 event types / 6 registration calls                                                                    |
| beta.92 AdditiveBatch1 loader | **clean**：enabled-mode verifier 31 / 0，packet verifier 61 / 0，10 event types / 14 registration calls                                                                   |
| Source parity fixes     | Big Fish Box identity、Divine Fountain curse prerequisite、Golden Idol trap branch identities、The Lab Open-only 都有 source/static guard 改进 |
| Default Off             | 当前可认可为 loader proof 层面通过                                                                                                                |

这些都有当前 validation 记录支持：beta.92 Off proof 和 beta.92 AdditiveBatch1 proof 显示 `v0.1.0-private-beta.92`、STS2-RitsuLib `0.4.29`、compat branch `0.107.1`、25/25 patches、main menu reached、audit clean；beta.92 AdditiveBatch1 proof 显示 10 event types / 14 registration calls、enabled-mode verifier 31 / 0、packet verifier 61 / 0。当前文档也明确这些 proof 只覆盖 loader/registration 层面，不覆盖 gameplay、save-load、replacement、multiplayer、image/render 或 QA。

但还不能认可的部分：

| 模块                                    | 当前状态                                     |
| ------------------------------------- | ---------------------------------------- |
| CanaryOnly enabled-mode               | loader proof 已完成；不等于 gameplay proof        |
| AdditiveBatch1 enabled-mode           | loader proof 已完成；不等于 gameplay proof        |
| 4 canary gameplay                     | 未完成                                      |
| 6 simple batch gameplay               | 未完成                                      |
| Save/load                             | 未完成                                      |
| EN/ZHS runtime render                 | 未完成                                      |
| Image/license/render                  | 未完成                                      |
| ReplacementPrototype functional proof | 未完成                                      |
| Multiplayer/fail-closed runtime proof | 未完成                                      |
| Combat events                         | blocked，缺 encounter models               |
| Independent QA                        | 未完成                                      |
| Release/live ready                    | **No**                                   |

当前 validation 明确说：没有 gameplay、clicked UI、save-load、co-op、event encounter、replacement、independent QA、release handoff proof；并且 beta.92 Off/AdditiveBatch1 loader proof 和 retained beta.85 CanaryOnly loader proof 不得扩展到 gameplay、save-load、replacement、multiplayer 或 QA gates。

## 2. 与目标对比

我们的目标不是“source 能编译”或“loader 到主菜单”，而是让 StS2 mod 尽量复刻 StS1 unknown-room event experience：

```text
- unknown-room event pool
- correct act bucket
- shared / semi-common / exclusive membership
- event option/page flow
- locked option conditions
- rewards/cards/relics/curses/potions/gold/HP/max HP
- Ascension 15 deltas
- EN/ZHS runtime text and layout
- event images or documented non-parity placeholders
- save/load
- multiplayer / IsShared
- default Off
- ReplacementPrototype functional proof
- independent QA
```

StS1 Wiki 的 event system 是 unknown location 事件系统：事件是否出现、出现哪个事件，取决于随机和当前 Act；部分事件限定 Act，部分可跨 Act；Act 4 没有 unknown location/event；Ascension 15 会强化部分不利事件。Wiki 事件列表按 16 shared、12 Act 1 exclusive、16 Act 2 exclusive、8 Act 3 exclusive 组织。([slay-the-spire.fandom.com](https://slay-the-spire.fandom.com/wiki/Events))

所以，当前任何 `52 / 54 / 50 / 48 / 47 / calls` 类数字都只能作为 matrix 管理依据，**不能当作 full parity 完成依据**。

## 3. 关键进展与仍存差距

最新 source/static 改进是有价值的：

* `Sts1DivineFountain` 现在通过 `IsAllowed(IRunState)` 要求 run participant 至少有一个 curse，并有 guard。
* `Sts1BigFish` 已改成 Wiki-aligned `Box` option identity，并有 EN/ZHS key guard。
* `Sts1GoldenIdol` 已使用 Outrun / Smash / Hide trap branch identities and values，但 Take 仍是 random relic substitute，因为 Golden Idol relic model 缺失。
* `Sts1TheLab` 现在只保留 Open option，并保留 3 potion / A15+ 2 potion split。
* Simple batch 的 Old Beggar、Shining Light、Golden Shrine、The Cleric 等已有 source/localization/doc guard coverage。

但 localization 还没有完全闭环：当前 validation 记录显示仍有 **33 个 source-referenced StS1 result-page keys** 同时缺 EN/ZHS，当前只是 known/non-failing gap，必须后续按 versioned resource pass 关闭或 owner-defer。

## 4. 管理决策

**继续优化 + 有限推进，两者兼顾，但优化优先。**

继续优化：

```text
- 保持 beta.92 RitsuLib-only Off clean proof
- 保留 beta.85 CanaryOnly enabled-mode proof as previous-package/game-version context
- 保持 beta.92 RitsuLib-only AdditiveBatch1 enabled-mode proof
- 关闭或 owner-defer 33 个 localization result-page gaps
- 保持 zero-warning build
- 保持 static-suite guards
- 更新 count matrix 和 gate ledger
- 明确 image/license 方案
```

有限推进：

```text
只推进 verified scope：

4 canary:
- Big Fish
- Golden Idol
- The Lab
- Divine Fountain

6 simple batch:
- Purifier
- Upgrade Shrine
- Golden Shrine
- The Cleric
- Old Beggar / Pleading Vagrant
- Shining Light
```

暂停扩大：

```text
- broad Phase 2/3/4 expansion
- combat full implementation
- custom UI full parity
- full parity claim
- release-ready claim
- commit/push without exact evidence-supported scope
```

项目边界仍然不变：`Spire Plus` 是 active deliverable，`EZMicroBalance` 仍是 technical id/compatibility surface。 项目 release policy 也继续禁止复制原版资产和大段反编译代码。

## 5. 下个月开发规范

目标名称：

**`StS1 Event Port Prototype Batch 1 - beta.92 RitsuLib-Only Loader Foundation`**

月末 Go/No-Go：

1. Build 保持 `0 errors / 0 warnings`。
2. Test matrix 全绿，包括 release evidence split、installed artifact lane、static suite。
3. skipped tests 继续按 release-artifact/runtime/local-source gating 解释。
4. Current-doc-claims、gate-ledger、subagent coverage、static-file hygiene 全部 pass。
5. Worktree clean 或 owner-approved dirty scope。
6. beta.92 RitsuLib-only Off loader clean proof 保留。
7. beta.85 CanaryOnly loader proof 捕获：4 event types / 6 registration calls。
8. beta.92 RitsuLib-only AdditiveBatch1 loader proof 捕获：10 event types / 14 registration calls。
9. AdditiveAllDraft 仍 unsafe-only。
10. ReplacementPrototype 仍 debug + unsafe-only。
11. Count matrix 更新并 Red-Team reviewed。
12. 33 个 localization source-key gaps 关闭，或明确 owner-deferred。
13. 4 个 canary runtime verified：screenshots、result logs、pre/post state、save/load、EN/ZHS render、image/license/render。
14. 6 个 simple batch runtime verified。
15. ReplacementPrototype functional proof：unknown rooms only draw StS1 candidates、act bucket correct、event bag/no-repeat、save/load。
16. Multiplayer/fail-closed runtime proof。
17. Combat blockers current。
18. Temporary substitutes 继续标 non-parity。
19. Independent QA/Red-Team 逐 gate pass/fail。
20. `current-validation`、`status-board`、monthly review、handoff docs 更新。
21. 不 commit/push，除非 exact scope 有 evidence 支持。

## 6. Mandatory Overnight Run v20

停止条件只有：

```text
A. O0-O84 全部 GREEN
B. HARD STOP BLOCKER REPORT
```

Hard Stop 只代表暂停，**不代表完成**。

不能因为这些停止：

```text
build passes
tests pass
static suite passes
Off loader is clean
source files exist
status-board updated
canonical matrix exists
hard-stop report exists
all code-side work complete
```

核心 gates：

| Gate    | 必须结果                                                                                                                                              |
| ------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| O0-O15  | worktree、build、zero-warning、test matrix、skips、static suite、format/diff/patch、dirty-scope 全部记录                                                     |
| O16-O25 | status-board、canonical matrix、feature gate、Off/Canary/Additive source guard、unsafe modes 全部过审                                                     |
| O26-O35 | beta.92 package parity/SHA、RitsuLib-only paths、godot.log、Off/Canary/Additive clean loader audits                                               |
| O36-O37 | 33 个 localization key gap ledger current，且 closed 或 owner-deferred                                                                                |
| O38-O50 | 4 canary code review、runtime screenshots/result logs/pre-post、save-load、EN/ZHS、image/license、Big Fish/Golden Idol/Lab/Divine Fountain gap closure |
| O51-O61 | 6 simple batch spec/code/runtime/save-load/localization/image proof                                                                               |
| O62-O66 | ReplacementPrototype source guard、unknown-room proof、Act bucket、event bag、save-load                                                               |
| O67-O73 | multiplayer、IsShared、combat blockers、temporary substitutes、content parity、asset/license、ZHS screenshots                                           |
| O74-O84 | independent QA、current-validation、status-board、monthly review、handoff、owner actions、no unsupported commit/push、final honest summary               |

## 7. 必须使用 subagent

必须启动这些 subagent，且实现者不能自验：

1. **BuildGate / Repo Health**：build/test/static/format/diff/patch/worktree、zero-warning、skipped tests。
2. **Runtime Environment Bootstrap**：beta.92 package、STS2-RitsuLib v0.4.29、EZMicroBalance install、godot.log、loader audit。
3. **Enabled-Mode Loader Subagent**：CanaryOnly 和 AdditiveBatch1 loader proof。
4. **Wiki Parity Spec Auditor**：52 public events、54 canonical rows、exact options、A15、semi-common membership。
5. **StS2 Source/API Auditor**：EventModel、ActModel、RitsuLib、card/relic/potion/gold/HP/save/replacement APIs。
6. **Feature Gate / Registration Engineer**：Off、CanaryOnly、AdditiveBatch1、AdditiveAllDraft、ReplacementPrototype。
7. **Canary Gameplay Subagent**：Big Fish、Golden Idol、Lab、Divine Fountain runtime proof。
8. **Simple Batch Gameplay Subagent**：Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant、Shining Light runtime proof。
9. **Localization Gap Closure Subagent**：33 result-page key gaps、EN/ZHS resources、missing-key scan、runtime render proof。
10. **Asset + Image Subagent**：image/license plan、local extraction hash proof、generated placeholders、render screenshots。
11. **Event Pool / RNG / Save Subagent**：replacement pool、seeded unknown rooms、event bag、visited ids、save/load。
12. **Multiplayer / IsShared Subagent**：per-event IsShared、combat true、fail-closed multiplayer proof。
13. **Content Parity Subagent**：Bite、face relics、Golden/Bloody Idol、Parasite/Madness、combat encounter models、temporary substitutes。
14. **QA / Red-Team Subagent**：独立逐 gate pass/fail，不写实现。
15. **Release Documentation Subagent**：status-board、current-validation、monthly review、handoff、release evidence、owner actions。

## 8. 直接发给他的指令

```text
当前状态不能标完成。

最新证据显示 beta.92 RitsuLib-only Off 与 AdditiveBatch1 enabled-mode loader proof 都已经通过：clean audit，25/25 patches applied，AdditiveBatch1 10 event types / 14 registration calls，enabled-mode verifier 31 / 0，packet verifier 61 / 0。Retained beta.85 CanaryOnly 仍只作为 previous-package/game-version loader context。source/test/static 也有强进展：build 0/0，retained split coverage 475 passed / 0 failed / 21 skipped / 496 total，static suite 15 steps / 0 failures，current-doc claims 1324 / 0。

但是 beta.92 Off/AdditiveBatch1 loader proof 和 retained beta.85 CanaryOnly loader proof 都不能外推到 gameplay、save-load、replacement、multiplayer、image/render 或 QA。AdditiveBatch1 的 beta.85 13/14 mismatch 只保留为历史 drift 诊断；当前 runtime parity 仍未完成，因为 gameplay/save-load/render/replacement/multiplayer/QA proof 都缺失。

继续 Mandatory Overnight Run v20。只能在 O0-O84 全绿后停止。若当前环境无法完成某 gate，写 HARD STOP BLOCKER REPORT，但 blocked gate 不得标完成。

优先级：
1. 保持 beta.92 RitsuLib-only Off clean proof。
2. 保留 beta.85 CanaryOnly loader proof as previous-package/game-version context：4 event types / 6 registration calls。
3. 保持 beta.92 RitsuLib-only AdditiveBatch1 loader proof：10 event types / 14 registration calls。
4. 处理或 owner-defer 33 个 localization result-page key gaps。
5. 做 4 canary gameplay：screenshots、result logs、pre/post state、save/load、EN/ZHS、image/license/render。
6. 做 6 simple batch gameplay。
7. 功能性证明 ReplacementPrototype。
8. Combat events 在 encounter models 完成前继续 blocked。
9. Temporary substitutes 必须继续标 non-parity。
10. 启动 subagents，QA/Red-Team 必须独立验收。
11. 不要 commit/push，除非 validation evidence 支持本次准确 scope。
```

管理红线：**不要把 source/test/static pass 当 gameplay completion；不要把 beta.92 Off/AdditiveBatch1 loader proof 或 retained beta.85 CanaryOnly loader proof 外推成 gameplay proof；不要把 registry count 当 StS1 experience；不要把 hard-stop report 当 completion；不要在相关 gameplay 证据 clean 前推进 gameplay claims。**
