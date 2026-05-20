using System.Text;
using System.Text.Json;
using Xunit;

namespace EZFuturePeek.Tests;

public sealed class FuturePeekGuardTests
{
    [Fact]
    public void ManifestStaysIndependentPreviewOnlyAndResourceBacked()
    {
        using var document = JsonDocument.Parse(ReadRepoText("EZFuturePeek.json"));
        var root = document.RootElement;

        Assert.Equal("EZFuturePeek", root.GetProperty("id").GetString());
        Assert.Equal("Future Peek", root.GetProperty("name").GetString());
        Assert.Contains("without changing run state, rewards, cards, or RNG", root.GetProperty("description").GetString(), StringComparison.Ordinal);
        Assert.False(root.GetProperty("affects_gameplay").GetBoolean());
        Assert.True(root.GetProperty("has_pck").GetBoolean());
        Assert.True(root.GetProperty("has_dll").GetBoolean());

        var dependencies = root.GetProperty("dependencies").EnumerateArray().ToArray();
        Assert.Contains(dependencies, dependency =>
            dependency.GetProperty("id").GetString() == "BaseLib" &&
            dependency.GetProperty("min_version").GetString() == "v3.1.2");
    }

    [Fact]
    public void FuturePeekDoesNotModifySpirePlusRuntimeFolders()
    {
        var activeRuntimeFiles = Directory
            .GetFiles(RepoPath("EZMicroBalanceCode"), "*", SearchOption.AllDirectories)
            .Concat(Directory.GetFiles(RepoPath("EZMicroBalance"), "*", SearchOption.AllDirectories))
            .Concat(
            [
                RepoPath("EZMicroBalance.csproj"),
                RepoPath("EZMicroBalance.json")
            ])
            .Where(File.Exists)
            .Where(path => Path.GetExtension(path) is ".cs" or ".csproj" or ".json" or ".tscn" or ".tres" or ".gd" or ".cfg")
            .ToArray();

        var forbiddenFragments = new[]
        {
            "EZFuturePeek",
            "Future Peek",
            "FuturePeek",
            "NCrystalSphere",
            "ScryMask",
            "NTransformPreview",
            "CycleThroughCards"
        };

        foreach (var path in activeRuntimeFiles)
        {
            var text = File.ReadAllText(path, Encoding.UTF8);
            foreach (var fragment in forbiddenFragments)
            {
                Assert.DoesNotContain(fragment, text, StringComparison.Ordinal);
            }
        }
    }

    [Fact]
    public void CrystalSpherePatchOnlyTouchesTheMaskAndButton()
    {
        var source = ReadRepoText("EZFuturePeekCode", "Patches", "CrystalSpherePeekPatch.cs");

        Assert.Contains("NCrystalSphereScreen", source, StringComparison.Ordinal);
        Assert.Contains("%ScryMask", source, StringComparison.Ordinal);
        Assert.Contains("GetPeekButtonText()", source, StringComparison.Ordinal);
        Assert.Contains("Modulate", source, StringComparison.Ordinal);
        Assert.Contains("ToggleMode = true", source, StringComparison.Ordinal);
        Assert.Contains("\"预知\"", source, StringComparison.Ordinal);
        Assert.Contains("OnMinigameFinished", source, StringComparison.Ordinal);
        Assert.Contains("HideForFinishedScreen", source, StringComparison.Ordinal);
        Assert.Contains("OriginalMaskAlpha", source, StringComparison.Ordinal);

        Assert.DoesNotContain("ClearCell", source, StringComparison.Ordinal);
        Assert.DoesNotContain("RevealItem", source, StringComparison.Ordinal);
        Assert.DoesNotContain("CellClicked", source, StringComparison.Ordinal);
        Assert.DoesNotContain("AddReward", source, StringComparison.Ordinal);
        Assert.DoesNotContain("IsHidden = false", source, StringComparison.Ordinal);
    }

