我这次按 **GitHub 当前 `main`** 严格审核。你之前上传的部分文本文件已经过期，我没有依赖它们；下面判断基于远程仓库文件和提交状态。

# 总结判断

当前最新 GitHub 提交是：

```text
c06ff17 Resolve merge conflict decisions
```

说明这轮 Batch 4b / merge conflict 相关工作已经落到远程，不是本地未提交状态。

但是，**工作没有完成到“RitsuLib 迁移闭环”**。它完成的是：

```text
RitsuLib 硬依赖：完成
RitsuLib runtime dependency：完成
RitsuLibBootstrap：完成第一层
RitsuLib ModPatcher patch migration：完成一小部分
Batch 4a/4b 文档：有记录
raw Harmony 剩余 patch inventory：有记录
```

没有完成的是：

```text
Full test truth：未闭环
RitsuLib runtime smoke：未证明
Batch 4a/4b 计数：仍然错误
Double-patch guard：未证明
Sts1Events 未完成 skeleton：仍然存在
RitsuLib lifecycle/DataStore/settings/content pack：未迁
StateCodec / RewardPipeline / CardPlayContext / DeathProtectionService / MultiplayerPolicy：未完成
```

所以当前状态应定义为：

```text
RitsuLib migration bootstrap + partial patch migration in progress.
Not migration-complete.
Not runtime-proven.
Not release-ready.
```

---

# 1. 每一步严格审核

## 1.1 RitsuLib NuGet dependency

**完成。**

