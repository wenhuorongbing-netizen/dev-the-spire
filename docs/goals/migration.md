部分早前上传的文件已经过期，我现在不能读取旧附件；如果你要我复核旧日志、截图或旧 Codex 报告，需要重新上传。下面这份审查只基于**当前 GitHub 远程仓库**、你这轮给出的状态，以及我刚刚重新读取的仓库文件。

# 严格审核结论

**任务没有完全完成。**

它已经完成了大量架构稳定化工作，尤其是 RitsuLib 迁移的 source-level guard、Sts1Events 治理、FeatureRegistry 扩展、UrdaStateCodec、RewardPipeline / CardPlayContext / DeathProtectionService / MultiplayerPolicy 的骨架与部分 canary；但它仍然没有完成最关键的 **runtime proof**，也没有达到 release-ready 或 full migration-ready 状态。

当前最准确的状态是：

```text
RitsuLib migration：进行中
Batch 4a/4b source-level closure：基本完成
Architecture canary：进行中
Runtime smoke：未完成，当前最大 blocker
Sts1Events：默认安全，内容未完成
Batch 4c：继续阻塞
Release-ready：否
```

战略上应当是：

```text
继续优化为主，有限推进为辅。
```

不能继续无条件推进 Batch 4c，也不能新增玩法。下一步必须先统一验证真相、完成 runtime smoke、巩固架构 canary，再决定是否推进新迁移批次。

---

# 1. 当前 GitHub 状态校准

当前 GitHub 最新提交已经不是你报告中的 `f4247553`，也不是 `85a38dd`，而是：

```text
8782030 — sprint 1
```

提交列表显示它位于当前最新位置，后面依次是 `24d4fe9`、`55faa34`、`8e4444a`、`85a38dd` 等。

`docs/reviews/current-validation.md` 当前已经刷新到：

```text
87820303
```

并且写明该验证基于 `87820303`。

此前仓库存在一个明确问题：

```text
最新 main 曾经超过 current-validation.md 的验证基线。
```

本轮已经重新跑 refactor validation；后续如果 HEAD 再变化，仍不能直接沿用 `87820303` 的结果代表新提交。

---

# 2. 自动化验证状态审查

`docs/reviews/current-validation.md` 当前记录：

```text
dotnet clean + build: PASS after retry
build: 0 errors, 89 warnings
dotnet test: prior full PASS, 462 passed / 0 failed / 21 skipped / 483 total
dotnet test --no-build: current project PASS, 464 passed / 0 failed / 21 skipped / 485 total
format: PASS
git diff --check: PASS
runtime smoke: BLOCKED
```



它还记录了此前有 stale `testhost` 锁文件和测试主机 crash，后来通过停止 stale PID 并禁用默认并行后才稳定通过。

## 审核判断

```text
自动化验证在 87820303 上基本可信。
但它不覆盖未来新提交。
```

所以当前状态不是“最终绿灯”，而是：

```text
当前 no-game 验证基线绿，runtime smoke 仍 hard blocked。
```

下一个 overnight run 必须第一步重新运行：

```powershell
git log -1 --oneline --decorate
dotnet clean
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

然后把 `docs/reviews/current-validation.md` 刷到最新 HEAD。

---

# 3. RitsuLib runtime smoke 审查

当前 runtime smoke 仍是最大阻塞。

`current-validation.md` 写：

```text
E:\Steam...\Slay the Spire 2 和 mods folder 存在；
E-drive BaseLib 和 EZMicroBalance 存在；
E-drive STS2-RitsuLib 不存在；
D-drive game root / mods / BaseLib / STS2-RitsuLib / EZMicroBalance 均不存在；
Batch 4c remains blocked。
```



`monthly-dev-spec.md` 也明确写：

```text
Runtime smoke: hard blocked because STS2-RitsuLib is not installed locally.
Batch 4c, high-risk migration, Off/CanaryOnly runtime claims, live-ready, release-ready remain blocked.
```



`next-overnight-run.md` 同样写：

```text
Runtime smoke remains the critical path blocker.
Batch 4c cannot proceed until STS2-RitsuLib is installed and runtime smoke passes.
```



## 审核判断

```text
FAIL / HARD BLOCKED
```

没有 STS2-RitsuLib 安装和 clean `godot.log`，不能声明：

```text
RitsuLib runtime-safe
ModPatcher runtime-safe
Off mode runtime-safe
CanaryOnly runtime-safe
release-ready
Batch 4c allowed
```

---

# 4. Patch migration 审查

当前 patch inventory 写：

```text
Total raw HarmonyPatch declarations: 142
Migrated to RitsuLib ModPatcher: 25
Raw HarmonyPatch remaining: 142
High risk raw: 22
Medium risk raw: 35
Low risk raw: 85
```



monthly spec 写：

```text
25 patches migrated
142 raw Harmony remaining
tracked patch units total = 167
hybrid bootstrap active
```



## 审核判断

```text
Batch 4a/4b source-level closure：PASS
Full patch migration：NOT DONE
```

注意这个表述还有一点容易混淆：

```text
Total raw HarmonyPatch declarations = 142
Raw HarmonyPatch remaining = 142
Migrated = 25
```

这容易让读者误解 “total” 是全部 patch。更准确应写：

```text
Raw HarmonyPatch declarations remaining: 142
Migrated IPatchMethod classes: 25
Tracked runtime patch units: 167
```

当前 monthly spec 已经开始使用 “tracked patch units total: 167”，这是正确方向。

---

# 5. Double-patch guard 审查

当前 monthly spec 记录：

```text
Double-patch guard tests ensure no patch is applied twice.
Source-level separation: migrated patch classes live in RitsuLib namespace and implement IPatchMethod; raw Harmony patches remain with HarmonyPatch attributes.
```



此前 `RitsuLibMigrationGuardTests` 已经覆盖 PatchId unique、migrated patch class 不含 `[HarmonyPatch]`、raw Harmony class 不进入 RegisterMigratedPatches 等 source-level 规则。

## 审核判断

```text
source-level PASS
runtime-level PENDING
```

这能防止明显双 patch，但不能替代 runtime smoke。

---

# 6. Sts1Events 审查

当前 Sts1Events issue 明确写：

```text
Open — governance hardened, content incomplete.
Default Off is safe.
CanaryOnly and AdditiveBatch1 are controlled source-test modes.
AdditiveAllDraft and ReplaceUnknownEventsPrototype are dev-only/unsafe and require the explicit unsafe override `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; replacement also requires `REPLACEMENT_PROTOTYPE_ENABLED`.
```



