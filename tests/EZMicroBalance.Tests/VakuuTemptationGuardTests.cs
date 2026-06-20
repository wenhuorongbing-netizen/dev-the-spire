using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class VakuuTemptationGuardTests
{
    [Fact]
    public void VakuuContractCardsAreHiddenTokenSkillsAndNotNormallyGenerated()
    {
        var card = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuTemptationCard.cs");
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightPowers.cs");
        var ritsuRegistration = ReadRepoText("EZMicroBalanceCode", "Core", "Integrations", "RitsuLib", "SpirePlusContentRegistrationService.cs");
        var exportPreset = ReadRepoText("export_presets.cfg");

        AssertSourceContains(
            card,
            "internal abstract class VakuuContractCard : ModCardTemplate",
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
            "public const string CardId = \"EZMB_VAKUU_KNIFE_CONTRACT\"",
            "public const string CardId = \"EZMB_VAKUU_TEMPTATION\"",
            "public const string CardId = \"EZMB_VAKUU_SHELTER_CONTRACT\"",
            "public const string CardId = \"EZMB_VAKUU_TRICK_CONTRACT\"",
            "public const string CardId = \"EZMB_VAKUU_CASH_OUT_CONTRACT\"");
        AssertSourceContains(
            ritsuRegistration,
            "content.Card<ColorlessCardPool, VakuuKnifeContract>(FullEntry(VakuuKnifeContract.CardId));",
            "content.Card<ColorlessCardPool, VakuuTemptation>(FullEntry(VakuuTemptation.CardId));",
            "content.Card<ColorlessCardPool, VakuuShelterContract>(FullEntry(VakuuShelterContract.CardId));",
            "content.Card<ColorlessCardPool, VakuuTrickContract>(FullEntry(VakuuTrickContract.CardId));",
            "content.Card<ColorlessCardPool, VakuuCashOutContract>(FullEntry(VakuuCashOutContract.CardId));");
        Assert.DoesNotContain("[CustomID(CardId)]", card, StringComparison.Ordinal);
        Assert.DoesNotContain("[Pool(typeof(ColorlessCardPool))]", card, StringComparison.Ordinal);
        AssertSourceContains(
            powers,
            "VakuuStolenVaultPower",
            "VakuuBloodDebtPower",
            "VakuuBacklashPower",
            "DamagePerDebt = 2",
            "props.IsPoweredAttack()",
            "AfterSideTurnEnd");
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
        var bloodDebt = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.BloodDebt.cs");
        var contracts = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.Contracts.cs");
        var lockBreaks = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.LockBreaks.cs");
        var stolenVault = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.StolenVault.cs");
        var vakuuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu");

        AssertSourceContains(
            card,
            "VakuuFightService.SignContract(choiceContext, Owner, this, hpLoss)",
            "new DamageVar(\"Damage\", 24m, ValueProp.Move)",
            "DynamicVars.Damage.BaseValue",
            "DamageCmd.Attack",
            "Targeting(target)",
            "new IntVar(\"Energy\", 2m)",
            "new IntVar(\"Cards\", 2m)",
            "PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner)",
            "CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner)",
            "new BlockVar(\"Block\", 22m, ValueProp.Move)",
            "CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay)",
            "VakuuFightService.ReduceBloodDebt",
            "VakuuFightService.BreakLockFromContract",
            "VakuuFightService.CashOut");
        AssertSourceContains(
            vakuuSource,
            "public static async Task SignContract",
            "CreatureCmd.Damage(",
            "ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move",
            "AddBloodDebt",
            "PowerCmd.Apply<VakuuBloodDebtPower>",
            "BreakLock(choiceContext, combatState, \"contract\")",
            "Vakuu contract signed",
            "OfferCashOutAfterLockBreak",
            "CreatureCmd.Kill(vakuu, force: true)");
        AssertSourceContains(
            contracts,
            "public static async Task SignContract",
            "public static async Task BreakLockFromContract",
            "public static async Task CashOut");
        AssertSourceContains(
            bloodDebt,
            "public static async Task ReduceBloodDebt",
            "private static async Task AddBloodDebt",
            "PowerCmd.Apply<VakuuBloodDebtPower>");
        AssertSourceContains(
            lockBreaks,
            "public static async Task AfterDamageGiven",
            "Core skips AfterDamageReceived for lethal hits",
            "private static async Task BreakLock",
            "OfferCashOutAfterLockBreak");
        AssertSourceContains(
            stolenVault,
            "public static async Task EnsureStolenVaultPower",
            "PowerCmd.Apply<VakuuStolenVaultPower>",
            "PowerCmd.ModifyAmount",
            "PowerCmd.Remove(vault)");
        AssertRepoPathDoesNotExist("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightService.CombatState.cs");
    }

    [Fact]
    public void VakuuFightInjectsContractsOnlyInsideCustomVakuuTrialCombat()
    {
        var featureRegistry = ReadRepoText("EZMicroBalanceCode", "Core", "Features", "SpirePlusFeatureRegistry.cs");
        var runHook = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu");
        var vakuuSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu");
        var encounter = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightEncounter.cs");
        var gate = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuFightFeatureGate.cs");
        var monster = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu", "VakuuTrialMonster.cs");

        Assert.Contains("VakuuFightFeatureModule", featureRegistry, StringComparison.Ordinal);
        AssertSourceContains(
            runHook,
            "ModHelper.SubscribeForCombatStateHooks",
            "ModelDb.GetById<VakuuFightCombatHook>",
            "VakuuFightFeatureGate.IsFightEnabledForRun(combatState.RunState)",
            "public override bool ShouldReceiveCombatHooks => true",
            "internal sealed class VakuuFightCombatHook",
            "public override Task AfterCreatureAddedToCombat",
            "public override Task AfterDamageGiven",
            "public override Task AfterPlayerTurnStart",
            "FirstContractTurn = 1",
            "ContractTurnCadence = 2",
            "LastContractOfferTurn = 5",
            "ContractOfferCount = 3",
            "typeof(VakuuKnifeContract)",
            "typeof(VakuuTemptation)",
            "typeof(VakuuShelterContract)",
            "typeof(VakuuTrickContract)",
            "player.Creature.CombatState is not { } combatState",
            "!IsVakuuTrialCombat(combatState)",
            "combatState.RunState.Players.Count != 1",
            "PileType.Hand.GetPile(player).Cards.Count >= CardPile.MaxCardsInHand",
            "CardSelectCmd.FromSimpleGrid",
            "EZMB_VAKUU_CASH_OUT.selectionScreenPrompt",
            "OfferImmediateCashOutChoice",
            "UnstableShuffle(player.RunState.Rng.CombatCardSelection)",
            "AncientCardHelpers.TryAddGeneratedCardToCombat",
            "PileType.Hand",
            "Vakuu fight added a chosen Contract to hand",
            "OfferCashOutAfterLockBreak");
        Assert.DoesNotContain("ModHelper.SubscribeForRunStateHooks", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("internal sealed class VakuuFightRunHook", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("ModelDb.GetById<VakuuFightRunHook>", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            runHook,
            "private static bool IsVakuuTrialCombat(ICombatState combatState) =>",
            "combatState.Encounter is EzmbVakuuTrialEncounter");
        AssertSourceContains(
            encounter,
            "public override RoomType RoomType => RoomType.Monster",
            "ShouldGiveRewards => false",
            "MaxLocks = 3",
            "DamageLockThreshold = 40",
            "GoldPerBrokenLock = 50",
            "GoldCostPerBloodDebt = 15",
            "HpLossPerDebtShortfall = 3",
            "VictoryChoiceCount => Math.Clamp(BrokenLocks + 1, 1, MaxLocks)",
            "VictoryLootGold => BrokenLocks * GoldPerBrokenLock",
            "VictoryGold => Math.Max(0m, VictoryLootGold - BloodDebtGoldCost)",
            "BloodDebtShortfall => Math.Max(0m, BloodDebtGoldCost - VictoryLootGold)",
            "CustomEncounterScenePath => VakuuFightAssetPaths.EncounterScene",
            "HasScene => true",
            "Slots => [VakuuSlot]",
            "ModelDb.Monster<EzmbVakuuTrialMonster>()");
        AssertSourceContains(
            vakuuSource,
            "public static async Task EnsureStolenVaultPower(Creature creature)",
            "public static async Task AfterDamageGiven",
            "Core skips AfterDamageReceived for lethal hits",
            "PowerCmd.Apply<VakuuStolenVaultPower>",
            "PowerCmd.ModifyAmount",
            "PowerCmd.Remove(vault)");
        AssertSourceContains(
            monster,
            "public override async Task AfterAddedToRoom()",
            "VakuuFightService.EnsureStolenVaultPower(Creature)");
        AssertSourceContains(
            gate,
            "EnableEnvironmentVariable = \"SPIREPLUS_ENABLE_VAKUU_FIGHT\"",
            "LegacyEnableEnvironmentVariable = \"EZMB_ENABLE_VAKUU_FIGHT\"",
            "ShouldEnableFight",
            "ShouldForceFight ||",
            "runState.Players.Count == 1");
    }

    [Fact]
    public void VakuuContractTimingTextMatchesAfterPlayerTurnStartHook()
    {
        var vakuuHookSource = ReadSourceTree("EZMicroBalanceCode", "Ancients", "Expansion", "Vakuu");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var apiResearch = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "api-research.md");

        AssertSourceContains(
            vakuuHookSource,
            "public override Task AfterPlayerTurnStart",
            "VakuuContractService.AfterPlayerTurnStart(choiceContext, player)",
            "PileType.Hand");
        Assert.Contains("greed trial", engAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"], StringComparison.OrdinalIgnoreCase);
        Assert.Contains("赃物试炼", zhsAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"], StringComparison.Ordinal);
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
        AssertLocalizedCard(engCards, "EZMB_VAKUU_TEMPTATION", "Gold Contract", "Gain {Energy:energyIcons()}", "draw {Cards:diff()} cards", "[gold]Stolen Lock[/gold]");
        AssertLocalizedCard(engCards, "EZMB_VAKUU_SHELTER_CONTRACT", "Shelter Contract", "Gain {Block:diff()} [gold]Block[/gold]", "Remove {Debt:diff()} [gold]Blood Debt[/gold]");
        AssertLocalizedCard(engCards, "EZMB_VAKUU_TRICK_CONTRACT", "Fraud Contract", "Break [blue]1[/blue] [gold]Stolen Lock[/gold]", "Add {Debt:diff()} [gold]Blood Debt[/gold]", "Vakuu's attacks deal {Backlash:diff()} more damage");
        AssertLocalizedCard(engCards, "EZMB_VAKUU_CASH_OUT_CONTRACT", "Cash Out", "End the Vakuu fight", "take the loot from broken locks");
        AssertLocalizedCard(zhsCards, "EZMB_VAKUU_KNIFE_CONTRACT", "刀契", "对瓦库造成{Damage:diff()}点伤害", "[gold]赃物锁[/gold]", "[gold]血债[/gold]");
        AssertLocalizedCard(zhsCards, "EZMB_VAKUU_TEMPTATION", "金契", "获得{Energy:energyIcons()}", "抽{Cards:diff()}张牌", "[gold]赃物锁[/gold]");
        AssertLocalizedCard(zhsCards, "EZMB_VAKUU_SHELTER_CONTRACT", "避债契", "获得{Block:diff()}点[gold]格挡[/gold]", "移除{Debt:diff()}层[gold]血债[/gold]");
        AssertLocalizedCard(zhsCards, "EZMB_VAKUU_TRICK_CONTRACT", "诈契", "打破[blue]1[/blue]把[gold]赃物锁[/gold]", "增加{Debt:diff()}层[gold]血债[/gold]");
        AssertLocalizedCard(zhsCards, "EZMB_VAKUU_CASH_OUT_CONTRACT", "收手契", "结束瓦库战斗", "带走已破锁的赃物");

        AssertSourceContains(
            engAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "Fight Vakuu",
            "greed trial",
            "[gold]Stolen Locks[/gold]",
            "loot Gold",
            "extra blessing choices",
            "[gold]Blood Debt[/gold]",
            "cash out",
            "Death ends the run");
        AssertSourceContains(
            engRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "Fight Vakuu",
            "greed trial",
            "[gold]Stolen Locks[/gold]",
            "extra blessing choices",
            "[gold]Blood Debt[/gold]",
            "cash out",
            "No normal combat rewards",
            "Death ends the run");
        AssertSourceContains(
            zhsAncients["VAKUU.pages.INITIAL.options.ezmb_vakuu_fight.description"],
            "与瓦库进行赃物试炼",
            "额外祝福选择",
            "本场没有普通战斗奖励",
            "若死亡会直接结束本局",
            "[gold]契约[/gold]",
            "[gold]赃物锁[/gold]",
            "[gold]血债[/gold]");
        AssertSourceContains(
            zhsRelics["EZMICROBALANCE-VAKUU_FIGHT_OPTION_RELIC.description"],
            "与瓦库进行赃物试炼",
            "额外祝福选择",
            "本场没有普通战斗奖励",
            "若死亡会直接结束本局",
            "[gold]契约[/gold]",
            "[gold]赃物锁[/gold]",
            "[gold]血债[/gold]");
        AssertSourceContains(
            engPowers["EZMICROBALANCE-VAKUU_STOLEN_VAULT_POWER.description"],
            "[gold]Stolen Vault[/gold]",
            "[blue]40[/blue] unblocked damage",
            "[gold]Cash Out[/gold]");
        AssertSourceContains(
            engPowers["EZMICROBALANCE-VAKUU_BLOOD_DEBT_POWER.description"],
            "[gold]Blood Debt[/gold]",
            "[blue]2[/blue] more damage",
            "[blue]15[/blue] loot Gold");
        AssertSourceContains(
            zhsPowers["EZMICROBALANCE-VAKUU_STOLEN_VAULT_POWER.description"],
            "[gold]赃物库[/gold]",
            "[blue]40[/blue]点未被格挡伤害",
            "[gold]收手[/gold]");
        AssertSourceContains(
            zhsPowers["EZMICROBALANCE-VAKUU_BLOOD_DEBT_POWER.description"],
            "[gold]血债[/gold]",
            "[blue]2[/blue]点",
            "[blue]15[/blue]赃物金币");

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
