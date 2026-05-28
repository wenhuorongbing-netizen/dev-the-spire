结论：**没有完成**。他的工作最多算“调研 + 文档/代码脚手架初稿”，不能算“杀戮尖塔 1 事件迁移完成”，也不应该进入“直接批量实现 Phase 2”的状态。

我把严格审核和下一步月度规格整理成了文件：

[下载严格审核与 June 2026 Monthly Dev Spec](sandbox:/mnt/data/sts1_event_port_strict_audit_monthly_dev_spec.md)

## 严格审核结论

### 1. 事件数量/状态板有明显问题

他在日志里说“48 个 spec 文件覆盖所有 unique events”，并声称 52 个清单条目里有 4 个是 act-specific duplicate。这个说法不严谨，也会误导后续开发。日志中还把 `Infrastructure` 标成 Done，把 `event-specs/ (48 unique events)` 标成 Done，并把 Big Fish / Golden Idol 的 Source、Loc EN、Loc ZHS 标成 Done，但 Asset 和 Test 仍是 Pending。 

Wiki 的事件范围应按 **52 个 unique Wiki event entries** 处理：16 个 Shared、12 个 Act 1 exclusive、16 个 Act 2 exclusive、8 个 Act 3 exclusive；事件来自 unknown location，按随机和当前 Act 决定，Act 4 没有 unknown location/event，Ascension 15 会让不利事件概率或强度上升。([Slay the Spire Wiki][1])

### 2. “每个 spec 包含精确效果”这个说法不成立

他的日志声称每个 spec 包含 Wiki 原始行为、每个选项精确效果、A15 差异、StS2 实现方案、localization keys、dependencies。

我检查了交付包后，实际情况是：spec 文件虽然存在，但大多数是模板级别，比如“记录 dependencies”“Normal branch 1/2”“A15 branch”这些还是空占位。它们不能作为直接编码依据。以 Big Fish 为例，Wiki 明确给出 Banana heal floor(maxHP/3)、Donut +5 max HP、Box 给随机 common/uncommon/rare relic 并加入 Regret。([Slay the Spire Wiki][2]) Golden Idol 也明确有 Take/Leave、多页 trap、Injury、25%/35% max HP damage、8%/10% max HP loss 等 A15 差异。([Slay the Spire Wiki][3]) 这些必须逐项落入 spec 表，而不是只写一句 summary。

### 3. 代码没有完成，甚至不能认为 canary 完成

交付包里的 `Sts1Events` 代码全部包在 `#if STS1_EVENT_PORT_PROTOTYPE` 下，默认不会进入现有构建。所有 52 个事件源文件仍有 TODO；多数事件只是一个 `Leave -> DONE` 的空壳。Canary 也没完成：Big Fish 的 Box 还没加 Regret，Golden Idol 没有真正获得 Golden Idol relic，也没加 Injury。日志自己也承认 blocker 仍包括 Regret、Injury、random relic helper、card removal/transform/upgrade UI、combat encounter models。

更严重的是，部分代码即使打开 prototype symbol 也有明显风险：Big Fish 从 `RelicFactory.PullNextRelicFromFront` 取 relic 后没有 `.ToMutable()` 就传给 `RelicCmd.Obtain`，这和 StS2 的 mutable model 规则冲突；Divine Fountain 用了 `c.IsCurse`，但我在上传的 StS2 v0.106.0 source 里看到的主路径是 `CardModel.Type` / `CardRarity.Curse`，不是 `IsCurse` 属性。也就是说，它不是“未验证的小问题”，而是需要先做 source/API matrix 的编译级风险。

### 4. 图片/素材没有完成

这一点他处理方向是对的：没有把 StS1 原图放进包里。项目规则也明确要求不要复制原版 StS2 非美术资产、不要复制大段反编译代码，原版美术只有在授权确认后才可进入 tracked/public files。

但“图片什么的全部都要做到”和“完全一模一样”还没有完成。当前只有 asset manifest 和 extraction script 模板，而且 source map 只填了 Big Fish / Golden Idol 两个 TODO 路径，没有 52 个事件的实际 mapping，也没有任何截图/验证证据。

### 5. 他的 next action 方向是错的

日志最后说下一步是“implement Phase 2 simple batch events in code”。

严格审核后，正确下一步不是 Phase 2，而是先回到 **Phase 0/1 修复**：

