import "dotenv/config";
import assert from "node:assert/strict";
import { test } from "node:test";
import { createApp } from "../src/server/app.js";
import { getConfig } from "../src/server/config.js";
import { createPool } from "../src/server/db.js";
import { runMigrations } from "../src/server/migrate.js";

const testDatabaseUrl = process.env.FORUM_TEST_DATABASE_URL;

test("forum API integration", { skip: testDatabaseUrl ? false : "FORUM_TEST_DATABASE_URL is not set" }, async () => {
  process.env.NODE_ENV = "test";
  process.env.DATABASE_URL = testDatabaseUrl;
  process.env.IP_HASH_SECRET = "test-secret-for-forum-api";
  process.env.FORUM_READ_ONLY = "0";

  const config = getConfig();
  const migrationPool = createPool(config);
  await runMigrations(migrationPool);
  await migrationPool.query("TRUNCATE forum_replies, forum_posts RESTART IDENTITY CASCADE");
  await migrationPool.end();

  const app = await createApp({ config, logger: false, serveStatic: false });
  const clientIp = "203.0.113.10";

  try {
    const health = await app.inject({ method: "GET", url: "/healthz" });
    assert.equal(health.statusCode, 200);
    assert.equal(health.json().db, "ok");

    const emptyPost = await app.inject({
      method: "POST",
      url: "/api/v1/posts",
      headers: { "x-forwarded-for": clientIp },
      payload: { title: "", body: "", website: "" }
    });
    assert.equal(emptyPost.statusCode, 400);

    const honeypot = await app.inject({
      method: "POST",
      url: "/api/v1/posts",
      headers: { "x-forwarded-for": clientIp },
      payload: { title: "测试标题", body: "测试正文", website: "https://spam.example" }
    });
    assert.equal(honeypot.statusCode, 400);

    const tooManyUrls = await app.inject({
      method: "POST",
      url: "/api/v1/posts",
      headers: { "x-forwarded-for": clientIp },
      payload: {
        title: "链接太多",
        body: "https://a.example https://b.example https://c.example https://d.example https://e.example https://f.example",
        website: ""
      }
    });
    assert.equal(tooManyUrls.statusCode, 400);

    const created = await app.inject({
      method: "POST",
      url: "/api/v1/posts",
      headers: { "x-forwarded-for": clientIp },
      payload: {
        authorName: "",
        title: "第一帖 <script>alert(1)</script>",
        body: "正文第一行\n<script>alert(1)</script>",
        website: ""
      }
    });
    assert.equal(created.statusCode, 201);
    const postId = created.json().id as string;
    assert.ok(postId);

    const list = await app.inject({ method: "GET", url: "/api/v1/posts" });
    assert.equal(list.statusCode, 200);
    assert.equal(list.json().posts.length, 1);
    assert.equal(list.json().posts[0].authorName, "匿名玩家");
    assert.match(list.json().posts[0].title, /<script>/);

    const detail = await app.inject({ method: "GET", url: `/api/v1/posts/${postId}` });
    assert.equal(detail.statusCode, 200);
    assert.match(detail.json().post.body, /<script>/);
    assert.deepEqual(detail.json().replies, []);

    const reply = await app.inject({
      method: "POST",
      url: `/api/v1/posts/${postId}/replies`,
      headers: { "x-forwarded-for": clientIp },
      payload: { authorName: "", body: "匿名回复\n第二行", website: "" }
    });
    assert.equal(reply.statusCode, 201);

    const detailAfterReply = await app.inject({ method: "GET", url: `/api/v1/posts/${postId}` });
    assert.equal(detailAfterReply.statusCode, 200);
    assert.equal(detailAfterReply.json().post.replyCount, 1);
    assert.equal(detailAfterReply.json().replies.length, 1);
    assert.equal(detailAfterReply.json().replies[0].authorName, "匿名玩家");

    await app.inject({
      method: "POST",
      url: "/api/v1/posts",
      headers: { "x-forwarded-for": "203.0.113.99" },
      payload: { title: "限流 1", body: "内容", website: "" }
    });
    await app.inject({
      method: "POST",
      url: "/api/v1/posts",
      headers: { "x-forwarded-for": "203.0.113.99" },
      payload: { title: "限流 2", body: "内容", website: "" }
    });
    await app.inject({
      method: "POST",
      url: "/api/v1/posts",
      headers: { "x-forwarded-for": "203.0.113.99" },
      payload: { title: "限流 3", body: "内容", website: "" }
    });
    const limited = await app.inject({
      method: "POST",
      url: "/api/v1/posts",
      headers: { "x-forwarded-for": "203.0.113.99" },
      payload: { title: "限流 4", body: "内容", website: "" }
    });
    assert.equal(limited.statusCode, 429);
  } finally {
    await app.close();
  }
});
