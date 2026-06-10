部分早前上传的文件已经过期，我现在无法读取旧附件；如果你要我复核旧截图、旧 godot.log 或旧 Codex 报告，需要重新上传。下面这份审查基于当前 GitHub `main`、最新提交列表、`current-validation.md`、`monthly-dev-spec.md`、runtime evidence 文档和 sprint4 diff。

# 严格审核结论

## 2026-06-10 current status override

This goal document is historical below this override unless a row is repeated in current validation docs.
Use `PROJECT_STATE.md`, `docs/reviews/current-validation.md`, and `docs/goals/m5-revision-l-*` as the current source of truth.

- Current dirty-source build evidence: solution build passed with 0 errors and 0 warnings.
- Current Revision L no-game validation passed after clearing overlapping `testhost` state: test-project and exact solution-level no-build lanes both report 464 passed / 0 failed / 21 skipped / 485 total; format, diff-check, patch-inventory, and batch-classifier checks pass.
- Historical RitsuLib/Sts1Events loader-gate proof remains useful only for the `v0.106.1` setup.
- Current runtime proof is blocked by local Slay the Spire 2 `v0.107.0`: installed RitsuLib `v0.4.16` / `lib\0.107.0` and beta.84 package parity are in place, but the fresh Off smoke at `.tools/runtime-evidence/v01070-off-package-parity-20260610-092045/` is non-clean.
- Batch 4c remains proposal-only. Do not claim tester-ready, live-ready, runtime-ready, or release-ready from this document.

这轮工作**不是失败**，而且已经从“runtime smoke blocked”推进到了更好的状态：**RitsuLib loader-gate 证据已经出现，Off / CanaryOnly / AdditiveBatch1 都有 clean runtime evidence**。但它仍然**没有完成最终目标**，因为 live gameplay、save/load、Mod Settings UI、event screenshots、multiplayer fail-closed、independent QA rerun、clean worktree / owner decision、versioned tester package handoff 仍然没有关闭。

当前真实状态应该写成：

```text
RitsuLib loader-gate proof: 基本完成
Off=0 runtime loader proof: 完成
CanaryOnly=4 runtime loader proof: 完成
AdditiveBatch1 loader proof: historical 10/11 完成; current source expects 10/13 and still needs fresh v0.107 proof
RitsuLib full gameplay proof: 未完成
Sts1Events content completion: 未完成
Batch 4c: 可以进入 low-risk candidate proposal，但不能自动执行
Release-ready: 否
```

最新远程提交是：

```text
f32c6767 — update refactor.md with implementation results and Green Stop check
```

它已经在 `8f2d79b sprint3`、`6b149ba sprint 2`、`8782030 sprint 1` 之后。

---

# 1. 当前目标完成度总表

| 项目                                     | 当前状态                                              | 严格判定    |
| -------------------------------------- | ------------------------------------------------- | ------- |
| RitsuLib 依赖接入                          | 已完成                                               | PASS    |
| STS2-RitsuLib 本地安装                     | 已完成，`v0.4.16`，含 `lib\0.107.0`                     | PASS    |
| Off 模式 loader smoke                    | clean audit，通过主菜单、25/25 patch、30 SavedSpireFields | PASS    |
| CanaryOnly loader smoke                | clean audit，exactly 4 canary events               | PASS    |
| AdditiveBatch1 loader smoke            | historical clean audit，10 event types / 11 calls; current source expects 10 / 13 | historical PASS / current proof pending |
| Batch 4a/4b patch migration            | 25 patches migrated，source-level guard 完成         | PASS    |
| Full gameplay verification             | 未完成                                               | PENDING |
| Sts1Events event encounter screenshots | 未完成                                               | PENDING |
| Save/load proof                        | 未完成                                               | PENDING |
| Mod Settings UI evidence               | 未完成或历史证据不足                                        | PENDING |
| Multiplayer fail-closed / co-op proof  | 未完成                                               | PENDING |
| Independent QA rerun                   | 未完成                                               | PENDING |
| Versioned tester-package handoff       | 未完成                                               | PENDING |
| Release-ready                          | 否                                                 | CORRECT |

