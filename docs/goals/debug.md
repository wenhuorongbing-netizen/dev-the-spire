不接受他的完成结论。你现在应该明确回他：**不要继续 audit，也不要 commit；先把当前 10 个失败测试逐项清零。**

他这份新报告比之前清楚，但也暴露了更严重的问题：他说 `Untracked files: None`，同时又说 `Sts1Events: Tracked (committed in HEAD)`；后面却继续说 “Sts1Events 是 untracked / unrelated”。这在逻辑上自相矛盾。既然当前报告里 `Sts1Events` 已经是 tracked、committed in HEAD，那它就是当前项目状态的一部分，不能再当成“无关临时文件”排除。

## 结论：当前状态不通过

当前有效状态应该写成：

```text
Build: pass
Tests: fail, 10 failed
Format: not proven clean because command timed out
Stash: empty
Untracked: none
HEAD: main / 649788c3 "debugging"
Uncommitted tracked changes: 6 files
Acceptance: rejected
Next action: stabilize failing tests, not continue longhaul audit
```

注意，`dotnet format ... --verify-no-changes` 那条命令显示的是：

```text
(No output)
(timeout 2m)
```

这不能算“format clean”。只有命令正常退出、exit code 为 0，才能算 clean。

## 他现在最明显的错误判断

### 1. `Sts1Events` 不能再被说成 unrelated

当前项目的 `.csproj` 编译规则是：

```xml
<Compile Include="EZMicroBalanceCode/**/*.cs" />
```

也就是 `EZMicroBalanceCode/` 下的 `.cs` 都会进入编译，不管文件是不是他这轮写的。

而他自己的 forensic report 已经说：

```text
Sts1Events: Tracked (committed in HEAD)
Untracked files: None
```

所以正确说法是：

```text
Sts1Events is part of current HEAD and must be included in source manifest / docs / export / test coverage decisions, or explicitly removed/excluded by a reviewed change.
```

不是：

```text
Sts1Events is untracked and unrelated.
```

### 2. 10 个失败测试不是都能甩给 Sts1Events

他列出的失败项里至少这些明显跟当前 uncommitted patch 改动有关：

```text
DistinguishedCapeUnaffordableVakuuPathPreservesVisibleOptionCount
DistinguishedCapeUsesV43MaxHpMathAndCannotBeSelectedWhenUnableToPay
HarmonyPatchTargetsAreDeclaredForImplementedAncientSurfaces
PatchInventoryIsGeneratedReadableAndClassified
```

而 forensic report 同时显示这些文件被修改：

```text
BlackStarCompensationPatches.cs
ChoicesParadoxPatches.cs
DistinguishedCapePatches.cs
FiddlePatches.cs
RitsuLibBootstrap.cs
docs/goals/debug.md
```

所以这不是“只有 Sts1Events 导致失败”。至少 `DistinguishedCapePatches.cs` 的修改和 Distinguished Cape 测试失败高度相关；patch target / patch inventory 失败也很可能来自这几个 patch 文件的 RitsuLib/IPatchMethod 尝试。不能接受他说“debug logging changes are clean”。

### 3. RitsuLib 迁移仍然只能算 partial

RitsuLib 官方文档的初始化示例不是只 `CreateLogger()`。它还包含注册 assembly、必要时注册 Godot scripts、创建 patcher、注册 patches、再 apply patcher。([RitsuLib][1])

他现在做的是：

```text
RitsuLib logger + raw Harmony.PatchAll()
```

这最多叫：

```text
RitsuLib diagnostics scaffold
```

不能叫：

```text
RitsuLib patching adopted
```

官方文档也说项目里可以加 `STS2.RitsuLib` PackageReference，并在 manifest 里声明 `{ "id": "STS2-RitsuLib" }`，但这只是 dependency 方向成立，不等于你的发布包、runtime loader、tester handoff 都已经完成。([RitsuLib][1])

## 现在应该怎么回他

他问：

```text
What should I do next — fix the test failures, or continue with the audit?
```