1. 纠正 52-event canonical catalog。
2. 把 status board 改成真实状态，不能虚标 Done。
3. 把 prototype symbol 打开后先确保能 build。
4. 完成 source/API command matrix。
5. 先把 Big Fish、Golden Idol、Lab、Divine Fountain 四个 canary 做到 playable + save/load + image validation。

## 当前完成度判定

| 模块                   |   审核状态 | 判定                                             |
| -------------------- | -----: | ---------------------------------------------- |
| Wiki catalog         |   部分完成 | 交付包里有 52 manifest/spec，但日志里的 48/duplicate 说法错误 |
| 文档结构                 |   部分完成 | 目录和文档框架有了，但 spec 不够细                           |
| 事件 spec              |   部分完成 | 52 个文件存在，但多数不是可编码级规格                           |
| 代码脚手架                |   部分完成 | 有 prototype scaffold，但未集成、未 build 证明           |
| Canary 实现            |    未完成 | Big Fish / Golden Idol 只是半成品                   |
| 所有事件实现               | 未开始/极低 | 绝大多数事件是 TODO stub                              |
| 图片                   |    未完成 | 没有图片，抽取脚本不完整                                   |
| 本地化                  |    未完成 | 大量 placeholder，不是最终文本                          |
| 测试                   |    未完成 | 只有 manifest-count 模板，无 live/manual evidence    |
| StS1-only event pool |    未开始 | RitsuLib additive 注册不等于替换 StS2 事件池             |
| 发布可用性                |    未完成 | 不能叫 full parity / 完全一样                         |

我的严格评分：**规划/脚手架 35%，可执行 spec 15%，实际 playable implementation 0–5%，full parity 0%。**

## 下一步 Monthly Dev Spec：June 2026

### 月目标

到 **2026-06-30**，交付一个诚实命名的：

**`StS1 Event Port Prototype Batch 1`**

不是 full parity，不是“全部完成”。目标是：

1. prototype flag 关闭时，对 Spire Plus 零行为影响；
2. prototype flag 开启时，项目能编译；
3. 四个 canary 事件可 debug spawn、可完成、可保存读取；
4. 至少 6 个简单事件进入 playable batch；
5. 文档、状态板、证据不再虚标 Done。

项目当前规则要求 Spire Plus 仍是单一 active mod，稳定 technical manifest id 是 `EZMicroBalance`，代码和资源分别落在 `EZMicroBalanceCode/` 与 `EZMicroBalance/`。 代码/资源改动后至少要 build；资源、本地化、打包改动后还要 publish，并且 release 文档与测试状态要更新。

### Week 0：2026-05-28 至 2026-05-31

目标：修复文档真实性，阻止错误路线继续扩大。

必须交付：

* `docs/features/sts1-events/audit-2026-05-28.md`
* 修正 `status-board.md`
* 修正 `wiki-event-catalog.md`
* 新建 `source-research/api-command-matrix.md`
* 更新 `docs/README.md`、`docs/PROJECT_MAP.md`、`docs/features/README.md`

验收标准：

* canonical target 明确写 **52 unique Wiki event entries**。
* status 只能使用 `planned / spec-drafted / source-verified / implemented / asset-verified / manual-verified / blocked`。
* 没有任何事件能在无代码、无测试、无截图时标成 Done。

### Week 1：2026-06-01 至 2026-06-07

目标：让 prototype 进入可编译、可注册、可 debug spawn 的状态。

必须交付：

* `Sts1EventFeatureGate`
* `Sts1EventRegistry` 接入 Spire Plus 初始化
* `Sts1EventAssetProvider`
* debug spawn 命令或等价 dev console path
* manifest/spec/localization/asset coverage tests

验收标准：

* `dotnet build` 在 prototype flag OFF 时通过。
* `dotnet build` 在 prototype flag ON 时通过。
* OFF 模式不注册任何 StS1 event。
* CanaryOnly 模式只注册 Big Fish、Golden Idol、Lab、Divine Fountain。
* RitsuLib 注册只用于 additive/dev；不得宣称 event-pool parity。

### Week 2：2026-06-08 至 2026-06-14

目标：四个 canary 全部 playable。

事件：

* Big Fish
* Golden Idol
* Lab
* Divine Fountain

