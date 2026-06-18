using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class Sts1EventFeatureGuardTests
{
    [Fact]
    public void DivineFountainRequiresEveryPlayerToHaveACurseAndUsesDrinkOption()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1DivineFountain.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "public override bool IsAllowed(IRunState runState)",
            "foreach (var player in runState.Players)",
            "if (!HasCurse(player))",
            "return runState.Players.Count > 0;",
            "private static bool HasCurse(Player player)",
            "card.Type == CardType.Curse",
            "new EventOption(this, Drink, InitialOptionKey(\"DRINK\"))",
            "private async Task Drink()",
            "STS1_DIVINE_FOUNTAIN.pages.DRINK.description");

        AssertSourceContains(engLoc,
            "\"STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.DRINK.title\": \"Drink\"",
            "\"STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.DRINK.description\": \"Remove all Curses from your deck.\"",
            "\"STS1_DIVINE_FOUNTAIN.pages.DRINK.description\"");

        AssertSourceContains(zhsLoc,
            "\"STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.DRINK.title\"",
            "\"STS1_DIVINE_FOUNTAIN.pages.INITIAL.options.DRINK.description\"",
            "\"STS1_DIVINE_FOUNTAIN.pages.DRINK.description\"");

        Assert.DoesNotContain("InitialOptionKey(\"PRAY\")", source, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_DIVINE_FOUNTAIN.pages.PRAY", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_DIVINE_FOUNTAIN.pages.PRAY", zhsLoc, StringComparison.Ordinal);
    }

    [Fact]
    public void BigFishUsesBoxOptionName()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1BigFish.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "new EventOption(this, Box, InitialOptionKey(\"BOX\"))",
            "private async Task Box()",
            "STS1_BIG_FISH.pages.BOX.description");

        AssertSourceContains(engLoc,
            "\"STS1_BIG_FISH.pages.INITIAL.options.BOX.title\"",
            "\"STS1_BIG_FISH.pages.INITIAL.options.BOX.description\"",
            "\"STS1_BIG_FISH.pages.BOX.description\"");

        AssertSourceContains(zhsLoc,
            "\"STS1_BIG_FISH.pages.INITIAL.options.BOX.title\"",
            "\"STS1_BIG_FISH.pages.INITIAL.options.BOX.description\"",
            "\"STS1_BIG_FISH.pages.BOX.description\"");

        Assert.DoesNotContain("SHOE", source, StringComparison.Ordinal);
        Assert.DoesNotContain("Shoe", source, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_BIG_FISH.pages.INITIAL.options.SHOE", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_BIG_FISH.pages.SHOE.description", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_BIG_FISH.pages.INITIAL.options.SHOE", zhsLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_BIG_FISH.pages.SHOE.description", zhsLoc, StringComparison.Ordinal);
    }

    [Fact]
    public void GoldenIdolTrapOptionsUseWikiBranchNames()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1GoldenIdol.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "new EventOption(this, Outrun,",
            "OptionKey(\"TRAP\", \"OUTRUN\")",
            "new EventOption(this, () => Smash(smashDamage),",
            "OptionKey(\"TRAP\", \"SMASH\")",
            "new EventOption(this, () => Hide(hideMaxHpLoss),",
            "OptionKey(\"TRAP\", \"HIDE\")",
            "private const decimal HideMaxHpPctNormal = 0.08m;",
            "private const decimal HideMaxHpPctA15 = 0.10m;");

        AssertSourceContains(engLoc,
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.OUTRUN.title\": \"Outrun\"",
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.SMASH.title\": \"Smash\"",
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.HIDE.title\": \"Hide\"");

        AssertSourceContains(zhsLoc,
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.OUTRUN.title\"",
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.SMASH.title\"",
            "\"STS1_GOLDEN_IDOL.pages.TRAP.options.HIDE.title\"");

        Assert.DoesNotContain("OptionKey(\"TRAP\", \"JUMP\")", source, StringComparison.Ordinal);
        Assert.DoesNotContain("OptionKey(\"TRAP\", \"DESTROY\")", source, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_GOLDEN_IDOL.pages.TRAP.options.JUMP", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_GOLDEN_IDOL.pages.TRAP.options.DESTROY", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_GOLDEN_IDOL.pages.TRAP.options.JUMP", zhsLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_GOLDEN_IDOL.pages.TRAP.options.DESTROY", zhsLoc, StringComparison.Ordinal);
    }

    [Fact]
    public void GoldenShrineUsesWikiGoldAndRegretOptions()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1GoldenShrine.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "private const int PrayGoldNormal = 100;",
            "private const int PrayGoldA15 = 50;",
            "private const int DesecrateGold = 275;",
            "new EventOption(this, Pray, InitialOptionKey(\"PRAY\"))",
            "new EventOption(this, Desecrate, InitialOptionKey(\"DESECRATE\"))",
            "await PlayerCmd.GainGold(PrayGoldAmount, owner);",
            "await PlayerCmd.GainGold(DesecrateGold, owner);",
            "ModelDb.Card<Regret>()",
            "STS1_GOLDEN_SHRINE.pages.PRAY.description");

        AssertSourceContains(engLoc,
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.PRAY.title\": \"Pray\"",
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.PRAY.description\": \"Gain 100 gold. (50 on Ascension 15+.)\"",
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.DESECRATE.description\": \"Gain 275 gold. Obtain Regret.\"",
            "\"STS1_GOLDEN_SHRINE.pages.PRAY.description\"");

        AssertSourceContains(zhsLoc,
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.PRAY.title\"",
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.PRAY.description\"",
            "\"STS1_GOLDEN_SHRINE.pages.INITIAL.options.DESECRATE.description\"",
            "\"STS1_GOLDEN_SHRINE.pages.PRAY.description\"");

        Assert.DoesNotContain("TAKE_GOLD", source, StringComparison.Ordinal);
        Assert.DoesNotContain("TAKE_GOLD", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("TAKE_GOLD", zhsLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("RemoveAllCurses", source, StringComparison.Ordinal);
        Assert.DoesNotContain("hasCurses", source, StringComparison.Ordinal);
        Assert.DoesNotContain("GoldNormal = 250", source, StringComparison.Ordinal);
        Assert.DoesNotContain("GoldA15 = 100", source, StringComparison.Ordinal);
    }

    [Fact]
    public void TheClericUsesA15PurifyCostAndGoldEligibility()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1TheCleric.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "private const int HealCost = 35;",
            "private const int PurifyCostNormal = 50;",
            "private const int PurifyCostA15 = 75;",
            "private int PurifyCost => HasA15 ? PurifyCostA15 : PurifyCostNormal;",
            "public override bool IsAllowed(IRunState runState)",
            "if (player.Gold < HealCost)",
            "return runState.Players.Count > 0;",
            "var canHeal = (Owner?.Gold ?? 0) >= HealCost;",
            "var canPurify = (Owner?.Gold ?? 0) >= PurifyCost;",
            "await PlayerCmd.LoseGold(HealCost, owner, GoldLossType.Spent);",
            "await PlayerCmd.LoseGold(PurifyCost, owner, GoldLossType.Spent);");

        AssertSourceContains(engLoc,
            "\"STS1_THE_CLERIC.pages.INITIAL.options.HEAL.title\": \"Heal (35 gold)\"",
            "\"STS1_THE_CLERIC.pages.INITIAL.options.PURIFY.title\": \"Purify (50/75 gold)\"",
            "\"STS1_THE_CLERIC.pages.INITIAL.options.PURIFY.description\": \"Pay 50 gold. (75 on Ascension 15+.) Remove a card from your deck.\"");

        AssertSourceContains(zhsLoc,
            "\"STS1_THE_CLERIC.pages.INITIAL.options.HEAL.title\"",
            "\"STS1_THE_CLERIC.pages.INITIAL.options.PURIFY.title\"",
            "\"STS1_THE_CLERIC.pages.INITIAL.options.PURIFY.description\"");

        Assert.DoesNotContain("private const int PurifyCost = 50;", source, StringComparison.Ordinal);
    }

    [Fact]
    public void TheLabHasOnlyOpenOption()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1TheLab.cs");
        var engLoc = ReadRepoText("EZMicroBalance", "localization", "eng", "sts1_events.json");
        var zhsLoc = ReadRepoText("EZMicroBalance", "localization", "zhs", "sts1_events.json");

        AssertSourceContains(source,
            "new EventOption(this, Open, InitialOptionKey(\"OPEN\"))",
            "int count = HasA15 ? 2 : 3;",
            "SetEventFinished(L10NLookup(\"STS1_THE_LAB.pages.OPEN.description\"))");

        Assert.Equal(1, CountOccurrences(source, "new EventOption(this,"));
        AssertSourceContains(engLoc, "\"STS1_THE_LAB.pages.INITIAL.options.OPEN.description\": \"Obtain 3 random potions. (2 on Ascension 15+.)\"");
        Assert.DoesNotContain("InitialOptionKey(\"LEAVE\")", source, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_THE_LAB.pages.INITIAL.options.LEAVE", engLoc, StringComparison.Ordinal);
        Assert.DoesNotContain("STS1_THE_LAB.pages.INITIAL.options.LEAVE", zhsLoc, StringComparison.Ordinal);
    }

    [Fact]
    public void OldBeggarOfferRequiresEnoughGold()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Shared", "Sts1OldBeggar.cs");

        AssertSourceContains(source,
            "private const int GoldCost = 75;",
            "var canOfferGold = (Owner?.Gold ?? 0) >= GoldCost;",
            "new EventOption(this, canOfferGold ? OfferGold : null, InitialOptionKey(\"OFFER_GOLD\"))",
            "await PlayerCmd.LoseGold(GoldCost, owner, GoldLossType.Spent);",
            "await Sts1EventHelpers.OpenCardRemoval(owner);");
    }

    [Fact]
    public void ShiningLightUpgradesRandomCardsWithoutManualSelection()
    {
        var source = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Models", "Act1", "Sts1ShiningLight.cs");
        var helpers = ReadRepoText("EZMicroBalanceCode", "Sts1Events", "Runtime", "Sts1EventHelpers.cs");

        AssertSourceContains(source,
            "await Sts1EventHelpers.UpgradeRandomCards(owner, Rng, count: 2);");
        Assert.DoesNotContain("OpenCardUpgrade(owner, count: 2)", source, StringComparison.Ordinal);

        AssertSourceContains(helpers,
            "public static Task UpgradeRandomCards(Player owner, Rng rng, int count = 1)",
            ".Where(card => card.IsUpgradable)",
            ".TakeRandom(count, rng)",
            "CardCmd.Upgrade(cards, CardPreviewStyle.EventLayout);");
    }

    [Fact]
    public void AdditiveBatch1SimpleBatchSpecsExist()
    {
        foreach (var fileName in new[]
        {
            "purifier.md",
            "upgrade-shrine.md",
            "golden-shrine.md",
            "the-cleric.md",
            "old-beggar.md",
            "shining-light.md"
        })
        {
            var path = Path.Combine(Root, "docs", "features", "sts1-events", "event-specs", fileName);
            Assert.True(File.Exists(path), $"Missing AdditiveBatch1 simple-batch event spec: {fileName}");
        }
    }
}
