using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseCoverageGuardTests
{
    [Fact]
    public void AncientExpansionV22DocsTrackActiveSourceReadySlices()
    {
        var issues = ReadRepoText("docs", "issues.md");
        var v22Issues = ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md");
        var featureReadme = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "README.md");
        var sourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var implementationPlan = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "implementation-plan.md");
        var safetyRules = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "card-and-power-safety-rules.md");
        var riskRegister = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md");
        var projectState = ReadRepoText("PROJECT_STATE.md");
        var docsIndex = ReadRepoText("docs", "README.md");
        var projectMap = ReadRepoText("docs", "PROJECT_MAP.md");
        var featuresIndex = ReadRepoText("docs", "features", "README.md");
        var activeExpansionSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion");

        Assert.Contains("docs/issues/ancient-expansion-v2.2.md", issues, StringComparison.Ordinal);
        Assert.Contains("morvi_forbidden_loan", issues, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("lotha_death_reprieve", issues, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Vakuu fight", issues, StringComparison.OrdinalIgnoreCase);

        Assert.Contains("Lotha is default-on", v22Issues, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Vakuu fight", v22Issues, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("save/load", v22Issues, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-DESIGN-DOC-INGEST", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-CARD-POWER-SAFETY-RULES", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-MORVI-V22-PLANNING", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-LOTHA-V22-PLANNING", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-VAKUU-FIGHT-V22-PLANNING", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-URDA-V22-ALIGNMENT", v22Issues, StringComparison.Ordinal);
        Assert.Contains("ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-MILESTONE-GATES", v22Issues, StringComparison.Ordinal);
        var milestoneGateIssue = SliceBetween(
            v22Issues,
            "## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-MILESTONE-GATES",
            "## ISSUE-2026-05-12-ANCIENT-EXPANSION-V22-MORVI-LOTHA-ART-INTEGRATION");
        AssertSourceContains(
            milestoneGateIssue,
            "Status: source-governed / live-pending",
            "issue row first, source research, focused guard, implementation, manual row, review note, then validation",
            "Live-ready still requires current-package screenshots/logs/manual notes");
        Assert.DoesNotContain("Status: open", milestoneGateIssue, StringComparison.Ordinal);

        AssertSourceContains(
            featureReadme,
            "default-on Morvi v2.2",
            "Lotha is default-on",
            "Vakuu fight",
            "Live gameplay and save/load verification for current Urda remains pending");
        AssertSourceContains(
            sourceDesign,
            "Seedbed",
            "Humus Pact",
            "Molting",
            "Moss Map",
            "Trial Branch",
            "Shallow-Root Relic",
            "Rooted Route",
            "After the Rain",
            "Root-Sight",
            "Seed Bank",
            "Morvi is default-on",
            "Lotha is default-on",
            "Vakuu fight");
        AssertSourceContains(
            implementationPlan,
            "Open or update a compact issue row with acceptance criteria and the manual proof needed",
            "Record source evidence in `api-research.md`",
            "Add focused source guard tests before or with implementation",
            "`source-ready`: implementation, source evidence, focused guards, localization/text/art coverage, build, tests, format, and diff-check pass",
            "`live-ready`: current-package screenshots, `godot.log`, manual notes, save/load or two-client evidence exist",
            "Do not start a future milestone as a documentation-only audit");
        AssertSourceContains(
            safetyRules,
            "Power cards are not copied, extra-played, or replayed by default",
            "Extra-played or copied cards must not recursively trigger the same blessing",
            "Each Morvi or Lotha blessing");
        AssertSourceContains(
            riskRegister,
            "Power-card extra-play exploit",
            "Death-interrupt complexity",
            "Reward UI softlock",
            "Multiplayer ownership/desync",
            "Save/load persistence");

        Assert.Contains("ancient-expansion-v2.2", projectState, StringComparison.Ordinal);
        Assert.Contains("default-on Morvi", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Vakuu fight", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("save/load", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("features/ancient-expansion-v2.2/README.md", docsIndex, StringComparison.Ordinal);
        Assert.Contains("docs/features/ancient-expansion-v2.2/README.md", projectMap, StringComparison.Ordinal);
        Assert.Contains("ancient-expansion-v2.2/README.md", featuresIndex, StringComparison.Ordinal);

        Assert.Contains("MorviFeatureGate", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EZMB_DISABLE_MORVI", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_DISABLE_MORVI", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EZMB_FORCE_MORVI_BLESSING", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("LothaFeatureGate", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EZMB_DISABLE_LOTHA", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_DISABLE_LOTHA", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("VakuuFightFeatureGate", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EZMB_DISABLE_VAKUU_FIGHT", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_FORCE_VAKUU_FIGHT", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EventModel.Resume", activeExpansionSource, StringComparison.Ordinal);
        Assert.Contains("EzmbVakuuTrialEncounter", activeExpansionSource, StringComparison.Ordinal);
    }

    [Fact]
    public void ActiveAncientExpansionEventArtIsExportedAndDocumented()
    {
        var exportPreset = ReadRepoText("export_presets.cfg");
        var artDirection = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "art-direction.md");
        var v22Issues = ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md");
        var workLog = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "work-log.md");

        var morviPng = AssertRepoFileExists("EZMicroBalance", "images", "events", "ezmb_morvi.png");

        Assert.Contains("res://EZMicroBalance/images/events/ezmb_morvi.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/scenes/events/background_scenes/ezmb_morvi.tscn", exportPreset, StringComparison.Ordinal);
        Assert.True(
            new FileInfo(morviPng).Length > 1_000_000,
            "Morvi event art must not regress to a small geometric placeholder.");
        AssertSmallUiPngHasAlpha(
            RepoPath("EZMicroBalance", "images", "ancients", "morvi", "ezmb_morvi_map_icon.png"),
            "Morvi map icon must remain a readable transparent UI resource.");
        AssertRepoFileExists("EZMicroBalance", "images", "events", "ezmb_lotha.png");
        Assert.Contains("res://EZMicroBalance/images/events/ezmb_lotha.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/scenes/events/background_scenes/ezmb_lotha.tscn", exportPreset, StringComparison.Ordinal);
        Assert.True(
            new FileInfo(RepoPath("EZMicroBalance", "images", "events", "ezmb_lotha.png")).Length > 1_000_000,
            "Lotha event art must not regress to the small geometric placeholder.");
        AssertSmallUiPngHasAlpha(
            RepoPath("EZMicroBalance", "images", "ancients", "lotha", "ezmb_lotha_map_icon.png"),
            "Lotha map icon must remain a readable transparent UI resource.");
        foreach (var optionArt in Directory.GetFiles(RepoPath("EZMicroBalance", "images", "ancients", "lotha", "options"), "*.png"))
        {
            AssertSmallUiPngHasAlpha(optionArt, $"{optionArt} must remain a readable transparent option icon.");
        }

        AssertSourceContains(
            artDirection,
            "Active Morvi event art uses the recovered user-uploaded blue-eye court source",
            "Active event art now uses the corrected user-uploaded horizontal mirror-ensemble source",
            "Active event art is the original user-accepted 16:9 Urda middle-draft",
            "Final browser GPTimage2 small art generated this pass",
            "Urda, Morvi, and Lotha option/icon art uses browser ChatGPT/GPTimage2 rebuilt transparent PNGs",
            "Custom card portraits now use browser GPTimage2 rebuilt files",
            "No `generic_temporary` or `final_required_before_release` art blockers remain",
            "Do not use placeholder art for Morvi or future active Ancients just to satisfy the export list.");
        AssertSourceContains(
            v22Issues,
            "Morvi is default-on",
            "Lotha is default-on");
        Assert.Contains("Recovered the user-uploaded Morvi blue-eye court background", workLog, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Recovered the correct user-uploaded horizontal mirror-ensemble image", workLog, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Restored the user-accepted 16:9 Urda root-mother background", workLog, StringComparison.OrdinalIgnoreCase);
    }
}