必须完成的 support services：

* `Sts1RewardService`：random relic、potion、curse add/remove。
* `Sts1HpService`：heal、gain max HP、damage by max HP percent、lose max HP percent。
* `Sts1AscensionRules`：StS1 A15 规则映射。
* `GoldenIdolRelic`：如果 StS2 没有等价 relic，就新建。
* Regret/Injury 使用 StS2 现有 curse 或创建 StS1-compatible wrapper，但要 source-verified。

验收标准：

* Big Fish 三个选项全对：Banana、Donut、Box。
* Golden Idol 全分支全对：Take/Leave、Outrun/Smash/Hide、A15 数值。
* Lab 给 3 个 potion 或文档化的 StS2-compatible equivalent。
* Divine Fountain 只在有 curse 时出现，且移除所有 curse。
* 四个事件 EN/ZHS 不再是 placeholder。
* 四个事件都有图片路径验证、debug spawn 截图、save/load 证明。

### Week 3：2026-06-15 至 2026-06-21

目标：简单事件 Batch 1。

优先实现：

* Purifier
* Upgrade Shrine
* Golden Shrine
* The Cleric
* Old Beggar
* Shining Light

必须完成：

* card removal/select helper
* card upgrade helper
* gold helper
* option lock helper
* 每个事件 normal/A15 表格

验收标准：

* 6 个事件 debug-spawn verified。
* reachable code 无 TODO。
* 每个事件有 manual test row + screenshot evidence。
* 未实现的事件不得在 status board 标 Done。

### Week 4：2026-06-22 至 2026-06-28

目标：硬化与 StS1-only pool prototype。

必须交付：

* `Sts1EventPoolService` 设计与 prototype
* Additive vs ReplaceUnknownEventsPrototype 文档
* shuffled event bag save fields
* visited ids save/load proof
* implemented events 的 asset extraction map 补齐

验收标准：

* Replacement mode 只能 debug-only。
* multiplayer fail-closed。
* 控制测试中，replacement mode 的 unknown room 不再抽到 StS2 原事件。
* save/load 后 event bag 不重复、不跳错 act bucket。

### Week 5 Buffer：2026-06-29 至 2026-06-30

目标：月末包与 handoff。

必须交付：

* package version increment，如果有 player-visible behavior；
* `docs/features/sts1-events/monthly-review-2026-06.md`
* release note 明确叫 Prototype / Batch，不叫 full parity；
* build/publish/test/manual evidence；
* commit + push，前提是验证通过。

## 立即要改的红线

1. **不要再写“完成 52 个事件”**。最多写“52 个事件进入 manifest/spec draft”。
2. **不要从 Phase 2 开始**。必须先让 canary 编译、运行、保存读取。
3. **不要把图片问题描述成完成**。现在只是资产策略，不是图片完成。
4. **不要把 localization placeholder 当完成**。
5. **不要把 RitsuLib additive event registration 当成 StS1-only parity**。
6. **不要在没有截图/日志/build 证据时标 Done**。

最终目标仍可以是“和杀戮尖塔 1 完全一致”，但当前状态距离这个目标还很远；下一月的合理目标是 **可验证 Batch 1 prototype**，不是 full migration。

[1]: https://slay-the-spire.fandom.com/wiki/Events "Events | Slay the Spire Wiki | Fandom"
[2]: https://slay-the-spire.fandom.com/wiki/Big_Fish "Big Fish | Slay the Spire Wiki | Fandom"
[3]: https://slay-the-spire.fandom.com/wiki/Golden_Idol_%28Event%29 "Golden Idol (Event) | Slay the Spire Wiki | Fandom"
# StS1 Event Port Strict Audit + June 2026 Monthly Dev Spec

Generated: 2026-05-28 Europe/Berlin

## Executive verdict

**Not complete.** The delivered work is a useful research/scaffold package, but it is not a playable StS1 event migration and should not be described as “all Slay the Spire 1 events implemented.”

Highest-confidence completion state:

