# Beta Compatibility

## 2026-06-21 Current Compatibility Boundary

Current compatibility work targets:

- Slay the Spire 2 `v0.107.1`
- Spire Plus `v0.1.0-private-beta.97`
- STS2-RitsuLib `v0.4.31` with selected runtime variant `lib\0.107.1`
- Stable technical manifest id `EZMicroBalance`

Spire Plus is RitsuLib-only for the current package line. The project
references `STS2.RitsuLib` `0.4.31`, and `EZMicroBalance.json` declares
only `STS2-RitsuLib >= 0.4.31` as the shared runtime dependency.

Current evidence:

- beta.97 build, publish, package refresh, and installed package parity are
  recorded in `PROJECT_STATE.md` and `docs/reviews/current-validation.md`.
- beta.97 clicked settings UI and Off loader proof remain pending after the
  settings-page I18N resource migration.
- Previous beta.96 clicked settings proof is retained at
  `.tools/runtime-evidence/beta96-ritsulib-mod-settings-clicked-ui-20260621-160701/`.
  It shows Settings -> `Mod Settings (RitsuLib)`, the RitsuLib Mods tree with
  only `RitsuLib` and `Spire Plus`, and the Spire Plus settings page.
- Previous beta.96 Off loader proof is retained at
  `.tools/runtime-evidence/v01071-beta96-ritsulib0431-off-direct-20260621-185056/`.
  It proves startup/loading and default-Off StS1Events behavior only.
- Older beta.93 AdditiveBatch1 loader packets are retained only as older
  package loader/registration context. They do not prove beta.97 enabled-mode
  gameplay or tester readiness.

Treat loader and settings evidence as scoped proof only: gameplay, save-load, replacement, multiplayer, independent QA, package handoff, and release-ready compatibility proof remain pending.

## Compatibility Policy

- Do not claim compatibility with a new Slay the Spire 2 build until the local
  game source snapshot, installed STS2-RitsuLib runtime variant, build,
  publish, package parity, and at least one controlled loader/settings proof
  have been refreshed for that exact target.
- Do not add another shared runtime framework dependency without explicit owner
  approval and a new migration record.
- Keep runtime STS2-RitsuLib, NuGet `STS2.RitsuLib`, manifest dependency
  minimum, package metadata, and tester instructions aligned.
- Preserve `EZMicroBalance` only as the technical manifest id, install folder,
  saved-field namespace, and compatibility surface. Player-facing docs and UI
  should say `Spire Plus`.

## Update Procedure

1. Record the exact Slay the Spire 2 version and date.
2. Run `dotnet list EZMicroBalance.csproj package --include-transitive`.
3. Verify runtime STS2-RitsuLib files under `mods/STS2-RitsuLib`, including
   the selected `lib\<game-version>` variant.
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
