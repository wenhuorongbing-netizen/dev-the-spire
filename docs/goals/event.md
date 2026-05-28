ChatGPT














STS2-RitsuLib.0.3.3.variant-pack.zip
压缩归档

StS2-v0.106.0-source-code-ai-analyze-codeonly-20260522.zip
压缩归档
结合项目内容和source code进行一次详细的调研,我需要的是让mod去实现杀戮尖塔1。杀戮尖塔1的所有事件全部迁移到这里,结合Wiki上的调研,建立一个详细的目标,一步怎么一步步去达成目标,怎么把它去有条理地document下来,然后怎么去一步步实现它,在杀戮尖塔2的mod中,你可以调研我的Devspire,然后把这东西都做出来,包括图片什么的全部都要做到。和杀戮尖塔1完全一模一样。

我会并行调研 StS2 源码、RitsuLib/DevSpire 的 mod 接口与资源加载机制，并用 StS1 Wiki 建立完整事件清单和机制矩阵。最终输出迁移目标、文档结构、任务拆分、实现骨架与资产管线；原版图文只设计本地合法导入/占位映射，不直接分发未授权素材。

已确认本地基线是 StS2 v0.106.0 C#/Godot 反编译源码与 RitsuLib 0.3.3，目标仓库是 wenhuorongbing-netizen/dev-the-spire。事件主入口集中在 EventModel/EventOption/ActModel/ModelDb、src/Core/Models/Events、EventRoom.cs、src/Core/Nodes/Events，并可通过 RitsuLib 的 RegisterActEvent/RegisterSharedEvent 等接口注册。

DevSpire 不是空白 mod，已有 EZMicroBalanceCode/、docs/、tests/，因此迁移包会按功能代码、研究文档、回归测试三层落地。难点会优先聚焦事件池替换、StS1 专属依赖、UI 小游戏、战斗事件与资源授权。


正在查找可用工具
已思考 41m 23s
已完成一版“调研 + 目标定义 + 文档体系 + 事件 manifest + 代码脚手架 + 图片本地抽取管线”的交付包：

下载 StS1 全事件迁移调研与脚手架 ZIP
查看一页执行摘要

我把方案按你现有 Devspire/Spire Plus 项目来落地：仓库当前目标是单一 Spire Plus Mod，技术 manifest id 仍保持 EZMicroBalance，不能原地改名；资源目录是 EZMicroBalance/，C# 代码目录是 EZMicroBalanceCode/，发布后也应输出到 <GameRoot>/mods/EZMicroBalance/ 里的 JSON/DLL/PCK 三件套。

 


这次包里已经生成了 52 个 StS1 Wiki 事件的主清单、每个事件的 spec 文档、实现优先级、依赖分类、图片 key、本地化 key、测试计划和分阶段 roadmap。Wiki 调研采用的事件范围是：14 个 Act 1/2/3 通用事件、2 个半通用事件、Act 1 的 Neow + 11 个专属事件、Act 2 的 16 个专属事件、Act 3 的 8 个专属事件；Wiki 也说明事件来自未知房间、按 Act/条件进入池，Act 4 没有事件，Ascension 15 会让不利事件更不利。

包里我没有直接放《杀戮尖塔 1》的原图或原文全文。原因是你项目自己的规则已经要求不要随便复制原版素材或大段反编译代码，原版美术也要有授权才能放进公开/可跟踪文件；所以我做的是“本地合法 StS1 安装目录抽取脚本 + asset manifest + 验证脚本”，这样可以在你自己的机器上做到图片一致，但不把版权素材提交/分发出去。


ZIP 里的关键内容：

docs/features/sts1-events/
  README.md
  goal.md
  wiki-event-catalog.md
  implementation-plan.md
  documentation-plan.md
  assets.md
  localization.md
  test-plan.md
  status-board.md
  source-research/sts2-event-engine.md
  event-specs/*.md       # 52 个事件逐个 spec

manifests/
  sts1_events_manifest.csv
  sts1_events_manifest.json
  asset_manifest.csv

code/
  EZMicroBalanceCode/Sts1Events/
    Runtime/
    Models/Shared/
    Models/Act1/
    Models/Act2/
    Models/Act3/
  EZMicroBalance/localization/eng/sts1_events.json
  EZMicroBalance/localization/zhs/sts1_events.json

scripts/
  extract-sts1-event-assets.ps1
  validate-sts1-event-assets.ps1

tests/
  Sts1EventManifestTests.cs
源码侧的结论是：StS2 的事件应该走 EventModel 子类；EventOption 负责选项、锁定、伤害/最大生命死亡提示；ActModel.GenerateRooms 会把章节事件和 shared events 生成到 RoomSet.events，PullNextEvent 会先验证下一个事件，再走 Hook.ModifyNextEvent。因此完整迁移不能只“注册事件”，还需要一个 StS1-only event pool，否则 StS2 原事件仍会混进未知房间。RitsuLib 则可以用于 RegisterSharedEvent / RegisterActEvent 注册，以及用事件图片覆盖入口把图片指向 EZMicroBalance/images/events/sts1/*.png。

我把第一批 canary 事件定为 Big Fish 和 Golden Idol。Big Fish 的 Wiki 行为是 Act 1 专属，三个选择分别对应回复 1/3 最大生命、最大生命 +5、随机遗物 + Regret 诅咒；Golden Idol 是 Act 1 专属，拿走 Golden Idol 后进入陷阱分支，选项包括 Injury 诅咒、当前生命伤害、最大生命损失，并且 A15 数值更严苛。
 

建议下一步在 Devspire 里按这个顺序做：

先把 docs/features/sts1-events/ 复制进仓库，更新 docs/README.md、docs/PROJECT_MAP.md、EZMicroBalanceCode/README.md。

复制 code/EZMicroBalanceCode/Sts1Events/，但保持 #if STS1_EVENT_PORT_PROTOTYPE，先不要影响现有 build。

接入 Sts1EventRegistry.Register(...)，只注册 Sts1BigFish、Sts1GoldenIdol、Sts1Lab、Sts1DivineFountain 四个 canary。

跑 scripts/extract-sts1-event-assets.ps1，从你本地 StS1 安装目录抽图到 EZMicroBalance/images/events/sts1/。

实现 canary 依赖的最小内容：Regret、Injury、Golden Idol relic、随机 relic reward helper、HP/max HP command helper。

dotnet build，再 dotnet publish，然后进游戏 debug spawn 这两个事件。

Canary 通过后，再进入简单事件批、卡牌服务事件批、战斗事件批、custom UI 事件批，最后做 ReplaceUnknownEvents 的 StS1-only 事件池。

这次交付包是“可执行工程蓝图 + scaffold”，不是已经完成 52/52 事件的最终 playable parity build。要真正宣称“和杀戮尖塔 1 完全一模一样”，还需要逐事件补齐 StS1 数值证据、A15 差异、依赖 relic/card/curse/monster、图片抽取、保存读取、事件池替换和截图验证。



