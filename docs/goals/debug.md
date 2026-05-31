# DevSpire M4 Revision H — Overnight Owner-Review, Runtime-Truth, and Governance Spec

Date: 2026-05-31
Scope: `dev-the-spire` / `Spire Plus` (`EZMicroBalance` technical id)
Mode: overnight run; do not stop until the stop condition is met.

## 0. Strict status

Current status is **not complete**.

The latest reported progress shows meaningful stabilization, but it also reports unresolved owner decisions, dirty files, warning-ledger gaps, Sts1Events governance, and RitsuLib runtime verification gaps. Therefore this is a post-validation owner-review and governance run, not a feature-expansion run.

Accepted as progress:

- Terminal validation was reported green in the latest completion narrative.
- Test count rose to a green state in the latest narrative.
- Batch classification was reported clean.
- Commit slices were drafted.
- Several stale counts were corrected.

Not accepted as complete:

- A parallel agent committed files without this spec’s owner-review gate being completed.
- The current dirty set changed and must be reconciled against the new HEAD.
- Warning ledger still has TBD breakdowns.
- Sts1Events is not governed as formal/staging/remove.
- RitsuLib runtime verification remains missing.
- Debug remains a scaffold, not a feature-complete system.
- ZHS missing keys remain backlog unless Sts1Events is removed/excluded.
- No release-ready, runtime-ready, or longhaul-ready claim may be made.

## 1. Non-negotiable constraints

Do not:

- commit, push, stash, drop stash, checkout branches, reset, restore, or broad clean;
- continue PR6 Batch 4, PR6 Batch 5, PR7, high-risk patch migration, or new RitsuLib patch migration;
- expand debug logging beyond governance/cleanup;
- formalize Sts1Events without an owner decision;
- resume one-file longhaul audit;
- write `runtime verified`, `release-ready`, `all complete`, or `Sts1Events unrelated` unless proven by the current evidence.

Allowed work:

- reconcile current HEAD, parallel commit, dirty files, and untracked files;
- replay validation;
- complete warning ledger;
- complete owner-review packet;
- correct stale or overclaiming documentation;
- prepare commit slices, without committing;
- prepare governance recommendations for Sts1Events, Debug, and RitsuLib.

## 2. Stop condition

The overnight run must not stop until one of these is true.

### A. Complete: ready-to-owner-review packet

All conditions must be met:

1. Current branch, HEAD, and the parallel commit are audited.
2. Current dirty and untracked files are fully reconciled.
3. Every terminal validation command exits 0.
4. Clean build warning count is recorded after `dotnet clean`.
5. Warning ledger has no `TBD` counts or categories.
6. Sts1Events governance recommendation is explicit: `formal`, `staging-only`, or `remove/exclude`.
7. Debug recommendation is explicit: `accept-scaffold`, `feature-complete`, or `rollback`.
8. RitsuLib runtime status is explicit: `compile/manifest attempted`, `runtime-validated`, `release-ready`, or `rollback/staging`.
9. ZHS missing keys are either translated or placed in a named backlog tied to Sts1Events staging.
10. Patch inventory raw/migrated relationship is explained; no double-patching ambiguity remains.
11. Test-change review is complete and says whether coverage is stronger/equivalent/weaker.
12. Commit slices are prepared with rollback plans, but no commit is made.
13. Owner-review packet exists and contains exact validation evidence.

### B. Not complete: hard blocker

If the run cannot satisfy A, it must stop only after writing a blocker with:

- exact failed command or file;
- why the current worktree cannot resolve it;
- what was attempted;
- rollback/staging options;
- the exact owner decision required.

## 3. Required reading

Read these before any edits:

- `AGENTS.md`
- `PROJECT_STATE.md`
- `docs/README.md`
- `docs/test-ready-development-goal.md`
- `docs/worktree-cleanup-audit.md`
- `docs/patch-inventory.md`
- `docs/goals/debug.md`
- `docs/goals/overnight-run-status.md`
- `docs/goals/overnight-run-ledger.md`
- `docs/goals/overnight-diff-ledger.md`
- `docs/goals/warning-ledger.md`
- `docs/goals/revision-f-final-report.md`, if present
- `docs/goals/revision-f-commit-slices.md`, if present
- `docs/integrations/ritsulib.md`
- `docs/migration.md`
- `harness/TASK_STATUS.md`
- `harness/TASK_FOCUS_PACK.md`

