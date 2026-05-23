import fs from "node:fs";
import path from "node:path";
import cors from "@fastify/cors";
import helmet from "@fastify/helmet";
import fastify, {
  type FastifyBaseLogger,
  type FastifyInstance,
  type FastifyReply,
  type FastifyServerOptions
} from "fastify";
import fastifyStatic from "@fastify/static";
import { z } from "zod";
import { getConfig, isAllowedOrigin, VERSION, type ForumConfig } from "./config.js";
import { createPool, type DbPool } from "./db.js";
import { countUrls, normalizeAuthorName, requestHashes, trimText } from "./security.js";

type CreateAppOptions = {
  config?: ForumConfig;
  pool?: DbPool;
  logger?: FastifyServerOptions["logger"];
  serveStatic?: boolean;
};

const postInputSchema = z.object({
  authorName: z.string().max(32).optional().default(""),
  title: z.string().max(120),
  body: z.string().max(10000),
  website: z.string().optional().default("")
});

const replyInputSchema = z.object({
  authorName: z.string().max(32).optional().default(""),
  body: z.string().max(5000),
  website: z.string().optional().default("")
});

const idParamsSchema = z.object({
  id: z.coerce.number().int().positive()
});

const listQuerySchema = z.object({
  limit: z.coerce.number().int().min(1).max(50).optional().default(20),
  cursor: z.string().optional()
});

type PostRow = {
  id: string;
  author_name: string;
  title: string;
  body: string;
  reply_count: number;
  created_at: Date;
  last_activity_at: Date;
};

type ReplyRow = {
  id: string;
  author_name: string;
  body: string;
  created_at: Date;
};

function toIso(value: Date | string): string {
  return value instanceof Date ? value.toISOString() : new Date(value).toISOString();
}

function makeCursor(row: PostRow): string {
  return `${toIso(row.last_activity_at)}_${row.id}`;
}

function parseCursor(value: string | undefined): { lastActivityAt: string; id: string } | null {
  if (!value) {
    return null;
  }

  const separator = value.lastIndexOf("_");
  if (separator <= 0 || separator === value.length - 1) {
    return null;
  }

  const lastActivityAt = value.slice(0, separator);
  const id = value.slice(separator + 1);
  if (!Number.isInteger(Number(id)) || Number(id) <= 0 || Number.isNaN(Date.parse(lastActivityAt))) {
    return null;
  }

  return { lastActivityAt, id };
}

function postSummary(body: string): string {
  const compact = body.replace(/\s+/g, " ").trim();
  return compact.length > 160 ? `${compact.slice(0, 160)}...` : compact;
}

function visiblePost(row: PostRow) {
  return {
    id: row.id,
    authorName: row.author_name,
    title: row.title,
    body: row.body,
    bodySummary: postSummary(row.body),
    replyCount: row.reply_count,
    createdAt: toIso(row.created_at),
    lastActivityAt: toIso(row.last_activity_at)
  };
}

function visibleReply(row: ReplyRow) {
  return {
    id: row.id,
    authorName: row.author_name,
    body: row.body,
    createdAt: toIso(row.created_at)
  };
}

function sendValidationError(reply: FastifyReply, message = "请求内容不符合要求") {
  return reply.code(400).send({ error: "BAD_REQUEST", message });
}

function sendRateLimited(reply: FastifyReply) {
  return reply.code(429).send({ error: "RATE_LIMITED", message: "发帖太频繁，请稍后再试" });
}

function ensureWritable(config: ForumConfig, reply: FastifyReply): boolean {
  if (!config.readOnly) {
    return true;
  }

  reply.code(503).send({ error: "READ_ONLY", message: "论坛当前为只读模式" });
  return false;
}

