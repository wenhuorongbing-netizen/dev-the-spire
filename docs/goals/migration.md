下面是基于**当前 GitHub `main` 最新状态**的严格审查。我不再沿用旧窗口或旧附件里的结论；部分早前上传文件已经过期，如果还要我复核旧日志、截图或 Codex 报告，需要重新上传。

# 1. 最新状态校准

当前远程最新提交已经到：

```text
3f01cb7 — sprint 4
```

它位于 `8f2d79b sprint3`、`6b149ba sprint 2`、`8782030 sprint 1` 之后。

这意味着：**任何仍写 `87820303`、`6b149ba`、`8f2d79b4` 的验证文档，都不能直接等价于最新 `3f01cb7` 的完整验证结果。** 不过，从 sprint4 diff 看，sprint4 主要是在同步 RitsuLib runtime evidence 和项目状态文档，确认 Off / CanaryOnly / AdditiveBatch1 的 loader-gate evidence 已经存在，并仍明确不 claim live-ready / release-ready。

当前最准确的总状态应该写成：

```text
RitsuLib loader-gate proof：已有 Off / CanaryOnly / AdditiveBatch1 证据
RitsuLib full runtime gameplay proof：未完成
Sts1Events source/governance：已强化
Sts1Events live gameplay：未完成
Batch 4a/4b：source-level 闭环
Batch 4c：可以提出低风险候选，但不应自动执行
Release-ready：否
```

---

# 2. 严格验收：是否完成？

## 2.1 RitsuLib runtime smoke

这轮有重大进展：**之前“STS2-RitsuLib 未安装”的 blocker 已经被部分解除。**

`current-validation.md` 记录，E 盘游戏目录、mods 目录、BaseLib、EZMicroBalance、STS2-RitsuLib 都已存在，其中 STS2-RitsuLib 是 `v0.3.10`，包含 `lib\0.106.1`。

更重要的是，当前文档记录了三组 runtime loader evidence：

```text
Off mode：PASS
CanaryOnly：PASS
AdditiveBatch1：PASS
```

其中：

* Off mode Steam smoke 到达主菜单，加载 exactly 3 mods：BaseLib v3.1.4、RitsuLib v0.3.10、Spire Plus v0.1.0-private-beta.84；25/25 ModPatcher patches 应用成功；Found 30 SavedSpireFields；Sts1Events disabled/default Off；clean audit。
* CanaryOnly direct launch 到达主菜单，加载 exactly 3 mods，25/25 patches，Found 30 SavedSpireFields，注册 exactly 4 canary events：`Sts1BigFish`、`Sts1GoldenIdol`、`Sts1TheLab`、`Sts1DivineFountain`，clean audit。
* AdditiveBatch1 direct launch 到达主菜单，加载 exactly 3 mods，25/25 patches，并注册 10 event types / 11 registration calls，clean audit。

**判定：loader-gate runtime proof 完成。**

但这仍不是完整 runtime gameplay proof。文档也明确写：虽然 Off=0、CanaryOnly=4、AdditiveBatch1=10/11 的 loader-gate proof 已经有了，但 event encounter screenshots、save/load、image rendering、replacement functional proof、multiplayer fail-closed、独立 QA、clean worktree / owner decision、versioned tester-package handoff 仍然 pending，live-ready / release-ready 仍不能 claim。

结论：

```text
RitsuLib loader proof：PASS
Gameplay proof：PENDING
Release proof：PENDING
```

---

## 2.2 Full validation truth

当前 `current-validation.md` 记录的主要 no-game validation 是：

```text
dotnet build EZMicroBalance.sln -m:1 --no-incremental：PASS，0 errors，89 Sts1Events nullable warnings
dotnet test ... --no-build：PASS，464 passed，0 failed，21 skipped，485 total
dotnet format：PASS
git diff --check：PASS
generate-patch-inventory：PASS
report-worktree-batches：PASS，17 dirty entries，0 unclassified
```



同时 Revision J 记录过一轮 full/no-build test：

```text
dotnet test EZMicroBalance.sln：462 passed / 0 failed / 21 skipped / 483 total
dotnet test EZMicroBalance.sln --no-build：462 passed / 0 failed / 21 skipped / 483 total
```

但这些又被后续 no-build 464/485 取代。

