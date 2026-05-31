下面是基于 **当前 GitHub `main` 仓库状态** 的严格审查。旧上传文件里有一部分已经过期；如果你希望我复核以前的日志、截图或旧 Codex 报告，需要重新上传。以下结论不依赖过期附件，只基于当前仓库文件、最新提交记录和你这轮给出的总结。

---

# 1. 当前真实状态校准

当前 GitHub 最新可见提交是：

```text
24d4fe9a — ci: regenerate patch inventory consistently
```

这是当前远程 `main` 的最新提交记录之一。

不过这里已经出现一个需要注意的“文档真相”问题：提交 message 写的是 `444/465`，但仓库当前 `docs/reviews/current-validation.md` 记录的是：

```text
HEAD: 24d4fe9a
dotnet clean + dotnet build: PASS, 0 errors, 89 warnings
dotnet test --no-build: PASS, 461 passed, 0 failed, 21 skipped, 482 total
format: PASS
git diff --check: PASS
runtime smoke: BLOCKED
```



`docs/features/ritsulib-migration/monthly-dev-spec.md` 也同步写了当前验证结果：

```text
0 errors / 89 warnings
461 passed / 0 failed / 21 skipped
runtime smoke blocked
```



所以当前应以 **current-validation.md 和 monthly-dev-spec.md 的 461/482 记录** 为主，而不是 commit message 里的 444/465。下一轮必须继续清理所有 stale count，避免 324 / 361 / 387 / 428 / 444 / 452 / 455 混用。

---

# 2. 从头梳理项目上下文

项目：`dev-the-spire`
当前方向：`Spire Plus` 大型玩法包 + RitsuLib 迁移 + 架构稳定化；`EZMicroBalance` 仅保留为 technical manifest id / install folder。

当前功能面已经很大，至少包括：

```text
1. Ancient reward rebalance v4
2. Ascension 11–20
3. Rootblight / Blight Sprout
4. Urda / Morvi / Lotha / Vakuu Ancient expansion
5. StS1 event port prototype
6. RitsuLib migration
7. Preview tools / website / art / package evidence
8. Multiplayer diagnostics / co-op fail-closed gates
```

之前 bug 多的主要原因不只是某个系统错，而是：

```text
- 功能面过大；
- static service 和 Harmony patch 过多；
- reward / combat / death / save / multiplayer 缺统一管线；
- 状态保存依赖 string / SavedSpireField / WeakTable；
- source-implemented 很快，但 runtime evidence 不足；
- 文档中 “source-ready / live-ready / release-ready” 口径曾经混杂。
```

因此当前主线已经切换为：

```text
RitsuLib-first migration + architecture hardening + runtime proof
```

---

# 3. 当前完成情况逐步审查

## 3.1 RitsuLib 依赖接入

**状态：完成。**

项目已经加入 RitsuLib 编译依赖，并保留 BaseLib 作为过渡期内容模型依赖。此前已经确认 `STS2.RitsuLib` 已进入 csproj，manifest 也依赖 `STS2-RitsuLib`。当前 monthly spec 继续基于这个状态推进。

判定：

```text
PASS
```

但注意：**依赖接入不等于 runtime 证明。**

---

## 3.2 RitsuLibBootstrap / hybrid patch 模式

**状态：阶段完成，但不是完整迁移。**

当前架构是：

```text
已迁移 patch -> RitsuLib ModPatcher
未迁移 patch -> raw Harmony.PatchAll()
```

当前 patch inventory 记录：

```text
Migrated to RitsuLib ModPatcher: 25
Raw HarmonyPatch remaining: 142
```



判定：

```text
PASS for hybrid bootstrap
NOT complete for full RitsuLib migration
```

这是一种合理的过渡方案，但不代表全部 patch 已经迁完。

---

## 3.3 Batch 4a / 4b patch migration

**状态：source-level 基本完成。**

当前 migration spec 记录：

```text
25 patches migrated
142 raw Harmony remaining
hybrid bootstrap active
```



