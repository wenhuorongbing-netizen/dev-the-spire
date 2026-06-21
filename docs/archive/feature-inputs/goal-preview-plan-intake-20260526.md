# Spire Plus Goal Guard

Current target: test-ready manual build, not release-ready.

Active implementation notes:
- Keep player-facing name `Spire Plus`; keep `EZMicroBalance` only where it is the stable technical manifest id, resource folder, namespace, saved-field prefix, or legacy environment alias.
- Keep Vakuu's Sere Talon separate from Tanx Claws: Sere Talon offers 4 Curses, choose 1, then adds 2 Wish and 1 Wish+; Tanx Claws remains the Maul+ transform relic.
- Keep recent source-level polish focused: A20 selector localization, direct-gain feedback, Elite Root payoff feedback, Seedbed / Planting clarity, light elite damage reduction, and co-op fail-closed crash hardening.
- Archive long prompt dumps under `docs/archive/feature-inputs/`; see `goal-md-mojibake-intake-20260523.md` and `goal-coop-preview-plan-20260525.md`.

我不认为“永久 gate 掉多人功能”是最好的最终解。**最好的最终解是：预见功能在多人里也生效，但必须和真实游戏状态分层。**

我会把它做成三类：

```text
1. 纯 UI 预见：水晶球、右侧变换预览卡。
   多人里可以直接启用，因为不改游戏状态。

2. 确定性预提交预见：地图未来房间、未知房间、怪物/事件。
   多人里由共享 seed 生成同一份 plan，host/client 都显示，进入房间时按 plan 执行。

3. 奖励/选牌类预见：卡牌奖励、Fission、Seed Bank、Prismatic Gem 等。
   不能直接改 vanilla reward index；要做 host-authoritative reward plan，否则最容易断线。
```

所以，你说“不要 gate”，可以。**但不能用“直接全开旧逻辑”的方式做。** 旧逻辑全开会继续让 host/client 对 reward、choice、RNG 的理解不同，然后掉线或闪退。正确做法是：**不 gate 预见功能，但把预见功能改成 preview layer / precommitted plan，而不是临时改 vanilla PlayerChoice。**

---

# 1. 游戏随机数到底怎么工作

杀戮尖塔 2 的随机数不是每次随机现场生成，而是确定性 RNG：

```text
Seed + Counter -> 下一个结果
```

也就是说，只要两边的：

```text
Seed 相同
Counter 相同
候选列表相同
调用顺序相同
```

那么结果就相同。

项目里的源码调研已经确认，游戏把随机数拆成很多条流。例如：

```text
PlayerRngSet:
- Rewards
- Shops
- Transformations

RunRngSet:
- UnknownMapPoint
- Combat
- Treasure
- 其他 run 级随机
```

多人同步时，combat sync 会同步 serialized players、run RNG、shared relic bag；client 会从收到的快照恢复 run RNG。

所以单机预见很简单：

```csharp
var fork = new Rng(source.Seed, source.Counter);
var predicted = fork.NextItem(options);
```

关键是：

```text
用 fork 预测，不推进真实 source RNG。
```

当前变换预览代码已经按这个方向做：`TransformPredictionRngContext` 保存 source RNG 的 seed/counter，`TryConsume()` 再 `new Rng(snapshot.Seed, snapshot.Counter)` 做预测。

这在单机里是对的。

---

# 2. 多人为什么会不同步

多人里至少有两份游戏状态：

```text
host 状态
client 状态
```

它们必须在关键节点一致：

```text
当前房间一致
当前 reward options 一致
当前 card reward alternative 一致
当前 PlayerChoice index 一致
当前 RNG counter 一致
当前 map point metadata 一致
```

如果一边多调用一次 RNG，或者多插入一个 reward alternative，就会变成：

```text
host:
  option 0 = 拿卡
  option 1 = 存入 Seed Bank

client:
  option 0 = 拿卡
  option 1 = 跳过
```

然后玩家点击 index 1，host 和 client 理解不同，就会出现：

