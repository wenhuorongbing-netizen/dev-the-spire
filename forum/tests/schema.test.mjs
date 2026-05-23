import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { test } from "node:test";

const schema = readFileSync(new URL("../supabase/schema.sql", import.meta.url), "utf8");

test("Supabase schema keeps anonymous forum tables under RLS", () => {
  assert.match(schema, /create table if not exists public\.forum_posts/i);
  assert.match(schema, /create table if not exists public\.forum_replies/i);
  assert.match(schema, /alter table public\.forum_posts enable row level security/i);
  assert.match(schema, /alter table public\.forum_replies enable row level security/i);
});

test("anonymous users cannot update or delete through granted columns", () => {
  assert.match(schema, /revoke all on table public\.forum_posts from anon, authenticated/i);
  assert.match(schema, /grant insert \(author_name, title, body, client_id\)\s+on public\.forum_posts/i);
  assert.match(schema, /grant insert \(post_id, author_name, body, client_id\)\s+on public\.forum_replies/i);
});

test("schema includes basic spam limits", () => {
  assert.match(schema, /forum_url_count\(body\) <= 5/i);
  assert.match(schema, /forum_recent_post_count\(client_id, interval '10 minutes'\) < 3/i);
  assert.match(schema, /forum_recent_reply_count\(client_id, interval '10 minutes'\) < 10/i);
  assert.match(schema, /security definer/i);
  assert.match(schema, /interval '10 minutes'/i);
  assert.match(schema, /interval '1 day'/i);
});