patch inventory 也记录：

```text
Total patch declarations: 142
Migrated to RitsuLib ModPatcher: 25
Raw HarmonyPatch remaining: 142
High risk raw: 22
Medium risk raw: 35
Low risk raw: 85
```



判定：

```text
PASS for Batch 4a/4b source-level closure
```

但这里有一个文档表达问题：

```text
Total patch declarations = 142
Raw HarmonyPatch remaining = 142
Migrated = 25
```

这个表容易让人误解为总数只有 142，但另有 25 个 migrated patches。建议下一轮改成：

```text
Migrated IPatchMethod classes: 25
Raw HarmonyPatch declarations remaining: 142
Tracked patch units total: 167
```

这样避免误读。

---

## 3.4 Double-patch guard

**状态：source-level 完成。**

当前 RitsuLib migration guard 已经覆盖：

```text
- PatchId unique
- migrated patch count = 25
- migrated patch classes have no HarmonyPatch
- raw HarmonyPatch classes are not registered
- RegisterMigratedPatches count matches source
- migration.md count matches source
- patch-inventory.md lists migrated patches
```

这些内容在之前的 guard 文件中已经可见，当前 monthly spec 也把 double-patch guard 作为完成项记录。

判定：

```text
PASS for source-level guard
PENDING for runtime behavior proof
```

这能防止最明显的双 patch，但还不能证明游戏 runtime 里 ModPatcher 行为和 raw Harmony 行为完全等价。

---

## 3.5 自动化测试真相

**状态：部分完成，但仍需继续维护 canonical truth。**

当前 `current-validation.md` 记录：

```text
build: 0 errors, 89 warnings
test --no-build: 461 passed, 0 failed, 21 skipped
format: pass
diff check: pass
```



当前 monthly spec 也写：

```text
Latest validation: 461 passed / 0 failed / 21 skipped
0 errors / 89 warnings
```



判定：

```text
PARTIAL PASS
```

原因：

1. 当前记录的是 `dotnet test --no-build`，不是完整 `dotnet test`。
2. commit message 仍出现 444/465，而 current-validation 是 461/482。
3. 仍有 89 warnings，虽然都在 Sts1Events prototype 范围内，但不能长期放任。

下一轮必须建立更严格的 canonical validation：

```text
dotnet clean
dotnet build
dotnet test
dotnet test --no-build
dotnet format
git diff --check
```

并统一所有文档中的测试数字。

---

## 3.6 Build warnings

**状态：可接受但未解决。**

当前 `current-validation.md` 写：

```text
89 warnings
warning codes: CS8602, CS8604, CS8625
scope: EZMicroBalanceCode/Sts1Events/Models/
decision: issue-worthy, accepted only because Sts1Events is gated Off by default and still prototype/dev-only outside Canary/Batch1 test modes
```



判定：

```text
OPEN
```

当前接受原因是合理的：Sts1Events 默认 Off，不进入普通 live path。
但如果后续要启用 CanaryOnly / AdditiveBatch1 / AllDraft，必须逐步处理 nullable warnings。

---

## 3.7 Runtime smoke

**状态：未完成，是当前最大 blocker。**

`current-validation.md` 明确写：

```text
Runtime Smoke: BLOCKED
D:\...\mods\STS2-RitsuLib 不存在
D:\...\mods\BaseLib 不存在
D:\...\mods\EZMicroBalance 不存在
E:\...\mods\STS2-RitsuLib 不存在
Batch 4c remains blocked
No runtime safety or release-readiness claim
```



`next-overnight-run.md` 也明确说：

```text
Runtime smoke remains the critical path blocker.
Batch 4c cannot proceed until STS2-RitsuLib is installed and runtime smoke passes.
```



判定：

```text
FAIL / BLOCKED
```

这是当前最高优先级。没有 runtime smoke，就不能说：

```text
RitsuLib runtime safe
ModPatcher runtime equivalent
release candidate
Batch 4c allowed
```

---

## 3.8 Sts1Events governance

**状态：默认安全完成；内容未完成。**

