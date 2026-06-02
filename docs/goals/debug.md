## 严格审核结论

当前助理的工作**仍未整体完成**。但这次要比前几轮更精确地区分：

```text
Revision I hard-blocker stop：可以有条件接受
Terminal validation：报告为通过
Runtime / live readiness：未通过
Release readiness：未通过
Commit readiness：未通过
Longhaul audit readiness：未通过
下一步：优化 + 有限推进，但必须 optimization-first
```

我已把新的下月开发规范和夜间运行任务写成文件：
[下载 M5 July Owner-Review & Runtime Closure Spec](sandbox:/mnt/data/devspire_m5_july_owner_review_runtime_closure_spec.md)

---

## 1. 当前状态与目标对比

最新可信状态是：

```text
HEAD: 87820303 (main, origin/main, origin/HEAD) sprint 1
Worktree: dirty, 54 entries
Batch classifier: 0 unclassified
No commit / push / stash / checkout / reset / restore
Build: pass, 0 errors, 89 Sts1Events nullable warnings
Tests: pass, 464 passed / 0 failed / 21 skipped / 485 total
Format: pass
git diff --check: pass
generate-patch-inventory.ps1 -Check: pass
report-worktree-batches.ps1 -FailOnUnclassified: pass
Runtime smoke: blocked
Release-ready: no
Runtime-ready/live-ready: no
```

阻塞点是：

```text
E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib missing
active godot.log missing
No Off=0 runtime proof
No CanaryOnly=4 runtime proof
```

这些来自当前上传的工作记录：助理报告了 `87820303`、54 个 dirty entries、0 unclassified、验证命令通过，同时 runtime smoke 因缺少 `STS2-RitsuLib` 运行时目录和 active `godot.log` 被阻塞。

我们的目标是：

```text
1. 当前 HEAD / worktree 状态真实可解释
2. Terminal validation 当前状态上通过
3. Dirty / untracked files 全部进入 owner-review
4. warning-ledger 没有 TBD
5. Sts1Events / Debug / RitsuLib 状态真实
6. 没有 runtime evidence 时不写 runtime-ready / release-ready
7. owner 未授权前不 commit
8. baseline 稳定后再恢复 one-file longhaul audit
```

当前达成了：

```text
Terminal validation 报告为绿
Batch classifier 为 0 unclassified
RitsuLib / Sts1Events / Debug 状态没有再过度写成 release-ready
助理在 runtime hard blocker 处停止，没有继续乱推进
```

当前没达成：

```text
54 dirty entries 还没有 owner-review 处理
89 warnings 还没治理
RitsuLib runtime 缺失
active godot.log 缺失
Off=0 / CanaryOnly=4 runtime proof 缺失
commit slices 还没有 owner 授权
longhaul audit 不能恢复
```

因此，当前不是失败回滚状态，而是：

```text
Validation green, runtime hard-blocked, owner-review pending.
```

---

## 2. 是否完成？

### 可以接受

可以接受的是：

```text
Revision I overnight run 按 hard-blocker 规则正确停止。
```

原因是助理没有继续 commit / push / stash / checkout / reset / restore，并明确记录了 runtime blocker。

这比早期状态好很多。之前记录里，助理曾经在测试还有 1 个失败、format timeout 的情况下写 “Format: Clean”，还把 WorktreeBatchScript 失败说成 “needs commit”；这类错误判断现在没有再次出现。

### 不能接受

不能接受这些说法：

```text
全部完成
release-ready
runtime-ready
RitsuLib hard dependency done
Sts1Events formal-ready
Debug feature-complete
可以继续 PR6 Batch4c / PR7
可以恢复 longhaul audit
可以直接 commit
```

因为当前仍有 54 dirty entries、89 warnings、缺 RitsuLib runtime、缺 active godot.log、缺 Off=0 / CanaryOnly=4 runtime proof。

---

## 3. 每一步严格检查

### 3.1 Git / 工作树

当前报告：

```text
HEAD: 87820303
Worktree: dirty, 54 entries
0 unclassified
No commit/push/stash/checkout/reset/restore
```

审核结论：

```text
Git 行为这轮合格，但 worktree 未收口。
```

54 个 dirty entries 必须进入 owner-review packet。每个条目都要有：

