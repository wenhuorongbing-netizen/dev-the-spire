using Xunit;

namespace EZMicroBalance.Tests;

public sealed class VakuuTemptationGuardTests
{
    [Fact]
    public void VakuuContractCardsAreHiddenTokenSkillsAndNotNormallyGenerated()
    {
        var card = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuTemptationCard.cs");
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPowers.cs");
        var exportPreset = ReadRepoText("export_presets.cfg");

        AssertSourceContains(
            card,
            "internal abstract class VakuuContractCard : CustomCardModel",
            "base(0, CardType.Skill, CardRarity.Token, TargetType.None, showInCardLibrary: false)",
            "CardKeyword.Ethereal",
            "CardKeyword.Exhaust",
            "CanBeGeneratedInCombat => false",
            "CanBeGeneratedByModifiers => false",
            "MaxUpgradeLevel => 0",
            "images/card_portraits/vakuu_temptation.png",
            "images/card_portraits/big/vakuu_temptation.png",
            "HoverTipFactory.FromPower<VakuuStolenVaultPower>()",
            "HoverTipFactory.FromPower<VakuuBloodDebtPower>()",
            "[CustomID(CardId)]",
            "[Pool(typeof(ColorlessCardPool))]",
            "public const string CardId = \"EZMB_VAKUU_KNIFE_CONTRACT\"",
            "public const string CardId = \"EZMB_VAKUU_TEMPTATION\"",
            "public const string CardId = \"EZMB_VAKUU_SHELTER_CONTRACT\"");
        AssertSourceContains(
            powers,
            "VakuuStolenVaultPower",
            "VakuuBloodDebtPower",
            "DamagePerDebt = 3",
            "props.IsPoweredAttack()");
        Assert.DoesNotContain("StatusCardPool", card, StringComparison.Ordinal);
        Assert.DoesNotContain("CardType.Status", card, StringComparison.Ordinal);
        Assert.DoesNotContain("CardKeyword.Unplayable", card, StringComparison.Ordinal);
        Assert.DoesNotContain("AfterCardExhausted", card, StringComparison.Ordinal);

        Assert.Contains("res://EZMicroBalance/images/card_portraits/vakuu_temptation.png", exportPreset, StringComparison.Ordinal);
        Assert.Contains("res://EZMicroBalance/images/card_portraits/big/vakuu_temptation.png", exportPreset, StringComparison.Ordinal);
    }

    [Fact]
    public void VakuuContractsUseSourceBackedCommandsAndSharedContractSigning()
    {
        var card = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuTemptationCard.cs");
        var patch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPatch.cs");

        AssertSourceContains(
            card,
            "VakuuFightService.SignContract(choiceContext, Owner, this, hpLoss)",
            "new DamageVar(\"Damage\", 22m, ValueProp.Move)",
            "DynamicVars.Damage.BaseValue",
            "DamageCmd.Attack",
            "Targeting(target)",
            "new IntVar(\"Energy\", 2m)",
            "new IntVar(\"Cards\", 2m)",
            "PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner)",
            "CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner)",
            "new BlockVar(\"Block\", 24m, ValueProp.Move)",
            "CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay)");
        AssertSourceContains(
            patch,
            "public static async Task SignContract",
            "CreatureCmd.Damage(",
            "ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move",
            "encounter.BloodDebt++",
            "PowerCmd.Apply<VakuuBloodDebtPower>",
            "BreakLock(choiceContext, combatState, \"contract\")",
            "Vakuu contract signed");
    }

