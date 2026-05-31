## Revision J 严格审核结论

当前状态仍然不能写“全部完成”。Revision J 的正确目标是 runtime hard-blocker closure and owner-review packet，不是 Batch 4c、Batch 5、PR7、debug expansion、Sts1Events formalization 或 longhaul audit。

```text
HEAD: 6b149ba0 (HEAD -> main, origin/main, origin/HEAD) sprint 2
Worktree: dirty; no commit/push/stash/checkout/reset/restore authorized
Runtime dependency: E-drive BaseLib, STS2-RitsuLib v0.3.10, and EZMicroBalance are installed
Runtime hard blocker: fresh loader log reaches main menu, but clean-audit/runtime proof is blocked by 11 Godot ERROR hits
Off=0 runtime proof: absent
CanaryOnly=4 runtime proof: absent
RitsuLib status: compile/manifest attempted, runtime unverified
Sts1Events recommendation: staging-only
Debug recommendation: accept-scaffold
Batch 4c / Batch 5 / PR7: blocked
Runtime-ready / live-ready / release-ready: no
```

Revision I can be treated only as a prior hard-blocker stop. Revision J must produce an owner-review packet or a precise hard-blocker report; build/test success alone is not completion.

---

## 1. 当前状态与我们的目标对比

Revision J 的主变化是：`STS2-RitsuLib` dependency path blocker 已经关闭为“installed”，fresh loader log 已经证明 BaseLib、RitsuLib、Spire Plus 可以到 main menu，但 runtime smoke 仍 hard blocked，因为 audit 仍有 11 个 Godot ERROR hits。

这说明：**terminal validation 可以重放；runtime/live 层面仍被正确阻塞；commit/owner-review 层面必须收口。** RitsuLib 仍只是 compile/manifest attempted，runtime unverified；Sts1Events 推荐 staging-only；Debug 推荐 accept-scaffold；这些不能被扩大成 runtime-ready 或 release-ready。

---

## 2. 是否完成？

### Revision I overnight run：**有条件完成**

如果 Revision I 的停止条件允许：

```text
A. terminal validation 全绿
B. 或遇到必须 owner/runtime 介入的 hard blocker
```

那么这次可以接受为：

```text
Revision I hard-blocker stop：通过
```

因为他没有继续 commit / push / stash / checkout，也明确把 blocker 写成：

```text
STS2-RitsuLib runtime path was missing in Revision I
Revision J fresh loader log exists but is not clean
runtime proof remains blocked
```

这比之前“测试失败还说 clean”“format timeout 还说 clean”“Sts1Events untracked/unrelated”的状态要好得多。旧日志里确实曾经出现过测试仍失败、format timeout 却写 clean 的情况，这次没有重复这个错误。

### 整体任务：**未完成**

不能验收为整体完成，原因有四个：

1. **49 dirty entries** 还没进入 owner 决策。
2. **89 warnings** 仍然存在，虽然是 Sts1Events nullable，但仍需 warning ledger 和治理决策。
3. **RitsuLib clean runtime proof 缺失**，fresh loader log 仍有 11 个 Godot ERROR hits。
4. **Sts1Events runtime proof 缺失**，没有 Off=0 / CanaryOnly=4 runtime proof。

所以当前状态应该写成：

```text
Validation green, runtime hard-blocked, owner-review pending.
```

不能写：

```text
complete
runtime-ready
release-ready
ready for PR7
ready for longhaul audit
```

---

## 3. 每一步严格检查

### 3.1 Git / 工作树

他报告：

```text
No commit/push/stash/checkout/reset/restore was performed
Worktree: dirty, 49 entries, 0 unclassified
```

这是进步。之前日志中曾出现 stash、checkout、stash pop conflict、stash drop 等危险操作，并造成状态混乱。 这次至少没有继续扩大这个风险。

但 49 dirty entries 仍意味着：

```text
commit-ready = no
```

下一步必须由 `DirtyStateReconciliationAgent` 和 `CommitSliceAgent` 把 38 项拆成：

```text
source
tests
docs
localization
scripts
runtime-status ledgers
owner decision required
rollback plan
```

---

### 3.2 Build / test / format / batch

