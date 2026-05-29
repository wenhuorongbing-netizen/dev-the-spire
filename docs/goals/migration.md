# 严格审核结论

**没有完全完成。**
这轮工作比之前扎实很多，但当前状态仍应定义为：

```text
RitsuLib 架构稳定化：阶段性完成
RitsuLib runtime 证明：未完成
Sts1Events 默认安全：完成
Sts1Events 内容完成：未完成
架构 skeleton：已创建
架构接入实际业务：大多未完成
可继续开发：可以
可 release：不可以
```

另外要先校准一个事实：你贴的报告说当前 HEAD 是 `a647c44d`，但我查 GitHub 当前 `main` 最新已经是：

```text
d290598c — debugging
```

`a647c44d` 确实存在，是上一条提交；但它不是当前最新 main。
所以严格验收必须以 `d290598c` 当前 `main` 为准。

---

# 1. 逐项验收

## 1.1 RitsuLib 依赖和 runtime dependency

**完成。**

之前已经确认：

```text
STS2.RitsuLib 0.3.2 已加入 csproj
STS2-RitsuLib 已加入 manifest dependency
BaseLib 3.1.4 保留
```

这部分没被回滚。当前后续文档也继续围绕 RitsuLib migration 展开。
**判定：PASS。**

但注意：dependency 加入不等于 runtime 已证明。

---

## 1.2 Batch 4a / 4b 计数修正

**完成。**

`docs/migration.md` 当前已经修正为：

```text
Batch 4a = 9
Batch 4b = 16
Total migrated = 25
Remaining raw HarmonyPatch = 141
```

并且 `DebtAndCardPatches.cs` 已修为 8 个 class。

**判定：PASS。**

---

## 1.3 Patch inventory 和 double-patch guard

**source-level 完成。**

`docs/patch-inventory.md` 现在记录：

```text
Migrated to RitsuLib ModPatcher: 25
Raw HarmonyPatch remaining: 141
High risk raw: 22
Medium risk raw: 35
Low risk raw: 84
```

并且有独立的 `Migrated Patches (RitsuLib ModPatcher)` section，明确这些 25 个 patch 由 `RitsuLibBootstrap.RegisterMigratedPatches()` 注册，不会被 raw `Harmony.PatchAll()` 拾取。

`RitsuLibMigrationGuardTests.cs` 也已经有专门 guard，覆盖：

```text
PatchId unique
migrated patch count = 25
migrated class has no HarmonyPatch
raw HarmonyPatch class not registered
RegisterMigratedPatches count matches source
migration.md count matches source
patch-inventory.md lists migrated patches
```



**判定：PASS。**

但这是 source-level guard，不是 runtime 行为证明。

---

## 1.4 Full test truth

**部分完成，但仍需统一最终 evidence。**

`monthly-dev-spec.md` 当前写：

```text
361 total tests passing
0 failed
21 skipped
```

并写 architecture skeleton guard、UrdaStateCodec guard、FeatureRegistry guard、Sts1Events guard 都已存在。

用户报告也说：

```text
dotnet build: 0 errors, 87 warnings
dotnet test: 361 passed, 0 failed, 21 skipped
```

这和最新 monthly spec 的 361 passed 对上。
但是旧的 `docs/reviews/overnight-run-20260529.md` 仍记录的是：

```text
324 passed, 21 skipped, 0 failed
```

那是旧 overnight review。

**判定：PARTIAL PASS。**

下一步需要一个 canonical validation doc，把当前最终测试事实统一为：

```text
Build: 0 errors, 87 warnings
Test: 361 passed, 0 failed, 21 skipped
```

并明确 87 个 warning 是否全部属于 Sts1Events null-safety，是否接受，何时清理。

---

## 1.5 Build warnings

**未完成。**

用户报告说：

```text
87 warnings, all CS8602/CS8604 in Sts1Events models
```

