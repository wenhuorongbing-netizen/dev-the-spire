# Beta Compatibility

## 2026-06-20 Current Compatibility Boundary

The `v0.106.1` baseline below is historical tested-baseline context. Current local installed game is Slay the Spire 2 `v0.107.1` with Spire Plus depending on RitsuLib `v0.4.28` / `lib\0.107.1` only; BaseLib remains installed locally only as previous-package/other-mod context. Beta.85 default-Off proof at `.tools/runtime-evidence/v01070-beta85-current-package-runtime-fix-20260611-0510/`, beta.85 CanaryOnly proof at `.tools/runtime-evidence/v01070-beta85-canary-20260617-233621/`, beta.87 AdditiveBatch1 proof at `.tools/runtime-evidence/v01070-beta87-additive-batch1-direct-20260618-152531/`, beta.88 BaseLib-backed proof, and beta.90 RitsuLib-only proof are previous-package contexts. Current beta.91 Off proof exists at `.tools/runtime-evidence/v01071-beta91-ritsulib0428-off-direct-20260620/`, and current beta.91 AdditiveBatch1 loader/registration proof exists at `.tools/runtime-evidence/v01071-beta91-ritsulib0428-additivebatch1-direct-20260620/` for `v0.107.1` with 25/25 Spire Plus patches, clean audits, 10 event types, and 14 registration calls. Treat these as loader proof only: gameplay, save-load, replacement, multiplayer, independent QA, clean-worktree, package handoff, and release-ready compatibility proof remain pending or blocked.

## Tested Baseline

- Game: Slay the Spire 2
- Branch target: public beta
- Verified version: `v0.106.1`
- Version date: `2026-05-21T16:17:40-07:00` upstream build, installed/tested locally on `2026-05-22`
- BaseLib runtime: `v3.1.4`
- BaseLib runtime path: `D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib`
- Project BaseLib package: `Alchyr.Sts2.BaseLib` `3.1.4`
- Template package: `Alchyr.Sts2.Templates` `2.3.9`
- Build: `dotnet build` succeeds.
- Publish: `dotnet publish` succeeds.
- Legacy manual verification: BaseLib and `EzDailyContent` appeared and were enabled in Mod Settings on `v0.104.0`.
- Historical static/package verification: `source code/` was refreshed from the v0.106.1 installed PCK/DLL, project build/test/publish/package passed with BaseLib v3.1.4, and package hashes were synced for that historical release-doc lane. Current `v0.107.1` / STS2-RitsuLib `v0.4.28` status is summarized in the override above.
- Historical normal Steam-client startup/log verification under `.tools\runtime-evidence\live-spire-plus-session-20260515-211414` reached main menu with exactly BaseLib and Spire Plus loaded, `Registered config for mod EZMicroBalance`, `Found 22 SavedSpireFields`, `Time to main menu: 13,539ms`, and 0 `ERROR` / release-blocking signature hits. That 22-field smoke is historical. The 2026-05-25 Steam-client loader evidence under `.tools\runtime-evidence\beta19-loader-smoke-20260525-213336` covers the beta.19 package hash with exactly BaseLib plus Spire Plus loaded and a clean log audit. Current Mod Settings UI list screenshot shows `Spire Plus`; gameplay matrix remains pending.

## Compatibility Policy

- Full compatibility is only confirmed for the tested public beta version above. Current `v0.107.1` compatibility evidence covers beta.91 Off plus AdditiveBatch1 loader/registration proof only; retained `v0.107.0` compatibility evidence is limited to the beta.85 default-Off/CanaryOnly loader context and beta.87 AdditiveBatch1 loader/registration proof noted here. Gameplay, save-load, replacement, multiplayer, QA, release, and handoff compatibility proof remain pending.
- Do not claim compatibility with future public beta versions until retested.
- Re-run build, publish, and manual game verification after game, STS2-RitsuLib, template, or SDK changes.
- Keep runtime STS2-RitsuLib and NuGet STS2-RitsuLib package versions aligned.
- Record any future failures with exact game version, STS2-RitsuLib version, selected compat branch, and log excerpts.

## Update Procedure

1. Record new game version and date.
2. Check `dotnet list EZMicroBalance.csproj package --include-transitive`.
3. Verify runtime STS2-RitsuLib files under `mods/STS2-RitsuLib`, including `lib\0.107.1`.
4. Run `dotnet build`.
5. Run `dotnet publish`.
6. Launch game.
7. Confirm STS2-RitsuLib appears and is enabled.
8. Confirm Spire Plus appears with manifest id `EZMicroBalance` and is enabled.
9. Test current feature surface.
10. Update this file and `docs/dev-environment.md`.

## Known Compatibility Risks

| Risk | Impact | Mitigation |
|---|---:|---|
| Public beta API changes | High | Keep feature surface small and retest after updates. |
| STS2-RitsuLib runtime/package mismatch | High | Align runtime release, NuGet package, and selected compat branch. |
| Template behavior changes | Medium | Re-check generated template examples before major work. |
| Localization packaging changes | Medium | Run publish and inspect PCK/output after localization changes. |
| Relic/power hooks change | Medium | Keep first relic/power simple and document hook assumptions. |

## Compatibility Log

| Date | Game version | BaseLib | Result | Notes |
|---|---|---|---|---|
| 2026-05-02 | `v0.104.0`, `2026.04.23` | `v3.1.0` | PASS | Build, publish, and Mod Settings verification succeeded. |
| 2026-05-05 | `v0.104.0`, `2026.04.23` | `v3.1.0` | PARTIAL | `EZMicroBalance` build, publish, tests, and isolated `--force-steam off` smoke passed; normal Steam-client Mod Settings and manual gameplay matrix remain pending. |
| 2026-05-08 | `v0.105.0`, `2026.05.07` | `v3.1.2` | PARTIAL | Source refreshed from the then-current PCK, build/publish/tests pass, isolated BaseLib + Spire Plus `--force-steam off` smoke reached main menu, and normal Steam-client Mod Settings passed after the no-op Spire Plus config page; manual gameplay matrix remains pending. |
| 2026-05-13 | `v0.105.0`, `2026.05.07` | `v3.1.2` | PARTIAL | Current-at-that-time package controlled smoke, normal Steam-client isolated startup/log pass, and refreshed Mod Settings UI list screenshot with Spire Plus under technical id `EZMicroBalance`, 16 SavedSpireFields, and 0 release-blocking log hits; manual gameplay matrix remains pending. |
| 2026-05-17 | `v0.105.1` observed in latest live log | `v3.1.2` | PARTIAL | Source/build/package refreshed after static review fixes; no fresh live loader smoke or gameplay matrix was run for this exact package. |
| 2026-05-22 | `v0.106.1`, `2026-05-21T16:17:40-07:00` | `v3.1.4` | PARTIAL | `source code/` cleaned and regenerated from current PCK/DLL; Core API diffs recorded; project build, default tests, publish, package refresh, and beta.19 30-field loader smoke passed. Gameplay matrix remains pending. |
