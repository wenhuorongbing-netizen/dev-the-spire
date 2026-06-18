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
            "Get-CanonicalCommandAckPattern",
            "plan_unresponsive_sample_threshold_positive",
            "RuntimeProbeSamplesPath",
            "CurrentIterationLogPath",
            "LogScanOffsetBytes",
            "runtime_probe_samples_under_iteration_dir",
            "current_iteration_log_under_iteration_dir",
            "runtime_probe_samples_leaf_expected",
            "current_iteration_log_leaf_expected",
            "runtime_probe_samples_path_matches_retained_file",
            "current_iteration_log_path_matches_retained_file",
            "log_scan_offset_recorded",
            "log_scan_offset_within_full_log",
            "current_iteration_log_matches_scan_offset",
            "command_ack_required_matches_pattern",
            "command_ack_pattern_present_when_required",
            "command_ack_pattern_matches_canonical_command",
            "command_ack_pattern_matches_current_iteration_log",
            "iteration_number_matches_directory",
            "scenario_present",
            "plan_entry_exists",
            "summary_result_exists",
            "scenario_matches_plan",
            "command_matches_plan",
            "command_index_matches_plan",
            "command_selection_mode_matches_plan",
            "scenario_tag_matches_plan",
            "owner_area_matches_plan",
            "command_ack_pattern_matches_plan",
            "summary_result_scenario_matches_iteration",
            "summary_result_command_matches_iteration",
            "summary_result_command_selection_mode_matches_iteration",
            "summary_result_scenario_tag_matches_iteration",
            "summary_result_owner_area_matches_iteration",
            "summary_result_command_ack_pattern_matches_iteration",
            "summary_result_command_ack_required_matches_iteration",
            "summary_result_passed_matches_iteration",
            "summary_result_command_ack_observed_matches_iteration",
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
    public void RuntimeMonkeyPacketCheckerRejectsResultPathsThatDoNotPointToRetainedFiles()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: true);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_current_iteration_log_path_matches_retained_file status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_path_matches_retained_file status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    [Fact]
    public void RuntimeMonkeyPacketCheckerRejectsIterationResultsThatDoNotMatchPlanOrSummary()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var resultPath = Path.Combine(workdir, "iteration-0001", "iteration-result.json");
            var resultJson = File.ReadAllText(resultPath)
                .Replace("VAKUU", "URDA", StringComparison.Ordinal)
                .Replace("\"Scenario\": \"VakuuFightSmoke\"", "\"Scenario\": \"AncientUiPlusVakuuFight\"", StringComparison.Ordinal)
                .Replace("\"CommandSelectionMode\": \"RoundRobin\"", "\"CommandSelectionMode\": \"Random\"", StringComparison.Ordinal)
                .Replace("\"CommandIndex\": 0", "\"CommandIndex\": 99", StringComparison.Ordinal)
                .Replace("\"ScenarioTag\": \"vakuu-fight\"", "\"ScenarioTag\": \"ancient-ui\"", StringComparison.Ordinal)
                .Replace("\"OwnerArea\": \"Ancients.Vakuu.ChildCombatResume\"", "\"OwnerArea\": \"Ancients.Urda.MapSaveState\"", StringComparison.Ordinal)
                .Replace("fight_option_shown", "other_ack", StringComparison.Ordinal);
            File.WriteAllText(resultPath, resultJson);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_scenario_matches_plan status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_command_matches_plan status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_command_index_matches_plan status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_command_selection_mode_matches_plan status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_scenario_tag_matches_plan status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_owner_area_matches_plan status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_command_ack_pattern_matches_plan status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_scenario_matches_iteration status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_command_matches_iteration status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_command_selection_mode_matches_iteration status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_scenario_tag_matches_iteration status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_owner_area_matches_iteration status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_command_ack_pattern_matches_iteration status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    [Fact]
    public void RuntimeMonkeyPacketCheckerRejectsCurrentIterationLogsThatDoNotMatchScanOffset()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            File.AppendAllText(Path.Combine(workdir, "iteration-0001", "godot.log.current-iteration"), $"{Environment.NewLine}stale appended slice content");

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_current_iteration_log_matches_scan_offset status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    [Fact]
    public void RuntimeMonkeyPacketCheckerRejectsLogScanOffsetsOutsideTheFullLog()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var resultPath = Path.Combine(workdir, "iteration-0001", "iteration-result.json");
            var resultJson = File.ReadAllText(resultPath)
                .Replace("\"LogScanOffsetBytes\": 0", "\"LogScanOffsetBytes\": 999999", StringComparison.Ordinal);
            File.WriteAllText(resultPath, resultJson);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_log_scan_offset_within_full_log status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    [Fact]
    public void RuntimeMonkeyPacketCheckerRejectsCommandAckClaimsWithoutRetainedLogMatch()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            foreach (var logPath in new[]
                     {
                         Path.Combine(iterationDir, "godot.log.current-iteration"),
                         Path.Combine(iterationDir, "godot.log.after-launch")
                     })
            {
                var log = File.ReadAllText(logPath)
                    .Replace(
                        "[SPIREPLUS-EVIDENCE] VakuuFight fight_option_shown",
                        "[Spire Plus] Different line without expected command acknowledgement.",
                        StringComparison.Ordinal);
                File.WriteAllText(logPath, log);
            }

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_command_ack_pattern_matches_current_iteration_log status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    [Fact]
    public void RuntimeMonkeyPacketCheckerRejectsDowngradedVakuuFightAckPattern()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            foreach (var jsonPath in new[]
                     {
                         Path.Combine(workdir, "monkey-plan.json"),
                         Path.Combine(workdir, "monkey-summary.json"),
                         Path.Combine(workdir, "iteration-0001", "iteration-result.json")
                     })
            {
                var json = File.ReadAllText(jsonPath)
                    .Replace("SPIREPLUS-EVIDENCE", "Spire Plus", StringComparison.Ordinal);
                File.WriteAllText(jsonPath, json);
            }

            var iterationDir = Path.Combine(workdir, "iteration-0001");
            foreach (var logPath in new[]
                     {
                         Path.Combine(iterationDir, "godot.log.current-iteration"),
                         Path.Combine(iterationDir, "godot.log.after-launch")
                     })
            {
                var log = File.ReadAllText(logPath)
                    .Replace(
                        "[SPIREPLUS-EVIDENCE]",
                        "[Spire Plus]",
                        StringComparison.Ordinal);
                File.WriteAllText(logPath, log);
            }

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_command_ack_pattern_matches_canonical_command status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("mismatches=1", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    [Fact]
    public void RuntimeMonkeyPacketCheckerRejectsCommandAckRequiredPatternDrift()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var resultPath = Path.Combine(workdir, "iteration-0001", "iteration-result.json");
            var resultJson = File.ReadAllText(resultPath)
                .Replace("\"CommandAckRequired\": true", "\"CommandAckRequired\": false", StringComparison.Ordinal);
            File.WriteAllText(resultPath, resultJson);

            var summaryPath = Path.Combine(workdir, "monkey-summary.json");
            var summaryJson = File.ReadAllText(summaryPath)
                .Replace("\"CommandAckRequired\": true", "\"CommandAckRequired\": false", StringComparison.Ordinal);
            File.WriteAllText(summaryPath, summaryJson);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_command_ack_required_matches_pattern status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
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
            "LogScanOffsetBytes",
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
            "current_iteration_log_slice_mismatch",
            "current_iteration_log_scan_offset_invalid",
            "iteration_result_missing_or_invalid",
            "process_unresponsive",
            "command_ack_missing",
            "Read-TextAfterByteOffset",
            "Normalize-LogSliceForComparison",
            "MissingFieldException",
            "MissingMethodException",
            "TypeLoadException",
            "runtime expectation",
            "Registered act event",
            "PreviewTransform",
            "PreviewCrystalSphere",
            "Spire Plus\\] Preview",
            "Transform prediction",
            "Crystal Sphere peek",
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
    public void RuntimeFailureAnalyzerRoutesLogDerivedOwnersBeforePlannedOwners()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteIteration(
                workdir,
                1,
                "spireplus_test_ancient VAKUU confirm fight",
                "vakuu-fight",
                "Ancients.Vakuu.ChildCombatResume",
                """["process_unresponsive"]""",
                """["process_unresponsive"]""",
                "[INFO] [Patcher - SpirePlus] Patch application complete:\r\n[ERROR] TypeLoadException stale full log should not own current iteration",
                "[ERROR] Spire Plus error in StS1 AdditiveBatch1 Registered act event Golden Idol",
                """{"SignatureHits":[{"Name":"Spire Plus error/exception","Count":1}]}""");
            WriteIteration(
                workdir,
                2,
                "spireplus_test_ancient VAKUU confirm fight",
                "vakuu-fight",
                "Ancients.Vakuu.FightOptionSetup",
                """["command_ack_missing"]""",
                """[]""",
                "[ERROR] TypeLoadException stale full log should not own current iteration",
                "[SPIREPLUS-EVIDENCE] StS1 AdditiveBatch1 Registered act event Golden Idol",
                """{"SignatureHits":[]}""");
            WriteIteration(
                workdir,
                3,
                "spireplus_test_ancient URDA confirm",
                "ancient-ui",
                "Ancients.Urda.MapSaveState",
                """["process_unresponsive"]""",
                """["process_unresponsive"]""",
                "[ERROR] TypeLoadException stale full log should not own current iteration",
                "[SPIREPLUS-EVIDENCE] PreviewTransform [Spire Plus] Preview: Transform prediction prediction_prepared_multiplayer_ui_only PreviewCrystalSphere Crystal Sphere peek",
                """{"SignatureHits":[]}""");
            WriteIteration(
                workdir,
                4,
                "spireplus_test_ancient MORVI confirm",
                "ancient-ui",
                "Ancients.Morvi.CardPlayState",
                """["process_unresponsive"]""",
                """["process_unresponsive"]""",
                "[ERROR] TypeLoadException stale full log should not own current iteration",
                "[SPIREPLUS-EVIDENCE] ALLOW_UNVERIFIED_COOP coop_override_enabled multiplayer",
                """{"SignatureHits":[]}""");
            WriteIteration(
                workdir,
                5,
                "spireplus_test_ancient URDA confirm",
                "ancient-ui",
                "Runtime.Unknown",
                """["process_unresponsive"]""",
                """["process_unresponsive"]""",
                "[ERROR] TypeLoadException stale full log should not own current iteration",
                "[SPIREPLUS-EVIDENCE] Root Sight Unknown map preview hover",
                """{"SignatureHits":[]}""");
            WriteIteration(
                workdir,
                6,
                "spireplus_test_ancient VAKUU confirm fight",
                "vakuu-fight",
                "Runtime.Unknown",
                """["command_ack_missing"]""",
                """[]""",
                "[ERROR] TypeLoadException stale full log should not own current iteration",
                "[SPIREPLUS-EVIDENCE] StS1 AdditiveBatch1 Registered act event Golden Idol",
                """{"SignatureHits":[]}""");
            WriteMonkeySummary(workdir, 1, 2, 3, 4, 5, 6);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration1 = FindIteration(root, 1);
            var iteration2 = FindIteration(root, 2);
            var iteration3 = FindIteration(root, 3);
            var iteration4 = FindIteration(root, 4);
            var iteration5 = FindIteration(root, 5);
            var iteration6 = FindIteration(root, 6);

            Assert.Equal("Sts1Events", iteration1.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("Ancients.Vakuu.FightOptionSetup", iteration1.GetProperty("OwnerAreaFromCommand").GetString());
            Assert.Equal("Sts1Events", FindFindingOwner(iteration1, "process_unresponsive"));
            Assert.Equal("Sts1Events", FindFindingOwner(iteration1, "audit:Spire Plus error/exception"));
            Assert.Equal("Ancients.Vakuu.ChildCombatResume", FindFindingOwner(iteration1, "vakuu_command_failed_or_hung"));
            Assert.Equal("Ancients.Vakuu.FightOptionSetup", FindFindingOwner(iteration2, "command_ack_missing"));
            Assert.Equal("Ancients.Vakuu.FightOptionSetup", FindFindingOwner(iteration2, "vakuu_command_failed_or_hung"));
            Assert.Equal("PreviewTools", iteration3.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("PreviewTools", FindFindingOwner(iteration3, "process_unresponsive"));
            Assert.Equal("MultiplayerPolicy", iteration4.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("MultiplayerPolicy", FindFindingOwner(iteration4, "process_unresponsive"));
            Assert.Equal("MultiplayerPolicy", FindFindingOwner(iteration4, "coop_override_enabled_runtime_failure"));
            Assert.Equal("Ancients.Urda.MapSaveState", iteration5.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("Ancients.Urda.MapSaveState", FindFindingOwner(iteration5, "process_unresponsive"));
            Assert.Equal("Sts1Events", iteration6.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("Ancients.Vakuu.FightOptionSetup", iteration6.GetProperty("OwnerAreaFromCommand").GetString());
            Assert.Equal("Ancients.Vakuu.FightOptionSetup", FindFindingOwner(iteration6, "command_ack_missing"));
            Assert.Equal("Ancients.Vakuu.FightOptionSetup", FindFindingOwner(iteration6, "vakuu_command_failed_or_hung"));
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    [Fact]
    public void RuntimeFailureAnalyzerReportsMismatchedCurrentIterationSlice()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            Directory.CreateDirectory(iterationDir);
            const string prefix = "[INFO] stale full-log prefix before accepted scan offset\r\n";
            const string actualSlice = "[ERROR] TypeLoadException actual current slice owner\r\n";
            const string staleCurrentSlice = "[SPIREPLUS-EVIDENCE] StS1 AdditiveBatch1 Registered act event Golden Idol\r\n";
            var offset = System.Text.Encoding.UTF8.GetByteCount(prefix);

            File.WriteAllText(
                Path.Combine(iterationDir, "iteration-result.json"),
                $$"""
                {
                  "Iteration": 1,
                  "Passed": false,
                  "Command": "spireplus_test_ancient URDA confirm",
                  "ScenarioTag": "ancient-ui",
                  "OwnerArea": "Runtime.Unknown",
                  "LogScanOffsetBytes": {{offset}},
                  "FailureReasonCodes": ["process_unresponsive"],
                  "HangSignals": ["process_unresponsive"]
                }
                """);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), prefix + actualSlice);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.current-iteration"), staleCurrentSlice);
            File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), """{"SignatureHits":[]}""");
            WriteMonkeySummary(workdir, 1);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var iteration = FindIteration(document.RootElement, 1);
            var mismatchFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "current_iteration_log_slice_mismatch");

            Assert.Equal("PackageRuntimeDrift", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("RuntimeHarness", mismatchFinding.GetProperty("OwnerArea").GetString());
            Assert.Equal("blocking", mismatchFinding.GetProperty("Severity").GetString());
            Assert.Equal("PackageRuntimeDrift", FindFindingOwner(iteration, "process_unresponsive"));
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    [Fact]
    public void RuntimeFailureAnalyzerReportsMissingIterationResultEvenWithSummaryFallback()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            Directory.CreateDirectory(iterationDir);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), "[Startup] Time to main menu\r\n");
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.current-iteration"), "[Startup] Time to main menu\r\n");
            File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), """{"SignatureHits":[]}""");
            File.WriteAllText(
                Path.Combine(workdir, "monkey-summary.json"),
                """
                {
                  "FailedIterationIds": [1],
                  "Results": [
                    {
                      "Iteration": 1,
                      "Passed": false,
                      "Command": "spireplus_test_ancient URDA confirm",
                      "ScenarioTag": "ancient-ui",
                      "OwnerArea": "Ancients.Urda.MapSaveState",
                      "FailureReasonCodes": ["process_unresponsive"],
                      "HangSignals": ["process_unresponsive"]
                    }
                  ]
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var iteration = FindIteration(document.RootElement, 1);
            var missingResultFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "iteration_result_missing_or_invalid");

            Assert.Equal("Ancients.Urda.MapSaveState", iteration.GetProperty("OwnerAreaHint").GetString());
            Assert.Equal("RuntimeHarness", missingResultFinding.GetProperty("OwnerArea").GetString());
            Assert.Equal("blocking", missingResultFinding.GetProperty("Severity").GetString());
            Assert.Equal("Ancients.Urda.MapSaveState", FindFindingOwner(iteration, "process_unresponsive"));
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
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

    private static void WriteIteration(
        string evidenceRoot,
        int iteration,
        string command,
        string scenarioTag,
        string ownerArea,
        string failureReasonCodesJson,
        string hangSignalsJson,
        string fullLog,
        string currentLog,
        string auditJson)
    {
        var iterationDir = Path.Combine(evidenceRoot, $"iteration-{iteration:D4}");
        Directory.CreateDirectory(iterationDir);
        File.WriteAllText(
            Path.Combine(iterationDir, "iteration-result.json"),
            $$"""
            {
              "Iteration": {{iteration}},
              "Passed": false,
              "Command": {{JsonSerializer.Serialize(command)}},
              "ScenarioTag": {{JsonSerializer.Serialize(scenarioTag)}},
              "OwnerArea": {{JsonSerializer.Serialize(ownerArea)}},
              "FailureReasonCodes": {{failureReasonCodesJson}},
              "HangSignals": {{hangSignalsJson}}
            }
            """);
        File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), fullLog);
        File.WriteAllText(Path.Combine(iterationDir, "godot.log.current-iteration"), currentLog);
        File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), auditJson);
    }

    private static void WriteMonkeySummary(string evidenceRoot, params int[] failedIterations)
    {
        File.WriteAllText(
            Path.Combine(evidenceRoot, "monkey-summary.json"),
            $$"""
            {
              "FailedIterationIds": [{{string.Join(", ", failedIterations)}}],
              "Results": []
            }
            """);
    }

    private static void WriteCleanRuntimeMonkeyPacket(string evidenceRoot, bool useShadowResultPaths)
    {
        const string command = "spireplus_test_ancient VAKUU confirm fight";
        const string scenarioTag = "vakuu-fight";
        const string ownerArea = "Ancients.Vakuu.ChildCombatResume";
        const string ackPattern = "\\[SPIREPLUS-EVIDENCE\\]\\s+VakuuFight\\s+fight_option_shown\\b";

        var iterationDir = Path.Combine(evidenceRoot, "iteration-0001");
        var shadowDir = Path.Combine(iterationDir, "shadow");
        Directory.CreateDirectory(iterationDir);
        Directory.CreateDirectory(shadowDir);

        var retainedCurrentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
        var retainedProbeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
        var resultCurrentLogPath = useShadowResultPaths ? Path.Combine(shadowDir, "godot.log.current-iteration") : retainedCurrentLogPath;
        var resultProbeSamplesPath = useShadowResultPaths ? Path.Combine(shadowDir, "runtime-probe-samples.json") : retainedProbeSamplesPath;
        var currentLog = """
            [Startup] Time to main menu
            [INFO] [EZMicroBalance] [Patcher - SpirePlus] Patch application complete: 25 applied, 0 ignored, 0 failed, 25 total
            [INFO] [EZMicroBalance] ModPatcher applied 25 patches (25 registered).
            v0.1.0-private-beta.86
            [SPIREPLUS-EVIDENCE] VakuuFight fight_option_shown
            """;
        var probeSamples = """[{"ProcessObserved":true,"HungWindow":false,"Responding":true}]""";

        File.WriteAllText(retainedCurrentLogPath, currentLog);
        File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), currentLog);
        File.WriteAllText(retainedProbeSamplesPath, probeSamples);
        File.WriteAllText(Path.Combine(shadowDir, "godot.log.current-iteration"), currentLog);
        File.WriteAllText(Path.Combine(shadowDir, "runtime-probe-samples.json"), probeSamples);
        File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), """{"Clean":true,"SignatureHits":[]}""");
        File.WriteAllText(Path.Combine(iterationDir, "sts1-mode-log-check.json"), """{"Mismatches":[],"Checks":[{"Passed":true}]}""");

        File.WriteAllText(
            Path.Combine(evidenceRoot, "monkey-plan.json"),
            $$"""
            {
              "HangProbeSchemaVersion": 1,
              "Launch": true,
              "Iterations": 1,
              "Scenario": "VakuuFightSmoke",
              "CommandSelectionMode": "RoundRobin",
              "CommandCorpusSource": "scenario:VakuuFightSmoke",
              "ObservationIntervalSeconds": 2,
              "UnresponsiveSampleThreshold": 3,
              "NoLogGrowthTimeoutSeconds": 90,
              "ProcessProbe": { "ProcessName": "SlayTheSpire2", "FailsOnlyAfterConsecutiveUnresponsiveSamples": true },
              "LogGrowthProbe": { "StartupFailsOnNoGrowth": true },
              "CommandCorpus": [{{JsonSerializer.Serialize(command)}}],
              "PlannedCommandCounts": { {{JsonSerializer.Serialize(command)}}: 1 },
              "PlannedScenarioTagCounts": { {{JsonSerializer.Serialize(scenarioTag)}}: 1 },
              "PlannedOwnerAreaCounts": { {{JsonSerializer.Serialize(ownerArea)}}: 1 },
              "PlannedVakuuFightIterationCount": 1,
              "CommandScenarioMatrix": [
                { "Command": {{JsonSerializer.Serialize(command)}}, "ScenarioTag": {{JsonSerializer.Serialize(scenarioTag)}}, "OwnerArea": {{JsonSerializer.Serialize(ownerArea)}}, "CommandAckPattern": {{JsonSerializer.Serialize(ackPattern)}} }
              ],
              "CommandAckPatterns": [
                { "Command": {{JsonSerializer.Serialize(command)}}, "ScenarioTag": {{JsonSerializer.Serialize(scenarioTag)}}, "OwnerArea": {{JsonSerializer.Serialize(ownerArea)}}, "Pattern": {{JsonSerializer.Serialize(ackPattern)}} }
              ],
              "PlannedCommands": [
                { "Iteration": 1, "Command": {{JsonSerializer.Serialize(command)}}, "CommandIndex": 0, "CommandSelectionMode": "RoundRobin", "ScenarioTag": {{JsonSerializer.Serialize(scenarioTag)}}, "OwnerArea": {{JsonSerializer.Serialize(ownerArea)}}, "CommandAckPattern": {{JsonSerializer.Serialize(ackPattern)}} }
              ]
            }
            """);

        File.WriteAllText(
            Path.Combine(evidenceRoot, "monkey-summary.json"),
            $$"""
            {
              "HangProbeSchemaVersion": 1,
              "Passed": true,
              "RequestedIterations": 1,
              "CompletedIterations": 1,
              "FailedIterations": 0,
              "FailedIterationIds": [],
              "FailureReasonCounts": {},
              "ProcessExitCount": 0,
              "MainWindowMissingCount": 0,
              "CurrentIterationLogMissingCount": 0,
              "UnresponsiveIterationCount": 0,
              "LogStallIterationCount": 0,
              "CommandAckMissingCount": 0,
              "CommandCounts": { {{JsonSerializer.Serialize(command)}}: 1 },
              "ScenarioTagCounts": { {{JsonSerializer.Serialize(scenarioTag)}}: 1 },
              "OwnerAreaCounts": { {{JsonSerializer.Serialize(ownerArea)}}: 1 },
              "VakuuFightIterationCount": 1,
              "MaxConsecutiveUnresponsiveSamples": 0,
              "Results": [
                { "Iteration": 1, "Passed": true, "Scenario": "VakuuFightSmoke", "CommandSelectionMode": "RoundRobin", "Command": {{JsonSerializer.Serialize(command)}}, "ScenarioTag": {{JsonSerializer.Serialize(scenarioTag)}}, "OwnerArea": {{JsonSerializer.Serialize(ownerArea)}}, "CommandAckPattern": {{JsonSerializer.Serialize(ackPattern)}}, "CommandAckRequired": true, "CommandAckObserved": true }
              ]
            }
            """);

        File.WriteAllText(
            Path.Combine(iterationDir, "iteration-result.json"),
            $$"""
            {
              "HangProbeSchemaVersion": 1,
              "Iteration": 1,
              "Scenario": "VakuuFightSmoke",
              "ScenarioTag": {{JsonSerializer.Serialize(scenarioTag)}},
              "OwnerArea": {{JsonSerializer.Serialize(ownerArea)}},
              "CommandSelectionMode": "RoundRobin",
              "Command": {{JsonSerializer.Serialize(command)}},
              "CommandIndex": 0,
              "CommandAckPattern": {{JsonSerializer.Serialize(ackPattern)}},
              "CommandAckRequired": true,
              "Passed": true,
              "MainMenuReached": true,
              "MainMenuObservationPassed": true,
              "RuntimeObservationPassed": true,
              "StartupLogProbePassed": true,
              "PostCommandLogProbePassed": true,
              "ResponsivenessProbePassed": true,
              "CommandAckObserved": true,
              "FailureReasonCodes": [],
              "HangSignals": [],
              "GameProcessId": 1234,
              "MainWindowObserved": true,
              "MainMenuElapsedSeconds": 12.3,
              "MaxSecondsWithoutLogGrowth": 1,
              "MaxConsecutiveUnresponsiveSamples": 0,
              "LogCopied": true,
              "CurrentIterationLogCopied": true,
              "AuditClean": true,
              "ExpectationPassed": true,
              "Sts1ModeVerifierPassed": true,
              "RestoreSucceeded": true,
              "RuntimeProbeSamplesPath": {{JsonSerializer.Serialize(resultProbeSamplesPath)}},
              "CurrentIterationLogPath": {{JsonSerializer.Serialize(resultCurrentLogPath)}},
              "LogScanOffsetBytes": 0,
              "MainMenuObservation": {
                "MainMenuReached": true,
                "ProcessObserved": true,
                "ProcessExitedAfterObservation": false,
                "HungWindowDetected": false,
                "NoLogGrowthTimeoutExceeded": false,
                "LogObserved": true,
                "Passed": true,
                "MaxConsecutiveUnresponsiveSamples": 0
              },
              "RuntimeObservation": {
                "MainMenuReached": true,
                "ProcessObserved": true,
                "ProcessExitedAfterObservation": false,
                "HungWindowDetected": false,
                "NoLogGrowthTimeoutExceeded": false,
                "LogObserved": true,
                "Passed": true,
                "MaxConsecutiveUnresponsiveSamples": 0
              }
            }
            """);
    }

    private static JsonElement FindIteration(JsonElement report, int iteration)
    {
        return report
            .GetProperty("Iterations")
            .EnumerateArray()
            .Single(item => item.GetProperty("Iteration").GetInt32() == iteration);
    }

    private static string FindFindingOwner(JsonElement iteration, string signal)
    {
        return iteration
            .GetProperty("Findings")
            .EnumerateArray()
            .Single(item => item.GetProperty("Signal").GetString() == signal)
            .GetProperty("OwnerArea")
            .GetString()!;
    }

    private static (int ExitCode, string Output, string Error) RunPowerShell(string scriptPath, params string[] arguments)
    {
        var executable = OperatingSystem.IsWindows() ? "powershell.exe" : "pwsh";
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-ExecutionPolicy");
        startInfo.ArgumentList.Add("Bypass");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(scriptPath);
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000), $"Timed out running {scriptPath}.");
        return (process.ExitCode, output, error);
    }
}
