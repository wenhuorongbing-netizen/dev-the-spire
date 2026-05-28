# DevSpire / Spire Plus 长程逐文件审计与修复 Prompt Pack

适用目标：`wenhuorongbing-netizen/dev-the-spire`，当前活跃交付面为 `Spire Plus`，技术 manifest id / project / resource folder / code folder 仍为 `EZMicroBalance`。

本包目的：让 Codex 或其他 coding agent 按“一个文件一轮”的方式，长期、可恢复、可验证地检查每一个 tracked 文件；发现真实 bug 时最小修复并补证据；没有问题时跳过并记录；每轮只推进一个 current file，避免大上下文、大重构、伪完成。

---

## 0. 调研结论：为什么要用这种 prompting 方法

1. **固定项目规则 + 当前任务状态分离。** Codex 官方说明中，Codex 会在工作前读取 `AGENTS.md`，并按 global → project → nested directory 的顺序合并指导；越靠近当前目录的指导优先级越高，默认合并大小有限。这个机制适合把长期规则放在 `AGENTS.md`，把逐文件状态放在独立 audit state 文件里。

2. **复杂代码任务要给 task、constraints、done criteria、verification。** OpenAI reasoning model 文档强调：对 agentic / research-heavy workflows，要定义“什么算完成”和“如何验证工作”。因此每轮 prompt 必须显式写入完成标准、验证命令、记录格式和停止线。

3. **不要把所有上下文一次塞进去。** OpenAI prompting guide 建议加入相关上下文/RAG，但同时要规划 context window；长程代码审计更适合“队列 + current file + related files only”，而不是一次性读完整项目。

4. **代码修复必须被测试/命令验证。** OpenAI prompting guide 对 software engineering agent 的建议包括：明确工具使用、测试更改、谨慎验证 patch。这里将“验证失败不得完成”写成硬规则。

5. **few-shot / schema 化输出能降低漂移。** OpenAI prompting guide 说明 few-shot 可以通过示例引导模型输出模式。因此本包给每轮输出、日志、状态文件固定模板，降低 Codex 每轮风格漂移。

---

## 1. DevSpire 当前源码边界：写给 Codex 的固定事实

### 1.1 必须保留的项目事实

- 玩家可见目标是 `Spire Plus`。
- 技术 manifest id、install folder、saved-field namespace、legacy alias surface 仍为 `EZMicroBalance`。
- 本轮逐文件审计不得原地改名 `EZMicroBalance`。
- 不得实现 Ascension 21-30。
- 不得实现 custom character。
- 不得复制原版 StS2 大段 decompiled code 进 repo。
- 不得把 uploaded `StS2-v0.106.0-source-code-ai-analyze-codeonly-20260522.zip`、DLL、PCK、ZIP、`.tools/`、`publish/`、`source code/` 提交为 tracked 文件。
- `source code/src/Core/**` 是高风险 patch / map / room / reward / save / multiplayer / UI / hook 的主要源码证据；没有 source evidence，不要改高风险 patch。

### 1.2 当前 repo 活跃模块

- `EZMicroBalanceCode/MainFile.cs`: mod entry；Harmony patching、config registration、feature registry bootstrap。
- `EZMicroBalanceCode/Core/Features/`: feature startup order / registry。
- `EZMicroBalanceCode/Ancients/`: Ancient reward rebalance + Ancient expansion。
- `EZMicroBalanceCode/Ancients/Common/`: shared saved state、card helpers、feature gates、relic helpers。
- `EZMicroBalanceCode/Ancients/Patches/`: Ancient reward / relic / card / reward surface patch families。
- `EZMicroBalanceCode/Ancients/Expansion/Urda/`: Urda + Root Eyes / Seed Bank / Seedbed / Rooted Route。
- `EZMicroBalanceCode/Ancients/Expansion/Morvi/`: Morvi blessings + debt/card state。
- `EZMicroBalanceCode/Ancients/Expansion/Lotha/`: Lotha blessings + Death Reprieve。
- `EZMicroBalanceCode/Ancients/Expansion/Vakuu/`: hidden Vakuu fight。
- `EZMicroBalanceCode/Ascension/`: A11-A20 development systems。
- `EZMicroBalanceCode/Preview/`: Crystal Sphere peek + transform preview；必须保持 read-only / UI-only / RNG-safe。
- `EZMicroBalance/localization/eng/` + `EZMicroBalance/localization/zhs/`: bilingual localization；新增/修改 key 要双语同步。
- `tests/EZMicroBalance.Tests/`: source shape、localization、docs、package、runtime evidence guard。
- `docs/`: active docs + archive；不要把 archive prompt dump 当默认读取路径。
- `scripts/`: validation / package / evidence helper。
- `website/`: promoted public site source。

