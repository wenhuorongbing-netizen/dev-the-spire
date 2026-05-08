# EZ Micro Balance Architecture Decision

Decision date: 2026-05-05

## Decision

`EZ Micro Balance` will become an independent mod project with stable manifest id `EZMicroBalance`.

The existing `EzDailyContent` manifest id remains unchanged. It must not be renamed in-place because mod ids are persistent player-facing identifiers after project creation.

## Reason

The repository is moving to multiple independent mods over time. Players should be able to enable or disable `EZ Micro Balance`, future Ascension changes, and future custom-character work separately. Keeping the Ancient rebalance inside `EzDailyContent` would make future features share one mod toggle and one release identity.

## Intended Release Shape

Private beta should ship as:

```text
<GameRoot>/mods/EZMicroBalance/
  EZMicroBalance.dll
  EZMicroBalance.json
  EZMicroBalance.pck
```

Manifest:

```json
{
  "id": "EZMicroBalance",
  "name": "EZ Micro Balance",
  "dependencies": [
    {
      "id": "BaseLib",
      "min_version": "v3.1.2"
    }
  ],
  "affects_gameplay": true
}
```

## Legacy Project Policy

`EzDailyContent` is treated as the original scaffold/legacy project. Its manifest id remains `EzDailyContent`. Release documentation must make clear whether it is:

- excluded from the active release build, or
- retained only as an inert scaffold, or
- still built for compatibility checks.

It must not carry a second copy of the Ancient rebalance in a production release path, because enabling both `EzDailyContent` and `EZMicroBalance` would duplicate Harmony patches.

Current implementation decision:

- `EZMicroBalance.sln` is the active solution for bare `dotnet build`.
- `EZMicroBalance.csproj` is the active project.
- The old `EzDailyContent.csproj` file has been moved to `legacy/EzDailyContent/EzDailyContent.csproj.legacy.xml` so root `dotnet build` is unambiguous.
- Legacy `EzDailyContent` resources/code remain in the repository for traceability but are not part of the active solution.
- `export_presets.cfg` uses a selected-resource export so `EZMicroBalance.pck` contains active resources only and does not package legacy files, source files, docs, or art research folders.

## Later Mods And Gated Expansion Work

The original Ancient-only private beta scope excluded Ascension 11-20-30 and custom-character work. The 2026-05-06 overnight sprint goal opened Ascension 11-20 as an active implementation track. The current private-beta multiplayer test candidate exposes A11-A20 single-player and host-multiplayer selection through the original Ascension UI by default, keeps all selector expansion disableable with `EZMB_ASCENSION_DISABLE_PUBLIC_SELECTION=1`, keeps host-multiplayer selection separately disableable with `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`, accepts legacy `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` without requiring it, and does not claim release readiness until live behavior is verified.

Ascension 21-30 and custom-character work remain out of scope. If later work requires a separate player-facing toggle, choose and document a new stable manifest id before the first build rather than renaming an existing id in-place.
