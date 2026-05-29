# 严格审核结论

这轮工作 **比上一轮明显推进**，但不能判定为“月度架构稳定化目标完成”。更准确的状态是：

```text
已完成：
- Batch 4a/4b 计数修正；
- RitsuLib patch migration guard 增强；
- Sts1Events 默认关闭 gate；
- CanaryOnly 4 事件安全矩阵；
- source/API 文档和 canary specs；
- no-build test 绿灯证据；
- monthly-dev-spec / next-overnight-run 文档已存在。

未完成：
- RitsuLib runtime smoke；
- Mod Settings UI 实测；
- live gameplay proof；
- dotnet build warning 状态有冲突；
- dotnet test 非 --no-build 是否全绿未在 review 文档中完整证明；
- FeatureRegistry 还没真正变成完整 runtime status registry；
- UrdaStateCodec 仍主要是 source-level guard，不等于完整迁移；
- RewardPipeline / CardPlayContext 仍是 skeleton；
- DeathProtectionService 仍是 spec；
- MultiplayerPolicy 仍是 taxonomy 文档；
- release-ready 仍然不是。
```

当前最新 GitHub 提交是：

```text
faf5860 — overnight run Packs 0-5: fix Sts1MindBloom build error, Phase 2 adapter checklist, StS1EventFeatureGate, source/API docs, canary specs, final validation review
```

这个提交已经在远程 main 上，说明这轮不是本地未提交。

---

# 1. 当前状态与目标对比

## 1.1 Batch 4a/4b closure

**基本完成。**

之前最大问题是迁移计数错误：Batch 4a 文档写 10，实际 9；总数文档写 26，实际 25。现在 `docs/migration.md` 已经修正：

```text
Batch 4a = 9
Batch 4b = 16
Total migrated = 25
Remaining raw HarmonyPatch = 141
```

并且 `DebtAndCardPatches.cs` 已从 7 修成 8。

`docs/patch-inventory.md` 也已经新增 migrated section，明确：

```text
Migrated to RitsuLib ModPatcher: 25
Raw HarmonyPatch remaining: 141
High risk raw: 22
Medium risk raw: 35
Low risk raw: 84
```

并列出所有 25 个 migrated patch。

**判定：PASS。**

---

## 1.2 Double-patch guard

**source-level 完成。**

`RitsuLibMigrationGuardTests.cs` 已经有 8 个 guard，覆盖：

```text
PatchId unique
migrated patch count = 25
migrated classes have no HarmonyPatch attribute
raw HarmonyPatch classes are not registered
RegisterMigratedPatches count matches source
migration.md count matches source
patch-inventory.md lists migrated patches
all expected PatchId strings appear in source
```

这些测试已经能防止最明显的 double-patch 风险。

**判定：source-level PASS。**

但注意：这仍然不是 runtime proof。它证明“源码结构上不会明显 double patch”，不证明 RitsuLib ModPatcher 在游戏 runtime 中行为完全等价。

---

## 1.3 Sts1Events 安全治理

**完成了一层 gate，但还不是完整内容完成。**

当前 review 文档记录：

```text
Sts1EventFeatureGate
SPIREPLUS_STS1_EVENT_MODE
Off = 0
CanaryOnly = 1
AdditiveAllDraft = 2
ReplaceUnknownEventsPrototype = 3
默认 Off = 0 registrations
CanaryOnly = exactly 4 events
13 guard tests
```



`Sts1EventFeatureGuardTests.cs` 也验证：

```text
默认 env unset 时 Off；
CanaryOnly 正好 4 个事件；
RegisterCanaryOnly 只注册 Big Fish / Golden Idol / The Lab / Divine Fountain；
FeatureRegistry 注册 Sts1EventsFeatureModule；
Sts1Duplicator 被 compile-excluded；
Sts1EventRegistrationService 编译进来。
```



**判定：默认 Off / CanaryOnly 安全层 PASS。**

但它还不是“Sts1Events 完成”。review 文档自己也写：

```text
StS1Events not marked complete
Canary events not playable
Prototype only
```



