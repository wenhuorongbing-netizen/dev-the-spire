using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    private static readonly string[] RequiredManualMatrixRows =
    [
        "Pael's Horn",
        "Black Star",
        "War Hammer",
        "Jewelry Box",
        "Preserved Fog / Folly",
        "Vakuu's Sere Talon",
        "Choices Paradox",
        "Jeweled Mask",
        "Prismatic Gem",
        "Distinguished Cape",
        "Velvet Choker",
        "Pael's Tooth",
        "Sovereign Blade / Forge",
        "Seal of Gold / Debt",
        "Sozu",
        "Ectoplasm",
        "Fiddle",
        "Iron Club",
        "Brilliant Scarf",
        "Beautiful Bracelet",
        "Music Box",
        "Crossbow",
        "Toasty Mittens",
        "Whispering Earring",
        "Brilliant Flame / Brightest Flame",
        "Meat Cleaver",
        "Blood-Soaked Rose / Enthralled"
    ];

    [Fact]
    public void ReleaseChecklistKeepsPendingRuntimeGatesAndManualRowsExplicit()
    {
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var manualMatrix = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-verification-matrix.md");

        AssertSourceContains(
            releaseChecklist,
            "Target manifest id: `EZMicroBalance`",
            "- [x] The active release surface is one mod: `Spire Plus`.",
            "- [x] Legacy `EzDailyContent` and standalone `EZFuturePeek` root mod surfaces have been removed from the active tree.",
            "- [x] `EZMicroBalance` has its own manifest, project, code folder, resource folder, DLL, and PCK.",
            "- [x] Manifest declares structured `BaseLib` dependency with `min_version: v3.3.0`.",
            "- [x] PCK audit packages only `EZMicroBalance` installable resources and excludes C# source, docs, art, asset, and archive folders.",
            "- [x] BaseLib appears in Mod Settings.",
            "- [x] Spire Plus appears in the current normal Steam-client manifest list and registers its config page under the refreshed display-name package.",
            "- [x] Historical refreshed Mod Settings UI list screenshot shows `Spire Plus` after the display-name refresh package is installed.",
            "- [ ] Current beta.88 Mod Settings list plus Spire Plus config page screenshots are captured under release-evidence row `mod-settings-current-display`.",
            "current-spire-plus-modsettings-20260513-111342",
            "- [x] Fresh loader smoke for the current beta.88 ZIP hash passed under `.tools\\runtime-evidence\\v01071-beta88-baselib330-additive-batch1-direct-cleanlog-20260619-103937`.",
            "- [x] Latest loader smoke for the current beta.88 package hash was recaptured on Slay the Spire 2 `v0.107.1`; retained beta.87 loader evidence is historical context only.",
            "- [x] `godot.log` reviewed after fresh beta.88 direct enabled-mode isolated startup/log verification.",
            "- [ ] `godot.log` reviewed after full normal Steam-client gameplay/manual verification.",
            "- [ ] Every implemented Ancient reward change has a completed manual runtime result.",
            "- [ ] Save/load-sensitive behavior is tested.",
            "- [ ] Disable-mod gameplay behavior is tested in a run.",
            "- [x] Author placeholder is replaced for this private beta; `EZMicroBalance.json` author is `wenhuorongbing-netizen`.",
            "- [x] Rootblight I/II/III and Blight Sprout generated portrait art is integrated and packaged; live in-game visual verification remains part of the manual matrix.",
            "- [ ] Multiplayer disposition is decided: verified, or release-noted as unsupported/unverified.",
            "- [ ] Worktree is clean.",
            "- [ ] Commit is created.",
            "- [ ] Push to `origin` is performed after validation, packaging, and an intentional commit.",
            "Fresh loader smoke for the current beta.88 package hash passed after the BaseLib `v3.3.0` package refresh.",
            "Refreshed normal Steam-client Mod Settings UI evidence at `.tools\\runtime-evidence\\current-spire-plus-modsettings-20260513-111342\\02-mod-config-list.png` shows `Spire Plus`",
            "current beta.88 list plus Spire Plus config-page proof remains pending under release-evidence row `mod-settings-current-display`",
            "Manual feature results are pending",
            "Unsupported Cases",
            "A11-A20 selection is default-on only for single-player standard lobbies",
            "SPIREPLUS_ASCENSION_DISABLE_PUBLIC_SELECTION=1",
            "SPIREPLUS_ASCENSION_DISABLE_MULTIPLAYER_SELECTION=1",
            "A11 widens maps by 1 column, inserts a reachable optional route node in the new column, and adds route rows by act: Act 1 +1, Act 2 +1, Act 3 +2 without A11-specific map markers or hover tips.",
            "A17 inserts one optional 3-4 node Deep Branch in Acts 2/3",
            "A20 uses the vanilla double-boss map path to create/reveal the final-act second Boss",
            "Ascension 21-30 and custom-character content are not included.",
            "Prismatic Gem intentionally skips custom pools, filtered pools, colorless-only pools");

        foreach (var row in RequiredManualMatrixRows)
        {
            Assert.Contains($"| {row} |", manualMatrix, StringComparison.Ordinal);
        }

        Assert.Contains("Status: automated gates passed; latest normal Steam-client startup/log verification is historical for the earlier 22-field package; refreshed normal Steam-client Mod Settings UI list screenshot shows Spire Plus; historical page-level Mod Settings UI passed under the old display name; A0/A10/A20 single-player DevConsole combat smoke passed; A11 Act 1 map/save-load spot check and saved-map boss-reachability graph proof passed; A11 Act 2/3 map-surface observation passed; targeted A14 Rootblight English/ZHS hover/starter-notice spot checks passed.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Full live Ancient reward gameplay, Rootblight combat-end behavior/notices, natural route-click first-node checks beyond the A11 spot check, Ancient save/load, natural A11 click-by-click traversal, and multiplayer verification are still pending.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Natural route-click first-node path remains pending.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Result: pending.", manualMatrix, StringComparison.Ordinal);
    }
}
