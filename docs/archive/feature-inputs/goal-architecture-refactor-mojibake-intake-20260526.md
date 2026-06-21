# Spire Plus Goal Guard

Current target: test-ready manual build, not release-ready.

## Active implementation notes

- Keep player-facing name `Spire Plus`; keep `EZMicroBalance` only where it is the stable technical manifest id, resource folder, namespace, saved-field prefix, or legacy environment alias.
- Keep Vakuu's Sere Talon separate from Tanx Claws: Sere Talon offers 4 Curses, choose 1, then adds 2 Wish and 1 Wish+; Tanx Claws remains the Maul+ transform relic.
- Keep recent source-level polish focused: A20 selector localization, direct-gain feedback, Elite Root payoff feedback, Seedbed / Planting clarity, light elite damage reduction, co-op fail-closed hardening, and Royal Decree safety.
- Crystal Sphere and transform-preview live proof inside Spire Plus is still required; source review alone does not prove multiplayer or reconnect safety.
- Archive long prompt dumps under `docs/archive/feature-inputs/`; see `goal-md-mojibake-intake-20260523.md`, `goal-coop-preview-plan-20260525.md`, and `goal-preview-plan-intake-20260526.md`.

## 结论：你的直觉基本对，但要精确一点

这个项目不是“完全没有 OOP”。它现在有很多 C# 类，也用了 `CustomCardModel`、`CustomAncientModel`、`AbstractModel` hook、Power、Relic、Card 这类游戏本身的对象模型。但真正的问题是：

```text
它是“OOP 外壳 + 大量 static service / partial service / 字符串状态 / patch 串联”的结构。
```

所以 bug 多不只是“没继承/没 abstract”这么简单，而是下面这些问题叠在一起：

```text
1. 模块初始化没有统一注册层。
2. Feature gate 分散，每个系统自己判断开关。
3. Reward / Combat / Death / Map / SaveLoad 都由很多 static service 分散处理。
4. 状态编码大量靠 string / bool / WeakTable / CardModel field，缺少 typed state object。
5. 多个系统改同一个生命周期，没有统一 pipeline / priority / ownership。
6. tests 很多是 source-string guard，不是行为级单元测试或集成测试。
7. 多人模式没有统一 MultiplayerPolicy。
8. 大功能已经 source-implemented，但 live evidence 还没覆盖。
```

也就是说，**bug 多确实和架构耦合、状态所有权不清、缺少 OOP 边界有关**。

---

# 1. 当前项目已经是大型玩法包，不是小 mod 了

`PROJECT_STATE.md` 当前记录的 active feature areas 已经包括：

* Ancient reward rebalance v4；
* Ascension 11–20；
* Rootblight；
* Urda；
* Morvi；
* Lotha；
* hidden Vakuu fight；
* Preview tools；
* multiplayer mismatch diagnostics；
* package / website / art / release evidence。

这说明当前项目规模已经接近“多系统 DLC 包”。但现在很多代码结构仍然像“单个 mod 功能持续追加”那样写。

最明显的例子是 `MainFile.Initialize()`：它直接创建 Harmony、注册 config，然后直接调用 `LothaInitializer.Initialize()`、`MorviInitializer.Initialize()`、`UrdaInitializer.Initialize()`、`VakuuFightInitializer.Initialize()`、`AscensionInitializer.Initialize()`。

这不是灾难，但它说明当前没有一个统一的：

```text
FeatureRegistry
FeatureModule
FeatureGate
InitOrder
RuntimeStatus
DependencyGraph
```

所以随着模块增加，初始化顺序、enable/disable、诊断、测试隔离都会越来越难。

---

# 2. 和游戏源码相比，项目没有充分利用“对象生命周期模型”

我检查了你上传的 v0.106 code-only 源码包。游戏源码本身其实是很强的 OOP / lifecycle hook 风格：

* `AbstractModel` 有大量 virtual hooks，例如 `BeforeCombatStart`、`AfterCardPlayed`、`AfterCardDrawn`、`AfterCombatEnd`、`AfterActEntered` 等。
* `CardPileCmd.Add(...)` 在 v0.106 里已经有 `clonedBy` 参数，说明官方命令 API 通过 source object / command context 来传递行为来源。
* `CardReward` 有 `OnSelect()`、`OnSkipped()`、`Reroll()`、`Populate()` 等对象级生命周期。
* `StartRunLobby` 有明确 lobby / BeginRun / ascension / preferred progress / multiplayer message flow。

