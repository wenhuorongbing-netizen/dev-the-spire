using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void RuntimeFailureAnalyzerRejectsMalformedRuntimeMonkeySummaryArrayShape()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var summaryPath = Path.Combine(workdir, "monkey-summary.json");
            var summaryJson = JsonNode.Parse(File.ReadAllText(summaryPath))!.AsObject();
            summaryJson["Results"] = summaryJson["Results"]!.AsArray()[0]!.DeepClone();
            summaryJson["FailedIterationIds"] = "1";
            File.WriteAllText(summaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-summary-array-shape.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);
            var summaryShapeFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "runtime_monkey_summary_counter_mismatch");
            var rationale = summaryShapeFinding.GetProperty("Rationale").GetString()!;

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Equal("RuntimeHarness", summaryShapeFinding.GetProperty("OwnerArea").GetString());
            Assert.Contains("Results must be retained as a native JSON array", rationale, StringComparison.Ordinal);
            Assert.Contains("FailedIterationIds must be retained as a native JSON array", rationale, StringComparison.Ordinal);
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
    public void RuntimeFailureAnalyzerAcceptsCleanRuntimeMonkeySummaryBooleanRows()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-clean-summary-booleans.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);
            var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

            Assert.DoesNotContain(findings, item => item.GetProperty("Signal").GetString() == "runtime_monkey_summary_boolean_malformed");
            Assert.DoesNotContain(findings, item => item.GetProperty("Signal").GetString() == "runtime_monkey_summary_result_boolean_malformed");
            Assert.DoesNotContain(findings, item => item.GetProperty("Signal").GetString() == "runtime_monkey_result_boolean_malformed");
            Assert.True(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.True(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
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
    public void RuntimeFailureAnalyzerRejectsRuntimeMonkeyScalarSignalArrays()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var resultPath = Path.Combine(iterationDir, "iteration-result.json");
            var resultJson = JsonNode.Parse(File.ReadAllText(resultPath))!.AsObject();
            resultJson["Passed"] = false;
            resultJson["FailureReasonCodes"] = "process_unresponsive";
            resultJson["HangSignals"] = "runtime_not_responding";
            File.WriteAllText(resultPath, resultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var summaryPath = Path.Combine(workdir, "monkey-summary.json");
            var summaryJson = JsonNode.Parse(File.ReadAllText(summaryPath))!.AsObject();
            var summaryResultJson = summaryJson["Results"]!.AsArray()[0]!.AsObject();
            summaryResultJson["Passed"] = false;
            summaryResultJson["FailureReasonCodes"] = "process_unresponsive";
            summaryResultJson["HangSignals"] = "runtime_not_responding";
            File.WriteAllText(summaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-scalar-signal-arrays.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);
            var signalShapeFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "runtime_monkey_signal_array_shape_invalid");
            var rationale = signalShapeFinding.GetProperty("Rationale").GetString()!;

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("RuntimeMonkeyProbeArtifactTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Equal("RuntimeHarness", signalShapeFinding.GetProperty("OwnerArea").GetString());
            Assert.Contains("iteration-result.json.FailureReasonCodes", rationale, StringComparison.Ordinal);
            Assert.Contains("iteration-result.json.HangSignals", rationale, StringComparison.Ordinal);
            Assert.Contains("monkey-summary.json.Results[].FailureReasonCodes", rationale, StringComparison.Ordinal);
            Assert.Contains("monkey-summary.json.Results[].HangSignals", rationale, StringComparison.Ordinal);
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
    public void RuntimeFailureAnalyzerRejectsMalformedRuntimeMonkeySummaryJson()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            File.WriteAllText(Path.Combine(workdir, "monkey-summary.json"), "{ invalid json");

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-malformed-summary-json.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);
            var summaryFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "runtime_monkey_summary_counter_mismatch");
            var rationale = summaryFinding.GetProperty("Rationale").GetString()!;

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Equal("RuntimeHarness", summaryFinding.GetProperty("OwnerArea").GetString());
            Assert.Contains("monkey-summary.json must parse as JSON", rationale, StringComparison.Ordinal);
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
    public void RuntimeFailureAnalyzerRejectsRuntimeMonkeySummaryMissingResults()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var summaryPath = Path.Combine(workdir, "monkey-summary.json");
            var summaryJson = JsonNode.Parse(File.ReadAllText(summaryPath))!.AsObject();
            summaryJson.Remove("Results");
            File.WriteAllText(summaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-summary-missing-results.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);
            var summaryFinding = iteration
                .GetProperty("Findings")
                .EnumerateArray()
                .Single(item => item.GetProperty("Signal").GetString() == "runtime_monkey_summary_counter_mismatch");
            var rationale = summaryFinding.GetProperty("Rationale").GetString()!;

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Equal("RuntimeHarness", summaryFinding.GetProperty("OwnerArea").GetString());
            Assert.Contains("Results must be retained as a native JSON array", rationale, StringComparison.Ordinal);
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
    public void RuntimeFailureAnalyzerRejectsRuntimeMonkeyMissingBooleanEvidence()
    {
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            var iterationDir = Path.Combine(workdir, "iteration-0001");
            var resultPath = Path.Combine(iterationDir, "iteration-result.json");
            var resultJson = JsonNode.Parse(File.ReadAllText(resultPath))!.AsObject();
            resultJson.Remove("CommandAckObserved");
            File.WriteAllText(resultPath, resultJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var summaryPath = Path.Combine(workdir, "monkey-summary.json");
            var summaryJson = JsonNode.Parse(File.ReadAllText(summaryPath))!.AsObject();
            summaryJson.Remove("Passed");
            var summaryResultJson = summaryJson["Results"]!.AsArray()[0]!.AsObject();
            summaryResultJson.Remove("CommandAckObserved");
            File.WriteAllText(summaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

            var outputPath = Path.Combine(workdir, "runtime-failure-analysis-missing-boolean-evidence.json");
            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);
            Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var root = document.RootElement;
            var iteration = FindIteration(root, 1);
            var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
            var summaryFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "runtime_monkey_summary_boolean_malformed");
            var resultFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "runtime_monkey_summary_result_boolean_malformed");

            Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
            Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
            Assert.False(iteration.GetProperty("RuntimeMonkeyRunArtifactsTrustedForOwner").GetBoolean());
            Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
            Assert.Equal("RuntimeHarness", summaryFinding.GetProperty("OwnerArea").GetString());
            Assert.Equal("RuntimeHarness", resultFinding.GetProperty("OwnerArea").GetString());
            Assert.Contains("Summary.Passed missing", summaryFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
            Assert.Contains("iteration-result.json.CommandAckObserved missing", resultFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
            Assert.Contains("monkey-summary.json.Results[].CommandAckObserved missing", resultFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
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
    public void RuntimeFailureAnalyzerAcceptsCleanGameNativeAutoSlaySummaryBooleanRows()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-clean-summary-booleans.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();

        Assert.DoesNotContain(findings, item => item.GetProperty("Signal").GetString() == "autoslay_summary_boolean_malformed");
        Assert.DoesNotContain(findings, item => item.GetProperty("Signal").GetString() == "autoslay_passed_boolean_malformed");
        Assert.True(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.True(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.True(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsMalformedGameNativeAutoSlaySummaryRunsShape()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        summaryJson["Runs"] = summaryJson["Runs"]!.AsArray()[0]!.DeepClone();
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-runs-shape.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var summaryShapeFinding = iteration
            .GetProperty("Findings")
            .EnumerateArray()
            .Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_shape_invalid");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySts1ModeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("Sts1ModeLogCheckTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", summaryShapeFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("Runs must be retained as a native JSON array", summaryShapeFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsMalformedGameNativeAutoSlaySummaryJson()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        File.WriteAllText(fixture.SummaryPath, "{ invalid json");

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-malformed-summary-json.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var summaryShapeFinding = iteration
            .GetProperty("Findings")
            .EnumerateArray()
            .Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_shape_invalid");
        var rationale = summaryShapeFinding.GetProperty("Rationale").GetString()!;

        Assert.Equal(1, root.GetProperty("AnalyzedIterationCount").GetInt32());
        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySts1ModeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("Sts1ModeLogCheckTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", summaryShapeFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("autoslay-summary.json must parse as JSON", rationale, StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlaySummaryMissingRunnerKind()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        summaryJson.Remove("RunnerKind");
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-missing-runner-kind.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var summaryShapeFinding = iteration
            .GetProperty("Findings")
            .EnumerateArray()
            .Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_shape_invalid");
        var rationale = summaryShapeFinding.GetProperty("Rationale").GetString()!;

        Assert.Equal(1, root.GetProperty("AnalyzedIterationCount").GetInt32());
        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySts1ModeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("Sts1ModeLogCheckTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", summaryShapeFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("RunnerKind must be GameNativeAutoSlay", rationale, StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlaySummaryRunMissingPassed()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        summaryJson["Runs"]!.AsArray()[0]!.AsObject().Remove("Passed");
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-summary-run-missing-passed.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var summaryBooleanFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_boolean_malformed");
        var passedFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "autoslay_passed_boolean_malformed");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySts1ModeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("Sts1ModeLogCheckTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", summaryBooleanFinding.GetProperty("OwnerArea").GetString());
        Assert.Equal("RuntimeHarness", passedFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("Summary.Runs[].Passed missing", summaryBooleanFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
        Assert.Contains("autoslay-summary.json.Runs[].Passed missing", passedFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsMalformedGameNativeAutoSlaySummaryPassedOnly()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        summaryJson["Passed"] = "true";
        summaryJson["Runs"]!.AsArray()[0]!.AsObject()["Passed"] = "true";
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-summary-passed-string.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var findings = iteration.GetProperty("Findings").EnumerateArray().ToArray();
        var summaryBooleanFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_boolean_malformed");
        var passedFinding = findings.Single(item => item.GetProperty("Signal").GetString() == "autoslay_passed_boolean_malformed");

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySts1ModeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("Sts1ModeLogCheckTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", summaryBooleanFinding.GetProperty("OwnerArea").GetString());
        Assert.Equal("RuntimeHarness", passedFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("Summary.Passed must be retained as a native JSON boolean", summaryBooleanFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
        Assert.Contains("Summary.Runs[].Passed must be retained as a native JSON boolean", summaryBooleanFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
        Assert.Contains("autoslay-summary.json.Runs[].Passed must be retained as a native JSON boolean", passedFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlaySummaryCounterDrift()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        summaryJson["Passed"] = false;
        summaryJson["FailedRuns"] = 1;
        summaryJson["AncientIdCounts"] = new JsonObject { ["VAKUU"] = 0 };
        File.WriteAllText(fixture.SummaryPath, summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-counter-drift.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var summaryCounterFinding = iteration
            .GetProperty("Findings")
            .EnumerateArray()
            .Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_counter_mismatch");
        var rationale = summaryCounterFinding.GetProperty("Rationale").GetString()!;

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySts1ModeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("Sts1ModeLogCheckTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", summaryCounterFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("Passed", rationale, StringComparison.Ordinal);
        Assert.Contains("FailedRuns", rationale, StringComparison.Ordinal);
        Assert.Contains("AncientIdCounts", rationale, StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerReportsGameNativeAutoSlaySummaryNumericMalformedSignal()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        summaryJson["TotalRuns"] = "1";
        summaryJson["FailedRuns"] = "0";
        summaryJson["AncientIdCounts"]!.AsObject()["VAKUU"] = "1";
        File.WriteAllText(
            fixture.SummaryPath,
            summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true })
                .Replace("\"FailedRuns\": \"0\"", "\"FailedRuns\": 0.0", StringComparison.Ordinal));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-summary-numeric-malformed.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var malformedFinding = iteration
            .GetProperty("Findings")
            .EnumerateArray()
            .Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_numeric_malformed");
        var rationale = malformedFinding.GetProperty("Rationale").GetString()!;

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySts1ModeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("Sts1ModeLogCheckTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", malformedFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("Summary.TotalRuns", rationale, StringComparison.Ordinal);
        Assert.Contains("Summary.FailedRuns", rationale, StringComparison.Ordinal);
        Assert.Contains("AncientIdCounts.VAKUU", rationale, StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerReportsGameNativeAutoSlaySummaryPlanNumericMalformedSignal()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var planJson = JsonNode.Parse(File.ReadAllText(fixture.PlanPath))!.AsObject();
        planJson["ExpectedPatchCount"] = "25";
        File.WriteAllText(fixture.PlanPath, planJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var summaryJson = JsonNode.Parse(File.ReadAllText(fixture.SummaryPath))!.AsObject();
        summaryJson["ExpectedPatchCount"] = "25";
        File.WriteAllText(
            fixture.SummaryPath,
            summaryJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true })
                .Replace("\"ExpectedPatchCount\": \"25\"", "\"ExpectedPatchCount\": 25.0", StringComparison.Ordinal));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-summary-plan-numeric-malformed.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var malformedFinding = iteration
            .GetProperty("Findings")
            .EnumerateArray()
            .Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_plan_numeric_malformed");
        var rationale = malformedFinding.GetProperty("Rationale").GetString()!;

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySts1ModeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("Sts1ModeLogCheckTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", malformedFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("Plan.ExpectedPatchCount", rationale, StringComparison.Ordinal);
        Assert.Contains("Summary.ExpectedPatchCount", rationale, StringComparison.Ordinal);
    }

    [Fact]
    public void RuntimeFailureAnalyzerRejectsGameNativeAutoSlayCaseDriftedSummaryPlanFields()
    {
        using var fixture = CreateGameNativeAutoSlayFixture();
        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");

        var planJson = JsonNode.Parse(File.ReadAllText(fixture.PlanPath))!.AsObject();
        var expectedPatchCount = planJson["ExpectedPatchCount"]!.DeepClone();
        planJson.Remove("ExpectedPatchCount");
        planJson["expectedpatchcount"] = expectedPatchCount;
        File.WriteAllText(fixture.PlanPath, planJson.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

        var outputPath = Path.Combine(fixture.Workdir, "runtime-failure-analysis-autoslay-case-drifted-plan-field.json");
        var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);
        Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");

        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        var root = document.RootElement;
        var iteration = FindIteration(root, 1);
        var finding = iteration
            .GetProperty("Findings")
            .EnumerateArray()
            .Single(item => item.GetProperty("Signal").GetString() == "autoslay_summary_plan_mismatch");
        var rationale = finding.GetProperty("Rationale").GetString()!;

        Assert.Equal("HarnessEvidenceInvalid", root.GetProperty("TriageDisposition").GetString());
        Assert.Equal(0, root.GetProperty("GameplayBlockingFindingCount").GetInt32());
        Assert.False(iteration.GetProperty("AutoSlayRunArtifactsTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayProbeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlayAuditArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySts1ModeArtifactTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("AutoSlaySidecarTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("Sts1ModeLogCheckTrustedForOwner").GetBoolean());
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", finding.GetProperty("OwnerArea").GetString());
        Assert.Contains("ExpectedPatchCount", rationale, StringComparison.Ordinal);
    }

}
