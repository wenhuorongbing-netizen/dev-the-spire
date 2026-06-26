using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
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
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
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
    public void RuntimeFailureAnalyzerRejectsRuntimeMonkeyProbeNumericStringEvidence()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamples = JsonNode.Parse(File.ReadAllText(probeSamplesPath))!.AsArray();
            foreach (var sampleNode in probeSamples)
            {
                var sample = sampleNode!.AsObject();
                sample["LogLengthBytes"] = JsonScalarValueToInvariantString(sample["LogLengthBytes"]);
                sample["ProcessId"] = JsonScalarValueToInvariantString(sample["ProcessId"]);
                sample["StaleProcessCount"] = JsonScalarValueToInvariantString(sample["StaleProcessCount"]);
                sample["CurrentProcessCount"] = JsonScalarValueToInvariantString(sample["CurrentProcessCount"]);
            }

            File.WriteAllText(probeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_numeric_malformed");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_log_length_negative");
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
    public void RuntimeFailureAnalyzerRejectsRuntimeMonkeyProbeObjectShapeEvidence()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            File.WriteAllText(
                probeSamplesPath,
                """
                {
                  "Phase": "StartupMainMenu",
                  "SampledAt": "2026-06-18T00:00:01.0000000Z",
                  "LogExists": true,
                  "LogLengthBytes": 1
                }
                """);
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-probe-object-shape.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_samples_shape_invalid");
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
    public void RuntimeFailureAnalyzerRejectsRuntimeMonkeyProbeBooleanStringEvidence()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamples = JsonNode.Parse(File.ReadAllText(probeSamplesPath))!.AsArray();
            foreach (var sampleNode in probeSamples)
            {
                var sample = sampleNode!.AsObject();
                sample["LogExists"] = "true";
                sample["ProcessObserved"] = "true";
                sample["HungWindow"] = "false";
                sample["Responding"] = "true";
            }

            File.WriteAllText(probeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-probe-boolean.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_boolean_malformed");
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
    public void RuntimeFailureAnalyzerAllowsNullRespondingBeforeMainWindowObserved()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamples = JsonNode.Parse(File.ReadAllText(probeSamplesPath))!.AsArray();
            var startupSample = probeSamples[0]!.AsObject();
            startupSample["MainWindowObserved"] = false;
            startupSample["Responding"] = null;
            File.WriteAllText(probeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-null-responding-before-window.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();

            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_boolean_malformed");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_not_responding");
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
    public void RuntimeFailureAnalyzerRejectsStartupProbeUnresponsiveDrift()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamples = JsonNode.Parse(File.ReadAllText(probeSamplesPath))!.AsArray();
            var startupSample = probeSamples[0]!.AsObject();
            startupSample["MainWindowObserved"] = true;
            startupSample["HungWindow"] = true;
            startupSample["Responding"] = false;
            File.WriteAllText(probeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-startup-unresponsive-drift.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_startup_unresponsive_mismatch");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_max_unresponsive_mismatch");
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
    public void RuntimeFailureAnalyzerRejectsStartupProbeThresholdBoundaryDrift()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamples = JsonNode.Parse(File.ReadAllText(probeSamplesPath))!.AsArray();
            var startupSample = probeSamples[0]!.AsObject();
            startupSample["MainWindowObserved"] = true;
            startupSample["HungWindow"] = true;
            startupSample["Responding"] = false;
            probeSamples.Insert(1, JsonNode.Parse(startupSample.ToJsonString())!);
            probeSamples.Insert(2, JsonNode.Parse(startupSample.ToJsonString())!);
            File.WriteAllText(probeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var resultPath = Path.Combine(iterationDir, "iteration-result.json");
            var resultJson = JsonNode.Parse(File.ReadAllText(resultPath))!.AsObject();
            resultJson["MaxConsecutiveUnresponsiveSamples"] = 3;
            var mainMenuObservation = resultJson["MainMenuObservation"]!.AsObject();
            mainMenuObservation["Samples"] = 3;
            mainMenuObservation["MaxConsecutiveUnresponsiveSamples"] = 3;
            mainMenuObservation["HungWindowDetected"] = false;
            File.WriteAllText(resultPath, resultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-startup-threshold-drift.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_startup_threshold_mismatch");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_startup_unresponsive_mismatch");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_max_unresponsive_mismatch");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_hung_window");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_not_responding");
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
    public void RuntimeFailureAnalyzerRejectsRuntimeProbeThresholdBoundaryDrift()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamples = JsonNode.Parse(File.ReadAllText(probeSamplesPath))!.AsArray();
            var runtimeSample = probeSamples[1]!.AsObject();
            runtimeSample["MainWindowObserved"] = true;
            runtimeSample["HungWindow"] = true;
            runtimeSample["Responding"] = false;
            probeSamples.Insert(2, JsonNode.Parse(runtimeSample.ToJsonString())!);
            probeSamples.Insert(3, JsonNode.Parse(runtimeSample.ToJsonString())!);
            File.WriteAllText(probeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var resultPath = Path.Combine(iterationDir, "iteration-result.json");
            var resultJson = JsonNode.Parse(File.ReadAllText(resultPath))!.AsObject();
            resultJson["MaxConsecutiveUnresponsiveSamples"] = 3;
            var runtimeObservation = resultJson["RuntimeObservation"]!.AsObject();
            runtimeObservation["Samples"] = 3;
            runtimeObservation["MaxConsecutiveUnresponsiveSamples"] = 3;
            runtimeObservation["HungWindowDetected"] = false;
            File.WriteAllText(resultPath, resultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-runtime-threshold-drift.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_runtime_threshold_mismatch");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_hung_window");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_not_responding");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_runtime_unresponsive_mismatch");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_max_unresponsive_mismatch");
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

    [Theory]
    [InlineData("string")]
    [InlineData("null")]
    [InlineData("zero")]
    [InlineData("missing")]
    public void RuntimeFailureAnalyzerRejectsMalformedPlanUnresponsiveThreshold(string thresholdShape)
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamples = JsonNode.Parse(File.ReadAllText(probeSamplesPath))!.AsArray();
            var startupSample = probeSamples[0]!.AsObject();
            startupSample["MainWindowObserved"] = true;
            startupSample["HungWindow"] = true;
            startupSample["Responding"] = false;
            probeSamples.Insert(1, JsonNode.Parse(startupSample.ToJsonString())!);
            probeSamples.Insert(2, JsonNode.Parse(startupSample.ToJsonString())!);
            File.WriteAllText(probeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var resultPath = Path.Combine(iterationDir, "iteration-result.json");
            var resultJson = JsonNode.Parse(File.ReadAllText(resultPath))!.AsObject();
            resultJson["MaxConsecutiveUnresponsiveSamples"] = 3;
            var mainMenuObservation = resultJson["MainMenuObservation"]!.AsObject();
            mainMenuObservation["Samples"] = 3;
            mainMenuObservation["MaxConsecutiveUnresponsiveSamples"] = 3;
            mainMenuObservation["HungWindowDetected"] = false;
            File.WriteAllText(resultPath, resultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var planPath = Path.Combine(workdir, "monkey-plan.json");
            var planJson = JsonNode.Parse(File.ReadAllText(planPath))!.AsObject();
            if (thresholdShape == "string")
            {
                planJson["UnresponsiveSampleThreshold"] = "3";
            }
            else if (thresholdShape == "null")
            {
                planJson["UnresponsiveSampleThreshold"] = null;
            }
            else if (thresholdShape == "zero")
            {
                planJson["UnresponsiveSampleThreshold"] = 0;
            }
            else if (thresholdShape == "missing")
            {
                planJson.Remove("UnresponsiveSampleThreshold");
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(thresholdShape), thresholdShape, null);
            }
            File.WriteAllText(planPath, planJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var outputPath = Path.Combine(workdir, $"runtime-failure-analysis-threshold-{thresholdShape}.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_plan_unresponsive_threshold_malformed");
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
    public void RuntimeFailureAnalyzerRejectsNullRespondingAfterMainWindowObserved()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamples = JsonNode.Parse(File.ReadAllText(probeSamplesPath))!.AsArray();
            var runtimeSample = probeSamples[1]!.AsObject();
            runtimeSample["MainWindowObserved"] = true;
            runtimeSample["Responding"] = null;
            File.WriteAllText(probeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-null-responding-after-window.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_boolean_malformed");
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
    public void RuntimeFailureAnalyzerRejectsRuntimeMonkeyPostCommandProbeHungAndNotRespondingEvidence()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamples = JsonNode.Parse(File.ReadAllText(probeSamplesPath))!.AsArray();
            var runtimeSample = probeSamples[1]!.AsObject();
            runtimeSample["HungWindow"] = true;
            runtimeSample["Responding"] = false;
            File.WriteAllText(probeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-probe-hung-not-responding.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_hung_window");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_not_responding");
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
    public void RuntimeFailureAnalyzerRejectsRuntimeMonkeyProbeIntegerValuedDecimalEvidence()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamplesJson = File.ReadAllText(probeSamplesPath);
            foreach (var fieldName in new[] { "LogLengthBytes", "ProcessId", "StaleProcessCount", "CurrentProcessCount" })
            {
                probeSamplesJson = Regex.Replace(
                    probeSamplesJson,
                    $"\"{fieldName}\"\\s*:\\s*\\d+",
                    $"\"{fieldName}\": 1.0",
                    RegexOptions.CultureInvariant);
            }

            File.WriteAllText(probeSamplesPath, probeSamplesJson);
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-decimal-probe-numeric.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_numeric_malformed");
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
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
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
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
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
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
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
    public void RuntimeFailureAnalyzerRejectsProbeLogLengthsBeyondRetainedAfterLaunch()
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
                "(\"Phase\":\"StartupMainMenu\"[^}]*\"LogLengthBytes\":)\\d+",
                "${1}999999999",
                RegexOptions.CultureInvariant);
            File.WriteAllText(probeSamplesPath, probeSamplesJson);
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(
                findings,
                finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_log_length_exceeds_retained_after_launch" &&
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
    public void RuntimeFailureAnalyzerReportsRuntimeMonkeyProbeTimelineDefects()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamples = JsonNode.Parse(File.ReadAllText(probeSamplesPath))!.AsArray();
            var firstSample = probeSamples[0]!.AsObject();
            var secondSample = probeSamples[1]!.AsObject();
            firstSample["Phase"] = "PostCommandRuntime";
            firstSample["SampledAt"] = "2026-06-18T00:00:22.0000000Z";
            firstSample["LogLastWriteTimeUtc"] = "2026-06-18T00:00:22.0000000Z";
            secondSample["Phase"] = "StartupMainMenu";
            secondSample["SampledAt"] = "2026-06-18T00:00:11.0000000Z";
            secondSample["LogLastWriteTimeUtc"] = "2026-06-18T00:00:11.0000000Z";
            secondSample["LogLengthBytes"] = 1;
            File.WriteAllText(probeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-probe-timeline.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_sampled_at_order_invalid");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_phase_order_invalid");
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_log_length_regression");
            Assert.DoesNotContain(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_timestamp_invalid");
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
    public void RuntimeFailureAnalyzerReportsRuntimeMonkeyProbeNegativeLogLength()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
            var probeSamples = JsonNode.Parse(File.ReadAllText(probeSamplesPath))!.AsArray();
            var runtimeSample = probeSamples[1]!.AsObject();
            runtimeSample["LogLengthBytes"] = -1;
            File.WriteAllText(probeSamplesPath, probeSamples.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-probe-negative-log-length.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Contains(findings, finding => finding.GetProperty("Signal").GetString() == "runtime_monkey_probe_log_length_negative");
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
            RefreshRuntimeProbeSamplesHash(iterationDir);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis.json");
            var result = RunPowerShell(script, "-IterationDir", iterationDir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var findings = root.GetProperty("HarnessBlockingFindings").EnumerateArray().ToArray();
            var iteration = FindIteration(root, 1);

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
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
            RefreshRuntimeProbeSamplesHash(iterationDir);

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
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
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
            RefreshRuntimeProbeSamplesHash(iterationDir);

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
            RefreshRuntimeProbeSamplesHash(iterationDir);

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

    private static void RefreshRuntimeProbeSamplesHash(string iterationDir)
    {
        var probeSamplesPath = Path.Combine(iterationDir, "runtime-probe-samples.json");
        var resultPath = Path.Combine(iterationDir, "iteration-result.json");
        var summaryPath = Path.Combine(Directory.GetParent(iterationDir)!.FullName, "monkey-summary.json");
        var probeSamplesHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(probeSamplesPath))).ToLowerInvariant();
        var resultJson = Regex.Replace(
            File.ReadAllText(resultPath),
            "\"RuntimeProbeSamplesSha256\"\\s*:\\s*\"[a-f0-9]{64}\"",
            $"\"RuntimeProbeSamplesSha256\": {JsonSerializer.Serialize(probeSamplesHash)}",
            RegexOptions.CultureInvariant);
        File.WriteAllText(resultPath, resultJson);
        if (File.Exists(summaryPath))
        {
            var summaryJson = Regex.Replace(
                File.ReadAllText(summaryPath),
                "\"RuntimeProbeSamplesSha256\"\\s*:\\s*\"[a-f0-9]{64}\"",
                $"\"RuntimeProbeSamplesSha256\": {JsonSerializer.Serialize(probeSamplesHash)}",
                RegexOptions.CultureInvariant);
            File.WriteAllText(summaryPath, summaryJson);
        }
    }
}
