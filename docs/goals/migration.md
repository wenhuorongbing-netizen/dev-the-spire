# 严格验收结论

**没有完成。**
这轮可以承认为：**RitsuLib Batch 4b 源码迁移已经推进，但 Batch 4a/4b 还没有完成闭环验收，更不能叫 RitsuLib 全面迁移完成。**

当前 GitHub `main` 最新提交是：

```text
c06ff17 Resolve merge conflict decisions
```

说明这轮工作已经 push 到远程，可以验收；不是本地未提交状态。

当前真实状态：

```text
RitsuLib compile/runtime dependency：完成
RitsuLibBootstrap：完成第一层
ModPatcher + raw Harmony hybrid mode：完成第一层
Batch 4a/4b patch migration：源码部分完成
Batch 4a/4b 计数和文档：仍有错误
double-patch guard：未证明完成
full test truth：未证明完成
RitsuLib runtime smoke：未证明完成
Sts1Events 未完成 skeleton：仍存在风险
RitsuLib lifecycle/DataStore/settings/content pack：未迁
RewardPipeline/CardPlayContext/DeathProtection/MultiplayerPolicy：未完成
release-ready：否
```

---

# 1. 分步骤验收

## 1.1 RitsuLib NuGet dependency

**状态：完成。**

`EZMicroBalance.csproj` 已经加入：

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All"/>
```

BaseLib 3.1.4 仍保留，这符合当前“RitsuLib 主导迁移，但 BaseLib 过渡期保留”的策略。

结论：

```text
PASS
```

---

## 1.2 RitsuLib runtime dependency

**状态：完成。**

`EZMicroBalance.json` 已经声明：

```json
{ "id": "STS2-RitsuLib", "min_version": "0.3.2" }
```

同时仍依赖 BaseLib。

这意味着测试安装链路现在必须是：

```text
BaseLib
STS2-RitsuLib
Spire Plus
```

结论：

```text
PASS
```

---

## 1.3 MainFile / bootstrap 解耦

**状态：部分完成。**

`MainFile.Initialize()` 现在调用：

```csharp
RitsuLibBootstrap.ApplyPatches(ModId);
ModConfigRegistry.Register(ModId, new SpirePlusModConfig());
SpirePlusFeatureRegistry.CreateDefault().InitializeAll();
```

说明入口已经不再直接手写所有 feature initializer，而是经过 RitsuLib bootstrap 和 FeatureRegistry。

结论：

```text
PARTIAL PASS
```

但注意：这只是第一层解耦。FeatureRegistry 仍然不是完整模块系统，还缺 live status、dependencies、diagnostics、multiplayer policy 等。

---

## 1.4 RitsuLibBootstrap / hybrid patching

**状态：部分完成。**

当前 `RitsuLibBootstrap` 会：

```csharp
RitsuLibFramework.CreateLogger(...)
RitsuLibFramework.CreatePatcher(...)
RegisterMigratedPatches(patcher)
patcher.PatchAll()
new Harmony(modId).PatchAll()
```

也就是说：

```text
已迁移 patch -> RitsuLib ModPatcher
未迁移 patch -> raw Harmony.PatchAll()
```



这是合理的过渡方案，但它不是完整 RitsuLib patch migration。

结论：

```text
PARTIAL PASS
```

---

## 1.5 Batch 4a / 4b patch migration

**状态：源码部分完成，但文档计数错误。**

### 源码实际注册数

`RitsuLibBootstrap.RegisterMigratedPatches()` 当前注册：

Batch 4a：

```text
FiddlePatches: 4
ChoicesParadoxPatch: 1
DistinguishedCapePatches: 3
BlackStarCompensationPatches: 1
```

实际总数：

```text
4 + 1 + 3 + 1 = 9
```

Batch 4b：

```text
CrossbowPatches: 2
BrightestFlameExhaustDrawPatch: 3
DebtAndCardPatches: 8
SealOfGoldPatches: 2
PickupRewardPatches: 1
```

实际总数：

```text
2 + 3 + 8 + 2 + 1 = 16
```

所以源码真实总数是：

```text
25 migrated classes
```



### 文档错误

`docs/migration.md` 当前写：

```text
Batch 4a: 10
Batch 4b: 16
Total migrated: 26
```

还把 `DebtAndCardPatches.cs` 写成 `7` 个 class，但列出的 patch ids 实际有 8 个。

正确应该是：

```text
Batch 4a = 9
Batch 4b = 16
Total = 25
DebtAndCardPatches = 8
```

结论：

```text
SOURCE PARTIAL PASS
DOC FAIL
```

这不是小问题。迁移表如果错了，后续 Batch 4c/4d 会继续基于错误数量推进。

---

## 1.6 Patch inventory

**状态：部分完成。**

`docs/patch-inventory.md` 当前记录：

```text
Total patch declarations: 141
High risk: 22
Medium risk: 35
Low risk: 84
Unclassified owner: 0
```



这说明 raw `[HarmonyPatch]` 剩余数量已经被统计。

但它仍然缺：

```text
MigratedToRitsuModPatcher inventory
RawHarmonyRemaining inventory
HighRiskBlocked inventory
```

也就是说，它只回答了“剩多少 raw Harmony”，没有完整回答：

```text
哪些已迁？
哪些未迁？
哪些禁止迁？
哪些必须等 evidence？
```

结论：

```text
PARTIAL PASS
```

---

## 1.7 Double-patch safety

**状态：未证明完成。**

当前同时执行：

```text
RitsuLib ModPatcher.PatchAll()
raw Harmony.PatchAll()
```



这必须有 guard 保证：

```text
[ ] RegisterMigratedPatches 里的 class 不含 [HarmonyPatch]
[ ] 含 [HarmonyPatch] 的 class 没有被 RegisterPatch 注册
[ ] PatchId 全局唯一
[ ] RegisterMigratedPatches 数量和 docs/inventory 一致
[ ] 不会同一个 target 被 patch 两次
```

目前我没有看到 GitHub 上能证明这些已经完全覆盖的 guard。

结论：

```text
FAIL / NOT PROVEN
```

---

## 1.8 Full test truth

**状态：未闭环。**

Codex 汇报只说：

```text
All 4 migration tests pass
Build 0 errors
Format clean
Pre-existing failures: Sts1Events, documentation
```

这不能算完整验收。

必须跑并报告：

```powershell
dotnet test EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
```

而不是只跑 migration tests。

`docs/migration.md` 也写了类似：

```text
Tests: 302 passed, 21 skipped, 0 failed (1 pre-existing batch script failure unrelated to RitsuLib)
```

这个表述本身不严谨。如果有 failure，就不能写 0 failed；如果不是测试失败，就必须说明是哪条脚本、是否影响 validation。

结论：

```text
FAIL / INCOMPLETE
```

---

## 1.9 Sts1Events scope creep

**状态：未闭环。**

`Sts1DeadAdventurer` 已经修了一部分。随机遗物分支现在调用：

```csharp
await Sts1EventHelpers.GrantRandomRelic(Owner);
```

而 helper 也确实会拉取 relic 并 `RelicCmd.Obtain(...)`。

但精英分支仍然是：

```csharp
// TODO: Enter combat with random elite
SetEventFinished(...)
```

也就是说，roll 到 elite 分支时不会进入精英战，只会结束事件。

`Sts1Joust` 仍然直接：

```csharp
await PlayerCmd.GainGold(-BetCost, Owner);
```

没有看到金币不足下注防护。

结论：

```text
FAIL / OPEN RISK
```

必须确认 Sts1Events 是否 live registered。如果未注册，要明确标 inactive skeleton 并加 guard；如果已注册，必须禁用或补完。

---

## 1.10 RitsuLib runtime smoke

**状态：未证明。**

虽然 csproj 和 manifest 都接入了 RitsuLib，但现在还没有看到 clean game log 证明：

```text
BaseLib + STS2-RitsuLib + Spire Plus
RitsuLib framework is active
ModPatcher applied expected patch count
No MissingMethodException
No TypeLoadException
```

结论：

```text
NOT PROVEN
```

这必须进入 overnight run。

---

# 2. 完成度总表

| 项目                           | 状态             |
| ---------------------------- | -------------- |
| RitsuLib NuGet dependency    | 完成             |
| RitsuLib manifest dependency | 完成             |
| RitsuLibBootstrap            | 第一层完成          |
| ModPatcher hybrid mode       | 第一层完成          |
| Batch 4a migrated patches    | 源码 9 个，文档错写 10 |
| Batch 4b migrated patches    | 源码 16 个        |
| Total migrated               | 源码 25，文档错写 26  |
| Raw Harmony remaining        | 141，已记录        |
| Migrated inventory           | 缺              |
| Double-patch guard           | 未证明            |
| Full test suite              | 未证明            |
| Sts1Events skeleton          | 风险未闭环          |
| Runtime smoke                | 未证明            |
| RitsuLib lifecycle           | 未开始            |
| RitsuLib DataStore           | 未开始            |
| RitsuLib settings            | 未开始            |
| RitsuLib content pack        | 未开始            |
| StateCodec                   | 未开始            |
| RewardPipeline               | 未开始            |
| CardPlayContext              | 未开始            |
| DeathProtectionService       | 未开始            |
| MultiplayerPolicy            | 未开始            |
| Release-ready                | 否              |

---

# 3. 下一步不要做什么

现在不要继续 Batch 4c。

原因：

```text
Batch 4a/4b 自身没有闭环；
迁移计数错误；
full test truth 不完整；
double-patch guard 不完整；
Sts1Events skeleton 仍然危险；
runtime smoke 未证明。
```

继续迁 patch 会扩大风险面。

---

# 4. Monthly Dev Spec：RitsuLib Migration Stabilization Month

## 月度目标

本月目标不是新增内容，也不是盲目继续迁更多 patch，而是：

```text
1. 关闭 Batch 4a/4b 迁移真相。
2. 证明 RitsuLib runtime 能 clean load。
3. 修复或隔离 Sts1Events 未完成 skeleton。
4. 建立 double-patch 防护。
5. 开始 FeatureRegistry / StateCodec / RewardPipeline / CardPlayContext / DeathProtection / MultiplayerPolicy 架构基础。
```

---

## Week 1：Batch 4a/4b Truth Closure

### 任务

```text
[ ] 修 docs/migration.md：
    Batch 4a = 9
    Batch 4b = 16
    Total = 25

