## 严格审核结论

这次不能简单判定“完成”。更准确的结论是：

```text
M2 Revision F terminal validation：通过
M2 Revision F overall / commit-readiness：未完成
下个月方向：优化 + 有限推进，两者兼顾，但先优化到 owner-review / commit-ready
```

他这轮确实完成了重要进展：`dotnet clean + build`、test、format、whitespace、batch classification 都有绿色结果；测试数也从 Revision E 的 354 提升到 387，且 0 failed；`report-worktree-batches.ps1` 现在是 10 个 worktree entries (script output)、0 unclassified，实际 git status 显示 12 dirty + 3 untracked = 15 total entries。

但他自己的状态里也写明：最终 Revision F 报告还没整合、CommitSliceAgent 还没准备提交计划、`debug.md` 仍有 stale counts、`migration.md` 可能还有 stale references、RitsuLib runtime 未验证且没有 fallback、Sts1Events runtime 未验证、ZHS 还有 38 个 placeholder entries。

所以现在不是失败状态，而是：

```text
验证门已过，但还没到可提交 / 可推进功能 / 可恢复 longhaul audit 的状态。
```

---

## 当前状态与目标对比

我们的原目标是：

```text
1. 恢复真实绿色 baseline
2. 用 subagent 分拆审计
3. 所有 dirty / untracked 文件可解释、可分组、可回滚
4. Sts1Events / Debug / RitsuLib 状态真实，不 overclaim
5. 只有 commit-ready 后，才进入 Week 2 governance 或恢复 longhaul audit
```

当前对比结果：

| 目标                 |                                                                                                            当前状态 | 审核                            |
| ------------------ | --------------------------------------------------------------------------------------------------------------: | ----------------------------- |
| 终端验证全绿             |                clean build 0 errors / 92 warnings，387 tests passed，format clean，diff clean，batch 0 unclassified | **通过**                        |
| 真实 warning 状态      |                                            92 CS warnings，全是 Sts1Events nullable；incremental build 会隐藏 warnings | **部分通过，需要治理决策**               |
| dirty/untracked 解释 |                                                                   15 total：12 dirty + 3 untracked；diff ledger 已重写 | **部分通过，仍需 commit slices**     |
| subagent 使用        |                                                                                                5 个 subagents 完成 | **部分通过，CommitSliceAgent 未完成** |
| Sts1Events 状态      |                             建议 staging-only，runtime 未验证，92 warnings，8 blocked combat events，33 missing ZHS result-page keys | **未完成治理**                     |
| Debug 状态           | 建议 accept-scaffold；default-off，但 Warn 无条件、LogPreview dead code、无 dedicated behavioral tests、无 settings exposure | **scaffold 可接受，feature 未完成**  |
| RitsuLib 状态        |     attempted；compile/manifest wired，25 patches migrated，但 runtime 未验证，bootstrap 无 fallback，0.3.2 = 0.3.2 aligned | **未完成 / runtime blocker**     |
| 文档真相               |                                                        已修一部分，但 debug.md、migration.md 仍有 stale counts/references | **未完成**                       |
| 可提交状态              |                                                                             还需要 CommitSliceAgent 和 final report | **未完成**                       |

---

## 逐步严格审核

### 1. Terminal validation：通过，但不能扩大解释

他报告：

```text
dotnet clean + dotnet build .\EZMicroBalance.csproj → 0 errors, 92 CS warnings
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj → 387 passed, 0 failed, 21 skipped
dotnet format .\EZMicroBalance.csproj --verify-no-changes → clean
git diff --check → clean
report-worktree-batches.ps1 → 10 dirty (script), 0 unclassified
```

这可以接受为：

```text
Validation gate passed.
```

但不能扩大成：

```text
全部完成
release-ready
runtime-ready
可直接 commit
```

因为 warnings、runtime、commit slices、governance 都还没收口。

---

### 2. Dirty / untracked 状态：部分收口，但还不能 commit

当前关键数字是：

```text
15 total worktree entries
12 dirty + 3 untracked
0 unclassified
```

