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
            "- [x] Manifest declares structured `STS2-RitsuLib` dependency with `min_version: 0.4.31`.",
            "- [x] PCK audit packages only `EZMicroBalance` installable resources and excludes C# source, docs, art, asset, and archive folders.",
            "- [ ] STS2-RitsuLib appears in Mod Settings for the beta.95 RitsuLib-only package.",
            "- [x] Spire Plus appears in the current normal Steam-client manifest list and registers its config page under the refreshed display-name package.",
            "- [x] Historical refreshed Mod Settings UI list screenshot shows `Spire Plus` after the display-name refresh package is installed.",
            "- [ ] Current beta.95 Mod Settings list plus Spire Plus config page screenshots are captured under release-evidence row `mod-settings-current-display`.",
            "current-spire-plus-modsettings-20260513-111342",
            "- [x] Previous RitsuLib-only Off loader smoke for the beta.93 ZIP hash is captured after the latest RitsuLib package refresh",
            "- [x] Previous RitsuLib-only AdditiveBatch1 registration smoke for the beta.93 ZIP hash is captured",
            "- [ ] Latest RitsuLib-only Off loader smoke for the current beta.95 package hash is recaptured on Slay the Spire 2 `v0.107.1`; retained beta.87/beta.88/beta.90 loader evidence is historical context only.",
            "- [x] `godot.log` reviewed after fresh beta.93 RitsuLib-only Off and AdditiveBatch1 isolated startup/log verification.",
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
            "Previous beta.93 RitsuLib-only Off proof has been recaptured under `.tools\\runtime-evidence\\v01071-beta93-ritsulib0431-off-direct-20260621` and closes the package-hash loader smoke for that Off-mode surface.",
            "Previous beta.93 AdditiveBatch1 registration proof has been recaptured under `.tools\\runtime-evidence\\v01071-beta93-ritsulib0431-additivebatch1-direct-20260621`",
            "loader/registration evidence, not gameplay proof.",
            "Refreshed normal Steam-client Mod Settings UI evidence at `.tools\\runtime-evidence\\current-spire-plus-modsettings-20260513-111342\\02-mod-config-list.png` shows `Spire Plus`",
            "current beta.95 list plus Spire Plus config-page proof remains pending under release-evidence row `mod-settings-current-display`",
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

        Assert.Contains("Status: automated gates passed for the current RitsuLib-only beta.95 package shape; previous beta.93 Off/AdditiveBatch1 loader proof reaches main menu with exactly STS2-RitsuLib and Spire Plus loaded, but it is loader/registration evidence only.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Full live Ancient reward gameplay, Rootblight combat-end behavior/notices, natural route-click first-node checks beyond the historical A11 spot check, Ancient save/load, natural A11 click-by-click traversal, and multiplayer verification are still pending.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Natural route-click first-node path remains pending.", manualMatrix, StringComparison.Ordinal);
        Assert.Contains("Result: pending.", manualMatrix, StringComparison.Ordinal);
    }
}
