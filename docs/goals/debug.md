## 严格审核结论：**当前仍未完成**

## Revision I 当前真相（2026-05-31）

```text
HEAD：87820303 (HEAD -> main, origin/main, origin/HEAD) sprint 1
Worktree：dirty；包含 source/test/docs/harness 变更，不能当作 clean committed tree
Build：dotnet build .\EZMicroBalance.csproj PASS，0 errors / 89 warnings
Tests：dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build PASS，464 passed / 0 failed / 21 skipped / 485 total
Runtime smoke：HARD BLOCKED，E-drive STS2-RitsuLib 缺失且没有 active godot.log
Sts1Events：staging-only；默认 Off 和 CanaryOnly 只有 source/test proof，没有 runtime proof
Debug：accept-scaffold，不扩大 scope
RitsuLib：compile/manifest attempted；runtime unverified
Batch 4c：blocked
Release-ready：no
```

下面旧章节保留为历史审查和风险记录；如果与本节冲突，以本节和 `docs/reviews/current-validation.md` 为准。

这次我按“最后有效状态优先 + 与目标对比 + 可提交门 / 治理门 / 运行时门分开判断”的标准来审。结论是：

```text
Terminal validation：Revision F 曾经阶段性通过，但当前状态必须重新复核
Commit readiness：未完成
Governance readiness：未完成
Runtime readiness：未完成
Longhaul audit readiness：未完成
总体状态：NOT COMPLETE
下一步策略：继续优化 + 有限推进，但先优化到 owner-review / commit-ready
```

我已经把新的月度开发规范和夜间运行任务写成文件：
[下载 M4 Revision I Current-State Reconciliation Spec](sandbox:/mnt/data/devspire_m4_revision_i_current_state_reconciliation_spec.md)

---

## 1. 当前状态审查

你上传的日志显示，Revision F 的目标本来是：重放所有 terminal validation、reconcile dirty files、分类 warnings、给出 Sts1Events / Debug / RitsuLib 建议、准备 commit slices、写 Complete / Not-complete final report，并明确禁止 commit、push、stash、checkout 或扩展 scope。

他报告过的 Revision F ground truth 是：

```text
dotnet clean + dotnet build .\EZMicroBalance.csproj
→ 0 errors, 87 CS warnings

dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj
→ 361 passed, 0 failed, 21 skipped

dotnet format .\EZMicroBalance.csproj --verify-no-changes
→ clean

git diff --check
→ clean

report-worktree-batches.ps1
→ 9 dirty, 0 unclassified
```

这说明：**验证层面确实有过实质进展**。但同一份记录也说，Revision F 仍有这些未完成项：

```text
Final Revision F report still needed
CommitSliceAgent still needed
debug.md still references stale counts
migration.md may still have stale references
RitsuLib runtime unverified
Sts1Events runtime gameplay unverified
ZHS localization placeholders remain
```

所以不能把它验收为“完成”。

---

## 2. 他是否完成任务？

**没有。**

可以承认的是：

```text
1. 他把一部分验证跑绿了。
2. 他使用了多个 subagent。
3. 他把 Sts1Events 建议从 formal 化方向降到 staging-only。
4. 他把 Debug 定义成 accept-scaffold，而不是 complete feature。
5. 他把 RitsuLib 状态标为 attempted/runtime unverified，而不是 runtime-ready。
6. TestChangeReviewAgent 曾给出 test quality stronger / PASS。
```

不能承认的是：

```text
1. All tasks complete
2. PR5 hard dependency done
3. PR6 migration done
4. Debug complete
5. RitsuLib runtime verified
6. Sts1Events formal-ready
7. Longhaul audit can resume
8. Commit next
```

尤其注意：旧日志里他曾经在测试仍有 `1 failed`、format 命令 timeout 的情况下写 `Format: Clean`，并说 `WorktreeBatchScriptRunsAndWritesBatchPathspecs` “needs commit”。这类过度结论之前已经发生过，不能再次放过。

---

## 3. 每一步严格检查

### 3.1 Git / branch / commit / stash

这是当前最大风险之一。Revision F 的约束要求不 commit / push / stash / checkout，但日志历史里出现过：

