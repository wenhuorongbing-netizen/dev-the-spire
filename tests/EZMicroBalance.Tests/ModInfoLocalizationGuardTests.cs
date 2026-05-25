using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class ModInfoLocalizationGuardTests
{
    [Fact]
    public void GameManifestDescriptionIsStaticAndSpirePlusLocalizesTheModInfoPanel()
    {
        var gameManifestSource = ReadRepoText("source code", "src", "Core", "Modding", "ModManifest.cs");
        var gameModInfoSource = ReadRepoText("source code", "src", "Core", "Nodes", "Screens", "ModdingScreen", "NModInfoContainer.cs");
        var patchSource = ReadRepoText("EZMicroBalanceCode", "Modding", "ModInfoLocalizationPatches.cs");

        AssertSourceContains(
            gameManifestSource,
            "[JsonPropertyName(\"description\")]",
            "public string? description;");
        AssertSourceContains(
            gameModInfoSource,
            "handler.AppendFormatted(mod.manifest?.description ?? \"No description\");");
        AssertSourceContains(
            patchSource,
            "HarmonyPatch(typeof(NModInfoContainer), nameof(NModInfoContainer.Fill))",
            "mod.manifest?.id, MainFile.ModId",
            "LocManager.Instance?.Language",
            "string.Equals(language, \"zhs\", StringComparison.Ordinal)",
            "Spire Plus 是一个用于私测的《杀戮尖塔 2》单体玩法扩展",
            "Spire Plus is a single Slay the Spire 2 gameplay expansion",
            "Seedbed is the clearest example",
            "种下不是消耗诅咒",
            "根蚀被种下后只在本战冻结",
            "GetNodeOrNull<MegaRichTextLabel>(\"ModDescription\")");
    }

    [Fact]
    public void ManifestKeepsAPlainEnglishFallbackDescription()
    {
        using var document = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        var root = document.RootElement;

        Assert.Equal("EZMicroBalance", root.GetProperty("id").GetString());
        Assert.Equal("Spire Plus", root.GetProperty("name").GetString());
        Assert.Equal("v0.1.0-private-beta.18", root.GetProperty("version").GetString());
        Assert.True(root.TryGetProperty("description", out var description));
        Assert.Contains("Spire Plus", description.GetString(), StringComparison.Ordinal);
        Assert.Contains("中文", description.GetString(), StringComparison.Ordinal);
        Assert.False(root.TryGetProperty("description_zhs", out _), "The game manifest schema does not read description_zhs; use the UI patch instead.");
    }
}