具体包括：

```text
batch 1: 1 docs
batch 3: 1 test
batch 5: 6 files (scripts, tests, test config)
batch 8: 3 goals docs
untracked: 2 source stubs + 1 test stub
```

这已经比之前的 32 dirty files 清晰很多。但下一步还必须由 `CommitSliceAgent` 把这 15 个文件分成可审查提交片：

```text
1. goals/status truth fixes
2. Sts1Events staging docs / research
3. localization staging debt
4. migration / RitsuLib truth alignment
```

没有 commit slices 前，不要 commit。

---

### 3. Sts1Events：建议 staging-only，不能 formal

Subagent 建议是：

```text
Sts1Events governance = staging-only
```

理由合理：feature gate 有双重安全，env unset 会 disabled，`RegisterGated()` 有 explicit `Off: return`，CanaryOnly 只注册 4 个正确事件，并且有 24 个 guard tests。

但不能 formal，因为还有：

```text
runtime gameplay unverified
92 nullable warnings
8 blocked combat events
33 missing ZHS result-page keys
no event images
```

所以结论：

```text
Sts1Events 可以作为 staging-only 保留。
不能作为正式功能推进。
不能进入 release claim。
不能恢复 full gameplay implementation。
```

---

### 4. Debug：accept-scaffold，但不是 feature-complete

Subagent 建议是：

```text
Debug = accept-scaffold
```

这可以接受，但要写清楚边界：

```text
default-off internal scaffold
zero runtime side effects when off
SpirePlusDebug.Warn() 无条件 log，需文档说明
LogPreview() dead code，需 backlog
没有 dedicated behavioral test coverage
没有 settings exposure
not feature-complete
```

所以结论：

```text
Debug scaffold 可保留。
不能写 debug complete。
不能继续扩展 debug，除非进入单独 feature-complete spec。
```

---

### 5. RitsuLib：仍然是 attempted，不是 runtime-ready

这是最大风险。Subagent 报告：

```text
RitsuLib = attempted
compile/manifest wired
STS2.RitsuLib 0.3.2
25 patches migrated via ModPatcher
RitsuLibBootstrap.ApplyPatches() unconditionally called from MainFile.Initialize()
no try-catch / feature gate / null guard
no runtime proof
version aligned: NuGet 0.3.2 = manifest min_version 0.3.2
will throw TypeLoadException/FileNotFoundException if STS2-RitsuLib.dll missing
```

这意味着：

```text
RitsuLib hard dependency / runtime integration 未完成。
```

更严格地说：如果 `EZMicroBalance.json` 已经声明 `STS2-RitsuLib` runtime dependency，那么 tester install instructions、package docs、loader smoke 都必须跟上。否则就是“compile/manifest attempted; runtime unverified”。

当前必须禁止这些表述：

```text
RitsuLib complete
hard dependency verified
release-ready
runtime validated
```

---

### 6. Patch inventory：需要专门核对

当前记录里有一个需要审查的数字组合：

```text
Patch inventory: 141 compile-active raw declarations + 25 migrated = 166 runtime-active
25 migrated to RitsuLib ModPatcher
141 raw HarmonyPatch remaining (1 additional dead-code declaration behind #if REPLACEMENT_PROTOTYPE_ENABLED)
```

这个表述可能有歧义：如果 25 个已经迁移到 RitsuLib ModPatcher，为什么 raw HarmonyPatch remaining 仍是 141？是否存在 double-patching？是否是"141 raw Harmony attributes 仍存在，但 25 也有 ModPatcher wrapper"？这必须交给 `PatchInventoryAgent` 复核，不能直接接受。

结论：

```text
PatchInventoryAgent 已复核：无 double-patching。25 migrated 类无 [HarmonyPatch] 属性。
```

这个表述可能有歧义：如果 25 个已经迁移到 RitsuLib ModPatcher，为什么 raw HarmonyPatch remaining 仍是 142？是否存在 double-patching？是否是“142 raw Harmony attributes 仍存在，但 25 也有 ModPatcher wrapper”？这必须交给 `PatchInventoryAgent` 复核，不能直接接受。