这次验证可以接受为 terminal validation passed：

```text
build passed
tests passed
format passed
diff check passed
patch inventory check passed
batch classifier passed
```

但 build 仍有：

```text
89 Sts1Events nullable warnings
```

这不是 release blocker 本身，但它是 **Sts1Events formalization blocker**。如果 Sts1Events 保持 staging-only，可以作为 staging debt；如果要 formal，就必须修复或逐项接受。

---

### 3.3 Runtime smoke

runtime 阻塞是明确的：

```text
STS2-RitsuLib installed; fresh loader log exists
loader audit not clean: 11 Godot ERROR hits
```

这是真 hard blocker，不应该由代码继续猜。因为当前 manifest 已经声明 RitsuLib dependency，而当前 loader log 说明 `RitsuLibBootstrap.ApplyPatches()` 是从 `MainFile.Initialize()` 进入的 runtime integration；在 8 个 optional Spire Plus ModPatcher failures 和 `ritsulib-variants.json` manifest parsing error 没有解决或明确接受前，不能声称 loader/runtime 可用。

正确状态：

```text
RitsuLib = compile/manifest attempted; runtime unverified.
```

---

### 3.4 Sts1Events

他现在记录：

```text
Sts1Events: staging-only
No Off=0 runtime proof
No CanaryOnly=4 runtime proof
89 nullable warnings
```

这个判断合理。Sts1Events 不能 formalize，也不能进入 release claim。此前它已经进入 source/export/localization/docs/tests surface，所以也不能再被说成 untracked 或 unrelated；项目 `.csproj` 会编译 `EZMicroBalanceCode/**/*.cs`，除非有明确 exclusion。

正确状态：

```text
Sts1Events staging-only; formal feature blocked by warnings + runtime proof + localization/resource debts.
```

---

### 3.5 Debug

他记录：

```text
Debug: accept-scaffold
```

可以接受，但只限于 scaffold。不能写 debug feature complete。历史上他曾经在测试失败时宣布 debug complete，这种表述已经被纠正。

正确状态：

```text
Debug accept-scaffold; feature-complete not achieved.
```

---

### 3.6 RitsuLib

当前记录：

```text
RitsuLib: compile/manifest attempted, runtime unverified
Batch 4c: blocked
```

这是真实状态。此前他曾经把 PR5 写成 hard dependency done，但后续 subagent 已经把它降级为 attempted/runtime unverified。

正确状态：

```text
PR5/PR6 source work partially done.
Runtime verification blocked.
High-risk migration blocked.
```

---

### 3.7 Patch inventory

他报告：

```text
25 migrated IPatchMethod
142 raw HarmonyPatch declarations
167 tracked patch units
generate-patch-inventory.ps1 -Check passed
```

这比之前“142 total / 142 raw / 25 migrated”含糊状态更清楚。可以接受为 source-level patch inventory truth improved。
但不能基于这个继续 PR6 Batch4c，因为 runtime gate 仍未过。

---

## 4. 综合决策：继续优化、推进，还是两者兼顾？

我的决定：

```text
两者兼顾，但必须 optimization-first。
```

具体解释：

### 继续优化，优先级 70%

必须先处理：

```text
1. 49 dirty entries owner-review packet
2. 89 warnings warning ledger
3. loader-error runtime blocker
4. clean godot.log blocker
5. Off=0 / CanaryOnly=4 runtime proof absence
6. RitsuLib runtime truth docs
7. Sts1Events staging-only docs
8. Debug scaffold docs
9. commit slices
```

### 有限推进，优先级 30%

可以推进的只有：

```text
1. runtime dependency installation plan
2. runtime smoke plan
3. owner decision packet
4. Sts1Events staging governance
5. RitsuLib runtime verification readiness
```

不能推进：

```text
PR6 Batch4c
PR6 Batch5
PR7
high-risk patch migration
debug expansion
Sts1Events formalization
longhaul audit
release packaging claim
```

---

## 5. 下个月开发规范：M4 Revision J

下个月规范目标：

```text
Runtime hard-blocker closure + owner-review packet.
```

不是继续写功能。

核心完成条件：