    [Fact]
    public void VakuuFightInjectsContractsOnlyInsideCustomVakuuTrialCombat()
    {
        var mainFile = ReadRepoText("EZMicroBalanceCode", "MainFile.cs");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightRunHook.cs");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        var battlewornDummy = ReadRepoText("source code", "src", "Core", "Models", "Encounters", "BattlewornDummyEventEncounter.cs");

        Assert.Contains("VakuuFightInitializer.Initialize();", mainFile, StringComparison.Ordinal);
        AssertSourceContains(
            runHook,
            "ModHelper.SubscribeForRunStateHooks",
            "ModelDb.GetById<VakuuFightRunHook>",
            "VakuuFightFeatureGate.IsFightEnabledForRun(runState)",
            "public override bool ShouldReceiveCombatHooks => true",
            "public override Task AfterCreatureAddedToCombat",
            "public override Task AfterDamageReceived",
            "public override Task AfterPlayerTurnStart",
            "FirstContractTurn = 1",
            "ContractTurnCadence = 2",
            "typeof(VakuuKnifeContract)",
            "typeof(VakuuTemptation)",
            "typeof(VakuuShelterContract)",
            "player.Creature.CombatState is not { } combatState",
            "!IsVakuuTrialCombat(combatState)",
            "combatState.RunState.Players.Count != 1",
            "PileType.Hand.GetPile(player).Cards.Count >= CardPile.MaxCardsInHand",
            "player.RunState.Rng.CombatCardSelection.NextItem(ContractTypes)",
            "AncientCardHelpers.TryAddGeneratedCardToCombat",
            "PileType.Hand",
            "Vakuu fight added a Contract to hand");
        AssertSourceContains(
            runHook,
            "private static bool IsVakuuTrialCombat(ICombatState combatState) =>",
            "combatState.Encounter is EzmbVakuuTrialEncounter");
        AssertSourceContains(
            encounter,
            "base(RoomType.Monster, autoAdd: false)",
            "ShouldGiveRewards => false",
            "MaxLocks = 3",
            "DamageLockThreshold = 40",
            "GoldPerBrokenLock = 50",
            "VictoryChoiceCount => Math.Clamp(BrokenLocks + 1, 1, MaxLocks)",
            "VictoryGold => BrokenLocks * GoldPerBrokenLock",
            "CustomScenePath => VakuuFightAssetPaths.EncounterScene",
            "Slots => [VakuuSlot]",
            "ModelDb.Monster<EzmbVakuuTrialMonster>()");
        AssertSourceContains(
            battlewornDummy,
            "public override RoomType RoomType => RoomType.Monster",
            "public override bool ShouldGiveRewards => false");
        AssertSourceContains(
            gate,
            "EnableEnvironmentVariable = \"EZMB_ENABLE_VAKUU_FIGHT\"",
            "SpirePlusEnableEnvironmentVariable = \"SPIREPLUS_ENABLE_VAKUU_FIGHT\"",
            "ShouldEnableFight",
            "ShouldForceFight ||",
            "runState.Players.Count == 1");
    }