`EZMicroBalance.csproj` 当前已经加入：

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All"/>
```

同时仍保留 BaseLib 3.1.4。

判定：

```text
PASS
```

但这只是编译依赖，不代表运行时已通过。

---

## 1.2 RitsuLib runtime dependency

**完成。**

`EZMicroBalance.json` 当前 dependencies 包含：

```json
{
  "id": "STS2-RitsuLib",
  "min_version": "0.3.2"
}
```

同时继续依赖 BaseLib。

判定：

```text
PASS
```

从现在开始，测试安装顺序必须是：

```text
1. BaseLib
2. STS2-RitsuLib
3. Spire Plus
```

如果测试员没装 RitsuLib，Spire Plus 应该无法正常加载。

---

## 1.3 RitsuLibBootstrap

**部分完成。**

当前 `RitsuLibBootstrap` 做了这些事：

```csharp
RitsuLibFramework.CreateLogger(modId)
RitsuLibFramework.CreatePatcher(modId, "SpirePlus")
RegisterMigratedPatches(patcher)
patcher.PatchAll()
new Harmony(modId).PatchAll()
```

也就是说：先用 RitsuLib `ModPatcher` 打已迁移 patch，再用 raw Harmony 打剩余 `[HarmonyPatch]`。

判定：

```text
PARTIAL PASS
```

这一步完成了 **hybrid patch bootstrap**，但还没有完成：

```text
RitsuLib lifecycle
RitsuLib DataStore
RitsuLib settings
RitsuLib content pack
full ModPatcher migration
```

---

## 1.4 Batch 4a / Batch 4b patch migration

**源码层面部分完成，文档计数仍然错误。**

`RitsuLibBootstrap.RegisterMigratedPatches()` 当前注册了：

### Batch 4a

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

### Batch 4b

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

这些注册都能在 `RitsuLibBootstrap.cs` 里看到。

但 `docs/migration.md` 当前写：

```text
Batch 4a: 10
Batch 4b: 16
Total migrated: 26
```

而实际应该是：

```text
Batch 4a: 9
Batch 4b: 16
Total migrated: 25
```

并且 `docs/migration.md` 的 Batch 4b 表格写 `DebtAndCardPatches.cs | 7`，但列出的 patch ids 实际是 8 个。

判定：

```text
SOURCE PARTIAL PASS
DOC FAIL
```

这必须修。迁移进度表不能靠手写估计，应该由测试或脚本自动算。

---

## 1.5 raw Harmony patch inventory

**部分完成。**

`docs/patch-inventory.md` 当前记录：

```text
Total patch declarations: 141
High risk: 22
Medium risk: 35
Low risk: 84
Unclassified owner: 0
```



这说明 raw `[HarmonyPatch]` 剩余面已经被记录。

但它还缺：

```text
MigratedToRitsuModPatcher inventory
RawHarmonyRemaining inventory
HighRiskBlocked inventory
```

现在它只能回答“剩下多少 raw Harmony”，不能完整回答“迁了哪些、剩哪些、哪些永远不该迁”。

判定：

```text
PARTIAL PASS
```

---

## 1.6 Double-patch safety

**未证明完成。**

当前同时运行：

```text
ModPatcher.PatchAll()
Harmony.PatchAll()
```



这种混合模式可以接受，但必须有 guard 证明：

```text
[ ] RegisterMigratedPatches 中的 class 不含 [HarmonyPatch]
[ ] 含 [HarmonyPatch] 的 class 不在 RegisterMigratedPatches
[ ] PatchId 全局唯一
[ ] RegisterMigratedPatches 数量和 docs / inventory 一致
[ ] migrated patch 不会被 raw Harmony 再扫一次
```

我没有看到当前 GitHub 上有完整的 double-patch guard。search 也没有找到清晰的 `MigrationGuardTests` 证据。

判定：

```text
FAIL / NOT PROVEN
```

---

## 1.7 Full test truth

**未闭环。**

Codex 报告说：

```text
All 4 migration tests pass
Build 0 errors
Format clean
Pre-existing failures (Sts1Events, documentation): unrelated
```

这不能作为完整验收。

必须明确跑：

```text
dotnet test EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
```

而不是只跑 migration tests。当前 `docs/migration.md` 里也写着：

```text
Tests: 302 passed, 21 skipped, 0 failed (1 pre-existing batch script failure unrelated to RitsuLib)
```

这句话本身就不严谨：如果有 failure，就不能写 0 failed；如果它不是 test suite failure，就必须说明是哪条脚本、是否影响 validation。

判定：

```text
FAIL / INCOMPLETE
```

---

## 1.8 Sts1Events scope creep

**未闭环，仍有风险。**

`Sts1DeadAdventurer` 现在已经修了一部分：随机遗物分支会调用：

```csharp
await Sts1EventHelpers.GrantRandomRelic(Owner);
```



`Sts1EventHelpers.GrantRandomRelic()` 也确实会从 relic pool 拉 relic 并 `RelicCmd.Obtain(...)`。

但 `Sts1DeadAdventurer` 的精英分支仍然是：

```csharp
// TODO: Enter combat with random elite
SetEventFinished(...)
```

也就是说 roll 到 elite 分支时不会进入精英战，只会结束事件。

`Sts1Joust` 仍然直接扣 50 金：

```csharp
await PlayerCmd.GainGold(-BetCost, Owner);
```

没有看到金币不足禁用或防负金币保护。

判定：

```text
FAIL / OPEN RISK
```

必须确认 Sts1Events 是否注册到 live event pool。如果未注册，标记 inactive skeleton 并加 guard；如果已注册，就必须禁用或补完。

---

## 1.9 RitsuLib runtime smoke

**未证明。**

虽然 manifest 和 csproj 都已接入 RitsuLib，但还没有看到 clean Steam loader log 证明：

```text
BaseLib + STS2-RitsuLib + Spire Plus
RitsuLib framework is active
ModPatcher applied expected patches
No MissingMethodException
No TypeLoadException
```

判定：

```text
NOT PROVEN
```

这必须成为下一个 overnight run 的核心目标之一。

---

# 2. 当前完成度总表

| 项目                          | 状态      | 审核结论                |
| --------------------------- | ------- | ------------------- |
| RitsuLib NuGet dependency   | 完成      | PASS                |
| RitsuLib runtime dependency | 完成      | PASS                |
| RitsuLibBootstrap           | 第一层完成   | PARTIAL PASS        |
| ModPatcher hybrid mode      | 存在      | PARTIAL PASS        |
| Batch 4a source migration   | 实际 9 个  | PASS but docs wrong |
| Batch 4b source migration   | 实际 16 个 | PASS                |
| migration.md 计数             | 错误      | FAIL                |
| raw Harmony inventory       | 有       | PARTIAL PASS        |
| migrated inventory          | 缺       | FAIL                |
| double-patch guard          | 未证明     | FAIL                |
| full test suite             | 未证明     | FAIL                |
| Sts1Events                  | 未闭环     | FAIL                |
| runtime smoke               | 未证明     | FAIL                |
| lifecycle migration         | 未开始     | OPEN                |
| DataStore migration         | 未开始     | OPEN                |
| StateCodec                  | 未开始     | OPEN                |
| RewardPipeline              | 未开始     | OPEN                |
| CardPlayContext             | 未开始     | OPEN                |
| DeathProtectionService      | 未开始     | OPEN                |
| MultiplayerPolicy           | 未开始     | OPEN                |
| release-ready               | 否       | FAIL if claimed     |

---

# 3. 下一步不能做什么

不要继续 Batch 4c。
不要迁 high-risk patch。
不要新增新玩法。
不要只跑 migration tests。
不要把 Sts1Events 的 TODO 当无关问题。
不要 claim release-ready。

原因：

```text
Batch 4a/4b 自身还没闭环。
runtime smoke 没有。
full test truth 没有。
double-patch guard 没有。
Sts1Events live-risk 没有关闭。
```

---

# 4. Monthly Dev Spec：RitsuLib Stabilization & Test Truth Month

## 月度目标

本月目标不是继续扩功能，也不是继续盲迁更多 patch，而是：

```text
把 RitsuLib migration 的基础层稳定下来。
证明 runtime 可用。
关闭 Batch 4a/4b 迁移风险。
建立 patch inventory / full test truth / Sts1Events safety。
然后再进入 StateCodec / RewardPipeline / CardPlayContext / DeathProtection / MultiplayerPolicy。
```

---

## Week 1：Batch 4a/4b Truth Closure

### 目标

修正迁移计数，建立自动化 patch truth。

### 必做

```text
[ ] 修 docs/migration.md：
    Batch 4a = 9
    Batch 4b = 16
    Total = 25