[ ] 修 DebtAndCardPatches row：
    Classes = 8

[ ] 新增/扩展 patch inventory：
    - MigratedToRitsuModPatcher
    - RawHarmonyRemaining
    - HighRiskBlocked

[ ] 添加 tests：
    - PatchId unique
    - migrated patch classes do not contain HarmonyPatch
    - raw Harmony patch classes are not registered in RitsuLibBootstrap
    - RegisterMigratedPatches count matches docs
    - docs/migration.md count matches source
```

### 验收

```text
[ ] 源码和文档计数一致
[ ] patch inventory 有 migrated/raw/high-risk 三类
[ ] double-patch risk 有 guard
```

---

## Week 2：Runtime Smoke + Full Test Truth

### 任务

```text
[ ] dotnet build EZMicroBalance.sln
[ ] dotnet test EZMicroBalance.sln
[ ] dotnet test EZMicroBalance.sln --no-build
[ ] dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
[ ] git diff --check
```

必须做 clean loader smoke：

```text
Only:
- BaseLib
- STS2-RitsuLib
- Spire Plus
```

log 必须包含：

```text
RitsuLib bootstrap starting
ModPatcher applied X/Y patches
RitsuLib framework is active
0 MissingMethodException
0 TypeLoadException
0 manifest dependency failure
```

### 验收

```text
[ ] full tests green，或失败项有 formal quarantine issue
[ ] clean loader log 通过 audit-godot-log
[ ] handoff/evidence docs 更新
```

---

## Week 3：Sts1Events Scope Closure

### 任务

```text
[ ] 确认 Sts1DeadAdventurer / Sts1Joust 是否注册到 live event pool
[ ] 若未注册：
    - docs 标 inactive source skeleton
    - guard：TODO EventModel 不能 live registered

