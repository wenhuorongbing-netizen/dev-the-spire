部分旧上传文件已经过期，我现在不能读取那些旧附件。如果你要我复核旧截图、旧 `godot.log` 或旧 Codex 报告，需要重新上传。下面这份审查基于**当前 GitHub `main` 仓库状态**、当前 `current-validation.md`、`monthly-dev-spec.md`、`next-overnight-run.md` 和最新提交记录。

# 严格审核结论

**没有完成最终目标，但已经从“RitsuLib 缺失 / runtime smoke blocked”推进到了“v0.107.0 loader proof 部分成立，下一步进入手动 QA 与 Batch 4c 候选评审”的阶段。**

当前 GitHub 查询到的最新提交是：

```text
3f01cb7 — sprint 4
```

它位于 `8f2d79b sprint3`、`6b149ba sprint 2`、`8782030 sprint 1` 之后。

当前状态最准确地写应该是：

```text
RitsuLib migration：进行中
v0.107.0 runtime alignment：部分完成
beta.85 Off loader proof：完成
CanaryOnly / AdditiveBatch1 当前 v0.107 enabled-mode proof：仍缺
Batch 4a/4b：source-level 完成
Batch 4c：可以 owner review 候选，但不能自动迁移
Sts1Events：治理完成，内容未完成
Release-ready：否
```

---

# 1. 当前情况和上一阶段相比的变化

## 已经解决的旧 blocker

旧 blocker 是：

```text
STS2-RitsuLib 没装，不能 runtime smoke。
```

现在已经不是这个问题。当前文档记录本地装的是：

```text
STS2-RitsuLib v0.4.16
runtime variant: 0.107.0
当前本地游戏：Slay the Spire 2 v0.107.0
```

`monthly-dev-spec.md` 明确写本地 STS2-RitsuLib 是 `v0.4.16`，带 `0.107.0` runtime variant，并且先前的 `v0.3.10` 安装已经被备份。

## 新 blocker

当前真正的 blocker 变成：

```text
v0.107.0 当前包 / source / runtime / RitsuLib 版本需要严格对齐。
```

`monthly-dev-spec.md` 记录：当前 beta.84 包曾经在 v0.107.0 Off smoke 中到达主菜单，但该 smoke 不干净，Spire Plus 只 applied 17/25 ModPatcher patches，并且 `EctoplasmGoldGatePatch` 出现 packaged API drift。

随后 `current-validation.md` 记录 beta.85 已经修复这一路：beta.85 Off proof 日志报告 `v0.1.0-private-beta.85`，RitsuLib `0.4.16` / compat branch `0.107.0`，25/25 Spire Plus ModPatcher patches applied，StS1Events default Off，并且 `godot-log-audit.json` clean。

所以当前更准确结论是：

```text
beta.84 v0.107 runtime proof：失败，已被 beta.85 修复路线取代
beta.85 Off loader proof：通过
beta.85 CanaryOnly / AdditiveBatch1 当前 enabled-mode proof：仍缺
```

---

# 2. 每一步严格审查

## 2.1 当前验证状态

`current-validation.md` 记录 June 10/11 的 no-game 验证：

```text
dotnet build: PASS, 0 warnings, 0 errors
Sts1EventFeatureGuardTests: PASS, 31 passed
test project --no-build: PASS, 464 passed / 0 failed / 21 skipped / 485 total
solution --no-build: PASS, 464 passed / 0 failed / 21 skipped / 485 total
format: PASS
generate-patch-inventory: PASS
report-worktree-batches: PASS
git diff --check: PASS with CRLF normalization warnings only
```

这比之前的 89 warnings 状态更好：`current-validation.md` 明确说当前 source 兼容修复使用了 `AbstractModel.ModifyPowerAmountGivenAdditive(...)`、`Ectoplasm.ModifyGoldGained(...)` 和 `CookRestSiteOption.get_IsEnabled`，并且当前强制 build 验证已经是 **0 errors / 0 warnings**。

**判定：no-game validation 基本完成。**

但这里仍有两个 caveat：

```text
1. 当前文档里仍有 historical sections，不能误用旧 warning/test 数字。
2. 最新提交已经到 sprint 4，下一次 handoff 前仍要重跑 actual HEAD validation。
```

---

## 2.2 Runtime proof

当前 runtime proof 不是“完全通过”，而是分层通过：

### 已完成：beta.85 Off loader proof

`current-validation.md` 记录 beta.85 Off proof：

```text
v0.1.0-private-beta.85
RitsuLib 0.4.16 / compat branch 0.107.0
25/25 Spire Plus ModPatcher patches applied
StS1Events default Off
main menu reached
godot-log-audit clean
```

`next-overnight-run.md` 也确认：当前 `v0.107.0` beta.85 package runtime proof 已经 clean for loader/patch application。

**判定：Off loader proof PASS。**