你的回答应该是：

```text
Fix the test failures first. Do not continue audit. Do not commit. Do not continue PR6/PR7. Do not call the migration/debug work complete until all default tests pass and the failing-test ledger is updated.
```

但不是让他“一口气乱修 10 个”。要按失败项逐个修，每修一个跑 targeted test。

## 给他下一条 prompt

你可以直接复制这段给他：

```text
停止继续 longhaul audit、RitsuLib migration、debug expansion、PR6 Batch4、PR7。

当前验收不通过。不要 commit，不要 stash，不要 checkout，不要继续新增功能。

先做 test stabilization，一次只处理一个 failing test。

当前事实以 forensic report 为准：
- Branch: main
- HEAD: 649788c3 "debugging"
- Stash: empty
- Untracked files: none
- Sts1Events: tracked / committed in HEAD
- Build: pass
- Tests: 293 passed, 21 skipped, 10 failed
- Modified tracked files:
  - EZMicroBalanceCode/Ancients/Patches/BlackStarCompensationPatches.cs
  - EZMicroBalanceCode/Ancients/Patches/ChoicesParadoxPatches.cs
  - EZMicroBalanceCode/Ancients/Patches/DistinguishedCapePatches.cs
  - EZMicroBalanceCode/Ancients/Patches/FiddlePatches.cs
  - EZMicroBalanceCode/Core/Integrations/RitsuLib/RitsuLibBootstrap.cs
  - docs/goals/debug.md

Rules:
1. Do not claim Sts1Events is untracked or unrelated. It is tracked in current HEAD.
2. Do not claim format is clean unless dotnet format exits with code 0 without timeout.
3. Do not claim debug logging is complete while default tests fail.
4. Do not claim PR5/PR6 done while tests fail.
5. Do not continue the longhaul file audit until default validation is green.
6. Do not edit more than the files needed for the current failing test.
7. For any patch behavior failure, inspect the old behavior, the current diff, and the relevant guard test before changing code.
8. If a failure is due to an out-of-scope patch migration, revert that specific uncommitted patch-file change after saving its diff to an ignored backup file.

First, create a failure ledger only; do not fix yet:
- docs/goals/debug.md or docs/longhaul-audit/BLOCKERS.md must record the 10 failing tests.
- For each failing test, classify:
  A. caused by Sts1Events tracked addition
  B. caused by uncommitted patch file migration
  C. caused by docs/localization/website/export drift
  D. caused by batch script/worktree state
  E. unknown, needs investigation

Then handle failures in this order:

1. DistinguishedCape behavior tests:
   - Inspect git diff for DistinguishedCapePatches.cs.
   - If the diff is RitsuLib/IPatchMethod migration or unrelated to debug, revert only this file to HEAD or restore original behavior.
   - Run:
     dotnet test EZMicroBalance.sln --no-build --filter "FullyQualifiedName~DistinguishedCape"
   - Record result.

2. Patch target / patch inventory tests:
   - Inspect BlackStarCompensationPatches.cs, ChoicesParadoxPatches.cs, FiddlePatches.cs, DistinguishedCapePatches.cs diffs.
   - Revert or fix patch declarations so Harmony patch targets match inventory.
   - If patch declarations changed, run:
     .\scripts\generate-patch-inventory.ps1
     .\scripts\validate-repository-hygiene.ps1
   - Run targeted patch inventory tests.

3. Active source manifest:
   - Since Sts1Events is tracked, either add it properly to source manifest/coverage, or explicitly remove/exclude it with a documented decision.
   - Do not call it untracked.

4. Chinese intro / mojibake / website / export preset:
   - Fix only the exact failing assertion after reading test output.
   - Do not broaden docs.

5. Worktree batch script:
   - Run:
     .\scripts\report-worktree-batches.ps1 -FailOnUnclassified
   - Fix classifier only if the failure is path classification, not by hiding files.

After each fix:
- Run the targeted failing test.
- Update the failure ledger.
- Do not proceed to the next failing test until the current one is green or explicitly blocked.

Final required validation:
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified

Final report must say:
- fixed tests
- remaining failures, if any
- exact files changed
- commands and exit status
- whether default validation is green
- no commit was made
```

