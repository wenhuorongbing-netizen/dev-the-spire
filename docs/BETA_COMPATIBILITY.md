# Beta Compatibility

## 2026-06-22 Current Compatibility Boundary

Current compatibility work targets:

- Slay the Spire 2 `v0.107.1`
- Spire Plus `v0.1.0-private-beta.123`
- STS2-RitsuLib `v0.4.34` in direct NuGet runtime layout
- Stable technical manifest id `EZMicroBalance`

Spire Plus is RitsuLib-only for the current package line. The project
references `STS2.RitsuLib` `0.4.34`, and `EZMicroBalance.json` declares
only `STS2-RitsuLib >= 0.4.34` as the runtime dependency.

Current evidence:

- beta.123 build, publish, package refresh, installed package parity, and
  clicked Ancient UI smoke are
  recorded in `PROJECT_STATE.md` and `docs/reviews/current-validation.md`.
- beta.123 runtime preflight passed 28 / 0, source-workspace validation passed
  57 / 0 with the retained GDRE warnings only and local RitsuLib XML/API marker
  coverage, and clicked Ancient UI smoke is retained at
  `.tools/runtime-evidence/monkey-stability-20260622-235746/`.
  The smoke covered Urda, Morvi, Lotha, and normal Vakuu with 4 / 4 iterations,
  all 127 migrated Spire Plus patches applied, and packet verification 1621 / 0.
- beta.99 clicked settings UI proof is previous-package context retained at
  `.tools/runtime-evidence/mod-settings-beta99-ritsulib-click-20260621-223210/`.
  It shows Settings -> `Mod Settings (RitsuLib)`, the RitsuLib Mods tree with
  only `RitsuLib` and `Spire Plus`, the Spire Plus settings page, a clean
  same-session log audit, and StS1 Off runtime shape verification 21 / 0.
- beta.99 direct Off loader proof is previous-package context retained at
  `.tools/runtime-evidence/v01071-beta99-ritsulib0432-off-direct-20260621-234221/`.
  It reached main menu with exactly STS2-RitsuLib and Spire Plus loaded, clean
  audit, 25/25 Spire Plus patches, StS1Events disabled with 0 registration
  lines, Off verifier 21 / 0, and packet verifier 43 / 0.
- Previous beta.96 Off loader proof is retained at
  `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/`.
  It proves startup/loading and default-Off StS1Events behavior only.
- Older beta.93 AdditiveBatch1 loader packets are retained only as older
  package loader/registration context. They do not prove beta.123 enabled-mode
  gameplay or tester readiness.

Treat loader, settings, and clicked-UI smoke evidence as scoped proof only:
enabled-mode proof, gameplay, save-load, replacement, multiplayer, independent
QA, package handoff, and release-ready compatibility proof remain pending.

## Compatibility Policy

- Do not claim compatibility with a new Slay the Spire 2 build until the local
  game source snapshot, installed STS2-RitsuLib runtime variant, build,
  publish, package parity, and at least one controlled loader/settings proof
  have been refreshed for that exact target.
- Do not add any runtime dependency besides STS2-RitsuLib without explicit
  owner approval and a new migration record.
- Keep runtime STS2-RitsuLib, NuGet `STS2.RitsuLib`, manifest dependency
  minimum, package metadata, and tester instructions aligned.
- Preserve `EZMicroBalance` only as the technical manifest id, install folder,
  saved-field namespace, and compatibility surface. Player-facing docs and UI
  should say `Spire Plus`.

## Update Procedure

1. Record the exact Slay the Spire 2 version and date.
2. Run `dotnet list EZMicroBalance.csproj package --include-transitive`.
3. Verify runtime STS2-RitsuLib files under `mods/STS2-RitsuLib`, including
   the root runtime DLL, manifest, XML docs, and any official runtime assets.
4. Run `dotnet build`.
5. Run `dotnet publish`.
6. Refresh the package and installed package parity.
7. Launch the game with only STS2-RitsuLib and Spire Plus enabled for the
   controlled compatibility lane.
8. Confirm STS2-RitsuLib appears and is enabled.
9. Confirm Spire Plus appears with manifest id `EZMicroBalance` and is enabled.
10. Open Settings -> `Mod Settings (RitsuLib)` and confirm the Spire Plus page.
11. Test the current feature surface.
12. Update this file, `PROJECT_STATE.md`, `docs/dev-environment.md`, and
    release evidence docs with the exact evidence boundary.

## Known Compatibility Risks

| Risk | Impact | Mitigation |
| --- | ---: | --- |
| Slay the Spire 2 API changes | High | Refresh local source, rerun source-workspace checks, and revalidate loader/settings proof. |
| STS2-RitsuLib runtime/package mismatch | High | Align runtime release, NuGet package, manifest minimum, and selected compat branch. |
| Unverified gameplay path | High | Keep compatibility claims scoped until gameplay, save-load, and co-op rows have direct evidence. |
| Template/tooling behavior changes | Medium | Re-check template examples and generated output before major packaging work. |
| Localization packaging changes | Medium | Run publish and inspect PCK/output after localization changes. |
