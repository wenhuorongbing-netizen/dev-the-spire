# goal-implementation-prompt.md — 严格执行版 Goal Prompt

> 用途：把上一轮只写文档的“完成 goal.md”纠偏为真正的实现推进。
> 适用仓库：`D:\Game\FOTN\dev-the-spire` / `dev-the-spire`。
> 当前日期基线：2026-05-20。
> 目标：落实 `docs/goal.md`，而不是再次写一批说明文档。
> 最高原则：**不能把“写了计划/规格/矩阵/审查结论”当作实现完成。**

---

## 0. 这次任务的第一句话必须承认的事实

上一轮没有完成 `docs/goal.md`。

它只完成了“治理基线和文档化的一部分”，没有完成 goal 的核心交付：

- 没有启动游戏。
- 没有 fresh current-package loader smoke。
- 没有 clicked Ancient UI proof。
- 没有 Ancient gameplay matrix。
- 没有 save/load proof。
- 没有 Vakuu victory/no-black-screen/failure/death proof。
- 没有 co-op two-client proof。
- 没有 Future Peek live proof。
- 没有重新 publish/package。
- 没有 release evidence manifest 通过。
- 没有关闭 `GOV-WIP-SPLIT`。
- 没有把 release traceability matrix 中的 pending live rows 变成 passed evidence rows。

所以这次任务的目标不是“继续整理 docs”，而是：

> **把 `docs/goal.md` 从规划推进到实现、验证、联机、证据和发布判断。**

---

## 1. 非协商约束

### 1.1 不能只改文档

这次 pass 如果只修改下面这些内容，必须判定为失败：

```text
docs/**/*.md
docs/specs/**/*.md
docs/architecture/**/*.md
docs/month-plan/**/*.md
docs/review.md
docs/issues.md
docs/README.md
```

文档可以改，但只能作为实现、测试、脚本、证据的附属产物。

本次 pass 必须至少完成下面三类之一的实质变化：

```text
A. 源码实现变化
   EZMicroBalanceCode/**/*.cs
   EZFuturePeekCode/**/*.cs

B. 自动化或证据脚本变化
   scripts/**/*.ps1
   .github/workflows/**/*.yml

C. 测试变化，并且测试约束真实代码/证据，不是只检查文档存在
   tests/**/*.cs
```

最低可接受组合：

```text
源码实现 + guard tests + validation
```

或者：

```text
运行证据脚本 + live evidence folder + release evidence row update
```

如果无法启动游戏，也必须做源码、测试、脚本层面的真实推进，并明确说 live rows 仍 pending。不能写“goal 已完成”。

### 1.2 不能把 source review 当 live proof

禁止使用这些句式关闭任何 runtime row：

```text
source-backed
source-complete
guarded
build passed
tests passed
no blocker known
should work
not observed in source
```

这些只能说明“源码层没有已知阻塞”，不能说明游戏里通过。

### 1.3 不能把联机入口当联机支持

`StartRunLobby` 能选 A11-A20 不等于 co-op gameplay 支持。

只有满足下面条件，才能声称“联机支持”：

```text
two-client host/join proof
+ both clients clean logs
+ no desync
+ relevant state visible/consistent
+ save/load or reconnect behavior明确
+ release evidence row passed
```

否则必须写：

```text
co-op pending
或
co-op unsupported
或
feature gated in multiplayer
```

### 1.4 不能把 Future Peek 标为无 gameplay 影响而不做产品决策

Future Peek 虽然不改 RNG / reward / card state，但它改变玩家决策信息。

本次必须完成二选一：

```text
A. 把 EZFuturePeek.json affects_gameplay 改为 true，并更新测试和说明；
```

或者：

```text
B. 写出明确产品决策：为什么 preview-only 可以保持 false，并说明联机公平性风险。
   这个决策必须进入 manifest/docs/tests，并不得宣传为公平联机功能。
```

默认建议：**改为 `affects_gameplay: true`**，因为它能让玩家提前知道水晶球和变换结果。

### 1.5 不得标记 `docs/goal.md` 完成

`docs/goal.md` 是一个 30 天计划，不是一次无启动游戏 pass 可以完成的任务。

本次只能完成：

```text
goal.md implementation pass N
```

