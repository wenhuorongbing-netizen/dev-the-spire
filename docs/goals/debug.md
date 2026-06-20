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
beta.88 v0.107.1 AdditiveBatch1 loader/registration packet：干净
Static/no-game validation：当前记录为绿
Release-ready：否
Live/gameplay-ready：否
Commit/owner decision：仍未完成
Longhaul audit：仍不能恢复
下一步：继续优化 + 有限推进，但现在可以从“修 loader blocker”进入“补 gameplay/manual evidence + owner commit authorization”
```

新的下月开发规范和 overnight run 以本文件当前内容为准；不要依赖 sandbox-only 下载链接作为当前证据。

## 2026-06-20 Current Override

Use `docs/goals/migration.md`, `PROJECT_STATE.md`, `docs/features/sts1-events/status-board.md`, and `docs/features/ritsulib-migration/runtime-smoke-checklist.md` for current migration/runtime truth. Current beta.91 loader truth is RitsuLib-only on Slay the Spire 2 `v0.107.1`: `.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/` and `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/` reached main menu with exactly STS2-RitsuLib `v0.4.28` and Spire Plus `v0.1.0-private-beta.91`, no Spire Plus BaseLib dependency, 25/25 Spire Plus patches applied, clean audits, Off packet verifier 43 / 0, AdditiveBatch1 enabled-mode verifier 31 / 0, and AdditiveBatch1 packet verifier 61 / 0. Any beta.88/BaseLib-backed notes below are historical previous-dependency context only.

## 2026-06-19 Historical Override

Use `docs/goals/event.md`, `PROJECT_STATE.md`, `docs/features/sts1-events/status-board.md`, and `docs/features/ritsulib-migration/next-overnight-run.md` for historical StS1 event proof status. Beta.85/beta.86/beta.87 loader proof remains previous-package/game-version context, beta.88 remains previous BaseLib-backed context, and beta.91 is the current RitsuLib-only loader/registration proof. Any older retained-loader notes that treat beta.85, beta.87, or beta.88 as the next active runtime target are historical working notes only. Gameplay, clicked UI, save-load, replacement, multiplayer, independent QA, release handoff, and owner clean-worktree/commit decisions remain pending.

2026-06-19 pause-safe implementation note: Revision N now routes through `docs/goals/m5-revision-n-final-report.md`, `docs/goals/m5-revision-n-owner-commit-packet.md`, `docs/goals/m5-revision-n-validation-replay.md`, and `docs/goals/m5-revision-n-runtime-evidence-plan.md`. This prepares owner-ready planning under the active coordination pause only; it does not run or replace validation, runtime smoke, gameplay, staging, commit, push, release, or handoff gates.

---

## 1. 当前真实状态

当前 `PROJECT_STATE.md` 已经明确：M5 Revision M 的 root cause 是 Spire Plus runtime API drift，不是 BaseLib/RitsuLib 缺失；随后 beta.87 在 `v0.107.1` 暴露 BaseLib `v3.2.1` patch drift；beta.88 的 BaseLib `v3.3.0` / STS2-RitsuLib `v0.4.24` AdditiveBatch1 direct smoke 现在只是历史 BaseLib-backed loader/registration proof。当前 loader truth 是 beta.91 / `v0.107.1` / STS2-RitsuLib `v0.4.28` / RitsuLib-only；这些证据都不是 live gameplay 或 release readiness。

当前 build/test/package 记录也比之前强：beta.88 记录有 build 0 warnings / 0 errors、publish/package refresh、installed package parity、runtime preflight、retained beta.88 AdditiveBatch1 packet verification、current-doc claims、static suite、static-file hygiene 和 split no-build runtime-harness coverage。它们仍然不是 gameplay、clicked UI、save-load、route traversal、preview-tools、Vakuu、co-op、independent QA、clean-worktree proof。

Runtime 部分已经更新到 beta.91：`STS2-RitsuLib v0.4.28` / `lib\0.107.1` 已安装；当前 RitsuLib-only Off/AdditiveBatch1 proof 在 `.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/` 和 `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/`，只加载 RitsuLib/Spire Plus，25/25 ModPatcher patches 应用，10 event types / 14 registration calls，audit clean。先前 beta.84/beta.87/beta.88 证据现在只作为 root-cause 或 previous-context 证据。

`docs/goals/m5-revision-n-final-report.md` 也写明：Revision N 对 beta.88 current-loader governance 已经形成 owner-ready planning，但不是 live-ready 或 release-ready；gameplay、clicked UI、save-load、replacement、multiplayer、independent QA、release handoff proof 仍 pending。

---

## 2. 是否完成？

### 可以验收的部分

可以验收的是：

```text
M5 Revision M 的 Off-loader runtime-drift closure，以及 beta.88 current-loader/registration blocker closure
```

理由：

```text
1. beta.84 的 red smoke root cause 已定位到 stale API / Ectoplasm target。
2. beta.85 package 已刷新。
3. beta.85 v0.107.0 Off smoke clean。
4. beta.91 / v0.107.1 / RitsuLib v0.4.28 Off/AdditiveBatch1 loader/registration smoke clean。
5. Spire Plus 25/25 migrated patches 应用。
6. RitsuLib v0.4.28 / compat 0.107.1 加载成功。
7. EctoplasmGoldGatePatch exception 和 BaseLib v3.2.1 blocker 都已有 root-cause / closure 记录。
8. package checker / opt-in artifact subset / static validations 有记录。
```

### 不能验收的部分

不能验收为整体完成：

```text
1. current CanaryOnly recapture before canary gameplay claims 仍 pending。
2. gameplay / clicked UI / save-load / replacement / multiplayer 仍 pending。
3. independent QA 仍 pending。
4. clean worktree / owner commit decision 仍 pending。
5. release handoff 仍 pending。
6. Longhaul audit 仍不能恢复。
7. Batch 4c 仍 proposal-only。
```

`m5-revision-n-owner-commit-packet.md` 的 owner 决策也支持这个分层：beta.88 package 只建议 accept as loader/registration context, not gameplay/release proof；RitsuLib compile/manifest min version 已对齐 `0.4.24`，BaseLib 已对齐 `v3.3.0`；StS1 events 仍保持 staging-only；commit/push 在 active validation processes 存在时不要做。

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

### 3.2 Runtime loader / registration

当前已经通过：

```text
beta.88 v0.107.1 AdditiveBatch1 loader/registration packet：clean
```

这是本轮最大的有效完成项。

### 3.3 CanaryOnly / AdditiveBatch1

当前状态：

```text
current beta.91 RitsuLib-only AdditiveBatch1 loader/registration：clean
current CanaryOnly recapture before canary gameplay claim：pending
gameplay/render/save-load proof：pending
```

历史 v0.106.1 / beta.85 / beta.87 evidence 不能替代 beta.88 gameplay evidence。当前 validation 文档明确把这些较旧证据标成 historical 或 previous-package/game-version context。

### 3.4 RitsuLib version/package decision

当前状态：

```text
Runtime installed: STS2-RitsuLib v0.4.28
Compile package: STS2.RitsuLib 0.4.28
Manifest min_version: 0.4.28
BaseLib floor: none for current Spire Plus package
```

Owner packet 建议：不要再自动 bump；除非 owner 决定做 future versioned tester package，否则保持 beta.88 dependency floor。

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

当前不能直接 commit。`m5-revision-n-owner-commit-packet.md` 只是 planning，明确说 no commit or push authorized；并给出 beta.88 dependency/package docs、runtime-harness hardening、StS1 governance、Revision N governance docs 等候选 slice。commit rule 明确要求：在无 overlapping same-repo processes 的情况下 replay validation；如果 validation fail，则 split failing source slice 并报告 exact blocker。

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
2. beta.88 v0.107.1 loader/registration blocker 已关闭
3. docs 已经明确不是 live/release proof
4. RitsuLib/Debug/Sts1Events 状态比之前真实
5. commit slices 已规划但未自动 commit
```

