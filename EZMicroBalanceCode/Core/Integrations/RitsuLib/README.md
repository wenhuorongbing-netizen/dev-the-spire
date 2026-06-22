# Core/Integrations/RitsuLib

RitsuLib bootstrap integration lives here.

Current source target: Slay the Spire 2 `v0.107.1`, `STS2.RitsuLib`
`0.4.33`, and Spire Plus `v0.1.0-private-beta.105`.

This directory owns the RitsuLib bootstrap, migrated patch registration,
content-pack registration, and SavedAttachedState field registration helper.
`SpirePlusMigratedPatchRegistry` owns the explicit migrated patch list so
`RitsuLibBootstrap` can stay focused on startup order and legacy Harmony
fallback boundaries.
Settings UI registration lives in `EZMicroBalanceCode/Config` but also uses
RitsuLib APIs.

Current beta.105 evidence covers package parity, runtime preflight,
source-workspace validation, and smoke-level clicked Ancient UI for Urda,
Morvi, Lotha, and normal Vakuu. That proves forced clicked UI visibility only;
gameplay, save-load, gated Vakuu fight-option/victory return, co-op, release,
and handoff proof remain separate gates.

Previous beta.99 settings/off proof, beta.96 direct Off proof, and beta.93
AdditiveBatch1 proof are retained only as previous-package context. Use
`docs/integrations/ritsulib.md` for the current RitsuLib record and
`docs/features/ritsulib-migration/README.md` as the migration entry point.
