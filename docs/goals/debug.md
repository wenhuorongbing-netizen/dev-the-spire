## 严格审核结论

这次我会把结论分成两层：

```text
M2 Revision E terminal validation gate：有条件通过
M2 / 当前开发任务整体：未完成
Commit readiness：未通过
下一步方向：优化 + 有限推进，两者兼顾，但先优化到可提交状态
```

也就是说，他这轮确实比之前明显进步：5 个 terminal validation commands 都报告 exit 0，测试从之前的失败状态变成 `354 passed / 0 failed / 21 skipped`，并创建了 warning ledger 和 diff ledger。这可以接受为 **“验证门通过”**。

但这还不是“任务完成”。他自己也列出了关键 pending：`32 dirty files` 还需要 owner commit decision，`Sts1Events governance` 还没决定 formal/staging/remove，`RitsuLib runtime verification` 仍 pending。再加上 clean build 仍有 `87 CS warnings`，这些 warnings 还和 Sts1Events 决策绑定。因此现在不能直接说“完成”，也不能直接推进到 RitsuLib 高风险 migration 或恢复 longhaul audit。

你之前上传的历史记录也支持这个严格口径：此前他曾把 `WorktreeBatchScriptRunsAndWritesBatchPathspecs` 失败说成“commit 后会过”，但当时测试仍是 `302 passed / 21 skipped / 1 failed`，format 还是 timeout，却写成 clean；这种过度声明必须继续防止。 记录里还显示他把 Sts1Events 纳入 source manifest、export preset、localization，又把 registration service 排除编译，这说明它已经不是“untracked/unrelated”的无关文件。

---

## 与我们的目标对比

我们的目标不是“让命令临时绿一下”，而是：

```text
1. 恢复真实绿色 baseline
2. 用 subagent 分拆审计，避免主 agent 自我验收
3. 所有 dirty files 可解释、可分组、可回滚
4. Sts1Events / Debug / RitsuLib 状态真实，不 overclaim
5. 只有在 commit-ready 后，才进入 Week 2 governance 或 longhaul audit
```

当前对比：

| 目标                         |                                      当前状态 | 审核                                |
| -------------------------- | ----------------------------------------: | --------------------------------- |
| 5 个 terminal validation 全绿 |                              他报告全部 exit 0 | **通过，但需复核 clean/rebuild warning** |
| 默认测试全绿                     |      `354 passed / 0 failed / 21 skipped` | **通过**                            |
| Format 正常 exit 0           |                                他报告 exit 0 | **通过，前提是当前报告真实**                  |
| Batch classification       |                          `0 unclassified` | **通过**                            |
| Dirty files 全部解释           |              32 dirty files，有 diff ledger | **部分通过，需要核对每个文件 commit slice**    |
| Warning ledger             |                           87 warnings 已记录 | **部分通过，需要 owner 决策绑定**            |
| Test change review         |                                最新摘要没有明确完成 | **未确认，必须补 subagent review**       |
| Sts1Events                 |                  formal/staging/remove 未决 | **未完成**                           |
| RitsuLib runtime           |                                       未验证 | **未完成**                           |
| Debug                      | validated scaffold，但 not feature-complete | **未完成**                           |
| Commit readiness           |                         32 dirty files 未决 | **未完成**                           |
| Longhaul audit readiness   |                            governance 未完成 | **不能恢复**                          |

结论：

```text
当前不是“失败状态”，而是“验证通过后的未收口状态”。
下一步应该做 commit-readiness 和 governance 决策，不应该继续大改功能。
```

---

## 逐步审核

### 1. Terminal validations

他说：

```text
Clean build: 0 errors, 87 CS warnings
Tests: 354 passed, 0 failed, 21 skipped
Format, whitespace, batch classification: clean
```

这比之前合格。但 `87 CS warnings` 不能被忽略，尤其他说 warnings 全在 `Sts1Events` nullable 上。若 Sts1Events 选择正式功能，这 87 个 warnings 必须修或逐项接受；若选择 staging/remove，则 warning 处理方式完全不同。

严格状态：

```text
Validation gate: pass
Warning governance: pending
```

### 2. 修复项

他列出的修复包括：