模式矩阵如下：

```text
Off: 0 registrations, safe
CanaryOnly: 4 registrations / 4 event types
AdditiveBatch1: 11 registrations / 10 event types
AdditiveAllDraft: 54 calls / 47 unique event types only with explicit unsafe override, unsafe/dev-only
ReplaceUnknownEventsPrototype: fail-closed in normal builds; debug-only with compile symbol and unsafe override, unsafe
```



AdditiveAllDraft 风险表已经列出：

```text
Dead Adventurer — combat path no-op
Scorpion Nest — combat path no-op
Treasure Ooze — combat path no-op
Masked Bandits — FIGHT no-op
Mind Bloom WAR — blocked no-op
Mysterious Sphere — combat path no-op
N'loth — relic select no-op
Vampires — partial, no Bite cards
```



## 审核判断

```text
Default Off safety：PASS
CanaryOnly source safety：PASS
AdditiveBatch1：controlled prototype only
AdditiveAllDraft / ReplaceUnknown：unsafe/dev-only
Full Sts1Events content：NOT DONE
Runtime Sts1Events proof：PENDING
```

这是当前正确状态。不能把 Sts1Events 当作完成内容，也不能让 AllDraft 进入普通测试或发布路径。

---

# 7. FeatureRegistry 审查

`IFeatureModule` 当前已有：

```text
Id
InitOrder
EvaluateGate
Initialize
DisplayName
Category
DisableEnvKeys
ForceEnvKeys
```



`FeatureBootstrapRecord` 当前有：

```text
FeatureLiveStatus.Enabled / Disabled / Failed
Id
DisplayName
Category
Gate
LiveStatus
FailureMessage
IsActive
```



`FeatureRegistry` 当前会：

```text
按 InitOrder + Id 初始化
EvaluateGate
写 BootstrapRecords
支持 GetBootstrapRecord
支持 LogFeatureSummary
```



Monthly spec 记录：

```text
FeatureRegistry hardened with metadata, bootstrap record, live status enum, truthy env key overrides.
```



## 审核判断

```text
FeatureRegistry scaffold hardening：PASS
Full feature governance：PARTIAL
```

缺的还有：

```text
Dependencies
feature dependency graph
runtime evidence status
status export
multiplayer policy integration as enforcement
```

但作为当前阶段，它已经明显进步。

---

# 8. UrdaStateCodec 审查

`UrdaStateCodec` 当前包含：

```text
UrdaStateSnapshot
Decode
Encode
legacy/current positional decode
malformed fallback
sanitize semicolon fields
```



最新 commit `8782030` 还修改了 decode 逻辑：

```text
HumusCompletionPending before MoltingActive; older full states have 28 parts.
hasHumusPendingField = parts.Length >= CurrentBaseIndex && parts.Length != 28
```



Monthly spec 写：

```text
UrdaStateCodec V1 encode/decode/legacy compat, current full positional decode, legacy full positional decode, edge cases.
```



## 审核判断

```text
PASS for codec bridge
NOT full RitsuLib DataStore migration
```

这是正确阶段。后续要继续加强旧存档迁移和行为测试。