```text
PlayerChoiceSynchronizer 同步错位
RewardSetSynchronizer 状态不同
host/client 断开
UI 退出时还在 SyncLocalChoice
严重时闪退
```

你上传的日志里确实出现了 `CardReward.OnSelect -> PlayerChoiceSynchronizer.SyncLocalChoice` 相关问题。v0.106 源码审计也指出，`CardRewardAlternative` 会调用 `Hook.ModifyCardRewardAlternatives`，并且 alternatives 超过两个会抛异常；`CardReward` 同步的是一个 card-or-alternative choice index，并通过 `AfterSelected` 决定 reward 是否完成。

所以多人里最危险的不是“看一眼”，而是：

```text
插入 reward alternative
修改 reward options
打开额外选卡界面
在 client 和 host 上各自 roll RNG
```

---

# 3. gate 是什么，为什么我之前提它

Gate 的意思是：

```text
这个功能当前没有多人同步证明，所以多人里先不执行这个高风险路径。
```

比如：

```csharp
if (!MultiplayerFeaturePolicy.IsSingleplayer(runState))
{
    return false;
}
```

这不是最终设计，只是止血。

现在项目里 `MultiplayerFeaturePolicy` 已经有三类开关：

```text
SPIREPLUS_ALLOW_UNVERIFIED_COOP_COMBAT_HOOKS
SPIREPLUS_ALLOW_UNVERIFIED_COOP_PREVIEW_TOOLS
SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY
```

如果设置对应环境变量，它会允许未验证路径在 co-op 中运行并打 evidence log。

所以你现在如果只是想**强行直接测试**，可以设置：

```powershell
$env:SPIREPLUS_ALLOW_UNVERIFIED_COOP_PREVIEW_TOOLS='1'
$env:SPIREPLUS_ALLOW_UNVERIFIED_COOP_GAMEPLAY='1'
$env:EZMB_RELEASE_EVIDENCE_LOG='1'
```

但我不推荐把这当最终方案。这个只是“强开未验证路径”，不是“稳定多人实现”。

你想要的“不要 gate”，最终应该是：

```text
移除 gate 的原因不是无脑放行，
而是这个功能已经被改造成多人安全实现。
```

---

# 4. 最好的解决方案：多人预见协议

我建议做一个统一的：

```text
Spire Plus Foresight Protocol
```

它的设计原则：

```text
预见层和真实游戏层分开
UI-only 预见不改状态
会影响未来结果的预见必须预提交
多人共享的结果必须 host-authoritative 或 deterministic-by-shared-seed
不新增 vanilla PlayerChoice
不改变 reward index
异常时回退 vanilla，不崩
```

---

# 5. 水晶球预见：可以直接多人启用

水晶球是最简单的。

当前代码在 `_Ready()` 里加按钮，只改 `%ScryMask.Modulate.A`，而且明确不调用 `ClearCell`、`RevealItem`、`CellClicked`、`AddReward`。

这类功能是：

```text
local UI-only
不改 RNG
不改 reward
不改 choice
不改 minigame cell state
```

所以多人里不需要 host-authoritative。每个玩家本地显示即可。

当前代码里还有一层：

```csharp
ShouldDisableUnverifiedCoopPreviewTool(...)
```

如果你不想 gate，这一段可以改成只记录 log，不 return：

```csharp
if (!MultiplayerFeaturePolicy.IsSingleplayer(RunManager.Instance?.DebugOnlyGetState()))
{
    ReleaseEvidenceLog.Log(
        "PreviewCrystalSphere",
        "coop_local_ui_preview_enabled",
        runState: RunManager.Instance?.DebugOnlyGetState(),
        data: new Dictionary<string, object?>
        {
            ["reason"] = "Crystal Sphere peek only changes local mask alpha."
        });
}
```

然后继续创建按钮。

**这部分我认为可以直接多人启用。**

---

# 6. 卡牌变化预见：做 local UI-only，然后升级 host-authoritative

