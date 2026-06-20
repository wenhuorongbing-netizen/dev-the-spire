# StS1 Event Port v20 Subagent Coverage

Date: 2026-06-17 (v19 file retained; latest v20 role update)
Scope: `docs/goals/event.md` Mandatory Overnight Run v20 subagent requirement.

This is a coverage ledger, not a completion claim. The current coordination pause allows only read-only/static work from this thread. Subagent work from this thread that requires `dotnet`, package validation, runtime smoke, gameplay, save-load, screenshots, replacement proof, multiplayer proof, or QA remains blocked until the shared validation lane is available.

## Current Thread Use

This thread used read-only explorer subagents for static sidecar audits only:

- v19 gate ledger/checker coverage audit.
- enabled-mode log verifier and runbook wording audit.
- 2026-06-15 current-proof and coordination-pause wording audit.
- 2026-06-17 subagent role coverage audit against `docs/goals/event.md` and `scripts/check-sts1-v19-subagent-coverage.ps1`.

These audits did not create loader, gameplay, replacement, multiplayer, or QA proof. They only help keep static docs/scripts aligned while validation is paused; later shared validation supplied separate CanaryOnly loader proof and AdditiveBatch1 loader/registration proof.

## Required Role Ledger

| Role | Current disposition | Current evidence | Next action |
|------|---------------------|------------------|-------------|
| BuildGate / Repo Health | runtime-validation-paused | `PROJECT_STATE.md` records beta.92 RitsuLib-only no-game/package validation; O0-O10 still require direct HEAD/worktree recapture before handoff. | Rerun build/test/format/diff/package lanes after any new code/resource/package changes or before handoff. |
| Runtime Environment Bootstrap | runtime-validation-paused / shared-loader-pass | `PROJECT_STATE.md` and `v19-gate-evidence-map.md` record current beta.92 RitsuLib-only Off and AdditiveBatch1 loader/registration proof; beta.85 CanaryOnly remains previous-package/game-version context. | Preserve retained loader proof; rerun only after package/source changes or before CanaryOnly-specific current-runtime claims. |
| Enabled-Mode Loader Subagent | shared-loader-pass | Current beta.92 AdditiveBatch1 loader/registration proof exists with 10 event types / 14 registration calls; beta.85 CanaryOnly loader proof remains previous-context evidence. | Preserve retained verifier reports; do not extend loader proof to gameplay, replacement, multiplayer, QA, handoff, or release readiness. |
| Wiki Parity Spec Auditor | static-only | `check-sts1-event-registry-shape.ps1`, `check-sts1-event-spec-registration-notes.ps1`, and `wiki-event-catalog.md` guard the current count/spec shape. | Keep static checkers passing; do not treat counts as gameplay parity. |
| StS2 Source/API Auditor | static-only | `source-research/sts2-event-engine.md`, `source-research/sts2-act-event-registration.md`, and source guard scripts document current API shape. | Refresh source/API evidence before changing event, reward, map, save/load, or hook behavior. |
| Feature Gate / Registration Engineer | static-only | Feature-gate, registry-shape, and enabled-log expected-shape checks guard Off, CanaryOnly, AdditiveBatch1, unsafe all-draft, and replacement gates. Retained CanaryOnly and AdditiveBatch1 loader proof exists separately. | Keep source/log expected-shape guards aligned; recapture loader proof only after package/source changes, otherwise proceed to gameplay evidence after the pause. |
| Canary Gameplay Subagent | runtime-blocked | O25 and O39 are loader-packet proof only; O26-O38 and O40-O41 remain blocked in `v19-gate-ledger.csv`. | Capture four canary runtime proofs after the coordination pause. |
| Simple Batch Gameplay Subagent | runtime-blocked | O33 is loader/registration proof only; O42-O52 remain blocked in `v19-gate-ledger.csv`. | Use retained AdditiveBatch1 loader proof only as prerequisite; capture six simple-batch gameplay proofs. |
| Localization Gap Closure Subagent | static-gap-known | Localization source-key and gap-baseline checkers guard the 33-key known localization gap. | Close localization gaps only in a versioned, validated resource pass or record explicit owner deferral. |
| Asset + Image Subagent | static-gap-known | Asset-safety checker guards zero tracked original StS1 images and current unconfirmed redistribution status. | Define owner-provided, local-extraction-hash, generated, or non-parity image plan before render claims. |
| Event Pool / RNG / Save Subagent | runtime-blocked | `O53` is source-guarded only; `O54-O57` replacement functional proof remains blocked in `v19-gate-ledger.csv`. | Run debug/unsafe replacement proof only after explicit owner approval and validation pause lift. |
| Multiplayer / IsShared Subagent | runtime-blocked | Multiplayer-shape checker covers source classification; O58 runtime proof remains blocked. | Capture fail-closed multiplayer proof after enabled-mode evidence exists. |
| Content Parity Subagent | static-gap-known | Parity-blocker checker and content-parity docs mark substitutes, combat blockers, and missing parity content. | Keep non-parity labels until replacement content/runtime proof exists. |
| QA / Red-Team Subagent | blocked | O65 independent QA is current-pending because runtime proof is missing. | Run independent QA only after current runtime/gameplay evidence exists. |
| Release Documentation Subagent | documentation-in-progress | O66-O71 are documentation-in-progress; current docs record the hard-stop and proof boundaries. | Refresh current-validation, status-board, handoff, and release docs after validation/runtime evidence arrives. |

