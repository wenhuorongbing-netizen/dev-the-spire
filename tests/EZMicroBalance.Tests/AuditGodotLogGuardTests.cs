using System.Diagnostics;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class AuditGodotLogGuardTests
{
    [Fact]
    public void AuditSkipsKnownThirdPartyManifestWarnings()
    {
        var script = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "audit-godot-log-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        var logPath = Path.Combine(workdir, "known-noise-godot.log");
        File.WriteAllText(
            logPath,
            "[INFO] Found mod manifest file D:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\RouteSuggest-v1.9.0\\RouteSuggest.json\r\n" +
            "[ERROR] Mod manifest D:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\RouteSuggest-v1.9.0\\RouteSuggestConfig.json is missing the 'id' field! This is not allowed. The mod will not be loaded.\r\n" +
            "[ERROR] Mod manifest D:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\sts2-heybox-support\\mod_mainfest.json is missing the 'id' field! This is not allowed. The mod will not be loaded.\r\n" +
            "[ERROR] Mod manifest D:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\STS2-RitsuLib\\ritsulib-variants.json is missing the 'id' field! This is not allowed. The mod will not be loaded.\r\n" +
            "[INFO] Found mod manifest file D:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\DamageMeter\\DamageMeter.json\r\n" +
            "[INFO] [Other] Completed startup");

        try
        {
            var result = RunPowerShell(script, "-Path", logPath);
            Assert.Equal(0, result.ExitCode);

            using var document = JsonDocument.Parse(result.Output);
            var logResult = GetSingleResult(document.RootElement);

            var clean = logResult.GetProperty("Clean").GetBoolean();
            var signatureHits = GetSignatureCounts(logResult);

            Assert.True(clean);
            Assert.Equal(0, signatureHits["RouteSuggest"]);
            Assert.Equal(0, signatureHits["DamageMeter"]);
            Assert.Equal(0, signatureHits["Godot ERROR line"]);
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
    public void AuditFailsOnRealSpirePlusErrorLines()
    {
        var script = AssertRepoFileExists("scripts", "audit-godot-log.ps1");
        var workdir = Path.Combine(Path.GetTempPath(), "audit-godot-log-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workdir);

        var logPath = Path.Combine(workdir, "failure-godot.log");
        File.WriteAllText(
            logPath,
            "[ERROR] Mod manifest D:\\Steam\\steamapps\\common\\Slay the Spire 2\\mods\\RouteSuggest-v1.9.0\\RouteSuggestConfig.json is missing the 'id' field! This is not allowed. The mod will not be loaded.\r\n" +
            "[ERROR] Spire Plus error: could not initialize startup hook because type was null.\r\n" +
            "[ERROR] Another unrelated error occurred after launch.\r\n");

        try
        {
            var result = RunPowerShell(script, "-Path", logPath);
            Assert.Equal(0, result.ExitCode);

            using var document = JsonDocument.Parse(result.Output);
            var logResult = GetSingleResult(document.RootElement);

            var clean = logResult.GetProperty("Clean").GetBoolean();
            var signatureHits = GetSignatureCounts(logResult);

            Assert.False(clean);
            Assert.True(signatureHits["Spire Plus error/exception"] >= 1);
            Assert.True(signatureHits["Godot ERROR line"] >= 1);
        }
        finally
        {
            if (Directory.Exists(workdir))
            {
                Directory.Delete(workdir, recursive: true);
            }
        }
    }

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
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        Assert.True(process.WaitForExit(30_000), $"Timed out running {scriptPath}.");
        return (process.ExitCode, output, error);
    }

    private static JsonElement GetSingleResult(JsonElement root)
    {
        return root.ValueKind == JsonValueKind.Array ? root[0] : root;
    }

    private static Dictionary<string, int> GetSignatureCounts(JsonElement logResult)
    {
        var counts = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var entry in logResult.GetProperty("SignatureHits").EnumerateArray())
        {
            counts[entry.GetProperty("Name").GetString()!] = entry.GetProperty("Count").GetInt32();
        }

        return counts;
    }
}
