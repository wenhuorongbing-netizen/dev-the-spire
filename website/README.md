# Spire Plus Website

这是 `Spire Plus` 的静态介绍站点。站点内容以当前仓库文档、源码和本地化文本为准，定位是“效果记录”和“下载入口”，不是发布就绪声明。

## 本地预览

从仓库根目录启动静态服务器：

```powershell
python -m http.server 4177 --bind 127.0.0.1
```

打开：

```text
http://127.0.0.1:4177/website/
```

本地环境下“下载当前包”按钮会指向：

```text
../publish/SpirePlus-v0.1.0-private-beta.0.zip
```

## 公开部署

`.github/workflows/spire-plus-site.yml` 会把 `website/` 发布到 GitHub Pages。公开环境下“下载当前包”按钮会指向 GitHub Release 的 zip 资产：

```text
https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/download/v0.1.0-private-beta.0/SpirePlus-v0.1.0-private-beta.0.zip
```

发布页按钮指向具体版本页：

```text
https://github.com/wenhuorongbing-netizen/dev-the-spire/releases/tag/v0.1.0-private-beta.0
```

当前仓库是 private 时，GitHub Pages 还取决于账号计划是否支持 private repository Pages。若 API 返回 `Your current plan does not support GitHub Pages for this repository`，需要把仓库改为 public、升级到支持 private Pages 的计划，或把 `website/` 发布到一个单独的 public Pages 仓库。

发布页面前，需要在 GitHub Release 上传同名 zip 资产。

## 更新方式

- 主要内容：`content-data.js`
- 页面结构：`index.html`
- 视觉样式：`styles.css`
- 渲染逻辑：`app.js`
- 图片：`assets/`

`assets/` 只放 `EZMicroBalance/images/` 下的自有或生成资源、站点自带的通用图标，以及已确认可用于本站发布的原版遗物图标和卡牌立绘。当前授权确认来自项目负责人在 2026-05-23 的明确说明；只分发更新页实际引用的 25 个遗物图标和 3 个卡牌立绘，位置为 `assets/vanilla-icons/`。不要复制 `source code/` 下的原版非美术资产。
