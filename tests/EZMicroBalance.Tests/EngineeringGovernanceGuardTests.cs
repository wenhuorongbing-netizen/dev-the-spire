using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class EngineeringGovernanceGuardTests
{
    [Fact]
    public void MainFileUsesFeatureRegistryForModuleBootstrap()
    {
        AssertRepoFileExists("EZMicroBalanceCode", "Core", "Features", "IFeatureModule.cs");
        AssertRepoFileExists("EZMicroBalanceCode", "Core", "Features", "FeatureGateResult.cs");
        AssertRepoFileExists("EZMicroBalanceCode", "Core", "Features", "FeatureRegistry.cs");
        AssertRepoFileExists("EZMicroBalanceCode", "Core", "Features", "FeatureRegistry.Environment.cs");
        AssertRepoFileExists("EZMicroBalanceCode", "Core", "Features", "SpirePlusFeatureRegistry.cs");
        AssertRepoFileExists("EZMicroBalanceCode", "Core", "Features", "FeatureOrders.cs");

        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");
        var registry = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "SpirePlusFeatureRegistry.cs");
        var featureRegistry = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "FeatureRegistry.cs");
        var featureOrders = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "FeatureOrders.cs");

        Assert.Contains("var registry = SpirePlusFeatureRegistry.CreateDefault();", mainFile, StringComparison.Ordinal);
        Assert.Contains("registry.InitializeAll();", mainFile, StringComparison.Ordinal);

        // MainFile should not directly call any feature initializer
        foreach (var directInitializerCall in new[]
                 {
                     "LothaInitializer.Initialize",
                     "MorviInitializer.Initialize",
                     "UrdaInitializer.Initialize",
                     "VakuuFightInitializer.Initialize",
                     "AscensionInitializer.Initialize"
                 })
        {
            Assert.DoesNotContain(directInitializerCall + "();", mainFile, StringComparison.Ordinal);
        }

        // Registry should register named feature modules
        AssertSourceContains(
            registry,
            "new LothaFeatureModule()",
            "new MorviFeatureModule()",
            "new UrdaFeatureModule()",
            "new VakuuFightFeatureModule()",
            "new AscensionFeatureModule()");

        // FeatureOrders should define named constants
        AssertSourceContains(
            featureOrders,
            "AncientsLotha",
            "AncientsMorvi",
            "AncientsUrda",
            "AncientsVakuuFight",
            "AscensionA11A20");

        // Feature module files should exist and delegate to initializers
        AssertRepoFileExists("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaFeatureModule.cs");
        AssertRepoFileExists("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviFeatureModule.cs");
        AssertRepoFileExists("EZMicroBalanceCode", "Ancients", "Expansion", "Urda", "UrdaFeatureModule.cs");
        AssertRepoFileExists("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureModule.cs");
        AssertRepoFileExists("EZMicroBalanceCode", "Ascension", "Core", "AscensionFeatureModule.cs");

        AssertSourceContains(
            featureRegistry,
            ".OrderBy(module => module.InitOrder)",
            ".ThenBy(module => module.Id, StringComparer.Ordinal)",
            "EvaluateGate()",
            "module.Initialize()",
            "bootstrap gate",
            "initialization failed",
            "throw;");
    }

    [Fact]
    public void DebugDiagnosticsStayScopedAndExplicitlyGated()
    {
        var config = ReadRepoText("EZMicroBalanceCode", "Config", "SpirePlusModConfig.cs");
        var debug = ReadRepoText("EZMicroBalanceCode", "Diagnostics", "SpirePlusDebug.cs");
        var previewLog = ReadRepoText("EZMicroBalanceCode", "Preview", "PreviewLog.cs");
        var sourceTree = ReadSourceTree("EZMicroBalanceCode");
        var testReadyGoal = ReadRepoText("docs", "test-ready-development-goal.md");
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var englishSettings = JsonStringMap("EZMicroBalance", "localization", "eng", "settings_ui.json");
        var simplifiedChineseSettings = JsonStringMap("EZMicroBalance", "localization", "zhs", "settings_ui.json");

        AssertSourceContains(
            config,
            "public static bool ShowPreviewDebugLogs { get; set; } = false;");
        Assert.DoesNotContain("EnableDebugLogs", config, StringComparison.Ordinal);
        Assert.DoesNotContain("SPIREPLUS_ENABLE_DEBUG_LOGS", config, StringComparison.Ordinal);
        Assert.DoesNotContain("EZMB_ENABLE_DEBUG_LOGS", config, StringComparison.Ordinal);

        AssertSourceContains(
            debug,
            "public const string DebugLogsEnvironmentVariable = \"SPIREPLUS_ENABLE_DEBUG_LOGS\";",
            "public const string LegacyDebugLogsEnvironmentVariable = \"EZMB_ENABLE_DEBUG_LOGS\";",
            "if (IsDebugLoggingEnabled)",
            "IsTruthy(Environment.GetEnvironmentVariable(name))",
            "var normalized = value?.Trim();",
            "!string.Equals(normalized, \"0\", StringComparison.OrdinalIgnoreCase)",
            "!string.Equals(normalized, \"false\", StringComparison.OrdinalIgnoreCase)",
            "!string.Equals(normalized, \"off\", StringComparison.OrdinalIgnoreCase)",
            "!string.Equals(normalized, \"no\", StringComparison.OrdinalIgnoreCase)",
            "MainFile.Logger.Info($\"[Spire Plus] [{category}] {message}\");",
            "public static void Warn(string category, string message)",
            "MainFile.Logger.Warn($\"[Spire Plus] [{category}] {message}\");");

        Assert.DoesNotContain("LogPreview(", debug, StringComparison.Ordinal);
        Assert.DoesNotContain("SpirePlusDebug.LogPreview", sourceTree, StringComparison.Ordinal);

        AssertSourceContains(
            previewLog,
            "if (SpirePlusModConfig.ShowPreviewDebugLogs)",
            "MainFile.Logger.Info(\"[Spire Plus] Preview: \" + message);",
            "MainFile.Logger.Warn(\"[Spire Plus] Preview: \" + message);");

        Assert.Contains("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.title", englishSettings.Keys);
        Assert.Contains("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.title", simplifiedChineseSettings.Keys);
        Assert.DoesNotContain("SPIREPLUS-ENABLE_DEBUG_LOGS.title", englishSettings.Keys);
        Assert.DoesNotContain("SPIREPLUS-ENABLE_DEBUG_LOGS.title", simplifiedChineseSettings.Keys);
        Assert.DoesNotContain("EZMICROBALANCE-ENABLE_DEBUG_LOGS.title", englishSettings.Keys);
        Assert.DoesNotContain("EZMICROBALANCE-ENABLE_DEBUG_LOGS.title", simplifiedChineseSettings.Keys);

        AssertSourceContains(
            testReadyGoal,
            "Internal broad diagnostics are not a player-facing mod setting.",
            "`SPIREPLUS_ENABLE_DEBUG_LOGS=1`",
            "`EZMB_ENABLE_DEBUG_LOGS=1`",
            "preview-tool diagnostics remain the localized `ShowPreviewDebugLogs` setting");
        AssertSourceContains(
            releaseChecklist,
            "Debug probes are removed from active behavior or gated behind an explicit debug flag",
            "broad info diagnostics require `SPIREPLUS_ENABLE_DEBUG_LOGS=1`",
            "legacy `EZMB_ENABLE_DEBUG_LOGS=1`",
            "preview diagnostics use the localized preview diagnostics setting");
    }

    [Fact]
    public void PatchInventoryIsGeneratedReadableAndClassified()
    {
        var inventory = ReadRepoText("docs", "patch-inventory.md");
        var sourcePatchCount = Directory
            .GetFiles(RepoPath("EZMicroBalanceCode"), "*.cs", SearchOption.AllDirectories)
            .Sum(path => Regex.Matches(File.ReadAllText(path), @"\[HarmonyPatch").Count);

        Assert.Contains($"| Total raw HarmonyPatch declarations | {sourcePatchCount} |", inventory, StringComparison.Ordinal);
        Assert.Contains($"| Tracked patch units total | {sourcePatchCount + 25} |", inventory, StringComparison.Ordinal);
        Assert.Contains("| Unclassified owner | 0 |", inventory, StringComparison.Ordinal);
        Assert.DoesNotContain("$(", inventory, StringComparison.Ordinal);
        Assert.DoesNotContain("@{File=", inventory, StringComparison.Ordinal);
        AssertSourceContains(
            inventory,
            "| Owner | Risk | File | Line | Patch |",
            "Vakuu",
            "Ascension core",
            "Ascension patches",
            "Preview tools",
            "High: run, room, save, lobby, multiplayer, or game lifecycle surface.");
    }

    [Fact]
    public void WorktreeBatchScriptRunsAndWritesBatchPathspecs()
    {
        var script = AssertRepoFileExists("scripts", "report-worktree-batches.ps1");
        var outputDirectory = RepoPath(".tools", "test-worktree-batches", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(outputDirectory);

        try
        {
            var result = RunPowerShell(
                script,
                "-Format",
                "Json",
                "-FailOnUnclassified",
                "-PathspecDirectory",
                outputDirectory);

            Assert.True(result.ExitCode == 0, $"report-worktree-batches.ps1 failed:{Environment.NewLine}{result.Output}{result.Error}");

            using var document = JsonDocument.Parse(result.Output);
            var root = document.RootElement;
            Assert.Equal("git status --short", root.GetProperty("Command").GetString());

            var totalDirtyEntries = root.GetProperty("TotalDirtyEntries").GetInt32();
            var summary = root.GetProperty("Summary").EnumerateArray().ToArray();
            var suggestedCommands = root.GetProperty("SuggestedGitAddCommands").EnumerateArray().ToArray();
            Assert.Contains(summary, row => row.GetProperty("Batch").GetInt32() == -1 && row.GetProperty("Count").GetInt32() == 0);
            Assert.Equal(9, suggestedCommands.Length);
            Assert.All(
                suggestedCommands,
                row => Assert.Contains("git add --pathspec-from-file=", row.GetProperty("GitAddCommand").GetString(), StringComparison.Ordinal));

            var pathspecLineTotal = 0;
            for (var batch = 0; batch <= 8; batch++)
            {
                var pathspecPath = Path.Combine(outputDirectory, $"batch-{batch}.pathspec");
                Assert.True(File.Exists(pathspecPath), $"Missing pathspec for batch {batch}.");
                pathspecLineTotal += File.ReadAllLines(pathspecPath).Length;
            }

            Assert.Equal(totalDirtyEntries, pathspecLineTotal);
            Assert.True(File.Exists(Path.Combine(outputDirectory, "manifest.json")));
        }
        finally
        {
            if (Directory.Exists(outputDirectory))
            {
                Directory.Delete(outputDirectory, recursive: true);
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

}