### 仍缺：current enabled-mode proof

`next-overnight-run.md` 明确写：当前 enabled-mode proof 仍然缺失；在做 gameplay 证据之前，CanaryOnly 必须证明 **4 event types / 6 registration calls**，AdditiveBatch1 必须证明 **10 event types / 14 registration calls**，并且要保留 `enabled-mode-log-check.json` 和 `runtime-evidence-packet-check.json`。

这很关键，因为历史 `v0.106.1` 证据只证明旧 source shape：

```text
CanaryOnly：旧形态 exactly 4 canary registrations
AdditiveBatch1：旧形态 10 event types / 11 calls
```

而当前 source shape 已变化：

```text
CanaryOnly：4 event types / 6 calls
AdditiveBatch1：10 event types / 14 calls
```

`next-overnight-run.md` 已经明确指出旧证据不能直接证明当前 source shape。

**判定：enabled-mode proof PENDING。**

---

## 2.3 Dependency / package alignment

当前状态：

```text
runtime installed: STS2-RitsuLib v0.4.16
runtime variant: 0.107.0
repo compile package: STS2.RitsuLib 0.3.2
manifest minimum: 0.3.2
```

`monthly-dev-spec.md` 明确记录：NuGet 现在已有 `STS2.RitsuLib 0.4.16`，但没有单独的 `STS2.RitsuLib.Compat.0.107.0` 包；当前 dirty source 状态的决策是**不要原地 bump compile package 或 manifest minimum**。未来 owner-approved `v0.107.0` tester package 应该在同一个版本化 package pass 里把二者 bump 到 `0.4.16`。

**判定：当前不 bump 是合理保守策略。**

但这也意味着：

```text
现在不能说 v0.107 package line 已完全完成。
```

---

## 2.4 Patch migration

当前 patch 状态：

```text
25 patch classes migrated to RitsuLib IPatchMethod
142 raw HarmonyPatch declarations remain
167 tracked patch units
hybrid bootstrap active
```

**判定：Batch 4a/4b source-level 完成。**

但 Batch 4c 当前只能是 proposal。`next-overnight-run.md` 明确写：Batch 4c 当前只是 proposal，候选列表在 `batch-4c-candidates.md`，迁移需要 explicit owner approval 和 fresh validation。

**判定：Batch 4c 不能自动执行。**

---

## 2.5 Sts1Events

当前状态分三层：

### Off mode

beta.85 Off loader proof 已有，Sts1Events default Off。

**判定：PASS for loader gate。**

### CanaryOnly

历史 `v0.106.1` 证据证明过旧 source shape，但当前 `v0.107.0` proof 仍需要重新证明 4 event types / 6 registration calls。

**判定：PENDING for current source/runtime。**

### AdditiveBatch1

历史证据证明过旧 10 event types / 11 calls；当前 source 需要证明 10 event types / 14 calls。

**判定：PENDING for current source/runtime。**

### AllDraft / Replacement

继续 unsafe/dev-only。`monthly-dev-spec.md` 也明确 AdditiveAllDraft 和 ReplaceUnknownEventsPrototype 仍 unsafe/dev-only，不是 tester/release-safe。

**判定：正确 blocked。**

---

## 2.6 Gameplay / UI / save-load / co-op

当前全部仍 pending。`next-overnight-run.md` 明确列出：

```text
Gameplay
Mod Settings UI page refresh
event screenshots
save-load
image/render
replacement functional proof
multiplayer fail-closed proof
independent QA
clean-worktree decision
tester-package handoff
```

仍未完成。

**判定：未完成。**

这就是为什么 release-ready 仍然必须是 no。

---

# 3. 与目标对比

## 我们的目标

```text
1. RitsuLib runtime 可用
2. v0.107.0 package/source/runtime 对齐
3. Sts1Events safe modes 可以被验证
4. Batch 4c 只在安全后推进
5. 不 release-ready 直到 gameplay/save-load/co-op/handoff 完成
```

## 当前结果

| 目标                                  | 当前状态                                                             | 判定      |
| ----------------------------------- | ---------------------------------------------------------------- | ------- |
| RitsuLib installed                  | v0.4.16 installed                                                | PASS    |
| v0.107 Off loader proof             | beta.85 clean                                                    | PASS    |
| v0.107 CanaryOnly current shape     | 4 types / 6 calls 未证明                                            | PENDING |
| v0.107 AdditiveBatch1 current shape | 10 types / 14 calls 未证明                                          | PENDING |
| package/source alignment            | beta.85 Off proof clean；compile/manifest still 0.3.2 by decision | PARTIAL |
| Batch 4c                            | proposal-only                                                    | CORRECT |
| gameplay proof                      | 未完成                                                              | PENDING |
| save/load proof                     | 未完成                                                              | PENDING |
| Mod Settings UI                     | 未完成                                                              | PENDING |
| co-op proof                         | 未完成                                                              | PENDING |
| release-ready                       | no                                                               | CORRECT |

