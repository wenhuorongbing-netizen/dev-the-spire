# Overnight Diff Ledger

Date: 2026-06-10

Current note: this ledger is a historical June 10 snapshot. Batch 4c
localization fallback patches were later migrated through RitsuLib on
2026-06-22; use
`docs/archive/feature-audits/ritsulib-migration/batch-4c-candidates-20260623.md`
for the historical batch record and `docs/patch-inventory.md` for current
patch status.

## Diff Groups

| Group | Summary |
|---|---|
| Source/test API compatibility | Lotha and Martyr Oath additive power hooks, Ectoplasm gold modifier patch, Meat Cleaver rest-site availability patch, related guard-test string updates. |
| Sts1Events null guards | Owner guards expanded across the compile-included Sts1Events model set, clearing the current nullable warning debt to 0. |
| RitsuLib docs | Runtime dependency status changed from missing/blocking to historical `v0.106.1` loader-gate evidence plus installed `STS2-RitsuLib` `v0.4.16` / `lib\0.107.0`; fresh current smoke, package parity, and release blockers remain open. |
| Initial Batch 4c candidate snapshot | At this June 10 snapshot, a candidate list existed and no migration had been performed; superseded by the 2026-06-22 localization fallback migration. |
| Revision L owner-review docs | New `m5-revision-l-*` packet docs and recreated overnight status ledgers. |
| Harness/current status docs | Harness status/focus and active status docs aligned to Revision L naming and current warning/runtime boundaries. |
| Generated inventory | `docs/patch-inventory.md` regenerated after patch-target row drift. |

## Package Boundary

No package file was refreshed. The diff is source/docs/test state only until the owner requests a versioned tester package.
