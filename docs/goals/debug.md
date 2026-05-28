可以。我已经把你要的“长程逐文件检查 + 发现 bug 就修 + 没 bug 就跳过 + 每轮只处理一个文件”的 prompt 体系整理成了两个可下载文件：

1. [详细 Prompt Pack / 方法论 / 检查清单](sandbox:/mnt/data/devspire_longhaul_file_audit_prompts.md)
2. [可直接放进 repo 的长程审计模板 zip](sandbox:/mnt/data/devspire_longhaul_templates.zip)

这次我没有直接改 GitHub repo 里的代码；我做的是给你一套可以交给 Codex 长期执行的固定协议。它会让 Codex 每次只拿一个文件出来检查、必要时修复、验证、记录、移出队列，再进入下一个文件。

## 我给你的方案核心

Codex 不应该一次性“扫全项目然后大修”。正确方式是建立一个固定状态目录：

```text
docs/longhaul-audit/
  FIXED_GOAL.md
  AUDIT_STATE.json
  QUEUE.tsv
  CURRENT_FILE.md
  REVIEW_LOG.md
  FIX_LEDGER.md
  SOURCE_EVIDENCE.md
  BLOCKERS.md
  SKIPPED.md
  PROMPTS.md
```

其中最重要的是：

```text
FIXED_GOAL.md      固定目标和不能越过的边界
QUEUE.tsv          所有 tracked files 的逐文件队列
CURRENT_FILE.md    当前正在检查的唯一文件
REVIEW_LOG.md      每个文件检查记录
FIX_LEDGER.md      真实 bug 和修复记录
BLOCKERS.md        缺源码、验证失败、需要你决定的阻塞
SKIPPED.md         检查过但没发现 bug 的文件
```

这个设计是为了避免 Codex 反复读无关上下文、忘记进度、跳文件、重复修、伪完成。OpenAI 的 Codex 文档说明 Codex 会在工作前读取 `AGENTS.md`，并且会按 global、project、nested directory 的顺序合并项目指令；所以长期规则应该放在 `AGENTS.md`，当前任务状态应该放在独立状态文件里，而不是塞进聊天记录。([OpenAI Developers][1])

OpenAI reasoning model 文档也强调，对于 agentic 或 research-heavy workflow，要定义任务、约束、输出格式、什么算完成、以及如何验证；这正是我把 fixed goal、current file、queue、validation command 都写死的原因。([platform.openai.com][2])

## 结合 DevSpire 当前源码的关键约束

当前项目不是普通 C# repo，而是一个 Slay the Spire 2 mod workspace。repo 现在的活跃目标是玩家可见的 `Spire Plus`，但技术 manifest id、compatibility package folder、saved-field namespace 仍然是 `EZMicroBalance`，所以逐文件检查中不允许顺手改名。

当前代码结构已经按模块分好：`MainFile.cs` 是 entry point，`Core/Features/` 管 feature bootstrap，`Ancients/`、`Ascension/`、`Preview/` 分别负责 Ancient、A11-A20、preview tools。这个分层会被用作 queue 的 owner/risk 分类依据。

高风险点非常多，不适合一次性大改。当前 patch inventory 显示有 **166 个 Harmony patch declarations，其中 22 个 high risk**；high risk 的含义是 run、room、save、lobby、multiplayer、game lifecycle 这些会影响运行流程的面。

对于 Ascension map、UI、reward、combat、save/load、hook 行为，当前 goal 文档明确要求把 `source code/src/Core/**` 当作 primary source evidence；所以每个高风险文件检查时，都必须对照你上传的 StS2 v0.106.0 source code，而不能靠猜。

我也本地检查了你上传的 StS2 v0.106.0 source zip：它有 4,248 个条目，其中 `.cs` 3,419 个，`source-code/src/Core/` 下 `.cs` 3,393 个，核心子目录有 46 个。这会作为每轮检查 patch target、method signature、save/load、room transition、RNG、multiplayer ownership 的对照源。

