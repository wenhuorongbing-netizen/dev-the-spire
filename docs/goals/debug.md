# DevSpire M2 Revision A — Strict Stabilization, Subagent-Gated Review, and Longhaul Recovery

Date: 2026-05-28
Scope: `dev-the-spire` / `Spire Plus` / `EZMicroBalance` technical manifest id
Status: **Not complete until default validation is fully green.**

## 0. Current audit verdict

The latest reported state is progress but not completion:

- Build: reported pass with `0 errors`, but `45–47 warnings` remain and need a warning ledger.
- Tests: `302 passed`, `21 skipped`, `1 failed`.
- Remaining failing test: `EngineeringGovernanceGuardTests.WorktreeBatchScriptRunsAndWritesBatchPathspecs`.
- Format: reported as clean, but the command output shows a timeout; timeout is not a clean pass.
- Debug/RitsuLib/Sts1Events work is not accepted while one default test fails.
- No commit should be made until the remaining failing test is fixed or formally blocked with owner approval.

Strict status: **Not complete / Not accepted / Do not continue PR6 Batch4, PR7, or longhaul audit yet.**

## 1. Non-negotiable rules

1. Do not commit to make `WorktreeBatchScriptRunsAndWritesBatchPathspecs` pass. A hygiene test must be understood and fixed before commit.
2. Do not continue RitsuLib patch migration while default tests fail.
3. Do not continue high-risk run/map/reward/save/multiplayer patch work.
4. Do not claim `format clean` unless `dotnet format ... --verify-no-changes` exits with code 0 and no timeout.
5. Do not claim debug complete while default tests fail.
6. Do not call Sts1Events unrelated. It is now treated as current project surface if tracked, in manifest, in export preset, or in localization.
7. Do not weaken tests merely to match a migration pattern. Any test update must preserve or improve source evidence coverage.
8. Keep `EZMicroBalance` as the technical manifest id, package folder, DLL/PCK identity, saved-field namespace, and compatibility surface.

## 2. Immediate gate: remaining failing test

### Required investigation

Run and capture the full output, not only the summary:

```powershell
dotnet test EZMicroBalance.sln --no-build --filter "FullyQualifiedName~WorktreeBatchScriptRunsAndWritesBatchPathspecs"
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
.\scripts\report-worktree-batches.ps1 -PathspecDirectory .tools\worktree-batches\current
```

Classify the failure:

- unclassified path
- dirty-state rule mismatch
- pathspec output failure
- script bug
- test expectation stale
- real governance violation

Acceptance: the failing test must pass without relying on a commit as the fix.

## 3. Subagent operating model

Use subagents for every non-trivial stabilization task. The main agent should coordinate and make final decisions; subagents should gather evidence and propose minimal fixes.

### 3.1 BatchScriptAgent

Purpose: resolve the remaining `WorktreeBatchScriptRunsAndWritesBatchPathspecs` failure.

Prompt:

```text
You are BatchScriptAgent. Do not edit files yet. Inspect the failing test and the script output.
Run:
- dotnet test EZMicroBalance.sln --no-build --filter "FullyQualifiedName~WorktreeBatchScriptRunsAndWritesBatchPathspecs"
- .\scripts\report-worktree-batches.ps1 -FailOnUnclassified
- .\scripts\report-worktree-batches.ps1 -PathspecDirectory .tools\worktree-batches\current
Return:
1. exact failure reason
2. unclassified paths, if any
3. whether the script expects a clean worktree or can classify dirty files
4. smallest fix
5. files that would need editing
Do not say "commit will fix it" unless you prove the test is explicitly a post-commit-only check and the repo policy allows that. Prefer fixing classifier/test expectations before commit.
```

### 3.2 Sts1EventsGovernanceAgent

Purpose: determine whether Sts1Events is formal, staging, or should be removed.

Prompt:

```text
You are Sts1EventsGovernanceAgent. Do not edit files yet.
Audit Sts1Events across:
- EZMicroBalanceCode/Sts1Events/**
- EZMicroBalance.csproj compile glob and exclusions
- export_presets.cfg
- localization eng/zhs files
- ActiveSourceManifestGuardTests
- docs/PROJECT_MAP.md and feature docs
Return a table:
1. path
2. tracked or untracked
3. compiled or excluded
4. exported or not
5. guarded by tests or not
6. release/package implication
Then recommend exactly one state: formal feature, staging-only, or remove/exclude.
```