---

# 9. RewardPipeline / CardPlayContext 审查

`RewardPipeline` 已经有：

```text
RewardPhase
IRewardHandler
RewardPipelineContext
RewardPipeline.Register
RewardPipeline.Diagnose
HandlerCount
RegisteredPhases
ClearHandlers
```

并明确写：

```text
Skeleton only — diagnostics and contract enforcement, no behavior changes.
```



`CardPlayContext` 已经有：

```text
ExtraPlayPolicy
MaxDepth = 10
TryIncrementDepth
DecrementDepth
Reset
IsPowerFallback
```

并明确写现有 Lotha extra-play 仍用 per-blessing flags。

最新 `8782030` commit 显示 RewardPipeline diagnostics 已接入 AscensionRewardService card reward / room reward surfaces，并且 CardPlayContext canary 增加 ReleaseEvidenceLog 记录。

Monthly spec 也写：

```text
RewardPipeline diagnostics are wired into FeatureRegistry bootstrap events and low-risk Ascension reward/card-reward surfaces.
CardPlayContext canary emits diagnostics through existing Lotha extra-play allow-only adapter.
No gameplay behavior changes are claimed.
```



## 审核判断

```text
RewardPipeline diagnostics canary：PASS
CardPlayContext canary：PASS
Gameplay enforcement：NOT DONE
```

这符合当前阶段。它们开始接触真实系统，但仍不改变玩法。

---

# 10. DeathProtectionService / MultiplayerPolicy 审查

`DeathProtectionService` 已存在，包含：

```text
DeathProtectionRequest
DeathProtectionResult
DeathProtectionPriority
IDeathProtectionProvider
Register
CheckProtection
ClearProviders
```

并明确写：

```text
Diagnostics-only stub — not wired into game logic.
No actual death prevention occurs.
```



`MultiplayerPolicy` 已存在，包含：

```text
LocalUiOnly
LocalPlayerOnly
HostAuthoritative
SharedRunState
CombatCommandReplicated
UnsafeInMultiplayer
MultiplayerPolicyRegistry
```

并明确写：

```text
Diagnostics-only stub — not wired into game logic.
No actual gating or enforcement occurs.
```



最新 `8782030` commit 显示 ArchitectureCanaryBootstrap 增加了 NoOpDeathProtectionProvider，注册了大量 multiplayer policy records，并把 co-op evidence payload 增加了 policy metadata。

## 审核判断

```text
DeathProtectionService provider-testable layer：PASS
MultiplayerPolicy diagnostics registry：PASS
Actual death protection gameplay：NOT DONE
Actual multiplayer enforcement：NOT DONE
```

当前状态正确：它们是 foundation，不是最终修复。

---

# 11. 当前任务是否完成？

如果目标是：

```text
完成 Runtime Proof + Architecture Integration
```

那么答案是：

```text
未完成。
```

如果目标是：

```text
完成 source-level architecture scaffold / canary / guard
```

那么答案是：

```text
基本完成。
```

当前最准确的验收表：

| 项目                        | 状态                     |
| ------------------------- | ---------------------- |
| RitsuLib dependency       | 完成                     |
| Batch 4a/4b               | source-level 完成        |
| Double-patch guard        | source-level 完成        |
| Current validation        | 24d4 基线完成，最新 878 需重跑   |
| Runtime smoke             | 未完成                    |
| Sts1Events default safety | 完成                     |
| Sts1Events content        | 未完成                    |
| FeatureRegistry hardening | 部分完成                   |
| UrdaStateCodec            | 完成第一层                  |
| RewardPipeline canary     | 已开始                    |
| CardPlayContext canary    | 已开始                    |
| DeathProtectionService    | provider-testable stub |
| MultiplayerPolicy         | diagnostics registry   |
| Batch 4c                  | 必须阻塞                   |
| Release-ready             | 否                      |

---

# 12. 继续优化、推进，还是兼顾？

**结论：优化为主，有限推进为辅。**

建议比例：

```text
85% 优化 / 验证 / 稳定化
15% 有限推进 / canary integration
```

现在不该做：

```text
Batch 4c 大量迁 patch
High-risk patch migration
Sts1Events AllDraft live
新 gameplay
Release packaging
```

现在该做：

```text
1. 重新跑最新 HEAD validation。
2. 安装 STS2-RitsuLib 并完成 runtime smoke。
3. 继续把 architecture canary 接入低风险 surface。
4. 继续治理 Sts1Events warning / placeholder / blocked events。
5. 维持 Batch 4c blocked until runtime smoke passes。
```

---

# 13. 下个月开发规范 Monthly Dev Spec

## 月度主题

```text
Runtime Proof + StS1 Governance + Architecture Canary Month
```

## Week 1：Latest HEAD Validation + Runtime Smoke

### 目标

