下面是从“新窗口起点”重建的完整上下文、严格审核、下一步开发规范和 overnight run 指令。我也整理成了一份可下载文档：

# DevSpire M3 Revision H — Full Context, Strict Audit, and Overnight Owner-Review Run Spec

Date: 2026-05-31
Scope: dev-the-spire; player-facing Spire Plus; technical manifest id `EZMicroBalance`.

## 0. One-line verdict

M3 Revision G/H is not complete. The latest report shows progress, but the work cannot be accepted until the parallel commit is audited, the current dirty state is reconciled, all terminal validations are replayed on the current HEAD, warning and localization ledgers are finished, and Sts1Events / Debug / RitsuLib governance decisions are recorded.

## 1. Project baseline context

- Player-facing mod name: Spire Plus.
- Stable technical manifest id / project / install folder / DLL / PCK surface: EZMicroBalance.
- Do not rename EZMicroBalance in place.
- Active code lives primarily under EZMicroBalanceCode/.
- Active resources and localization live under EZMicroBalance/.
- Tests live under tests/EZMicroBalance.Tests/.
- Current package / runtime evidence is not equivalent to live gameplay evidence.
- Commands can prove source/package/static correctness, but not live save-load, co-op, failure/death, or runtime loader behavior.

## 2. Historical work context

### 2.1 RitsuLib integration

- RitsuLib was introduced as a compile / manifest dependency attempt.
- STS2.RitsuLib 0.3.2 is used in the project.
- Uploaded runtime variant pack was 0.3.3, creating a version-skew concern.
- EZMicroBalance.json declares STS2-RitsuLib runtime dependency.
- RitsuLibBootstrap is called from MainFile.Initialize().
- Runtime loader proof is not available.
- If RitsuLib runtime is missing, the current bootstrap may fail before the mod can run unless manifest dependency prevents load cleanly; this must be proven by loader smoke, not assumed.

Current truth wording:

> RitsuLib compile/manifest dependency attempted; runtime unverified; release readiness false.

Do not write:

- RitsuLib hard dependency complete.
- runtime verified.
- release-ready.

### 2.2 RitsuLib patch migration

- Earlier bootstrap kept raw Harmony for patching.
- Later work claims 25 patches migrated to RitsuLib ModPatcher.
- Patch inventory statements are inconsistent: examples include “142 total declarations”, “25 migrated”, and “142 raw HarmonyPatch remaining”.
- This creates a possible double-patching / stale inventory / terminology ambiguity.

Required review:

- PatchInventoryAgent must reconcile raw Harmony attributes, migrated ModPatcher registrations, and actual runtime patch application.
- No PR6 Batch4 / PR7 work until baseline governance is complete.

### 2.3 Debug scaffold

- SpirePlusDebug exists as a default-off internal logging scaffold.
- It is not feature-complete.
- It lacks full behavioral test coverage and runtime side-effect proof.
- SpirePlusDebug.Warn may log unconditionally; this policy must be documented.
- LogPreview has had zero call sites in prior summaries; dead code must be recorded or removed.

Current truth wording:

> Debug scaffold accepted as default-off internal scaffold; not feature-complete.

### 2.4 Sts1Events

- Sts1Events has entered current source / localization / docs / export / test surface.
- It must not be described as untracked or unrelated.
- Current recommendation has been staging-only.
- It has had 87 to 92 nullable warnings depending on recount.
- It has 33–38 ZHS placeholder / missing result-page key items depending on latest count.
- Runtime gameplay is unverified.
- Some combat/event work remains blocked.

Current truth wording:

> Sts1Events is staging-only unless owner explicitly promotes it to formal feature.

## 3. Latest reported state requiring strict audit

Latest assistant report claims:

- Build passes: 0 errors, 92 warnings.
- A parallel agent session committed 8 files as f4247553.
- Current dirty state: 7 files, 6 modified + 1 untracked.
- revision-f-commit-slices.md written: 6 slices, 15 entries.
- debug.md stale counts fixed in 13 locations.
- migration.md stale counts fixed in 12 locations.
- overnight-run-status.md updated to HEAD d290598c, 92 warnings, 387 tests, 15 dirty.
- overnight-run-ledger.md updated to M3 content.
- overnight-diff-ledger.md rewritten as 12+3 dirty files, 3-way comparison table.
- warning-ledger.md updated: 92 total, breakdown still TBD.
- remaining owner decisions: commit slices, warning per-file recount, ZHS 33 missing result-page keys, RitsuLib/Sts1Events runtime verification.

