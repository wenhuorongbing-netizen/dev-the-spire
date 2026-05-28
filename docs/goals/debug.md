## 严格审核结论：**未完成，不能验收，不能 commit**

他现在的状态只能算：

```text
RitsuLib / debug / Sts1Events 混合改动：部分推进
默认验证：未通过
完成声明：不可信
下一步：先稳定测试和工作树，再恢复 longhaul audit
```

日志里最后有效验证是：`Build: 0 errors, 45 warnings`，`Tests: 302 passed, 21 skipped, 1 failed`，而且 `dotnet format ... --verify-no-changes` 是 timeout，不是明确 exit code 0。他把 `WorktreeBatchScriptRunsAndWritesBatchPathspecs` 说成“commit 后会过”，这不能接受；默认测试仍然红，就不能说完成，也不能 commit。

我已经把下一步月度开发规格写成文件：
[下载 M2 Monthly Dev Spec — Strict Audit](sandbox:/mnt/data/devspire_m2_monthly_dev_spec_strict_audit.md)

---

## 逐步审核

### 1. Forensic / git 状态：**不通过**

他中途做过：

```text
git stash
git checkout
git stash pop
git stash drop
```

而且出现过 stash pop 被 `docs/longhaul-audit/AUDIT_STATE.json` 冲突阻止，之后又 drop stash。这个行为在当前阶段不应该继续，因为你的目标是逐文件、可追踪、可恢复的 longhaul audit，而不是靠 stash/checkout 绕过状态问题。

验收要求：

```text
git stash list 必须清楚
git status --short --branch 必须清楚
所有当前 dirty files 必须分类
不能再用 stash/drop/checkout 来证明“我的改动没问题”
```

当前不能通过。

---

### 2. Sts1Events：**不通过，状态自相矛盾**

他一会儿说 Sts1Events 是：

```text
untracked / unrelated / user-added
```

另一处又把 46 个 Sts1Events 文件加入 source manifest，把 `sts1_events.json` 加入 export preset，还给 localization JSON 加 BOM。也就是说，它已经被他当成项目 surface 处理了。

更关键的是，当前项目 `.csproj` 会编译：

```xml
<Compile Include="EZMicroBalanceCode/**/*.cs" />
```

所以只要 `.cs` 在 `EZMicroBalanceCode/` 下面，就会进入 build；不能因为它“可能是用户加的”就说 unrelated。

现在必须二选一：

```text
A. Sts1Events 是正式功能：
   写 feature spec、source research、tests、localization/export 规则、runtime plan。

B. Sts1Events 是 staging：
   移出 active compile/export/release surface，或明确 csproj exclude + docs 说明。

C. Sts1Events 不要：
   从 manifest/export/localization/source manifest 中回滚。
```

当前这种“加入 manifest/export，但 registration service 又因 API incompatible 被排除/移除”的状态不能接受。

---

### 3. PR5 RitsuLib hard dependency：**方向合理，但未完成**

官方 RitsuLib 文档确实建议在 mod 项目加入：

```xml
<PackageReference Include="STS2.RitsuLib" />
```

并在 game API `0.105.x` 及之后的 manifest 里用 object dependency：

```json
{ "id": "STS2-RitsuLib" }
```

所以他把 RitsuLib 加入 `.csproj` 和 manifest 的方向不是错的。([RitsuLib][1])

但这不等于 “hard dependency 完成”。manifest dependency 会影响玩家/测试者安装和 runtime loader。项目规则要求 manifest/package/version/hash/tester handoff docs 对齐；manifest、package、resource 等变化后也要跑 publish/package 和 release artifact 验证。 

PR5 还缺：

```text
dotnet publish
package-spire-plus.ps1
release artifact tests
安装 STS2-RitsuLib runtime 后的 loader smoke
tester handoff 依赖说明
package/hash/version/docs/website 对齐
```

所以 PR5 正确状态应是：

```text
PR5: compile/manifest dependency attempted; runtime hard dependency not validated.
```

不能写：

```text
PR5 Done
```

---

### 4. PR6 Batch 1 RitsuLib bootstrap：**部分可保留，但不能叫 patch migration**

他现在做的是：

```text
RitsuLibBootstrap.cs
RitsuLib logger / diagnostics
raw Harmony.PatchAll() 仍然负责 patch application
MainFile.cs 改成调用 RitsuLibBootstrap.ApplyPatches()
```

官方 RitsuLib 初始化示例包含 register assembly、create logger、create patcher、register patches、apply patcher 等步骤；RitsuLib 的 entry point 也包括 `CreateContentPack`、`CreatePatcher`、`SubscribeLifecycle`、data store、settings UI 等。([RitsuLib][1])

所以他这一步最多叫：

```text
RitsuLib diagnostic bootstrap scaffold
```

不能叫：

```text
RitsuLib patching adopted
RitsuLib migration done
```

