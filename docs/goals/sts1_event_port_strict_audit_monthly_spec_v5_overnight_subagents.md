# StS1 Event Port 严格审核 v5 — Monthly Dev Spec + Overnight Run + Subagent 工作单
结论：**没有完成。**这次最多只能认定为 **registration/infrastructure 有进展，但 StS1 事件体验迁移没有完成**。我已经把新版严格审核、monthly dev spec、subagent 工作单，以及“必须跑完才能停止”的 overnight run 写成文档：

Legacy sandbox export link removed. Use `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md` for current StS1 event guidance.

## 核心审核结论

他现在声称做了 `Sts1EventRegistrationService`，并把 `Sts1EventRegistrationService.RegisterAll(ModId)` 接进 `MainFile.Initialize()`；这确实是一个工程进展，但也是一个风险点，因为它看起来是**默认无条件注册**，会污染现有 Spire Plus 默认体验。 当前项目仍是单一 `Spire Plus` mod，技术 manifest id 仍是 `EZMicroBalance`，代码和资源目录应保持在 `EZMicroBalanceCode/` 与 `EZMicroBalance/`。

他把 `Underdocks=Act1, Overgrowth=Act2, Hive=Act3` 写进注册服务注释/发现结论，这是严重问题。按你上传的 StS2 v0.106.0 source 检查，正确方向应是：

| StS1 bucket | StS2 ActModel               |
| ----------- | --------------------------- |
| Act 1       | `Overgrowth` + `Underdocks` |
| Act 2       | `Hive`                      |
| Act 3       | `Glory`                     |

如果他按错误映射注册，Act 2 事件会跑到 StS2 Act 1，Act 3 事件会跑到 StS2 Act 2，真正 Act 3 的 `Glory` 反而缺 StS1 Act 3 事件。这会直接造成你说的“和杀戮尖塔 1 游戏体验出入很大”。

他声称 `build succeeds — 0 errors`，但这最多只能标为 **Build Claimed**，不能标为“完成”。必须要求完整 unfiltered build log 和 exit code；不能只看 `tail`，也不能用 `grep -v` 过滤错误。

更大的问题是状态口径不可信。他把 48 个 spec、文档、localization、assets、test-plan 都标成 Done，但 blocker 里还列着 Regret、Injury、random relic helper、card UI、combat encounter models。 这说明当前状态不能叫 Done，只能叫 `spec-drafted / build-claimed / blocked / pending-manual-evidence`。

## 是否完成：逐项判定

| 模块                   | 判定                                                                                                                                                                         |
| -------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Wiki catalog         | **未通过**。StS1 Wiki 事件体系应按 16 shared、12 Act 1、16 Act 2、8 Act 3 管理，并区分 shared/semi-common/act-exclusive；Act 4 没有 unknown/event，A15 会强化部分不利事件。([slay-the-spire.fandom.com][1]) |
| 46/48/52 口径          | **未通过**。必须建立 `wiki_event_entries / runtime_event_models / act_bucket_memberships` 三栏 matrix。                                                                               |
| Event spec           | **草稿级**。不能因文件存在就标 Done。                                                                                                                                                    |
| C# event models      | **未完成**。能编译不等于 playable，不等于 StS1 parity。                                                                                                                                   |
| Registration service | **部分完成但危险**。默认无条件 `RegisterAll` 必须撤掉或 feature-gate。                                                                                                                        |
| Act mapping          | **严重错误/必须优先修**。错误 mapping 会直接破坏体验。                                                                                                                                         |
| Canary events        | **未完成**。Big Fish / Golden Idol 关键依赖仍未闭环。                                                                                                                                   |
| Assets               | **未完成**。脚本不是图片完成；必须有 image map + extraction proof + 截图。                                                                                                                    |
| Localization         | **未完成**。JSON 存在不等于游戏内 EN/ZHS 渲染正确。                                                                                                                                         |
| StS1-only event pool | **未完成**。RitsuLib additive 注册不等于替换 unknown room pool。                                                                                                                       |
| Full parity          | **0%**。不能宣称“和杀戮尖塔 1 完全一样”。                                                                                                                                                 |

