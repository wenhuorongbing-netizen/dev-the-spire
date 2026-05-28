# Spire Plus 重构与优化指南：把“大而脏的 mod”收束成可演进的 Modular Monolith

我先给一个关键判断：**现在真正脏的不是字面意义上的 `MainFile.cs`，而是“FeatureRegistry + 166 个 Harmony patch + 静态 partial service + source-shape guard tests”组成的工程复杂度。** `MainFile.Initialize()` 现在只做 RitsuLib bootstrap、配置注册、FeatureRegistry 初始化，入口文件本身已经很薄。 但 `SpirePlusFeatureRegistry` 仍然直接 import 并 hard-code Lotha、Morvi、Urda、Vakuu、Ascension 的初始化顺序，这会让它逐步变成“新的 main”。

我按你上传的审计要求，把下面内容区分成 **Observation / Inference / Recommendation**，并给出可执行路线图。

---

## 1. 当前项目性质判断

### Observation

这是一个 **Slay the Spire 2 mod workspace**，当前玩家可见目标是 `Spire Plus`，但技术 manifest id、资源目录、namespace、DLL/PCK/install folder 仍然稳定保留为 `EZMicroBalance`。这个命名不能在这轮重构里随便改。README 和 AGENTS 都明确要求不要原地改 manifest id。 

当前技术栈大致是：

| 项       | 当前状态                                                                                   |
| ------- | -------------------------------------------------------------------------------------- |
| 产品类型    | Slay the Spire 2 C# / Godot / BaseLib mod                                              |
| Runtime | .NET 9 / Godot .NET 4.5.1                                                              |
| 主要依赖    | BaseLib 3.1.4、Harmony、RitsuLib 0.3.2、Krafs.Publicizer、StS2 runtime DLL                 |
| 构建      | `dotnet build`                                                                         |
| 发布      | `dotnet publish` + Godot export pack                                                   |
| 测试      | xUnit + 大量 source/docs/package guard tests                                             |
| CI      | hosted hygiene workflow + self-hosted Windows full validation + website Pages workflow |
| 主要源码    | `EZMicroBalanceCode/`                                                                  |
| 主要资源    | `EZMicroBalance/`                                                                      |
| 主要测试    | `tests/EZMicroBalance.Tests/`                                                          |
| 当前功能    | Ancient Rebalance、Urda/Morvi/Lotha/Vakuu、Ascension A11-A20、Preview Tools、website/forum |

项目地图已经把 active mod surface 分成 `MainFile.cs`、`Config`、`Core/Features`、`Diagnostics`、`Map`、`Modding`、`Ancients`、`Ascension`、`Preview` 等区域。

### Inference

这个项目不是“完全没治理的垃圾堆”。它的问题更像是：

1. **治理文档很多，但代码边界还没完全兑现。**
2. **patch 数量太多，且高风险 patch 横跨 run、room、save、lobby、multiplayer、UI、reward。**
3. **很多业务逻辑仍然藏在静态 partial service 或 patch 文件周围，难以做真正的行为测试。**
4. **未来功能仍在推进，特别是 docs/goal 里的 test-ready 目标、A11-A20、Ancient 扩展、Preview 工具、co-op fail-closed，会继续放大耦合。**

当前 patch inventory 显示 **166 个 Harmony patch declarations，其中 22 个 high risk**。 这对一个 mod 来说不是“不能接受”，但它必须被当成核心架构风险管理，而不是普通代码风格问题。

### Recommendation

目标架构不要是微服务，也不要拆成多个 mod。正确方向是：

> **Single Mod Modular Monolith + Harmony Adapter Layer + Feature Module Catalog + Policy/Flow/State seams**

也就是保留一个 `Spire Plus` mod，但内部按 feature/context 分成清楚模块。Harmony patch 只做 adapter，不承载业务规则。

---

## 2. docs/goal 对重构的硬约束

你提到要考虑 `docs\goal` 中正在开发的功能，这一点非常重要。当前 `docs/goal.md` 明确说：目标是 **test-ready manual build，不是 release-ready**；loader、clicked UI、gameplay、save/load、failure/death、co-op 都必须靠当前 package 的 live evidence 才能关闭；source-only pass 不能标记 goal complete。

所以重构必须遵守这些规则：

1. **不要因为重构关闭 live proof gate。** 代码变干净不等于 gameplay、save/load、co-op 已验证。
2. **不要改 manifest id / DLL / PCK / install folder。** `EZMicroBalance` 是兼容边界。
3. **不要把 A21-A30、自定义角色塞进这轮。** A11-A20 仍是当前 scope，自定义角色明确 out of scope。
4. **Vakuu 仍然 hidden-by-default，不能因为结构整理就变成默认 live-ready。** Ancient 扩展文档要求未来工作先承认 blockers、更新 issue、检查 source、加 guard 和 manual row，不能直接宣称 live-ready。
5. **Preview tools 必须继续保持 local UI-only / read-only。** Crystal Sphere 和 transform preview 不能调用 reward、reveal、real RNG mutation 路径。
6. **RitsuLib adoption 不能和高风险 patch 迁移混在一起。** migration 文档已经把 PR6+ 分成 bootstrap、future registration、persistence experiment、low-risk wrappers、high-risk patch migrations，最后一类被明确放到 manual evidence backlog 降低之后。

