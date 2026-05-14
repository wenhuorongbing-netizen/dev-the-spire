using System.Text;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class VakuuTemptationGuardTests
{
    [Fact]
    public void TemptationStatusCardIsHiddenPoolAppropriateAndNotNormallyGenerated()
    {
        var card = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuTemptationCard.cs");
        var exportPreset = ReadRepoText("export_presets.cfg");

        AssertSourceContains(
            card,
            "[CustomID(CardId)]",
            "[Pool(typeof(StatusCardPool))]",
            "public const string CardId = \"EZMB_VAKUU_TEMPTATION\"",
            "base(-1, CardType.Status, CardRarity.Status, TargetType.None, showInCardLibrary: false)",
            "CardKeyword.Ethereal",
            "CardKeyword.Unplayable",
            "CanBeGeneratedInCombat => false",
            "CanBeGeneratedByModifiers => false",
            "MaxUpgradeLevel => 0",
            "images/card_portraits/vakuu_temptation.png",
            "images/card_portraits/big/vakuu_temptation.png",
            "HoverTipFactory.FromKeyword(CardKeyword.Ethereal)",
            "HoverTipFactory.FromKeyword(CardKeyword.Unplayable)");

        Assert.Contains("res://EZMicroBalance/images/card_portraits/vakuu_temptation.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/images/card_portraits/big/vakuu_temptation.png", exportPreset, StringComparison.Ordinal);
    }

    [Fact]
    public void TemptationExhaustRewardUsesSourceBackedEnergyAndHpLossCommands()
    {
        var card = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuTemptationCard.cs");
        var exhaustBlock = SliceFrom(card, "public override async Task AfterCardExhausted");

        AssertSourceContains(
            exhaustBlock,
            "if (card != this)",
            "PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner)",
            "CreatureCmd.Damage(",
            "Owner.Creature",
            "DynamicVars.HpLoss.BaseValue",
            "ValueProp.Unblockable | ValueProp.Unpowered",
            "Vakuu Temptation exhausted: gained 1 Energy and lost 3 HP");
    }

    [Fact]
    public void VakuuFightInjectsTemptationOnlyInsideCustomVakuuTrialCombat()
    {
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightRunHook.cs");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");

        Assert.Contains("VakuuFightInitializer.Initialize();", mainFile, StringComparison.Ordinal);
        AssertSourceContains(
            runHook,
            "ModHelper.SubscribeForRunStateHooks",
            "ModelDb.GetById<VakuuFightRunHook>",
            "VakuuFightFeatureGate.IsFightEnabledForRun(runState)",
            "public override bool ShouldReceiveCombatHooks => true",
            "public override Task AfterPlayerTurnStart",
            "FirstTemptationTurn = 1",
            "TemptationTurnCadence = 2",
            "player.Creature.CombatState is not { } combatState",
            "!IsVakuuTrialCombat(combatState)",
            "combatState.RunState.Players.Count != 1",
            "combatState.RoundNumber",
            "(round - FirstTemptationTurn) % TemptationTurnCadence",
            "CombatStates.GetOrCreateValue(combatState)",
            "InjectedRounds.Add(round)",
            "combatState.CreateCard<VakuuTemptation>(player)",
            "PileType.Draw",
            "CardPilePosition.Top",
            "Vakuu fight added Temptation to the top of the draw pile");
        AssertSourceContains(
            runHook,
            "private static bool IsVakuuTrialCombat(ICombatState combatState) =>",
            "combatState.Encounter is EzmbVakuuTrialEncounter");
        AssertSourceContains(
            encounter,
            "base(RoomType.Event, autoAdd: false)",
            "ShouldGiveRewards => false",
            "ModelDb.Monster<OwlMagistrate>()");
        Assert.Contains("runState.Players.Count == 1", gate, StringComparison.Ordinal);
    }

    [Fact]
    public void VakuuTemptationLocalizationIsBilingualReadableAndWarnsAboutCadence()
    {
        var engCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        AssertLocalizedCard(engCards, "EZMB_VAKUU_TEMPTATION", "Temptation", "[gold]Energy[/gold]", "{HpLoss:diff()} HP");
        AssertLocalizedCard(zhsCards, "EZMB_VAKUU_TEMPTATION", "诱惑", "[gold]能量[/gold]", "{HpLoss:diff()}点生命");

        AssertSourceContains(
            engAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "After your hand is drawn",
            "turns [blue]1[/blue], [blue]3[/blue], [blue]5[/blue]",
            "[gold]Temptation[/gold]",
            "top of your [gold]Draw Pile[/gold]",
            "no normal combat rewards",
            "if enough remain");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "After your hand is drawn",
            "turns [blue]1[/blue], [blue]3[/blue], [blue]5[/blue]",
            "[gold]Temptation[/gold]",
            "top of your [gold]Draw Pile[/gold]",
            "No normal combat rewards",
            "otherwise no extra blessing");
        AssertSourceContains(
            zhsAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "抽完起始手牌后",
            "[blue]1[/blue]",
            "[blue]3[/blue]",
            "[blue]5[/blue]",
            "[gold]诱惑[/gold]");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "抽完起始手牌后",
            "[blue]1[/blue]",
            "[blue]3[/blue]",
            "[blue]5[/blue]",
            "[gold]诱惑[/gold]");

        foreach (var value in new[]
        {
            engCards["EZMB_VAKUU_TEMPTATION.description"],
            zhsCards["EZMB_VAKUU_TEMPTATION.description"],
            engAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            zhsAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            engRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            zhsRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"]
        })
        {
            Assert.DoesNotContain("TODO", value, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("source", value, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("test", value, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("\uFFFD", value, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ActiveDocsDescribeTemptationAsSourceBackedWhileRuntimeClaimsRemainPending()
    {
        var docs = new[]
        {
            ReadRepoText("docs", "issues.md"),
            ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "implementation-plan.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md"),
            ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md")
        };

        foreach (var doc in docs)
        {
            Assert.DoesNotContain("Temptation remains not implemented", doc, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("future content and is not implemented", doc, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Temptation | Future", doc, StringComparison.Ordinal);
        }

        var joined = string.Join(Environment.NewLine, docs);
        Assert.Contains("Temptation", joined, StringComparison.Ordinal);
        Assert.Contains("live", joined, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("save/load", joined, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("co-op", joined, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("pending", joined, StringComparison.OrdinalIgnoreCase);
    }

    private static void AssertLocalizedCard(
        IReadOnlyDictionary<string, string> cards,
        string id,
        string title,
        params string[] descriptionSnippets)
    {
        Assert.Equal(title, cards[$"{id}.title"]);
        AssertSourceContains(cards[$"{id}.description"], descriptionSnippets);
        Assert.DoesNotContain("[gold]Exhaust[/gold]", cards[$"{id}.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("[gold]Ethereal[/gold]", cards[$"{id}.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("[gold]Unplayable[/gold]", cards[$"{id}.description"], StringComparison.Ordinal);
    }

    private static SortedDictionary<string, string> JsonStringMap(params string[] parts)
    {
        using var document = JsonDocument.Parse(ReadRepoText(parts));
        var map = new SortedDictionary<string, string>(StringComparer.Ordinal);

        foreach (var property in document.RootElement.EnumerateObject())
        {
            Assert.Equal(JsonValueKind.String, property.Value.ValueKind);
            map.Add(property.Name, property.Value.GetString() ?? string.Empty);
        }

        return map;
    }

    private static string SliceFrom(string value, string start)
    {
        var startIndex = value.IndexOf(start, StringComparison.Ordinal);
        Assert.True(startIndex >= 0, $"Missing start marker: {start}");
        return value[startIndex..];
    }

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    private static string ReadRepoText(params string[] parts)
    {
        return File.ReadAllText(RepoPath(parts), Encoding.UTF8);
    }

    private static string RepoPath(params string[] parts)
    {
        return Path.Combine(new[] { FindRepoRoot() }.Concat(parts).ToArray());
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "EZMicroBalance.csproj")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find repository root from test output directory.");
    }
}
