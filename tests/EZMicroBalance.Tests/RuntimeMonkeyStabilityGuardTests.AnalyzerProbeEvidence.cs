using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void RuntimeFailureAnalyzerReportsRuntimeMonkeyProbePhaseCoverageDefects()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamplesJson = File.ReadAllText(probeSamplesPath)
                .Replace("\"Phase\":\"PostCommandRuntime\"", "\"Phase\":\"StartupMainMenu\"", StringComparison.Ordinal);
            File.WriteAllText(probeSamplesPath, probeSamplesJson);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_runtime_phase_missing");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_startup_sample_count_mismatch");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_runtime_sample_count_mismatch");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_startup_phase_missing");
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
    public void RuntimeFailureAnalyzerReportsRuntimeMonkeyProbeSamplesPathMissing()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var resultPath = Path.Combine(iterationDir, "iteration-result.json");
            var resultJson = File.ReadAllText(resultPath);
            resultJson = Regex.Replace(
                resultJson,
                "\\s+\"RuntimeProbeSamplesPath\"\\s*:\\s*\"(?:\\\\.|[^\"])*\",",
                string.Empty,
                RegexOptions.CultureInvariant);
            resultJson = resultJson
                .Replace("\"Passed\": true,", "\"Passed\": false,", StringComparison.Ordinal)
                .Replace("\"FailureReasonCodes\": [],", "\"FailureReasonCodes\": [\"process_unresponsive\"],", StringComparison.Ordinal)
                .Replace("\"HangSignals\": [],", "\"HangSignals\": [\"process_unresponsive\"],", StringComparison.Ordinal);
            File.WriteAllText(resultPath, resultJson);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_samples_path_missing");
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "process_unresponsive" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
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
    public void RuntimeFailureAnalyzerReportsRuntimeMonkeyProbeSamplesPathBlank()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var resultPath = Path.Combine(iterationDir, "iteration-result.json");
            var resultJson = Regex.Replace(
                File.ReadAllText(resultPath),
                "\"RuntimeProbeSamplesPath\"\\s*:\\s*\"(?:\\\\.|[^\"])*\"",
                "\"RuntimeProbeSamplesPath\": \"   \"",
                RegexOptions.CultureInvariant)
                .Replace("\"Passed\": true,", "\"Passed\": false,", StringComparison.Ordinal)
                .Replace("\"FailureReasonCodes\": [],", "\"FailureReasonCodes\": [\"process_unresponsive\"],", StringComparison.Ordinal)
                .Replace("\"HangSignals\": [],", "\"HangSignals\": [\"process_unresponsive\"],", StringComparison.Ordinal);
            File.WriteAllText(resultPath, resultJson);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_samples_path_missing");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_samples_missing");
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "process_unresponsive" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "vakuu_command_failed_or_hung" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
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
    public void RuntimeFailureAnalyzerReportsRuntimeMonkeyProbeLogGrowthMismatch()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamplesJson = Regex.Replace(
                File.ReadAllText(probeSamplesPath),
                "(\"Phase\":\"PostCommandRuntime\"[^}]*\"LogLengthBytes\":)\\d+",
                "${1}1",
                RegexOptions.CultureInvariant);
            File.WriteAllText(probeSamplesPath, probeSamplesJson);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var findings = document.RootElement.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();

            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_runtime_log_growth_mismatch" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
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
    public void RuntimeFailureAnalyzerReportsRuntimeMonkeyProbeTimestampDefects()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamplesJson = File.ReadAllText(probeSamplesPath)
                .Replace("\"SampledAt\":\"2026-06-18T00:00:12.0000000Z\"", "\"SampledAt\":\"not-a-time\"", StringComparison.Ordinal)
                .Replace("\"LogLastWriteTimeUtc\":\"2026-06-18T00:00:12.0000000Z\"", "\"LogLastWriteTimeUtc\":\"not-a-time\"", StringComparison.Ordinal)
                .Replace("\"LogLastWriteTimeUtc\":\"2026-06-18T00:00:22.0000000Z\"", "\"LogLastWriteTimeUtc\":\"2999-01-01T00:00:00Z\"", StringComparison.Ordinal);
            File.WriteAllText(probeSamplesPath, probeSamplesJson);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_timestamp_invalid");
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
    public void RuntimeFailureAnalyzerReportsRuntimeMonkeyProbeProcessCountDefects()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var resultPath = Path.Combine(iterationDir, "iteration-result.json");
            var resultJson = File.ReadAllText(resultPath)
                .Replace("\"Passed\": true,", "\"Passed\": false,", StringComparison.Ordinal)
                .Replace("\"FailureReasonCodes\": [],", "\"FailureReasonCodes\": [\"process_unresponsive\"],", StringComparison.Ordinal)
                .Replace("\"HangSignals\": [],", "\"HangSignals\": [\"process_unresponsive\"],", StringComparison.Ordinal);
            File.WriteAllText(resultPath, resultJson);

            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamplesJson = File.ReadAllText(probeSamplesPath)
                .Replace("\"StaleProcessCount\":0", "\"StaleProcessCount\":1", StringComparison.Ordinal)
                .Replace("\"CurrentProcessCount\":1", "\"CurrentProcessCount\":2", StringComparison.Ordinal)
                .Replace("\"UnknownStartTimeProcessCount\":0", "\"UnknownStartTimeProcessCount\":1", StringComparison.Ordinal)
                .Replace("\"AmbiguousCurrentProcessCount\":0", "\"AmbiguousCurrentProcessCount\":1", StringComparison.Ordinal);
            File.WriteAllText(probeSamplesPath, probeSamplesJson);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_stale_process");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_unknown_start_time_process");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_ambiguous_current_process");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_current_process_count_invalid");
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "process_unresponsive" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
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
    public void RuntimeFailureAnalyzerDistrustsRuntimeMonkeyLogMetadataMismatch()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var resultPath = Path.Combine(iterationDir, "iteration-result.json");
            var resultJson = Regex.Replace(
                File.ReadAllText(resultPath),
                "\"GodotLogCurrentIterationSha256\"\\s*:\\s*\"[a-f0-9]{64}\"",
                "\"GodotLogCurrentIterationSha256\": \"dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd\"",
                RegexOptions.CultureInvariant);
            File.WriteAllText(resultPath, resultJson);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.Equal("Runtime.Unknown", iteration.GetProperty("OwnerAreaFromLog").GetString());
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_godot_log_metadata_mismatch" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
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
    public void RuntimeFailureAnalyzerReportsRuntimeMonkeyProbeProcessIdentityMismatch()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var resultPath = Path.Combine(iterationDir, "iteration-result.json");
            var resultJson = File.ReadAllText(resultPath)
                .Replace("\"Passed\": true,", "\"Passed\": false,", StringComparison.Ordinal)
                .Replace("\"FailureReasonCodes\": [],", "\"FailureReasonCodes\": [\"process_unresponsive\"],", StringComparison.Ordinal)
                .Replace("\"HangSignals\": [],", "\"HangSignals\": [\"process_unresponsive\"],", StringComparison.Ordinal);
            File.WriteAllText(resultPath, resultJson);

            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamplesJson = File.ReadAllText(probeSamplesPath)
                .Replace("\"ProcessId\":1234", "\"ProcessId\":9999", StringComparison.Ordinal);
            File.WriteAllText(probeSamplesPath, probeSamplesJson);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_process_identity_mismatch");
            Assert.Equal("RuntimeHarness", FindFindingOwner(iteration, "process_unresponsive"));
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
    public void RuntimeFailureAnalyzerReportsRuntimeMonkeyProbeSamplesInvalidJson()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var resultPath = Path.Combine(iterationDir, "iteration-result.json");
            var resultJson = File.ReadAllText(resultPath)
                .Replace("\"Passed\": true,", "\"Passed\": false,", StringComparison.Ordinal)
                .Replace("\"FailureReasonCodes\": [],", "\"FailureReasonCodes\": [\"process_unresponsive\"],", StringComparison.Ordinal)
                .Replace("\"HangSignals\": [],", "\"HangSignals\": [\"process_unresponsive\"],", StringComparison.Ordinal);
            File.WriteAllText(resultPath, resultJson);

            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            File.WriteAllText(probeSamplesPath, "{ invalid runtime probe sample json");

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_samples_invalid");
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "process_unresponsive" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "vakuu_command_failed_or_hung" &&
                    finding.GetProperty("OwnerArea").GetString() == "RuntimeHarness");
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }
}