也就是说，游戏本身的抽象思路是：

```text
模型对象 + 生命周期 hook + command API + 明确 owner/context。
```

而我们项目里很多地方变成了：

```text
static service + static patch + shared string state + weak table context + source-string tests。
```

这就是“看起来有类，但核心逻辑不是面向对象”的地方。

---

# 3. 主要架构问题逐项分析

## 3.1 初始化耦合：MainFile 直接依赖所有大模块

当前 `MainFile.Initialize()` 直接依赖所有 feature initializer。

问题：

```text
[ ] MainFile 知道所有模块细节。
[ ] 模块之间没有统一 metadata。
[ ] 无法统一打印 feature enabled/disabled reason。
[ ] 无法优雅做 test profile，比如只开 Ascension / 只开 Morvi。
[ ] 无法统一处理初始化失败。
[ ] 未来每加一个系统都要改 MainFile。
```

应该改成：

```csharp
FeatureRegistry.Register(new AscensionFeatureModule());
FeatureRegistry.Register(new AncientRebalanceFeatureModule());
FeatureRegistry.Register(new UrdaFeatureModule());
FeatureRegistry.Register(new MorviFeatureModule());
FeatureRegistry.Register(new LothaFeatureModule());
FeatureRegistry.Register(new VakuuFeatureModule());
FeatureRegistry.Register(new PreviewToolsFeatureModule());

FeatureRegistry.InitializeAll();
```

每个 module 提供：

```csharp
interface IFeatureModule
{
    string Id { get; }
    string DisplayName { get; }
    int InitOrder { get; }
    FeatureGateResult EvaluateGate();
    void Register();
    FeatureRuntimeStatus GetStatus();
}
```

这样日志能变成：

```text
[Spire Plus] Feature Morvi: enabled=true, reason=default-on
[Spire Plus] Feature VakuuFight: enabled=false, reason=requires SPIREPLUS_ENABLE_VAKUU_FIGHT
[Spire Plus] Feature Ascension: enabled=true
```

---

## 3.2 Feature gate 分散，缺少统一语义

Morvi 和 Lotha 现在都有自己的 gate，逻辑是 default-on，disable env var 关闭。Morvi 支持 `EZMB_DISABLE_MORVI` / `SPIREPLUS_DISABLE_MORVI`，也支持 force ancient / force blessing。 Lotha 也是同样模式。

这符合你现在“默认开没问题”的方向。但问题是：这些 gate 都是手写的，各自实现。长期会出现：

```text
[ ] 某个 feature 支持 SPIREPLUS_*，另一个只支持 EZMB_*。
[ ] force gate 和 disable gate 优先级不统一。
[ ] docs 说默认开，但 source 某处实际默认关。
[ ] test 不知道该用哪个 env var。
```

建议统一成：

```csharp
FeatureGate
FeatureGateResult
FeatureGateReason
```

示例：

```csharp
FeatureGateResult EnabledByDefault(string featureId);
FeatureGateResult DisabledByEnv(string featureId, string env);
FeatureGateResult ForcedByEnv(string featureId, string env);
```

并统一 env 规则：

```text
SPIREPLUS_DISABLE_MORVI=1
EZMB_DISABLE_MORVI=1

SPIREPLUS_FORCE_ANCIENT=MORVI
EZMB_FORCE_ANCIENT=MORVI

SPIREPLUS_FORCE_MORVI_BLESSING=morvi_debt_settlement
EZMB_FORCE_MORVI_BLESSING=morvi_debt_settlement
```

---

## 3.3 状态模型太脆：字符串状态 + previous saved-state API 多，但没有 typed codec

`AncientSavedStateFields.cs` 现在有很多 `previous saved-state API`，包括 Urda/Morvi/Lotha 的 player state、deck state、card marker。 这说明项目已经在认真做存档。但问题是：很多状态最终仍是 `string`。

Urda 是典型例子。`UrdaBlessingService.State.cs` 用 `AncientPlayerState.Get(...)` 取出 string，然后用 `;` 分隔，按 index 解析一长串字段；写入时又 `string.Join(ProgressSeparator, ...)` 拼接几十个字段。

