我已经把 `dev-the-spire` 当前 repo、两个上传包和 RitsuLib 官方文档都过了一遍。我的判断是：**不要做“一次性大爆炸重构”**，应该拆成 5 个可验证阶段；尤其不要直接把两个 zip 解压进项目根目录，否则会覆盖现有 `AGENTS.md`、引入 DLL/ZIP、打乱当前 release evidence 体系。

我也整理了一份可交给 Codex 执行的详细计划文件：
Legacy sandbox export link removed. Use `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md` for current StS1 event guidance; current beta.91 proves only RitsuLib-only `v0.107.1` Off and AdditiveBatch1 loader/registration behavior, beta.85/beta.87/beta.88/beta.90 rows remain previous-context evidence, and CanaryOnly gameplay/runtime, save-load, replacement, multiplayer, QA, handoff, and release-ready proof remain pending or blocked.

## 核心结论

当前 repo 的活跃交付目标是 **一个 mod：`Spire Plus`**，但技术 manifest id、项目名、资源目录、代码目录、DLL/PCK/install folder 仍然是 `EZMicroBalance`，这是兼容性边界，不应该在这轮重构里改名。repo 自己也明确要求不要原地改 manifest id，并把 `Spire Plus` 作为玩家可见名、`EZMicroBalance` 作为技术 id 保留。

现在 repo 的活跃结构其实已经被划分过：`EZMicroBalance.sln/.csproj/.json`、`EZMicroBalance/` 资源、`EZMicroBalanceCode/` 源码、`tests/EZMicroBalance.Tests/`、`docs/`、`scripts/`、`website/` 都是当前面；`source code/`、`.tools/`、`publish/` 是 ignored/local-only，但不是随便能删的垃圾。

`codex-app-better-token-main.zip` 是一套 Codex 工作流/状态模板，不是代码依赖。它里面有自己的 `AGENTS.md`、`PROMPTS.md` 和 `harness/` 模板。**不能直接覆盖 root `AGENTS.md`**，因为当前 repo 的 `AGENTS.md` 已经包含 StS2、BaseLib、manifest、source evidence、release validation 的硬规则。正确做法是“薄接入”：把它变成 `docs/codex-harness/` 模板或很薄的 `harness/` 状态文件，长期事实仍然回指 `PROJECT_STATE.md`、`docs/PROJECT_MAP.md`、`docs/issues.md`、`docs/codex-workflow.md`。

`STS2-RitsuLib.0.3.10.variant-pack.zip` 是当前本机安装的运行时库包。RitsuLib 官方 README/文档建议 mod 项目通过 `PackageReference Include="STS2.RitsuLib"` 编译引用，并在 manifest 里声明运行时依赖 `{ "id": "STS2-RitsuLib" }`；variant-pack 是给玩家安装到 `mods/STS2-RitsuLib/` 的运行时包，root DLL 是 loader，真实 API build 在 `lib/<api-version>/` 下。([RitsuLib][1]) ([GitHub][2])

当前 repo 目标是 **Slay the Spire 2 v0.106.1 + BaseLib v3.1.4**，官方 RitsuLib `v0.3.10` variant pack 已确认包含 `0.106.1` 变体。当前 `EZMicroBalance.json` 已声明 `STS2-RitsuLib >= 0.3.2`，但 runtime smoke 仍必须等 fresh `godot.log` 证明后才算通过。repo 当前目标和命令状态在 `PROJECT_STATE.md`/README 里也写得很清楚。

## 我建议的执行顺序

### 1. 先锁 baseline，不要马上动文件夹

先开分支，例如：

```powershell
git checkout -b refactor/integrate-harness-ritsulib-cleanup
git status --short --branch
git log -1 --oneline --decorate
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
.\scripts\report-worktree-batches.ps1 -FailOnUnclassified
```

repo 当前可用命令包括 `dotnet build`、`dotnet publish`、`dotnet test`、`dotnet format ... --verify-no-changes` 和 `git diff --check`；测试 README 也明确普通测试会跳过依赖 ignored package/smoke artifacts 的测试，release artifact 测试要单独开环境变量。

### 2. Codex harness：只做“薄接入”

不要这样做：

```text
直接复制上传包：
AGENTS.md  -> 覆盖 root AGENTS.md  ❌
PROMPTS.md -> 丢到 root            ⚠️
harness/   -> 长期状态重复          ⚠️
```

推荐这样做：

```text
docs/codex-harness/
  README.md
  PROMPTS.md
  templates/
    TASK_FOCUS_PACK.md
    TASK_STATUS.md
    TASK_RESULT.md
    ERROR_LEDGER.md

harness/
  README.md
  TASK_FOCUS_PACK.md
  TASK_STATUS.md
  TASK_RESULT.md
  ERROR_LEDGER.md
```