把当前 `8782030` 或最新 HEAD 的验证和 runtime 证据建立起来。

### 任务

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

写入：

```text
docs/reviews/current-validation.md
```

runtime smoke：

```text
[ ] 安装 STS2-RitsuLib 到 <GameRoot>/mods/STS2-RitsuLib
[ ] 确认 BaseLib v3.1.4
[ ] 确认 Spire Plus
[ ] 移除其他 mods
[ ] 启动游戏
[ ] 收集 godot.log
```

log 必须证明：

```text
[ ] RitsuLib bootstrap starting
[ ] ModPatcher applied 25 patches
[ ] RitsuLib framework is active
[ ] BaseLib initialized
[ ] Spire Plus initialized
[ ] no MissingMethodException
[ ] no TypeLoadException
[ ] no manifest dependency failure
```

验收：

```text
[ ] current-validation.md 代表最新 HEAD
[ ] runtime-smoke-checklist.md 有真实 evidence
[ ] runtime 不过则 Batch 4c 继续 blocked
```

---

## Week 2：Sts1Events Governance + Canary Runtime

### 任务

```text
[ ] Off mode runtime：0 registrations
[ ] CanaryOnly runtime：4 registrations
[ ] Debug-spawn 4 canary events
[ ] 手测 BigFish / GoldenIdol / TheLab / DivineFountain
[ ] Save/load after canary event completion
[ ] ZHS/EN 渲染检查
```

继续保持：

```text
AdditiveBatch1 = controlled prototype only
AdditiveAllDraft = unsafe/dev-only and requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`
ReplaceUnknownEventsPrototype = unsafe/debug-only and requires `REPLACEMENT_PROTOTYPE_ENABLED` plus `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`
```

验收：

```text
[ ] CanaryOnly 从 source-safe 升级到 runtime-proven
[ ] AllDraft 仍不进入 release path
```

---

## Week 3：Architecture Canary Integration

### 任务

```text
[ ] RewardPipeline diagnostics 继续接入一个低风险 reward surface
[ ] CardPlayContext 继续接入一个低风险 card-play surface
[ ] DeathProtectionService 注册 no-op provider 并输出 runtime evidence
[ ] MultiplayerPolicy records 覆盖所有 active feature
[ ] FeatureRegistry summary 输出到 runtime log
```

验收：

```text
[ ] 无 gameplay behavior change
[ ] runtime log 可看到 architecture diagnostics
[ ] tests 全绿
```

---

## Week 4：Batch 4c Decision Gate

只有 runtime smoke 通过后才允许评估。

如果 runtime smoke 通过：

```text
[ ] 选择 5–10 个真正 low-risk patch candidates
[ ] 每个 candidate 有 rollback plan
[ ] 每个 candidate 有 source evidence
[ ] 不碰 high-risk patch
[ ] 不迁 save/load, map, multiplayer, death, run lifecycle
```

如果 runtime smoke 未通过：

```text
[ ] Batch 4c 继续 blocked
[ ] 修 runtime blocker
[ ] 不迁 patch
```

---

# 14. 子代理分工

必须使用 subagents，否则 Codex 会只做一小块就总结“完成”。

## Subagent A — Runtime/Test Truth Agent

负责：

```text
git status
git log
clean/build/test/test --no-build
format
diff
runtime smoke
godot.log
audit-godot-log
current-validation.md
```

## Subagent B — Docs Truth Agent

负责：

```text
清除旧 HEAD
清除旧测试数：324/361/387/428/444/452/461
统一 warning count
统一 runtime status
防止 release-ready 假 claim
```

## Subagent C — Sts1Events Governance Agent

负责：

```text
Off
CanaryOnly
AdditiveBatch1
AdditiveAllDraft
ReplaceUnknownEventsPrototype
canary runtime matrix
blocked event backlog
ZHS localization backlog
```

## Subagent D — Architecture Canary Agent

负责：

```text
RewardPipeline
CardPlayContext
FeatureRegistry log
ReleaseEvidenceLog
no-behavior-change diagnostics
```

## Subagent E — State/Death/Multiplayer Agent

负责：

```text
UrdaStateCodec
DeathProtectionService
MultiplayerPolicyRegistry
co-op evidence metadata
```

## Subagent F — Patch Gate Agent

负责：

```text
阻止 Batch 4c 直到 runtime smoke pass
仅在 runtime pass 后提出 low-risk candidates
禁止 high-risk migration
```

---

# 15. Overnight Run Spec：必须跑完才能停

下面这段可以直接发给 Codex。

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：Runtime Proof + StS1 Canary Runtime + Architecture Diagnostics Overnight Run。

这不是 Batch 4c。
不要迁更多 patches，除非 runtime smoke 已通过且 owner 明确接受 Batch 4c candidate。
不要新增 gameplay。
不要 claim release-ready。
必须使用 subagents。
必须跑完所有 phase；如果 blocker 存在，必须记录 issue、证据、下一步，不能直接停止。

当前状态：
- Latest main must be checked at start. Previous validation may be stale.
- RitsuLib dependency and manifest dependency exist.
- 25 patches migrated to RitsuLib ModPatcher.
- Raw Harmony count must be reconciled with latest source.
- Runtime smoke is blocked until STS2-RitsuLib is installed.
- Sts1Events Off and CanaryOnly are safe by source/tests.
- AdditiveBatch1 is controlled prototype.
- AdditiveAllDraft and ReplaceUnknownEventsPrototype are unsafe/dev-only and require `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; replacement also requires `REPLACEMENT_PROTOTYPE_ENABLED`.
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
   - Remove stale counts and stale HEADs.
   - Unify latest HEAD, test count, warning count, runtime status.
   - Ensure no doc says runtime-ready or release-ready.