```text
1. runtime hard blocker closed or precisely documented
2. 49 dirty entries fully reconciled
3. 89 warnings classified
4. terminal validation still green
5. Sts1Events / Debug / RitsuLib decisions truthful
6. commit slices complete
7. no unauthorized commit
```

下载文件：
[devspire_m4_revision_j_runtime_hard_blocker_spec.md](sandbox:/mnt/data/devspire_m4_revision_j_runtime_hard_blocker_spec.md)

---

## 6. 必须提醒他使用 subagent

下一轮必须继续强制 subagent。主 agent 不能直接边做边验收。

必用 subagent：

```text
RuntimeDependencyAgent
RuntimeSmokeAgent
DirtyStateReconciliationAgent
WarningLedgerAgent
RitsuLibRuntimeAgent
Sts1EventsGovernanceAgent
DebugDecisionAgent
PatchInventoryAgent
TestChangeReviewAgent
DocsTruthAgent
CommitSliceAgent
```

特别强调：

```text
RuntimeDependencyAgent 必须先查 STS2-RitsuLib runtime path。
RuntimeSmokeAgent 只有在 runtime dependency 存在后才能运行。
CommitSliceAgent 只能准备 commit plan，不允许 commit。
```

---

## 7. 夜间运行任务：必须跑到完成才能停

你可以直接发给他：

```text
进入 M4 Revision J overnight runtime hard-blocker closure and owner-review run。

当前状态不是 complete。Revision J 正确停止在 runtime hard blocker：STS2-RitsuLib 已安装，fresh loader log 到达 main menu，但 11 个 Godot ERROR hits 和 Off=0/CanaryOnly=4 runtime proof absent 仍阻塞。不要继续 PR6 Batch4c、PR6 Batch5、PR7、debug expansion、Sts1Events formalization 或 longhaul audit。

禁止：
- commit
- push
- stash / stash drop
- checkout branch
- reset / restore
- broad clean
- 写 runtime verified / release-ready，除非有 runtime evidence

你不能停止，直到满足以下之一：

A. Ready-to-owner-review packet 完成：
- runtime hard blocker closed or precisely documented
- current 49 dirty entries fully reconciled
- untracked files have decisions
- all terminal validation commands exit 0
- warning-ledger has no TBD
- 89 warnings classified by file/code/owner
- Sts1Events formal/staging/remove recommendation recorded
- Debug accept-scaffold/feature-complete/rollback recommendation recorded
- RitsuLib attempted/runtime-validated/release-ready/rollback status recorded
- patch inventory raw/migrated/tracked unit relationship explained
- test changes reviewed
- localization backlog recorded
- commit slices complete
- no unauthorized commit

B. Hard blocker:
- exact command/file/path
- why current worktree cannot resolve it
- rollback/staging/owner-decision options
- exact owner action required

必须先读：
AGENTS.md
PROJECT_STATE.md
docs/README.md
docs/test-ready-development-goal.md
docs/worktree-cleanup-audit.md
docs/patch-inventory.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/overnight-diff-ledger.md
docs/goals/warning-ledger.md
docs/integrations/ritsulib.md
docs/migration.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

必须先使用 subagents，只调查后修改：
1. RuntimeDependencyAgent
2. RuntimeSmokeAgent
3. DirtyStateReconciliationAgent
4. WarningLedgerAgent
5. RitsuLibRuntimeAgent
6. Sts1EventsGovernanceAgent
7. DebugDecisionAgent
8. PatchInventoryAgent
9. TestChangeReviewAgent
10. DocsTruthAgent
11. CommitSliceAgent

RuntimeDependencyAgent 必须先验证：
- Game root
- BaseLib install
- STS2-RitsuLib install
- EZMicroBalance install
- runtime DLL presence
- active godot.log availability or absence

ValidationReplay 必须运行：
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet build .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified

必须创建或更新：
docs/goals/revision-j-final-report.md
docs/goals/revision-j-owner-review-packet.md
docs/goals/revision-j-runtime-hard-blocker.md
docs/goals/revision-j-runtime-smoke-plan.md
docs/goals/revision-j-dirty-ledger.md
docs/goals/revision-j-commit-slices.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/warning-ledger.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

最终报告只能写两种之一：
Complete: ready-to-owner-review packet complete.
Not complete: exact hard blocker encountered.

不要因为 build/tests pass 就写 complete。
不要因为 runtime path missing 就继续猜。
不要因为 RitsuLib compiles 就写 runtime-ready。
不要未经 owner 授权 commit。
```