---

# 4. 当前决策：优化、推进，还是兼顾？

**结论：两者兼顾，但仍偏优化。**

建议比例：

```text
65% 优化 / QA / runtime proof
35% 有限推进 / Batch 4c owner review
```

理由：

```text
Off loader proof 已通过；
runtime install blocker 已解决；
beta.85 package line 可作为当前基础；
但 CanaryOnly/AdditiveBatch1 当前 shape 未证明；
gameplay/save-load/co-op/Mod Settings 未完成。
```

允许推进：

```text
Batch 4c candidate review
CanaryOnly current enabled-mode smoke
AdditiveBatch1 current enabled-mode smoke
Mod Settings UI proof
Canary gameplay matrix
```

禁止推进：

```text
Batch 4c actual migration without owner approval
high-risk migration
AllDraft/Replacement tester path
release-ready claim
```

---

# 5. 下个月开发规范 Monthly Dev Spec

## 月度主题

```text
v0.107 Runtime QA + Batch 4c Candidate Review Month
```

## 月度目标

```text
1. 以 beta.85 / v0.107.0 为当前 runtime baseline。
2. 完成 current CanaryOnly 与 AdditiveBatch1 enabled-mode loader proof。
3. 完成 Mod Settings UI proof。
4. 完成 CanaryOnly 4 event gameplay smoke。
5. 完成 save/load 与 image/render 最小证明。
6. 完成 Batch 4c candidate review，但不默认迁移。
```

---

## Week 1：Validation Truth + Enabled-Mode Loader Proof

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

runtime：

```text
[ ] CanaryOnly v0.107 enabled-mode smoke
    expected: 4 event types / 6 registration calls
    retain enabled-mode-log-check.json
    retain runtime-evidence-packet-check.json

[ ] AdditiveBatch1 v0.107 enabled-mode smoke
    expected: 10 event types / 14 registration calls
    retain verifier reports
```

验收：

```text
[ ] current-validation.md 对应最新 HEAD
[ ] CanaryOnly current shape proof 完成
[ ] AdditiveBatch1 current shape proof 完成
```

---

## Week 2：Mod Settings UI + Canary Event Gameplay

任务：

```text
[ ] Mod Settings UI screenshot
[ ] Spire Plus display name correct
[ ] BaseLib/RitsuLib/Spire Plus only enabled
```

Canary gameplay：

```text
[ ] BigFish
[ ] GoldenIdol
[ ] TheLab
[ ] DivineFountain
```

每个事件记录：

```text
EN/ZHS render
options clickable
reward/effect works
no softlock
exit works
screenshot
```

---

## Week 3：Save/Load + Image/Render + QA

任务：

```text
[ ] save during/after canary event
[ ] reload stable
[ ] event image/placeholder decision
[ ] independent QA rerun
[ ] update Sts1Events issue evidence
```

验收：

```text
[ ] CanaryOnly 从 loader proof 升级为 gameplay smoke proof
```

---

## Week 4：Batch 4c Candidate Review

只做 candidate review：

```text
[ ] 5–10 low-risk candidates
[ ] no run lifecycle
[ ] no save/load
[ ] no map generation
[ ] no multiplayer/lobby
[ ] no death handling
[ ] no A20 boss flow
[ ] no reward-state mutation
```

每个候选必须有：

```text
file/class/target
risk reason
source evidence
targeted tests
rollback plan
owner decision
```

如果 owner 未批准：

```text
不迁移。
```

---

# 6. 子代理分工

## Subagent A — Validation Agent

负责：

```text
完整命令链
current-validation.md
worktree dirty 分类
testhost/stale process 防护
```

## Subagent B — Runtime Enabled-Mode Agent

负责：

```text
CanaryOnly 4 types / 6 calls
AdditiveBatch1 10 types / 14 calls
evidence packet verifier
audit-godot-log
```

## Subagent C — UI/Game QA Agent

负责：

```text
Mod Settings UI
BigFish
GoldenIdol
TheLab
DivineFountain
screenshots
EN/ZHS
```

## Subagent D — Save/Image QA Agent

负责：

```text
save-load
image/render
placeholder decision
proof paths
```

## Subagent E — Batch 4c Candidate Agent

负责：

```text
candidate list
risk classification
rollback plan
targeted tests
owner decision
```

## Subagent F — Release Gate Agent

负责：

```text
阻止 release-ready
阻止 AllDraft/Replacement
阻止 unapproved Batch 4c
阻止 package refresh 冒充 gameplay proof
```

---

# 7. Overnight Run Spec：必须跑完才能停

下面可以直接发给 Codex。

