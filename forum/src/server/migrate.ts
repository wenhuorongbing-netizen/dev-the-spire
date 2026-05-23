import "dotenv/config";
import fs from "node:fs/promises";
import path from "node:path";
import pg from "pg";
import { getConfig } from "./config.js";
import { createPool } from "./db.js";

const { DatabaseError } = pg;
const MIGRATION_LOCK_ID = "8753412907450841482";

async function ensureSchemaMigrations(client: pg.PoolClient): Promise<void> {
  await client.query(`
    CREATE TABLE IF NOT EXISTS schema_migrations (
      version text PRIMARY KEY,
      applied_at timestamptz NOT NULL DEFAULT now()
    )
  `);
}

export async function runMigrations(pool = createPool(getConfig())): Promise<string[]> {
  const migrationsDir = path.resolve(process.cwd(), "db", "migrations");
  const files = (await fs.readdir(migrationsDir))
    .filter((file) => file.endsWith(".sql"))
    .sort((a, b) => a.localeCompare(b));

  const applied: string[] = [];
  const client = await pool.connect();

  try {
    await client.query(`SELECT pg_advisory_lock(${MIGRATION_LOCK_ID})`);
    await ensureSchemaMigrations(client);

    for (const file of files) {
      const version = path.basename(file, ".sql");
      const existing = await client.query("SELECT 1 FROM schema_migrations WHERE version = $1", [version]);
      if (existing.rowCount && existing.rowCount > 0) {
        continue;
      }

      const sql = await fs.readFile(path.join(migrationsDir, file), "utf8");
      await client.query("BEGIN");
      try {
        await client.query(sql);
        await client.query("INSERT INTO schema_migrations (version) VALUES ($1)", [version]);
        await client.query("COMMIT");
        applied.push(version);
      } catch (error) {
        await client.query("ROLLBACK");
        throw error;
      }
    }
  } finally {
    await client.query(`SELECT pg_advisory_unlock(${MIGRATION_LOCK_ID})`);
    client.release();
  }

  return applied;
}

if (process.argv[1] && import.meta.url.endsWith(path.basename(process.argv[1]))) {
  const pool = createPool(getConfig());
  runMigrations(pool)
    .then(async (applied) => {
      console.log(applied.length ? `Applied migrations: ${applied.join(", ")}` : "No migrations to apply.");
      await pool.end();
    })
    .catch(async (error: unknown) => {
      if (error instanceof DatabaseError) {
        console.error(`Migration failed: ${error.message}`);
      } else {
        console.error(error);
      }
      await pool.end();
      process.exitCode = 1;
    });
}