---

## 一句话总评

这次他做对了一件关键事：**在 runtime hard blocker 处停止，而不是继续乱推进。**
因此 Revision I 可以算“按 hard blocker 规则正确停止”。但整体仍未完成：49 dirty entries、89 warnings、缺 clean RitsuLib runtime proof、loader log 仍有 11 个 Godot ERROR hits、缺 Off=0 / CanaryOnly=4 runtime proof，都还需要下一个 overnight run 收口。下一步是 M4 Revision J：先解决 loader error disposition 和 owner-review packet，再决定 commit、rollback，或进入治理下一阶段。
# DevSpire M4 Revision J — Runtime Hard-Blocker Closure & Owner-Review Monthly Spec

Date: 2026-05-31
Scope: dev-the-spire / Spire Plus (`EZMicroBalance` technical manifest id)

## 0. Strict Audit Verdict

### Conditional completion

The latest run can be accepted only as a **Revision I hard-blocker stop**, not as total completion.

Accepted:

- No commit, push, stash, checkout, reset, or restore was performed during the reported Revision I run.
- Terminal validation reportedly passed:
  - `dotnet clean .\EZMicroBalance.csproj`
  - `dotnet build .\EZMicroBalance.csproj`
  - `dotnet build .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj`
  - `dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build`
  - `dotnet format .\EZMicroBalance.csproj --verify-no-changes`
  - `git diff --check`
  - `.\scripts\generate-patch-inventory.ps1 -Check`
  - `.\scripts\report-worktree-batches.ps1 -FailOnUnclassified`
- The run correctly stopped on a runtime hard blocker:
  - `E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib` is missing.
  - fresh `godot.log` exists but is not clean.
  - no Off=0 runtime proof exists.
  - no CanaryOnly=4 runtime proof exists.

Rejected / still incomplete:

- Overall project completion.
- Release-readiness.
- Runtime-readiness.
- Live-readiness.
- Commit-readiness.
- Longhaul audit readiness.
- RitsuLib hard dependency completion.
- Sts1Events formal feature completion.
- Debug feature completion.

Current state must be recorded as:

```text
Revision I overnight run: conditionally complete via hard-blocker stop.
Project status: not complete.
Release-ready: no.
Runtime-ready/live-ready: no.
Commit-ready: no.
Next mode: M4 Revision J runtime hard-blocker closure + owner-review packet.
```

## 1. Current Ground Truth to Preserve

Latest reported state:

```text
HEAD: 87820303 (HEAD -> main, origin/main, origin/HEAD) sprint 1
Worktree: dirty, 54 entries
Batch classifier: 0 unclassified
Build: passed, 0 errors, 89 Sts1Events nullable warnings
Tests: 464 passed / 0 failed / 21 skipped / 485 total
Format: passed
Whitespace: passed
Patch inventory check: passed
Runtime smoke: blocked by non-clean loader log after STS2-RitsuLib, BaseLib, and Spire Plus loaded
```

Project decisions currently in force:

```text
Sts1Events: staging-only
Debug: accept-scaffold
RitsuLib: compile/manifest attempted; runtime unverified
FeatureRegistry: source-level metadata/bootstrap truth guarded; runtime unproven
RewardPipeline/CardPlayContext: diagnostics/canary only; no gameplay claim
DeathProtectionService/MultiplayerPolicy: diagnostics/taxonomy only; no gameplay enforcement claim
Batch 4c: blocked
Decision posture: both optimize and advance, but optimization-first
```

## 2. Non-Negotiable Rules

Do not:

- Commit.
- Push.
- Stash or drop stash.
- Checkout branches.
- Reset / restore.
- Run broad clean.
- Continue PR6 Batch4c / Batch5 / PR7.
- Expand debug.
- Formalize Sts1Events.
- Resume longhaul audit.
- Claim runtime verified / release-ready without runtime evidence.
- Claim RitsuLib hard dependency done without loader proof.
- Treat Sts1Events as unrelated or untracked while it appears in compile, export, localization, docs, or tests.