---

## 3. 当前架构评估

### Current Architecture Pattern

当前更像：

> **Modular Monolith in progress + Patch-heavy Adapter Architecture + partial Big Ball of Patch Logic**

它已经不是原始 big ball of mud，因为文档里有 bounded contexts、extension rules、patch boundaries、feature registry 和 module map。bounded-context 文档已经列出 AncientRewardRebalance、AncientExpansionUrda/Morvi/Lotha/Vakuu、AscensionCore、RootDeck、PreviewTools、ReleaseEvidence 等 context。

但它还不是干净的 Clean Architecture，因为：

* Harmony patches 太多；
* 高风险生命周期逻辑仍然依赖 source API、reflection、static state；
* 很多测试是 source-shape guard，而不是独立业务行为测试；
* 部分 policy/flow seam 还只存在于文档，不存在于代码。

文档已经明确要求 patches 应该是 thin adapters，feature services 应该拥有 gameplay decisions，UI 只展示状态和接收 user intent，preview 必须保持纯净。 这就是目标方向。

---

## 4. “Main 太脏”的真实拆解

### Observation

`MainFile.cs` 本身已经不是问题。它只有常量、logger 和 `Initialize()`，而 `Initialize()` 只调用：

```csharp
RitsuLibBootstrap.ApplyPatches(ModId);
ModConfigRegistry.Register(ModId, new SpirePlusModConfig());
SpirePlusFeatureRegistry.CreateDefault().InitializeAll();
```

证据在 `MainFile.cs`。

真正的“main 感”转移到了：

1. `SpirePlusFeatureRegistry.cs`
2. 各 feature initializer
3. Harmony patch inventory
4. static partial services
5. source-text guard tests

`SpirePlusFeatureRegistry` 直接依赖 Urda/Morvi/Lotha/Vakuu/Ascension，并用硬编码 order number 注册。 另外，`AscensionInitializer` 自己也带 `[ModInitializer(nameof(Initialize))]`，同时又被 FeatureRegistry 调用；虽然有 `initialized` guard，但这会让 bootstrap source of truth 变模糊。

### Inference

现在的问题不是“MainFile.cs 太长”，而是：

> **feature bootstrap 和 patch ownership 还没有完全模块化，导致任何新增功能都倾向于往 central registry、static initializer、patch 文件、guard test 里继续堆东西。**

这会带来三类风险：

* 新功能必须改共享启动中心，形成高变更耦合；
* 初始化顺序靠数字和字符串维护，容易出现隐式依赖；
* 未来 A11-A20、Vakuu、Root Sight、Preview Tools、RitsuLib adoption 并行开发时，冲突会集中在 registry、patch、state、docs/test guard 上。

### Recommendation

保留 `MainFile` 现在的薄入口，但把 `SpirePlusFeatureRegistry` 改成真正的 **Feature Catalog**：

```text
EZMicroBalanceCode/
  MainFile.cs
  Bootstrap/
    SpirePlusBootstrap.cs
    FeatureCatalog.cs
  Core/
    Features/
      IFeatureModule.cs
      FeatureRegistry.cs
      FeatureModuleDescriptor.cs
      FeatureGateResult.cs
```

每个 feature 自己提供 module：

```text
Ancients/Expansion/Urda/UrdaFeatureModule.cs
Ancients/Expansion/Morvi/MorviFeatureModule.cs
Ancients/Expansion/Lotha/LothaFeatureModule.cs
Ancients/Expansion/Vakuu/VakuuFightFeatureModule.cs
Ascension/Core/AscensionFeatureModule.cs
Preview/PreviewToolsFeatureModule.cs
```

Registry 只接收 `IEnumerable<IFeatureModule>`，不再知道每个 initializer 的静态方法。这样未来新增 feature 时，不再修改“新的 main”，而是新增一个 feature module。

---

## 5. 目标架构

### Target Architecture Pattern

推荐目标：

```text
Mod Entry
  ↓
Bootstrap / Feature Catalog
  ↓
Feature Modules
  ↓
Application Services / Policies / Flows
  ↓
State Codecs / Domain Models
  ↓
Adapters
    - Harmony patches
    - Godot UI nodes
    - BaseLib config
    - RitsuLib integration
    - Release evidence logger
```

核心原则：

