# 严格验收结论

**没有完成“全面迁移到 RitsuLib”。**
这次工作完成的是 **RitsuLib 接入 + 部分 patch migration + FeatureRegistry scaffold + 迁移文档**，不是完整架构迁移。

更精确的状态是：

```text
RitsuLib dependency: 已加入
RitsuLib runtime dependency: 已加入
RitsuLib bootstrap: 已加入
RitsuLib ModPatcher: 部分使用
FeatureRegistry: 有 scaffold
Patch migration: 仅少量 patches 已迁
RitsuLib lifecycle events: 未迁
RitsuLib DataStore / persistence: 未迁
RitsuLib settings: 未迁
RitsuLib content pack: 未迁
State codec: 未完成
Reward pipeline: 未完成
CardPlayContext: 未完成
DeathProtectionService: 未完成
MultiplayerPolicy: 未完成
Release-ready: 否
```

所以它的汇报“migration plan implemented”只能理解为 **迁移计划前半段 / PR1–PR6 Batch 1 完成**，不能理解为“RitsuLib 迁移完成”。

---

# 1. 当前 GitHub 远程状态

当前远程最新提交是：

```text
b53ddb0 migration
```

它已经在 GitHub main 上可见。
也就是说，这次不再是“本地未 push”，现在可以按 GitHub 审查。

它最近的提交链包括：

```text
737acab docs: add Codex harness templates, RitsuLib staging, and migration plan
29d1b93 refactor: create target directory structure for move-only refactor
7d6d736 docs: update README and PROJECT_MAP with new directory structure
2fd2b1a docs: expand PR 5 version mismatch blockers in migration plan
6001a6e docs: add NuGet package status to RitsuLib staging record
591b596 migration middle
b53ddb0 migration
```

这些提交都已经能在 GitHub 上看到。

---

# 2. 逐步验收

## Step 1：RitsuLib NuGet dependency

**完成。**

