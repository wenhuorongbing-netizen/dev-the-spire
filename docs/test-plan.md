# Test Plan

## Automated checks
- `dotnet build`
- `dotnet publish`
- manifest exists
- dll exists
- pck exists if required by manifest/template

## Manual checks
- launch game
- open Settings
- open Mod Settings
- confirm BaseLib is listed
- confirm BaseLib is enabled
- confirm EzDailyContent is listed
- confirm EzDailyContent is enabled
- check logs for load errors

## Failure triage
- Manifest problem: missing/invalid json fields, id mismatch, or wrong dependency declaration.
- DLL problem: build output missing due SDK/template/project errors.
- PCK problem: publish pipeline missing tool path or packaging failure.
- BaseLib problem: missing files in game mods directory or incompatible version.
- Game path problem: unresolved Steam install path, invalid mod output target.
- Godot/MegaDot problem: missing engine/tool executable needed by publish flow.
- Template problem: missing template package or wrong shortname/options.

## Current status
- Current branch target: public beta
- Observed in-game version: `v0.104.0`, date `2026.04.23`
- Template check: PASS (`Alchyr.Sts2.Templates` `2.3.9`, content short name `alchyrsts2contentmod`)
- Manifest check: PASS (`EzDailyContent.json`, id `EzDailyContent`)
- Build check: PASS (`dotnet build`, 0 warnings, 0 errors)
- Publish check: PASS (`dotnet publish`, artifacts exist; previous missing-solution warnings resolved)
- DLL check: PASS (`D:\Steam\steamapps\common\Slay the Spire 2\mods\EzDailyContent\EzDailyContent.dll`)
- JSON artifact check: PASS (`D:\Steam\steamapps\common\Slay the Spire 2\mods\EzDailyContent\EzDailyContent.json`)
- PCK check: PASS (`D:\Steam\steamapps\common\Slay the Spire 2\mods\EzDailyContent\EzDailyContent.pck`)
- BaseLib runtime path check: PASS (`D:\Steam\steamapps\common\Slay the Spire 2\mods\BaseLib`)
- BaseLib file check: PASS (`BaseLib.json`, `BaseLib.dll`, `BaseLib.pck`)
- BaseLib version check: PASS (runtime `v3.1.0`, project package `Alchyr.Sts2.BaseLib` `3.1.0`)
- Manual game verification succeeded: yes
- Game launch check: PASS (manual in-game verification succeeded)
- Mod Settings check: PASS (BaseLib appeared and was enabled; EzDailyContent appeared and was enabled)
- Last successful `dotnet build`: 2026-05-02 during final setup review
- Last successful `dotnet publish`: 2026-05-02 during final setup review

## Future feature validation
- Before implementation, complete the API research in `docs/API_RESEARCH_PLAN.md`.
- For MVP-1, verify `Focus Tap` can be registered, localized, played, and removed safely.
- After card code changes, run `dotnet build`.
- After localization/resource changes, run `dotnet publish`.
- After publish, verify Mod Settings and inspect `godot.log` for `EzDailyContent`, `BaseLib`, `error`, and `exception`.
