我会先说明一个限制：你之前某些上传文件已经过期，我现在不能再读取那些旧附件内容。如果你需要我重新审旧日志或旧报告，需要再上传一次。下面这份审核**只基于当前 GitHub 仓库状态和你这轮提供的报告**，不依赖旧附件。

# 一、从头重建当前项目上下文

项目是：

```text
wenhuorongbing-netizen/dev-the-spire
当前 mod 名称/方向：Spire Plus（保留 `EZMicroBalance` 作为技术 manifest id）
当前主线方向：大型玩法包 + RitsuLib 迁移 + 架构稳定化
```

目前项目已经不是一个小型平衡 mod，而是包含多条大系统：

```text
1. Ancient reward rebalance v4
2. Ascension 11–20
3. Rootblight / Blight Sprout
4. Urda / Morvi / Lotha / Vakuu Ancient expansion
5. StS1 event port prototype
6. RitsuLib migration
7. Preview tools / website / art / package evidence
8. Multiplayer diagnostics / fail-closed gates
```

过去几轮的核心目标已经从“加内容”转成：

```text
RitsuLib-first migration
架构解耦
状态稳定
runtime smoke
多人和 save/load 风险控制
```

---

# 二、当前 GitHub 真实状态

最新提交搜索显示，远程仓库里最新可见提交已经出现：

```text
85a38dd — architecture canary: add DeathProtectionService + MultiplayerPolicy behavioral tests, update test counts to 444/465
```

它在 `f4247553`、`aed2a49` 之后。

但同时我也发现一个**文档真相问题**：`next-overnight-run.md` 仍写当前 HEAD 是 `aed2a498`，测试数是 `444 passed`，build 是 `92 warnings`。 这说明当前仓库仍存在“最新提交、文档状态、用户报告”之间的同步问题。

所以严格结论是：

```text
当前不是最终完成态。
当前是“架构稳定化持续推进中，但 runtime proof 未完成，文档真相仍需统一”的状态。
```

---

# 三、逐项严格验收

## 1. RitsuLib 依赖接入

状态：**完成。**

