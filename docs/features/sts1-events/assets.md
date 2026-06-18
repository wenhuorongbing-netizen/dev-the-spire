# StS1 Event Assets Pipeline

## Current Decision

Original StS1 art is not committed to the repository, and local extraction is local QA evidence only. Extracted original StS1 images must not be included in a tester package, release package, handoff bundle, tracked file, or public artifact unless redistribution permission is confirmed and documented.

Until an owner chooses a redistributable art path, StS1 event image/render gates stay open. Acceptable future paths are:

1. Confirmed redistribution permission for original StS1 event art.
2. Spire Plus-owned generated or commissioned replacement art.
3. Local-only extraction for private visual QA, with extracted files kept out of tracked files and out of handoff packages.
4. Explicit owner acceptance of non-parity/no-custom-portrait placeholders.

## Local Extraction Script

`scripts/extract-sts1-event-assets.ps1`

Parameters:

- `-Sts1Path`: path to a local Slay the Spire 1 installation.
- `-OutputPath`: local output directory. The default is `EZMicroBalance/images/events/`.

Behavior:

1. Reads `manifests/asset_manifest.csv` for source/destination mapping.
2. Copies event portrait PNGs from the local StS1 install to the local output directory.
3. Renames files to match StS2 event entry names with `sts1_*.png` filenames.
4. Reports required source files that are missing locally.

The default output directory is gitignored. That prevents accidental commits; it does not grant redistribution permission and does not make extracted art safe for package handoff.

## Validation Script

`scripts/validate-sts1-event-assets.ps1`

Behavior:

1. Checks that all required event portraits exist in the local output directory.
2. Verifies that each found file is non-empty.
3. Reports missing or invalid local files.

Passing this script proves only that local extracted files exist for the current machine. It does not close image/license/render gates unless the owner also records the selected redistributable art path or explicitly accepts local-only/non-parity placeholders.

## Asset Manifest

`manifests/asset_manifest.csv`

Format:

```csv
sts1_event_name,sts2_entry,source_filename,dest_filename,required
bigFish,sts1_big_fish,bigFish.png,sts1_big_fish.png,true
goldenIdol,sts1_golden_idol,goldenIdol.png,sts1_golden_idol.png,true
```

## Image Requirements

| Type | Size | Format |
|------|------|--------|
| Event portrait | 1024x600 | PNG |
| Phobia mode portrait | 1024x600 | PNG, optional |

## Gitignore Rule

The repository must keep these rules:

```text
/EZMicroBalance/images/events/sts1_*.png
/EZMicroBalance/images/events/sts1/
```

## Placeholder Strategy

Until an owner chooses and validates a redistributable art path, events use the default StS2 event layout with no custom StS1 portrait. Runtime screenshots can document this as a non-parity placeholder, but they cannot be described as StS1 art parity.