不能写：

```text
goal.md completed
goal 已标记完成
release-ready
full co-op ready
feature complete
```

---

## 2. 本次 pass 的真正目标

本次 pass 目标是完成 **Goal Implementation Pass 1**：

```text
1. 审计上一轮 docs-only 结果，并把“未完成”写进工程治理。
2. 加入防止以后 docs-only 冒充完成的自动化 guard。
3. 做至少一个真实源码实现推进。
4. 做至少一个真实 runtime-evidence 脚本推进。
5. 对 Future Peek、co-op、save/load、Vakuu、Root Eyes 等高风险项做可执行实现推进。
6. 跑完整 no-game 验证。
7. 如果具备本地游戏启动条件，执行 fresh loader smoke；否则明确留下 live rows pending。
```

---

## 3. Pass 1 必须完成的交付物

### 3.1 `goal.md` Completion Guard

新增或扩展测试，防止 future agent 再次把 pending goal 标记完成。

建议文件：

```text
tests/EZMicroBalance.Tests/GoalCompletionGuardTests.cs
```

测试要求：

1. 如果 `docs/goal.md` 或 `docs/review.md` 出现以下 claim，则必须失败，除非 release evidence verifier 已通过并有 final release review：

```text
goal completed
goal 已完成
release-ready
full multiplayer support
feature complete
fully implemented
```

2. 如果 `docs/specs/release-traceability-matrix.md` 仍存在：

```text
pending
Manual-test candidate
Development-test surface
Hidden by default
Do not advertise full support
```

则 `docs/review.md` / `docs/issues.md` 不能声称 release-ready。

3. `docs/goal.md` 必须保留“live proof required / source review 不关闭 runtime rows”的规则。

4. `docs/issues.md` 中如果仍有 `Manual Proof Gates`，不得出现“当前 release-ready”。

验收：

```powershell
dotnet test EZMicroBalance.sln --filter FullyQualifiedName~GoalCompletionGuardTests
```

### 3.2 Release Evidence Enforcement Guard

新增或扩展测试，确保 release evidence 不是可选摆设。

建议文件：

```text
tests/EZMicroBalance.Tests/ReleaseEvidenceGateTests.cs
```

测试要求：

- 默认测试不要求 live evidence 已存在，但必须保证：
  - pending rows 没被错误关闭；
  - historical 16/22-field loader logs 不能算 current 25-field loader proof；
  - release evidence dashboard 中 runtime rows 仍 pending 时，release checklist 不能 claim publish-proven。
- 当设置环境变量时，严格要求证据：

```powershell
$env:EZMB_ENFORCE_RELEASE_READY='1'
dotnet test EZMicroBalance.sln --filter FullyQualifiedName~ReleaseEvidenceGateTests
Remove-Item Env:\EZMB_ENFORCE_RELEASE_READY
```

严格模式必须检查：

```text
fresh loader smoke evidence
clicked Ancient UI evidence
save/load evidence
Vakuu evidence
co-op evidence
Future Peek evidence if shipping Future Peek
verify-spire-plus-release-evidence.ps1 pass marker
```

如果证据不存在，严格模式应该失败，并明确告诉开发者缺哪一项。

### 3.3 Runtime Evidence Logging 实现

不要只写“需要证据”。要让游戏运行时更容易收集证据。

新增一个轻量日志服务：

```text
EZMicroBalanceCode/Diagnostics/ReleaseEvidenceLog.cs
```

职责：

- 只在环境变量开启时输出详细 release-evidence 日志。
- 默认安静，不污染普通玩家日志。
- 输出格式固定，方便 grep / audit：

```text
[EZMB-EVIDENCE] <Feature> <Event> run=<run-id?> player=<player?> net=<single/host/client?> data=<json-ish>
```

环境变量：

```text
EZMB_RELEASE_EVIDENCE_LOG=1
```

最低必须接入这些关键路径：

#### Urda Root Eyes

记录：

```text
selection opened
node selected
preview generated
preview saved
preview refunded
node entered
preview consumed
save hydrate marker restored
```

#### Urda Seed Bank

记录：

```text
card stored
relic hover count
extract opened
cards selected
deck add success/failure
storage cleared
save hydrate storage restored
```

