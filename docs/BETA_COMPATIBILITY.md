# Beta Compatibility

## Tested Baseline

- Game: Slay the Spire 2
- Branch target: public beta
- Verified version: `v0.104.0`
- Version date: `2026.04.23`
- BaseLib runtime: `v3.1.0`
- BaseLib runtime path: `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib`
- Project BaseLib package: `Alchyr.Sts2.BaseLib` `3.1.0`
- Template package: `Alchyr.Sts2.Templates` `2.3.9`
- Build: `dotnet build` succeeds.
- Publish: `dotnet publish` succeeds.
- Manual verification: BaseLib and EzDailyContent appear and are enabled in Mod Settings.

## Compatibility Policy

- Compatibility is only confirmed for the tested public beta version above.
- Do not claim compatibility with future public beta versions until retested.
- Re-run build, publish, and manual game verification after game, BaseLib, template, or SDK changes.
- Keep runtime BaseLib and NuGet BaseLib package versions aligned.
- Record any future failures with exact game version, BaseLib version, and log excerpts.

## Update Procedure

1. Record new game version and date.
2. Check `dotnet list EzDailyContent.csproj package --include-transitive`.
3. Verify runtime BaseLib files under `mods/BaseLib`.
4. Run `dotnet build`.
5. Run `dotnet publish`.
6. Launch game.
7. Confirm BaseLib appears and is enabled.
8. Confirm EzDailyContent appears and is enabled.
9. Test current feature surface.
10. Update this file and `docs/dev-environment.md`.

## Known Compatibility Risks

| Risk | Impact | Mitigation |
|---|---:|---|
| Public beta API changes | High | Keep feature surface small and retest after updates. |
| BaseLib runtime/package mismatch | High | Align runtime release and NuGet package. |
| Template behavior changes | Medium | Re-check generated template examples before major work. |
| Localization packaging changes | Medium | Run publish and inspect PCK/output after localization changes. |
| Relic/power hooks change | Medium | Keep first relic/power simple and document hook assumptions. |

## Compatibility Log

| Date | Game version | BaseLib | Result | Notes |
|---|---|---|---|---|
| 2026-05-02 | `v0.104.0`, `2026.04.23` | `v3.1.0` | PASS | Build, publish, and Mod Settings verification succeeded. |