### 1.3 上传的 StS2 v0.106.0 source zip 本地检查结果

本地检查 `StS2-v0.106.0-source-code-ai-analyze-codeonly-20260522.zip`：

- total entries: 4,248
- `.cs`: 3,419
- `.json`: 648
- `.md`: 122
- `.gd`: 48
- `source-code/src/Core/` 下 `.cs`: 3,393
- `source-code/src/Core/` 主要子目录数量: 46
- 关键 source evidence 子目录：`Runs`、`Rooms`、`Saves`、`Multiplayer`、`Map`、`Rewards`、`Combat`、`Events`、`Models`、`Nodes`、`Hooks`、`Random`、`Localization`。

注意：这个 zip 内部路径使用 Windows backslash；在 Linux/macOS 解压时可能变成包含 `\` 的文件名。Codex 在 Windows repo 中应优先把它恢复/刷新到 ignored `source code/`，或者用脚本 normalize 到 local-only scratch。不要把它提交。

---

## 2. 推荐新建的审计状态目录

把以下目录作为本长程工作唯一状态源：

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

### 2.1 `FIXED_GOAL.md` 模板

```markdown
# Longhaul File Audit Fixed Goal

## Goal
逐文件检查 `dev-the-spire` 的 tracked files，结合当前 repo docs/tests、实际源码、StS2 v0.106.0 local source evidence，发现真实 bug 时最小修复并验证；没有 bug 时记录 skip；每轮只处理一个 current file。

## Stop Line
- 不做 release-ready claim。
- 不改 `EZMicroBalance` manifest id / project / install folder / DLL / PCK / resource folder / saved-field namespace。
- 不实现 Ascension 21-30。
- 不实现 custom character。
- 不提交 uploaded source zip、DLL、PCK、ZIP、`.tools/`、`publish/`、`source code/`。
- 不做 unrelated broad refactor。
- 每轮最多一个 current file；如修复需要 touching related files，必须在 `CURRENT_FILE.md` 写明原因。

## Done Criteria For Entire Longhaul Audit
- `QUEUE.tsv` 中所有 tracked files 为 `done` / `skipped` / `blocked-with-owner-decision`。
- 每个 fixed file 有 root cause、changed files、validation command、validation result。
- 每个 skipped file 有检查证据和 skip reason。
- 每个 blocker 有明确 owner decision / missing source / failing command。
- 最终跑过 baseline validation：
  - `dotnet build EZMicroBalance.sln`
  - `dotnet test EZMicroBalance.sln --no-build`
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`
  - `git diff --check`
  - `scripts/report-worktree-batches.ps1 -FailOnUnclassified`
```

### 2.2 `AUDIT_STATE.json` 模板