当前 Sts1Events issue 已经变成 current 状态，写明：

```text
Open — governance hardened, content incomplete.
Default Off is safe.
CanaryOnly and AdditiveBatch1 are controlled source-test modes.
AdditiveAllDraft and ReplaceUnknownEventsPrototype are dev-only/unsafe.
```



当前模式矩阵：

```text
Off: 0 registrations, safe
CanaryOnly: 4 registrations / 4 events, controlled
AdditiveBatch1: 11 registrations / 10 events, controlled prototype testing
AdditiveAllDraft: 54 registration calls / 47 unique event types, unsafe/dev-only
ReplaceUnknownEventsPrototype: unsafe/debug-only
```



风险表明确列出：

```text
Dead Adventurer — TODO combat path no-op
Scorpion Nest — TODO combat path no-op
Treasure Ooze — TODO combat path no-op
Masked Bandits — TODO fight no-op
Mind Bloom — BLOCKED War no-op
Mysterious Sphere — TODO combat no-op
N'loth — BLOCKED relic select
Vampires — partial, no Bite cards
```



判定：

```text
PASS for governance
NOT complete for content
```

这个状态是正确的：Sts1Events 不能算完成，但默认安全。

---

## 3.9 FeatureRegistry

**状态：第一层 hardening 完成。**

当前 monthly spec 记录：

```text
IFeatureModule metadata
FeatureBootstrapRecord
LiveStatus enum
unified truthy env key overrides
metadata/override guard tests
```



当前 feature registry 已有 bootstrap/live status 的概念。判定：

```text
PASS for scaffold hardening
PARTIAL for runtime governance
```

仍需补：

```text
- dependencies
- actual runtime evidence status
- status export
- multiplayer policy integration
- feature dependency graph
```

---

## 3.10 UrdaStateCodec

**状态：第一层完成。**

Monthly spec 记录：

```text
UrdaStateCodec V1: encode/decode/legacy compat, 41 tests
```



判定：

```text
PASS for codec bridge
NOT complete for RitsuLib DataStore migration
```

当前仍是 SavedSpireField string bridge + codec，不是完整 RitsuLib persistence migration。作为阶段性成果可以接受。

---

## 3.11 RewardPipeline / CardPlayContext

**状态：skeleton / canary 完成，未完全接入 gameplay。**

Monthly spec 写：

```text
RewardPipeline diagnostics are wired into FeatureRegistry bootstrap events
CardPlayContext canary is touched by Lotha extra-play through an allow-only adapter
No gameplay behavior changes intended
```



判定：

```text
PASS for architecture canary
NOT complete for real pipeline enforcement
```

这是合理的中间阶段。下一步应接入一个更真实但低风险的 surface，而不是直接大改 gameplay。

---

## 3.12 DeathProtectionService

**状态：diagnostics-only stub。**

`DeathProtectionService` 明确写：

```text
Diagnostics-only stub — not wired into game logic.
No actual death prevention occurs.
```

它已有 Request / Result / Priority / Provider / Registry 结构。

判定：

```text
PASS for stub
NOT real Death Reprieve fix
```

后续还不能声称 Lotha DeathReprieve 递归风险已解决。

---

## 3.13 MultiplayerPolicy

**状态：diagnostics-only registry。**

`MultiplayerPolicy` 明确写：

```text
Diagnostics-only stub — not wired into game logic.
No actual gating or enforcement occurs.
```

它定义了 6 类 policy，并有 registry。

判定：

```text
PASS for taxonomy
NOT real multiplayer enforcement
```

这对规划很有价值，但还不能当作 co-op 安全证据。

---

# 4. 当前目标对比

## 原目标

```text
1. RitsuLib dependency landing
2. Patch migration safe closure
3. Runtime smoke
4. FeatureRegistry hardening
5. UrdaStateCodec
6. RewardPipeline / CardPlayContext
7. DeathProtectionService / MultiplayerPolicy
8. Sts1Events governance
9. 不 release-ready
```

