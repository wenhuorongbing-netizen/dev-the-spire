using System.Diagnostics;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ReleaseEvidenceGateTests
{
    private sealed record RequiredEvidence(string Key, string Description, Func<bool> IsPresent);

    [Fact]
    public void EvidenceCollectionScriptsCreatePendingNoLaunchTemplates()
    {
        foreach (var scriptName in new[]
        {
            "collect-release-evidence.ps1",
            "collect-future-peek-evidence.ps1",
            "collect-coop-evidence.ps1"
        })
        {
            var script = AssertRepoFileExists("scripts", scriptName);
            var source = ReadRepoText("scripts", scriptName);

            Assert.Contains("[switch]$NoLaunch", source, StringComparison.Ordinal);
            Assert.Contains(".tools\\runtime-evidence", source, StringComparison.Ordinal);
            Assert.Contains("command.txt", source, StringComparison.Ordinal);
            Assert.Contains("environment.json", source, StringComparison.Ordinal);
            Assert.Contains("package-hashes.json", source, StringComparison.Ordinal);
            Assert.Contains("manual-rows-template.json", source, StringComparison.Ordinal);
            Assert.DoesNotContain("EZFuturePeekCode", source, StringComparison.Ordinal);
            Assert.DoesNotContain("EZFuturePeek.json", source, StringComparison.Ordinal);
            Assert.DoesNotContain("EZFuturePeek.sln", source, StringComparison.Ordinal);

            var evidenceDir = RepoPath(
                ".tools",
                "runtime-evidence",
                "test-release-evidence-gate",
                Path.GetFileNameWithoutExtension(scriptName),
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(evidenceDir);
            try
            {
                var result = RunPowerShell(script, "-NoLaunch", "-EvidenceDir", evidenceDir);
                Assert.True(result.ExitCode == 0, $"{scriptName} -NoLaunch failed:{Environment.NewLine}{result.Output}");

                Assert.True(File.Exists(Path.Combine(evidenceDir, "command.txt")), $"{scriptName} did not write command.txt.");
                Assert.True(File.Exists(Path.Combine(evidenceDir, "environment.json")), $"{scriptName} did not write environment.json.");
                Assert.True(File.Exists(Path.Combine(evidenceDir, "package-hashes.json")), $"{scriptName} did not write package-hashes.json.");
                Assert.True(File.Exists(Path.Combine(evidenceDir, "manual-rows-template.json")), $"{scriptName} did not write manual-rows-template.json.");

                using var rowsDocument = JsonDocument.Parse(File.ReadAllText(Path.Combine(evidenceDir, "manual-rows-template.json")));
                var rows = rowsDocument.RootElement.GetProperty("Rows").EnumerateArray().ToArray();
                Assert.NotEmpty(rows);
                Assert.All(rows, row =>
                {
                    var status = row.GetProperty("Status").GetString();
                    Assert.Equal("pending", status);
                    Assert.NotEqual("passed", status, StringComparer.OrdinalIgnoreCase);
                    Assert.NotEqual("pass", status, StringComparer.OrdinalIgnoreCase);
                });
            }
            finally
            {
                if (Directory.Exists(evidenceDir))
                {
                    Directory.Delete(evidenceDir, recursive: true);
                }
            }
        }
    }

    [Fact]
    public void DefaultModeDoesNotRequireLiveEvidenceButBlocksReleaseReadyClaims()
    {
        var enforce = string.Equals(
            Environment.GetEnvironmentVariable("EZMB_ENFORCE_RELEASE_READY"),
            "1",
            StringComparison.Ordinal);

        if (enforce)
        {
            var missing = RequiredReleaseEvidence()
                .Where(requirement => !requirement.IsPresent())
                .Select(requirement => $"{requirement.Key}: {requirement.Description}")
                .ToArray();

            Assert.True(
                missing.Length == 0,
                "EZMB_ENFORCE_RELEASE_READY=1 requires complete live release evidence. Missing:" +
                Environment.NewLine +
                string.Join(Environment.NewLine, missing));
            return;
        }

        var currentDocs = ReadCurrentFacingDocs(
            "PROJECT_STATE.md",
            "README.md",
            "docs/release-checklist.md",
            "docs/release-evidence-status.md",
            "docs/specs/release-traceability-matrix.md",
            "docs/private-beta-release-completion-audit.md",
            "docs/private-beta-verification-handoff.md");

        Assert.Contains("Fresh current-package loader smoke | Pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Clicked Ancient UI | Pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Save/load | Pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Co-op disposition | Pending", currentDocs, StringComparison.Ordinal);
        Assert.Contains("Preview tools", currentDocs, StringComparison.Ordinal);
        foreach (var falseClaim in new[]
        {
            "currently release-ready",
            "is release-ready",
            "release-ready: true",
            "publish-proven: true",
            "full co-op support: true",
            "full multiplayer support: true"
        })
        {
            Assert.DoesNotContain(falseClaim, currentDocs, StringComparison.OrdinalIgnoreCase);
        }

        var projectState = ReadRepoText("PROJECT_STATE.md");
        Assert.Contains("current source defines 26 SavedSpireFields", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("live loader parity remains pending", projectState, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Found 22 SavedSpireFields", projectState, StringComparison.OrdinalIgnoreCase);
    }

    private static RequiredEvidence[] RequiredReleaseEvidence()
    {
        return
        [
            new(
                "fresh-loader-smoke",
                "Fresh current-package loader smoke with current package hashes and clean log audit",
                () => HasEvidenceDirectory(
                    "release-evidence-*",
                    "command.txt",
                    "environment.json",
                    "package-hashes.json",
                    "godot.log",
                    "godot-log-audit.json")),
            new(
                "clicked-ancient-ui",
                "Clicked Urda/Morvi/Lotha/Vakuu Ancient UI screenshots plus foreground/log evidence",
                () => HasEvidenceDirectory(
                    "ancient-ui-click-*",
                    "command.txt",
                    "window-preflight.json",
                    "godot.log",
                    "godot-log-audit.json",
                    "route-note.md") &&
                    Directory.GetFiles(RuntimeEvidenceRoot(), "ancient-ui-click-*", SearchOption.AllDirectories)
                        .Any(path => path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))),
            new(
                "save-load",
                "Ancient and gameplay save/load evidence",
                () => HasEvidenceDirectoryContaining("save-load", "godot.log", "godot-log-audit.json")),
            new(
                "vakuu",
                "Vakuu victory/no-black-screen/failure/death evidence",
                () => HasEvidenceDirectoryContaining("vakuu", "godot.log", "godot-log-audit.json", "vakuu-release-evidence-pass.json")),
            new(
                "coop",
                "Two-client host/client co-op evidence",
                () => HasEvidenceDirectory(
                    "coop-evidence-*",
                    "host/command.txt",
                    "host/godot.log",
                    "host/godot-log-audit.json",
                    "client/command.txt",
                    "client/godot.log",
                    "client/godot-log-audit.json")),
            new(
                "preview-tools",
                "Live Preview tools evidence for Crystal Sphere and transform preview",
                () => HasEvidenceDirectory(
                    "preview-tools-evidence-*",
                    "command.txt",
                    "environment.json",
                    "package-hashes.json",
                    "godot.log",
                    "godot-log-audit.json")),
            new(
                "release-evidence-verifier",
                "verify-spire-plus-release-evidence.ps1 pass marker",
                HasVerifierPassMarker)
        ];
    }

    private static bool HasEvidenceDirectory(string searchPattern, params string[] requiredRelativeFiles)
    {
        var root = RuntimeEvidenceRoot();
        if (!Directory.Exists(root))
        {
            return false;
        }

        return Directory.GetDirectories(root, searchPattern, SearchOption.AllDirectories)
            .Any(directory => requiredRelativeFiles.All(relativeFile => File.Exists(Path.Combine(directory, relativeFile))));
    }

    private static bool HasEvidenceDirectoryContaining(string directoryNameFragment, params string[] requiredRelativeFiles)
    {
        var root = RuntimeEvidenceRoot();
        if (!Directory.Exists(root))
        {
            return false;
        }

        return Directory.GetDirectories(root, "*", SearchOption.AllDirectories)
            .Where(directory => directory.Contains(directoryNameFragment, StringComparison.OrdinalIgnoreCase))
            .Any(directory => requiredRelativeFiles.All(relativeFile => File.Exists(Path.Combine(directory, relativeFile))));
    }

    private static bool HasVerifierPassMarker()
    {
        var root = RuntimeEvidenceRoot();
        if (!Directory.Exists(root))
        {
            return false;
        }

        foreach (var marker in Directory.GetFiles(root, "release-evidence-verifier-pass.json", SearchOption.AllDirectories))
        {
            try
            {
                using var document = JsonDocument.Parse(File.ReadAllText(marker));
                var rootElement = document.RootElement;
                if (rootElement.TryGetProperty("Status", out var status) &&
                    string.Equals(status.GetString(), "pass", StringComparison.OrdinalIgnoreCase) &&
                    rootElement.TryGetProperty("Verifier", out var verifier) &&
                    verifier.GetString()?.Contains("verify-spire-plus-release-evidence.ps1", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }
            }
            catch (JsonException)
            {
            }
        }

        return false;
    }

    private static string RuntimeEvidenceRoot()
    {
        return RepoPath(".tools", "runtime-evidence");
    }

    private static (int ExitCode, string Output) RunPowerShell(string scriptPath, params string[] arguments)
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
        var output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000), $"Timed out running {scriptPath}.");
        return (process.ExitCode, output);
    }
}