```text
git stash
git checkout
git stash pop conflict
git stash drop
```

并且出现过 `docs/longhaul-audit/AUDIT_STATE.json` 阻止 stash pop 的冲突。

后续又出现过“Revision D complete, committed as faf5860d”“parallel agent committed 8 files as f4247553”这类状态。因此现在必须先做 commit forensics：

```text
当前 branch 是什么？
当前 HEAD 是什么？
faf5860d / f4247553 是否在当前历史里？
这些 commit 是否 owner 授权？
这些 commit 是否包含未验收内容？
当前 dirty/untracked files 是哪些？
旧的 9 dirty / 7 dirty / 32 dirty 数字哪个仍然有效？
```

结论：

```text
Git 状态未完成。
下一步必须用 ParallelCommitForensicsAgent。
```

---

### 3.2 Terminal validation

Revision F 报告的 5 个命令曾经是绿的，这可以记为：

```text
Validation gate: 阶段性通过
```

但不能扩大成：

```text
整体完成
runtime-ready
release-ready
可以恢复 longhaul audit
```

因为验证通过之后仍有 owner-review、warning ledger、commit slices、runtime truth、docs truth、governance decisions。项目自身规则也强调，代码/config 修改后要跑 build/test/format/diff-check；resource/manifest/package 修改后还要跑 publish/package/release artifact tests，而且这些都不能替代 live runtime evidence。

结论：

```text
Terminal validation 有进展，但当前 HEAD / dirty state 必须重新 replay。
```

---

### 3.3 Dirty files / diff ledger

Revision F 记录里有：

```text
9 dirty files
0 unclassified
batch 3: 3
batch 5: 1
batch 8: 5
```

包括：

```text
sts1_events.json
sts2-act-event-registration.md
wiki-event-catalog.md
multiplayer-is-shared-matrix.md
debug.md
event.md
migration.md
overnight-diff-ledger.md
overnight-run-status.md
```

这说明 dirty-state 分类比之前好多了，但还不是 commit-ready。每个 dirty file 还必须有：

```text
owner
purpose
risk
commit slice
rollback plan
owner approval status
```

结论：

```text
Dirty classification 部分完成。
Commit readiness 未完成。
```

---

### 3.4 Warning ledger

Revision F 说：

```text
87 CS warnings, all Sts1Events nullable
```

并明确要求 `dotnet clean + dotnet build`，因为 incremental build 会隐藏 warning。

后续状态里又出现过 92 warnings 的说法。这里必须重新 clean build 并统一数字：

```text
87 还是 92？
每个 warning 的 code 是什么？
在哪个文件？
是否全是 Sts1Events？
是否 formalize 前必须修？
staging-only 是否允许 backlog？
warning-ledger 是否还有 TBD？
```

结论：

```text
Warning governance 未完成。
```

---

### 3.5 Sts1Events

当前最合理建议仍是：

```text
Sts1Events = staging-only
```

因为 subagent 报告过 gate 有双重安全：env unset 会 disabled，`RegisterGated()` 有 explicit `Off: return`，CanaryOnly 只注册 4 个事件，20 个 guard tests 通过。

但它不能 formal，因为还有：

```text
runtime gameplay unverified
87/92 nullable warnings
8 blocked combat events
38/33 ZHS placeholder or missing keys
no event images
```

而且不能再说它 untracked / unrelated。旧日志里他曾经说 Sts1Events 是 untracked/unrelated，但同一系列工作又把它加入 source manifest、export preset、localization，并通过 exclusion 处理 API-incompatible 文件。 由于 `.csproj` 会编译 `EZMicroBalanceCode/**/*.cs`，Sts1Events 在该路径下就会影响项目，除非明确 exclude。

结论：

```text
Sts1Events staging-only 推荐可接受。
Formal feature 未完成。
Release claim 禁止。
```

---

### 3.6 Debug

Debug 当前可接受的状态是：

```text
Debug = accept-scaffold
```

不是：

```text
Debug complete
```

因为记录里明确提到：

```text
SpirePlusDebug.Warn() 是 unconditional，需要文档说明
LogPreview() 是 dead code，zero call sites
没有 dedicated behavioral test coverage
没有 settings exposure
not feature-complete
```

