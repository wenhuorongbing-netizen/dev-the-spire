## 严格验收结论

这轮 **Batch 4b 不是“全面 RitsuLib 迁移完成”**，但它确实推进了 **RitsuLib ModPatcher patch migration**。当前 GitHub 最新提交已经往前走到 `e7080aa try improve`，也就是说这不是只在本地的工作；远程主线已经包含本轮迁移痕迹。

准确状态应该写成：

```text
RitsuLib dependency: 完成
RitsuLib runtime dependency: 完成
RitsuLib bootstrap: 完成第一层
FeatureRegistry scaffold: 完成第一层
Batch 4a/4b patch migration: 部分完成
RitsuLib lifecycle event migration: 未完成
RitsuLib DataStore / persistence: 未完成
RitsuLib settings: 未完成
RitsuLib content pack: 未完成
RewardPipeline / CardPlayContext / DeathProtection / MultiplayerPolicy: 未完成
Full test suite truth: 未闭环
Release-ready: 否
```

最重要的问题是：**本轮迁移数量和文档计数存在不一致，且报告只说 migration tests 通过，没有证明 full test suite 全绿。**

---

# 1. 每一步检查

## 1.1 RitsuLib NuGet dependency

**完成。**

`EZMicroBalance.csproj` 当前包含：

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All"/>
```

并且仍保留：

```xml
<PackageReference Include="Alchyr.Sts2.BaseLib" Version="3.1.4" PrivateAssets="All"/>
```

这符合“RitsuLib 接入，但 BaseLib 仍在过渡期保留”的路线。

---

## 1.2 RitsuLib runtime dependency

**完成。**

`EZMicroBalance.json` 当前 dependencies 里已经有：

```json
{ "id": "STS2-RitsuLib", "min_version": "0.3.2" }
```

同时也保留 BaseLib v3.1.4。

这意味着测试员必须安装：

```text
BaseLib
STS2-RitsuLib
Spire Plus
```

这一步完成，但后续必须继续补 runtime smoke evidence。

---

## 1.3 RitsuLib bootstrap

**完成第一阶段。**

`MainFile.Initialize()` 现在调用：

```csharp
RitsuLibBootstrap.ApplyPatches(ModId);
ModConfigRegistry.Register(ModId, new SpirePlusModConfig());
SpirePlusFeatureRegistry.CreateDefault().InitializeAll();
```



`RitsuLibBootstrap` 当前会创建 RitsuLib logger、创建 RitsuLib patcher、注册已迁移 patch、执行 `patcher.PatchAll()`，然后再执行 raw `Harmony.PatchAll()` 来处理未迁移 patch。

这是合理的混合模式：

```text
已迁移 patch -> RitsuLib ModPatcher
未迁移 patch -> raw Harmony
```

但这还不是完整 RitsuLib migration。

---

## 1.4 Batch 4a/4b patch migration

**部分完成，但有计数错误。**

`RitsuLibBootstrap.RegisterMigratedPatches()` 当前注册了：

### Batch 4a

```text
FiddlePatches: 4
ChoicesParadoxPatch: 1
DistinguishedCapePatches: 3
BlackStarCompensationPatches: 1
```

总数是：

```text
4 + 1 + 3 + 1 = 9
```



但是 `docs/migration.md` 写 Batch 4a “Migrated 10 low-risk patch classes”。这和源码注册数不一致。

### Batch 4b

`RitsuLibBootstrap` 当前注册：

```text
CrossbowPatches: 2
BrightestFlameExhaustDrawPatch: 3
DebtAndCardPatches: 8
SealOfGoldPatches: 2
PickupRewardPatches: 1
```

总数是：

```text
2 + 3 + 8 + 2 + 1 = 16
```



`docs/migration.md` 写 Batch 4b migrated 16 classes，这部分是对的。

### 总数

源码注册总数是：

```text
9 + 16 = 25
```

但 `docs/migration.md` 写：

```text
Total migrated: 26 classes
```



所以这里有明确文档/计数 bug。不是大功能 bug，但会误导后续迁移计划。

### 判定

```text
Batch 4b 源码迁移：完成。
Batch 4a/总数文档计数：错误，需要修。
Patch migration 总体：远未完成。
```

---

## 1.5 Patch inventory

**完成一部分。**

`docs/patch-inventory.md` 当前显示：

```text
Total patch declarations: 141
High risk: 22
Medium risk: 35
Low risk: 84
Unclassified owner: 0
```



这和 Codex 汇报“141 declarations remaining on raw Harmony”一致。

但注意：这份 inventory 只列 raw `[HarmonyPatch]` declarations；它不等于“所有 migrated patches 的 inventory”。下一步应该新增：

```text
Migrated ModPatcher patch inventory
Raw Harmony patch inventory
High-risk blocked patch inventory
```

否则未来无法判断哪些 patch 已迁、哪些仍然 raw、哪些不该迁。

---

## 1.6 Full tests

**未闭环。**

Codex 汇报：

```text
All 4 migration tests pass
Build 0 errors
Format clean
Pre-existing failures (Sts1Events, documentation): unrelated
```

这不是完整验收。只跑 “migration tests 4/4” 不等于：

```text
dotnet test EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
```

全绿。

如果仍有 Sts1Events / documentation failures，就必须修或者 quarantine；不能长期写“unrelated”。

---

## 1.7 Sts1Events scope creep

**仍然是风险。**

当前仓库里有 `Sts1DeadAdventurer.cs`，里面明确有：

```csharp
// TODO: Enter combat with random elite
// TODO: Grant random relic
```

并且如果 roll 到 elite/relic 分支，当前只是 `SetEventFinished(...)`，不会真的打精英或给遗物。

`Sts1Joust.cs` 则直接扣 50 金下注，没有看到 option 是否在金币不足时被禁用。

这两个不是 RitsuLib 迁移必须内容，属于 scope creep。必须确认：

```text
[ ] 是否注册到 live event pool？
[ ] 如果未注册，文档标 source skeleton only。
[ ] 如果已注册，必须禁用或补完。
[ ] 加 guard：含 TODO 的 EventModel 不能进入 live pool。
```

---

## 1.8 RitsuLib lifecycle / DataStore / settings

**未完成。**

`docs/migration.md` 明确写 Batch 1 没有迁：

```text
No content registration
No settings page
No persistence
```



也就是说：

```text
RitsuLib lifecycle events: 未迁
RitsuLib DataStore: 未迁
RitsuLib settings: 未迁
RitsuLib content pack: 未迁
```

这没有问题，但必须诚实标注为下一阶段，不要说全面迁移完成。

---

# 2. 当前最大问题清单

## P0：Patch migration 计数不一致

文档写：

```text
Batch 4a: 10
Batch 4b: 16
Total: 26
```

源码实际注册：

```text
Batch 4a: 9
Batch 4b: 16
Total: 25
```

这个必须修。后续所有迁移统计都应由测试自动计算，不能人工写错。

---

## P0：Full test truth 未闭环

报告说 “Migration tests 4/4 pass”，但又说有 pre-existing failures。
这不能算完成。

必须要求：

```text
dotnet test EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
```

全量结果。如果失败，必须列出失败项、原因、是否 quarantine。

---

## P0/P1：Sts1Events skeleton 不能进入 live pool

`Sts1DeadAdventurer` 有 TODO 且奖励/战斗分支未实现。
`Sts1Joust` 可能金币不足仍可下注。

这必须单独处理，不然会变成新的 runtime bug 来源。

---

## P1：ModPatcher + raw Harmony 双 patch 风险仍需 guard

当前 `RitsuLibBootstrap` 同时运行 `patcher.PatchAll()` 和 `harmony.PatchAll()`。

这很合理，但必须有 tests 保证：

```text
[ ] RegisterMigratedPatches 中的 class 不含 [HarmonyPatch]
[ ] 含 [HarmonyPatch] 的 class 不在 RegisterMigratedPatches
[ ] PatchId 唯一
[ ] RegisterMigratedPatches count 与 docs 自动一致
```

---

## P1：RitsuLib runtime smoke 仍需要真实证据

Manifest 已经依赖 STS2-RitsuLib。
csproj 已经依赖 STS2.RitsuLib。
但必须在游戏里证明：

```text
RitsuLib framework is active
ModPatcher applied expected patches
BaseLib + RitsuLib + Spire Plus clean load
```

没有这个就不能说 runtime 迁移完成。

---

# 3. 本轮完成度表

| 项目                                | 状态                                   |
| --------------------------------- | ------------------------------------ |
| STS2.RitsuLib NuGet               | 完成                                   |
| STS2-RitsuLib manifest dependency | 完成                                   |
| RitsuLibBootstrap                 | 完成第一层                                |
| ModPatcher hybrid mode            | 完成第一层                                |
| Batch 4a patch migration          | 源码完成 9 个；文档错写 10 个                   |
| Batch 4b patch migration          | 完成 16 个                              |
| Raw Harmony remaining count       | 141，已记录                              |
| Patch inventory                   | 有 raw inventory；缺 migrated inventory |
| Full tests                        | 未闭环                                  |
| Sts1Events risk                   | 未闭环                                  |
| Ritsu lifecycle                   | 未开始                                  |
| Ritsu DataStore                   | 未开始                                  |
| State codec                       | 未开始                                  |
| RewardPipeline                    | 未开始                                  |
| CardPlayContext                   | 未开始                                  |
| DeathProtectionService            | 未开始                                  |
| MultiplayerPolicy                 | 未开始                                  |
| Release-ready                     | 否                                    |

---

# 4. 下一步 Monthly Dev Spec

## 月度目标：RitsuLib Migration Stabilization Month

本月不要继续无脑迁更多 patch。先把当前混合迁移模式稳定下来。

核心目标：

```text
1. 证明 RitsuLib runtime 可用。
2. 关闭 Batch 4a/4b 计数和 double-patch 风险。
3. 清理 Sts1Events 未完成 skeleton 风险。
4. 让 FeatureRegistry 从 wrapper 变成真实 status registry。
5. 启动 StateCodec / RewardPipeline / CardPlayContext / DeathProtection / MultiplayerPolicy 的架构工作。
```

---

## Week 1：RitsuLib Runtime + Test Truth

### 工作

```text
[ ] 运行完整 dotnet test，不只 migration tests。
[ ] 如果 Sts1Events/documentation failures 存在，修复或 quarantine。
[ ] 运行 clean loader smoke：
    BaseLib + STS2-RitsuLib + Spire Plus
