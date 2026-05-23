import path from "node:path";
import { fileURLToPath } from "node:url";

export const VERSION = "0.1.0";

const rootDir = path.resolve(path.dirname(fileURLToPath(import.meta.url)), "../..");

export type ForumConfig = {
  nodeEnv: string;
  host: string;
  port: number;
  databaseUrl: string;
  databaseSsl: boolean;
  ipHashSecret: string;
  corsOrigins: string[];
  readOnly: boolean;
  rootDir: string;
  clientDistDir: string;
};

function requireEnv(name: string): string {
  const value = process.env[name];
  if (!value) {
    throw new Error(`${name} is required`);
  }
  return value;
}

function parsePort(value: string | undefined): number {
  const parsed = Number(value ?? "8787");
  return Number.isInteger(parsed) && parsed > 0 ? parsed : 8787;
}

function parseCorsOrigins(value: string | undefined): string[] {
  return (value ?? "")
    .split(",")
    .map((origin) => origin.trim())
    .filter(Boolean);
}

export function getConfig(): ForumConfig {
  const nodeEnv = process.env.NODE_ENV ?? "development";
  const production = nodeEnv === "production";
  const databaseUrl = process.env.DATABASE_URL ?? "postgres://forum:forum@localhost:54329/forum";
  const ipHashSecret =
    process.env.IP_HASH_SECRET ??
    (production ? requireEnv("IP_HASH_SECRET") : "dev-only-change-this-ip-hash-secret");

  return {
    nodeEnv,
    host: process.env.HOST ?? "0.0.0.0",
    port: parsePort(process.env.PORT),
    databaseUrl,
    databaseSsl: process.env.DATABASE_SSL === "1",
    ipHashSecret,
    corsOrigins: parseCorsOrigins(process.env.CORS_ORIGINS),
    readOnly: process.env.FORUM_READ_ONLY === "1",
    rootDir,
    clientDistDir: path.join(rootDir, "dist", "client")
  };
}

export function isAllowedOrigin(origin: string | undefined, config: ForumConfig): boolean {
  if (!origin) {
    return true;
  }

  if (config.corsOrigins.includes(origin)) {
    return true;
  }

  try {
    const parsed = new URL(origin);
    return (
      (parsed.hostname === "localhost" || parsed.hostname === "127.0.0.1" || parsed.hostname === "::1") &&
      ["http:", "https:"].includes(parsed.protocol)
    );
  } catch {
    return false;
  }
}
