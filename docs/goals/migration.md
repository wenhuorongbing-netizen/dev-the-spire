## 严格验收结论

这轮 **Batch 4a 不能算“RitsuLib 迁移完成”**，但可以算：

```text
RitsuLib Patch Migration Batch 4a：部分完成，可以进入下一阶段审查。
```

它完成了一个小而有价值的目标：**把少量低风险 Ancient patch 从纯 `[HarmonyPatch]` 迁移到 RitsuLib `IPatchMethod` / `ModPatcher` 路径**。但它没有完成更大的目标：完整 RitsuLib 迁移、完整 patch 迁移、生命周期迁移、DataStore 迁移、状态 codec、RewardPipeline、DeathProtection、MultiplayerPolicy 都还没有完成。

最新 GitHub 远端已经能看到 `b53ddb0 migration`，说明这轮不是本地未提交状态。

---

# 1. 这轮实际完成了什么

## 1.1 RitsuLib 依赖已经是硬依赖

`EZMicroBalance.csproj` 当前已经包含：

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All"/>
```

同时仍保留 BaseLib 3.1.4。

`EZMicroBalance.json` 当前也已经加入：

```json
{
  "id": "STS2-RitsuLib",
  "min_version": "0.3.2"
}
```



所以这部分是完成的。现在测试员安装链路必须是：

```text
BaseLib
STS2-RitsuLib
Spire Plus
```

---

## 1.2 MainFile 已经走 RitsuLibBootstrap + FeatureRegistry

`MainFile.Initialize()` 当前是：

```csharp
RitsuLibBootstrap.ApplyPatches(ModId);
ModConfigRegistry.Register(ModId, new SpirePlusModConfig());
SpirePlusFeatureRegistry.CreateDefault().InitializeAll();
```



这说明入口不再直接写所有 initializer，已经进入“RitsuLib bootstrap + FeatureRegistry scaffold”阶段。

---

## 1.3 RitsuLibBootstrap 已经进入混合 patch 模式

`RitsuLibBootstrap` 当前会：

1. 创建 RitsuLib logger；
2. 创建 RitsuLib patcher；
3. `RegisterMigratedPatches(patcher)`；
4. `patcher.PatchAll()`；
5. 再执行 raw `Harmony.PatchAll()` 处理未迁移 patch。

也就是说当前是：

```text
RitsuLib ModPatcher：用于已迁移 patch
Raw Harmony.PatchAll：继续用于未迁移 patch
```

这符合安全迁移策略，不是一次性迁完整个 patch 面。

---

## 1.4 Batch 4a 确实迁移了一小批 patch

`RitsuLibBootstrap.RegisterMigratedPatches()` 目前注册：

```text
FiddlePatches: 4 classes
ChoicesParadoxPatch: 1 class
DistinguishedCapePatches: 3 classes
BlackStarObtainPatch: 1 class
```

总计 9 个 patch class。

commit diff 也显示这些 patch class 改成实现 `IPatchMethod`，并加了：

```text
PatchId
IsCritical
Description
GetTargets()
```

例如 `BlackStarObtainPatch`、`ChoicesParadoxPatch`、`DistinguishedCapeVarsPatch`、`FiddleVarsPatch` 等。

这部分是本轮最实质的代码成果。

---

## 1.5 测试 guard 已经开始适配 ModPatchTarget

`AncientBehaviorGuardTests` 里已经出现对 `ModPatchTarget(...)` 的断言，例如：

```text
ModPatchTarget(typeof(RelicCmd), nameof(RelicCmd.Obtain)
ModPatchTarget(typeof(DistinguishedCape), "get_CanonicalVars"
ModPatchTarget(typeof(Vakuu), "GenerateInitialOptions")
ModPatchTarget(typeof(ChoicesParadox), nameof(ChoicesParadox.AfterPlayerTurnStart))
ModPatchTarget(typeof(Fiddle), ...)
```

这说明测试已开始接受 RitsuLib patch path。

---

# 2. 没有完成什么

## 2.1 没有完成完整 patch migration

`docs/migration.md` 自己写得很清楚：RitsuLib 的 `ModPatcher.PatchAll()` 不扫描 `[HarmonyPatch]`，需要每个 patch class 实现 `IPatchMethod` 或 `IModPatchProvider`，并且 63 个 patch class 仍然需要后续迁移。

当前只迁了约 9 个 patch class。剩下高风险 patch 包括：

```text
run
room
save
lobby
multiplayer
reward
death
A20 boss flow
map hover
card reward alternative
```

仍然不应该动，除非单独任务。

---

## 2.2 没有完成 RitsuLib lifecycle migration

当前没有看到 `SubscribeLifecycle<TEvent>` 的实际迁移。`RitsuLibBootstrap` 只做 logger、patcher、raw Harmony fallback。

所以：

```text
RitsuLib lifecycle events：未迁。
```

---

## 2.3 没有完成 RitsuLib DataStore / persistence

`docs/migration.md` 明确写：

```text
No persistence (BeginModDataRegistration) — existing SavedSpireFields stay.
```



所以：

```text
Urda/Morvi/Lotha 复杂状态仍不是 RitsuLib DataStore。
State codec 仍是下一阶段核心。
```

---

## 2.4 没有完成 settings / content pack migration

`docs/migration.md` 明确写：

```text
No content registration (CreateContentPack)
No settings page (RegisterModSettings)
No persistence
```



所以这轮不能说“全面迁移到 RitsuLib”。

---

## 2.5 FeatureRegistry 仍是 scaffold，不是真正状态管理器

`FeatureRegistry` 当前只是：

```text
Register module
OrderBy InitOrder
EvaluateGate
Initialize
```



`IFeatureModule` 当前也只有：

```text
Id
InitOrder
EvaluateGate
Initialize
```



还没有：

```text
DisplayName
Category
Dependencies
DisableEnvKeys
ForceEnvKeys
BootstrapStatus
LiveStatus
RuntimeStatus
Diagnostics
MultiplayerPolicy
```

这意味着它只是初始化顺序管理，不是完整 feature architecture。

---

# 3. 这轮存在的风险 / 奇怪点

## 3.1 “Pre-existing failures” 不能轻描淡写

Codex 汇报说：

```text
Migration tests: 4/4 pass
Pre-existing failures (Sts1Events, documentation): unrelated
```

这很危险。因为如果完整 test suite 有失败，就不能只说“migration tests 通过”。下一轮必须明确：

```text
dotnet test EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
```

到底是否全绿。

如果 Sts1Events / documentation 失败还存在，必须二选一：

```text
修复
或明确 Skip/Quarantine 并记录 issue
```

不能让 full test suite 带失败进入月度主线。

---

## 3.2 Sts1Events 出现未完成 TODO skeleton

当前仓库有 `Sts1DeadAdventurer.cs`，里面存在：

```csharp
// TODO: Enter combat with random elite
// TODO: Grant random relic
```

实际逻辑是如果 roll 到 elite/relic，只 `SetEventFinished(...)`，不会真的进入精英战或给遗物。

`Sts1Joust.cs` 当前下注时直接扣金币，没有看到 option 是否在金币不足时禁用。

这不是 RitsuLib migration 的一部分，是明显 scope creep / 未完成内容风险。

必须确认：

```text
[ ] Sts1Events 是否被注册进任何 event pool？
[ ] 如果未注册，文档写明 source skeleton only / inactive。
[ ] 如果已注册，必须禁用或完成。
[ ] 加 guard：带 TODO 的 EventModel 不能进入 live registration。
```

---

## 3.3 ModPatcher + raw Harmony 双 patch 风险

当前策略是先 `patcher.PatchAll()`，再 raw `harmony.PatchAll()`。

理论上如果迁移后的 class 已经没有 `[HarmonyPatch]`，就不会被 raw Harmony 再扫一遍。但必须有 guard：

```text
[ ] 所有 RegisterMigratedPatches 中的 class 不得有 [HarmonyPatch]
[ ] 所有 raw Harmony patch class 不得被 RegisterPatch 注册
[ ] PatchId 唯一
[ ] RegisteredPatchCount 与文档 inventory 一致
```

否则未来很容易出现“同一个 patch 被打两次”。

---

## 3.4 RitsuLib compile/runtime version 仍需 live smoke

当前 compile package 是 `STS2.RitsuLib 0.3.2`，runtime variant pack 文档写 variant pack 是 `0.3.3`，并且 `0.106.1` runtime variant 可用，但 NuGet compat `0.106.1` 未发布。

这不一定错，但必须通过真实 loader smoke：

```text
BaseLib 3.1.4
STS2-RitsuLib 0.3.3 variant pack
Spire Plus
```

看 log 里是否有：

```text
RitsuLib framework is active
ModPatcher applied X/Y patches
0 MissingMethodException
0 TypeLoadException
0 manifest dependency failure
```

---

## 3.5 v0.106.0 / v0.106.1 版本口径仍要统一

`docs/integrations/ritsulib.md` 写 current target 是 `v0.106.1`。
但此前 `PROJECT_STATE.md` 仍有 `v0.106.0` 口径。这个必须统一，否则源码证据和 runtime package 都会混乱。

---

# 4. 每一步完成度表

| 项目                           | 状态        | 评价                                      |
| ---------------------------- | --------- | --------------------------------------- |
| RitsuLib NuGet dependency    | 完成        | csproj 已有 `STS2.RitsuLib 0.3.2`         |
| RitsuLib manifest dependency | 完成        | manifest 已有 `STS2-RitsuLib`             |
| RitsuLibBootstrap            | 完成第一步     | logger + patcher + raw Harmony fallback |
| ModPatcher migration         | 部分完成      | 约 9 个低风险 patch class                    |
| Raw Harmony fallback         | 保留        | 合理，但需 double-patch guard                |
| FeatureRegistry              | 部分完成      | 只是 scaffold                             |
| RitsuLib lifecycle           | 未完成       | 没有事件订阅迁移                                |
| RitsuLib DataStore           | 未完成       | SavedSpireField 仍保留                     |
| State codec                  | 未完成       | Urda/Morvi/Lotha 仍需 codec               |
| RitsuLib settings            | 未完成       | BaseLib config 仍保留                      |
| RitsuLib content pack        | 未完成       | 没迁                                      |
| Patch inventory              | 部分完成      | 157 declarations regenerated，但需核对       |
| Full test suite              | 不确定/可能未全绿 | 报告提到 pre-existing failures              |
| Sts1Events                   | 风险        | TODO skeleton 存在                        |
| Release-ready                | 否         | 绝对不能标 ready                             |

---

# 5. 下一步 Monthly Dev Spec

下面是我建议给 Codex 的月度规格。重点：**不要再直接推进更多 patch migration，先把 Batch 4a 的风险闭环，再进入 Batch 4b。**

---

# Monthly Dev Spec：RitsuLib Migration Stabilization Month

## Month Goal

把当前 RitsuLib 迁移从“build passes + 少量 patch migration”推进到：

```text
runtime-proven
double-patch-safe
version-consistent
test-suite-clean
Sts1Events scope-safe
state/pipeline architecture ready
```

不新增玩法，不迁高风险 patch，不声称 release-ready。

---

## Week 1：Runtime Smoke + Version Truth

### 目标

证明 RitsuLib 运行时真的可用，而不仅是编译通过。

### 任务

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

统一到实际 runtime：

```text
v0.106.1
BaseLib 3.1.4
STS2-RitsuLib variant pack 0.3.3
Spire Plus current version
```

2. Clean loader smoke：

```text
Only:
- BaseLib
- STS2-RitsuLib
- Spire Plus
```

必须 log：

```text
RitsuLib bootstrap starting
ModPatcher applied X/Y patches
RitsuLib framework is active
Spire Plus loaded
0 MissingMethodException
0 TypeLoadException
0 manifest dependency failure
```

3. 更新 install docs：

```text
Install order:
1. BaseLib
2. STS2-RitsuLib
3. Spire Plus
```

### Week 1 验收

```text
[ ] clean godot.log 存档
[ ] audit-godot-log 通过
[ ] docs 版本统一
[ ] install docs 包含 RitsuLib
[ ] full dotnet test 没有未解释 failure
```

---

## Week 2：Patch Migration Safety / Batch 4a Closure

### 目标

确保混合 patch 模式不会双 patch，也不会把未迁移 patch 漏掉。

### 任务

1. 生成 patch inventory：

```text
docs/patch-inventory.md
```

分类：

```text
MigratedToRitsuModPatcher
RawHarmonyRemaining
HighRiskBlocked
```

2. 增加 tests：

```text
[ ] RegisterMigratedPatches 中的 class 不得含 [HarmonyPatch]
[ ] 含 [HarmonyPatch] 的 class 不得出现在 RegisterMigratedPatches
[ ] PatchId 唯一
[ ] 每个 IPatchMethod 有 Description
[ ] 每个 IPatchMethod 有 GetTargets
[ ] RegisterMigratedPatches count 与 docs/migration.md 一致
[ ] docs/patch-inventory.md 与 source 一致
```

3. 修复 / quarantine Sts1Events：

```text
[ ] 确认 Sts1DeadAdventurer / Sts1Joust 是否 live registered
[ ] TODO event skeleton 不能进入 live event pool
[ ] 如果未注册，docs 写 inactive source skeleton
[ ] 如果注册了，立即禁用或完成
```

### Week 2 验收

```text
[ ] Batch 4a double-patch risk closed
[ ] Sts1Events scope creep handled
[ ] full test suite green or explicit quarantines
```

---

## Week 3：FeatureRegistry Hardening

### 目标

把 FeatureRegistry 从 wrapper 变成真实 feature status system。

### 任务

扩展 `IFeatureModule`：

```csharp
string DisplayName { get; }
FeatureCategory Category { get; }
IReadOnlyList<string> DisableEnvKeys { get; }
IReadOnlyList<string> ForceEnvKeys { get; }
FeatureBootstrapStatus GetBootstrapStatus();
FeatureLiveStatus GetLiveStatus();
```

分离：

```text
Bootstrap enabled
Live available
```

示例：

```text
VakuuFight:
  bootstrap=enabled
  live=disabled
  reason=requires SPIREPLUS_ENABLE_VAKUU_FIGHT
```

### Week 3 验收

```text
[ ] MainFile 保持短
[ ] 所有 feature 有 DisplayName/Category
[ ] bootstrap/live status log 清楚
[ ] default-on 行为不改变
```

---

## Week 4：State Codec / Architecture Prep

### 目标

开始解决长期 bug 根源：string state 和 lifecycle/reward/death 耦合。

### 任务

1. UrdaStateCodec：

```text
UrdaStateV1
Encode
Decode
MalformedFallback
OldStateMigration
RoundTrip tests
```

2. RitsuLib DataStore POC：

```text
InMemory or harmless Global smoke only
Do not migrate gameplay state yet
```

3. RewardPipeline spec：

```text
RewardPhase
IRewardHandler
Priority
Diagnostics
```

4. CardPlayContext spec：

```text
ExtraPlayPolicy
Power fallback
No recursion
Depth guard
```

5. DeathProtectionService spec：

```text
used flag
inReprieve flag
forced unavoidable death
co-op player owner
```

### Week 4 验收

```text
[ ] UrdaStateCodec tests pass
[ ] DataStore POC documented
[ ] RewardPipeline skeleton/spec exists
[ ] CardPlayContext skeleton/spec exists
[ ] DeathProtectionService spec exists
```

---

# 6. 下一个具体 Goal

## GOAL：RitsuLib Batch 4a Closure + Runtime Proof

### 给 Codex 的 Prompt

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：RitsuLib Batch 4a closure + runtime proof。

当前状态：
- STS2.RitsuLib 0.3.2 已加入 csproj。
- STS2-RitsuLib 已加入 manifest。
- RitsuLibBootstrap 使用 CreatePatcher + raw Harmony fallback。
- Batch 4a 已迁移 Fiddle/ChoicesParadox/DistinguishedCape/BlackStar patches。
- docs/migration.md 记录 Batch 4a done。
- 仍有 pre-existing failures: Sts1Events / documentation。
- Sts1DeadAdventurer 中存在 TODO: fight elite / grant relic。
- Sts1Joust 可能缺少金币不足校验。
- RitsuLib lifecycle/DataStore/settings/content pack 未迁。

不要新增 gameplay。
不要迁高风险 patch。
不要 claim release-ready。

必须使用 subagents：

Subagent A — Runtime/RitsuLib Compatibility:
- 统一 v0.106.x target docs。
- 检查 BaseLib + STS2-RitsuLib + Spire Plus install docs。
- 准备 clean loader smoke checklist。

Subagent B — Patch Migration Safety:
- 扫描 IPatchMethod 和 HarmonyPatch。
- 确认无 double-patch。
- 生成 migrated/raw/high-risk patch inventory。
- 加 PatchId unique tests。

Subagent C — Sts1Events Scope Creep:
- 检查 Sts1DeadAdventurer / Sts1Joust 是否被注册。
- 若未注册，标 inactive source skeleton。
- 若注册，禁用或完成。
- 加 guard：TODO EventModel 不能 live。

Subagent D — Docs/Test Truth:
- 确认 full dotnet test 状态。
- 不允许只说 migration tests pass。
- 任何 pre-existing failure 必须修复或显式 quarantine。
- 更新 docs/migration.md / docs/integrations/ritsulib.md。

任务：
1. 统一版本口径 v0.106.0/v0.106.1。
2. 添加/更新 RitsuLib runtime smoke checklist。
3. 添加 double-patch guards。
4. 处理 Sts1Events TODO skeleton。
5. 确保 docs/migration.md 精确写：
   - Batch 4a done
   - Batch 4b not started
   - high-risk patch migration blocked
6. 运行：
   - dotnet build EZMicroBalance.sln
   - dotnet test EZMicroBalance.sln
   - dotnet test EZMicroBalance.sln --no-build
   - dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
   - git diff --check
7. 如果 source/package 变更需要：
   - dotnet publish
   - release artifact tests

Final report:
- current HEAD
- subagent findings
- whether full test suite is green
- RitsuLib runtime status
- patch migration count
- double-patch guard status
- Sts1Events status
- remaining blockers
- release-ready: no
```

---

# 7. 必须提醒 Codex 使用 subagents

这类任务已经不适合单 agent 一口气做。你应该强制它这样分工：

## Subagent A：Ritsu Runtime Agent

负责：

```text
RitsuLib package/version/runtime variant
install docs
loader smoke checklist
v0.106.x 口径统一
```

## Subagent B：Patch Inventory Agent

负责：

```text
扫描 HarmonyPatch / IPatchMethod
分类 migrated/raw/high-risk
检测 double patch
PatchId unique
```

## Subagent C：Scope Creep Agent

负责：

```text
Sts1Events
TODO skeleton
未注册实验代码
是否进入 live pool
```

## Subagent D：Test Truth Agent

负责：

```text
完整 dotnet test
pre-existing failures
docs 不能掩盖失败
quarantine 规则
```

## Subagent E：Architecture Roadmap Agent

负责：

```text
下月 state codec
RewardPipeline
CardPlayContext
DeathProtectionService
MultiplayerPolicy
```

---

## 最终判断

这轮是有价值的，但必须准确命名：

```text
RitsuLib Batch 4a partial patch migration complete.
RitsuLib full migration not complete.
Runtime proof pending.
Full test truth uncertain due pre-existing failures.
Sts1Events skeleton risk open.
```

下一个月的方向不是继续狂迁 patch，而是：

```text
先 runtime proof，
再 close Batch 4a risks，
再 FeatureRegistry hardening，
再 StateCodec / RewardPipeline / CardPlayContext / DeathProtection。
```

这样项目才会真正变稳定，而不是只是“更多文件实现了 IPatchMethod”。