[ ] 记录 RitsuLib framework active。
[ ] 记录 ModPatcher applied count。
[ ] 更新 install docs。
```

### 验收

```text
[ ] full test suite 绿，或失败项有正式 quarantine issue。
[ ] clean loader log 有 RitsuLib active。
[ ] docs 不再只写 “migration tests pass”。
```

---

## Week 2：Patch Migration Integrity

### 工作

```text
[ ] 修 docs/migration.md 计数：Batch 4a 9，Batch 4b 16，总 25。
[ ] 自动生成 migrated patch inventory。
[ ] 自动生成 raw Harmony patch inventory。
[ ] 自动生成 high-risk blocked patch inventory。
[ ] 添加 PatchId 唯一测试。
[ ] 添加 migrated/no-HarmonyPatch guard。
[ ] 添加 raw/not-registered guard。
```

### 验收

```text
[ ] docs/migration.md 计数和源码一致。
[ ] RegisterMigratedPatches 自动或测试覆盖。
[ ] 没有 double patch 风险。
```

---

## Week 3：Scope Creep / FeatureRegistry Hardening

### 工作

```text
[ ] Sts1Events：确认是否 live。
[ ] TODO EventModel 不得 live registered。
[ ] Sts1Joust 金币不足逻辑做 source review。
[ ] FeatureRegistry 增加：
    DisplayName
    Category
    DisableEnvKeys
    ForceEnvKeys
    BootstrapStatus
    LiveStatus
