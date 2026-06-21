# 2026-05-15 Current Package Verification Note

Archived from the top of `docs/issues.md` during the 2026-05-16 cleanup pass so the active issue index stays compact.

The 2026-05-15 Vakuu/text/reward polish package added a dedicated source Vakuu monster and custom encounter scene, changed the fight encounter to `RoomType.Monster` with `ShouldGiveRewards=false` so previous framework no longer warned about unexpected `RoomType.Event`, kept the fight hidden behind explicit gates, fixed Root-Sight hover text, cleared any existing map-point hover before showing the Root-Sight explanation, renamed the Meat Cleaver rest action to `Cleaver`, shortened A12 Firemarked Elite wording, repaired active Simplified Chinese localization encoding, extended Vakuu victory rewards to custom Lotha visible relics, and removed stale current-doc placeholder/default-scene language.

Validation recorded for that package:

- `dotnet build`
- normal `dotnet test`
- `dotnet format` verification
- `git diff --check`
- `dotnet publish` and package refresh
- opt-in release artifact tests
- installed-package hash check
- art audit
- current-package loader smoke

Current package hashes from that note:

| Artifact | SHA-256 |
| --- | --- |
| zip | `6CE68AC88EB548C383BC652B556996DD4427D07A3ACC1BC6EE8A566F9A083CB8` |
| DLL | `2D223DE0F1424C15AA7CFFAFF97D0C0ED91FBF15BBAF39252C8C681AFF2FDF0C` |
| PCK | `ABBA11A88842879152393A83FCB9729B37789FEC496EE60AADE60A8100152996` |
| manifest | `659943569D01C1DDD8B5C351D763497F7FEE513AD0BB84903D05B69F8DBD1AB2` |
| README | `C9F19363848AEECD4B763BFF7BB2B75980A90BFE22358ACEC8FF5E9E5C129CE4` |

Latest loader/log evidence at the time was `.tools/runtime-evidence/live-spire-plus-session-20260515-211414`; it covered this exact package, loaded only previous framework plus Spire Plus, and had no release-blocking log hits or unexpected room-type warning.

This was a manual-test handoff note, not a release-ready closeout. Current-build live gameplay, clicked Ancient UI/manual feature results, save-load, natural A11 route-click traversal, death/failure path, co-op, and Rootblight manual proof remained pending.