这比“0 warnings”更真实，也更值得保留。
问题是：**87 warnings 不能长期当作无影响噪音**。既然 Sts1Events 已经编译进项目，而且有 FeatureGate，那么 warnings 会持续污染 build truth。

**判定：OPEN。**

建议下月把它归为 P1：

```text
ISSUE-2026-05-29-STS1EVENTS-NULL-SAFETY-WARNINGS
```

不要急着全修，但要分类：

```text
[ ] Owner nullability
[ ] Deck nullability
[ ] Rng nullability
[ ] Option availability
[ ] Event owner/player availability
```

---

## 1.6 Runtime smoke

**未完成，是当前最大 blocker。**

`runtime-smoke-checklist.md` 仍明确写：

```text
PENDING — no local game environment available for automated runtime smoke.
```

要求 clean Steam client、Slay the Spire 2 v0.106.1、BaseLib、STS2-RitsuLib、Spire Plus，并且所有 evidence 都是 `[PENDING]`。

`monthly-dev-spec.md` 也写：

```text
Runtime smoke: blocked — STS2-RitsuLib not installed locally; Batch 4c blocked until runtime smoke passes
```



**判定：FAIL / BLOCKED。**

没有 runtime smoke，就不能说 RitsuLib migration runtime-safe，也不能进入更多 patch migration。

---

## 1.7 Sts1Events safe-mode status

**默认安全完成，内容完成未完成。**

现在 Sts1Events 已经有明确 gate：

```text
Off = safe default
CanaryOnly = 4 safe shared events
AdditiveAllDraft = all 52 events, includes DeadAdventurer TODO and Joust no gold guard
ReplaceUnknownEventsPrototype = debug-only
```

`monthly-dev-spec.md` 也明确记录了这些模式，并指出 AdditiveAllDraft 包含 DeadAdventurer TODO 和 Joust no-gold-guard。

`Sts1EventFeatureGuardTests.cs` 确认：

```text
env unset -> Off
CanaryOnly exactly 4 events
RegisterCanaryOnly only registers BigFish, GoldenIdol, TheLab, DivineFountain
Feature module registered in SpirePlusFeatureRegistry
```



**判定：安全 gate PASS；内容完整性 FAIL / OPEN。**

这正是应该的当前状态：

```text
Default Off = safe
CanaryOnly = testable
AllDraft / ReplaceUnknown = unsafe dev-only
```

不要把 Sts1Events 当作可发布内容。

---

## 1.8 FeatureRegistry hardening

**部分完成。**

`IFeatureModule` 已经扩展出：

```text
DisplayName
Category
DisableEnvKeys
ForceEnvKeys
```



`FeatureBootstrapRecord` 已经有：

```text
Id
DisplayName
Category
Gate
LiveStatus
FailureMessage
IsActive
```

并有 `FeatureLiveStatus.Enabled / Disabled / Failed`。

`FeatureRegistry` 已经会记录 bootstrapRecords，并且有 `LogFeatureSummary()` 输出：

```text
bootstrap=enabled/disabled
live=Enabled/Disabled/Failed
reason=...
```



**判定：PASS for scaffold。**

但还没达到“完整 feature governance”：

```text
[ ] Dependencies 没有
[ ] Runtime diagnostics 没有统一导出
[ ] LiveStatus 仍基于 bootstrap gate，不等于真实 gameplay live availability
[ ] DisableEnvKeys / ForceEnvKeys 只是 metadata，还没有统一 gate evaluation
```

---

## 1.9 UrdaStateCodec

**完成第一层，但不是完整 state migration。**

`UrdaStateCodec.cs` 已存在，包含：

```text
Decode
Encode
legacy minimum part count
legacy/current index handling
malformed fallback
semicolon wire format
sanitize
```



这解决了之前“裸 split + index parse”最不透明的问题。

**判定：PASS for codec scaffold。**

但它仍然是：

```text
SavedSpireField string bridge + codec
```

不是 RitsuLib DataStore migration，也不是完整 typed persistence。
这可以接受，作为阶段一。

---

## 1.10 RewardPipeline skeleton

