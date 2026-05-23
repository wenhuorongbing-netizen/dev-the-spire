import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { test } from "node:test";

const source = readFileSync(new URL("../src/client/App.tsx", import.meta.url), "utf8");

test("post page distinguishes missing posts from connection failures", () => {
  assert.match(source, /\.maybeSingle\(\)/);
  assert.match(source, /论坛暂时无法连接，请稍后重试。/);
  assert.match(source, /帖子不存在，可能已被隐藏、删除，或链接不是最新。/);
  assert.doesNotMatch(source, /帖子不存在或论坛暂时无法连接。/);
});

test("reply loading failure does not hide a readable post", () => {
  assert.match(source, /setPost\(postData\);[\s\S]*from\("forum_replies"\)/);
  assert.match(source, /回帖暂时无法加载，主帖仍可阅读。/);
});

test("public forum queries explicitly filter visible rows", () => {
  const visibleFilters = source.match(/\.eq\("status", "visible"\)/g) ?? [];
  assert.ok(visibleFilters.length >= 3, "home, post, and replies queries should all filter visible rows");
});