```text
path
tracked/untracked
batch
purpose
owner
risk
commit slice
rollback option
validation coverage
owner decision needed
```

否则不能 commit。

---

### 3.2 Terminal validation

当前报告：

```text
dotnet clean + build: pass, 0 errors, 89 warnings
test: pass, 464 passed / 0 failed / 21 skipped
format: pass
diff check: pass
patch inventory check: pass
batch classifier: pass
```

审核结论：

```text
Terminal validation：通过。
```

但必须限定：

```text
Terminal validation 不等于 runtime validation，也不等于 release readiness。
```

项目自己的 `test-ready-development-goal.md` 也明确区分了代码/config 验证、resource/manifest/package 验证，以及 live/manual evidence；命令验证不能替代 live-game、save-load、death/failure、co-op evidence。

---

### 3.3 Runtime smoke

当前 runtime blocker：

```text
STS2-RitsuLib missing
active godot.log missing
No Off=0 runtime proof
No CanaryOnly=4 runtime proof
```

审核结论：

```text
Runtime hard blocker：真实。
```

RitsuLib 当前只能写：

```text
compile/manifest attempted; runtime unverified。
```

不能写：

```text
hard dependency done
runtime validated
release-ready
```

此前记录也明确说明 RitsuLibBootstrap 是 runtime integration，且缺少 RitsuLib runtime 时会导致 loader 风险；因此没有 runtime DLL 和 loader log 不能宣称 runtime-ready。

---

### 3.4 RitsuLib

当前决策：

```text
RitsuLib: compile/manifest attempted, runtime unverified
Batch 4c: blocked
```

审核结论：

```text
真实，但未完成。
```

禁止继续：

```text
PR6 Batch4c
PR6 Batch5
PR7
high-risk patch migration
```

原因是 runtime gate 没过。

---

### 3.5 Sts1Events

当前决策：

```text
Sts1Events: staging-only
```

审核结论：

```text
可以接受为 staging-only。
不能 formalize。
不能 release claim。
```

原因：

```text
89 nullable warnings
无 Off=0 runtime proof
无 CanaryOnly=4 runtime proof
runtime gameplay unverified
```

另外，Sts1Events 不能再被说成 untracked / unrelated。历史记录里它已经被纳入 source manifest、export preset、localization，并通过 exclusion 处理 API-incompatible 文件；由于 `.csproj` 默认编译 `EZMicroBalanceCode/**/*.cs`，它位于该树下就会影响项目。

---

### 3.6 Debug

当前决策：

```text
Debug: accept-scaffold
```

审核结论：

```text
可以接受为 scaffold。
不能写 feature-complete。
```

下一步仍需记录：

```text
Warn() policy
LogPreview dead-code/use-remove decision
settings exposure or internal-only decision
behavioral test coverage
side-effect audit
```

---

### 3.7 Patch inventory

当前对齐：

```text
25 migrated IPatchMethod
142 raw HarmonyPatch declarations
167 tracked patch units
generate-patch-inventory.ps1 -Check passed
```

审核结论：

```text
source-level patch inventory truth improved。
```

但这不是继续 Batch 4c 的理由。runtime gate 仍未通过。

---

## 4. 综合判断：继续优化、推进，还是两者兼顾？

我的决定是：

```text
两者兼顾，但 optimization-first。
```

### 优化优先，70%

先处理：

```text
1. 54 dirty entries owner-review packet
2. 89 warnings warning ledger
3. STS2-RitsuLib missing runtime blocker
4. godot.log missing blocker
5. Off=0 / CanaryOnly=4 runtime proof absence
6. RitsuLib runtime truth docs
7. Sts1Events staging-only docs
8. Debug scaffold docs
9. commit slices
```

### 有限推进，30%

只推进：

```text
1. runtime dependency installation plan
2. runtime smoke plan
3. owner decision packet
4. Sts1Events staging governance
5. RitsuLib runtime verification readiness
```

### 不能推进

```text
PR6 Batch4c
PR6 Batch5
PR7
high-risk patch migration
debug expansion
Sts1Events formalization
longhaul audit
release packaging claim
runtime-ready claim
```

---

## 5. 下个月开发规范：M5 July 2026