**存在，但 diagnostics-only。**

`RewardPipeline.cs` 已有：

```text
RewardPhase enum
IRewardHandler interface
RewardPipelineContext
RewardPipeline.Register()
RewardPipeline.Diagnose()
HandlerCount
RegisteredPhases
ClearHandlers()
```

并明确写：

```text
Skeleton only — diagnostics and contract enforcement, no behavior changes.
```



**判定：PASS for skeleton。**

但还没有接入 Urda/Morvi/Lotha/Fission/Prismatic/ClosedCourt 等真实 reward mutation。

---

## 1.11 CardPlayContext skeleton

**存在，但未接入实际 extra-play 逻辑。**

`CardPlayContext.cs` 已有：

```text
ExtraPlayPolicy Allow / Block / FallbackToPower
MaxDepth = 10
TryIncrementDepth()
DecrementDepth()
Reset()
IsPowerFallback
```

并明确写目前是 skeleton，现有 Lotha extra-play 仍使用 per-blessing flags。

**判定：PASS for skeleton。**

但还没解决真实递归问题，下一步必须接入一个低风险 extra-play effect 做 canary。

---

## 1.12 DeathProtectionService

**只有 spec，没有代码服务。**

`monthly-dev-spec.md` 写 DeathProtectionService 当前是 spec，文档描述 Lotha DeathReprieve lifecycle、inReprieve flag、forced death bypass、co-op owner attribution、future `IDeathProtectionProvider` interface。

**判定：DOC PASS；CODE NOT DONE。**

---

## 1.13 MultiplayerPolicy

**taxonomy 完成，行为接入未完成。**

`monthly-dev-spec.md` 记录 MultiplayerPolicy taxonomy 有 6 类：

```text
LocalUiOnly
LocalPlayerOnly
HostAuthoritative
SharedRunState
CombatCommandReplicated
UnsafeInMultiplayer
```

并映射到现有 MultiplayerFeaturePolicy。

**判定：DOC PASS；CODE/POLICY ENFORCEMENT NOT DONE。**

---

## 1.14 Latest `d290598` 后续改动风险

最新 `d290598` commit 又做了新变化：加了 `InternalsVisibleTo`、新增 Sts1 Purifier / Golden Shrine localization 和 models、FeatureModule DisplayName/Category、MainFile 调用 `registry.LogFeatureSummary()` 等。

这里有两个重要提醒：

1. **最新 HEAD 已经超出用户报告的 `a647c44d`。**
2. **Sts1Events 又新增了更多事件内容，虽然 default Off，但 AllDraft 风险面继续扩大。**

这强化了结论：下一轮不能继续扩 Sts1Events 或 Batch 4c，必须先 runtime smoke + governance。

---

# 2. 当前目标对比

## 我们的目标

```text
1. RitsuLib runtime proof
2. RitsuLib migration batch closure
3. Sts1Events default safety
4. FeatureRegistry hardening
5. State codec foundation
6. Reward/CardPlay/Death/Multiplayer architecture skeletons
7. 不 release-ready
8. subagent 工作流
```

## 当前结果

| 目标                        | 状态                      |
| ------------------------- | ----------------------- |
| RitsuLib runtime proof    | 未完成                     |
| Batch 4a/4b closure       | 完成                      |
| Sts1Events default safety | 完成                      |
| FeatureRegistry hardening | 部分完成                    |
| UrdaStateCodec            | 完成第一层                   |
| RewardPipeline skeleton   | 完成                      |
| CardPlayContext skeleton  | 完成                      |
| DeathProtectionService    | spec only               |
| MultiplayerPolicy         | taxonomy only           |
| subagent 工作流              | 文档中记录有 subagent summary |
| release-ready no          | 保持正确                    |

---

# 3. 综合决策：继续优化、推进，还是两者兼顾？

**决策：优化为主，推进为辅。**

原因：