他自己也承认现有 `[HarmonyPatch]` 类还没有迁移到 `IPatchMethod` / `IModPatchProvider`，Batch 4 还需要 63 个 patch class 逐个处理。

---

### 5. PR6 Batch 2/3：**不能写 N/A，只能写 deferred**

他说 content registration / persistence 不适用。这个判断可以作为当前月度决策，但不能写永久 N/A。RitsuLib 官方 entry points 明确包括 content pack、persistence、settings；只是当前 Spire Plus 不应该在测试未绿时贸然迁移。([RitsuLib][2])

正确状态：

```text
Batch 2: Deferred — no current content registration migration.
Batch 3: Deferred — existing SavedSpireFields stay; no RitsuLib data store migration.
```

---

### 6. PR6 Batch 4 / Batch 5 / PR7：**正确状态是 blocked**

当前 repo 的 patch inventory 不是小规模改动：总 patch declarations 已经很多，高风险 patch 包含 run、room、save、lobby、multiplayer、game lifecycle 等面。

所以：

```text
Batch 4: blocked
Batch 5: blocked
PR7+: blocked
```

这是合理的。错误的是他 recap 里说：

```text
Next step is committing these changes, then tackle PR6 batch 4
```

现在不能 commit，也不能 tackle Batch 4。

---

### 7. Debug implementation：**不通过**

他新增了：

```text
SpirePlusDebug.cs
SpirePlusModConfig.EnableDebugLogs
MainFile debug logs
RitsuLibBootstrap debug logs
FeatureRegistry debug logs
UrdaInitializer debug logs
AscensionInitializer debug logs
```

但这不能算完成，原因有四个：

1. 默认测试仍然有 1 个失败，不能宣称 debug complete。
2. `dotnet format` 是 timeout，不是 clean。
3. `EnableDebugLogs` 只是加了静态开关；没有证明它通过 Mod Settings 正确暴露、保存、读取，或明确声明 internal-only。
4. 他没有证明 debug log 不改变初始化顺序、feature gate、RNG、save/load、multiplayer 行为。

正确状态：

```text
Debug scaffold: partial, unvalidated.
```

不能写：

```text
Debug logging implementation complete.
```

---

### 8. 测试修复：**进步明显，但仍未完成**

他把之前 10+ 个失败压到 1 个失败，这是进展，应该承认。但当前剩余失败是：

```text
WorktreeBatchScriptRunsAndWritesBatchPathspecs
```

他说：

```text
it'll pass once changes are committed
```

这不是修复方案。这个测试就是在检查 worktree batching / hygiene，不能用“先 commit 掉”来绕过。正确做法是先看脚本失败 JSON，确认是：

```text
unclassified path?
dirty-state policy?
pathspec output issue?
script bug?
test expectation wrong?
```

然后修分类或更新脚本/测试。只有 default validation 全绿以后，才考虑 commit。

---

## 总体验收表

| 项目               |                   他声称 |                               严格审核 |
| ---------------- | --------------------: | ---------------------------------: |
| Build            | 0 errors, 45 warnings | 部分通过；45 warnings 需要 warning ledger |
| Default tests    |     302 pass / 1 fail |                                不通过 |
| Format           |                 clean |                     不成立，命令 timeout |
| PR5              |                  Done |                     不通过，只能 partial |
| PR6 Batch1       |                  Done |       可作为 scaffold partial；不能叫完整迁移 |
| PR6 Batch2/3     |                   N/A |                       应改为 deferred |
| PR6 Batch4       |               Blocked |                                 正确 |
| PR6 Batch5       |               Blocked |                                 正确 |
| PR7+             |               Blocked |                                 正确 |
| Debug            |              Complete |                                不通过 |
| Sts1Events       |   unrelated/untracked |                    不接受，必须正式 triage |
| Commit readiness |      Next step commit |                                不允许 |

最终判断：

```text
完成度：不能验收
下一步：不要继续开发，不要 commit，先稳定 baseline
```

---

## 下一步 Monthly Dev Spec：M2

我建议把接下来一个月定义为：

```text
M2: Strict Stabilization, Subagent Review, and Longhaul Recovery
周期：2026-05-28 到 2026-06-27
```

下载版在这里：
[devspire_m2_monthly_dev_spec_strict_audit.md](sandbox:/mnt/data/devspire_m2_monthly_dev_spec_strict_audit.md)

### Week 1：Forensic stabilization + Sts1Events decision

目标：

```text
恢复一个真实、可验证、默认测试全绿的 baseline。
```

必须完成：

```text
git branch / HEAD / stash / diff / status 审计
WorktreeBatchScript 失败原因定位
Sts1Events 三选一：formal / staging / remove
dotnet build
dotnet test --no-build
dotnet format --verify-no-changes --no-restore
git diff --check
report-worktree-batches.ps1 -FailOnUnclassified
```