Strict interpretation:

- Build-only pass is not enough.
- The latest report does not prove that test / format / diff / batch classification were replayed after f4247553 and current dirty-state changes.
- Parallel commit f4247553 violates prior no-commit policy unless owner-authorized.
- Dirty-state counts conflict across reports: 7 files, 12+3 dirty, 15 dirty, and earlier 9 dirty.
- Warning ledger has TBD and is incomplete.
- Runtime verification remains pending.

## 4. Current acceptance decision

### Accepted as progress

- Build error count is reportedly 0.
- Dirty state is smaller than earlier 32-file state.
- Commit slices have been drafted.
- Several stale counts were corrected.
- Sts1Events is no longer consistently called untracked in the latest context.

### Not accepted as complete

- M3 Week 1 is not complete.
- Commit-readiness packet is not complete.
- Owner-review packet is not complete.
- Runtime readiness is not complete.
- Sts1Events governance is not complete.
- RitsuLib runtime status is not complete.
- Debug is not feature-complete.
- Longhaul one-file audit must not resume yet.

## 5. Next-month development spec — M3 Revision H

### Goal

Convert the current mixed state — green-ish build, parallel commit, dirty files, warning ledger TBD, and pending governance decisions — into a truthful, owner-review-ready, rollback-safe packet.

### Stop conditions

The overnight run must not stop until one of these is true:

A. Ready-to-owner-review packet complete:

- Parallel commit f4247553 audited.
- Current branch / HEAD / stash / dirty files recorded.
- Current dirty files fully reconciled.
- The untracked file has an owner decision.
- All terminal validation commands replayed on current HEAD and current dirty state.
- Warning ledger has no TBD fields.
- 92 warnings are classified by file, code, owner, and decision dependency.
- Sts1Events has a formal / staging-only / remove-exclude recommendation.
- Debug has an accept-scaffold / feature-complete / rollback recommendation.
- RitsuLib has attempted / runtime-validated / release-ready / rollback status.
- ZHS missing keys are translated or entered into explicit backlog.
- Commit slices are updated and match the current dirty state.
- No unauthorized commit was made.

B. Hard blocker:

- Exact command or file.
- Why current worktree cannot resolve it.
- Rollback / staging / owner-decision options.
- What owner must decide.

## 6. Required subagents

1. ParallelCommitForensicsAgent
   - Audit f4247553.
   - Determine whether it was owner-authorized.
   - List changed files and whether they belong to prior accepted slices.
   - Recommend accept / revert / follow-up.

2. ValidationReplayAgent
   - Run all terminal validation commands on current HEAD and dirty state.
   - Record exact exit codes.

3. DirtyStateReconciliationAgent
   - Reconcile 7 dirty, 12+3 dirty, 15 dirty, earlier 9 dirty.
   - Explain which counts are stale.
   - Update diff ledger.

4. WarningRecountAgent
   - Remove TBD from warning-ledger.md.
   - Classify all 92 warnings.

5. CommitSliceAgent
   - Prepare commit plan only.
   - No commit without owner authorization.

6. Sts1EventsGovernanceAgent
   - Recommend formal / staging-only / remove-exclude.
   - Account for warnings, ZHS keys, runtime proof, export/localization surface.

7. RitsuLibRuntimeAgent
   - Decide attempted / runtime-validated / release-ready / rollback.
   - Identify loader smoke, package, manifest, and fallback requirements.

8. DebugDecisionAgent
   - Decide accept-scaffold / feature-complete / rollback.
   - Verify default-off policy, Warn behavior, settings exposure, and side-effect risk.

9. LocalizationAgent
   - Handle 33 missing ZHS result-page keys.
   - Translate or backlog.

10. DocsTruthAgent
   - Remove unsupported Done / complete / all verified / runtime verified / release-ready / untracked unrelated claims.