```json
{
  "schemaVersion": 1,
  "auditName": "Spire Plus longhaul one-file audit",
  "repo": "wenhuorongbing-netizen/dev-the-spire",
  "technicalModId": "EZMicroBalance",
  "playerFacingName": "Spire Plus",
  "currentPackageLine": "v0.1.0-private-beta.N",
  "mode": "one-file-per-run",
  "currentFile": null,
  "lastCompletedFile": null,
  "queuePath": "docs/longhaul-audit/QUEUE.tsv",
  "remainingCount": null,
  "fixedCount": 0,
  "skippedCount": 0,
  "blockedCount": 0,
  "requiredBaselineCommands": [
    "git status --short --branch",
    "dotnet build EZMicroBalance.sln",
    "dotnet test EZMicroBalance.sln --no-build",
    "dotnet format EZMicroBalance.sln --verify-no-changes --no-restore",
    "git diff --check"
  ],
  "lastValidation": {
    "commands": [],
    "result": "not-run",
    "notes": ""
  }
}
```

### 2.3 `QUEUE.tsv` columns

```text
index	status	path	kind	risk	owner	source_evidence_hint	related_tests	last_action	last_result	notes
```

Status enum:

```text
queued
current
fixed
done
skipped
blocked
blocked-with-owner-decision
```

Kind enum:

```text
source-cs
test-cs
resource-json
localization-json
godot-resource
script-ps1
script-sh
workflow-yml
website-js-css-html
doc-md
config
other
```

Risk enum:

```text
critical-high
high
medium
low
governance
local-only-boundary
```

### 2.4 `CURRENT_FILE.md` 模板

```markdown
# Current File Audit

## Current File
- Path:
- Queue index:
- Status: current
- Started at:

## Why This File Is In Scope
-

## Related Files Read This Round
-

## Source Evidence Read This Round
-

## Checklist Results
- Compile/type risk:
- Harmony/source signature risk:
- Null/state/save-load risk:
- RNG/preview purity risk:
- Multiplayer/host-client risk:
- Localization/resource/path risk:
- Tests/docs drift risk:
- Dead code/duplication/readability risk:

## Findings
- Finding 1:
  - Severity:
  - Evidence:
  - Root cause:
  - Fix plan:
  - Validation:

## Decision
- fixed / skipped / blocked:
- Reason:

## Commands Run
-

## Result
-
```

### 2.5 `REVIEW_LOG.md` entry template

```markdown
## AUDIT-0001 — <path>

- Date:
- Queue index:
- Risk:
- Decision: fixed / skipped / blocked
- Files read:
- Source evidence:
- Findings:
- Changes:
- Validation:
- Remaining risk:
- Next file:
```

### 2.6 `FIX_LEDGER.md` entry template

```markdown
## FIX-0001 — <short title>

- Current file:
- Bug class:
- Symptom:
- Source evidence:
- Root cause:
- Minimal fix:
- Tests added/updated:
- Validation commands:
- Validation result:
- Follow-up required:
```

---

## 3. Queue generation method：不漏文件夹、不漏细节

### 3.1 Codex 初始化 prompt

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

### 3.2 PowerShell queue helper prompt

```text
请写一个小脚本 `scripts/longhaul-audit-queue.ps1`，只生成/检查 `docs/longhaul-audit/QUEUE.tsv`，不要修改业务代码。

要求：
1. 输入来自 `git ls-files`。
2. 每个 tracked file 一行。
3. 自动判定 kind/risk/owner/source_evidence_hint。
4. 如果 `docs/patch-inventory.md` 中有该文件，优先使用 patch inventory 的 owner/risk。
5. risk 规则：
   - patch inventory high => critical-high
   - path contains `Patches`, `RunHook`, `CombatHook`, `Save`, `Multiplayer`, `Map`, `Rewards`, `VakuuFight`, `RootSight`, `A20`, `AscensionSelection` => high 或 medium，视是否 touches run/room/save/lobby/multiplayer/lifecycle
   - localization/resources/manifest/export/workflow/package scripts => medium
   - tests => governance
   - docs archive => low，除非 active docs/tests 引用
6. 输出列：index, status, path, kind, risk, owner, source_evidence_hint, related_tests, last_action, last_result, notes。
7. 支持 `-Check`：如果 queue 缺 tracked file 或包含不存在 file，返回 nonzero。
8. 运行脚本后执行 `git diff --check`。
```

---

## 4. 每轮只处理一个文件的固定 prompt