## 当前实际

| 目标                               | 状态                 |
| -------------------------------- | ------------------ |
| RitsuLib dependency              | 完成                 |
| Patch migration Batch 4a/4b      | 完成 source-level    |
| Double-patch guard               | 完成 source-level    |
| Runtime smoke                    | 未完成                |
| FeatureRegistry hardening        | 第一层完成              |
| UrdaStateCodec                   | 第一层完成              |
| RewardPipeline / CardPlayContext | skeleton/canary 完成 |
| DeathProtectionService           | diagnostics-only   |
| MultiplayerPolicy                | diagnostics-only   |
| Sts1Events governance            | default-safe 完成    |
| Sts1Events content               | 未完成                |
| Release-ready                    | 否，正确               |

---

# 5. 综合决策：继续优化、推进，还是两者兼顾？

**结论：继续优化为主，有限推进为辅。**

建议比例：

```text
80% 优化 / 稳定 / 验证
20% 有限推进 / canary integration
```

当前不应该继续：

```text
Batch 4c 大量 patch migration
high-risk patch migration
Sts1Events AllDraft live
release packaging
new gameplay
```

当前应该继续：

```text
runtime smoke
canonical validation truth
Sts1Events CanaryOnly runtime proof
FeatureRegistry runtime logging
RewardPipeline/CardPlayContext low-risk canary
DeathProtection/MultiplayerPolicy provider/policy records
```

---

# 6. 下个月开发规范 Monthly Dev Spec

## 月度主题

```text
Runtime Proof + Canary Integration Month
```

## 月度目标

```text
1. 完成 BaseLib + STS2-RitsuLib + Spire Plus runtime smoke。
2. 建立唯一 validation truth。
3. 保持 Sts1Events safe modes，推进 CanaryOnly runtime proof。
4. 将 RewardPipeline/CardPlayContext 从 skeleton 推进到 low-risk real surface canary。
5. 将 DeathProtectionService/MultiplayerPolicy 从 diagnostics-only 推进到 provider/policy testable layer。
6. 只有 runtime smoke 通过后，才评估 Batch 4c。
```

---

## Week 1：Canonical Validation + Runtime Smoke

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

更新：

```text
docs/reviews/current-validation.md
```

必须写唯一真相：

```text
HEAD
build errors/warnings
test passed/failed/skipped
format result
diff result
publish/package status
```

runtime smoke：

```text
[ ] 安装 BaseLib v3.1.4
[ ] 安装 STS2-RitsuLib
[ ] 安装 Spire Plus
[ ] 移除其他 mods
[ ] 启动 Steam 客户端
[ ] 捕获 godot.log
[ ] 运行 audit-godot-log
```

日志必须验证：

```text
[ ] RitsuLib bootstrap starting
[ ] ModPatcher applied 25 patches
[ ] RitsuLib framework is active
[ ] BaseLib initialized
[ ] Spire Plus initialized
[ ] SavedSpireFields expected count
[ ] no MissingMethodException
[ ] no TypeLoadException
[ ] no manifest dependency failure
```

验收：

```text
[ ] current-validation.md 是唯一真相
[ ] runtime-smoke-checklist.md 有真实 evidence
[ ] 若 runtime smoke 失败/缺失，Batch 4c 继续 blocked
```

---

## Week 2：Sts1Events Canary Runtime

任务：

```text
[ ] Off mode runtime smoke：0 registrations
[ ] CanaryOnly runtime smoke：4 registrations
[ ] Debug-spawn BigFish / GoldenIdol / TheLab / DivineFountain
[ ] 手测 4 个 canary 事件
[ ] save/load after event completion
[ ] EN/ZHS 渲染检查
```

仍保持：

```text
AdditiveBatch1 = controlled prototype only
AdditiveAllDraft = unsafe/dev-only
ReplaceUnknownEventsPrototype = unsafe/debug-only
```

验收：

```text
[ ] CanaryOnly 从 source-safe 进入 runtime-proven
[ ] AllDraft 不进入 release path
```

