# StS1 Event Port 严格审核 v20 — beta.86 / v0.107 当前状态、June Dev Spec、Subagent 与 Overnight Run

日期：2026-06-18
项目：`dev-the-spire` / `Spire Plus` / technical id `EZMicroBalance`
审查对象：助理关于“将《杀戮尖塔 1》事件迁移到 StS2 Mod”的当前工作状态。
最新证据基线：GitHub 当前 `README.md`、`docs/reviews/current-validation.md`、`docs/features/sts1-events/status-board.md`。

---

## 0. 总结论

**未完成。**

当前项目相比 v19 又有实质进展：beta.85 在 StS2 `v0.107.0` + RitsuLib `v0.4.16` 下的 **Off loader smoke 和 CanaryOnly enabled-mode loader proof 已 clean**，API drift 的关键 red blocker 已修复；随后 beta.86 package/source alignment 让 AdditiveBatch1 enabled-mode loader/registration proof 也达到 clean。

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
Current beta.85 default-Off loader: pass
Current beta.85 CanaryOnly enabled-mode loader: pass
Current beta.86 AdditiveBatch1 enabled-mode loader/registration: pass
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

Latest pause-safe static checkpoint after this beta.86 doc-alignment pass: `scripts/check-sts1-event-current-doc-claims.ps1 -FailOnMismatch` returned 962 checks / 0 mismatches after the v20 subagent coverage, status-board header, current-doc summary, optional no-launch preflight script, test-plan preflight prerequisite, read-only preflight guards, PROJECT_STATE static-summary alignment, active current-guidance route alignment, historical-review current-route alignment, v20 hard-stop report, v20 O76-O84 final-gate overlay, tuple-aware enabled-mode log verifier guards, CanaryOnly current-pass guard, repo-manifest runtime-preflight drift guard, beta.86 AdditiveBatch1 doc alignment, retained-loader subagent split, current pause-state snapshot alignment, and current diff-check wording were guarded. The latest beta.86-target read-only `scripts/check-sts1-runtime-preflight.ps1 -FailOnMismatch` returned 27 checks / 0 mismatches because both the repo and installed `EZMicroBalance.json` now report `v0.1.0-private-beta.86`; `scripts/check-sts1-event-static-suite.ps1` returned 15 static steps / 0 suite failures with the known 33-key localization gap; `scripts/check-sts1-static-file-hygiene.ps1 -FailOnMismatch` returned 11 checks / 0 mismatches; `scripts/check-sts1-v19-gate-ledger.ps1 -FailOnMismatch` returned 534 checks / 0 mismatches; `scripts/check-sts1-v20-final-gate-overlay.ps1 -FailOnMismatch` returned 29 checks / 0 mismatches; `scripts/check-sts1-v19-subagent-coverage.ps1 -FailOnMismatch` returned 70 checks / 0 mismatches; focused `git diff --check --` on the touched event-governance docs and guard script exited 0 with only CRLF warnings. This is static/preflight evidence and does not itself close gameplay, save/load, replacement, multiplayer, QA, release, or handoff gates.

