# Release Evidence Status

Current target: `Spire Plus` manual-test build for user validation. This page is a compact dashboard for evidence state; it does not replace `docs/release-checklist.md`.

Do not mark a row passed from source review alone. A runtime row needs live evidence, logs, screenshots, or an explicit owner-approved deferral.

## Current Package

Source of truth: `docs/issues.md`.

| Artifact | SHA256 |
| --- | --- |
| ZIP | `CE417F595E2CCE8435C0575D95A3A866CBDA8FD605DE3F40014639E9301EFF62` |
| DLL | `940F1FEA66B01CB54A1CCEC388D4F023693C947395C7B7F9922BF596A8586E1E` |
| PCK | `3CDB72F1225FF2492F536091772979983653865F2902E2B485BBCB16B4FD1392` |
| Manifest | `A41EBF8ABEDCFC09DBB02CB655D7E50465888065ABA77F8EF087E87206F276CF` |
| README_INSTALL | `BA885193452EBA22A78433304F383A87A0830FA5E935A20B63BBAA08ABEBB906` |

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

## Runtime Evidence Logs

Set this before launching the game to add grep-friendly evidence lines to `godot.log`:

```powershell
$env:EZMB_RELEASE_EVIDENCE_LOG='1'
```

Expected marker format:

```text
[EZMB-EVIDENCE] <Feature> <Event> run=<run> player=<player> net=<single/host/client> data=<json-ish>
```

The marker helps collect proof; it does not turn a pending row into passed evidence by itself.