### 3.3 TestChangeReviewAgent

Purpose: review whether the recent test updates preserved coverage.

Prompt:

```text
You are TestChangeReviewAgent. Do not edit files yet.
Review changes to tests, especially AncientBehaviorGuardTests and ActiveSourceManifestGuardTests.
Check whether converting expectations to ModPatchTarget patterns preserved source evidence coverage or weakened tests.
For each test change, return:
- old assertion intent
- new assertion intent
- whether coverage is equivalent
- extra source evidence needed
- targeted test command
```

### 3.4 DebugConfigAgent

Purpose: decide whether debug scaffold is acceptable.

Prompt:

```text
You are DebugConfigAgent. Do not edit files yet.
Audit SpirePlusDebug and EnableDebugLogs.
Check:
- default off
- actual config UI binding or explicit internal-only status
- persistence/load behavior
- no initialization order change
- no RNG/save/load/multiplayer side effect
- no release claim overreach
Return acceptance gaps and minimal fix list.
```

### 3.5 RitsuLibRuntimeAgent

Purpose: truth-check the RitsuLib hard dependency.

Prompt:

```text
You are RitsuLibRuntimeAgent. Do not edit files yet.
Audit:
- EZMicroBalance.csproj PackageReference
- EZMicroBalance.json dependencies
- docs/integrations/ritsulib.md
- docs/migration.md
- package/publish/tester handoff docs
Return whether the state is:
A. compile-only staging
B. hard runtime dependency attempted but unverified
C. hard dependency release-ready
List missing runtime/package/loader evidence for B -> C.
```

### 3.6 DocsTruthAgent

Purpose: remove or flag overclaims.

Prompt:

```text
You are DocsTruthAgent. Do not edit files yet.
Search current docs and harness files for overclaims:
- Done
- complete
- all verified
- tests pass
- format clean
- Sts1Events unrelated
- commit next
Return exact file/line, why it is unsupported, and the replacement wording.
```

### 3.7 WarningLedgerAgent

Purpose: classify the 45–47 build warnings.

Prompt:

```text
You are WarningLedgerAgent. Do not edit files yet.
Run build with full warning output.
Group warnings by code/file/owner.
Classify each as:
- new from current work
- pre-existing
- generated/local-only
- actionable
- acceptable with documented reason
Return a warning ledger and which warnings must be fixed before commit.
```

## 4. Monthly plan

### Week 1 — Stabilization gate

Goal: default validation green.

Deliverables:

- Full forensic state: branch, HEAD, stash, diff, dirty files, untracked files.
- `WorktreeBatchScriptRunsAndWritesBatchPathspecs` fixed or formally blocked.
- Format command rerun with confirmed exit code 0.
- Warning ledger created.
- No new migration or feature work.

Acceptance commands:

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

### Week 2 — Sts1Events and debug decision

Goal: decide whether Sts1Events and debug are accepted surfaces.

Deliverables:

- Sts1Events formal/staging/remove decision.
- If formal: feature README, source research, localization/export/package/test plan.
- If staging: remove from active export/release surface or explicitly exclude with docs/guards.
- If remove: remove source/resource/localization/export/test entries.
- Debug scaffold accepted or reverted.
- Debug config behavior documented and tested.

Acceptance:

- Default validation green.
- No unsupported `complete` wording.

### Week 3 — RitsuLib truth alignment

Goal: make RitsuLib state truthful.

Options:

- If hard dependency is retained: complete package/publish/runtime/handoff/loader evidence plan.
- If not ready: downgrade docs to `hard dependency attempted; runtime unverified` or `staging`.

Required if retained as hard dependency:

```powershell
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

Also required: tester install docs for BaseLib + STS2-RitsuLib + Spire Plus and a fresh loader-smoke plan.

### Week 4 — Resume one-file longhaul audit

Goal: resume the user's original one-file-at-a-time audit.

Rules:

- One current file only.
- No batch migration.
- No high-risk patch conversion.
- Every file ends as fixed/skipped/blocked.

Initial queue:

1. `EZMicroBalance.csproj`
2. `EZMicroBalance.json`
3. `EZMicroBalanceCode/MainFile.cs`
4. `EZMicroBalanceCode/Core/Integrations/RitsuLib/RitsuLibBootstrap.cs`
5. `EZMicroBalanceCode/Diagnostics/SpirePlusDebug.cs`
6. `EZMicroBalanceCode/Config/SpirePlusModConfig.cs`
7. `tests/EZMicroBalance.Tests/ActiveSourceManifestGuardTests.cs`
8. `tests/EZMicroBalance.Tests/AncientBehaviorGuardTests.cs`
9. `docs/migration.md`
10. `docs/integrations/ritsulib.md`

## 5. Next prompt to send

```text
Current status is NOT COMPLETE. Do not commit, do not continue RitsuLib patch migration, do not continue PR7, and do not resume longhaul audit yet.

Use subagents first:
1. BatchScriptAgent: diagnose the remaining WorktreeBatchScriptRunsAndWritesBatchPathspecs failure with full script output. Do not say commit fixes it unless proven by test policy.
2. Sts1EventsGovernanceAgent: decide whether Sts1Events is formal, staging, or remove/exclude. It is not unrelated if tracked/manifest/export/localization entries exist.
3. TestChangeReviewAgent: verify that recent test changes to ModPatchTarget patterns did not weaken coverage.
4. DebugConfigAgent: verify EnableDebugLogs is default-off, actually configurable or documented internal-only, and has no runtime side effects.
5. RitsuLibRuntimeAgent: classify RitsuLib state as compile-only, hard dependency attempted, or release-ready hard dependency.
6. DocsTruthAgent: identify and replace unsupported Done/complete/all verified/format clean/Sts1Events unrelated claims.
7. WarningLedgerAgent: classify the 45–47 build warnings.

After subagent reports, only fix Week 1 stabilization. The remaining default validation must pass:
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
- git diff --check
- .\scripts\report-worktree-batches.ps1 -FailOnUnclassified

