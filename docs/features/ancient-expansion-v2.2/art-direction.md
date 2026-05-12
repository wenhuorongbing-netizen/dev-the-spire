# Ancient Expansion v2.2 Art Direction

Status: art direction approved from user-provided chat references; final asset files are not yet verified in the repository.

## Approved Event Art Direction

| Ancient | Target resource path | Visual direction | Current status |
| --- | --- | --- | --- |
| Morvi, the Lender-Scribe | `EZMicroBalance/images/events/ezmb_morvi.png` | Blue-lit lender-scribe court, sealed contract, skeletal hands, ledger/typewriter, one central blue eye. Fits debt, archive, and borrowed-power rules. | Approved direction; source image must be copied from an explicit local file before export. |
| Lotha, the Judge | `EZMicroBalance/images/events/ezmb_lotha.png` | Dark mirror tribunal, crystal panes showing monsters/evidence, central judge figure with mirror/heart motif. Fits verdict, evidence, rebuttal, and judgment rules. | Approved direction; source image must be copied from an explicit local file before export. |

## Asset Hygiene Rules

- Use original user-provided/generated art only.
- Do not copy official Slay the Spire 2 assets.
- Do not use images with visible logos, UI text, watermarks, or unofficial release numbers.
- Do not generate or copy placeholder event art just to satisfy the export list.
- Do not promote unverified temporary files from `AppData/Local/Temp` or `.codex/generated_images` unless the image is visually confirmed against the approved reference.
- When final image bytes are available, copy them to the target resource paths, let Godot generate `.import` files, add the PNGs to `export_presets.cfg`, and add package/resource guard coverage.
- Current source pass found no explicit local source file for `ezmb_morvi.png` or `ezmb_lotha.png`; both files remain pending and are intentionally absent from `export_presets.cfg`.

## Next Integration Checklist

- [ ] Place Morvi art at `EZMicroBalance/images/events/ezmb_morvi.png`.
- [ ] Place Lotha art at `EZMicroBalance/images/events/ezmb_lotha.png`.
- [ ] Generate/refresh `.import` files through `dotnet publish` or Godot import.
- [ ] Add both PNG paths to `export_presets.cfg`.
- [ ] Add source/package guard tests for event-art resource coverage.
- [ ] Bind Morvi event portrait to `ezmb_morvi.png`.
- [ ] Keep Lotha portrait path documented or gated until Lotha source is implemented.
- [ ] Verify no release doc claims Lotha is playable before the gate and source slice exist.
- [ ] Run `dotnet build`, `dotnet test --no-build`, `dotnet publish`, and `git diff --check`.