并且 `ISSUE-2026-05-28-STS1EVENTS-INCOMPLETE-SKELETON-LIVE-RISK.md` 仍写当前是 gated Off、CanaryOnly 有 guard，但这只是说明默认安全，不是说明所有 draft events 可用。

**结论：Sts1Events 默认安全，内容未完成。**

---

## 1.4 Full test truth

**部分完成，有冲突必须澄清。**

review 文档记录：

```text
dotnet build EZMicroBalance.sln -> 0 errors, 87 warnings (all Sts1Events nullable)
dotnet test EZMicroBalance.sln --no-build -> 361 passed, 21 skipped, 0 failed (382 total)
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore -> clean
git diff --check -> clean (after trailing whitespace fix in event.md)
report-worktree-batches -> pass (3 dirty, 0 unclassified)
```



但是你这次贴的 summary 说：

```text
Build: 0 errors, 87 warnings (pre-existing Sts1Events null-safety)
Tests: 361 passed, 0 failed, 21 skipped
```

这和 repo 中 review 文档的 `0 warnings` / `324 passed` 不一致。

**严格判定：FULL TEST TRUTH 仍需复核。**

下一步必须让 Codex 重新跑并记录同一轮完整命令：

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

并统一写入同一份 review 文件。现在不能同时接受“0 warnings”和“87 warnings”。

---

## 1.5 Runtime smoke

**未完成。**

`runtime-smoke-checklist.md` 当前明确写：

```text
Status: PENDING — no local game environment available for automated runtime smoke.
```

所有 runtime evidence 仍是 `[PENDING]`。

这说明：

```text
RitsuLib compile dependency 已经有；
RitsuLib runtime dependency 已经有；
但 BaseLib + STS2-RitsuLib + Spire Plus 的真实游戏加载仍未证明。
```

**判定：FAIL / PENDING。**

这仍然是最大 blocker。没有 runtime smoke，就不能说 RitsuLib migration runtime-safe。

---

## 1.6 Monthly dev spec

**已创建，但方向需要修订。**

`monthly-dev-spec.md` 已存在，记录当前状态：

```text
25 patches migrated
141 raw Harmony remaining
8 migration guard tests + 1 Sts1Events guard test
Sts1Events default Off
runtime smoke pending
```

它也把 Week 1 标为 DONE，Week 2 runtime smoke 标为 In Progress。

但这份计划的问题是：Week 3 仍然写了继续迁 Batch 4c，Week 4 写 high-risk migration prep。

在 runtime smoke pending、test count 有冲突、架构 skeleton 还很薄的情况下，**继续 Batch 4c 不是最优先**。

---

## 1.7 next-overnight-run

当前 `next-overnight-run.md` 的 objective 是：

```text
Continue RitsuLib migration by executing Batch 4c
```

并要求迁 10–15 个新的 low-risk patches。

**我不建议执行这个版本。**

现在应该替换为：

```text
Runtime proof + test truth reconciliation + architecture foundation overnight run
```

而不是继续 patch 迁移。

---

# 2. 当前总体判断：继续优化、推进，还是两者兼顾？

结论：**两者兼顾，但优先优化/稳定化。**

具体优先级：

```text
第一优先级：优化与稳定
- runtime smoke；
- test truth reconciliation；
- Sts1Events governance；
- FeatureRegistry hardening；
- UrdaStateCodec；
- RewardPipeline / CardPlayContext / DeathProtection / MultiplayerPolicy skeleton。

第二优先级：有限推进
- 只在 runtime smoke 和 full tests 过后，再考虑 Batch 4c；
- 不碰 high-risk run/map/reward/save/multiplayer patches；
- 不新增玩法。
```

不建议现在继续“推进功能”。当前项目最大风险不是“迁得不够快”，而是：

```text
编译/测试证据和 runtime 证据之间还断着；
架构 skeleton 还没转化为行为安全；
Sts1Events 只是 gate 安全，内容不完整；
RitsuLib 只接管了部分 patch，没有接管生命周期/状态/设置/内容注册。
```

---

# 3. 修订后的 Monthly Dev Spec

## 月度主题

```text
RitsuLib Runtime Proof + Architecture Foundation Month
```