考虑到当前日期已经进入 2026 年 6 月，下个月应定义为 **M5 July 2026**；当前剩余的 M4 收口也可以作为 M5 Week 1 的第一目标。

M5 总目标：

```text
Owner-review packet + runtime hard-blocker closure + governance decisions + longhaul recovery。
```

下载文件：
[devspire_m5_july_owner_review_runtime_closure_spec.md](sandbox:/mnt/data/devspire_m5_july_owner_review_runtime_closure_spec.md)

### Week 1：Owner Review + Runtime Hard-Blocker Closure

目标：

```text
把当前 “validation green, runtime hard-blocked” 状态转成 owner 可审、可回滚的 packet。
```

必须产出：

```text
docs/goals/m5-week1-owner-review-packet.md
docs/goals/m5-week1-runtime-hard-blocker.md
docs/goals/m5-week1-runtime-smoke-plan.md
docs/goals/m5-week1-dirty-ledger.md
docs/goals/m5-week1-warning-ledger.md
docs/goals/m5-week1-commit-slices.md
updated docs/goals/overnight-run-status.md
updated docs/goals/overnight-run-ledger.md
updated harness/TASK_STATUS.md
updated harness/TASK_FOCUS_PACK.md
```

必须跑：

```powershell
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet build .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

完成条件：

```text
A. STS2-RitsuLib runtime exists, runtime smoke prerequisites pass, and smoke evidence is collected；
或
B. STS2-RitsuLib runtime remains missing, exact hard blocker + owner action are documented。
```

---

### Week 2：Governance Decisions

默认决策：

```text
Sts1Events: staging-only
Debug: accept-scaffold
RitsuLib: compile/manifest attempted; runtime unverified
```

Promotion 只能在有证据后发生。

Sts1Events formal 前必须完成：

```text
89 nullable warnings fixed or explicitly accepted
ZHS localization backlog resolved
Off=0 runtime proof collected
CanaryOnly=4 runtime proof collected
event images/resources resolved or explicitly deferred
blocked combat rows resolved
manual runtime plan exists
```

Debug feature-complete 前必须完成：

```text
settings exposure or explicit internal-only policy
dedicated behavioral tests
Warn policy docs
LogPreview use/remove decision
side-effect audit
```

RitsuLib runtime-validated 前必须完成：

```text
STS2-RitsuLib runtime installed
loader smoke passes
godot.log audit clean
package/handoff docs align if manifest dependency remains
runtime fallback or install-enforced hard-dependency decision documented
```

---

### Week 3：Runtime / Packaging Truth

如果 runtime validation 可做，运行：

```powershell
dotnet publish .\EZMicroBalance.csproj
.\scripts\package-spire-plus.ps1
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

并补：

```text
BaseLib + STS2-RitsuLib + Spire Plus loader smoke
godot.log audit
tester handoff dependency instructions
package/hash/version docs
runtime fallback or install-enforced hard-dependency decision
```

---

### Week 4：恢复 one-file longhaul audit

只有这些条件都满足后恢复：

```text
owner-review packet accepted
commit / rollback decisions made
terminal validation still green
runtime blocker closed or explicitly owner-deferred
Sts1Events / Debug / RitsuLib governance decisions recorded
no stale truth blockers remain
```

第一批文件：

```text
1. EZMicroBalance.csproj
2. EZMicroBalance.json
3. EZMicroBalanceCode/MainFile.cs
4. EZMicroBalanceCode/Core/Integrations/RitsuLib/RitsuLibBootstrap.cs
5. EZMicroBalanceCode/Sts1Events/Sts1EventFeatureGate.cs
6. EZMicroBalanceCode/Sts1Events/Sts1EventRegistrationService.cs
7. tests/EZMicroBalance.Tests/RitsuLibMigrationGuardTests.cs
8. tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.cs
9. scripts/generate-patch-inventory.ps1
10. docs/integrations/ritsulib.md
```

每轮只允许一个文件，结果只能是：

```text
fixed
skipped
blocked
```

---

## 6. 必须提醒他使用 subagent

下一轮必须继续强制 subagent。主 agent 不能直接自我验收。

必用：

