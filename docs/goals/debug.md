## 严格审核结论

这次必须更新结论：**上一个主要 runtime blocker 已经被关闭，但整体任务仍未完成。**

2026-06-17 debug closeout：debug scope 可以验收为 source/static 完成。
`SpirePlusDebug` 的 general info diagnostics 只通过 `SPIREPLUS_ENABLE_DEBUG_LOGS=1`
或 legacy `EZMB_ENABLE_DEBUG_LOGS=1` 打开；玩家 Mod Settings 不再暴露 broad debug 开关；
preview diagnostics 仍使用 localized `ShowPreviewDebugLogs`。
本次复验在没有重叠 testhost 后通过：`dotnet build EZMicroBalance.sln -m:1 --no-incremental`
0 warnings / 0 errors；`ReleaseEvidenceGateTests` 9 / 0 / 0；
互补 no-build lane 448 / 0 / 39 / 487，合计 split coverage
457 passed / 0 failed / 39 skipped / 496 total；`dotnet format`、patch inventory、
worktree batch classifier、`git diff --check` 通过。
未运行 publish/package/runtime smoke，不产生 gameplay、clicked UI、save-load、co-op、QA
或 release-ready 证明。

最准确的状态是：

```text
M5 Revision M Off-loader runtime drift closure：完成
beta.85 v0.107.0 Off loader packet：干净
Static validation：当前记录为绿
Release-ready：否
Live/gameplay-ready：否
Commit/owner decision：仍未完成
Longhaul audit：仍不能恢复
下一步：继续优化 + 有限推进，但现在可以从“修 Off blocker”进入“补 evidence + owner commit authorization”
```

新的下月开发规范和 overnight run 以本文件当前内容为准；不要依赖 sandbox-only 下载链接作为当前证据。

---

## 1. 当前真实状态

最新 `PROJECT_STATE.md` 已经明确：M5 Revision M 的 root cause 是 Spire Plus runtime API drift，不是 BaseLib/RitsuLib 缺失；beta.84 的 v0.107.0 Off smoke 曾经进主菜单但只有 17/25 ModPatcher patch 成功，并触发 stale `EctoplasmGoldGatePatch` initializer exception；现在这轮已经修掉 getter-target drift 并打包 beta.85，新的 isolated v0.107.0 Off smoke 已经进主菜单、选择 RitsuLib compat branch `0.107.0`、应用 25/25 Spire Plus patches，并且 audit clean。这个证据只是 loader proof，不是 live gameplay 或 release readiness。

当前 build/test/package 记录也比之前强：beta.85 runtime-fix validation 是 0 build errors / 0 warnings；`ReleaseEvidenceGateTests` 单独 9/0/0 通过，互补 no-build test lane 是 466/0/21，通过后合计 split coverage 475 passed / 0 failed / 21 skipped；opt-in package/artifact subset 67/0/0 也通过；但文档明确说 June 11 dirty changes 仍没有 gameplay、clicked UI、save-load、route traversal、preview-tools、Vakuu、co-op、independent QA、clean-worktree proof。

Runtime 部分也已经更新：`STS2-RitsuLib v0.4.16` 已安装，beta.85 v0.107.0 package Off smoke 在 `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`，已加载 BaseLib/RitsuLib，选择 compat branch 0.107.0，25/25 ModPatcher patches 应用，并 audit clean；先前 beta.84 失败证据现在只作为 root-cause 证据。

`docs/goals/m5-revision-m-final-report.md` 也写明：Revision M 对 beta.85 v0.107.0 Off loader runtime drift closure 完成，但不是 live-ready 或 release-ready；fresh beta.85 CanaryOnly 和 AdditiveBatch1 smokes 还没记录，gameplay、clicked UI、save-load、replacement、multiplayer、independent QA、release handoff proof 仍 pending。

---

## 2. 是否完成？

### 可以验收的部分

可以验收的是：

```text
M5 Revision M 的 Off-loader runtime-drift closure
```

理由：

```text
1. beta.84 的 red smoke root cause 已定位到 stale API / Ectoplasm target。
2. beta.85 package 已刷新。
3. beta.85 v0.107.0 Off smoke clean。
4. RitsuLib v0.4.16 / compat 0.107.0 加载成功。
5. Spire Plus 25/25 migrated patches 应用。
6. EctoplasmGoldGatePatch exception 消失。
7. package checker / opt-in artifact subset / static validations 有记录。
```