`EZMicroBalance.csproj` 当前已经有：

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All"/>
```

同时 BaseLib 仍保留：

```xml
<PackageReference Include="Alchyr.Sts2.BaseLib" Version="3.1.4" PrivateAssets="All"/>
```



### 结论

```text
RitsuLib compile dependency 已加入。
BaseLib 没有移除。
符合“过渡期混合架构”。
```

---

## Step 2：RitsuLib runtime manifest dependency

**完成。**

`EZMicroBalance.json` 现在 dependencies 包含：

```json
{
  "id": "STS2-RitsuLib",
  "min_version": "0.3.2"
}
```

并且仍依赖 BaseLib v3.1.4。

### 结论

```text
玩家/测试员现在必须安装：
- BaseLib
- STS2-RitsuLib
- Spire Plus
```

这意味着所有安装文档、测试文档、网站下载说明都必须同步，否则测试员会直接加载失败。

---

## Step 3：RitsuLib bootstrap

**部分完成。**

当前 `MainFile.Initialize()` 调用：

```csharp
RitsuLibBootstrap.ApplyPatches(ModId);
ModConfigRegistry.Register(ModId, new SpirePlusModConfig());
SpirePlusFeatureRegistry.CreateDefault().InitializeAll();
```



`RitsuLibBootstrap` 做了：

```csharp
RitsuLibFramework.CreateLogger(modId)
RitsuLibFramework.CreatePatcher(modId, "SpirePlus")
RegisterMigratedPatches(patcher)
patcher.PatchAll()
new Harmony(modId).PatchAll()
```



### 结论

```text
RitsuLib logger: 已用
RitsuLib CreatePatcher: 已用
RitsuLib ModPatcher: 已部分用
raw Harmony PatchAll: 仍然保留
```

这不是坏事，因为一次性迁完所有 patch 很危险。但它说明目前只是 **混合 patch 模式**。

---

## Step 4：RitsuLib ModPatcher patch migration

**部分完成。**

`RitsuLibBootstrap.RegisterMigratedPatches()` 当前注册了：

```text
FiddlePatches: 4 classes
ChoicesParadoxPatches: 1 class
DistinguishedCapePatches: 3 classes
BlackStarCompensationPatches: 1 class
```

也就是约 9 个 patch class。

提交 diff 也显示多个 patch 从 `[HarmonyPatch(...)]` 改成实现 `IPatchMethod`，并提供 `PatchId`、`IsCritical`、`Description`、`GetTargets()`。

同时测试也改成接受 `ModPatchTarget(...)`，例如 AncientBehaviorGuardTests 已经把部分原始 `[HarmonyPatch(...)]` 断言改成 `ModPatchTarget(...)`。

### 未完成点

`docs/migration.md` 明确写：63 个 patch class 迁到 RitsuLib `ModPatcher` 仍然是未来 batch，需要每个 patch class 实现 `IPatchMethod` 或 `IModPatchProvider`，PR7+ 高风险 patch migrations 仍 blocked。

### 结论

```text
Patch migration: 少量低风险 patches 已迁。
High-risk run/map/reward/save/multiplayer patches 未迁。
不能叫完成。
```

---

## Step 5：FeatureRegistry / MainFile 解耦

**部分完成。**

`MainFile` 已经不直接调用 Lotha/Morvi/Urda/Vakuu/Ascension initializer，而是调用 `SpirePlusFeatureRegistry.CreateDefault().InitializeAll()`。

`SpirePlusFeatureRegistry` 注册了：

```text
Ancients.Lotha
Ancients.Morvi
Ancients.Urda
Ancients.VakuuFight
Ascension.A11A20
```

然后分别 delegate 到原来的 initializer。

`FeatureRegistry` 会按 `InitOrder` 排序、打印 gate 状态、初始化模块，失败时 log warning 并 rethrow。

### 未完成点

`IFeatureModule` 目前只有：

```text
Id
InitOrder
EvaluateGate()
Initialize()
```



`FeatureGateResult` 也只有：

```text
IsEnabled
Reason
```



缺少：

```text
DisplayName
Category
DisableEnvKeys
ForceEnvKeys
Dependencies
BootstrapStatus
LiveStatus
RuntimeStatus
Diagnostics
MultiplayerPolicy
```

更重要的是，`SpirePlusFeatureRegistry` 里的 gate 很多只是写：

```text
default-on; runtime gates remain in MorviFeatureGate
default-on; runtime gates remain in LothaFeatureGate
hooks registered; fight entry remains hidden by VakuuFightFeatureGate
```



也就是说：

```text
FeatureRegistry 只决定 bootstrap 是否执行。
真正 live gate 还在各 feature 内部。
```

### 结论

```text
MainFile 解耦：完成第一步。
真正模块化：未完成。
```

---

## Step 6：RitsuLib lifecycle event migration

**未完成。**

这次没有看到使用 `SubscribeLifecycle<TEvent>` 的实际代码。
`docs/migration.md` 也没有声称 lifecycle events 已迁；它只说 Batch 1 是 bootstrap + diagnostics。

### 结论

```text
Lifecycle migration: 未开始。
```

---

## Step 7：RitsuLib DataStore / persistence migration

**未完成。**

`docs/migration.md` 明确写：

```text
No persistence (BeginModDataRegistration) — existing SavedSpireFields stay.
```



当前 `AncientSavedStateFields` 仍是 BaseLib SavedSpireField 风格，包含大量 `SavedSpireField<Player, string>`、`SavedSpireField<CardModel, string>`、`SavedSpireField<CardModel, bool>`。

Urda 仍使用长字符串 `string.Join(ProgressSeparator, ...)` 写状态。

### 结论

```text
RitsuLib DataStore: 未迁。
State codec: 未做。
这是下一个 monthly spec 的核心。
```

---

## Step 8：RitsuLib settings migration

**未完成。**

`docs/migration.md` 写：

```text
No settings page (RegisterModSettings) — existing BaseLib config stays.
```



### 结论

```text
Settings migration: 未做。
可以暂缓。
```

---

## Step 9：RitsuLib content pack migration

**未完成。**

`docs/migration.md` 写：

```text
No content registration (CreateContentPack) — Spire Plus doesn't register new cards/relics/potions through RitsuLib.
```



### 结论

```text
Content registration migration: 未做。
可以暂缓，但不要说全面迁移完成。
```

---

## Step 10：Docs / migration plan

**完成一部分，但还不够统一。**

现在有：

```text
docs/migration.md
docs/integrations/ritsulib.md
docs/refactor-map.md
```

`docs/migration.md` 记录 PR sequence、PR5 hard dependency、PR6 Batch 1 bootstrap + diagnostics、Batch 4/5 blockers。

`docs/integrations/ritsulib.md` 记录 RitsuLib hard dependency、runtime variant pack、NuGet status、upgrade path。

`docs/refactor-map.md` 记录目标目录结构，包括 `Core/Integrations/RitsuLib/`、`Ancients/Rebalance/`、`Ascension/Ui/`、`Ascension/Save/` 等。

### 问题

1. 这些文档不在统一的 `docs/features/ritsulib-migration/` 下。
2. `docs/integrations/ritsulib.md` 写当前目标是 v0.106.1，但 `PROJECT_STATE.md` 仍写游戏 snapshot 是 v0.106.0。 
3. `docs/migration.md` 写测试结果里有“302 passed, 21 skipped, 0 failed”，同时又写“1 pre-existing batch script failure unrelated”，这句话自相矛盾：如果有 failure，就不能写 0 failed，除非那不是 test suite failure。

### 结论

```text
迁移文档存在。
版本/测试表述需要清理。
```

---

# 3. 明显风险和可能 bug

## 3.1 RitsuLib compile/runtime mismatch

当前：

```text
Compile package: STS2.RitsuLib 0.3.2
Runtime variant pack: 0.3.3
Runtime variant target: 0.106.1
NuGet compat package 0.106.1: not published
```



这可能能跑，但必须 live smoke：

```text
BaseLib + STS2-RitsuLib + Spire Plus
```

看是否有：

```text
MissingMethodException
TypeLoadException
RitsuLib framework not active
ModPatcher failure
```

现在不能只靠 build 通过。

---

## 3.2 raw Harmony + ModPatcher 混合可能双 patch

`RitsuLibBootstrap` 先 `patcher.PatchAll()`，再 `harmony.PatchAll()`。

迁移后的 patch classes 已经移除了 `[HarmonyPatch]` attribute 并实现 `IPatchMethod`，这可以避免同一个 patch 被 raw Harmony 再扫一次。

但必须保证：

```text
所有迁移 patch classes 都真的没有 HarmonyPatch attribute。
所有未迁移 patch classes 仍有 HarmonyPatch attribute。
没有一个 patch 同时被 ModPatcher 和 raw Harmony patch。
```

当前测试只改了一部分 guard，仍需专门的 double-patch guard。

---

## 3.3 新增了 STS1 event skeleton，存在 TODO 和 scope creep

当前 `EZMicroBalanceCode/Sts1Events/Models/Act1/Sts1DeadAdventurer.cs` 已经存在，里面有：

```csharp
// TODO: Enter combat with random elite
// TODO: Grant random relic
```

并且现在遇到 elite/relic 分支时只是 `SetEventFinished(...)`，不会真的打精英或给遗物。

`Sts1Joust` 也存在，并且下注时直接：

```csharp
await PlayerCmd.GainGold(-BetCost, Owner);
```

没有看到是否检查玩家金币足够。

这有两个问题：

1. **这不是 RitsuLib 迁移目标的一部分。** 它是 scope creep。
2. 如果这些 event 已注册或未来被误注册，会导致玩家选到未完成事件。

### 必须处理

```text
[ ] 确认 StS1Events 是否注册进任何 event pool。
[ ] 若未注册，明确 docs 写 “source skeleton only, inactive”。
[ ] 若已注册，必须马上禁用或完成。
[ ] 加 guard：TODO event skeleton 不能进入 live pool。
```

---

## 3.4 Version target 文档冲突

`PROJECT_STATE.md` 写：

```text
Slay the Spire 2 game snapshot: v0.106.0
```



`docs/integrations/ritsulib.md` 写：

```text
Current repo StS2 target: v0.106.1
Runtime variant: 0.106.1 available
```



这必须统一，否则后面所有源码证据都会乱。

---

## 3.5 Docs 说 “PR5 hard dependency done”，但 runtime smoke 未闭环

硬依赖已经加了，但还没有看到远程证据证明：

```text
游戏实际安装 STS2-RitsuLib variant pack 后，Spire Plus 能 clean load。
```

这必须是下一个 P0。

---

# 4. 这次是否完成我们的目标？

按“阶段目标”看：

| 阶段                                  | 状态                     |
| ----------------------------------- | ---------------------- |
| PR1 docs harness / migration plan   | 完成                     |
| PR3 directory scaffolds             | 完成，但多为目录/规划            |
| PR5 RitsuLib hard dependency        | 完成                     |
| PR6 Batch 1 bootstrap diagnostics   | 完成                     |
| MainFile FeatureRegistry 解耦         | 完成第一步                  |
| RitsuLib ModPatcher patch migration | 部分完成，约 9 patch classes |
| RitsuLib lifecycle                  | 未完成                    |
| RitsuLib DataStore                  | 未完成                    |
| RitsuLib settings                   | 未完成                    |
| RitsuLib content pack               | 未完成                    |
| BaseLib reduction                   | 未完成                    |
| State codec                         | 未完成                    |
| Reward pipeline                     | 未完成                    |
| CardPlayContext                     | 未完成                    |
| DeathProtectionService              | 未完成                    |
| MultiplayerPolicy                   | 未完成                    |
| Live loader smoke                   | 未证明                    |
| Release ready                       | 否                      |

**结论：完成了 monthly migration 的前置阶段，不是完成全面迁移。**

---

# 5. 下一步 Monthly Dev Spec

## Monthly Goal

```text
Spire Plus RitsuLib Stabilization Month
```

目标不是继续扩功能，而是把 RitsuLib 接入从“build passes”推进到：

```text
runtime proven
state safer
patch migration controlled
pipeline architecture started
```

---

## Week 1：RitsuLib Runtime Proof + Version Alignment

### 目标

证明 BaseLib + RitsuLib + Spire Plus 能在真实游戏 clean load。

### 必做

1. 统一版本口径：

```text
PROJECT_STATE.md
docs/integrations/ritsulib.md
docs/dev-environment.md
docs/private-beta-verification-handoff.md
docs/release-checklist.md
README_INSTALL.txt
website install docs
```

统一成：

```text
v0.106.1
```

或者如果你实际游戏还是 v0.106.0，就把 RitsuLib docs 改回“variant mismatch”。

2. RitsuLib loader smoke：

```text
BaseLib v3.1.4
STS2-RitsuLib variant pack 0.3.3
Spire Plus
```

要求 log 里有：

```text
RitsuLib bootstrap starting
ModPatcher applied X patches
RitsuLib framework is active
Spire Plus loaded
0 MissingMethodException
0 TypeLoadException
0 release-blocking ERROR
```

3. Install docs：

```text
BaseLib
STS2-RitsuLib
Spire Plus
```

都要写清楚。

### Week 1 验收

```text
[ ] clean Steam loader smoke 通过
[ ] docs 版本统一
[ ] RitsuLib install docs 完成
[ ] package evidence 更新
```

---

## Week 2：Patch Migration Safety

### 目标

让 ModPatcher + raw Harmony 混合模式可控，不双 patch。

### 必做

1. Patch inventory 分类：

```text
MigratedToRitsuModPatcher
RawHarmonyRemaining
HighRiskBlocked
```

2. Guard：

```text
[ ] migrated patch class 不得含 [HarmonyPatch]
[ ] raw patch class 不得被 RegisterPatch 注册
[ ] 每个 IPatchMethod 有 PatchId/Description/GetTargets
[ ] PatchId 唯一
[ ] patcher.RegisterPatch list 与 inventory 一致
```

3. 不迁高风险 patch：

```text
run
room
save
lobby
multiplayer
reward
death
A20 boss flow
```

除非单独任务。

### Week 2 验收

```text
[ ] 无 double-patch risk
[ ] docs/patch-inventory.md 更新
[ ] 迁移 patches 的 behavior smoke 通过
```

---

## Week 3：State Codec / Persistence Safety

### 目标

开始解决 string state 的根本问题。

### 必做

1. UrdaStateCodec：

```text
UrdaStateV1
Encode
Decode
MigrateOldString
MalformedFallback
RoundTrip tests
```

2. 不立刻改 Morvi/Lotha，但写 spec：

```text
MorviStateV1
LothaStateV1
```

3. RitsuLib DataStore POC：

```text
InMemory or harmless Global smoke
```

不把真实 gameplay state 直接迁过去，先证明 API 可用。

### Week 3 验收

```text
[ ] Urda codec tests pass
[ ] old state still readable
[ ] malformed state safe
[ ] Ritsu DataStore POC documented
```

---

## Week 4：Domain Pipeline Scaffolds

### 目标

开始减少未来 bug 面积。

### 必做

1. RewardPipeline skeleton：

```text
RewardPipelineContext
RewardPhase
IRewardHandler
handler priority docs
diagnostics only
```

覆盖文档：

```text
Urda Seedbed
Urda Humus
Morvi Forbidden Loan
Morvi Debt Settlement
Lotha Closed Court
Prismatic Gem
Fission
A19 Boss reward
```

2. CardPlayContext skeleton：

```text
ExtraPlayPolicy
Power fallback rule
No recursion
No clone/autoplay first-card trigger
```

先接入一个低风险效果或只加 tests/docs。

3. DeathProtectionService spec：

```text
Lotha Death Reprieve
forced death
inReprieveTurn
co-op owner
save/load policy
```

4. MultiplayerPolicy doc：

```text
LocalUiOnly
LocalPlayerOnly
HostAuthoritative
SharedRunState
CombatCommandReplicated
UnsafeInMultiplayer
```

### Week 4 验收

```text
[ ] RewardPipeline docs/tests
[ ] CardPlayContext docs/tests
[ ] DeathProtectionService spec
[ ] MultiplayerPolicy matrix
```

---

# 6. 下一个具体 Goal

## GOAL-2026-05-28-RITSULIB-RUNTIME-PROOF-AND-PATCH-SAFETY

### Prompt 给 Codex

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：RitsuLib runtime proof + patch safety hardening。

当前状态：
- STS2.RitsuLib 0.3.2 已加到 csproj。
- STS2-RitsuLib 已加到 manifest dependencies。
- RitsuLibBootstrap 已用 CreateLogger/CreatePatcher。
- 约 9 个 patch class 已迁到 IPatchMethod。
- raw Harmony PatchAll 仍保留。
- RitsuLib lifecycle/DataStore/settings/content pack 未迁。
- docs/integrations/ritsulib.md 写 current target v0.106.1，但 PROJECT_STATE.md 仍写 v0.106.0，必须统一。
- Sts1Events 里有未完成 TODO skeleton，不属于当前 Ritsu migration 目标，必须确认未注册或禁用。

不要新增 gameplay。
不要迁高风险 patch。
不要 claim release-ready。

任务：
1. 统一 v0.106 target docs。
2. 更新 install docs：BaseLib + STS2-RitsuLib + Spire Plus。
3. 添加 loader-smoke checklist，要求 RitsuLib framework active。
4. 检查/更新 patch inventory：
   - migrated Ritsu patches
   - raw Harmony patches
   - high-risk blocked patches
5. 添加 tests：
   - migrated patches do not have HarmonyPatch attribute
   - raw Harmony patches are not registered in RitsuLibBootstrap
   - PatchId unique
   - RitsuLib dependency exists in csproj and manifest
   - version docs do not conflict
6. Sts1Events：
   - confirm not registered, or disable.
   - add issue if incomplete skeleton remains.
   - guard that TODO event skeleton cannot be live.
7. Run:
   - dotnet build
   - dotnet test
   - dotnet test --no-build
   - dotnet format --verify-no-changes --no-restore
   - git diff --check
8. If source/package changed:
   - dotnet publish
   - release artifact tests

Final:
- version target
- RitsuLib install status
- patch migration status
- STS1 skeleton status
- tests
- remaining blockers
- release-ready: no
```