[ ] 修 DebtAndCardPatches row：
    Classes = 8，不是 7

[ ] 新增或扩展 patch inventory：
    - MigratedToRitsuModPatcher
    - RawHarmonyRemaining
    - HighRiskBlocked

[ ] 自动统计 RegisterMigratedPatches。
[ ] 添加 PatchId unique test。
[ ] 添加 RegisterMigratedPatches count test。
[ ] 添加 docs/migration.md count consistency test。
```

### 验收

```text
[ ] docs count 与源码自动统计一致。
[ ] PatchId 无重复。
[ ] raw/migrated/high-risk 三类清楚。
```

---

## Week 2：Runtime Smoke + Full Test Truth

### 目标

证明 RitsuLib 真能运行，不只是 build 通过。

### 必做

```text
[ ] dotnet build EZMicroBalance.sln
[ ] dotnet test EZMicroBalance.sln
[ ] dotnet test EZMicroBalance.sln --no-build
[ ] dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
[ ] git diff --check
```

不允许只报告：

```text
migration tests pass
```

还必须跑 clean Steam loader smoke：

```text
BaseLib
STS2-RitsuLib
Spire Plus
```

log 必须包含：

```text
RitsuLib bootstrap starting
ModPatcher applied X/Y patches
RitsuLib framework is active
Spire Plus loaded
0 MissingMethodException
0 TypeLoadException
0 manifest dependency failure
```

### 验收

```text
[ ] full test suite green，或失败项有正式 quarantine issue。
[ ] clean loader log 通过 audit-godot-log。
[ ] docs/release-evidence-status.md 或 handoff 更新。
```

---

## Week 3：Sts1Events Scope Closure

### 目标

把 Sts1Events 未完成 skeleton 变成安全状态。

### 必做

```text
[ ] 确认 Sts1Events 是否注册 live event pool。
[ ] 若未注册：
    - docs 标 inactive source skeleton。
    - guard：TODO EventModel 不能 live registered。

[ ] 若已注册：
    - 禁用 Sts1DeadAdventurer elite branch，或补完整 elite combat。
    - 修 Sts1Joust 金币不足下注问题。
```

新增 issue：

```text
ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK
```

### 验收

```text
[ ] Dead Adventurer 不会假装进入精英战。
[ ] Joust 不会在金币不足时负金币下注。
[ ] TODO skeleton 不能进入 live pool。
```

---

## Week 4：Architecture Foundation

### 目标

开始解决真正导致 bug 多的架构问题。

### 必做

```text
[ ] FeatureRegistry 增加：
    - DisplayName
    - Category
    - DisableEnvKeys
    - ForceEnvKeys
    - BootstrapStatus
    - LiveStatus

[ ] UrdaStateCodec V1 设计和最小实现：
    - Encode
    - Decode
    - MalformedFallback
    - OldStateMigration
    - RoundTrip tests

[ ] RewardPipeline skeleton：
    - RewardPhase
    - IRewardHandler
    - Diagnostics only

[ ] CardPlayContext skeleton：
    - ExtraPlayPolicy
    - Power fallback
    - No recursion
    - Depth guard

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
[ ] FeatureRegistry 不再只是 wrapper。
[ ] UrdaStateCodec tests pass。
[ ] RewardPipeline / CardPlayContext / DeathProtection / MultiplayerPolicy 有代码或明确 spec。
```

---

# 5. Overnight Run Spec：必须跑完才能停

下面是给 Codex 的 **overnight run prompt**。重点是：它不能只做一小段就停，必须跑完整个闭环；无法完成的阶段必须留下 blocker、证据、issue 和下一步。

---

## Overnight Run Prompt

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：RitsuLib Batch 4a/4b closure + runtime truth + monthly architecture foundation。

这不是继续 Batch 4c。
这不是新增 gameplay。
这不是 release build。
这是一个 overnight stabilization run：必须跑完所有 phase，除非遇到真实 blocker。遇到 blocker 时必须记录证据、创建/更新 issue、给出下一步，而不是直接停止。

当前审核发现：
- 当前 GitHub main 已经有 RitsuLib dependency 和 manifest dependency。
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

# 6. 最终结论

当前工作有进展，但仍然没完成。
最准确的状态是：

```text
RitsuLib Batch 4b source migration exists.
Batch 4a/4b closure not complete.
Runtime proof not complete.
Full test truth not complete.
Sts1Events scope risk open.
Architecture migration not started.
```

下一步必须是：

```text
Batch 4a/4b closure + runtime truth + monthly architecture foundation
```

并且必须使用 subagents 拆分任务，否则 Codex 会继续只完成一小块，然后把 “migration tests pass” 当成整体完成。