| Area | Audit result | Reason |
| --- | --- | --- |
| Wiki catalog | Partial / inconsistent | Package manifest has 52 rows and 52 spec files, but the pasted work log claims 48 specs cover all unique events, which conflicts with the canonical 52-event Wiki grouping. |
| Per-event specs | Partial | Files exist, but most are generic templates, not exact option-by-option specs. Dependencies/tests are blank placeholders. |
| Code | Prototype scaffold only | All Sts1Events code is behind `#if STS1_EVENT_PORT_PROTOTYPE`; 52 source files contain TODOs; most event classes only expose a Leave/Done stub. |
| Canary events | Not complete | Big Fish / Golden Idol have partial code, missing curse/relic application, mutability fixes, source-verified helpers, asset proof, localization parity, save/load proof, and manual tests. |
| Assets/images | Not complete | No StS1 event images are present. Extraction script only has TODO source mappings for two events, not 52. |
| Localization | Skeleton only | English and Chinese files exist, but many entries are `[PLACEHOLDER]` / `[占位]`, not parity text or final rewritten text. |
| Tests | Not complete | Only a manifest-count test template exists. No build/test/publish evidence, no live spawn evidence, no screenshot evidence. |
| Event pool parity | Not started | Additive registration is not the same as replacing StS2 unknown-room events with the StS1 event pool. |

## Evidence-based checks

### 1. Pasted work log claims

The pasted log states “48 spec files created” and justifies this by saying the 52 catalog rows contain four act-specific duplicates. It then marks `event-specs/ (48 unique events)` as Done and marks Big Fish / Golden Idol source + localization Done, with assets and tests still Pending. It also lists blockers such as Regret, Injury, random relic helper, card removal/transform/upgrade UI, and combat encounter models.

Strict finding: these claims are internally inconsistent and too optimistic. Marking Phase 0 as Done is not justified unless the manifest, spec count, source verification, current-project integration, and test guard all agree.

### 2. Extracted package metrics

I inspected `sts1_event_port_research_and_scaffold.zip` locally. It contains:

- 52 rows in `manifests/sts1_events_manifest.csv`.
- 52 files under `docs/features/sts1-events/event-specs/`.
- 57 C# files under `code/EZMicroBalanceCode/Sts1Events/`.
- 52 event source files containing TODOs.
- 260 EN localization keys and 260 ZHS localization keys.
- 104 placeholder localization values in each language file.
- 52 asset manifest rows, all marked as local extraction / do-not-commit original StS1 assets.

Strict finding: the package is better than the pasted log on file count, but still not implementation-complete.

### 3. Wiki scope check

The Slay the Spire Wiki groups events as 16 shared events, 12 Act 1 exclusive events, 16 Act 2 exclusive events, and 8 Act 3 exclusive events. That totals 52 unique event entries. The Wiki also states events are selected by random chance and current Act, do not occur in Act 4, and Ascension 15 makes unfavorable events more likely or more intense.

Strict finding: the next docs must use “52 unique Wiki event entries” as the canonical target unless a deliberate product decision excludes something like Neow or A Note For Yourself.

### 4. Source/API check

StS2 v0.106.0 source confirms the core implementation direction is basically right:

- `EventModel` is the correct base abstraction for events.
- `EventOption` supports option callbacks, locked options, relic display, damage death warning, max-HP-loss warning, and choice-history controls.
- `ActModel.GenerateRooms` adds act events plus shared events into `RoomSet.events`.
- `ActModel.PullNextEvent` validates the next event, then calls `Hook.ModifyNextEvent`.
- RitsuLib 0.3.3 exposes `RegisterSharedEvent<T>()` and `RegisterActEvent<TAct,TEvent>()`, plus event asset override hooks.

Strict finding: additive registration is sufficient for debug/prototype testing, but not sufficient for “StS1-only unknown room parity.” That needs an explicit event-pool replacement service or a narrowly documented patch.

### 5. Code defect samples

- `Sts1BigFish.Box()` obtains a relic from `RelicFactory.PullNextRelicFromFront(Owner)` and passes it to `RelicCmd.Obtain` without `.ToMutable()`. In StS2 source, `RelicCmd.Obtain` asserts the relic is mutable, so this likely fails at runtime.
- `Sts1BigFish.Box()` does not add Regret.
- `Sts1GoldenIdol.Take()` does not obtain Golden Idol.
- `Sts1GoldenIdol.Outrun()` does not add Injury.
- `Sts1DivineFountain.IsAllowed()` uses `c.IsCurse`; StS2 card source shows `CardModel.Type` and curse rarity/type patterns, but not an `IsCurse` property in the checked source. This likely fails when prototype compilation is enabled.
- Most event files are placeholder classes that immediately finish or only expose Leave.