```

### 验收

```text
[ ] Sts1Events 安全状态明确。
[ ] FeatureRegistry 区分 bootstrap enabled 和 live available。
[ ] Vakuu log 显示 bootstrap enabled but live hidden。
```

---

## Week 4：Architecture Foundation

### 工作

```text
[ ] UrdaStateCodec V1：Encode/Decode/MalformedFallback/OldMigration。
[ ] RewardPipeline skeleton：只做 diagnostics。
[ ] CardPlayContext skeleton：Power fallback / recursion policy。
[ ] DeathProtectionService spec：Lotha Reprieve。
[ ] MultiplayerPolicy doc：所有 active feature 标 policy。
```

### 验收

```text
[ ] UrdaStateCodec tests pass。
[ ] RewardPipeline handler order documented。
[ ] CardPlayContext tests exist。
[ ] DeathProtectionService spec exists。
[ ] MultiplayerPolicy matrix exists。
```

---

# 5. 下一个具体 Goal

## GOAL：RitsuLib Batch 4a/4b Closure + Runtime Truth

这是下一轮最合理目标。不要直接 Batch 4c。

### 给 Codex 的 Prompt

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：RitsuLib Batch 4a/4b closure + runtime truth。

当前状态：
- STS2.RitsuLib 0.3.2 已加入 csproj。
- STS2-RitsuLib 已加入 manifest。
- RitsuLibBootstrap 使用 ModPatcher + raw Harmony fallback。
- Batch 4a/4b 已迁移部分 patch。
- docs/migration.md 当前计数可能不准确：Batch 4a 表写 10，但源码 RegisterMigratedPatches 中 Batch 4a 实际是 9；Batch 4b 是 16；总数应为 25，不是 26。
- docs/patch-inventory.md 当前 raw Harmony declarations = 141。
- Sts1Events 存在 TODO skeleton，需要确认未 live。
- 报告说 pre-existing failures，不允许继续忽略。

不要新增 gameplay。
不要迁 high-risk patch。
不要 claim release-ready。
不要只跑 migration tests。

必须使用 subagents：

Subagent A — Runtime/RitsuLib Compatibility:
- 检查 BaseLib + STS2-RitsuLib + Spire Plus 安装文档。
- 准备/记录 loader smoke checklist。
- 检查 RitsuLib runtime variant 和 target version 口径。

Subagent B — Patch Migration Integrity:
- 扫描 RegisterMigratedPatches。
- 统计 migrated patch count。
- 统计 raw HarmonyPatch count。
- 检查 PatchId 唯一。
- 检查 migrated class 不含 HarmonyPatch。
- 检查 raw Harmony class 不在 RegisterMigratedPatches。

Subagent C — Test Truth:
- 运行 full dotnet test。
- 列出所有失败。
- 不允许只写 “migration tests pass”。
- 失败要修复或 quarantine。

Subagent D — Scope Creep/Sts1Events:
- 检查 Sts1DeadAdventurer / Sts1Joust 是否注册进 live pool。
- 如果未注册，文档标 inactive source skeleton。
- 如果注册，必须禁用或完成。
- 加 guard：TODO EventModel 不能 live registered。

Subagent E — Monthly Architecture Planner:
- 输出下一步 state codec / reward pipeline / cardplay context / death protection / multiplayer policy 计划。

任务：
1. 修 docs/migration.md 计数。
2. 生成/更新 migrated patch inventory。
3. 添加 double-patch guard tests。
4. 处理 Sts1Events skeleton。
5. 运行：
   - dotnet build EZMicroBalance.sln
   - dotnet test EZMicroBalance.sln
   - dotnet test EZMicroBalance.sln --no-build
   - dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
   - git diff --check
6. 如果 code/package 变更，publish + release artifact tests。

最终汇报：
- 当前 HEAD
- full test suite 是否全绿
- Batch 4a/4b 实际 migrated count
- raw Harmony remaining count
- double-patch guard 结果
- Sts1Events 状态
- RitsuLib runtime smoke 状态
- 下月计划
- release-ready: no
```

