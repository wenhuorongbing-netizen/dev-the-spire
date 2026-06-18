using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientPlayerFacingPolishGuardTests
{
    [Fact]
    public void AscensionTextHighlightsRootblightBannerFiremarkAndBossTerms()
    {
        var engAscension = JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json");
        var zhsAscension = JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json");

        AssertSourceContains(
            engAscension["LEVEL_14.description"],
            "[gold]Rootblight I[/gold]",
            "[blue]4[/blue]",
            "[gold]Rootblights[/gold]");
        AssertSourceContains(
            engAscension["LEVEL_15.description"],
            "[gold]Blight Sprouts[/gold]",
            "[blue]3[/blue]",
            "[blue]4[/blue]");
        AssertSourceContains(
            engAscension["LEVEL_16.description"],
            "[gold]Banner Rooms[/gold]",
            "banner",
            "extra rewards");
        AssertSourceContains(
            engAscension["FIREMARK_ELITE.description"],
            "[gold]Firemarked Elite[/gold]");
        AssertSourceContains(
            engAscension["BOSS_DEDICATED_ABILITY.description"],
            "[gold]dedicated ability[/gold]");
        AssertSourceContains(
            engAscension["BOSS_BRANDED_FORM.description"],
            "[gold]Branded Form[/gold]");
        AssertSourceContains(
            engAscension["BOSS_SEAL_MARTYR_OATH.brand"],
            "[blue]2[/blue]",
            "+[blue]4[/blue]",
            "[gold]Artifact[/gold]");
        AssertSourceContains(
            engAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.summary"],
            "After [gold]Ebb[/gold]",
            "[blue]2[/blue] Time Sand",
            "[gold]Wither[/gold]");
        AssertSourceContains(
            engAscension["ROOTBLIGHT_ADDED"],
            "[gold]Rootblight[/gold]");
        AssertSourceContains(
            engAscension["ROOT_SYSTEM_FULL"],
            "max [blue]4[/blue]",
            "[gold]Rootblights[/gold]");

        AssertSourceContains(
            zhsAscension["LEVEL_14.description"],
            "[gold]",
            "[blue]4[/blue]");
        AssertSourceContains(
            zhsAscension["LEVEL_15.description"],
            "[blue]2[/blue]",
            "[blue]3[/blue]",
            "[blue]4[/blue]",
            "[gold]");
        AssertSourceContains(
            zhsAscension["LEVEL_16.description"],
            "[gold]",
            "额外奖励");
        AssertSourceContains(
            zhsAscension["FIREMARK_ELITE.description"],
            "[gold]");
        AssertSourceContains(
            zhsAscension["BOSS_DEDICATED_ABILITY.description"],
            "[gold]");
        AssertSourceContains(
            zhsAscension["BOSS_BRANDED_FORM.description"],
            "[gold]");
        AssertSourceContains(
            zhsAscension["BOSS_SEAL_MARTYR_OATH.brand"],
            "[blue]2[/blue]",
            "[blue]4[/blue]",
            "[gold]人工制品[/gold]");
        AssertSourceContains(
            zhsAscension["BOSS_SEAL_AEONGLASS_HOURGLASS.summary"],
            "[gold]消退[/gold]",
            "[blue]2[/blue]",
            "时砂",
            "[gold]枯萎[/gold]");
        Assert.Equal("Royal Decree", engAscension["BOSS_SEAL_CHOSEN_DECREE.title"]);
        Assert.Equal("御令", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.title"]);
        Assert.Contains("[gold]御令[/gold]", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("王令", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.summary"], StringComparison.Ordinal);
        Assert.DoesNotContain("择令", zhsAscension["BOSS_SEAL_CHOSEN_DECREE.title"], StringComparison.Ordinal);
        AssertSourceContains(
            zhsAscension["ROOTBLIGHT_ADDED"],
            "[gold]根蚀[/gold]");
        AssertSourceContains(
            zhsAscension["ROOT_SYSTEM_FULL"],
            "[blue]4[/blue]",
            "[gold]根蚀[/gold]");
    }

    [Fact]
    public void ForgeTokenTextDoesNotExposeTemporaryDevelopmentWording()
    {
        var forgeToken = ReadRepoText("EZMicroBalanceCode", "Ascension", "Relics", "ForgeTokenRelic.cs");

        AssertSourceContains(
            forgeToken,
            "Only [gold]Rest[/gold] or [gold]Smith[/gold] spends this token",
            "只有[gold]休息[/gold]或[gold]锻造[/gold]会消耗铸令");
        Assert.DoesNotContain("do not spend this yet", forgeToken, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("暂不消耗", forgeToken, StringComparison.Ordinal);
    }
}