结论：**loader-gate 层面可以继续向前；release / live-ready 仍然绝对不能 claim。**

---

# 2. 每一步严格审查

## 2.1 最新 HEAD 与验证基线

GitHub 最新提交基线是 `f32c6767 update refactor.md with implementation results and Green Stop check`。
`current-validation.md` 里最新 “June 10 Refactor Validation” 的 HEAD 是：

```text
f32c6767 (HEAD -> main, origin/main, origin/HEAD) update refactor.md with implementation results and Green Stop check
```

并记录 worktree dirty；no-game validation 在没有重叠 test/build 进程后完成，runtime smoke 仍只有历史 `v0.106.1` loader evidence。

`f32c6767` 之后当前本地 worktree 又有 docs/source/test edits。当前 no-game validation 已经覆盖 dirty source；runtime dependency 已更新为 RitsuLib `v0.4.16` / `lib\0.107.0`，installed-package parity 已恢复，但 fresh `v0.107.0` Off loader smoke 是 non-clean；gameplay proof 仍未完成，并且仍然明确不要 claim live-ready / release-ready。

**判定：基本可接受，但仍需注意：**

```text
latest commit = f32c6767
fresh runtime proof target = fixed versioned package + clean v0.107.0 loader smoke; beta.84 package-parity smoke failed clean audit
```

如果要做正式 handoff 或 release package，必须先刷新 package/install parity 并补 fresh runtime smoke。

---

## 2.2 Build / test / format / diff

`current-validation.md` 记录 June 10 required commands：

```text
dotnet build EZMicroBalance.sln -m:1 --no-incremental: PASS, 0 errors, 0 warnings
dotnet test tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build: PASS, 464 passed, 0 failed, 21 skipped
dotnet test EZMicroBalance.sln --no-build: PASS, 464 passed, 0 failed, 21 skipped
dotnet format: PASS
git diff --check: PASS
generate-patch-inventory.ps1 -Check: PASS
report-worktree-batches.ps1 -FailOnUnclassified: PASS
```



**判定：自动化验证层基本通过，但不是 release clean。**

原因：

```text
[✓] Build/test/format/diff 通过。
[✓] patch inventory fresh。
[✓] batch classifier 0 unclassified。
[✓] 当前 dirty source 的 Sts1Events nullable warning debt 已清到 0。
[!] Worktree dirty，有 62 dirty entries。
[!] 当前 validation 不包含 fresh runtime smoke、package refresh 或 live gameplay proof。
```

这意味着：可以说“当前 no-game validation 通过”，不能说“clean release state”。

---

Current correction: the 70-warning item in the preceding historical checklist is superseded by the current build evidence: `dotnet build EZMicroBalance.sln -m:1 --no-incremental` reports `0 Warning(s)` / `0 Error(s)`. Worktree dirt, clean current runtime proof, package refresh, and live gameplay proof remain blockers.

## 2.3 Runtime smoke / loader gate

这是这轮的最大进步。

`current-validation.md` 记录 STS2-RitsuLib 已安装：

```text
E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib
True (v0.4.16, includes lib\0.107.0)
```

同时 BaseLib 和 EZMicroBalance 也存在。

Historical June 2 K1 runtime smoke 结果：

* Off mode Steam smoke 到主菜单，loaded exactly 3 mods：BaseLib v3.1.4、RitsuLib v0.3.10、Spire Plus v0.1.0-private-beta.84；25/25 ModPatcher patches；30 SavedSpireFields；Sts1Events default Off；clean audit。
* CanaryOnly direct launch 到主菜单，loaded exactly 3 mods，25/25 patches，30 SavedSpireFields，registered exactly 4 canary events：`Sts1BigFish`、`Sts1GoldenIdol`、`Sts1TheLab`、`Sts1DivineFountain`；clean audit。
* AdditiveBatch1 direct launch 到主菜单，10 event types / 11 registration calls，clean audit。

Stop decision 也明确写：

```text
Runtime loader gate: PASS
Off runtime proof: PASS
CanaryOnly runtime proof: PASS
AdditiveBatch1 runtime proof: PASS
```

但同时：