1. **Patch 是 adapter，不是业务逻辑。**
2. **Feature module 是启动单元，不是 God class。**
3. **Policy 负责纯规则。**
4. **Flow 负责状态机和生命周期。**
5. **Codec 负责保存格式。**
6. **UI patch 只读状态和发送 user intent。**
7. **multiplayer-sensitive 行为必须明确 single-player-only / host-authoritative / local UI-only。**

Patch boundary 文档已经写明：Harmony patches 是 entry points，应该 forward into named services；run、room、save、lobby、multiplayer、lifecycle 都是 release-sensitive surface；patch 不能作为 live UI/save-load/co-op proof。

---

## 6. 模块边界地图

| Context / Module         | Responsibility                            | 当前风险 | 推荐边界                                                               |
| ------------------------ | ----------------------------------------- | ---: | ------------------------------------------------------------------ |
| `Bootstrap`              | mod 初始化、feature 顺序、config 注册              |    中 | 只做 composition，不做 feature 逻辑                                       |
| `AncientRewardRebalance` | vanilla Ancient reward 改动                 |   中高 | 独立 `Ancients/Rebalance`，patch adapter + reward policy              |
| `AncientExpansionUrda`   | Urda blessing、Root Eyes、Seed Bank、Seedbed |    高 | `RootSightPreviewPolicy`、`UrdaStateCodecVx`、`UrdaMapSelectionFlow` |
| `AncientExpansionMorvi`  | debt/card state、Forbidden Loan、Open Book  |    高 | card-state policy + save mirror seam                               |
| `AncientExpansionLotha`  | Death Reprieve、cost/play/death lifecycle  |    高 | lethal prevention policy + combat lifecycle flow                   |
| `AncientExpansionVakuu`  | hidden fight、child combat、parent restore  |   极高 | `VakuuFightFlow` 状态机，patch 只转发                                     |
| `AscensionCore`          | A11-A20 enablement、selector、diagnostics   |    高 | `AscensionSelectionPolicy` + `LobbySelectionAdapter`               |
| `AscensionMap`           | A11 map、A17 branches、markers              |    高 | map geometry policy + metadata service                             |
| `AscensionCombat`        | Firemark、Banner、Rootblight、boss abilities |    高 | `BannerCombatPolicy`、`FiremarkWindowPolicy`、boss ability policy    |
| `PreviewTools`           | Crystal Sphere / transform preview        |    中 | `PreviewTransformPolicy`，patch/UI 与 prediction 分离                  |
| `ReleaseEvidence`        | evidence log、package/hash/docs/guard      |    高 | docs/test/scripts owner，不能和 gameplay 混                             |

项目自己的 architecture 文档也已经把最高价值 seam 列出来：RootSightPreviewPolicy、VakuuFightFlow、BannerCombatPolicy / FiremarkWindowPolicy、PreviewTransformPolicy。 Patch boundary 文档也重复列出这些 required seams。

---

## 7. 最高优先级 Clean Code / Architecture Findings

### Issue 1: Patch surface 过大，patch 风险已经成为主架构风险

* **Severity:** 4
* **Priority:** P0
* **Confidence:** High
* **File / Module:** `docs/patch-inventory.md`, `EZMicroBalanceCode/**/Patches`, feature patch files
* **Observation:** 当前 patch inventory 记录 166 个 patch declaration，22 个 high risk。
* **Inference:** 后续任何 A11-A20、Vakuu、Root Sight、co-op、Preview 改动都可能触碰 game lifecycle，patch 数量本身已经是维护和回归风险。
* **Recommendation:** 不要先做目录大搬家。先建立 patch adapter rule：所有高风险 patch 只能做 gate、source object lookup、调用 service、return。
* **Acceptance Criteria:**

  * 每个 high-risk patch group 有 owner；
  * 每个 high-risk patch group 有 service seam；
  * patch inventory 和 patch-boundary 文档数字一致；
  * 新 patch 必须在 PR checklist 中说明 owner / risk / manual evidence row。

---

### Issue 2: `SpirePlusFeatureRegistry` 正在变成新的 main

* **Severity:** 3
* **Priority:** P1
* **Confidence:** High
* **File / Module:** `EZMicroBalanceCode/Core/Features/SpirePlusFeatureRegistry.cs`
* **Observation:** registry 直接引用 Urda/Morvi/Lotha/Vakuu/Ascension initializer，并 hard-code order `100/200/300/400/500`。
* **Inference:** 未来新增 Preview module、RitsuLib module、forum/website hooks 或更多 Ascension slices 时，会继续修改这个 central file。
* **Recommendation:** 把每个 feature 改成独立 `IFeatureModule` implementation。Registry 只排序和执行。
* **Before:**

```csharp
.Register(new DelegateFeatureModule(
    "Ancients.Urda",
    300,
    () => FeatureGateResult.EnabledByDefault(...),
    UrdaInitializer.Initialize))
```