这会导致：

```text
[ ] 字段顺序一错，旧存档就解析错。
[ ] 新增字段只能靠 parts.Length 猜版本。
[ ] 字段里如果包含分隔符，要靠 sanitize。
[ ] 很难单独测试某个状态。
[ ] 很难做 save migration。
[ ] 多人/断线重连时状态不一致很难排查。
```

这不是“不能用 string”，而是必须有 typed codec：

```csharp
record UrdaStateV1
{
    string SelectedBlessing;
    SeedbedState Seedbed;
    HumusState Humus;
    MoltingState Molting;
    MossMapState MossMap;
    TrialBranchState TrialBranch;
    RootSightState RootSight;
    int Version = 1;
}

interface IFeatureStateCodec<TState>
{
    string Encode(TState state);
    TState Decode(string raw);
    TState Default { get; }
}
```

然后 tests 覆盖：

```text
[ ] Decode empty string -> Default
[ ] Decode malformed string -> Default / partial fallback
[ ] Decode old version -> migrate
[ ] Encode/Decode round-trip
[ ] field with delimiter is escaped or rejected safely
```

---

## 3.4 `ConditionalWeakTable` 用作 UI 上下文，高风险

Urda 早期和当前 reward flow 都有类似：把 `CardReward` 对象作为 key，保存 reward context。`UrdaRunHook` / `UrdaBlessingService` 里曾用 `ConditionalWeakTable<CardReward, CardRewardContext>` 来识别当前卡牌奖励是不是一幕普通战斗奖励、是否已记录 Seedbed、是否已处理 Humus skip。

这是可以作为 runtime UI session cache，但不能当作 save/load 状态。

风险：

```text
[ ] 在 reward screen 保存/退出，WeakTable 消失。
[ ] Continue 后 reward object 重新构造，context 不在。
[ ] 玩家可以通过 save/load 重复触发 Seedbed/Humus。
[ ] skip / alternative 可能丢失来源信息。
```

建议把状态分层：

```text
PersistentState：必须保存到 previous saved-state API
RuntimeSessionState：可以 WeakTable，但不保证 save/load
UiRenderState：只用于显示
```

并在每个 service 文件里明确：

```csharp
// UI-session-only. Not save/load persistent.
```

同时在 manual matrix 里加：

```text
Save on reward screen before alternative.
Save after alternative click.
Save after skip before Humus completion.
Continue and verify no duplicated HP/card/gold.
```

---

## 3.5 Reward 系统缺少统一 pipeline，多个 feature 会互相踩

现在很多系统都在改 reward：

```text
Urda Seedbed
Urda Humus Pact
Morvi Forbidden Loan
Morvi Debt Settlement
Lotha Closed Court
Prismatic Gem
A13 Fission
A19 Boss reward +1 option
Ancient reward rebalance
```

Morvi 的测试 guard 里也可以看到它涉及 option relic、reward candidates、Forbidden Loan、Debt Settlement、generated-card guards、OpenBook sealed cards 等。

如果没有统一 pipeline，就会出现：

```text
[ ] Closed Court 移除 CardReward 后，Fission 还想改 CardReward。
[ ] Prismatic reroll 后，Seedbed context 指向旧 reward。
[ ] Boss reward +1 和 Fission 先后顺序不稳定。
[ ] Humus skip 和 other skip listener 同时触发。
[ ] Debt Settlement / Forbidden Loan 打开选择界面时 reward 已完成或未完成。
```

建议建立：

```csharp
EzmbRewardPipeline
RewardContext
RewardHandlerPriority
IRewardHandler
```

阶段：

```text
BeforePopulate
AfterPopulate
ModifyOptions
ModifyOptionsLate
AddAlternatives
BeforeSelect
AfterSelect
OnSkipped
OnCompleted
```

每个 feature handler 声明：

```csharp
FeatureId
Priority
CanHandle(context)
Handle(context)
```

这不是为了“抽象而抽象”，而是让 bug 可排查。log 可以写：

```text
[SpirePlus RewardPipeline]
source=Encounter
room=Monster
act=1
handlers=PrismaticGem,Fission,UrdaSeedbed
alternatives=UrdaSeedbed
skippedHandlers=HumusPact
```