### 不能验收的部分

不能验收为整体完成：

```text
1. CanaryOnly / AdditiveBatch1 fresh beta.85 runtime proof 仍 pending。
2. gameplay / clicked UI / save-load / replacement / multiplayer 仍 pending。
3. independent QA 仍 pending。
4. clean worktree / owner commit decision 仍 pending。
5. release handoff 仍 pending。
6. Longhaul audit 仍不能恢复。
7. Batch 4c 仍 proposal-only。
```

`m5-revision-m-owner-review-packet.md` 的 owner 决策也支持这个分层：runtime drift source fix 建议 accept，beta.85 package 只建议 “accept as loader-smoke package, not gameplay/release proof”；RitsuLib compile/manifest min version 是否 bump 到 0.4.16 要 defer 到 owner package-version decision；StS1 events 仍保持 staging-only；commit/push 在 active validation processes 存在时不要做。

---

## 3. 每一步严格检查

### 3.1 Static validation

当前可以接受为绿：

```text
build/test/format/diff/patch inventory/batch classifier：绿
package checker：通过
opt-in package/artifact subset：通过
```

但必须注意：这些仍不是 gameplay/live proof。项目自己的 goal 也明确说 build/test/publish 不能作为 live-game、save-load、death/failure、co-op evidence。

### 3.2 Runtime Off loader

当前已经通过：

```text
beta.85 v0.107.0 Off loader packet：clean
```

这是本轮最大的有效完成项。

### 3.3 CanaryOnly / AdditiveBatch1

仍未完成：

```text
fresh beta.85 CanaryOnly：pending
fresh beta.85 AdditiveBatch1：pending
```

历史 v0.106.1 / June 2 evidence 不能替代 beta.85 v0.107.0 evidence。当前 validation 文档明确把 June 2 Off/Canary/Additive evidence 标成 historical，并说不是当前 beta.85 enabled-mode proof。

### 3.4 RitsuLib version/package decision

当前状态：

```text
Runtime installed: STS2-RitsuLib v0.4.16
Compile package: STS2.RitsuLib 0.3.2
Manifest min_version: 0.3.2
```

Owner packet 建议：不要自动 bump；除非 owner 决定做 future versioned tester package，否则 compile/manifest min 先保留 0.3.2。

### 3.5 Sts1Events

当前状态：

```text
Sts1Events: staging-only
```

不能 formalize。原因是 runtime event encounter proof、gameplay proof、ZHS/key/resource 等还没闭环。Owner packet 也建议 StS1 events 保持 staging-only，因为 Big Fish/Divine Fountain 变化仍缺 runtime event encounter proof。

### 3.6 Debug

当前状态：

```text
Debug: accept-scaffold
```

这不是 feature-complete。`m5-revision-m-final-report` 写明已移除 unused `SpirePlusDebug.LogPreview`，并增加了 debug logging gates 的 static guard coverage：general info diagnostics 是 internal-only，需要 `SPIREPLUS_ENABLE_DEBUG_LOGS=1` 或旧 alias；preview diagnostics 保持在 localized `ShowPreviewDebugLogs` setting 后面；warnings 仍允许 degraded runtime paths。

### 3.7 Commit / owner review

当前不能直接 commit。`m5-revision-m-commit-slices.md` 只是 planning，明确说 no commit or push authorized；并给出 source fix、beta.85 package/hash docs、StS1 event staging updates、Revision M governance docs 四个 slice。commit rule 明确要求：在无 overlapping same-repo processes 的情况下 replay validation；如果 validation fail，则 split failing source slice 并报告 exact blocker。

---

## 4. 与我们的目标对比

我们的目标是：

```text
1. 静态验证真实全绿
2. 当前 runtime blocker 关闭
3. 不把 loader proof 扩大成 gameplay/release proof
4. Sts1Events / Debug / RitsuLib 状态真实
5. owner 未授权不 commit
6. 恢复 longhaul audit 前必须有 governance / evidence closure
```

现在达成：

