# Core/Integrations/RitsuLib

RitsuLib bootstrap integration lives here.

Historical loader proof used StS2 v0.106.1 with the RitsuLib runtime
variant 0.106.1, and later beta.87 proof covered StS2 v0.107.0 with
RitsuLib v0.4.24. The current local game install is v0.107.1, and
official RitsuLib v0.4.28 with lib/0.107.1 is installed. The repo now
compiles against `STS2.RitsuLib` 0.4.28, and the Spire Plus manifest
requires `STS2-RitsuLib` for the beta.91 package line. Current beta.91
Off and AdditiveBatch1 loader proof is retained under
`.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/`
and
`.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/`.

See `docs/integrations/ritsulib.md` for the full staging record and
migration plan.
