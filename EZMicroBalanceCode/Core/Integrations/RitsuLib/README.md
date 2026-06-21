# Core/Integrations/RitsuLib

RitsuLib bootstrap integration lives here.

Historical loader proof used StS2 v0.106.1 with the RitsuLib runtime
variant 0.106.1, and later beta.87 proof covered StS2 v0.107.0 with
RitsuLib v0.4.24. The current local game install is v0.107.1, and
official RitsuLib v0.4.32 direct NuGet runtime layout is installed. The
repo now compiles against `STS2.RitsuLib` 0.4.32, and the Spire Plus
manifest requires `STS2-RitsuLib` for the beta.99 package line. Current
beta.99 clicked settings UI proof is retained under
`.tools/runtime-evidence/mod-settings-beta99-ritsulib-click-20260621-223210/`.
Previous beta.96 Off proof and beta.93 AdditiveBatch1 loader proof are
retained only as previous-package loader/registration context under
`.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/`,
`.tools/runtime-evidence/v01071-beta93-ritsulib0431-off-direct-20260621/`
and
`.tools/runtime-evidence/v01071-beta93-ritsulib0431-additivebatch1-direct-20260621/`.

See `docs/integrations/ritsulib.md` for the full staging record and
migration plan.