```text
你现在在 D:\Game\FOTN\dev-the-spire。

目标：v0.107 Enabled-Mode Proof + Manual QA + Batch 4c Owner Review Overnight Run。

这不是自动 Batch 4c migration。
不要迁更多 patches，除非 owner 在本轮明确接受候选并授权执行。
不要新增 gameplay。
不要 claim release-ready。
必须使用 subagents。
必须跑完所有 phase；如果 blocker 存在，必须记录 issue、证据、下一步，不能直接停止。

当前状态：
- STS2-RitsuLib v0.4.16 with lib/0.107.0 is installed locally.
- beta.85 Off loader proof is clean on v0.107.0.
- Historical v0.106.1 CanaryOnly/AdditiveBatch1 evidence exists but current source shape changed.
- Current CanaryOnly must prove 4 event types / 6 registration calls.
- Current AdditiveBatch1 must prove 10 event types / 14 registration calls.
- 25 patches migrated to RitsuLib IPatchMethod; 142 raw HarmonyPatch declarations remain.
- Batch 4c is proposal-only; owner approval required.
- Gameplay, Mod Settings UI, screenshots, save-load, image/render, co-op, independent QA, versioned package handoff remain pending.
- Release-ready remains no.

Subagents:

1. Validation Agent
   - Run full validation.
   - Update current-validation.md to actual HEAD.
   - Classify dirty worktree.

2. Runtime Enabled-Mode Agent
   - Capture CanaryOnly enabled-mode smoke.
   - Capture AdditiveBatch1 enabled-mode smoke.
   - Retain enabled-mode-log-check.json and runtime-evidence-packet-check.json.
   - Audit godot logs.

3. UI/Game QA Agent
   - Capture Mod Settings UI evidence.
   - Test BigFish, GoldenIdol, TheLab, DivineFountain if possible.
   - Record EN/ZHS render, options, rewards, no softlock, exit.

4. Save/Image QA Agent
   - Capture save/load proof for canary events if possible.
   - Verify image/render/placeholder decisions.

5. Batch 4c Candidate Agent
   - Review batch-4c-candidates.md.
   - Enforce low-risk rules.
   - Provide owner decision table.
   - Do not migrate without explicit approval.

6. Release Gate Agent
   - Block release-ready.
   - Block AllDraft and Replacement tester/release paths.
   - Block unapproved Batch 4c.
   - Block high-risk migration.

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

Phase 2 — Current enabled-mode runtime proof

Run CanaryOnly:
- verify 4 event types / 6 registration calls
- clean audit
- retain verifier reports

Run AdditiveBatch1:
- verify 10 event types / 14 registration calls
- clean audit
- retain verifier reports

Do not run AdditiveAllDraft or ReplaceUnknownEventsPrototype.

Phase 3 — Mod Settings UI

Capture:
- Spire Plus appears correctly
- dependency list acceptable
- screenshot evidence

Phase 4 — Canary gameplay smoke

For each canary:
- Sts1BigFish
- Sts1GoldenIdol
- Sts1TheLab
- Sts1DivineFountain

Record:
- spawn/route status
- EN/ZHS render
- options clickable
- reward/effect
- no softlock
- exit
- screenshot

Phase 5 — Save/load and image/render

If possible:
- save after event
- reload
- verify state
- verify art/placeholder

If not possible:
- create blocker rows.

Phase 6 — Batch 4c candidate review

Review candidate list:
- reject high-risk surfaces
- keep only 5–10 low-risk candidates
- provide rollback and targeted tests
- record owner decision
- do not migrate unless explicitly accepted

Phase 7 — Docs update

Update:
- docs/reviews/current-validation.md
- docs/features/ritsulib-migration/monthly-dev-spec.md
- docs/features/ritsulib-migration/next-overnight-run.md
- runtime-smoke-checklist.md
- Sts1Events issue
- batch-4c-candidates.md if needed

Final report:
1. actual HEAD
2. validation result
3. CanaryOnly enabled-mode proof
4. AdditiveBatch1 enabled-mode proof
5. Mod Settings UI status
6. canary gameplay status
7. save/load status
8. image/render status
9. Batch 4c owner decision
10. subagent findings
11. files changed
12. decision: optimize / advance / both
13. release-ready: no

Stop condition:
Do not stop until every phase is complete or blocked with explicit issue, evidence, and next action.
```

---

# 8. 最终判断

这轮状态已经比之前好很多：**v0.107.0 Off loader proof 已经 clean，RitsuLib 安装和 beta.85 package proof 已经不再是完全 blocker。**

但还没完成：

```text
current CanaryOnly / AdditiveBatch1 enabled-mode proof
manual gameplay
save-load
UI
co-op
independent QA
versioned tester handoff
```

因此现在的正确策略是：

```text
两者兼顾：继续优化 QA 证据，同时开始 Batch 4c owner review。
```

但注意：**只是 review 候选，不是直接迁移。**
