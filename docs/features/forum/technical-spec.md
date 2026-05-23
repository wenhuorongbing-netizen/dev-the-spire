# Spire Plus 匿名论坛技术规格

## 目标

把 `website/#forum` 从 GitHub 反馈入口和本地草稿，改为真正可用的匿名论坛入口。

论坛第一版必须支持：

- 无需注册、无需登录。
- 输入名字、标题、正文即可发帖；名字可留空，显示为“匿名玩家”。
- 所有人都能看到帖子列表。
- 点进帖子能查看正文和回复。
- 可匿名回复帖子。
- 现有 GitHub Pages 介绍站能跳转到公网论坛。

## 非目标

- 不做账号、邮箱、OAuth、权限组。
- 不做图片上传、富文本、私信、关注、精华、复杂管理后台。
- 不把后端塞进 GitHub Pages。GitHub Pages 只能发布静态文件。
- 不依赖 GitHub Issues / Discussions 作为论坛数据源。

## 架构

采用独立全栈论坛服务：

```text
website/ GitHub Pages 静态站
  -> 论坛入口按钮
  -> 独立公网论坛服务

forum/ 服务
  -> React + Vite 前端
  -> Node.js + TypeScript API
  -> PostgreSQL
```

生产环境下，Node 服务同时提供：

- `/api/v1/*` JSON API。
- React 构建产物。
- 非 API 路径回退到 `index.html`，支持前端路由。

## 技术栈

- 前端：React、Vite、TypeScript。
- 后端：Node.js、TypeScript、Fastify。
- 数据库：PostgreSQL。
- SQL：`pg` 参数化查询，不引入 ORM。
- Migration：`forum/db/migrations/*.sql`，按文件名顺序执行并记录到 `schema_migrations`。
- 部署：优先 Render Blueprint，一套 Node Web Service + PostgreSQL。

## 数据模型

```sql
CREATE TABLE IF NOT EXISTS schema_migrations (
  version text PRIMARY KEY,
  applied_at timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS forum_posts (
  id bigserial PRIMARY KEY,
  author_name varchar(32) NOT NULL DEFAULT '匿名玩家',
  title varchar(120) NOT NULL,
  body text NOT NULL,
  status varchar(16) NOT NULL DEFAULT 'visible'
    CHECK (status IN ('visible', 'hidden', 'deleted')),
  ip_hash char(64),
  user_agent_hash char(64),
  reply_count integer NOT NULL DEFAULT 0 CHECK (reply_count >= 0),
  last_activity_at timestamptz NOT NULL DEFAULT now(),
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now(),
  CHECK (char_length(trim(title)) BETWEEN 1 AND 120),
  CHECK (char_length(trim(body)) BETWEEN 1 AND 10000)
);

CREATE TABLE IF NOT EXISTS forum_replies (
  id bigserial PRIMARY KEY,
  post_id bigint NOT NULL REFERENCES forum_posts(id) ON DELETE CASCADE,
  author_name varchar(32) NOT NULL DEFAULT '匿名玩家',
  body text NOT NULL,
  status varchar(16) NOT NULL DEFAULT 'visible'
    CHECK (status IN ('visible', 'hidden', 'deleted')),
  ip_hash char(64),
  user_agent_hash char(64),
  created_at timestamptz NOT NULL DEFAULT now(),
  updated_at timestamptz NOT NULL DEFAULT now(),
  CHECK (char_length(trim(body)) BETWEEN 1 AND 5000)
);
```

必要索引：

- `forum_posts(status, last_activity_at desc, id desc)` 的可见帖列表索引。
- `forum_replies(post_id, created_at asc, id asc)` 的可见回复索引。
- `forum_posts(ip_hash, created_at desc)` 和 `forum_replies(ip_hash, created_at desc)` 用于限流。

## API

Base URL: `/api/v1`

### 健康检查

`GET /healthz`

返回：

```json
{ "ok": true, "db": "ok", "version": "0.1.0" }
```

### 帖子列表

`GET /api/v1/posts?limit=20&cursor=...`

- 按 `last_activity_at DESC, id DESC` 排序。
- `limit` 限制在 1-50，默认 20。
- `cursor` 使用 `lastActivityAt_id`。

返回帖子摘要、回复数、创建时间、最后活动时间和下一页游标。

### 发帖

`POST /api/v1/posts`

请求：

```json
{
  "authorName": "测试玩家",
  "title": "标题",
  "body": "正文",
  "website": ""
}
```

规则：

