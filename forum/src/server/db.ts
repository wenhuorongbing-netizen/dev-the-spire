import pg from "pg";
import type { ForumConfig } from "./config.js";

const { Pool } = pg;

export function createPool(config: ForumConfig): pg.Pool {
  return new Pool({
    connectionString: config.databaseUrl,
    max: 10,
    ssl: config.databaseSsl ? { rejectUnauthorized: false } : undefined
  });
}

export type DbPool = pg.Pool;
export type DbClient = pg.PoolClient;