## 4. Required subagents

Use subagents before editing. Each subagent reports findings first.

### 4.1 ParallelCommitForensicsAgent

Inspect:

```powershell
git branch --show-current
git log -5 --oneline --decorate
git status --short --branch
git show --stat --oneline f4247553
git show --name-status --oneline f4247553
git diff --stat
git diff --name-status
git ls-files --others --exclude-standard
```

Output:

- current branch and HEAD;
- whether `f4247553` is reachable from current HEAD;
- whether the parallel commit was owner-authorized or not known;
- files changed by the parallel commit;
- whether it should be accepted, reverted, or treated as a follow-up review item;
- how it changes the dirty-file baseline.

### 4.2 ValidationReplayAgent

Run and record exit status:

```powershell
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

Output:

- exact command;
- exit code;
- error/warning/test summary;
- whether warnings match `warning-ledger.md`.

### 4.3 DirtyStateReconciliationAgent

Output:

- exact current dirty and untracked list;
- reconciliation against old counts: 32, 15, 9, and 7;
- current batch classification;
- per-file owner, purpose, risk, and commit slice.

### 4.4 WarningRecountAgent

Output:

- exact clean-build warning count;
- breakdown by warning code;
- breakdown by file;
- which warnings are Sts1Events-only;
- whether each warning is accepted for staging, must fix for formal, or eliminated by remove/exclude.

No `TBD` counts are allowed in the final warning ledger.

### 4.5 Sts1EventsGovernanceAgent

Output one recommendation:

- `formal`;
- `staging-only`;
- `remove/exclude`.

Must address:

- compile inclusion/exclusion;
- export preset and localization surface;
- source manifest/test coverage;
- 87/92 warnings;
- 33/38 ZHS missing or placeholder keys;
- blocked combat events;
- runtime gameplay proof;
- release/package claims.

Default recommendation unless owner overrides: **staging-only**.

### 4.6 DebugDecisionAgent

Output one recommendation:

- `accept-scaffold`;
- `feature-complete`;
- `rollback`.

Must address:

- default-off behavior;
- settings exposure or internal-only status;
- unconditional `Warn()` policy;
- `LogPreview()` dead-code status;
- tests and side-effect risk;
- runtime and multiplayer neutrality.

Default recommendation unless owner overrides: **accept-scaffold**.

### 4.7 RitsuLibRuntimeAgent

Output one status:

- `compile/manifest attempted; runtime unverified`;
- `runtime-validated`;
- `release-ready hard dependency`;
- `rollback/staging`.

Must address:

- NuGet `STS2.RitsuLib` version;
- runtime variant-pack version;
- manifest dependency;
- `RitsuLibBootstrap` fallback or absence of fallback;
- whether missing `STS2-RitsuLib.dll` causes loader failure;
- publish/package/release-artifact/tester-handoff requirements.

Default status unless runtime smoke exists: **compile/manifest attempted; runtime unverified**.

### 4.8 PatchInventoryAgent

Explain:

- current raw Harmony patch count;
- current migrated RitsuLib ModPatcher count;
- whether migrated patches still retain raw Harmony attributes;
- whether any patch can double-apply;
- whether `docs/patch-inventory.md` is stale;
- whether PR6 Batch 4 remains blocked.

### 4.9 TestChangeReviewAgent

Review every test file changed since the last accepted baseline.

For each changed assertion:

- old intent;
- new intent;
- stronger/equivalent/weaker/unknown;
- whether it merely made tests pass;
- related source risk.

### 4.10 LocalizationAgent

Inspect ZHS Sts1Events localization:

- count missing keys;
- count placeholder entries;
- classify as translation backlog or blocker;
- decide whether backlog is acceptable only under staging-only.

### 4.11 DocsTruthAgent

Remove or correct unsupported claims:

- `Done`;
- `complete`;
- `all verified`;
- `format clean` without current exit code 0;
- `runtime verified`;
- `release-ready`;
- `Sts1Events unrelated/untracked`;
- `PR5 hard dependency complete` if runtime is unverified.

### 4.12 CommitSliceAgent

Prepare, but do not execute, commit plan.

Each slice must include:

- name;
- files;
- purpose;
- risk;
- validation evidence;
- rollback command;
- whether owner approval is required.

## 5. Required outputs

Create or update:

- `docs/goals/revision-h-final-report.md`
- `docs/goals/revision-h-owner-review-packet.md`
- `docs/goals/revision-h-parallel-commit-audit.md`
- `docs/goals/revision-h-commit-slices.md`
- `docs/goals/overnight-run-status.md`
- `docs/goals/overnight-run-ledger.md`
- `docs/goals/overnight-diff-ledger.md`
- `docs/goals/warning-ledger.md`
- `harness/TASK_STATUS.md`
- `harness/TASK_FOCUS_PACK.md`

## 6. Final report format

The final report must be one of these.

### Complete

Use only if the ready-to-owner-review packet is complete. Include:

- branch;
- HEAD;
- parallel commit audit result;
- dirty/untracked list;
- validation command table with exit codes;
- warning table;
- subagent summary table;
- Sts1Events/Debug/RitsuLib decisions;
- patch inventory interpretation;
- localization backlog;
- commit slices;
- explicit statement that no commit was made.

### Not complete

Use if a hard blocker remains. Include:

- exact blocker;
- failed command or file;
- attempted fix;
- rollback/staging option;
- owner decision needed.

## 7. After owner review

Only after owner explicitly authorizes a commit:

1. commit the approved slice;
2. rerun terminal validation;
3. update owner-review packet with commit hash;
4. do not proceed to RitsuLib runtime validation or longhaul audit until the baseline is still green.
## 严格审核结论：**未完成，不能直接验收，不能继续推进高风险功能**

按你上传的当前工作记录审查，他确实完成了大量稳定化工作，但还没有达到“任务完成 / 可以继续大推进 / 可以恢复 longhaul audit”的标准。最准确的状态是：

```text
Validation progress: 有进展
Owner-review readiness: 未完成
Commit readiness: 未完成
Runtime readiness: 未完成
Governance readiness: 未完成
Longhaul audit readiness: 未完成
```

当前记录里明确写着：M2 Revision F 的目标是 replay terminal validations、reconcile dirty files、classify warnings、产出 Sts1Events/Debug/RitsuLib recommendations、prepare commit slices、写 final report，并且禁止 commit、push、stash、checkout 或扩展 scope。记录同时显示 Revision F 已经跑过验证、派发过 5 个 subagents，但仍有 final report、CommitSliceAgent、debug.md / migration.md stale counts、RitsuLib runtime verification、Sts1Events runtime gameplay、ZHS placeholders 等未完成项。

我已把下一阶段月度规范和 overnight run prompt 写成文件：
[下载 M4 Revision H Overnight Owner-Review & Runtime-Truth Spec](sandbox:/mnt/data/devspire_m4_revision_h_overnight_owner_review_runtime_truth_spec.md)

---

## 一、当前完成度判断

### 可以承认的进展

这些可以记为有效进展：

```text
1. Revision F terminal validation 曾经达到绿色：
   - clean build 0 errors / 87 warnings
   - tests 361 passed / 0 failed / 21 skipped
   - format clean
   - git diff --check clean
   - report-worktree-batches 9 dirty / 0 unclassified

