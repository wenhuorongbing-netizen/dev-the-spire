# M5 Revision M Patch Failure Boundary

Date: 2026-06-11
Status: archived beta.84/beta.87 API-drift history.

Full archived record:

- `docs/archive/legacy-planning/m5-revision-m-patch-failure-ledger-20260611.md`

The archived ledger keeps the red beta.84 Off-smoke patch failures and the then-current source dispositions. `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/godot.log.after-launch` was the then-current beta.87 package proof for `v0.107.0`.

Current Spire Plus beta.93 no longer depends on BaseLib and uses
STS2-RitsuLib `0.4.31` with `lib/0.107.1`.

Required next proof is no longer loader smoke; run gameplay, save-load, render, replacement, multiplayer, and QA checks only if process coordination is clear.
