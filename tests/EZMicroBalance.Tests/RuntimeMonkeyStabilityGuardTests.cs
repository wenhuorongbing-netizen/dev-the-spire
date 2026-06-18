using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void RuntimeMonkeyPacketCheckerIsNoLaunchAndFailsOnRequiredEvidenceDrift()
    {
        var checker = ReadRepoText("scripts", "check-spire-plus-runtime-monkey-packet.ps1");

        AssertSourceContains(
            checker,
            "[string]$EvidenceDir",
            "[int]$ExpectedIterations",
            "[string]$ExpectedPackageVersion",
            "[string]$ExpectedGameVersion",
            "[string]$ExpectedRitsuLibVersion",
            "[string]$ExpectedRitsuCompatBranch",
            "[int]$ExpectedPatchCount",
            "[switch]$RequireCurrentSourceSnapshot",
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
            "Test-BytePrefix",
            "Test-CurrentSliceBinding",
            "plan_unresponsive_sample_threshold_positive",
            "RuntimeProbeSamplesPath",
            "runtime_probe_samples_log_growth_matches_runtime_observation",
            "runtime_observation_log_length_growth_matches_log_grew",
            "GodotLogBeforePath",
            "GodotLogBeforeLengthBytes",
            "GodotLogBeforeSha256",
            "GodotLogAfterLaunchPath",
            "GodotLogAfterLaunchLengthBytes",
            "GodotLogAfterLaunchSha256",
            "GodotLogCurrentIterationPath",
            "GodotLogCurrentIterationLengthBytes",
            "GodotLogCurrentIterationSha256",
            "CurrentIterationLogPath",
            "LogScanOffsetBytes",
            "prepare-output.json",
            "LiveSessionPrepareOutputPath",
            "LiveSessionPrepareOutputSha256",
            "LiveSessionSelectedGameProcessId",
            "LiveSessionSelectedGameProcessStartTimeUtc",
            "LiveSessionSelectedGameProcessPath",
            "GameProcessIdMatchesLiveSession",
            "GameProcessStartTimeMatchesLiveSession",
            "GameProcessPathMatchesLiveSession",
            "ConvertTo-NormalizedPathOrEmpty",
            "live_session_prepare_output_path_matches_retained_file",
            "live_session_prepare_output_sha256_matches_retained_file",
            "prepare_output_selected_game_process_id_matches_result",
            "result_live_session_launcher_kind_steam_app_launch",
            "result_live_session_launch_argument_list_matches_sts2",
            "result_live_session_pid_attribution_passed",
            "result_live_session_prelaunch_slay_process_count_zero",
            "result_live_session_selected_game_process_id_matches_result",
            "result_game_process_id_matches_live_session",
            "result_game_process_start_time_matches_live_session",
            "result_game_process_path_matches_live_session",
            "result_game_process_start_time_after_live_session_launch",
            "prepare_output_launch_kind_steam_app_launch",
            "prepare_output_launch_argument_list_matches_sts2",
            "prepare_output_selected_game_process_start_time_matches_result",
            "runtime_probe_samples_under_iteration_dir",
            "current_iteration_log_under_iteration_dir",
            "runtime_probe_samples_leaf_expected",
            "current_iteration_log_leaf_expected",
            "runtime_probe_samples_path_matches_retained_file",
            "current_iteration_log_path_matches_retained_file",
            "runtime_probe_samples_phase_field_present",
            "runtime_probe_samples_allowed_phase_values",
            "runtime_probe_samples_startup_main_menu_phase_observed",
            "runtime_probe_samples_post_command_runtime_phase_observed",
            "runtime_probe_samples_startup_count_matches_main_menu_observation",
            "runtime_probe_samples_runtime_count_matches_runtime_observation",
            "runtime_probe_samples_process_id_field_present",
            "runtime_probe_samples_process_start_time_field_present",
            "runtime_probe_samples_process_path_field_present",
            "runtime_probe_samples_expected_process_id_field_present",
            "runtime_probe_samples_expected_process_start_time_field_present",
            "runtime_probe_samples_expected_process_path_field_present",
            "runtime_probe_samples_process_id_match_field_present",
            "runtime_probe_samples_process_start_time_match_field_present",
            "runtime_probe_samples_process_path_match_field_present",
            "runtime_probe_samples_process_identity_match_field_present",
            "runtime_probe_samples_all_match_live_session_identity",
            "runtime_probe_samples_process_observed_field_present",
            "runtime_probe_samples_main_window_observed_field_present",
            "runtime_probe_samples_hung_window_field_present",
            "runtime_probe_samples_responding_field_present",
            "runtime_probe_samples_stale_process_count_field_present",
            "runtime_probe_samples_current_process_count_field_present",
            "runtime_probe_samples_unknown_start_time_count_field_present",
            "runtime_probe_samples_ambiguous_current_process_count_field_present",
            "runtime_probe_samples_no_unknown_start_times",
            "runtime_probe_samples_no_ambiguous_current_processes",
            "runtime_probe_samples_single_current_process",
            "runtime_probe_samples_single_positive_process_id",
            "runtime_probe_samples_single_process_start_time",
            "runtime_probe_samples_single_process_path",
            "runtime_probe_samples_process_id_matches_result",
            "runtime_probe_samples_process_start_time_matches_result",
            "runtime_probe_samples_process_path_matches_result",
            "runtime_probe_samples_expected_process_id_matches_live_session",
            "runtime_probe_samples_expected_process_start_time_matches_live_session",
            "runtime_probe_samples_expected_process_path_matches_live_session",
            "plan_expected_game_version_matches",
            "plan_expected_ritsulib_version_matches",
            "plan_expected_ritsu_compat_branch_matches",
            "expected_game_version_in_log",
            "expected_ritsulib_marker_in_log",
            "plan_source_workspace_required_authorized_source_origin_verified",
            "godot_log_before_exists",
            "godot_log_before_under_iteration_dir",
            "godot_log_before_leaf_expected",
            "godot_log_before_path_matches_retained_file",
            "godot_log_before_length_matches_retained_file",
            "godot_log_before_sha256_matches_retained_file",
            "result_before_log_copied",
            "log_scan_offset_recorded",
            "log_scan_offset_within_full_log",
            "log_scan_offset_matches_before_length",
            "current_iteration_log_matches_after_launch_prefix",
            "current_iteration_log_matches_after_launch_slice",
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
            "runtime_probe_samples_no_unknown_start_times",
            "runtime_probe_samples_no_ambiguous_current_processes",
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
            "plan_source_workspace_authorized_source_origin_field_present",
            "plan_source_workspace_origin_matches_installed_game_pck_field_present",
            "plan_source_workspace_report_authorized_source_origin_field_present",
            "plan_source_workspace_report_origin_matches_installed_game_pck_field_present",
            "plan_source_workspace_required_current_snapshot_origin_verified",
            "plan_command_scenario_matrix_present",
            "summary_passed",
            "summary_failed_iterations_zero",
            "summary_process_exit_count_zero",
            "summary_main_window_missing_count_zero",
            "summary_godot_log_before_missing_count_zero",
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
            "runtime_observation_log_growth_requirement_matches_command",
            "runtime_observation_log_growth_not_required",
            "runtime_observation_log_grew",
            "runtime_observation_no_log_growth_timeout",
            "responsiveness_probe_passed",
            "command_ack_observed",
            "failure_reason_codes_empty",
            "hang_signals_empty",
            "godot_log_before_exists",
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
    public void RuntimeFailureAnalyzerRoutesLiveSessionBindingFailuresToHarness()
    {
        var analyzer = ReadRepoText("scripts", "analyze-spire-plus-runtime-failure.ps1");

        AssertSourceContains(
            analyzer,
            "'live_session_prepare_output_missing'",
            "'live_session_launch_metadata_missing'",
            "'live_session_pid_attribution_missing'",
            "'live_session_pid_attribution_failed'",
            "'game_process_start_time_unbound'",
            "'game_process_path_missing'",
            "'game_process_id_mismatch'",
            "'game_process_start_time_mismatch'",
            "'game_process_path_mismatch'",
            "$runtimeMonkeyProbeEvidenceInvalid",
            "'runtime_monkey_probe_samples_path_missing'",
            "'runtime_monkey_probe_samples_missing'",
            "'runtime_monkey_probe_samples_empty'",
            "'runtime_monkey_probe_samples_incomplete'",
            "'runtime_monkey_probe_samples_invalid'",
            "'runtime_monkey_probe_startup_phase_missing'",
            "'runtime_monkey_probe_runtime_phase_missing'",
            "'runtime_monkey_probe_unknown_phase'",
            "'runtime_monkey_probe_startup_sample_count_mismatch'",
            "'runtime_monkey_probe_runtime_sample_count_mismatch'",
            "'runtime_monkey_probe_runtime_log_growth_mismatch'",
            "OwnerArea 'RuntimeHarness'");
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
            Assert.Contains("iteration-0001_godot_log_before_path_matches_retained_file status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_godot_log_after_launch_path_matches_retained_file status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_godot_current_iteration_log_path_matches_retained_file status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsResultPathsOutsideIterationDirectory()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var resultPath = Path.Combine(workdir, "iteration-0001", "iteration-result.json");
            var resultJson = File.ReadAllText(resultPath);
            foreach (var (propertyName, escapedPath) in new[]
                     {
                         ("GodotLogBeforePath", "../godot.log.before"),
                         ("GodotLogAfterLaunchPath", "../godot.log.after-launch"),
                         ("GodotLogCurrentIterationPath", "../godot.log.current-iteration"),
                         ("CurrentIterationLogPath", "../godot.log.current-iteration"),
                         ("RuntimeProbeSamplesPath", "../runtime-probe-samples.json"),
                     })
            {
                resultJson = Regex.Replace(
                    resultJson,
                    $"\"{propertyName}\"\\s*:\\s*\"(?:\\\\.|[^\"])*\"",
                    $"\"{propertyName}\": \"{escapedPath}\"",
                    RegexOptions.CultureInvariant);
            }

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
            Assert.Contains("iteration-0001_godot_log_before_under_iteration_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_godot_log_after_launch_under_iteration_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_godot_current_iteration_log_under_iteration_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_current_iteration_log_under_iteration_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_under_iteration_dir status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerReportsMalformedResultPathsAsFailedRows()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var resultPath = Path.Combine(workdir, "iteration-0001", "iteration-result.json");
            var resultJson = File.ReadAllText(resultPath);
            foreach (var propertyName in new[]
                     {
                         "LiveSessionPrepareOutputPath",
                         "LiveSessionEvidenceDir",
                         "GodotLogBeforePath",
                         "GodotLogAfterLaunchPath",
                         "GodotLogCurrentIterationPath",
                         "CurrentIterationLogPath",
                         "RuntimeProbeSamplesPath",
                     })
            {
                resultJson = Regex.Replace(
                    resultJson,
                    $"\"{propertyName}\"\\s*:\\s*\"(?:\\\\.|[^\"])*\"",
                    $"\"{propertyName}\": \"\\u0000bad-{propertyName}\"",
                    RegexOptions.CultureInvariant);
            }

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
            Assert.Contains("iteration-0001_live_session_prepare_output_under_iteration_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_live_session_prepare_output_path_matches_retained_file status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_result_live_session_evidence_dir_matches_iteration status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_godot_log_before_under_iteration_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_godot_log_after_launch_under_iteration_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_godot_current_iteration_log_under_iteration_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_current_iteration_log_under_iteration_dir status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_under_iteration_dir status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerReportsMalformedSts1ModeLogPathAsFailedRow()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var sts1ModeLogCheckPath = Path.Combine(workdir, "iteration-0001", "sts1-mode-log-check.json");
            var sts1ModeLogCheckJson = Regex.Replace(
                File.ReadAllText(sts1ModeLogCheckPath),
                "\"LogPath\"\\s*:\\s*\"(?:\\\\.|[^\"])*\"",
                "\"LogPath\": \"\\u0000bad-sts1-log\"",
                RegexOptions.CultureInvariant);
            File.WriteAllText(sts1ModeLogCheckPath, sts1ModeLogCheckJson);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_sts1_mode_log_check_log_path_matches_current_iteration_log status=fail", result.Output, StringComparison.Ordinal);
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
            Assert.Contains("iteration-0001_current_iteration_log_matches_after_launch_slice status=fail", result.Output, StringComparison.Ordinal);
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
            var resultJson = Regex.Replace(
                File.ReadAllText(resultPath),
                "\"LogScanOffsetBytes\": \\d+",
                "\"LogScanOffsetBytes\": 999999");
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
            Assert.Contains("iteration-0001_log_scan_offset_matches_before_length status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsRuntimeObservationWithoutLogGrowth()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var resultPath = Path.Combine(workdir, "iteration-0001", "iteration-result.json");
            var resultJson = File.ReadAllText(resultPath)
                .Replace("\"PostCommandLogProbePassed\": true", "\"PostCommandLogProbePassed\": false", StringComparison.Ordinal)
                .Replace("\"NoLogGrowthTimeoutExceeded\": false", "\"NoLogGrowthTimeoutExceeded\": true", StringComparison.Ordinal)
                .Replace("\"LogGrew\": true", "\"LogGrew\": false", StringComparison.Ordinal);
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
            Assert.Contains("iteration-0001_post_command_log_probe_passed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_observation_log_grew status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_observation_no_log_growth_timeout status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsRuntimeProbeLogGrowthMismatch()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var probeSamplesPath = Path.Combine(workdir, "iteration-0001", "runtime-probe-samples.json");
            var probeSamplesJson = Regex.Replace(
                File.ReadAllText(probeSamplesPath),
                "(\"Phase\":\"PostCommandRuntime\"[^}]*\"LogLengthBytes\":)\\d+",
                "${1}1",
                RegexOptions.CultureInvariant);
            File.WriteAllText(probeSamplesPath, probeSamplesJson);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_runtime_probe_samples_log_growth_matches_runtime_observation status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_observation_log_length_growth_matches_log_grew status=pass", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsProbeExpectedIdentityDrift()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var probeSamplesPath = Path.Combine(workdir, "iteration-0001", "runtime-probe-samples.json");
            var originalGamePath = Path.Combine(workdir, "SlayTheSpire2.exe");
            var driftedGamePath = Path.Combine(workdir, "other", "SlayTheSpire2.exe");
            var probeSamplesJson = File.ReadAllText(probeSamplesPath)
                .Replace(
                    "\"ExpectedGameProcessStartTimeUtc\":\"2026-06-18T00:00:05.0000000Z\"",
                    "\"ExpectedGameProcessStartTimeUtc\":\"2026-06-18T00:00:06.0000000Z\"",
                    StringComparison.Ordinal)
                .Replace(
                    $"\"ExpectedGameProcessPath\":{JsonSerializer.Serialize(originalGamePath)}",
                    $"\"ExpectedGameProcessPath\":{JsonSerializer.Serialize(driftedGamePath)}",
                    StringComparison.Ordinal);
            File.WriteAllText(probeSamplesPath, probeSamplesJson);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_runtime_probe_samples_expected_process_id_matches_live_session status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_expected_process_start_time_matches_live_session status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_expected_process_path_matches_live_session status=fail", result.Output, StringComparison.Ordinal);
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
            Assert.Contains("iteration-0001_runtime_probe_samples_sampled_at_field_present status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_log_exists_field_present status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_log_length_field_present status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_log_last_write_field_present status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsProbeSamplesMissingRuntimePhaseCoverage()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var probeSamplesPath = Path.Combine(workdir, "iteration-0001", "runtime-probe-samples.json");
            var probeSamplesJson = File.ReadAllText(probeSamplesPath)
                .Replace("\"Phase\":\"PostCommandRuntime\"", "\"Phase\":\"StartupMainMenu\"", StringComparison.Ordinal);
            File.WriteAllText(probeSamplesPath, probeSamplesJson);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");
            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_runtime_probe_samples_post_command_runtime_phase_observed status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_startup_count_matches_main_menu_observation status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_runtime_count_matches_runtime_observation status=fail", result.Output, StringComparison.Ordinal);
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
            var beforeLogPath = Path.Combine(iterationDir, "godot.log.before");
            var fullLogPath = Path.Combine(iterationDir, "godot.log.after-launch");
            var dirtyLog = File.ReadAllText(currentLogPath) + "[ERROR] TypeLoadException\r\n";
            File.WriteAllText(currentLogPath, dirtyLog);
            File.WriteAllText(fullLogPath, File.ReadAllText(beforeLogPath) + dirtyLog);
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
            "RuntimeProbeSamplesPath",
            "MainMenuObservation",
            "RuntimeObservation",
            "LogInitialLengthBytes",
            "LogFinalLengthBytes",
            "LogLengthBytes",
            "LogGrew",
            "NoLogGrowthTimeoutExceeded",
            "AutoSlayLogSha256",
            "RuntimeMonkeyRunArtifactsTrustedForOwner",
            "AutoSlaySidecarTrustedForOwner",
            "AutoSlayRunArtifactsTrustedForOwner",
            "AutoSlayProbeArtifactTrustedForOwner",
            "AutoSlayAuditArtifactTrustedForOwner",
            "AutoSlaySts1ModeArtifactTrustedForOwner",
            "GodotLogBeforeLengthBytes",
            "GodotLogBeforeSha256",
            "GodotLogAfterLaunchLengthBytes",
            "GodotLogAfterLaunchSha256",
            "GodotLogCurrentIterationLengthBytes",
            "GodotLogCurrentIterationSha256",
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
            "current_iteration_log_scan_offset_before_length_mismatch",
            "runtime_monkey_before_log_outside_iteration_dir",
            "runtime_monkey_after_launch_log_outside_iteration_dir",
            "runtime_monkey_current_iteration_log_outside_iteration_dir",
            "runtime_monkey_runtime_probe_samples_outside_iteration_dir",
            "runtime_monkey_before_log_not_retained_file",
            "runtime_monkey_after_launch_log_not_retained_file",
            "runtime_monkey_current_iteration_log_not_retained_file",
            "runtime_monkey_runtime_probe_samples_not_retained_file",
            "ConvertTo-NormalizedPathOrEmpty -Path $artifactPath",
            "runtime_monkey_probe_process_identity_mismatch",
            "ProcessIdentityMatchesExpected",
            "AutoSlayLogPath",
            "EventKind",
            "AncientId",
            "autoslay_event_kind_not_ancient",
            "autoslay_ancient_id_missing",
            "autoslay_sidecar_event_sequence_missing",
            "autoslay_run_result_path_outside_evidence_dir",
            "autoslay_sidecar_log_outside_run_dir",
            "autoslay_before_log_outside_run_dir",
            "autoslay_after_launch_log_outside_run_dir",
            "autoslay_current_iteration_log_outside_run_dir",
            "autoslay_runtime_probe_samples_outside_run_dir",
            "autoslay_godot_log_audit_outside_run_dir",
            "autoslay_sts1_mode_log_check_outside_run_dir",
            "autoslay_sidecar_log_hash_missing",
            "autoslay_sidecar_log_hash_mismatch",
            "autoslay_current_log_event_sequence_missing",
            "autoslay_sidecar_ancient_id_missing",
            "autoslay_current_log_ancient_id_missing",
            "autoslay_runtime_probe_samples_missing",
            "autoslay_runtime_probe_samples_incomplete",
            "autoslay_runtime_probe_main_menu_phase_missing",
            "autoslay_runtime_probe_runtime_phase_missing",
            "autoslay_runtime_probe_process_identity_unstable",
            "autoslay_runtime_probe_process_identity_mismatch",
            "autoslay_runtime_probe_log_growth_mismatch",
            "autoslay_godot_log_metadata_missing",
            "autoslay_godot_log_metadata_mismatch",
            "autoslay_run_result_start_timestamp_invalid",
            "autoslay_run_result_end_timestamp_invalid",
            "autoslay_run_result_timestamp_order_invalid",
            "autoslay_main_menu_observation_unhealthy",
            "autoslay_runtime_observation_unhealthy",
            "FailureReasonCodes",
            "HangSignals",
            "Get-JsonArrayValues",
            "Get-UnhealthyObservationFields",
            "Test-JsonFileParses",
            "EvidenceFiles",
            "Confidence",
            "ScenarioTag",
            "OwnerAreaHint",
            "OwnerAreaFromLog",
            "OwnerAreaFromCommand",
            "TriageDisposition",
            "HarnessBlockingFindingCount",
            "PackageBlockingFindingCount",
            "GameplayBlockingFindingCount",
            "HarnessBlockingFindings",
            "PackageBlockingFindings",
            "GameplayBlockingFindings",
            "RecommendedNextActions",
            "triage_disposition=",
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
            "runtime_log_stalled",
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
    public void RuntimeFailureAnalyzerReportsMissingGameNativeAutoSlayObservations()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        var runDir = Path.Combine(workdir, "run-0001");
        Directory.CreateDirectory(runDir);

        try
        {
            File.WriteAllText(
                Path.Combine(runDir, "run-result.json"),
                """
                {
                  "RunnerKind": "GameNativeAutoSlay",
                  "Iteration": 1,
                  "Seed": "TEST-SEED-001",
                  "EventKind": "Ancient",
                  "AncientId": "Urda",
                  "Invocation": "AutoSlayer.Start(seed, logFile)",
                  "Command": "AutoSlayer.Start(seed, logFile)"
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", runDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_main_menu_observation_missing");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_observation_missing");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_main_menu_observation_unhealthy");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_observation_unhealthy");
            Assert.Contains(
                findings,
                finding =>
                    finding.GetProperty("Signal").GetString() == "autoslay_main_menu_observation_missing" &&
                    finding.GetProperty("Rationale").GetString()?.Contains("missing", StringComparison.Ordinal) == true);
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
    public void RuntimeFailureAnalyzerReportsGameNativeAutoSlayProbePhaseAndTimestampDefects()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        var runDir = Path.Combine(workdir, "run-0001");
        Directory.CreateDirectory(runDir);

        try
        {
            File.WriteAllText(
                Path.Combine(runDir, "runtime-probe-samples.json"),
                """
                [
                  {
                    "Phase": "runtime",
                    "SampledAt": "2026-06-18T10:00:20Z",
                    "LogExists": true,
                    "LogLengthBytes": 200,
                    "LogLastWriteTimeUtc": "2026-06-18T10:00:20Z",
                    "ProcessId": 4242,
                    "ProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                    "ProcessPath": "C:/Games/SlayTheSpire2.exe",
                    "ExpectedGameProcessId": 4242,
                    "ExpectedGameProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                    "ExpectedGameProcessPath": "C:/Games/SlayTheSpire2.exe",
                    "ProcessIdMatchesExpected": true,
                    "ProcessStartTimeMatchesExpected": true,
                    "ProcessPathMatchesExpected": true,
                    "ProcessIdentityMatchesExpected": true,
                    "ProcessObserved": true,
                    "MainWindowObserved": true,
                    "HungWindow": false,
                    "Responding": true,
                    "StaleProcessCount": 0,
                    "CurrentProcessCount": 1,
                    "UnknownStartTimeProcessCount": 0,
                    "AmbiguousCurrentProcessCount": 0
                  }
                ]
                """);
            File.WriteAllText(
                Path.Combine(runDir, "run-result.json"),
                """
                {
                  "RunnerKind": "GameNativeAutoSlay",
                  "Iteration": 1,
                  "Seed": "TEST-SEED-001",
                  "EventKind": "Ancient",
                  "AncientId": "Urda",
                  "Invocation": "AutoSlayer.Start(seed, logFile)",
                  "Command": "AutoSlayer.Start(seed, logFile)",
                  "Passed": true,
                  "FailureReasonCodes": [],
                  "HangSignals": [],
                  "StartTimestamp": "2026-06-18T10:01:00Z",
                  "EndTimestamp": "2026-06-18T10:00:00Z",
                  "RuntimeProbeSamplesPath": "runtime-probe-samples.json",
                  "MainMenuObservation": {
                    "Passed": true,
                    "MainMenuReached": true,
                    "ProcessObserved": true,
                    "ProcessExitedAfterObservation": false,
                    "HungWindowDetected": false,
                    "StaleProcessObserved": false,
                    "MaxStaleProcessCount": 0,
                    "NoLogGrowthTimeoutExceeded": false,
                    "LogObserved": true
                  },
                  "RuntimeObservation": {
                    "Passed": true,
                    "ProcessObserved": true,
                    "ProcessExitedAfterObservation": false,
                    "HungWindowDetected": false,
                    "StaleProcessObserved": false,
                    "MaxStaleProcessCount": 0,
                    "NoLogGrowthTimeoutExceeded": false,
                    "LogGrew": true,
                    "LogObserved": true,
                    "LogInitialLengthBytes": 100,
                    "LogFinalLengthBytes": 200
                  }
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", runDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_main_menu_phase_missing");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_runtime_phase_missing");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_run_result_timestamp_order_invalid");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_run_result_start_timestamp_invalid");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_run_result_end_timestamp_invalid");
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
    public void RuntimeFailureAnalyzerReportsGameNativeAutoSlayProbeLogGrowthMismatch()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        var runDir = Path.Combine(workdir, "run-0001");
        Directory.CreateDirectory(runDir);

        try
        {
            File.WriteAllText(
                Path.Combine(runDir, "runtime-probe-samples.json"),
                """
                [
                  {
                    "Phase": "main-menu",
                    "SampledAt": "2026-06-18T10:00:10Z",
                    "LogExists": true,
                    "LogLengthBytes": 150,
                    "LogLastWriteTimeUtc": "2026-06-18T10:00:10Z",
                    "ProcessId": 4242,
                    "ProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                    "ProcessPath": "C:/Games/SlayTheSpire2.exe",
                    "ExpectedGameProcessId": 4242,
                    "ExpectedGameProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                    "ExpectedGameProcessPath": "C:/Games/SlayTheSpire2.exe",
                    "ProcessIdMatchesExpected": true,
                    "ProcessStartTimeMatchesExpected": true,
                    "ProcessPathMatchesExpected": true,
                    "ProcessIdentityMatchesExpected": true,
                    "ProcessObserved": true,
                    "MainWindowObserved": true,
                    "HungWindow": false,
                    "Responding": true,
                    "StaleProcessCount": 0,
                    "CurrentProcessCount": 1,
                    "UnknownStartTimeProcessCount": 0,
                    "AmbiguousCurrentProcessCount": 0
                  },
                  {
                    "Phase": "runtime",
                    "SampledAt": "2026-06-18T10:00:20Z",
                    "LogExists": true,
                    "LogLengthBytes": 150,
                    "LogLastWriteTimeUtc": "2026-06-18T10:00:20Z",
                    "ProcessId": 4242,
                    "ProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                    "ProcessPath": "C:/Games/SlayTheSpire2.exe",
                    "ExpectedGameProcessId": 4242,
                    "ExpectedGameProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                    "ExpectedGameProcessPath": "C:/Games/SlayTheSpire2.exe",
                    "ProcessIdMatchesExpected": true,
                    "ProcessStartTimeMatchesExpected": true,
                    "ProcessPathMatchesExpected": true,
                    "ProcessIdentityMatchesExpected": true,
                    "ProcessObserved": true,
                    "MainWindowObserved": true,
                    "HungWindow": false,
                    "Responding": true,
                    "StaleProcessCount": 0,
                    "CurrentProcessCount": 1,
                    "UnknownStartTimeProcessCount": 0,
                    "AmbiguousCurrentProcessCount": 0
                  }
                ]
                """);
            File.WriteAllText(
                Path.Combine(runDir, "run-result.json"),
                """
                {
                  "RunnerKind": "GameNativeAutoSlay",
                  "Iteration": 1,
                  "Seed": "TEST-SEED-001",
                  "EventKind": "Ancient",
                  "AncientId": "Urda",
                  "Invocation": "AutoSlayer.Start(seed, logFile)",
                  "Command": "AutoSlayer.Start(seed, logFile)",
                  "Passed": true,
                  "FailureReasonCodes": [],
                  "HangSignals": [],
                  "ProcessId": 4242,
                  "StartTimestamp": "2026-06-18T10:00:00Z",
                  "EndTimestamp": "2026-06-18T10:01:00Z",
                  "RuntimeProbeSamplesPath": "runtime-probe-samples.json",
                  "MainMenuObservation": {
                    "Passed": true,
                    "MainMenuReached": true,
                    "ProcessObserved": true,
                    "ProcessExitedAfterObservation": false,
                    "HungWindowDetected": false,
                    "StaleProcessObserved": false,
                    "MaxStaleProcessCount": 0,
                    "NoLogGrowthTimeoutExceeded": false,
                    "LogObserved": true
                  },
                  "RuntimeObservation": {
                    "Passed": true,
                    "ProcessObserved": true,
                    "ProcessExitedAfterObservation": false,
                    "HungWindowDetected": false,
                    "StaleProcessObserved": false,
                    "MaxStaleProcessCount": 0,
                    "NoLogGrowthTimeoutExceeded": false,
                    "LogGrew": true,
                    "LogObserved": true,
                    "LogInitialLengthBytes": 150,
                    "LogFinalLengthBytes": 220
                  }
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", runDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "autoslay_runtime_probe_log_growth_mismatch");
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
    public void RuntimeFailureAnalyzerRejectsRuntimeMonkeyArtifactsOutsideIterationDirectory()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            Directory.CreateDirectory(iterationDir);
            const string beforeLog = "[INFO] stale full-log prefix before accepted scan offset\r\n";
            const string currentLog = "[SPIREPLUS-EVIDENCE] PreviewTransform [Spire Plus] Preview: Transform prediction\r\n";
            var offset = Encoding.UTF8.GetByteCount(beforeLog);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.before"), beforeLog);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), beforeLog + currentLog);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.current-iteration"), currentLog);
            var sharedCurrentLogPath = Path.Combine(workdir, "godot.log.current-iteration");
            File.WriteAllText(sharedCurrentLogPath, currentLog);
            File.WriteAllText(
                Path.Combine(workdir, "runtime-probe-samples.json"),
                """
                [
                  {
                    "Phase": "StartupMainMenu",
                    "SampledAt": "2026-06-18T10:00:05Z",
                    "LogExists": true,
                    "LogLengthBytes": 100,
                    "LogLastWriteTimeUtc": "2026-06-18T10:00:05Z",
                    "ProcessId": 4242,
                    "ProcessObserved": true,
                    "MainWindowObserved": true,
                    "HungWindow": false,
                    "Responding": true,
                    "StaleProcessCount": 0,
                    "CurrentProcessCount": 1,
                    "UnknownStartTimeProcessCount": 0,
                    "AmbiguousCurrentProcessCount": 0
                  }
                ]
                """);
            File.WriteAllText(
                Path.Combine(iterationDir, "iteration-result.json"),
                $$"""
                {
                  "Iteration": 1,
                  "HangProbeSchemaVersion": 1,
                  "Passed": false,
                  "Command": "spireplus_test_ancient URDA confirm",
                  "ScenarioTag": "ancient-ui",
                  "OwnerArea": "Runtime.Unknown",
                  "GodotLogBeforePath": "godot.log.before",
                  "GodotLogAfterLaunchPath": "godot.log.after-launch",
                  "GodotLogCurrentIterationPath": "../godot.log.current-iteration",
                  "RuntimeProbeSamplesPath": "../runtime-probe-samples.json",
                  "LogScanOffsetBytes": {{offset}},
                  "FailureReasonCodes": ["process_unresponsive"],
                  "HangSignals": ["process_unresponsive"],
                  "MainMenuObservation": { "Samples": 1 },
                  "RuntimeObservation": { "Samples": 0 }
                }
                """);
            File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), ToBoundAuditJson(sharedCurrentLogPath, """{"SignatureHits":[]}"""));
            WriteMonkeySummary(workdir, 1);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.Contains(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "runtime_monkey_current_iteration_log_outside_iteration_dir");
            Assert.Contains(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "runtime_monkey_runtime_probe_samples_outside_iteration_dir");
            Assert.NotEqual("PreviewTools", FindFindingOwner(iteration, "process_unresponsive"));
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
    public void RuntimeFailureAnalyzerRejectsRuntimeMonkeyArtifactsThatDoNotMatchRetainedFiles()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var shadowDir = Path.Combine(iterationDir, "shadow");
            Directory.CreateDirectory(shadowDir);
            const string beforeLog = "[INFO] stale full-log prefix before accepted scan offset\r\n";
            const string currentLog = "[SPIREPLUS-EVIDENCE] PreviewTransform [Spire Plus] Preview: Transform prediction\r\n";
            var offset = Encoding.UTF8.GetByteCount(beforeLog);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.before"), beforeLog);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), beforeLog);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.current-iteration"), string.Empty);
            var shadowBeforeLogPath = Path.Combine(shadowDir, "godot.log.before");
            var shadowAfterLaunchLogPath = Path.Combine(shadowDir, "godot.log.after-launch");
            var shadowCurrentLogPath = Path.Combine(shadowDir, "godot.log.current-iteration");
            var shadowProbeSamplesPath = Path.Combine(shadowDir, "runtime-probe-samples.json");
            File.WriteAllText(shadowBeforeLogPath, beforeLog);
            File.WriteAllText(shadowAfterLaunchLogPath, beforeLog + currentLog);
            File.WriteAllText(shadowCurrentLogPath, currentLog);
            File.WriteAllText(
                shadowProbeSamplesPath,
                """
                [
                  {
                    "Phase": "StartupMainMenu",
                    "SampledAt": "2026-06-18T10:00:05Z",
                    "LogExists": true,
                    "LogLengthBytes": 100,
                    "LogLastWriteTimeUtc": "2026-06-18T10:00:05Z",
                    "ProcessId": 4242,
                    "ProcessObserved": true,
                    "MainWindowObserved": true,
                    "HungWindow": false,
                    "Responding": true,
                    "StaleProcessCount": 0,
                    "CurrentProcessCount": 1,
                    "UnknownStartTimeProcessCount": 0,
                    "AmbiguousCurrentProcessCount": 0
                  }
                ]
                """);
            File.WriteAllText(
                Path.Combine(iterationDir, "iteration-result.json"),
                $$"""
                {
                  "Iteration": 1,
                  "HangProbeSchemaVersion": 1,
                  "Passed": false,
                  "Command": "spireplus_test_ancient URDA confirm",
                  "ScenarioTag": "ancient-ui",
                  "OwnerArea": "Runtime.Unknown",
                  "GodotLogBeforePath": "shadow/godot.log.before",
                  "GodotLogAfterLaunchPath": "shadow/godot.log.after-launch",
                  "GodotLogCurrentIterationPath": "shadow/godot.log.current-iteration",
                  "RuntimeProbeSamplesPath": "shadow/runtime-probe-samples.json",
                  "LogScanOffsetBytes": {{offset}},
                  "FailureReasonCodes": ["process_unresponsive"],
                  "HangSignals": ["process_unresponsive"],
                  "MainMenuObservation": { "Samples": 1 },
                  "RuntimeObservation": { "Samples": 0 }
                }
                """);
            File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), ToBoundAuditJson(shadowCurrentLogPath, """{"SignatureHits":[]}"""));
            WriteMonkeySummary(workdir, 1);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.Contains(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "runtime_monkey_current_iteration_log_not_retained_file");
            Assert.Contains(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "runtime_monkey_runtime_probe_samples_not_retained_file");
            Assert.NotEqual("PreviewTools", FindFindingOwner(iteration, "process_unresponsive"));
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
    public void RuntimeFailureAnalyzerReportsMalformedRuntimeMonkeyArtifactPathsAsHarnessEvidence()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            Directory.CreateDirectory(iterationDir);
            const string beforeLog = "[INFO] stale full-log prefix before accepted scan offset\r\n";
            const string currentLog = "[ERROR] synthetic current slice\r\n";
            var offset = Encoding.UTF8.GetByteCount(beforeLog);
            var currentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.before"), beforeLog);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), beforeLog + currentLog);
            File.WriteAllText(currentLogPath, currentLog);
            File.WriteAllText(
                Path.Combine(iterationDir, "iteration-result.json"),
                $$"""
                {
                  "Iteration": 1,
                  "HangProbeSchemaVersion": 1,
                  "Passed": false,
                  "Command": "spireplus_test_ancient URDA confirm",
                  "ScenarioTag": "ancient-ui",
                  "OwnerArea": "Runtime.Unknown",
                  "GodotLogBeforePath": "godot.log.before",
                  "GodotLogAfterLaunchPath": "godot.log.after-launch",
                  "GodotLogCurrentIterationPath": "\u0000bad-current-log",
                  "RuntimeProbeSamplesPath": "\u0000bad-probe-samples",
                  "LogScanOffsetBytes": {{offset}},
                  "FailureReasonCodes": ["process_unresponsive"],
                  "HangSignals": ["process_unresponsive"],
                  "MainMenuObservation": { "Samples": 1 },
                  "RuntimeObservation": { "Samples": 0 }
                }
                """);
            File.WriteAllText(Path.Combine(iterationDir, "godot-log-audit.json"), ToBoundAuditJson(currentLogPath, """{"SignatureHits":[]}"""));
            WriteMonkeySummary(workdir, 1);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.Contains(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "current_iteration_log_missing");
            Assert.Contains(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "runtime_monkey_probe_samples_missing");
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
            var beforeLogPath = Path.Combine(iterationDir, "godot.log.before");
            File.WriteAllText(beforeLogPath, prefix);
            var offset = new FileInfo(beforeLogPath).Length;

            File.WriteAllText(
                Path.Combine(iterationDir, "iteration-result.json"),
                $$"""
                {
                  "Iteration": 1,
                  "Passed": false,
                  "Command": "spireplus_test_ancient URDA confirm",
                  "ScenarioTag": "ancient-ui",
                  "OwnerArea": "Runtime.Unknown",
                  "GodotLogBeforePath": "godot.log.before",
                  "GodotLogAfterLaunchPath": "godot.log.after-launch",
                  "GodotLogCurrentIterationPath": "godot.log.current-iteration",
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

            Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Equal("RuntimeHarness", mismatchFinding.GetProperty("OwnerArea").GetString());
            Assert.Equal("blocking", mismatchFinding.GetProperty("Severity").GetString());
            Assert.Equal("Ancients.Urda", FindFindingOwner(iteration, "process_unresponsive"));
            Assert.NotEqual("PackageRuntimeDrift", FindFindingOwner(iteration, "process_unresponsive"));
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
            const string beforeLog = "[INFO] previous Godot log\r\n";
            const string currentLog = "[SPIREPLUS-EVIDENCE] StS1 AdditiveBatch1 Registered act event Golden Idol\r\n";
            File.WriteAllText(
                Path.Combine(iterationDir, "iteration-result.json"),
                """
                {
                  "Iteration": 1,
                  "Passed": false,
                  "Command": "spireplus_test_ancient VAKUU confirm fight",
                  "ScenarioTag": "vakuu-fight",
                  "OwnerArea": "Ancients.Vakuu.FightOptionSetup",
                  "GodotLogBeforePath": "godot.log.before",
                  "GodotLogAfterLaunchPath": "godot.log.after-launch",
                  "GodotLogCurrentIterationPath": "godot.log.current-iteration",
                  "FailureReasonCodes": ["process_unresponsive"],
                  "HangSignals": ["process_unresponsive"]
                }
                """);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.before"), beforeLog);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.after-launch"), beforeLog + currentLog);
            var currentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
            File.WriteAllText(currentLogPath, currentLog);
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
                [INFO] [AutoSlay] Sidecar retained beside run-result.json
                """;
            var beforeLogPath = Path.Combine(runDir, "godot.log.before");
            var afterLaunchLogPath = Path.Combine(runDir, "godot.log.after-launch");
            var currentLogPath = Path.Combine(runDir, "godot.log.current-iteration");
            File.WriteAllText(beforeLogPath, beforeLog);
            File.WriteAllText(currentLogPath, currentLog);
            File.WriteAllText(afterLaunchLogPath, beforeLog + currentLog);
            File.WriteAllText(Path.Combine(runDir, "sts1-mode-log-check.json"), """{"Mismatches":[]}""");
            var autoSlayLogPath = Path.Combine(runDir, "autoslay.log");
            File.WriteAllText(autoSlayLogPath, autoSlayLog);
            var autoSlayLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(autoSlayLogPath))).ToLowerInvariant();
            var beforeLogLength = new FileInfo(beforeLogPath).Length;
            var beforeLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(beforeLogPath))).ToLowerInvariant();
            var afterLaunchLogLength = new FileInfo(afterLaunchLogPath).Length;
            var afterLaunchLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(afterLaunchLogPath))).ToLowerInvariant();
            var currentLogLength = new FileInfo(currentLogPath).Length;
            var currentLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(currentLogPath))).ToLowerInvariant();
            var runtimeProbeSamplesPath = Path.Combine(runDir, "runtime-probe-samples.json");
            File.WriteAllText(
                runtimeProbeSamplesPath,
                """
                [
                  {
                    "Phase": "main-menu",
                    "SampledAt": "2026-06-18T10:00:05Z",
                    "LogExists": true,
                    "LogLengthBytes": 100,
                    "LogLastWriteTimeUtc": "2026-06-18T10:00:05Z",
                    "ProcessId": 4242,
                    "ProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                    "ProcessPath": "C:/Games/SlayTheSpire2.exe",
                    "ExpectedGameProcessId": 4242,
                    "ExpectedGameProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                    "ExpectedGameProcessPath": "C:/Games/SlayTheSpire2.exe",
                    "ProcessIdMatchesExpected": true,
                    "ProcessStartTimeMatchesExpected": true,
                    "ProcessPathMatchesExpected": true,
                    "ProcessIdentityMatchesExpected": true,
                    "ProcessObserved": true,
                    "MainWindowObserved": true,
                    "HungWindow": false,
                    "Responding": true,
                    "StaleProcessCount": 0,
                    "CurrentProcessCount": 1,
                    "UnknownStartTimeProcessCount": 0,
                    "AmbiguousCurrentProcessCount": 0
                  },
                  {
                    "Phase": "runtime",
                    "SampledAt": "2026-06-18T10:00:25Z",
                    "LogExists": true,
                    "LogLengthBytes": 200,
                    "LogLastWriteTimeUtc": "2026-06-18T10:00:25Z",
                    "ProcessId": 4242,
                    "ProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                    "ProcessPath": "C:/Games/SlayTheSpire2.exe",
                    "ExpectedGameProcessId": 4242,
                    "ExpectedGameProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                    "ExpectedGameProcessPath": "C:/Games/SlayTheSpire2.exe",
                    "ProcessIdMatchesExpected": true,
                    "ProcessStartTimeMatchesExpected": true,
                    "ProcessPathMatchesExpected": true,
                    "ProcessIdentityMatchesExpected": true,
                    "ProcessObserved": true,
                    "MainWindowObserved": true,
                    "HungWindow": false,
                    "Responding": true,
                    "StaleProcessCount": 0,
                    "CurrentProcessCount": 1,
                    "UnknownStartTimeProcessCount": 0,
                    "AmbiguousCurrentProcessCount": 0
                  }
                ]
                """);
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
                  "ProcessId": 4242,
                  "ProcessStartTimeUtc": "2026-06-18T09:59:50Z",
                  "ProcessPath": "C:/Games/SlayTheSpire2.exe",
                  "StartTimestamp": "2026-06-18T10:00:00Z",
                  "EndTimestamp": "2026-06-18T10:00:30Z",
                  "AutoSlayLogPath": "autoslay.log",
                  "AutoSlayLogSha256": {{JsonSerializer.Serialize(autoSlayLogHash)}},
                  "RuntimeProbeSamplesPath": "runtime-probe-samples.json",
                  "MainMenuObservation": {
                    "Passed": true,
                    "MainMenuReached": true,
                    "ProcessObserved": true,
                    "ProcessExitedAfterObservation": false,
                    "HungWindowDetected": false,
                    "StaleProcessObserved": false,
                    "MaxStaleProcessCount": 0,
                    "NoLogGrowthTimeoutExceeded": false,
                    "LogObserved": true
                  },
                  "RuntimeObservation": {
                    "Passed": true,
                    "ProcessObserved": true,
                    "ProcessExitedAfterObservation": false,
                    "HungWindowDetected": false,
                    "StaleProcessObserved": false,
                    "MaxStaleProcessCount": 0,
                    "NoLogGrowthTimeoutExceeded": false,
                    "LogGrew": true,
                    "LogInitialLengthBytes": 100,
                    "LogFinalLengthBytes": 200,
                    "LogObserved": true
                  },
                  "GodotLogBeforePath": "godot.log.before",
                  "GodotLogBeforeLengthBytes": {{beforeLogLength}},
                  "GodotLogBeforeSha256": {{JsonSerializer.Serialize(beforeLogHash)}},
                  "GodotLogAfterLaunchPath": "godot.log.after-launch",
                  "GodotLogAfterLaunchLengthBytes": {{afterLaunchLogLength}},
                  "GodotLogAfterLaunchSha256": {{JsonSerializer.Serialize(afterLaunchLogHash)}},
                  "GodotLogCurrentIterationPath": "godot.log.current-iteration",
                  "GodotLogCurrentIterationLengthBytes": {{currentLogLength}},
                  "GodotLogCurrentIterationSha256": {{JsonSerializer.Serialize(currentLogHash)}},
                  "GodotLogAuditPath": "godot-log-audit.json",
                  "Sts1ModeLogCheckPath": "sts1-mode-log-check.json"
                }
                """);
            var summaryPath = Path.Combine(workdir, "autoslay-summary.json");
            File.WriteAllText(
                summaryPath,
                """
                {
                  "RunnerKind": "GameNativeAutoSlay",
                  "Runs": [
                    {
                      "Seed": "AUTOSLAY-ANALYZER",
                      "EventKind": "Ancient",
                      "AncientId": "VAKUU",
                      "RunResultPath": "run-0001/run-result.json",
                      "RuntimeProbeSamplesPath": "run-0001/runtime-probe-samples.json"
                    }
                  ]
                }
                """);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);

            Assert.Equal("GameplayOwnerAction", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("HarnessBlockingFindingCount").GetInt32());
            Assert.Equal(0, root.GetProperty("PackageBlockingFindingCount").GetInt32());
            Assert.Equal(1, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.Single(root.GetProperty("GameplayBlockingFindings").EnumerateArray());
            Assert.NotEmpty(root.GetProperty("RecommendedNextActions").EnumerateArray());
            Assert.Equal(seed, iteration.GetProperty("Seed").GetString());
            Assert.Equal("GameNativeAutoSlay", iteration.GetProperty("RunnerKind").GetString());
            Assert.Equal("Ancient", iteration.GetProperty("EventKind").GetString());
            Assert.Equal("VAKUU", iteration.GetProperty("AncientId").GetString());
            Assert.Equal("game-native-autoslay", iteration.GetProperty("ScenarioTag").GetString());
            Assert.True(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.True(iteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
            Assert.True(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
            Assert.Equal("Ancients.Vakuu", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("Ancients.Vakuu", FindFindingOwner(iteration, "process_unresponsive"));
            Assert.Contains(
                iteration.GetProperty("EvidenceFiles").EnumerateArray(),
                item => item.GetString()?.EndsWith("runtime-probe-samples.json", StringComparison.OrdinalIgnoreCase) == true);
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
            Assert.DoesNotContain(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_runtime_probe_samples_missing");
            Assert.DoesNotContain(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_main_menu_observation_unhealthy");
            Assert.DoesNotContain(
                iteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_runtime_observation_unhealthy");

            var originalRunResultJson = File.ReadAllText(Path.Combine(runDir, "run-result.json"));
            var originalProbeSamplesJson = File.ReadAllText(runtimeProbeSamplesPath);
            File.WriteAllText(
                Path.Combine(runDir, "run-result.json"),
                originalRunResultJson.Replace(
                    JsonSerializer.Serialize(autoSlayLogHash),
                    JsonSerializer.Serialize(new string('f', 64)),
                    StringComparison.Ordinal));
            var sidecarHashOutputPath = Path.Combine(workdir, "runtime-failure-analysis-sidecar-hash-mismatch.json");
            var sidecarHashResult = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", sidecarHashOutputPath);
            Assert.True(sidecarHashResult.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{sidecarHashResult.Output}{sidecarHashResult.Error}");

            using var sidecarHashDocument = JsonDocument.Parse(File.ReadAllText(sidecarHashOutputPath));
            var sidecarHashRoot = sidecarHashDocument.RootElement;
            var sidecarHashIteration = FindIteration(sidecarHashRoot, 1);
            var sidecarHashFinding = sidecarHashIteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "autoslay_sidecar_log_hash_mismatch");

            Assert.Equal("HarnessEvidenceInvalid", sidecarHashRoot.GetProperty("TriageDisposition").GetString());
            Assert.True(sidecarHashIteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(sidecarHashIteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
            Assert.Equal("RuntimeHarness", sidecarHashFinding.GetProperty("OwnerArea").GetString());
            File.WriteAllText(Path.Combine(runDir, "run-result.json"), originalRunResultJson);

            File.WriteAllText(
                Path.Combine(runDir, "run-result.json"),
                originalRunResultJson.Replace(
                    JsonSerializer.Serialize(currentLogHash),
                    JsonSerializer.Serialize(new string('0', 64)),
                    StringComparison.Ordinal));
            var mismatchOutputPath = Path.Combine(workdir, "runtime-failure-analysis-metadata-mismatch.json");
            var mismatchResult = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", mismatchOutputPath);
            Assert.True(mismatchResult.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{mismatchResult.Output}{mismatchResult.Error}");

            using var mismatchDocument = JsonDocument.Parse(File.ReadAllText(mismatchOutputPath));
            var mismatchRoot = mismatchDocument.RootElement;
            var mismatchIteration = FindIteration(mismatchRoot, 1);
            var metadataFinding = mismatchIteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "autoslay_godot_log_metadata_mismatch");

            Assert.Equal("HarnessEvidenceInvalid", mismatchRoot.GetProperty("TriageDisposition").GetString());
            Assert.False(mismatchIteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Equal("Runtime.Unknown", mismatchIteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Equal("RuntimeHarness", metadataFinding.GetProperty("OwnerArea").GetString());
            Assert.Equal("blocking", metadataFinding.GetProperty("Severity").GetString());
            File.WriteAllText(Path.Combine(runDir, "run-result.json"), originalRunResultJson);

            File.Copy(currentLogPath, Path.Combine(workdir, "godot.log.current-iteration"), overwrite: true);
            File.Copy(
                Path.Combine(runDir, "runtime-probe-samples.json"),
                Path.Combine(workdir, "runtime-probe-samples.json"),
                overwrite: true);
            File.WriteAllText(
                Path.Combine(runDir, "run-result.json"),
                originalRunResultJson
                    .Replace(
                        "\"RuntimeProbeSamplesPath\": \"runtime-probe-samples.json\"",
                        "\"RuntimeProbeSamplesPath\": \"../runtime-probe-samples.json\"",
                        StringComparison.Ordinal)
                    .Replace(
                        "\"GodotLogCurrentIterationPath\": \"godot.log.current-iteration\"",
                        "\"GodotLogCurrentIterationPath\": \"../godot.log.current-iteration\"",
                        StringComparison.Ordinal));
            var sharedArtifactOutputPath = Path.Combine(workdir, "runtime-failure-analysis-shared-artifacts.json");
            var sharedArtifactResult = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", sharedArtifactOutputPath);
            Assert.True(sharedArtifactResult.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{sharedArtifactResult.Output}{sharedArtifactResult.Error}");

            using var sharedArtifactDocument = JsonDocument.Parse(File.ReadAllText(sharedArtifactOutputPath));
            var sharedArtifactRoot = sharedArtifactDocument.RootElement;
            var sharedArtifactIteration = FindIteration(sharedArtifactRoot, 1);

            Assert.Equal("HarnessEvidenceInvalid", sharedArtifactRoot.GetProperty("TriageDisposition").GetString());
            Assert.False(sharedArtifactIteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(sharedArtifactIteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
            Assert.Equal("Runtime.Unknown", sharedArtifactIteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Contains(
                sharedArtifactIteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_runtime_probe_samples_outside_run_dir");
            Assert.Contains(
                sharedArtifactIteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_current_iteration_log_outside_run_dir");
            File.WriteAllText(Path.Combine(runDir, "run-result.json"), originalRunResultJson);

            var originalSummaryJson = File.ReadAllText(summaryPath);
            File.WriteAllText(
                summaryPath,
                originalSummaryJson.Replace(
                    "\"RunResultPath\": \"run-0001/run-result.json\"",
                    "\"RunResultPath\": \"../run-0001/run-result.json\"",
                    StringComparison.Ordinal));
            var outsideRunResultOutputPath = Path.Combine(workdir, "runtime-failure-analysis-outside-run-result.json");
            var outsideRunResultResult = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outsideRunResultOutputPath);
            Assert.True(outsideRunResultResult.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{outsideRunResultResult.Output}{outsideRunResultResult.Error}");

            using var outsideRunResultDocument = JsonDocument.Parse(File.ReadAllText(outsideRunResultOutputPath));
            var outsideRunResultRoot = outsideRunResultDocument.RootElement;
            var outsideRunResultIteration = FindIteration(outsideRunResultRoot, 1);

            Assert.Equal("HarnessEvidenceInvalid", outsideRunResultRoot.GetProperty("TriageDisposition").GetString());
            Assert.False(outsideRunResultIteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(outsideRunResultIteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
            Assert.Contains(
                outsideRunResultIteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_run_result_path_outside_evidence_dir");
            File.WriteAllText(summaryPath, originalSummaryJson);

            File.WriteAllText(
                runtimeProbeSamplesPath,
                originalProbeSamplesJson
                    .Replace(
                        "\"ExpectedGameProcessStartTimeUtc\": \"2026-06-18T09:59:50Z\"",
                        "\"ExpectedGameProcessStartTimeUtc\": \"2026-06-18T09:59:51Z\"",
                        StringComparison.Ordinal)
                    .Replace(
                        "\"ExpectedGameProcessPath\": \"C:/Games/SlayTheSpire2.exe\"",
                        "\"ExpectedGameProcessPath\": \"C:/Games/OtherSlayTheSpire2.exe\"",
                        StringComparison.Ordinal));
            var identityDriftOutputPath = Path.Combine(workdir, "runtime-failure-analysis-autoslay-identity-drift.json");
            var identityDriftResult = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", identityDriftOutputPath);
            Assert.True(identityDriftResult.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{identityDriftResult.Output}{identityDriftResult.Error}");

            using var identityDriftDocument = JsonDocument.Parse(File.ReadAllText(identityDriftOutputPath));
            var identityDriftRoot = identityDriftDocument.RootElement;
            var identityDriftIteration = FindIteration(identityDriftRoot, 1);

            Assert.Equal("HarnessEvidenceInvalid", identityDriftRoot.GetProperty("TriageDisposition").GetString());
            Assert.False(identityDriftIteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(identityDriftIteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
            Assert.Equal("Runtime.Unknown", identityDriftIteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Contains(
                identityDriftIteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_runtime_probe_process_identity_mismatch");
            File.WriteAllText(runtimeProbeSamplesPath, originalProbeSamplesJson);

            File.WriteAllText(
                Path.Combine(runDir, "run-result.json"),
                originalRunResultJson.Replace(
                    "\"AutoSlayLogPath\": \"autoslay.log\"",
                    "\"AutoSlayLogPath\": \"\\u0000bad-autoslay-log\"",
                    StringComparison.Ordinal));
            var malformedPathOutputPath = Path.Combine(workdir, "runtime-failure-analysis-malformed-autoslay-path.json");
            var malformedPathResult = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", malformedPathOutputPath);
            Assert.True(malformedPathResult.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{malformedPathResult.Output}{malformedPathResult.Error}");

            using var malformedPathDocument = JsonDocument.Parse(File.ReadAllText(malformedPathOutputPath));
            var malformedPathRoot = malformedPathDocument.RootElement;
            var malformedPathIteration = FindIteration(malformedPathRoot, 1);

            Assert.Equal("HarnessEvidenceInvalid", malformedPathRoot.GetProperty("TriageDisposition").GetString());
            Assert.False(malformedPathIteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(malformedPathIteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(malformedPathIteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
            Assert.Equal("Runtime.Unknown", malformedPathIteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Contains(
                malformedPathIteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_sidecar_log_missing");
            File.WriteAllText(Path.Combine(runDir, "run-result.json"), originalRunResultJson);

            File.WriteAllText(
                Path.Combine(runDir, "run-result.json"),
                Regex.Replace(
                    originalRunResultJson,
                    "\\s+\"RuntimeProbeSamplesPath\": \"runtime-probe-samples\\.json\",\\r?\\n",
                    Environment.NewLine,
                    RegexOptions.CultureInvariant));
            var missingPathOutputPath = Path.Combine(workdir, "runtime-failure-analysis-missing-autoslay-path.json");
            var missingPathResult = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", missingPathOutputPath);
            Assert.True(missingPathResult.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{missingPathResult.Output}{missingPathResult.Error}");

            using var missingPathDocument = JsonDocument.Parse(File.ReadAllText(missingPathOutputPath));
            var missingPathRoot = missingPathDocument.RootElement;
            var missingPathIteration = FindIteration(missingPathRoot, 1);

            Assert.Equal("HarnessEvidenceInvalid", missingPathRoot.GetProperty("TriageDisposition").GetString());
            Assert.False(missingPathIteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(missingPathIteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(missingPathIteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
            Assert.Equal("Runtime.Unknown", missingPathIteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Contains(
                missingPathIteration.GetProperty("Findings").EnumerateArray(),
                item => item.GetProperty("Signal").GetString() == "autoslay_runtime_probe_samples_path_missing");
            File.WriteAllText(Path.Combine(runDir, "run-result.json"), originalRunResultJson);
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
                  "AutoSlayLogPath": "autoslay.log",
                  "GodotLogBeforePath": "godot.log.before",
                  "GodotLogAfterLaunchPath": "godot.log.after-launch",
                  "GodotLogCurrentIterationPath": "godot.log.current-iteration",
                  "GodotLogAuditPath": "godot-log-audit.json"
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
                  "GodotLogBeforePath": "godot.log.before",
                  "GodotLogAfterLaunchPath": "godot.log.after-launch",
                  "GodotLogCurrentIterationPath": "godot.log.current-iteration",
                  "LogScanOffsetBytes": 0,
                  "FailureReasonCodes": [],
                  "HangSignals": []
                }
                """);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.before"), "");
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
            var findingsJson = iteration.GetProperty("Findings").GetRawText();
            var failedWithoutSignalFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .SingleOrDefault(item => item.GetProperty("Signal").GetString() == "iteration_failed_without_failure_signal");
            Assert.True(
                failedWithoutSignalFinding.ValueKind != JsonValueKind.Undefined,
                $"Expected iteration_failed_without_failure_signal. Analyzer output:{Environment.NewLine}{result.Output}{Environment.NewLine}Findings:{Environment.NewLine}{findingsJson}");

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
                  "GodotLogBeforePath": "godot.log.before",
                  "GodotLogAfterLaunchPath": "godot.log.after-launch",
                  "GodotLogCurrentIterationPath": "godot.log.current-iteration",
                  "LogScanOffsetBytes": 0,
                  "FailureReasonCodes": [],
                  "HangSignals": []
                }
                """);
            File.WriteAllText(Path.Combine(iterationDir, "godot.log.before"), "");
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
    public void LocalGodotSourceWorkspaceCheckerIsNoLaunchAndGuardsFreshness()
    {
        var checker = ReadRepoText("scripts", "check-local-godot-source-workspace.ps1");

        AssertSourceContains(
            checker,
            "[string]$SourceRoot = 'source code'",
            "[string]$GameRoot = 'E:\\Steam\\steamapps\\common\\Slay the Spire 2'",
            "[string]$GodotExe",
            "[switch]$FailOnMismatch",
            "Normalize-ComparablePath",
            "Get-ObjectPropertyString",
            "SlayTheSpire2.pck",
            "installed_game_pck_exists",
            "source_version_matches_installed_game",
            "source_commit_matches_installed_game",
            "source_branch_matches_installed_game",
            "source_main_assembly_hash_matches_installed_game",
            "source_release_identity_matches_installed_game",
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
            "gdre_opening_file_matches_installed_game_pck",
            "OpeningFileMatchesInstalledGame",
            "OriginMatchesInstalledGamePck",
            "PckSha256",
            "ritsulib_manifest_exists",
            "ritsulib_variant_matches_installed_game",
            "ritsulib_variant_dll_exists",
            "ManifestSha256",
            "VariantDllSha256",
            "ExpectedVariantDllSha256",
            "CompatTargetText",
            "ritsulib_viewer_exists",
            "RitsuLib viewer exists; it is a log viewer, not an unpacker or monkey runner",
            "OpenProjectCommand",
            "EvidenceUsePolicy",
            "NotRuntimeProof",
            "AuthorizedLocalInstallOnly",
            "AuthorizedSourceOriginVerified",
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
            "`RecoveredSource.OriginMatchesInstalledGamePck`",
            "`RuntimeProbeSamplesPath`",
            "`MainMenuObservation`",
            "`RuntimeObservation`",
            "`Phase`",
            "`SampledAt`",
            "`LogExists`",
            "`LogLengthBytes`",
            "`LogLastWriteTimeUtc`",
            "`RuntimeObservation.LogInitialLengthBytes`",
            "`ProcessId`",
            "`ProcessStartTimeUtc`",
            "`ProcessPath`",
            "`ExpectedGameProcessId`",
            "`ExpectedGameProcessStartTimeUtc`",
            "`ExpectedGameProcessPath`",
            "`ProcessIdentityMatchesExpected`",
            "`MainWindowObserved`",
            "same per-seed `run-####`",
            "Ancient id, ordered",
            "start/event/Ancient-dialogue/event-option/completion markers",
            "every per-seed artifact retained in",
            "sidecar text to",
            "observed ordered event-room lines in both",
            "check-spire-plus-autoslay-packet.ps1",
            "GameNativeAutoSlay",
            "single-seed fixture packet is not batch proof",
            "verifier must fail",
            "-ExpectedAncientIds",
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
                    "Version": "v0.107.0",
                    "Commit": "fixture",
                    "Branch": "v0.107.0",
                    "MainAssemblyHash": "12345"
                  },
                  "RecoveredSource": {
                    "Version": "v0.107.0",
                    "Commit": "fixture",
                    "Branch": "v0.107.0",
                    "MainAssemblyHash": "12345",
                    "Disposition": "current",
                    "MatchesInstalledGame": true,
                    "OriginPckPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\SlayTheSpire2.pck",
                    "OriginMatchesInstalledGamePck": true
                  },
                  "RitsuLib": {
                    "Version": "0.4.24",
                    "CompatBranch": "0.107.0",
                    "RootPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib",
                    "ManifestPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\mod_manifest.json",
                    "ManifestSha256": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                    "VariantsPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\ritsulib-variants.json",
                    "VariantsSha256": "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                    "ViewerPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\viewer\\index.html",
                    "VariantDirectory": "lib\\0.107.0",
                    "VariantAssembly": "STS2-RitsuLib.dll",
                    "VariantDllPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\STS2-RitsuLib.dll",
                    "VariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                    "ExpectedVariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                    "CompatTargetPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\compat-target.txt",
                    "CompatTargetText": "0.107.0"
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
                    "AuthorizedSourceOriginVerified": true,
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
            var runtimeProbeSamplesPath = Path.Combine(runDir, "runtime-probe-samples.json");
            var autoSlayLog = string.Join(
                Environment.NewLine,
                $"12:00:00.000 [INFO] [AutoSlay] Starting run with seed={seed}",
                "12:00:01.000 [INFO] [AutoSlay] Entering Event room (Act 1, Floor 2)",
                "12:00:01.500 [INFO] [AutoSlay] Detected Ancient event, clicking through dialogue: VAKUU",
                "12:00:02.000 [INFO] [AutoSlay] Action: Selecting event option: VAKUU (option: contract)",
                $"12:00:03.000 [INFO] [AutoSlay] Run completed successfully with seed={seed}") + Environment.NewLine;
            var currentLog = string.Join(
                Environment.NewLine,
                "v0.1.0-private-beta.87",
                "release = v0.107.0",
                "RitsuLib Version: 0.4.24 [compat branch: 0.107.0]",
                "[INFO] [EZMicroBalance] [Patcher - SpirePlus] Patch application complete: 25 applied, 0 ignored, 0 failed, 25 total",
                "[INFO] [EZMicroBalance] ModPatcher applied 25 patches (25 registered).",
                autoSlayLog.TrimEnd()) + Environment.NewLine;
            var beforeLog = "11:59:59.000 [INFO] [Previous] old shared log line" + Environment.NewLine;
            File.WriteAllText(autoSlayLogPath, autoSlayLog);
            File.WriteAllText(beforeLogPath, beforeLog);
            File.WriteAllText(currentLogPath, currentLog);
            File.WriteAllText(afterLogPath, beforeLog + currentLog);
            var beforeLogLength = new FileInfo(beforeLogPath).Length;
            var afterLaunchLogLength = new FileInfo(afterLogPath).Length;
            var gameProcessPath = Path.Combine(workdir, "SlayTheSpire2.exe");
            const string gameProcessStartTimeUtc = "2026-06-18T09:59:50Z";
            File.WriteAllText(
                runtimeProbeSamplesPath,
                $$"""
                [
                  {
                    "Phase": "main-menu",
                    "SampledAt": "2026-06-18T10:00:05Z",
                    "ProcessId": 4242,
                    "ProcessStartTimeUtc": {{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},
                    "ProcessPath": {{JsonSerializer.Serialize(gameProcessPath)}},
                    "ExpectedGameProcessId": 4242,
                    "ExpectedGameProcessStartTimeUtc": {{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},
                    "ExpectedGameProcessPath": {{JsonSerializer.Serialize(gameProcessPath)}},
                    "ProcessIdMatchesExpected": true,
                    "ProcessStartTimeMatchesExpected": true,
                    "ProcessPathMatchesExpected": true,
                    "ProcessIdentityMatchesExpected": true,
                    "ProcessObserved": true,
                    "MainWindowObserved": true,
                    "HungWindow": false,
                    "Responding": true,
                    "LogExists": true,
                    "LogLengthBytes": {{beforeLogLength}},
                    "LogLastWriteTimeUtc": "2026-06-18T10:00:05Z",
                    "StaleProcessCount": 0,
                    "CurrentProcessCount": 1,
                    "UnknownStartTimeProcessCount": 0,
                    "AmbiguousCurrentProcessCount": 0
                  },
                  {
                    "Phase": "runtime",
                    "SampledAt": "2026-06-18T10:00:25Z",
                    "ProcessId": 4242,
                    "ProcessStartTimeUtc": {{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},
                    "ProcessPath": {{JsonSerializer.Serialize(gameProcessPath)}},
                    "ExpectedGameProcessId": 4242,
                    "ExpectedGameProcessStartTimeUtc": {{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},
                    "ExpectedGameProcessPath": {{JsonSerializer.Serialize(gameProcessPath)}},
                    "ProcessIdMatchesExpected": true,
                    "ProcessStartTimeMatchesExpected": true,
                    "ProcessPathMatchesExpected": true,
                    "ProcessIdentityMatchesExpected": true,
                    "ProcessObserved": true,
                    "MainWindowObserved": true,
                    "HungWindow": false,
                    "Responding": true,
                    "LogExists": true,
                    "LogLengthBytes": {{afterLaunchLogLength}},
                    "LogLastWriteTimeUtc": "2026-06-18T10:00:25Z",
                    "StaleProcessCount": 0,
                    "CurrentProcessCount": 1,
                    "UnknownStartTimeProcessCount": 0,
                    "AmbiguousCurrentProcessCount": 0
                  }
                ]
                """);
            var autoSlayLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(autoSlayLogPath))).ToLowerInvariant();
            var beforeLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(beforeLogPath))).ToLowerInvariant();
            var afterLaunchLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(afterLogPath))).ToLowerInvariant();
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
                  "ProcessStartTimeUtc": {{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},
                  "ProcessPath": {{JsonSerializer.Serialize(gameProcessPath)}},
                  "StartTimestamp": "2026-06-18T10:00:00Z",
                  "EndTimestamp": "2026-06-18T10:00:30Z",
                  "ExitCode": 0,
                  "StaleProcessCount": 0,
                  "RuntimeProbeSamplesPath": "run-0001/runtime-probe-samples.json",
                  "MainMenuObservation": {
                    "Passed": true,
                    "MainMenuReached": true,
                    "ProcessObserved": true,
                    "ProcessExitedAfterObservation": false,
                    "HungWindowDetected": false,
                    "StaleProcessObserved": false,
                    "MaxStaleProcessCount": 0,
                    "NoLogGrowthTimeoutExceeded": false,
                    "LogObserved": true
                  },
                  "RuntimeObservation": {
                    "Passed": true,
                    "ProcessObserved": true,
                    "ProcessExitedAfterObservation": false,
                    "HungWindowDetected": false,
                    "StaleProcessObserved": false,
                    "MaxStaleProcessCount": 0,
                    "NoLogGrowthTimeoutExceeded": false,
                    "LogGrew": true,
                    "LogInitialLengthBytes": {{beforeLogLength}},
                    "LogFinalLengthBytes": {{afterLaunchLogLength}},
                    "LogObserved": true
                  },
                  "AutoSlayLogPath": "run-0001/autoslay.log",
                  "AutoSlayLogSha256": {{JsonSerializer.Serialize(autoSlayLogHash)}},
                  "GodotLogBeforePath": "run-0001/godot.log.before",
                  "GodotLogBeforeLengthBytes": {{beforeLogLength}},
                  "GodotLogBeforeSha256": {{JsonSerializer.Serialize(beforeLogHash)}},
                  "GodotLogAfterLaunchPath": "run-0001/godot.log.after-launch",
                  "GodotLogAfterLaunchLengthBytes": {{afterLaunchLogLength}},
                  "GodotLogAfterLaunchSha256": {{JsonSerializer.Serialize(afterLaunchLogHash)}},
                  "GodotLogCurrentIterationPath": "run-0001/godot.log.current-iteration",
                  "GodotLogCurrentIterationLengthBytes": {{currentLogLength}},
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
                  "ExpectedAncientIds": ["VAKUU"],
                  "PackageVersion": "v0.1.0-private-beta.87",
                  "GameVersion": "0.107.0",
                  "RitsuLibVersion": "0.4.24",
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
                    "SourceBranch": "v0.107.0",
                    "SourceMainAssemblyHash": "12345",
                    "InstalledGameVersion": "v0.107.0",
                    "InstalledGameCommit": "fixture",
                    "InstalledGameBranch": "v0.107.0",
                    "InstalledGameMainAssemblyHash": "12345",
                    "Disposition": "current",
                    "MatchesInstalledGame": true,
                    "OriginPckPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\SlayTheSpire2.pck",
                    "OriginMatchesInstalledGamePck": true,
                    "RitsuLibVersion": "0.4.24",
                    "RitsuLibCompatBranch": "0.107.0",
                    "RitsuLibManifestPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\mod_manifest.json",
                    "RitsuLibManifestSha256": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                    "RitsuLibVariantsPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\ritsulib-variants.json",
                    "RitsuLibVariantsSha256": "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                    "RitsuLibVariantDllPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\STS2-RitsuLib.dll",
                    "RitsuLibVariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                    "RitsuLibExpectedVariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                    "RitsuLibCompatTargetPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\compat-target.txt",
                    "RitsuLibCompatTargetText": "0.107.0",
                    "RefreshSourceSnapshotBeforeCurrentApiClaims": false,
                    "NotRuntimeProof": true,
                    "AuthorizedSourceOriginVerified": true
                  }
                }
                """);
            var summaryPath = Path.Combine(workdir, "autoslay-summary.json");
            File.WriteAllText(
                summaryPath,
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
                      "RuntimeProbeSamplesPath": "run-0001/runtime-probe-samples.json",
                      "AutoSlayLogPath": "run-0001/autoslay.log",
                      "AutoSlayLogSha256": {{JsonSerializer.Serialize(autoSlayLogHash)}},
                      "GodotLogBeforePath": "run-0001/godot.log.before",
                      "GodotLogBeforeLengthBytes": {{beforeLogLength}},
                      "GodotLogBeforeSha256": {{JsonSerializer.Serialize(beforeLogHash)}},
                      "GodotLogAfterLaunchPath": "run-0001/godot.log.after-launch",
                      "GodotLogAfterLaunchLengthBytes": {{afterLaunchLogLength}},
                      "GodotLogAfterLaunchSha256": {{JsonSerializer.Serialize(afterLaunchLogHash)}},
                      "GodotLogCurrentIterationPath": "run-0001/godot.log.current-iteration",
                      "GodotLogCurrentIterationLengthBytes": {{currentLogLength}},
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
                "-ExpectedAncientIds",
                "VAKUU",
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
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
            Assert.Contains("expected_ancient_ids_required_for_proof_mode status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("expected_ancient_ids_unique status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("plan_expected_ancient_ids_match_parameter status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("summary_expected_ancient_ids_observed status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_summary_run_passed_true status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_event_kind_is_ancient status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_leaf_expected status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_parent_expected status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_launch_true status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_launcher_sha256_matches_plan status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_passed_true status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_process_id_matches_runtime_probe_samples status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_process_start_time_matches_runtime_probe_samples status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_process_path_matches_runtime_probe_samples status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_expected_process_id_matches_run_result status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_expected_process_start_time_matches_run_result status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_expected_process_path_matches_run_result status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_under_run_dir status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_exists status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_under_run_dir status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_leaf_expected status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_before_log_under_run_dir status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_after_launch_log_under_run_dir status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_current_iteration_log_under_run_dir status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_audit_under_run_dir status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_sts1_mode_check_under_run_dir status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_before_log_length_matches status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_before_log_sha256_matches status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_after_launch_log_length_matches status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_after_launch_log_sha256_matches status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_current_iteration_log_length_matches status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_sampled_at_field_present status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_log_exists_field_present status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_log_length_field_present status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_log_last_write_time_field_retained status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_main_menu_phase_observed status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_runtime_phase_observed status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_single_positive_process_id status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_single_process_start_time status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_single_process_path status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_single_expected_process_start_time status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_single_expected_process_path status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_all_match_expected_identity status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_no_unknown_start_times status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_no_ambiguous_current_processes status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_single_current_process status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_process_start_time_parseable status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_process_path_present status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_start_timestamp_parseable status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_end_timestamp_parseable status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_timestamp_order_valid status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_probe_samples_path_matches_summary status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_before_log_length_matches_summary status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_before_log_sha256_matches_summary status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_after_launch_log_length_matches_summary status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_after_launch_log_sha256_matches_summary status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_current_iteration_log_length_matches_summary status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_current_iteration_log_hash_matches_summary status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_main_menu_observation_passed status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_main_menu_no_log_growth_timeout status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_observation_passed status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_log_grew status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_log_initial_length_present status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_log_final_length_present status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_log_length_growth_matches_log_grew status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_log_growth_matches_runtime_observation status=pass", passResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_no_log_growth_timeout status=pass", passResult.Output, StringComparison.Ordinal);
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
            File.WriteAllText(
                planPath,
                File.ReadAllText(planPath).Replace(
                    "\"ExpectedAncientIds\": [\"VAKUU\"]",
                    "\"ExpectedAncientIds\": [\"VAKUU\", \"URDA\"]",
                    StringComparison.Ordinal));

            var missingExpectedAncientResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedAncientIds",
                "VAKUU,URDA",
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(missingExpectedAncientResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{missingExpectedAncientResult.Output}{missingExpectedAncientResult.Error}");
            Assert.Contains("expected_ancient_ids_unique status=pass", missingExpectedAncientResult.Output, StringComparison.Ordinal);
            Assert.Contains("plan_expected_ancient_ids_match_parameter status=pass", missingExpectedAncientResult.Output, StringComparison.Ordinal);
            Assert.Contains("summary_expected_ancient_ids_observed status=fail", missingExpectedAncientResult.Output, StringComparison.Ordinal);
            Assert.Contains("ExpectedAncientIds missing=URDA", missingExpectedAncientResult.Output, StringComparison.Ordinal);

            var missingProofTargetResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25",
                "-FailOnMismatch");
            Assert.NotEqual(0, missingProofTargetResult.ExitCode);
            Assert.Contains("expected_ancient_ids_required_for_proof_mode status=fail", missingProofTargetResult.Output, StringComparison.Ordinal);

            File.WriteAllText(
                planPath,
                File.ReadAllText(planPath).Replace(
                    "\"ExpectedAncientIds\": [\"VAKUU\", \"URDA\"]",
                    "\"ExpectedAncientIds\": [\"VAKUU\"]",
                    StringComparison.Ordinal));

            var underSizedBatchResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-MinRuns",
                "2",
                "-ExpectedAncientIds",
                "VAKUU",
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25",
                "-FailOnMismatch");
            Assert.NotEqual(0, underSizedBatchResult.ExitCode);
            Assert.Contains("expected_ancient_ids_required_for_proof_mode status=pass", underSizedBatchResult.Output, StringComparison.Ordinal);
            Assert.Contains("plan_seed_count_meets_minimum status=fail", underSizedBatchResult.Output, StringComparison.Ordinal);
            Assert.Contains("summary_total_runs_meets_minimum status=fail", underSizedBatchResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_event_room_traversal_observed status=pass", underSizedBatchResult.Output, StringComparison.Ordinal);

            var originalRunResultJson = File.ReadAllText(runResultPath);
            var originalSummaryJson = File.ReadAllText(summaryPath);
            var rootAutoSlayLogPath = Path.Combine(workdir, "autoslay.log");
            File.Copy(autoSlayLogPath, rootAutoSlayLogPath, overwrite: true);
            File.WriteAllText(
                runResultPath,
                originalRunResultJson.Replace("\"AutoSlayLogPath\": \"run-0001/autoslay.log\"", "\"AutoSlayLogPath\": \"autoslay.log\"", StringComparison.Ordinal));
            File.WriteAllText(
                summaryPath,
                originalSummaryJson.Replace("\"AutoSlayLogPath\": \"run-0001/autoslay.log\"", "\"AutoSlayLogPath\": \"autoslay.log\"", StringComparison.Ordinal));
            var sharedSidecarResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(sharedSidecarResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{sharedSidecarResult.Output}{sharedSidecarResult.Error}");
            Assert.Contains("run_0001_autoslay_log_under_evidence_dir status=pass", sharedSidecarResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_under_run_dir status=fail", sharedSidecarResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_autoslay_log_leaf_expected status=pass", sharedSidecarResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runResultPath, originalRunResultJson);
            File.WriteAllText(summaryPath, originalSummaryJson);

            var rootRuntimeProbeSamplesPath = Path.Combine(workdir, "runtime-probe-samples.json");
            File.Copy(runtimeProbeSamplesPath, rootRuntimeProbeSamplesPath, overwrite: true);
            File.WriteAllText(
                runResultPath,
                originalRunResultJson.Replace("\"RuntimeProbeSamplesPath\": \"run-0001/runtime-probe-samples.json\"", "\"RuntimeProbeSamplesPath\": \"runtime-probe-samples.json\"", StringComparison.Ordinal));
            File.WriteAllText(
                summaryPath,
                originalSummaryJson.Replace("\"RuntimeProbeSamplesPath\": \"run-0001/runtime-probe-samples.json\"", "\"RuntimeProbeSamplesPath\": \"runtime-probe-samples.json\"", StringComparison.Ordinal));
            var sharedRuntimeProbeResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(sharedRuntimeProbeResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{sharedRuntimeProbeResult.Output}{sharedRuntimeProbeResult.Error}");
            Assert.Contains("run_0001_runtime_probe_samples_under_evidence_dir status=pass", sharedRuntimeProbeResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_under_run_dir status=fail", sharedRuntimeProbeResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_leaf_expected status=pass", sharedRuntimeProbeResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_probe_samples_path_matches_summary status=pass", sharedRuntimeProbeResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runResultPath, originalRunResultJson);
            File.WriteAllText(summaryPath, originalSummaryJson);

            File.WriteAllText(
                summaryPath,
                originalSummaryJson.Replace("\"RuntimeProbeSamplesPath\": \"run-0001/runtime-probe-samples.json\"", "\"RuntimeProbeSamplesPath\": \"\\u0000bad-runtime-probe-samples\"", StringComparison.Ordinal));
            var malformedSummaryPathResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(malformedSummaryPathResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{malformedSummaryPathResult.Output}{malformedSummaryPathResult.Error}");
            Assert.Contains("run_0001_runtime_probe_samples_path_present status=fail", malformedSummaryPathResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_under_evidence_dir status=fail", malformedSummaryPathResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_exists status=fail", malformedSummaryPathResult.Output, StringComparison.Ordinal);
            File.WriteAllText(summaryPath, originalSummaryJson);

            File.WriteAllText(
                runResultPath,
                originalRunResultJson.Replace("\"RuntimeProbeSamplesPath\": \"run-0001/runtime-probe-samples.json\"", "\"RuntimeProbeSamplesPath\": \"\\u0000bad-runtime-probe-samples\"", StringComparison.Ordinal));
            var malformedRunResultPathResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(malformedRunResultPathResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{malformedRunResultPathResult.Output}{malformedRunResultPathResult.Error}");
            Assert.Contains("run_0001_run_result_runtime_probe_samples_path_matches_summary status=fail", malformedRunResultPathResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runResultPath, originalRunResultJson);

            File.WriteAllText(
                summaryPath,
                originalSummaryJson.Replace("\"RuntimeProbeSamplesPath\": \"run-0001/runtime-probe-samples.json\"", "\"RuntimeProbeSamplesPath\": \"\\u0000bad-runtime-probe-samples\"", StringComparison.Ordinal));
            File.WriteAllText(
                runResultPath,
                originalRunResultJson.Replace("\"RuntimeProbeSamplesPath\": \"run-0001/runtime-probe-samples.json\"", "\"RuntimeProbeSamplesPath\": \"\\u0000bad-runtime-probe-samples\"", StringComparison.Ordinal));
            var bothMalformedPathResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(bothMalformedPathResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{bothMalformedPathResult.Output}{bothMalformedPathResult.Error}");
            Assert.Contains("run_0001_runtime_probe_samples_path_present status=fail", bothMalformedPathResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_probe_samples_path_matches_summary status=fail", bothMalformedPathResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runResultPath, originalRunResultJson);
            File.WriteAllText(summaryPath, originalSummaryJson);

            File.WriteAllText(
                summaryPath,
                originalSummaryJson.Replace(
                    JsonSerializer.Serialize(afterLaunchLogHash),
                    JsonSerializer.Serialize(new string('0', 64)),
                    StringComparison.Ordinal));
            var logMetadataMismatchResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(logMetadataMismatchResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{logMetadataMismatchResult.Output}{logMetadataMismatchResult.Error}");
            Assert.Contains("run_0001_after_launch_log_sha256_matches status=fail", logMetadataMismatchResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_after_launch_log_sha256_matches_summary status=fail", logMetadataMismatchResult.Output, StringComparison.Ordinal);
            File.WriteAllText(summaryPath, originalSummaryJson);

            File.WriteAllText(
                runResultPath,
                originalRunResultJson.Replace("\"NoLogGrowthTimeoutExceeded\": false", "\"NoLogGrowthTimeoutExceeded\": true", StringComparison.Ordinal));
            var logGrowthTimeoutResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(logGrowthTimeoutResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{logGrowthTimeoutResult.Output}{logGrowthTimeoutResult.Error}");
            Assert.Contains("run_0001_run_result_main_menu_no_log_growth_timeout status=fail", logGrowthTimeoutResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_runtime_no_log_growth_timeout status=fail", logGrowthTimeoutResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runResultPath, originalRunResultJson);

            File.WriteAllText(
                runResultPath,
                originalRunResultJson.Replace("\"LogGrew\": true", "\"LogGrew\": false", StringComparison.Ordinal));
            var runtimeLogGrowthResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(runtimeLogGrowthResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{runtimeLogGrowthResult.Output}{runtimeLogGrowthResult.Error}");
            Assert.Contains("run_0001_run_result_runtime_log_grew status=fail", runtimeLogGrowthResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runResultPath, originalRunResultJson);

            File.WriteAllText(
                runResultPath,
                originalRunResultJson.Replace("\"ProcessId\": 4242", "\"ProcessId\": 5151", StringComparison.Ordinal));
            var processIdMismatchResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(processIdMismatchResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{processIdMismatchResult.Output}{processIdMismatchResult.Error}");
            Assert.Contains("run_0001_run_result_process_id_matches_runtime_probe_samples status=fail", processIdMismatchResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runResultPath, originalRunResultJson);

            var originalRuntimeProbeSamplesJson = File.ReadAllText(runtimeProbeSamplesPath);
            var driftedProbePath = Path.Combine(workdir, "other", "SlayTheSpire2.exe");
            File.WriteAllText(
                runtimeProbeSamplesPath,
                originalRuntimeProbeSamplesJson
                    .Replace(
                        "\"ExpectedGameProcessStartTimeUtc\": \"2026-06-18T09:59:50Z\"",
                        "\"ExpectedGameProcessStartTimeUtc\": \"2026-06-18T09:59:51Z\"",
                        StringComparison.Ordinal)
                    .Replace(
                        $"\"ExpectedGameProcessPath\": {JsonSerializer.Serialize(gameProcessPath)}",
                        $"\"ExpectedGameProcessPath\": {JsonSerializer.Serialize(driftedProbePath)}",
                        StringComparison.Ordinal));
            var runtimeProbeExpectedIdentityDriftResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(runtimeProbeExpectedIdentityDriftResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{runtimeProbeExpectedIdentityDriftResult.Output}{runtimeProbeExpectedIdentityDriftResult.Error}");
            Assert.Contains("run_0001_run_result_process_id_matches_runtime_probe_samples status=pass", runtimeProbeExpectedIdentityDriftResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_expected_process_start_time_matches_run_result status=fail", runtimeProbeExpectedIdentityDriftResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_expected_process_path_matches_run_result status=fail", runtimeProbeExpectedIdentityDriftResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runtimeProbeSamplesPath, originalRuntimeProbeSamplesJson);

            File.WriteAllText(
                runtimeProbeSamplesPath,
                Regex.Replace(
                    originalRuntimeProbeSamplesJson,
                    "(\"Phase\": \"runtime\"[\\s\\S]*?\"LogLengthBytes\": )\\d+",
                    "${1}1",
                    RegexOptions.CultureInvariant));
            var runtimeProbeLogGrowthResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(runtimeProbeLogGrowthResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{runtimeProbeLogGrowthResult.Output}{runtimeProbeLogGrowthResult.Error}");
            Assert.Contains("run_0001_run_result_runtime_log_length_growth_matches_log_grew status=pass", runtimeProbeLogGrowthResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_log_growth_matches_runtime_observation status=fail", runtimeProbeLogGrowthResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runtimeProbeSamplesPath, originalRuntimeProbeSamplesJson);

            File.WriteAllText(
                runtimeProbeSamplesPath,
                originalRuntimeProbeSamplesJson.Replace("\"Phase\": \"main-menu\"", "\"Phase\": \"startup\"", StringComparison.Ordinal));
            var missingMainMenuProbePhaseResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(missingMainMenuProbePhaseResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{missingMainMenuProbePhaseResult.Output}{missingMainMenuProbePhaseResult.Error}");
            Assert.Contains("run_0001_runtime_probe_samples_main_menu_phase_observed status=fail", missingMainMenuProbePhaseResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_runtime_probe_samples_runtime_phase_observed status=pass", missingMainMenuProbePhaseResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runtimeProbeSamplesPath, originalRuntimeProbeSamplesJson);

            File.WriteAllText(
                runResultPath,
                originalRunResultJson.Replace("\"EndTimestamp\": \"2026-06-18T10:00:30Z\"", "\"EndTimestamp\": \"not-a-timestamp\"", StringComparison.Ordinal));
            var invalidTimestampResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(invalidTimestampResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{invalidTimestampResult.Output}{invalidTimestampResult.Error}");
            Assert.Contains("run_0001_run_result_end_timestamp_parseable status=fail", invalidTimestampResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_timestamp_order_valid status=fail", invalidTimestampResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runResultPath, originalRunResultJson);

            File.WriteAllText(
                runResultPath,
                originalRunResultJson.Replace("\"StartTimestamp\": \"2026-06-18T10:00:00Z\"", "\"StartTimestamp\": \"2026-06-18T10:01:00Z\"", StringComparison.Ordinal));
            var reversedTimestampResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
                "-ExpectedRitsuCompatBranch",
                "0.107.0",
                "-ExpectedPatchCount",
                "25");
            Assert.True(reversedTimestampResult.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{reversedTimestampResult.Output}{reversedTimestampResult.Error}");
            Assert.Contains("run_0001_run_result_start_timestamp_parseable status=pass", reversedTimestampResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_end_timestamp_parseable status=pass", reversedTimestampResult.Output, StringComparison.Ordinal);
            Assert.Contains("run_0001_run_result_timestamp_order_valid status=fail", reversedTimestampResult.Output, StringComparison.Ordinal);
            File.WriteAllText(runResultPath, originalRunResultJson);

            var originalPlanJson = File.ReadAllText(planPath);
            File.WriteAllText(
                planPath,
                originalPlanJson.Replace("\"HookId\": \"SpirePlus.AutoSlayHarness.Start\"", "\"HookId\": \"\"", StringComparison.Ordinal));
            var hookFailResult = RunPowerShell(
                verifier,
                "-EvidenceDir",
                workdir,
                "-ExpectedPackageVersion",
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
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
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
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
                "v0.1.0-private-beta.87",
                "-ExpectedGameVersion",
                "0.107.0",
                "-ExpectedRitsuLibVersion",
                "0.4.24",
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
        var beforeLogPath = Path.Combine(iterationDir, "godot.log.before");
        var afterLaunchLogPath = Path.Combine(iterationDir, "godot.log.after-launch");
        var currentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
        File.WriteAllText(beforeLogPath, fullLog);
        File.WriteAllText(afterLaunchLogPath, fullLog + currentLog);
        File.WriteAllText(currentLogPath, currentLog);
        var offset = new FileInfo(beforeLogPath).Length;
        File.WriteAllText(
            Path.Combine(iterationDir, "iteration-result.json"),
            $$"""
            {
              "Iteration": {{iteration}},
              "Passed": false,
              "Command": {{JsonSerializer.Serialize(command)}},
              "ScenarioTag": {{JsonSerializer.Serialize(scenarioTag)}},
              "OwnerArea": {{JsonSerializer.Serialize(ownerArea)}},
              "GodotLogBeforePath": "godot.log.before",
              "GodotLogAfterLaunchPath": "godot.log.after-launch",
              "GodotLogCurrentIterationPath": "godot.log.current-iteration",
              "LogScanOffsetBytes": {{offset}},
              "FailureReasonCodes": {{failureReasonCodesJson}},
              "HangSignals": {{hangSignalsJson}}
            }
            """);
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
                "v0.1.0-private-beta.87",
                "release = v0.107.0",
                "RitsuLib Version: 0.4.24 [compat branch: 0.107.0]",
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
        const int gameProcessId = 1234;
        const int launcherProcessId = 4321;
        const int parentProcessId = 5678;
        const string liveSessionLaunchedAt = "2026-06-18T00:00:00.0000000Z";
        const string liveSessionLaunchReturnedAt = "2026-06-18T00:00:01.0000000Z";
        const string gameProcessStartTimeUtc = "2026-06-18T00:00:05.0000000Z";

        var iterationDir = Path.Combine(evidenceRoot, "iteration-0001");
        var shadowDir = Path.Combine(iterationDir, "shadow");
        Directory.CreateDirectory(iterationDir);
        Directory.CreateDirectory(shadowDir);

        var retainedBeforeLogPath = Path.Combine(iterationDir, "godot.log.before");
        var retainedAfterLaunchLogPath = Path.Combine(iterationDir, "godot.log.after-launch");
        var retainedCurrentLogPath = Path.Combine(iterationDir, "godot.log.current-iteration");
        var retainedProbeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
        var retainedPrepareOutputPath = Path.Combine(iterationDir, "prepare-output.json");
        var sourceWorkspaceCheckPath = Path.Combine(evidenceRoot, "local-godot-source-workspace-check.json");
        var resultBeforeLogPath = useShadowResultPaths ? Path.Combine(shadowDir, "godot.log.before") : retainedBeforeLogPath;
        var resultAfterLaunchLogPath = useShadowResultPaths ? Path.Combine(shadowDir, "godot.log.after-launch") : retainedAfterLaunchLogPath;
        var resultCurrentLogPath = useShadowResultPaths ? Path.Combine(shadowDir, "godot.log.current-iteration") : retainedCurrentLogPath;
        var resultProbeSamplesPath = useShadowResultPaths ? Path.Combine(shadowDir, "runtime-probe-samples.json") : retainedProbeSamplesPath;
        var gameProcessPath = Path.Combine(evidenceRoot, "SlayTheSpire2.exe");
        var steamExePath = Path.Combine(evidenceRoot, "steam.exe");
        var beforeLog = """
            [INFO] Previous unrelated Godot log content
            """;
        var currentLog = """
            [Startup] Time to main menu
            [INFO] [EZMicroBalance] [Patcher - SpirePlus] Patch application complete: 25 applied, 0 ignored, 0 failed, 25 total
            [INFO] [EZMicroBalance] ModPatcher applied 25 patches (25 registered).
            v0.1.0-private-beta.87
            [SPIREPLUS-EVIDENCE] VakuuFight fight_option_shown
            """;
        var afterLaunchLog = beforeLog + currentLog;
        var fixtureBeforeLogLength = System.Text.Encoding.UTF8.GetByteCount(beforeLog);
        var fixtureAfterLaunchLogLength = System.Text.Encoding.UTF8.GetByteCount(afterLaunchLog);
        const string startupSampledAt = "2026-06-18T00:00:12.0000000Z";
        const string runtimeSampledAt = "2026-06-18T00:00:22.0000000Z";
        var probeSamples = $$"""[{"Phase":"StartupMainMenu","SampledAt":{{JsonSerializer.Serialize(startupSampledAt)}},"LogExists":true,"LogLengthBytes":{{fixtureAfterLaunchLogLength}},"LogLastWriteTimeUtc":{{JsonSerializer.Serialize(startupSampledAt)}},"ProcessId":{{gameProcessId}},"ProcessStartTimeUtc":{{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},"ProcessPath":{{JsonSerializer.Serialize(gameProcessPath)}},"ExpectedGameProcessId":{{gameProcessId}},"ExpectedGameProcessStartTimeUtc":{{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},"ExpectedGameProcessPath":{{JsonSerializer.Serialize(gameProcessPath)}},"ProcessIdMatchesExpected":true,"ProcessStartTimeMatchesExpected":true,"ProcessPathMatchesExpected":true,"ProcessIdentityMatchesExpected":true,"ProcessObserved":true,"MainWindowObserved":true,"HungWindow":false,"Responding":true,"StaleProcessCount":0,"CurrentProcessCount":1,"UnknownStartTimeProcessCount":0,"AmbiguousCurrentProcessCount":0},{"Phase":"PostCommandRuntime","SampledAt":{{JsonSerializer.Serialize(runtimeSampledAt)}},"LogExists":true,"LogLengthBytes":{{fixtureAfterLaunchLogLength}},"LogLastWriteTimeUtc":{{JsonSerializer.Serialize(runtimeSampledAt)}},"ProcessId":{{gameProcessId}},"ProcessStartTimeUtc":{{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},"ProcessPath":{{JsonSerializer.Serialize(gameProcessPath)}},"ExpectedGameProcessId":{{gameProcessId}},"ExpectedGameProcessStartTimeUtc":{{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},"ExpectedGameProcessPath":{{JsonSerializer.Serialize(gameProcessPath)}},"ProcessIdMatchesExpected":true,"ProcessStartTimeMatchesExpected":true,"ProcessPathMatchesExpected":true,"ProcessIdentityMatchesExpected":true,"ProcessObserved":true,"MainWindowObserved":true,"HungWindow":false,"Responding":true,"StaleProcessCount":0,"CurrentProcessCount":1,"UnknownStartTimeProcessCount":0,"AmbiguousCurrentProcessCount":0}]""";

        File.WriteAllText(retainedBeforeLogPath, beforeLog);
        File.WriteAllText(retainedAfterLaunchLogPath, afterLaunchLog);
        File.WriteAllText(retainedCurrentLogPath, currentLog);
        File.WriteAllText(retainedProbeSamplesPath, probeSamples);
        File.WriteAllText(
            retainedPrepareOutputPath,
            $$"""
            {
              "EvidenceDir": {{JsonSerializer.Serialize(iterationDir)}},
              "LaunchKind": "SteamAppLaunch",
              "SteamAppId": "2868840",
              "LaunchFilePath": {{JsonSerializer.Serialize(steamExePath)}},
              "LaunchArgumentList": ["-applaunch", "2868840"],
              "LaunchedProcessId": {{launcherProcessId}},
              "LaunchedAt": {{JsonSerializer.Serialize(liveSessionLaunchedAt)}},
              "LaunchReturnedAt": {{JsonSerializer.Serialize(liveSessionLaunchReturnedAt)}},
              "PidAttributionSchemaVersion": 1,
              "PidAttributionPassed": true,
              "PidAttributionMethod": "fixture selected process",
              "PidProbeStartedAtUtc": "2026-06-18T00:00:01.0000000Z",
              "PidProbeFinishedAtUtc": "2026-06-18T00:00:06.0000000Z",
              "PreLaunchSlayProcessCount": 0,
              "PreLaunchSlayProcessIds": [],
              "SelectedGameProcessId": {{gameProcessId}},
              "SelectedGameProcessStartTimeUtc": {{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},
              "SelectedGameProcessPath": {{JsonSerializer.Serialize(gameProcessPath)}},
              "SelectedGameProcessParentProcessId": {{parentProcessId}},
              "ObservedGameProcessCandidates": [
                {
                  "ProcessName": "SlayTheSpire2",
                  "Id": {{gameProcessId}},
                  "StartTimeUtc": {{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},
                  "ProcessPath": {{JsonSerializer.Serialize(gameProcessPath)}},
                  "ParentProcessId": {{parentProcessId}},
                  "IsPreLaunchProcessId": false,
                  "IsStartedAfterMinimum": true
                }
              ],
              "AttributionFailureReason": ""
            }
            """);
        File.WriteAllText(Path.Combine(shadowDir, "godot.log.before"), beforeLog);
        File.WriteAllText(Path.Combine(shadowDir, "godot.log.after-launch"), afterLaunchLog);
        File.WriteAllText(Path.Combine(shadowDir, "godot.log.current-iteration"), currentLog);
        File.WriteAllText(Path.Combine(shadowDir, "runtime-probe-samples.json"), probeSamples);
        var retainedBeforeLogLength = new FileInfo(retainedBeforeLogPath).Length;
        var retainedBeforeLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(retainedBeforeLogPath))).ToLowerInvariant();
        var retainedAfterLaunchLogLength = new FileInfo(retainedAfterLaunchLogPath).Length;
        var retainedAfterLaunchLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(retainedAfterLaunchLogPath))).ToLowerInvariant();
        var retainedCurrentLogLength = new FileInfo(retainedCurrentLogPath).Length;
        var retainedCurrentLogHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(retainedCurrentLogPath))).ToLowerInvariant();
        var retainedPrepareOutputHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(retainedPrepareOutputPath))).ToLowerInvariant();
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
              "Game": {
                "Version": "v0.107.0",
                "Commit": "fixture",
                "Branch": "v0.107.0",
                "MainAssemblyHash": "12345"
              },
              "RecoveredSource": {
                "Version": "v0.107.0",
                "Commit": "fixture",
                "Branch": "v0.107.0",
                "MainAssemblyHash": "12345",
                "MatchesInstalledGame": true,
                "Disposition": "current-source-match",
                "OriginPckPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\SlayTheSpire2.pck",
                "OriginMatchesInstalledGamePck": true
              },
              "RitsuLib": {
                "Version": "0.4.24",
                "CompatBranch": "0.107.0",
                "RootPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib",
                "ManifestPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\mod_manifest.json",
                "ManifestSha256": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                "VariantsPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\ritsulib-variants.json",
                "VariantsSha256": "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                "ViewerPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\viewer\\index.html",
                "VariantDirectory": "lib\\0.107.0",
                "VariantAssembly": "STS2-RitsuLib.dll",
                "VariantDllPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\STS2-RitsuLib.dll",
                "VariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                "ExpectedVariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                "CompatTargetPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\compat-target.txt",
                "CompatTargetText": "0.107.0"
              },
              "EvidenceUsePolicy": {
                "NotRuntimeProof": true,
                "LocalSourceReferenceOnly": true,
                "AuthorizedLocalInstallOnly": true,
                "AuthorizedSourceOriginVerified": true,
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
              "ExpectedPackageVersion": "v0.1.0-private-beta.87",
              "ExpectedGameVersion": "0.107.0",
              "ExpectedRitsuLibVersion": "0.4.24",
              "ExpectedRitsuCompatBranch": "0.107.0",
              "ExpectedPatchCount": 25,
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
                "SourceBranch": "v0.107.0",
                "SourceMainAssemblyHash": "12345",
                "InstalledGameVersion": "v0.107.0",
                "InstalledGameCommit": "fixture",
                "InstalledGameBranch": "v0.107.0",
                "InstalledGameMainAssemblyHash": "12345",
                "Disposition": "current-source-match",
                "MatchesInstalledGame": true,
                "OriginPckPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\SlayTheSpire2.pck",
                "OriginMatchesInstalledGamePck": true,
                "RitsuLibVersion": "0.4.24",
                "RitsuLibCompatBranch": "0.107.0",
                "RitsuLibManifestPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\mod_manifest.json",
                "RitsuLibManifestSha256": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                "RitsuLibVariantsPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\ritsulib-variants.json",
                "RitsuLibVariantsSha256": "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb",
                "RitsuLibVariantDllPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\STS2-RitsuLib.dll",
                "RitsuLibVariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                "RitsuLibExpectedVariantDllSha256": "cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc",
                "RitsuLibCompatTargetPath": "E:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\lib\\0.107.0\\compat-target.txt",
                "RitsuLibCompatTargetText": "0.107.0",
                "RefreshSourceSnapshotBeforeCurrentApiClaims": false,
                "NotRuntimeProof": true,
                "AuthorizedSourceOriginVerified": true
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
              "LiveSessionBindingMissingCount": 0,
              "GodotLogBeforeMissingCount": 0,
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
              "LiveSessionPrepareOutputPath": {{JsonSerializer.Serialize(retainedPrepareOutputPath)}},
              "LiveSessionPrepareOutputSha256": {{JsonSerializer.Serialize(retainedPrepareOutputHash)}},
              "LiveSessionEvidenceDir": {{JsonSerializer.Serialize(iterationDir)}},
              "LiveSessionLauncherKind": "SteamAppLaunch",
              "LiveSessionSteamAppId": "2868840",
              "LiveSessionLaunchFilePath": {{JsonSerializer.Serialize(steamExePath)}},
              "LiveSessionLaunchArgumentList": ["-applaunch", "2868840"],
              "LiveSessionLaunchedProcessId": {{launcherProcessId}},
              "LiveSessionLaunchedAt": {{JsonSerializer.Serialize(liveSessionLaunchedAt)}},
              "LiveSessionLaunchReturnedAt": {{JsonSerializer.Serialize(liveSessionLaunchReturnedAt)}},
              "LiveSessionPidAttributionSchemaVersion": 1,
              "LiveSessionPidAttributionPassed": true,
              "LiveSessionPidAttributionMethod": "fixture selected process",
              "LiveSessionPidProbeStartedAtUtc": "2026-06-18T00:00:01.0000000Z",
              "LiveSessionPidProbeFinishedAtUtc": "2026-06-18T00:00:06.0000000Z",
              "LiveSessionPreLaunchSlayProcessCount": 0,
              "LiveSessionPreLaunchSlayProcessIds": [],
              "LiveSessionSelectedGameProcessId": {{gameProcessId}},
              "LiveSessionSelectedGameProcessStartTimeUtc": {{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},
              "LiveSessionSelectedGameProcessPath": {{JsonSerializer.Serialize(gameProcessPath)}},
              "LiveSessionSelectedGameProcessParentProcessId": {{parentProcessId}},
              "LiveSessionAttributionFailureReason": "",
              "GameProcessId": {{gameProcessId}},
              "GameProcessStartTimeUtc": {{JsonSerializer.Serialize(gameProcessStartTimeUtc)}},
              "GameProcessPath": {{JsonSerializer.Serialize(gameProcessPath)}},
              "GameProcessStartTimeAfterLiveSessionLaunch": true,
              "GameProcessIdMatchesLiveSession": true,
              "GameProcessStartTimeMatchesLiveSession": true,
              "GameProcessPathMatchesLiveSession": true,
              "MainWindowObserved": true,
              "MainMenuElapsedSeconds": 12.3,
              "MaxSecondsWithoutLogGrowth": 1,
              "MaxConsecutiveUnresponsiveSamples": 0,
              "StaleProcessObserved": false,
              "StaleProcessCount": 0,
              "LogCopied": true,
              "CurrentIterationLogCopied": true,
              "GodotLogBeforeCopied": true,
              "AuditClean": true,
              "ExpectationPassed": true,
              "Sts1ModeVerifierPassed": true,
              "RestoreSucceeded": true,
              "RuntimeProbeSamplesPath": {{JsonSerializer.Serialize(resultProbeSamplesPath)}},
              "GodotLogBeforePath": {{JsonSerializer.Serialize(resultBeforeLogPath)}},
              "GodotLogBeforeLengthBytes": {{retainedBeforeLogLength}},
              "GodotLogBeforeSha256": {{JsonSerializer.Serialize(retainedBeforeLogHash)}},
              "GodotLogAfterLaunchPath": {{JsonSerializer.Serialize(resultAfterLaunchLogPath)}},
              "GodotLogAfterLaunchLengthBytes": {{retainedAfterLaunchLogLength}},
              "GodotLogAfterLaunchSha256": {{JsonSerializer.Serialize(retainedAfterLaunchLogHash)}},
              "GodotLogCurrentIterationPath": {{JsonSerializer.Serialize(resultCurrentLogPath)}},
              "GodotLogCurrentIterationLengthBytes": {{retainedCurrentLogLength}},
              "GodotLogCurrentIterationSha256": {{JsonSerializer.Serialize(retainedCurrentLogHash)}},
              "PreLaunchLogLengthBytes": {{retainedBeforeLogLength}},
              "CurrentIterationLogPath": {{JsonSerializer.Serialize(resultCurrentLogPath)}},
              "LogScanOffsetBytes": {{retainedBeforeLogLength}},
              "MainMenuObservation": {
                "MainMenuReached": true,
                "ProcessObserved": true,
                "ProcessExitedAfterObservation": false,
                "HungWindowDetected": false,
                "StaleProcessObserved": false,
                "MaxStaleProcessCount": 0,
                "NoLogGrowthTimeoutExceeded": false,
                "LogGrew": true,
                "LogObserved": true,
                "LogInitialLengthBytes": {{fixtureBeforeLogLength}},
                "LogFinalLengthBytes": {{fixtureAfterLaunchLogLength}},
                "Passed": true,
                "Samples": 1,
                "MaxConsecutiveUnresponsiveSamples": 0
              },
              "RuntimeObservation": {
                "MainMenuReached": true,
                "ProcessObserved": true,
                "ProcessExitedAfterObservation": false,
                "HungWindowDetected": false,
                "StaleProcessObserved": false,
                "MaxStaleProcessCount": 0,
                "RuntimeLogGrowthRequired": true,
                "NoLogGrowthTimeoutExceeded": false,
                "LogGrew": true,
                "LogObserved": true,
                "LogInitialLengthBytes": {{fixtureBeforeLogLength}},
                "LogFinalLengthBytes": {{fixtureAfterLaunchLogLength}},
                "Passed": true,
                "Samples": 1,
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
        var output = new StringBuilder();
        var error = new StringBuilder();
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                output.AppendLine(e.Data);
            }
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is not null)
            {
                error.AppendLine(e.Data);
            }
        };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        if (!process.WaitForExit(30_000))
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException)
            {
            }

            Assert.Fail($"Timed out running {scriptPath}.");
        }

        process.WaitForExit();
        return (process.ExitCode, output.ToString(), error.ToString());
    }
}