```text
gameplay proof, event screenshots, save/load, image/render proof, replacement functional proof, multiplayer fail-closed, independent QA, versioned tester-package handoff pending
```



**判定：loader-gate proof PASS；gameplay proof PENDING。**

这非常关键：以前最大的 “STS2-RitsuLib missing” blocker 已经不再是 blocker。现在 blocker 变成：

```text
runtime gameplay / manual proof / package handoff / co-op proof
```

---

## 2.4 RitsuLib patch migration

当前 project state / migration docs 已经同步：

```text
25 patches migrated to RitsuLib IPatchMethod
142 raw Harmony declarations remaining
tracked patch units total = 167
hybrid bootstrap active
```



Patch inventory 当前也记录：

```text
Migrated to RitsuLib ModPatcher: 25
Raw HarmonyPatch remaining: 142
High risk raw: 22
Medium risk raw: 35
Low risk raw: 85
```



**判定：Batch 4a/4b source-level closure PASS。**

而且现在 loader-gate 证明了 25/25 ModPatcher patches 能在 runtime 加载。下一步可以**提出** Batch 4c low-risk candidate list，但不应该自动迁移。因为 gameplay proof 还没补齐。

---

## 2.5 Sts1Events governance

Sts1Events issue 当前状态是：

```text
Open — governance hardened, content incomplete.
Default Off is safe.
CanaryOnly and AdditiveBatch1 are controlled source-test modes.
AdditiveAllDraft and ReplaceUnknownEventsPrototype are dev-only/unsafe.
```



模式矩阵：

```text
Off: 0 registrations, safe
CanaryOnly: 4 registrations / 4 event types, controlled
AdditiveBatch1: 11 registrations / 10 event types, controlled prototype testing
AdditiveAllDraft: 54 calls / 47 unique events, unsafe/dev-only
ReplaceUnknownEventsPrototype: unsafe/debug-only
```



风险表列出 7 个 HIGH 和 1 个 MEDIUM risk event，包括 Dead Adventurer、Scorpion Nest、Treasure Ooze、Masked Bandits、Mind Bloom、Mysterious Sphere、N’loth、Vampires。

**判定：governance PASS；content NOT DONE。**

这轮完成的是：

```text
Off / CanaryOnly / AdditiveBatch1 loader-gate proof
```

还没完成：

```text
event encounter gameplay
event screenshots
save/load
image/render proof
ZHS placeholder cleanup
combat/relic-select missing APIs
```

---

## 2.6 FeatureRegistry / diagnostics architecture

`current-validation.md` 记录：

```text
FeatureRegistry diagnostics observed for all 6 features
RewardPipeline diagnostics: PASS
CardPlayContext: Allow-only
DeathProtectionService: no-op / diagnostics-only
MultiplayerPolicy: taxonomy / diagnostics-only
MultiplayerFeaturePolicy: active feature suppression in co-op
```



Architecture status 也写：

```text
RewardPipeline diagnostics wired into FeatureRegistry bootstrap events and low-risk AscensionRewardService reward/card-reward surfaces.
ArchitectureCanaryBootstrap registers FeatureRegistry and Ascension reward diagnostic handlers, no-op DeathProtection provider, and multiplayer policy records.
Lotha extra-play touches CardPlayContextCanary through allow-only adapter.
Existing co-op gates still make same allow/disable decisions; evidence payloads include policy metadata.
```



**判定：architecture diagnostics/canary PASS；behavior enforcement NOT DONE。**

不能说：

```text
RewardPipeline fully owns rewards
CardPlay recursion fully solved
DeathProtection is implemented in gameplay
Multiplayer policy enforces co-op safety
```

只能说：

```text
Diagnostics and canary infrastructure now exists and is observed.
```

---

# 3. 这项任务是否完成？

取决于目标定义。

如果目标是：

```text
解除 STS2-RitsuLib missing blocker，并证明 Off/Canary/AdditiveBatch1 loader-gates。
```

那么：

```text
基本完成。
```

如果目标是：

```text
完成 RitsuLib migration、可测试包、live-ready 或 release-ready。
```

那么：

```text
没有完成。
```

当前正确状态：

