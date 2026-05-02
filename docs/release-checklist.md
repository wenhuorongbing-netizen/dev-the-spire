# Release Checklist

## Pre-release
- [x] build succeeds
- [x] publish succeeds
- [ ] manifest version updated
- [x] manifest id unchanged from initial creation
- [x] matching BaseLib runtime release installed under `mods\BaseLib`
- [x] game version documented (`v0.104.0`, `2026.04.23`, public beta)
- [ ] no original game assets included
- [ ] no copied decompiled game code
- [x] manual in-game verification completed

## Current generated artifacts

```text
D:\Steam\steamapps\common\Slay the Spire 2\mods\EzDailyContent\
  EzDailyContent.json
  EzDailyContent.dll
  EzDailyContent.pck
```

## Notes
- Steam Workshop/mod distribution flow may differ by Slay the Spire 2 version.
- Re-check community guidance for template/BaseLib compatibility before release.
- `EzDailyContent.sln` exists and contains the generated project.
- Manual Mod Settings verification succeeded: BaseLib and EzDailyContent appeared and were enabled.
