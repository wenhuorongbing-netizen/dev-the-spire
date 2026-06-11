## 2026-06-11 Revision M continuation update

The earlier conclusion below is now historical root-cause context. The beta.84 `v0.107.0` Off smoke was red because of Spire Plus API drift, but the beta.85 runtime-fix package now has clean Off-loader proof under `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`: Spire Plus `v0.1.0-private-beta.85`, RitsuLib `0.4.16` compat branch `0.107.0`, 25/25 Spire Plus ModPatcher patches applied, StS1Events default Off, main menu reached, and clean audit. Do not extend that loader proof to live-ready or release-ready; gameplay, clicked UI, save-load, co-op, independent QA, and non-Off StS1 runtime proof remain pending.

## 严格审核结论

我按**当前 GitHub main 上的最新 PROJECT_STATE / validation docs**重新审了一遍：之前“`STS2-RitsuLib` 目录缺失”的 blocker 已经过时。当前状态已经进入下一层 blocker：

```text
当前不是“RitsuLib 未安装”；
当前是“RitsuLib v0.4.16 已安装，beta.85 在 v0.107.0 Off 模式下 clean loader proof 已经通过；但 live/manual、CanaryOnly/AdditiveBatch1、replacement、multiplayer、QA 和 release proof 仍未完成”。
```

所以这次必须更新判断：

```text
Static validation：当前记录为绿
Off loader proof：beta.85 绿
Runtime/live proof：未完成
Release-ready：否
Runtime-ready/live-ready：否
Commit/owner-review：仍需收口
Longhaul audit：不能恢复为完成态
下一步：在不扩大 release claim 的前提下，继续收口非 Off runtime、manual/live、owner-review 和 handoff
```

我已把新的 M5 Revision M runtime-drift 判断写入 repo 文件：
`docs/goals/m5-revision-m-runtime-drift-report.md`、`docs/goals/m5-revision-m-patch-failure-ledger.md`、`docs/goals/m5-revision-m-owner-review-packet.md`、`docs/goals/m5-revision-m-commit-slices.md`。

---

## 1. 当前真实状态

最新 `PROJECT_STATE.md` 记录：当前 pushed baseline before local runtime-fix pass 是 `bdb51c39 ... sprint7`；本地安装的游戏已经是 **Slay the Spire 2 v0.107.0**，官方 `STS2-RitsuLib v0.4.16` 已经安装在 E 盘 game root，并带有 `lib\0.107.0`；beta.85 package parity 在 2026-06-11 通过 package checker。

当前静态验证方面，`PROJECT_STATE.md` 记录 beta.85 runtime-fix validation 已经是 **0 build errors / 0 warnings**，split no-build `dotnet test` lanes 是 `475 passed / 0 failed / 21 skipped / 496 total`。后续 dirty changes 仍需在同 repo test/build overlap 清理后按 touched surface 复验；本线程不能把暂停期间的状态写成本线程已重跑。

Off loader/runtime-drift 部分已经收口：当前 `v0.107.0` beta.85 Off smoke 进了 main menu，BaseLib 和 RitsuLib 都加载，RitsuLib 选择 compat branch `0.107.0`，Spire Plus 应用 **25/25 ModPatcher patches**，StS1Events default Off，并且 audit clean。beta.84 的 17/25 patch + `EctoplasmGoldGatePatch` initializer exception 只保留为 root-cause history。

`docs/integrations/ritsulib.md` 也确认：当前 compile package 仍是 `STS2.RitsuLib 0.3.2`，manifest min_version 也仍是 `0.3.2`；当前 runtime 装的是 `STS2-RitsuLib v0.4.16`，当前游戏是 `v0.107.0`，beta.85 Off smoke 可作为 startup/default-Off loader proof，但不能扩展为 gameplay、CanaryOnly/AdditiveBatch1、replacement、multiplayer、QA 或 release proof。

---

## 2. 与我们的目标对比

我们的目标一直是：

```text
1. 静态验证真实全绿
2. runtime/live evidence 不能被静态验证替代
3. RitsuLib / Sts1Events / Debug 状态必须真实
4. 未经 owner 授权不 commit
5. 只有 Off loader proof 时不恢复 longhaul audit 为完成态
6. 不能把 source-level patch migration 当成 release readiness
```

当前达成：

```text
静态验证：达成，当前 docs 记录 build/test/format/diff/patch/batch 绿
RitsuLib install：达成，v0.4.16 installed with lib\0.107.0
Main menu reach：达成，v0.107.0 Off smoke reached main menu
Clean Off loader proof：达成，beta.85 25/25 patches，clean audit
Docs truth：当前 docs 明确 Off loader proof 与 live/release proof 的边界
```