#### Vakuu fight

记录：

```text
fight option shown
fight started
child combat room entered
combat room serialized
prefinished restore
victory rewards suppressed
parent event resume attempted
parent event resume success
fallback map exit
failure/death path
```

#### Lotha Death Reprieve

记录：

```text
pending created
active entered
resolved
save hydrate
lethal prevented
state cleared
```

#### Morvi state

记录：

```text
debt created
debt paid
debt unpaid fallback
open book restore
blueprint proof initialized
overdue library page created
```

#### Rootblight

记录：

```text
rootblight added
deck cap enforced
combat-end notice queued
sprout buried
save hydrate
```

验收：

- 测试检查这些 log marker 常量存在。
- `docs/release-evidence-status.md` 增加“how to enable release evidence logs”，但不要把 row 标 passed。

### 3.4 Runtime Evidence Collection Scripts

不要只写 manual checklist。新增或强化脚本，能把 evidence folder 结构生成出来。

建议新增：

```text
scripts/collect-release-evidence.ps1
scripts/collect-future-peek-evidence.ps1
scripts/collect-coop-evidence.ps1
```

#### `collect-release-evidence.ps1`

功能：

- 创建 evidence root：

```text
.tools/runtime-evidence/release-evidence-YYYYMMDD-HHMMSS/
```

- 写入：

```text
command.txt
environment.json
package-hashes.json
enabled-mods-template.txt
manual-rows-template.json
```

- 如果传入 `-Launch`，调用现有 live session helper 启动游戏。
- 如果不传 `-Launch`，只生成 evidence plan，不声称通过。

#### `collect-future-peek-evidence.ps1`

功能：

- 创建 Future Peek evidence folder。
- 写入手动步骤：
  - Crystal Sphere toggle；
  - no charges spent；
  - no reward；
  - transform preview single/multi/combat/non-combat；
  - cancel/reopen no RNG advance。
- 收集 package hash / manifest / log audit template。

#### `collect-coop-evidence.ps1`

功能：

- 创建 two-client evidence folder 模板。
- 要求 host/client 两份：
  - command.txt；
  - godot.log；
  - audit json；
  - screenshots；
  - result notes。
- 明确列出 A11-A20、Ancients、Root Eyes、Rootblight、save/load rows。

验收：

```powershell
.\scripts\collect-release-evidence.ps1 -NoLaunch
.\scripts\collect-future-peek-evidence.ps1 -NoLaunch
.\scripts\collect-coop-evidence.ps1 -NoLaunch
```

脚本必须能在不启动游戏时生成模板，但不能标记 row passed。

### 3.5 Future Peek 真实推进

必须完成以下至少两项，不能只写 spec。

#### 3.5.1 Manifest 决策

默认执行：

```text
EZFuturePeek.json: "affects_gameplay": true
```

同时更新：

```text
tests/EZFuturePeek.Tests/FuturePeekGuardTests.cs
docs/features/future-peek/README.md
```

如果选择不改为 true，必须写出明确 release decision，并加测试保证文案包含“information advantage / multiplayer fairness risk / preview-only does not change state”。

#### 3.5.2 Transform Preview Source Hardening

审核并改进：

```text
EZFuturePeekCode/Patches/TransformPredictionRngContext.cs
EZFuturePeekCode/Patches/TransformPreviewPatch.cs
EZFuturePeekCode/Prediction/TransformPredictionService.cs
```

必须检查：

- multi-card transform 顺序；
- transformation.Replacement != null 的队列对齐；
- stale RNG snapshot cleanup；
- selection task exception cleanup；
- combat vs non-combat transform context；
- Astrolabe upgraded preview；
- no real card creation；
- no real RNG advance。

新增测试：

```text
TransformPredictionQueueOrderIsGuarded
TransformPredictionContextCannotBeReusedAfterSelection
FuturePeekManifestDeclaresGameplayImpactOrFairnessDecision
```

#### 3.5.3 Crystal Sphere UI Hardening

审核并改进：

```text
EZFuturePeekCode/Patches/CrystalSpherePeekPatch.cs
```

必须检查：

- button duplicate 防护；
- OnMinigameFinished 恢复 mask alpha；
- screen reentry；
- language fallback；
- no ClearCell / RevealItem / CellClicked / AddReward；
- no divination count mutation；
- no item reveal event。