结论：

```text
Patch inventory / migrated count 需要再核对。
```

---

### 7. 文档真相：仍未完成

他已修：

```text
TASK_STATUS.md
TASK_FOCUS_PACK.md
overnight-run-status.md
overnight-diff-ledger.md
migration.md blockquote
```

但他也承认：

```text
debug.md still references "354 passed" and "32 dirty files"
migration.md may still have additional stale references
overnight-run-ledger.md still Revision D internally
```

所以文档不能算收口。下一轮必须由 `DocsTruthAgent` 修完 stale counts 和 unsupported claims。

---

## 综合判断：优化、推进，还是两者兼顾？

我的决定是：

```text
两者兼顾，但先优化，后推进。
```

具体比例：

```text
70% 优化 / 收口：
- final report
- commit slices
- stale docs
- warning ledger
- patch inventory reconciliation
- runtime status truth

30% 推进：
- Sts1Events staging decision
- Debug scaffold acceptance
- RitsuLib runtime verification plan
```

不能做的推进：

```text
继续 PR6 Batch4
继续 PR7
继续迁移高风险 patch
扩展 debug
把 Sts1Events formalize
恢复 longhaul audit
```

---

## 下个月开发规范：M3 June 2026

我已经写成文件：
[下载 M3 June 2026 Overnight Commit Governance Spec](sandbox:/mnt/data/devspire_m3_june_overnight_commit_governance_spec.md)

### M3 总目标

```text
把 M2 Revision F 的绿色验证状态，转成 owner 可审、可提交、可回滚的 commit-ready packet；
然后完成 Sts1Events / Debug / RitsuLib governance；
最后恢复 one-file longhaul audit。
```

---

## M3 Week 1：Commit Readiness Gate

### 目标

完成 Revision F 收口，不写新功能。

### 必须产出

```text
docs/goals/revision-f-final-report.md
docs/goals/revision-f-commit-slices.md
updated docs/goals/overnight-run-status.md
updated docs/goals/overnight-run-ledger.md
updated docs/goals/overnight-diff-ledger.md
updated docs/goals/warning-ledger.md
fixed docs/goals/debug.md
fixed docs/goals/migration.md
```

### 必跑验证

```powershell
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

### Week 1 完成条件

```text
1. 所有命令 exit 0
2. warning count 真实 (92, not 87)
3. 15 个 worktree entries 全部 reconciled (12 dirty + 3 untracked)
4. CommitSliceAgent 完成
5. final report 完成
6. stale counts 全部修正
7. RitsuLib/Sts1Events/Debug 状态真实
8. 没有 owner 授权前不 commit
```

---

## M3 Week 2：Governance decisions

### Sts1Events

默认建议：

```text
staging-only
```

除非 owner 明确要 formalize。

Formalize 前必须完成：

```text
92 nullable warnings 修复或正式接受
33 missing ZHS result-page keys 翻译
8 blocked combat events 处理
event images / resource plan
runtime gameplay proof
manual test plan
```

### Debug

默认建议：

```text
accept-scaffold
```

Feature-complete 前必须完成：

```text
settings exposure
behavioral tests
side-effect audit
Warn() policy docs
LogPreview() use/remove decision
```

---

## M3 Week 3：RitsuLib runtime truth

当前状态必须写成：

```text
compile/manifest dependency attempted; runtime unverified
```

如果要升级为 runtime-validated，必须补：

```powershell
dotnet publish .\EZMicroBalance.csproj
.\scripts\package-spire-plus.ps1
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

还要补：

```text
BaseLib + STS2-RitsuLib + Spire Plus loader smoke
godot.log audit
tester handoff dependency instructions
package/hash/version docs
runtime fallback or install-enforced hard dependency decision
```

---

## M3 Week 4：恢复 one-file longhaul audit

只有这些完成后才能恢复：

