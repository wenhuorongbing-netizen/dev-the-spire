# Beta Compatibility

## Tested Baseline

- Game: Slay the Spire 2
- Branch target: public beta
- Verified version: `v0.106.0`
- Version date: `2026-05-21T16:17:40-07:00` upstream build, installed/tested locally on `2026-05-22`
- BaseLib runtime: `v3.1.4`
- BaseLib runtime path: `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib`
- Project BaseLib package: `Alchyr.Sts2.BaseLib` `3.1.4`
- Template package: `Alchyr.Sts2.Templates` `2.3.9`
- Build: `dotnet build` succeeds.
- Publish: `dotnet publish` succeeds.
- Legacy manual verification: BaseLib and `EzDailyContent` appeared and were enabled in Mod Settings on `v0.104.0`.
- Active static/package verification: `source code/` was refreshed from the v0.106.0 installed PCK/DLL, project build/test/publish/package pass with BaseLib v3.1.4, and package hashes are synced in current release docs.
- Historical normal Steam-client startup/log verification: the latest live loader smoke under `.tools\runtime-evidence\live-spire-plus-session-20260515-211414` reached main menu with exactly BaseLib and Spire Plus loaded, `Registered config for mod EZMicroBalance`, `Found 22 SavedSpireFields`, `Time to main menu: 13,539ms`, and 0 `ERROR` / release-blocking signature hits. current source now defines 26 SavedSpireFields after the 2026-05-17 static fixes, so this live loader smoke is historical until rerun for v0.106.0. Current Mod Settings UI list screenshot shows `Spire Plus`; gameplay matrix remains pending.

## Compatibility Policy

- Compatibility is only confirmed for the tested public beta version above.
- Do not claim compatibility with future public beta versions until retested.
- Re-run build, publish, and manual game verification after game, BaseLib, template, or SDK changes.
- Keep runtime BaseLib and NuGet BaseLib package versions aligned.
- Record any future failures with exact game version, BaseLib version, and log excerpts.

## Update Procedure

1. Record new game version and date.
2. Check `dotnet list EZMicroBalance.csproj package --include-transitive`.
3. Verify runtime BaseLib files under `mods/BaseLib`.
4. Run `dotnet build`.
5. Run `dotnet publish`.
6. Launch game.
7. Confirm BaseLib appears and is enabled.
8. Confirm Spire Plus appears with manifest id `EZMicroBalance` and is enabled.
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
| 2026-05-05 | `v0.104.0`, `2026.04.23` | `v3.1.0` | PARTIAL | `EZMicroBalance` build, publish, tests, and isolated `--force-steam off` smoke passed; normal Steam-client Mod Settings and manual gameplay matrix remain pending. |
| 2026-05-08 | `v0.105.0`, `2026.05.07` | `v3.1.2` | PARTIAL | Source refreshed from current PCK, build/publish/tests pass, isolated BaseLib+EZMB `--force-steam off` smoke reached main menu, and normal Steam-client Mod Settings passed after the no-op EZMB config page; manual gameplay matrix remains pending. |
| 2026-05-13 | `v0.105.0`, `2026.05.07` | `v3.1.2` | PARTIAL | Current-at-that-time package controlled smoke, normal Steam-client isolated startup/log pass, and refreshed Mod Settings UI list screenshot with Spire Plus / `EZMicroBalance`, 16 SavedSpireFields, and 0 release-blocking log hits; manual gameplay matrix remains pending. |
| 2026-05-17 | `v0.105.1` observed in latest live log | `v3.1.2` | PARTIAL | Source/build/package refreshed after static review fixes; no fresh live loader smoke or gameplay matrix was run for this exact package. |
| 2026-05-22 | `v0.106.0`, `2026-05-21T16:17:40-07:00` | `v3.1.4` | PARTIAL | `source code/` cleaned and regenerated from current PCK/DLL; Core API diffs recorded; project build, default tests, publish, and package refresh pass. Fresh live loader smoke and gameplay matrix remain pending. |