新增测试检查这些 guard。

### 3.6 Co-op Contract 真实推进

新增一个中心化 co-op policy/helper，而不是在文档里说 pending。

建议文件：

```text
EZMicroBalanceCode/Multiplayer/MultiplayerFeaturePolicy.cs
```

或在现有 Ascension/Core 下建立：

```text
EZMicroBalanceCode/Ascension/Core/MultiplayerFeaturePolicy.cs
```

职责：

- 判断当前 run/lobby 是 singleplayer / host / client。
- 提供统一方法：

```csharp
IsSingleplayer(...)
IsHost(...)
IsClient(...)
CanMutateSharedRunState(...)
ShouldDisableUnverifiedCoopFeature(...)
LogCoopEvidence(...)
```

必须先应用到至少两个高风险 surface：

```text
A. A20 co-op / King Brand / second boss
B. Urda Root Eyes shared map mutation
```

策略：

- 如果能实现 host-authoritative mutation，就实现并记录 evidence log。
- 如果不能保证同步，则在 multiplayer 下 gate 掉或降级，并且 release notes 不能声称支持。
- 不能保持“看起来能选，但实际 gameplay downgraded 却宣传 full co-op”。

新增测试：

```text
A20CoopCannotBeAdvertisedWhileDowngraded
RootEyesSharedMapMutationRequiresHostAuthority
MultiplayerPolicyHasEvidenceLogging
```

### 3.7 Vakuu Fight 实现推进

不是只写 `vakuu-fight-spec.md`。必须触碰 source 或 tests。

目标文件候选：

```text
EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightPatch.cs
EZMicroBalanceCode/Ancients/Expansion/Vakuu/VakuuFightService*.cs
tests/EZMicroBalance.Tests/Vakuu*.cs
```

必须做：

- 增加 `ReleaseEvidenceLog` markers。
- 强化 source guard：
  - active child combat 不存 `ParentEventId`；
  - prefinished restore 才能记录 parent；
  - victory no-reward path 不重复 Ancient heal；
  - fallback map exit 有 log；
  - failure/death path 有明确状态清理；
  - co-op 未证明时 fight 不自动公开。
- 如果发现实际代码没有 failure/death cleanup，补实现。

### 3.8 Save/Load Contract 实现推进

新增或扩展 source guards，要求每个 stateful feature 有：

```text
save field or deck mirror
hydrate path
clear path
log marker
manual test row
```

目标 features：

```text
Root Eyes
Seed Bank
Morvi Debt/OpenBook/BlueprintProof
Lotha Death Reprieve
Vakuu child combat
Rootblight
```

建议新增测试：

```text
SaveStateContractsGuardTests.cs
```

测试不是只检查 docs，而要检查源码中存在：

```text
SavedSpireField
FromSerializable / ToSerializable / hydrate / restore
AfterRunLoaded / AfterCombatStarted / AfterRoomEntered 等恢复路径
ReleaseEvidenceLog marker
state clear/reset path
```

---

## 4. Subagent 使用要求

你必须使用 subagent，但 subagent 不能只输出审查结论。每个 subagent 必须绑定一个实物交付。

### 4.1 Completion Auditor

任务：

- 对照 `docs/goal.md` 和上一轮报告。
- 写出“不完成”的审计结论。
- 新增 `GoalCompletionGuardTests`。

交付：

```text
tests/EZMicroBalance.Tests/GoalCompletionGuardTests.cs
docs/review.md 更新：上一轮 docs-only 不算完成
```

### 4.2 Source Implementation Agent

任务：

- 实现 `ReleaseEvidenceLog`。
- 接入 Root Eyes、Seed Bank、Vakuu、Lotha/Morvi/Rootblight 至少三个 surface。

交付：

```text
EZMicroBalanceCode/Diagnostics/ReleaseEvidenceLog.cs
相关 feature source 修改
相关 guard tests
```

### 4.3 Future Peek Agent

任务：

- 完成 Future Peek `affects_gameplay` 决策。
- 强化 Crystal Sphere / Transform Preview。
- 更新 tests。

交付：