Big Fish 必须严格实现 Banana 回复 `floor(maxHP / 3)`、Donut `Max HP +5`、Box 随机 common/uncommon/rare relic + Regret；Golden Idol 必须实现 Take/Leave、获得 Golden Idol、Outrun 给 Injury、Smash 25%/35% max HP 伤害、Hide 8%/10% max HP 损失。([slay-the-spire.fandom.com][2]) 这些 canary 没有全部 playable + save/load + screenshot proof 前，不能继续说 Phase 2 Done。

## 下一步 Monthly Dev Spec：June 2026

月目标名称必须改成：

**`StS1 Event Port Prototype Batch 1 — Parity Foundation`**

不是 full parity，不是 all events complete。

月末验收标准：

1. 默认 Off，对 Spire Plus 零影响。
2. `CanaryOnly` 只注册 Big Fish / Golden Idol / Lab / Divine Fountain。
3. 修正 Act mapping：`Overgrowth + Underdocks = Act 1`，`Hive = Act 2`，`Glory = Act 3`。
4. 完整 `dotnet build --no-restore` unfiltered exit code 0。
5. 四个 canary playable、可 save/load、有图片和 EN/ZHS 渲染截图。
6. 六个 simple batch playable：Purifier、Upgrade Shrine、Golden Shrine、The Cleric、Old Beggar/Pleading Vagrant canonical mapping、Shining Light。
7. 做 debug-only `ReplaceUnknownEventsPrototype`，证明 unknown room 不抽 StS2 原事件。
8. 所有状态禁止泛泛写 `Done`，必须改成证据状态。
9. QA subagent 独立验收；实现者不能自验。

项目规则也要求代码/资源改动后跑相应验证，资源、本地化、打包改动后要 publish，并更新文档/版本/package metadata；同时不能随意复制原版素材或大段反编译代码，原版 art 只有授权确认后才能进入 tracked/public files。

## Overnight Run：让他跑完才能停止

我在文档里设定了 **Overnight Exit Gates O0-O12**。核心规则是：

**他不能在“48 specs Done / 46 models Done / registration done / build passes / localization files exist / asset scripts exist”时停止。**

允许停止的唯一条件：

1. O0-O12 全部 GREEN；或
2. 出现 hard stop blocker，并写完整 blocker report。

Overnight gates 摘要：

| Gate | 必须结果                                                                     |
| ---- | ------------------------------------------------------------------------ |
| O0   | Worktree/diff/file list 记录完成                                             |
| O1   | full unfiltered build exit code 0                                        |
| O2   | 移除所有无证据 `Done`                                                           |
| O3   | 52 wiki entries / runtime models / act memberships matrix 完成             |
| O4   | Act mapping 修正并测试                                                        |
| O5   | Off / CanaryOnly / AdditiveBatch1 / ReplacementPrototype feature gate 完成 |
| O6   | CanaryOnly 只注册 4 个事件                                                     |
| O7   | Big Fish / Golden Idol / Lab / Divine Fountain playable                  |
| O8   | 四个 canary 有截图、result log、save-load proof                                 |
| O9   | 六个 simple batch scope/spec/API/dependency ready                          |
| O10  | debug replacement pool 不抽 StS2 原事件                                       |
| O11  | QA Red-Team pass/fail 报告                                                 |
| O12  | status-board、monthly review、README/PROJECT_MAP 更新                        |

## 给他的直接指令

## 2026-06-11 Revision M Current Override

