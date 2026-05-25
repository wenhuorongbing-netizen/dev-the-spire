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
            "Spire Plus 是用于私测的《杀戮尖塔 2》单体玩法扩展",
            "Spire Plus is a single Slay the Spire 2 gameplay expansion",
            "Seedbed shows that rule clearly",
            "Planting means combat-only isolation",
            "种下就是“本战隔离”",
            "根蚀被种下后只冻结这一场",
            "[gold]作者[/gold]",
            "[gold]版本[/gold]",
            "GetNodeOrNull<MegaRichTextLabel>(\"ModDescription\")");

        foreach (var mojibake in new[] { "鏄", "銆", "涓", "绉", "鐗", "浣滆" })
        {
            Assert.DoesNotContain(mojibake, patchSource, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ManifestKeepsAPlainEnglishFallbackDescription()
    {
        using var document = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        var root = document.RootElement;

        Assert.Equal("EZMicroBalance", root.GetProperty("id").GetString());
        Assert.Equal("Spire Plus", root.GetProperty("name").GetString());
        Assert.Equal("v0.1.0-private-beta.21", root.GetProperty("version").GetString());
        Assert.True(root.TryGetProperty("description", out var description));
        Assert.Contains("Spire Plus", description.GetString(), StringComparison.Ordinal);
        Assert.Contains("中文", description.GetString(), StringComparison.Ordinal);
        Assert.Contains("Planting is combat-only isolation", description.GetString(), StringComparison.Ordinal);
        Assert.Contains("种下=本战隔离", description.GetString(), StringComparison.Ordinal);
        Assert.Contains("Rootblight is frozen", description.GetString(), StringComparison.Ordinal);
        Assert.Contains("根蚀只冻结这一场", description.GetString(), StringComparison.Ordinal);
        Assert.DoesNotContain("涓", description.GetString(), StringComparison.Ordinal);
        Assert.DoesNotContain("銆", description.GetString(), StringComparison.Ordinal);
        Assert.False(root.TryGetProperty("description_zhs", out _), "The game manifest schema does not read description_zhs; use the UI patch instead.");
    }
}