If any command fails or times out, final status is Not complete.
```
## 严格审核结论：**仍然未完成，不能验收，不能 commit**

这轮有明显进展：他把失败从多项压到 **1 个失败**，并且把 Sts1Events 从“无关/untracked”的说法修正成了“tracked、在 compile glob、在 manifest、在 export preset、localization 有 BOM”。但严格验收不是看“还剩一个小问题”，而是看默认验证是否全绿。当前报告仍然是：

```text
Build: 0 errors, 47 warnings
Tests: 302 passed, 21 skipped, 1 failed
Remaining failure: WorktreeBatchScriptRunsAndWritesBatchPathspecs
Format: timeout, 不能算 clean
No commits made
```

日志里明确显示 `dotnet test` 仍然失败 1 项，而且 `dotnet format ... --verify-no-changes --no-restore` 是 timeout；他说 “Format: Clean” 不成立。

我已经把下一步 monthly dev spec 写成文件：
[下载 M2 Revision A Monthly Dev Spec — Subagent Stabilization](sandbox:/mnt/data/devspire_m2_revision_a_monthly_dev_spec_subagents.md)

---

## 1. 当前完成状态判断

| 项目               |                                                     他当前说法 |                                                   严格审核 |
| ---------------- | --------------------------------------------------------: | -----------------------------------------------------: |
| Build            |                                     0 errors, 47 warnings |      **部分通过**；build 过了，但 47 warnings 需要 warning ledger |
| Tests            |                          302 passed, 21 skipped, 1 failed |                              **不通过**；只要 1 failed，就不能验收 |
| Format           |                                                     Clean |                           **不成立**；命令 timeout 不能算 clean |
| Batch script     |                                            “needs commit” |                    **不接受**；不能用 commit 来绕过 hygiene test |
| Sts1Events       | tracked / compile glob / manifest / export / localization |                      **进入当前项目 surface**；不能再说 unrelated |
| Debug            |                                     implementation status |                              **partial**；未通过默认验证，不能算完成 |
| RitsuLib         |                                       PR5/PR6 Batch1 done | **只能算 partial / scaffold**；runtime hard dependency 未验证 |
| Longhaul audit   |                                                       未恢复 |                               **不能恢复**，先 stabilization |
| Commit readiness |                                             暗示 commit 后会过 |                                         **不允许 commit** |

结论：

```text
Not complete.
Do not commit.
Do not continue PR6 Batch4.
Do not continue PR7.
Do not resume longhaul one-file audit yet.
First fix the remaining failing test and resolve truth/status issues.
```

---

## 2. 最关键问题：`WorktreeBatchScriptRunsAndWritesBatchPathspecs`

他现在说：

```text
WorktreeBatchScriptRunsAndWritesBatchPathspecs — requires committing changes
```

这个说法不能直接接受。这个测试的名字和 repo 现有工作流都说明它是在检查 worktree batch 分类 / pathspec 输出 / dirty path governance。当前 repo 的 workflow 也要求在 staging 或 release handoff 前跑 `scripts/report-worktree-batches.ps1 -FailOnUnclassified`。

所以正确下一步不是 commit，而是先问：

```text
这个 test 到底为什么失败？
是 unclassified path？
是 dirty-state policy？
是 pathspec 输出目录问题？
是脚本不支持新文件？
是测试期望旧了？
```

只有定位清楚，才能修。如果失败原因是新增路径没有被 classifier 识别，那应该修 `scripts/report-worktree-batches.ps1` 或补路径分类；如果是 `.tools/worktree-batches/current` 输出未按预期生成，那应该修脚本或测试；如果它真的设计为“只有 clean commit 后才过”，那也必须有明确测试注释和项目政策支持，而不是口头说 “commit 后会过”。

**默认验收命令没全绿之前不许 commit。**

---

## 3. Sts1Events 状态：比之前好，但仍未治理完成

他现在承认：

```text
Sts1Events — tracked, in compile glob, with exclusion for API-incompatible file, in manifest, in export preset, localization has BOM
```

这比之前“untracked/unrelated”的说法进步很多。但这也意味着 Sts1Events 已经不再是无关草稿，而是当前项目 surface 的一部分。由于 `.csproj` 规则会编译 `EZMicroBalanceCode/**/*.cs`，只要 Sts1Events 在这个目录下，它就默认进入 build，除非有明确 exclusion。

当前风险是：他用了“排除 API-incompatible file”的方式让 build 过，但这不是最终治理。必须做一个明确决策：

```text
A. Formal feature:
   Sts1Events 是正式功能。
   必须补 feature README、source research、tests、localization/export/package policy、runtime plan。

B. Staging only:
   Sts1Events 暂时保留，但不能进入 active release/export surface。
   必须从 active export 或 package claim 中降级，或者清楚标注 excluded/staging。

C. Remove / exclude:
   当前月不做 Sts1Events。
   从 source manifest/export/localization/docs 中回滚或移出 active tree。