* **After:**

```csharp
internal sealed class UrdaFeatureModule : IFeatureModule
{
    public string Id => "Ancients.Urda";
    public int InitOrder => FeatureOrders.AncientsUrda;
    public FeatureGateResult EvaluateGate() => UrdaFeatureGate.EvaluateBootstrapGate();
    public void Initialize() => UrdaInitializer.Initialize();
}
```

* **Acceptance Criteria:** adding a new feature does not require editing `SpirePlusFeatureRegistry` except adding one catalog line, or ideally only adding to a module list.

---

### Issue 3: Bootstrap source of truth 有轻微混乱

* **Severity:** 3
* **Priority:** P1
* **Confidence:** Medium
* **File / Module:** `MainFile.cs`, `AscensionInitializer.cs`
* **Observation:** `MainFile` 通过 registry 初始化所有模块，但 `AscensionInitializer` 自己也有 `[ModInitializer(nameof(Initialize))]`。 
* **Inference:** `initialized` guard 防止重复初始化，但读代码的人会不确定到底是 mod loader 还是 registry 负责启动 Ascension。
* **Recommendation:** 除非 StS2/Godot loader 必须直接发现该 initializer，否则移除 feature initializer 上的 `[ModInitializer]`，只保留 root `MainFile`。如果必须保留，则新增注释和 guard test，明确这是兼容 fallback，不是主启动路径。
* **Acceptance Criteria:** `MainFile` 是唯一 primary bootstrap，feature initializer 没有隐式 loader path。

---

### Issue 4: 高风险 flow 还藏在 static partial service 和 transient static state 里

* **Severity:** 4
* **Priority:** P0 / P1
* **Confidence:** High
* **File / Module:** `VakuuFightService.*`, `UrdaBlessingService.RootSight*`
* **Observation:** Vakuu child combat 通过 static partial `VakuuFightService` 管理 parent restore、pre-finished save、no-reward victory、fallback map exit，并使用 `ConditionalWeakTable<IRunState,...>` 保存 transient state。  Urda Root Sight selection 也用 static `RootSightSelectionPlayer` 保存 UI selection state，并直接操作 `NMapScreen`。
* **Inference:** 这些代码是最容易在 save/load、co-op、fallback path、black screen 中出问题的地方。
* **Recommendation:**

  * 提取 `VakuuFightFlow`，把状态转移写成可测试状态机；
  * 提取 `RootSightSelectionFlow` 和 `RootSightPreviewPolicy`；
  * static service 只保留 thin facade，真正逻辑放到可构造 class 或纯函数 policy。
* **Acceptance Criteria:** 可以不启动 Godot、不进真实游戏，测试 Vakuu victory/no-parent/save-restore 的状态决策。

---

### Issue 5: Urda save state 是强 primitive obsession，未来极易坏

* **Severity:** 3
* **Priority:** P1
* **Confidence:** High
* **File / Module:** `UrdaStateCodec.cs`, `UrdaProgress`
* **Observation:** `UrdaProgress` 是一个超长 positional record，混合 Seedbed、Humus、Molting、Moss、Trial、RootedRoute、RootSight、SeedBank 等多个概念。 `UrdaStateCodec` 用 `;`、`|`、`~` 和 index offset 解析 legacy/current state。
* **Inference:** 这会让任何新增 Urda blessing 或 save field 都变成高风险 shotgun surgery。
* **Recommendation:** 保持 wire format 不变，但内部引入 versioned DTO 和子状态对象：

```text
UrdaProgress
  SeedbedState
  TrialBranchState
  RootSightState
  SeedBankState
  RootedRouteState
```

* **Acceptance Criteria:** legacy raw string 和 current raw string 都能 round-trip；新增字段必须有版本迁移测试。

---

### Issue 6: 测试体系过度依赖 source-shape guard，行为测试不足

* **Severity:** 3
* **Priority:** P1
* **Confidence:** High
* **File / Module:** `tests/EZMicroBalance.Tests/**`
* **Observation:** 测试 README 明确说测试主要 guard source shape、localization、release docs、package artifacts、runtime evidence。 `AscensionFeatureGuardTests` 中大量断言是 `AssertSourceContains` / `Assert.DoesNotContain` 形式。
* **Inference:** 这类测试能防文档漂移和 source regression，但很难证明复杂 gameplay policy 的正确性。
* **Recommendation:** 保留 source guards，但在 seam 提取后新增纯行为测试。
* **Acceptance Criteria:** RootSight、Vakuu、Banner、Firemark、PreviewTransform 至少各有 2–3 个不读源码字符串的行为测试。

---

### Issue 7: 文档治理很好，但存在 drift