11. PatchInventoryAgent
   - Reconcile 141/142 raw Harmony declarations, 25 migrated ModPatcher entries, and double-patching risk.

## 7. Required commands

Run from repo root:

```powershell
git branch --show-current
git log -5 --oneline --decorate
git stash list
git status --short --branch
git diff --name-status
git diff --stat
git show --stat --oneline f4247553

dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

## 8. Required files to create or update

- docs/goals/revision-h-final-report.md
- docs/goals/revision-h-owner-review-packet.md
- docs/goals/revision-h-parallel-commit-audit.md
- docs/goals/revision-h-commit-slices.md
- docs/goals/overnight-run-status.md
- docs/goals/overnight-run-ledger.md
- docs/goals/overnight-diff-ledger.md
- docs/goals/warning-ledger.md
- docs/goals/debug.md
- docs/goals/migration.md
- harness/TASK_STATUS.md
- harness/TASK_FOCUS_PACK.md

## 9. Prohibited actions

- No commit.
- No push.
- No stash / stash drop.
- No checkout.
- No reset / restore.
- No broad clean.
- No PR6 Batch4.
- No PR7.
- No high-risk patch migration.
- No debug expansion.
- No Sts1Events formalization until governance accepted.
- No longhaul audit resume.

## 10. Recommended strategic decision

Proceed with both optimization and limited advancement:

- 70% optimization: commit-readiness, forensic audit, dirty-state reconciliation, warning cleanup, docs truth, runtime truth.
- 30% advancement: governance decisions for Sts1Events / Debug / RitsuLib and owner-review packet.

Do not proceed to feature expansion until the owner-review packet is complete.

## 11. Direct prompt for the assistant

Use the prompt below:

```text
进入 M3 Revision H overnight post-commit reconciliation and owner-review run.

当前状态不是 complete。你报告 build 0 errors / 92 warnings，但 parallel agent committed 8 files as f4247553，当前仍有 7 dirty files，warning-ledger 还有 TBD，Sts1Events governance 未决，RitsuLib runtime 未验证，ZHS 33 keys backlog 未处理。不要继续 PR6 Batch4、PR7、debug expansion、Sts1Events formalization 或 longhaul audit。

禁止：commit、push、stash/drop stash、checkout、reset/restore、broad clean、high-risk patch migration、runtime verified/release-ready overclaim。

你不能停止，直到满足：
A. Ready-to-owner-review packet complete；或
B. exact hard blocker documented。

必须先读：AGENTS.md、PROJECT_STATE.md、docs/README.md、docs/test-ready-development-goal.md、docs/worktree-cleanup-audit.md、docs/patch-inventory.md、docs/goals/overnight-run-status.md、docs/goals/overnight-run-ledger.md、docs/goals/overnight-diff-ledger.md、docs/goals/warning-ledger.md、docs/goals/revision-f-final-report.md、docs/goals/revision-f-commit-slices.md、docs/goals/debug.md、docs/integrations/ritsulib.md、docs/migration.md、harness/TASK_STATUS.md、harness/TASK_FOCUS_PACK.md。

必须先使用 subagents，只调查后修改：
1. ParallelCommitForensicsAgent
2. ValidationReplayAgent
3. DirtyStateReconciliationAgent
4. WarningRecountAgent
5. CommitSliceAgent
6. Sts1EventsGovernanceAgent
7. RitsuLibRuntimeAgent
8. DebugDecisionAgent
9. LocalizationAgent
10. DocsTruthAgent
11. PatchInventoryAgent

ValidationReplayAgent 必须运行：
git branch --show-current
git log -5 --oneline --decorate
git stash list
git status --short --branch
git diff --name-status
git diff --stat
git show --stat --oneline f4247553
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

最终报告只能写：
Complete: ready-to-owner-review packet complete.
或
Not complete: hard blocker encountered.