This v5 audit/spec is historical planning context only. Do not use its O0-O12 overnight gates, old registration assumptions, or old task scope as current `event.md` guidance. Current StS1 event work routes through `docs/goals/event.md`, `docs/features/sts1-events/v19-gate-evidence-map.md`, `docs/features/sts1-events/v19-gate-ledger.csv`, `docs/features/sts1-events/v20-final-gate-overlay.csv`, `docs/features/sts1-events/hard-stop-blocker-report-v20-coordination-pause-20260617.md`, `PROJECT_STATE.md`, and `docs/test-ready-development-goal.md`: beta.85 proves retained `v0.107.0` default-Off and CanaryOnly loader behavior, beta.87 proves retained `v0.107.0` AdditiveBatch1 loader/registration behavior, current `v0.107.1` loader proof needs recapture, and gameplay, save-load, replacement, multiplayer, QA, handoff, and release-ready proof remain pending or blocked.

```text
当前工作没有完成。不要再用“46 event models Done / 48 specs Done / build passes”推进 Phase 2。

从现在开始跑 Overnight Run。你不能在只完成 spec、model、registration、localization 文件、asset script、或单次 build pass 后停止。

必须启动 subagents：
1. BuildGate / Repo Health
2. StS2 Source/API Auditor
3. Wiki Parity Spec Auditor
4. Feature Gate / Registration Engineer
5. Canary Gameplay Engineer
6. Content Parity
7. Event Pool / RNG / Save
8. Asset + Localization
9. QA / Red-Team
10. Release Documentation

Overnight Exit Gates O0-O12 必须全绿才能停止。Hard stop 只能用于不可继续的 blocker，并必须输出 blocker report。

最高优先级：
1. full unfiltered dotnet build exit code 0，保存完整 log；grep/tail 不算。
2. 修正 Act mapping：Overgrowth + Underdocks = StS1 Act 1，Hive = StS1 Act 2，Glory = StS1 Act 3。
3. 移除默认无条件 RegisterAll；StS1 events 默认 Off。
4. 建立 52 wiki entries / runtime models / act memberships canonical matrix。
5. 先完成 Big Fish、Golden Idol、Lab、Divine Fountain playable + image/loc render + save/load。
6. 做 debug-only ReplacementPrototype，证明 unknown room 不再抽 StS2 原事件。
7. 任何没有 source/API/test/screenshot/log/save-load 证据的内容不得标 Done。
8. RitsuLib registration 只是 additive，不等于杀戮尖塔1事件体验。
```

最重要的管理判断：**不要让他继续堆更多空壳事件。先让他 overnight run 跑绿 build、Act mapping、feature gate、canary、asset/loc proof、replacement pool；这些没跑完前，不准停止，也不准再写 Done。**

[1]: https://slay-the-spire.fandom.com/wiki/Events "Events | Slay the Spire Wiki | Fandom"
[2]: https://slay-the-spire.fandom.com/wiki/Big_Fish "Big Fish | Slay the Spire Wiki | Fandom"

日期：2026-05-28
对象：Devspire / `dev-the-spire` / Spire Plus (`EZMicroBalance`) 中的 StS1 events migration 工作

---

## 0. 审核结论

**没有完成。**

当前工作可以承认为：

- 已有一批 `Sts1Events` 代码文件与文档草稿；
- 已写 `Sts1EventRegistrationService`；
- 已把 `Sts1EventRegistrationService.RegisterAll(ModId)` 接入 `MainFile.Initialize()`；
- 声称 `dotnet build --no-restore` 现在 0 errors；
- 声称有 46 个 event C# model、48 个 spec doc、EN/ZHS localization、asset scripts、manifest。

但这不能被验收为“杀戮尖塔 1 事件迁移完成”，甚至不能被验收为“Batch 1 playable”。原因：

