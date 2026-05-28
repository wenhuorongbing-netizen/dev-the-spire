# StS1 全事件迁移到 StS2 / Spire Plus：调研与执行摘要

交付物：`sts1_event_port_research_and_scaffold.zip`

## 结论

迁移应分成三层：

1. **事件框架层**：`Sts1EventModel`、`Sts1EventRegistry`、feature gate、debug spawn、StS1-only event pool。
2. **依赖内容层**：StS1 事件需要的 curses、special cards、relics、encounters、reward helpers、custom UI。
3. **事件实现层**：52 个事件按 canary、简单事件、卡牌服务事件、战斗事件、custom UI/特殊 run hook 分批完成。

## 最小第一步

先做 `Big Fish` 和 `Golden Idol`：它们覆盖 heal、max HP、relic、curse、multi-page、A15 数值、图片、本地化和事件完成流程。

## 目录落点

```text
EZMicroBalanceCode/Sts1Events/
EZMicroBalance/localization/eng/sts1_events.json
EZMicroBalance/localization/zhs/sts1_events.json
docs/features/sts1-events/
scripts/extract-sts1-event-assets.ps1
scripts/validate-sts1-event-assets.ps1
```

## 版权边界

本包不包含 StS1 原图或全文文本。图片 parity 通过本地合法 StS1 安装目录抽取；公开发布时不要把原图放进包里，除非有授权。

## 验收定义

不能在 52/52 事件、StS1-only pool、A15 差异、保存/读取、图片/文本来源、战斗事件、custom UI 都验证前宣称“完全一致”。中间版本应命名为 `StS1 Event Port Prototype` 或 `Batch N`。
