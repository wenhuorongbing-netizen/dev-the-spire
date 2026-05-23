import crypto from "node:crypto";
import type { FastifyRequest } from "fastify";
import type { ForumConfig } from "./config.js";

const URL_PATTERN = /\b(?:https?:\/\/|www\.)\S+/gi;

export function hmacSha256(value: string, secret: string): string {
  return crypto.createHmac("sha256", secret).update(value).digest("hex");
}

export function requestHashes(request: FastifyRequest, config: ForumConfig): {
  ipHash: string;
  userAgentHash: string;
} {
  const userAgent = request.headers["user-agent"] ?? "";
  return {
    ipHash: hmacSha256(request.ip || "unknown", config.ipHashSecret),
    userAgentHash: hmacSha256(String(userAgent), config.ipHashSecret)
  };
}

export function countUrls(text: string): number {
  return text.match(URL_PATTERN)?.length ?? 0;
}

export function normalizeAuthorName(value: string | undefined): string {
  const trimmed = value?.trim() ?? "";
  return trimmed.length > 0 ? trimmed : "匿名玩家";
}

export function trimText(value: string): string {
  return value.trim();
}
