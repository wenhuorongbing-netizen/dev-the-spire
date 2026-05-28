# Longhaul Audit Prompts

## Initialization Prompt (already executed)

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
```

## One-File Audit Prompt

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

## Resume Prompt

```text
继续 longhaul one-file audit。先读 `docs/longhaul-audit/AUDIT_STATE.json` 和 `docs/longhaul-audit/CURRENT_FILE.md`，确定当前状态，然后按 PROMPTS.md 的 one-file prompt 执行下一轮。
```

## Commit Prompt

```text
为 longhaul audit 的本轮修复创建一个 git commit。使用格式：

git commit -m "audit: fix <short bug description> in <file>"

只包含本轮修改的文件。
```