## 给 Codex 的首轮初始化 prompt

下面这个是第一条要跑的 prompt，用来初始化队列，不修代码：

```text
你现在要为 dev-the-spire 建立 longhaul one-file audit 队列。不要修代码，不要重构，只生成状态目录和队列。

必须先读：
- AGENTS.md
- PROJECT_STATE.md
- docs/README.md
- docs/PROJECT_MAP.md
- docs/test-ready-development-goal.md
- docs/worktree-cleanup-audit.md
- docs/patch-inventory.md
- EZMicroBalanceCode/README.md
- tests/EZMicroBalance.Tests/README.md
- scripts/README.md

约束：
1. 不得改 `EZMicroBalance` manifest id。
2. 不得提交 zip/dll/pck/.tools/publish/source code。
3. 不得把 archive prompt dump 作为默认读取路径。
4. 只创建/更新 `docs/longhaul-audit/` 下的状态文件，必要时只更新 docs index 指针。
5. 使用 `git ls-files` 作为 tracked file source of truth。
6. 使用 `.gitignore` 和 `docs/worktree-cleanup-audit.md` 记录 ignored/local-only 边界，但不要把 ignored local files 加入逐文件审计队列。
7. 所有 tracked files 必须进入 `QUEUE.tsv`，包括 source、tests、resources、scripts、docs、website、workflows、configs。
8. 对 `.cs` 文件按 high-risk patch / feature state / test guard / normal source 排序；对非代码文件按 package/resource/localization/workflow/script/docs/website 排序。

生成：
- docs/longhaul-audit/FIXED_GOAL.md
- docs/longhaul-audit/AUDIT_STATE.json
- docs/longhaul-audit/QUEUE.tsv
- docs/longhaul-audit/CURRENT_FILE.md
- docs/longhaul-audit/REVIEW_LOG.md
- docs/longhaul-audit/FIX_LEDGER.md
- docs/longhaul-audit/SOURCE_EVIDENCE.md
- docs/longhaul-audit/BLOCKERS.md
- docs/longhaul-audit/SKIPPED.md

运行并记录：
- git status --short --branch
- git ls-files
- git clean -ndX
- dotnet build EZMicroBalance.sln
- dotnet test EZMicroBalance.sln --no-build
- git diff --check

不要处理第一个文件。只完成队列初始化。
```

## 每轮只处理一个文件的 prompt

初始化以后，每次你只给 Codex 这一条，让它跑一个文件：