```text
EZFuturePeek.json
EZFuturePeekCode/**/*.cs
tests/EZFuturePeek.Tests/*.cs
```

### 4.4 Multiplayer Agent

任务：

- 建立 multiplayer policy。
- 应用到 A20 co-op 和 Root Eyes。
- 确保未证明 co-op 的 feature 不被宣传 full support。

交付：

```text
EZMicroBalanceCode/**/MultiplayerFeaturePolicy.cs
Ascension / Urda 相关修改
co-op guard tests
```

### 4.5 Save/Load Agent

任务：

- 建立 `SaveStateContractsGuardTests`。
- 加强 hydrate/reset/log paths。
- 明确 runtime evidence markers。

交付：

```text
tests/EZMicroBalance.Tests/SaveStateContractsGuardTests.cs
相关 source 修改
```

### 4.6 Release Evidence Engineer

任务：

- 新增 evidence collection scripts。
- 严格 release verifier 模式。
- 不允许模板被当作 passed evidence。

交付：

```text
scripts/collect-release-evidence.ps1
scripts/collect-future-peek-evidence.ps1
scripts/collect-coop-evidence.ps1
tests/EZMicroBalance.Tests/ReleaseEvidenceGateTests.cs
```

### 4.7 Red-Team Reviewer

任务：

- 攻击本 pass 的实现。
- 查找 docs-only、source-only、co-op false claim、Future Peek fairness、save/load transient-only 问题。

交付：

```text
docs/reviews/red-team-goal-implementation-pass-1.md
```

注意：red-team 文档不能作为唯一交付。它只是审查附属物。

---

## 5. 执行顺序

严格按顺序执行。

### Step 0 — 工作树和基线

运行：

```powershell
git status --short --branch
git log -1 --oneline --decorate
```

记录当前状态，但不要把它当完成。

### Step 1 — 审计上一轮“完成”声明

输出结论：

```text
Result: NOT COMPLETE
Reason: docs-only / governance-only pass did not satisfy goal.md implementation/live/co-op/evidence criteria.
```

更新 `docs/review.md` 或新增 review note。

### Step 2 — 防止重复偷懒

先写 guard tests：

```text
GoalCompletionGuardTests
ReleaseEvidenceGateTests
```

这些测试应该能阻止以后再用 docs-only 标完成。

### Step 3 — 做真实源码实现

至少完成三项：

```text
ReleaseEvidenceLog 接入
Future Peek manifest/gameplay impact 决策
MultiplayerFeaturePolicy 接入
Vakuu evidence/failure path hardening
SaveStateContractsGuardTests + source markers
```

少于三项视为本 pass 不合格。

### Step 4 — 做真实脚本推进

新增 evidence collection scripts，至少能 `-NoLaunch` 生成 evidence folder 模板。

### Step 5 — 自动化验证

必须运行：

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet build EZFuturePeek.sln
dotnet test EZFuturePeek.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
dotnet format EZFuturePeek.sln --verify-no-changes --no-restore
git diff --check
.\scripts\validate-repository-hygiene.ps1
```

如果修改 manifest/resources/export/package，还必须运行：

```powershell
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:EZMB_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\EZMB_RUN_RELEASE_ARTIFACT_TESTS