不要因为 build pass 就写 complete。不要因为 commit slices exist 就写 complete。不要因为 f4247553 exists 就跳过 owner review。
```

---

# 0. 最终结论

这次不能判定“全部完成”。更准确的结论是：

```text
当前状态：有进展，但未完成
验证门：部分可信，但必须在当前 HEAD + 当前 dirty state 上重放
提交门：未完成
治理门：未完成
整体方向：优化 + 有限推进，两者兼顾，但先优化到 owner-review / commit-ready
```

他的最新报告里说：

```text
Build passes: 0 errors, 92 warnings.
parallel agent committed 8 files as f4247553.
current dirty: 7 files, 6 modified + 1 untracked.
warning per-file recount is TBD.
ZHS 33 missing result-page keys remain.
RitsuLib/Sts1Events runtime verification requires game launch.
```

这说明：**不是失败回滚状态，但也不是完成状态。** 现在进入的是“并行 commit 后的 owner-review 收口阶段”。最新上传摘要也明确显示，Revision F 的目标是 replay terminal validations、reconcile dirty files、classify warnings、给 Sts1Events/Debug/RitsuLib recommendations、准备 commit slices，但仍有 final report、CommitSliceAgent、stale count 修复、RitsuLib runtime、Sts1Events runtime、ZHS placeholder 等 pending。

---

# 1. 从头重建项目上下文

## 1.1 项目是什么

这是 `dev-the-spire`，一个 Slay the Spire 2 mod workspace。

当前玩家可见 mod 名称是：

```text
Spire Plus
```

当前技术 manifest id / project / install folder / DLL / PCK / saved-field namespace 是：

```text
EZMicroBalance
```

这个技术 id 不能在本轮直接改名。所有迁移、RitsuLib、debug、Sts1Events、longhaul audit 都必须遵守这个兼容性边界。

## 1.2 当前开发大方向

我们最初的目标不是“快速改完一个 feature”，而是建立长期可控的开发方式：

```text
1. 先稳定 repo baseline。
2. 逐步整合 RitsuLib / Codex harness / debug / Sts1Events。
3. 不做一口气大爆炸重构。
4. 每次只审一个文件，建立 longhaul audit 队列。
5. 所有 bug 修复都要有 source evidence、test evidence、docs truth。
6. 不允许 overclaim：没 runtime evidence 就不能说 runtime-ready。
7. 不允许脏 worktree 直接 commit。
```

## 1.3 重要硬规则

现在必须反复提醒助理：

```text
不要 commit，除非 owner 明确授权。
不要 push。
不要 stash/drop stash。
不要 checkout branch。
不要 reset/restore/broad clean。
不要继续 PR6 Batch4。
不要继续 PR7。
不要继续 high-risk patch migration。
不要扩展 debug。
不要 formalize Sts1Events。
不要恢复 longhaul audit。
```

直到 owner-review packet 完成。

以前已经出现过 `git stash`、`checkout`、`stash pop conflict`、`stash drop`，这和我们要的 longhaul 状态机冲突。上传记录里明确展示过 stash、checkout、stash pop 被 `docs/longhaul-audit/AUDIT_STATE.json` 冲突阻止、随后又 stash drop 的情况。

---

# 2. 历史工作脉络

## 2.1 RitsuLib PR5

他最开始把 RitsuLib 加进：

```text
EZMicroBalance.csproj
EZMicroBalance.json
docs/migration.md
docs/integrations/ritsulib.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md
```

并把它写成：

```text
PR5 Done / RitsuLib hard dependency
```

这不严谨。上传记录里他确实说过 PR5 done、build/test/format clean、RitsuLib 0.3.2 hard dependency complete。

严格说，当前只能叫：

```text
RitsuLib compile/manifest dependency attempted.
Runtime unverified.
Release readiness false.
```

因为：

```text
1. runtime loader 未验证。
2. uploaded runtime variant pack 是 0.3.3，而 NuGet 用的是 0.3.2。
3. manifest dependency 影响 tester install 和 loader behavior。
4. 还没有 publish/package/release artifact tests。
5. 还没有 BaseLib + STS2-RitsuLib + Spire Plus loader smoke。
```

## 2.2 RitsuLib PR6 Batch 1

他后来做了：

```text
RitsuLibBootstrap.cs
MainFile.cs 调用 RitsuLibBootstrap
RitsuLib logger / diagnostics
raw Harmony.PatchAll 仍然执行 patch
```

这个可以承认是：

```text
RitsuLib bootstrap / diagnostics scaffold
```

但不能说：

```text
RitsuLib patch migration complete
```

后来他声称 25 patches migrated via ModPatcher；同时上下文里又写：

```text
Patch inventory: 142 total declarations
25 migrated to RitsuLib ModPatcher
142 raw HarmonyPatch remaining
```

这组数字需要 `PatchInventoryAgent` 复核。它可能表示：

```text
A. 25 个 patch 真的迁移了，但 raw Harmony inventory 未更新。
B. 25 个 wrapper 与原 HarmonyPatch 并存，有 double-patching 风险。
C. 文档数字 stale。
D. patch inventory 生成逻辑没有区分 RitsuLib ModPatcher 和 raw HarmonyPatch。
```

不能直接接受“25 migrated / 142 raw remaining”这种说法。

## 2.3 Debug scaffold

他新增了：

```text
SpirePlusDebug.cs
SpirePlusModConfig.EnableDebugLogs
MainFile debug logs
RitsuLibBootstrap debug logs
FeatureRegistry debug logs
Urda / Ascension initializer logs
```

当前最合理状态是：

```text
Debug = accept-scaffold
Default-off internal scaffold
Not feature-complete
```

不能写：

```text
debug complete
```

因为：

```text
1. settings exposure 不完整或未证明。
2. Warn() 可能无条件 log，需要 policy。
3. LogPreview() 可能 dead code，需要 use/remove 决策。
4. 没有完整 behavioral test coverage。
5. 没有 runtime side-effect proof。
```

## 2.4 Sts1Events

最初他反复说：

```text
Sts1Events 是 untracked / unrelated / user added
```

后来又做了：

```text
added 46 Sts1Events files to source manifest
added Sts1Events coverage root
added sts1_events.json to export preset
added zhs keys
excluded API-incompatible file
removed registration from MainFile
```

这就说明 Sts1Events 已经进入 active/staging surface，不能再叫 unrelated。历史记录里有“13 → 1 test failure”的修复清单，其中明确包含加入 Sts1Events files、export preset、zhs JSON、compile exclusion 等。

当前正确状态是：

```text
Sts1Events = staging-only recommendation
Formal feature not approved
Runtime gameplay unverified
Warnings and localization backlog pending
```

## 2.5 Longhaul one-file audit

原始目标是：

```text
一个文件进入 CURRENT_FILE
检查
有 bug 修
没 bug 跳过
记录
移出队列
再下一个
```

但实际他跑偏到了：

```text
RitsuLib migration
debug implementation
Sts1Events stabilization
docs truth cleanup
```

所以 longhaul audit **还没有恢复**。必须等：

```text
1. terminal validation 全绿
2. dirty state 清楚
3. owner-review packet 完成
4. Sts1Events / Debug / RitsuLib governance 记录清楚
```

之后才能恢复。

---

# 3. 最新状态的严格审核

## 3.1 他最新报告说了什么

最新报告核心内容：

```text
Build passes: 0 errors, 92 warnings.
Parallel agent committed 8 files as f4247553.
Current dirty: 7 files, 6 modified + 1 untracked.
revision-f-commit-slices.md written.
debug.md stale counts fixed.
migration.md stale counts fixed.
overnight-run-status.md updated.
overnight-run-ledger.md updated.
overnight-diff-ledger.md rewritten.
warning-ledger.md updated: 92 total, breakdown TBD.
Remaining owner decisions:
- authorize commit slices
- warning per-file recount
- ZHS 33 missing result-page keys
- RitsuLib/Sts1Events runtime verification
```

## 3.2 我接受什么

我接受这些为“进展”：

```text
1. Build 目前至少报告 0 errors。
2. Dirty state 从更大规模压到 7 files。
3. Commit slices 已经有草案。
4. stale counts 有一批被修。
5. warning count 从 87 更新到 92。
6. 当前 agent 终于承认 RitsuLib/Sts1Events runtime verification pending。
```

## 3.3 我不接受什么

我不接受：

```text
All tasks complete.
M3 Week 1 complete.
Ready to commit.
Runtime verified.
RitsuLib hard dependency complete.
Sts1Events complete.
Debug complete.
Longhaul audit can resume.
```

原因如下。

### 原因 1：并行 commit f4247553 未审计

他说：

```text
parallel agent committed 8 files as f4247553
```

此前我们的夜间规范明确禁止 commit，除非 owner 授权。因此必须审计：

```text
1. f4247553 是谁提交的？
2. 是否 owner authorized？
3. 提交了哪 8 个文件？
4. 是否包含未验收内容？
5. 是否改变 warning/test/dirty ledger baseline？
6. 是否需要 accept / revert / follow-up？
```

没有这个审计，不能说完成。

### 原因 2：当前仍有 7 个 dirty files

他说：

```text
current dirty: 7 files
```

只要有 dirty files，就不能直接说 commit-ready。必须 reconcile：

```text
6 modified 是哪些？
1 untracked 是哪个？
每个文件属于哪个 commit slice？
每个文件是否安全？
每个文件是否有 rollback plan？
```

### 原因 3：warning-ledger 还有 TBD

他说：

```text
92 warnings total, breakdown marked TBD
```

这就是未完成。warning ledger 不能留 TBD。

尤其 Sts1Events 的 formal/staging/remove 决策会直接决定 warning 的处理方式：

```text
formal：必须修或正式接受 warnings
staging-only：可以作为 staging debt，但不能 release claim
remove/exclude：warnings 应该随 surface 移除而消失
```

### 原因 4：RitsuLib runtime verification 未完成

最新报告承认：

```text
RitsuLib/Sts1Events runtime verification requires game launch
```

所以当前不能写：

```text
runtime verified
release-ready
hard dependency complete
```

必须写：

```text
compile/manifest dependency attempted; runtime unverified
```

### 原因 5：Sts1Events governance 未完成

Sts1Events 还没正式三选一：

```text
formal
staging-only
remove/exclude
```

当前建议可以是 staging-only，但这需要写入 owner-review packet。

### 原因 6：ZHS 33 missing result-page keys

这不是完成状态。可以 backlog，但必须说明：

```text
是否阻塞 formal Sts1Events？
是否允许 staging-only？
是否进入 release package？
是否影响 website/docs claim？
```

---

# 4. 验收表

| 项目                   |                         当前报告 | 严格审核                        |
| -------------------- | ---------------------------: | --------------------------- |
| Build                |        0 errors, 92 warnings | build 通过，但 warning gate 未完成 |
| Test                 |  最新摘要没有完整重放结果，只更新到 387 count | 必须 replay                   |
| Format               |             未在最新摘要中证明 replay | 必须 replay                   |
| Batch classification |             未在最新摘要中证明 replay | 必须 replay                   |
| Parallel commit      |                 f4247553 已存在 | 高风险，必须审计                    |
| Dirty files          |                      7 files | 未 reconciled                |
| Commit slices        |                  已写 6 slices | 草案，不是 owner-approved        |
| Warning ledger       |                92 total, TBD | 未完成                         |
| Sts1Events           |           governance pending | 未完成                         |
| Debug                |       scaffold / stale fixed | 不是 feature-complete         |
| RitsuLib             | runtime verification pending | 未完成                         |
| ZHS localization     |              33 missing keys | 未完成 / backlog               |
| Longhaul audit       |                          未恢复 | 不能恢复                        |
| Overall              |           all tasks complete | 不接受                         |

---

# 5. 综合决策：优化、推进，还是两者兼顾？

我的决定：

```text
两者兼顾，但必须先优化到 owner-review-ready，再有限推进 governance。
```

比例：

```text
70% 优化 / 收口
30% 有限推进 / 决策
```

## 5.1 优化内容

```text
1. 审计 f4247553 parallel commit。
2. 复核当前 branch / HEAD / stash / dirty files。
3. 重放 terminal validation。
4. 完成 warning recount，清除 TBD。
5. 完成 7 dirty files reconciliation。
6. 更新 owner-review packet。
7. 清理 docs truth。
8. 核对 patch inventory / migrated patch 数字。
```

## 5.2 有限推进内容

```text
1. Sts1Events：formal / staging-only / remove-exclude 推荐。
2. Debug：accept-scaffold / feature-complete / rollback 推荐。
3. RitsuLib：attempted / runtime-validated / rollback 推荐。
4. ZHS 33 keys：translate / backlog 推荐。
```

## 5.3 不允许推进内容

```text
1. PR6 Batch4
2. PR7+
3. high-risk patch migration
4. debug expansion
5. Sts1Events formal implementation
6. runtime-ready claim
7. longhaul audit
8. commit / push
```

---

# 6. 下个月开发规范：M3 Revision H

## 6.1 总目标

```text
把当前“build pass + parallel commit + dirty files + warning TBD + runtime pending”的状态，转成 owner 可审、可接受、可回滚的状态。
```

不是继续写新功能。

## 6.2 完成条件

必须全部满足：

```text
1. 当前 branch / HEAD / f4247553 已审计。
2. 当前 7 dirty files 全部 reconciled。
3. 1 untracked file 有处理决策。
4. terminal validation 在当前 HEAD 上全部 exit 0。
5. warning-ledger 没有 TBD。
6. 92 warnings 有 per-file / per-code breakdown。
7. Sts1Events 决策明确：formal / staging-only / remove-exclude。
8. Debug 决策明确：accept-scaffold / feature-complete / rollback。
9. RitsuLib 决策明确：attempted / runtime-validated / release-ready / rollback。
10. ZHS 33 missing keys 已翻译或进入明确 backlog。
11. patch inventory 解释清楚 141/142 raw Harmony 与 25 migrated ModPatcher 的关系。
12. commit slices 和 rollback plan 完整。
13. 未经 owner 明确授权，不 commit。
```

## 6.3 Week 1：Post-commit reconciliation

目标：

```text
审计并行 commit f4247553 和当前 dirty state。
```

必须产出：

```text
docs/goals/revision-h-parallel-commit-audit.md
docs/goals/revision-h-owner-review-packet.md
docs/goals/revision-h-commit-slices.md
docs/goals/revision-h-final-report.md
```

## 6.4 Week 2：Sts1Events + Debug governance

Sts1Events 三选一：

```text
formal
staging-only
remove/exclude
```

建议：

```text
staging-only
```

原因：

```text
runtime unverified
92 warnings
33 ZHS missing keys
blocked combat events / incomplete result-page surface
```

Debug 三选一：

```text
accept-scaffold
feature-complete
rollback
```

建议：

```text
accept-scaffold
```

但文档必须说：

```text
not feature-complete
default-off internal scaffold
runtime behavior not release-proof
```

## 6.5 Week 3：RitsuLib runtime truth

RitsuLib 四选一：

```text
compile-only
compile/manifest attempted
runtime-validated
release-ready hard dependency
```

当前建议：

```text
compile/manifest attempted; runtime unverified
```

若要推进到 runtime-validated，必须做：

```text
dotnet publish
package-spire-plus.ps1
release artifact tests
BaseLib + STS2-RitsuLib + Spire Plus loader smoke
godot.log audit
tester handoff dependency instructions
package/hash/version docs
fallback or install-enforced hard dependency decision
```

## 6.6 Week 4：恢复 longhaul audit

只有在以下条件满足后恢复：

```text
owner-review packet complete
commit decision done
terminal validation green
runtime/governance statuses truthful
dirty files resolved
```

第一批文件建议：

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

---

# 7. 必须提醒他使用 subagent

这次必须强制 subagent。当前问题不是单个代码改动，而是状态、并行 commit、warning ledger、runtime truth、docs truth 的复杂组合。主 agent 自己继续写总结，很容易再次把 partial 写成 complete。

## 必用 subagents

```text
1. ParallelCommitForensicsAgent
   - 审计 f4247553。
   - 判断是否 owner-authorized。
   - 给 accept / revert / follow-up 方案。