---

## Week 3：Architecture Canary Integration

任务：

```text
[ ] RewardPipeline diagnostics 接入一个实际 reward surface
[ ] CardPlayContext 接入一个实际 low-risk card-play path
[ ] FeatureRegistry summary 输出到 runtime log
[ ] MultiplayerPolicy 注册所有 active feature policy records
[ ] DeathProtectionService 注册 no-op provider test path
```

要求：

```text
不改变 gameplay behavior
只做 diagnostics / canary / guard
```

验收：

```text
[ ] runtime log 能看到 architecture diagnostics
[ ] tests 全绿
[ ] 无行为回归
```

---

## Week 4：Batch 4c Decision Gate

只有 runtime smoke 通过后才允许评估 Batch 4c。

如果 runtime smoke 通过：

```text
[ ] 选择 5–10 个真正 low-risk patch candidates
[ ] 每个 patch 有 rollback plan
[ ] 每迁一个跑 build + targeted tests
[ ] 更新 inventory
[ ] 不碰 high-risk patch
```

如果 runtime smoke 失败或未跑：

```text
[ ] 不迁 Batch 4c
[ ] 继续修 runtime blocker
[ ] 产出 issue 和 evidence
```

验收：

```text
Batch 4c 只有 runtime smoke passed 后才允许。
```

---

# 7. 子代理分工要求

必须显式要求 Codex 使用 subagents。建议固定 6 个：

## Subagent A — Runtime/Test Truth Agent

负责：

```text
完整 validation
runtime smoke
godot.log
audit-godot-log
current-validation.md
```

## Subagent B — Docs Truth Agent

负责：

```text
清理 stale counts
统一 HEAD
统一 warnings/test/runtime status
确保无 release-ready 假 claim
```

## Subagent C — Sts1Events Governance Agent

负责：

```text
Off / CanaryOnly / AdditiveBatch1 / AdditiveAllDraft / ReplaceUnknownEventsPrototype
safe mode runtime matrix
risk table
ZHS / placeholders
```

## Subagent D — Architecture Integration Agent

负责：

```text
RewardPipeline
CardPlayContext
FeatureRegistry runtime diagnostics
no behavior change canaries
```

## Subagent E — State/Death/Multiplayer Agent

负责：

```text
UrdaStateCodec
DeathProtection provider tests
MultiplayerPolicy active feature records
```

## Subagent F — Release Gate Agent

负责：

```text
阻止 release-ready claim
阻止 AllDraft 进入 release
阻止 Batch 4c 越过 runtime smoke
阻止 high-risk patch migration
```

---

# 8. Overnight Run Spec：必须跑完才能停止

下面可以直接发给 Codex。它必须跑完；无法完成的阶段必须留下 issue、证据、下一步。

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：Runtime Proof + Canary Integration Overnight Run。

这不是 Batch 4c。
不要迁更多 patches，除非 runtime smoke 已通过且明确进入 Batch 4c decision gate。
不要新增 gameplay。
不要 claim release-ready。
必须使用 subagents。
必须跑完所有 phase；如果 blocker 存在，必须记录 issue、证据、下一步，不能直接停止。

当前状态：
- Latest main must be checked at start.
- RitsuLib dependency and manifest dependency exist.
- 25 patches migrated to RitsuLib ModPatcher.
- Raw Harmony count must be reconciled with latest source.
- Runtime smoke is blocked until STS2-RitsuLib is installed.
- Sts1Events Off and CanaryOnly are safe by source/tests.
- AdditiveBatch1 is controlled prototype.
- AdditiveAllDraft and ReplaceUnknownEventsPrototype are unsafe/dev-only.
- FeatureRegistry metadata exists.
- UrdaStateCodec exists.
- RewardPipeline/CardPlayContext/DeathProtectionService/MultiplayerPolicy are canary/skeleton/diagnostics, not full gameplay enforcement.
- Do not proceed to Batch 4c until runtime smoke passes.

Subagents:

1. Runtime/Test Truth Agent
   - Run complete validation.
   - Execute runtime smoke if environment exists.
   - Record godot.log and audit.
   - Create/update docs/reviews/current-validation.md.

2. Docs Truth Agent
   - Remove stale counts.
   - Unify latest HEAD, test count, warning count, runtime status.
   - Ensure no doc says runtime-ready or release-ready.

3. Sts1Events Governance Agent
   - Verify Off, CanaryOnly, AdditiveBatch1, AdditiveAllDraft, ReplaceUnknown modes.
   - Ensure safe modes contain no TODO/BLOCKED events.
   - Prepare canary runtime matrix.

4. Architecture Integration Agent
   - Wire RewardPipeline diagnostics into one real low-risk surface.
   - Wire CardPlayContext canary into one low-risk path.
   - Register MultiplayerPolicy records for active systems.
   - Keep gameplay behavior unchanged.

5. State/Death/Multiplayer Agent
   - Expand UrdaStateCodec behavior tests.
   - Convert DeathProtectionService from pure stub toward provider-testable service.
   - Register MultiplayerPolicy records for active systems.

6. Release Gate Agent
   - Do not allow release-ready claim.
   - Do not allow AllDraft to be release-safe.
   - Do not allow Batch 4c before runtime smoke.
   - Do not allow high-risk patch migration.

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

Write docs/reviews/current-validation.md with:
- exact HEAD
- build errors/warnings
- test passed/failed/skipped
- format result
- diff result
- whether package publish was run

Phase 2 — Runtime smoke

If game environment exists:
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
- do not proceed to Batch 4c

Phase 3 — Sts1Events runtime gates

If runtime smoke passes:
- test Off mode: 0 registrations
- test CanaryOnly mode: exactly 4 registrations
- debug-spawn 4 canary events if tooling exists
- update Sts1Events issue evidence

If runtime smoke unavailable:
- keep source/test status only
- mark runtime pending

Phase 4 — Architecture canary integration

- RewardPipeline diagnostics on one low-risk real surface.
- CardPlayContext canary on one low-risk real surface.
- MultiplayerPolicy records for active features.
- DeathProtectionService provider tests.
- No gameplay behavior changes.

Phase 5 — Batch 4c decision

If runtime smoke passed:
- propose 5–10 low-risk Batch 4c candidates.
- do not migrate them unless explicitly accepted in this run's scope.

If runtime smoke blocked/failed:
- Batch 4c remains blocked.

Phase 6 — Monthly spec update

Update:
- docs/features/ritsulib-migration/monthly-dev-spec.md
- docs/features/ritsulib-migration/next-overnight-run.md
- docs/reviews/current-validation.md
- runtime-smoke-checklist.md
- Sts1Events issue if evidence changes

Final report must include:
1. actual HEAD
2. full validation result
3. runtime smoke status
4. Sts1Events runtime/source status
5. FeatureRegistry status
6. RewardPipeline/CardPlayContext canary status
7. DeathProtectionService/MultiplayerPolicy status
8. Batch 4c decision
9. subagent findings
10. files changed
11. decision: optimize / advance / both
12. release-ready: no

Stop condition:
Do not stop until every phase is complete or blocked with explicit issue, evidence, and next action.
```

---

# 9. 最终判断

这轮工作不是失败。项目比以前健康很多：

```text
RitsuLib patch migration 有 guard；
Sts1Events 有 default Off；
FeatureRegistry 有 metadata；
state / reward / card / death / multiplayer 都有 skeleton；
测试数显著增长；
文档治理改善。
```

但核心目标仍没完成：

```text
RitsuLib runtime proof 没有；
architecture skeleton 多数未真正接入 gameplay；
Sts1Events 仍是 prototype；
Batch 4c 仍应阻塞；
release-ready 否。
```

因此当前策略是：

```text
继续优化为主，有限推进为辅。
```

下一步主线：

```text
Runtime smoke + canonical validation + architecture canary integration
```

只有 runtime smoke 通过后，才考虑 Batch 4c。
