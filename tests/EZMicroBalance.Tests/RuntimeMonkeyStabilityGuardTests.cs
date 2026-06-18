using System.Diagnostics;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void MonkeyRunnerDefaultsToDryRunAndLaunchIsExplicit()
    {
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");

        AssertSourceContains(
            runner,
            "[switch]$Launch",
            "[string]$Scenario = 'AncientUiSmoke'",
            "[string]$CommandSelectionMode = 'RoundRobin'",
            "[string]$CommandCorpusFile",
            "Get-ScenarioCommandCorpus",
            "AncientUiPlusVakuuFight",
            "VakuuFightSmoke",
            "StartupOnly",
            "spireplus_test_ancient VAKUU confirm fight",
            "monkey-plan.json",
            "PlannedCommands",
            "CommandScenarioMatrix",
            "ScenarioTag",
            "OwnerArea",
            "EffectiveNoDevConsoleCommands",
            "ReleaseEvidenceLogEnabled",
            "SPIREPLUS_RELEASE_EVIDENCE_LOG",
            "fight_option_shown",
            "if (-not $Launch)",
            "Dry-run only. Re-run with -Launch to start Steam sessions.",
            "exit 0",
            "& $liveSessionScript @prepareArgs",
            "'-Launch'");

        AssertBefore(runner, "if (-not $Launch)", "& $liveSessionScript @prepareArgs");
        Assert.DoesNotContain("Start-Process", runner, StringComparison.Ordinal);
        Assert.DoesNotContain("dotnet", runner, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MonkeyRunnerUsesRestoreSafeLiveSessionAndCanonicalLogAudit()
    {
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");

        AssertSourceContains(
            runner,
            "spire-plus-live-session.ps1",
            "audit-godot-log.ps1",
            "MainMenuTimeoutSeconds",
            "'\\[Startup\\] Time to main menu'",
            "godot.log.after-launch",
            "godot.log.current-iteration",
            "godot-log-audit.json",
            "iteration-result.json",
            "monkey-summary.json",
            "-StopGameOnRestore",
            "-PreserveNewCurrentRunsOnRestore",
            "main menu log line missing before timeout",
            "godot.log missing or empty",
            "audit-godot-log reports release-blocking signature hits",
            "live-session restore fails");
    }

    [Fact]
    public void MonkeyRunnerRecordsHangProbeTelemetryAndCommandAcks()
    {
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");

        AssertSourceContains(
            runner,
            "$hangProbeSchemaVersion = 1",
            "HangProbeSchemaVersion",
            "ObservationIntervalSeconds",
            "NoLogGrowthTimeoutSeconds",
            "UnresponsiveSampleThreshold",
            "SpirePlusRuntimeMonkeyNative",
            "IsHungAppWindow",
            "Test-LogContainsAfterOffset",
            "Get-SpireProcessSnapshot",
            "Add-ProbeSample",
            "runtime-probe-samples.json",
            "Write-CurrentIterationLogSlice",
            "CurrentIterationLogPath",
            "ProcessProbe",
            "LogGrowthProbe",
            "FailsOnlyAfterConsecutiveUnresponsiveSamples",
            "RequiresMainWindowAfterMainMenu",
            "PreLaunchLogLengthBytes",
            "BaselineLogLengthBytes",
            "MinimumProcessStartTimeUtc",
            "StaleProcessCount",
            "MaxConsecutiveUnresponsiveSamples",
            "CommandAckPatterns",
            "CommandSelectionMode",
            "RoundRobin",
            "CommandCorpusSource",
            "PlannedCommandCounts",
            "PlannedScenarioTagCounts",
            "PlannedOwnerAreaCounts",
            "PlannedVakuuFightIterationCount",
            "CommandCounts",
            "ScenarioTagCounts",
            "OwnerAreaCounts",
            "VakuuFightIterationCount",
            "Get-CommandOwnerArea",
            "Get-CommandScenarioTag",
            "Get-CommandAckPattern",
            "Starting unsaved live-test run for",
            "CommandAckObserved",
            "FailureReasonCodes",
            "HangSignals",
            "game_process_missing",
            "game_process_exited",
            "main_window_missing",
            "main_menu_timeout",
            "startup_log_stalled",
            "process_unresponsive",
            "current_iteration_log_missing",
            "command_ack_missing",
            "FailedIterationIds",
            "FailureReasonCounts",
            "ProcessExitCount",
            "MainWindowMissingCount",
            "CurrentIterationLogMissingCount",
            "UnresponsiveIterationCount",
            "LogStallIterationCount",
            "CommandAckMissingCount");
    }

    [Fact]
    public void MonkeyRunnerAndPacketCheckerGuardScenarioDistribution()
    {
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");
        var checker = ReadRepoText("scripts", "check-spire-plus-runtime-monkey-packet.ps1");

        AssertSourceContains(
            runner,
            "Get-ValueCounts",
            "PlannedCommandCounts",
            "PlannedScenarioTagCounts",
            "PlannedOwnerAreaCounts",
            "PlannedVakuuFightIterationCount",
            "CommandCounts",
            "ScenarioTagCounts",
            "OwnerAreaCounts",
            "VakuuFightIterationCount");

        AssertSourceContains(
            checker,
            "Test-CountMapMatches",
            "plan_planned_scenario_tag_counts_match",
            "plan_planned_owner_area_counts_match",
            "plan_planned_command_counts_match",
            "plan_vakuu_fight_planned_count_matches",
            "plan_vakuu_fight_smoke_all_iterations_are_fight",
            "plan_ancient_ui_plus_vakuu_fight_includes_fight",
            "plan_ancient_ui_plus_vakuu_fight_1000_balanced",
            "summary_scenario_tag_counts_match_results",
            "summary_owner_area_counts_match_results",
            "summary_command_counts_match_results",
            "summary_vakuu_fight_iteration_count_matches_results");
    }

    [Fact]
    public void RuntimeMonkeyPacketCheckerIsNoLaunchAndFailsOnRequiredEvidenceDrift()
    {
        var checker = ReadRepoText("scripts", "check-spire-plus-runtime-monkey-packet.ps1");

        AssertSourceContains(
            checker,
            "[string]$EvidenceDir",
            "[int]$ExpectedIterations",
            "[string]$ExpectedPackageVersion",
            "[int]$ExpectedPatchCount",
            "[switch]$FailOnMismatch",
            "monkey-plan.json",
            "monkey-summary.json",
            "iteration-result.json",
            "godot.log.after-launch",
            "godot-log-audit.json",
            "HangProbeSchemaVersion",
            "ProcessProbe",
            "LogGrowthProbe",
            "CommandAckPatterns",
            "plan_unresponsive_sample_threshold_positive",
            "RuntimeProbeSamplesPath",
            "CurrentIterationLogPath",
            "runtime_probe_samples_under_iteration_dir",
            "current_iteration_log_under_iteration_dir",
            "runtime_probe_samples_leaf_expected",
            "current_iteration_log_leaf_expected",
            "runtime_probe_samples_exist",
            "current_iteration_log_exists",
            "current_iteration_log_non_empty",
            "runtime_probe_samples_no_hung_window",
            "runtime_probe_samples_no_not_responding",
            "plan_launch_true",
            "plan_scenario_present",
            "plan_command_selection_mode_present",
            "plan_command_corpus_source_present",
            "plan_command_scenario_matrix_present",
            "summary_passed",
            "summary_failed_iterations_zero",
            "summary_process_exit_count_zero",
            "summary_main_window_missing_count_zero",
            "summary_current_iteration_log_missing_count_zero",
            "summary_unresponsive_iteration_count_zero",
            "summary_log_stall_iteration_count_zero",
            "summary_command_ack_missing_count_zero",
            "summary_max_consecutive_unresponsive_recorded",
            "scenario_tag_present",
            "owner_area_present",
            "command_selection_mode_present",
            "main_menu_reached",
            "main_menu_observation_passed",
            "runtime_observation_passed",
            "startup_log_probe_passed",
            "post_command_log_probe_passed",
            "responsiveness_probe_passed",
            "command_ack_observed",
            "failure_reason_codes_empty",
            "hang_signals_empty",
            "godot_log_exists",
            "godot_log_non_empty",
            "main_menu_log_line_present",
            "audit_clean",
            "sts1-mode-log-check.json",
            "sts1_mode_log_check_exists",
            "sts1_mode_log_check_mismatches_empty",
            "result_expectation_passed",
            "result_sts1_mode_verifier_passed",
            "expected_package_version_in_log",
            "expected_patch_count_in_log",
            "\\[Patcher - SpirePlus\\]",
            "Patch application complete:",
            "ModPatcher applied",
            "if ($FailOnMismatch -and $mismatches.Count -gt 0)");

        Assert.DoesNotContain("[switch]$Launch", checker, StringComparison.Ordinal);
        Assert.DoesNotContain("Start-Process", checker, StringComparison.Ordinal);
        Assert.DoesNotContain("spire-plus-live-session.ps1", checker, StringComparison.Ordinal);
        Assert.DoesNotContain("dotnet", checker, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RuntimeFailureAnalyzerIsNoLaunchAndMapsOwnerAreas()
    {
        var analyzer = ReadRepoText("scripts", "analyze-spire-plus-runtime-failure.ps1");

        AssertSourceContains(
            analyzer,
            "[string]$EvidenceDir",
            "[string]$IterationDir",
            "[string]$LogPath",
            "[switch]$FailOnBlockingFinding",
            "monkey-summary.json",
            "iteration-result.json",
            "godot.log.after-launch",
            "godot.log.current-iteration",
            "godot-log-audit.json",
            "FailureReasonCodes",
            "HangSignals",
            "EvidenceFiles",
            "Confidence",
            "ScenarioTag",
            "OwnerAreaHint",
            "OwnerAreaFromLog",
            "OwnerAreaFromCommand",
            "Get-OwnerAreaFromText",
            "Get-AuditOwnerText",
            "Resolve-OwnerArea",
            "-PreferLog",
            "PackageRuntimeDrift",
            "Sts1Events",
            "LiveSessionRestore",
            "DevConsoleHarness",
            "RuntimeLogAudit",
            "Runtime.Unknown",
            "Ancients.Vakuu.ChildCombatResume",
            "Ancients.Vakuu.FightOptionSetup",
            "Ancients.Morvi.CardPlayState",
            "Ancients.Lotha.CardPlayState",
            "Ancients.Urda.MapSaveState",
            "Ascension11To20.Rootblight",
            "Ancients.Vakuu",
            "Ancients.Morvi",
            "Ancients.Lotha",
            "Ancients.Urda",
            "Ascension11To20",
            "PreviewTools",
            "MultiplayerPolicy",
            "game_process_exited",
            "main_window_missing",
            "main_menu_timeout",
            "startup_log_stalled",
            "current_iteration_log_missing",
            "iteration_result_missing_or_invalid",
            "process_unresponsive",
            "command_ack_missing",
            "MissingFieldException",
            "MissingMethodException",
            "TypeLoadException",
            "runtime expectation",
            "Registered act event",
            "Spire Plus error/exception",
            "coop_gameplay_disabled",
            "coop_local_ui_preview_enabled",
            "ALLOW_UNVERIFIED_COOP",
            "blocking_findings=");

        Assert.DoesNotContain("Start-Process", analyzer, StringComparison.Ordinal);
        Assert.DoesNotContain("dotnet", analyzer, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Patch application complete:", analyzer, StringComparison.Ordinal);
        Assert.DoesNotContain("ModPatcher applied", analyzer, StringComparison.Ordinal);
        Assert.DoesNotContain("package version", analyzer, StringComparison.Ordinal);
        Assert.DoesNotContain("compat branch", analyzer, StringComparison.Ordinal);
    }

    [Fact]
    public void LocalGodotSourceWorkspaceCheckerIsNoLaunchAndGuardsFreshness()
    {
        var checker = ReadRepoText("scripts", "check-local-godot-source-workspace.ps1");

        AssertSourceContains(
            checker,
            "[string]$SourceRoot = 'source code'",
            "[string]$GameRoot = 'E:\\Steam\\steamapps\\common\\Slay the Spire 2'",
            "[string]$GodotExe",
            "[switch]$FailOnMismatch",
            "source_version_matches_installed_game",
            "Severity",
            "Warnings",
            "source_root_is_git_ignored",
            "source_root_has_no_tracked_files",
            "godot_open_command_prepared",
            "gdre_log_recovery_finished",
            "gdre_log_engine_version_godot_451",
            "ritsulib_manifest_exists",
            "ritsulib_variant_matches_installed_game",
            "ritsulib_variant_dll_exists",
            "ritsulib_viewer_exists",
            "RitsuLib viewer exists; it is a log viewer, not an unpacker or monkey runner",
            "OpenProjectCommand",
            "EvidenceUsePolicy",
            "NotRuntimeProof",
            "AuthorizedLocalInstallOnly",
            "ThirdPartyDumpsProhibited",
            "OriginalGameSourceMustNotBeTracked",
            "RefreshSourceSnapshotBeforeCurrentApiClaims",
            "if ($FailOnMismatch -and $mismatches.Count -gt 0)");

        Assert.DoesNotContain("Start-Process", checker, StringComparison.Ordinal);
        Assert.DoesNotContain("--editor", checker, StringComparison.Ordinal);
        Assert.DoesNotContain("& dotnet", checker, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("dotnet build", checker, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("dotnet test", checker, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("dotnet publish", checker, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FullLocalCiDoesNotAddRuntimeMonkeyAsDefaultLiveLane()
    {
        var ci = ReadRepoText("scripts", "ci-full-validation.ps1");

        Assert.DoesNotContain("run-spire-plus-monkey-stability.ps1", ci, StringComparison.Ordinal);
        Assert.DoesNotContain("check-spire-plus-runtime-monkey-packet.ps1", ci, StringComparison.Ordinal);
        Assert.DoesNotContain("monkey-stability", ci, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WorktreeBatchClassifierIncludesRuntimeMonkeySurfaces()
    {
        var classifier = ReadRepoText("scripts", "report-worktree-batches.ps1");
        var batch5Classifier = SliceBetween(
            classifier,
            "if ($p.StartsWith('scripts/'",
            "return 5");

        Assert.Contains("RuntimeMonkey", batch5Classifier, StringComparison.Ordinal);
        Assert.Contains("$p -eq 'docs/testing'", batch5Classifier, StringComparison.Ordinal);
        Assert.Contains("$p.StartsWith('docs/testing/'", batch5Classifier, StringComparison.Ordinal);
        Assert.DoesNotContain("$p.StartsWith('docs/', [System.StringComparison]", classifier, StringComparison.Ordinal);
    }
}