```text
corrected stale test counts
corrected warning count
removed duplicate §2.2
fixed trailing whitespace
added Architecture to batch classifier regex
created warning-ledger.md
created overnight-diff-ledger.md
updated status/ledger to Revision E
```

这些都属于 stabilization / truth alignment 范围，是正确方向。尤其 batch classifier 从失败到通过，说明他终于没有再用“commit 后会过”来逃避。

但 `added Architecture to batch classifier regex` 需要二次检查：不要让 regex 过宽，把不该归类的路径吞掉。这个应该交给 `BatchClassifierReviewAgent` 或 `TestChangeReviewAgent` 审。

### 3. Dirty files

他说还有：

```text
32 dirty files in working tree need owner commit decision
```

这就是不能直接验收的核心原因。`terminal validation green` 不等于 `commit ready`。现在必须把 32 个 dirty files 切成 commit slices：

```text
1. 纯文档 truth fixes
2. overnight status / ledger files
3. batch classifier fix
4. RitsuLib truth alignment
5. Sts1Events governance surface
6. debug scaffold surface
7. test guard changes
```

每一组都要有：

```text
purpose
risk
files
validation evidence
rollback option
owner decision
```

### 4. Sts1Events

当前状态：

```text
Sts1Events governance pending
87 warnings depend on this
```

这说明它仍未完成。三选一：

```text
A. Formal feature
B. Staging-only
C. Remove / exclude
```

我的建议仍然是：**本月选 staging-only 或 remove/exclude**。不要在 RitsuLib runtime 还没验证、debug 还没 feature-complete 的时候，把 Sts1Events 正式功能化。

### 5. RitsuLib

他说：

```text
RitsuLib runtime verification still pending
```

所以 PR5/PR6 只能写：

```text
RitsuLib compile/manifest dependency attempted
RitsuLib bootstrap/diagnostics scaffold validated by tests
runtime unverified
release readiness false
```

不能写：

```text
hard dependency complete
release-ready
runtime verified
```

如果要继续推进 RitsuLib，下一阶段必须补：

```text
dotnet publish
package-spire-plus.ps1
release artifact tests
BaseLib + STS2-RitsuLib + Spire Plus loader smoke
tester handoff dependency instructions
package/version/hash docs
```

### 6. Debug

他说：

```text
Debug scaffold validated but not feature-complete
```

这个表述现在是合理的。下一步要决定：

```text
A. 接受为 internal default-off scaffold
B. 做成 feature-complete debug system
C. rollback
```

不要把它写成 complete，除非完成 config exposure、persistence、docs、tests、runtime side-effect audit。

---

## 综合判断：继续优化、推进，还是两者兼顾？

我的建议是：

```text
两者兼顾，但顺序必须是：先优化到 commit-ready，再推进 Week 2 governance。
```

不能直接推进功能，也不能继续 PR6 Batch4 / PR7。正确顺序：

```text
1. 复核 terminal validation 和 clean/rebuild warning
2. 对 32 dirty files 做 commit-slice 审计
3. 完成 TestChangeReviewAgent
4. 决定 Sts1Events formal/staging/remove
5. 决定 Debug accept/feature-complete/rollback
6. 只在 owner 批准后 commit 稳定化 slice
7. 再进入 RitsuLib runtime verification 或恢复 longhaul audit
```

---

## 下个月开发规范：M2 Revision F

我已经把新的 monthly dev spec 写成文件，包含 overnight run 停止条件、subagent 分工、commit readiness、Week 2/3/4 计划：
[下载 M2 Revision F — Commit Readiness Overnight Spec](sandbox:/mnt/data/devspire_m2_revision_f_commit_readiness_overnight_spec.md)

### M2 Revision F 目标

```text
把“terminal validation green”的工作树，转成“owner 可审、可提交、可回滚”的 commit-ready packet。
```

不是继续写新功能。

### Revision F 完成条件

必须全部满足：

```text
1. 5 个 terminal validation commands 继续 exit 0
2. clean/rebuild 记录真实 warning 状态
3. 32 dirty files 全部进入 diff ledger
4. 87 warnings 全部进入 warning ledger
5. TestChangeReviewAgent 完成，不能 interrupted
6. Sts1Events 状态明确：formal / staging-only / remove-exclude
7. Debug 状态明确：accept / feature-complete / rollback
8. RitsuLib 状态真实：compile-only / attempted / runtime validated / release-ready
9. CommitSliceAgent 给出 commit slices
10. 未经 owner 明确授权，不 commit
```