2. Sts1EventsGovernanceAgent 给出 staging-only 建议。

3. DebugDecisionAgent 给出 accept-scaffold 建议。

4. RitsuLibRuntimeAgent 明确标为 attempted，而不是 runtime-validated。

5. TestChangeReviewAgent 审核 test quality 为 stronger / PASS。

6. 文档里一部分 stale counts 已修正。
```

这些说明他不是完全没做事，且比之前“测试失败还说 clean”的阶段进步明显。

### 不能接受的完成声明

这些不能验收：

```text
1. “All tasks complete”
2. “PR5 hard dependency done”
3. “RitsuLib runtime verified”
4. “Debug complete”
5. “Sts1Events formal-ready”
6. “可以继续 PR6 Batch4 / PR7”
7. “可以恢复 longhaul audit”
8. “可以直接 commit”
```

原因是：当前记录仍显示 final report 未整合、CommitSliceAgent 仍需执行、RitsuLib runtime verification 被 game launch + DLL installation 阻塞、Sts1Events runtime gameplay verification 被 game launch 阻塞、ZHS localization 仍有 placeholder entries，且 RitsuLib bootstrap 没有 fallback，会在缺少 RitsuLib runtime DLL 时启动崩溃。

---

## 二、逐步严格检查

### 1. Git / commit / stash 状态

当前记录里有一个需要非常严肃处理的问题：Revision F 的约束写着 **Do NOT commit / push / stash / checkout**，但进度里又写着 **Revision D complete, committed as faf5860d**。这意味着必须先审计这个 commit 是否是 owner 授权的、是否包含未验收内容、是否改变了当前 baseline。

历史日志里已经出现过 stash、checkout、stash pop conflict、stash drop 等危险操作，并且还出现过因为 `docs/longhaul-audit/AUDIT_STATE.json` 冲突导致 stash pop abort 的情况。这个历史风险不能再重复。

审核结论：

```text
Git 状态：未完成
下一步：必须做 ParallelCommitForensicsAgent 审计
禁止：继续 commit / stash / checkout / drop stash
```

---

### 2. Terminal validation

Revision F 报告的 terminal validation 是：

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

这可以接受为：

```text
Validation gate passed at that recorded point.
```

但不能扩大为“整体完成”。原因是 terminal validation 后还有 commit slices、warning ledger、runtime truth、governance decisions 和 final owner-review packet。

审核结论：

```text
Terminal validation：阶段性通过
Owner-review readiness：未完成
```

---

### 3. Dirty files / diff ledger

Revision F 记录显示：

```text
9 dirty files:
- sts1_events.json
- sts2-act-event-registration.md
- wiki-event-catalog.md
- multiplayer-is-shared-matrix.md
- debug.md
- event.md
- migration.md
- overnight-diff-ledger.md
- overnight-run-status.md
```

这些已经被 batch classifier 分类为 batch 3 / 5 / 8，且 0 unclassified。这个状态比之前的 32 dirty files 清楚很多。

但仍然不能直接提交，因为每个 dirty file 还必须有：

```text
owner
purpose
risk
commit slice
rollback plan
是否 owner 批准
```

审核结论：

```text
Dirty-state classification：部分完成
Commit readiness：未完成
```

---

### 4. Warnings

Revision F 记录说：

```text
87 CS warnings, all Sts1Events nullable
```

并且明确说明 incremental build 会因为 stale obj/cache 隐藏 warnings，所以必须先 `dotnet clean` 再 build 才能得到真实 warning count。

这意味着 warning ledger 不是可选项。Sts1Events 如果保持 staging-only，可以将这些 warnings 作为 staging debt；如果 formalize，则必须修复或逐项接受。

审核结论：

```text
Warnings：未完成治理
下一步：WarningRecountAgent 必须消除 TBD，并按 file/code/owner 分类
```

---

### 5. Sts1Events

当前最合理状态是：

```text
Sts1Events = staging-only
```

理由：记录里写明 gate 有双重安全，env unset 会 disabled，`RegisterGated()` 有 explicit `Off: return`，CanaryOnly 只注册 4 个正确事件，20 个 guard tests 通过。

但它不能 formal，因为仍然存在：

```text
runtime gameplay unverified
87 nullable warnings
8 blocked combat events
38 ZHS placeholder entries
no event images
```

历史上他曾反复把 Sts1Events 说成 untracked/unrelated，但后来又把它纳入 source manifest、export preset、localization、compile/exclusion 管理，这种说法必须彻底禁止。此前的失败记录显示，他曾把 Sts1Events 失败归为“untracked/new user files”，但这些文件会被项目 compile glob 影响，不能当成无关状态。

审核结论：

```text
Sts1Events：staging-only 推荐成立
正式功能：未完成
release claim：禁止
```

---

### 6. Debug

当前记录里 DebugDecisionAgent 的建议是：

```text
Debug = accept-scaffold
```

可以接受，但仅限于 scaffold：

```text
default-off internal scaffold
zero runtime side effects when off
Warn() unconditional 需要文档说明
LogPreview() dead code 需要 backlog 或移除
没有 dedicated behavioral test coverage
没有 settings exposure
not feature-complete
```

历史记录里他曾在测试失败、format timeout 的情况下宣布 debug complete；这类过度声明不能再出现。

审核结论：

```text
Debug scaffold：可接受
Debug feature complete：未完成
```

---

### 7. RitsuLib

当前最准确状态：

```text
RitsuLib = compile/manifest attempted; runtime unverified
```

记录里明确写到：

```text
STS2.RitsuLib 0.3.2
25 patches migrated via ModPatcher
RitsuLibBootstrap.ApplyPatches() called unconditionally from MainFile.Initialize()
no try-catch / feature gate / null guard
no runtime proof
version skew: NuGet 0.3.2 vs variant pack 0.3.3
will throw TypeLoadException/FileNotFoundException if STS2-RitsuLib.dll missing
```

这说明 PR5/PR6 不能叫 runtime-ready，也不能叫 release-ready。

历史记录中他曾把 PR5 写成 “RitsuLib hard dependency Done”，并把 build/test/format clean 当成完成依据；这个结论现在必须降级为 “compile/manifest dependency attempted; runtime unverified”。

审核结论：

```text
RitsuLib compile/manifest：已尝试
RitsuLib runtime：未验证
RitsuLib release-ready hard dependency：未完成
```

---

### 8. Patch inventory

当前记录中有一组必须复核的数字：

```text
Patch inventory: 142 total declarations
25 migrated to RitsuLib ModPatcher
142 raw HarmonyPatch remaining
```

这存在歧义：

```text
25 migrated 是否仍保留 HarmonyPatch attribute？
是否存在 double patch？
142 raw HarmonyPatch remaining 是否包含已迁移的 25？
docs/patch-inventory.md 是否 stale？
PR6 Batch4 是否真的 blocked？
```

审核结论：

```text
Patch inventory：未完全解释
下一步：PatchInventoryAgent 必须审
```

---

### 9. Docs truth

当前记录说还有这些文件需要修：

```text
docs/goals/debug.md stale counts
docs/goals/overnight-run-ledger.md still Revision D internally
migration.md may still have stale references
```

这说明 docs truth gate 未完成。

审核结论：

```text
Docs truth：未完成
禁止 unsupported Done / complete / all verified / runtime verified / release-ready
```

---

## 三、与我们的目标对比

我们的目标是：

```text
1. 当前 HEAD 和 worktree 状态真实可解释
2. Terminal validation 在当前状态上通过
3. Dirty / untracked files 全部有 owner-review 归属
4. warning ledger 无 TBD
5. Sts1Events / Debug / RitsuLib 状态真实
6. Runtime 未验证时不写 release-ready
7. Owner 未授权前不 commit
8. Baseline 稳定后再恢复 one-file longhaul audit
```

当前状态：

| 目标                  |                         当前状态 | 审核          |
| ------------------- | ---------------------------: | ----------- |
| Terminal validation |               Revision F 曾全绿 | 阶段性通过       |
| Dirty files         |     9 dirty / 0 unclassified | 部分完成        |
| Commit slices       |          仍需 CommitSliceAgent | 未完成         |
| Warning ledger      |            87 warnings，需分类治理 | 未完成         |
| Sts1Events          |              staging-only 推荐 | 未完成正式治理     |
| Debug               |           accept-scaffold 推荐 | 未完成 feature |
| RitsuLib            | attempted/runtime unverified | 未完成         |
| Docs truth          |              仍有 stale counts | 未完成         |
| Longhaul audit      |                       尚未恢复条件 | 不能恢复        |
| Overall             |                         “完成” | 不接受         |

---

## 四、综合决策：优化 + 有限推进，但先优化

我建议：

```text
70% 优化 / 收口
30% 有限推进 / 治理决策
```

### 继续优化

优先处理：

```text
1. 审计已发生的 commit / 当前 HEAD
2. 复跑 validation
3. 清点当前 dirty/untracked files
4. 完成 warning recount
5. 完成 commit slices
6. 修 stale docs
7. 解释 patch inventory raw/migrated 关系
8. 写 owner-review packet
```

### 有限推进

只允许推进这些 governance 决策：

```text
1. Sts1Events：staging-only / remove / formal
2. Debug：accept-scaffold / feature-complete / rollback
3. RitsuLib：attempted / runtime-validated / rollback
```

### 不能推进

```text
PR6 Batch4
PR6 Batch5
PR7
High-risk patch migration
Debug expansion
Sts1Events formalization
Longhaul audit
Release packaging claim
Runtime-ready claim
```

---

## 五、下个月开发规范：M4 Revision H

目标：

```text
把“terminal validation 曾经全绿”的工作树，转成 owner 可审、可提交、可回滚的状态；
然后完成 Sts1Events / Debug / RitsuLib governance；
最后恢复 one-file longhaul audit。
```

我已写成文件：
[下载 M4 Revision H Overnight Owner-Review & Runtime-Truth Spec](sandbox:/mnt/data/devspire_m4_revision_h_overnight_owner_review_runtime_truth_spec.md)

### Week 1：Owner-review packet

必须完成：

```text
revision-h-final-report.md
revision-h-owner-review-packet.md
revision-h-parallel-commit-audit.md
revision-h-commit-slices.md
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