1. **事件体验不是 StS1 parity**：RitsuLib 注册是 additive，不是 StS1-only unknown room pool。
2. **Act mapping 明显有误/至少未 source-backed**：他写了 `Underdocks=Act1, Overgrowth=Act2, Hive=Act3`，这会把事件放错章节；本地 v0.106.0 source 显示 `Overgrowth` 和 `Underdocks` 都是 Act 1 风格，`Hive` 是 Act 2，`Glory` 是 Act 3。
3. **默认无条件注册危险**：`RegisterAll(ModId)` 直接接入 `MainFile.Initialize()`，会污染现有 Spire Plus 默认体验。
4. **46 / 48 / 52 口径混乱**：缺少 canonical matrix，不知道哪些是 Wiki 条目、哪些是 runtime model、哪些是 act bucket membership。
5. **`Done` 状态词滥用**：spec、localization、assets、test-plan、canary source 被标 Done，但无 in-game proof / screenshot / save-load / event result log。
6. **关键内容仍缺失**：Regret、Injury、Golden Idol relic、random relic helper、HP/max HP command helper、card remove/upgrade/transform UI、combat event encounters、event pool save/load。
7. **StS1 游戏体验差异非常大**：Reward/card/relic/potion pool、curse equivalence、A15 modifiers、条件过滤、event bag/no-repeat、图片/文本节奏都未闭环。

---

## 1. 逐步审核

| Step | 他声称 | 审核判定 | 必须修正 |
|---|---|---|---|
| 1. Wiki/event catalog | 52 events / 48 unique specs | **未通过**。必须拆成 `wiki_event_entries`、`runtime_event_models`、`act_bucket_memberships`。 | 建立 canonical matrix，禁止再笼统说 48 covers all。 |
| 2. Event specs | 48 specs Done | **只算 draft**。没有逐事件 source proof、exact options、A15、dependency、StS2 command mapping 的 spec 不能标 Done。 | 每个 spec 必须有 source-backed option table + implementation checklist。 |
| 3. C# event models | 46 models Done | **未通过**。空壳/半成品不等于 playable。 | 每个 model 必须通过 debug spawn、结果校验、save/load 后才算 implemented。 |
| 4. Registration service | Done | **部分完成但高风险**。无条件注册 + act mapping 需要修。 | 默认 Off；只允许 gated modes；修正 Act mapping。 |
| 5. Build | 声称 0 errors | **只能标 Build Claimed**。必须保存 full unfiltered log + exit code。tail/grep 不算。 | `dotnet build --no-restore *> evidence/build.log`，记录 `$LASTEXITCODE`。 |
| 6. Localization | 46 EN/ZHS Done | **未通过**。JSON 存在不等于游戏内渲染正确。 | 每个 playable event 需要 EN/ZHS screenshot。 |
| 7. Assets | scripts Done | **未完成**。script 不是图片。 | 52-entry asset map + local extraction + file existence + event-screen screenshot。 |
| 8. Canary events | Big Fish / Golden Idol source done | **未完成**。缺 Regret/Injury/random relic/Golden Idol exact effect proof。 | 先完成 Big Fish、Golden Idol、Lab、Divine Fountain。 |
| 9. StS1-only pool | 未见实现 | **0%**。RitsuLib additive 不等于替换池。 | 做 debug-only `ReplaceUnknownEventsPrototype`。 |
| 10. Release/readiness | 暗示可继续 Phase 2 | **不允许**。当前应回到 parity foundation。 | 禁止继续堆空壳；先修 foundation gates。 |

---

## 2. 体验差异根因

用户反馈“事件和杀戮尖塔 1 的游戏体验出入很大”是合理的。根因不是事件数量不足，而是系统层没有复刻：

1. **章节映射错**：Act 2/Act 3 事件可能出现在错误 StS2 act。
2. **事件池混合**：StS1 事件和 StS2 原事件混在 unknown room pool 里。
3. **没有 StS1 event bag/no-repeat/save-load**：事件抽取节奏不像原作。
4. **Reward pool 不同**：StS2 relic/card/potion pool 与 StS1 不同，事件价值判断会改变。
5. **Curse/relic/card 临时代用品破坏 parity**：`Parasite -> Clumsy`、`Madness -> Debt` 只能算 prototype substitute，不能算完成。
6. **A15 未闭环**：StS1 A15 会改变部分不利事件的概率或强度。
7. **条件过滤未闭环**：如 Divine Fountain 必须有 curse 才能出现，很多事件有金币、牌组、遗物条件。
8. **图片/文本/页面节奏未验证**：事件体验包括图片、选项锁、动态数值、死亡提示、hover、分页。