```

不能维持现在这种：

```text
一部分进入 manifest/export/localization，
一部分因为 API 不兼容被排除，
但文档没有正式 feature spec。
```

---

## 4. Debug implementation：仍然不能算完成

他做了 debug scaffold，包括：

```text
SpirePlusDebug.cs
SpirePlusModConfig.EnableDebugLogs
MainFile init debug logs
RitsuLibBootstrap debug logs
FeatureRegistry debug logs
Urda / Ascension initializer logs
```

但它还不能验收，原因是：

1. 默认测试仍然有 1 个失败。
2. `dotnet format` timeout，不能算 clean。
3. `EnableDebugLogs` 是否真正接入 Mod Settings、是否持久化、是否只作为 internal static 开关，还没有证明。
4. 没有证明 debug logging 不改变 init order、feature gate、RNG、save/load、multiplayer 行为。
5. 不能因为“debug 默认 false”就跳过 config / docs / guard 验收。

所以正确状态：

```text
Debug scaffold: partial, unvalidated.
```

不是：

```text
Debug complete.
```

---

## 5. RitsuLib 状态：仍然是 partial

RitsuLib 这块要拆开看：

### PR5：PackageReference + manifest dependency

方向可以接受，但不能叫 release-ready hard dependency。因为 manifest dependency 影响测试者安装和 runtime loader；项目自己的规则要求 manifest/package/tester handoff/version/hash 对齐，资源、manifest、package 变化后还要跑 publish/package/release artifact tests。 

所以状态应该写成：

```text
RitsuLib compile/manifest dependency attempted.
Runtime hard dependency unverified.
```

### PR6 Batch1：RitsuLibBootstrap

可以承认是：

```text
RitsuLib diagnostics/bootstrap scaffold
```

但不能说：

```text
RitsuLib patch migration complete
```

因为现有 patch 仍然主要依赖 Harmony。当前 patch inventory 本身就很大，风险也高：patch inventory 记录有大量 patch declaration，其中 high-risk patch 触及 run、room、save、lobby、multiplayer、lifecycle 等面。

### PR6 Batch2/3

不能写永久 N/A。应该写：

```text
Deferred: current month no content registration migration, no persistence migration.
```

### PR6 Batch4/5 / PR7

继续 blocked。不能现在做 63 patch migration，也不能碰 high-risk migration。

---

## 6. 他这轮做得好的地方

严格审核不是全盘否定，他这轮确实有进展：

```text
1. 把失败从多项压到 1 项。
2. 承认 Sts1Events 已经是 tracked / compile / manifest / export surface。
3. 补了部分 zhs localization key。
4. 修了部分 Simplified Chinese placeholder / Boss -> 首领问题。
5. 修了 BrightestFlame / Crossbow / OnPlayWrapper 的 ModPatchTarget 测试适配。
6. 没有 commit。
```

这些可以记为 progress。

但验收仍是：

```text
Progress acknowledged.
Completion rejected.
```

---

## 7. 现在必须提醒他使用 subagent

这轮暴露的问题是：主 agent 一边修 test、一边做判断、一边更新 docs，容易出现“剩一个失败但说完成”“format timeout 但说 clean”“commit 后会过”这种过度结论。下一轮必须拆 subagent。

你可以明确要求：

```text
每个非平凡子任务先用 subagent，只做调查和最小修复建议。
主 agent 不允许直接跳到完成结论。
```

### 必须使用的 subagent

```text
BatchScriptAgent
- 专门查 WorktreeBatchScriptRunsAndWritesBatchPathspecs 为什么失败。
- 必须给出完整脚本输出和具体失败原因。
- 不许只说 commit 后会过。

Sts1EventsGovernanceAgent
- 审 Sts1Events 是否正式功能、staging、还是 remove/exclude。
- 检查 compile/export/localization/source manifest/docs/test/package surface。

TestChangeReviewAgent
- 审最近把 test 改成 ModPatchTarget pattern 是否削弱覆盖。
- 每个 test change 都要说明 old assertion intent / new assertion intent。

DebugConfigAgent
- 审 EnableDebugLogs 是否默认 off、是否真正配置化、是否无副作用。

RitsuLibRuntimeAgent
- 审 RitsuLib 是 compile-only、hard dependency attempted，还是 release-ready hard dependency。
- 列出 package/runtime/loader/handoff 缺口。

DocsTruthAgent
- 找出所有 Done / complete / all verified / format clean / commit next / Sts1Events unrelated 这种过度声明。