```text
1. 静态验证记录为绿
2. beta.85 v0.107.0 Off loader blocker 已关闭
3. docs 已经明确不是 live/release proof
4. RitsuLib/Debug/Sts1Events 状态比之前真实
5. commit slices 已规划但未自动 commit
```

未达成：

```text
1. beta.85 CanaryOnly / AdditiveBatch1 runtime proof
2. gameplay / clicked UI / save-load / co-op proof
3. independent QA
4. owner commit authorization
5. release handoff
6. longhaul audit readiness
```

综合判断：

```text
继续优化 + 有限推进，两者兼顾。
```

比例我建议调整为：

```text
55% 优化：
- 复跑 validation，保证没有 overlapping process
- owner commit packet
- docs truth / package hash truth
- commit authorization

45% 有限推进：
- CanaryOnly smoke
- AdditiveBatch1 smoke
- package/RitsuLib version decision
```

不允许：

```text
PR6 Batch4c
Batch5
PR7
high-risk migration
Sts1Events formalization
debug expansion
release-ready claim
longhaul audit
```

---

## 5. 下个月开发规范：M5 Revision N

我建议把下一阶段定义为：

```text
M5 Revision N: beta.85 Evidence Expansion + Owner Commit Authorization
```

本节内容就是当前 M5 Revision N spec；不要依赖 sandbox-only 下载链接作为当前证据。

### Week 1：Owner Commit Authorization + Validation Replay

目标：

```text
确认 beta.85 Off-loader closure packet 当前有效，并准备 owner-authorized commit slices。
```

必跑：

```powershell
dotnet build EZMicroBalance.sln -m:1 --no-incremental
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test EZMicroBalance.sln --no-build --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

必须产出：

```text
docs/goals/m5-revision-n-final-report.md
docs/goals/m5-revision-n-owner-commit-packet.md
docs/goals/m5-revision-n-validation-replay.md
docs/goals/m5-revision-n-runtime-evidence-plan.md
```

### Week 2：beta.85 CanaryOnly / AdditiveBatch1 Evidence

只在没有 overlapping validation/runtime processes 时运行：

```text
1. beta.85 CanaryOnly runtime smoke
2. beta.85 AdditiveBatch1 runtime smoke
3. godot.log audit
4. evidence folder recorded
```

必须保持：

```text
Sts1Events staging-only
no gameplay proof claim
no release-ready claim
```

### Week 3：Package / RitsuLib Version Decision

Owner 二选一：

```text
A. 保持 beta.85 compile/manifest 0.3.2，外部 runtime 使用 STS2-RitsuLib v0.4.16。
B. 准备 future package-version increment，把 compile package 和 manifest min_version 都升到 0.4.16。
```

如果选择 B，必须做 version bump、publish/package、artifact tests、handoff/hash/website docs。

### Week 4：恢复 one-file longhaul audit

仅在这些条件满足后恢复：

```text
owner commit/rollback decision complete
static validation green
Off loader proof clean
CanaryOnly/AdditiveBatch1 clean or owner-deferred
Sts1Events/Debug/RitsuLib governance recorded
no stale truth blockers
```

首批文件仍建议从 RitsuLib / runtime drift 相关开始：

```text
EZMicroBalance.csproj
EZMicroBalance.json
EZMicroBalanceCode/MainFile.cs
EZMicroBalanceCode/Core/Integrations/RitsuLib/RitsuLibBootstrap.cs
EZMicroBalanceCode/Ancients/Patches/PickupRewardGatePatches.cs
EZMicroBalanceCode/Ancients/Patches/PickupRewardService.cs
tests/EZMicroBalance.Tests/RitsuLibMigrationGuardTests.cs
tests/EZMicroBalance.Tests/AncientHighRiskSourceGuardTests.cs
docs/integrations/ritsulib.md
docs/reviews/current-validation.md
```

---

## 6. 必须使用 subagent

下一轮要继续使用 subagent，但重点要更新：

```text
CurrentStateAgent
ProcessCoordinationAgent
ValidationReplayAgent
RuntimeEvidenceAgent
CanaryOnlySmokeAgent
AdditiveBatch1SmokeAgent
OwnerCommitAgent
PackageHandoffAgent
DocsTruthAgent
Sts1EventsGovernanceAgent
DebugDecisionAgent
RitsuLibRuntimeAgent
LonghaulRecoveryAgent
```

特别强调：

```text
ProcessCoordinationAgent 必须先确认没有 active dotnet/testhost/runtime processes。
CanaryOnlySmokeAgent 和 AdditiveBatch1SmokeAgent 不能和 validation lanes 重叠。
OwnerCommitAgent 只能准备 commit packet，不能自行 commit。
RitsuLibRuntimeAgent 不能擅自 bump 0.4.16。
```

---

## 7. Overnight run：必须跑到完成才能停

你可以直接发给他：

```text
进入 M5 Revision N overnight beta.85 evidence expansion and owner-commit authorization run。

