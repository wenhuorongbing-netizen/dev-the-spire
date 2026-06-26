# PRD — Close the Golden Idol Parity Gap (custom relic)

> **Track:** event · **Date:** 2026-06-26 · **Reconciled against:** beta.135 / STS2 v0.107.1 / RitsuLib 0.4.34
> **Gate:** Everything here stays behind the existing `Sts1EventMode` `CanaryOnly` gate. Default `Off` => zero registrations => zero impact on the default Spire Plus experience.

## 1. Problem / current truth

`EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1GoldenIdol.cs` `TakeIdol()` currently grants
`RelicFactory.PullNextRelicFromFront(owner)` — a *random* relic — because StS2 has no native
Golden Idol relic. In StS1 the Take/Leave branch always grants the **Golden Idol** relic itself.
This is the last open content-parity item flagged "GAP / temporary-substitute" for the Golden Idol
canary in `docs/features/sts1-events/content-parity-gaps.md` (row 2).

## 2. Scope

- Add a custom `Sts1GoldenIdolRelic : ModRelicTemplate` (mirrors the existing Ancient option-relic
  pattern: `RelicRarity.Event`, not allowed in pools/shops/Neow, custom icon paths).
- Register that relic through the **same RitsuLib content pack** that registers the canary events
  (`Sts1EventRegistrationService.RegisterCanaryOnly` and `RegisterAll`), using
  `content.Relic<SharedRelicPool, Sts1GoldenIdolRelic>()` — the exact call shape used by
  `SpirePlusContentRegistrationService.RegisterRelics`.
- Wire `Sts1GoldenIdol.TakeIdol()` to grant THAT relic via the established
  `ModelDb.Relic<T>().ToMutable()` + `RelicCmd.Obtain(relic, owner)` pattern
  (same as `AncientRewardRelicService` / `VakuuFightVictoryChoices`).
- Add EN + ZHS localization in `EZMicroBalance/localization/{eng,zhs}/relics.json`, using the
  three established Spire Plus relic key forms (see §5).
- Placeholder art: reuse the existing generic relic placeholder textures already shipped at
  `res://EZMicroBalance/images/relics/relic.png` (icon + outline) and
  `res://EZMicroBalance/images/relics/big/relic.png` (big icon). Marked **art pending**; no original
  game art is copied.
- Add a guard test asserting `TakeIdol` grants the Golden Idol relic (not a random one), and that
  the relic is registered in both canary and all-draft content packs.

## 3. Non-scope

- No change to the trap branch (Outrun/Smash/Hide) — already parity-correct.
- No change to `Core/Features/**` (the shared feature registry). The canary content pack already
  exists and is gated; adding a `content.Relic<...>()` call inside it needs no registry change.
- No new event registrations, no new modes, no behavior in `Off` mode.
- No bespoke art asset authored; placeholder only.
- Do NOT launch the game; do NOT commit/push.

## 4. File plan (all within Allowed Files)

| File | Change |
|------|--------|
| `EZMicroBalanceCode/Sts1Events/Content/Sts1GoldenIdolRelic.cs` | **New.** Custom relic model + asset-path helper. (Placed in a new `Content/` folder, not `Models/Shared/`, so the relic is not mistaken for an event model by the `AllSharedEventModelsDeclareIsSharedTrue` guard.) |
| `EZMicroBalanceCode/Sts1Events/Models/Shared/Sts1GoldenIdol.cs` | `TakeIdol()` grants `Sts1GoldenIdolRelic` instead of a random relic. |
| `EZMicroBalanceCode/Sts1Events/Runtime/Sts1EventRegistrationService.Canary.cs` | Register the relic in the CanaryOnly content pack. |
| `EZMicroBalanceCode/Sts1Events/Runtime/Sts1EventRegistrationService.AllDraft.cs` | Register the relic in the all-draft content pack (parity). |
| `EZMicroBalanceCode/Sts1Events/Runtime/Sts1EventRegistrationService.cs` | Fix stale doc-comment (Task 2). |
| `EZMicroBalance/localization/eng/relics.json` | EN relic loc (3 key forms). |
| `EZMicroBalance/localization/zhs/relics.json` | ZHS relic loc (3 key forms). |
| `tests/EZMicroBalance.Tests/Sts1EventFeatureGuardTests.EventContent.cs` | New guard test(s). |
| `docs/features/sts1-events/content-parity-gaps.md` | Mark gap closed (art pending). |

