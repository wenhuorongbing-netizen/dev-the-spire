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

    private static void RewriteRuntimeMonkeyPacketJsonObject(string path, Action<JsonObject> mutate)
    {
        var json = JsonNode.Parse(File.ReadAllText(path))!.AsObject();
        mutate(json);
        File.WriteAllText(path, json.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
    }
}