当前 `TransformPreviewPatch` 已经把 prediction queue 绑定到了具体 `NTransformPreview` 实例，用 `ConditionalWeakTable<NTransformPreview, PredictionQueue>`，这比之前的全局 queue 安全。

`TransformPredictionRngContext` 也会记录 source seed/counter，并用 fork 预测。

## 6.1 第一版：多人本地 UI-only 预见

多人里每个玩家只看自己当前打开的变换界面：

```text
host 打开自己的变换界面 -> host 本地预测
client 打开自己的变换界面 -> client 本地预测
不要给远端玩家的选择界面生成 UI
不要发送 PlayerChoice
不要修改 Reward
不要推进真实 RNG
```

这种方案不需要网络消息，最容易马上测试。

关键补丁：

```csharp
internal static bool CanShowTransformPrediction(Player owner)
{
    // 单机允许
    if (MultiplayerFeaturePolicy.IsSingleplayer(owner.RunState))
        return true;

    // 多人允许，但仅限 UI-only preview。
    // 不新增 PlayerChoice，不新增 Reward，不改真实 RNG。
    return true;
}
```

然后在 `PreparePredictions()` 中不是 gate，而是改成：

```csharp
var owner = transformations[0].Original.Owner;

if (!CanShowTransformPrediction(owner))
{
    ReleaseEvidenceLog.Log(
        "PreviewTransform",
        "prediction_skipped_not_local_preview_surface",
        owner);
    return;
}

ReleaseEvidenceLog.Log(
    "PreviewTransform",
    "coop_ui_only_prediction_enabled",
    owner);
```

另外 `CycleThroughCards` 的 prefix 要做 fail-open：

```csharp
private static bool Prefix(
    NTransformPreview __instance,
    NPreviewCardHolder holder,
    CardPile cardPile,
    ref Task __result)
{
    try
    {
        if (!SpirePlusModConfig.EnableTransformPrediction ||
            !SpirePlusModConfig.TransformPredictionAlwaysOn)
        {
            return true;
        }

        if (!PredictionsByPreview.TryGetValue(__instance, out var predictions) ||
            predictions.Pending.Count == 0)
        {
            return true;
        }

        var predicted = predictions.Pending.Dequeue();
        if (predicted == null)
        {
            return true;
        }

        holder.Hitbox.MouseFilter = Control.MouseFilterEnum.Stop;
        holder.ReassignToCard(predicted, cardPile.Type, null, ModelVisibility.Visible);

        ReleaseEvidenceLog.Log(
            "PreviewTransform",
            "prediction_displayed",
            data: new Dictionary<string, object?>
            {
                ["card"] = predicted.Id.Entry
            });

        __result = Task.CompletedTask;
        return false;
    }
    catch (Exception ex)
    {
        ClearPredictions(__instance);

        ReleaseEvidenceLog.Log(
            "PreviewTransform",
            "prediction_display_failed_fallback_vanilla",
            runState: RunManager.Instance?.DebugOnlyGetState(),
            data: new Dictionary<string, object?>
            {
                ["exception"] = ex.GetType().Name
            });

        return true; // 回到 vanilla 轮播，不崩
    }
}
```

这可以避免 UI 生命周期异常导致闪退。

## 6.2 第二版：host-authoritative 变化预见

如果你要更强：host 和 client 都看到同一个准确预测，就做 host-authoritative。

数据结构：

```csharp
internal readonly record struct TransformForesightKey(
    int ActIndex,
    int Floor,
    int PlayerSlot,
    string SourceName,
    uint Seed,
    int Counter,
    string OriginalCardId,
    string PileType,
    int PileIndex);

internal sealed record TransformForesightRecord(
    TransformForesightKey Key,
    string ReplacementCardId,
    bool UpgradedPreview);
```

流程：

```text
1. host 捕获 source RNG seed/counter。
2. host fork RNG 算 replacement。
3. host 保存 record。
4. client 请求/接收 record。
5. 两边 UI 显示 record.ReplacementCardId。
6. confirm 后 host 执行真实 transform。
7. host 验证 actual == predicted。
8. mismatch 则写 log 并本局禁用该 source preview。
```