## 本月总目标

```text
1. 证明 RitsuLib runtime 真能加载。
2. 统一 build/test truth，解决 0 warnings vs 87 warnings、324 pass vs 361 pass 的冲突。
3. 固化 Sts1Events 安全边界。
4. 强化 FeatureRegistry，让它不只是 wrapper。
5. 把 UrdaStateCodec 从 source-level guard 推进到实际状态安全。
6. 将 RewardPipeline / CardPlayContext / DeathProtectionService / MultiplayerPolicy 从 skeleton/spec 推进到最小可测试架构。
7. 继续阻塞 high-risk patch migration。
```

---

## Week 1：Runtime Smoke + Test Truth Reconciliation

### 任务

```text
[ ] 重新跑完整命令：
    dotnet build EZMicroBalance.sln
    dotnet test EZMicroBalance.sln
    dotnet test EZMicroBalance.sln --no-build
    dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
    git diff --check

[ ] 统一测试事实：
    - build warnings 到底是 0 还是 87？
    - tests 到底是 324 passed 还是 361 passed？
    - skipped 到底是多少？
    - 是否有任何 warning 应该转 issue？

[ ] clean runtime smoke：
    BaseLib v3.1.4
    STS2-RitsuLib
    Spire Plus
    no other mods

[ ] runtime log 必须检查：
    - RitsuLib bootstrap starting
    - ModPatcher applied 25/25 patches
    - RitsuLib framework is active
    - SavedSpireFields count
    - no MissingMethodException
    - no TypeLoadException
    - no manifest dependency failure
```

### 验收

```text
[ ] docs/reviews/current-validation.md 写入唯一 test truth
[ ] runtime-smoke-checklist.md 至少 loader smoke 不再全 PENDING
[ ] 如果 runtime smoke 无法执行，明确 pending，并禁止 Batch 4c
```

---

## Week 2：Sts1Events Governance Hardening

### 任务

```text
[ ] 重写 Sts1Events issue，移除 stale wording。
[ ] 明确 4 个 mode 的风险等级：
    Off = safe default
    CanaryOnly = controlled test
    AdditiveAllDraft = unsafe/dev-only
    ReplaceUnknownEventsPrototype = unsafe/dev-only

[ ] guard：
    - Off registers 0
    - CanaryOnly registers exactly 4
    - AdditiveAllDraft cannot be used in release package unless dev env explicitly set
    - TODO/BLOCKED events cannot enter safe modes

[ ] 对 Sts1DeadAdventurer / Sts1Joust：
    - 若只在 unsafe draft mode，可文档标明；
    - 若进入 CanaryOnly，必须补完。
```

### 验收

```text
[ ] default package 不会加载未完成 Sts1Events
[ ] CanaryOnly 只含 source-verified canary events
[ ] Sts1Events docs 不再自称完成
```

---

## Week 3：FeatureRegistry Hardening

### 任务

扩展 feature model：

```text
DisplayName
Category
DisableEnvKeys
ForceEnvKeys
BootstrapStatus
LiveStatus
RuntimeStatus
Dependencies
```

日志示例：

```text
Feature Sts1Events:
  bootstrap=enabled
  live=disabled
  reason=SPIREPLUS_STS1_EVENT_MODE unset -> Off

Feature VakuuFight:
  bootstrap=enabled
  live=hidden
  reason=explicit enable env not set

Feature Morvi:
  bootstrap=enabled
  live=enabled
```

### 验收

```text
[ ] MainFile 保持短
[ ] FeatureRegistry 不只是 wrapper
[ ] bootstrap/live status 有 tests
[ ] 默认开启行为不变
```

---

## Week 4：State / Pipeline / Death / Multiplayer Foundations

### 任务