当前未达成：

```text
CanaryOnly / AdditiveBatch1 current runtime proof：未达成
Replacement functional proof：未达成
Current tester-package live/manual proof：未达成
gameplay/UI/save/co-op：未达成
release-ready：未达成
longhaul audit completion recovery：未达成
```

结论：

```text
现在不是 rollback 状态，但也不是完成状态。
应该继续：优化 + 有限推进。
优先级：保持 beta.85 Off loader proof 边界清楚，再推进 CanaryOnly / AdditiveBatch1 / gameplay / package decision。
```

---

## 3. 逐步严格检查

### 3.1 Static validation

当前记录：

```text
dotnet build EZMicroBalance.sln -m:1 --no-incremental: PASS, 0 warnings, 0 errors
split no-build test lanes: PASS, 475 / 0 / 21 / 496
dotnet format: PASS
generate-patch-inventory.ps1 -Check: PASS
report-worktree-batches.ps1 -FailOnUnclassified: PASS
git diff --check: PASS, only CRLF normalization warnings
```

这些可以接受为：

```text
static validation green
```

但不能扩展成：

```text
runtime ready
release ready
```

项目自己的 test-ready goal 也明确说：这些命令不能作为 live-game、save-load、death/failure、co-op evidence。

---

### 3.2 Runtime smoke

当前 Off loader smoke 是绿的，不是缺 RitsuLib 目录，也不是 beta.84 的 stale patch target blocker：

```text
RitsuLib exists
v0.107.0 branch selected
main menu reached
25/25 Spire Plus ModPatcher patches applied
StS1Events default Off
godot-log audit clean
```

所以当前 blocker 应该改写为：

```text
non-Off runtime proof + live/manual/release evidence blocker
```

不是旧的：

```text
STS2-RitsuLib path missing
```

---

### 3.3 RitsuLib status

当前正确状态：

```text
compile/manifest dependency active
runtime installed
historical v0.106.1 loader proof exists
current beta.85 v0.107.0 Off loader proof is clean
current CanaryOnly/AdditiveBatch1/gameplay proof is pending
```

不能写：

```text
RitsuLib migration complete
RitsuLib runtime verified
hard dependency release-ready
```

`docs/integrations/ritsulib.md` 明确说：Batch 4a/4b 合计 25 个 patch classes 已迁移，并有 historical v0.106.1 proof；Batch 4c 只是 proposal-only；Batch 5 high-risk 仍 blocked。

---

### 3.4 Version decision

当前 compile package 仍是：

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All" />
```

manifest 仍是：

```json
{ "id": "STS2-RitsuLib", "min_version": "0.3.2" }
```

但 installed runtime 是 `0.4.16`，NuGet 也有 `STS2.RitsuLib 0.4.16`，而 `Compat.0.107.0` 没有单独发布。文档建议：未来 owner-approved v0.107.0 tester package 应该在同一个 package-version increment 里把 compile package 和 manifest minimum 都移到 `0.4.16`。

所以现在不要马上 bump，除非你决定做新的 versioned tester package。

---

### 3.5 EctoplasmGoldGatePatch

当前 Ectoplasm blocker 状态是：

```text
beta.84 root-cause history: EctoplasmGoldGatePatch initializer exception from stale package API targets
beta.85 current Off proof: exception gone, 25/25 patches applied
```

相关文件仍应作为 future regression guard/reference：

```text
EZMicroBalanceCode/Ancients/Patches/PickupRewardGatePatches.cs
EZMicroBalanceCode/Ancients/Patches/PickupRewardService.cs
tests/EZMicroBalance.Tests/AncientHighRiskSourceGuardTests.cs
```



下一步不能把 Batch 4c 当作已经批准，而是应该：

```text
1. 保持 beta.85 Off proof 与 package parity 记录一致
2. 对 CanaryOnly / AdditiveBatch1 做 owner-approved current proof plan
3. 继续记录 gameplay/UI/save/co-op/manual proof blockers
4. 任何新的 patch/package change 都要重新跑 static validation 和需要的 loader proof
```

---

## 4. 是否完成？

### 可以承认完成的部分

```text
1. RitsuLib runtime 已经安装，不再是 missing-directory blocker。
2. 当前静态 validation 记录为全绿。
3. Package parity restore 已做过，checker passed。
4. Historical v0.106.1 Off/CanaryOnly/AdditiveBatch1 loader evidence 存在。
5. Docs 当前已经明确 beta.85 v0.107.0 Off loader proof clean，且 live/non-Off/release proof 仍 pending。
```

### 不能承认完成的部分

```text
1. CanaryOnly / AdditiveBatch1 current proof 未完成。
2. Replacement functional proof 未完成。
3. gameplay / UI / save-load / co-op 均未完成。
4. 本线程不能在 same-repo test/build overlap pause 期间重跑 validation；pause 解除后，后续 code/resource/package changes 仍需按 touched surface 复验。
5. release-ready / runtime-ready 不能从 Off loader proof 单独宣称。
```

最终判断：

```text
整体未完成。
Off-loader drift closure 已完成到 beta.85 proof；当前正确任务不是继续 PR6 Batch4c，而是收口 non-Off runtime、manual/live、owner-review、handoff 和验证重放。
```

---

## 5. 策略：优化、推进，还是两者兼顾？

我的决定：

```text
两者兼顾，但 optimization-first。
```

比例：

```text
75% 优化/收口：
- 保持 beta.85 v0.107.0 Off loader proof / package parity / docs truth 一致
- 把 beta.84 EctoplasmGoldGatePatch 和 8 failures 保留为 root-cause history
- 明确 CanaryOnly / AdditiveBatch1 需要 current proof
- 明确 gameplay/UI/save/co-op 仍 pending
- 更新 docs truth