未达成：

```text
1. current CanaryOnly recapture before canary gameplay claims
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
- current CanaryOnly recapture if process coordination allows
- gameplay/UI/save-load/co-op evidence
- owner package/dependency decision only if a new tester package is proposed
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
M5 Revision N: beta.88 Evidence Governance + Owner Commit Authorization
```

本节内容是历史 M5 Revision N spec；不要依赖 sandbox-only 下载链接作为当前证据。beta.85/beta.87 证据现在只是 previous-package/game-version context；beta.88 / `v0.107.1` / BaseLib `v3.3.0` / RitsuLib `v0.4.24` 的 AdditiveBatch1 loader/registration proof 也是 previous-dependency context。当前 clean-loader truth 是 beta.91 / `v0.107.1` / STS2-RitsuLib `v0.4.28` / RitsuLib-only Off 和 AdditiveBatch1 loader/registration proof。

### Week 1：Owner Commit Authorization + Validation Replay

目标：

```text
确认 beta.88 loader/registration packet 当前有效，并准备 owner-authorized commit slices。
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

### Week 2：beta.88 Runtime Evidence Expansion

只在没有 overlapping validation/runtime processes 时运行。当前 coordination pause 期间不运行：

```text
1. current CanaryOnly recapture before any canary gameplay claim
2. beta.91 gameplay/render/save-load evidence rows
3. Mod Settings current display screenshots and same-session log/audit
4. game-native AutoSlay/runtime-monkey packets only with current schema-bound proof
5. evidence folders recorded without turning loader proof into gameplay proof
```

必须保持：

```text
Sts1Events staging-only
no gameplay proof claim
no release-ready claim
```

### Week 3：Package / Dependency Decision

当前默认决策：

```text
A. 保持 beta.91 package line：STS2-RitsuLib v0.4.28，当前 Spire Plus 包不依赖 BaseLib。
B. 只有 owner 明确批准新的 versioned tester package 时，才再次调整 dependency floor 或 package version。
```

如果选择 B，必须做 version bump、publish/package、artifact tests、handoff/hash/website docs。

### Week 4：恢复 one-file longhaul audit

仅在这些条件满足后恢复：

```text
owner commit/rollback decision complete
static validation green
beta.88 loader/registration proof clean
current runtime/gameplay evidence plan complete or owner-deferred
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
RitsuLibRuntimeAgent 不能擅自 bump dependency/package versions。
```

---

## 7. Overnight run：必须跑到完成才能停

你可以直接发给他：

```text
进入 M5 Revision N overnight beta.88 evidence governance and owner-commit authorization run。

当前状态 NOT COMPLETE，但 beta.91 / v0.107.1 / STS2-RitsuLib v0.4.28 RitsuLib-only Off/AdditiveBatch1 loader/registration packets 已 clean。不要继续 PR6 Batch4c、Batch5、PR7、debug expansion、Sts1Events formalization 或 longhaul audit。

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
- beta.88 loader/registration proof remains documented as clean
- runtime/gameplay evidence plan is complete, or current smokes/manual rows are run cleanly if process coordination allows
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
docs/goals/m5-revision-n-final-report.md
docs/goals/m5-revision-n-owner-commit-packet.md
docs/goals/m5-revision-n-validation-replay.md
docs/goals/m5-revision-n-runtime-evidence-plan.md
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

这次状态已经从“v0.107.0 runtime drift 红灯”和“beta.88 BaseLib-backed loader proof”推进到“beta.91 / v0.107.1 RitsuLib-only clean loader-registration 绿灯”。这是实质完成项；但仍不是 release/live 完成。下一步不是继续迁移，而是补 gameplay/UI/save-load/co-op/QA/handoff evidence、做 owner commit authorization，然后再考虑恢复 one-file longhaul audit。
