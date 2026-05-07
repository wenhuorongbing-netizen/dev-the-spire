# EZ Micro Balance Issues

This file tracks player-reported and runtime-observed issues. Do not mark an item release-ready unless source validation and live verification both support it.

## Open

### ISSUE-2026-05-07-A11-MAP-LENGTH-NOT-PLAYER-VISIBLE

Priority: P1

Status: source-patched again; live verification pending

Area: A11 Wide Tower, Long Road / map generation

Player report: A11 still looks like the original map size. It was longer once, then regressed.

Current source fix:

- `AscensionFeatureGate` now sets A11 rows to Act 1 `+1`, Act 2 `+1`, Act 3 `+2`.
- `AscensionMapService` now accepts old width-only adjusted maps and inserts missing late rows instead of returning early.
- A11 still expands from 7 to 8 columns and inserts a reachable optional route node.
- A11 no longer marks ordinary route nodes with a dedicated long-road marker or hover explanation; map growth is represented only by vanilla-looking rows, columns, nodes, and paths.

Manual retest:

- Start a fresh A11 run and inspect Act 1 for one extra late route layer.
- Continue to Act 2 and Act 3; Act 3 should be visibly longer than Act 1/2.
- Confirm the map still has a low-risk route and boss reachability.

### ISSUE-2026-05-07-A11-MAP-CHANGE-ANIMATION

Priority: P3

Status: open

Area: A11/A17 map UI feedback

Player report: map and visibility changes should not feel random; the player should clearly see that something changed.

Current mitigation:

- A17 deep-branch nodes still have map hover tips.
- A11 long-road node tips were removed after player feedback; A11 now relies on normal map geometry instead of a special visible marker.

Remaining work:

- No bespoke map-generation animation or transition sequence has been implemented yet.
- Live UI pass should decide whether hover tips are enough or whether a short map pulse/overlay is needed.

### ISSUE-2026-05-07-A12-TOOLTIP-RICHTEXT-COLORS

Priority: P2

Status: source-patched; live verification pending

Area: A12 Firemark / Forge Token / Banner tooltip text

Player report: A12 text works mechanically, but numbers should be blue and important words such as upgrade, Gold, Skill card, Firemark, Forge Token, Rest, and Smith should be gold.

Current source fix:

- `ForgeTokenRelic` English/ZHS relic text and rest-site extra text now use `[blue]` for values and `[gold]` for important terms.
- Firemark power tooltips now color values and core terms.
- Banner room power/localization strings now color values and core terms.
- Ascension panel localization for A12/A13/Banners now uses the same markup.

Manual retest:

- Hover Forge Token, Firemark powers, Banner powers, A12/A13 ascension rows, and rest-site Forge Token extra text.
- Confirm rich text renders instead of showing raw tags.
- Confirm Chinese text wraps cleanly.

### ISSUE-2026-05-07-A13-FISSION-TOO-RARE-AT-HIGH-ASCENSION

Priority: P2

Status: source-patched; live verification pending

Area: A13 Fission Enchantment / A16 inherited ascension behavior

Player report: A16 should include earlier ascension effects, but Fission nearly disappeared while testing A16.

Current source evidence:

- `AscensionFeatureGate.IsLevelEnabled(...)` uses `runState.AscensionLevel >= requiredAscensionLevel`, so A16 includes A13 when the public/debug gate is active.
- Fission source chances were raised from `10/15/20/5` to `25/35/40/15` for normal combat / Banner Room / Firemarked Elite / Boss rewards.

Manual retest:

- Test A16 with public/debug ascension enabled.
- Check repeated normal combat rewards and Banner Room rewards.
- Confirm Fission remains limited to eligible Attack/Skill cards and still appears at most once per reward screen.

### ISSUE-2026-05-07-ROOTBUD-ROOTBLIGHT-REWORK

Priority: P1

Status: source-patched; live verification pending

Area: A14/A15/A18 Rootblight and Blight Sprout

Player report: Root Bud and Rootblight were conceptually unclear, Boss Sprout count was too low, and Boss/Elite Sprout text was too long.

Current source fix:

- ZHS player-facing term is now `根芽`.
- Boss fights in Acts 2/3 now seed 2 Root Bud cards.
- Root Bud text is shortened: play to Exhaust; Boss sprouts use rounds 3/4 and elite sprouts use round 3; if seen and not played, add Rootblight I after combat.
- Rootblight I/II/III costs are 2/3/4.
- Played Rootblight removes its master-deck card and queues the downgrade card after combat.
- Unplayed Rootblight I/II upgrades after combat; ignored Rootblight III stays III and adds one Rootblight I only once per card.
- Rootblight is capped at 4 cards, and cap hits show `Root system full.` / `根系已满。`.
- Rest removes exactly one highest-stage Rootblight instead of clearing all Rootblight.

Manual retest:

- A14 new run starts with Rootblight I.
- A15 Act 2/3 Boss fights bury 2 Root Buds.
- A18 eligible Act 2/3 Elite fights bury 1 Root Bud.
- Seen-but-unplayed Root Bud adds one Rootblight I after combat.
- Rootblight I/II/III play and post-combat behavior matches the new card text.

### ISSUE-2026-05-07-MULTIPLAYER-A11-A20-SELECTION-BLOCKED

Priority: P1

Status: source-patched; live co-op verification pending

Area: A11-A20 Ascension selection / multiplayer lobby

Player report: A11-A20 cannot be used in multiplayer, but co-op should eventually support the same expanded Ascension range instead of being single-player only.

Desired behavior:

- A11-A20 selection is available in multiplayer when the explicit public/development Ascension gate is enabled.
- Multiplayer selection must not patch or corrupt vanilla A1-A10 progress.
- Earlier Ascension effects still inherit normally at higher levels.
- Per-player systems such as Rootblight and Blight Sprout remain independent and do not desync.
- A21-A30 remains out of scope.

Implementation notes:

- Local source inspection found that `StartRunLobby.UpdateMaxMultiplayerAscension()` computes the multiplayer cap from each `LobbyPlayer.maxMultiplayerAscensionUnlocked`, while `UpdatePreferredAscension()` writes host selections to `PreferredMultiplayerAscension`.
- Current source patch expands only host multiplayer lobbies when `EZMB_ASCENSION_ALLOW_PUBLIC_ASCENSION=1` is set, temporarily raises in-memory lobby unlock caps only during max recomputation, restores them in a finalizer, and skips A11-A20 preferred-progress writes.
- Host-multiplayer A11-A20 selection is independently disableable with `EZMB_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1`.
- A11-A20 gameplay, per-player Rootblight/Blight Sprout ownership, and desync behavior still require live co-op verification.

Manual retest:

- Host a multiplayer lobby with BaseLib and EZ Micro Balance enabled.
- Confirm A1-A10 behavior is unchanged with the gate disabled.
- Enable the development/public A11-A20 gate and confirm the lobby can select A11-A20.
- Start a co-op run at A11/A12/A14/A16 and confirm all players load without desync.

### ISSUE-2026-05-07-A11-LONG-ROAD-MAP-MARKER-UNWANTED

Priority: P2

Status: source-patched; live verification pending

Area: A11 Wide Tower, Long Road / map UI

Player report: A11 should make the map longer/wider through normal map geometry only. It should not put a special visible marker or hover tooltip on the map just to explain the extra route space.

Desired behavior:

- Remove the dedicated A11 long-road map marker/hover indicator from newly inserted route nodes.
- A11 map changes should look like vanilla map rows and paths, not like a special event or quest node.
- Keep the actual map-length/width tuning separately testable; if final tuning is only one added row, update docs/localization/changelog to avoid claiming larger route growth.

Implementation notes:

- `LongRoad` metadata, `MarkLongRoad`, and `LONG_ROAD_NODE` localization were removed from active source/resources.
- `AscensionMapQuestMarker` remains in use only for A17 Deep Branch generic markers; A12 Firemark, A16 Banner, A17 Deep Branch, A19 Seal, and A20 Brand indicators remain on their own paths.

Manual retest:

- Start A11 and inspect all acts' maps.
- Confirm the map has the intended route geometry change.
- Confirm no A11-specific icon, marker, or hover tooltip appears on ordinary route nodes.
- Confirm Firemark/Banner/Deep Branch/Boss Seal indicators still appear when their own Ascensions are enabled.

## Resolved / Player-Verified

### ISSUE-2026-05-07-A12-FORGE-TOKEN-RESTSITE-CRASH

Priority: P1

Status: player reported fixed on 2026-05-07; keep in regression list

Area: A12 Forge Token / rest-site transition

Player verification: carrying Forge Token into a rest site no longer crashes.

Regression retest:

- Enter a rest site while holding Forge Token.
- Test Rest payout and Smith payout separately.
- Confirm token relic is removed after payout.