## 10 个失败项的严格分类

| 失败测试                                                                  | 初步归类                                                  | 应该怎么处理                                       |
| --------------------------------------------------------------------- | ----------------------------------------------------- | -------------------------------------------- |
| `ActiveSourceFilesAreCoveredByTheGuardManifest`                       | `Sts1Events` tracked addition / source manifest drift | 不许说 untracked；要么纳入 manifest，要么移除/排除          |
| `ChineseIntroKeepsPreviewToolsInsideSpirePlus`                        | docs drift                                            | 读 assertion，修 `docs/intro.zh.md` 或对应 doc     |
| `DistinguishedCapeUnaffordableVakuuPathPreservesVisibleOptionCount`   | patch behavior regression                             | 优先检查/回滚 `DistinguishedCapePatches.cs` diff   |
| `DistinguishedCapeUsesV43MaxHpMathAndCannotBeSelectedWhenUnableToPay` | patch behavior regression                             | 同上，不要改 test 迁就 bug                           |
| `HarmonyPatchTargetsAreDeclaredForImplementedAncientSurfaces`         | patch target drift                                    | 检查 4 个 modified patch files                  |
| `PatchInventoryIsGeneratedReadableAndClassified`                      | patch inventory stale / patch migration drift         | 先恢复 patch declaration，再 regenerate inventory |
| `SimplifiedChineseLocalizationFilesDoNotContainMojibake`              | localization encoding/text bug                        | 找具体文件和坏片段，修文本                                |
| `WebsiteHardcodedGameplaySummariesStayCurrent`                        | website/docs drift                                    | 修 `website/content-data.js` 或当前 docs，不扩大范围   |
| `ExportPresetTracksEveryActiveResourceAndExcludesNonReleaseFolders`   | resource/export drift                                 | 检查 Sts1Events 是否带资源，或 export preset 缺/多      |
| `WorktreeBatchScriptRunsAndWritesBatchPathspecs`                      | worktree classifier / dirty state                     | 跑 batch script，看 unclassified path           |

## 当前不能做的事

不要让他做这些：

```text
commit these changes
continue PR6 Batch4
migrate 63 patch classes
continue longhaul audit
mark debug complete
mark PR5/PR6 done
ignore Sts1Events
update docs to say tests pass
```

尤其不要 commit。现在有 10 个失败测试和 6 个 uncommitted tracked changes，commit 只会把不稳定状态固化。

## 你这边的下一步决策

现在你只需要决定一件事：`Sts1Events` 是否要成为当前 Spire Plus 的正式功能。

我的建议是：

```text
本月不要把 Sts1Events 作为功能推进。
```

原因是当前项目的月度主线应该是：

```text
1. 恢复绿色 baseline
2. 收口 RitsuLib/debug scaffold
3. 恢复逐文件 longhaul audit
4. 等 manual evidence backlog 降低后再碰高风险 patch migration
```

如果 `Sts1Events` 是你想保留的长期功能，也应该先写成：

```text
docs/features/sts1-events/README.md
docs/features/sts1-events/source-research.md
```

然后排进 longhaul queue，而不是现在和 RitsuLib/debug 混在一起修。

## 简短回复版本

你可以直接对他说：

```text
不继续 audit。先修 10 个 failing tests。当前报告显示 Sts1Events 是 tracked/committed，不是 untracked；不要再说 unrelated。format timeout 也不能算 clean。PR5/PR6/debug 都不能标 complete。按 failing test ledger 一次修一个测试，先处理 DistinguishedCape patch behavior 和 patch inventory，再处理 Sts1Events manifest/docs/localization/website/export/batch script。所有 default validation 绿之前不要 commit。
```

[1]: https://sts2-ritsulib.ritsukage.com/guide/getting-started "Getting Started - RitsuLib"
