# Website Claim Audit

Source: `.tools/archive/local-website-preview-20260516/`.

The archived website is not an active release surface. It is retained only as a source of old claims that must be accepted, corrected, or excluded before any public page is restored.

## Source Condition

| Source | Observation | Decision |
| --- | --- | --- |
| `website/content-data.js` | Contains old `EasyFirePlus` naming and visible mojibake text. | Do not publish as-is. |
| `website/README.md` | Describes static preview and a deleted Pages workflow. | Historical only. |
| `.github/workflows/spire-plus-site.yml` | Archived with the removed website draft. | Not active CI. |
| `website/assets/**` | Contains draft icons/cards/events copied into the local website snapshot. | Not release evidence. |

## Claim Audit

| Claim | Source | Current evidence | Status | Release risk |
| --- | --- | --- | --- | --- |
| Download package `SpirePlus-v0.1.0-private-beta.0.zip` | `content-data.js` download block | `docs/issues.md`, `publish/SpirePlus-v0.1.0-private-beta.0.zip` | partial | Hash can go stale; website must read from release docs or be updated during package. |
| Product name `EasyFirePlus` / mojibake Ancient v2.3 label | `content-data.js`, `website/README.md` | Current player name is `Spire Plus`; manifest id is `EZMicroBalance` | needs owner decision | Wrong name/version must be removed or explicitly rebranded before website restoration. |
| Ancient reward rebalance details | `content-data.js` rework groups | `EZMicroBalanceCode/Ancients/`, `docs/features/ancients-rework-v4/` | partial | Some claims are source-backed but live reward proof remains pending. |
| Urda ten blessings | `content-data.js` Urda section | `EZMicroBalanceCode/Ancients/Expansion/Urda/`, guard tests | partial | Live clicked UI, Root Eyes, Seed Bank, save/load, and co-op proof remain pending. |
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

The 2026-05-20 Product Spec Curator read the archived website and current docs in read-only mode. Their highest-risk findings still apply to player claims: old `EasyFirePlus` branding conflicts with `Spire Plus`, Vakuu must stay hidden until live proof exists, A11-A20 and Ancient blessings are partial because live proof is pending, and Preview tools now ship inside the Spire Plus page with live proof still pending.
