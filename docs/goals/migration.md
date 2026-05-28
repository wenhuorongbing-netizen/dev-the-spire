# 严格验收结论

**没有全部达成“全面迁移到 RitsuLib”的目标。**
但它确实比上一轮前进了一大步：**RitsuLib 已经从“仅规划”变成了真实 compile dependency + runtime dependency + bootstrap logging layer**，并且 `MainFile` 已经通过轻量 `FeatureRegistry` 解耦了直接 initializer 调用。

现在的真实状态应该定义为：

```text
RitsuLib Migration Phase 0/1: partially complete
RitsuLib hard dependency: yes
RitsuLib runtime dependency: yes
RitsuLib bootstrap diagnostics: yes
FeatureRegistry scaffold: yes
RitsuLib lifecycle migration: no
RitsuLib DataStore migration: no
RitsuLib content-pack migration: no
RitsuLib ModPatcher migration: no
State codec migration: no
Reward pipeline: no
CardPlayContext: no
DeathProtectionService: no
MultiplayerPolicy: no
Release-ready: no
```

也就是说：**这是 RitsuLib 接入 + 架构骨架阶段，不是全面迁移完成。**

---

## 1. GitHub 当前状态确认

最新远端提交已经不是之前我看到的 `2f0afd`，而是：

```text
591b596 migration middle
```

提交列表里能看到你提到的那组 RitsuLib 迁移提交：`737acab`、`29d1b93`、`7d6d736`、`49435f0`、`2fd2b1a`、`6001a6e`，并且后面又有 `780c65d new era` 和 `591b596 migration middle`。

这次它确实已经 push 到 GitHub 了，不是之前那种“本地 dirty / branch 找不到”的状态。

---

# 2. 按迁移目标逐项验收

## 2.1 RitsuLib NuGet dependency

**完成。**