Shared validation update from the migration lane, 2026-06-18: fresh beta.85 CanaryOnly proof under `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/` reached main menu, audited clean, and passed retained log/packet verifiers with 4 event types / 6 registration calls. The beta.85 AdditiveBatch1 evidence under `.tools/runtime-evidence/v01070-beta85-additive-batch1-20260617-233759/` remains root-cause history for the 13/14 package/source-shape drift. The beta.86 direct AdditiveBatch1 proof under `.tools/runtime-evidence/v01070-beta86-additive-batch1-direct-20260618-031254/` reached main menu, audited clean, reported Spire Plus `v0.1.0-private-beta.86`, RitsuLib `0.4.16`, compat branch `0.107.0`, 25/25 Spire Plus patches, 30 SavedSpireFields, 10 event types / 14 registration calls, exact act/shared tuple parity including The Cleric in Overgrowth and Underdocks, retained log verifier 21 / 0, and packet verifier 45 / 0. Treat O25 and O33 as loader/registration proof only, and do not treat either as gameplay, save-load, render, replacement, multiplayer, QA, release, or handoff evidence.

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
Current split coverage after beta.86 post-doc/test reconciliation: 489 passed / 0 failed / 39 skipped / 528 total
Opt-in installed-artifact lane: 67 passed / 0 failed / 2 skipped / 69 total
Static suite: 15 static steps / 0 suite failures
current-doc-claims: 962 checks / 0 mismatches
runtime-preflight: 27 checks / 0 mismatches (repo and installed package versions beta.86)
static-file-hygiene: 11 checks / 0 mismatches
v19 gate ledger: 534 checks / 0 mismatches
v20 final-gate overlay: 29 checks / 0 mismatches
v19 subagent coverage: 70 checks / 0 mismatches
git diff --check: exit 0 with CRLF normalization warnings only; no whitespace errors
```

严格解释：

- Source/test/static guard 层面可以算强进展。
- `0 warnings / 0 errors` 已保留到 beta.86 package-alignment 与 post-doc/test reconciliation 证据。
- skipped tests 已解释为 release-artifact/runtime/local-source gating。
- 这些仍然不等于 gameplay proof。
- 当前文档明确说没有 gameplay、clicked UI、save-load、co-op、event encounter、replacement、independent QA、release handoff proof。

---

## 3. 当前 runtime / loader 状态

### 3.1 已经通过的 loader 部分

当前 beta.85 Off proof：

```text
v0.107.0
RitsuLib 0.4.16 / compat branch 0.107.0
Spire Plus v0.1.0-private-beta.85
25/25 Spire Plus ModPatcher patches applied
StS1Events default Off
main menu reached
godot-log-audit clean with 0 blocking signature hits
installed beta.85 package parity passed
```

严格解释：

- v19 的 `v0.107 Off smoke red` 已被 beta.85 Off proof superseded。
- 当前 default-Off loader proof 可以算通过。
- 这只证明 Off path，不证明 CanaryOnly、AdditiveBatch1、gameplay、save/load、replacement、multiplayer、QA。

当前 beta.85 CanaryOnly enabled-mode loader proof：

```text
v0.107.0
RitsuLib 0.4.16 / compat branch 0.107.0
Spire Plus v0.1.0-private-beta.85
StS1Events CanaryOnly mode
4 event types / 6 registration calls
main menu reached
godot-log-audit clean with 0 blocking signature hits
retained enabled-mode log/packet verifiers passed with 0 mismatches
tuple-aware copied-log dry-run returned 21 checks / 0 mismatches
```

严格解释：

- Current CanaryOnly loader registration proof can be treated as current-pass for `O25` and loader-packet `O39`.
- It still does not prove Big Fish, Golden Idol, The Lab, or Divine Fountain gameplay, result state, save/load, EN/ZHS render, image/license/render, replacement, multiplayer, QA, or handoff readiness.
- Do not derive AdditiveBatch1 proof from CanaryOnly proof.

当前 beta.86 AdditiveBatch1 enabled-mode loader proof：

```text
v0.107.0
RitsuLib 0.4.16 / compat branch 0.107.0
Spire Plus v0.1.0-private-beta.86
StS1Events AdditiveBatch1 mode
10 event types / 14 registration calls
main menu reached
godot-log-audit clean with 0 blocking signature hits
retained enabled-mode log verifier 21 / 0
retained runtime packet verifier 45 / 0
```

严格解释：

- Current AdditiveBatch1 loader registration proof can be treated as current-pass for `O33`.
- It still does not prove event encounter gameplay, result state, save/load, EN/ZHS render, image/license/render, replacement functional behavior, multiplayer, QA, or handoff readiness.

### 3.2 仍未通过的部分

当前 validation 明确说：

```text
Beta.85 Off, beta.85 CanaryOnly, and beta.86 AdditiveBatch1 loader proof must not be extended to:
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
Current enabled-mode runtime counts: beta.85 CanaryOnly 4 event types / 6 registration calls pass; beta.86 AdditiveBatch1 10 event types / 14 registration calls pass. The beta.85 AdditiveBatch1 13/14 `Sts1TheCleric` mismatch remains root-cause history for stale package/source shape.
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
| CanaryOnly enabled-mode | Current beta.85 proof retained | Pass for loader proof only |
| AdditiveBatch1 enabled-mode | Current beta.86 direct proof retained: 10 event types / 14 calls | Pass for loader proof only |
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
- preserve current beta.85 CanaryOnly loader proof
- preserve current beta.86 AdditiveBatch1 loader/registration proof
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
StS1 Event Port Prototype Batch 1 — beta.86 Enabled Runtime Foundation
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
9. beta.86 AdditiveBatch1 loader proof captured:
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
| O26 | beta.86 package parity proof |
| O27 | beta.86 package SHA recorded |
| O28 | BaseLib/RitsuLib/Spire Plus path report |
| O29 | Active godot.log archived |
| O30 | beta.85 Off loader audit clean |
| O31 | Off runtime proof: 0 StS1 registrations |
| O32 | beta.85 CanaryOnly loader audit clean |
| O33 | CanaryOnly runtime proof: 4 event types / 6 registration calls |
| O34 | beta.86 AdditiveBatch1 loader audit clean |
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
   - beta.85 package,
   - BaseLib,
   - RitsuLib v0.4.16,
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

