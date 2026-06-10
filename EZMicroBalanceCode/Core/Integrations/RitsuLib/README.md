# Core/Integrations/RitsuLib

Reserved for future RitsuLib bootstrap module.

This directory will contain the RitsuLib integration entry point when the
RitsuLib compatibility target is resolved. Historical loader proof used
StS2 v0.106.1 with the RitsuLib runtime variant 0.106.1. The current
local game install is v0.107.0, and official RitsuLib v0.4.16 with
lib/0.107.0 is installed. Installed beta.84 package parity is restored,
but the fresh v0.107.0 beta.84 Off smoke is non-clean and cannot be used
as current runtime proof. The repo still compiles against `STS2.RitsuLib`
0.3.2.
Do not bump the compile package or manifest minimum in this dirty source
state; that belongs with the next owner-approved versioned package refresh
if it targets the v0.107.0 runtime.

See `docs/integrations/ritsulib.md` for the full staging record and
migration plan.