当前 `EZMicroBalance.csproj` 已经包含：

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All"/>
```

同时 BaseLib 仍然保留为：

```xml
<PackageReference Include="Alchyr.Sts2.BaseLib" Version="3.1.4" PrivateAssets="All"/>
```



这符合我们原定的“不是立刻删除 BaseLib，而是先引入 RitsuLib 做架构层”的路线。

### 风险

当前使用的是 `STS2.RitsuLib` base package `0.3.2`，而 runtime variant pack 文档里写 runtime pack 是 `0.3.3`，并且 NuGet 没有 `STS2.RitsuLib.Compat.0.106.1`。`docs/integrations/ritsulib.md` 已明确记录：runtime variant `0.106.1` 可用，但 NuGet compat package 缺失，所以现在用 base NuGet 0.3.2 编译，运行时使用 variant pack 0.3.3。

这不是立刻错误，但必须实机验证：

```text
BaseLib 3.1.4 + STS2-RitsuLib runtime 0.3.3 variant + Spire Plus
```

是否 clean load。

---

## 2.2 Manifest runtime dependency

**完成。**

当前 `EZMicroBalance.json` dependencies 已包含：

```json
{
  "id": "STS2-RitsuLib",
  "min_version": "0.3.2"
}
```

同时仍依赖 BaseLib `v3.1.4`。

### 风险

`docs/integrations/ritsulib.md` 写 runtime variant pack manifest version 是 `0.3.3`，但项目 manifest 要求 `min_version: 0.3.2`。这通常没问题，因为 0.3.3 >= 0.3.2。但测试包说明里必须写清：

```text
玩家需要安装 STS2-RitsuLib variant pack，而不是普通单 DLL。
```

文档已有安装目录说明：`<GameRoot>/mods/STS2-RitsuLib/`，包含 root loader、manifest、variant config 和 `lib/0.106.1/`。

---

## 2.3 RitsuLib bootstrap

**部分完成。**

`MainFile.Initialize()` 现在已经调用：

```csharp
RitsuLibBootstrap.ApplyPatches(ModId);
ModConfigRegistry.Register(ModId, new SpirePlusModConfig());
SpirePlusFeatureRegistry.CreateDefault().InitializeAll();
```

这比原来直接 `new Harmony(ModId).PatchAll()` + 所有 initializer 要好。

`RitsuLibBootstrap` 当前会：

```csharp
var logger = RitsuLibFramework.CreateLogger(modId);
logger.Info($"RitsuLib {GetRitsuLibVersion()} bootstrap starting.");
var harmony = new Harmony(modId);
harmony.PatchAll();
logger.Info($"Harmony patches applied via {modId}.");
if (RitsuLibFramework.IsActive) ...
```



### 关键问题

它并没有使用 RitsuLib 的 `CreatePatcher` / `ModPatcher` / `IPatchMethod` / `IModPatchProvider`。文档自己也承认：

* 当前仍然 raw Harmony；
* 63 个 patch class 如果要迁到 RitsuLib managed patcher，需要实现 `IPatchMethod` 或 `IModPatchProvider`；
* PR 6 Batch 1 只是 bootstrap + diagnostics；
* Batch 4 patch class migration blocked。

所以这里是：

```text
RitsuLib bootstrap logging: yes
RitsuLib patcher migration: no
```

---

## 2.4 FeatureRegistry / 初始化解耦

**部分完成。**

`MainFile` 已经不直接调用 Lotha/Morvi/Urda/Vakuu/Ascension initializer，而是走 `SpirePlusFeatureRegistry.CreateDefault().InitializeAll()`。

`FeatureRegistry` 会按 `InitOrder` 初始化 module，并打印：

```text
[Spire Plus] Feature {module.Id} bootstrap gate: enabled/disabled (reason)
```



`SpirePlusFeatureRegistry` 当前注册了：

```text
Ancients.Lotha
Ancients.Morvi
Ancients.Urda
Ancients.VakuuFight
Ascension.A11A20
```

并分别代理到旧的 initializer。

### 仍未完成

`IFeatureModule` 目前只有：

```text
Id
InitOrder
EvaluateGate()
Initialize()
```



`FeatureGateResult` 也只有：

```text
bool IsEnabled
string Reason
```



这还不是完整 feature architecture。它缺：

```text
DisplayName
DefaultEnabled
DisableEnvKeys
ForceEnvKeys
Dependencies
RuntimeStatus
Diagnostics
MultiplayerPolicy
PackageEvidenceStatus
LiveVerificationStatus
```

更重要的是，`SpirePlusFeatureRegistry` 目前的 gate 文案是：

```text
default-on; Lotha runtime gates remain in LothaFeatureGate.
default-on; Morvi runtime gates remain in MorviFeatureGate.
```



也就是说：**FeatureRegistry 现在只是 bootstrap gate，不是真正的 feature live gate。**

这可以接受作为第一阶段，但不能说完全解耦。

---

## 2.5 RitsuLib migration docs

**部分完成，但目录设计和我们之前 spec 不一致。**

我没有在 GitHub main 找到：

```text
docs/features/ritsulib-migration/README.md
```

但我找到了：

```text
docs/integrations/ritsulib.md
docs/migration.md
docs/refactor-map.md
```

`docs/integrations/ritsulib.md` 记录 RitsuLib hard dependency、runtime variant pack、NuGet package status、upgrade path 和 API adoption plan。

`docs/migration.md` 记录 PR sequence，包含 PR1–PR6 的状态，说明 PR5 hard dependency done，PR6 Batch 1 bootstrap + diagnostics done，RitsuLib patch class migration blocked。

`docs/refactor-map.md` 记录目录重构计划，并明确 `Core/Integrations/RitsuLib/` 是 future RitsuLib bootstrap module。

### 问题

我们之前建议的是：

```text
docs/features/ritsulib-migration/
  migration-decision.md
  base-vs-ritsulib-comparison.md
  migration-spec.md
  migration-plan.md
  risk-register.md
