# Spire Plus 论坛上线清单

这份清单用于把论坛发布到 GitHub Pages，并用 Supabase 保存帖子和回复。这个方案不需要 Render，也不需要本机 24 小时开机。

## 1. Supabase 项目

1. 创建 Supabase 项目。
2. 打开 SQL Editor。
3. 执行完整脚本：

```text
forum/supabase/schema.sql
```

4. 记录项目 URL 和公开 anon key：

```text
Project Settings -> API -> Project URL
Project Settings -> API -> anon public key
```

`anon key` 本来就会出现在浏览器里。安全边界不在这个 key，而在 `schema.sql` 里的 RLS、列级授权和插入策略。

## 2. GitHub 仓库 Variables

在仓库 Variables 里设置：

```text
SPIRE_PLUS_SUPABASE_URL=https://your-project-ref.supabase.co
SPIRE_PLUS_SUPABASE_ANON_KEY=your-public-anon-key
SPIRE_PLUS_FORUM_READ_ONLY=0
```

`SPIRE_PLUS_FORUM_READ_ONLY` 是可选项。设置为 `1` 时，论坛可读但不能发帖或回复。

## 3. GitHub Pages

在仓库设置里确认：

1. 仓库是 public，或当前账号计划支持 private repository Pages。
2. 打开 `Settings -> Pages`。
3. Build and deployment source 选择 GitHub Actions。
4. 运行 workflow：

```text
.github/workflows/spire-plus-site.yml
```

workflow 会执行：

```text
cd forum
npm ci
npm test
npm run build
```

然后上传 `website/` 作为 Pages artifact。

## 4. Release 下载链接

网站下载按钮当前指向这个 release asset：

```text
https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/download/v0.1.0-private-beta.13/SpirePlus-v0.1.0-private-beta.13.zip
```

公开分享前确认 GitHub Release 存在，并且包含：

```text
SpirePlus-v0.1.0-private-beta.13.zip
```

如果版本号变化，同步更新 `website/content-data.js` 和发布文档。

## 5. 论坛实机验证

Pages 部署后，在浏览器里验证：

1. 打开公开网站。
2. 打开“论坛”。
3. 确认不再显示“论坛还没有连接数据库”。
4. 留空名字发一条主帖，应显示为“匿名玩家”。
5. 打开帖子详情。
6. 回复该帖子。
7. 刷新页面，主帖和回复仍然存在。
8. 将 `SPIRE_PLUS_FORUM_READ_ONLY=1`，重新运行 Pages，确认发帖和回复被关闭。
9. 将 `SPIRE_PLUS_FORUM_READ_ONLY=0`，重新运行 Pages，确认发帖和回复恢复。

也可以在本机运行一次 live smoke test。它会用 anon key 发帖和回帖，再用 service role key 把测试帖隐藏：

```powershell
cd forum
$env:SPIRE_PLUS_FORUM_LIVE_TEST="1"
$env:VITE_SUPABASE_URL="https://your-project-ref.supabase.co"
$env:VITE_SUPABASE_ANON_KEY="your-public-anon-key"
$env:SUPABASE_SERVICE_ROLE_KEY="your-service-role-key"
npm run test:live
```

不要把 `SUPABASE_SERVICE_ROLE_KEY` 配到 GitHub Pages 或任何前端构建变量里。它只能留在本机或受控 CI 环境，用于测试后的管理清理。

## 6. 管理和恢复

第一版没有公开管理后台。需要隐藏内容时，用 Supabase SQL 或 table editor：

```sql
update public.forum_posts
set status = 'hidden'
where id = '<post-id>';

update public.forum_replies
set status = 'hidden'
where id = '<reply-id>';
```

Supabase 免费项目低活动后可能暂停。正式发测试包或集中招募前，先导出一次数据库备份。

## 7. 当前限制

- 匿名 `client_id` 限流可以被清 localStorage、换浏览器等方式绕过。
- 可靠的 IP 级反刷屏需要 Supabase Edge Function 或独立后端。
- 第一版不开放图片上传。
- 第一版没有公开管理后台，管理通过 Supabase 后台完成。