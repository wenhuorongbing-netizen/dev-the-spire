# StS1 Event Assets Pipeline

## Strategy

Original StS1 art is **not committed** to the repository. Instead, a PowerShell
script extracts event portraits from a local StS1 installation into
`EZMicroBalance/images/events/sts1/` (gitignored).

## Extraction Script

`scripts/extract-sts1-event-assets.ps1`

**Parameters:**
- `-Sts1Path` — path to local Slay the Spire 1 installation
- `-OutputPath` — output directory (default: `EZMicroBalance/images/events/`)

**Behavior:**
1. Reads `manifests/asset_manifest.csv` for source→destination mapping
2. Copies event portrait PNGs from StS1 install to mod images directory
3. Renames files to match StS2 event entry names (lowercase, prefixed)
4. Reports any missing source files

## Validation Script

`scripts/validate-sts1-event-assets.ps1`

**Behavior:**
1. Checks that all required event portraits exist in the output directory
2. Verifies image dimensions match expected sizes
3. Reports missing or invalid assets

## Asset Manifest

`manifests/asset_manifest.csv`

Format:
```csv
sts1_event_name,sts2_entry,source_path,dest_filename,required
bigFish,sts1_big_fish,resources/images/events/bigFish.png,sts1_big_fish.png,true
goldenIdol,sts1_golden_idol,resources/images/events/goldenIdol.png,sts1_golden_idol.png,true
```

## Image Requirements

| Type | Size | Format |
|------|------|--------|
| Event portrait | 1024x600 | PNG |
| Phobia mode portrait | 1024x600 | PNG (optional) |

## Gitignore Rule

Add to `.gitignore`:
```
EZMicroBalance/images/events/sts1_*.png
```

This prevents extracted StS1 assets from being committed.

## Placeholder Strategy

Until extraction runs, events use the default StS2 event layout with no
custom portrait. The game falls back to a generic event background.
