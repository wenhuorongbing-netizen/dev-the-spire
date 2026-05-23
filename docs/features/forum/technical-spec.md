# Spire Plus 匿名论坛技术规格

## 当前决策

论坛改为 `GitHub Pages + Supabase`。

上线操作清单见 `docs/features/forum/go-live-checklist.md`。

原因：

- GitHub Pages 只能发布静态文件，不能运行 Node 后端或 PostgreSQL。
- 用户不想维护 Render，也不想让本机 24 小时开机。
- Supabase 提供托管 PostgreSQL、REST API 和 RLS，适合第一版匿名文字论坛。

## 目标

- 玩家无需注册即可发帖和回复。
- 名字可留空，显示为“匿名玩家”。
- 论坛随 `website/` 一起部署到 GitHub Pages。
- 帖子和回复持久化在 Supabase PostgreSQL。
- 第一版只支持纯文本，不上传图片。

## 非目标

- 不做账号、邮箱、OAuth。
- 不做富文本、图片上传、私信、关注。
- 不做独立 Node 后端。
- 不把 Supabase service role key 放到前端。

## 架构

```text
GitHub Pages
  website/
    index.html
    forum/
      React forum build

Supabase
  forum_posts
  forum_replies
  Row Level Security
```

构建流程：

```text
forum/ React source
  npm run build
  -> website/forum/
  -> GitHub Pages upload website/
```

## 技术栈

- React + Vite + TypeScript。
- `@supabase/supabase-js`。
- Supabase PostgreSQL。
- SQL schema 和 RLS 文件：`forum/supabase/schema.sql`。

## 数据表

### `forum_posts`

主要字段：

- `id uuid`
- `author_name`
- `title`
- `body`
- `status`
- `client_id`
- `reply_count`
- `last_activity_at`
- `created_at`

### `forum_replies`

主要字段：

- `id uuid`
- `post_id`
- `author_name`
- `body`
- `status`
- `client_id`
- `created_at`

`client_id` 由浏览器生成并保存在 localStorage，只用于基础限频。它不是身份认证，不能作为安全边界。

## RLS 和授权

必须启用 RLS：

- 匿名用户只能读取 `visible` 帖子和回复。
- 匿名用户只能插入新帖和新回复。
- 匿名用户不能更新、删除、隐藏帖子。
- 匿名用户不能写入 `reply_count`、`status`、时间字段。

列级授权：

- `forum_posts` 只允许匿名插入 `author_name`、`title`、`body`、`client_id`。
- `forum_replies` 只允许匿名插入 `post_id`、`author_name`、`body`、`client_id`。

触发器：

- 插入主帖前清理作者、标题、正文，并强制 `status = visible`。
- 插入回复前清理作者和正文，并强制 `status = visible`。
- 插入回复后自动增加主帖 `reply_count`，更新 `last_activity_at`。

## 防刷策略

第一版不做账号，也不做服务端 IP 限流。

已有保护：

- honeypot 字段。
- 标题、正文长度限制。
- 链接数量限制。
- 同一 `client_id` 十分钟和一天内的发帖/回复数量限制。
- 只读开关 `VITE_FORUM_READ_ONLY=1`。

已知不足：

- 用户清 localStorage 或换浏览器可以绕过 `client_id` 限频。
- 真正可靠的 IP 限流需要 Supabase Edge Function 或独立后端。
- 如果出现刷屏，短期处理方式是打开只读、手动 SQL 隐藏帖子，长期再做 Edge Function。

## 免费档限制

Supabase Free Plan 适合文字论坛起步，但有代价：

- 低活跃项目可能被暂停，需要手动恢复。
- 数据库、流量、存储有免费额度。
- 免费档备份和恢复能力有限。
- 不建议开放图片上传。

如果论坛有稳定玩家使用，再评估升级 Supabase Pro 或迁移到独立后端。

## 环境变量

前端构建使用：

```text
VITE_SUPABASE_URL
VITE_SUPABASE_ANON_KEY
VITE_FORUM_READ_ONLY
```

`VITE_SUPABASE_ANON_KEY` 是公开 key，不能当作秘密。安全规则必须写在 RLS 里。

## 页面

论坛地址：

```text
/forum/
```

页面：

- 首页：帖子列表、发帖按钮、加载更多。
- 发帖页：名字、标题、正文。
- 详情页：正文、回复列表、回复表单。
- 未配置 Supabase 时：显示配置提示，不崩溃。

## 现有网站接入

`website/#forum` 不再显示本地草稿。它作为入口页：

- 说明论坛无需注册。
- 按钮指向 `website/forum/`。
- 说明论坛数据由 Supabase 保存。

## 验收标准

- `npm run build` 成功，并生成 `website/forum/`。
- `npm test` 通过 schema guard。
- 配好 Supabase 后，`npm run test:live` 能用 anon key 发帖、回帖、读取，并用 service role key 隐藏测试帖。
- `website/#forum` 按钮能打开 `/forum/`。
- 未配置 Supabase 时，论坛页面显示清楚的配置提示。
- 配好 Supabase 后，能发帖、看帖、回复，刷新后数据仍存在。
- RLS 禁止匿名更新和删除。
- 截图覆盖桌面和移动端。