```

现在实际是散在：

```text
docs/integrations/ritsulib.md
docs/migration.md
docs/refactor-map.md
```

这不是功能错误，但长期阅读成本仍偏高。建议后续把这些整理到 `docs/features/ritsulib-migration/`，然后保留旧路径 redirect/summary。

---

## 2.6 RitsuLib lifecycle event migration

**未完成。**

当前 bootstrap 没有 `SubscribeLifecycle<TEvent>`。`RitsuLibBootstrap` 只创建 logger、调用 raw Harmony PatchAll、检查 `RitsuLibFramework.IsActive`。

`docs/migration.md` 也没有声称 lifecycle 已迁，只说 Batch 1 bootstrap + diagnostics done。

### 判定

```text
Lifecycle migration: not started
```

---

## 2.7 RitsuLib DataStore / persistence migration

**未完成。**

`docs/migration.md` 直接写：

```text
No persistence (BeginModDataRegistration) — existing SavedSpireFields stay.
```

并且 Batch 3 “Persistence sidecar experiments” 标成 not applicable / existing SavedSpireFields work。

当前 `AncientSavedStateFields` 仍是大量 `SavedSpireField<Player, string>` / `SavedSpireField<CardModel, string>`。
Urda 仍然有 string state parse/write 逻辑。

### 判定

```text
DataStore migration: not done
State codec migration: not done
```

这仍是下月的核心目标。

---

## 2.8 Content registration migration

**未完成。**

`docs/migration.md` 明确写：

```text
No content registration (CreateContentPack) — Spire Plus doesn't register new cards/relics/potions through RitsuLib.
```



### 判定

```text
RitsuLib content pack migration: not done
```

这个可以暂缓，不是 P0。

---

## 2.9 RitsuLib settings migration

**未完成。**

`docs/migration.md` 写：

```text
No settings page (RegisterModSettings) — existing BaseLib config stays.
```



### 判定

```text
Settings migration: not done
```

暂缓可接受。

---

## 2.10 Patch migration to RitsuLib ModPatcher

**未完成，而且文档已正确标记为 blocked。**

`docs/migration.md` 写：

```text
Patch classes still use [HarmonyPatch] attributes, not RitsuLib's IPatchMethod/IModPatchProvider interfaces.
Migrating 63 patch classes to RitsuLib's managed ModPatcher is a future batch.
```



这是合理的，因为一次迁 63 个 patch 很危险。

---

# 3. 本轮是否完成“全面迁移到 RitsuLib”？

严格说：**没有。**

完成的是：

```text
[✓] 添加 STS2.RitsuLib NuGet dependency
[✓] 添加 STS2-RitsuLib runtime dependency
[✓] 添加 RitsuLibBootstrap
[✓] 用 RitsuLib logger 打 bootstrap diagnostics
[✓] MainFile 使用 FeatureRegistry
[✓] 建立轻量 FeatureRegistry scaffold
[✓] 记录 migration plan 和 blocker
[✓] build 至少本地报告 0 warnings / 0 errors
```

未完成的是：

```text
[ ] RitsuLib lifecycle events
[ ] RitsuLib DataStore
[ ] RitsuLib content pack
[ ] RitsuLib settings
[ ] RitsuLib ModPatcher
[ ] BaseLib dependency reduction
[ ] typed state codec
[ ] RewardPipeline
[ ] CardPlayContext
[ ] DeathProtectionService
[ ] MultiplayerPolicy
[ ] behavior-level tests
[ ] clean Steam loader smoke with BaseLib + RitsuLib + Spire Plus
[ ] co-op Ritsu dependency parity
```

所以应写成：

```text
RitsuLib Phase 0/1 complete.
Full migration not complete.
```

---

# 4. 存在的可能 bug / 奇怪内容

## 4.1 RitsuLib compile/runtime version mismatch

当前 compile 使用 `STS2.RitsuLib` 0.3.2，runtime variant pack 是 0.3.3。

这可能没问题，但必须实机验证。尤其是：

```text
RitsuLibFramework.CreateLogger
RitsuLibFramework.IsActive
```

在 0.3.2 compile / 0.3.3 runtime 下必须行为一致。

## 4.2 当前文档说 “current target v0.106.1”，但 PROJECT_STATE 仍多处写 v0.106.0

`PROJECT_STATE.md` 当前 game target 写 `v0.106.0`。
但 `docs/integrations/ritsulib.md` 写 current repo StS2 target 是 `v0.106.1`，runtime variant 也是 `0.106.1`。

这是一个明确的不一致。

### 必须修

统一到底是：

```text
v0.106.0
```

还是：

```text
v0.106.1
```

如果游戏已更新到 v0.106.1，就 `PROJECT_STATE.md` 必须更新。如果仍是 v0.106.0，就 RitsuLib integration doc 的 “current repo target v0.106.1” 不应这么写。

这个会直接影响源码证据和 runtime dependency。

---

## 4.3 FeatureRegistry gate 只是文字，不是真正 gate

当前 registry 总是 `EnabledByDefault(...)`，真实可用性仍在各 feature gate 内部。

这可能导致 log 误导：

```text
Feature VakuuFight bootstrap gate: enabled
```

但实际 fight entry hidden by `VakuuFightFeatureGate`。

建议改成两层状态：

```text
BootstrapRegistered
LiveAvailable
```

日志写：

```text
Feature VakuuFight: bootstrap=enabled, live=disabled, reason=requires SPIREPLUS_ENABLE_VAKUU_FIGHT
```

---

## 4.4 RitsuLib hard dependency 会影响测试安装

Manifest 已经要求 `STS2-RitsuLib`。
这意味着现在测试员必须安装：

```text
BaseLib
STS2-RitsuLib
Spire Plus
```

如果测试员没装 RitsuLib，游戏应该无法加载 Spire Plus。需要更新所有 tester handoff / install docs / website download instructions。

---

## 4.5 `docs/features/ritsulib-migration/` 缺失

这不是运行 bug，但和我们之前“monthly migration plan”不一致。现在文档分散在 `docs/integrations/ritsulib.md`、`docs/migration.md`、`docs/refactor-map.md`。

建议下月第一步整理。

---

# 5. 是否应该继续推进？

是，但目标要改成：

```text
从“接入 RitsuLib”进入“用 RitsuLib 重构高收益低风险部分”。
```

不要马上迁 63 个 Harmony patch，不要马上迁所有 persistence。`docs/migration.md` 把 high-risk patch migrations 标成 blocked 是对的。

---

# 6. 下一步 Monthly Dev Spec

下面是我建议的 30 天开发规格。它不要求每天固定产出，但每周一个主目标。

---

# Monthly Dev Spec：Spire Plus RitsuLib Stabilization & Architecture Month

## 目标总述

本月目标不是新增内容，而是让当前超大 Spire Plus 项目进入“可稳定测试、可维护、可逐步迁移”的状态。

核心成果：

```text
1. RitsuLib runtime install + loader smoke 真实通过。
2. v0.106.x 目标版本统一。
3. FeatureRegistry 从轻量 wrapper 变成真实 feature status registry。
4. Urda/Morvi/Lotha 的 state codec 开始替换脆弱 string 状态。
5. RewardPipeline / CardPlayContext / DeathProtectionService 至少完成设计和一个最小实现。
6. 多人策略矩阵落地。
7. 所有测试员文档更新为 BaseLib + RitsuLib + Spire Plus。
```

---

## Week 1：RitsuLib Environment & Evidence Closure

### Goal

让 RitsuLib 依赖变成**真实可安装、可加载、可审计**，而不是只在 csproj/manifest 里存在。

### Tasks

#### 1.1 统一 v0.106 target

修正文档冲突：

```text
PROJECT_STATE.md: v0.106.0
docs/integrations/ritsulib.md: v0.106.1
```

必须确定当前实际游戏版本，然后统一：

```text
[ ] PROJECT_STATE.md
[ ] docs/integrations/ritsulib.md
[ ] docs/dev-environment.md
[ ] docs/release-checklist.md
[ ] docs/private-beta-verification-handoff.md
[ ] website install docs
```

### Acceptance

```text
[ ] 所有 current docs 只写一个当前目标版本。
[ ] 如果 v0.106.1 是 runtime target，source code/sourcecodeonlyaianalysis 也要记录是否匹配。
```

#### 1.2 Clean loader smoke

必须实机跑：

```text
BaseLib 3.1.4
STS2-RitsuLib variant pack 0.3.3
Spire Plus
```

检查：

```text
[ ] RitsuLib loaded
[ ] BaseLib loaded
[ ] Spire Plus loaded
[ ] RitsuLib bootstrap log present
[ ] RitsuLibFramework.IsActive true
[ ] Found expected SavedSpireFields
[ ] no MissingMethodException
[ ] no TypeLoadException
[ ] no manifest dependency failure
```

### Acceptance

```text
[ ] clean godot.log saved
[ ] audit-godot-log passes
[ ] docs/release-evidence-status.md updated
```

#### 1.3 Install docs update

更新：

```text
README.md
README_INSTALL.txt
website download/install page
docs/private-beta-verification-handoff.md
docs/platform-testing.md
```

写清：

```text
Install order:
1. BaseLib
2. STS2-RitsuLib variant pack
3. Spire Plus
```

### Week 1 Definition of Done

```text
[ ] v0.106 target docs unified
[ ] RitsuLib loader smoke passed
[ ] install docs include RitsuLib
[ ] package evidence updated if needed
```

---

## Week 2：FeatureRegistry Hardening

### Goal

把现有轻量 registry 从“wrapper”提升为“真实状态 registry”。

### Tasks

#### 2.1 Extend IFeatureModule

当前接口只有 `Id / InitOrder / EvaluateGate / Initialize`。 扩展到：

```csharp
string DisplayName { get; }
FeatureCategory Category { get; }
IReadOnlyList<string> DisableEnvKeys { get; }
IReadOnlyList<string> ForceEnvKeys { get; }
FeatureRuntimeStatus GetRuntimeStatus();
```

#### 2.2 分离 BootstrapEnabled 和 LiveAvailable

避免现在这种：

```text
VakuuFight bootstrap enabled, but live fight hidden
```

新增：

```text
FeatureBootstrapStatus
FeatureLiveStatus
```

日志：

```text
Feature VakuuFight:
  bootstrap=enabled
  live=disabled
  reason=requires SPIREPLUS_ENABLE_VAKUU_FIGHT