    [Fact]
    public void VakuuContractTimingTextMatchesCoreDrawThenAfterPlayerTurnStartOrder()
    {
        var combatManager = ReadRepoText("source code", "src", "Core", "Combat", "CombatManager.cs");
        var setupPlayerTurn = SliceBetween(
            combatManager,
            "private async Task SetupPlayerTurn",
            "public void SetReadyToEndTurn");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightRunHook.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var apiResearch = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");

        AssertSourceContains(
            setupPlayerTurn,
            "await Hook.BeforeHandDraw(state, player, playerChoiceContext)",
            "await CardPileCmd.Draw(playerChoiceContext, handDraw, player, fromHandDraw: true)",
            "await Hook.AfterPlayerTurnStart(state, playerChoiceContext, player)");
        AssertBefore(
            setupPlayerTurn,
            "await CardPileCmd.Draw(playerChoiceContext, handDraw, player, fromHandDraw: true)",
            "await Hook.AfterPlayerTurnStart(state, playerChoiceContext, player)");
        AssertSourceContains(
            runHook,
            "public override Task AfterPlayerTurnStart",
            "VakuuContractService.AfterPlayerTurnStart(choiceContext, player)",
            "PileType.Hand");
        Assert.Contains("after your hand is drawn", engAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"], StringComparison.OrdinalIgnoreCase);
        Assert.Contains("抽完起始手牌后", zhsAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"], StringComparison.Ordinal);
        AssertSourceContains(
            apiResearch,
            "CombatManager.cs",
            "calls `Hook.AfterPlayerTurnStart(...)` after normal hand draw");
    }

    [Fact]
    public void VakuuContractLocalizationIsBilingualReadableAndWarnsAboutRiskReward()
    {
        var engCards = JsonStringMap("EZMicroBalance", "localization", "eng", "cards.json");
        var zhsCards = JsonStringMap("EZMicroBalance", "localization", "zhs", "cards.json");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsPowers = JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json");

        AssertLocalizedCard(engCards, "EZMB_VAKUU_KNIFE_CONTRACT", "Knife Contract", "Deal {Damage:diff()} damage to Vakuu", "If a [gold]Stolen Lock[/gold] remains", "[gold]Blood Debt[/gold]");
        AssertLocalizedCard(engCards, "EZMB_VAKUU_TEMPTATION", "Gold Contract", "Gain {Energy:energyIcons()}", "Draw {Cards:diff()} cards", "[gold]Stolen Lock[/gold]");
        AssertLocalizedCard(engCards, "EZMB_VAKUU_SHELTER_CONTRACT", "Shelter Contract", "Gain {Block:diff()} [gold]Block[/gold]", "[gold]Blood Debt[/gold]");
        AssertLocalizedCard(zhsCards, "EZMB_VAKUU_KNIFE_CONTRACT", "刀契", "对瓦库造成{Damage:diff()}点伤害", "[gold]赃物锁[/gold]", "[gold]血债[/gold]");
        AssertLocalizedCard(zhsCards, "EZMB_VAKUU_TEMPTATION", "金契", "获得{Energy:energyIcons()}", "抽{Cards:diff()}张牌", "[gold]赃物锁[/gold]");
        AssertLocalizedCard(zhsCards, "EZMB_VAKUU_SHELTER_CONTRACT", "避债契", "获得{Block:diff()}点[gold]格挡[/gold]", "[gold]血债[/gold]");

        AssertSourceContains(
            engAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "Fight Vakuu",
            "after your hand is drawn",
            "turns [blue]1[/blue], [blue]3[/blue], [blue]5[/blue]",
            "random [gold]Contract[/gold]",
            "while any remain",
            "[gold]Stolen Locks[/gold]",
            "[gold]Blood Debt[/gold]",
            "[blue]40[/blue] unblocked damage",
            "[blue]50[/blue] [gold]Gold[/gold]",
            "Death ends the run");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "Fight Vakuu",
            "after your hand is drawn",
            "random [gold]Contract[/gold]",
            "while any remain",
            "[gold]Stolen Locks[/gold]",
            "[gold]Blood Debt[/gold]",
            "No normal combat rewards",
            "[blue]50[/blue] [gold]Gold[/gold]",
            "Death ends the run");
        AssertSourceContains(
            zhsAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "与瓦库战斗",
            "抽完起始手牌后",
            "本场没有普通战斗奖励",
            "死亡会结束本局",
            "[blue]1[/blue]",
            "[blue]3[/blue]",
            "[blue]5[/blue]",
            "随机[gold]契约[/gold]",
            "[gold]赃物锁[/gold]",
            "[gold]血债[/gold]",
            "[blue]50[/blue][gold]金币[/gold]");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "与瓦库战斗",
            "抽完起始手牌后",
            "本场没有普通战斗奖励",
            "死亡会结束本局",
            "随机[gold]契约[/gold]",
            "[gold]赃物锁[/gold]",
            "[gold]血债[/gold]",
            "[blue]50[/blue][gold]金币[/gold]");
        AssertSourceContains(
            engPowers["EZMICROBALANCE-VAKUU_STOLEN_VAULT_POWER.description"],
            "[gold]Stolen Vault[/gold]",
            "[blue]40[/blue] unblocked damage",
            "[blue]50[/blue] [gold]Gold[/gold]");
        AssertSourceContains(
            engPowers["EZMICROBALANCE-VAKUU_BLOOD_DEBT_POWER.description"],
            "[gold]Blood Debt[/gold]",
            "[blue]3[/blue] more damage");
        AssertSourceContains(
            zhsPowers["EZMICROBALANCE-VAKUU_STOLEN_VAULT_POWER.description"],
            "[gold]赃物库[/gold]",
            "[blue]40[/blue]点未被格挡伤害",
            "[blue]50[/blue][gold]金币[/gold]");
        AssertSourceContains(
            zhsPowers["EZMICROBALANCE-VAKUU_BLOOD_DEBT_POWER.description"],
            "[gold]血债[/gold]",
            "[blue]3[/blue]点");

        foreach (var value in new[]
        {
            engCards["EZMB_VAKUU_KNIFE_CONTRACT.description"],
            engCards["EZMB_VAKUU_TEMPTATION.description"],
            engCards["EZMB_VAKUU_SHELTER_CONTRACT.description"],
            zhsCards["EZMB_VAKUU_KNIFE_CONTRACT.description"],
            zhsCards["EZMB_VAKUU_TEMPTATION.description"],
            zhsCards["EZMB_VAKUU_SHELTER_CONTRACT.description"],
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
            Assert.DoesNotContain("top of your [gold]Draw Pile[/gold]", value, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ActiveDocsDescribeContractsAsSourceBackedWhileRuntimeClaimsRemainPending()
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
            Assert.DoesNotContain("top of your [gold]Draw Pile[/gold]", doc, StringComparison.Ordinal);
        }

        var joined = string.Join(Environment.NewLine, docs);
        Assert.Contains("Contract", joined, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Stolen", joined, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Blood Debt", joined, StringComparison.OrdinalIgnoreCase);
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
}