**严格判定：自动化验证基本绿，但最新 HEAD 仍需再跑一次。**

原因：

```text
1. 最新远程已经是 3f01cb7，验证文档主要落在 8f2d79b4 / 87820303 / 6b149ba 系列。
2. Worktree 在验证文档中仍标为 dirty。
3. 89 warnings 仍存在。
4. 有历史 testhost crash/stale process 记录，虽已处理，但需要继续保持单 worker / no stale testhost 规则。
```

所以不能说“最终完成”；只能说：

```text
当前 validation 证据趋势良好，但最新 HEAD 还需要 canonical validation refresh。
```

---

## 2.3 Build warnings

当前 build 仍有：

```text
89 warnings
CS8602 / CS8604 / CS8625
全部在 EZMicroBalanceCode/Sts1Events/Models/
```

`current-validation.md` 明确说这些 warnings 是 issue-worthy，只是因为 Sts1Events 默认 Off，且除 Canary/Batch1 测试模式外仍 prototype/dev-only，所以暂时接受。

**判定：OPEN。**

这不是 release blocker 的唯一原因，但它是下个月必须处理的技术债。尤其如果 CanaryOnly 要进入更正式的测试包，至少 CanaryOnly path 相关 warnings 应优先清理。

---

## 2.4 RitsuLib patch migration

当前 patch migration 状态：

```text
25 patches migrated to RitsuLib IPatchMethod
142 raw Harmony declarations remaining
tracked patch units total = 167
hybrid bootstrap active
```

`monthly-dev-spec.md` 已经这么记录。

Patch inventory 也记录：

```text
Migrated to RitsuLib ModPatcher: 25
Raw HarmonyPatch remaining: 142
High risk raw: 22
Medium risk raw: 35
Low risk raw: 85
```



**判定：Batch 4a/4b source-level closure 完成。**

现在 runtime loader gate 也通过了，因此 Batch 4c **可以进入候选评审阶段**。但不是自动执行。当前文档也说：

```text
Batch 4c is ready for low-risk candidate proposal.
```



结论：

```text
Batch 4c 可提出候选，但必须 owner acceptance 后再执行。
High-risk migration 仍然不允许。
```

---

## 2.5 Sts1Events governance

当前 issue 已经非常清楚：

```text
Open — governance hardened, content incomplete.
Default Off is safe.
CanaryOnly and AdditiveBatch1 are controlled source-test modes.
AdditiveAllDraft and ReplaceUnknownEventsPrototype are dev-only/unsafe.
```



模式矩阵：

```text
Off：0 registrations，Safe
CanaryOnly：4 registrations / 4 event types，Controlled
AdditiveBatch1：11 registrations / 10 event types，Controlled prototype testing only
AdditiveAllDraft：54 calls / 47 unique event types，Unsafe/dev-only
ReplaceUnknownEventsPrototype：debug-only / unsafe
```



风险表列出 7 个 HIGH-risk 和 1 个 MEDIUM-risk event，包括 Dead Adventurer、Scorpion Nest、Treasure Ooze、Masked Bandits、Mind Bloom、Mysterious Sphere、N'loth、Vampires。

**判定：source governance + loader-gate proof 完成；gameplay content 未完成。**

现在可以说：

```text
Off / CanaryOnly / AdditiveBatch1 loader-gate proven
```

不能说：

```text
Sts1Events gameplay complete
Canary events manually verified
Save/load proven
AllDraft safe
```

---

## 2.6 FeatureRegistry

FeatureRegistry 已经有：

```text
IFeatureModule metadata
FeatureBootstrapRecord
LiveStatus enum
truthy env key override before bootstrap record creation
runtime log plan
```

当前 runtime evidence 也显示：FeatureRegistry diagnostics observed for all 6 features。

**判定：FeatureRegistry hardening 阶段完成。**

但仍不是完整 feature governance，因为还缺：

```text
Dependencies
runtime evidence export
feature graph
formal multiplayer policy enforcement
```

---

## 2.7 RewardPipeline / CardPlayContext / DeathProtection / MultiplayerPolicy

当前 architecture status 写：