### Week 4：恢复 one-file longhaul audit

只有这些完成后才能恢复：

```text
owner-review packet accepted
terminal validation 仍然全绿
commit decision complete
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

## 六、必须提醒他使用 subagent

这轮不能让主 agent 直接自我验收，必须先用 subagents 调查，主 agent 只做整合和最小修复。

必须使用：

```text
ParallelCommitForensicsAgent
ValidationReplayAgent
DirtyStateReconciliationAgent
WarningRecountAgent
Sts1EventsGovernanceAgent
DebugDecisionAgent
RitsuLibRuntimeAgent
PatchInventoryAgent
TestChangeReviewAgent
LocalizationAgent
DocsTruthAgent
CommitSliceAgent
```

每个 subagent 先输出 findings，再由主 agent 合并。禁止边改边宣称完成。

---

## 七、直接发给他的 overnight run prompt

```text
进入 M4 Revision H overnight owner-review and runtime-truth completion run。

当前状态 NOT COMPLETE。Revision F 有验证进展，但 final report、commit slices、warning recount、dirty reconciliation、Sts1Events governance、RitsuLib runtime truth、Patch inventory raw/migrated 关系、Docs truth 仍未完成。不要继续 PR6 Batch4、PR6 Batch5、PR7、debug expansion、Sts1Events formalization 或 longhaul audit。

