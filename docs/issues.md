# EZ Micro Balance Issues

## Active blockers

| ID | Feature | Severity | Status | Blocker |
| --- | --- | --- | --- | --- |
| URDA-PROTOTYPE | Ancient expansion | P0 | open | Urda is default-on for private-beta testing and has a source-backed first gameplay slice for Seedbed, Humus Pact, Molting, and Moss Map; live gameplay/save-load verification is still pending, so do not make a release-ready gameplay claim. |
| MULTI-LIVE-MATRIX | Ascension 11-20 multiplayer | P1 | open | Two-client Steam validation and co-op save/load matrix still pending. |
| MULTI-ROUTE | Ascension 11 route checks | P1 | open | Natural route traversal and boss reachability evidence pending. |
| ROOTBLIGHT-PROOF | Ancient/Rootblight manual matrix | P1 | open | Runtime/manual validation matrix still pending. |

## Issue detail links

- `docs/issues/urda.md` (Urda prototype, blessed ids, and blockers)
- `docs/issues/ancient-expansion-v2.2.md` (Urda stabilization plus default-off Morvi hardening; Lotha/Vakuu planning)
- `docs/issues/waiting-tests.md` (current manual evidence queue)
- `docs/features/ancients-rework-v4/manual-verification-matrix.md`
- `docs/features/ascension-11-20/manual-test-checklist.md`

## Closing evidence rule

- Do not close a blocker without the linked source evidence, guard coverage, and the current manual test row marked done.
- `docs/issues.md` is an index only; implementation details are kept in feature-level docs.