async function isRateLimited(
  pool: DbPool,
  table: "forum_posts" | "forum_replies",
  ipHash: string,
  tenMinuteLimit: number,
  dailyLimit: number
): Promise<boolean> {
  const result = await pool.query<{ window_count: string; day_count: string }>(
    `
      SELECT
        count(*) FILTER (WHERE created_at >= now() - interval '10 minutes') AS window_count,
        count(*) FILTER (WHERE created_at >= now() - interval '1 day') AS day_count
      FROM ${table}
      WHERE ip_hash = $1
    `,
    [ipHash]
  );

  const row = result.rows[0];
  return Number(row?.window_count ?? 0) >= tenMinuteLimit || Number(row?.day_count ?? 0) >= dailyLimit;
}

function registerRoutes(app: FastifyInstance, pool: DbPool, config: ForumConfig) {
  app.get("/healthz", async (_request, reply) => {
    await pool.query("SELECT 1");
    return reply.send({ ok: true, db: "ok", version: VERSION });
  });

  app.get("/api/v1/posts", async (request, reply) => {
    const query = listQuerySchema.safeParse(request.query);
    if (!query.success) {
      return sendValidationError(reply);
    }

    const cursor = parseCursor(query.data.cursor);
    if (query.data.cursor && !cursor) {
      return sendValidationError(reply, "分页游标不正确");
    }

    const params: Array<string | number> = [query.data.limit + 1];
    let cursorWhere = "";
    if (cursor) {
      params.push(cursor.lastActivityAt, cursor.id);
      cursorWhere = "AND (last_activity_at, id) < ($2::timestamptz, $3::bigint)";
    }

    const result = await pool.query<PostRow>(
      `
        SELECT id, author_name, title, body, reply_count, created_at, last_activity_at
        FROM forum_posts
        WHERE status = 'visible'
        ${cursorWhere}
        ORDER BY last_activity_at DESC, id DESC
        LIMIT $1
      `,
      params
    );

    const rows = result.rows.slice(0, query.data.limit);
    const extra = result.rows[query.data.limit];
    return reply.send({
      posts: rows.map(visiblePost),
      nextCursor: extra ? makeCursor(extra) : null
    });
  });

  app.post("/api/v1/posts", async (request, reply) => {
    if (!ensureWritable(config, reply)) {
      return;
    }

    const input = postInputSchema.safeParse(request.body);
    if (!input.success) {
      return sendValidationError(reply);
    }

    const title = trimText(input.data.title);
    const body = trimText(input.data.body);
    if (input.data.website.trim().length > 0 || title.length < 1 || body.length < 1) {
      return sendValidationError(reply);
    }

    if (countUrls(body) > 5) {
      return sendValidationError(reply, "正文里的链接数量过多");
    }

    const hashes = requestHashes(request, config);
    if (await isRateLimited(pool, "forum_posts", hashes.ipHash, 3, 20)) {
      return sendRateLimited(reply);
    }

    const result = await pool.query<{ id: string }>(
      `
        INSERT INTO forum_posts (author_name, title, body, ip_hash, user_agent_hash)
        VALUES ($1, $2, $3, $4, $5)
        RETURNING id
      `,
      [normalizeAuthorName(input.data.authorName), title, body, hashes.ipHash, hashes.userAgentHash]
    );

    return reply.code(201).send({ id: result.rows[0].id });
  });

  app.get("/api/v1/posts/:id", async (request, reply) => {
    const params = idParamsSchema.safeParse(request.params);
    if (!params.success) {
      return sendValidationError(reply);
    }

    const postResult = await pool.query<PostRow>(
      `
        SELECT id, author_name, title, body, reply_count, created_at, last_activity_at
        FROM forum_posts
        WHERE id = $1 AND status = 'visible'
      `,
      [params.data.id]
    );
    const post = postResult.rows[0];
    if (!post) {
      return reply.code(404).send({ error: "NOT_FOUND", message: "帖子不存在" });
    }

    const repliesResult = await pool.query<ReplyRow>(
      `
        SELECT id, author_name, body, created_at
        FROM forum_replies
        WHERE post_id = $1 AND status = 'visible'
        ORDER BY created_at ASC, id ASC
      `,
      [params.data.id]
    );

    return reply.send({
      post: visiblePost(post),
      replies: repliesResult.rows.map(visibleReply)
    });
  });

  app.post("/api/v1/posts/:id/replies", async (request, reply) => {
    if (!ensureWritable(config, reply)) {
      return;
    }

    const params = idParamsSchema.safeParse(request.params);
    const input = replyInputSchema.safeParse(request.body);
    if (!params.success || !input.success) {
      return sendValidationError(reply);
    }

    const body = trimText(input.data.body);
    if (input.data.website.trim().length > 0 || body.length < 1) {
      return sendValidationError(reply);
    }

    if (countUrls(body) > 5) {
      return sendValidationError(reply, "正文里的链接数量过多");
    }

    const hashes = requestHashes(request, config);
    if (await isRateLimited(pool, "forum_replies", hashes.ipHash, 10, 80)) {
      return sendRateLimited(reply);
    }

    const client = await pool.connect();
    try {
      await client.query("BEGIN");
      const postResult = await client.query<{ id: string }>(
        "SELECT id FROM forum_posts WHERE id = $1 AND status = 'visible' FOR UPDATE",
        [params.data.id]
      );

      if (!postResult.rows[0]) {
        await client.query("ROLLBACK");
        return reply.code(404).send({ error: "NOT_FOUND", message: "帖子不存在" });
      }

      const replyResult = await client.query<{ id: string }>(
        `
          INSERT INTO forum_replies (post_id, author_name, body, ip_hash, user_agent_hash)
          VALUES ($1, $2, $3, $4, $5)
          RETURNING id
        `,
        [params.data.id, normalizeAuthorName(input.data.authorName), body, hashes.ipHash, hashes.userAgentHash]
      );
      await client.query(
        `
          UPDATE forum_posts
          SET reply_count = reply_count + 1,
              last_activity_at = now(),
              updated_at = now()
          WHERE id = $1
        `,
        [params.data.id]
      );
      await client.query("COMMIT");
      return reply.code(201).send({ id: replyResult.rows[0].id });
    } catch (error) {
      await client.query("ROLLBACK");
      throw error;
    } finally {
      client.release();
    }
  });
}