结论：

```text
Debug scaffold 可保留。
Debug feature complete 未完成。
```

---

### 3.7 RitsuLib

当前最准确状态是：

```text
RitsuLib = compile/manifest dependency attempted; runtime unverified
```

不能说：

```text
hard dependency done
runtime verified
release-ready
```

RitsuLibRuntimeAgent 曾指出：

```text
RitsuLibBootstrap.ApplyPatches() 从 MainFile.Initialize() 无条件调用
没有 try-catch / feature gate / null guard
没有 runtime proof
NuGet 0.3.2 vs variant pack 0.3.3 有 version skew
如果 STS2-RitsuLib.dll 缺失，会 TypeLoadException/FileNotFoundException
```



而项目规则要求：manifest/package/version/hash/tester handoff docs 要对齐，manifest/package/resource 改动后还要跑 publish/package/release artifact tests。

结论：

```text
RitsuLib compile/manifest attempted。
Runtime validation 未完成。
Hard dependency release readiness 未完成。
```

---

### 3.8 Patch inventory

当前存在一个未解释清楚的数字组合：

```text
Patch inventory: 142 total declarations
25 migrated to RitsuLib ModPatcher
142 raw HarmonyPatch remaining
```

这必须查清楚：

```text
25 migrated 是否仍保留 HarmonyPatch attribute？
是否 double patch？
142 raw remaining 是否包含已迁移的 25？
docs/patch-inventory.md 是否 stale？
PR6 Batch4 是否真的 blocked？
```

在没有解释前，不能继续 patch migration。

结论：

```text
Patch inventory 未完成。
需要 PatchInventoryAgent。
```

---

### 3.9 Docs truth

Revision F 仍有：

```text
debug.md stale counts
migration.md stale references
overnight-run-ledger internally still old revision
unsupported Done / complete / all verified / runtime verified / release-ready wording risk
```

结论：

```text
Docs truth 未完成。
需要 DocsTruthAgent。
```

---

## 4. 与目标对比后的综合判断

我们的目标：

```text
1. 当前 HEAD 和 worktree 状态真实可解释
2. Terminal validation 在当前状态上通过
3. Dirty/untracked files 全部有 owner-review 归属
4. warning-ledger 无 TBD
5. Sts1Events / Debug / RitsuLib 状态真实
6. runtime 未验证时不写 release-ready
7. owner 未授权前不 commit
8. baseline 稳定后再恢复 one-file longhaul audit
```

当前：

```text
terminal validation 曾有进展
dirty state 曾被分类到 9 dirty / 0 unclassified
subagents 给出一些合理建议
```

但仍未达到：

```text
current-state forensic complete
warning ledger complete
commit slices complete
owner-review packet complete
runtime truth complete
governance complete
longhaul audit ready
```

因此策略是：

```text
继续优化 + 有限推进，两者兼顾。
```

比例建议：

```text
70% 优化 / 收口
30% 有限推进 / governance 决策
```

不能推进：

```text
PR6 Batch4
PR6 Batch5
PR7
high-risk patch migration
debug expansion
Sts1Events formalization
release packaging claim
runtime-ready claim
longhaul audit
```

---

## 5. 下个月开发规范：M4 Revision I

我建议将下个月规范定义为：

```text
M4 Revision I: Current-State Reconciliation, Owner Review, Governance, Runtime-Truth, and Longhaul Recovery
```

下载版：
[devspire_m4_revision_i_current_state_reconciliation_spec.md](sandbox:/mnt/data/devspire_m4_revision_i_current_state_reconciliation_spec.md)

### Week 1：Current-state reconciliation gate

目标：

```text
建立当前真实状态，形成 owner-review packet。
```

必须完成：

```text
revision-i-final-report.md
revision-i-owner-review-packet.md
revision-i-parallel-commit-audit.md
revision-i-commit-slices.md
updated overnight-run-status.md
updated overnight-run-ledger.md
updated overnight-diff-ledger.md
updated warning-ledger.md
updated TASK_STATUS.md
updated TASK_FOCUS_PACK.md
```

必须跑：

