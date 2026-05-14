# Windows/Mac Platform Risk Matrix

Reviewed source baseline: `a2183ee`.

| Area | Windows command/path | macOS equivalent | Risk | Follow-up |
| --- | --- | --- | --- | --- |
| Steam game path | `D:\Steam\steamapps\common\Slay the Spire 2` or Steam library path | `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2` | Medium; user installs vary | Keep docs path-parametric; avoid hardcoded release claims. |
| Mod install path | `D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance` | From `Sts2PathDiscovery.props`: `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods/EZMicroBalance` | Medium; macOS app-bundle path must be live-confirmed | Add macOS package check notes before Mac test. |
| Game data/source path | Windows build props use `Sts2Path` and `GodotPath` | `Sts2PathDiscovery.props` includes macOS app-bundle data path | Medium | Confirm `Directory.Build.props` local values on Mac. |
| Godot log path | `%APPDATA%\SlayTheSpire2\logs\godot.log` | Likely `~/Library/Application Support/SlayTheSpire2/logs/godot.log`; needs live confirmation | Medium | Document as "expected" until Mac tester confirms. |
| Launch game | `D:\Steam\steam.exe -applaunch 2868840` | `open -a Steam --args -applaunch 2868840` or `open steam://run/2868840` | Medium | Confirm actual Mac Steam launch command. |
| Hash files | `Get-FileHash -Algorithm SHA256 <path>` | `shasum -a 256 <path>` | Low | Include both in tester handoff. |
| Extract zip | `Expand-Archive .\EZMicroBalance.zip -DestinationPath .\tmp` | `unzip EZMicroBalance.zip -d tmp` | Low | Include both in tester handoff. |
| Env vars | PowerShell: `$env:EZMB_DISABLE_URDA='1'; dotnet test` | zsh/bash: `EZMB_DISABLE_URDA=1 dotnet test` or `export EZMB_DISABLE_URDA=1` | Low/Medium | Every env-gated test doc should show shell-specific syntax. |
| Build | `dotnet build EZMicroBalance.sln` | `dotnet build EZMicroBalance.sln` | Low if props configured | Mac needs valid `GodotPath`/`Sts2Path`. |
| Publish | `dotnet publish EZMicroBalance.sln` | `dotnet publish EZMicroBalance.sln` | Medium | Godot export tooling path and app bundle paths must be proven. |
| Log audit script | `.\scripts\audit-godot-log.ps1 -Path "$env:APPDATA\SlayTheSpire2\logs\godot.log"` | PowerShell Core may run: `pwsh ./scripts/audit-godot-log.ps1 -Path "$HOME/Library/Application Support/SlayTheSpire2/logs/godot.log"` | Medium | Script is PowerShell; Mac path unverified. |
| Installed package check | `.\scripts\check-installed-ezmb-package.ps1` | `pwsh ./scripts/check-installed-ezmb-package.ps1 -InstallRoot "<mac-mod-path>"` may work if paths passed explicitly | High | Defaults include hardcoded `D:\Steam`; add cross-platform helper later. |
| Backslashes in docs | Common in Windows examples | Use quoted POSIX paths with spaces on macOS | Low/Medium | Tester docs need both variants. |
| PowerShell availability | Built into Windows | Requires PowerShell Core install (`pwsh`) | Medium | Do not require PowerShell for Mac-only manual validation; provide shell alternatives. |
| Case sensitivity | Usually case-insensitive | Usually case-insensitive APFS, but can be case-sensitive | Low | Keep manifest/resource paths exact-case. |
| Mod-list/ModelDb hash | Same concept | Same concept | Medium | Capture host/client logs on both platforms if co-op mixed-platform testing happens. |

## Windows Test Commands

```powershell
git status --short --branch
git log -1 --oneline --decorate
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
Get-FileHash -Algorithm SHA256 "D:\Steam\steamapps\common\Slay the Spire 2\mods\EZMicroBalance\EZMicroBalance.dll"
.\scripts\audit-godot-log.ps1 -Path "$env:APPDATA\SlayTheSpire2\logs\godot.log"
```

## macOS Equivalent Commands

```bash
git status --short --branch
git log -1 --oneline --decorate
dotnet build EZMicroBalance.sln
dotnet test EZMicroBalance.sln --no-build
dotnet format EZMicroBalance.sln --verify-no-changes --no-restore
git diff --check
shasum -a 256 "$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/MacOS/mods/EZMicroBalance/EZMicroBalance.dll"
pwsh ./scripts/audit-godot-log.ps1 -Path "$HOME/Library/Application Support/SlayTheSpire2/logs/godot.log"
```

## Platform Conclusion

Windows is the only locally proven platform in current evidence. macOS can likely build and validate with correct paths, but package install path, log path, Steam launch, and PowerShell availability need a dedicated Mac tester note before private beta release claims.