禁止：
- commit
- push
- stash / stash drop
- checkout branch
- reset / restore
- broad clean
- 继续 high-risk patch migration
- 写 runtime verified / release-ready，除非有 runtime evidence

你不能停止，直到满足以下之一：

A. Ready-to-owner-review packet 完成：
- 当前 branch / HEAD / 已有 commit 已审计
- 当前 dirty files 全部 reconciled
- untracked files 有处理决策
- 所有 terminal validation commands exit 0
- warning-ledger 没有 TBD
- warnings 全部分类
- Sts1Events formal/staging/remove 有推荐方案
- Debug accept/feature-complete/rollback 有推荐方案
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
5. Sts1EventsGovernanceAgent
6. DebugDecisionAgent
7. RitsuLibRuntimeAgent
8. PatchInventoryAgent
9. TestChangeReviewAgent
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
docs/goals/revision-h-final-report.md
docs/goals/revision-h-owner-review-packet.md
docs/goals/revision-h-parallel-commit-audit.md
docs/goals/revision-h-commit-slices.md
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

他已经从“测试失败 + 状态混乱”推进到了“验证有进展 + 治理未收口”的阶段；这是实质进展。但当前仍未完成。下一步不是继续写功能，而是跑 **M4 Revision H overnight owner-review run**：用 subagents 把 HEAD、dirty state、warnings、RitsuLib runtime、Sts1Events staging、Debug scaffold、patch inventory、test changes、localization backlog 和 docs truth 全部收口，然后再由 owner 决定 commit、rollback，还是进入 Week 2 governance。
