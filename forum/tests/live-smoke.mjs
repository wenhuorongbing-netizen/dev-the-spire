import assert from "node:assert/strict";
import { randomUUID } from "node:crypto";
import { createClient } from "@supabase/supabase-js";

const enabled = process.env.SPIRE_PLUS_FORUM_LIVE_TEST === "1";
const url = process.env.VITE_SUPABASE_URL || process.env.SPIRE_PLUS_SUPABASE_URL;
const anonKey = process.env.VITE_SUPABASE_ANON_KEY || process.env.SPIRE_PLUS_SUPABASE_ANON_KEY;
const serviceRoleKey = process.env.SUPABASE_SERVICE_ROLE_KEY || process.env.SPIRE_PLUS_SUPABASE_SERVICE_ROLE_KEY;
const allowVisiblePost = process.env.SPIRE_PLUS_FORUM_ALLOW_VISIBLE_SMOKE_POST === "1";

function fail(message) {
  console.error(message);
  process.exit(1);
}

if (!enabled) {
  fail("Set SPIRE_PLUS_FORUM_LIVE_TEST=1 to run the live Supabase forum smoke test.");
}

if (!url || !anonKey || url.includes("your-project-ref")) {
  fail("Set VITE_SUPABASE_URL and VITE_SUPABASE_ANON_KEY, or SPIRE_PLUS_SUPABASE_URL and SPIRE_PLUS_SUPABASE_ANON_KEY.");
}

if (!serviceRoleKey && !allowVisiblePost) {
  fail("Set SUPABASE_SERVICE_ROLE_KEY for automatic cleanup, or set SPIRE_PLUS_FORUM_ALLOW_VISIBLE_SMOKE_POST=1 to leave the smoke-test post visible.");
}

const anon = createClient(url, anonKey, {
  auth: { persistSession: false, autoRefreshToken: false }
});

const admin = serviceRoleKey
  ? createClient(url, serviceRoleKey, {
      auth: { persistSession: false, autoRefreshToken: false }
    })
  : null;

const clientId = randomUUID();
const marker = `smoke-${Date.now()}-${clientId.slice(0, 8)}`;
const title = `[smoke] Spire Plus forum ${marker}`;
const postBody = `Automated forum smoke test post ${marker}.`;
const replyBody = `Automated forum smoke test reply ${marker}.`;

let postId = null;
let replyId = null;

try {
  const postInsert = await anon
    .from("forum_posts")
    .insert({
      author_name: "",
      title,
      body: postBody,
      client_id: clientId
    })
    .select("id, author_name, title, body, reply_count")
    .single();

  if (postInsert.error) throw postInsert.error;
  postId = postInsert.data.id;

  assert.equal(postInsert.data.author_name, "匿名玩家");
  assert.equal(postInsert.data.title, title);
  assert.equal(postInsert.data.body, postBody);

  const replyInsert = await anon
    .from("forum_replies")
    .insert({
      post_id: postId,
      author_name: "Smoke Test",
      body: replyBody,
      client_id: clientId
    })
    .select("id, post_id, author_name, body")
    .single();

  if (replyInsert.error) throw replyInsert.error;
  replyId = replyInsert.data.id;

  assert.equal(replyInsert.data.post_id, postId);
  assert.equal(replyInsert.data.author_name, "Smoke Test");
  assert.equal(replyInsert.data.body, replyBody);

  const postRead = await anon
    .from("forum_posts")
    .select("id, title, body, reply_count")
    .eq("id", postId)
    .single();

  if (postRead.error) throw postRead.error;
  assert.equal(postRead.data.title, title);
  assert.equal(postRead.data.body, postBody);
  assert.equal(postRead.data.reply_count, 1);

  const repliesRead = await anon
    .from("forum_replies")
    .select("id, post_id, body")
    .eq("post_id", postId);

  if (repliesRead.error) throw repliesRead.error;
  assert.ok(repliesRead.data.some((reply) => reply.id === replyId && reply.body === replyBody));

  console.log(`Forum live smoke test passed. post=${postId} reply=${replyId}`);
} finally {
  if (admin && replyId) {
    const { error } = await admin.from("forum_replies").update({ status: "hidden" }).eq("id", replyId);
    if (error) console.error(`Failed to hide smoke-test reply ${replyId}: ${error.message}`);
  }

  if (admin && postId) {
    const { error } = await admin.from("forum_posts").update({ status: "hidden" }).eq("id", postId);
    if (error) console.error(`Failed to hide smoke-test post ${postId}: ${error.message}`);
  }

  if (!admin && postId) {
    console.log(`Smoke-test post remains visible: ${postId}`);
  }
}