* **Severity:** 2
* **Priority:** P2
* **Confidence:** High
* **File / Module:** `docs/architecture/patch-boundaries.md`, `docs/patch-inventory.md`, `docs/refactor-map.md`
* **Observation:** `patch-boundaries.md` 写 current count 是 137 个 Harmony patch，但 `patch-inventory.md` 现在是 166 个。 
* **Inference:** 这说明文档治理方向对，但 source-of-truth 更新机制还不够自动化。
* **Recommendation:** patch count 只允许 `docs/patch-inventory.md` 做 source of truth；其他文档引用“见 patch inventory”，不要复制数字。
* **Acceptance Criteria:** docs 中不再硬编码 patch 总数，或 CI 检查硬编码数字一致。

---

### Issue 8: Feature gates 和 config 分散在 UI config、env vars、static gate 中

* **Severity:** 2
* **Priority:** P2
* **Confidence:** Medium
* **File / Module:** `SpirePlusModConfig.cs`, `AscensionFeatureGate.cs`, Ancient gates
* **Observation:** `SpirePlusModConfig` 只配置 preview tools。 Ascension gate 则使用大量 `SPIREPLUS_*` / `EZMB_*` env vars 和 static helpers。
* **Inference:** 对 tester/debug 很方便，但长期会让行为开关散落，难以回答“当前 package 到底启用了什么”。
* **Recommendation:** 引入 `SpirePlusRuntimeOptions`，统一从 config/env 解析，feature service 只依赖 typed options。
* **Acceptance Criteria:** 每个 feature module 启动时输出一行 structured gate summary；测试能构造 options，不需要改环境变量。

---

## 8. 推荐目录结构

不要重命名 `EZMicroBalanceCode`，也不要改 `EZMicroBalance` manifest/resource surface。只做内部结构收束。

```text
EZMicroBalanceCode/
  MainFile.cs

  Bootstrap/
    SpirePlusBootstrap.cs
    FeatureCatalog.cs
    FeatureOrders.cs

  Core/
    Features/
      IFeatureModule.cs
      FeatureRegistry.cs
      FeatureGateResult.cs
      FeatureModuleDescriptor.cs
    Config/
      SpirePlusRuntimeOptions.cs
      EnvironmentOptionReader.cs
    Logging/
      SpirePlusLogger.cs
    Multiplayer/
      MultiplayerFeaturePolicy.cs
      NetModeDescriptor.cs
    SourceInterop/
      ReflectionField.cs
      SourceApiGuard.cs
    Integrations/
      RitsuLib/

  Diagnostics/
    ReleaseEvidenceLog.cs
    LiveTestConsoleCommand.cs

  Ancients/
    Common/
      SelectionRelics/
      SavedFields/
      Cards/
    Rebalance/
      Patches/
      Policies/
    Expansion/
      Urda/
        UrdaFeatureModule.cs
        Application/
        Policies/
          RootSightPreviewPolicy.cs
        State/
          UrdaStateCodec.cs
          RootSightState.cs
          SeedBankState.cs
        Patches/
        Ui/
      Morvi/
        Application/
        State/
        Patches/
      Lotha/
        Application/
        State/
        Patches/
      Vakuu/
        Application/
          VakuuFightFlow.cs
        State/
        Patches/
        Combat/

  Ascension/
    Core/
      AscensionFeatureModule.cs
      AscensionFeatureGate.cs
    Selection/
      AscensionSelectionPolicy.cs
      LobbyAscensionAdapter.cs
    Map/
      Policies/
      Patches/
    Combat/
      Policies/
        BannerCombatPolicy.cs
        FiremarkWindowPolicy.cs
      Services/
    Rewards/
    Save/
    Ui/
    Patches/

  Preview/
    PreviewToolsFeatureModule.cs
    CrystalSphere/
    Transform/
      PreviewTransformPolicy.cs
      TransformPredictionService.cs
      Patches/
```

### 防止 `Common/Shared/Utils` 失控的规则

1. **只在两个以上 context 真正复用时才上移到 Common。**
2. `Common` 不允许出现 `Manager`、`Helper`、`Util` 这种万能命名。
3. 上移前必须回答：这是 domain rule、adapter helper、serialization helper、UI helper，还是 test helper？
4. `Common` 里的类不能依赖具体 feature，例如 `Urda`、`Vakuu`、`A20`。
5. 每个 context 只暴露一个小 public/internal surface，其余 implementation 保持 feature-local。

---

## 9. 1–2 周重构计划

### Phase 0：止血与基线固定

| 工作                                                                     | 风险 | 验收标准                                     |
| ---------------------------------------------------------------------- | -: | ---------------------------------------- |
| 更新 `docs/patch-boundaries.md` / `docs/refactor-map.md` 中过期 patch count |  低 | 文档不再和 patch inventory 冲突                 |
| 记录当前 beta.84 source/package/manual proof 状态                            |  低 | 不把 source-only work 当 live proof         |
| 运行 no-game validation 命令                                               |  中 | `dotnet build/test/format/diff check` 通过 |
| PR template 加一项“是否触碰 high-risk patch seam”                             |  低 | 每个相关 PR 都标注 patch owner/risk             |