Strict finding: do not move to “Phase 2 simple batch implementation” yet. First fix canary compile/runtime correctness.

## Corrected phase status

| Phase | Correct status | Notes |
| --- | --- | --- |
| 0. Documentation/inventory | Partial | Need correct 52-event canonical list, source matrix, status board truthfulness, repo integration. |
| 1. Infrastructure | Partial | Feature gate and registry scaffold exist, but disabled and not integrated; no debug spawn; no build proof. |
| 2. Asset pipeline | Partial | Manifest exists; extraction map incomplete; no images; no validation evidence. |
| 3. Canary events | Not complete | Big Fish / Golden Idol / Lab / Divine Fountain not production-ready. |
| 4. Simple events | Not started | Specs only; no implemented behavior. |
| 5. Card service events | Not started | No card select/remove/upgrade/transform service. |
| 6. Combat events | Not started | No StS1 encounters/rewards/resume flow. |
| 7. Custom UI events | Not started | No Match and Keep / Wheel of Change UI. |
| 8. StS1-only pool | Not started | Needed for “完全一模一样”的 unknown-room event distribution. |
| 9. QA/release evidence | Not started | No build/publish/live screenshots/save-load evidence. |

## Monthly Dev Spec: June 2026

### Month goal

By 2026-06-30, deliver a **StS1 Event Port Prototype Batch 1** for Spire Plus/EZMicroBalance that is honest, buildable, and testable:

1. The prototype flag OFF path has zero behavior change.
2. The prototype flag ON path compiles.
3. Four canary events are fully playable through debug spawn: Big Fish, Golden Idol, Lab, Divine Fountain.
4. At least six simple events are implemented after canaries: Purifier, Upgrade Shrine, Golden Shrine, The Cleric, Old Beggar, Shining Light.
5. No claim of full parity is made.

### Non-goals for June

- Do not claim 52/52 full parity.
- Do not implement/ship StS1 original images in a public package unless permission is documented.
- Do not ship custom UI events as final parity.
- Do not ship combat events as final parity.
- Do not replace all unknown-room events in release mode without save/load and multiplayer guard proof.

### Weekly plan

#### Week 0: 2026-05-28 to 2026-05-31 — audit repair and repo integration

Deliverables:

- `docs/features/sts1-events/audit-2026-05-28.md`
- corrected `status-board.md`
- corrected `wiki-event-catalog.md`
- `docs/features/sts1-events/source-research/api-command-matrix.md`
- update `docs/README.md`, `docs/PROJECT_MAP.md`, `docs/features/README.md`

Acceptance:

- Status board has only these states: `planned`, `spec-drafted`, `source-verified`, `implemented`, `asset-verified`, `manual-verified`, `blocked`.
- No event marked implemented unless it has source file, no TODO in implementation path, loc keys, asset path, and test row.
- Canonical target says 52 unique Wiki events.

#### Week 1: 2026-06-01 to 2026-06-07 — infrastructure and compile-on gate

Deliverables:

- `Sts1EventFeatureGate` with explicit modes: Disabled, CanaryOnly, AdditiveAll, ReplaceUnknownEventsPrototype.
- `Sts1EventRegistry` integrated into current Spire Plus initialization.
- `Sts1EventAssetProvider` using RitsuLib event portrait override or `ModEventTemplate` asset override.
- `Sts1EventDebugSpawnCommand` or equivalent dev console path.
- Test coverage for manifest, loc keys, spec coverage, asset manifest coverage.

Acceptance:

- `dotnet build` passes with prototype flag OFF.
- `dotnet build` passes with prototype flag ON.
- Running with flag OFF registers no StS1 events.
- Running with CanaryOnly registers exactly the canary events.

#### Week 2: 2026-06-08 to 2026-06-14 — four canaries fully playable

Events:

- Big Fish
- Golden Idol
- Lab
- Divine Fountain

Required support:

- `Sts1RewardService`: random relic, potion generation, curse add, curse detection/removal.
- `Sts1HpService`: heal, gain max HP, damage by max HP percent, lose max HP percent, death-warning option helper.
- `Sts1AscensionRules`: StS1 A15 event-difficulty mode mapped from Spire Plus/StS2 ascension setting.
- `GoldenIdolRelic` if StS2 has no compatible Golden Idol relic.