```text
Loader-gate 阶段完成；
manual gameplay / save-load / package handoff / co-op proof 阶段未完成。
```

---

# 4. 当前与目标对比

| 目标                               | 当前结果                              | 判定           |
| -------------------------------- | --------------------------------- | ------------ |
| STS2-RitsuLib installed          | v0.4.16 installed                 | PASS         |
| Off loader smoke                 | clean audit, 0 Sts1 registrations | PASS         |
| CanaryOnly loader smoke          | clean audit, exactly 4 events     | PASS         |
| AdditiveBatch1 loader smoke      | clean audit, 10/11                | PASS         |
| 25 ModPatcher patches runtime    | 25/25 applied                     | PASS         |
| Mod Settings UI screenshot       | pending                           | FAIL/PENDING |
| Event gameplay screenshots       | pending                           | FAIL/PENDING |
| Save/load proof                  | pending                           | FAIL/PENDING |
| Image/render proof               | pending                           | FAIL/PENDING |
| Replacement functional proof     | pending                           | FAIL/PENDING |
| Multiplayer fail-closed          | pending                           | FAIL/PENDING |
| Independent QA rerun             | pending                           | FAIL/PENDING |
| Versioned tester package handoff | pending                           | FAIL/PENDING |
| Release-ready                    | no                                | CORRECT      |

---

# 5. 战略判断：继续优化、推进，还是两者兼顾？

现在不再是单纯优化；可以进入**两者兼顾**：

```text
60% 优化 / 证明 / 手测
40% 有限推进 / Batch 4c candidate review
```

原因：

```text
[✓] loader-gate blocker 已经解除；
[✓] 25/25 migrated patches runtime proof 已经有；
[✓] Off/Canary/AdditiveBatch1 loader proof 已经有；
[ ] gameplay / save-load / co-op / handoff 仍缺。
```

所以可以开始：

```text
Batch 4c low-risk candidate proposal
```

但不应该直接执行：

```text
Batch 4c migration
```

除非你明确批准候选列表。

---

# 6. 下个月开发规范 Monthly Dev Spec

## 月度主题

```text
Loader-Proven Manual QA + Controlled Batch 4c Planning Month
```

## 月度目标

```text
1. 将 loader-gate proof 升级为 manual gameplay proof。
2. 完成 Mod Settings UI / event screenshots / save-load / image-render / co-op fail-closed proof。
3. 保持 Sts1Events 0-warning regression guard；不要把已清理的 warning debt 当作当前 blocker。
4. 提出 Batch 4c low-risk candidate list。
5. 只有 owner 接受后才执行 Batch 4c。
```

---

## Week 1：Canonical Validation + Evidence Reconciliation

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
[ ] generate-patch-inventory.ps1 -Check
[ ] report-worktree-batches.ps1 -FailOnUnclassified
```

文档更新：

```text
docs/reviews/current-validation.md
docs/features/ritsulib-migration/monthly-dev-spec.md
docs/features/ritsulib-migration/runtime-smoke-checklist.md
```

验收：

```text
[ ] 最新 HEAD 与 validation 一致
[ ] dirty worktree 有 owner decision
[ ] Off/Canary/AdditiveBatch1 evidence paths 明确
```

---

## Week 2：Manual Gameplay QA — Off / CanaryOnly

任务：

```text
[ ] Off mode normal game boot
[ ] Mod Settings UI screenshot
[ ] CanaryOnly mode boot
[ ] Debug-spawn or route to:
    - Sts1BigFish
    - Sts1GoldenIdol
    - Sts1TheLab
    - Sts1DivineFountain
```

每个 canary event 必测：

```text
[ ] EN/ZHS text renders
[ ] options clickable
[ ] reward/effect correct
[ ] no softlock
[ ] event exits correctly
[ ] save/load after event completion
[ ] screenshot evidence
```

验收：

```text
[ ] CanaryOnly 从 loader-proof 变为 gameplay-smoke proof
```

---

## Week 3：Save/Load + Image/Render + Co-op Fail-Closed

任务：

```text
[ ] save during/after canary event
[ ] reload stability
[ ] event image/render check
[ ] placeholder decision if art missing
[ ] co-op mode check:
    - Sts1Events Off default
    - CanaryOnly fail-closed or explicitly disabled
    - no desync / no registration mismatch