```

#### 2.3 Feature status diagnostics

新增命令/日志：

```text
[Spire Plus Feature Status]
Ancients.Urda: bootstrap yes, live yes
Ancients.Morvi: bootstrap yes, live yes
Ancients.Lotha: bootstrap yes, live yes
Ancients.VakuuFight: bootstrap yes, live no
Ascension.A11A20: bootstrap yes, live yes/co-op fail-closed
```

### Week 2 Definition of Done

```text
[ ] Registry logs real bootstrap/live status.
[ ] MainFile remains simple.
[ ] Tests cover feature status output.
[ ] No default-on behavior changed.
```

---

## Week 3：State Codec + Persistence Bridge

### Goal

不要再让大型状态依赖 `;` 分隔 string + index。

### Tasks

#### 3.1 UrdaStateV1

从 Urda 开始。当前 `UrdaBlessingService.State.cs` 按 `;` split 并 index 解析大量字段。

新增：

```text
UrdaStateV1
UrdaStateCodec
UrdaStateMigration
```

先保留原 `SavedSpireField<string>`，但读写都走 codec。

#### 3.2 MorviStateV1 / LothaStateV1 spec

不一定本周全改，但要写 spec：

```text
Morvi debt/openbook/borrowed state
Lotha mirror/reprieve/verdict/evidence state
```

#### 3.3 RitsuLib DataStore POC

本周目标不是立刻替换所有 SavedSpireField，而是做一个低风险 POC：

```text
[ ] InMemory store smoke
[ ] maybe RunSidecar research
[ ] no gameplay dependency yet
```

### Week 3 Definition of Done

```text
[ ] UrdaStateCodec roundtrip tests
[ ] malformed state tests
[ ] old state migration tests
[ ] no gameplay regression
[ ] RitsuLib DataStore POC documented
```

---

## Week 4：Domain Pipelines

### Goal

建立真正降低 bug 的 domain architecture。

### Tasks

#### 4.1 RewardPipeline skeleton

建立：

```text
RewardPipeline
RewardPipelineContext
RewardPhase
IRewardHandler
```

先只做 diagnostics，不迁全部逻辑。

阶段：

```text
BeforePopulate
AfterPopulate
ModifyOptions
ModifyOptionsLate
AddAlternatives
OnPicked
OnSkipped
OnCompleted
```

覆盖文档/诊断：

```text
Urda Seedbed
Urda Humus
Morvi Forbidden Loan
Morvi Debt Settlement
Lotha Closed Court
Prismatic Gem
Fission
A19 Boss Reward
```

#### 4.2 CardPlayContext skeleton

建立：

```text
EzmbCardPlayContext
ExtraPlayPolicy
```

先接入 Morvi Misprint 或 Lotha SingleSentence 的一个低风险入口。

规则：

```text
Power fallback only
No recursion
No clone/autoplay first-card trigger
Depth guard
```

#### 4.3 DeathProtectionService design + minimal guard

建立：

```text
DeathProtectionService
DeathProtectionRequest
DeathProtectionResult
```

先不全迁 Lotha Death Reprieve，但把 forced death / in-resolution policy 写清，并加 tests/guards。

#### 4.4 MultiplayerPolicy document

创建/更新：

```text
docs/features/multiplayer-safety-policy.md
```

标注：

```text
Urda
Morvi
Lotha
Vakuu
Rootblight
Ascension BossSeal
Preview tools
```

### Week 4 Definition of Done

```text
[ ] RewardPipeline diagnostics exists
[ ] CardPlayContext exists and one effect uses it or has adapter
[ ] DeathProtectionService design exists
[ ] MultiplayerPolicy doc exists
[ ] Tests pass
```

---

# 7. Monthly Acceptance Criteria

这个月结束时，必须满足：

```text
[ ] RitsuLib runtime install and loader smoke passed.
[ ] v0.106 target docs unified.
[ ] FeatureRegistry has real status semantics.
[ ] MainFile stays short and registry-driven.
[ ] UrdaStateCodec implemented and tested.
[ ] RitsuLib DataStore POC completed or explicitly blocked.
[ ] RewardPipeline skeleton exists.
[ ] CardPlayContext skeleton exists.
[ ] DeathProtectionService design/minimal guard exists.
[ ] MultiplayerPolicy matrix exists.
[ ] Install docs include BaseLib + RitsuLib + Spire Plus.
[ ] No new gameplay scope creep.
[ ] Manual pending rows remain honest.
```

---

# 8. Concrete Next Goal

我建议下一步先做：

```text
GOAL-2026-05-28-RITSULIB-LOADER-SMOKE-AND-FEATURE-STATUS-HARDENING
```

## Prompt

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：RitsuLib loader smoke + feature status hardening。

当前状态：
- RitsuLib 0.3.2 已作为 NuGet dependency。
- Manifest 已添加 STS2-RitsuLib min_version 0.3.2。
- Runtime variant pack docs 记录 0.106.1 variant exists.
- MainFile uses RitsuLibBootstrap.ApplyPatches and SpirePlusFeatureRegistry.
- FeatureRegistry currently only wraps existing initializers.
- Full RitsuLib migration is not complete.

不要新增 gameplay。
不要关闭默认开启功能。
不要迁移高风险 patch。
不要 claim release-ready。

任务：
1. 统一 v0.106 target docs：
   - PROJECT_STATE.md
   - docs/integrations/ritsulib.md
   - docs/release-evidence-status.md
   - docs/private-beta-verification-handoff.md
   - website install docs if applicable

2. 更新 install docs：
   - BaseLib 3.1.4
   - STS2-RitsuLib variant pack
   - Spire Plus
   - Windows/macOS install and hash instructions

3. FeatureRegistry hardening:
   - Add DisplayName, Category, DisableEnvKeys, ForceEnvKeys.
   - Add BootstrapStatus vs LiveStatus distinction.
   - Log both.
   - Keep current default-on behavior unchanged.
   - Vakuu should log bootstrap enabled but live hidden unless gate set.

4. Tests:
   - RitsuLib dependency exists in csproj.
   - STS2-RitsuLib dependency exists in manifest.
   - MainFile uses RitsuLibBootstrap and FeatureRegistry.
   - FeatureRegistry reports bootstrap/live distinction.
   - Docs no longer conflict on v0.106.0 vs v0.106.1.

5. Validation:
   - dotnet build
   - dotnet test
   - dotnet test --no-build
   - dotnet format --verify-no-changes --no-restore
   - git diff --check
   - If package/docs changed only, do not publish unless needed.

Final report:
- current HEAD
- files changed
- target version unified to what
- RitsuLib dependency status
- FeatureRegistry changes
- tests run
- remaining blockers
- release-ready: no
```

---

## 最终判断

这次不是失败，但也不是“完成全面迁移”。

它完成了：

```text
RitsuLib hard dependency
runtime dependency
bootstrap diagnostics
FeatureRegistry wrapper
migration docs
```

它没完成：

```text
RitsuLib lifecycle
RitsuLib DataStore
RitsuLib ModPatcher
state codec
reward pipeline
card play context
death protection
multiplayer policy
live loader smoke
```

所以下个月的主题应该是：

```text
RitsuLib stabilization + feature status + state codec + domain pipelines
```

而不是继续扩功能。