---

## 3.6 Combat extra-play 需要统一 execution context

Morvi/Lotha 有大量 extra-play / replay / verdict / mirror / sentence 逻辑。测试 guard 已经检查它们避免 `CreateClone`、避免 `CardCmd.AutoPlay`、检查 `!card.IsClone`、`cardPlay.IsAutoPlay`、Power fallback 等。

但现在很多规则大概率散在各自 service 里。这会导致组合 bug：

```text
Misprint Press 额外打出的牌触发 Single Sentence。
Single Sentence 额外打出的牌触发 Mirror Rebuttal。
Mirror Rebuttal 额外打出的牌触发 Deferred Verdict。
Power fallback 触发了“本回合第一张牌”规则。
AutoPlay 被当成玩家真实打出。
```

需要一个统一对象：

```csharp
EzmbCardPlayContext
```

包含：

```text
SourceFeature
SourceEffect
IsExtraPlay
IsReplay
IsAutoPlay
IsClone
SuppressSameFeature
Depth
OriginalCardId
OriginalCardInstance
```

所有 extra-play 前都检查：

```csharp
if (context.Depth > 0 && context.SourceFeature == currentFeature) return;
if (card.Type == CardType.Power) return PowerFallback;
if (card.IsClone || cardPlay.IsAutoPlay) return;
```

这能把“能力牌安全规则”从散落 if 变成统一 policy。

---

## 3.7 Death protection 应该从 Lotha 里拆成服务

Lotha guard 显示它现在已经碰到了 `ShouldDieLate`、`ShouldDie`、`AfterPreventingDeath`、`CreatureCmd.Kill(player.Creature, force: true)`。

这是最需要 OOP/架构保护的地方。死亡保护不是普通 blessing effect，它是全局规则，应该由：

```text
EzmbDeathProtectionService
```

统一管理。

需要处理：

```text
[ ] priority：Lotha Death Reprieve、Urda After Rain、可能未来 Fairy / vanilla
[ ] once-per-run
[ ] once-per-combat
[ ] in-death-resolution flag
[ ] force-unpreventable death
[ ] enemy-turn interruption
[ ] co-op player ownership
[ ] save/load while reprieved
```

否则最容易出现：

```text
[ ] 缓期失败死亡又被缓期保护
[ ] 强制死亡被其他保护救下
[ ] 敌方行动队列继续执行，玩家第二次死亡
[ ] 多人里一个玩家死亡，另一个也被错误影响
[ ] save/load 后 inReprieve 状态丢失
```

---

## 3.8 Multiplayer 缺少 policy，导致每个功能自己猜

`PROJECT_STATE.md` 仍明确说 multiplayer co-op verification matrix pending，A11-A20 co-op fail-closed / diagnostics 仍在验证中。

现在这些功能都可能改共享状态：

```text
Urda：奖励、牌组、房间奖励、HP、金币
Morvi：债务、牌组、临时牌、能量、HP
Lotha：死亡、出牌限制、奖励抑制、debuff
Rootblight：牌组、生成牌、战后状态
Ascension：地图、Boss、奖励、战斗增强
```

应该给每个 effect 标注：

```text
LocalUiOnly
LocalPlayerOnly
HostAuthoritative
SharedRunState
CombatCommandReplicated
UnsafeInMultiplayer
```

然后 code review 时要求：

```text
[ ] 改 deck/gold/hp 必须有 policy。
[ ] 本地 UI 提示必须 LocalContext.IsMe 或等价。
[ ] map metadata 必须 host authoritative。
[ ] reward mutation 不能 host/client 双触发。
[ ] combat generated card 必须通过 command API。
```

这是多人稳定的前提。

---

## 3.9 Tests 过度依赖 source-string guard

`AncientExpansionReleaseCoverageGuardTests.cs` 里大量使用 `Assert.Contains(...)` 去检查源码字符串，例如 Morvi 的 blessing ids、Power 名、常量、`!card.IsClone`、`CardType.Attack or CardType.Skill`、`CreateClone` 不存在等。

这类测试有价值，能防止明显删错。但它的问题是：