```text
RewardPipeline diagnostics wired into FeatureRegistry bootstrap events and low-risk AscensionRewardService reward/card-reward surfaces as no-mutation diagnostics
ArchitectureCanaryBootstrap registers FeatureRegistry + Ascension reward diagnostics, no-op DeathProtection provider, and multiplayer policy records
Lotha extra-play touches CardPlayContextCanary through single-depth adapter returning Allow
Existing co-op gates make same decisions; evidence payloads now include policy metadata
```



**判定：architecture diagnostics/canary 层完成；实际 enforcement 未完成。**

不能说：

```text
Reward pipeline fully governs rewards
CardPlay recursion fully solved
Lotha DeathReprieve fixed
Multiplayer desync solved
```

可以说：

```text
diagnostics hooks and policy metadata now exist.
```

---

# 3. 当前目标对比

| 目标                                | 当前状态                               | 结论                   |
| --------------------------------- | ---------------------------------- | -------------------- |
| RitsuLib dependency               | 已完成                                | PASS                 |
| RitsuLib loader proof             | Off/Canary/Batch1 有 clean evidence | PASS for loader gate |
| Full gameplay proof               | 未完成                                | PENDING              |
| Batch 4a/4b                       | source-level 完成                    | PASS                 |
| Batch 4c                          | 可提候选，不可自动执行                        | CONDITIONAL          |
| Sts1Events Off/Canary             | source + loader proof              | PASS                 |
| Sts1Events gameplay               | 未完成                                | PENDING              |
| FeatureRegistry hardening         | 完成第一层                              | PASS                 |
| UrdaStateCodec                    | 完成第一层                              | PASS/PARTIAL         |
| RewardPipeline/CardPlayContext    | diagnostics/canary                 | PARTIAL              |
| DeathProtection/MultiplayerPolicy | diagnostics/stub                   | PARTIAL              |
| Build warnings                    | 89 warnings                        | OPEN                 |
| Worktree / latest HEAD truth      | 仍需刷新                               | OPEN                 |
| Release-ready                     | 否                                  | CORRECT              |

---

# 4. 当前应继续优化、推进，还是两者兼顾？

现在的策略应从之前的 “优化为主” 稍微前进到：

```text
优化为主，有限推进可以开始。
```

比例建议：

```text
70% 优化 / 验证 / 稳定化
30% 有限推进 / low-risk Batch 4c 候选评审
```

原因：

* 之前最大的 blocker “STS2-RitsuLib missing” 已经被解决。
* Off / CanaryOnly / AdditiveBatch1 loader gate 已有 clean audit。
* 25/25 ModPatcher patches 已经在 runtime 中加载成功。
* 但 gameplay、save/load、co-op、Mod Settings、event screenshots、versioned package handoff 都仍没完成。

所以可以做：

```text
1. 继续 runtime/manual proof。
2. 提出 Batch 4c low-risk candidate list。
3. 不立即执行 Batch 4c，除非 owner 明确同意。
```

不能做：

```text
high-risk migration
release
AllDraft live
new gameplay
claim co-op safe
claim save/load safe
```

---

# 5. 下个月开发规范 Monthly Dev Spec

## 月度主题

```text
Loader-Proven Stabilization + Controlled Batch 4c Planning Month
```

## 月度目标

```text
1. 将最新 HEAD 的 canonical validation 刷新到 3f01cb7 或后续最新提交。
2. 完成 Off / CanaryOnly / AdditiveBatch1 loader evidence 整理和独立 QA。
3. 进行 CanaryOnly 4 个事件的手动 gameplay smoke。
4. 开始 Sts1Events nullable warning triage。
5. 提出 Batch 4c low-risk candidate list，但执行需 owner acceptance。
6. 保持 release-ready = no。
```

---

## Week 1：Latest HEAD Validation + Evidence Reconciliation

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
[ ] generate-patch-inventory.ps1 -Check
[ ] report-worktree-batches.ps1 -FailOnUnclassified
```

更新：

```text
docs/reviews/current-validation.md
```

必须写唯一真相：

```text
HEAD = 最新 main
worktree status
build warnings
test passed/failed/skipped
runtime smoke evidence paths
publish/package status
```

### 验收

```text
[ ] current-validation 不再引用旧 HEAD 作为当前状态
[ ] 所有 stale test count 清理
[ ] dirty worktree 有 owner decision：commit / defer / archive
```

---

## Week 2：Runtime Evidence QA + Mod Settings UI

### 任务

```text
[ ] 独立 QA 复核 Off / CanaryOnly / AdditiveBatch1 logs
[ ] Mod Settings UI screenshot
[ ] SavedSpireFields = 30 复核
[ ] 只启用 BaseLib + RitsuLib + Spire Plus
[ ] audit-godot-log clean proof
```

### 验收

```text
[ ] runtime-smoke-checklist.md 从 loader proof 升级为 QA-reviewed loader proof
[ ] Mod Settings UI 不再只靠历史截图
```

---

## Week 3：Sts1Events Canary Gameplay Smoke

### 任务

```text
[ ] Debug-spawn / force 4 canary events：
    BigFish
    GoldenIdol
    TheLab
    DivineFountain