25% 有限推进：
- 准备 CanaryOnly / AdditiveBatch1 rerun
- 准备 RitsuLib 0.4.16 package upgrade decision
- 准备 owner-review packet
```

禁止：

```text
PR6 Batch4c implementation
Batch5
PR7
high-risk patch migration
Sts1Events formalization
debug expansion
longhaul audit
release-ready claim
```

---

## 6. 下个月开发规范：M5 Revision M

新目标：

```text
v0.107.0 runtime drift closure + owner-review + runtime truth。
```

Repo 文件：
`docs/goals/m5-revision-m-runtime-drift-report.md`
`docs/goals/m5-revision-m-patch-failure-ledger.md`
`docs/goals/m5-revision-m-owner-review-packet.md`
`docs/goals/m5-revision-m-commit-slices.md`
`docs/goals/m5-revision-m-version-decision.md`
`docs/goals/m5-revision-m-final-report.md`

### Week 1：v0.107.0 runtime drift closure

当前状态：

```text
1. beta.84 evidence 已作为 root-cause history 保留
2. beta.85 Off smoke 已 clean
3. 25/25 migrated patches 已在 beta.85 Off proof 中应用
4. EctoplasmGoldGatePatch stale target blocker 已在 Off loader path 关闭
5. 本线程未新跑 dotnet validation，因为同 repo validation lane 仍需避免重叠
```

剩余完成条件：

```text
A. Current CanaryOnly / AdditiveBatch1 proof if owner wants StS1 enabled-mode runtime closure
B. Gameplay/UI/save/co-op/manual proof if owner wants test-ready/live closure
C. Fresh validation replay after overlapping test/build processes are cleared and owner coordination allows it
```

### Week 2：CanaryOnly / AdditiveBatch1 rerun

只在 Off clean 后做：

```text
CanaryOnly runtime proof
AdditiveBatch1 runtime proof
Sts1Events staging-only proof
```

### Week 3：RitsuLib version/package decision

Owner 决策：

```text
继续 beta.85 compile/manifest 0.3.2 + runtime 0.4.16 外部安装
或
新版本 package bump：compile package 和 manifest min_version 都升到 0.4.16
```

如果 bump，就必须 version increment + publish/package + release artifact tests + handoff/hash/docs。

### Week 4：恢复 longhaul audit

只有在 clean current Off proof 边界清楚、且 owner 明确 non-Off/live proof 处理方式后，才恢复为完成态。

第一批建议文件：

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

## 7. 必须使用 subagent

下一轮必须使用 subagent，尤其是 runtime drift 不能让主 agent 一边猜一边改。

必用：

```text
CurrentStateAgent
RuntimeDriftAgent
PatchFailureAgent
EctoplasmPatchAgent
RitsuLibVersionAgent
PackageParityAgent
RuntimeSmokeAgent
Sts1EventsGovernanceAgent
DebugDecisionAgent
DocsTruthAgent
ValidationReplayAgent
CommitSliceAgent
LonghaulRecoveryAgent
```

特别强调：

```text
EctoplasmPatchAgent 的 beta.84 blocker 已关闭到 beta.85 Off proof；未来改动仍必须对照 v0.107.0 source / installed DLL API。
PatchFailureAgent 的 8 failures 是 beta.84 root-cause history；当前 beta.85 Off proof 是 25/25。
RuntimeSmokeAgent 可以把 Off clean 作为进入 CanaryOnly planning 的前置条件，但不能把 Off clean 当成 CanaryOnly proof。
RitsuLibVersionAgent 不能擅自 bump 0.4.16，必须 owner 批准 package-version increment。
```

---

## 8. Overnight run：必须跑到完成才能停

你可以直接发给他：

```text
进入 M5 Revision M overnight v0.107.0 runtime-drift closure and owner-review run。