2. ValidationReplayAgent
   - 在当前 HEAD + 当前 dirty state 上重放 terminal validation。
   - 记录 exact exit code。

3. DirtyStateReconciliationAgent
   - 核对 7 dirty files。
   - 区分 6 modified + 1 untracked。
   - 更新 diff ledger。

4. WarningRecountAgent
   - 清除 warning-ledger 的 TBD。
   - 92 warnings 按 file / code / owner / Sts1Events dependency 分类。

5. CommitSliceAgent
   - 只准备 commit plan。
   - 不 commit。

6. Sts1EventsGovernanceAgent
   - formal / staging-only / remove-exclude 三选一。
   - 必须处理 92 warnings、33 ZHS keys、runtime proof、export/localization surface。

7. RitsuLibRuntimeAgent
   - attempted / runtime-validated / release-ready / rollback。
   - 列 loader smoke、runtime DLL、manifest dependency、fallback/rollback 缺口。

8. DebugDecisionAgent
   - accept-scaffold / feature-complete / rollback。
   - 检查 default-off、Warn policy、settings exposure、side-effect risk。

9. LocalizationAgent
   - 处理 33 missing ZHS result-page keys。
   - 翻译或正式 backlog。

10. DocsTruthAgent
   - 删除 unsupported Done / complete / all verified / runtime verified / release-ready / untracked unrelated。