最新证据显示 beta.85 已经修复 v0.107 Off loader 红灯，并且 CanaryOnly enabled-mode loader proof 已经通过：Off smoke clean，25/25 patches applied，StS1Events default Off，package parity pass；CanaryOnly retained verifiers 证明 4 event types / 6 registration calls。beta.86 direct AdditiveBatch1 proof 也已经通过 loader/registration 层面：10 event types / 14 registration calls，retained log verifier 21 / 0，packet verifier 45 / 0。source/test/static 也有强进展：build 0/0，current split coverage 489 passed / 0 failed / 39 skipped / 528 total，installed artifact lane 67 passed / 0 failed / 2 skipped / 69 total，static suite 15 steps / 0 failures。

但是 beta.85 Off、beta.85 CanaryOnly、beta.86 AdditiveBatch1 loader proof 都不能外推到 gameplay、save-load、replacement、multiplayer、image/render 或 QA。beta.85 AdditiveBatch1 13/14 mismatch 现在只保留为 package/source-shape drift 的历史 root-cause 证据。当前 runtime parity 仍未完成，因为还没有 event encounter gameplay、save-load、render、replacement、multiplayer、independent QA 或 handoff proof。

coordination pause 解除后，继续 Mandatory Overnight Run v20。只能在 O0-O84 全绿后停止。若当前环境无法完成某 gate，写 HARD STOP BLOCKER REPORT，但 blocked gate 不得标完成。

优先级：
1. 保持 beta.85 Off clean proof。
2. 保持 beta.85 CanaryOnly loader proof：4 event types / 6 registration calls。
3. 保持 beta.86 AdditiveBatch1 loader proof：10 event types / 14 registration calls。
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
- 不要把 beta.85 Off、beta.85 CanaryOnly、beta.86 AdditiveBatch1 loader proof 外推成 gameplay proof。
- 不要把 registry count 当 StS1 experience。
- 不要把 hard-stop report 当 completion。
- 不要在相关 enabled-mode 和 gameplay 证据 clean 前推进 gameplay claims。
- 不要在 verified scope 绿之前扩大到 broad Phase。
结论：**当前仍未完成**，但审查口径要更新到最新 beta.86 状态：**v0.107.0 的 Off loader 红灯已经修掉，CanaryOnly enabled-mode loader proof 已通过，AdditiveBatch1 loader/registration proof 也已通过；现在真正阻塞点转移到 gameplay、save/load、replacement、image/render、multiplayer 和独立 QA。**

新版完整审查内容已内联在本文件；不要依赖 sandbox-only 下载链接作为当前证据。

## 1. 当前严格审核结论

当前可以认可的进展：

| 模块                      | 审核结论                                                                                                                                    |
| ----------------------- | --------------------------------------------------------------------------------------------------------------------------------------- |
| Build                   | 最新记录为 `0 warnings / 0 errors`                                                                                                           |
| Test matrix             | beta.86 post-doc/test reconciliation 记录为 `489 passed / 0 failed / 39 skipped / 528 total`                                                  |
| Installed artifact lane | `67 passed / 0 failed / 2 skipped / 69 total`                                                                                           |
| Static suite            | `15 static steps / 0 suite failures`                                                                                                    |
| Current doc claims      | 后续静态检查最高记录为 `962 checks / 0 mismatches`                                                                                                 |
| beta.85 Off loader      | **clean**：v0.107.0 + RitsuLib 0.4.16，25/25 patches，StS1Events default Off                                                               |
| beta.85 CanaryOnly loader | **clean**：retained verifiers 证明 4 event types / 6 registration calls                                                                    |
| beta.86 AdditiveBatch1 loader | **clean**：retained verifiers 证明 10 event types / 14 registration calls                                                                   |
| Source parity fixes     | Big Fish Box identity、Divine Fountain curse prerequisite、Golden Idol trap branch identities、The Lab Open-only 都有 source/static guard 改进 |
| Default Off             | 当前可认可为 loader proof 层面通过                                                                                                                |

这些都有当前 validation 记录支持：beta.85 Off proof 显示 `v0.1.0-private-beta.85`、RitsuLib `0.4.16`、compat branch `0.107.0`、25/25 patches、StS1Events default Off、main menu reached、audit clean；beta.85 CanaryOnly proof 显示 4 event types / 6 registration calls；beta.86 AdditiveBatch1 proof 显示 10 event types / 14 registration calls。当前文档也明确这些 proof 只覆盖 loader/registration 层面，不覆盖 gameplay、save-load、replacement、multiplayer、image/render 或 QA。

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

当前 validation 明确说：没有 gameplay、clicked UI、save-load、co-op、event encounter、replacement、independent QA、release handoff proof；并且 beta.85 Off、beta.85 CanaryOnly、beta.86 AdditiveBatch1 loader proof 不得扩展到 gameplay、save-load、replacement、multiplayer 或 QA gates。

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
- 保持 beta.85 Off clean proof
- 保持 beta.85 CanaryOnly enabled-mode proof
- 保持 beta.86 AdditiveBatch1 enabled-mode proof
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

