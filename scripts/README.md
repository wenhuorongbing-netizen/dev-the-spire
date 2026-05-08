# Scripts

Repository helper scripts live here. Keep scripts small, idempotent where possible, and documented in this file when added.

| Script | Purpose |
| --- | --- |
| `audit-godot-log.ps1` | Scan `godot.log` for known loader/API drift/runtime failure patterns and emit a JSON-style audit summary. |
| `bootstrap-windows.ps1` | Bootstrap local Windows setup for this workspace. Use with care because local paths and installed tools vary by machine. |

Do not put downloaded binaries or generated tool output in this folder. Use ignored local folders such as `.tools/`, `publish/`, or `source code/` for machine-specific material.