把下面 prompt 原样复制给 Codex；每次只跑一轮。

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
D. 按本 prompt 的 checklist 审计：
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

验证命令规则：
- source/config changed:
  - `dotnet build EZMicroBalance.sln`
  - targeted `dotnet test EZMicroBalance.sln --no-build --filter ...` if a relevant guard exists
  - `dotnet test EZMicroBalance.sln --no-build` when fix affects shared/high-risk behavior
  - `dotnet format EZMicroBalance.sln --verify-no-changes --no-restore`
  - `git diff --check`
- resource/localization/manifest/export/package changed:
  - all source/config commands above
  - `dotnet publish EZMicroBalance.sln`
  - `.scripts\package-spire-plus.ps1`
  - opt-in release artifact tests only after package refresh
- docs-only changed:
  - targeted doc/guard tests if available
  - `git diff --check`
  - build/test only if active guard docs/tests require it

最终回复只包含：
1. current file
2. decision: fixed / skipped / blocked
3. actual bug found or no-bug reason
4. changed files
5. validation commands and result
6. next queued file path
7. remaining risks
```

---

## 5. 专项 checklist：按文件类型审计

### 5.1 `source-cs` 通用检查

- Namespace 与 folder/module 是否一致。
- public/internal 类型是否必要。
- nullable flow 是否正确；有无 `!` 遮掩真实 null risk。
- static state 是否按 run/player/combat 清理。
- Dictionary/ConditionalWeakTable key 是否正确：`Player`、`RunState`、`Combat`、`Room` 是否混用。
- 是否有跨 run 污染、重进主菜单不清理、load 后重复 hook。
- 是否有 hardcoded string id 与 localization/model id/manifest id 不一致。
- 是否有不受 gate 控制的 feature 行为。
- 是否直接 new canonical `AbstractModel`；应优先 `ModelDb`。
- 是否吞异常导致 release evidence 假干净。
- 是否只靠 log 代替实际状态修复。

### 5.2 Harmony patch / hook 文件

必须检查：

- patch target class/method 在 `source code/src/Core/**` 是否存在。
- signature 是否匹配当前 v0.106.0。
- prefix/postfix/finalizer 参数类型是否正确。
- patch 是否过宽，是否会影响 vanilla unrelated paths。
- 是否 guard feature gate。
- 是否在 multiplayer/client path 误改 shared run state。
- 是否在 save/load/room transition 中破坏 vanilla invariant。
- 是否有 fallback log，但 fallback 不应隐藏错误。
- 修改 patch 后必须 regenerate/check `docs/patch-inventory.md`。

### 5.3 Save/load / state 文件

必须检查：

- save field id 稳定，不破坏 legacy decode。
- serialization invariant culture；不依赖本地语言小数/日期格式。
- load hydrate 是否会创建 duplicate state。
- combat transient state 是否不会写入 persistent save，除非设计要求。
- run end / combat end / main menu restore 是否清理。
- multiplayer player slot / owner 是否记录。
- tests 是否覆盖 old format / missing field / malformed field。

### 5.4 RNG / preview 文件

必须检查：

- preview 必须 fork/copy RNG，不消耗 committed `RunState.Rng` / `PlayerRngSet`。
- 不调用会 reveal / reward / transform / add / resolve 的真实 mutation API。
- preview UI close/cancel 是否清空临时状态。
- save/reopen 后是否不会 commit preview-only state。
- co-op 中仅 local UI，不增加选择/奖励/真实 RNG call。

### 5.5 Multiplayer 文件

必须检查：

- host-authoritative vs client-read-only 是否写清。
- client path 是否只显示/记录，不改 run state。
- unverified co-op gameplay 是否 fail-closed。
- env override 是否只用于 deliberate debugging，且 log 清楚。
- mismatch diagnostics 是否只 log，不改变 join/save/quit/act-entry state。

### 5.6 Localization / resource / export

必须检查：

- EN/ZHS key 成对存在。
- placeholder 数量/名称一致。
- 中文没有 mojibake。
- player-facing text 不泄漏 implementation terms。
- resource path 与 `MainFile.ResPath` / manifest / export preset 一致。
- PNG/resource 在 active package export list 中。
- 不引用 official game assets unless permission documented。

### 5.7 Tests

必须检查：

- 新 guard 是否用 `TestRepo.cs` shared helpers，不复制 `FindRepoRoot`/JSON/ZIP/PCK/hash helper。
- 测试是否依赖 ignored `publish/` 或 installed artifacts；依赖则必须 opt-in skip。
- 测试不是 tautological assertion。
- 测试断言 source invariant，而不是只检查某个容易漂移的片段。
- 测试名描述真实行为。

### 5.8 Docs / scripts / website

必须检查：

- active docs 是否短、当前、可执行；历史内容进 archive。
- docs 是否仍说旧 beta/current package/hash。
- scripts 是否 path-safe，避免 broad `git clean -fdX`。
- website content 是否与 package metadata / current features / validation state一致。
- workflow 是否不上传 ignored/local artifacts。

---

## 6. 风险优先级排序

建议 `QUEUE.tsv` 排序如下：

1. critical-high patch files from `docs/patch-inventory.md`：run、room、save、lobby、multiplayer、lifecycle。
2. `Vakuu` child combat / room transition / save restore。
3. `Urda RootSight` map preview / commit / save / UI click。
4. `Ascension` selector / A11 map / A20 boss / multiplayer diagnostics。
5. RNG/preview files。
6. Ancient reward rebalance patch files。
7. save-state contracts / feature gates / initializers。
8. localization/resources/export/manifest/package scripts。
9. tests / scripts / docs / website。
10. low-risk pure constants / docs archive。

---

## 7. 源码 evidence 对照表

| DevSpire area | 必读 repo file | 必查 StS2 source evidence |
| --- | --- | --- |
| Ancient reward patches | `EZMicroBalanceCode/Ancients/Patches/*` | `source code/src/Core/Models/Relics`, `Cards`, `Rewards`, `GameActions`, `Hooks` |
| Urda Root Sight | `Ancients/Expansion/Urda/*RootSight*`, `*Map*`, `*RoomPatches*` | `source code/src/Core/Runs`, `Map`, `Rooms`, `Events`, `Nodes` |
| Seedbed / Rootblight | `UrdaBlessingService.Seedbed*`, `Ascension/Rewards/RootDeckService*`, `Ascension/Combat/RootBud*` | `source code/src/Core/Combat`, `Cards`, `Hooks`, `GameActions` |
| Morvi card/debt state | `Ancients/Expansion/Morvi/*` | `source code/src/Core/Cards`, `Combat`, `Saves`, `GameActions` |
| Lotha Death Reprieve | `Ancients/Expansion/Lotha/*Death*`, hooks | `source code/src/Core/Combat`, `Saves`, `Hooks` |
| Vakuu fight | `Ancients/Expansion/Vakuu/*` | `source code/src/Core/Rooms`, `Events`, `Combat`, `Saves`, `Runs` |
| Ascension selection | `Ascension/Patches/AscensionSelectionPatches.cs`, gates | `source code/src/Core/Runs`, `Multiplayer`, `Settings`, `Nodes` |
| Ascension map | `Ascension/Map/*`, map patches | `source code/src/Core/Map`, `Runs`, `Rooms`, `Nodes` |
| A20 boss flow | `Ascension/Combat/*A20*`, events, reward patches | `source code/src/Core/Runs`, `Rooms`, `Rewards`, `Events` |
| Preview tools | `Preview/*` | `source code/src/Core/Nodes`, `Rewards`, `Random`, transform/card APIs |
| Localization | `EZMicroBalance/localization/*` | `source code/src/Core/Localization`, current card/relic/power model ids |
| Package/export | `EZMicroBalance.json`, `export_presets.cfg`, scripts | installed BaseLib/StS2 paths, package evidence scripts |

---

## 8. 常见 bug 类型与修复策略

### BUG-HARMONY-SIGNATURE-DRIFT

现象：build passes 但 runtime patch no-op 或 crash；patch target private method signature changed。

修复：
1. 查 `source code/src/Core/**` 当前 signature。
2. 更新 patch target / argument list。
3. 缩窄 patch scope。
4. regenerate `docs/patch-inventory.md`。
5. 添加 source guard test。

### BUG-STATE-CROSS-RUN-LEAK

现象：新 run 继承旧 run marker / preview / combat state。

修复：
1. 把 static state 改为 keyed by `RunState`/`Player`/`Combat`。
2. run start/end/main menu restore 清理。
3. save hydrate 时避免 duplicate。
4. 加 guard test 模拟两个 run/player。

### BUG-RNG-PREVIEW-COMMITS

现象：Crystal Sphere / transform preview 改变真实 RNG 结果。

修复：
1. fork/copy RNG snapshot。
2. preview path 禁止调用 mutation API。
3. close/cancel 清状态。
4. test assert committed RNG counter unchanged。

### BUG-CLIENT-MUTATES-RUN

现象：co-op client path 修改 run/reward/map/combat state，导致 desync。

修复：
1. host-authoritative gate。
2. unverified co-op fail-closed。
3. client path only UI/log。
4. log `coop_*_disabled`。
5. add source guard + manual evidence row remains pending。

### BUG-LOCALIZATION-DRIFT

现象：EN/ZHS key mismatch、placeholder mismatch、mojibake、user-facing implementation terms。

修复：
1. EN/ZHS 同步 key。
2. placeholder count/name parity。
3. update localization guard。
4. avoid debug/backend/source-safe terms in player text。

### BUG-RESOURCE-PACKAGE-DRIFT

现象：resource exists but not exported; website/package metadata stale。

修复：
1. update export preset / resource path。
2. run publish + package script。
3. update package hash docs only if package refreshed。
4. opt-in artifact tests。

---

## 9. 修复轮 prompt：当发现 bug 后使用

```text
继续当前 file audit，但只执行已经证明的 bug fix，不扩大范围。

Current file:
<path>

已证明 bug:
<用代码片段/source evidence/触发路径说明>

修复约束：
1. 只改 root cause 所需最小文件。
2. 保留 existing save field format，除非 bug 本身是 format migration；如果迁移，必须加 legacy decode test。
3. 不改变 manifest id。
4. 不把 release/manual evidence 标为通过。
5. 修改 patch 后更新 patch inventory 或说明为何无需更新。
6. 修改 player-facing text 时同步 EN/ZHS。
7. 修改 resource/export/package 时按 package validation 路径跑。

请执行：
A. 应用最小修复。
B. 添加/更新最小 guard test。
C. 运行 targeted validation。
D. 更新 CURRENT_FILE、FIX_LEDGER、REVIEW_LOG、QUEUE、AUDIT_STATE。
E. 汇报 changed files、root cause、validation result、remaining risks。
```

---

## 10. 跳过轮 prompt：当没有 bug 时使用

```text
当前文件检查未发现真实 bug。不要改代码。请完成 skip/done 记录。

Current file:
<path>

已检查证据：
- files read:
- source evidence:
- references scanned:
- relevant tests/docs:

请更新：
- CURRENT_FILE.md: decision = skipped/done
- REVIEW_LOG.md: add audit entry
- SKIPPED.md: add skip evidence
- QUEUE.tsv: status = skipped 或 done
- AUDIT_STATE.json: lastCompletedFile, skippedCount/done state

最后汇报：
1. skipped file
2. why no fix was needed
3. evidence read
4. validation, if any
5. next queued file
```

---

## 11. Blocker prompt：当缺 source 或 validation failed

```text
当前文件审计遇到 blocker。不要猜，不要绕过验证，不要扩大范围。

Current file:
<path>

Blocker:
<missing source / failing command / ambiguous owner decision / required runtime-only evidence>

请执行：
1. 把 QUEUE.tsv 当前文件 status 标为 blocked。
2. 在 BLOCKERS.md 写明：
   - blocker id
   - file
   - exact command or missing evidence
   - why no safe fix can proceed
   - minimal next action
3. 在 CURRENT_FILE.md 写明 blocked 状态。
4. AUDIT_STATE.json 更新 blockedCount。
5. 不要把该文件标记完成。
6. 选择 next queued file 但不要处理它，除非用户明确说 continue。
```

---

## 12. Resume prompt：恢复长程工作

```text
恢复 dev-the-spire longhaul one-file audit。

先读：
- AGENTS.md
- PROJECT_STATE.md
- docs/longhaul-audit/FIXED_GOAL.md
- docs/longhaul-audit/AUDIT_STATE.json
- docs/longhaul-audit/CURRENT_FILE.md
- docs/longhaul-audit/QUEUE.tsv
- docs/longhaul-audit/BLOCKERS.md
- docs/longhaul-audit/FIX_LEDGER.md

要求：
1. 总结当前状态：current file、last completed、remaining count、blocked count。
2. 如果 current file 未完成，继续 current file。
3. 如果 current file 已完成/空，选下一个 queued 文件并只处理一轮。
4. 不要重复审计已经 skipped/fixed/done 的文件，除非它被后续修改重新入队。
5. 按 one-file prompt 执行。
```

---

## 13. Requeue prompt：当某个文件被相关修复影响后重新入队

```text
某些文件因 related fix 被修改或可能受影响，需要重新入队检查。

请只更新 queue，不做审计。

Files to requeue:
<list>

规则：
1. 如果文件已经 fixed/skipped/done，但本次 diff 修改了它，status 改为 queued，notes 加 `requeued-after-related-fix`。
2. 如果文件是 test/doc，只把 relevant tests/docs 标清楚。
3. 更新 AUDIT_STATE remaining count。
4. 不要处理文件。
```

---

## 14. Commit prompt：完成一批后提交

```text
准备提交 longhaul one-file audit 已完成的一小批结果。

要求：
1. 先运行：
   - git status --short --branch
   - git diff --stat
   - git diff --check
   - dotnet build EZMicroBalance.sln
   - dotnet test EZMicroBalance.sln --no-build
   - dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
   - scripts/report-worktree-batches.ps1 -FailOnUnclassified
2. 不要提交 ignored/local-only artifacts。
3. 检查 changed files 是否都属于本批 fixed/skipped/audit state。
4. commit message 格式：
   `audit: review <area> longhaul files`
5. 如果 validation fails，不要 commit；更新 BLOCKERS.md。
```

---

## 15. 建议的首次执行顺序

1. 初始化队列。
2. 先审计 `MainFile.cs` / `Core/Features/*`，确认 bootstrap 不丢 feature。
3. 审计 critical-high patches：从 `docs/patch-inventory.md` high risk 行开始。
4. 审计 Vakuu flow。
5. 审计 Urda RootSight / Seedbed。
6. 审计 Ascension selection/map/A20/multiplayer。
7. 审计 Preview RNG/UI。
8. 审计 localization/resources/export/package scripts。
9. 审计 tests / docs / website / workflows。
10. 最后跑 full baseline + queue completeness check。

---

## 16. 最终报告模板

```markdown
# Longhaul File Audit Final Report

## Scope
- Queue total:
- Fixed:
- Skipped/done:
- Blocked:

## Bugs Fixed
| Fix id | File | Bug class | Root cause | Validation |
| --- | --- | --- | --- | --- |

## Files Skipped With Evidence
| File | Evidence | Reason |
| --- | --- | --- |

## Blockers
| Blocker id | File | Missing evidence/command | Next action |
| --- | --- | --- | --- |

## Commands
-

## Not Claimed
- No release-ready claim.
- No live-game save/load/death/failure/co-op claim unless evidence exists.

## Remaining User-Test Rows
-
```