```text
[ ] UrdaStateCodec V1
    - Encode
    - Decode
    - malformed fallback
    - old-state migration
    - roundtrip tests

[ ] RewardPipeline minimum testable skeleton
    - RewardPhase
    - IRewardHandler
    - priority
    - diagnostics

[ ] CardPlayContext minimum testable skeleton
    - ExtraPlayPolicy
    - depth guard
    - Power fallback flag
    - no-recursion test

[ ] DeathProtectionService spec + code stub
    - Lotha DeathReprieve lifecycle
    - inReprieve flag
    - forced unavoidable death
    - co-op owner attribution

[ ] MultiplayerPolicy taxonomy + feature matrix
    - LocalUiOnly
    - LocalPlayerOnly
    - HostAuthoritative
    - SharedRunState
    - CombatCommandReplicated
    - UnsafeInMultiplayer
```

### 验收

```text
[ ] UrdaStateCodec has behavior tests
[ ] RewardPipeline and CardPlayContext are more than docs-only if possible
[ ] DeathProtectionService at least has enforceable guard/spec
[ ] MultiplayerPolicy covers active systems
```

---

# 4. 新 Overnight Run Spec：必须跑到完成才停

这版应该替换当前 `next-overnight-run.md`。当前仓库里的版本太偏向继续 Batch 4c，不符合当前风险优先级。

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：RitsuLib Runtime Proof + Test Truth Reconciliation + Architecture Foundation Overnight Run。

这不是 Batch 4c。
不要迁更多 patch，除非是修复当前 guard/bug 必需。
不要新增 gameplay。
不要 claim release-ready。

当前状态：
- latest main 已有 RitsuLib dependency and manifest dependency。
- 25 patches migrated to RitsuLib ModPatcher。
- 141 raw HarmonyPatch declarations remain。
- Batch 4a/4b count fixed。
- RitsuLibMigrationGuardTests exist。
- Sts1Events default Off / CanaryOnly 4 events。
- Runtime smoke still PENDING。
- Review docs contain conflicting test/build facts:
  - review doc says build 0 warnings, 324 passed / 21 skipped;
  - latest user summary says build 87 warnings, 361 passed / 21 skipped.
- FeatureRegistry still scaffold-level。
- UrdaStateCodec / RewardPipeline / CardPlayContext / DeathProtection / MultiplayerPolicy not complete.

Hard rules:
- Do not start Batch 4c.
- Do not migrate high-risk patch.
- Do not add new gameplay.
- Do not change default-on feature policy.
- Do not ignore warnings.
- Do not only run --no-build tests.
- Do not claim release-ready.
- Use subagents.

Subagents:

1. Runtime/Test Truth Agent
   Scope:
   - Run full validation commands.
   - Reconcile build warning count and test count.
   - Execute runtime smoke if local game environment exists.
   - If runtime smoke cannot run, mark pending and block Batch 4c.

2. Sts1Events Governance Agent
   Scope:
   - Rewrite stale Sts1Events issue.
   - Verify Off / CanaryOnly / AdditiveAllDraft / ReplaceUnknownEventsPrototype safety.
   - Ensure TODO/BLOCKED events cannot enter safe modes.
   - Add/update guard tests.

3. FeatureRegistry Agent
   Scope:
   - Add DisplayName, Category, DisableEnvKeys, ForceEnvKeys.
   - Add BootstrapStatus and LiveStatus distinction.
   - Add feature status logs and tests.

4. State/Persistence Agent
   Scope:
   - Implement or complete UrdaStateCodec V1.
   - Add roundtrip/malformed/old-state tests.
   - Document RitsuLib DataStore future migration.

5. Architecture Pipeline Agent
   Scope:
   - RewardPipeline skeleton.
   - CardPlayContext skeleton.
   - DeathProtectionService spec/stub.
   - MultiplayerPolicy taxonomy/matrix.

Phase 1 — Full validation truth

Run:
- git status --short --branch
- git log -1 --oneline --decorate
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check

Write one canonical validation section:
- build warnings exact count
- test passed/failed/skipped exact count
- whether warnings are accepted or issue-worthy

If tests fail:
- fix if in scope
- otherwise create quarantine issue
- do not continue as green

Phase 2 — Runtime smoke

If local game environment exists:
- install only BaseLib + STS2-RitsuLib + Spire Plus
- launch game
- collect godot.log
- run audit-godot-log
- update runtime-smoke-checklist.md