```text
[ ] 代码里有字符串，不代表行为正确。
[ ] 顺序不对也能过。
[ ] state 保存坏了也能过。
[ ] UI softlock 也能过。
[ ] 多人双触发也能过。
[ ] 文案说完成，但 live 没测也能过。
```

下一层需要补：

```text
unit tests for state codec
simulation tests for reward pipeline
simulation tests for card play context
diagnostic logs for live runs
manual-test evidence required for issue closure
```

---

# 重构路线：不要重写，做 Strangler Refactor

不要一次性重写全部。建议用“绞杀式重构”：新框架慢慢包住旧 service，每次只搬一个高风险区域。

## Milestone A：FeatureRegistry + Gate 统一

目标：

```text
MainFile 不直接知道所有模块细节。
```

改动：

```text
Core/Features/IFeatureModule.cs
Core/Features/FeatureRegistry.cs
Core/Features/FeatureGateResult.cs
```

验收：

```text
[ ] MainFile 只注册 modules
[ ] 每个 module log enabled reason
[ ] Morvi/Lotha/Urda 默认开启不变
[ ] Vakuu hidden 不变
[ ] Disable/force gate 全部照旧
```

---

## Milestone B：State Codec

目标：

```text
把复杂 string state 变成 typed state + codec。
```

先做 Urda，因为 Urda state 现在最明显。`UrdaBlessingService.State.cs` 当前一长串 `string.Join` 和 index parse 是很典型的重构对象。

验收：

```text
[ ] UrdaStateV1 Encode/Decode
[ ] malformed fallback
[ ] old short state migration
[ ] round-trip tests
[ ] progress fields named, not index-only
```

然后再 Morvi / Lotha。

---

## Milestone C：RewardPipeline

目标：

```text
统一 reward 修改顺序。
```

先不搬所有，只建 pipeline docs + diagnostics，然后逐步把 Urda / Fission / Prismatic / ClosedCourt 纳入。

验收：

```text
[ ] reward source/room/act/handlers log
[ ] handler priority documented
[ ] skipped/select/complete phase 分清
[ ] save/load weak context risk 写清
```

---

## Milestone D：CardPlayContext

目标：

```text
统一 extra-play / replay / verdict / fallback 规则。
```

先覆盖：

```text
Morvi Misprint Press
Lotha Mirror Rebuttal
Lotha Single Sentence
Lotha Deferred Verdict
```

验收：

```text
[ ] Power fallback 不进 extra-play
[ ] AutoPlay/Clone 不触发 first-card rules
[ ] 同 feature 不递归
[ ] depth guard
[ ] diagnostics
```

---

## Milestone E：DeathProtectionService

目标：

```text
Lotha Death Reprieve 不再孤立处理死亡。
```

验收：

```text
[ ] force death unblockable
[ ] in-death-resolution flag
[ ] co-op player ownership
[ ] save/load policy
[ ] source evidence for ShouldDie/ShouldDieLate order
```

---

## Milestone F：MultiplayerPolicy annotations

目标：

```text
每个高风险 effect 都声明多人策略。
```

验收：

```text
[ ] mutate HP/gold/deck/reward 必须有 policy
[ ] local UI only 使用 local player guard
[ ] shared map metadata host authoritative
[ ] co-op test matrix 自动生成
```

---

# 给 Codex 的重构 prompt

