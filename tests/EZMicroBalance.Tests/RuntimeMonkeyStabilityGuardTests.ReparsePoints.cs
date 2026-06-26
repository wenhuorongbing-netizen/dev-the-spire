using System.Diagnostics;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class RuntimeMonkeyStabilityGuardTests
{
    [Fact]
    public void RuntimeMonkeyPacketCheckerRejectsJunctionedIterationEvidence()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var script = AssertRepoFileExists("scripts", "check-spire-plus-runtime-monkey-packet.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        var externalRoot = Path.Combine(Path.GetTempPath(), "runtime-monkey-packet-checker-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);
        Directory.CreateDirectory(externalRoot);
        var iterationDir = Path.Combine(workdir, "iteration-0001");
        var externalIterationDir = Path.Combine(externalRoot, "iteration-0001");

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            Directory.Move(iterationDir, externalIterationDir);
            if (!TryCreateDirectoryJunction(iterationDir, externalIterationDir))
            {
                return;
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
            Assert.Contains("iteration-0001_directory_reparse_point_free status=fail", result.Output, StringComparison.Ordinal);
            Assert.Contains("iteration-0001_canonical_artifact_paths_reparse_point_free status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryReparsePoint(iterationDir);
            DeleteDirectoryIfExists(workdir);
            DeleteDirectoryIfExists(externalRoot);
        }
    }

    [Fact]
    public void GameNativeAutoSlayPacketVerifierRejectsJunctionedRunEvidence()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var externalRoot = Path.Combine(Path.GetTempPath(), "autoslay-packet-verifier-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(externalRoot);
        using var fixture = CreateGameNativeAutoSlayFixture();
        var runDir = fixture.RunDir;
        var externalRunDir = Path.Combine(externalRoot, "run-0001");

        try
        {
            Directory.Move(runDir, externalRunDir);
            if (!TryCreateDirectoryJunction(runDir, externalRunDir))
            {
                return;
            }

            var result = fixture.RunVerifier();

            Assert.True(result.ExitCode == 0, $"AutoSlay packet verifier crashed:{Environment.NewLine}{result.Output}{result.Error}");
            Assert.Contains("run_0001_canonical_artifact_paths_reparse_point_free status=fail", result.Output, StringComparison.Ordinal);
        }
        finally
        {
            DeleteDirectoryReparsePoint(runDir);
            DeleteDirectoryIfExists(externalRoot);
        }
    }

    [Fact]
    public void RuntimeFailureAnalyzerRoutesJunctionedEvidenceToRuntimeHarness()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        var externalRoot = Path.Combine(Path.GetTempPath(), "runtime-monkey-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);
        Directory.CreateDirectory(externalRoot);
        var iterationDir = Path.Combine(workdir, "iteration-0001");
        var externalIterationDir = Path.Combine(externalRoot, "iteration-0001");
        var outputPath = Path.Combine(workdir, "analysis.json");

        try
        {
            WriteCleanRuntimeMonkeyPacket(workdir, useShadowResultPaths: false);
            Directory.Move(iterationDir, externalIterationDir);
            if (!TryCreateDirectoryJunction(iterationDir, externalIterationDir))
            {
                return;
            }

            var result = RunPowerShell(script, "-EvidenceDir", workdir, "-OutFile", outputPath);

            Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var finding = document
                .RootElement
                .GetProperty("Iterations")
                .EnumerateArray()
                .SelectMany(iteration => iteration.GetProperty("Findings").EnumerateArray())
                .Single(item => item.GetProperty("Signal").GetString() == "runtime_evidence_reparse_point_path");
            Assert.Equal("RuntimeHarness", finding.GetProperty("OwnerArea").GetString());
        }
        finally
        {
            DeleteDirectoryReparsePoint(iterationDir);
            DeleteDirectoryIfExists(workdir);
            DeleteDirectoryIfExists(externalRoot);
        }
    }

    [Theory]
    [InlineData("autoslay-summary.json")]
    [InlineData("autoslay-plan.json")]
    public void RuntimeFailureAnalyzerRoutesSymlinkedTopLevelAutoSlayMetadataToRuntimeHarness(string fileName)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var script = AssertRepoFileExists("scripts", "analyze-spire-plus-runtime-failure.ps1");
        var externalRoot = Path.Combine(Path.GetTempPath(), "autoslay-analyzer-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(externalRoot);
        using var fixture = CreateGameNativeAutoSlayFixture();
        var linkPath = Path.Combine(fixture.Workdir, fileName);
        var externalPath = Path.Combine(externalRoot, fileName);
        var outputPath = Path.Combine(fixture.Workdir, $"analysis-{Path.GetFileNameWithoutExtension(fileName)}.json");

        try
        {
            File.Copy(linkPath, externalPath);
            File.Delete(linkPath);
            if (!TryCreateFileSymlink(linkPath, externalPath))
            {
                return;
            }

            var result = RunPowerShell(script, "-EvidenceDir", fixture.Workdir, "-OutFile", outputPath);

            Assert.True(result.ExitCode == 0, $"Analyzer crashed:{Environment.NewLine}{result.Output}{result.Error}");
            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var finding = document
                .RootElement
                .GetProperty("Iterations")
                .EnumerateArray()
                .SelectMany(iteration => iteration.GetProperty("Findings").EnumerateArray())
                .Single(item => item.GetProperty("Signal").GetString() == "runtime_evidence_reparse_point_path");
            Assert.Equal("RuntimeHarness", finding.GetProperty("OwnerArea").GetString());
        }
        finally
        {
            DeleteFileReparsePoint(linkPath);
            DeleteDirectoryIfExists(externalRoot);
        }
    }

    private static bool TryCreateDirectoryJunction(string linkPath, string targetPath)
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("/c");
        startInfo.ArgumentList.Add("mklink");
        startInfo.ArgumentList.Add("/J");
        startInfo.ArgumentList.Add(linkPath);
        startInfo.ArgumentList.Add(targetPath);

        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        if (!process.WaitForExit(30_000))
        {
            process.Kill(entireProcessTree: true);
            return false;
        }

        return process.ExitCode == 0 &&
            Directory.Exists(linkPath) &&
            (File.GetAttributes(linkPath) & FileAttributes.ReparsePoint) != 0;
    }

    private static bool TryCreateFileSymlink(string linkPath, string targetPath)
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.ArgumentList.Add("/c");
        startInfo.ArgumentList.Add("mklink");
        startInfo.ArgumentList.Add(linkPath);
        startInfo.ArgumentList.Add(targetPath);

        using var process = Process.Start(startInfo);
        Assert.NotNull(process);
        if (!process.WaitForExit(30_000))
        {
            process.Kill(entireProcessTree: true);
            return false;
        }

        return process.ExitCode == 0 &&
            File.Exists(linkPath) &&
            (File.GetAttributes(linkPath) & FileAttributes.ReparsePoint) != 0;
    }

    private static void DeleteDirectoryReparsePoint(string path)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
        {
            Directory.Delete(path);
        }
    }

    private static void DeleteFileReparsePoint(string path)
    {
        if (!File.Exists(path))
        {
            return;
        }

        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
        {
            File.Delete(path);
        }
    }

    private static void DeleteDirectoryIfExists(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }
    }
}