Allowed scope before owner approval:

- Reconcile current dirty state.
- Finish owner-review packet.
- Close runtime hard blocker if owner provides or installs missing runtime files.
- Validate runtime smoke only after prerequisites exist.
- Correct docs truth.
- Improve ledgers, warning classification, and commit-slice planning.

## 3. Subagent Requirements

Every non-trivial task must use a subagent first. The main agent must only integrate findings and apply minimal safe changes.

Required subagents:

### 3.1 RuntimeDependencyAgent

Purpose:

- Verify whether `STS2-RitsuLib` exists under the active game root.
- Confirm BaseLib, STS2-RitsuLib, and EZMicroBalance install folders.
- Confirm NuGet/runtime version mismatch status.
- Confirm whether RitsuLib runtime is required by manifest and bootstrap.

Must report:

```text
GameRoot
BaseLib path
STS2-RitsuLib path
EZMicroBalance path
runtime DLL presence
ritsulib-variants.json presence
selected runtime variant
version skew
install instructions
hard blocker status
```

### 3.2 RuntimeSmokeAgent

Purpose:

- Run loader smoke only after RuntimeDependencyAgent clears prerequisites.
- Collect `godot.log`.
- Audit log for loader/API/runtime failures.
- Verify Off=0 and CanaryOnly=4 runtime evidence if Sts1Events remains staging.

Must not claim live gameplay proof.

### 3.3 DirtyStateReconciliationAgent

Purpose:

- Reconcile all 49 dirty entries.
- Confirm 0 unclassified remains true.
- Update dirty ledger and owner-review packet.
- Distinguish code, tests, docs, localization, scripts, generated/ignored paths.

Must produce:

```text
path
tracked/untracked
batch
purpose
owner
risk
commit slice
rollback option
validation coverage
owner decision needed
```

### 3.4 WarningLedgerAgent

Purpose:

- Recount all 89 warnings using clean build.
- Classify by file and warning code.
- Mark Sts1Events staging-only debt vs formal-feature blocker.

Must produce no `TBD` rows.

### 3.5 RitsuLibRuntimeAgent

Purpose:

- Keep RitsuLib status truthful.
- Confirm whether current state is:
  - compile-only
  - compile/manifest attempted
  - runtime-validated
  - release-ready
  - rollback-needed

Must list missing publish/package/runtime/handoff evidence.

### 3.6 Sts1EventsGovernanceAgent

Purpose:

- Maintain or revise the current recommendation:
  - formal
  - staging-only
  - remove/exclude

Default recommendation remains `staging-only` unless owner explicitly approves formalization.

Must evaluate:

```text
89 nullable warnings
runtime proof absence
Off=0 proof absence
CanaryOnly=4 proof absence
localization debt
event image/resource debt
combat-event blocked rows
release-claim risk
```

### 3.7 DebugDecisionAgent

Purpose:

- Keep Debug as `accept-scaffold` unless feature-complete evidence exists.
- Review default-off behavior, Warn policy, settings exposure, behavioral tests, side-effect risk.

### 3.8 PatchInventoryAgent

Purpose:

- Confirm patch inventory truth:
  - 25 migrated `IPatchMethod`
  - 142 raw `HarmonyPatch` declarations
  - 167 tracked patch units
- Confirm no double-patching claim is hidden or ambiguous.
- Confirm `generate-patch-inventory.ps1 -Check` passes.

### 3.9 TestChangeReviewAgent

Purpose:

- Confirm test changes did not weaken coverage.
- Review guard changes around source manifests, patch inventory, Sts1Events mode selector, RitsuLib migration, and runtime gates.

### 3.10 DocsTruthAgent

Purpose:

- Remove or correct:
  - Done / complete overclaims
  - runtime verified claims
  - release-ready claims
  - Sts1Events unrelated/untracked claims
  - format/test/build claims not backed by current exit codes
  - stale warning/test/dirty counts

### 3.11 CommitSliceAgent

Purpose:

- Prepare commit plan only.
- Do not commit.
- Split work into owner-reviewable slices.

Possible slices:

```text
Slice A: runtime hard-blocker docs and status
Slice B: warning ledger and Sts1Events staging debt
Slice C: patch inventory wording and guards
Slice D: RitsuLib runtime truth alignment
Slice E: owner-review artifacts
Slice F: localization backlog/status
Slice G: tests/guards that support current governance
```

## 4. Monthly Plan

### Week 1 — Runtime Hard-Blocker Closure + Owner-Review Packet

Goal:

```text
Close or formally document the runtime hard blocker, then produce an owner-review packet for the 49 dirty entries.
```

Required outputs:

```text
docs/goals/revision-j-final-report.md
docs/goals/revision-j-owner-review-packet.md
docs/goals/revision-j-runtime-hard-blocker.md
docs/goals/revision-j-runtime-smoke-plan.md
docs/goals/revision-j-dirty-ledger.md
docs/goals/revision-j-commit-slices.md
updated docs/goals/overnight-run-status.md
updated docs/goals/overnight-run-ledger.md
updated docs/goals/warning-ledger.md
updated harness/TASK_STATUS.md
updated harness/TASK_FOCUS_PACK.md
```

Required terminal validations:

```powershell
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet build .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

If runtime dependencies are installed, additionally run:

```powershell
.\scripts\spire-plus-live-session.ps1 -Mode Prepare -MoveOtherMods -MoveCurrentRuns
# launch/smoke according to generated instructions
.\scripts\audit-godot-log.ps1 -LogPath <active-godot-log>
```

Week 1 completion requires either:

```text
A. runtime smoke prerequisites pass and runtime smoke evidence is collected, or
B. runtime hard blocker is documented with exact missing path/files and owner action.
```

### Week 2 — Governance Decisions

Goal:

```text
Finalize Sts1Events, Debug, and RitsuLib governance status.
```

Default decisions:

```text
Sts1Events: staging-only
Debug: accept-scaffold
RitsuLib: compile/manifest attempted; runtime unverified
```

Promotion requirements:

Sts1Events can become formal only after:

```text
89 warnings fixed or explicitly accepted
ZHS localization debt resolved
Off=0 runtime proof exists
CanaryOnly=4 runtime proof exists
event images/resources resolved
blocked combat rows resolved
manual runtime plan exists
```

Debug can become feature-complete only after:

```text
settings exposure exists
dedicated behavioral tests exist
Warn policy is documented
LogPreview dead-code decision is resolved
side-effect audit exists
```

RitsuLib can become runtime-validated only after:

```text
STS2-RitsuLib runtime installed
loader smoke passes
godot.log audit clean
publish/package/handoff docs align if manifest dependency remains
```

### Week 3 — RitsuLib Runtime / Packaging Truth

Goal:

```text
Either validate RitsuLib as a true runtime dependency or downgrade docs to compile/manifest attempted.
```

If validating runtime, run:

```powershell
dotnet publish .\EZMicroBalance.csproj
.\scripts\package-spire-plus.ps1
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

Also required:

```text
BaseLib + STS2-RitsuLib + Spire Plus loader smoke
godot.log audit
tester handoff dependency instructions
package/hash/version docs
runtime fallback or install-enforced hard-dependency decision
```

### Week 4 — Longhaul Audit Recovery

Only resume after:

```text
owner-review packet accepted
commit/rollback decisions made
terminal validation still green
runtime hard blocker closed or explicitly owner-deferred
Sts1Events/Debug/RitsuLib governance decisions recorded
no stale truth blockers remain
```

First ten files:

```text
1. EZMicroBalance.csproj
2. EZMicroBalance.json
3. EZMicroBalanceCode/MainFile.cs
4. EZMicroBalanceCode/Core/Integrations/RitsuLib/RitsuLibBootstrap.cs
5. EZMicroBalanceCode/Sts1Events/Sts1EventFeatureGate.cs
6. EZMicroBalanceCode/Sts1Events/Sts1EventRegistrationService.cs
7. tests/EZMicroBalance.Tests/RitsuLibMigrationGuardTests.cs
8. tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.cs
9. scripts/generate-patch-inventory.ps1
10. docs/integrations/ritsulib.md
```

Every longhaul round must end with one of:

```text
fixed
skipped
blocked
```

## 5. Overnight Run Prompt

Use this prompt for the next assistant run.

```text
Enter M4 Revision J overnight runtime hard-blocker closure and owner-review run.

Current status is NOT COMPLETE.

Known latest status:
- HEAD: 87820303 (main, origin/main) sprint 1
- Worktree: 49 dirty entries, 0 unclassified
- Terminal validation passed
- Build: 0 errors, 89 Sts1Events nullable warnings
- Tests: 464 passed / 0 failed / 21 skipped
- Patch inventory check passed
- Runtime smoke blocked because E:\Steam\steamapps\common\Slay the Spire 2\mods\STS2-RitsuLib is missing
- fresh godot.log exists but has 11 Godot ERROR hits
- no Off=0 runtime proof
- no CanaryOnly=4 runtime proof
- Release-ready: no
- Runtime-ready/live-ready: no

Do not:
- commit
- push
- stash / stash drop
- checkout branch
- reset / restore
- broad clean
- continue PR6 Batch4c, PR6 Batch5, or PR7
- expand debug
- formalize Sts1Events
- resume longhaul audit
- claim runtime verified or release-ready without runtime evidence

You cannot stop until one of these terminal conditions is met:

A. Ready-to-owner-review packet complete:
- runtime hard blocker either closed or precisely documented
- current dirty files fully reconciled
- untracked files have decisions
- terminal validation commands exit 0
- warning-ledger has no TBD
- 89 warnings classified by file/code/owner
- Sts1Events formal/staging/remove recommendation recorded
- Debug accept-scaffold/feature-complete/rollback recommendation recorded
- RitsuLib attempted/runtime-validated/release-ready/rollback status recorded
- patch inventory raw/migrated/tracked unit relationship explained
- test changes reviewed
- localization backlog recorded
- commit slices complete
- no unauthorized commit

B. Hard blocker:
- exact command/file/path
- why current worktree cannot resolve it
- rollback/staging/owner-decision options
- exact owner action required

First read:
AGENTS.md
PROJECT_STATE.md
docs/README.md
docs/test-ready-development-goal.md
docs/worktree-cleanup-audit.md
docs/patch-inventory.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/overnight-diff-ledger.md
docs/goals/warning-ledger.md
docs/integrations/ritsulib.md
docs/migration.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

Use subagents before modifying files:
1. RuntimeDependencyAgent
2. RuntimeSmokeAgent
3. DirtyStateReconciliationAgent
4. WarningLedgerAgent
5. RitsuLibRuntimeAgent
6. Sts1EventsGovernanceAgent
7. DebugDecisionAgent
8. PatchInventoryAgent
9. TestChangeReviewAgent
10. DocsTruthAgent
11. CommitSliceAgent

RuntimeDependencyAgent must first verify:
- Game root
- BaseLib install
- STS2-RitsuLib install
- EZMicroBalance install
- runtime DLL presence
- active godot.log availability or absence

ValidationReplay must run:
dotnet clean .\EZMicroBalance.csproj
dotnet build .\EZMicroBalance.csproj
dotnet build .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj
dotnet test .\tests\EZMicroBalance.Tests\EZMicroBalance.Tests.csproj --no-build
dotnet format .\EZMicroBalance.csproj --verify-no-changes
git diff --check
.\scripts\generate-patch-inventory.ps1 -Check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified

Create or update:
docs/goals/revision-j-final-report.md
docs/goals/revision-j-owner-review-packet.md
docs/goals/revision-j-runtime-hard-blocker.md
docs/goals/revision-j-runtime-smoke-plan.md
docs/goals/revision-j-dirty-ledger.md
docs/goals/revision-j-commit-slices.md
docs/goals/overnight-run-status.md
docs/goals/overnight-run-ledger.md
docs/goals/warning-ledger.md
harness/TASK_STATUS.md
harness/TASK_FOCUS_PACK.md

Final report must be either:
Complete: ready-to-owner-review packet complete.
Not complete: exact hard blocker encountered.

Do not write Complete merely because build/tests pass.
Do not write runtime-ready without loader evidence.
Do not write release-ready without publish/package/runtime/handoff evidence.
```