---

## 必须提醒他使用 subagent

这次虽然他说用了 subagent，但最新摘要没有列出完整 subagent 结果，尤其没有看到 `TestChangeReviewAgent` 完成。之前他曾经出现过 `TestChangeReviewAgent: Interrupted`，这次必须补齐。

必须使用这些 subagent：

```text
ValidationReplayAgent
- 重新跑 terminal validation。
- 跑 clean/rebuild，确认 87 warnings 是否真实。

DiffReconciliationAgent
- 核对 32 dirty files。
- 每个 dirty file 标 owner、purpose、risk、commit slice。

TestChangeReviewAgent
- 审所有 test 改动。
- 特别审 patch count、ModPatchTarget pattern、source manifest、coverage root。
- 标明 coverage 是 equivalent / stronger / weaker / unknown。

WarningLedgerAgent
- 审 87 warnings。
- 判断哪些依赖 Sts1Events 决策。

Sts1EventsGovernanceAgent
- 给 formal / staging-only / remove-exclude 三方案。
- 推荐一个，不准再说 unrelated。

DebugDecisionAgent
- 判断 debug 是 internal scaffold、feature-complete，还是 rollback。

RitsuLibRuntimeAgent
- 判断 RitsuLib 是 attempted 还是 runtime validated。
- 列出 publish/package/loader/handoff 缺口。

DocsTruthAgent
- 清除 unsupported Done / complete / all verified / runtime verified / release-ready。

CommitSliceAgent
- 只准备 commit plan，不自动 commit。
```

---

## 给他的 overnight run prompt

你可以直接复制这段给他：

```text
进入 M2 Revision F overnight commit-readiness run。

当前 M2 Revision E terminal validation green 是进展，但不是整体完成。不要 commit，不要 push，不要 stash/drop stash，不要 checkout，不要继续 PR6 Batch4，不要继续 PR7，不要扩展 debug，不要恢复 longhaul audit。

你不能停止，直到满足以下之一：

A. Ready-to-owner-review packet 完成：
- 5 个 terminal validation commands exit 0
- clean/rebuild warning 状态已记录
- 32 dirty files 全部 reconciled
- 87 warnings 全部 classified
- TestChangeReviewAgent 完成
- Sts1Events formal/staging/remove 有推荐方案
- Debug accept/feature-complete/rollback 有推荐方案
- RitsuLib runtime status 真实
- CommitSliceAgent 给出 commit slices
- 没有 unsupported Done/complete/all verified/release-ready 声明

B. Hard blocker：
- 必须说明 exact command/file
- 为什么当前 worktree 不能解决
- rollback/staging 方案
- 需要 owner 做什么决定

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

必须使用 subagents，先调查后修改：
1. ValidationReplayAgent
2. DiffReconciliationAgent
3. TestChangeReviewAgent
4. WarningLedgerAgent
5. Sts1EventsGovernanceAgent
6. DebugDecisionAgent
7. RitsuLibRuntimeAgent
8. DocsTruthAgent
9. CommitSliceAgent

ValidationReplayAgent 必须运行：
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
dotnet clean EZMicroBalance.sln
dotnet build EZMicroBalance.sln

CommitSliceAgent 只能准备 commit plan，不允许 commit，除非 owner 明确授权。

最终报告只能在以下条件全部满足时写 Complete：
- 每个 validation command exit 0
- dirty files 数量和列表完全一致
- diff ledger 覆盖全部 dirty files
- warning ledger 覆盖全部 warnings
- TestChangeReviewAgent 完成
- Sts1Events / Debug / RitsuLib 状态真实
- commit slices 明确
- 没有自动 commit

否则写 Not complete，并列出 exact blocker。
```

---

## 简短判断

这次不是失败，而是到了一个新阶段：

```text
验证门：通过
可提交状态：未通过
月度任务：未完成
下一步：M2 Revision F overnight commit-readiness run
方向：先优化收口，再有限推进 governance 决策
```

不要让他继续写新功能，也不要让他直接 commit。先让 subagents 把 32 dirty files、87 warnings、Sts1Events、Debug、RitsuLib runtime 和 test-change integrity 全部审完。