**`StS1 Event Port Prototype Batch 1 — beta.86 Enabled Runtime Foundation`**

月末 Go/No-Go：

1. Build 保持 `0 errors / 0 warnings`。
2. Test matrix 全绿，包括 release evidence split、installed artifact lane、static suite。
3. skipped tests 继续按 release-artifact/runtime/local-source gating 解释。
4. Current-doc-claims、gate-ledger、subagent coverage、static-file hygiene 全部 pass。
5. Worktree clean 或 owner-approved dirty scope。
6. beta.85 Off loader clean proof 保留。
7. beta.85 CanaryOnly loader proof 捕获：4 event types / 6 registration calls。
8. beta.86 AdditiveBatch1 loader proof 捕获：10 event types / 14 registration calls。
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
| O26-O35 | beta.86 package parity/SHA、BaseLib/RitsuLib paths、godot.log、Off/Canary/Additive clean loader audits                                               |
| O36-O37 | 33 个 localization key gap ledger current，且 closed 或 owner-deferred                                                                                |
| O38-O50 | 4 canary code review、runtime screenshots/result logs/pre-post、save-load、EN/ZHS、image/license、Big Fish/Golden Idol/Lab/Divine Fountain gap closure |
| O51-O61 | 6 simple batch spec/code/runtime/save-load/localization/image proof                                                                               |
| O62-O66 | ReplacementPrototype source guard、unknown-room proof、Act bucket、event bag、save-load                                                               |
| O67-O73 | multiplayer、IsShared、combat blockers、temporary substitutes、content parity、asset/license、ZHS screenshots                                           |
| O74-O84 | independent QA、current-validation、status-board、monthly review、handoff、owner actions、no unsupported commit/push、final honest summary               |

## 7. 必须使用 subagent

必须启动这些 subagent，且实现者不能自验：

1. **BuildGate / Repo Health**：build/test/static/format/diff/patch/worktree、zero-warning、skipped tests。
2. **Runtime Environment Bootstrap**：beta.85 package、BaseLib、RitsuLib v0.4.16、EZMicroBalance install、godot.log、loader audit。
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

最新证据显示 beta.85 已经修复 v0.107 Off loader 红灯，并且 beta.85 CanaryOnly 与 beta.86 AdditiveBatch1 enabled-mode loader proof 都已经通过：Off smoke clean，25/25 patches applied，StS1Events default Off，CanaryOnly 4 event types / 6 registration calls，AdditiveBatch1 10 event types / 14 registration calls。source/test/static 也有强进展：build 0/0，current split coverage 489 passed / 0 failed / 39 skipped / 528 total，installed artifact lane 67 passed / 0 failed / 2 skipped / 69 total，static suite 15 steps / 0 failures。

但是 beta.85 Off、beta.85 CanaryOnly、beta.86 AdditiveBatch1 loader proof 都不能外推到 gameplay、save-load、replacement、multiplayer、image/render 或 QA。AdditiveBatch1 的 beta.85 13/14 mismatch 只保留为历史 drift 诊断；当前 runtime parity 仍未完成，因为 gameplay/save-load/render/replacement/multiplayer/QA proof 都缺失。

继续 Mandatory Overnight Run v20。只能在 O0-O84 全绿后停止。若当前环境无法完成某 gate，写 HARD STOP BLOCKER REPORT，但 blocked gate 不得标完成。

优先级：
1. 保持 beta.85 Off clean proof。
2. 保持 beta.85 CanaryOnly loader proof：4 event types / 6 registration calls。
3. 保持 beta.86 AdditiveBatch1 loader proof：10 event types / 14 registration calls。
4. 处理或 owner-defer 33 个 localization result-page key gaps。
5. 做 4 canary gameplay：screenshots、result logs、pre/post state、save/load、EN/ZHS、image/license/render。
6. 做 6 simple batch gameplay。
7. 功能性证明 ReplacementPrototype。
8. Combat events 在 encounter models 完成前继续 blocked。
9. Temporary substitutes 必须继续标 non-parity。
10. 启动 subagents，QA/Red-Team 必须独立验收。
11. 不要 commit/push，除非 validation evidence 支持本次准确 scope。
```

管理红线：**不要把 source/test/static pass 当 gameplay completion；不要把 beta.85 Off、beta.85 CanaryOnly、beta.86 AdditiveBatch1 loader proof 外推成 gameplay proof；不要把 registry count 当 StS1 experience；不要把 hard-stop report 当 completion；不要在相关 gameplay 证据 clean 前推进 gameplay claims。**
