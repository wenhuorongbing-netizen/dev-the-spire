# Spire Plus 论坛

这是 `Spire Plus` 的静态匿名论坛前端。它构建到 `website/forum/`，由 GitHub Pages 托管；帖子和回复存入 Supabase PostgreSQL。

## 运行方式

```powershell
cd forum
npm ci
Copy-Item .env.example .env
npm run dev
```

开发地址：

```text
http://127.0.0.1:5173
```

## Supabase 配置

完整上线步骤见 `../docs/features/forum/go-live-checklist.md`。

1. 新建 Supabase 项目。
2. 打开 SQL Editor，执行 `supabase/schema.sql`。
3. 在 `.env` 或 GitHub Actions Variables 中设置：

```text
VITE_SUPABASE_URL=https://your-project-ref.supabase.co
VITE_SUPABASE_ANON_KEY=your-public-anon-key
VITE_FORUM_READ_ONLY=0
```

`anon key` 会出现在浏览器里，这是 Supabase 的正常用法。安全边界在 `schema.sql` 的 RLS、列级授权和插入策略里，不在前端密钥里。

## 构建

```powershell
npm run build
```

构建产物写入：

```text
../website/forum/
```

现有 GitHub Pages workflow 会上传整个 `website/`，所以论坛地址会变成：

```text
https://wenhuorongbing-netizen.github.io/dev-the-spire/forum/
```

## 免费档限制

Supabase Free Plan 足够用于第一版文字论坛，但有这些限制：

- 免费项目长时间低活跃可能暂停，需要手动恢复。
- 数据库、流量和存储都有额度。
- 纯前端匿名论坛无法做可靠 IP 限流；当前只做 RLS、client id 频率限制、honeypot、长度限制和链接数限制。
- 不建议第一版开放图片上传。
- 正式公开前建议定期导出 SQL 备份。

## 测试

```powershell
npm test
```

当前测试是静态 schema guard，检查 RLS、匿名授权和基础防刷策略是否仍存在。真实发帖/回复需要连接 Supabase 项目后用浏览器手动验证。