### Week 2：Debug scaffold acceptance or rollback

目标：

```text
决定 debug scaffold 留下还是回滚。
```

验收：

```text
默认 off
config 行为明确
无初始化顺序副作用
无 RNG/save/load/multiplayer 副作用
tests 全绿
docs 不 overclaim
```

### Week 3：RitsuLib 状态纠正

目标：

```text
把 RitsuLib 从“过度完成声明”改成真实状态。
```

如果保留 hard dependency：

```text
publish/package
release artifact tests
runtime dependency install docs
loader smoke with BaseLib + STS2-RitsuLib + Spire Plus
tester handoff
hash/version docs
```

如果不保留：

```text
降级为 staging
移除 manifest hard dependency
PR5 标 partial/staging
```

### Week 4：恢复 one-file longhaul audit

目标：

```text
回到你原本要求的一次只审一个文件。
```

前 10 个文件：

```text
EZMicroBalance.csproj
EZMicroBalance.json
EZMicroBalanceCode/MainFile.cs
EZMicroBalanceCode/Core/Integrations/RitsuLib/RitsuLibBootstrap.cs
EZMicroBalanceCode/Diagnostics/SpirePlusDebug.cs
EZMicroBalanceCode/Config/SpirePlusModConfig.cs
tests/EZMicroBalance.Tests/ActiveSourceManifestGuardTests.cs
docs/migration.md
docs/integrations/ritsulib.md
docs/longhaul-audit/AUDIT_STATE.json
```

---

## 必须提醒他使用 subagent

他用过一次 `Agent(Research RitsuLib API surface)`，这是对的，但后面没有持续让 subagent 分担 forensic、test、docs、source evidence、package/runtime 审核，导致主 agent 一边修一边判断，出现了“测试没绿但标完成”的问题。

下一轮必须明确要求：

```text
每个非平凡子任务先开 subagent，主 agent 只做整合和最终决策。
```

推荐 subagent 分工：

```text
GitForensicsAgent
- 只读检查 branch / HEAD / stash / diff / untracked / dirty files。

BatchScriptAgent
- 专门检查 WorktreeBatchScriptRunsAndWritesBatchPathspecs 为什么失败。

Sts1EventsTriageAgent
- 列出 Sts1Events 影响的 compile/export/localization/source manifest/test/docs 面。
- 给 formal / staging / remove 三种方案。

DebugConfigAgent
- 检查 EnableDebugLogs 是否真正可配置、默认 off、无副作用。

RitsuLibRuntimeAgent
- 检查 RitsuLib hard dependency 是否需要 package/runtime/handoff/loader smoke。

DocsTruthAgent
- 删除 “Done / all verified / complete” 这类 overclaim。
```

---

## 你可以直接发给他的下一条 prompt

```text
当前工作未验收。不要 commit，不要继续 PR6 Batch4，不要继续 PR7，不要扩展 debug，不要继续 longhaul audit。

必须先使用 subagent 做稳定化审计：

1. GitForensicsAgent:
   只读检查 branch、HEAD、stash、git status、git diff、untracked files、dirty files。
   不要改文件。

2. BatchScriptAgent:
   只读运行并分析：
   .\scripts\report-worktree-batches.ps1 -FailOnUnclassified
   解释 WorktreeBatchScriptRunsAndWritesBatchPathspecs 为什么失败。
   不要说“commit 后会过”，必须指出脚本输出里的具体原因。

3. Sts1EventsTriageAgent:
   列出 Sts1Events 是否 tracked、是否在 compile glob、是否在 source manifest、是否在 export preset、是否有 localization。
   给出 formal / staging / remove 三个方案的最小改动清单。

4. DebugConfigAgent:
   检查 SpirePlusDebug 和 EnableDebugLogs 是否默认 off、是否真正接入 config、是否可能改变初始化顺序或 runtime 行为。

5. DocsTruthAgent:
   找出所有 “Done / complete / all verified / tests pass / format clean / Sts1Events unrelated” 的不实或未证实声明，列出要改的文件。

所有 subagent 先只汇报，不改文件。

主 agent 汇总后，只处理 Week 1 stabilization：
- 不处理 RitsuLib patch migration
- 不处理 high-risk patches
- 不新增 debug
- 不 commit

验收命令必须全部通过：
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified

如果任何一个失败，最终报告必须写 Not complete。
```

最简洁地说：**他有进展，但没完成；现在不能 commit。下一步不是继续开发，而是用 subagent 分拆审计，把 WorktreeBatchScript、Sts1Events、debug、RitsuLib overclaim 四件事收口。**

[1]: https://sts2-ritsulib.ritsukage.com/guide/getting-started "Getting Started - RitsuLib"
[2]: https://sts2-ritsulib.ritsukage.com/guide/framework-design "How RitsuLib Is Organized - RitsuLib"
