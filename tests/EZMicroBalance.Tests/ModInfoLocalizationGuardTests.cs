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
            "Seedbed shows that style",
            "Planting removes a card from this combat before it reaches hand",
            "种下会在牌进入手牌前把它移出本场战斗",
            "根蚀被种下后只冻结本场结束结算",
            "[gold]作者[/gold]",
            "[gold]版本[/gold]",
            "GetNodeOrNull<MegaRichTextLabel>(\"ModDescription\")");

        AssertNoMojibake(patchSource);
        Assert.DoesNotContain("鏄", patchSource, StringComparison.Ordinal);
        Assert.DoesNotContain("鐗堟湰", patchSource, StringComparison.Ordinal);
        Assert.DoesNotContain("浣滆", patchSource, StringComparison.Ordinal);
    }

    [Fact]
    public void ManifestKeepsReadableFallbackDescription()
    {
        using var document = JsonDocument.Parse(ReadRepoText("EZMicroBalance.json"));
        var root = document.RootElement;

        Assert.Equal("EZMicroBalance", root.GetProperty("id").GetString());
        Assert.Equal("Spire Plus", root.GetProperty("name").GetString());
        Assert.Equal("v0.1.0-private-beta.29", root.GetProperty("version").GetString());
        Assert.True(root.TryGetProperty("description", out var description));

        var manifestDescription = description.GetString() ?? string.Empty;
        Assert.Contains("Spire Plus", manifestDescription, StringComparison.Ordinal);
        Assert.Contains("Planting removes the card from this combat before it reaches hand", manifestDescription, StringComparison.Ordinal);
        Assert.Contains("Rootblight freezes only this combat's end check", manifestDescription, StringComparison.Ordinal);
        Assert.Contains("no cleanse, downgrade, worsen, or split", manifestDescription, StringComparison.Ordinal);
        Assert.Contains("The Mod Settings panel localizes this description by client language.", manifestDescription, StringComparison.Ordinal);
        Assert.DoesNotContain("鏄", manifestDescription, StringComparison.Ordinal);
        Assert.DoesNotContain("鐗堟湰", manifestDescription, StringComparison.Ordinal);
        AssertNoMojibake(manifestDescription);
        Assert.False(root.TryGetProperty("description_zhs", out _), "The game manifest schema does not read description_zhs; use the UI patch instead.");
    }
}
