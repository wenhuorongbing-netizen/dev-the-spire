using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void RuntimeMonkeyPacketCheckerRejectsMalformedPerIterationArrayEvidence()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);

            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var iterationResultPath = Path.Combine(iterationDir, "iteration-result.json");
            var iterationResultJson = JsonNode.Parse(File.ReadAllText(iterationResultPath))!.AsObject();
            iterationResultJson["FailureReasonCodes"] = "";
            iterationResultJson["HangSignals"] = "process_unresponsive";
            iterationResultJson["LiveSessionPreLaunchSlayProcessIds"] = "1234";
            File.WriteAllText(iterationResultPath, iterationResultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var prepareOutputPath = Path.Combine(iterationDir, "prepare-output.json");
            var prepareOutputJson = JsonNode.Parse(File.ReadAllText(prepareOutputPath))!.AsObject();
            prepareOutputJson["PreLaunchSlayProcessIds"] = "1234";
            File.WriteAllText(prepareOutputPath, prepareOutputJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var summaryPath = Path.Combine(workdir, "monkey-summary.json");
            var summaryJson = JsonNode.Parse(File.ReadAllText(summaryPath))!.AsObject();
            var summaryResultJson = summaryJson["Results"]!.AsArray()[0]!.AsObject();
            summaryResultJson["FailureReasonCodes"] = "";
            summaryResultJson["HangSignals"] = "process_unresponsive";
            File.WriteAllText(summaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            File.WriteAllText(
                Path.Combine(iterationDir, "runtime-probe-samples.json"),
                """
                {
                  "Phase": "StartupMainMenu"
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
            Assert.Contains("iteration-0001_failure_reason_codes_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_hang_signals_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_result_live_session_prelaunch_slay_process_ids_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_prepare_output_prelaunch_slay_process_ids_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_failure_reason_codes_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_hang_signals_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_array status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsMalformedSessionAndRestoreArrayEvidence()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);

            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var sessionStatePath = Path.Combine(iterationDir, "session-state.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                sessionStatePath,
                sessionStateJson =>
                {
                    sessionStateJson["MovedMods"] = "not-array";
                    sessionStateJson["MovedCurrentRuns"] = "not-array";
                });

            var restoreStatePath = Path.Combine(iterationDir, "restore-state.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                restoreStatePath,
                restoreStateJson =>
                {
                    restoreStateJson["StoppedProcesses"] = "not-array";
                    restoreStateJson["PostRestoreSlayProcessIds"] = "not-array";
                    restoreStateJson["PostRestoreGodotProcessIds"] = "not-array";
                });

            var sessionStateHash = Sha256File(sessionStatePath);
            var restoreStateHash = Sha256File(restoreStatePath);
            RewriteRuntimeMonkeyPacketJsonObject(
                Path.Combine(iterationDir, "iteration-result.json"),
                iterationResultJson =>
                {
                    iterationResultJson["LiveSessionSessionStateSha256"] = sessionStateHash;
                    iterationResultJson["LiveSessionRestoreStateSha256"] = restoreStateHash;
                });
            RewriteRuntimeMonkeyPacketJsonObject(
                Path.Combine(workdir, "monkey-summary.json"),
                summaryJson =>
                {
                    var summaryResultJson = summaryJson["Results"]!.AsArray()[0]!.AsObject();
                    summaryResultJson["LiveSessionSessionStateSha256"] = sessionStateHash;
                    summaryResultJson["LiveSessionRestoreStateSha256"] = restoreStateHash;
                });

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_session_state_moved_mods_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_session_state_moved_current_runs_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_restore_state_stopped_processes_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_restore_state_post_restore_slay_process_ids_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_restore_state_post_restore_godot_process_ids_array status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_live_session_session_state_sha256_matches_retained_file status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_live_session_restore_state_sha256_matches_retained_file status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_live_session_session_state_sha256_matches_iteration status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_live_session_restore_state_sha256_matches_iteration status=pass", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsMalformedNumericEvidence()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);

            var planPath = Path.Combine(workdir, "monkey-plan.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                planPath,
                planJson =>
                {
                    planJson["Iterations"] = "1";
                    planJson["ExpectedPatchCount"] = "25";
                });

            var summaryPath = Path.Combine(workdir, "monkey-summary.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                summaryPath,
                summaryJson =>
                {
                    summaryJson["RequestedIterations"] = "1";
                    summaryJson["UnresponsiveIterationCount"] = "0";
                    summaryJson["MaxMainMenuElapsedSeconds"] = "12.3";
                    var summaryResultJson = summaryJson["Results"]!.AsArray()[0]!.AsObject();
                    summaryResultJson["Iteration"] = "1";
                    summaryResultJson["MaxSecondsWithoutLogGrowth"] = "1";
                    summaryResultJson["MainMenuElapsedSeconds"] = "12.3";
                });

            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var iterationResultPath = Path.Combine(iterationDir, "iteration-result.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                iterationResultPath,
                iterationResultJson =>
                {
                    iterationResultJson["Iteration"] = "1";
                    iterationResultJson["GameProcessId"] = "4242";
                    iterationResultJson["MainMenuElapsedSeconds"] = "12.3";
                    iterationResultJson["MaxSecondsWithoutLogGrowth"] = "1";
                    var mainMenuObservationJson = iterationResultJson["MainMenuObservation"]!.AsObject();
                    mainMenuObservationJson["Samples"] = "1";
                    mainMenuObservationJson["LogInitialLengthBytes"] = "21";
                    mainMenuObservationJson["LogFinalLength"] = "203";
                    var runtimeObservationJson = iterationResultJson["RuntimeObservation"]!.AsObject();
                    runtimeObservationJson["Samples"] = "1";
                    runtimeObservationJson["LogFinalLengthBytes"] = "203";
                });

            var runtimeProbeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamplesJson = JsonNode.Parse(File.ReadAllText(runtimeProbeSamplesPath))!.AsArray();
            var firstProbeSample = probeSamplesJson[0]!.AsObject();
            firstProbeSample["LogLengthBytes"] = "203";
            firstProbeSample["ProcessId"] = "4242";
            firstProbeSample["StaleProcessCount"] = "0";
            File.WriteAllText(runtimeProbeSamplesPath, probeSamplesJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            var runtimeProbeSamplesHash = Sha256File(runtimeProbeSamplesPath);
            RewriteRuntimeMonkeyPacketJsonObject(
                iterationResultPath,
                iterationResultJson => iterationResultJson["RuntimeProbeSamplesSha256"] = runtimeProbeSamplesHash);
            RewriteRuntimeMonkeyPacketJsonObject(
                summaryPath,
                summaryJson =>
                {
                    var summaryResultJson = summaryJson["Results"]!.AsArray()[0]!.AsObject();
                    summaryResultJson["RuntimeProbeSamplesSha256"] = runtimeProbeSamplesHash;
                });

            var auditPath = Path.Combine(iterationDir, "godot-log-audit.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                auditPath,
                auditJson =>
                {
                    auditJson["Length"] = auditJson["Length"]!.GetValue<long>().ToString(CultureInfo.InvariantCulture);
                    auditJson["SignatureHits"] = new JsonObject
                    {
                        ["Name"] = "fixture-zero-count",
                        ["Count"] = "0",
                    };
                });

            var sts1ModeLogCheckPath = Path.Combine(iterationDir, "sts1-mode-log-check.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                sts1ModeLogCheckPath,
                sts1ModeLogCheckJson =>
                {
                    sts1ModeLogCheckJson["LogLength"] = sts1ModeLogCheckJson["LogLength"]!.GetValue<long>().ToString(CultureInfo.InvariantCulture);
                });

            var prepareOutputPath = Path.Combine(iterationDir, "prepare-output.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                prepareOutputPath,
                prepareOutputJson =>
                {
                    prepareOutputJson["LaunchedProcessId"] = "4321";
                    prepareOutputJson["PidAttributionSchemaVersion"] = "1";
                    prepareOutputJson["PreLaunchSlayProcessCount"] = "0";
                    prepareOutputJson["SelectedGameProcessId"] = "1234";
                    prepareOutputJson["SelectedGameProcessParentProcessId"] = "5678";
                });

            var restoreStatePath = Path.Combine(iterationDir, "restore-state.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                restoreStatePath,
                restoreStateJson =>
                {
                    restoreStateJson["RestoreSchemaVersion"] = "1";
                    restoreStateJson["RestoredModCount"] = "0";
                    restoreStateJson["RestoredCurrentRunCount"] = "0";
                    restoreStateJson["PreservedNewCurrentRunCount"] = "0";
                    restoreStateJson["PostRestoreSlayProcessCount"] = "0";
                    restoreStateJson["PostRestoreGodotProcessCount"] = "0";
                });

            var prepareOutputHash = Sha256File(prepareOutputPath);
            var restoreStateHash = Sha256File(restoreStatePath);
            RewriteRuntimeMonkeyPacketJsonObject(
                iterationResultPath,
                iterationResultJson =>
                {
                    iterationResultJson["LiveSessionPrepareOutputSha256"] = prepareOutputHash;
                    iterationResultJson["LiveSessionRestoreStateSha256"] = restoreStateHash;
                });
            RewriteRuntimeMonkeyPacketJsonObject(
                summaryPath,
                summaryJson =>
                {
                    var summaryResultJson = summaryJson["Results"]!.AsArray()[0]!.AsObject();
                    summaryResultJson["LiveSessionPrepareOutputSha256"] = prepareOutputHash;
                    summaryResultJson["LiveSessionRestoreStateSha256"] = restoreStateHash;
                });

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("plan_iterations_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("plan_expected_patch_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_requested_iterations_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_unresponsive_iteration_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_max_main_menu_elapsed_seconds_number status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("summary_max_main_menu_elapsed_recorded status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_iteration_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_game_process_id_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_main_menu_elapsed_seconds_number status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_main_menu_elapsed_recorded status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_main_menu_observation_samples_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_main_menu_observation_log_final_length_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_observation_log_final_length_bytes_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_iteration_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_summary_result_main_menu_elapsed_seconds_number status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_log_length_bytes_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_process_id_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_array_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_sts1_mode_log_check_log_length_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_prepare_output_launched_process_id_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_prepare_output_prelaunch_slay_process_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_restore_state_restore_schema_version_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_restore_state_post_restore_slay_process_count_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_runtime_probe_samples_sha256_matches_retained_file status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_live_session_prepare_output_sha256_matches_retained_file status=pass", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_live_session_restore_state_sha256_matches_retained_file status=pass", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsInvalidAuditJsonAsFailedRows()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            File.WriteAllText(auditPath, "{ bad json");

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_array_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_schema_fields_current status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("ConvertFrom-Json", result.Output + result.Error, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsObjectRootAuditEvidence()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            var auditRoot = JsonNode.Parse(File.ReadAllText(auditPath))!.AsArray();
            File.WriteAllText(auditPath, auditRoot[0]!.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_array_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsScalarRootAuditEvidence()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            File.WriteAllText(auditPath, "\"not an audit array\"");

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_array_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_schema_fields_current status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsAuditMissingSchemaVersion()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                auditPath,
                auditJson => auditJson.Remove("AuditSchemaVersion"));

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_schema_fields_current status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_has_single_schema_version status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsOutOfRangeAuditSchemaVersion()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                auditPath,
                auditJson => auditJson["AuditSchemaVersion"] = 2147483648L);

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_schema_fields_current status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_has_single_schema_version status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("OverflowException", result.Output + result.Error, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsAuditWrongSignatureSetHash()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                auditPath,
                auditJson => auditJson["SignatureSetSha256"] = new string('0', 64));

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_signature_set_matches_recomputed status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsAuditEmptySignatureHitVector()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                auditPath,
                auditJson => auditJson["SignatureHits"] = new JsonArray());

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
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
    public void RuntimeMonkeyPacketCheckerRejectsOutOfRangeAuditSignatureCount()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                auditPath,
                auditJson =>
                {
                    var signatureHits = auditJson["SignatureHits"]!.AsArray();
                    signatureHits[0]!.AsObject()["Count"] = 2147483648L;
                });

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("OverflowException", result.Output + result.Error, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsAuditSignatureNameSetDrift()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                auditPath,
                auditJson => auditJson["SignatureHits"]!.AsArray().RemoveAt(0));

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_signature_names_current status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsAuditSignatureHitMissingCount()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                auditPath,
                auditJson =>
                {
                    auditJson["SignatureHits"] = new JsonArray(
                        new JsonObject
                        {
                            ["Name"] = "Godot ERROR line",
                        });
                });

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsAuditSignatureHitStringCount()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                auditPath,
                auditJson =>
                {
                    auditJson["SignatureHits"] = new JsonArray(
                        new JsonObject
                        {
                            ["Name"] = "Godot ERROR line",
                            ["Count"] = "0",
                        });
                });

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsAuditMissingLength()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                auditPath,
                auditJson => auditJson.Remove("Length"));

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsOutOfRangeAuditLength()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var auditPath = Path.Combine(workdir, "iteration-0001", "godot-log-audit.json");
            RewriteRuntimeMonkeyPacketJsonObject(
                auditPath,
                auditJson => auditJson["Length"] = decimal.Parse("9223372036854775808", CultureInfo.InvariantCulture));

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("iteration-0001_audit_numeric_fields_native status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_has_single_length status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_audit_clean status=fail", result.Output, StringComparison.Ordinal);
            Assert.DoesNotContain("OverflowException", result.Output + result.Error, StringComparison.Ordinal);
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
    public void RuntimeMonkeyPacketCheckerRejectsIntegerValuedDecimalNumericEvidence()
    {
        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);

            var planPath = Path.Combine(workdir, "monkey-plan.json");
            File.WriteAllText(
                planPath,
                File.ReadAllText(planPath)
                    .Replace("\"Iterations\": 1", "\"Iterations\": 1.0", StringComparison.Ordinal)
                    .Replace("\"ExpectedPatchCount\": 25", "\"ExpectedPatchCount\": 25.0", StringComparison.Ordinal));

            var result = RunPowerShell(
                script,
                "-EvidenceDir",
                workdir,
                "-ExpectedIterations",
                "1",
                "-ExpectedPatchCount",
                "25");

            Assert.True(result.ExitCode == 0, $"Packet checker crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("plan_iterations_integer status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("plan_expected_patch_count_integer status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    private static void RewriteRuntimeMonkeyPacketJsonObject(string path, Action<JsonObject> mutate)
    {
        var root = JsonNode.Parse(File.ReadAllText(path))!;
        var json = GetSingleJsonObject(root);
        mutate(json);
        File.WriteAllText(path, root.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }

    private static JsonObject GetSingleJsonObject(JsonNode root)
    {
        return root is JsonArray array
            ? array[0]!.AsObject()
            : root.AsObject();
    }
}