```text
RitsuLib runtime smoke 未通过；
build warnings 87 个；
Sts1Events AllDraft 仍有半成品内容；
架构 skeleton 大多未接入实际系统；
高风险 patch 迁移仍 blocked。
```

所以：

```text
80% 优化/硬化
20% 有限推进
```

可以推进的只有：

```text
Runtime smoke evidence
FeatureRegistry status refinement
UrdaStateCodec tests
RewardPipeline/CardPlayContext canary integration
Sts1Events canary proof
```

不能推进的：

```text
Batch 4c 大量迁 patch
High-risk patches
Sts1Events AdditiveAllDraft live
Release packaging
```

---

# 4. 下个月开发规范 Monthly Dev Spec

## 月度主题

```text
RitsuLib Runtime Proof + Architecture Integration Month
```

---

## Week 1：Runtime Proof + Warning Truth

### 目标

把 RitsuLib 从“编译通过”推进到“真实游戏加载通过”。

### 任务

```text
[ ] 安装 BaseLib v3.1.4
[ ] 安装 STS2-RitsuLib variant pack
[ ] 安装 Spire Plus
[ ] 只启用这三个
[ ] 启动 Steam 客户端
[ ] 收集 godot.log
[ ] 运行 audit-godot-log
```

日志必须证明：

```text
[ ] RitsuLib bootstrap starting
[ ] ModPatcher applied 25 patches
[ ] RitsuLib framework is active
[ ] BaseLib initialized
[ ] Spire Plus initialized
[ ] SavedSpireFields expected count
[ ] 0 MissingMethodException
[ ] 0 TypeLoadException
[ ] 0 manifest dependency failure
```

同时处理 87 warnings：

```text
[ ] 列出 87 warning 文件分布
[ ] 按 Owner null / Rng null / Deck null / Option null 分类
[ ] 先不要求全部修，但必须建立 warning issue
```

验收：

```text
[ ] runtime-smoke-checklist.md 有真实证据
[ ] docs/reviews/current-validation.md 有唯一 validation truth
[ ] 若 runtime smoke 无法跑，Batch 4c 继续 blocked
```

---

## Week 2：Sts1Events Governance

### 目标

把 Sts1Events 从“default safe”推进到“测试边界完全清晰”。

### 任务

```text
[ ] Off 模式：确认注册 0
[ ] CanaryOnly：确认 4 个 canary 事件均无 TODO/BLOCKED
[ ] AdditiveAllDraft：列出所有 TODO/BLOCKED/partial-substitute events
[ ] ReplaceUnknownEventsPrototype：标 debug-only unsafe
[ ] GoldenShrine / Purifier 新增事件进入 registry 后更新 canary/all draft 风险
```

特别检查：

```text
DeadAdventurer elite TODO
Joust no-gold-guard
Vampires no Bite custom card
Nloth no RelicSelectCmd
MindBloom War blocked
所有新加 shrine/purifier 是否 source/API safe
```

验收：

```text
[ ] Sts1Events current issue 不再 stale
[ ] AdditiveAllDraft 不被测试员误认为可玩
[ ] CanaryOnly 可进入手测矩阵
```

---

## Week 3：Architecture Skeleton 接入 canary

### 目标

不再只写 skeleton，选低风险系统做真实接入。

### 任务

#### RewardPipeline canary

```text
[ ] 选择 1 个 diagnostics-only reward path
[ ] 不改变 reward 行为
[ ] 只打印 phase / handler / context
[ ] 证明 pipeline 不 softlock
```

#### CardPlayContext canary

```text
[ ] 选择 1 个低风险 extra-play 或 fallback source
[ ] 接入 depth guard
[ ] 不改变玩家可见行为
[ ] 添加 recursion test
```

#### FeatureRegistry canary

```text
[ ] 所有 module 输出 DisplayName / Category / live status
[ ] Sts1Events 显示 live=disabled unless env mode
[ ] VakuuFight 显示 live=hidden
```

验收：

```text
[ ] 至少一个 skeleton 被真实系统使用
[ ] 没有 gameplay behavior change
[ ] tests + diagnostics 通过
```