---

## 3. 正确完成定义

以后每个事件只允许这些状态：

```text
planned
spec-drafted
wiki-verified
api-verified
dependency-ready
implemented
asset-verified
loc-render-verified
manual-verified
save-load-verified
blocked
```

禁止使用泛泛的 `Done`。

事件只有同时满足以下条件，才能进入 `manual-verified`：

- Wiki spec 有 exact option table；
- A15 差异已写明；
- dependencies 已列出并验收；
- StS2 API/command mapping 已验证；
- event model 可 debug spawn；
- 每个选项能执行并产生正确结果；
- EN/ZHS 游戏内渲染截图；
- 图片加载截图；
- save/load 后状态正确；
- release note 不夸大为 full parity。

---

## 4. Monthly Dev Spec — June 2026

### 名称

`StS1 Event Port Prototype Batch 1 — Parity Foundation`

### 截止

2026-06-30

### 非目标

- 不叫 full parity。
- 不叫 all events complete。
- 不继续批量堆 46 个空壳。
- 不把 RitsuLib registration 当作 StS1-only event pool。
- 不把 substitute content 当作 exact parity。

### 月末 Must Pass

1. **默认 Off**：不开 feature flag 时，对 Spire Plus 零行为影响。
2. **安全注册**：`CanaryOnly` 只注册 Big Fish / Golden Idol / Lab / Divine Fountain。
3. **Act mapping 修正**：
   - StS1 Act 1 -> `Overgrowth` + `Underdocks`
   - StS1 Act 2 -> `Hive`
   - StS1 Act 3 -> `Glory`
4. **完整 build 绿灯**：unfiltered `dotnet build --no-restore` exit code 0。
5. **四个 canary playable**：
   - Big Fish
   - Golden Idol
   - Lab
   - Divine Fountain
6. **六个 simple batch playable**：
   - Purifier
   - Upgrade Shrine
   - Golden Shrine
   - The Cleric
   - Old Beggar / Pleading Vagrant canonical mapping
   - Shining Light
7. **debug-only replacement prototype**：`ReplaceUnknownEventsPrototype` 能证明 unknown room 不抽 StS2 原事件。
8. **证据完整**：build/test/publish logs、screenshots、save-load proof、status-board、monthly review。
9. **QA subagent 独立验收**：实现者不能自己给自己盖章。

---

## 5. Weekly Plan

### Week 0 — 2026-05-28 至 2026-05-31

目标：停止错误方向。

必须完成：

- 修 `status-board.md`：移除所有无证据 `Done`。
- 新建 `wiki-event-canonical-matrix.md`。
- 新建 `source-research/sts2-act-mapping.md`。
- 新建 `source-research/api-command-matrix.md`。
- 从默认初始化移除无条件 `RegisterAll(ModId)`。
- 记录 full build 状态；如果仍有 Vakuu blocker，作为 repo health blocker 修复或隔离。

验收：

- 46/48/52 口径解释清楚。
- Act mapping 已修。
- 默认 Off 注册 0 个 StS1 event。
- 不再宣称 full parity。

### Week 1 — 2026-06-01 至 2026-06-07

目标：Feature gate + clean build。

必须完成：

- `Sts1EventFeatureGate`
- `Sts1EventRegistrationMode`
  - `Off`
  - `CanaryOnly`
  - `AdditiveBatch1`
  - `ReplaceUnknownEventsPrototype`
- registration count tests
- act bucket tests
- full build evidence

验收：

- Off = 0 registrations。
- CanaryOnly = 4 registrations。
- Act 1 event 同时注册到 Overgrowth + Underdocks。
- Act 2 event 注册到 Hive。
- Act 3 event 注册到 Glory。
- `dotnet build --no-restore` unfiltered 0 errors。

### Week 2 — 2026-06-08 至 2026-06-14

目标：四个 canary 真正可玩。

必须实现：

