using System.Text;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class MorviV22GuardTests
{
    private static readonly string[] BlessingIds =
    [
        "morvi_forbidden_loan",
        "morvi_misprint_press",
        "morvi_red_ink_overdraft",
        "morvi_overdue_library",
        "morvi_open_book_exam",
        "morvi_paperstorm",
        "morvi_blueprint_proof",
        "morvi_debt_settlement"
    ];

    [Fact]
    public void MorviIsDefaultOnDisableableForceableAndHasEightBlessings()
    {
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviFeatureGate.cs");
        var ancient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAncient.cs");
        var blessings = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviBlessingIds.cs");
        var initializer = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviInitializer.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");

        AssertSourceContains(
            gate,
            "EZMB_DISABLE_MORVI",
            "SPIREPLUS_DISABLE_MORVI",
            "EZMB_FORCE_ANCIENT",
            "SPIREPLUS_FORCE_ANCIENT",
            "EZMB_FORCE_MORVI_BLESSING",
            "SPIREPLUS_FORCE_MORVI_BLESSING",
            "ShouldForceMorvi",
            "!IsTruthy(Environment.GetEnvironmentVariable(DisableEnvironmentVariable))");
        Assert.DoesNotContain("return IsTruthy(value);", gate, StringComparison.Ordinal);

        AssertSourceContains(
            ancient,
            "CustomAncientModel",
            "HarmonyPatch(typeof(Hive), nameof(Hive.GetUnlockedAncients))",
            "MorviFeatureGate.ShouldForceMorvi",
            "unlockedAncients = [morvi]",
            "list.Add(morvi)",
            "ExpectedInitialOptionCount = 3",
            "options.UnstableShuffle(Rng).Take(ExpectedInitialOptionCount).ToList()",
            "MorviAssetPaths.BackgroundScene");
        Assert.Contains("MorviInitializer.Initialize();", mainFile, StringComparison.Ordinal);
        AssertSourceContains(
            initializer,
            "ModHelper.SubscribeForRunStateHooks",
            "ModelDb.GetById<MorviRunHook>",
            "default-on");
        AssertSourceContains(
            savedFields,
            "SavedSpireField<Player, string> MorviStateKey",
            "SavedSpireField<CardModel, string> MorviDeckStateKey",
            "SavedSpireField<CardModel, bool> MorviBorrowedAncientCard",
            "SavedSpireField<CardModel, bool> MorviOpenBookSealedCard");

        foreach (var id in BlessingIds)
        {
            Assert.Contains(id, blessings, StringComparison.Ordinal);
            Assert.Contains(id, ancient, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void MisprintPressUsesPlayCountNotHandCopiesAndBlocksPowerAutoplayRecursion()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviRunHook.cs");
        var misprint = SliceBetween(runHook, "public static int ModifyCardPlayCount", "public static bool TryModifyEnergyCostInCombat");

        AssertSourceContains(
            misprint,
            "MorviBlessingIds.MisprintPress",
            "combatState.MisprintUsedThisTurn",
            "TryConsumeAutoPlayModifierBlock(card, combatState)",
            "!IsNaturalPlayerCombatCard(card)",
            "card.Type is not (CardType.Attack or CardType.Skill)",
            "combatState.MisprintDrawAfterCards.Add(card)",
            "return playCount + MisprintExtraPlayCount;");
        AssertSourceContains(
            runHook,
            "card.DeckVersion != null",
            "!card.IsClone",
            "card.Type is not CardType.Status and not CardType.Curse",
            "public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)",
            "cardPlay.IsAutoPlay");
        Assert.DoesNotContain("CreateClone", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("CardCmd.AutoPlay", runHook, StringComparison.Ordinal);
    }

    [Fact]
    public void MorviSourceConstantsAndStatefulBlessingsMatchV22Numbers()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviRunHook.cs");

        AssertSourceContains(
            runHook,
            "ForbiddenLoanKeepGoldCost = 180",
            "ForbiddenLoanAttackSkillHpLoss = 1",
            "ForbiddenLoanPowerHpLoss = 8",
            "player.Character.CardPool",
            "card.Rarity == CardRarity.Ancient",
            "CardSelectCmd.FromChooseACardScreen",
            "var addResult = await CardPileCmd.Add(selected, PileType.Deck)",
            "if (!addResult.success)",
            "AncientSavedStateFields.MorviBorrowedAncientCard[borrowedCard] = true",
            "player.RunState.CurrentActIndex == 1",
            "AutoSettleForbiddenLoan");

        AssertSourceContains(
            runHook,
            "RedInkOverdraftDraw = 2",
            "RedInkOverdraftEnergy = 1",
            "RedInkOverdraftGoldPerDebt = 12",
            "RedInkOverdraftHpPerUnpaidDebt = 3",
            "MorviRedInkOverdraftCard",
            "CanUseRedInkOverdraft",
            "player.PlayerCombatState?.Energy != 0",
            "combatState.RedInkUsedThisTurn",
            "hand.Cards.Count >= CardPile.MaxCardsInHand",
            "card.Pile?.Type != PileType.Hand",
            "CardPileCmd.RemoveFromCombat(result.cardAdded, skipVisuals: true)",
            "visibleDebtCount = player.Creature.GetPower<MorviOverdraftPower>()?.Amount ?? 0",
            "debtCount = Math.Max(combatState.RedInkDebtsThisCombat, visibleDebtCount)",
            "DamagePlayerNonlethal(player, RedInkOverdraftHpPerUnpaidDebt)");

        AssertSourceContains(
            runHook,
            "OpenBookDraw = 5",
            "OpenBookEnergy = 2",
            "OpenBookSealTurn = 1",
            "OpenBookReturnTurn = 3",
            "CardPileCmd.Draw(choiceContext, OpenBookDraw, player)",
            "CardPileCmd.Add(card, PileType.Exhaust)",
            "AncientSavedStateFields.MorviOpenBookSealedCard[addResult.cardAdded] = true",
            "FindOpenBookSealedCards(player, combatState)",
            "addResult.cardAdded.SetToFreeThisTurn()");

        AssertSourceContains(
            runHook,
            "PaperstormWastePaperCount = 4",
            "PaperstormStatusTriggersPerTurn = 2",
            "AncientCardHelpers.TryAddGeneratedCardToCombat(waste, PileType.Draw, player, CardPilePosition.Random)",
            "card.Type != CardType.Status",
            "card.Pile?.Type != PileType.Hand",
            "CardCmd.Exhaust(choiceContext, card, skipVisuals: true)",
            "PlayerCmd.GainEnergy(1m, player)");

        AssertSourceContains(
            runHook,
            "BlueprintProofStacks = 3",
            "BlueprintProofCostReduction = 1",
            "BlueprintProofBlock = 4",
            "CardCmd.Upgrade(card, CardPreviewStyle.None)",
            "CardCmd.Downgrade(card)",
            "combatState.BlueprintDrawAfterCards.Add(card)",
            "combatState.BlueprintBlockAfterCards.Add(card)");

        AssertSourceContains(
            runHook,
            "DebtSettlementImmediateGold = 220",
            "DebtSettlementStartingDebt = 320",
            "DebtSettlementCombatDue = 40",
            "DebtSettlementHpPerTenShortfall = 3",
            "CardSelectCmd.FromDeckForRemoval",
            "CardSelectCmd.FromDeckForUpgrade",
            "Math.Ceiling(shortfall / 10m)",
            "DamagePlayerNonlethal(player, calculatedHpLoss)",
            "maximumNonlethalHpLoss = Math.Max(0m, player.Creature.CurrentHp - 1m)",
            "hpLoss = Math.Min(calculatedHpLoss, maximumNonlethalHpLoss)",
            "DebtRemaining = Math.Max(0, progress.DebtRemaining - due)");
    }

    [Fact]
    public void OverdueLibraryCardsPowersAndCleanupAreSourceBacked()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviRunHook.cs");
        var cards = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviCards.cs");
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviPowers.cs");
        var engCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");

        AssertSourceContains(
            runHook,
            "OverdueLibraryPageCount = 3",
            "ArchivePageTypes",
            "MorviArchiveDrawPage",
            "MorviArchiveVeilPage",
            "MorviArchiveBurnPage",
            "MorviArchiveDiscountPage",
            "MorviArchiveBraveryPage",
            "MorviArchiveDexterityPage",
            "CleanupMorviTemporaryCards",
            "AncientCardHelpers.TryAddGeneratedCardToCombat(page, PileType.Hand, player)",
            "AncientCardHelpers.TryAddGeneratedCardToCombat(card, PileType.Hand, player)",
            "CardPileCmd.RemoveFromCombat(cards, skipVisuals: true)");
        Assert.DoesNotContain("CardPileCmd.AddGeneratedCardToCombat", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("CardPileCmd.AddGeneratedCardsToCombat", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            cards,
            "CardKeyword.Ethereal",
            "CardKeyword.Exhaust",
            "CanBeGeneratedInCombat => false",
            "CanBeGeneratedByModifiers => false",
            "CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner)",
            "CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay)",
            "TargetingAllOpponents(CombatState)",
            "ArmOverdueLibraryDiscount(Owner)",
            "PowerCmd.Apply<MorviBraveryPagePower>",
            "PowerCmd.Apply<MorviDexterityPagePower>");
        AssertSourceContains(
            powers,
            "MorviBraveryPagePower : TemporaryStrengthPower, ICustomModel",
            "MorviDexterityPagePower : TemporaryDexterityPower, ICustomModel");

        foreach (var key in new[]
        {
            "EZMB_MORVI_ARCHIVE_DRAW_PAGE.title",
            "EZMB_MORVI_ARCHIVE_VEIL_PAGE.title",
            "EZMB_MORVI_ARCHIVE_BURN_PAGE.title",
            "EZMB_MORVI_ARCHIVE_DISCOUNT_PAGE.title",
            "EZMB_MORVI_ARCHIVE_BRAVERY_PAGE.title",
            "EZMB_MORVI_ARCHIVE_DEXTERITY_PAGE.title",
            "EZMB_MORVI_WASTE_PAPER.title"
        })
        {
            Assert.True(engCards.ContainsKey(key), $"Missing English Morvi card localization: {key}");
            Assert.True(zhsCards.ContainsKey(key), $"Missing zhs Morvi card localization: {key}");
        }
    }

    [Fact]
    public void MorviLocalizationAssetsAndHoverSupportArePresentAndReadable()
    {
        var ancient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviAncient.cs");
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Morvi", "MorviPowers.cs");
        var exportPreset = ReadRepoText("export_presets.cfg");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsPowers = JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<MorviOverdraftPower>()",
            "HoverTipFactory.FromPower<MorviOpenBookPower>()",
            "HoverTipFactory.FromPower<MorviPaperstormPower>()",
            "HoverTipFactory.FromPower<MorviProofreadPower>()",
            "HoverTipFactory.FromPower<MorviDebtPower>()",
            "HoverTipFactory.Static(StaticHoverTip.Energy)",
            "HoverTipFactory.Static(StaticHoverTip.Block)");
        AssertSourceContains(
            powers,
            "internal sealed class MorviDebtPower",
            "internal sealed class MorviProofreadPower",
            "internal sealed class MorviOpenBookPower",
            "internal sealed class MorviOverdraftPower",
            "internal sealed class MorviPaperstormPower");

        foreach (var id in BlessingIds)
        {
            var key = $"EZMB_MORVI.pages.INITIAL.options.{id}.description";
            Assert.True(engAncients.TryGetValue(key, out var engDescription), $"Missing English Morvi ancient localization: {key}");
            Assert.True(zhsAncients.TryGetValue(key, out var zhsDescription), $"Missing zhs Morvi ancient localization: {key}");
            AssertNoMojibake(engDescription);
            AssertNoMojibake(zhsDescription);
            Assert.Contains("[blue]", engDescription, StringComparison.Ordinal);
            Assert.Contains("[blue]", zhsDescription, StringComparison.Ordinal);
        }

        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_misprint_press.description"],
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "[gold]Energy[/gold]");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_red_ink_overdraft.description"],
            "[gold]Overdraft[/gold]",
            "[gold]Energy[/gold]",
            "nonlethal HP");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_blueprint_proof.description"],
            "[gold]Proofread[/gold]",
            "[gold]Block[/gold]");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_debt_settlement.description"],
            "[gold]Debt[/gold]");
        AssertSourceContains(
            engAncients["EZMB_MORVI.pages.INITIAL.options.morvi_open_book_exam.description"],
            "sealed in the [gold]Exhaust Pile[/gold]");

        AssertLocalizedKeys(MorviRelicKeys(), engRelics, zhsRelics, "Morvi option relic localization");
        AssertLocalizedKeys(MorviPowerKeys(), engPowers, zhsPowers, "Morvi power localization");

        foreach (var relativePath in MorviResourcePaths())
        {
            Assert.True(File.Exists(RepoPath(relativePath.Split('/'))), $"Missing Morvi resource: {relativePath}");
            Assert.Contains($"res://{relativePath}", exportPreset, StringComparison.Ordinal);
        }
    }

    private static IEnumerable<string> MorviRelicKeys()
    {
        foreach (var key in new[]
        {
            "FORBIDDEN_LOAN",
            "MISPRINT_PRESS",
            "RED_INK_OVERDRAFT",
            "OVERDUE_LIBRARY",
            "OPEN_BOOK_EXAM",
            "PAPERSTORM",
            "BLUEPRINT_PROOF",
            "DEBT_SETTLEMENT"
        })
        {
            yield return $"EZMICROBALANCE-MORVI_{key}_OPTION_RELIC.title";
            yield return $"EZMICROBALANCE-MORVI_{key}_OPTION_RELIC.description";
            yield return $"EZMICROBALANCE-MORVI_{key}_OPTION_RELIC.flavor";
        }
    }

    private static IEnumerable<string> MorviPowerKeys()
    {
        foreach (var key in new[]
        {
            "DEBT",
            "PROOFREAD",
            "OPEN_BOOK",
            "OVERDRAFT",
            "PAPERSTORM"
        })
        {
            yield return $"EZMICROBALANCE-MORVI_{key}_POWER.title";
            yield return $"EZMICROBALANCE-MORVI_{key}_POWER.description";
            yield return $"EZMICROBALANCE-MORVI_{key}_POWER.smartDescription";
        }
    }

    private static IEnumerable<string> MorviResourcePaths()
    {
        yield return "EZMicroBalance/images/events/ezmb_morvi.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_map_icon_outline.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon.png";
        yield return "EZMicroBalance/images/ancients/morvi/ezmb_morvi_run_history_icon_outline.png";
        foreach (var id in BlessingIds)
        {
            yield return $"EZMicroBalance/images/ancients/morvi/options/{id}.png";
        }

        yield return "EZMicroBalance/scenes/events/background_scenes/ezmb_morvi.tscn";
    }

    private static void AssertLocalizedKeys(
        IEnumerable<string> keys,
        IReadOnlyDictionary<string, string> eng,
        IReadOnlyDictionary<string, string> zhs,
        string context)
    {
        foreach (var key in keys)
        {
            Assert.True(eng.TryGetValue(key, out var engValue), $"Missing English {context}: {key}");
            Assert.True(zhs.TryGetValue(key, out var zhsValue), $"Missing zhs {context}: {key}");
            Assert.False(string.IsNullOrWhiteSpace(engValue), $"Empty English {context}: {key}");
            Assert.False(string.IsNullOrWhiteSpace(zhsValue), $"Empty zhs {context}: {key}");
            AssertNoMojibake(engValue);
            AssertNoMojibake(zhsValue);
        }
    }

    private static void AssertNoMojibake(string value)
    {
        Assert.DoesNotContain("\uFFFD", value, StringComparison.Ordinal);
        Assert.DoesNotContain("鐟佷礁", value, StringComparison.Ordinal);
        Assert.DoesNotContain("瀵偓", value, StringComparison.Ordinal);
        Assert.DoesNotContain("閺€", value, StringComparison.Ordinal);
        Assert.DoesNotContain("鍊哄姟", value, StringComparison.Ordinal);
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

    private static string SliceBetween(string source, string startMarker, string endMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing start marker: {startMarker}");
        var end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        Assert.True(end >= 0, $"Missing end marker: {endMarker}");
        return source[start..end];
    }

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    private static string ReadRepoText(params string[] parts) =>
        File.ReadAllText(RepoPath(parts), Encoding.UTF8);

    private static string RepoPath(params string[] parts) =>
        Path.Combine(new[] { FindRepoRoot() }.Concat(parts).ToArray());

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