## Post-Pause Evidence Packet Checklist

These are retained or future handoff targets for role owners after the coordination pause is lifted. The retained CanaryOnly and AdditiveBatch1 packets are loader evidence only; missing future gameplay packets keep the mapped gameplay, replacement, multiplayer, QA, and handoff gates open.

| Role owner | Required packet or proof | Gates guarded |
|------------|--------------------------|---------------|
| Runtime Environment Bootstrap + Enabled-Mode Loader Subagent + Feature Gate / Registration Engineer | Retained CanaryOnly packet at `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` with `session-state.json`, `settings.save.before`, `game-release-info.json`, `godot.log.after-launch`, `godot-log-audit.json`, `enabled-mode-log-check.json`, `runtime-evidence-packet-check.json`, and `restore-state.json`; copied-log verifier used `-ExpectedPackageVersion`, `-ExpectedRitsuCompatBranch`, `-ExpectedRitsuLibVersion`, `-ExpectedGameVersion`, `-OutFile`, and `-FailOnMismatch`; packet verifier shows matching `Sts1EventModeEnvironment`, explicit package/Ritsu-compat/RitsuLib-version/game-version targets, no unsafe-mode env leakage, no `-AllowMissingSessionState` / `-AllowMissingRestoreState` bypass, and CanaryOnly 4 event types / 6 registration calls. Copied-log proof covers registration-call count and class set; Act-target tuple proof remains source-derived until future logs or gameplay evidence prove those targets directly. | O25, O39 |
| Runtime Environment Bootstrap + Enabled-Mode Loader Subagent + Feature Gate / Registration Engineer | Current beta.92 AdditiveBatch1 packet at `.tools/runtime-evidence/v01071-beta92-ritsulib0429-additivebatch1-direct-20260621/` with explicit package/Ritsu-compat/RitsuLib-version/game-version target checks, clean audit, `sts1-enabled-mode-report.json`, `runtime-evidence-packet-check.json`, AdditiveBatch1 10 event types / 14 registration calls, and exact act/shared tuple parity. This closes O33 as loader/registration proof only; O51 still requires gameplay packet proof. | O33, O51 |
| Canary Gameplay Subagent | Big Fish, Golden Idol, The Lab, and Divine Fountain screenshots, result logs, pre/post state notes, save-load proof, EN/ZHS render screenshots, and image/license disposition after CanaryOnly proof exists. | O26-O29, O31-O38, O40-O41 |
| Simple Batch Gameplay Subagent | Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, and Shining Light screenshots, result logs, save-load proof, EN/ZHS render screenshots, image/license disposition, and independent QA with AdditiveBatch1 loader proof already retained. | O42-O52 |
| Localization Gap Closure Subagent | Versioned resource pass for `STS1_GOLDEN_IDOL.pages.LEAVE.description` first, then the remaining 32 source-referenced keys; update package version, package docs, handoff docs, and localization baseline guards in the same validated pass. Closing only the direct Golden Idol key is not O33 AdditiveBatch1 proof or gameplay proof. | O19, O35-O36, O49, O64 |
| Asset + Image Subagent | Image/license plan for canary and simple-batch events, including owner-provided licensed art, local extraction hash proof, generated replacements, or explicit non-parity placeholders before any render claim. | O37, O45, O50, O61, O72 |
| Event Pool / RNG / Save Subagent | Owner-approved debug/unsafe replacement packet proving unknown-room draw, act bucket, event-bag/no-repeat behavior, and save-load stability. | O54-O57 |
| Multiplayer / IsShared Subagent | Runtime fail-closed multiplayer proof before any multiplayer gameplay claim; source `IsShared` classification remains static evidence only. | O58 runtime proof; O59 static classification |
| QA / Red-Team Subagent | Independent pass/fail QA only after current runtime/gameplay packets exist. | O65 |
| Release Documentation Subagent | Update current-validation, status-board, hard-stop/monthly review, private beta handoff, release checklist, and owner action list from evidence paths. | O66-O71 |
| Owner / Final Handoff | Make explicit owner decisions for commit/push scope, final blocked-gate summary, and the all-gates-green-before-completion invariant. | O72-O76 |

## Non-Claims

- Static subagent coverage does not close gameplay, save/load, replacement, multiplayer, image/render, localization-resource, or QA gates; O25/O33/O39 were closed separately by retained runtime evidence as loader/audit proof only.
- Read-only explorer audits are not independent QA/Red-Team acceptance.
- This ledger does not authorize commit, push, release, or private-beta readiness claims.
