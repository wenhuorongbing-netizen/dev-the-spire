using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class AscensionFeatureGuardTests
{
    [Fact]
    public void FiremarkTokenAndFissionPlayerFacingSurfacesAreGuarded()
    {
        var mapPatch = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Patches");
        var forgeToken = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var forgeRelic = ReadRepoText("EZMicroBalanceCode", "Ascension", "Relics", "ForgeTokenRelic.cs");
        var firemarkPowers = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Powers");
        var fission = ReadRepoText("EZMicroBalanceCode", "Ascension", "Enchantments", "FissionEnchantment.cs");
        var rewardService = ReadSourceTree("EZMicroBalanceCode", "Ascension", "Rewards");
        var manualChecklist = ReadRepoText("docs", "features", "ascension-11-20", "manual-test-checklist.md");
        var engAscension = JsonStringMap("EZMicroBalance", "localization", "eng", "ascension.json");
        var zhsAscension = JsonStringMap("EZMicroBalance", "localization", "zhs", "ascension.json");

        AssertSourceContains(
            mapPatch,
            "NNormalMapPoint",
            "\"_questIcon\"",
            "FiremarkedEliteMapQuestMarker",
            "AscensionAssetPaths.GetFiremarkIndicator(firemark)",
            "AscensionAssetPaths.GetBannerIndicator(banner)",
            "AscensionAssetPaths.FiremarkedEliteIndicator");

        AssertSourceContains(
            forgeToken,
            "await RelicCmd.Obtain<ForgeTokenRelic>(player)",
            "await RelicCmd.Remove(token)",
            "return targets.FirstOrDefault()",
            "SpecialRestSiteActionPayoutEnabled = false",
            "ModifyExtraRestSiteHealText");
        Assert.DoesNotContain("player.RunState.Rng.Niche.NextItem(targets)", forgeToken, StringComparison.Ordinal);
        Assert.DoesNotContain("RestSiteSynchronizer", forgeToken, StringComparison.Ordinal);
        Assert.DoesNotContain("ApplyAfterSpecialRestSiteAction", forgeToken, StringComparison.Ordinal);

        AssertSourceContains(
            forgeRelic,
            "public override RelicRarity Rarity => RelicRarity.Event",
            "public override bool ShowCounter => true",
            "public override int DisplayAmount => 1",
            "additionalRestSiteHealText",
            "[gold]Smith[/gold] heals [blue]7[/blue] HP");

        AssertSourceContains(
            firemarkPowers,
            "MightMarkFiremarkPower",
            "AscensionAssetPaths.FiremarkMightIndicator",
            "GiantMarkFiremarkPower",
            "AscensionAssetPaths.FiremarkGiantIndicator",
            "ForgeArmorMarkFiremarkPower",
            "AscensionAssetPaths.FiremarkForgeArmorIndicator",
            "ConstantHealMarkFiremarkPower",
            "AscensionAssetPaths.FiremarkConstantHealIndicator",
            "FiremarkMightOverflowPower",
            "Overflow: Might",
            "protected override IEnumerable<DynamicVar> CanonicalVars => [new InterruptDamageDynamicVar()]",
            "description.Add(InterruptDamageVar, InterruptDamage)",
            "private int InterruptDamage => Amount switch",
            "Taking [blue]{InterruptDamage}[/blue] damage interrupts its next heal.",
            "internal abstract class FiremarkPower",
            "public override PowerStackType StackType => PowerStackType.Counter",
            "public override int DisplayAmount => Amount",
            "Firemark: Might",
            "Firemark: Constant Heal");

        AssertSourceContains(
            fission,
            "CustomIconPath => AscensionAssetPaths.FissionEnchantmentIcon",
            "这张牌的[gold]耗能[/gold]降低[blue]1[/blue]。打出后进入[gold]消耗[/gold]牌堆，并正常触发[gold]消耗[/gold]效果。",
            "[gold]耗能[/gold]降低[blue]1[/blue]。正常触发[gold]消耗[/gold]效果。",
            "This card costs [blue]1[/blue] less. After play, it enters the [gold]Exhaust[/gold] pile and triggers [gold]Exhaust[/gold] effects normally.",
            "Costs [blue]1[/blue] less. Triggers [gold]Exhaust[/gold] effects normally.",
            "HoverTipFactory.FromKeyword(CardKeyword.Exhaust)");
        Assert.DoesNotContain("energyPrefix:energyIcons", fission, StringComparison.Ordinal);
        Assert.DoesNotContain("\"[gold]能量[/gold]费用降低[blue]1[/blue]。\"", fission, StringComparison.Ordinal);

        AssertSourceContains(
            rewardService,
            "ModelDb.Enchantment<FissionEnchantment>().CanEnchant(card)",
            "card.Type is CardType.Attack or CardType.Skill",
            "!card.EnergyCost.CostsX",
            "!card.HasStarCostX",
            "card.CurrentStarCost <= 0",
            "!card.Keywords.Contains(CardKeyword.Exhaust)",
            "!card.ExhaustOnNextPlay",
            "card.Enchantment == null");

        Assert.StartsWith("火印精英", zhsAscension["LEVEL_12.title"], StringComparison.Ordinal);
        Assert.Contains("[gold]Firemarked Elites[/gold] appear on the map", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]Forge Token[/gold]", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Act 1 has", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Acts 2 and 3", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("Firemark Host", engAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.Contains("地图上会出现[gold]火印精英[/gold]", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.Contains("铸令", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("第一幕", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("第二幕", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("第三幕", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("火印宿主", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("注令", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("路线", zhsAscension["LEVEL_12.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("费用", zhsAscension["LEVEL_13.description"], StringComparison.Ordinal);
        Assert.Contains("[gold]耗能[/gold]降低[blue]1[/blue]", zhsAscension["LEVEL_13.description"], StringComparison.Ordinal);
        var allAscensionText = string.Join(Environment.NewLine, engAscension.Values.Concat(zhsAscension.Values));
        Assert.DoesNotContain("Wake-up source", allAscensionText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Queen-side settlement", allAscensionText, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\u7206\u53d1\u538b\u529b", allAscensionText, StringComparison.Ordinal);
        Assert.DoesNotContain("\u7206\u53d1\u9884\u8b66", allAscensionText, StringComparison.Ordinal);
        Assert.Contains("Forge Token special rest-site action payout is disabled", manualChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("Special rest-site actions heal 5 HP", manualChecklist, StringComparison.Ordinal);
    }

    [LocalSourceFact]
    public void CorePowerNodeStillDisplaysCounterStackAmounts()
    {
        var corePowerNode = ReadLocalCoreText("Nodes", "Combat", "NPower.cs");

        Assert.Contains("Model.StackType == PowerStackType.Counter", corePowerNode, StringComparison.Ordinal);
    }

    [Fact]
    public void FissionUsesCanonicalExhaustPipelineAndTriggersExhaustListeners()
    {
        var fission = ReadRepoText("EZMicroBalanceCode", "Ascension", "Enchantments", "FissionEnchantment.cs");

        AssertSourceContains(
            fission,
            "Card.AddKeyword(CardKeyword.Exhaust)",
            "triggers [gold]Exhaust[/gold] effects normally",
            "正常触发[gold]消耗[/gold]效果");

    }

    [LocalSourceFact]
    public void CoreExhaustPipelineStillMovesExhaustCardsAndNotifiesListeners()
    {
        var cardModel = ReadLocalCoreText("Models", "CardModel.cs");
        var cardCmd = ReadLocalCoreText("Commands", "CardCmd.cs");
        var hook = ReadLocalCoreText("Hooks", "Hook.cs");
        var drumOfBattle = ReadLocalCoreText("Models", "Cards", "DrumOfBattle.cs");
        var howlFromBeyond = ReadLocalCoreText("Models", "Cards", "HowlFromBeyond.cs");
        var feelNoPain = ReadLocalCoreText("Models", "Powers", "FeelNoPainPower.cs");
        var darkEmbrace = ReadLocalCoreText("Models", "Powers", "DarkEmbracePower.cs");
        var charonsAshes = ReadLocalCoreText("Models", "Relics", "CharonsAshes.cs");

        var resultPile = SliceBetween(cardModel, "protected virtual PileType GetResultPileTypeForCardPlay()", "public async Task MoveToResultPileWithoutPlaying");
        AssertSourceContains(
            resultPile,
            "if (ExhaustOnNextPlay || Keywords.Contains(CardKeyword.Exhaust))",
            "return PileType.Exhaust;");

        var playWrapper = SliceBetween(cardModel, "public async Task OnPlayWrapper", "protected async Task<int> GeneratePlayCount");
        AssertSourceContains(
            playWrapper,
            "var (resultPileType, resultPilePosition) = Hook.ModifyCardPlayResultPileTypeAndPosition",
            "case PileType.Exhaust:",
            "await CardCmd.Exhaust(choiceContext, this, causedByEthereal: false, skipCardPileVisuals);");

        AssertSourceContains(
            cardCmd,
            "public static async Task Exhaust",
            "await CardPileCmd.Add(card, PileType.Exhaust",
            "CombatManager.Instance.History.CardExhausted(combatState, card)",
            "await Hook.AfterCardExhausted(combatState, choiceContext, card, causedByEthereal)");

        var exhaustHook = SliceBetween(hook, "public static async Task AfterCardExhausted", "public static async Task AfterCardGeneratedForCombat");
        AssertSourceContains(
            exhaustHook,
            "foreach (AbstractModel model in combatState.IterateHookListeners())",
            "await model.AfterCardExhausted(choiceContext, card, causedByEthereal)");

        AssertSourceContains(
            drumOfBattle,
            "public override async Task AfterCardExhausted",
            "if (card == this",
            "await PlayerCmd.GainEnergy");
        AssertSourceContains(
            howlFromBeyond,
            "public override async Task AfterAutoPostPlayPhaseEntered",
            "pile.Type == PileType.Exhaust",
            "await CardCmd.AutoPlay(choiceContext, this, null)");
        AssertSourceContains(
            feelNoPain,
            "public override async Task AfterCardExhausted",
            "await CreatureCmd.GainBlock");
        AssertSourceContains(
            darkEmbrace,
            "public override async Task AfterCardExhausted",
            "await CardPileCmd.Draw");
        AssertSourceContains(
            charonsAshes,
            "public override async Task AfterCardExhausted",
            "await CreatureCmd.Damage");
    }

}