```text
你现在在仓库 D:\Game\FOTN\dev-the-spire。

目标：Spire Plus architecture decoupling pass。不要新增 gameplay。不要关闭 Morvi/Lotha/Urda 默认开启。不要回滚已有内容。只做架构解耦、状态 codec、pipeline、执行上下文、死亡保护、多人策略、测试护栏。

必须先读：
1. PROJECT_STATE.md
2. AGENTS.md
3. EZMicroBalanceCode/MainFile.cs
4. EZMicroBalanceCode/Ancients/Common/AncientSavedStateFields.cs
5. EZMicroBalanceCode/Ancients/Expansion/Urda/UrdaBlessingService.State.cs
6. EZMicroBalanceCode/Ancients/Expansion/Urda/**
7. EZMicroBalanceCode/Ancients/Expansion/Morvi/**
8. EZMicroBalanceCode/Ancients/Expansion/Lotha/**
9. EZMicroBalanceCode/Ancients/Expansion/Vakuu/**
10. EZMicroBalanceCode/Ascension/**
11. tests/EZMicroBalance.Tests/AncientExpansionReleaseCoverageGuardTests.cs
12. source code/src/Core/**
13. sourcecodeonlyaianalysis/**

硬规则：
- 不要实现新祝福。
- 不要改 manifest id。
- 不要默认关闭当前默认开启功能。
- 不要大规模重写。
- 每次只做可验证的小步。
- 不要 claim release-ready。

Phase 1：FeatureRegistry

新增：
- EZMicroBalanceCode/Core/Features/IFeatureModule.cs
- EZMicroBalanceCode/Core/Features/FeatureRegistry.cs
- EZMicroBalanceCode/Core/Features/FeatureGateResult.cs

重构 MainFile：
- MainFile 不再直接调用所有 Initializer。
- MainFile 注册 feature modules。
- FeatureRegistry 负责按 InitOrder 初始化。
- 每个 feature log id/enabled/reason/env gates。
- 现有默认开启/隐藏逻辑不能改变。

Phase 2：UrdaState codec

新增：
- UrdaStateV1
- UrdaStateCodec

替换 UrdaBlessingService.State.cs 中 index-heavy parse/string.Join 逻辑。
保留 previous saved-state API<string>，但通过 codec 读写。
支持：
- empty state
- malformed state
- old short state
- full current state
- round-trip tests

Phase 3：Reward pipeline diagnostics

新增轻量：
- EzmbRewardPipeline docs 或 code wrapper
- Reward handler priority map

先记录，不强行搬完所有逻辑。
必须列出：
- Urda Seedbed
- Urda Humus
- Prismatic Gem
- Fission
- Lotha Closed Court
- Morvi Forbidden Loan/Debt Settlement

加 diagnostics:
- reward source
- room type
- active handlers
- alternatives
- skip handlers

Phase 4：CardPlayContext

新增：
- EzmbCardPlayContext
- ExtraPlayPolicy

先覆盖 Morvi/Lotha extra-play 入口。
要求：
- Power card fallback only
- AutoPlay/clone 不递归
- same feature depth guard
- source diagnostics

Phase 5：DeathProtectionService

新增：
- EzmbDeathProtectionService

把 Lotha Death Reprieve 最危险的 flags/forced death logic 集中。
要求：
- used flag
- inReprieve flag
- forced unavoidable death flag
- co-op player identity
- source evidence docs

Phase 6：MultiplayerPolicy docs + annotations

新增：
- docs/features/multiplayer-safety-policy.md

每个高风险 feature 标注：
- LocalUiOnly
- LocalPlayerOnly
- HostAuthoritative
- SharedRunState
- CombatCommandReplicated
- UnsafeInMultiplayer

至少覆盖：
- Urda
- Morvi
- Lotha
- Vakuu
- Rootblight
- Ascension BossSeal/Fission/Banner/Firemark

Phase 7：tests

新增/更新 tests：
- MainFile should use FeatureRegistry
- UrdaStateCodec roundtrip/malformed/old version
- ExtraPlayPolicy blocks Power copy/replay
- DeathProtectionService has forced death guard
- multiplayer policy doc exists and covers active features
- source-string tests can stay, but do not rely only on them for new codec/policy tests

Validation:
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check
- publish only if source/resources changed and package refresh is needed

Final report:
- What was decoupled
- What remained old/static
- Which modules now use FeatureRegistry
- UrdaState codec status
- Reward pipeline status
- ExtraPlay context status
- Death protection status
- Multiplayer policy status
- tests results
- remaining architecture debt
- release-ready: no
```

---

## 最后判断

你这个判断是对的：

```text
bug 多，确实有一部分来自架构不够解耦、状态模型太脆、hook 顺序不统一、缺少执行上下文和多人策略。
```

但不要简单理解成“少 abstract / 少 inheritance”。真正要补的是：

```text
Feature module abstraction
State codec abstraction
Reward pipeline abstraction
Card play execution context
Death protection service
Multiplayer policy annotation
Runtime diagnostics
Behavior tests beyond source-string guard
```

这几个做完，项目的可维护性会明显提升；否则继续加内容，bug 会越来越像“随机出现”，因为根本不知道是谁在哪个生命周期里改了哪个状态。