---

# 7. Subagent 使用要求

你应该明确要求 Codex 用 subagents，不然它会在一个上下文里把所有事混起来。

## 建议 subagents

### Subagent A：RitsuLib Compatibility Agent

负责：

```text
[ ] 查 csproj / manifest / docs/integrations/ritsulib.md
[ ] 对齐 v0.106.0 / v0.106.1
[ ] 检查 Ritsu runtime variant pack 安装要求
[ ] 输出 install checklist
```

### Subagent B：Patch Migration Agent

负责：

```text
[ ] 扫所有 Harmony patches
[ ] 分类 migrated/raw/high-risk
[ ] 检查 double patch risk
[ ] 更新 patch inventory
[ ] 写 PatchId 唯一测试
```

### Subagent C：State/Persistence Agent

负责：

```text
[ ] 审 Urda/Morvi/Lotha state
[ ] 设计 UrdaStateCodec
[ ] 找 semicolon string 风险
[ ] 准备 Week 3 计划
```

### Subagent D：Docs/Release Evidence Agent

负责：

```text
[ ] 统一 docs version
[ ] 更新 README_INSTALL / handoff / PROJECT_STATE
[ ] 确保 release-ready 仍是 no
[ ] 不让 docs 说 “fully migrated”
```

### Subagent E：Scope Creep Agent

负责：

```text
[ ] 检查 Sts1Events 未完成 skeleton
[ ] 检查是否有 TODO live content
[ ] 检查是否有未注册但编译进来的试验代码
[ ] 形成 issue/guard
```

---

# 8. 最终判断

这次工作**不是失败**，但不能说“全面迁移已完成”。

它真实完成的是：

```text
RitsuLib dependency landing
RitsuLib bootstrap
部分 ModPatcher migration
FeatureRegistry scaffold
migration docs
```

它仍未完成：

```text
runtime loader proof
version target consistency
lifecycle events
DataStore
state codec
reward pipeline
cardplay context
death protection
multiplayer policy
most patch migration
```

所以下一步的月度目标应该是：

```text
先证明 RitsuLib 在真实运行时稳定；
再确保 patch 混合模式不会 double patch；
再做 state codec；
最后才迁高风险 lifecycle/reward/death/multiplayer。
```

这才是安全的 RitsuLib 迁移路线。