dotnet publish EZFuturePeek.sln
```

如果只改 Future Peek manifest/source/resources，必须 publish Future Peek。

### Step 6 — 如果能启动游戏，做最小 live proof

如果环境允许启动游戏，至少做：

```text
fresh current-package loader smoke
Future Peek Crystal Sphere smoke
Future Peek transform preview smoke
```

如果不能启动游戏，必须写：

```text
No game was opened.
Live rows remain pending.
This pass is implementation-hardening only, not goal completion.
```

### Step 7 — 最终报告

最终报告必须包含：

```text
1. Completion judgment: NOT COMPLETE / PARTIAL IMPLEMENTATION PASS
2. Source files changed
3. Tests changed
4. Scripts changed
5. Docs changed
6. Subagents used and concrete deliverables
7. Validation commands and results
8. Live proof status
9. Remaining blockers
10. Next exact pass
```

禁止写：

```text
goal completed
goal 已完成
release-ready
all implemented
co-op supported
```

除非 release evidence manifest 已全部通过。

---

## 6. 本 pass 的最低验收标准

本 pass 成功的最低标准：

```text
- GoalCompletionGuardTests exists and passes.
- ReleaseEvidenceGateTests exists and passes in default mode.
- Strict release-ready mode fails clearly when evidence is missing.
- ReleaseEvidenceLog exists and is used by at least three high-risk feature surfaces.
- Future Peek affects_gameplay decision is implemented and tested.
- Evidence collection scripts exist and can generate NoLaunch templates.
- At least one multiplayer policy/source change exists, not only docs.
- At least one save/load contract guard exists, not only docs.
- Build/test/format/diff/hygiene pass.
- Final report explicitly says goal.md is not complete unless live evidence exists.
```

本 pass 失败条件：

```text
- 只写 docs/specs/architecture/month-plan。
- 只增加一个文档存在性测试。
- 把 pending live rows 标 completed。
- 不碰 EZMicroBalanceCode 或 EZFuturePeekCode 或 scripts。
- 不新增能阻止 false completion 的 guard。
- 声称 release-ready 但没有 live evidence。
- 声称 co-op supported 但没有 two-client proof。
```

---

## 7. 建议的具体文件变更清单

优先级从上到下。

```text
tests/EZMicroBalance.Tests/GoalCompletionGuardTests.cs
tests/EZMicroBalance.Tests/ReleaseEvidenceGateTests.cs
tests/EZMicroBalance.Tests/SaveStateContractsGuardTests.cs

EZMicroBalanceCode/Diagnostics/ReleaseEvidenceLog.cs
EZMicroBalanceCode/Ascension/Core/MultiplayerFeaturePolicy.cs
EZMicroBalanceCode/Ancients/Expansion/Urda/...
EZMicroBalanceCode/Ancients/Expansion/Vakuu/...
EZMicroBalanceCode/Ancients/Expansion/Lotha/...
EZMicroBalanceCode/Ancients/Expansion/Morvi/...
EZMicroBalanceCode/Ascension/Rewards/RootDeckService*.cs

EZFuturePeek.json
EZFuturePeekCode/Patches/CrystalSpherePeekPatch.cs
EZFuturePeekCode/Patches/TransformPreviewPatch.cs
EZFuturePeekCode/Patches/TransformPredictionRngContext.cs
EZFuturePeekCode/Prediction/TransformPredictionService.cs
tests/EZFuturePeek.Tests/FuturePeekGuardTests.cs

scripts/collect-release-evidence.ps1
scripts/collect-future-peek-evidence.ps1
scripts/collect-coop-evidence.ps1

docs/review.md
docs/issues.md
docs/release-evidence-status.md
docs/features/future-peek/README.md
docs/reviews/red-team-goal-implementation-pass-1.md
```

---

## 8. 对上一轮报告的正确回应模板

你最后必须使用类似下面的措辞：

```text
I audited the previous "goal completed" claim and rejected it.
That pass completed documentation/governance groundwork only.
This pass implemented concrete guards/source/scripts to prevent docs-only completion claims and advanced runtime-evidence readiness.
No release-ready claim is made because live loader, clicked UI, save/load, Vakuu, co-op, and Future Peek live proof are still pending unless explicitly collected in this pass.
```

中文报告：

```text
我已审核上一轮“goal 已完成”的说法，并判定不成立。
上一轮只完成了文档/治理基线，没有完成 goal.md 的实现、实机、存读档、联机和 release evidence 要求。
本轮已通过源码、测试和脚本推进，防止以后再次用 docs-only 冒充完成。
除非本轮实际收集了 live evidence，否则仍不得声称 release-ready。
```

---

## 9. 最终目标

这次不要再做“看起来很完整的计划”。

真正目标是：

> **让仓库开始强制执行 goal.md，而不是让 goal.md 变成另一个被标记完成的文档。**

完成标准不是“写了更多 docs”，而是：

```text
false completion 被测试阻止
release evidence 被脚本化
高风险功能有 runtime markers
Future Peek 产品属性被修正
co-op 未证明不能被宣传
save/load contracts 有源码和测试约束
本轮有真实 source/test/script 变更
```

如果做不到，请诚实报告：

```text
Partial implementation only. Goal remains open.
```
