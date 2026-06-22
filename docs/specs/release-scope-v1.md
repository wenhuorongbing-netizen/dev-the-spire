# Release Scope v1

Scope date: 2026-05-20.

This file freezes the release-candidate decision boundary requested by `docs/goal.md`. It does not claim release readiness.

## Product Decision

| Surface | Release stance | Reason |
| --- | --- | --- |
| Spire Plus | Manual-test build now; release candidate only after live evidence | Automation and beta.105 forced Ancient clicked UI smoke pass, but gameplay, save/load, failure/death, hover/readability follow-through, gated Vakuu fight-option, and co-op rows remain pending. |
| `EZMicroBalance` manifest id | Keep unchanged | Saved fields, package folders, env vars, and compatibility depend on the stable id. |
| Ascension 11-20 | Development-test surface, default-on for private beta testing | Source and guard coverage exist; live traversal and co-op proof remain pending. |
| Ascension 21-30 | Out of scope | Explicit project rule. |
| Custom character | Out of scope | Explicit project rule. |
| Vakuu fight | Hidden by default | Source hardening exists, but victory/no-black-screen, failure/death, save/load, and co-op proof remain pending. |
| Ancient Urda/Morvi/Lotha | Manual-test candidates | Source-backed and visible reward markers exist; beta.105 forced clicked UI smoke exists, while gameplay, hover/readability follow-through, save/load, and co-op proof remain pending. |
| Rootblight / Blight Sprout | Manual-test candidate | Source and art are packaged; combat-end behavior and visual proof remain pending. |
| Preview tools | Part of Spire Plus | Crystal Sphere peek and transform preview are integrated under `EZMicroBalance`; live proof remains pending. |
| Website | Public-info surface, not mod-release proof | Current source is tracked under `website/` with a Pages workflow. It may describe the manual-test package, but it must not turn pending live rows into release-ready claims. |

## Release Candidate Gate

The project may use `GO: Release Candidate can be published` only when all applicable rows below have direct evidence:

| Gate | Required evidence |
| --- | --- |
| Loader | Current package loader smoke with current RitsuLib saved-state registration shape and clean `godot.log`. |
| UI | Clicked Ancient UI screenshots/logs with readable hover and marker relic visibility. |
| Gameplay | Manual feature matrix for Ancient rewards, A11-A20, Rootblight, and gated Vakuu. |
| Save/load | Live save/quit/load rows for every stateful feature. |
| Failure/death | Vakuu and Lotha lethal paths do not corrupt room, reward, or combat state. |
| Co-op | Two-client logs or explicit unsupported/gated release notes. |
| Governance | Reviewable commit split, fresh patch inventory, full no-game validation, and release evidence verifier. |

## Acceptable No-Go Gate

If live proof is incomplete, the acceptable result is `NO-GO: release blocked, manual-test build only`.

Unproven features must stay gated, hidden, unadvertised, or explicitly marked unsupported. Source review, tests, and package hashes are not enough to close live rows.

## Preview Tools Decision

Preview tools are no longer a separate mod. They ship inside `Spire Plus` and stay isolated under `EZMicroBalanceCode/Preview/`. They can move from manual-test helper to release-candidate surface only after:

- Crystal Sphere peek is proven live to only change mask opacity and never call reveal/reward paths.
- Transform preview is proven live to match the actual transformation result without advancing real RNG.
- The product decision for `affects_gameplay` is recorded.

## Website Decision

The current website source lives under tracked `website/` and `.github/workflows/spire-plus-site.yml`. The archived preview under `.tools/archive/local-website-preview-20260516/` is historical claim evidence only.

Website rules:

1. Keep source changes visible in `git status`.
2. Keep generated `website/forum/` output ignored.
3. Make every claim match `docs/specs/release-traceability-matrix.md`.
4. Keep the Pages workflow build/test step aligned with `forum/`.
5. Keep download/install text aligned with current package hashes and release state.
