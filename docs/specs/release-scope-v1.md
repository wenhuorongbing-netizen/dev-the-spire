# Release Scope v1

Scope date: 2026-05-20.

This file freezes the release-candidate decision boundary requested by `docs/goal.md`. It does not claim release readiness.

## Product Decision

| Surface | Release stance | Reason |
| --- | --- | --- |
| Spire Plus / `EZMicroBalance` | Manual-test build now; release candidate only after live evidence | Automation passes, but loader, clicked UI, gameplay, save/load, failure/death, and co-op rows remain pending. |
| `EZMicroBalance` manifest id | Keep unchanged | Saved fields, package folders, env vars, and compatibility depend on the stable id. |
| Ascension 11-20 | Development-test surface, default-on for private beta testing | Source and guard coverage exist; live traversal and co-op proof remain pending. |
| Ascension 21-30 | Out of scope | Explicit project rule. |
| Custom character | Out of scope | Explicit project rule. |
| Vakuu fight | Hidden by default | Source hardening exists, but victory/no-black-screen, failure/death, save/load, and co-op proof remain pending. |
| Ancient Urda/Morvi/Lotha | Manual-test candidates | Source-backed and visible reward markers exist; live clicked UI, gameplay, save/load, and co-op proof remain pending. |
| Rootblight / Blight Sprout | Manual-test candidate | Source and art are packaged; combat-end behavior and visual proof remain pending. |
| Future Peek / `EZFuturePeek` | Separate beta decision, not part of Spire Plus | It has an independent project/manifest; live Crystal Sphere and transform-preview proof remain pending. |
| Website | Not in current release surface | The old static preview is archived only and contains stale/mojibake claims. Do not publish or advertise it until rebuilt from this scope. |

## Release Candidate Gate

The project may use `GO: Release Candidate can be published` only when all applicable rows below have direct evidence:

| Gate | Required evidence |
| --- | --- |
| Loader | Current package loader smoke with current SavedSpireField count and clean `godot.log`. |
| UI | Clicked Ancient UI screenshots/logs with readable hover and marker relic visibility. |
| Gameplay | Manual feature matrix for Ancient rewards, A11-A20, Rootblight, and gated Vakuu. |
| Save/load | Live save/quit/load rows for every stateful feature. |
| Failure/death | Vakuu and Lotha lethal paths do not corrupt room, reward, or combat state. |
| Co-op | Two-client logs or explicit unsupported/gated release notes. |
| Governance | Reviewable commit split, fresh patch inventory, full no-game validation, and release evidence verifier. |

## Acceptable No-Go Gate

If live proof is incomplete, the acceptable result is `NO-GO: release blocked, manual-test build only`.

Unproven features must stay gated, hidden, unadvertised, or explicitly marked unsupported. Source review, tests, and package hashes are not enough to close live rows.

## Future Peek Decision

`EZFuturePeek` remains independent. It can move toward a separate beta only after:

- Crystal Sphere peek is proven live to only change mask opacity and never call reveal/reward paths.
- Transform preview is proven live to match the actual transformation result without advancing real RNG.
- The product decision for `affects_gameplay` is recorded.

## Website Decision

The archived website preview under `.tools/archive/local-website-preview-20260516/` is a claim source, not a publishing source.

To restore a website:

1. Promote source into a tracked `website/` folder.
2. Replace stale names and mojibake text.
3. Make every claim match `docs/specs/release-traceability-matrix.md`.
4. Add a build/smoke workflow.
5. Keep download/install text aligned with current package hashes and release state.