---

## Week 4：State / Death / Multiplayer Foundations

### 目标

为后续高风险系统做准备，不迁 high-risk patch。

### 任务

#### UrdaStateCodec

```text
[ ] 行为级测试补充
[ ] malformed state 不丢 selected blessing
[ ] old-state migration 覆盖更多字段
[ ] 写明 RitsuLib DataStore 迁移计划
```

#### DeathProtectionService

```text
[ ] 建立 code stub，不只是 docs
[ ] Define Request/Result/Priority
[ ] 不接入 Lotha 真实行为前先测试 forced-unpreventable semantics
```

#### MultiplayerPolicy

```text
[ ] 把当前 active features 标 policy
[ ] 每个 policy 写 required evidence
[ ] co-op unsafe features fail-closed list
```

验收：

```text
[ ] no high-risk patch migrated
[ ] next high-risk migration plan 有 rollback strategy
```

---

# 5. 新 Overnight Run Spec：跑完才能停

下面这版建议替换现有 `next-overnight-run.md`。当前不要以 Batch 4c 为目标。

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：RitsuLib Runtime Proof + Architecture Integration Overnight Run。

这不是 Batch 4c。
不要迁更多 patches，除非是修复 guard 或 runtime blocker。
不要新增 gameplay。
不要 claim release-ready。
必须使用 subagents。
必须跑完所有 phase；如果 blocker 存在，必须记录 issue、证据、下一步，不能直接停止。

当前状态：
- Latest main includes RitsuLib dependency and runtime dependency.
- 25 patches migrated to RitsuLib ModPatcher.
- 142 raw Harmony patches remain.
- Batch 4a/4b counts fixed.
- RitsuLibMigrationGuardTests pass source-level double-patch checks.
- Full test truth currently reported as 361 passed / 0 failed / 21 skipped.
- Build has 87 warnings, all Sts1Events null-safety per prior report.
- Runtime smoke blocked because STS2-RitsuLib not installed locally.
- Sts1Events default Off and CanaryOnly are guarded.
- AdditiveAllDraft and ReplaceUnknownEventsPrototype are unsafe/dev-only.
- FeatureRegistry metadata exists but live semantics need stronger diagnostics.
- RewardPipeline/CardPlayContext are skeletons.
- DeathProtectionService and MultiplayerPolicy are docs/spec, not enforcement.

Subagents:

1. Runtime/Test Truth Agent
   - Run full build/test/test --no-build/format/diff.
   - Reconcile warning count.
   - Execute runtime smoke if game environment exists.
   - If runtime smoke unavailable, update blocker docs and keep Batch 4c blocked.

2. Sts1Events Governance Agent
   - Audit all four Sts1 modes.
   - List TODO/BLOCKED/partial events in AdditiveAllDraft.
   - Confirm CanaryOnly has no TODO/BLOCKED.
   - Update issue docs and guard tests.

3. FeatureRegistry Agent
   - Verify DisplayName/Category/env keys on all modules.
   - Improve BootstrapStatus vs LiveStatus logging.
   - Add tests for Sts1Events Off/CanaryOnly and Vakuu hidden status.

4. Architecture Integration Agent
   - Wire RewardPipeline diagnostics into one low-risk reward surface.
   - Wire CardPlayContext into one low-risk extra-play or no-op diagnostic path.
   - Do not change gameplay behavior.

5. State/Death/Multiplayer Agent
   - Expand UrdaStateCodec tests.
   - Create DeathProtectionService code stub if not present.
   - Create active feature MultiplayerPolicy matrix.

Phase 1 — Full validation truth
Run:
- git status --short --branch
- git log -1 --oneline --decorate
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check

Output canonical:
- exact warnings count
- exact passed/failed/skipped count
- list top warning owners
- whether warnings accepted or issue-worthy