Required log evidence:
- RitsuLib bootstrap starting
- ModPatcher applied 25 patches
- RitsuLib framework is active
- BaseLib initialized
- Spire Plus initialized
- no MissingMethodException
- no TypeLoadException
- no manifest dependency failure

If local game environment unavailable:
- mark runtime smoke pending
- update monthly spec
- block Batch 4c

Phase 3 — Sts1Events governance

- Rewrite issue status so it is not stale.
- Document:
  Off = safe default
  CanaryOnly = controlled test
  AdditiveAllDraft = unsafe/dev-only unless all events pass blockers
  ReplaceUnknownEventsPrototype = unsafe/dev-only
- Guard:
  safe modes do not register TODO/BLOCKED events
  default Off registers 0
  CanaryOnly registers exactly 4

Phase 4 — FeatureRegistry hardening

- Extend IFeatureModule metadata.
- Add BootstrapStatus / LiveStatus distinction.
- Keep existing default-on behavior unchanged.
- Add tests for status logs and env keys.

Phase 5 — UrdaStateCodec

- Implement/complete UrdaStateV1 and UrdaStateCodec.
- Maintain compatibility with existing SavedSpireField bridge.
- Tests:
  empty
  malformed
  old state
  current state
  roundtrip

Phase 6 — Architecture skeletons

- RewardPipeline skeleton with phases and diagnostics.
- CardPlayContext skeleton with ExtraPlayPolicy/depth guard.
- DeathProtectionService spec/stub.
- MultiplayerPolicy doc + active feature matrix.

Phase 7 — Docs update

Update:
- docs/features/ritsulib-migration/monthly-dev-spec.md
- docs/features/ritsulib-migration/next-overnight-run.md
- docs/reviews/current-validation.md or equivalent
- docs/issues for Sts1Events and runtime smoke blockers

Next overnight run rule:
- If runtime smoke passed and architecture phase complete, Batch 4c may be considered.
- If runtime smoke pending/failed, Batch 4c remains blocked.
- If test truth inconsistent, Batch 4c remains blocked.

Final report must include:
1. current HEAD
2. full validation results
3. runtime smoke status
4. Sts1Events safe-mode status
5. FeatureRegistry status
6. UrdaStateCodec status
7. RewardPipeline/CardPlayContext/DeathProtection/MultiplayerPolicy status
8. subagent findings
9. files changed
10. remaining blockers
11. explicit decision: optimize, advance, or both
12. release-ready: no

Stop condition:
Do not stop until every phase is either:
- complete and verified, or
- blocked with explicit issue, evidence, and next action.
```

---

# 5. Subagent 使用要求

必须让 Codex 显式分派子代理。建议如下：

## Subagent A — Runtime/Test Truth Agent

负责：

```text
full build/test
warning count reconciliation
runtime smoke
log audit
canonical validation report
```

## Subagent B — Sts1Events Governance Agent

负责：

```text
Sts1Events 4 mode 安全性
TODO/BLOCKED event guard
issue rewrite
canary-only scope
```

## Subagent C — FeatureRegistry Agent

负责：

```text
IFeatureModule metadata
BootstrapStatus / LiveStatus
env key docs
status logs
tests
```

## Subagent D — State/Persistence Agent

负责：

```text
UrdaStateCodec
malformed fallback
old-state migration
SavedSpireField bridge
RitsuLib DataStore future plan
```

## Subagent E — Architecture Pipeline Agent

负责：

```text
RewardPipeline
CardPlayContext
DeathProtectionService
MultiplayerPolicy
next monthly plan
```

---

# 6. 最终战略判断

现在应该 **优化为主，有限推进为辅**。

```text
继续优化：
- runtime smoke
- test truth
- Sts1Events governance
- FeatureRegistry hardening
- state/pipeline/death/multiplayer foundations

有限推进：
- 只有以上完成后，才考虑 Batch 4c
```

不要现在继续迁 patch。
不要现在扩玩法。
不要把 skeleton 当完成。

当前最正确的下一步是：

```text
Runtime Proof + Architecture Foundation Overnight Run
```

跑完之后，再决定是否进入 Batch 4c。