## 5. Localization key forms (mirror existing relics)

For class `Sts1GoldenIdolRelic`, slug = `STS1_GOLDEN_IDOL_RELIC`. Every existing Spire Plus relic
carries these prefixes in `relics.json`; replicate all three with `.title` / `.description` /
`.flavor`:

- `EZMICROBALANCE-STS1_GOLDEN_IDOL_RELIC.*`
- `EZMICROBALANCE-Sts1GoldenIdolRelic.*`
- `EZ_MICRO_BALANCE_RELIC_STS1_GOLDEN_IDOL_RELIC.*`

## 6. Acceptance ("fully implemented")

1. `dotnet build EZMicroBalance.sln` exits 0 (unfiltered).
2. Focused Sts1 guard tests pass, including the new test asserting:
   - `Sts1GoldenIdol.TakeIdol` calls `ModelDb.Relic<Sts1GoldenIdolRelic>()` / `RelicCmd.Obtain` and
     does **not** call `RelicFactory.PullNextRelicFromFront`.
   - `RegisterCanaryOnly` and `RegisterAll` register `content.Relic<SharedRelicPool, Sts1GoldenIdolRelic>()`.
   - EN + ZHS `relics.json` contain the relic keys.
3. CanaryOnly still registers exactly 4 event *types* (relic registration is not an event; existing
   event-count guards stay green).
4. `Off` mode: still returns immediately, zero registrations (existing guard stays green).
5. Doc-comment in `Sts1EventRegistrationService.cs` cites `ActModel.Index` evidence, not the
   non-existent `*AncientService` classes.
6. `content-parity-gaps.md` row 2 marked closed (art pending).

## 7. Risks / rollback

- **Art:** placeholder only; a coordinator/art follow-up can drop a real icon at the asset path
  later. Low risk — generic placeholder already ships and imports cleanly.
- **Registration location:** relic is registered ONLY inside the gated event content packs, never in
  the always-on `SpirePlusContentRegistrationService`. This preserves "default Off => zero impact".
- **Rollback:** revert the new file + the `TakeIdol` line + the two `content.Relic<...>` lines + loc
  keys; the gap reverts to the random-relic substitute.
- **Coordinator hookup (`Core/Features`):** none required. The relic rides the existing gated canary
  content pack; no shared feature-registry change. (If a future always-on registration is wanted, that
  would be a separate scope.)

## 8. Coordinator hookup — website localization mirror (OUT OF EVENT-TRACK FENCE)

Adding the relic to `EZMicroBalance/localization/{eng,zhs}/relics.json` (in-fence, done) breaks one
guard outside the event-track fence:

- **Test:** `WebsiteContentGuardTests.WebsiteLocalizationSubsetMatchesCurrentModLocalization`
- **Why:** `website/assets/localization/{eng,zhs}/relics.json` is maintained as a **byte-verbatim
  mirror** of the mod's `relics.json`. At `HEAD` the two files are identical; my append of 9 relic keys
  per language desyncs them.
- **Files to sync (NOT in event-track Allowed list — `website/**`):**
  `website/assets/localization/eng/relics.json` and `website/assets/localization/zhs/relics.json`.
- **Turnkey fix (verbatim copy — the website mirror is a pure copy, no transformation):**

  ```powershell
  Copy-Item -Force EZMicroBalance/localization/eng/relics.json website/assets/localization/eng/relics.json
  Copy-Item -Force EZMicroBalance/localization/zhs/relics.json website/assets/localization/zhs/relics.json
  ```

  After copying, `WebsiteLocalizationSubsetMatchesCurrentModLocalization` goes green again.
- **Scope note:** The event track intentionally did NOT touch `website/**` (hard Allowed-list
  boundary). This is the only cross-fence follow-up for the Golden Idol relic.

### Pre-existing failure (NOT caused by this change, NOT in scope)

`ReleaseSafetyExpandedGuardTests.ReleaseEvidenceVerifierCoversManualBlockersBeforeReleaseClaims` is
**already red at `HEAD`** independent of this work: it asserts strings in
`scripts/verify-spire-plus-release-evidence.ps1` (e.g. `Get-SpirePlusPackageSha256 -RepoRoot $repoRoot
-PackagePath $PackagePath`) that the current script does not contain. None of those files were touched
here. Flagging for the release/debug track; not an event-track item.
