# Release Evidence Status

Current target: `Spire Plus` manual-test build for user validation. This page is a compact dashboard for evidence state; it does not replace `docs/release-checklist.md`.

Do not mark a row passed from source review alone. A runtime row needs live evidence, logs, screenshots, or an explicit owner-approved deferral.

## Current Package

Source of truth: `docs/issues.md`.

| Artifact | SHA256 |
| --- | --- |
| ZIP | `B19620D8D8A15D5B96208D3DE8C3B372BCA0874E076DD2DEBEDE09422FF28BD2` |
| DLL | `A1D86D01E57E0F58617ACA23EA8094B1AF35F525E3254007DE3675A1289B8159` |
| PCK | `073CAF976C91D9E6CEA39FA90FB5A6417E66CD5E12DED5EDD8169C892A0F0538` |
| Manifest | `659943569D01C1DDD8B5C351D763497F7FEE513AD0BB84903D05B69F8DBD1AB2` |
| README_INSTALL | `C9F19363848AEECD4B763BFF7BB2B75980A90BFE22358ACEC8FF5E9E5C129CE4` |

## Evidence Rows

| Row | Status | Owner | Evidence Needed | Notes |
| --- | --- | --- | --- | --- |
| Current package automation | Passed | Codex | Build, tests, format, publish, package, artifact tests | Latest hashes are in `docs/issues.md`; rerun after package changes. |
| Fresh current-package loader smoke | Pending | User/Codex with game launch approval | Current 25-field loader log and clean `godot.log` audit | Historical loader evidence is for older field count. |
| Clicked Ancient UI | Pending | User | Urda, Morvi, Lotha, Vakuu normal, Vakuu fight screenshots and logs | Must prove event art, option art, dialogue, marker relic visibility, and hover readability. |
| Ancient reward gameplay | Pending | User | Manual runs for Urda, Morvi, Lotha, and vanilla rebalance rewards | Source guards are green; gameplay proof remains open. |
| Vakuu fight victory | Pending | User | Victory returns to parent event without black screen | Fight remains hidden by default. |
| Vakuu fight failure/death | Pending | User | Failure/death path logs and result note | Must not corrupt room/reward/combat state. |
| Save/load | Pending | User | Urda, Morvi, Lotha, Vakuu, Root Sight, Seed Bank, Rootblight restore rows | Deck mirrors are source mitigation, not live proof. |
| A11 natural route traversal | Pending | User | Click-by-click map traversal, width/row proof, logs | Source graph proof exists; natural UI route proof remains open. |
| Rootblight visuals and combat-end behavior | Pending | User | In-game art, hover, combat-end notice timing, Blight Sprout behavior | Generated art is packaged. |
| Disable-mod gameplay | Pending | User | Actual run with Spire Plus disabled and BaseLib enabled | Existing evidence covers startup only. |
| Co-op disposition | Pending | User | Two-client runbook logs or explicit unsupported release note | A20 co-op remains downgraded/unverified. |

## Verification Command

When manual evidence folders are filled, run:

```powershell
.\scripts\verify-spire-plus-release-evidence.ps1
```

Use `-AllowDeferred` only when the project owner explicitly accepts a release-note deferral for a row.