- `authorName` 可空，空值保存为“匿名玩家”。
- `title` 1-120 字。
- `body` 1-10000 字。
- `website` 是隐藏 honeypot 字段，非空拒绝。

成功返回 `201` 和新帖子 id。

### 帖子详情

`GET /api/v1/posts/:id`

返回帖子正文和可见回复。隐藏、删除或不存在返回 `404`。

### 回复

`POST /api/v1/posts/:id/replies`

请求：

```json
{
  "authorName": "",
  "body": "回复内容",
  "website": ""
}
```

成功时在同一事务内插入回复，并更新主帖 `reply_count` 和 `last_activity_at`。

## 前端页面

### 首页

- 标题：`Spire Plus 论坛`
- 主按钮：`发帖`
- 帖子列表显示：标题、作者、时间、回复数、正文摘要。
- 空状态：`还没有帖子，发第一帖。`
- 错误状态：`论坛暂时无法连接`，提供重试。
- 分页：`加载更多`。

### 发帖页

- 路径：`/new`
- 字段：名字、标题、正文。
- 提交成功跳转到 `/posts/:id`。
- `429` 显示“发帖太频繁，请稍后再试”。

### 详情页

- 路径：`/posts/:id`
- 显示完整标题、作者、发布时间、正文、回复列表。
- 底部回复框支持匿名回复。
- 回复成功后刷新详情并清空表单。

### 文本渲染

- 只支持纯文本。
- 保留换行。
- 不解析 HTML，不使用 `dangerouslySetInnerHTML`。
- 不自动转换链接，降低垃圾链接收益。

## 匿名和安全

匿名原则：

- 不注册、不登录。
- 展示名不代表身份。
- 不公开 IP、UA。
- 服务端只保存 HMAC-SHA256 后的 `ip_hash` 和 `user_agent_hash`。

最低限度防刷：

- JSON body 最大 32KB。
- 同一 IP hash 每 10 分钟最多 3 个主帖，每日最多 20 个主帖。
- 同一 IP hash 每 10 分钟最多 10 条回复，每日最多 80 条回复。
- 正文 URL 超过 5 个拒绝。
- honeypot 字段非空拒绝。
- `FORUM_READ_ONLY=1` 时关闭发帖和回复，只保留浏览。

安全要求：

- 所有 SQL 使用参数化查询。
- CORS 只允许生产站、论坛域名和 localhost。
- 不使用 cookie 鉴权。
- 返回通用错误，不暴露 SQL 细节。
- 设置基础安全 header。

## 本地开发

```powershell
cd forum
npm ci
npm run db:start
npm run migrate
npm run dev
```

默认端口：

- 前端开发：`http://localhost:5173`
- 后端 API：`http://localhost:8787`
- 生产预览：`http://localhost:8787`

## 部署

首选 Render Blueprint：

- 一个 Node Web Service。
- 一个 Render PostgreSQL。
- `DATABASE_URL` 来自 Render database。
- `IP_HASH_SECRET` 在 Dashboard 中填写。
- `CORS_ORIGINS` 包含 GitHub Pages 站点和论坛域名。
- `buildCommand`: `cd forum && npm ci && npm run build`
- `startCommand`: `cd forum && npm run migrate && npm start`
- `healthCheckPath`: `/healthz`

GitHub Pages 仍只部署 `website/`。它不能运行论坛后端，也不能保存 PostgreSQL 数据。

## 与现有网站接入

`website/#forum` 改为公网论坛入口页：

- 删除本地草稿表单定位。
- 显示 `进入 Spire Plus 论坛` 主按钮。
- 文案说明论坛无需注册，可匿名发帖和回复。
- 按钮指向 `content-data.js` 中的论坛公网地址。

如果论坛尚未部署，入口按钮可先指向本地开发地址或 Render 待配置地址，但文档必须标明需要替换。

## 验收标准

- 从 GitHub Pages 网站能进入论坛。
- 未登录用户可以发帖。
- 新帖子刷新后仍存在。
- 另一浏览器能看到帖子。
- 能进入帖子详情。
- 未登录用户可以回复。
- 回复刷新后仍存在，列表回复数增加。
- API 健康检查返回数据库可用。
- 高频发帖或回复返回 `429`。
- `<script>` 输入只显示文本，不执行。
- `website/#forum` 不再显示“反馈草稿”。
- README 记录 `DATABASE_URL`、`IP_HASH_SECRET`、`CORS_ORIGINS`、`FORUM_READ_ONLY`。