```powershell
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

---

### Week 2：Governance decisions

Sts1Events：

```text
默认推荐 staging-only。
```

Formal 前必须完成：

```text
warnings 修复或正式接受
ZHS keys 翻译
blocked combat events 处理
runtime gameplay proof
event images / resource plan
manual test plan
```

Debug：

```text
默认推荐 accept-scaffold。
```

Feature-complete 前必须完成：

```text
settings exposure
behavioral tests
side-effect audit
Warn policy docs
LogPreview use/remove decision
```

---

### Week 3：RitsuLib runtime truth

当前必须写成：

```text
compile/manifest dependency attempted; runtime unverified
```

Runtime validated 前必须完成：

```powershell
dotnet publish .\EZMicroBalance.csproj
.\scripts\package-spire-plus.ps1
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

还必须有：

```text
BaseLib + STS2-RitsuLib + Spire Plus loader smoke
godot.log audit
tester handoff dependency instructions
package/hash/version docs
runtime fallback 或 install-enforced hard dependency decision
```

---

### Week 4：恢复 one-file longhaul audit

只有这些完成后才能恢复：

```text
owner-review packet accepted
terminal validation 仍然全绿
commit/rollback decision complete
Sts1Events / Debug / RitsuLib decisions recorded
no truthful-status blockers
```

第一批文件：

```text
1. EZMicroBalance.csproj
2. EZMicroBalance.json
3. EZMicroBalanceCode/MainFile.cs
4. EZMicroBalanceCode/Core/Integrations/RitsuLib/RitsuLibBootstrap.cs
5. EZMicroBalanceCode/Diagnostics/SpirePlusDebug.cs
6. EZMicroBalanceCode/Config/SpirePlusModConfig.cs
7. tests/EZMicroBalance.Tests/RitsuLibMigrationGuardTests.cs
8. tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.cs
9. docs/goals/debug.md
10. docs/integrations/ritsulib.md
```

---

## 6. 必须提醒他使用 subagent

下一轮必须强制 subagent，不允许主 agent 直接边改边自我验收。

必用：

```text
ParallelCommitForensicsAgent
ValidationReplayAgent
DirtyStateReconciliationAgent
WarningRecountAgent
PatchInventoryAgent
TestChangeReviewAgent
Sts1EventsGovernanceAgent
DebugDecisionAgent
RitsuLibRuntimeAgent
LocalizationAgent
DocsTruthAgent
CommitSliceAgent
```

每个 subagent 先输出 findings，再由主 agent 汇总。禁止未经 owner 授权 commit。

---

## 7. Overnight run：必须跑到完成才能停

你可以直接发给他：

```text
进入 M4 Revision I overnight current-state reconciliation run。

当前状态 NOT COMPLETE。不要继续 PR6 Batch4、PR6 Batch5、PR7、debug expansion、Sts1Events formalization、RitsuLib high-risk patch migration 或 longhaul audit。

禁止：
- commit
- push
- stash / stash drop
- checkout branch
- reset / restore
- broad clean
- 写 runtime verified / release-ready，除非有 runtime evidence

你不能停止，直到满足以下之一：

A. Ready-to-owner-review packet 完成：
- 当前 branch / HEAD / relevant commits 已审计
- 当前 dirty files 全部 reconciled
- untracked files 有处理决策
- 所有 terminal validation commands exit 0
- warning-ledger 没有 TBD
- warnings 全部按 file/code/owner 分类
- Sts1Events formal/staging/remove 有推荐方案
- Debug accept-scaffold/feature-complete/rollback 有推荐方案
- RitsuLib attempted/runtime-validated/release-ready/rollback 有真实状态
- patch inventory raw/migrated 关系已解释
- test changes 已审计
- localization placeholders 已翻译或进入明确 backlog
- commit slices 完整
- no unauthorized commit

B. Hard blocker：
- exact command / file
- 为什么当前 worktree 无法解决
- rollback / staging / owner decision 选项
- 需要 owner 决定什么

必须先读：
AGENTS.md
PROJECT_STATE.md
docs/README.md
docs/test-ready-development-goal.md
docs/worktree-cleanup-audit.md
docs/patch-inventory.md
docs/goals/debug.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/overnight-diff-ledger.md
docs/goals/warning-ledger.md
docs/goals/revision-f-final-report.md
docs/goals/revision-f-commit-slices.md
docs/integrations/ritsulib.md
docs/migration.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

必须先使用 subagents，只调查后修改：
1. ParallelCommitForensicsAgent
2. ValidationReplayAgent
3. DirtyStateReconciliationAgent
4. WarningRecountAgent
5. PatchInventoryAgent
6. TestChangeReviewAgent
7. Sts1EventsGovernanceAgent
8. DebugDecisionAgent
9. RitsuLibRuntimeAgent
10. LocalizationAgent
11. DocsTruthAgent
12. CommitSliceAgent

ValidationReplayAgent 必须运行：
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified

必须创建或更新：
docs/goals/revision-i-final-report.md
docs/goals/revision-i-owner-review-packet.md
docs/goals/revision-i-parallel-commit-audit.md
docs/goals/revision-i-commit-slices.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/overnight-diff-ledger.md
docs/goals/warning-ledger.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

最终报告只能写两种之一：
Complete: ready-to-owner-review packet complete.
Not complete: hard blocker encountered.

不要因为 build pass 就写 complete。
不要因为 tests pass 就写 complete。
不要因为 commit slices written 就写 complete。
不要因为 RitsuLib compiles 就写 runtime-ready。
```