[ ] 若已注册：
    - 禁用或补完 DeadAdventurer elite branch
    - 修 Sts1Joust 金币不足下注
```

新增或更新 issue：

```text
ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK
```

### 验收

```text
[ ] Dead Adventurer 不会假装进入精英战
[ ] Joust 不会金币不足仍扣到负数
[ ] TODO EventModel 不能进入 live pool
```

---

## Week 4：Architecture Foundation

### 任务

```text
[ ] FeatureRegistry 增加：
    - DisplayName
    - Category
    - DisableEnvKeys
    - ForceEnvKeys
    - BootstrapStatus
    - LiveStatus

[ ] UrdaStateCodec V1：
    - Encode
    - Decode
    - MalformedFallback
    - OldStateMigration
    - RoundTrip tests

[ ] RewardPipeline skeleton：
    - RewardPhase
    - IRewardHandler
    - diagnostics only

[ ] CardPlayContext skeleton：
    - ExtraPlayPolicy
    - Power fallback
    - no recursion
    - depth guard

[ ] DeathProtectionService spec：
    - Lotha Reprieve
    - forced unavoidable death
    - inReprieve flag
    - co-op owner

[ ] MultiplayerPolicy doc：
    - LocalUiOnly
    - LocalPlayerOnly
    - HostAuthoritative
    - SharedRunState
    - CombatCommandReplicated
    - UnsafeInMultiplayer