```text
GitForensicsAgent
RuntimeDependencyAgent
RuntimeSmokeAgent
DirtyStateReconciliationAgent
WarningLedgerAgent
RitsuLibRuntimeAgent
Sts1EventsGovernanceAgent
DebugDecisionAgent
PatchInventoryAgent
TestChangeReviewAgent
DocsTruthAgent
CommitSliceAgent
LocalizationBacklogAgent
```

特别强调：

```text
RuntimeDependencyAgent 必须先查 STS2-RitsuLib runtime path。
RuntimeSmokeAgent 只有在 runtime dependency 存在后才能运行。
CommitSliceAgent 只能准备 commit plan，不允许 commit。
```

---

## 7. 夜间运行任务：必须跑到完成才能停

你可以直接发给他：

```text
进入 M5 July Week 1 overnight owner-review and runtime hard-blocker closure run。

当前状态不是 complete。Revision I 正确停止在 runtime hard blocker：STS2-RitsuLib runtime path missing、active godot.log missing、Off=0/CanaryOnly=4 runtime proof absent。不要继续 PR6 Batch4c、PR6 Batch5、PR7、debug expansion、Sts1Events formalization 或 longhaul audit。

禁止：
- commit
- push
- stash / stash drop
- checkout branch
- reset / restore
- broad clean
- 写 runtime verified / release-ready，除非有 runtime evidence

你不能停止，直到满足以下之一：

A. Ready-to-owner-review packet complete:
- runtime hard blocker either closed or precisely documented
- current dirty files fully reconciled
- untracked files have decisions
- terminal validation commands exit 0
- warning-ledger has no TBD
- 89 warnings classified by file/code/owner
- Sts1Events formal/staging/remove recommendation recorded
- Debug accept-scaffold/feature-complete/rollback recommendation recorded
- RitsuLib attempted/runtime-validated/release-ready/rollback status recorded
- patch inventory raw/migrated/tracked unit relationship explained
- test changes reviewed
- localization backlog recorded
- commit slices complete
- no unauthorized commit

B. Hard blocker:
- exact command/file/path
- why current worktree cannot resolve it
- rollback/staging/owner-decision options
- exact owner action required

必须先读：
AGENTS.md
PROJECT_STATE.md
docs/README.md
docs/test-ready-development-goal.md
docs/worktree-cleanup-audit.md
docs/patch-inventory.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/overnight-diff-ledger.md
docs/goals/warning-ledger.md
docs/integrations/ritsulib.md
docs/migration.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

必须先使用 subagents，只调查后修改：
1. GitForensicsAgent
2. RuntimeDependencyAgent
3. RuntimeSmokeAgent
4. DirtyStateReconciliationAgent
5. WarningLedgerAgent
6. RitsuLibRuntimeAgent
7. Sts1EventsGovernanceAgent
8. DebugDecisionAgent
9. PatchInventoryAgent
10. TestChangeReviewAgent
11. DocsTruthAgent
12. CommitSliceAgent
13. LocalizationBacklogAgent

RuntimeDependencyAgent 必须先验证：
- Game root
- BaseLib install
- STS2-RitsuLib install
- EZMicroBalance install
- runtime DLL presence
- active godot.log availability or absence

ValidationReplay 必须运行：
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet build .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified

必须创建或更新：
docs/goals/m5-week1-owner-review-packet.md
docs/goals/m5-week1-runtime-hard-blocker.md
docs/goals/m5-week1-runtime-smoke-plan.md
docs/goals/m5-week1-dirty-ledger.md
docs/goals/m5-week1-warning-ledger.md
docs/goals/m5-week1-commit-slices.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

最终报告只能写两种之一：
Complete: ready-to-owner-review packet complete.
Not complete: exact hard blocker encountered.

不要因为 build/tests pass 就写 complete。
不要因为 runtime path missing 就继续猜。
不要因为 RitsuLib compiles 就写 runtime-ready。
不要未经 owner 授权 commit。
```

---

## 一句话总评

他这轮做对了关键动作：**在 runtime hard blocker 处停止，而不是继续乱推进。** 所以这轮可以算“正确停止”，但不能算“整体完成”。现在的正确策略是 **继续优化 + 有限推进**：先用 subagents 把 54 dirty entries、89 warnings、RitsuLib runtime 缺失、godot.log 缺失、Off=0 / CanaryOnly=4 proof 缺失全部收口，然后再由你决定 commit、rollback，或进入下一阶段治理。
# DevSpire M5 July 2026 — Runtime Closure, Owner Commit Gate, and Longhaul Recovery Spec