3. Sts1Events Governance Agent
   - Verify Off, CanaryOnly, AdditiveBatch1, AdditiveAllDraft, ReplaceUnknown modes.
   - Ensure safe modes contain no TODO/BLOCKED events.
   - Prepare canary runtime matrix.

4. Architecture Canary Agent
   - Verify RewardPipeline diagnostics on low-risk surfaces.
   - Verify CardPlayContext canary on Lotha extra-play adapter or another low-risk path.
   - Verify FeatureRegistry runtime summary logs.
   - Keep gameplay behavior unchanged.

5. State/Death/Multiplayer Agent
   - Expand UrdaStateCodec behavior tests if latest changes require it.
   - Verify DeathProtectionService provider tests.
   - Verify MultiplayerPolicy records for active systems and co-op evidence metadata.

6. Patch Gate Agent
   - Block Batch 4c until runtime smoke passes.
   - If runtime smoke passes, propose only 5–10 low-risk candidates.
   - Do not migrate high-risk patches.

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

Phase 4 — Architecture diagnostics

- Verify RewardPipeline diagnostics fire on low-risk reward/card-reward surfaces.
- Verify CardPlayContext canary logs on low-risk path.
- Verify DeathProtectionService no-op provider tests.
- Verify MultiplayerPolicy records include active features and co-op evidence metadata.
- No gameplay behavior changes.

Phase 5 — Batch 4c decision

If runtime smoke passed:
- propose 5–10 low-risk Batch 4c candidates.
- do not migrate them unless explicitly accepted.

If runtime smoke blocked/failed:
- Batch 4c remains blocked.

Phase 6 — Docs update

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

# 16. 最终结论

当前工作有实质进展，但还没有完成最终目标。

它已经完成了：

```text
RitsuLib dependency
Batch 4a/4b source closure
Sts1Events safe gating
FeatureRegistry hardening
UrdaStateCodec
RewardPipeline/CardPlayContext canary
DeathProtection/MultiplayerPolicy stub
```

它仍未完成：

```text
RitsuLib runtime proof
Sts1Events runtime proof
release proof
full RitsuLib lifecycle/DataStore/settings/content pack migration
high-risk patch migration readiness
```

因此最终决策是：

```text
优化为主，有限推进为辅。
```

下一步只做：

```text
Runtime smoke + canonical validation + canary diagnostics
```

**不要继续 Batch 4c，直到 runtime smoke 通过。**


先说明：你之前上传的一些旧文件/日志已经过期，我现在不能读取那些旧附件。如果后续要我复核旧 log、旧截图、旧 Codex 报告，请重新上传。下面这份审核基于**当前 GitHub `main` 仓库状态**、我刚刚读取到的仓库文件、以及你这轮提供的总结。

# 0. 严格结论

当前助理工作 **没有完全完成最终目标**，但已经完成了一个重要的中间阶段：**RitsuLib migration 的架构稳定化和 canary 基础已经明显推进**。

当前真实状态可以定义为：

```text
RitsuLib 接入：完成
Batch 4a/4b patch migration：source-level 基本闭环
Double-patch guard：source-level 完成
FeatureRegistry hardening：第一层完成
UrdaStateCodec：第一层完成
RewardPipeline/CardPlayContext：canary/skeleton 完成
DeathProtectionService/MultiplayerPolicy：diagnostics-only stub 完成
Sts1Events：默认 Off 安全，Canary/Batch1 可控，完整内容未完成
Runtime smoke：未完成，是最大 blocker
Release-ready：否
```

所以战略判断是：

```text
继续优化为主，有限推进为辅。
```

现在不应该直接继续 Batch 4c，也不应该新增玩法或高风险 patch migration。下一步必须先做：

```text
1. Runtime smoke
2. Canonical validation truth
3. Sts1Events runtime governance
4. Architecture canary 接入真实低风险 surface
5. Death/Multiplayer/State 的进一步可测试化
```

---

