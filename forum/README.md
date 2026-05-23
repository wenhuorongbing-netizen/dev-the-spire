# Spire Plus 论坛

独立匿名论坛服务：React + Vite 前端、Fastify/Node TypeScript 后端、PostgreSQL 数据库。用户无需注册即可发帖和回复。

## 本地开发

```powershell
cd forum
npm ci
Copy-Item .env.example .env
npm run db:start
npm run migrate
npm run dev
```

默认地址：

- 前端开发：`http://localhost:5173`
- API：`http://localhost:8787`
- 生产预览：`http://localhost:8787`

## 环境变量

- `DATABASE_URL`：PostgreSQL 连接字符串。
- `IP_HASH_SECRET`：用于 HMAC-SHA256 哈希 IP 和 User-Agent 的长随机密钥。生产环境必须设置。
- `CORS_ORIGINS`：允许跨域访问的公网网站和论坛域名，逗号分隔；本地 `localhost` 会自动允许。
- `FORUM_READ_ONLY`：设为 `1` 时关闭发帖和回复，只保留浏览。
- `DATABASE_SSL`：设为 `1` 时使用 PostgreSQL TLS，适合需要 SSL 的托管数据库。
- `PORT` / `HOST`：Node 服务监听地址。

## 数据库迁移

迁移文件放在 `db/migrations/*.sql`，按文件名顺序执行，并写入 `schema_migrations`。

```powershell
npm run migrate
```

## 构建和生产运行

```powershell
npm run build
npm start
```

生产模式下，Node 服务同时提供 `/api/v1/*` JSON API、React 构建产物和前端路由回退。

Render 部署建议：

- `buildCommand`: `cd forum && npm ci && npm run build`
- `startCommand`: `cd forum && npm run migrate && npm start`
- `healthCheckPath`: `/healthz`
- Web Service 设置 `DATABASE_URL`、`IP_HASH_SECRET`、`CORS_ORIGINS`。
- PostgreSQL 使用 Render PostgreSQL。

## 测试

API 测试需要 PostgreSQL。可直接使用本地 Docker 数据库：

```powershell
cd forum
npm run db:start
npm run migrate
$env:FORUM_TEST_DATABASE_URL="postgres://forum:forum@localhost:54329/forum"
npm test
```

测试会清空 `forum_posts` 和 `forum_replies`，不要指向生产数据库。