    [Fact]
    public void TransformPredictionDoesNotCreateRealCardsOrAdvanceRealRng()
    {
        var patchSource = ReadRepoText("EZFuturePeekCode", "Patches", "TransformPreviewPatch.cs");
        var predictionSource = ReadRepoText("EZFuturePeekCode", "Prediction", "TransformPredictionService.cs");
        var combined = patchSource + Environment.NewLine + predictionSource;

        Assert.Contains("TransformPredictionRngContext.TryConsume", patchSource, StringComparison.Ordinal);
        Assert.Contains("no verified transform RNG source", patchSource, StringComparison.Ordinal);
        Assert.Contains("pendingPredictions.Count == 0", patchSource, StringComparison.Ordinal);
        Assert.Contains("return true;", patchSource, StringComparison.Ordinal);
        Assert.Contains("holder.ReassignToCard", patchSource, StringComparison.Ordinal);
        Assert.Contains("CardFactory.GetDefaultTransformationOptions", predictionSource, StringComparison.Ordinal);
        Assert.Contains("rng.NextItem(optionArray)", predictionSource, StringComparison.Ordinal);
        Assert.Contains("predicted.ToMutable()", predictionSource, StringComparison.Ordinal);
        Assert.Contains("preview.UpgradeInternal()", predictionSource, StringComparison.Ordinal);
        Assert.Contains("preview.FinalizeUpgradeInternal()", predictionSource, StringComparison.Ordinal);

        Assert.DoesNotContain("GetReplacement(", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("CreateRandomCardForTransform", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("RunState.CreateCard", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("CombatState.CreateCard", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("Original.Owner.PlayerRng.Transformations", patchSource, StringComparison.Ordinal);
        Assert.DoesNotContain("realRng.Next", patchSource, StringComparison.Ordinal);
        Assert.DoesNotContain("realRng.FastForward", patchSource, StringComparison.Ordinal);
    }

    [Fact]
    public void TransformPredictionOnlyUsesSourceBackedRngContexts()
    {
        var contextSource = ReadRepoText("EZFuturePeekCode", "Patches", "TransformPredictionRngContext.cs");

        foreach (var eventRngSource in new[]
        {
            "AromaOfChaos",
            "EndlessConveyor",
            "Symbiote",
            "WhisperingHollow"
        })
        {
            Assert.Contains(eventRngSource, contextSource, StringComparison.Ordinal);
        }

        Assert.Contains("RegisterEventRng(__instance", contextSource, StringComparison.Ordinal);
        Assert.Contains("\"{sourceName}.Rng\"", contextSource, StringComparison.Ordinal);

        foreach (var nicheRngSource in new[]
        {
            "MorphicGrove",
            "Trial",
            "NewLeaf",
            "Astrolabe"
        })
        {
            Assert.Contains(nicheRngSource, contextSource, StringComparison.Ordinal);
        }

        Assert.Contains("RegisterNicheRng(__instance.Owner", contextSource, StringComparison.Ordinal);
        Assert.Contains("\"{sourceName}.RunState.Rng.Niche\"", contextSource, StringComparison.Ordinal);
        Assert.Contains("upgradeReplacementPreview: true", contextSource, StringComparison.Ordinal);

        Assert.Contains("new Rng(snapshot.Seed, snapshot.Counter)", contextSource, StringComparison.Ordinal);
        Assert.Contains("snapshot.Source.Counter != snapshot.Counter", contextSource, StringComparison.Ordinal);
        Assert.Contains("ConditionalWeakTable<Player, Snapshot>", contextSource, StringComparison.Ordinal);
        Assert.Contains("SnapshotsByPlayer.TryGetValue(player", contextSource, StringComparison.Ordinal);
        Assert.DoesNotContain("Dictionary<Player, Snapshot>", contextSource, StringComparison.Ordinal);
        Assert.DoesNotContain("SnapshotsByPlayer.Remove(player, out var snapshot)", contextSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionSelectionLifetimePatch", contextSource, StringComparison.Ordinal);
        Assert.Contains("ClearContextWhenSelectionCompletes", contextSource, StringComparison.Ordinal);
        Assert.Contains("TransformPredictionRngContext.Clear(player)", contextSource, StringComparison.Ordinal);
        Assert.DoesNotContain("NextItem", contextSource, StringComparison.Ordinal);
        Assert.DoesNotContain("FastForward", contextSource, StringComparison.Ordinal);
    }

    [Fact]
    public void ConfigLocalizationContainsReadableGeneratedRows()
    {
        var english = JsonStringMap("EZFuturePeek", "localization", "eng", "settings_ui.json");
        var simplifiedChinese = JsonStringMap("EZFuturePeek", "localization", "zhs", "settings_ui.json");
        var requiredKeys = new[]
        {
            "EZFUTUREPEEK.mod_title",
            "EZFUTUREPEEK-CRYSTAL_SPHERE_PEEK_BUTTON.title",
            "EZFUTUREPEEK-ENABLE_CRYSTAL_SPHERE_PEEK.title",
            "EZFUTUREPEEK-CRYSTAL_SPHERE_MASK_ALPHA.title",
            "EZFUTUREPEEK-ENABLE_TRANSFORM_PREDICTION.title",
            "EZFUTUREPEEK-TRANSFORM_PREDICTION_ALWAYS_ON.title",
            "EZFUTUREPEEK-SHOW_DEBUG_LOGS.title"
        };

        foreach (var key in requiredKeys)
        {
            Assert.True(english.TryGetValue(key, out var englishValue), $"Missing English key: {key}");
            Assert.True(simplifiedChinese.TryGetValue(key, out var zhsValue), $"Missing zhs key: {key}");
            Assert.False(string.IsNullOrWhiteSpace(englishValue), $"Empty English key: {key}");
            Assert.False(string.IsNullOrWhiteSpace(zhsValue), $"Empty zhs key: {key}");
        }

        Assert.Equal("预知未来", simplifiedChinese["EZFUTUREPEEK.mod_title"]);
        Assert.Equal("预知", simplifiedChinese["EZFUTUREPEEK-CRYSTAL_SPHERE_PEEK_BUTTON.title"]);
        Assert.Equal("水晶球预知按钮", simplifiedChinese["EZFUTUREPEEK-ENABLE_CRYSTAL_SPHERE_PEEK.title"]);
        Assert.Equal("水晶球雾层透明度", simplifiedChinese["EZFUTUREPEEK-CRYSTAL_SPHERE_MASK_ALPHA.title"]);

        var combinedZhs = string.Join('\n', simplifiedChinese.Values);
        foreach (var mojibake in new[] { "棰", "鍗", "鏄", "鑷", "璋", "€", "?" })
        {
            Assert.DoesNotContain(mojibake, combinedZhs, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ProjectUsesBaseLibHarmonyNoRitsuLibAndSeparatePublishPck()
    {
        var project = ReadRepoText("EZFuturePeek.csproj");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var exportScript = ReadRepoText("scripts", "export-future-peek.ps1");

        Assert.Contains("Alchyr.Sts2.BaseLib", project, StringComparison.Ordinal);
        Assert.Contains("0Harmony", project, StringComparison.Ordinal);
        Assert.DoesNotContain("RitsuLib", project, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("EZFuturePeekCode/**/*.cs", project, StringComparison.Ordinal);
        Assert.DoesNotContain("EZMicroBalanceCode/**/*.cs", project, StringComparison.Ordinal);

        Assert.Contains("scripts\\export-future-peek.ps1", project, StringComparison.Ordinal);
        Assert.Contains("Write-Utf8NoBom", exportScript, StringComparison.Ordinal);
        Assert.Contains("FuturePeekExport", exportScript, StringComparison.Ordinal);
        Assert.Contains("EZFuturePeek/localization/*/*.json", exportScript, StringComparison.Ordinal);
        Assert.Contains("res://EZFuturePeek/mod_icon.svg", exportScript, StringComparison.Ordinal);
        Assert.Contains("res://EZFuturePeek/localization/eng/settings_ui.json", exportScript, StringComparison.Ordinal);
        Assert.Contains("res://EZFuturePeek/localization/zhs/settings_ui.json", exportScript, StringComparison.Ordinal);
        Assert.DoesNotContain("res://EZFuturePeek", exportPreset, StringComparison.Ordinal);
        Assert.DoesNotContain("res://EZMicroBalance", exportScript, StringComparison.Ordinal);
    }

    [Fact]
    public void ReleaseSolutionBuildsFuturePeekProjectAsRelease()
    {
        var solution = ReadRepoText("EZFuturePeek.sln");
        const string projectGuid = "{ADDA3F08-E027-4229-B740-8826B5D4F818}";

        Assert.Contains($"{projectGuid}.Release|Any CPU.ActiveCfg = Release|Any CPU", solution, StringComparison.Ordinal);
        Assert.Contains($"{projectGuid}.Release|Any CPU.Build.0 = Release|Any CPU", solution, StringComparison.Ordinal);
        Assert.DoesNotContain($"{projectGuid}.Release|Any CPU.ActiveCfg = Debug|Any CPU", solution, StringComparison.Ordinal);
        Assert.DoesNotContain($"{projectGuid}.Release|Any CPU.Build.0 = Debug|Any CPU", solution, StringComparison.Ordinal);
    }

    private static SortedDictionary<string, string> JsonStringMap(params string[] parts)
    {
        using var document = JsonDocument.Parse(ReadRepoText(parts));
        return document.RootElement.EnumerateObject()
            .ToDictionary(property => property.Name, property => property.Value.GetString() ?? string.Empty, StringComparer.Ordinal)
            .ToSortedDictionary(StringComparer.Ordinal);
    }

    private static string ReadRepoText(params string[] parts)
    {
        return File.ReadAllText(RepoPath(parts), Encoding.UTF8);
    }

    private static string RepoPath(params string[] parts)
    {
        return Path.Combine(new[] { Root }.Concat(parts).ToArray());
    }

    private static string Root => LazyRoot.Value;

    private static readonly Lazy<string> LazyRoot = new(FindRepoRoot);

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "EZFuturePeek.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root from test output directory.");
    }
}

internal static class DictionaryExtensions
{
    public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>(
        this IDictionary<TKey, TValue> source,
        IComparer<TKey> comparer)
        where TKey : notnull
    {
        var result = new SortedDictionary<TKey, TValue>(comparer);
        foreach (var (key, value) in source)
        {
            result.Add(key, value);
        }

        return result;
    }
}
