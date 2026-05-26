# Spire Plus 网站论坛集成 QA

审核范围：主站 `#forum` 页面、直接访问 `/forum/` 的玩家路径、论坛发帖页嵌入效果。

结论：无必须修复项。

## 已复查

- 主站导航中的“论坛”现在直接显示论坛内容，不再显示“进入论坛”式跳转入口。
- 直接访问 `/forum/` 会回到主站 `#forum` 页面，避免玩家进入另一套独立外壳。
- `/forum/#/new` 会回到主站并保留发帖页状态，最终 iframe 地址为 `/forum/?embedded=1#/new`。
- 嵌入模式隐藏论坛自己的独立页头，保留主站品牌、导航和语言切换。
- 公开站点验证无浏览器 console error。

## 截图证据

- `output/playwright/forum-public-main-tab-aff471a.png`
- `output/playwright/forum-public-direct-redirect-aff471a.png`

## 待后续

- 论坛真实运营前，仍需持续观察匿名发帖刷屏风险；当前方案依赖 Supabase RLS、honeypot、长度限制和基础速率约束。