这个是最终最好方案，但要研究 vanilla 的 multiplayer message bus，不能硬塞进 PlayerChoice。

---

# 7. 地图预见：不要用临时 fork，要做预提交 map plan

地图预见比变换预见更难，因为“未来地图结果”可能会因为玩家先走了哪个 unknown、哪个 event 而改变。

如果只是：

```csharp
var fork = new Rng(runState.Rng.UnknownMapPoint.Seed, runState.Rng.UnknownMapPoint.Counter);
```

然后看一个 Unknown 节点，这个预见可能会因为玩家先进入别的 Unknown 节点而变错。

所以最好的多人方案是：

```text
Act 开始 / map 生成后，生成一份 MapForesightPlan。
这份 plan 就是未来真实结果。
UI 显示 plan。
进入房间时按 plan 创建 room。
```

也就是：

```text
不是“猜未来”，而是“提前决定未来并公开”。
```

## 7.1 MapForesightPlan

新增：

```text
EZMicroBalanceCode/Preview/MapForesight/
  MapForesightPlan.cs
  MapForesightService.cs
  MapForesightStorage.cs
  MapForesightMapUiPatch.cs
  MapForesightRoomPatch.cs
```

记录：

```csharp
internal sealed record MapForesightRecord(
    int ActIndex,
    int Col,
    int Row,
    MapPointType PointType,
    RoomType RoomType,
    string? ModelId,
    string DisplayTitleKey,
    string DisplayBodyKey);
```

保存格式：

```text
act|col|row|pointType|roomType|modelId
```

## 7.2 生成规则

用共享 seed + act + coord + feature id 派生 RNG：

```csharp
uint foresightSeed = StableHash(
    runState.Rng.Seed,
    runState.CurrentActIndex,
    point.coord.col,
    point.coord.row,
    "SpirePlus.MapForesight.v1");

var rng = new Rng(foresightSeed, 0);
```

这样 host 和 client 不需要网络消息，也能得到同一结果。

对每个 map point：

```text
Monster -> 预定 EncounterModel
Elite -> 预定 Elite EncounterModel
Unknown -> 预定 RoomType + Event/Monster/Elite 等
Treasure/Shop/Rest/Boss -> 可以显示固定类型，不需要 roll
```

## 7.3 进入房间时按 plan 执行

patch：

```text
RunManager.RollRoomTypeFor
RunManager.CreateRoom
```

当前 Root Eyes 已经 patch 了这两个位置。

MapForesight 可以复用思路，但不要只针对 Root Eyes：

```csharp
[HarmonyPatch(typeof(RunManager), "RollRoomTypeFor")]
internal static class MapForesightRollRoomTypePatch
{
    private static bool Prefix(RunManager __instance, MapPointType pointType, ref RoomType __result)
    {
        if (!MapForesightService.TryGetCurrentPointPlan(__instance, pointType, out var record))
            return true;

        __result = record.RoomType;
        return false;
    }
}

[HarmonyPatch(typeof(RunManager), "CreateRoom")]
internal static class MapForesightCreateRoomPatch
{
    private static void Prefix(
        RunManager __instance,
        RoomType roomType,
        MapPointType mapPointType,
        ref AbstractModel? model)
    {
        if (model != null)
            return;

        if (MapForesightService.TryGetCurrentPointModel(__instance, roomType, mapPointType, out var previewModel))
            model = previewModel;
    }
}
```

这会让地图预见变成：

```text
多人 host/client 都同一 plan
UI 看到什么，进入就是什么
没有临时 RNG drift
```

## 7.4 这会不会改变游戏？

会。它会把地图相关随机从“进入时 roll”改成“地图生成后预提交”。

但这是你想要的“预见未来”的真正稳定实现。否则如果只是临时 fork RNG 看一眼，未来可能因为别的节点先消耗 RNG 而变化。

---

# 8. 水晶球预见：保持 UI-only

水晶球不要做预提交，因为它本身已经在 minigame 生成时摆好了物品。