当前状态当时 NOT COMPLETE；2026-06-11 beta.85 Off loader proof has since closed the Off-loader drift portion, while live/release proof remains pending.

当前最新事实：
- Latest pushed baseline before local runtime-fix pass: HEAD bdb51c39 sprint7。
- Current local game: Slay the Spire 2 v0.107.0。
- Installed RitsuLib: official STS2-RitsuLib v0.4.16 with lib\0.107.0。
- Compile package still STS2.RitsuLib 0.3.2。
- Manifest dependency still STS2-RitsuLib min_version 0.3.2。
- Latest recorded beta.85 runtime-fix validation: build 0 errors / 0 warnings and split no-build test lanes 475 passed / 0 failed / 21 skipped / 496 total; later dirty changes still need replay when same-repo validation pause is lifted。
- Current v0.107.0 beta.85 Off smoke is clean: main menu reached, RitsuLib loaded compat branch 0.107.0, Spire Plus applied 25/25 ModPatcher patches, StS1Events default Off, clean audit。
- Current CanaryOnly / AdditiveBatch1 / gameplay / save-load / replacement / co-op / QA proof remains pending。
- Release-ready: no。
- Runtime-ready/live-ready: no。
- Batch 4c: proposal-only。

禁止：
- commit
- push
- stash / stash drop
- checkout branch
- reset / restore
- broad clean
- continue PR6 Batch4c, Batch5, or PR7
- expand debug
- formalize Sts1Events
- resume longhaul audit
- claim runtime verified or release-ready from Off loader proof alone

你不能停止，直到满足以下之一：

A. Clean current v0.107.0 Off runtime packet complete:
- static validation commands exit 0
- installed package parity check passes
- Off smoke reaches main menu
- BaseLib + STS2-RitsuLib + Spire Plus load
- RitsuLib selects compat branch 0.107.0
- expected migrated patches apply or optional skips are explicitly non-blocking and documented
- EctoplasmGoldGatePatch initializer exception is gone
- godot-log audit has 0 blocking hits
- runtime evidence folder recorded
- docs and harness truth updated
- no unauthorized commit

B. Hard blocker:
- exact failing patch/command/log line
- source evidence
- why current worktree cannot resolve it
- rollback/demotion/staging options
- exact owner decision required

必须先读：
AGENTS.md
PROJECT_STATE.md
docs/README.md
docs/test-ready-development-goal.md
docs/reviews/current-validation.md
docs/integrations/ritsulib.md
docs/patch-inventory.md
docs/worktree-cleanup-audit.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

必须先使用 subagents：
1. CurrentStateAgent
2. RuntimeDriftAgent
3. PatchFailureAgent
4. EctoplasmPatchAgent
5. RitsuLibVersionAgent
6. PackageParityAgent
7. RuntimeSmokeAgent
8. Sts1EventsGovernanceAgent
9. DebugDecisionAgent
10. DocsTruthAgent
11. ValidationReplayAgent
12. CommitSliceAgent
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
docs/goals/m5-revision-m-runtime-drift-report.md
docs/goals/m5-revision-m-patch-failure-ledger.md
docs/goals/m5-revision-m-owner-review-packet.md
docs/goals/m5-revision-m-commit-slices.md
docs/reviews/current-validation.md
docs/integrations/ritsulib.md
PROJECT_STATE.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

最终报告只能写两种之一：
Complete for Off-loader slice: clean current v0.107.0 Off runtime packet complete.
Not complete overall: non-Off/live/handoff proof remains pending or exact hard blocker encountered.

不要因为 static build/tests pass 就写 complete。
不要从 Off loader proof 单独写 runtime-ready。
不要把 CanaryOnly/AdditiveBatch1 planning 当成 proof。
不要没有 publish/package/runtime/handoff evidence 就写 release-ready。
```

一句话总结：**他已经过了“RitsuLib 没安装”和 beta.84 Off-loader drift 阶段；beta.85 已拿到 clean Off proof。下一步不是继续迁移，而是守住 proof 边界，收口 CanaryOnly/AdditiveBatch1、gameplay/UI/save-load/co-op、QA 和 handoff。**