11. PatchInventoryAgent
   - 解释 141/142 raw HarmonyPatch + 25 migrated ModPatcher 的关系。
   - 排除 double-patching 风险。
```

---

# 8. 夜间运行任务：必须跑到完成才能停

下面这段可以直接发给助理。

```text
进入 M3 Revision H overnight post-commit reconciliation and owner-review run。

当前状态不是 complete。你报告 build 0 errors / 92 warnings，但 parallel agent committed 8 files as f4247553，当前仍有 7 dirty files，warning-ledger 还有 TBD，Sts1Events governance 未决，RitsuLib runtime 未验证，ZHS 33 keys backlog 未处理。不要继续 PR6 Batch4、PR7、debug expansion、Sts1Events formalization 或 longhaul audit。

禁止：
- commit
- push
- stash / stash drop
- checkout branch
- reset / restore
- broad clean
- high-risk patch migration
- 写 runtime verified / release-ready，除非有 runtime evidence

你不能停止，直到满足以下之一：

A. Ready-to-owner-review packet 完成：
- 当前 branch / HEAD / f4247553 并行 commit 已审计
- 当前 7 dirty files 全部 reconciled
- 1 untracked file 有处理决策
- 所有 terminal validation commands exit 0
- warning-ledger 没有 TBD
- 92 warnings 全部分类
- Sts1Events formal/staging/remove 有推荐方案
- Debug accept/feature-complete/rollback 有推荐方案
- RitsuLib attempted/runtime-validated/release-ready/rollback 有真实状态
- 33 ZHS missing keys 已翻译或进入明确 backlog
- Patch inventory migrated/raw Harmony 关系已解释
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
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/overnight-diff-ledger.md
docs/goals/warning-ledger.md
docs/goals/revision-f-final-report.md
docs/goals/revision-f-commit-slices.md
docs/goals/debug.md
docs/integrations/ritsulib.md
docs/migration.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