---

## 一句话总评

他已经把项目从“明显失败 / 状态混乱”推进到“验证有进展 / 治理未收口”的阶段，这是实际进步。但当前仍未完成。下一步不是继续写功能，而是跑 **M4 Revision I overnight current-state reconciliation**：用 subagents 把当前 HEAD、dirty state、warnings、RitsuLib runtime、Sts1Events staging、Debug scaffold、patch inventory、test changes、localization backlog 和 docs truth 全部收口，然后再由你决定 commit、rollback，还是进入 Week 2 governance。
# DevSpire M4 Revision I — Current-State Reconciliation, Owner Review, and Runtime-Truth Overnight Run

## Decision

Current status is **not complete**. The project has made stabilization progress, but the latest evidence still contains unresolved state-management, governance, runtime, and documentation-truth blockers.

The next month should focus on converting the current mixed state into an owner-reviewable, commit-ready packet before any further feature migration or longhaul audit resumes.

## Non-negotiable constraints

- Do not commit, push, stash, drop stash, checkout branches, reset, restore, or broad-clean unless the owner explicitly authorizes it.
- Do not continue PR6 Batch 4, PR6 Batch 5, PR7, high-risk patch migration, debug expansion, Sts1Events formalization, or longhaul one-file audit until the current-state reconciliation gate passes.
- Do not claim `RitsuLib` runtime validation unless a runtime install plus loader-smoke evidence exists.
- Do not claim `Sts1Events` is untracked or unrelated if it appears in compile/export/localization/source manifest/docs/tests.
- Do not claim `dotnet format` is clean unless the command exits with code 0.
- Do not claim all work is complete merely because build/test pass.

## Current-state findings to reconcile

The assistant has reported several different states over time. The next overnight run must treat this as a forensic reconciliation problem, not as a normal feature-development pass.

Known states mentioned in recent reports:

- Earlier failure state: `302 passed / 21 skipped / 1 failed`, with `WorktreeBatchScriptRunsAndWritesBatchPathspecs` failing and `dotnet format` timing out.
- Later Revision F state: clean build with `87 CS warnings`, `361 passed / 0 failed / 21 skipped`, format clean, diff clean, and `9 dirty / 0 unclassified`.
- Later user-provided status: `Build passes: 0 errors, 92 warnings`, parallel agent committed 8 files as `f4247553`, current dirty state changed to 7 files, warning split still TBD, and owner decisions still pending.

These states cannot all be treated as equivalent. The overnight run must establish the current branch, current HEAD, current dirty files, current validation, and current warning count from fresh commands.

## M4 Revision I goals