当前验证命令已经在目标文档中定义：build、test、format、diff check；资源/package 变更还要 publish、package、release artifact tests。

---

### Phase 1：Bootstrap cleanup

| 工作                                                        | 风险 | 验收标准                                 |
| --------------------------------------------------------- | -: | ------------------------------------ |
| 新增 `Bootstrap/FeatureCatalog.cs`                          |  低 | `MainFile` 仍然只有 bootstrap call       |
| 把 `DelegateFeatureModule` 替换成 named feature modules       |  中 | 新增 feature 不需要在 registry 内写匿名 lambda |
| 处理 `AscensionInitializer` 的 `[ModInitializer]` 双入口        |  中 | 启动 source of truth 明确                |
| 把 `VakuuFightInitializer` 从 `VakuuFightRunHook.cs` 移到独立文件 |  低 | 文件名表达真实职责                            |

---

### Phase 2：Patch adapter rule

| 工作                                        | 风险 | 验收标准                         |
| ----------------------------------------- | -: | ---------------------------- |
| 给 high-risk patch 文件加 adapter checklist   |  中 | patch 只 lookup/gate/delegate |
| 低风险 patch 先做 move-only                    |  低 | patch inventory regenerate   |
| 高风险 patch 不在同 PR 做 move + behavior change |  高 | PR checklist 强制分离            |

Patch boundary 文档已经要求移动或新增 patch 后 regenerate inventory。

---

### Phase 3：提取最高价值 seams

优先顺序：

1. `PreviewTransformPolicy`
   这是最低风险，因为 `TransformPredictionService` 已经比较接近纯逻辑。
2. `BannerCombatPolicy` / `FiremarkWindowPolicy`
   A16/A12 需要 live proof，但 policy 可先抽。
3. `RootSightPreviewPolicy`
   高价值、高风险，必须保留 RNG / co-op gate / save format。
4. `VakuuFightFlow`
   极高风险，必须以 characterization tests 包住 parent restore / no reward / fallback map。
5. Ascension selection scope
   把 private reflection、progress temporary override、multiplayer unlock override 包成可测试 scope。

---

### Phase 4：State / Save cleanup

| 工作                       | 风险 | 验收标准                                      |
| ------------------------ | -: | ----------------------------------------- |
| `UrdaProgress` 拆子状态对象    | 中高 | wire format 不变                            |
| 新增 `UrdaStateCodecTests` |  中 | legacy/current round-trip                 |
| 保存字段文档对齐                 |  中 | docs/architecture/save-state-contracts 更新 |
| 禁止没有 migration 的字段重排     |  高 | guard test 覆盖                             |

---

### Phase 5：测试升级

保留现有 guard tests，但新增行为测试：

| Test Name                                                     | Target Module              | Scenario                                   | Priority |
| ------------------------------------------------------------- | -------------------------- | ------------------------------------------ | -------- |
| `RootSightPreview_DoesNotAdvanceLiveRngUntilEntryCommit`      | `RootSightPreviewPolicy`   | preview 只 fork，不 commit live RNG           | P0       |
| `RootSightPreview_RejectsCoopMutationUnlessHostAuthoritative` | `RootSightPreviewPolicy`   | co-op 默认 fail-closed                       | P0       |
| `VakuuFightFlow_VictoryWithParentStackResumesParentEvent`     | `VakuuFightFlow`           | victory 后回 parent event                    | P0       |
| `VakuuFightFlow_NoParentStackFallsBackToMap`                  | `VakuuFightFlow`           | parent stack 缺失时安全打开 map                   | P0       |
| `UrdaStateCodec_RoundTripsLegacyAndCurrentState`              | `UrdaStateCodec`           | legacy/current save 字符串稳定                  | P0       |
| `AscensionSelection_RestoresProgressMaxAscensionOnException`  | `AscensionSelectionPolicy` | BeginRun 异常也恢复 progress                    | P1       |
| `BannerCombatPolicy_ShieldwallTriggersAtEnemyTurnEnd`         | `BannerCombatPolicy`       | shieldwall timing 正确                       | P1       |
| `FiremarkWindowPolicy_ForgeArmorSuppressesOnlyNextArmor`      | `FiremarkWindowPolicy`     | Forge Armor suppress 范围正确                  | P1       |
| `PreviewTransformPolicy_UsesForkedRngOnly`                    | `PreviewTransformPolicy`   | 预测不创建 live card、不推进 real RNG               | P1       |
| `CoopFailClosed_AllowsPreviewButBlocksSharedStateMutation`    | `MultiplayerFeaturePolicy` | preview local UI-only，gameplay mutation 禁止 | P0       |

