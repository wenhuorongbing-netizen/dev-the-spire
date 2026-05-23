import assert from "node:assert/strict";
import { existsSync, readFileSync } from "node:fs";
import { test } from "node:test";

const config = readFileSync(new URL("../vite.config.ts", import.meta.url), "utf8");

test("forum build uses stable asset names for GitHub Pages caching", () => {
  assert.match(config, /entryFileNames:\s*"assets\/forum\.js"/);
  assert.match(config, /assetInfo\.name\?\.endsWith\("\.css"\)/);
  assert.match(config, /"assets\/forum\.css"/);
});

test("legacy hashed forum entry files forward stale cached pages", () => {
  for (const fileName of ["index-Bl0NLLnu.js", "index-BooZ1VlJ.js", "index-BhStG2lr.js"]) {
    const fileUrl = new URL(`../public/assets/${fileName}`, import.meta.url);
    assert.equal(existsSync(fileUrl), true, `${fileName} should exist`);
    assert.equal(readFileSync(fileUrl, "utf8").trim(), 'import "./forum.js";');
  }

  const legacyCssUrl = new URL("../public/assets/index-CiKFE3yG.css", import.meta.url);
  assert.equal(existsSync(legacyCssUrl), true, "legacy CSS shim should exist");
  assert.equal(readFileSync(legacyCssUrl, "utf8").trim(), '@import "./forum.css";');
});