项目已经添加了 RitsuLib 编译依赖：

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All"/>
```

同时仍保留 BaseLib 3.1.4。

manifest 也已经添加 runtime dependency：

```json
{ "id": "STS2-RitsuLib", "min_version": "0.3.2" }
```



结论：

```text
PASS
```

但这只是接入，不等于 runtime 已验证。

---

## 2. RitsuLibBootstrap / hybrid patch 模式

状态：**第一阶段完成。**

当前 `RitsuLibBootstrap` 会：

```text
1. CreateLogger
2. CreatePatcher
3. RegisterMigratedPatches
4. patcher.PatchAll()
5. raw Harmony.PatchAll()
6. log RitsuLibFramework.IsActive
```



这意味着当前是混合模式：

```text
已迁移 patch -> RitsuLib ModPatcher
未迁移 patch -> raw Harmony.PatchAll()
```

结论：

```text
PASS for hybrid bootstrap
NOT complete for full RitsuLib migration
```

---

## 3. Batch 4a / 4b patch migration

状态：**源码和文档基础闭环基本完成。**

之前有错误：Batch 4a 文档写 10，实际 9；总数写 26，实际 25。现在 `docs/migration.md` 已修正为：

```text
Batch 4a = 9
Batch 4b = 16
Total migrated = 25
Remaining = 141 HarmonyPatch declarations
```



`docs/patch-inventory.md` 也记录：

```text
Migrated to RitsuLib ModPatcher: 25
Raw HarmonyPatch remaining: 141
High risk raw: 22
Medium risk raw: 35
Low risk raw: 84
```

并列出了 migrated patches。

结论：

```text
PASS for Batch 4a/4b source + doc count
```

但这只是 25 个 patch。仍有大量 raw Harmony patch，尤其 high-risk run/map/save/multiplayer/reward patch 不应继续贸然迁。

---

## 4. Double-patch guard

状态：**source-level 完成。**

`RitsuLibMigrationGuardTests` 已经覆盖：

```text
PatchId 唯一
migrated patch count = 25
migrated classes have no HarmonyPatch attribute
raw HarmonyPatch classes are not registered
RegisterMigratedPatches count matches source
migration.md count matches source
patch-inventory.md lists migrated patches
all expected PatchId strings appear in source
```



结论：

```text
PASS for source-level double-patch prevention
```

但仍不是 runtime proof。ModPatcher 在真实游戏里是否完全等价，还需要 loader smoke 和 gameplay smoke。

---

## 5. Full validation truth

状态：**仍有文档不一致，必须统一。**

你这轮报告说：

```text
Build: 0 errors, 0 warnings
Tests: 428 passed, 0 failed, 21 skipped
```

最新提交搜索又显示 `85a38dd` 的 commit message 说测试数更新到 `444/465`。

`monthly-dev-spec.md` 的版本也出现过多个状态：先是 428，后续 diff 表示改到 444。

`next-overnight-run.md` 当前仍写：

```text
HEAD: aed2a498
Build: 0 errors, 92 warnings
Tests: 444 passed, 0 failed, 21 skipped
```



结论：

```text
PARTIAL PASS
```

自动化测试趋势是绿的，但当前文档仍有“哪个 HEAD、哪个测试数、是否 0 warnings”的真相不统一问题。

下一轮第一件事必须是：

```text
git log -1
dotnet clean
dotnet build
dotnet test
dotnet test --no-build
format
diff check
然后写入唯一 canonical validation doc
```

---

## 6. Runtime smoke

状态：**未完成，当前最大 blocker。**

`runtime-smoke-checklist.md` 明确写：

```text
PENDING — no local game environment available for automated runtime smoke.
```

并要求安装：

```text
Slay the Spire 2 v0.106.1
BaseLib v3.1.4
STS2-RitsuLib
Spire Plus
No other mods
```

所有 loader smoke、Mod Settings UI、gameplay、multiplayer evidence 仍是 pending。

`next-overnight-run.md` 也明确说：

```text
Batch 4c cannot proceed until STS2-RitsuLib is installed and runtime smoke passes.
```



结论：

```text
FAIL / BLOCKED
```

没有 runtime smoke，就不能说 RitsuLib migration runtime-safe，也不能 release。

---

## 7. Sts1Events governance

状态：**默认安全完成；内容完成未完成。**

Sts1Events 当前设计：

```text
Off = default, 0 registrations
CanaryOnly = 4 safe events
AdditiveAllDraft = all events, unsafe/dev-only
ReplaceUnknownEventsPrototype = debug-only/unsafe
```

`Sts1EventFeatureGuardTests` 确认：

```text
env unset -> Off
CanaryOnly exactly 4 events
CanaryOnly only registers BigFish, GoldenIdol, TheLab, DivineFountain
Feature module registered
```



当前 issue 也记录：

```text
Feature gate defaults to Off
Zero events registered unless env var set
CanaryOnly / AdditiveAllDraft / ReplaceUnknownEventsPrototype 有明确风险
```



结论：

```text
PASS for default-safe governance
NOT complete for Sts1Events content
```

AllDraft 里仍有高风险/未完成事件。不能让测试员把它当可玩内容。

---

## 8. FeatureRegistry hardening

状态：**第一层完成。**

`IFeatureModule` 已扩展：

```text
DisplayName
Category
DisableEnvKeys
ForceEnvKeys
```



`FeatureBootstrapRecord` 已有：

```text
Id
DisplayName
Category
Gate
LiveStatus
FailureMessage
IsActive
```



`FeatureRegistry` 已支持：

```text
BootstrapRecords
GetBootstrapRecord
LogFeatureSummary
```

并输出：

```text
bootstrap enabled/disabled
live Enabled/Disabled/Failed
reason
```



结论：

```text
PASS for scaffold hardening
PARTIAL for full runtime governance
```

尚未完成：

```text
Dependencies
runtime evidence status
real gameplay live availability
feature dependency graph
multiplayer policy integration
```

---

## 9. UrdaStateCodec

状态：**第一层完成。**

`UrdaStateCodec` 已存在，支持：

```text
Decode
Encode
legacy minimum part count
legacy/current index handling
malformed fallback
sanitize state fields
```



结论：

```text
PASS for string-state codec bridge
```

但它仍是 SavedSpireField string bridge，不是 RitsuLib DataStore 迁移。下一步应逐渐增加 behavior tests 和 DataStore migration spec。

---

## 10. RewardPipeline

状态：**skeleton 完成，业务未接入。**

`RewardPipeline` 有：

```text
RewardPhase enum
IRewardHandler
RewardPipelineContext
Register
Diagnose
HandlerCount
RegisteredPhases
ClearHandlers
```

并明确写：

```text
Skeleton only — diagnostics and contract enforcement, no behavior changes.
```



结论：

```text
PASS for skeleton
NOT wired into reward gameplay yet
```

---

## 11. CardPlayContext

状态：**skeleton 完成，业务未接入。**

`CardPlayContext` 有：

```text
ExtraPlayPolicy
MaxDepth = 10
TryIncrementDepth
DecrementDepth
Reset
IsPowerFallback
```

并明确写现有 Lotha extra-play 仍使用 per-blessing flags。

结论：

```text
PASS for skeleton
NOT wired into actual extra-play yet
```

---

## 12. DeathProtectionService

状态：**diagnostics-only stub 完成。**

`DeathProtectionService` 有：

```text
DeathProtectionRequest
DeathProtectionResult
DeathProtectionPriority
IDeathProtectionProvider
Register
CheckProtection
ProviderCount
RegisteredPriorities
ClearProviders
```

但源码注释明确说：

```text
Diagnostics-only stub — not wired into game logic.
No actual death prevention occurs.
```



结论：

```text
PASS for stub
NOT real death-protection fix
```

---

## 13. MultiplayerPolicy

状态：**diagnostics-only registry 完成。**

`MultiplayerPolicy` 定义了 6 类：

```text
LocalUiOnly
LocalPlayerOnly
HostAuthoritative
SharedRunState
CombatCommandReplicated
UnsafeInMultiplayer
```

并有 `MultiplayerPolicyRegistry`。但注释明确写：

```text
Diagnostics-only stub — not wired into game logic.
No actual gating or enforcement occurs.
```



结论：

```text
PASS for taxonomy/stub
NOT real multiplayer enforcement
```

---

# 四、当前与我们的目标对比

## 我们的目标

```text
1. 全面 RitsuLib 迁移
2. Runtime proof
3. Patch migration 安全
4. FeatureRegistry 解耦
5. State codec
6. Reward / CardPlay / Death / Multiplayer architecture
7. Sts1Events 安全治理
8. 继续推进但不制造更多 runtime 风险
```

## 当前结果

| 目标                          | 当前状态             | 结论      |
| --------------------------- | ---------------- | ------- |
| RitsuLib dependency         | 已完成              | PASS    |
| Runtime smoke               | 未完成              | BLOCKER |
| Patch migration Batch 4a/4b | 已完成 source-level | PASS    |
| Batch 4c                    | 阻塞               | 正确      |
| FeatureRegistry             | 第一层完成            | PARTIAL |
| UrdaStateCodec              | 第一层完成            | PARTIAL |
| RewardPipeline              | skeleton         | PARTIAL |
| CardPlayContext             | skeleton         | PARTIAL |
| DeathProtectionService      | diagnostics-only | PARTIAL |
| MultiplayerPolicy           | diagnostics-only | PARTIAL |
| Sts1Events Off/CanaryOnly   | 完成               | PASS    |
| Sts1Events full content     | 未完成              | BLOCKER |
| Release-ready               | 否                | 正确      |

---

# 五、综合决策：继续优化、推进，还是两者兼顾？

结论：

```text
优化为主，有限推进为辅。
```

比例建议：

```text
80% optimization / hardening
20% limited advancement
```

现在不该做：

```text
Batch 4c 大量 patch migration
high-risk patch migration
Sts1Events AllDraft live
新增 gameplay
release packaging
```

现在应该做：

```text
runtime smoke
canonical validation truth
Sts1Events governance
FeatureRegistry runtime status
RewardPipeline / CardPlayContext canary 接入
DeathProtection / MultiplayerPolicy 从 stub 走向 enforcement prep
```

---

# 六、下个月开发规范 Monthly Dev Spec

## 月度主题

```text
Runtime Proof + Architecture Integration Month
```

---

## Week 1：Canonical Validation + Runtime Smoke

目标：把测试真相和 runtime 真相统一。

任务：

```text
[ ] git status --short --branch
[ ] git log -1 --oneline --decorate
[ ] dotnet clean
[ ] dotnet build EZMicroBalance.sln
[ ] dotnet test EZMicroBalance.sln
[ ] dotnet test EZMicroBalance.sln --no-build
[ ] dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
[ ] git diff --check
```

创建/更新：

```text
docs/reviews/current-validation.md
```

必须写唯一真相：

```text
HEAD
build warnings count
tests passed/failed/skipped
format status
diff status
whether package was published
```

runtime smoke：

```text
BaseLib v3.1.4
STS2-RitsuLib
Spire Plus
no other mods
```

log 必须证明：

```text
RitsuLib bootstrap starting
ModPatcher applied 25 patches
RitsuLib framework is active
BaseLib initialized
Spire Plus initialized
SavedSpireFields expected count
0 MissingMethodException
0 TypeLoadException
0 manifest dependency failure
```

验收：

```text
[ ] current-validation.md 是唯一真相
[ ] runtime-smoke-checklist.md 至少 loader smoke 有证据
[ ] 若 runtime smoke 失败/无法跑，Batch 4c 继续 blocked
```

---

## Week 2：Sts1Events Governance Finalization

目标：让 Sts1Events 的 4 个模式成为明确、不可误用的治理状态。

任务：

```text
[ ] Off：确认注册 0
[ ] CanaryOnly：确认 4 个 canary safe events
[ ] AdditiveAllDraft：列出所有 HIGH/MEDIUM/TODO/BLOCKED/partial events
[ ] ReplaceUnknownEventsPrototype：标 debug-only unsafe
[ ] DeadAdventurer / Joust / Nloth / Vampires / MindBloom War 进入 blockers
[ ] ZHS missing keys / placeholders 进入 backlog
```

验收：

```text
[ ] 默认包不加载 Sts1Events
[ ] CanaryOnly 可以手测
[ ] AllDraft/ReplaceUnknown 明确 dev-only
```

---

## Week 3：Architecture Canary Integration

目标：让 skeleton 开始接触真实系统，但不改变玩法。

任务：

```text
[ ] RewardPipeline diagnostics 接入一个低风险 reward surface
[ ] CardPlayContext 接入一个低风险 extra-play/fallback path 或 no-op adapter
[ ] FeatureRegistry summary 输出所有 module status
[ ] MultiplayerPolicy 为 active systems 注册 diagnostics policy
```

验收：

```text
[ ] 至少一个真实系统调用 RewardPipeline diagnostics
[ ] 至少一个真实系统使用 CardPlayContext 或 adapter
[ ] 无 gameplay behavior change
[ ] tests 覆盖
```

---

## Week 4：State / Death / Multiplayer Foundations

目标：从 skeleton/spec 走向可测试架构。

任务：

```text
[ ] UrdaStateCodec 增加 behavior tests
[ ] DeathProtectionService 增加 provider tests
[ ] MultiplayerPolicyRegistry 注册 active feature policies
[ ] Lotha DeathReprieve adapter plan
[ ] high-risk migration rollback strategy
```

验收：

```text
[ ] high-risk patch 不迁
[ ] Death/Multiplayer/State 有可执行 plan
[ ] Runtime smoke 仍未过时继续阻塞 Batch 4c
```

---

# 七、给 Codex 的 Overnight Run：必须跑完才能停

下面这段可以直接发给 Codex，替换当前任何偏向 Batch 4c 的计划。

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：Runtime Proof + Architecture Integration Overnight Run。

这不是 Batch 4c。
不要迁更多 patches，除非是修复 guard/runtime blocker 必需。
不要新增 gameplay。
不要 claim release-ready。
必须使用 subagents。
必须跑完所有 phase；如果 blocker 存在，必须记录 issue、证据、下一步，不能直接停止。

当前状态：
- Latest main must be rechecked at start; user reports f4247553 but GitHub may be newer.
- RitsuLib dependency and manifest dependency exist.
- 25 patches migrated to RitsuLib ModPatcher.
- raw Harmony count must be reconciled with latest source.
- Batch 4a/4b source-level closure mostly done.
- Runtime smoke is blocked / pending.
- Sts1Events default Off and CanaryOnly are safe.
- AdditiveAllDraft and ReplaceUnknownEventsPrototype are unsafe/dev-only.
- FeatureRegistry metadata exists but live semantics need runtime proof.
- UrdaStateCodec exists but still uses SavedSpireField bridge.
- RewardPipeline and CardPlayContext are skeletons.
- DeathProtectionService and MultiplayerPolicy are diagnostics-only stubs.
- Do not proceed to Batch 4c until runtime smoke passes.

Subagents:

1. Runtime/Test Truth Agent
   - Run complete validation.
   - Reconcile all test/warning counts.
   - Execute runtime smoke if possible.
   - If runtime smoke blocked, keep Batch 4c blocked.

2. Docs Truth Agent
   - Find stale counts: 324, 361, 387, 428, 444, 0 warnings, 87 warnings, 92 warnings.
   - Replace with canonical latest values.
   - Update current-validation doc.

3. Sts1Events Governance Agent
   - Audit Off / CanaryOnly / AdditiveAllDraft / ReplaceUnknown modes.
   - List all unsafe events and blockers.
   - Ensure safe modes contain no TODO/BLOCKED events.

4. FeatureRegistry Agent
   - Verify all modules expose DisplayName / Category / EnvKeys.
   - Improve bootstrap/live status logs.
   - Add tests for status output.

5. Architecture Integration Agent
   - Wire RewardPipeline diagnostics into one low-risk surface.
   - Wire CardPlayContext into one low-risk canary path.
   - No gameplay behavior change.

6. State/Death/Multiplayer Agent
   - Expand UrdaStateCodec behavior tests.
   - Convert DeathProtectionService from pure stub toward provider-testable service.
   - Register MultiplayerPolicy records for active systems.

Phase 1 — Canonical validation

Run:
- git status --short --branch
- git log -1 --oneline --decorate
- dotnet clean
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check

Write:
- docs/reviews/current-validation.md

Include:
- exact HEAD
- build warning count
- tests passed/failed/skipped
- format result
- diff result
- whether package publish was run

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
- update runtime-smoke-checklist.md

If environment unavailable:
- mark blocker
- do not continue Batch 4c

Phase 3 — Sts1Events governance

- Make issue doc current.
- Off = safe default
- CanaryOnly = controlled test
- AdditiveAllDraft = unsafe/dev-only
- ReplaceUnknownEventsPrototype = unsafe/dev-only
- Add event risk table:
  - event id
  - risk
  - missing API
  - manual evidence needed
- No TODO/BLOCKED event in safe modes.

Phase 4 — FeatureRegistry runtime status

- Log FeatureRegistry summary with:
  - Id
  - DisplayName
  - Category
  - Bootstrap gate
  - Live status
  - reason
- Add tests for all feature modules.
- Keep default-on behavior unchanged.

Phase 5 — Architecture canary

- RewardPipeline diagnostics on one low-risk reward surface.
- CardPlayContext canary on one low-risk extra-play/fallback path.
- No behavior changes.
- Add tests.

Phase 6 — State/Death/Multiplayer foundation

- UrdaStateCodec edge cases.
- DeathProtectionService provider registration tests.
- MultiplayerPolicy records for active features.
- No high-risk gameplay wiring.

Phase 7 — Monthly spec update

Update:
- docs/features/ritsulib-migration/monthly-dev-spec.md
- docs/features/ritsulib-migration/next-overnight-run.md
- docs/reviews/current-validation.md
- docs/issues for runtime/Sts1Events blockers.

Final decision:
- If runtime smoke passed: Batch 4c may be considered next.
- If runtime smoke blocked/failed: optimization continues; Batch 4c blocked.
- If canonical validation has warnings/failures: fix or quarantine before any migration.

Final report must include:
1. actual HEAD
2. full validation results
3. runtime smoke status
4. docs truth cleanup status
5. Sts1Events governance status
6. FeatureRegistry status
7. RewardPipeline/CardPlayContext canary status
8. DeathProtectionService/MultiplayerPolicy status
9. subagent findings
10. files changed
11. decision: optimize / advance / both
12. release-ready: no

Stop condition:
Do not stop until every phase is complete or blocked with issue, evidence, and next action.
```