1. Reconcile current branch, HEAD, parallel commits, dirty files, and untracked files.
2. Re-run terminal validation on the current worktree.
3. Recount warnings from a clean build and remove all `TBD` rows from warning ledger.
4. Complete subagent review for test changes, patch inventory, localization placeholders, RitsuLib runtime status, Sts1Events governance, and debug scaffold decision.
5. Produce an owner-review packet and commit-slice plan.
6. Decide whether to continue optimizing, to advance governance, or both.

The expected strategy is: **optimize first, then limited governance advancement**.

## Week 1 — Overnight current-state reconciliation gate

### Required subagents

Run subagents first. They investigate and report before the main agent edits files.

1. `ParallelCommitForensicsAgent`
   - Identify current branch and HEAD.
   - Inspect whether commits such as `faf5860d`, `f4247553`, or later commits are in history.
   - For each relevant commit, list files changed and whether owner authorization is known.
   - Recommend accept / revert / follow-up; do not execute.

2. `ValidationReplayAgent`
   - Run fresh validation commands on the current worktree.
   - Record exact exit code, command, and summarized output.

3. `DirtyStateReconciliationAgent`
   - Reconcile current dirty and untracked files.
   - Update diff ledger so every file has owner, purpose, risk, batch, and rollback plan.

4. `WarningRecountAgent`
   - Run clean build and count warnings by file, code, and owner.
   - Remove all warning-ledger `TBD` entries.

5. `PatchInventoryAgent`
   - Explain the relationship between raw Harmony patches and RitsuLib `ModPatcher` migrated classes.
   - Detect double-patching or stale inventory claims.

6. `TestChangeReviewAgent`
   - Review all test changes since the RitsuLib/Sts1Events/debug work began.
   - For each changed assertion, document old intent, new intent, and whether coverage is stronger, equivalent, weaker, or unknown.

7. `Sts1EventsGovernanceAgent`
   - Recommend exactly one: `formal`, `staging-only`, or `remove/exclude`.
   - Consider warnings, blocked combat events, ZHS placeholders, export/localization surface, runtime gameplay proof, and release claims.

8. `DebugDecisionAgent`
   - Recommend exactly one: `accept-scaffold`, `feature-complete`, or `rollback`.
   - Review default-off behavior, settings exposure, `Warn()` policy, `LogPreview()` dead code, and side-effect risk.

9. `RitsuLibRuntimeAgent`
   - Recommend exactly one: `compile/manifest attempted`, `runtime-validated`, `release-ready`, or `rollback`.
   - Check whether runtime DLL installation, loader smoke, package/handoff docs, and fallback/manifest enforcement exist.

10. `LocalizationAgent`
    - Reconcile ZHS placeholder/missing-key counts.
    - Either translate or put each unresolved key into a named backlog.

11. `DocsTruthAgent`
    - Remove unsupported `Done`, `complete`, `all verified`, `format clean`, `runtime verified`, `release-ready`, `untracked/unrelated`, and `commit next` claims.

12. `CommitSliceAgent`
    - Prepare commit slices only.
    - Do not commit.

### Required validation commands

Run exactly these, then record exit status:

```powershell
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

### Required artifacts

Create or update:

```text
docs/goals/revision-i-final-report.md
docs/goals/revision-i-owner-review-packet.md
docs/goals/revision-i-parallel-commit-audit.md
docs/goals/revision-i-commit-slices.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/overnight-diff-ledger.md
docs/goals/warning-ledger.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md
```

## Week 2 — Governance decisions

### Sts1Events

Recommended default: `staging-only` unless the owner explicitly requests formalization.

Formalization requires:

- zero build-blocking API drift,
- warning treatment decision,
- translated ZHS strings or explicit backlog,
- runtime gameplay proof,
- event image/resource policy,
- export/package claim decision,
- manual test plan.

### Debug

Recommended default: `accept-scaffold`.

Feature-complete debug requires:

- settings exposure or explicit internal-only policy,
- behavioral tests,
- side-effect audit for init order, RNG, save/load, and multiplayer,
- `Warn()` logging policy,
- `LogPreview()` use/remove decision.

## Week 3 — RitsuLib runtime truth

Current default status should remain:

```text
compile/manifest dependency attempted; runtime unverified
```

Runtime validation requires:

```powershell
dotnet publish .\EZMicroBalance.csproj
.\scripts\package-spire-plus.ps1
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