- `Sts1HpService`
- `Sts1RewardService`
- `Sts1CurseService`
- `Sts1RelicService`
- `Sts1AscensionRules`
- Regret / Injury exact model 或 StS1 custom-compatible model
- Golden Idol exact relic 或 StS1 custom-compatible relic
- random relic reward helper
- potion reward helper
- curse removal helper

验收：

- Big Fish 三选项完全正确。
- Golden Idol 全分支 + A15 完全正确。
- Lab 药水奖励正确或 source-backed equivalent。
- Divine Fountain 只在有 curse 时出现并移除所有 curse。
- 四事件都有 debug spawn、截图、result log、save-load proof。

### Week 3 — 2026-06-15 至 2026-06-21

目标：Simple Batch 1 playable。

实现：

- Purifier
- Upgrade Shrine
- Golden Shrine
- The Cleric
- Old Beggar / Pleading Vagrant canonical mapping
- Shining Light

验收：

- 每个事件 exact options。
- 每个事件动态数值正确。
- EN/ZHS render proof。
- image render proof。
- save/load proof。
- status-board 更新为证据状态。

### Week 4 — 2026-06-22 至 2026-06-28

目标：修复“体验不像 StS1”的核心。

必须完成：

- `Sts1EventPoolService`
- `ReplaceUnknownEventsPrototype`
- event bag saved fields
- visited ids saved fields
- condition filters
- no-repeat proof
- multiplayer fail-closed

验收：

- Replacement mode 下 unknown room 不抽 StS2 原事件。
- 默认关闭。
- save/load 后 event bag 不乱序、不重复、不跨 act。
- multiplayer 默认 fail-closed。

### Week 5 — 2026-06-29 至 2026-06-30

目标：月末收口。

必须完成：

- `monthly-review-2026-06.md`
- updated feature README
- updated PROJECT_MAP/docs index
- evidence folder
- version bump if player-visible behavior shipped
- package/release note 只能写 Prototype Batch 1

验收：

- 所有 blocker 明确列出。
- QA subagent pass。
- commit + push 只能在验证通过后执行。

---

# 6. Overnight Run Spec — 必须跑完才能停

## 6.1 运行目标

这是一次 **overnight run**，目标不是“写更多事件”，而是把 parity foundation 跑到可验收状态。

**不能在只完成 spec、只完成 model、只完成 registration、或只 build pass 时停止。**

允许停止的唯一条件：

1. 所有 Overnight Exit Gates 全部 GREEN；或
2. 出现 Hard Stop Blocker，且已输出完整 blocker report、失败命令、日志路径、根因假设、下一步最小修复。

## 6.2 Overnight Exit Gates

| Gate | 名称 | 必须结果 |
|---|---|---|
| O0 | Worktree gate | 当前分支、diff、文件清单记录完成，无无关改动。 |
| O1 | Build health | `dotnet build --no-restore` unfiltered exit code 0；日志保存。 |
| O2 | Status truth | 所有无证据 `Done` 被替换为证据状态。 |
| O3 | Canonical matrix | 52 wiki entries / runtime models / act memberships 映射完成。 |
| O4 | Act mapping | Overgrowth+Underdocks/Hive/Glory 映射修正并测试。 |
| O5 | Feature gate | Off/CanaryOnly/AdditiveBatch1/ReplacementPrototype 实现；默认 Off。 |
| O6 | Canary compile | CanaryOnly 模式只注册 4 个 canary。 |
| O7 | Canary gameplay | Big Fish / Golden Idol / Lab / Divine Fountain playable。 |
| O8 | Canary evidence | 四事件截图、result log、save-load proof。 |
| O9 | Simple batch scope | 六个 simple event 至少 spec/API/dependency ready；实现优先级明确。 |
| O10 | Replacement prototype | debug-only pool replacement 能证明不抽 StS2 原事件。 |
| O11 | QA red-team | 独立 QA subagent pass/fail 报告。 |
| O12 | Handoff docs | monthly spec、status board、README/PROJECT_MAP 更新。 |

