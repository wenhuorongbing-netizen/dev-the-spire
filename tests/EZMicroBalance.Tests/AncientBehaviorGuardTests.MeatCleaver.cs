using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AncientBehaviorGuardTests
{
    [Fact]
    public void MeatCleaverCookRestSiteOptionIsSafeAndScoped()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Ancients", "Patches", "MeatCleaverCookPatches.cs");
        var relics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var restSite = JsonStringMap("EZMicroBalance", "localization", "eng", "rest_site_ui.json");
        var staticHovers = JsonStringMap("EZMicroBalance", "localization", "eng", "static_hover_tips.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var zhsRestSite = JsonStringMap("EZMicroBalance", "localization", "zhs", "rest_site_ui.json");
        var zhsStaticHovers = JsonStringMap("EZMicroBalance", "localization", "zhs", "static_hover_tips.json");
        var manualChecklist = ReadRepoText("docs", "features", "ancients-rework-v4", "manual-test-checklist.md");

        Assert.Equal("Adds a [gold]Cleaver[/gold] option to rest sites: remove [blue]2[/blue] cards and lose [blue]5[/blue] HP.", relics["MEAT_CLEAVER.description"]);
        Assert.Equal("Cleaver", restSite["OPTION_COOK.name"]);
        Assert.Equal("Remove 2 cards. Lose 5 HP.", restSite["OPTION_COOK.ezDescription"]);
        Assert.Equal("Requires at least 2 removable cards and more than 5 HP.", restSite["OPTION_COOK.ezDescriptionDisabled"]);
        Assert.Equal("At a [gold]Rest Site[/gold], [gold]remove[/gold] [blue]2[/blue] cards from your [gold]Deck[/gold] and lose [blue]5[/blue] HP.", staticHovers["COOK.description"]);
        Assert.Equal("Cleaver", staticHovers["COOK.title"]);
        Assert.Equal("\u5728\u4f11\u606f\u5904\u52a0\u5165[gold]\u5207\u8089[/gold]\u9009\u9879\uff1a\u79fb\u9664[blue]2[/blue]\u5f20\u724c\u5e76\u5931\u53bb[blue]5[/blue]\u70b9\u751f\u547d\u3002", zhsRelics["MEAT_CLEAVER.description"]);
        Assert.Equal("\u5207\u8089", zhsRestSite["OPTION_COOK.name"]);
        Assert.Equal("\u79fb\u96642\u5f20\u724c\u3002\u5931\u53bb5\u70b9\u751f\u547d\u3002", zhsRestSite["OPTION_COOK.ezDescription"]);
        Assert.Equal("\u9700\u8981\u81f3\u5c112\u5f20\u53ef\u79fb\u9664\u724c\u4e14\u751f\u547d\u503c\u5927\u4e8e5\u3002", zhsRestSite["OPTION_COOK.ezDescriptionDisabled"]);
        Assert.Equal("\u5728[gold]\u4f11\u606f\u5904[/gold]\u4ece\u4f60\u7684[gold]\u724c\u7ec4[/gold]\u4e2d[gold]\u79fb\u9664[/gold][blue]2[/blue]\u5f20\u724c\uff0c\u5e76\u5931\u53bb[blue]5[/blue]\u70b9\u751f\u547d\u3002", zhsStaticHovers["COOK.description"]);
        Assert.Equal("\u5207\u8089", zhsStaticHovers["COOK.title"]);
        Assert.DoesNotContain("gain [green]9[/green] Max HP", staticHovers["COOK.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\u83b7\u5f97[green]9[/green]\u70b9\u6700\u5927\u751f\u547d", zhsStaticHovers["COOK.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Cooking", relics["MEAT_CLEAVER.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsRelics["MEAT_CLEAVER.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsRestSite["OPTION_COOK.name"], StringComparison.Ordinal);
        Assert.DoesNotContain("\u95bb\u621d\u7d8a\u9288", zhsStaticHovers["COOK.title"], StringComparison.Ordinal);

        AssertSourceContains(
            source,
            "public const int CardsToRemove = 2",
            "public const int HpToLose = 5",
            "class MeatCleaverCookIsEnabledPatch : IPatchMethod",
            "class MeatCleaverCookDescriptionPatch : IPatchMethod",
            "class MeatCleaverCookPatch : IPatchMethod",
            "\"meat-cleaver-cook-is-enabled\"",
            "\"meat-cleaver-cook-description\"",
            "\"meat-cleaver-cook-on-select\"",
            "new ModPatchTarget(typeof(CookRestSiteOption), \"IsEnabled\", MethodType.Getter)",
            "new ModPatchTarget(typeof(CookRestSiteOption), \"Description\", MethodType.Getter)",
            "new ModPatchTarget(typeof(CookRestSiteOption), nameof(CookRestSiteOption.OnSelect))",
            "owner.GetRelic<MeatCleaver>() != null && !MeatCleaverCookPatch.CanCook(owner)",
            "__result = false",
            "if (owner.GetRelic<MeatCleaver>() == null)",
            "__instance.IsEnabled ? \"OPTION_COOK.ezDescription\" : \"OPTION_COOK.ezDescriptionDisabled\"",
            "owner.Creature.CurrentHp > HpToLose",
            "Cards.Count(card => card.IsRemovable) >= CardsToRemove",
            "Cancelable = true",
            "RequireManualConfirmation = true",
            "if (cards.Count != CardsToRemove)",
            "await CardPileCmd.RemoveFromDeck(card)",
            "await CreatureCmd.SetCurrentHp(owner.Creature, owner.Creature.CurrentHp - HpToLose)");

        Assert.DoesNotContain("[HarmonyPatch", source, StringComparison.Ordinal);
        Assert.DoesNotContain("MaxHp", source, StringComparison.Ordinal);
        Assert.Contains("Verify option disabled when too few removable cards.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Verify option disabled when HP is not greater than 5.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Expected: Cleaver / \u5207\u8089 option removes 2 removable cards and costs 5 HP.", manualChecklist, StringComparison.Ordinal);
        Assert.Contains("Verify no other rest-site source is affected unexpectedly.", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Expected: Cook option", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("\u70f9\u996a", manualChecklist, StringComparison.Ordinal);
    }
}