And runtime evidence:

- BaseLib + STS2-RitsuLib + Spire Plus installed together,
- loader smoke reaches menu,
- `godot.log` audit clean,
- tester handoff dependency instructions,
- package/version/hash docs updated,
- fallback policy or install-enforced hard dependency decision.

## Week 4 — Resume one-file longhaul audit only if gates pass

Do not resume longhaul audit until:

- Revision I owner-review packet is complete,
- validation is green,
- warning ledger has no TBD,
- Sts1Events/Debug/RitsuLib decisions are recorded,
- commit or rollback decision is made by owner,
- docs truth has no unsupported claims.

First file queue:

```text
EZMicroBalance.csproj
EZMicroBalance.json
EZMicroBalanceCode/MainFile.cs
EZMicroBalanceCode/Core/Integrations/RitsuLib/RitsuLibBootstrap.cs
EZMicroBalanceCode/Diagnostics/SpirePlusDebug.cs
EZMicroBalanceCode/Config/SpirePlusModConfig.cs
tests/EZMicroBalance.Tests/RitsuLibMigrationGuardTests.cs
tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.cs
docs/goals/debug.md
docs/integrations/ritsulib.md
```

## Overnight run prompt

```text
进入 M4 Revision I overnight current-state reconciliation run。

当前状态 NOT COMPLETE。不要继续 PR6 Batch4、PR6 Batch5、PR7、debug expansion、Sts1Events formalization、RitsuLib high-risk patch migration 或 longhaul audit。

禁止：
- commit
- push
- stash / stash drop
- checkout branch
- reset / restore
- broad clean
- 写 runtime verified / release-ready，除非有 runtime evidence

你不能停止，直到满足以下之一：

A. Ready-to-owner-review packet 完成：
- 当前 branch / HEAD / relevant commits 已审计
- 当前 dirty files 全部 reconciled
- untracked files 有处理决策
- 所有 terminal validation commands exit 0
- warning-ledger 没有 TBD
- warnings 全部按 file/code/owner 分类
- Sts1Events formal/staging/remove 有推荐方案
- Debug accept-scaffold/feature-complete/rollback 有推荐方案
- RitsuLib attempted/runtime-validated/release-ready/rollback 有真实状态
- patch inventory raw/migrated 关系已解释
- test changes 已审计
- localization placeholders 已翻译或进入明确 backlog
- commit slices 完整
- no unauthorized commit

B. Hard blocker：
- exact command / file
- 为什么当前 worktree 无法解决
- rollback / staging / owner decision 选项
- 需要 owner 决定什么

必须先读：
AGENTS.md
PROJECT_STATE.md
docs/README.md
docs/test-ready-development-goal.md
docs/worktree-cleanup-audit.md
docs/patch-inventory.md
docs/goals/debug.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/overnight-diff-ledger.md
docs/goals/warning-ledger.md
docs/goals/revision-f-final-report.md
docs/goals/revision-f-commit-slices.md
docs/integrations/ritsulib.md
docs/migration.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

必须先使用 subagents，只调查后修改：
1. ParallelCommitForensicsAgent
2. ValidationReplayAgent
3. DirtyStateReconciliationAgent
4. WarningRecountAgent
5. PatchInventoryAgent
6. TestChangeReviewAgent
7. Sts1EventsGovernanceAgent
8. DebugDecisionAgent
9. RitsuLibRuntimeAgent
10. LocalizationAgent
11. DocsTruthAgent
12. CommitSliceAgent

ValidationReplayAgent 必须运行：
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified

必须创建或更新：
docs/goals/revision-i-final-report.md
docs/goals/revision-i-owner-review-packet.md
docs/goals/revision-i-parallel-commit-audit.md
docs/goals/revision-i-commit-slices.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/overnight-diff-ledger.md
docs/goals/warning-ledger.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

最终报告只能写两种之一：
Complete: ready-to-owner-review packet complete.
Not complete: hard blocker encountered.

不要因为 build pass 就写 complete。
不要因为 tests pass 就写 complete。
不要因为 commit slices written 就写 complete。
不要因为 RitsuLib compiles 就写 runtime-ready。
```