## 6.3 Overnight Loop

他必须按下面循环执行，直到 O0-O12 全绿或 hard stop：

```text
while not all_exit_gates_green:
    run BuildGate subagent
    if build fails:
        diagnose root cause
        patch smallest safe fix
        rerun full unfiltered build
        continue

    run Source/API Auditor subagent
    patch act mapping and API misuse
    rerun build and registration tests

    run FeatureGate/Registration subagent
    enforce default Off and CanaryOnly
    rerun build and count tests

    run Wiki Spec Auditor subagent
    repair canonical matrix and status board
    rerun doc tests

    run Canary Gameplay subagent
    implement missing services and four canaries
    rerun build, debug spawn tests, save/load tests

    run Asset/Localization subagent
    validate images and EN/ZHS render
    capture screenshots

    run EventPool/RNG/Save subagent
    implement debug replacement prototype
    verify no StS2 original events in replacement mode

    run QA/Red-Team subagent
    reject any unsupported Done
    reject any substitute marked parity
    record blocker list

    run Release Documentation subagent
    update monthly-review/status-board/docs index
```

## 6.4 Overnight Command Evidence

必须保存以下证据：

```powershell
mkdir .tools/runtime-evidence/sts1-events-overnight-202606

dotnet build --no-restore *> .tools/runtime-evidence/sts1-events-overnight-202606/build.log
echo $LASTEXITCODE > .tools/runtime-evidence/sts1-events-overnight-202606/build.exitcode.txt

dotnet test --no-restore *> .tools/runtime-evidence/sts1-events-overnight-202606/test.log
echo $LASTEXITCODE > .tools/runtime-evidence/sts1-events-overnight-202606/test.exitcode.txt

dotnet publish --no-restore *> .tools/runtime-evidence/sts1-events-overnight-202606/publish.log
echo $LASTEXITCODE > .tools/runtime-evidence/sts1-events-overnight-202606/publish.exitcode.txt
```

如果某命令因已知环境限制不能执行，必须写：

```text
command:
reason not run:
evidence gap created:
next unblock step:
owner:
```

## 6.5 禁止停止条件

以下情况不允许停止：

- “48 specs Done”
- “46 models Done”
- “RitsuLib registration done”
- “build passes”
- “localization files exist”
- “asset scripts exist”
- “StS1 events appear in event pool”
- “canary source done”

这些都只是中间态，不是 overnight exit。

## 6.6 Hard Stop Blocker 模板

只有写出以下报告，才能因 blocker 停止：

```text
Hard Stop Blocker:
Gate blocked:
Command/output:
Files touched:
Root cause:
Why cannot proceed safely:
Smallest next patch:
Subagent owner:
Evidence path:
```

---

# 7. Subagent 工作单

## 7.1 BuildGate / Repo Health Subagent

目标：让 full unfiltered build 可信。

任务：

- 不允许 `grep -v`。
- 不允许只看 `tail`。
- 保存完整 build log 和 exit code。
- 如果 Vakuu 或其他历史错误存在，修复或明确隔离。
- 给出 pass/fail。

输出：

```text
build.log
build.exitcode.txt
repo-health-report.md
```

## 7.2 StS2 Source/API Auditor Subagent

目标：防止错用 API 和错配 Act。

任务：

- 验证 `Overgrowth`、`Underdocks`、`Hive`、`Glory` 的 act role。
- 验证 RitsuLib `SharedEvent` / `ActEvent` 行为。
- 验证 HP、max HP、relic obtain、curse add/remove、potion、card remove/upgrade/transform、save fields API。
- 输出 `api-command-matrix.md`。

## 7.3 Wiki Parity Spec Auditor Subagent

目标：把 spec 从模板变成可编码规格。

任务：

- 建立 52-entry canonical matrix。
- 每个事件写 exact options、A15、dependencies、condition filters。
- 任何未 source-backed 的 spec 只能是 `spec-drafted`，不能 Done。

## 7.4 Feature Gate / Registration Engineer Subagent