Acceptance:

- Big Fish: Banana heals floor(maxHP/3), Donut grants +5 max HP and heals gained HP, Box gives a mutable random relic and adds Regret.
- Golden Idol: Take grants Golden Idol, then Outrun adds Injury, Smash deals 25%/35% max HP damage, Hide loses 8%/10% max HP, Leave does nothing.
- Lab: grants exactly three potions or documented StS2-compatible equivalents.
- Divine Fountain: allowed only with curses; removes all curses from deck.
- All four have EN/ZHS non-placeholder keys and local screenshot evidence.
- Save/load after the trap page or reward page does not duplicate rewards.

#### Week 3: 2026-06-15 to 2026-06-21 — simple batch 1

Events:

- Purifier
- Upgrade Shrine
- Golden Shrine
- The Cleric
- Old Beggar
- Shining Light

Required support:

- card removal/select service
- card upgrade select service
- gold command helper
- HP-loss and heal helpers
- option lock helper when deck/gold/card prerequisites are absent

Acceptance:

- Six events debug-spawn verified.
- Each event has normal values and A15 differences if applicable.
- Implemented event files contain no TODOs in reachable code.
- Implemented localization values are not placeholders.
- Each event has one manual test row and one screenshot row.

#### Week 4: 2026-06-22 to 2026-06-28 — batch hardening and replacement-pool prototype

Deliverables:

- `Sts1EventPoolService` design and prototype tests.
- Additive vs ReplaceUnknownEventsPrototype documented.
- Save-state fields for shuffled event bag and visited event ids.
- Manual test pass for all implemented events.
- Asset extraction map filled for implemented events.

Acceptance:

- Additive mode remains the only recommended manual testing mode unless replacement-pool save/load proof exists.
- Replacement mode is debug-only and multiplayer fail-closed.
- No StS2 event appears in a replacement-mode unknown room during a controlled test run for the current act bucket.

#### Week 5 buffer: 2026-06-29 to 2026-06-30 — package and handoff

Deliverables:

- package version increment if any player-visible behavior is included.
- updated release notes that call it prototype/batch, not full parity.
- `docs/features/sts1-events/monthly-review-2026-06.md`.
- commit and push if validation succeeds.

Acceptance:

- Build, publish, tests, and manual evidence are recorded.
- Open blockers are explicit and not hidden in archive docs.
- A tester can reproduce debug spawn and asset validation from the docs.

## Issue backlog IDs

| ID | Task | Priority | Depends on | Exit criteria |
| --- | --- | --- | --- | --- |
| STS1-MONTH-001 | Correct catalog/status board | P0 | none | 52 unique target, no false Done states |
| STS1-MONTH-002 | Integrate docs into repo | P0 | 001 | docs index/project map/features index updated |
| STS1-MONTH-003 | API command matrix | P0 | uploaded source/RitsuLib | HP/gold/card/relic/potion APIs documented |
| STS1-MONTH-004 | Prototype flag ON build | P0 | 003 | build passes with StS1 code included |
| STS1-MONTH-005 | Asset override + extraction for canaries | P0 | 003 | images load locally, no original assets committed |
| STS1-MONTH-006 | Big Fish implemented | P0 | 004,005 | all 3 options verified |
| STS1-MONTH-007 | Golden Idol implemented | P0 | 004,005 | all branches + A15 verified |
| STS1-MONTH-008 | Lab implemented | P0 | 004,005 | 3 potion reward verified |
| STS1-MONTH-009 | Divine Fountain implemented | P0 | 004,005 | curse detection/removal verified |
| STS1-MONTH-010 | Simple batch 1 implemented | P1 | 006-009 | six simple events verified |
| STS1-MONTH-011 | Replacement-pool prototype | P1 | 006-010 | debug-only save/load-safe pool prototype |
| STS1-MONTH-012 | Monthly QA handoff | P0 | all | evidence, package, honest docs |

## Release language rule

Allowed language for June builds:

- `StS1 Event Port Prototype`
- `Canary Batch`
- `Batch 1`
- `Additive test mode`

Forbidden language until full validation:

- `StS1 full parity`
- `all events implemented`
- `same as Slay the Spire 1`
- `complete migration`