当前状态 NOT COMPLETE，但 beta.85 v0.107.0 Off loader packet 已 clean。不要继续 PR6 Batch4c、Batch5、PR7、debug expansion、Sts1Events formalization 或 longhaul audit。

禁止：
- commit or push unless owner explicitly authorizes after commit packet review
- stash / stash drop
- checkout branch
- reset / restore
- broad clean
- continue PR6 Batch4c, Batch5, or PR7
- expand debug
- formalize Sts1Events
- resume longhaul audit
- claim gameplay/live/release readiness without corresponding evidence

你不能停止，直到满足以下之一：

A. Owner-ready packet complete:
- no overlapping validation/runtime processes
- static validation replay exits 0
- current dirty state reconciled
- commit slices are ready for owner decision
- beta.85 Off loader proof remains documented as clean
- CanaryOnly/AdditiveBatch1 plan is complete, or smokes are run cleanly if process coordination allows
- Sts1Events staging-only recommendation recorded
- Debug accept-scaffold recommendation recorded
- RitsuLib runtime/package decision recorded
- docs/harness truth updated
- no unauthorized commit

B. Hard blocker:
- exact command/log/file
- why current worktree cannot resolve it
- rollback/staging/owner-decision options
- exact owner action required

必须先读：
AGENTS.md
PROJECT_STATE.md
docs/README.md
docs/test-ready-development-goal.md
docs/reviews/current-validation.md
docs/integrations/ritsulib.md
docs/goals/m5-revision-m-final-report.md
docs/goals/m5-revision-m-owner-review-packet.md
docs/goals/m5-revision-m-commit-slices.md
docs/patch-inventory.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

必须先使用 subagents：
1. CurrentStateAgent
2. ProcessCoordinationAgent
3. ValidationReplayAgent
4. RuntimeEvidenceAgent
5. CanaryOnlySmokeAgent
6. AdditiveBatch1SmokeAgent
7. OwnerCommitAgent
8. PackageHandoffAgent
9. DocsTruthAgent
10. Sts1EventsGovernanceAgent
11. DebugDecisionAgent
12. RitsuLibRuntimeAgent
13. LonghaulRecoveryAgent

ValidationReplay 必须运行：
dotnet build EZMicroBalance.sln -m:1 --no-incremental
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet test EZMicroBalance.sln --no-build --logger "console;verbosity=minimal" -- RunConfiguration.MaxCpuCount=1
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified

必须创建/更新：
docs/goals/m5-revision-n-final-report.md
docs/goals/m5-revision-n-owner-commit-packet.md
docs/goals/m5-revision-n-validation-replay.md
docs/goals/m5-revision-n-runtime-evidence-plan.md
docs/reviews/current-validation.md
docs/integrations/ritsulib.md
PROJECT_STATE.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

最终报告只能写两种之一：
Complete: owner-ready packet complete.
Not complete: exact hard blocker encountered.

不要因为 Off loader proof clean 就写整体 complete。
不要没有 gameplay/UI/save/co-op evidence 就写 live-ready。
不要没有 publish/package/runtime/handoff evidence 就写 release-ready。
不要在 owner decision gates 关闭前恢复 longhaul audit。
```

---

## 一句话总评

这次状态已经从“v0.107.0 runtime drift 红灯”推进到“beta.85 Off loader 绿灯”。这是实质完成项；但仍不是 release/live 完成。下一步不是继续迁移，而是 **M5 Revision N：补 beta.85 CanaryOnly/AdditiveBatch1 evidence、做 owner commit authorization、确认 RitsuLib/package version decision，然后再考虑恢复 one-file longhaul audit。**
