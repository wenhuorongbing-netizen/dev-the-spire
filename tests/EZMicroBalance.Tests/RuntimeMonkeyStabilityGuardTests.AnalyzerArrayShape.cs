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
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", summaryShapeFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("Runs must be retained as a native JSON array", summaryShapeFinding.GetProperty("Rationale").GetString(), StringComparison.Ordinal);
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
        Assert.False(iteration.GetProperty("LogTextTrustedForOwner").GetBoolean());
        Assert.Equal("RuntimeHarness", summaryCounterFinding.GetProperty("OwnerArea").GetString());
        Assert.Contains("Passed", rationale, StringComparison.Ordinal);
        Assert.Contains("FailedRuns", rationale, StringComparison.Ordinal);
        Assert.Contains("AncientIdCounts", rationale, StringComparison.Ordinal);
    }

}