它只要：

```text
多人也显示按钮
只改 mask alpha
不调用 ClearCell / RevealItem / CellClicked / AddReward
```

当前源码已经满足主要 UI-only 条件，只要取消 co-op preview gate 即可。

---

# 9. 预见卡牌奖励：必须做 RewardForesightPlan

你还提到“预见卡牌”。如果是“预见卡牌变化”，上面 transform 已经讲了。
如果是“预见卡牌奖励”，那更接近 reward sync，要小心。

不能这样：

```text
host 和 client 各自 roll reward
client 自己显示预测
```

要做：

```text
host 生成 RewardForesightPlan
host/client 都按 plan 显示
真实 reward 也按 plan populate
```

记录：

```csharp
internal sealed record CardRewardForesightRecord(
    string RewardContextId,
    int RewardIndex,
    IReadOnlyList<string> CardIds,
    IReadOnlyList<string> EnchantmentIds,
    IReadOnlyList<bool> UpgradedFlags);
```

然后 patch：

```text
CardReward.Populate
NCardRewardSelectionScreen.RefreshOptions
```

但注意 v0.106 审计明确说 card reward alternatives 和 reward choice index 很敏感。

所以 reward 预见必须原则：

```text
不新增 alternative
不改变 option count，除非 host/client 都从同一 plan 读取
不额外打开 FromSimpleGrid
不让 client 独立 roll
```

---

# 10. 避免不同步和闪退的关键规则

## 10.1 不要让 preview 变成 PlayerChoice

预见功能只是显示信息，不应该新增：

```text
PlayerChoice
CardRewardAlternative
Reward
CardSelectCmd
```

变换预见右侧换卡是 UI，不是 choice。

水晶球透明化是 UI，不是 choice。

地图 tooltip 是 UI，不是 choice。

## 10.2 真要改变未来，就预提交

地图预见如果要求“看到的就是未来真实”，那就必须：

```text
生成 plan
保存 plan
进入时消费 plan
```

不能临时看一眼。

## 10.3 共享结果必须由共享 seed 或 host 生成

多人结果来源只能是：

```text
shared deterministic seed
或
host authoritative record
```

不能是：

```text
client 自己本地 roll
```

## 10.4 失败必须回退 vanilla

任何 preview patch 出错都要：

```text
catch
log
clear preview cache
return true
```

不要让 UI patch 把游戏打崩。

## 10.5 不要碰 reward index

如果必须碰 reward：

```text
host/client 同一个 plan
同一个 option count
同一个 option order
同一个 reward context id
```

否则就会继续 PlayerChoiceSynchronizer 断线。

---

# 11. 我给你的直接方案

如果你要**直接让多人测试可用**，我建议这版：

## Multiplayer Foresight v1

### 功能

```text
1. 水晶球预见：多人直接启用，local UI-only。
2. 卡牌变化预见：多人直接启用，local UI-only，fail-open。
3. 地图预见：Act/map 生成后，用 shared seed + coord 生成 MapForesightPlan；UI 显示，进入房间按 plan 执行。
```

### 不做

```text
1. 不新增 card reward alternative。
2. 不新增 PlayerChoice。
3. 不让 client 独立决定 reward mutation。
4. 不让 Seed Bank/Fission/Prismatic Gem 这类 reward mutation 混进本版。
```

这不是 gate 预见功能，而是：

```text
把预见功能做成独立层。
```

---

# 12. 具体开发任务

你可以直接让他按这个做：

