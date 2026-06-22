using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    private static string ReadSereTalonVisualSource() =>
        string.Join(
            Environment.NewLine,
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualAssetPaths.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRelicModelRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualNodeRoutes.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualTextures.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualRouteLog.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualPatches.cs"),
            ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonVisualUiPatches.cs"));

    [Fact]
    public void VakuuSereTalonAndTanxClawsStayOnSeparateSourceRoutes()
    {
        var sereTalonPickupPatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "SereTalonPickupPatches.cs");
        var sereTalonVisualSource = ReadSereTalonVisualSource();
        var tanxClawsPatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "TanxClawsMaulTuningPatches.cs");
        var ancientPatchSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Patches");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        AssertSourceContains(
            sereTalonPickupPatch,
            "[HarmonyPatch(typeof(SereTalon), nameof(SereTalon.AfterObtained))]",
            "private const int CurseOfferCount = 4",
            "private const int CursePickCount = 1",
            "private const int NormalWishCount = 2",
            "private const int UpgradedWishCount = 1",
            "ModelDb.CardPool<CurseCardPool>()",
            "owner.RunState.Rng.Niche.NextItem(availableCurses)",
            "sereTalon.Flash()",
            "CardSelectCmd.FromSimpleGrid",
            "new BlockingPlayerChoiceContext()",
            "new LocString(\"relics\", \"SERE_TALON.selectionScreenPrompt\")",
            "Cancelable = false",
            "RequireManualConfirmation = true",
            "AncientCardHelpers.RemoveUnpiledRunCard(curse)",
            "owner.RunState.CreateCard<Wish>(owner)",
            "CardCmd.Upgrade(wish, CardPreviewStyle.None)",
            "SpirePlusFeedback.PreviewDeckAdds(successfulAdds, sereTalon, 2f)");
        Assert.DoesNotContain("Claws", sereTalonPickupPatch, StringComparison.Ordinal);
        Assert.DoesNotContain("Maul", sereTalonPickupPatch, StringComparison.Ordinal);

        AssertSourceContains(
            sereTalonVisualSource,
            "internal static class SereTalonVisualAssetPaths",
            "internal static class SereTalonVisualRelicModelRoutes",
            "internal static class SereTalonVisualNodeRoutes",
            "internal static class SereTalonVisualTextures",
            "internal static class SereTalonVisualRouteLog",
            "relic is not SereTalon",
            "IPatchMethod.PatchId => \"sere-talon-event-option-button-ready\"",
            "new ModPatchTarget(typeof(NEventOptionButton), nameof(NEventOptionButton._Ready))",
            "TryApplyEventOptionButton",
            "button.Option?.Relic is not SereTalon",
            "GetNodeOrNull<TextureRect>(\"%RelicIcon\")",
            "Ancient event option button",
            "IPatchMethod.PatchId => \"sere-talon-relic-node-reload\"",
            "new ModPatchTarget(typeof(NRelic), \"Reload\")",
            "TryApplyRelicNode",
            "IsNodeReady()",
            "InvalidOperationException",
            "NRelic small node",
            "NRelic large node",
            "RelicModel packed icon texture",
            "RelicModel big icon texture",
            "SereTalon uses Spire Plus art and Tanx Claws is untouched");
        AssertSourceContains(
            tanxClawsPatch,
            "[HarmonyPatch(typeof(Claws), nameof(Claws.AfterObtained))]",
            "owner.RunState.CreateCard<Maul>(owner)",
            "maul.UpgradeInternal()",
            "CardCmd.Upgrade(maul, CardPreviewStyle.None)");
        Assert.DoesNotContain("SereTalon", tanxClawsPatch, StringComparison.Ordinal);
        Assert.DoesNotContain("[HarmonyPatch(typeof(Maul)", ancientPatchSource, StringComparison.Ordinal);

        Assert.Equal("Vakuu's Sere Talon", engRelics["SERE_TALON.title"]);
        Assert.Equal("On pickup, choose [blue]1[/blue] of [blue]4[/blue] Curses. Add it, [blue]2[/blue] Wish, and [blue]1[/blue] Wish+ to your deck.", engRelics["SERE_TALON.description"]);
        Assert.Equal("Choose 1 Curse.", engRelics["SERE_TALON.selectionScreenPrompt"]);
        Assert.Equal("\u74e6\u5e93\u539f\u521d\u4e4b\u722a", zhsRelics["SERE_TALON.title"]);
        Assert.Equal("\u62fe\u53d6\u65f6\uff0c\u4ece[blue]4[/blue]\u5f20\u8bc5\u5492\u4e2d\u9009\u62e9[blue]1[/blue]\u5f20\u3002\u5c06\u5b83\u3001[blue]2[/blue]\u5f20[gold]\u8bb8\u613f[/gold]\u548c[blue]1[/blue]\u5f20[gold]\u8bb8\u613f+[/gold]\u52a0\u5165\u4f60\u7684\u724c\u7ec4\u3002", zhsRelics["SERE_TALON.description"]);
        Assert.Equal("\u9009\u62e91\u5f20\u8bc5\u5492\u3002", zhsRelics["SERE_TALON.selectionScreenPrompt"]);

        Assert.Equal("Tanx Claws", engRelics["CLAWS.title"]);
        Assert.Equal("On pickup, transform up to [blue]{Cards}[/blue] cards into upgraded Maul.", engRelics["CLAWS.description"]);
        Assert.Equal("\u5766\u514b\u65af\u5229\u722a", zhsRelics["CLAWS.title"]);
        Assert.Equal("\u62fe\u53d6\u65f6\uff0c\u5c06\u81f3\u591a[blue]{Cards}[/blue]\u5f20\u724c\u53d8\u5316\u4e3a\u6495\u54ac+\u3002", zhsRelics["CLAWS.description"]);
    }

    [LocalSourceFact]
    public void CoreVakuuSereTalonAndTanxClawsStayOnSeparateRoutes()
    {
        var vakuuSource = ReadLocalCoreText("Models", "Events", "Vakuu.cs");
        var tanxSource = ReadLocalCoreText("Models", "Events", "Tanx.cs");
        var sereTalonSource = ReadLocalCoreText("Models", "Relics", "SereTalon.cs");
        var clawsSource = ReadLocalCoreText("Models", "Relics", "Claws.cs");

        AssertSourceContains(
            vakuuSource,
            "RelicOption<SereTalon>()");
        Assert.DoesNotContain("RelicOption<Claws>()", vakuuSource, StringComparison.Ordinal);

        AssertSourceContains(
            tanxSource,
            "RelicOption<Claws>()");
        Assert.DoesNotContain("RelicOption<SereTalon>()", tanxSource, StringComparison.Ordinal);

        AssertSourceContains(
            sereTalonSource,
            "new DynamicVar(\"Curses\", 2m)",
            "new DynamicVar(\"Wishes\", 3m)",
            "HoverTipFactory.FromCardWithCardHoverTips<Wish>()",
            "CardPileCmd.Add(card, PileType.Deck)",
            "CardPileCmd.Add(card2, PileType.Deck)");
        Assert.DoesNotContain("Maul", sereTalonSource, StringComparison.Ordinal);

        AssertSourceContains(
            clawsSource,
            "new CardsVar(6)",
            "HoverTipFactory.FromCardWithCardHoverTips<Maul>()",
            "CreateMaulFromOriginal",
            "CardCmd.Transform(transformations, base.Owner.PlayerRng.Transformations)");
        Assert.DoesNotContain("Wish", clawsSource, StringComparison.Ordinal);
        Assert.DoesNotContain("CurseCardPool", clawsSource, StringComparison.Ordinal);
    }
}