```text
执行 dev-the-spire longhaul one-file audit 的下一轮。每轮只处理一个 current file。

硬规则：
1. 先读 `AGENTS.md`、`PROJECT_STATE.md`、`docs/longhaul-audit/FIXED_GOAL.md`、`docs/longhaul-audit/AUDIT_STATE.json`、`docs/longhaul-audit/QUEUE.tsv`、`docs/longhaul-audit/CURRENT_FILE.md`、`docs/longhaul-audit/FIX_LEDGER.md`。
2. 如果 `CURRENT_FILE.md` 已经有未完成 current file，继续它；否则从 `QUEUE.tsv` 取第一个 `queued` 文件，标为 `current`。
3. 本轮只审计这个 current file。除非修复这个文件的真实 bug 必须 touching related file，否则不要改其他文件。
4. 不要做 broad refactor，不要顺手清理无关文件。
5. 不要改 `EZMicroBalance` manifest id / project / install folder / DLL / PCK / saved-field namespace。
6. 不要实现 Ascension 21-30 或 custom character。
7. 不要提交 zip/dll/pck/.tools/publish/source code。
8. 没有 source evidence、test、command 或 diff 证据，不得宣称完成。

本轮步骤：
A. 读取 current file。
B. 用 `rg` 找它的直接引用、被引用、同 feature 邻近文件、相关 guard tests。
C. 如果它是 Harmony patch、hook、run/map/room/reward/save/multiplayer/UI/RNG 文件，必须读取对应 `source code/src/Core/**` 的目标 class/method/signature；如果 local source 不存在，标记 blocked，不要猜。
D. 按 checklist 审计：
   1. compile/type/nullability
   2. source signature drift
   3. save/load/state lifecycle
   4. run/player/multiplayer ownership
   5. RNG / preview purity
   6. Harmony patch scope and fallback behavior
   7. localization/resource/path/export consistency
   8. package/docs/test drift
   9. duplicate helper / broad coupling
   10. user-visible text quality
E. 对每个疑似问题，必须先证明它是真问题：指出代码片段、source evidence、触发路径、为什么现有 tests 没覆盖。
F. 如果没有真实 bug：
   - 不改代码。
   - 更新 `CURRENT_FILE.md`、`REVIEW_LOG.md`、`SKIPPED.md`。
   - 把 `QUEUE.tsv` 中该文件 status 改为 `skipped` 或 `done`。
G. 如果有真实 bug：
   - 写最小 fix plan。
   - 修改 current file；只有必要时修改 tests/docs/localization/source-adjacent files。
   - 增加或更新 guard test，优先使用已有 `TestRepo.cs` helper。
   - 运行 targeted validation，再运行必要 baseline validation。
   - 更新 `FIX_LEDGER.md`、`REVIEW_LOG.md`、`CURRENT_FILE.md`、`AUDIT_STATE.json`、`QUEUE.tsv`。
H. 本轮结束必须清空 `CURRENT_FILE.md` 或写明 blocked current file，不能留下不明状态。

最终回复只包含：
1. current file
2. decision: fixed / skipped / blocked
3. actual bug found or no-bug reason
4. changed files
5. validation commands and result
6. next queued file path
7. remaining risks
```

完整版本我已经放在下载文件里了，比这里更长，包含各种文件类型的检查表、bug 分类、修复策略、blocker prompt、resume prompt、commit prompt。

## 验证命令怎么跑

项目自己的 test-ready goal 已经定义了代码/config 修改后的验证序列：

```powershell
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
```

如果改了 resource、localization、manifest、export、package，还要跑：

```powershell
dotnet publish EZMicroBalance.sln
.\scripts\package-spire-plus.ps1
$env:SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS='1'
dotnet test EZMicroBalance.sln --no-build
Remove-Item Env:\SPIREPLUS_RUN_RELEASE_ARTIFACT_TESTS
```

这些命令不能被当成 live-game、save/load、death/failure、co-op evidence；当前文档也明确说 no live-game/manual evidence claim。

## 我建议你实际怎么跑

第一轮：

```text
把 devspire_longhaul_templates.zip 解压到 repo 根目录，然后给 Codex 运行“初始化 prompt”。
```

之后每轮：

```text
给 Codex 运行“每轮只处理一个文件的 prompt”。
```

每跑完一次，它应该只完成一个文件，并更新：

```text
QUEUE.tsv
CURRENT_FILE.md
REVIEW_LOG.md
FIX_LEDGER.md 或 SKIPPED.md
AUDIT_STATE.json
```

你可以用一句话继续：

```text
继续 longhaul one-file audit，按 docs/longhaul-audit/PROMPTS.md 的 one-file prompt 执行下一轮。
```

这套流程的重点是：**不会漏文件，因为 queue 来自 `git ls-files`；不会乱修，因为 current file 一次只有一个；不会伪完成，因为每个 fixed/skipped/blocked 都必须写证据；不会把项目越搞越臃肿，因为所有历史、状态、bug、blocker 都进固定文件，而不是散落在聊天和临时文档里。**

[1]: https://developers.openai.com/codex/guides/agents-md "Custom instructions with AGENTS.md – Codex | OpenAI Developers"
[2]: https://platform.openai.com/docs/guides/reasoning "Reasoning models | OpenAI API"