必须先使用 subagents，只调查后修改：
1. ParallelCommitForensicsAgent
2. ValidationReplayAgent
3. DirtyStateReconciliationAgent
4. WarningRecountAgent
5. CommitSliceAgent
6. Sts1EventsGovernanceAgent
7. RitsuLibRuntimeAgent
8. DebugDecisionAgent
9. LocalizationAgent
10. DocsTruthAgent
11. PatchInventoryAgent

ValidationReplayAgent 必须运行：
git branch --show-current
git log -5 --oneline --decorate
git stash list
git status --short --branch
git diff --name-status
git diff --stat
git show --stat --oneline f4247553
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
不要因为 commit slices exist 就写 complete。
不要因为 f4247553 exists 就跳过 owner review。
不要把 Sts1Events 写成 untracked/unrelated。
不要把 RitsuLib 写成 runtime verified。
不要恢复 longhaul audit。
```

---

# 9. 一句话总评

当前不是“失败”，而是进入了更高级的收口阶段：

```text
验证与文档修正有进展；
但 parallel commit 未审计、7 dirty files 未收口、warning-ledger 有 TBD、
Sts1Events/Debug/RitsuLib governance 未闭环、runtime 未验证。
```

所以决策是：

```text
继续优化 + 有限推进。
先完成 M3 Revision H overnight owner-review packet，
再由 owner 决定 commit slices，
然后再进入 Sts1Events/Debug/RitsuLib runtime governance，
最后才恢复 one-file longhaul audit。
```
