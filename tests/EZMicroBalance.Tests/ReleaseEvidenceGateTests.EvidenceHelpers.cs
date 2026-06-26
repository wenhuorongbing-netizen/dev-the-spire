using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class ReleaseEvidenceGateTests
{
    private const string OwnerLiveReleaseLogOrigin = "owner-live-release-log";

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
        const int scriptTimeoutMilliseconds = 180_000;
        var executable = OperatingSystem.IsWindows() ? "powershell.exe" : "pwsh";
        var tempRoot = Path.Combine(Path.GetTempPath(), "SpirePlusTests", Guid.NewGuid().ToString("N"));
        var invocationPath = Path.Combine(tempRoot, "invocation.json");
        var stdoutPath = Path.Combine(tempRoot, "stdout.txt");
        var stderrPath = Path.Combine(tempRoot, "stderr.txt");
        var wrapperPath = Path.Combine(tempRoot, "invoke-script.ps1");
        Directory.CreateDirectory(tempRoot);
        File.WriteAllText(
            invocationPath,
            JsonSerializer.Serialize(new { ScriptPath = scriptPath, Arguments = arguments }));
        File.WriteAllText(
            wrapperPath,
            """
            param(
                [Parameter(Mandatory = $true)][string]$InvocationPath,
                [Parameter(Mandatory = $true)][string]$StdoutPath,
                [Parameter(Mandatory = $true)][string]$StderrPath
            )

            $ErrorActionPreference = 'Stop'
            $payload = Get-Content -Raw -LiteralPath $InvocationPath | ConvertFrom-Json
            $scriptArguments = @()
            $argumentsValue = $payload.Arguments
            if ($null -ne $argumentsValue) {
                if ($argumentsValue -is [System.Array]) {
                    foreach ($argument in $argumentsValue) {
                        $scriptArguments += [string]$argument
                    }
                } else {
                    $scriptArguments += [string]$argumentsValue
                }
            }

            try {
                $powerShellExe = (Get-Process -Id $PID).Path
                if ([string]::IsNullOrWhiteSpace($powerShellExe) -or -not (Test-Path -LiteralPath $powerShellExe)) {
                    $powerShellExe = 'powershell.exe'
                }

                $childArgs = @('-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', [string]$payload.ScriptPath) + $scriptArguments
                $scriptOutput = & $powerShellExe @childArgs 2>&1
                $scriptOutput | ForEach-Object { $_.ToString() } | Set-Content -LiteralPath $StdoutPath -Encoding UTF8
                if ($null -eq $LASTEXITCODE) {
                    exit 0
                }

                exit ([int]$LASTEXITCODE)
            } catch {
                $_ | Out-String | Set-Content -LiteralPath $StderrPath -Encoding UTF8
                exit 1
            }
            """);

        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            WorkingDirectory = Root,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-ExecutionPolicy");
        startInfo.ArgumentList.Add("Bypass");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(wrapperPath);
        startInfo.ArgumentList.Add("-InvocationPath");
        startInfo.ArgumentList.Add(invocationPath);
        startInfo.ArgumentList.Add("-StdoutPath");
        startInfo.ArgumentList.Add(stdoutPath);
        startInfo.ArgumentList.Add("-StderrPath");
        startInfo.ArgumentList.Add(stderrPath);

        using var process = new Process
        {
            StartInfo = startInfo
        };

        Assert.True(process.Start(), $"Failed to start {scriptPath}.");
        if (!process.WaitForExit(scriptTimeoutMilliseconds))
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException)
            {
            }

            process.WaitForExit();
            Assert.Fail($"Timed out running {scriptPath}.{Environment.NewLine}{ReadPowerShellOutput(stdoutPath, stderrPath)}");
        }

        var capturedOutput = ReadPowerShellOutput(stdoutPath, stderrPath);
        try
        {
            Directory.Delete(tempRoot, recursive: true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }

        return (process.ExitCode, capturedOutput);
    }

    private static string ReadPowerShellOutput(string stdoutPath, string stderrPath)
    {
        return
            (File.Exists(stdoutPath) ? File.ReadAllText(stdoutPath) : string.Empty) +
            (File.Exists(stderrPath) ? File.ReadAllText(stderrPath) : string.Empty);
    }

    private static void AssertChecklistTemplate(
        JsonElement[] rows,
        string evidenceDir,
        string rowId,
        string requiredChecklist,
        string templateFile,
        string[] expectedFragments)
    {
        var row = rows.Single(row => row.GetProperty("Id").GetString() == rowId);
        var requiredFiles = row
            .GetProperty("RequiredFiles")
            .EnumerateArray()
            .Select(file => file.GetString())
            .ToArray();
        Assert.Contains(requiredChecklist, requiredFiles);

        var rowDir = Path.Combine(evidenceDir, rowId);
        var templatePath = Path.Combine(rowDir, templateFile);
        Assert.True(File.Exists(templatePath), $"{rowId} did not get {templateFile}.");
        var rowReadme = File.ReadAllText(Path.Combine(rowDir, "README.md"));
        var checklist = File.ReadAllText(templatePath);
        Assert.Contains("Manual checkpoints:", rowReadme, StringComparison.Ordinal);
        AssertTemplateChecklistCreated(checklist, requiredChecklist);
        foreach (var expectedFragment in expectedFragments)
        {
            Assert.Contains(expectedFragment, checklist, StringComparison.Ordinal);
        }

        AssertWorkingChecklistCreated(rowDir, requiredChecklist, expectedFragments);
    }

    private static string AssertWorkingChecklistCreated(
        string rowDir,
        string checklistFile,
        string[] expectedFragments)
    {
        var workingPath = Path.Combine(rowDir, checklistFile);
        Assert.True(File.Exists(workingPath), $"Missing generated working checklist: {checklistFile}.");
        var workingChecklist = File.ReadAllText(workingPath);
        Assert.Contains("Fill this checklist with live results before marking this row pass.", workingChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Copy this file to", workingChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Template reference for", workingChecklist, StringComparison.Ordinal);
        foreach (var expectedFragment in expectedFragments)
        {
            Assert.Contains(expectedFragment, workingChecklist, StringComparison.Ordinal);
        }

        return workingChecklist;
    }

    private static void AssertTemplateChecklistCreated(string templateChecklist, string checklistFile)
    {
        Assert.Contains($"Template reference for `{checklistFile}`.", templateChecklist, StringComparison.Ordinal);
        Assert.Contains($"Fill the working `{checklistFile}` with live results before marking this row pass.", templateChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Copy this file to", templateChecklist, StringComparison.Ordinal);
    }

    private static string CleanGodotLogAuditJson(string logPath)
    {
        var auditScript = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var auditResult = RunPowerShell(auditScript, "-Path", logPath);
        Assert.True(auditResult.ExitCode == 0, $"audit-godot-log.ps1 failed:{Environment.NewLine}{auditResult.Output}");
        return auditResult.Output;
    }

    private static void WriteOwnerLiveLogOrigin(JsonObject rowNode, string logFiles = "godot.log")
    {
        var evidenceDir = rowNode["EvidenceDir"]!.GetValue<string>();
        File.WriteAllText(
            Path.Combine(evidenceDir, "log-origin-note.md"),
            string.Join(
                Environment.NewLine,
                $"LogOriginProofStatus: {OwnerLiveReleaseLogOrigin}",
                "Source: synthetic owner/live release row for verifier contract.",
                $"Log files: {logFiles}",
                string.Empty));
        rowNode["LogOriginProofStatus"] = OwnerLiveReleaseLogOrigin;
    }

    private static void PrepareChecklistPassAttempt(
        JsonObject rowNode,
        string templateFile,
        string checklistFile,
        string requiredNoteFile,
        string noteText,
        string resultNote)
    {
        var evidenceDir = rowNode["EvidenceDir"]!.GetValue<string>();
        var godotLogPath = Path.Combine(evidenceDir, "godot.log");
        File.WriteAllText(
            godotLogPath,
            $"Synthetic live log for {checklistFile} verifier contract.");
        File.WriteAllText(
            Path.Combine(evidenceDir, "godot-log-audit.json"),
            CleanGodotLogAuditJson(godotLogPath));
        File.WriteAllText(Path.Combine(evidenceDir, requiredNoteFile), noteText);
        File.Copy(
            Path.Combine(evidenceDir, templateFile),
            Path.Combine(evidenceDir, checklistFile),
            overwrite: true);

        rowNode["Status"] = "pass";
        rowNode["ResultNote"] = resultNote;
        rowNode["ExplicitOwnerDecision"] = false;
        rowNode["ReleaseNote"] = "";
        WriteOwnerLiveLogOrigin(rowNode);
    }

    private static void WriteTinyPng(string path, int width, int height)
    {
        static byte[] UInt32BigEndian(int value)
        {
            return
            [
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            ];
        }

        var bytes = new List<byte>
        {
            137,
            80,
            78,
            71,
            13,
            10,
            26,
            10,
            0,
            0,
            0,
            13,
            (byte)'I',
            (byte)'H',
            (byte)'D',
            (byte)'R'
        };
        bytes.AddRange(UInt32BigEndian(width));
        bytes.AddRange(UInt32BigEndian(height));
        bytes.AddRange(new byte[] { 8, 6, 0, 0, 0, 0, 0, 0, 0 });
        File.WriteAllBytes(path, bytes.ToArray());
    }
}
