# Urda Ancient Implementation Plan

## 0. Context

Current dependency and architecture baseline:

- Base game source refresh target: `v0.105.0`.
- BaseLib package target: `v3.1.2`.
- Active private-beta project: `EZMicroBalance`.

Urda work should remain independently disableable and independent from other ancient families.

## 1. Phase 0: Source evidence guard pass

Goal:

- Confirm local source APIs for Act 1 ancient registration and reward hooks.
- Confirm no immediate breakage from adding Urda classes and files.

Actions:

1. Read local `source code/src/Core` for `Ancient`, `AncientModel`, `AncientEvent`, and reward flow.
2. Confirm BaseLib command APIs for card/relic/add/remove and save fields.
3. Reconfirm current `EZMB_...` manifest and `docs/issues.md` Urda issue list.
4. Record findings in `api-research.md`.

Exit:

- Build remains green after docs-only changes.
- `api-research.md` has concrete pass/fail API items.

## 2. Phase 1: Urda framework

Goal:

- Make Urda selectable in Act 1 and expose an enabled blessing pool limited to implemented blessings.

Actions:

1. Implement or wire Urda identity registration path.
2. Add `urda` blessing registry with enabled flags.
3. Ensure inactive blessings are excluded from live pool.
4. Add explicit debug/test env vars (`EZMB_FORCE_ANCIENT`, `EZMB_FORCE_URDA_BLESSING`) as default-off if needed for diagnostics.
5. Add source-guarded fallback behavior when registration path is unavailable.

Exit:

- New run can reach and select Urda.
- Offer count appears as configured for enabled blessing set.

## 3. Phase 2: Active blessing slice

Order by safety:

1. Seedbed.
2. Humus Pact.
3. Molting + `Withered Husk`.
4. Moss Map.

For each blessing:

- Implement guarded hooks only.
- Add required save/load fields.
- Add EN and ZHS localization keys.
- Add manual checklist rows.
- Add/adjust tests where practical.

## 4. Phase 3: Diagnostics and release hygiene

Goal:

- Add lightweight read-only diagnostics and keep unsafe content out of live path.

Actions:

- Add one-time logs for selection, blessing id, and state transitions.
- Add fallback notices for missing required references.
- Keep all non-live diagnostics in default-off mode.

## 5. Phase 4: Finalization

Goal:

- Close documentation and runtime evidence for private beta handoff.

Actions:

- Populate all Urda manual rows.
- Add source-guard tests if coverage is practical.
- Update `docs/issues.md` issue status as work lands.
- Run `dotnet build`.
- Publish only if resources or localization changed.
- Run smoke/load checks before any public readiness claim.

No milestone in this branch should ship active behavior if:

- `Seedbed`, `Humus Pact`, `Molting`, or `Moss Map` logic is incomplete.
- save/load behavior is broken.
- live manual checks are still unexecuted.