`harness/` 只放“当前任务状态”，不要放长期项目事实。长期事实仍然在：

```text
AGENTS.md
PROJECT_STATE.md
docs/README.md
docs/PROJECT_MAP.md
docs/issues.md
docs/worktree-cleanup-audit.md
docs/features/*/README.md
```

这能达到你想要的“让 Codex 少读无关上下文、省 token”的目标，同时不会让项目再长出一套重复文档系统。

### 3. RitsuLib：先作为 runtime companion staging

先把官方 RitsuLib variant pack 解压到游戏 mods 目录，而不是 repo（当前本机已安装 `v0.3.10`）：

```text
<GameRoot>/mods/STS2-RitsuLib/
  STS2-RitsuLib.dll
  mod_manifest.json
  ritsulib-variants.json
  lib/
    0.103.2/
    0.105.1/
    0.106.1/
```

然后在 repo 里只加文档记录，例如：

```text
docs/integrations/ritsulib.md
docs/PROJECT_MAP.md 更新一行
docs/codex-workflow.md 更新一行
```

当前 manifest 必须保留稳定 id `EZMicroBalance`，并且当前 runtime dependency 只允许 `STS2-RitsuLib`；不要重新加入 BaseLib，也不要仅因为本机 runtime pack 更新就提高 minimum version：

```json
"dependencies": [
  {
    "id": "STS2-RitsuLib",
    "min_version": "0.4.28"
  }
]
```

除非后续 RitsuLib API 使用确实需要更高版本并完成 build/test/publish/package/runtime smoke。当前 manifest/package 版本是 `v0.1.0-private-beta.91`。

### 4. 文件夹重构：先 move-only，再行为迁移

我建议不要重命名 root 兼容面，只重构内部目录。目标结构可以是：

```text
EZMicroBalanceCode/
  MainFile.cs
  Core/
    Config/
    Features/
    Integrations/
      RitsuLib/
    Logging/
    Multiplayer/
  Ancients/
    Common/
    Rebalance/
    Expansion/
      Urda/
      Morvi/
      Lotha/
      Vakuu/
    Patches/
  Ascension/
    Core/
    Map/
    Combat/
    Rewards/
    Cards/
    Powers/
    Relics/
    Events/
    Ui/
    Save/
    Patches/
  Preview/
```

当前源码已经有较清楚的模块图：`MainFile.cs` 是入口，`Core/Features/` 管启动顺序，`Ancients/`、`Ascension/`、`Preview/` 分别负责 Ancient、A11-A20 和预览工具。这个结构可以继续沿用，只是把 shared/integration/UI/save 等边界再明确一点。

尤其注意：当前项目有 **137 个 Harmony patch 声明，其中 22 个高风险 patch**，patch 边界文档明确说 patch 不是 live UI/save-load/co-op 证明；涉及 run、room、save、lobby、multiplayer、lifecycle 的 patch 都是 release-sensitive。也就是说，RitsuLib patcher 迁移不能和大规模文件夹移动混在同一个 PR 里。

### 5. RitsuLib 真迁移：从低风险入口开始

等版本问题解决后，再进入硬依赖：

```xml
<PackageReference Include="STS2.RitsuLib" Version="0.3.2" PrivateAssets="All" />
```

然后 manifest 加：

```json
{ "id": "STS2-RitsuLib", "min_version": "0.3.2" }
```

RitsuLib 官方文档列出的主入口包括 `CreateContentPack`、`CreatePatcher`、`SubscribeLifecycle<TEvent>`、`BeginModDataRegistration/GetDataStore`、`RegisterModSettings`，这些适合逐步替换 scattered helpers，而不是一次性替换所有 Harmony 和 BaseLib 用法。([RitsuLib][3])

迁移顺序建议：

```text
第一批：RitsuLib bootstrap / diagnostics / optional settings page
第二批：未来新增内容的 registration，不动现有高风险内容
第三批：persistence sidecar 小实验，不碰当前 30 SavedSpireFields
第四批：低风险 patch wrapper
第五批：高风险 run/map/reward/save/multiplayer patch，必须等 manual evidence backlog 降下来
```

当前 `MainFile.Initialize()` 是直接 `Harmony.PatchAll()`、注册 BaseLib config、启动 feature registry；这意味着你可以先加一个 `Core/Integrations/RitsuLib/` 模块，但不要马上拆掉原 Harmony 启动链。

## 哪些东西可以处理，哪些不要碰

