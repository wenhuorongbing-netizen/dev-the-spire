using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
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
            "Get-Process -Name SlayTheSpire2",
            "$preExistingProcesses.Count",
            "pre-existing SlayTheSpire2 process(es) before launch",
            "} else {",
            "StaleProcessObserved",
            "StaleProcessCount",
            "MaxConsecutiveUnresponsiveSamples",
            "shared godot.log cannot be trusted for this iteration",
            "CommandAckPatterns",
            "check-local-godot-source-workspace.ps1",
            "SourceWorkspaceCheckPath",
            "SourceWorkspaceCheckSha256",
            "RefreshSourceSnapshotBeforeCurrentApiClaims",
            "RequireCurrentSourceSnapshot",
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
            "stale_process_observed",
            "current_iteration_log_missing",
            "command_ack_missing",
            "FailedIterationIds",
            "FailureReasonCounts",
            "ProcessExitCount",
            "MainWindowMissingCount",
            "CurrentIterationLogMissingCount",
            "UnresponsiveIterationCount",
            "StaleProcessObservedCount",
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
            "Get-CanonicalCommandOwnerArea",
            "Get-CanonicalCommandScenarioTag",
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
            "runtime_probe_samples_process_observed_field_present",
            "runtime_probe_samples_hung_window_field_present",
            "runtime_probe_samples_responding_field_present",
            "runtime_probe_samples_stale_process_count_field_present",
            "log_scan_offset_recorded",
            "log_scan_offset_within_full_log",
            "current_iteration_log_matches_scan_offset",
            "command_ack_required_matches_pattern",
            "command_ack_required_for_canonical_command",
            "command_ack_pattern_present_when_required",
            "command_ack_pattern_matches_canonical_command",
            "command_ack_pattern_matches_current_iteration_log",
            "scenario_tag_matches_canonical_command",
            "owner_area_matches_canonical_command",
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
            "runtime_probe_samples_no_stale_processes",
            "plan_launch_true",
            "plan_scenario_present",
            "plan_command_selection_mode_present",
            "plan_command_corpus_source_present",
            "plan_source_workspace_check_path_present",
            "plan_source_workspace_check_under_evidence_dir",
            "plan_source_workspace_check_exists",
            "plan_source_workspace_check_hash_present",
            "plan_source_workspace_check_hash_matches",
            "plan_source_workspace_summary_present",
            "plan_command_scenario_matrix_present",
            "summary_passed",
            "summary_failed_iterations_zero",
            "summary_process_exit_count_zero",
            "summary_main_window_missing_count_zero",
            "summary_current_iteration_log_missing_count_zero",
            "summary_unresponsive_iteration_count_zero",
            "summary_stale_process_observed_count_zero",
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
            "audit_has_single_scanned_path",
            "audit_path_matches_current_iteration_log",
            "audit_has_single_length",
            "audit_length_matches_current_iteration_log",
            "audit_has_single_sha256",
            "audit_sha256_matches_current_iteration_log",
            "audit_recomputed_from_current_iteration_log",
            "audit_recomputed_clean",
            "audit_signature_counts_match_recomputed",
            "audit_sha256_matches_recomputed",
            "sts1-mode-log-check.json",
            "sts1_mode_log_check_exists",
            "sts1_mode_log_check_mismatches_empty",
            "sts1_mode_log_check_mode_matches_plan",
            "sts1_mode_log_check_log_path_matches_current_iteration_log",
            "sts1_mode_log_check_log_length_matches_current_iteration_log",
            "sts1_mode_log_check_log_sha256_matches_current_iteration_log",
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
    public void RuntimeMonkeyRunnerChecksForPreExistingProcessesBeforeBaselineLog()
    {
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");

        AssertSourceContains(
            runner,
            "$preExistingProcesses = @(Get-Process -Name SlayTheSpire2",
            "$preExistingProcesses.Count -gt 0",
            "pre-existing SlayTheSpire2 process(es) before launch",
            "$preLaunchLog = Get-LogSnapshot -Path $godotLogPath",
            "& $liveSessionScript @prepareArgs");
        AssertBefore(runner, "$preExistingProcesses = @(Get-Process -Name SlayTheSpire2", "$preLaunchLog = Get-LogSnapshot -Path $godotLogPath");
        AssertBefore(runner, "$preExistingProcesses = @(Get-Process -Name SlayTheSpire2", "& $liveSessionScript @prepareArgs");
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
                .Replace("\"OwnerArea\": \"Ancients.Vakuu.FightOptionSetup\"", "\"OwnerArea\": \"Runtime.Unknown\"", StringComparison.Ordinal)
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
            Assert.Contains("iteration-0001_scenario_tag_matches_canonical_command status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_owner_area_matches_canonical_command status=fail", result.Output, StringComparison.Ordinal);
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
            Assert.DoesNotContain("mismatches=0", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsKnownCommandsWithoutCanonicalAckMetadata()
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
                var json = ClearCommandAckPatternJsonValues(File.ReadAllText(jsonPath))
                    .Replace("\"CommandAckRequired\": true", "\"CommandAckRequired\": false", StringComparison.Ordinal);
                File.WriteAllText(jsonPath, json);
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
            Assert.Contains("iteration-0001_command_ack_required_for_canonical_command status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_command_ack_pattern_present_when_required status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_command_ack_pattern_matches_canonical_command status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsKnownCommandsWithSelfConsistentWrongOwner()
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
                    .Replace("Ancients.Vakuu.FightOptionSetup", "Ancients.Vakuu.ChildCombatResume", StringComparison.Ordinal);
                File.WriteAllText(jsonPath, json);
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
            Assert.Contains("iteration-0001_owner_area_matches_canonical_command status=fail", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("mismatches=0", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsStalePreExistingProcesses()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            foreach (var jsonPath in new[]
                     {
                         Path.Combine(workdir, "monkey-summary.json"),
                         Path.Combine(workdir, "iteration-0001", "iteration-result.json"),
                         Path.Combine(workdir, "iteration-0001", "runtime-probe-samples.json")
                     })
            {
                var json = File.ReadAllText(jsonPath)
                    .Replace("\"StaleProcessObservedCount\": 0", "\"StaleProcessObservedCount\": 1", StringComparison.Ordinal)
                    .Replace("\"StaleProcessObserved\": false", "\"StaleProcessObserved\": true", StringComparison.Ordinal)
                    .Replace("\"StaleProcessCount\": 0", "\"StaleProcessCount\": 1", StringComparison.Ordinal)
                    .Replace("\"StaleProcessCount\":0", "\"StaleProcessCount\":1", StringComparison.Ordinal)
                    .Replace("\"MaxStaleProcessCount\": 0", "\"MaxStaleProcessCount\": 1", StringComparison.Ordinal);
                File.WriteAllText(jsonPath, json);
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
            Assert.Contains("summary_stale_process_observed_count_zero status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_stale_process_observed_false status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_stale_process_count_zero status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_no_stale_processes status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_main_menu_observation_no_stale_process status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_observation_no_stale_process status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsProbeSamplesMissingFreezeDetectionFields()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            File.WriteAllText(
                Path.Combine(workdir, "iteration-0001", "runtime-probe-samples.json"),
                """[{"ProcessObserved":true,"StaleProcessCount":0}]""");

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_runtime_probe_samples_hung_window_field_present status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_responding_field_present status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsSourceWorkspaceReportHashDrift()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            File.AppendAllText(
                Path.Combine(workdir, "local-godot-source-workspace-check.json"),
                $"{Environment.NewLine} ");

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("plan_source_workspace_check_hash_matches status=fail", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("mismatches=0", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsSourceWorkspaceReportThatDoesNotMatchPlanSummary()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var sourceWorkspaceCheckPath = Path.Combine(workdir, "local-godot-source-workspace-check.json");
            File.WriteAllText(
                sourceWorkspaceCheckPath,
                """
                {
                  "Passed": false,
                  "SourceRoot": "D:\\Game\\FOTN\\dev-the-spire\\source code",
                  "Game": { "Version": "v0.107.0", "Commit": "fixture" },
                  "RecoveredSource": {
                    "Version": "v0.106.1",
                    "Commit": "stale-fixture",
                    "MatchesInstalledGame": false,
                    "Disposition": "stale-source-snapshot"
                  },
                  "EvidenceUsePolicy": {
                    "NotRuntimeProof": true,
                    "LocalSourceReferenceOnly": true,
                    "AuthorizedLocalInstallOnly": true,
                    "ThirdPartyDumpsProhibited": true,
                    "RefreshSourceSnapshotBeforeCurrentApiClaims": true
                  },
                  "Mismatches": ["source_version_matches_installed_game: source version=v0.106.1 installed version=v0.107.0"]
                }
                """);

            var updatedHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(sourceWorkspaceCheckPath))).ToLowerInvariant();
            var planPath = Path.Combine(workdir, "monkey-plan.json");
            var planJson = File.ReadAllText(planPath);
            var originalHash = Regex.Match(planJson, """
                "SourceWorkspaceCheckSha256":\s*"(?<hash>[^"]+)"
                """, RegexOptions.IgnorePatternWhitespace).Groups["hash"].Value;
            Assert.False(string.IsNullOrWhiteSpace(originalHash));
            File.WriteAllText(planPath, planJson.Replace(originalHash, updatedHash, StringComparison.Ordinal));

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("plan_source_workspace_check_hash_matches status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("plan_source_workspace_report_passed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("plan_source_workspace_report_mismatches_empty status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("plan_source_workspace_report_matches_summary status=fail", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("mismatches=0", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsAuditJsonNotBoundToCurrentIterationLog()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var fullLogPath = Path.Combine(iterationDir, "godot.log.after-launch");
            File.WriteAllText(
                Path.Combine(iterationDir, "godot-log-audit.json"),
                $$"""
                [
                  {
                    "Path": {{JsonSerializer.Serialize(fullLogPath)}},
                    "Clean": true,
                    "SignatureHits": []
                  }
                ]
                """);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_path_matches_current_iteration_log status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRecomputesAuditFromCurrentIterationLog()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var currentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
            var fullLogPath = Path.Combine(iterationDir, "godot.log.after-launch");
            var dirtyLog = File.ReadAllText(currentLogPath) + "[ERROR] TypeLoadException\r\n";
            File.WriteAllText(currentLogPath, dirtyLog);
            File.WriteAllText(fullLogPath, dirtyLog);
            var dirtyLogLength = new FileInfo(currentLogPath).Length;
            var dirtyLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(currentLogPath))).ToLowerInvariant();

            File.WriteAllText(
                Path.Combine(iterationDir, "godot-log-audit.json"),
                $$"""
                {
                  "Path": {{JsonSerializer.Serialize(currentLogPath)}},
                  "Length": {{dirtyLogLength}},
                  "Sha256": {{JsonSerializer.Serialize(dirtyLogHash)}},
                  "Clean": true,
                  "SignatureHits": []
                }
                """);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_recomputed_clean status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_signature_counts_match_recomputed status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsStaleSts1ModeLogCheckJson()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var fullLogPath = Path.Combine(iterationDir, "godot.log.after-launch");
            File.WriteAllText(
                Path.Combine(iterationDir, "sts1-mode-log-check.json"),
                $$"""
                {
                  "Mode": "CanaryOnly",
                  "LogPath": {{JsonSerializer.Serialize(fullLogPath)}},
                  "LogLength": 1,
                  "LogSha256": "stale",
                  "Mismatches": [],
                  "Checks": [{ "Passed": true }]
                }
                """);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_sts1_mode_log_check_mode_matches_plan status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_sts1_mode_log_check_log_path_matches_current_iteration_log status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_sts1_mode_log_check_log_length_matches_current_iteration_log status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_sts1_mode_log_check_log_sha256_matches_current_iteration_log status=fail", result.Output, StringComparison.Ordinal);
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
            "autoslay-summary.json",
            "iteration-result.json",
            "run-result.json",
            "GameNativeAutoSlay",
            "AutoSlayer.Start(seed, logFile)",
            "godot.log.after-launch",
            "godot.log.current-iteration",
            "godot-log-audit.json",
            "LogScanOffsetBytes",
            "autoslay-summary.json",
            "run-result.json",
            "GameNativeAutoSlay",
            "Resolve-AnalysisPath",
            "Test-CurrentSliceFromBeforeAfter",
            "GodotLogBeforePath",
            "GodotLogAfterLaunchPath",
            "GodotLogCurrentIterationPath",
            "current_iteration_log_before_after_binding_missing",
            "AutoSlayLogPath",
            "EventKind",
            "AncientId",
            "autoslay_event_kind_not_ancient",
            "autoslay_ancient_id_missing",
            "autoslay_sidecar_event_sequence_missing",
            "autoslay_current_log_event_sequence_missing",
            "autoslay_sidecar_ancient_id_missing",
            "autoslay_current_log_ancient_id_missing",
            "FailureReasonCodes",
            "HangSignals",
            "Get-JsonArrayValues",
            "Test-JsonFileParses",
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
            "current_iteration_log_before_after_binding_missing",
            "current_iteration_log_offset_binding_missing",
            "current_iteration_log_scan_offset_invalid",
            "autoslay_sidecar_event_sequence_missing",
            "autoslay_current_log_event_sequence_missing",
            "Detected Ancient event, clicking through dialogue",
            "iteration_result_missing_or_invalid",
            "iteration_failed_without_failure_signal",
            "godot_log_audit_json_invalid",
            "godot_log_audit_current_iteration_binding_mismatch",
            "godot_log_audit_recomputed_mismatch",
            "AuditTrustedForOwner",
            "Get-FileSha256OrEmpty",
            "Invoke-RecomputedAudit",
            "process_unresponsive",
            "stale_process_observed",
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

        Assert.DoesNotContain("@((Get-JsonValue", analyzer, StringComparison.Ordinal);
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
                """{"SignatureHits":[{"Name":"Spire Plus error/exception","Count":1},{"Name":"Godot ERROR line","Count":1}]}""");
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
    public void RuntimeFailureAnalyzerScansIterationDirectoriesWithoutMonkeySummary()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteIteration(
                workdir,
                1,
                "spireplus_test_ancient URDA confirm",
                "ancient-ui",
                "Runtime.Unknown",
                """["process_unresponsive"]""",
                """["process_unresponsive"]""",
                "[INFO] stale full-log prefix before accepted scan offset",
                "[SPIREPLUS-EVIDENCE] PreviewTransform [Spire Plus] Preview: Transform prediction",
                """{"SignatureHits":[]}""");

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);

            Assert.Equal(1, root.GetProperty("AnalyzedIterationCount").GetInt32());
            Assert.Equal("PreviewTools", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("PreviewTools", FindFindingOwner(iteration, "process_unresponsive"));
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
            var currentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
            File.WriteAllText(currentLogPath, staleCurrentSlice);
            File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), ToBoundAuditJson(currentLogPath, """{"SignatureHits":[]}"""));
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
    public void RuntimeFailureAnalyzerReportsUnboundCurrentIterationSlice()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            Directory.CreateDirectory(iterationDir);
            File.WriteAllText(
                Path.Combine(iterationDir, "iteration-result.json"),
                """
                {
                  "Iteration": 1,
                  "Passed": false,
                  "Command": "spireplus_test_ancient VAKUU confirm fight",
                  "ScenarioTag": "vakuu-fight",
                  "OwnerArea": "Ancients.Vakuu.FightOptionSetup",
                  "FailureReasonCodes": ["process_unresponsive"],
                  "HangSignals": ["process_unresponsive"]
                }
                """);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), "[ERROR] TypeLoadException stale full log should not route owner\r\n");
            var currentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
            File.WriteAllText(currentLogPath, "[SPIREPLUS-EVIDENCE] StS1 AdditiveBatch1 Registered act event Golden Idol\r\n");
            File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), ToBoundAuditJson(currentLogPath, """{"SignatureHits":[]}"""));
            WriteMonkeySummary(workdir, 1);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var iteration = FindIteration(document.RootElement, 1);
            var bindingFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "current_iteration_log_offset_binding_missing");

            Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("RuntimeHarness", bindingFinding.GetProperty("OwnerArea").GetString());
            Assert.Equal("blocking", bindingFinding.GetProperty("Severity").GetString());
            Assert.Equal("Ancients.Vakuu.FightOptionSetup", FindFindingOwner(iteration, "process_unresponsive"));
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
    public void RuntimeFailureAnalyzerReadsGameNativeAutoSlayRunResultsAndByteBoundSlices()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var runDir = Path.Combine(workdir, "run-0001");
            Directory.CreateDirectory(runDir);
            const string seed = "AUTOSLAY-ANALYZER";
            var beforeLog = "[INFO] previous shared godot.log content\r\n";
            var currentLog = $"""
                [INFO] [AutoSlay] Starting run with seed={seed}
                [INFO] [AutoSlay] Entering Event room (Act 1, Floor 2)
                [INFO] [AutoSlay] Detected Ancient event, clicking through dialogue: VAKUU
                [INFO] [AutoSlay] Action: Selecting event option: VAKUU (option: contract)
                [INFO] [AutoSlay] Run completed successfully with seed={seed}
                """;
            var autoSlayLog = $"""
                [INFO] [AutoSlay] Starting run with seed={seed}
                [INFO] [AutoSlay] Entering Event room (Act 1, Floor 2)
                [INFO] [AutoSlay] Detected Ancient event, clicking through dialogue: VAKUU
                [INFO] [AutoSlay] Action: Selecting event option: VAKUU (option: contract)
                [INFO] [AutoSlay] Run completed successfully with seed={seed}
                """;
            var currentLogPath = Path.Combine(runDir, "godot.log.current-iteration");
            File.WriteAllText(Path.Combine(runDir, "godot.log.before"), beforeLog);
            File.WriteAllText(currentLogPath, currentLog);
            File.WriteAllText(Path.Combine(runDir, "godot.log.after-launch"), beforeLog + currentLog);
            File.WriteAllText(Path.Combine(runDir, "autoslay.log"), autoSlayLog);
            File.WriteAllText(Path.Combine(runDir, "godot-log-audit.json"), ToBoundAuditJson(currentLogPath, """{"SignatureHits":[]}"""));
            File.WriteAllText(
                Path.Combine(runDir, "run-result.json"),
                $$"""
                {
                  "SchemaVersion": 1,
                  "Launch": true,
                  "RunnerKind": "GameNativeAutoSlay",
                  "Invocation": "Spire Plus test hook calls AutoSlayer.Start(seed, logFile)",
                  "Seed": {{JsonSerializer.Serialize(seed)}},
                  "EventKind": "Ancient",
                  "AncientId": "VAKUU",
                  "Passed": false,
                  "OwnerArea": "Runtime.Unknown",
                  "FailureReasonCodes": ["process_unresponsive"],
                  "HangSignals": ["process_unresponsive"],
                  "AutoSlayLogPath": "run-0001/autoslay.log",
                  "GodotLogBeforePath": "run-0001/godot.log.before",
                  "GodotLogAfterLaunchPath": "run-0001/godot.log.after-launch",
                  "GodotLogCurrentIterationPath": "run-0001/godot.log.current-iteration",
                  "GodotLogAuditPath": "run-0001/godot-log-audit.json"
                }
                """);
            File.WriteAllText(
                Path.Combine(workdir, "autoslay-summary.json"),
                """
                {
                  "RunnerKind": "GameNativeAutoSlay",
                  "Runs": [
                    {
                      "Seed": "AUTOSLAY-ANALYZER",
                      "EventKind": "Ancient",
                      "AncientId": "VAKUU",
                      "RunResultPath": "run-0001/run-result.json"
                    }
                  ]
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var iteration = FindIteration(document.RootElement, 1);

            Assert.Equal(seed, iteration.GetProperty("Seed").GetString());
            Assert.Equal("GameNativeAutoSlay", iteration.GetProperty("RunnerKind").GetString());
            Assert.Equal("Ancient", iteration.GetProperty("EventKind").GetString());
            Assert.Equal("VAKUU", iteration.GetProperty("AncientId").GetString());
            Assert.Equal("game-native-autoslay", iteration.GetProperty("ScenarioTag").GetString());
            Assert.True(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Equal("Ancients.Vakuu", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("Ancients.Vakuu", FindFindingOwner(iteration, "process_unresponsive"));
            Assert.DoesNotContain(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "current_iteration_log_before_after_binding_missing");
            Assert.DoesNotContain(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_sidecar_event_sequence_missing");
            Assert.DoesNotContain(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_current_log_event_sequence_missing");
            Assert.DoesNotContain(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_event_kind_not_ancient");
            Assert.DoesNotContain(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_ancient_id_missing");
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
    public void RuntimeFailureAnalyzerFailClosesUnboundGameNativeAutoSlaySlices()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var runDir = Path.Combine(workdir, "run-0001");
            Directory.CreateDirectory(runDir);
            var currentLogPath = Path.Combine(runDir, "godot.log.current-iteration");
            File.WriteAllText(currentLogPath, "[SPIREPLUS-EVIDENCE] StS1 AdditiveBatch1 Registered act event Golden Idol\r\n");
            File.WriteAllText(Path.Combine(runDir, "autoslay.log"), "[INFO] [AutoSlay] Starting run with seed=UNBOUND\r\n");
            File.WriteAllText(Path.Combine(runDir, "godot-log-audit.json"), ToBoundAuditJson(currentLogPath, """{"SignatureHits":[]}"""));
            File.WriteAllText(
                Path.Combine(runDir, "run-result.json"),
                """
                {
                  "SchemaVersion": 1,
                  "Launch": true,
                  "RunnerKind": "GameNativeAutoSlay",
                  "Invocation": "Spire Plus test hook calls AutoSlayer.Start(seed, logFile)",
                  "Seed": "UNBOUND",
                  "Passed": false,
                  "OwnerArea": "Ancients.Vakuu.FightOptionSetup",
                  "FailureReasonCodes": ["process_unresponsive"],
                  "HangSignals": ["process_unresponsive"],
                  "AutoSlayLogPath": "run-0001/autoslay.log",
                  "GodotLogBeforePath": "run-0001/godot.log.before",
                  "GodotLogAfterLaunchPath": "run-0001/godot.log.after-launch",
                  "GodotLogCurrentIterationPath": "run-0001/godot.log.current-iteration",
                  "GodotLogAuditPath": "run-0001/godot-log-audit.json"
                }
                """);
            File.WriteAllText(
                Path.Combine(workdir, "autoslay-summary.json"),
                """
                {
                  "RunnerKind": "GameNativeAutoSlay",
                  "Runs": [
                    {
                      "Seed": "UNBOUND",
                      "RunResultPath": "run-0001/run-result.json"
                    }
                  ]
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var iteration = FindIteration(document.RootElement, 1);
            var bindingFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "current_iteration_log_before_after_binding_missing");
            var sidecarFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "autoslay_sidecar_event_sequence_missing");

            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("RuntimeHarness", bindingFinding.GetProperty("OwnerArea").GetString());
            Assert.Equal("RuntimeHarness", sidecarFinding.GetProperty("OwnerArea").GetString());
            Assert.Equal("blocking", bindingFinding.GetProperty("Severity").GetString());
            Assert.Equal("Ancients.Vakuu.FightOptionSetup", FindFindingOwner(iteration, "process_unresponsive"));
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
    public void RuntimeFailureAnalyzerDoesNotRouteMissingCurrentSliceFromFullLog()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            Directory.CreateDirectory(iterationDir);
            File.WriteAllText(
                Path.Combine(iterationDir, "iteration-result.json"),
                """
                {
                  "Iteration": 1,
                  "Passed": false,
                  "Command": "spireplus_test_ancient VAKUU confirm fight",
                  "ScenarioTag": "vakuu-fight",
                  "OwnerArea": "Ancients.Vakuu.FightOptionSetup",
                  "LogScanOffsetBytes": 0,
                  "FailureReasonCodes": ["process_unresponsive"],
                  "HangSignals": ["process_unresponsive"]
                }
                """);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), "[ERROR] TypeLoadException stale full log should not route owner\r\n");
            File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), """{"SignatureHits":[]}""");
            WriteMonkeySummary(workdir, 1);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var iteration = FindIteration(document.RootElement, 1);
            var missingSliceFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "current_iteration_log_missing");

            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("RuntimeHarness", missingSliceFinding.GetProperty("OwnerArea").GetString());
            Assert.Equal("Ancients.Vakuu.FightOptionSetup", FindFindingOwner(iteration, "process_unresponsive"));
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
            var currentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
            File.WriteAllText(currentLogPath, "[Startup] Time to main menu\r\n");
            File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), ToBoundAuditJson(currentLogPath, """{"SignatureHits":[]}"""));
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
    public void RuntimeFailureAnalyzerReportsFailedIterationWithoutRetainedSignals()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            Directory.CreateDirectory(iterationDir);
            File.WriteAllText(
                Path.Combine(iterationDir, "iteration-result.json"),
                """
                {
                  "Iteration": 1,
                  "Passed": false,
                  "Command": "spireplus_test_ancient URDA confirm",
                  "ScenarioTag": "ancient-ui-urda",
                  "OwnerArea": "Ancients.Urda.MapSaveState",
                  "LogScanOffsetBytes": 0,
                  "FailureReasonCodes": [],
                  "HangSignals": []
                }
                """);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), "[Startup] Time to main menu\r\n");
            var currentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
            File.WriteAllText(currentLogPath, "[Startup] Time to main menu\r\n");
            File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), ToBoundAuditJson(currentLogPath, """{"SignatureHits":[]}"""));
            WriteMonkeySummary(workdir, 1);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var iteration = FindIteration(document.RootElement, 1);
            var failedWithoutSignalFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "iteration_failed_without_failure_signal");

            Assert.Equal("RuntimeHarness", failedWithoutSignalFinding.GetProperty("OwnerArea").GetString());
            Assert.Equal("blocking", failedWithoutSignalFinding.GetProperty("Severity").GetString());
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
    public void RuntimeFailureAnalyzerReportsInvalidAuditJsonAsHarnessBlocker()
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
                "Ancients.Vakuu.FightOptionSetup",
                """[]""",
                """[]""",
                "[Startup] Time to main menu",
                "[Startup] Time to main menu",
                "{ invalid audit json");
            WriteMonkeySummary(workdir, 1);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var iteration = FindIteration(document.RootElement, 1);
            var auditFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_json_invalid");

            Assert.Equal("RuntimeHarness", auditFinding.GetProperty("OwnerArea").GetString());
            Assert.Equal("blocking", auditFinding.GetProperty("Severity").GetString());
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
    public void RuntimeFailureAnalyzerRejectsAuditJsonNotBoundToCurrentIterationLog()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            Directory.CreateDirectory(iterationDir);
            var currentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
            var staleLogPath = Path.Combine(iterationDir, "stale-source.log");
            File.WriteAllText(
                Path.Combine(iterationDir, "iteration-result.json"),
                """
                {
                  "Iteration": 1,
                  "Passed": false,
                  "Command": "spireplus_test_ancient VAKUU confirm fight",
                  "ScenarioTag": "vakuu-fight",
                  "OwnerArea": "Ancients.Vakuu.FightOptionSetup",
                  "LogScanOffsetBytes": 0,
                  "FailureReasonCodes": [],
                  "HangSignals": []
                }
                """);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), "[Startup] Time to main menu\r\n");
            File.WriteAllText(currentLogPath, "[Startup] Time to main menu\r\n");
            File.WriteAllText(staleLogPath, "[ERROR] Spire Plus error from stale audit source\r\n");
            var staleHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(staleLogPath))).ToLowerInvariant();
            File.WriteAllText(
                Path.Combine(iterationDir, "godot-log-audit.json"),
                $$"""
                {
                  "Path": {{JsonSerializer.Serialize(staleLogPath)}},
                  "Length": {{new FileInfo(staleLogPath).Length}},
                  "Sha256": "{{staleHash}}",
                  "Clean": false,
                  "SignatureHits": [{ "Name": "Spire Plus error/exception", "Count": 1 }]
                }
                """);
            WriteMonkeySummary(workdir, 1);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var iteration = FindIteration(document.RootElement, 1);
            var auditBindingFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "godot_log_audit_current_iteration_binding_mismatch");

            Assert.False(iteration.GetProperty("AuditTrustedForOwner").GetBoolean());
            Assert.Equal("RuntimeHarness", auditBindingFinding.GetProperty("OwnerArea").GetString());
            Assert.DoesNotContain(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "audit:Spire Plus error/exception");
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
    public void Sts1RuntimeEvidencePacketVerifierRejectsStaleFullLogPrefixForEnabledMode()
    {
        var packetVerifier = AssertRepoFileExists("scripts", "check-sts1-runtime-evidence-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-runtime-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteSts1RuntimePacketState(workdir, mode: "AdditiveBatch1");
            var stalePrefix = BuildSts1ModeRuntimeLog("AdditiveBatch1");
            var currentOffSlice = """
                v0.1.0-private-beta.86
                release = v0.107.0
                RitsuLib Version: 0.4.16 [compat branch: 0.107.0]
                Feature Sts1Events bootstrap=disabled, live=Disabled
                """;
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), stalePrefix);
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), stalePrefix + currentOffSlice);

            var result = RunPowerShell(
                packetVerifier,
                "-Mode",
                "AdditiveBatch1",
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.86",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.16",
                "-ExpectedGameVersion",
                "0.107.0",
                "-OutFile",
                Path.Combine(workdir, "runtime-evidence-packet-check.json"));

            Assert.True(result.ExitCode == 0, $"Packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("enabled_mode_log_verifier_uses_current_slice status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("full_log_not_used_as_canonical_verifier_input status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("current_slice_derived_from_before_after status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("enabled_mode_log_verifier_clean status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("log_verifier log_path=", result.Output, StringComparison.Ordinal);
            Assert.Contains("godot.log.current-iteration", result.Output, StringComparison.Ordinal);
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
    public void Sts1RuntimeEvidencePacketVerifierRejectsRetainedCurrentSliceThatDoesNotMatchBeforeAfter()
    {
        var packetVerifier = AssertRepoFileExists("scripts", "check-sts1-runtime-evidence-packet.ps1");
        var auditScript = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-runtime-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteSts1RuntimePacketState(workdir, mode: "AdditiveBatch1");
            const string preLaunchPrefix = "[Startup] retained pre-launch log prefix\r\n";
            var actualOffSlice = """
                v0.1.0-private-beta.86
                release = v0.107.0
                RitsuLib Version: 0.4.16 [compat branch: 0.107.0]
                Feature Sts1Events bootstrap=disabled, live=Disabled
                """;
            var retainedStaleCurrentSlice = BuildSts1ModeRuntimeLog("AdditiveBatch1");
            var retainedCurrentSlicePath = Path.Combine(workdir, "godot.log.current-iteration");
            var retainedCurrentAuditPath = Path.Combine(workdir, "godot-log-current-iteration-audit.json");
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), preLaunchPrefix);
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), preLaunchPrefix + actualOffSlice);
            File.WriteAllText(retainedCurrentSlicePath, retainedStaleCurrentSlice);

            var auditResult = RunPowerShell(auditScript, "-Path", retainedCurrentSlicePath, "-OutFile", retainedCurrentAuditPath);
            Assert.True(auditResult.ExitCode == 0, $"Audit failed:{Environment.NewLine}{auditResult.Output}{auditResult.Error}");

            var result = RunPowerShell(
                packetVerifier,
                "-Mode",
                "AdditiveBatch1",
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.86",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.16",
                "-ExpectedGameVersion",
                "0.107.0",
                "-OutFile",
                Path.Combine(workdir, "runtime-evidence-packet-check.json"));

            Assert.True(result.ExitCode == 0, $"Packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("current_slice_matches_before_after status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("enabled_mode_log_verifier_uses_current_slice status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("enabled_mode_log_verifier_clean status=pass", result.Output, StringComparison.Ordinal);

            using var report = JsonDocument.Parse(File.ReadAllText(Path.Combine(workdir, "runtime-evidence-packet-check.json")));
            Assert.False(report.RootElement.GetProperty("CurrentSliceMatchesBeforeAfter").GetBoolean());
            Assert.Contains("current slice", report.RootElement.GetProperty("CurrentSliceBindingDetail").GetString(), StringComparison.Ordinal);
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
    public void Sts1RuntimeEvidencePacketVerifierDerivesAndAuditsCurrentSlice()
    {
        var packetVerifier = AssertRepoFileExists("scripts", "check-sts1-runtime-evidence-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-runtime-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteSts1RuntimePacketState(workdir, mode: "AdditiveBatch1");
            const string stalePrefix = "[Startup] stale pre-launch log prefix without StS1 registrations\r\n";
            var currentSlice = BuildSts1ModeRuntimeLog("AdditiveBatch1");
            File.WriteAllText(Path.Combine(workdir, "godot.log.before"), stalePrefix);
            File.WriteAllText(Path.Combine(workdir, "godot.log.after-launch"), stalePrefix + currentSlice);

            var result = RunPowerShell(
                packetVerifier,
                "-Mode",
                "AdditiveBatch1",
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.86",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.16",
                "-ExpectedGameVersion",
                "0.107.0",
                "-OutFile",
                Path.Combine(workdir, "runtime-evidence-packet-check.json"),
                "-FailOnMismatch");

            Assert.True(result.ExitCode == 0, $"Packet verifier failed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("derived_current_slice_audit_generated status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("enabled_mode_log_verifier_clean status=pass", result.Output, StringComparison.Ordinal);

            using var report = JsonDocument.Parse(File.ReadAllText(Path.Combine(workdir, "runtime-evidence-packet-check.json")));
            Assert.True(report.RootElement.GetProperty("CurrentSliceDerivedFromBeforeAfter").GetBoolean());
            Assert.EndsWith("godot.log.current-iteration", report.RootElement.GetProperty("CanonicalLogPath").GetString(), StringComparison.OrdinalIgnoreCase);
            Assert.EndsWith("godot-log-current-iteration-audit.json", report.RootElement.GetProperty("CanonicalAuditPath").GetString(), StringComparison.OrdinalIgnoreCase);
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
    public void Sts1EnabledModeLogVerifierRecomputesAuditFromCopiedLog()
    {
        var verifier = AssertRepoFileExists("scripts", "check-sts1-enabled-mode-runtime-log.ps1");
        var auditScript = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "sts1-enabled-mode-log-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var logPath = Path.Combine(workdir, "godot.log.after-launch");
            var auditPath = Path.Combine(workdir, "godot-log-audit.json");
            var cleanLog = """
                StS1 events default Off; set SPIREPLUS_STS1_EVENT_MODE to enable.
                Feature Sts1Events bootstrap=disabled, live=Disabled
                """;
            File.WriteAllText(logPath, cleanLog);

            var cleanAudit = RunPowerShell(auditScript, "-Path", logPath, "-OutFile", auditPath);
            Assert.True(cleanAudit.ExitCode == 0, $"Audit failed:{Environment.NewLine}{cleanAudit.Output}{cleanAudit.Error}");

            var cleanResult = RunPowerShell(verifier, "-Mode", "Off", "-LogPath", logPath, "-AuditPath", auditPath);
            Assert.True(cleanResult.ExitCode == 0, $"Verifier crashed:{Environment.NewLine}{cleanResult.Output}{cleanResult.Error}");
            Assert.Contains("mismatches=0", cleanResult.Output, StringComparison.Ordinal);

            var dirtyLog = cleanLog + Environment.NewLine + "[ERROR] TypeLoadException" + Environment.NewLine;
            File.WriteAllText(logPath, dirtyLog);
            var dirtyLogLength = new FileInfo(logPath).Length;
            var dirtyLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(logPath))).ToLowerInvariant();
            File.WriteAllText(
                auditPath,
                $$"""
                {
                  "Path": {{JsonSerializer.Serialize(logPath)}},
                  "Length": {{dirtyLogLength}},
                  "Sha256": {{JsonSerializer.Serialize(dirtyLogHash)}},
                  "Clean": true,
                  "SignatureHits": []
                }
                """);

            var dirtyResult = RunPowerShell(verifier, "-Mode", "Off", "-LogPath", logPath, "-AuditPath", auditPath);
            Assert.True(dirtyResult.ExitCode == 0, $"Verifier crashed:{Environment.NewLine}{dirtyResult.Output}{dirtyResult.Error}");
            Assert.Contains("audit_recomputed_clean status=fail", dirtyResult.Output, StringComparison.Ordinal);
            Assert.Contains("audit_signature_counts_match_recomputed status=fail", dirtyResult.Output, StringComparison.Ordinal);
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
            "autoslay_autoslayer_source_exists",
            "autoslay_event_room_handler_exists",
            "autoslay_start_seed_logfile_signature_present",
            "public void Start(string seed, string? logFile = null)",
            "autoslay_noninteractive_mode_check_present",
            "NonInteractiveMode.AutoSlayerCheck = () => IsActive",
            "autoslay_debug_seed_override_present",
            "NGame.Instance.DebugSeedOverride = seed",
            "autoslay_card_selector_present",
            "CardSelectCmd.UseSelector(new AutoSlayCardSelector(_random))",
            "autoslay_ancient_dialogue_handler_present",
            "Detected Ancient event, clicking through dialogue",
            "autoslay_event_option_selection_logged",
            "Selecting event option:",
            "autoslay_event_triggered_combat_logged",
            "Event triggered combat, handling combat first",
            "autoslay_event_combat_started_logged",
            "Event combat started, applying buffs and killing enemies",
            "AutoSlay = [pscustomobject]$autoSlaySummary",
            "GameNativeAutoSlayStillRequiresRuntimeLaunchEvidence",
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
    public void RuntimeMonkeyDocsKeepGameNativeAutoSlayProofSeparateFromDevConsoleHarness()
    {
        var docs = ReadRepoText("docs", "testing", "runtime-monkey-stability.md");
        var runner = ReadRepoText("scripts", "run-spire-plus-monkey-stability.ps1");

        AssertSourceContains(
            docs,
            "## Game-Native AutoSlay Batch Lane",
            "`source code\\src\\Core\\AutoSlay\\AutoSlayer.cs`",
            "`AutoSlayer.Start(seed, logFile)`",
            "`NonInteractiveMode.AutoSlayerCheck = () => IsActive`",
            "`NGame.Instance.DebugSeedOverride = seed`",
            "`CardSelectCmd.UseSelector(new AutoSlayCardSelector(_random))`",
            "`source code\\src\\Core\\AutoSlay\\Handlers\\Rooms\\EventRoomHandler.cs`",
            "Detected Ancient event, clicking",
            "Selecting event option:",
            "Event triggered combat",
            "Event combat started",
            "Current `scripts\\run-spire-plus-monkey-stability.ps1` lane is not",
            "AutoSlay-backed",
            "Do not count a packet from that lane as game-native",
            "the exact launcher or mod hook that calls `AutoSlayer.Start(seed, logFile)`",
            "one `run-result.json` per seed",
            "`RecoveredSource.MatchesInstalledGame`",
            "Ancient id, ordered",
            "start/event/Ancient-dialogue/event-option/completion markers",
            "observed ordered event-room lines in both",
            "check-spire-plus-autoslay-packet.ps1",
            "GameNativeAutoSlay",
            "-ExpectedPatchCount 25");

        Assert.DoesNotContain("AutoSlayer", runner, StringComparison.Ordinal);
        Assert.DoesNotContain("AutoSlayLog", runner, StringComparison.Ordinal);
        Assert.Contains("DevConsole", runner, StringComparison.Ordinal);
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRequiresNativeRunnerAndEventTraversal()
    {
        var verifier = AssertRepoFileExists("scripts", "check-spire-plus-autoslay-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "autoslay-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            const string seed = "AUTOSLAYSEED1";
            var sourceWorkspaceReportPath = Path.Combine(workdir, "local-godot-source-workspace-check.json");
            File.WriteAllText(
                sourceWorkspaceReportPath,
                """
                {
                  "SchemaVersion": 1,
                  "CreatedAt": "2026-06-18T10:00:00Z",
                  "Passed": true,
                  "RepoRoot": "D:\\Game\\FOTN\\dev-the-spire",
                  "SourceRoot": "D:\\Game\\FOTN\\dev-the-spire\\source code",
                  "GameRoot": "E:\\Steam\\steamapps\\common\\Slay the Spire 2",
                  "Mismatches": [],
                  "Game": {
                    "Version": "v0.107.0"
                  },
                  "RecoveredSource": {
                    "Version": "v0.107.0",
                    "Commit": "fixture",
                    "Disposition": "current",
                    "MatchesInstalledGame": true
                  },
                  "AutoSlay": {
                    "StartSeedLogFileSignature": true,
                    "NonInteractiveCheck": true,
                    "DebugSeedOverride": true,
                    "AutoCardSelector": true,
                    "AncientDialogueHandler": true,
                    "EventOptionSelectionLog": true,
                    "EventTriggeredCombatLog": true,
                    "EventCombatStartedLog": true
                  },
                  "EvidenceUsePolicy": {
                    "NoLaunch": true,
                    "NotRuntimeProof": true,
                    "RefreshSourceSnapshotBeforeCurrentApiClaims": false,
                    "LocalSourceReferenceOnly": true,
                    "AuthorizedLocalInstallOnly": true,
                    "ThirdPartyDumpsProhibited": true,
                    "RuntimeProofStillRequiresLaunchEvidence": true,
                    "GameNativeAutoSlayStillRequiresRuntimeLaunchEvidence": true
                  }
                }
                """);
            var sourceWorkspaceReportHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(sourceWorkspaceReportPath))).ToLowerInvariant();
            var launcherPath = Path.Combine(workdir, "autoslay-launcher-proof.json");
            File.WriteAllText(
                launcherPath,
                """
                {
                  "LauncherKind": "SpirePlusDebugHook",
                  "HookId": "SpirePlus.AutoSlayHarness.Start",
                  "HookAssembly": "EZMicroBalanceCode",
                  "InvocationCommand": "SpirePlus.AutoSlayHarness.Start -> AutoSlayer.Start(seed, logFile)"
                }
                """);
            var launcherHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(launcherPath))).ToLowerInvariant();

            var runDir = Path.Combine(workdir, "run-0001");
            Directory.CreateDirectory(runDir);
            var runResultPath = Path.Combine(runDir, "run-result.json");
            var autoSlayLogPath = Path.Combine(runDir, "autoslay.log");
            var beforeLogPath = Path.Combine(runDir, "godot.log.before");
            var afterLogPath = Path.Combine(runDir, "godot.log.after-launch");
            var currentLogPath = Path.Combine(runDir, "godot.log.current-iteration");
            var auditPath = Path.Combine(runDir, "godot-log-audit.json");
            var sts1ModeCheckPath = Path.Combine(runDir, "sts1-mode-log-check.json");
            var autoSlayLog = string.Join(
                Environment.NewLine,
                $"12:00:00.000 [INFO] [AutoSlay] Starting run with seed={seed}",
                "12:00:01.000 [INFO] [AutoSlay] Entering Event room (Act 1, Floor 2)",
                "12:00:01.500 [INFO] [AutoSlay] Detected Ancient event, clicking through dialogue: VAKUU",
                "12:00:02.000 [INFO] [AutoSlay] Action: Selecting event option: VAKUU (option: contract)",
                $"12:00:03.000 [INFO] [AutoSlay] Run completed successfully with seed={seed}") + Environment.NewLine;
            var currentLog = string.Join(
                Environment.NewLine,
                "v0.1.0-private-beta.86",
                "release = v0.107.0",
                "RitsuLib Version: 0.4.16 [compat branch: 0.107.0]",
                "[INFO] [EZMicroBalance] [Patcher - SpirePlus] Patch application complete: 25 applied, 0 ignored, 0 failed, 25 total",
                "[INFO] [EZMicroBalance] ModPatcher applied 25 patches (25 registered).",
                autoSlayLog.TrimEnd()) + Environment.NewLine;
            var beforeLog = "11:59:59.000 [INFO] [Previous] old shared log line" + Environment.NewLine;
            File.WriteAllText(autoSlayLogPath, autoSlayLog);
            File.WriteAllText(beforeLogPath, beforeLog);
            File.WriteAllText(currentLogPath, currentLog);
            File.WriteAllText(afterLogPath, beforeLog + currentLog);
            var autoSlayLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(autoSlayLogPath))).ToLowerInvariant();
            var currentLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(currentLogPath))).ToLowerInvariant();
            var currentLogLength = new FileInfo(currentLogPath).Length;
            File.WriteAllText(auditPath, ToBoundAuditJson(currentLogPath, """{"SignatureHits":[]}"""));
            File.WriteAllText(
                sts1ModeCheckPath,
                $$"""
                {
                  "Mode": "Off",
                  "LogPath": {{JsonSerializer.Serialize(currentLogPath)}},
                  "LogLength": {{currentLogLength}},
                  "LogSha256": {{JsonSerializer.Serialize(currentLogHash)}},
                  "Mismatches": [],
                  "Checks": [{ "Passed": true }]
                }
                """);
            File.WriteAllText(
                runResultPath,
                $$"""
                {
                  "SchemaVersion": 1,
                  "Launch": true,
                  "RunnerKind": "GameNativeAutoSlay",
                  "Invocation": "Spire Plus test hook calls AutoSlayer.Start(seed, logFile)",
                  "LauncherKind": "SpirePlusDebugHook",
                  "LauncherPath": "autoslay-launcher-proof.json",
                  "LauncherSha256": {{JsonSerializer.Serialize(launcherHash)}},
                  "HookId": "SpirePlus.AutoSlayHarness.Start",
                  "HookAssembly": "EZMicroBalanceCode",
                  "InvocationCommand": "SpirePlus.AutoSlayHarness.Start -> AutoSlayer.Start(seed, logFile)",
                  "Seed": {{JsonSerializer.Serialize(seed)}},
                  "EventKind": "Ancient",
                  "AncientId": "VAKUU",
                  "Passed": true,
                  "FailureReasonCodes": [],
                  "HangSignals": [],
                  "ProcessId": 4242,
                  "StartTimestamp": "2026-06-18T10:00:00Z",
                  "EndTimestamp": "2026-06-18T10:00:30Z",
                  "ExitCode": 0,
                  "StaleProcessCount": 0,
                  "AutoSlayLogPath": "run-0001/autoslay.log",
                  "AutoSlayLogSha256": {{JsonSerializer.Serialize(autoSlayLogHash)}},
                  "GodotLogBeforePath": "run-0001/godot.log.before",
                  "GodotLogAfterLaunchPath": "run-0001/godot.log.after-launch",
                  "GodotLogCurrentIterationPath": "run-0001/godot.log.current-iteration",
                  "GodotLogCurrentIterationSha256": {{JsonSerializer.Serialize(currentLogHash)}},
                  "GodotLogAuditPath": "run-0001/godot-log-audit.json",
                  "Sts1ModeLogCheckPath": "run-0001/sts1-mode-log-check.json"
                }
                """);

            File.WriteAllText(
                Path.Combine(workdir, "autoslay-plan.json"),
                $$"""
                {
                  "RunnerKind": "GameNativeAutoSlay",
                  "Invocation": "Spire Plus test hook calls AutoSlayer.Start(seed, logFile)",
                  "LauncherKind": "SpirePlusDebugHook",
                  "LauncherPath": "autoslay-launcher-proof.json",
                  "LauncherSha256": {{JsonSerializer.Serialize(launcherHash)}},
                  "HookId": "SpirePlus.AutoSlayHarness.Start",
                  "HookAssembly": "EZMicroBalanceCode",
                  "InvocationCommand": "SpirePlus.AutoSlayHarness.Start -> AutoSlayer.Start(seed, logFile)",
                  "Seeds": [{{JsonSerializer.Serialize(seed)}}],
                  "PackageVersion": "v0.1.0-private-beta.86",
                  "GameVersion": "0.107.0",
                  "RitsuLibVersion": "0.4.16",
                  "RitsuCompatBranch": "0.107.0",
                  "Sts1EventMode": "Off",
                  "SourceWorkspaceCheckPath": "local-godot-source-workspace-check.json",
                  "SourceWorkspaceCheckSha256": {{JsonSerializer.Serialize(sourceWorkspaceReportHash)}},
                  "SourceWorkspace": {
                    "Checked": true,
                    "ReportPath": "local-godot-source-workspace-check.json",
                    "ReportSha256": {{JsonSerializer.Serialize(sourceWorkspaceReportHash)}},
                    "Passed": true,
                    "SourceVersion": "v0.107.0",
                    "SourceCommit": "fixture",
                    "InstalledGameVersion": "v0.107.0",
                    "Disposition": "current",
                    "MatchesInstalledGame": true,
                    "RefreshSourceSnapshotBeforeCurrentApiClaims": false,
                    "NotRuntimeProof": true
                  }
                }
                """);
            File.WriteAllText(
                Path.Combine(workdir, "autoslay-summary.json"),
                $$"""
                {
                  "RunnerKind": "GameNativeAutoSlay",
                  "Passed": true,
                  "TotalRuns": 1,
                  "FailedRuns": 0,
                  "Runs": [
                    {
                      "Seed": {{JsonSerializer.Serialize(seed)}},
                      "Passed": true,
                      "ExitCode": 0,
                      "EventKind": "Ancient",
                      "AncientId": "VAKUU",
                      "FailureReasonCodes": [],
                      "HangSignals": [],
                      "RunResultPath": "run-0001/run-result.json",
                      "AutoSlayLogPath": "run-0001/autoslay.log",
                      "AutoSlayLogSha256": {{JsonSerializer.Serialize(autoSlayLogHash)}},
                      "GodotLogBeforePath": "run-0001/godot.log.before",
                      "GodotLogAfterLaunchPath": "run-0001/godot.log.after-launch",
                      "GodotLogCurrentIterationPath": "run-0001/godot.log.current-iteration",
                      "GodotLogCurrentIterationSha256": {{JsonSerializer.Serialize(currentLogHash)}},
                      "GodotLogAuditPath": "run-0001/godot-log-audit.json",
                      "Sts1ModeLogCheckPath": "run-0001/sts1-mode-log-check.json"
                    }
                  ]
                }
                """);

            var passResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.86",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.16",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25",
                "-OutFile",
                Path.Combine(workdir, "autoslay-packet-check.json"),
                "-FailOnMismatch");

            Assert.True(passResult.ExitCode == 0, $"AutoSlay packet verifier failed:{Environment.NewLine}{passResult.Output}{passResult.Error}");
            Assert.Contains("plan_runner_kind_is_game_native_autoslay status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("plan_invocation_calls_autoslayer_start status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("plan_launcher_sha256_matches status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("plan_hook_id_present status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("plan_source_workspace_schema_version_one status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("plan_source_workspace_policy_no_launch status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("plan_source_workspace_policy_runtime_proof_still_requires_launch status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("plan_source_workspace_report_matches_installed_game status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("summary_run_seeds_match_plan_seeds status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_summary_run_passed_true status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_event_kind_is_ancient status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_launch_true status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_launcher_sha256_matches_plan status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_passed_true status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_hash_present status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_current_iteration_log_under_evidence_dir status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_current_iteration_log_matches_after_launch_slice status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_expected_patch_count_in_current_log status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_audit_recomputed_clean status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_sts1_mode_log_check_mode_matches_plan status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_contains_ancient_id status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_event_sequence_observed status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_current_log_event_sequence_observed status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_event_room_traversal_observed status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("mismatches=0", passResult.Output, StringComparison.Ordinal);

            var planPath = Path.Combine(workdir, "autoslay-plan.json");
            var originalPlanJson = File.ReadAllText(planPath);
            File.WriteAllText(
                planPath,
                originalPlanJson.Replace("\"HookId\": \"SpirePlus.AutoSlayHarness.Start\"", "\"HookId\": \"\"", StringComparison.Ordinal));
            var hookFailResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.86",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.16",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(hookFailResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{hookFailResult.Output}{hookFailResult.Error}");
            Assert.Contains("plan_hook_id_present status=fail", hookFailResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_hook_id_matches_plan status=fail", hookFailResult.Output, StringComparison.Ordinal);
            File.WriteAllText(planPath, originalPlanJson);

            var originalSourceWorkspaceReportJson = File.ReadAllText(sourceWorkspaceReportPath);
            var schemaMissingSourceReportJson = Regex.Replace(
                originalSourceWorkspaceReportJson,
                "\\s*\"SchemaVersion\": 1,\\r?\\n",
                string.Empty,
                RegexOptions.CultureInvariant);
            File.WriteAllText(sourceWorkspaceReportPath, schemaMissingSourceReportJson);
            var schemaMissingSourceWorkspaceReportHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(sourceWorkspaceReportPath))).ToLowerInvariant();
            File.WriteAllText(
                planPath,
                originalPlanJson.Replace(sourceWorkspaceReportHash, schemaMissingSourceWorkspaceReportHash, StringComparison.Ordinal));
            var schemaFailResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.86",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.16",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(schemaFailResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{schemaFailResult.Output}{schemaFailResult.Error}");
            Assert.Contains("plan_source_workspace_check_hash_matches status=pass", schemaFailResult.Output, StringComparison.Ordinal);
            Assert.Contains("plan_source_workspace_schema_version_one status=fail", schemaFailResult.Output, StringComparison.Ordinal);
            File.WriteAllText(sourceWorkspaceReportPath, originalSourceWorkspaceReportJson);
            File.WriteAllText(planPath, originalPlanJson);

            File.WriteAllText(
                autoSlayLogPath,
                string.Join(
                    Environment.NewLine,
                    $"12:00:00.000 [INFO] [AutoSlay] Starting run with seed={seed}",
                    "12:00:01.000 [INFO] [AutoSlay] Entering Event room (Act 1, Floor 2)",
                    "12:00:02.000 [INFO] [AutoSlay] Action: Selecting event option: VAKUU (option: contract)",
                    $"12:00:03.000 [INFO] [AutoSlay] Run completed successfully with seed={seed}"));
            var updatedAutoSlayLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(autoSlayLogPath))).ToLowerInvariant();
            var summaryPath = Path.Combine(workdir, "autoslay-summary.json");
            File.WriteAllText(
                summaryPath,
                File.ReadAllText(summaryPath).Replace(autoSlayLogHash, updatedAutoSlayLogHash, StringComparison.Ordinal));
            File.WriteAllText(
                runResultPath,
                File.ReadAllText(runResultPath).Replace(autoSlayLogHash, updatedAutoSlayLogHash, StringComparison.Ordinal));
            var failResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.86",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.16",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");

            Assert.True(failResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{failResult.Output}{failResult.Error}");
            Assert.Contains("run_0001_autoslay_log_hash_matches status=pass", failResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_current_log_event_sequence_observed status=pass", failResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_event_sequence_observed status=fail", failResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_event_room_traversal_observed status=fail", failResult.Output, StringComparison.Ordinal);
            Assert.Contains("batch_event_room_traversal_observed status=fail", failResult.Output, StringComparison.Ordinal);
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
        var offset = System.Text.Encoding.UTF8.GetByteCount(fullLog);
        File.WriteAllText(
            Path.Combine(iterationDir, "iteration-result.json"),
            $$"""
            {
              "Iteration": {{iteration}},
              "Passed": false,
              "Command": {{JsonSerializer.Serialize(command)}},
              "ScenarioTag": {{JsonSerializer.Serialize(scenarioTag)}},
              "OwnerArea": {{JsonSerializer.Serialize(ownerArea)}},
              "LogScanOffsetBytes": {{offset}},
              "FailureReasonCodes": {{failureReasonCodesJson}},
              "HangSignals": {{hangSignalsJson}}
            }
            """);
        File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), fullLog + currentLog);
        var currentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
        File.WriteAllText(currentLogPath, currentLog);
        File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), ToBoundAuditJson(currentLogPath, auditJson));
    }

    private static string BuildSts1ModeRuntimeLog(string mode)
    {
        var verifier = AssertRepoFileExists("scripts", "check-sts1-enabled-mode-runtime-log.ps1");
        var expectedPath = Path.Combine(Path.GetTempPath(), "sts1-runtime-expected-" + Guid.NewGuid().ToString("N") + ".json");

        try
        {
            var result = RunPowerShell(verifier, "-Mode", mode, "-PrintExpected", "-OutFile", expectedPath);
            Assert.True(result.ExitCode == 0, $"Expected-shape command failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(expectedPath));
            var tuples = document.RootElement.GetProperty("ExpectedRegistrationTuples").EnumerateArray().Select(item => item.GetString()!).ToArray();
            var lines = new List<string>
            {
                "v0.1.0-private-beta.86",
                "release = v0.107.0",
                "RitsuLib Version: 0.4.16 [compat branch: 0.107.0]",
                "Feature Sts1Events bootstrap=enabled, live=Enabled",
                mode == "CanaryOnly"
                    ? "StS1 events CanaryOnly mode: registering 4 canary events."
                    : "StS1 events AdditiveBatch1 mode: registering 10 verified-scope events.",
                mode == "CanaryOnly"
                    ? "[StS1 Events] Registering canary events"
                    : "[StS1 Events] Registering AdditiveBatch1 events"
            };

            foreach (var tuple in tuples)
            {
                var parts = tuple.Split(':');
                Assert.True(parts.Length == 3, $"Unexpected registration tuple: {tuple}");
                lines.Add(parts[0] == "ActEvent"
                    ? $"[StS1 Events] Registered act event: {parts[2]} -> {parts[1]}"
                    : $"[StS1 Events] Registered shared event: {parts[2]}");
            }

            lines.Add(mode == "CanaryOnly"
                ? "[StS1 Events] Canary events registered successfully."
                : "[StS1 Events] AdditiveBatch1 events registered successfully.");

            return string.Join("\r\n", lines) + "\r\n";
        }
        finally
        {
            if (File.Exists(expectedPath))
            {
                File.Delete(expectedPath);
            }
        }
    }

    private static void WriteSts1RuntimePacketState(string evidenceRoot, string mode)
    {
        File.WriteAllText(Path.Combine(evidenceRoot, "settings.save.before"), "{}");
        File.WriteAllText(Path.Combine(evidenceRoot, "game-release-info.json"), """{"version":"0.107.0"}""");
        Directory.CreateDirectory(Path.Combine(evidenceRoot, "mods"));
        File.WriteAllText(
            Path.Combine(evidenceRoot, "session-state.json"),
            $$"""
            {
              "AllowedModIds": ["BaseLib", "STS2-RitsuLib", "EZMicroBalance"],
              "DisableSpirePlus": false,
              "MoveOtherMods": true,
              "MoveCurrentRuns": true,
              "MovedMods": [],
              "MovedCurrentRuns": [],
              "GameRoot": {{JsonSerializer.Serialize(evidenceRoot)}},
              "ModsRoot": {{JsonSerializer.Serialize(Path.Combine(evidenceRoot, "mods"))}},
              "LogPath": {{JsonSerializer.Serialize(Path.Combine(evidenceRoot, "godot.log.after-launch"))}},
              "Sts1EventModeEnvironment": {{JsonSerializer.Serialize(mode)}},
              "Sts1UnsafeModeEnvironment": ""
            }
            """);
        File.WriteAllText(
            Path.Combine(evidenceRoot, "restore-state.json"),
            """
            {
              "RestoredAt": "2026-06-18T00:00:00.0000000Z",
              "RestoredModCount": 0,
              "RestoredCurrentRunCount": 0,
              "SettingsHashAfterRestore": "same",
              "SettingsBackupHashAfterRestore": "same"
            }
            """);
    }

    private static string ToBoundAuditJson(string currentLogPath, string auditJson)
    {
        JsonDocument auditDocument;
        try
        {
            auditDocument = JsonDocument.Parse(auditJson);
        }
        catch (JsonException)
        {
            return auditJson;
        }

        using (auditDocument)
        {
            var auditScript = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
            var result = RunPowerShell(auditScript, "-Path", currentLogPath);
            if (result.ExitCode != 0)
            {
                throw new InvalidOperationException($"Audit fixture generation failed:{Environment.NewLine}{result.Output}{result.Error}");
            }

            return result.Output;
        }
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
        const string ownerArea = "Ancients.Vakuu.FightOptionSetup";
        const string ackPattern = "\\[SPIREPLUS-EVIDENCE\\]\\s+VakuuFight\\s+fight_option_shown\\b";

        var iterationDir = Path.Combine(evidenceRoot, "iteration-0001");
        var shadowDir = Path.Combine(iterationDir, "shadow");
        Directory.CreateDirectory(iterationDir);
        Directory.CreateDirectory(shadowDir);

        var retainedCurrentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
        var retainedProbeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
        var sourceWorkspaceCheckPath = Path.Combine(evidenceRoot, "local-godot-source-workspace-check.json");
        var resultCurrentLogPath = useShadowResultPaths ? Path.Combine(shadowDir, "godot.log.current-iteration") : retainedCurrentLogPath;
        var resultProbeSamplesPath = useShadowResultPaths ? Path.Combine(shadowDir, "runtime-probe-samples.json") : retainedProbeSamplesPath;
        var currentLog = """
            [Startup] Time to main menu
            [INFO] [EZMicroBalance] [Patcher - SpirePlus] Patch application complete: 25 applied, 0 ignored, 0 failed, 25 total
            [INFO] [EZMicroBalance] ModPatcher applied 25 patches (25 registered).
            v0.1.0-private-beta.86
            [SPIREPLUS-EVIDENCE] VakuuFight fight_option_shown
            """;
        var probeSamples = """[{"ProcessObserved":true,"HungWindow":false,"Responding":true,"StaleProcessCount":0}]""";

        File.WriteAllText(retainedCurrentLogPath, currentLog);
        File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), currentLog);
        File.WriteAllText(retainedProbeSamplesPath, probeSamples);
        File.WriteAllText(Path.Combine(shadowDir, "godot.log.current-iteration"), currentLog);
        File.WriteAllText(Path.Combine(shadowDir, "runtime-probe-samples.json"), probeSamples);
        var retainedCurrentLogLength = new FileInfo(retainedCurrentLogPath).Length;
        var retainedCurrentLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(retainedCurrentLogPath))).ToLowerInvariant();
        File.WriteAllText(
            Path.Combine(iterationDir, "godot-log-audit.json"),
            $$"""
            {
              "Path": {{JsonSerializer.Serialize(retainedCurrentLogPath)}},
              "Length": {{retainedCurrentLogLength}},
              "Sha256": {{JsonSerializer.Serialize(retainedCurrentLogHash)}},
              "Clean": true,
              "SignatureHits": []
            }
            """);
        File.WriteAllText(
            Path.Combine(iterationDir, "sts1-mode-log-check.json"),
            $$"""
            {
              "Mode": "Off",
              "LogPath": {{JsonSerializer.Serialize(retainedCurrentLogPath)}},
              "LogLength": {{retainedCurrentLogLength}},
              "LogSha256": {{JsonSerializer.Serialize(retainedCurrentLogHash)}},
              "Mismatches": [],
              "Checks": [{ "Passed": true }]
            }
            """);
        File.WriteAllText(
            sourceWorkspaceCheckPath,
            """
            {
              "Passed": true,
              "SourceRoot": "D:\\Game\\FOTN\\dev-the-spire\\source code",
              "Game": { "Version": "v0.107.0", "Commit": "fixture" },
              "RecoveredSource": {
                "Version": "v0.107.0",
                "Commit": "fixture",
                "MatchesInstalledGame": true,
                "Disposition": "current-source-match"
              },
              "EvidenceUsePolicy": {
                "NotRuntimeProof": true,
                "LocalSourceReferenceOnly": true,
                "AuthorizedLocalInstallOnly": true,
                "ThirdPartyDumpsProhibited": true,
                "RefreshSourceSnapshotBeforeCurrentApiClaims": false
              },
              "Mismatches": []
            }
            """);
        var sourceWorkspaceCheckHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(sourceWorkspaceCheckPath))).ToLowerInvariant();

        File.WriteAllText(
            Path.Combine(evidenceRoot, "monkey-plan.json"),
            $$"""
            {
              "HangProbeSchemaVersion": 1,
              "Launch": true,
              "Iterations": 1,
              "Scenario": "VakuuFightSmoke",
              "CommandSelectionMode": "RoundRobin",
              "Sts1EventMode": "Off",
              "CommandCorpusSource": "scenario:VakuuFightSmoke",
              "ObservationIntervalSeconds": 2,
              "UnresponsiveSampleThreshold": 3,
              "NoLogGrowthTimeoutSeconds": 90,
              "ProcessProbe": { "ProcessName": "SlayTheSpire2", "FailsOnlyAfterConsecutiveUnresponsiveSamples": true },
              "LogGrowthProbe": { "StartupFailsOnNoGrowth": true },
              "SourceWorkspaceCheckPath": {{JsonSerializer.Serialize(sourceWorkspaceCheckPath)}},
              "SourceWorkspaceCheckSha256": {{JsonSerializer.Serialize(sourceWorkspaceCheckHash)}},
              "SourceWorkspace": {
                "Checked": true,
                "ReportPath": {{JsonSerializer.Serialize(sourceWorkspaceCheckPath)}},
                "ReportSha256": {{JsonSerializer.Serialize(sourceWorkspaceCheckHash)}},
                "Passed": true,
                "SourceVersion": "v0.107.0",
                "SourceCommit": "fixture",
                "InstalledGameVersion": "v0.107.0",
                "Disposition": "current-source-match",
                "MatchesInstalledGame": true,
                "RefreshSourceSnapshotBeforeCurrentApiClaims": false,
                "NotRuntimeProof": true
              },
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
              "StaleProcessObservedCount": 0,
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
              "StaleProcessObserved": false,
              "StaleProcessCount": 0,
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
                "StaleProcessObserved": false,
                "MaxStaleProcessCount": 0,
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
                "StaleProcessObserved": false,
                "MaxStaleProcessCount": 0,
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

    private static string ClearCommandAckPatternJsonValues(string json) =>
        Regex.Replace(
            json,
            "\"(?<name>CommandAckPattern|Pattern)\"\\s*:\\s*\"(?:\\\\.|[^\"])*\"",
            match => $"\"{match.Groups["name"].Value}\": \"\"");

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