```

验收：

```text
[ ] save-load proof 不再 pending
[ ] image/render proof 不再 pending
[ ] multiplayer fail-closed proof 不再 pending，或有 blocker issue
```

---

## Week 4：Batch 4c Candidate Review

任务：只提候选，不自动迁移。

候选要求：

```text
[ ] 5–10 low-risk patches
[ ] no run lifecycle
[ ] no save/load
[ ] no map generation
[ ] no multiplayer/lobby
[ ] no death
[ ] no A20 boss flow
[ ] no reward mutation with player state
```

每个候选必须写：

```text
file
class
target method
why low-risk
expected behavior unchanged
source evidence
targeted tests
rollback plan
```

验收：

```text
[ ] Batch 4c candidate list ready
[ ] owner approval required before migration
```

---

# 7. 子代理分工

必须继续使用 subagents。

## Subagent A — Validation Truth Agent

负责：

```text
完整命令链
current-validation.md
dirty worktree 分类
patch inventory check
```

## Subagent B — Runtime QA Agent

负责：

```text
Off / CanaryOnly / AdditiveBatch1 logs
Mod Settings UI
SavedSpireFields
25/25 patches
audit-godot-log
```

## Subagent C — Sts1Events Gameplay Agent

负责：

```text
BigFish
GoldenIdol
TheLab
DivineFountain
event options
reward/effect
save/load
screenshots
EN/ZHS
```

## Subagent D — Architecture Evidence Agent

负责：

```text
FeatureRegistry diagnostics
RewardPipeline diagnostics
CardPlayContext canary
DeathProtection no-op provider
MultiplayerPolicy metadata
```

## Subagent E — Co-op / Fail-Closed Agent

负责：

```text
Sts1Events multiplayer policy
CanaryOnly co-op behavior
fail-closed evidence
desync risks
```

## Subagent F — Batch 4c Candidate Agent

负责：

```text
low-risk candidate list
source evidence
rollback plan
targeted tests
no high-risk
```

## Subagent G — Release Gate Agent

负责：

```text
阻止 release-ready
阻止 AllDraft
阻止 unapproved Batch 4c
阻止 package refresh 冒充 gameplay proof
```

---

# 8. Overnight Run Spec：必须跑完才能停止

下面可以直接发给 Codex。

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：Loader-Proven Manual QA + Batch 4c Candidate Review Overnight Run。

这是 manual QA + candidate review。
这不是直接 Batch 4c migration。
不要迁更多 patches，除非 owner 在本轮明确批准 candidate execution。
不要新增 gameplay。
不要 claim release-ready。
必须使用 subagents。
必须跑完所有 phase；如果 blocker 存在，必须记录 issue、证据、下一步，不能直接停止。

当前状态：
- Latest main must be checked at start.
- STS2-RitsuLib v0.4.16 is installed with `lib\0.107.0`; historical v0.106.1 loader-gate proof for Off, CanaryOnly, and AdditiveBatch1 remains the only loader proof.
- Current v0.107.0 runtime proof is blocked by the non-clean beta.84 Off smoke; installed-package parity is available, but clean current loader proof is not.
- Historical Off mode loader proof: 0 Sts1Events registrations.
- Historical CanaryOnly loader proof: exactly 4 canary registrations.
- Historical AdditiveBatch1 loader proof: 10 event types / 11 calls.
- 25/25 migrated ModPatcher patches applied in runtime smoke.
- Gameplay, screenshots, save-load, image rendering, replacement functional proof, multiplayer fail-closed, independent QA, versioned tester-package handoff remain pending.
- Release-ready remains no.

Subagents:

1. Validation Truth Agent
   - Run full validation.
   - Refresh current-validation.md to latest HEAD.
   - Classify dirty worktree.
   - Ensure patch inventory is fresh.

2. Runtime QA Agent
   - Verify existing Off / CanaryOnly / AdditiveBatch1 evidence.
   - If evidence stale relative to latest HEAD, rerun loader smoke.
   - Verify Mod Settings UI if possible.
   - Confirm 25/25 patches and 30 SavedSpireFields.

3. Sts1Events Gameplay Agent
   - Test or prepare manual matrix for:
     BigFish
     GoldenIdol
     TheLab
     DivineFountain
   - Check EN/ZHS, options, reward/effect, no softlock, exit, save/load, screenshots.

4. Architecture Evidence Agent
   - Verify runtime logs contain:
     FeatureRegistry summary
     RewardPipeline diagnostics
     CardPlayContext canary
     DeathProtection no-op provider
     MultiplayerPolicy metadata
   - No behavior changes.

5. Co-op / Fail-Closed Agent
   - Determine Sts1Events behavior in co-op:
     default Off
     CanaryOnly disabled or fail-closed
     no registration mismatch
   - If no co-op environment, create blocker.

6. Batch 4c Candidate Agent
   - Propose 5–10 low-risk candidates only.
   - No run/save/map/multiplayer/death/lobby/A20/reward-state patches.
   - Include source evidence, targeted tests, rollback plan.
   - Do not migrate.

7. Release Gate Agent
   - Block release-ready.
   - Block AllDraft release path.
   - Block unapproved Batch 4c migration.
   - Block package refresh as substitute for gameplay proof.

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

Verify existing evidence:
- Off
- CanaryOnly
- AdditiveBatch1
- audit json
- 25/25 patches
- 30 SavedSpireFields
- exact loaded mods

If stale, rerun smoke.

Phase 3 — Mod Settings UI

If environment allows:
- open Mod Settings
- capture screenshot
- verify Spire Plus display name
- update checklist

If not:
- mark pending.

Phase 4 — Sts1Events Canary gameplay

For each:
- Sts1BigFish
- Sts1GoldenIdol
- Sts1TheLab
- Sts1DivineFountain

Record:
- can spawn or route
- EN/ZHS renders
- options clickable
- reward/effect works
- no softlock
- save/load after event
- screenshot evidence

Phase 5 — Save/load and image/render

- save during/after event if possible
- reload
- verify state stable
- verify event art/placeholder

Phase 6 — Co-op / fail-closed

If co-op environment exists:
- verify Sts1Events default Off
- verify unsafe modes do not desync
- verify CanaryOnly policy

If not:
- mark pending with blocker.

Phase 7 — Architecture evidence

Verify diagnostics:
- FeatureRegistry
- RewardPipeline
- CardPlayContext
- DeathProtection
- MultiplayerPolicy

Phase 8 — Batch 4c candidate review

Produce candidate list only:
- 5–10 low-risk patches
- source target
- risk reason
- targeted tests
- rollback plan

Do not migrate without explicit owner approval.

Phase 9 — Docs update

Update:
- docs/reviews/current-validation.md
- docs/features/ritsulib-migration/monthly-dev-spec.md
- docs/features/ritsulib-migration/next-overnight-run.md
- runtime-smoke-checklist.md
- Sts1Events issue
- manual QA matrix if created

Final report must include:
1. actual HEAD
2. validation result
3. runtime evidence status
4. Mod Settings UI status
5. Sts1Events canary gameplay status
6. save/load status
7. image/render status
8. co-op/fail-closed status
9. architecture diagnostics status
10. Batch 4c candidate list
11. subagent findings
12. files changed
13. decision: optimize / advance / both
14. release-ready: no

Stop condition:
Do not stop until every phase is complete or blocked with explicit issue, evidence, and next action.
```

---

# 9. 最终判断

这轮工作已经把最关键的 runtime install blocker 推过去了，进步很大：

```text
Off / CanaryOnly / AdditiveBatch1 loader-gate proof 已经有；
25/25 migrated patches runtime applied；
clean audit 已经有；
```

但还没完成：

```text
gameplay proof
save-load
screenshots
Mod Settings UI
co-op fail-closed
independent QA
versioned package handoff
```

因此下一步应当是：

```text
两者兼顾：继续优化 QA 证据，同时开始 Batch 4c 候选评审。
```

但仍然不能直接执行 Batch 4c，也不能 claim release-ready。