```text
commit-ready packet 完成
owner 处理 commit decision
Sts1Events/Debug/RitsuLib governance 已记录
terminal validation 仍然全绿
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

每次仍然只允许一个文件，结果只能是：

```text
fixed
skipped
blocked
```

---

## 必须使用的 subagent

下一轮不能让主 agent 自己一边做一边验收，必须先开 subagents：

```text
ValidationReplayAgent
- 重新跑 clean build / test / format / diff / batch classification。

DiffReconciliationAgent
- 核对 9 total worktree entries，区分 tracked dirty 和 untracked。

CommitSliceAgent
- 准备 commit slices，不准 commit。

DocsTruthAgent
- 修 stale counts 和 unsupported claims。

PatchInventoryAgent
- 核对 141 raw HarmonyPatch + 25 migrated ModPatcher 的关系，排除 double-patching。

RitsuLibRuntimeAgent
- 判断 attempted/runtime-unverified，提出 fallback 或 install-enforced 方案。

Sts1EventsGovernanceAgent
- 维持 staging-only 或给出 formal/remove 方案。

DebugDecisionAgent
- 维持 accept-scaffold 或给出 feature-complete/rollback 方案。

LocalizationAgent
- 核对 33 missing ZHS result-page keys，列 translation backlog。
```

---

## 夜间运行任务：必须跑到完成才能停

你可以直接把下面这段发给他：

```text
进入 M3 Week 1 overnight Commit Readiness Gate。

当前 M2 Revision F terminal validation green 是进展，但不是整体完成。不要 commit，不要 push，不要 stash/drop stash，不要 checkout，不要 reset，不要 broad clean，不要继续 PR6 Batch4，不要继续 PR7，不要扩展 debug，不要恢复 longhaul audit。

你不能停止，直到满足以下之一：

A. Ready-to-owner-review packet 完成：
- 所有 terminal validation commands exit 0
- clean/rebuild warning count 真实记录
- 15 个 worktree entries 全部 reconciled (12 dirty + 3 untracked)
- CommitSliceAgent 完成 commit plan
- Revision F final report 完成
- debug.md / migration.md / overnight-run-ledger stale counts 全部修正
- RitsuLib / Sts1Events / Debug 状态真实
- Patch inventory migrated/raw Harmony 关系已解释
- 33 missing ZHS result-page keys 进入 backlog
- 没有 unsupported Done / complete / runtime verified / release-ready / untracked unrelated 声明
- 没有 owner 授权前不 commit

B. Hard blocker：
- 写明 exact command / exact file
- 为什么当前 worktree 无法解决
- rollback / staging / owner decision 选项
- 下一步需要 owner 决定什么

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
docs/integrations/ritsulib.md
docs/migration.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

必须先用 subagents，只调查后修改：
1. ValidationReplayAgent
2. DiffReconciliationAgent
3. CommitSliceAgent
4. DocsTruthAgent
5. PatchInventoryAgent
6. RitsuLibRuntimeAgent
7. Sts1EventsGovernanceAgent
8. DebugDecisionAgent
9. LocalizationAgent

必跑命令：
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified

必须产出：
docs/goals/revision-f-final-report.md
docs/goals/revision-f-commit-slices.md
updated docs/goals/overnight-run-status.md
updated docs/goals/overnight-run-ledger.md
updated docs/goals/overnight-diff-ledger.md
updated docs/goals/warning-ledger.md
fixed docs/goals/debug.md
fixed docs/goals/migration.md

最终报告只能写两种之一：
Complete：ready-to-owner-review packet 完成，且所有命令 exit 0。
Not complete：遇到 hard blocker，并列出 exact blocker。
```

---

## 一句话总评

他现在已经把**验证门**跑绿了，这是实际进展；但还没完成**提交门**和**治理门**。下一步不是继续写功能，也不是直接 commit，而是进入 M3 Week 1 overnight commit-readiness run：用 subagents 把 15 个 worktree entries (12 dirty + 3 untracked)、92 warnings、RitsuLib runtime 风险、Sts1Events staging、Debug scaffold、patch inventory、stale docs 全部收口。