async function registerStatic(app: FastifyInstance, config: ForumConfig): Promise<void> {
  if (!fs.existsSync(config.clientDistDir)) {
    return;
  }

  await app.register(fastifyStatic, {
    root: config.clientDistDir,
    prefix: "/",
    index: "index.html"
  });

  app.setNotFoundHandler((request, reply) => {
    if (request.method === "GET" && !request.url.startsWith("/api/") && request.url !== "/healthz") {
      return reply.sendFile("index.html");
    }

    return reply.code(404).send({ error: "NOT_FOUND", message: "请求的资源不存在" });
  });
}

export async function createApp(options: CreateAppOptions = {}): Promise<FastifyInstance> {
  const config = options.config ?? getConfig();
  const pool = options.pool ?? createPool(config);
  const app = fastify({
    logger: options.logger ?? true,
    trustProxy: true,
    bodyLimit: 32 * 1024
  });

  await app.register(helmet, {
    contentSecurityPolicy: false
  });
  await app.register(cors, {
    origin(origin, callback) {
      callback(null, isAllowedOrigin(origin, config));
    }
  });

  registerRoutes(app, pool, config);

  app.setErrorHandler((error, _request, reply) => {
    const logger = app.log as FastifyBaseLogger;
    const errorWithStatus = error as { statusCode?: unknown };
    const statusCode = typeof errorWithStatus.statusCode === "number" ? errorWithStatus.statusCode : 500;
    if (statusCode >= 500) {
      logger.error(error);
      return reply.code(500).send({ error: "INTERNAL_ERROR", message: "论坛暂时无法连接" });
    }

    return reply.code(statusCode).send({ error: "BAD_REQUEST", message: "请求内容不符合要求" });
  });

  app.addHook("onClose", async () => {
    await pool.end();
  });

  if (options.serveStatic ?? fs.existsSync(config.clientDistDir)) {
    await registerStatic(app, config);
  }

  return app;
}
