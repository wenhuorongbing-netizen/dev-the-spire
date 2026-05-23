# Platform Testing

This page is for testers moving the same `Spire Plus / EZMicroBalance` manual-test package between Windows and macOS. It does not replace live gameplay evidence; it only verifies that the installed files, hashes, logs, and environment-variable setup are comparable across machines.

## Package Hash Source

Use `docs/private-beta-verification-handoff.md` as the current hash source. It lists the expected SHA256 for:

- `EZMicroBalance.dll`
- `EZMicroBalance.json`
- `EZMicroBalance.pck`
- `README_INSTALL.txt`
- `publish/SpirePlus-v0.1.0-private-beta.0.zip`

The test package should contain only:

```text
EZMicroBalance/EZMicroBalance.dll
EZMicroBalance/EZMicroBalance.json
EZMicroBalance/EZMicroBalance.pck
EZMicroBalance/README_INSTALL.txt
```

It must not include duplicate runtime dependency DLLs such as `BaseLib.dll`, `0Harmony.dll`, or `sts2.dll`. Those belong to the installed game or the separate `mods/BaseLib` dependency.

## Windows

Typical game path:

```powershell
$env:STS2_PATH='D:\Steam\steamapps\common\Slay the Spire 2'
```

Verify the installed mod folder against the handoff hashes:

```powershell
.\scripts\check-installed-ezmb-package.ps1 -ModDirectory "$env:STS2_PATH\mods\EZMicroBalance"
```

Check a package hash directly:

```powershell
Get-FileHash -LiteralPath .\publish\SpirePlus-v0.1.0-private-beta.0.zip -Algorithm SHA256
```

Unpack a package for inspection:

```powershell
Expand-Archive -LiteralPath .\publish\SpirePlus-v0.1.0-private-beta.0.zip -DestinationPath .\publish\inspect -Force
```

Current Windows log path used by the project helpers:

```powershell
$env:APPDATA\SlayTheSpire2\logs\godot.log
```

Example feature gates:

```powershell
$env:EZMB_DISABLE_MORVI='1'
$env:EZMB_DISABLE_LOTHA='1'
$env:EZMB_FORCE_ANCIENT='URDA'
$env:EZMB_RELEASE_EVIDENCE_LOG='1'
```

## macOS

Typical Steam game path:

```sh
export STS2_PATH="$HOME/Library/Application Support/Steam/steamapps/common/Slay the Spire 2"
```

Verify the installed mod folder against the handoff hashes:

```sh
scripts/check-installed-ezmb-package.sh "$STS2_PATH/mods/EZMicroBalance"
```

Check a package hash directly:

```sh
shasum -a 256 publish/SpirePlus-v0.1.0-private-beta.0.zip
```

Unpack a package for inspection:

```sh
unzip -q publish/SpirePlus-v0.1.0-private-beta.0.zip -d publish/inspect
```

Expected macOS log path to check first:

```sh
"$HOME/Library/Application Support/SlayTheSpire2/logs/godot.log"
```

If the game writes logs to a different per-user support path on the target machine, copy that actual `godot.log` into the evidence folder and record the path in `command.txt`.

Example feature gates:

```sh
EZMB_DISABLE_MORVI=1
EZMB_DISABLE_LOTHA=1
EZMB_FORCE_ANCIENT=URDA
EZMB_RELEASE_EVIDENCE_LOG=1
```

## Co-op Hash Rule

For Windows/macOS co-op tests, record the host and client values from the same run attempt:

- package zip SHA256
- installed `EZMicroBalance.dll`, `.json`, `.pck`, and `README_INSTALL.txt` SHA256
- BaseLib version and installed `mods/BaseLib` folder presence
- `godot.log` from host and client
- `godot-log-audit.json` for both logs
- loaded mod list and any ModelDb mismatch diagnostics

The current source logs additional multiplayer mismatch details, but it does not bypass the game's version or ModelDb checks. A same-version visible game build can still fail to join if the gameplay model hash differs.

## Evidence Boundary

Passing the hash checks only proves that the same files are installed. It does not prove:

- live loader parity for the current 26 saved fields
- clicked Ancient UI
- reward gameplay
- save/load
- Vakuu victory or no-black-screen behavior
- A11-A20 traversal and combat behavior
- co-op ownership or desync safety

Keep those rows open until their own screenshots, logs, and result notes exist.