Date: 2026-06-02
Project: `dev-the-spire` / player-facing `Spire Plus` / technical manifest id `EZMicroBalance`

## 0. Strict Audit Verdict

Current assistant work is **not fully complete**.

Accepted as complete only for the narrow stop condition:

```text
Revision I hard-blocker stop: conditionally accepted.
```

Not accepted:

```text
Overall completion.
Release readiness.
Runtime readiness / live readiness.
Commit readiness.
RitsuLib hard dependency completion.
Sts1Events formal feature completion.
Debug feature completion.
Longhaul audit readiness.
PR6 Batch4c / Batch5 / PR7 advancement.
```

Current required wording:

```text
Terminal validation: reported green.
Runtime smoke: blocked by missing STS2-RitsuLib runtime and missing active godot.log.
Project status: not complete.
Release-ready: no.
Runtime-ready/live-ready: no.
Commit-ready: no.
Next action: owner-review packet + runtime hard-blocker closure.
```

## 1. Current State to Preserve

Latest reported state:

```text
HEAD: 87820303 (main, origin/main, origin/HEAD) sprint 1
Worktree: dirty, 54 entries
Batch classifier: 0 unclassified
No commit/push/stash/checkout/reset/restore performed
Build: passed, 0 errors, 89 Sts1Events nullable warnings
Tests: 464 passed / 0 failed / 21 skipped / 485 total
Format: passed
git diff --check: passed
generate-patch-inventory.ps1 -Check: passed
report-worktree-batches.ps1 -FailOnUnclassified: passed
Runtime smoke: blocked
```

Runtime blockers:

```text
E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib is missing.
active godot.log is missing.
No Off=0 runtime proof.
No CanaryOnly=4 runtime proof.
D-drive game root/mod paths are missing.
```

Current governance decisions:

```text
Sts1Events: staging-only
Debug: accept-scaffold
RitsuLib: compile/manifest attempted; runtime unverified
FeatureRegistry: source-level metadata/bootstrap truth guarded; runtime unproven
RewardPipeline/CardPlayContext: diagnostics/canary only; no gameplay claim
DeathProtectionService/MultiplayerPolicy: diagnostics/taxonomy only; no gameplay enforcement claim
Batch 4c: blocked
Strategy: both optimize and advance, but optimization-first
```

Patch inventory truth to preserve:

```text
25 migrated IPatchMethod
142 raw HarmonyPatch declarations
167 tracked patch units
```

## 2. Goals Comparison

Original goals:

1. Recover a truthful green baseline.
2. Use subagents for non-trivial review work.
3. Explain all dirty/untracked files.
4. Keep Sts1Events, Debug, and RitsuLib statuses truthful.
5. Avoid runtime/release claims without runtime evidence.
6. Do not commit without owner authorization.
7. Resume one-file longhaul audit only after owner-review and runtime/governance blockers are closed.

Current comparison:

| Goal | Current status | Result |
| --- | --- | --- |
| Terminal validation green | Reported green | Pass, replay after each change |
| Dirty files explained | 54 dirty, 0 unclassified | Partial; owner-review packet required |
| Warnings classified | 89 Sts1Events nullable warnings | Partial; warning ledger must remain no-TBD |
| Runtime proof | Missing STS2-RitsuLib runtime and godot.log | Fail / hard blocker |
| RitsuLib status truth | compile/manifest attempted, runtime unverified | Truthful, not complete |
| Sts1Events status truth | staging-only | Truthful, not formal |
| Debug status truth | accept-scaffold | Truthful, not feature-complete |
| Commit readiness | dirty worktree | Fail |
| Longhaul readiness | runtime/owner-review blockers open | Fail |

Strategic decision:

```text
Proceed with both optimization and limited advancement, but optimization-first.
```

Allocation:

```text
70% optimization / closure:
- runtime hard-blocker packet
- dirty ledger
- warning ledger
- owner-review packet
- commit slices
- docs truth
- validation replay

30% limited advancement:
- runtime dependency installation plan
- runtime smoke plan
- Sts1Events staging governance
- RitsuLib runtime verification readiness
```

Forbidden advancement before gates close:

```text
PR6 Batch4c
PR6 Batch5
PR7
High-risk patch migration
Debug expansion
Sts1Events formalization
Longhaul audit
Release packaging claim
Runtime-ready claim
```

## 3. Subagent Policy

Main agent must not self-certify. Every non-trivial task must use subagents first.

Required subagents:

1. GitForensicsAgent
2. RuntimeDependencyAgent
3. RuntimeSmokeAgent
4. DirtyStateReconciliationAgent
5. WarningLedgerAgent
6. RitsuLibRuntimeAgent
7. Sts1EventsGovernanceAgent
8. DebugDecisionAgent
9. PatchInventoryAgent
10. TestChangeReviewAgent
11. DocsTruthAgent
12. CommitSliceAgent
13. LocalizationBacklogAgent

Each subagent must report:

```text
scope
files inspected
commands run
findings
risk
recommended action
whether edits are needed
```

Main agent may edit only after subagent findings are summarized.

## 4. July 2026 Monthly Plan

### Week 1 — Owner Review and Runtime Hard-Blocker Closure

Goal:

```text
Turn the green static-validation state into an owner-review packet and close or precisely document the runtime blocker.
```

Required outputs:

```text
docs/goals/m5-week1-owner-review-packet.md
docs/goals/m5-week1-runtime-hard-blocker.md
docs/goals/m5-week1-runtime-smoke-plan.md
docs/goals/m5-week1-dirty-ledger.md
docs/goals/m5-week1-warning-ledger.md
docs/goals/m5-week1-commit-slices.md
updated docs/goals/overnight-run-status.md
updated docs/goals/overnight-run-ledger.md
updated harness/TASK_STATUS.md
updated harness/TASK_FOCUS_PACK.md
```

Required validation:

```powershell
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet build .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

Week 1 completion requires one of:

```text
A. STS2-RitsuLib runtime exists, runtime smoke prerequisites pass, and smoke evidence is collected.
B. STS2-RitsuLib runtime remains missing, and exact hard blocker + owner action are documented.
```

### Week 2 — Governance Decisions

Goal:

```text
Finalize Sts1Events, Debug, and RitsuLib governance state.
```

Default decisions:

```text
Sts1Events: staging-only
Debug: accept-scaffold
RitsuLib: compile/manifest attempted; runtime unverified
```

Promotion requirements:

Sts1Events can become formal only after:

```text
89 nullable warnings fixed or explicitly accepted
ZHS localization backlog resolved
Off=0 runtime proof collected
CanaryOnly=4 runtime proof collected
event images/resources resolved or explicitly deferred
blocked combat rows resolved
manual runtime plan exists
```

Debug can become feature-complete only after:

```text
settings exposure exists or internal-only policy is explicit
dedicated behavioral tests exist
Warn policy is documented
LogPreview dead-code decision is resolved
side-effect audit exists
```

RitsuLib can become runtime-validated only after:

```text
STS2-RitsuLib runtime installed
loader smoke passes
godot.log audit clean
package/handoff docs align if manifest dependency remains
runtime fallback or install-enforced hard-dependency decision documented
```

### Week 3 — Runtime / Packaging Truth

Goal:

```text
Either validate RitsuLib as a true runtime dependency or keep docs explicitly downgraded.
```

If runtime validation is possible, run:

```powershell
dotnet publish .\EZMicroBalance.csproj
.\scripts\package-spire-plus.ps1
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

Also required:

```text
BaseLib + STS2-RitsuLib + Spire Plus loader smoke
godot.log audit
tester handoff dependency instructions
package/hash/version docs
runtime fallback or install-enforced hard-dependency decision
```

### Week 4 — One-file Longhaul Audit Recovery

Resume only after:

```text
owner-review packet accepted
commit/rollback decisions made
terminal validation still green
runtime blocker closed or explicitly owner-deferred
Sts1Events/Debug/RitsuLib governance decisions recorded
no stale truth blockers remain
```

First ten longhaul files:

```text
1. EZMicroBalance.csproj
2. EZMicroBalance.json
3. EZMicroBalanceCode/MainFile.cs
4. EZMicroBalanceCode/Core/Integrations/RitsuLib/RitsuLibBootstrap.cs
5. EZMicroBalanceCode/Sts1Events/Sts1EventFeatureGate.cs
6. EZMicroBalanceCode/Sts1Events/Sts1EventRegistrationService.cs
7. tests/EZMicroBalance.Tests/RitsuLibMigrationGuardTests.cs
8. tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.cs
9. scripts/generate-patch-inventory.ps1
10. docs/integrations/ritsulib.md
```

Each round must end with exactly one status:

```text
fixed
skipped
blocked
```

## 5. Overnight Run Prompt

Use this prompt for the next assistant run.

```text
Enter M5 July Week 1 overnight owner-review and runtime hard-blocker closure run.

Current status is NOT COMPLETE.

Known latest status:
- HEAD: 87820303 (main, origin/main) sprint 1
- Worktree: 54 dirty entries, 0 unclassified
- Terminal validation passed
- Build: 0 errors, 89 Sts1Events nullable warnings
- Tests: 464 passed / 0 failed / 21 skipped
- Patch inventory check passed
- Runtime smoke blocked because E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib is missing
- active godot.log is missing
- no Off=0 runtime proof
- no CanaryOnly=4 runtime proof
- Release-ready: no
- Runtime-ready/live-ready: no

Do not:
- commit
- push
- stash / stash drop
- checkout branch
- reset / restore
- broad clean
- continue PR6 Batch4c, PR6 Batch5, or PR7
- expand debug
- formalize Sts1Events
- resume longhaul audit
- claim runtime verified or release-ready without runtime evidence

You cannot stop until one terminal condition is met:

A. Ready-to-owner-review packet complete:
- runtime hard blocker either closed or precisely documented
- current dirty files fully reconciled
- untracked files have decisions
- terminal validation commands exit 0
- warning-ledger has no TBD
- 89 warnings classified by file/code/owner
- Sts1Events formal/staging/remove recommendation recorded
- Debug accept-scaffold/feature-complete/rollback recommendation recorded
- RitsuLib attempted/runtime-validated/release-ready/rollback status recorded
- patch inventory raw/migrated/tracked unit relationship explained
- test changes reviewed
- localization backlog recorded
- commit slices complete
- no unauthorized commit

B. Hard blocker:
- exact command/file/path
- why current worktree cannot resolve it
- rollback/staging/owner-decision options
- exact owner action required

First read:
AGENTS.md
PROJECT_STATE.md
docs/README.md
docs/test-ready-development-goal.md
docs/worktree-cleanup-audit.md
docs/patch-inventory.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/overnight-diff-ledger.md
docs/goals/warning-ledger.md
docs/integrations/ritsulib.md
docs/migration.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

Use subagents before modifying files:
1. GitForensicsAgent
2. RuntimeDependencyAgent
3. RuntimeSmokeAgent
4. DirtyStateReconciliationAgent
5. WarningLedgerAgent
6. RitsuLibRuntimeAgent
7. Sts1EventsGovernanceAgent
8. DebugDecisionAgent
9. PatchInventoryAgent
10. TestChangeReviewAgent
11. DocsTruthAgent
12. CommitSliceAgent
13. LocalizationBacklogAgent

RuntimeDependencyAgent must first verify:
- Game root
- BaseLib install
- STS2-RitsuLib install
- EZMicroBalance install
- runtime DLL presence
- active godot.log availability or absence

ValidationReplay must run:
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet build .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified

Create or update:
docs/goals/m5-week1-owner-review-packet.md
docs/goals/m5-week1-runtime-hard-blocker.md
docs/goals/m5-week1-runtime-smoke-plan.md
docs/goals/m5-week1-dirty-ledger.md
docs/goals/m5-week1-warning-ledger.md
docs/goals/m5-week1-commit-slices.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

Final report must be either:
Complete: ready-to-owner-review packet complete.
Not complete: exact hard blocker encountered.

Do not write Complete merely because build/tests pass.
Do not write runtime-ready without loader evidence.
Do not write release-ready without publish/package/runtime/handoff evidence.
```