---

## 10. CI/CD 与工程自动化建议

### Observation

当前 hosted `Repository Hygiene` workflow 主要做 manifest、JSON、docs、patch inventory、whitespace 检查。 full validation 是 self-hosted Windows workflow，需要显式 StS2 和 Godot path。 issues 文档还记录 `GOV-CI-FIRST-RUN` pending，最新 100 个 main runs 找不到完成的 Full Local Validation run。

### Recommendation

CI 分三层：

1. **Hosted CI：不依赖游戏本地路径**

   * docs hygiene
   * JSON/localization validation
   * patch inventory consistency
   * source-shape guard tests that do not require StS2 local install
   * `dotnet format --verify-no-changes` 若依赖可恢复

2. **Self-hosted no-game CI：依赖 StS2/Godot path，但不跑 live game**

   * build
   * test
   * format
   * publish
   * package
   * release artifact opt-in tests

3. **Manual evidence lane：只记录，不伪装成 CI**

   * loader proof
   * clicked UI screenshots
   * gameplay
   * save/load
   * failure/death
   * two-client co-op

不要让 CI 把 source-only guard 的通过误表述成 release-ready。

---

## 11. 项目管理与协作建议

当前项目管理成熟度：**70/100**。

好的一面：

* 有 `PROJECT_STATE.md`、docs index、feature docs、issues、patch inventory、PR template、issue template、ADR template；
* PR template 已要求 manifest id 不变、patch inventory 更新、本地化对齐、manual evidence rows 不能随便关闭。
* issue template 已包含 Problem、Player Impact、Source Evidence、Risk、Acceptance Criteria、Validation Commands、Manual Evidence Needed。

主要问题：

* 当前 blocker 很多是 `source/package-fixed / live-pending`，说明工程交付和手动验证之间积压很大。
* 文档过多，虽然有 active reading path，但仍然容易 drift。
* 技术债还没有完全按 P0/P1/P2 绑定到“玩家风险 / release 风险 / save-load 风险 / co-op 风险”。

### Recommended Definition of Done

任何代码变更必须满足：

1. build 通过；
2. normal tests 通过；
3. format check 通过；
4. `git diff --check` 通过；
5. touched feature docs 更新；
6. touched patch 更新 patch inventory；
7. touched localization 同步 English/zhs；
8. touched save field 更新 save-state contract；
9. touched high-risk lifecycle 行为添加 manual evidence row；
10. source-only pass 不关闭 live proof gate。

---

## 12. 24–48 小时 Quick Wins

| Action                                                  | File / Module                                                   | Expected Impact   | Effort | Owner              |
| ------------------------------------------------------- | --------------------------------------------------------------- | ----------------- | -----: | ------------------ |
| 修正文档 patch count drift                                  | `docs/architecture/patch-boundaries.md`, `docs/refactor-map.md` | 立刻减少治理噪音          |      S | Release/Docs owner |
| 给 `SpirePlusFeatureRegistry` 增加 `FeatureOrders` 常量      | `Core/Features`                                                 | 去掉魔法数字            |      S | Core owner         |
| 把 `VakuuFightInitializer` 移出 `VakuuFightRunHook.cs`     | `Ancients/Expansion/Vakuu`                                      | 文件职责更清楚           |      S | Vakuu owner        |
| 明确 `AscensionInitializer` 的 ModInitializer 是否保留         | `Ascension/Core`                                                | 消除双 bootstrap 疑惑  |    S/M | Ascension owner    |
| 新增 `PreviewTransformPolicy` wrapper                     | `Preview/Transform`                                             | 先从低风险 seam 练手     |      M | Preview owner      |
| 给 Urda state codec 加 round-trip 测试                      | `UrdaStateCodec`                                                | 防 save 格式破坏       |      M | Urda owner         |
| PR checklist 增加 high-risk patch seam 字段                 | `.github/pull_request_template.md`                              | 让 review 更聚焦      |      S | Maintainer         |
| 新增 “source-only cannot close live proof” checklist item | PR/Issue templates                                              | 防错误 release claim |      S | Maintainer         |

---

## 13. 30–60–90 Day Roadmap

### 30 天：止血、基线、边界可见性

* patch inventory / patch boundary / refactor map 数字一致；
* FeatureRegistry 不再用匿名 DelegateFeatureModule 堆 feature；
* low-risk seam 先抽 `PreviewTransformPolicy`；
* Urda save codec 有 round-trip tests；
* 每个 high-risk feature 有 owner + manual evidence row；
* self-hosted full validation 至少成功跑一次并记录。

### 60 天：模块边界、测试体系、CI/CD