# 1. 当前 GitHub 状态

当前最新提交记录显示最新 `main` 为：

```text
87820303 — sprint 1
```

这是当前最新可见提交。

不过仓库文档中仍可能有历史“测试数字不一致”的残留；当前 `docs/reviews/current-validation.md` 写的是：

```text
HEAD: 87820303
dotnet clean + dotnet build: PASS, 0 errors, 89 warnings
dotnet test --no-build: PASS, 464 passed, 0 failed, 21 skipped, 485 total
format: PASS
git diff --check: PASS
runtime smoke: BLOCKED
```



`monthly-dev-spec.md` 也已经同步到较新的状态，写明当前验证为 `0 errors / 89 warnings`、`464 passed / 0 failed / 21 skipped / 485 total`，并且 runtime smoke 仍 blocked。

因此当前最可信状态是：

```text
HEAD: 87820303
Build: 0 errors, 89 warnings
Tests: 464 passed, 0 failed, 21 skipped, 485 total
Runtime smoke: BLOCKED
```

---

# 2. 项目上下文从头梳理

这个项目当前的玩家可见名称是 **Spire Plus**；`EZMicroBalance` 只保留为技术 manifest id、工程/资源目录、兼容安装目录和存档命名空间。当前主要模块包括：

```text
1. Ancient reward rebalance v4
2. Ascension 11–20
3. Rootblight / Blight Sprout
4. Urda / Morvi / Lotha / Vakuu Ancient expansion
5. StS1 event port prototype
6. RitsuLib migration
7. Preview tools / art / website / package evidence
8. Multiplayer diagnostics / co-op fail-closed gates
```

过去 bug 多的根因并不只是单点功能写错，而是架构层面：

```text
- 功能面过大；
- static service 和 Harmony patch 太多；
- reward / combat / death / save / multiplayer 缺统一 pipeline；
- 保存状态依赖 string / SavedSpireField / WeakTable；
- source implementation 进度快，但 runtime evidence 不足；
- 文档中 source-ready、runtime-ready、release-ready 曾经混杂。
```

因此当前主线已经变成：

```text
RitsuLib-first migration + architecture hardening + runtime proof
```

也就是先把架构和验证体系稳住，再继续扩功能或迁更多 patch。

---

# 3. 当前任务逐项严格审查

## 3.1 RitsuLib dependency

**状态：完成。**

项目已经加入 RitsuLib compile dependency，并且 manifest 已经加入 `STS2-RitsuLib` runtime dependency；BaseLib 仍作为过渡期内容模型依赖保留。当前 `monthly-dev-spec.md` 明确记录了 RitsuLib migration 当前状态和 hybrid bootstrap active。

判定：

```text
PASS
```

但这只是“依赖已接入”，不代表运行时已验证。

---

## 3.2 RitsuLibBootstrap / hybrid patching

**状态：阶段完成。**

当前架构是：

```text
已迁移 patch -> RitsuLib ModPatcher
未迁移 patch -> raw Harmony.PatchAll()
```

`monthly-dev-spec.md` 明确写了 hybrid bootstrap active：`ModPatcher.PatchAll()` 负责 migrated patches，`Harmony.PatchAll()` 负责 remaining patches。

判定：

```text
PASS for hybrid bootstrap
NOT complete for full migration
```

这是一种正确的过渡方案，但不能说“RitsuLib 完全接管”。

---

## 3.3 Batch 4a / 4b patch migration

**状态：source-level 基本完成。**

当前 patch inventory 写：

```text
Total raw HarmonyPatch declarations: 142
Migrated to RitsuLib ModPatcher: 25
Raw HarmonyPatch remaining: 142
High risk raw: 22
Medium risk raw: 35
Low risk raw: 85
Unclassified owner: 0
```



`monthly-dev-spec.md` 也写：

```text
25 patches migrated
142 raw Harmony remaining
runtime migration beyond Batch 4b remains blocked
```



判定：

```text
PASS for Batch 4a/4b source-level closure
```

但 inventory 表述仍容易误读。建议下轮改成更清楚：

```text
Migrated IPatchMethod classes: 25
Compile-active raw HarmonyPatch declarations: 142
Tracked patch units total: 167
High-risk raw: 22
Medium-risk raw: 35
Low-risk raw: 85
```

否则“Total raw HarmonyPatch declarations = 142”和“Raw Harmony remaining = 142”才不会让人误以为 migrated 25 不算 patch。

---

## 3.4 Double-patch guard

**状态：source-level 完成。**

当前 guard 体系已经覆盖 migrated patches 与 raw Harmony patches 的 source-level 分离。`monthly-dev-spec.md` 记录了 double-patch guard tests，patch inventory 也说明 migrated classes 实现 `IPatchMethod` 并通过 `RitsuLibBootstrap.RegisterMigratedPatches()` 注册，不会被 raw `Harmony.PatchAll()` 拾取。