---

# 八、子代理分工提醒

必须要求它显式使用 subagents：

## Runtime/Test Truth Agent

```text
验证命令
warning count
runtime log
RitsuLib active
```

## Docs Truth Agent

```text
清理 stale counts
统一 HEAD/test/warnings
更新 current-validation
```

## Sts1Events Governance Agent

```text
四模式风险
safe modes
unsafe draft blockers
ZHS/missing keys
```

## FeatureRegistry Agent

```text
metadata
env keys
bootstrap/live status
module tests
```

## Architecture Integration Agent

```text
RewardPipeline
CardPlayContext
canary wiring
无行为改动
```

## State/Death/Multiplayer Agent

```text
UrdaStateCodec
DeathProtection provider tests
MultiplayerPolicy active feature records
```

---

# 九、最终判断

这轮工作不是失败。它已经把项目推进到一个更健康的架构阶段。

但它也还没完成目标：

```text
RitsuLib runtime proof 没有；
架构 skeleton 大多没有接入真实系统；
Sts1Events 仍是 default-off prototype；
Batch 4c 不应继续；
release-ready 否。
```

当前战略：

```text
继续优化为主，有限推进为辅。
```

下一步唯一合理方向：

```text
Runtime smoke + canonical validation + architecture canary integration
```

跑完这些，再决定是否进入 Batch 4c。