```

### 验收

```text
[ ] FeatureRegistry 不再只是 wrapper
[ ] UrdaStateCodec tests pass
[ ] RewardPipeline / CardPlayContext / DeathProtection / MultiplayerPolicy 至少有 skeleton/spec
```

---

# 5. Overnight Run Spec：必须跑完才能停

下面这段可以直接发给 Codex。重点是：**不允许它只做一小块就停**。它必须跑完所有 phase；如果遇到 blocker，必须留下 issue、证据和下一步。

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：RitsuLib Batch 4a/4b closure + runtime truth + monthly architecture foundation。

这不是 Batch 4c。
这不是新增 gameplay。
这不是 release build。
这是一个 overnight stabilization run。必须跑完所有 phase；如果遇到真实 blocker，必须记录证据、创建/更新 issue、给出下一步，而不是直接停止。

当前审核发现：
- GitHub main 已有 RitsuLib dependency 和 manifest dependency。
- RitsuLibBootstrap 使用 ModPatcher + raw Harmony fallback。
- Batch 4a/4b 已迁移部分 patch。
- 源码实际迁移数：
  - Batch 4a = 9
  - Batch 4b = 16
  - Total = 25
- docs/migration.md 当前写 Batch 4a=10, Total=26，错误。
- DebtAndCardPatches row 写 7，但实际有 8 个 patch ids。
- docs/patch-inventory.md 当前只统计 raw Harmony declarations = 141。
- 需要 migrated/raw/high-risk 三类 inventory。
- Sts1DeadAdventurer 仍有 TODO elite branch。
- Sts1Joust 可能没有金币不足下注保护。
- full test truth 未闭环，不允许只报告 migration tests pass。
- RitsuLib runtime smoke 未证明。
- 不能 claim release-ready。

硬规则：
- 不要新增 gameplay。
- 不要迁 high-risk patch。
- 不要开始 Batch 4c。
- 不要关闭现有默认开启功能。
- 不要改 manifest id。
- 不要只跑 migration tests。
- 不要忽略 pre-existing failures。
- 不要 claim release-ready。
- 必须使用 subagents。

Subagents:

1. Patch Migration Integrity Agent
   - 扫描 RitsuLibBootstrap.RegisterMigratedPatches。
   - 统计 migrated patch count。
   - 统计 raw HarmonyPatch count。
   - 统计 high-risk blocked patch count。
   - 检查 PatchId 唯一。
   - 检查 migrated patch 不含 HarmonyPatch。
   - 检查 raw Harmony patch 未被 RegisterPatch 注册。
   - 生成/更新 patch inventory。

2. Runtime/Test Truth Agent
   - 运行 full build/test。
   - 不允许只跑 migration tests。
   - 记录 dotnet build/test/test --no-build/format/diff。
   - 准备或执行 clean loader smoke checklist：
     BaseLib + STS2-RitsuLib + Spire Plus。
   - 如果不能执行 live loader，明确标 pending，不许说 complete。

3. Sts1Events Scope Agent
   - 检查 Sts1Events 是否 live registered。
   - 检查 Sts1DeadAdventurer elite TODO。
   - 检查 Sts1Joust 金币不足下注。
   - 未注册则标 inactive skeleton 并加 guard。
   - 已注册则禁用或补完。
   - 创建/更新 ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK。

4. Docs/Version Agent
   - 修 docs/migration.md 计数。
   - 更新 docs/integrations/ritsulib.md。
   - 更新 docs/patch-inventory.md。
   - 确认 v0.106.x 目标口径一致。
   - docs 不得说 full migration complete。
   - docs 必须写 release-ready: no。

5. Architecture Planner Agent
   - 产出下一月 architecture plan。
   - 包含 FeatureRegistry hardening。
   - 包含 UrdaStateCodec。
   - 包含 RewardPipeline。
   - 包含 CardPlayContext。
   - 包含 DeathProtectionService。
   - 包含 MultiplayerPolicy。

Phase 1 — Patch truth
- 修 docs/migration.md 计数：
  Batch 4a = 9
  Batch 4b = 16
  Total = 25
- 修 DebtAndCardPatches row 为 8。
- 生成/更新 migrated/raw/high-risk patch inventory。
- 添加 tests：
  - PatchId unique
  - migrated class has no HarmonyPatch
  - raw HarmonyPatch class not registered in RitsuLibBootstrap
  - RegisterMigratedPatches count matches docs
  - docs/migration.md count matches source

Phase 2 — Full test truth
Run:
- git status --short --branch
- git log -1 --oneline --decorate
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check

If tests fail:
- list exact failed tests
- fix if in scope
- otherwise create explicit quarantine issue
- do not claim pass

Phase 3 — Sts1Events safety
- Determine whether Sts1DeadAdventurer/Sts1Joust are live registered.
- If not live:
  - add docs: inactive source skeleton
  - add guard: TODO EventModel cannot be live registered
- If live:
  - disable or complete:
    - DeadAdventurer elite branch
    - Joust insufficient gold path
- No TODO branch may be reachable in live gameplay.

Phase 4 — Runtime smoke evidence
- If local game environment available, run clean loader smoke:
  - BaseLib
  - STS2-RitsuLib
  - Spire Plus
- Require log:
  - RitsuLib bootstrap starting
  - ModPatcher applied expected count
  - RitsuLib framework is active
  - no MissingMethodException
  - no TypeLoadException
- If not available, update manual checklist and mark pending.

Phase 5 — Architecture monthly spec
Create/update:
- docs/features/ritsulib-migration/monthly-dev-spec.md
- docs/features/ritsulib-migration/next-overnight-run.md

Must include:
- Week 1: runtime/test truth
- Week 2: patch integrity
- Week 3: Sts1Events scope closure + FeatureRegistry hardening
- Week 4: UrdaStateCodec + RewardPipeline + CardPlayContext + DeathProtectionService + MultiplayerPolicy
- subagent assignments
- acceptance criteria
- stop conditions

Phase 6 — Package
If code changed:
- dotnet publish EZMicroBalance.sln
- EZMB_RUN_RELEASE_ARTIFACT_TESTS=1 dotnet test EZMicroBalance.sln --no-build
- refresh hashes only from actual artifacts

If docs/tests only:
- do not publish unless needed.

Final report must include:
1. current HEAD
2. files changed
3. Batch 4a/4b exact migrated count
4. raw Harmony remaining count
5. double-patch guard status
6. full test suite result
7. Sts1Events live status
8. RitsuLib runtime smoke status
9. monthly dev spec path
10. subagent findings
11. remaining blockers
12. release-ready: no

Do not stop until all phases are either:
- complete and verified, or
- blocked with explicit blocker issue, source evidence, and next action.
```

---

# 最终判断

当前工作有进展，但还没有完成。

可以承认：

```text
Batch 4b source migration exists.
```

不能承认：

```text
Batch 4a/4b closure complete.
RitsuLib migration complete.
Runtime proof complete.
Full tests complete.
Release-ready.
```

下一步必须是：

```text
Batch 4a/4b closure + runtime truth + architecture foundation
```

并且必须使用 subagents。否则 Codex 很容易继续只完成“一个小批次 patch 迁移”，然后又把它总结成“migration complete”。