---

# 6. 最重要提醒：必须用 subagents

这类任务已经超过单 agent 一口气处理的复杂度。必须拆成：

### Subagent A：Runtime/RitsuLib Compatibility

负责：

```text
RitsuLib variant pack
install docs
loader smoke
v0.106.x 版本口径
```

### Subagent B：Patch Migration Integrity

负责：

```text
RegisterMigratedPatches
PatchId uniqueness
double patch
migrated/raw/high-risk inventory
```

### Subagent C：Test Truth

负责：

```text
full dotnet test
pre-existing failures
quarantine policy
no cherry-picked migration-only pass
```

### Subagent D：Scope Creep / Sts1Events

负责：

```text
TODO skeleton
live registration
event pool safety
gold/relic/combat branch completeness
```

### Subagent E：Architecture Planner

负责：

```text
UrdaStateCodec
RewardPipeline
CardPlayContext
DeathProtectionService
MultiplayerPolicy
monthly roadmap
```

---

## 最终判断

这轮工作有价值，但不能说“完成迁移”。

它完成的是：

```text
RitsuLib Batch 4b patch migration source work
```

但还必须补：

```text
migration count truth
full test truth
double-patch guards
Sts1Events safety
runtime loader evidence
monthly architecture foundation
```

所以，下一步不是继续 Batch 4c，而是先做：

```text
Batch 4a/4b closure + runtime truth
```

否则继续迁 patch 会把风险越滚越大。