WarningLedgerAgent
- 分类 45–47 个 warning，判断哪些必须修。
```

---

## 8. 下一步 Monthly Dev Spec：M2 Revision A

我建议下个月规格定义为：

```text
M2 Revision A: Strict Stabilization, Subagent-Gated Review, and Longhaul Recovery
周期：2026-05-28 至 2026-06-27
当前目标：恢复绿色 baseline，治理 Sts1Events/debug/RitsuLib 状态，再恢复 one-file longhaul audit。
```

下载版：
[devspire_m2_revision_a_monthly_dev_spec_subagents.md](sandbox:/mnt/data/devspire_m2_revision_a_monthly_dev_spec_subagents.md)

### Week 1：Stabilization gate

目标：

```text
默认验证全绿。
```

必须完成：

```text
1. BatchScriptAgent 定位剩余 failing test。
2. 修复 WorktreeBatchScriptRunsAndWritesBatchPathspecs，不允许靠 commit 规避。
3. dotnet format 重新跑，并确认 exit code 0。
4. 47 warnings 建 warning ledger。
5. 不做新 migration，不做 debug 扩展，不做 longhaul audit。
```

验收：

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

### Week 2：Sts1Events + debug decision

目标：

```text
决定 Sts1Events 和 debug scaffold 是否留在 active surface。
```

Sts1Events 三选一：

```text
formal feature
staging-only
remove/exclude
```

Debug 二选一：

```text
accept with config/docs/tests
rollback
```

### Week 3：RitsuLib truth alignment

目标：

```text
把 RitsuLib 状态写真实。
```

如果继续 hard dependency：

```text
dotnet publish
package-spire-plus.ps1
release artifact tests
runtime dependency install docs
fresh loader smoke plan
tester handoff dependency notes
```

如果暂不 hard dependency：

```text
改回 staging / attempted
不要写 Done
```

### Week 4：恢复 one-file longhaul audit

目标：

```text
回到你原本要求的一次只审一个文件。
```

前 10 个文件建议：

```text
EZMicroBalance.csproj
EZMicroBalance.json
EZMicroBalanceCode/MainFile.cs
EZMicroBalanceCode/Core/Integrations/RitsuLib/RitsuLibBootstrap.cs
EZMicroBalanceCode/Diagnostics/SpirePlusDebug.cs
EZMicroBalanceCode/Config/SpirePlusModConfig.cs
tests/EZMicroBalance.Tests/ActiveSourceManifestGuardTests.cs
tests/EZMicroBalance.Tests/AncientBehaviorGuardTests.cs
docs/migration.md
docs/integrations/ritsulib.md
```

---

## 9. 你可以直接发给他的 prompt

```text
当前工作仍然未完成。不要 commit，不要继续 RitsuLib patch migration，不要继续 PR7，不要恢复 longhaul audit，不要新增 debug。

你必须先使用 subagent 完成 stabilization：

1. BatchScriptAgent:
   运行并分析：
   dotnet test EZMicroBalance.sln --no-build --filter "FullyQualifiedName~WorktreeBatchScriptRunsAndWritesBatchPathspecs"
   .\scripts\report-worktree-batches.ps1 -FailOnUnclassified
   .\scripts\report-worktree-batches.ps1 -PathspecDirectory .tools\worktree-batches\current
   输出完整失败原因。不要说“commit 后会过”，除非能证明测试政策允许。

2. Sts1EventsGovernanceAgent:
   检查 Sts1Events 是否 tracked、compiled/excluded、exported、localized、in source manifest、in docs/tests。
   给 formal / staging / remove 三个方案，并推荐一个。

3. TestChangeReviewAgent:
   审查最近把测试改成 ModPatchTarget pattern 的改动，证明没有削弱 coverage。

4. DebugConfigAgent:
   审查 SpirePlusDebug / EnableDebugLogs 是否默认 off、是否真正配置化、是否无初始化/RNG/save/load/multiplayer 副作用。

5. RitsuLibRuntimeAgent:
   判断 RitsuLib 当前是 compile-only、hard dependency attempted，还是 release-ready hard dependency。
   列出缺失的 publish/package/runtime loader/handoff 证据。

6. DocsTruthAgent:
   找出并替换所有未被验证支持的 Done / complete / all verified / format clean / Sts1Events unrelated / commit next 表述。

7. WarningLedgerAgent:
   分类 45–47 个 build warnings，列出哪些必须修。

所有 subagent 先只调查，不改文件。主 agent 汇总后只处理 Week 1 stabilization。

最终验收必须全部通过：
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified

如果任何命令失败或 timeout，最终状态必须写 Not complete。
```

最简洁的判断就是：**他有进展，但没完成；不能 commit；下一步必须用 subagent 先解决剩余 batch script 失败、Sts1Events governance、debug/RitsuLib 过度声明和 warning ledger，然后才恢复 longhaul audit。**
