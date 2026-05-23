# web_issue

## 必须修

- [x] 公开站点已移除旧的 Node/PostgreSQL/Render 论坛部署文案，改为 GitHub Pages + Supabase 配置说明。
- [x] GitHub Pages 部署流程已在构建论坛前运行 `npm test`，避免 RLS/schema guard 失败时继续发布。
- [x] 未配置 Supabase 时，论坛页面显示数据库配置提示，不显示可提交的假表单。

## 可后续修

- [ ] 连接真实 Supabase 项目后，需要补一次发帖、回帖、刷新保留数据、只读模式的浏览器验证。
- [ ] 匿名论坛当前只有 client id、RLS、honeypot、长度和链接数量限制；如果出现刷屏，需要升级到 Supabase Edge Function 或独立后端做 IP/行为限流。
- [ ] 免费 Supabase 项目低活动会暂停；正式公开前需要写清楚手动恢复和备份流程。