* `RootSightPreviewPolicy`、`BannerCombatPolicy`、`FiremarkWindowPolicy` 落地；
* source-shape guard tests 旁边新增行为 tests；
* `Ancients/Common` 和 `Ascension/Core` 清理掉不该共享的 feature-specific helper；
* high-risk patch adapter rule 被 CI 或 guard tests 检查；
* release evidence dashboard 和 issues 自动防 stale hash / stale proof claim。

### 90 天：架构演进、团队协作、技术债治理

* `VakuuFightFlow` 状态机落地，并有 characterization tests；
* Ascension selection / multiplayer downgrade / co-op fail-closed 形成稳定 policy；
* RitsuLib 只迁移低风险 API，高风险 patch 等 manual evidence backlog 降低再做；
* 每个 feature 有明确 owner、state contract、manual validation matrix；
* 技术债 backlog 按 P0/P1/P2/P3 和玩家风险管理。

---

## 14. Scorecard

| 维度           |           分数 | 理由                                                                  |
| ------------ | -----------: | ------------------------------------------------------------------- |
| 架构清晰度 / 12   |            8 | 文档有 bounded contexts 和 dependency rules，但代码 seam 未完全兑现              |
| 模块解耦与边界 / 12 |            6 | 目录已有模块，但 registry、patch、static services 仍强耦合                        |
| 领域建模 / 10    |            5 | gameplay concepts 有命名，但 UrdaProgress 等仍偏 primitive/positional state |
| 代码可读性 / 10   |            6 | Main 变薄，部分 service 拆分，但 patch/static partial 仍难读                    |
| 代码可维护性 / 12  |            6 | 文档和 guard 强，但 166 patches 增加维护成本                                    |
| 可拓展性 / 10    |            6 | extension landmarks 明确，但新增功能仍会碰 registry/patch/state                |
| 可测试性 / 10    |            5 | guard tests 多，行为 seam tests 不足                                      |
| CI/CD / 8    |            5 | workflows 存在，但 full local validation 首次成功 evidence 仍 pending        |
| 项目管理协作 / 8   |            7 | issue/PR/docs 流程较强，但 live-proof backlog 积压                          |
| 文档知识传承 / 5   |            4 | 文档很强，但存在 drift 和过多历史材料                                              |
| 安全稳定生产准备 / 3 |            2 | fail-closed 和 evidence rules 好，但 live/save/co-op proof 未完成          |
| **总分**       | **61 / 100** | 工程治理强于普通 mod，但核心风险集中在 patch surface、save/load、co-op、行为测试不足          |

---

## 15. Final Prioritized Backlog

| Priority | Issue                    | Area         | Recommendation                     | Impact                       | Effort | Metric                            | Acceptance Criteria               |
| -------- | ------------------------ | ------------ | ---------------------------------- | ---------------------------- | -----: | --------------------------------- | --------------------------------- |
| P0       | High-risk patch surface  | Architecture | patch adapter rule + owner map     | 降低回归/黑屏/save/co-op 风险        |      M | high-risk patch groups with seams | 每个 high-risk group 有 service seam |
| P0       | Live proof backlog       | Release      | 不允许 source-only close live rows    | 防错误 release claim            |      S | open/closed evidence rows         | docs/goal closure rules 保持        |
| P1       | FeatureRegistry 变成新 main | Bootstrap    | named feature modules              | 降低新增功能冲突                     |      M | registry churn                    | 新 feature 不写匿名 lambda             |
| P1       | Vakuu child combat flow  | Vakuu        | `VakuuFightFlow`                   | 降低黑屏/save/death 风险           |      L | characterization tests            | victory/fallback/save cases 可测    |
| P1       | Root Sight preview       | Urda         | `RootSightPreviewPolicy`           | 降低 RNG/co-op/map mutation 风险 |      L | policy tests                      | preview 不推进 live RNG              |
| P1       | Urda save state          | Save         | 子状态 + versioned codec              | 降低 save/load breakage        |      M | round-trip tests                  | legacy/current raw state 稳定       |
| P1       | Ascension selection      | Ascension    | selection policy + scoped restore  | 降低 lobby/progress/co-op 风险   |      M | behavior tests                    | exception restore covered         |
| P2       | Documentation drift      | Docs         | patch count single source of truth | 降低误导                         |      S | zero stale counts                 | docs 不复制 patch count              |
| P2       | Config gates scattered   | Config       | typed runtime options              | 提高 tester/debug 清晰度          |      M | gate summary logs                 | 每个 feature 输出 gate summary        |
| P2       | Preview transform seam   | Preview      | policy + patch adapter split       | 低风险示范重构                      |      M | pure policy tests                 | preview 不 mutate real state       |

---

## 最终结论

**这个项目当前最应该优先解决的是 high-risk Harmony patch surface 的 service seam 和行为测试缺口，因为它直接影响 Vakuu、Root Sight、Ascension A11-A20、Preview Tools、save/load、co-op 这些正在开发功能能否安全演进。**
