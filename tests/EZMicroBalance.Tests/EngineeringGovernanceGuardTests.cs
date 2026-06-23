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
        var config = ReadSourceTree("EZMicroBalanceCode", "Config");
        var debug = ReadRepoText("EZMicroBalanceCode", "Diagnostics", "SpirePlusDebug.cs");
        var previewLog = ReadRepoText("EZMicroBalanceCode", "Preview", "PreviewLog.cs");
        var sourceTree = ReadSourceTree("EZMicroBalanceCode");
        var testReadyGoal = ReadRepoText("docs", "test-ready-development-goal.md");
        var releaseChecklist = ReadRepoText("docs", "release-checklist.md");
        var englishSettings = JsonStringMap("EZMicroBalance", "localization", "eng", "settings_ui.json");
        var simplifiedChineseSettings = JsonStringMap("EZMicroBalance", "localization", "zhs", "settings_ui.json");
        var englishRitsuSettings = JsonStringMap("EZMicroBalance", "localization", "settings_ui", "eng.json");
        var simplifiedChineseRitsuSettings = JsonStringMap("EZMicroBalance", "localization", "settings_ui", "zhs.json");

        AssertSourceContains(
            config,
            "public bool ShowPreviewDebugLogs { get; set; }");
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
        Assert.Contains("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.title", englishRitsuSettings.Keys);
        Assert.Contains("SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.title", simplifiedChineseRitsuSettings.Keys);
        foreach (var key in new[]
                 {
                     "SPIREPLUS.settings_page.description",
                     "SPIREPLUS-MIGRATION_STATUS.title",
                     "SPIREPLUS-MIGRATION_STATUS.description",
                     "SPIREPLUS-RITSULIB_ONLY_SUMMARY.title",
                     "SPIREPLUS-RITSULIB_ONLY_SUMMARY.description",
                     "SPIREPLUS-REQUIRED_RUNTIME_DEPENDENCY.title",
                     "SPIREPLUS-REQUIRED_RUNTIME_DEPENDENCY.description",
                     "SPIREPLUS-STABLE_MANIFEST_ID.title",
                     "SPIREPLUS-STABLE_MANIFEST_ID.description",
                     "SPIREPLUS-PROOF_BOUNDARY.title",
                     "SPIREPLUS-PROOF_BOUNDARY.subtitle",
                     "SPIREPLUS-PROOF_BOUNDARY.description",
                     "SPIREPLUS-PREVIEW_TOOLS.title",
                     "SPIREPLUS-PREVIEW_TOOLS.description",
                     "SPIREPLUS-ENABLE_CRYSTAL_SPHERE_PEEK.description",
                     "SPIREPLUS-CRYSTAL_SPHERE_MASK_ALPHA.description",
                     "SPIREPLUS-ENABLE_TRANSFORM_PREDICTION.description",
                     "SPIREPLUS-TRANSFORM_PREDICTION_ALWAYS_ON.description",
                     "SPIREPLUS-SHOW_PREVIEW_DEBUG_LOGS.description"
                 })
        {
            Assert.Contains(key, englishSettings.Keys);
            Assert.Contains(key, simplifiedChineseSettings.Keys);
            Assert.Contains(key, englishRitsuSettings.Keys);
            Assert.Contains(key, simplifiedChineseRitsuSettings.Keys);
        }

        foreach (var key in new[]
                 {
                     "EZMICROBALANCE.settings_page.description",
                     "EZMICROBALANCE-MIGRATION_STATUS.title",
                     "EZMICROBALANCE-MIGRATION_STATUS.description",
                     "EZMICROBALANCE-RITSULIB_ONLY_SUMMARY.title",
                     "EZMICROBALANCE-RITSULIB_ONLY_SUMMARY.description",
                     "EZMICROBALANCE-REQUIRED_RUNTIME_DEPENDENCY.title",
                     "EZMICROBALANCE-REQUIRED_RUNTIME_DEPENDENCY.description",
                     "EZMICROBALANCE-STABLE_MANIFEST_ID.title",
                     "EZMICROBALANCE-STABLE_MANIFEST_ID.description",
                     "EZMICROBALANCE-PROOF_BOUNDARY.title",
                     "EZMICROBALANCE-PROOF_BOUNDARY.subtitle",
                     "EZMICROBALANCE-PROOF_BOUNDARY.description",
                     "EZMICROBALANCE-PREVIEW_TOOLS.title",
                     "EZMICROBALANCE-PREVIEW_TOOLS.description",
                     "EZMICROBALANCE-ENABLE_CRYSTAL_SPHERE_PEEK.description",
                     "EZMICROBALANCE-CRYSTAL_SPHERE_MASK_ALPHA.description",
                     "EZMICROBALANCE-ENABLE_TRANSFORM_PREDICTION.description",
                     "EZMICROBALANCE-TRANSFORM_PREDICTION_ALWAYS_ON.description",
                     "EZMICROBALANCE-SHOW_PREVIEW_DEBUG_LOGS.description"
                 })
        {
            Assert.Contains(key, englishSettings.Keys);
            Assert.Contains(key, simplifiedChineseSettings.Keys);
            Assert.Contains(key, englishRitsuSettings.Keys);
            Assert.Contains(key, simplifiedChineseRitsuSettings.Keys);
        }

        Assert.DoesNotContain("SPIREPLUS-ENABLE_DEBUG_LOGS.title", englishSettings.Keys);
        Assert.DoesNotContain("SPIREPLUS-ENABLE_DEBUG_LOGS.title", simplifiedChineseSettings.Keys);
        Assert.DoesNotContain("SPIREPLUS-ENABLE_DEBUG_LOGS.title", englishRitsuSettings.Keys);
        Assert.DoesNotContain("SPIREPLUS-ENABLE_DEBUG_LOGS.title", simplifiedChineseRitsuSettings.Keys);
        Assert.DoesNotContain("EZMICROBALANCE-ENABLE_DEBUG_LOGS.title", englishSettings.Keys);
        Assert.DoesNotContain("EZMICROBALANCE-ENABLE_DEBUG_LOGS.title", simplifiedChineseSettings.Keys);
        Assert.DoesNotContain("EZMICROBALANCE-ENABLE_DEBUG_LOGS.title", englishRitsuSettings.Keys);
        Assert.DoesNotContain("EZMICROBALANCE-ENABLE_DEBUG_LOGS.title", simplifiedChineseRitsuSettings.Keys);

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
        var migratedPatchCount = Directory
            .GetFiles(
                RepoPath("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib"),
                "SpirePlusMigratedPatchRegistry*.cs",
                SearchOption.TopDirectoryOnly)
            .Sum(path => Regex.Matches(File.ReadAllText(path), @"\.RegisterPatch<").Count);

        Assert.Contains($"| Total raw HarmonyPatch declarations | {sourcePatchCount} |", inventory, StringComparison.Ordinal);
        Assert.Contains($"| Migrated to RitsuLib ModPatcher | {migratedPatchCount} |", inventory, StringComparison.Ordinal);
        Assert.Contains($"| Tracked patch units total | {sourcePatchCount + migratedPatchCount} |", inventory, StringComparison.Ordinal);
        Assert.Contains("| Unclassified owner | 0 |", inventory, StringComparison.Ordinal);
        Assert.DoesNotContain("$(", inventory, StringComparison.Ordinal);
        Assert.DoesNotContain("@{File=", inventory, StringComparison.Ordinal);
        AssertSourceContains(
            inventory,
            "| Owner | Risk | File | Line | Patch |",
            "Vakuu",
            "ascension-diagnostics",
            "Ascension patches",
            "CrystalSpherePeekPatch.cs",
            "TransformPreviewPatch.cs",
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