判定：

```text
PASS for source-level double-patch protection
PENDING for runtime proof
```

还不能证明 runtime 中 ModPatcher 行为完全等价；这需要 smoke log。

---

## 3.5 自动化验证真相

**状态：部分完成，仍需统一。**

`current-validation.md` 当前记录：

```text
dotnet clean + build: PASS, 0 errors, 89 warnings
dotnet test --no-build: PASS, 464 passed, 0 failed, 21 skipped, 485 total
format: PASS
diff check: PASS
publish: NOT RUN
```



判定：

```text
PARTIAL PASS
```

原因：

1. 当前记录了完整 `dotnet test EZMicroBalance.sln` 和 `dotnet test --no-build`。
2. 历史 commit message / goal ledger 中仍可能出现 `444/465` 或 `452/473`，只能当历史记录。
3. build 仍有 89 warnings。
4. `publish` 没跑，这是可以接受的，因为这轮没有 package refresh，但文档必须继续写清楚。

下一轮需要：

```text
dotnet clean
dotnet build
dotnet test
dotnet test --no-build
dotnet format
git diff --check
```

然后写入唯一 `docs/reviews/current-validation.md`。

---

## 3.6 Build warnings

**状态：开放问题。**

当前 clean build 有：

```text
89 warnings
codes: CS8602, CS8604, CS8625
scope: EZMicroBalanceCode/Sts1Events/Models/
decision: issue-worthy, accepted only because Sts1Events is gated Off by default and prototype/dev-only outside Canary/Batch1 test modes
```



判定：

```text
OPEN
```

这些 warnings 不能永久接受。当前暂时可接受的唯一理由是：

```text
Sts1Events 默认 Off，不进入普通 live path。
```

如果未来要启用 CanaryOnly / AdditiveBatch1 / AllDraft，就必须清理 Sts1Events null-safety。

---

## 3.7 Runtime smoke

**状态：未完成，是最大 blocker。**

`current-validation.md` 写：

```text
Runtime Smoke: BLOCKED
D:\Steam...\mods\STS2-RitsuLib 不存在
D:\Steam...\mods\BaseLib 不存在
D:\Steam...\mods\EZMicroBalance 不存在
E:\Steam...\mods\STS2-RitsuLib 不存在
Batch 4c remains blocked
No runtime safety or release-readiness claim
```



`next-overnight-run.md` 也明确写：

```text
Runtime smoke remains the critical path blocker.
Batch 4c cannot proceed until STS2-RitsuLib is installed and runtime smoke passes.
```



判定：

```text
FAIL / BLOCKED
```

没有 runtime smoke，不能说：

```text
RitsuLib runtime safe
ModPatcher runtime equivalent
release candidate
Batch 4c allowed
```

---

## 3.8 Sts1Events governance

**状态：默认安全完成；完整内容未完成。**

Sts1Events issue 当前写：

```text
Open — governance hardened, content incomplete.
Default Off is safe.
CanaryOnly and AdditiveBatch1 are controlled source-test modes.
AdditiveAllDraft and ReplaceUnknownEventsPrototype are dev-only/unsafe and require the explicit unsafe override `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; replacement also requires `REPLACEMENT_PROTOTYPE_ENABLED`.
```



模式矩阵当前是：

```text
Off: 0 registrations, safe
CanaryOnly: 4 registrations / 4 event types, controlled
AdditiveBatch1: 11 registrations / 10 event types, controlled prototype testing only
AdditiveAllDraft: 54 registration calls / 47 unique event types only with explicit unsafe override, unsafe/dev-only
ReplaceUnknownEventsPrototype: fail-closed in normal builds; debug-only with compile symbol and unsafe override, unsafe
```



风险表列出：

```text
Dead Adventurer — combat path no-op
Scorpion Nest — combat path no-op
Treasure Ooze — combat path no-op
Masked Bandits — fight path no-op
Mind Bloom — WAR option no-op
Mysterious Sphere — combat path no-op
N'loth — OFFER option no-op / relic select API missing
Vampires — removes Strikes but no Bite cards
```



判定：

```text
PASS for governance
NOT complete for content
```

Sts1Events 不能被描述为“完成”；只能说“默认安全、canary/batch1 可控、完整内容未完成”。

---

## 3.9 FeatureRegistry

**状态：第一层 hardening 完成。**

`monthly-dev-spec.md` 写 FeatureRegistry 已经有：

```text
IFeatureModule metadata
FeatureBootstrapRecord
LiveStatus enum
unified truthy env key overrides before bootstrap record creation
metadata/override guard tests
```



判定：

```text
PASS for scaffold hardening
PARTIAL for full runtime governance
```

还缺：

```text
Dependencies
runtime evidence status
feature dependency graph
status export
multiplayer policy integration
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