[ ] 每个事件检查：
    EN/ZHS 文本
    选项可点
    不 softlock
    奖励正确
    退出/返回流程正常
    save/load after completion
```

### 验收

```text
[ ] CanaryOnly 从 loader-proof 进展到 minimal gameplay-proof
[ ] issue 中关闭 CanaryOnly loader-only blocker，但保留 save/load / full proof rows if pending
```

---

## Week 4：Batch 4c Candidate Review

### 任务

只提候选，不默认执行。

```text
[ ] 选 5–10 个 low-risk patches
[ ] 每个候选写：
    file
    class
    target
    current HarmonyPatch risk
    why low-risk
    expected runtime behavior unchanged
    rollback plan
    targeted tests
```

禁止候选：

```text
run lifecycle
save/load
map generation
multiplayer
lobby
death
reward mutation with player state
A20 boss transition
Sts1Events AdditiveAllDraft
```

### 验收

```text
[ ] Batch 4c candidate list ready
[ ] owner acceptance required before migration
[ ] high-risk remains blocked
```

---

# 6. 子代理分工要求

必须继续用 subagents。

## Subagent A — Validation Truth Agent

负责：

```text
完整 validation
stale test count 清理
current-validation.md
worktree batch truth
```

## Subagent B — Runtime QA Agent

负责：

```text
Off / CanaryOnly / AdditiveBatch1 logs
audit-godot-log
Mod Settings UI
SavedSpireFields
```

## Subagent C — Sts1Events Gameplay Agent

负责：

```text
4 个 canary event 手动矩阵
EN/ZHS
save/load
softlock check
```

## Subagent D — Architecture Evidence Agent

负责：

```text
FeatureRegistry runtime diagnostics
RewardPipeline diagnostics
CardPlayContext canary
DeathProtection no-op provider
MultiplayerPolicy metadata
```

## Subagent E — Batch 4c Candidate Agent

负责：

```text
low-risk candidate list
rollback plan
targeted tests
禁止 high-risk
```

## Subagent F — Release Gate Agent

负责：

```text
阻止 release-ready
阻止 AllDraft
阻止 high-risk migration
阻止 package refresh 冒充 gameplay proof
```

---

# 7. Overnight Run Spec：必须跑完才能停

下面是下一轮 overnight prompt，可直接发给 Codex。

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：Loader-Proven Runtime QA + Canary Gameplay Planning + Batch 4c Candidate Review。

这不是直接 Batch 4c migration。
不要迁更多 patches，除非 owner 明确接受 candidate 并授权执行。
不要新增 gameplay。
不要 claim release-ready。
必须使用 subagents。
必须跑完所有 phase；如果 blocker 存在，必须记录 issue、证据、下一步，不能直接停止。

当前状态：
- Latest main must be checked at start. Current validation may be stale relative to sprint 4.
- RitsuLib dependency and manifest dependency exist.
- 25 patches migrated to RitsuLib ModPatcher.
- Runtime loader evidence exists for Off, CanaryOnly, and AdditiveBatch1; verify it against latest HEAD and logs.
- Off mode loader proof: 0 Sts1Events registrations.
- CanaryOnly loader proof: exactly 4 canary events.
- AdditiveBatch1 loader proof: 10 event types / 11 calls.
- Gameplay, Mod Settings UI, save-load, co-op, independent QA, versioned package handoff remain pending.
- AdditiveAllDraft and ReplaceUnknownEventsPrototype are unsafe/dev-only.
- Release-ready remains no.

Subagents:

1. Validation Truth Agent
   - Run full validation.
   - Refresh current-validation.md to latest HEAD.
   - Remove stale test/warning/head counts.
   - Report worktree state and owner-decision needs.

2. Runtime QA Agent
   - Verify Off/CanaryOnly/AdditiveBatch1 evidence paths.
   - Audit godot logs.
   - Verify Mod Settings UI if environment allows.
   - Confirm 25/25 patches and 30 SavedSpireFields.

3. Sts1Events Gameplay Agent
   - Build manual test matrix for BigFish, GoldenIdol, TheLab, DivineFountain.
   - If environment allows, debug-spawn or route to those events.
   - Check EN/ZHS, options, rewards, no softlock, save/load.

4. Architecture Evidence Agent
   - Confirm FeatureRegistry diagnostics in runtime logs.
   - Confirm RewardPipeline diagnostics.
   - Confirm CardPlayContext canary logs.
   - Confirm MultiplayerPolicy metadata is emitted.
   - No gameplay behavior changes.

5. Batch 4c Candidate Agent
   - Propose 5-10 low-risk patches only.
   - No run/save/map/multiplayer/death/lobby/A20 boss patches.
   - Include rollback plan and targeted tests.
   - Do not migrate unless owner accepts.

6. Release Gate Agent
   - Block release-ready.
   - Block AllDraft tester/release path.
   - Block high-risk migration.
   - Block package refresh as gameplay proof.

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
- scripts/generate-patch-inventory.ps1 -Check
- scripts/report-worktree-batches.ps1 -FailOnUnclassified

Update docs/reviews/current-validation.md.

Phase 2 — Runtime evidence QA

Verify:
- Off mode log
- CanaryOnly log
- AdditiveBatch1 log
- godot-log-audit json
- 25/25 patches
- 30 SavedSpireFields
- exact loaded mods = BaseLib, RitsuLib, Spire Plus
- no MissingMethodException
- no TypeLoadException
- no release-blocking errors

If evidence is stale relative to latest HEAD:
- rerun smoke or mark pending.

Phase 3 — Mod Settings UI

If environment allows:
- verify Spire Plus appears in Mod Settings
- capture screenshot
- update runtime checklist

If not:
- mark pending.

Phase 4 — Sts1Events Canary gameplay matrix

For each canary event:
- BigFish
- GoldenIdol
- TheLab
- DivineFountain

Record:
- can spawn / can route
- EN/ZHS text renders
- options clickable
- reward/effect works
- no softlock
- save/load after completion

If cannot run:
- create manual matrix only and mark pending.

Phase 5 — Architecture evidence

Verify runtime logs or source evidence:
- FeatureRegistry summary
- RewardPipeline diagnostics
- CardPlayContext canary
- DeathProtection no-op provider
- MultiplayerPolicy metadata

No behavior changes.

Phase 6 — Batch 4c candidate review

Produce candidate list only:
- 5-10 low-risk patches
- file/class/target
- why low-risk
- targeted tests
- rollback plan
- owner approval required

Do not execute migration unless owner explicitly authorizes.

Phase 7 — Docs update

Update:
- docs/features/ritsulib-migration/monthly-dev-spec.md
- docs/features/ritsulib-migration/next-overnight-run.md
- docs/reviews/current-validation.md
- docs/issues/Sts1Events issue
- runtime-smoke-checklist.md

Final report must include:
1. actual HEAD
2. validation result
3. runtime evidence status
4. Mod Settings UI status
5. Sts1Events canary gameplay status
6. architecture diagnostics status
7. Batch 4c candidate list / decision
8. subagent findings
9. files changed
10. decision: optimize / advance / both
11. release-ready: no

Stop condition:
Do not stop until every phase is complete or blocked with explicit issue, evidence, and next action.
```

---

# 8. 最终判断

这轮工作的方向是对的，而且已经解决了以前最大的 runtime install blocker。现在已经从：

```text
runtime smoke blocked
```

进展到：

```text
loader-gate proof exists for Off / CanaryOnly / AdditiveBatch1
```

但它还没完成 release 所需的核心证明：

```text
gameplay
save/load
Mod Settings UI
co-op
event screenshots
versioned tester package
independent QA
clean latest HEAD validation
```

所以最终决策是：

```text
优化为主，有限推进为辅。
```

下一步可以开始 **Batch 4c candidate review**，但不能直接执行 migration；必须先完成 latest validation 和 runtime QA。