目标：安全注册，不污染默认 Spire Plus。

任务：

- 实现 `Sts1EventFeatureGate`。
- `RegisterAll` 改成 mode-based。
- 默认 Off。
- CanaryOnly 精确 4 事件。
- 修正 Act mapping。
- 写 registration count tests。

## 7.5 Canary Gameplay Engineer Subagent

目标：四个 canary playable。

任务：

- Big Fish。
- Golden Idol。
- Lab。
- Divine Fountain。
- 补齐 hp/reward/curse/relic services。
- debug spawn + result logs + save/load。

## 7.6 Content Parity Subagent

目标：处理 StS1-specific cards/relics/monsters。

任务：

- Regret、Injury、Golden Idol、Bloody Idol。
- Parasite、Bite、Madness、Apparition 等 parity blockers。
- 每项标 `native-equivalent` / `custom-required` / `temporary-substitute` / `blocked`。
- `temporary-substitute` 不允许计入 parity。

## 7.7 Event Pool / RNG / Save Subagent

目标：修复事件池体验。

任务：

- `Sts1EventPoolService`。
- debug-only replacement mode。
- no-repeat event bag。
- visited ids。
- condition filters。
- save/load。
- multiplayer fail-closed。

## 7.8 Asset + Localization Subagent

目标：图片和文本不再停留在脚本层。

任务：

- 52-entry image mapping。
- 本地 extraction 验证。
- 每个 canary 的 event image render screenshot。
- EN/ZHS render screenshot。
- placeholder 扫描。

## 7.9 QA / Red-Team Subagent

目标：独立否决虚假完成。

任务：

- 审核所有 `Done`。
- 审核 canary 是否真可玩。
- 审核 substitute 是否被错误标 parity。
- 审核 screenshot/log/save-load evidence。
- 审核 default Off 是否真的零影响。

## 7.10 Release Documentation Subagent

目标：让 handoff 不误导测试者。

任务：

- 更新 feature README。
- 更新 status-board。
- 更新 PROJECT_MAP / docs index。
- 更新 monthly-review。
- release note 只能写 Prototype Batch 1。

---

# 8. 直接发给他的指令

```text
当前工作没有完成。不要再用“46 event models Done / 48 specs Done / build passes”推进 Phase 2。

从现在开始跑 Overnight Run。你不能在只完成 spec、model、registration、localization 文件、asset script、或单次 build pass 后停止。

必须启动 subagents：
1. BuildGate / Repo Health
2. StS2 Source/API Auditor
3. Wiki Parity Spec Auditor
4. Feature Gate / Registration Engineer
5. Canary Gameplay Engineer
6. Content Parity
7. Event Pool / RNG / Save
8. Asset + Localization
9. QA / Red-Team
10. Release Documentation

Overnight Exit Gates O0-O12 必须全绿才能停止。Hard stop 只能用于不可继续的 blocker，并必须输出 blocker report。

最高优先级：
1. full unfiltered dotnet build exit code 0，保存完整 log；grep/tail 不算。
2. 修正 Act mapping：Overgrowth + Underdocks = StS1 Act 1，Hive = StS1 Act 2，Glory = StS1 Act 3。
3. 移除默认无条件 RegisterAll；StS1 events 默认 Off。
4. 建立 52 wiki entries / runtime models / act memberships canonical matrix。
5. 先完成 Big Fish、Golden Idol、Lab、Divine Fountain playable + image/loc render + save/load。
6. 做 debug-only ReplacementPrototype，证明 unknown room 不再抽 StS2 原事件。
7. 任何没有 source/API/test/screenshot/log/save-load 证据的内容不得标 Done。
8. RitsuLib registration 只是 additive，不等于杀戮尖塔1事件体验。
```

---

# 9. 一句话决策

**不要让他继续堆更多空壳事件；让他跑 overnight run，把 build、Act mapping、feature gate、canary、asset/loc proof、replacement pool 这些 foundation gates 全部跑绿，跑完前不准停止并不准再说 Done。**