当前仍是 SavedSpireField string bridge + codec，不是完整 RitsuLib persistence migration。这个阶段可以接受。

---

## 3.11 RewardPipeline / CardPlayContext

**状态：skeleton/canary 完成，实际玩法接入有限。**

Monthly spec 记录：

```text
RewardPipeline diagnostics are wired into FeatureRegistry bootstrap events
CardPlayContext canary is touched by Lotha extra-play through an allow-only adapter
No gameplay behavior changes are intended
```



判定：

```text
PASS for canary/skeleton
NOT complete for actual reward/card-play governance
```

下一步应接入更真实但低风险的 surface，不要大改 gameplay。

---

## 3.12 DeathProtectionService

**状态：diagnostics-only stub。**

Monthly spec 写：

```text
DeathProtectionService stub: diagnostics-only code stub with Request/Result/Priority, 21 tests
```



判定：

```text
PASS for diagnostics/provider-testable stub
NOT actual Lotha DeathReprieve fix
```

不能说 death recursion 已修。

---

## 3.13 MultiplayerPolicy

**状态：diagnostics-only taxonomy/registry。**

Monthly spec 写：

```text
MultiplayerPolicy stub: diagnostics-only registry with 6-category taxonomy, 14 tests
```



判定：

```text
PASS for taxonomy/stub
NOT actual multiplayer enforcement
```

不能说 co-op desync 已解决。

---

# 4. 当前目标完成度表

| 目标                            | 当前状态                             | 严格判定    |
| ----------------------------- | -------------------------------- | ------- |
| RitsuLib dependency           | 已完成                              | PASS    |
| RitsuLib runtime dependency   | 已完成                              | PASS    |
| RitsuLibBootstrap hybrid mode | 已完成第一层                           | PASS    |
| Batch 4a/4b migration         | 25 patches source-level 完成       | PASS    |
| Double-patch guard            | source-level 完成                  | PASS    |
| Runtime smoke                 | 未完成                              | BLOCKER |
| Full validation truth         | no-build 测试完成，完整 dotnet test 缺记录 | PARTIAL |
| Build warnings                | 89 warnings                      | OPEN    |
| Sts1Events governance         | 默认安全，可控模式清晰                      | PASS    |
| Sts1Events full content       | 未完成                              | OPEN    |
| FeatureRegistry               | metadata/live status 第一层完成       | PARTIAL |
| UrdaStateCodec                | 第一层完成                            | PARTIAL |
| RewardPipeline                | skeleton/canary                  | PARTIAL |
| CardPlayContext               | skeleton/canary                  | PARTIAL |
| DeathProtectionService        | diagnostics-only                 | PARTIAL |
| MultiplayerPolicy             | diagnostics-only                 | PARTIAL |
| Release-ready                 | 否                                | CORRECT |

---

# 5. 继续优化、推进，还是两者兼顾？

**结论：继续优化为主，有限推进为辅。**

建议比例：

```text
80% optimization / hardening / verification
20% limited advancement / canary integration
```

当前不能推进：

```text
Batch 4c 大规模 patch migration
high-risk patch migration
Sts1Events AllDraft live
release packaging
new gameplay
```

当前可以推进：

```text
Runtime smoke
Canonical validation truth
Sts1Events CanaryOnly runtime proof
FeatureRegistry runtime log
RewardPipeline/CardPlayContext canary integration
DeathProtection/MultiplayerPolicy provider/policy record tests
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
4. 将 RewardPipeline/CardPlayContext 从 skeleton 推进到低风险真实 surface canary。
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
AdditiveAllDraft = unsafe/dev-only and requires `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`
ReplaceUnknownEventsPrototype = unsafe/debug-only and requires `REPLACEMENT_PROTOTYPE_ENABLED` plus `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`
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

必须显式要求 Codex 使用 subagents。

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

下面这段可以直接发给 Codex。

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
- AdditiveAllDraft and ReplaceUnknownEventsPrototype are unsafe/dev-only and require `SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1`; replacement also requires `REPLACEMENT_PROTOTYPE_ENABLED`.
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

当前工作不是失败，实际上已经进入更健康的工程阶段：

```text
- patch migration 有 guard；
- Sts1Events 有 default Off；
- FeatureRegistry 有 metadata；
- state/reward/card/death/multiplayer 都有 skeleton；
- no-build test green；
- 文档治理改善。
```

但最终目标仍未达成：

```text
- RitsuLib runtime proof 没有；
- architecture skeleton 多数未接入真实 gameplay；
- Sts1Events 仍是 prototype；
- Batch 4c 仍应阻塞；
- release-ready 否。
```

因此当前最合理策略是：

```text
继续优化为主，有限推进为辅。
```

下一步唯一合理主线：

```text
Runtime smoke + canonical validation + architecture canary integration
```

只有 runtime smoke 通过后，才考虑 Batch 4c。
