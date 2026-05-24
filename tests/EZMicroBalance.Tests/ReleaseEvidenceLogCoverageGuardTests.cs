using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ReleaseEvidenceLogCoverageGuardTests
{
    [Fact]
    public void HighRiskRuntimeSurfacesEmitReleaseEvidenceMarkers()
    {
        var source = ReadSourceTree("EZMicroBalanceCode");

        AssertEvidenceSurface(
            source,
            "Vakuu child combat",
            "VakuuFight",
            "fight_started",
            "child_combat_room_entered",
            "parent_event_resume_success",
            "fallback_map_exit");
        AssertEvidenceSurface(
            source,
            "Urda Root Eyes",
            "UrdaRootEyes",
            "selection_opened",
            "node_selected",
            "preview_saved",
            "node_entered",
            "preview_refunded",
            "selection_cancelled",
            "selection_cleared_context_changed",
            "save_hydrate_marker_restored");
        AssertEvidenceSurface(
            source,
            "Urda Seed Bank",
            "UrdaSeedBank",
            "save_hydrate_storage_restored",
            "settlement_empty",
            "selection_cancelled",
            "deck_add_failed",
            "extracted_by_relic_click");
        AssertEvidenceSurface(
            source,
            "Rootblight",
            "Rootblight",
            "state_hydrated_from_deck",
            "combat_start_marked",
            "combat_end_notice_queued");
        AssertEvidenceSurface(
            source,
            "Blight Sprout",
            "BlightSprout",
            "gate_skipped",
            "seeded",
            "sprouted_to_draw_top",
            "entered_hand",
            "played",
            "growth_rootblight_queued",
            "player_death_growth_cleared");
        AssertEvidenceSurface(
            source,
            "Preview tools",
            "PreviewCrystalSphere",
            "peek_button_added",
            "peek_enabled",
            "peek_hidden_after_minigame",
            "PreviewTransform",
            "rng_context_registered",
            "prediction_prepared",
            "prediction_displayed",
            "rng_context_cleared");
        AssertEvidenceSurface(
            source,
            "A11-A20 and co-op gates",
            "A20BrandedForm",
            "coop_gate_disabled",
            "boss_marker_applied",
            "courtyard_entered");
    }

    [Fact]
    public void ReleaseEvidenceLogRemainsOptInAndGrepFriendly()
    {
        var logSource = ReadRepoText("EZMicroBalanceCode", "Diagnostics", "ReleaseEvidenceLog.cs");
        var statusDoc = ReadRepoText("docs", "release-evidence-status.md");
        var issues = ReadRepoText("docs", "issues.md");

        Assert.Contains("SPIREPLUS_RELEASE_EVIDENCE_LOG", logSource, StringComparison.Ordinal);
        Assert.Contains("EZMB_RELEASE_EVIDENCE_LOG", logSource, StringComparison.Ordinal);
        Assert.Contains("[SPIREPLUS-EVIDENCE]", logSource, StringComparison.Ordinal);
        Assert.Contains("if (!IsEnabled)", logSource, StringComparison.Ordinal);
        Assert.Contains("return;", SliceFrom(logSource, "if (!IsEnabled)"), StringComparison.Ordinal);
        Assert.Contains("FormatData", logSource, StringComparison.Ordinal);
        Assert.Contains("CultureInfo.InvariantCulture", logSource, StringComparison.Ordinal);
        Assert.Contains("SPIREPLUS_RELEASE_EVIDENCE_LOG=1", statusDoc, StringComparison.Ordinal);
        Assert.Contains("EZMB_RELEASE_EVIDENCE_LOG=1", statusDoc, StringComparison.Ordinal);
        Assert.Contains("STRICT-AUDIT-EVIDENCE-LOG", issues, StringComparison.Ordinal);
    }

    private static void AssertEvidenceSurface(string source, string label, params string[] snippets)
    {
        foreach (var snippet in snippets)
        {
            Assert.Contains(snippet, source, StringComparison.Ordinal);
        }

        var surfaceSlice = SliceFrom(source, snippets[0]);
        Assert.Contains(
            "ReleaseEvidenceLog.Log",
            surfaceSlice,
            StringComparison.Ordinal);
    }
}