```text
实现 Multiplayer Foresight v1，不允许再用 “gate 掉 preview” 作为最终方案。

目标：
- 多人里水晶球预见生效。
- 多人里卡牌变化预见生效。
- 多人里地图预见生效。
- 不增加 PlayerChoice。
- 不修改 reward index。
- 不消耗真实 RNG 做 preview。
- 进入地图房间时使用预提交 MapForesightPlan，确保看见的就是实际结果。

代码任务：

1. 修改 CrystalSpherePeekPatch
   - 移除 ShouldDisableUnverifiedCoopPreviewTool return。
   - 多人时仍创建按钮。
   - 只记录 coop_local_ui_preview_enabled。
   - 保持只改 %ScryMask.Modulate.A。

2. 修改 TransformPreviewPatch
   - 多人时允许 UI-only preview。
   - Prefix display 包 catch，异常 return true 回 vanilla。
   - 每个 NTransformPreview 用自己的 PredictionQueue。
   - 记录 prediction_prepared_multiplayer_ui_only、prediction_displayed、prediction_display_failed_fallback_vanilla。

3. 修改 TransformPredictionRngContext
   - 多人不再直接 gate。
   - 注册 context 时记录 net mode、source seed/counter。
   - 如果 source stale，清理并 fallback。
   - 不推进真实 RNG。

4. 新增 MapForesightPlan
   - 存 act/col/row/pointType/roomType/modelId。
   - 使用 shared run seed + act + coord + feature id 生成 deterministic RNG。
   - 对 Monster/Elite/Unknown 生成预提交结果。

5. 新增 MapForesight UI
   - patch NNormalMapPoint.OnFocus 或相关 hover。
   - 显示预见结果。
   - 不改变点击行为。

6. 新增 MapForesight room execution
   - patch RunManager.RollRoomTypeFor。
   - patch RunManager.CreateRoom。
   - 如果当前 node 有 plan，使用 plan。
   - 如果 plan 缺失，return true 回 vanilla。

7. 新增 tests
   - CrystalSpherePreviewAllowedInCoopAndOnlyTouchesMask
   - TransformPreviewAllowedInCoopButDoesNotCreateChoiceOrReward
   - TransformPreviewFailsOpenToVanilla
   - MapForesightPlanIsDeterministicFromSharedSeedAndCoord
   - MapForesightRollRoomTypeUsesPlan
   - MapForesightCreateRoomUsesPlan
   - PreviewFeaturesDoNotCall PlayerChoiceSynchronizer / CardRewardAlternative / AddReward

8. 实机测试
   - previous framework + Spire Plus only。
   - host/client 都设置 EZMB_RELEASE_EVIDENCE_LOG=1。
   - 测水晶球。
   - 测 Aroma of Chaos 变换。
   - 测地图 hover 和进入。
   - 收 host/client godot.log。
```

---

# 13. 我不建议你立刻做的部分

不要在这版里同时让下面这些东西多人全开：

```text
Seed Bank reward storage
Fission reward mutation
Prismatic Gem reward reroll
Firemarked Elite reward extra option
Boss reward extra option
Deep Branch reward mutation
```

原因不是我想 gate，而是它们不是“预见层”，它们是“reward/choice 变更层”。它们需要单独做 `RewardForesightPlan`，否则仍会影响 PlayerChoiceSynchronizer。

等 Multiplayer Foresight v1 稳了，再做：

```text
Multiplayer Reward Plan v2
```

那时才恢复这些 reward mutation 的多人完整支持。

---

# 14. 最终回答：有没有比 gate 更好的？

有。

**比 gate 更好的方案是：**

```text
纯 UI 预见直接多人启用；
会影响未来的预见改成预提交 plan；
共享结果由 shared seed 或 host 生成；
所有 preview 不新增 PlayerChoice、不改 Reward index；
失败回退 vanilla。
```

这比“直接全开旧逻辑”好，因为不会不同步。
这也比“永久 gate”好，因为多人里真的能用预见功能。

所以我推荐的最终方案是：

```text
Multiplayer Foresight v1:
- Crystal Sphere: local UI-only
- Transform Preview: local UI-only with fail-open
- Map Preview: deterministic precommitted MapForesightPlan

Multiplayer Reward Plan v2:
- Card reward / Fission / Seed Bank / Prismatic Gem 再做 host-authoritative reward plan
```

这样你可以先在多人游戏里测试：

```text
预见地图
预见卡牌变化
水晶球预见
```

同时最大限度避免不同步、掉线和闪退。
