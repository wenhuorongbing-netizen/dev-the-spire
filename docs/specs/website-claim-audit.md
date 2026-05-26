# Website Claim Audit

Current source: `website/` and `.github/workflows/spire-plus-site.yml`.

The archived website under `.tools/archive/local-website-preview-20260516/` is historical comparison material. The active website is a public-info surface, not release-readiness proof.

## Source Condition

| Source | Observation | Decision |
| --- | --- | --- |
| `website/content-data.js` | Current tracked website data for public effect tables and package metadata. | Keep aligned with current docs and manual-test status. |
| `website/README.md` | Current site maintenance notes for GitHub Pages and the forum entry. | Active support doc. |
| `.github/workflows/spire-plus-site.yml` | Current Pages workflow; builds `forum/` before uploading `website/`. | Active CI for the website only. |
| `.tools/archive/local-website-preview-20260516/` | Historical pre-promotion website snapshot. | Comparison source only. |

## Claim Audit

| Claim | Source | Current evidence | Status | Release risk |
| --- | --- | --- | --- | --- |
| Download package `SpirePlus-v0.1.0-private-beta.78.zip` | `content-data.js` download block | `docs/issues.md`, `publish/SpirePlus-v0.1.0-private-beta.78.zip` | partial | Hash can go stale; website must read from release docs or be updated during package. |
| Product name / stale terminology / mojibake page copy | `content-data.js`, `website/README.md` | Current player name is `Spire Plus`; manifest id is `EZMicroBalance`; README is readable; A19/A20 website copy now uses dedicated abilities / Branded Form. | source-fixed / live-site-pending | Keep guarded because public pages can drift from current mod localization and package hashes. |
| Ancient reward rebalance details | `content-data.js` rework groups | `EZMicroBalanceCode/Ancients/`, `docs/features/ancients-rework-v4/` | partial | Some claims are source-backed but live reward proof remains pending. |
| Urda eleven blessings | `content-data.js` Urda section | `EZMicroBalanceCode/Ancients/Expansion/Urda/`, guard tests | partial | Live clicked UI, Root Eyes, Seed Bank, save/load, and co-op proof remain pending. |
| Morvi eight blessings | `content-data.js` Morvi section | `EZMicroBalanceCode/Ancients/Expansion/Morvi/`, guard tests | partial | Live freeze reports, save/load, and co-op proof remain pending. |
| Lotha eight blessings | `content-data.js` Lotha section | `EZMicroBalanceCode/Ancients/Expansion/Lotha/`, guard tests | partial | Lethal path, save/load, and co-op proof remain pending. |
| Vakuu fight | `content-data.js` Vakuu section | `EZMicroBalanceCode/Ancients/Expansion/Vakuu/` | hidden | Hidden-by-default; do not advertise as playable until victory/failure/save/co-op live proof exists. |
| Ascension A11-A20 | `content-data.js` ascensions array | `EZMicroBalanceCode/Ascension/`, guard tests | partial | A11 traversal, A20 flow, save/load, and co-op proof remain pending. |
| Rootblight and Blight Sprout | `content-data.js` tokens/global mechanics | `EZMicroBalanceCode/Ascension/Cards/`, package art guards | partial | Live visual and combat-end proof remain pending. |
| Preview tools | Crystal Sphere peek and transform preview are now part of Spire Plus | `EZMicroBalanceCode/Preview/`, `tests/EZMicroBalance.Tests/PreviewToolsGuardTests.cs` | partial | Preview tools now ship inside the Spire Plus page; live proof is still pending. |
| Full co-op support | Implied by release plan goals, not proven by website | `docs/release-evidence-status.md` co-op row is pending | needs owner decision | Must be either proven with two clients or marked unsupported/gated. |

## Restoration Rule

A restored website must be generated from `docs/specs/release-scope-v1.md` and `docs/specs/release-traceability-matrix.md`, not from the archived `content-data.js` text.

## Subagent Review Note

The 2026-05-20 Product Spec Curator read the archived website and current docs in read-only mode. The same release-risk standard still applies to the active site: Vakuu must stay hidden until live proof exists, A11-A20 and Ancient blessings are partial because live proof is pending, and Preview tools now ship inside the Spire Plus page with live proof still pending.