| 类型                                                         | 处理方式                                                                                        |
| ---------------------------------------------------------- | ------------------------------------------------------------------------------------------- |
| `*.cs.uid`、`tests/**/TestResults/`、`bin/`、`obj/`、`.godot/` | 可以清理，已经在 `.gitignore`。                                                                      |
| `*.zip`、`*.dll`、`*.pck`                                    | 不要提交到 repo；RitsuLib DLL/PCK/ZIP 应该在游戏 mods 或 ignored local 目录。                              |
| `source code/`                                             | 先保留。当前 docs/tests 把它当本地 source evidence。                                                    |
| `publish/`                                                 | 先保留当前 package output；只用脚本删旧 beta 包。                                                         |
| `.tools/`                                                  | 不要 wholesale delete；里面可能有 runtime evidence、art provenance、Godot/ILSpy/GDRE 工具和本地 archive。   |
| `docs/archive/`                                            | 不要删；用来收纳历史 prompt、audit、superseded spec。当前 docs 也要求历史材料进 archive，而不是留在 active reading path。 |
| root `AGENTS.md`                                           | 不要覆盖。只把上传包里的 AGENTS 当模板归档。                                                                  |
| `EZMicroBalance*` 命名                                       | 不要改，除非你另开 manifest migration 计划。                                                            |

清理时用已有脚本：

```powershell
.\scripts\prune-generated-sidecars.ps1 -DryRun
.\scripts\prune-generated-sidecars.ps1

.\scripts\prune-stale-publish-packages.ps1 -DryRun
.\scripts\prune-stale-publish-packages.ps1
```

不要用：

```powershell
git clean -fdX
git clean -fdx
git reset --hard
git restore .
```

repo 的 cleanup audit 明确说 broad ignored-file deletion 不安全，因为 ignored dry run 会包含 `.godot/`、`.tools/`、`Directory.Build.props`、`publish/`、`source code/`、test `bin/obj` 等仍然有用途的本地面。

## 最推荐的 PR/commit 切法

```text
PR 1: baseline + docs-only Codex harness integration
PR 2: RitsuLib staging docs + install instructions + version mismatch record
PR 3: move-only source folder refactor, no behavior changes
PR 4: test/docs/script path updates after move-only refactor
PR 5: RitsuLib hard dependency, only after 0.106.1/0.106.1 decision
PR 6: low-risk RitsuLib API adoption
PR 7+: high-risk patch migrations, one feature surface at a time
```

这样做的好处是：任何一步坏了，都能知道是“路径移动坏了”“依赖坏了”“Ritsu migration 坏了”还是“行为坏了”。

## 直接给 Codex 的执行提示词

```text
请在 dev-the-spire 中执行一个 no-behavior integration/refactor 预备阶段。

约束：
1. 不要覆盖 root AGENTS.md。
2. 不要改名 EZMicroBalance manifest id、project、resource folder、code folder、DLL、PCK、install folder。
3. 不要提交 DLL/PCK/ZIP、本地工具、runtime evidence 或 build output。
4. 先记录 RitsuLib 版本阻塞：当前 repo target 是 StS2 v0.106.1；上传的 RitsuLib variant pack 有 0.103.2、0.105.1、0.106.1，没有 0.106.1。
5. 这一步只允许 docs/harness/staging/refactor map，不允许改玩家可见行为。

步骤：
A. 读取 AGENTS.md、PROJECT_STATE.md、docs/README.md、docs/PROJECT_MAP.md、docs/worktree-cleanup-audit.md、docs/codex-workflow.md。
B. 运行 git status --short --branch、git log -1 --oneline --decorate、dotnet build、dotnet test --no-build、dotnet format --verify-no-changes、git diff --check、scripts/report-worktree-batches.ps1 -FailOnUnclassified。
C. 把上传的 Codex harness 作为 docs/codex-harness 模板接入，不覆盖 root AGENTS.md，不重复长期项目事实。
D. 写 docs/integrations/ritsulib.md，说明 runtime 安装位置、variant pack 内容、0.106.1/0.106.1 阻塞、暂不改 manifest dependency。
E. 产出 move-only folder refactor map，不实际移动高风险文件。
F. 更新 PROJECT_STATE/docs/PROJECT_MAP/docs/README 中必要索引。
G. 最后重新跑 build/test/format/diff-check，并汇报实际文件、命令结果、阻塞和下一步。
```

我的建议很明确：**先减“阅读复杂度”和“状态重复”，不要先删大目录；先 staging RitsuLib，不要先硬依赖；先 move-only 重构，不要把行为迁移混进去。**

[1]: https://sts2-ritsulib.ritsukage.com/guide/getting-started "Getting Started - RitsuLib"
[2]: https://github.com/BAKAOLC/STS2-RitsuLib "GitHub - BAKAOLC/STS2-RitsuLib: A shared framework library for Slay the Spire 2 mods. · GitHub"
[3]: https://sts2-ritsulib.ritsukage.com/guide/framework-design "How RitsuLib Is Organized - RitsuLib"