Phase 2 — Runtime smoke
If local game environment exists:
- install only BaseLib + STS2-RitsuLib + Spire Plus
- launch game
- collect godot.log
- verify:
  - RitsuLib bootstrap starting
  - ModPatcher applied 25 patches
  - RitsuLib framework is active
  - BaseLib initialized
  - Spire Plus initialized
  - no MissingMethodException
  - no TypeLoadException
  - no manifest dependency failure

If unavailable:
- mark blocked
- create/update runtime blocker issue
- do not proceed to Batch 4c

Phase 3 — Sts1Events governance
- Confirm Off registers 0.
- Confirm CanaryOnly registers exactly 4 safe canaries.
- Produce AdditiveAllDraft risk table:
  - event id
  - TODO/BLOCKED/partial status
  - missing API
  - manual risk
- Mark AdditiveAllDraft and ReplaceUnknownEventsPrototype dev-only.
- Add tests if safe modes can include unsafe events.

Phase 4 — FeatureRegistry hardening
- Log feature summary with:
  - Id
  - DisplayName
  - Category
  - Bootstrap gate
  - Live status
  - reason
- Add tests for all feature modules.
- Keep default-on behavior unchanged.

Phase 5 — Architecture canary integration
- RewardPipeline: attach diagnostics to one low-risk reward path.
- CardPlayContext: attach to one low-risk extra-play/fallback path or create no-op canary test.
- No gameplay behavior changes.
- Add tests.

Phase 6 — State/Death/Multiplayer foundation
- Add/extend UrdaStateCodec tests.
- Add DeathProtectionService code stub with Request/Result/Priority.
- Add MultiplayerPolicy matrix for active systems.
- Keep high-risk behavior unmodified.

Phase 7 — Docs/monthly update
Update:
- docs/features/ritsulib-migration/monthly-dev-spec.md
- docs/features/ritsulib-migration/next-overnight-run.md
- runtime-smoke-checklist.md
- Sts1Events issue
- current validation review

Next action decision:
- If runtime smoke passes and architecture canary passes: Batch 4c may be considered.
- If runtime smoke remains blocked: focus on runtime setup, not patch migration.
- If warnings remain 87: create warning cleanup sprint.
- If Sts1Events unsafe events remain: keep AllDraft dev-only.

Final report:
1. current HEAD
2. full validation results
3. runtime smoke status
4. warning analysis
5. Sts1Events mode safety
6. FeatureRegistry status
7. RewardPipeline/CardPlayContext canary status
8. DeathProtectionService/MultiplayerPolicy status
9. subagent findings
10. files changed
11. next decision: optimize / advance / both
12. release-ready: no

Stop condition:
Do not stop until every phase is complete or blocked with issue, evidence, and next action.
```

---

# 6. 子代理要求

必须显式要求 Codex 使用子代理：

## Runtime/Test Truth Agent

```text
build/test/warning/runtime log
```

## Sts1Events Governance Agent

```text
Off/CanaryOnly/AdditiveAllDraft/ReplaceUnknown
TODO/BLOCKED event table
```

## FeatureRegistry Agent

```text
metadata/live status/env keys/logs/tests
```

## Architecture Integration Agent

```text
RewardPipeline/CardPlayContext canary wiring
```

## State/Death/Multiplayer Agent

```text
UrdaStateCodec
DeathProtectionService stub
MultiplayerPolicy matrix
```

---

# 7. 最终结论

这轮工作不是失败。它完成了不少基础设施：

```text
Batch 4a/4b closure
Sts1Events gate
FeatureRegistry metadata
UrdaStateCodec
RewardPipeline skeleton
CardPlayContext skeleton
Death/Multiplayer docs
```

但还没有达到我们的最终目标：

```text
RitsuLib runtime proof 没有
架构 skeleton 大多没接入真实系统
Sts1Events 仍是 dev/prototype
87 warnings 仍未清
release 仍不可能
```

当前决策应该是：

```text
继续优化为主，有限推进为辅。
```

只有当 runtime smoke 真正通过之后，才考虑继续 Batch 4c。
