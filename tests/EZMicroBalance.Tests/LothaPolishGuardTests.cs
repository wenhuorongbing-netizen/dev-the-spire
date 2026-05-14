using System.Text;
using System.Text.Json;
using Xunit;

namespace EZMicroBalance.Tests;

public sealed class LothaPolishGuardTests
{
    private static readonly (string AncientKey, string RelicKey)[] LothaDescriptionKeys =
    [
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description", "EZMICROBALANCE-LOTHA_MIRROR_REBUTTAL_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description", "EZMICROBALANCE-LOTHA_MIRROR_HALL_ECHO_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_presumption.description", "EZMICROBALANCE-LOTHA_PRESUMPTION_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.description", "EZMICROBALANCE-LOTHA_CLOSED_COURT_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description", "EZMICROBALANCE-LOTHA_DEFERRED_VERDICT_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.description", "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description", "EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_OPTION_RELIC.description"),
        ("EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description", "EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.description")
    ];

    [Fact]
    public void MirrorRebuttalUsesChosenDeckCardAndPowerTwoTwoReplacement()
    {
        var ancient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAncient.cs");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var savedFields = ReadRepoText("EZMicroBalanceCode", "Ancients", "Common", "AncientSavedStateFields.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");

        AssertSourceContains(
            ancient,
            "CardSelectCmd.FromDeckGeneric",
            "EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.selectionScreenPrompt",
            "LothaBlessingService.IsMirrorRebuttalDeckCardCandidate",
            "LothaBlessingService.MarkMirrorRebuttalCard",
            "HoverTipFactory.Static(StaticHoverTip.ReplayStatic)",
            "HoverTipFactory.Static(StaticHoverTip.Energy)");
        AssertSourceContains(
            savedFields,
            "SavedSpireField<CardModel, bool> LothaMirrorRebuttalCard",
            "\"EZMicroBalanceLothaMirrorRebuttalCard\"");
        AssertSourceContains(
            runHook,
            "MirrorRebuttalPowerFallbackEnergy = 2",
            "MirrorRebuttalPowerFallbackCards = 2",
            "TryMoveMirrorRebuttalCardToHand",
            "CardPileCmd.Add(selectedCard, PileType.Hand)",
            "PileType.Hand.GetPile(player).Cards.Count >= CardPile.MaxCardsInHand",
            "CardPileCmd.Add(selectedCard, PileType.Draw, CardPilePosition.Top)",
            "could not move selected card",
            "addResult.cardAdded.Pile?.Type == PileType.Hand",
            "IsMirrorRebuttalCombatCard",
            "AncientSavedStateFields.LothaMirrorRebuttalCard[deckCard]",
            "TryResolveMirrorRebuttalPowerFallback",
            "IsPowerReplacementCostZeroCard",
            "PowerReplacementCardPendingBenefit",
            "ApplyPowerReplacementBenefit(",
            "MirrorRebuttalPowerFallbackEnergy",
            "MirrorRebuttalPowerFallbackCards");

        Assert.DoesNotContain("MirrorRebuttalMinimumBlock", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("MirrorRebuttalArmed", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("after unblocked damage", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"], StringComparison.OrdinalIgnoreCase);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"],
            "Choose [blue]1[/blue] [gold]Attack[/gold], [gold]Skill[/gold], or [gold]Power[/gold] from your deck",
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "costs [blue]0[/blue]",
            "gives [blue]2[/blue] [gold]Energy[/gold]",
            "draws [blue]2[/blue]");
    }

    [Fact]
    public void MirrorHallEchoRecordsLastTurnTypeAndRejectsCopyCardPlaceholder()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");

        AssertSourceContains(
            runHook,
            "public override Task AfterTurnEnd",
            "MirrorHallEchoRecordedType",
            "MirrorHallEchoArmedType",
            "MirrorHallEchoExtraPlayCount = 1",
            "RecordMirrorHallEchoType",
            "CombatManager.Instance.History.CardPlaysFinished",
            "!entry.CardPlay.IsAutoPlay",
            "!entry.CardPlay.Card.IsClone",
            "entry.HappenedThisTurn(player.Creature.CombatState)",
            "TryResolveMirrorHallEchoPowerFallback",
            "IsPowerReplacementCostZeroCard",
            "Lotha Mirror Hall Echo used the Power-card replacement benefit: cost 0 and draw 1");

        Assert.DoesNotContain("TryCreateMirrorHallEcho", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("copy.EnergyCost", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("CardKeyword.Ethereal", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("CardKeyword.Exhaust", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("creates a [blue]0[/blue]-cost", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description"], StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("铏氭棤", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description"], StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description"],
            "At turn end",
            "remember the last",
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "play [blue]1[/blue] extra time",
            "cost [blue]0[/blue]",
            "draw [blue]1[/blue]");
        Assert.DoesNotContain("gain [blue]1[/blue] [gold]Energy[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_hall_echo.description"], StringComparison.Ordinal);
    }

    [Fact]
    public void PowerReplacementEligibilityUsesActualPlayedPowerNotHandOrderOrPendingPreview()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");

        Assert.DoesNotContain("IsCurrentEligiblePowerInHand", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("FirstOrDefault(IsPowerCard)", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("!ReferenceEquals(combatState.PowerReplacementCardPendingBenefit, cardPlay.Card)", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            runHook,
            "CanUseMirrorRebuttalPowerReplacement(cardPlay.Card, combatState)",
            "CanUseMirrorHallEchoPowerReplacement(cardPlay.Card, combatState)",
            "CanUseDeferredVerdictPowerReplacement(cardPlay.Card, player, combatState)",
            "CanUseSingleSentencePowerReplacement(cardPlay.Card, combatState)",
            "CanUseMirrorHallEchoPowerReplacement(card, combatState)",
            "CanUseDeferredVerdictPowerReplacement(card, player, combatState)",
            "CanUseSingleSentencePowerReplacement(card, combatState)",
            "combatState.PowerReplacementCardPendingBenefit = card",
            "combatState.PowerReplacementCardPendingBenefit = null");
    }

    [Fact]
    public void PresumptionUsesPersistentInnocentStateAndEnemyAttackDamageBreak()
    {
        var ancient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAncient.cs");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<LothaPresumptionPower>()",
            "HoverTipFactory.Static(StaticHoverTip.Energy)",
            "HoverTipFactory.Static(StaticHoverTip.Block)");
        AssertSourceContains(
            powers,
            "internal sealed class LothaPresumptionPower",
            "PowerType.Buff",
            "PowerStackType.Single");
        AssertSourceContains(
            runHook,
            "PresumptionCards = 2",
            "PresumptionEnergy = 1",
            "PresumptionBlock = 8",
            "PresumptionHpLoss = 8",
            "BeforeCombatStart",
            "PowerCmd.Apply<LothaPresumptionPower>",
            "CardPileCmd.Draw(choiceContext, PresumptionCards, player)",
            "PlayerCmd.GainEnergy(PresumptionEnergy, player)",
            "CreatureCmd.GainBlock(player.Creature, PresumptionBlock",
            "public override Task AfterDamageReceived",
            "IsUnblockedEnemyAttackDamage",
            "result.UnblockedDamage > 0",
            "dealer is { IsEnemy: true }",
            "cardSource == null",
            "props.HasFlag(ValueProp.Move)",
            "PowerCmd.Remove<LothaPresumptionPower>",
            "CreatureCmd.Damage(");

        Assert.DoesNotContain("PresumptionUsed", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("At the start of your first turn each combat, gain [blue]8[/blue] [gold]Block[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_presumption.description"], StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_presumption.description"],
            "[gold]Innocent[/gold]",
            "draw [blue]2[/blue] cards",
            "gain [blue]1[/blue] [gold]Energy[/gold]",
            "gain [blue]8[/blue] [gold]Block[/gold]",
            "unblocked enemy [gold]Attack[/gold] damage",
            "lose [blue]8[/blue] HP");
    }

    [Fact]
    public void ClosedCourtSuppressesOnlyCombatCardRewardsAndUsesFirstTurnFourEnergyPlan()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");

        AssertSourceContains(
            runHook,
            "ClosedCourtEnergy = 4",
            "ClosedCourtDiscountCount = 3",
            "CardPile.MaxCardsInHand",
            "PlayerCmd.GainEnergy(ClosedCourtEnergy, player)",
            "ClosedCourtDiscountActiveThisTurn = true",
            "TryModifyRewardsLate",
            "room is not CombatRoom",
            "rewards.RemoveAll(reward => reward is CardReward)",
            "gold, potion, and relic rewards remain",
            "TryModifyEnergyCostInCombat",
            "Math.Max(0, originalCost - 1)",
            "TrackClosedCourtDiscountUse");

        Assert.DoesNotContain("ClosedCourtEnergy = 1", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("ClosedCourtCards = 2", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("reward is GoldReward", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("reward is PotionReward", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("reward is RelicReward", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("gain [blue]1[/blue] [gold]Energy[/gold] and draw [blue]2[/blue] cards", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.description"], StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_closed_court.description"],
            "post-combat card rewards no longer appear",
            "[gold]Gold[/gold], potions, and relics remain",
            "draw until your hand has [blue]10[/blue] cards",
            "gain [blue]4[/blue] [gold]Energy[/gold]",
            "first [blue]3[/blue] player-played cards");
    }

    [Fact]
    public void DeferredVerdictUsesTurnFourStacksAndDoesNotLeakPower()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");

        AssertSourceContains(
            runHook,
            "DeferredVerdictTurn = 4",
            "DeferredVerdictStacks = 3",
            "DeferredVerdictEnergy = 4",
            "DeferredVerdictCards = 4",
            "DeferredVerdictExtraPlayCount = 1",
            "DeferredVerdictEarlyEndHeal = 4",
            "PowerFallbackCards = 1",
            "IsPowerReplacementCostZeroCard",
            "combatState.DeferredVerdictActiveThisTurn = true",
            "PowerCmd.Apply<LothaVerdictPower>",
            "PowerCmd.Decrement(verdict)",
            "PowerCmd.Remove<LothaVerdictPower>(player.Creature)",
            "CreatureCmd.Heal(player.Creature, DeferredVerdictEarlyEndHeal");
        AssertSourceContains(
            powers,
            "internal sealed class LothaVerdictPower",
            "PowerType.Buff",
            "PowerStackType.Counter");

        Assert.DoesNotContain("DeferredVerdictReadyThisTurn", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("TryBurstDeferredVerdict", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("DeferredVerdictDamagePerStack", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description"],
            "draw [blue]4[/blue] cards",
            "gain [blue]4[/blue] [gold]Energy[/gold]",
            "gain [blue]3[/blue] [gold]Verdict[/gold]",
            "each next non-Status card consumes [blue]1[/blue] [gold]Verdict[/gold]",
            "[gold]Power[/gold] cards cost [blue]0[/blue]",
            "draw [blue]1[/blue]",
            "heal [blue]4[/blue] HP");
        Assert.DoesNotContain("[gold]Power[/gold] cards instead gain [blue]1[/blue] [gold]Energy[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("PowerFallbackEnergy = 1", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("private const int PowerFallbackEnergy", runHook, StringComparison.Ordinal);
    }

    [Fact]
    public void DeathReprieveIsNotTwentyFivePercentHealOnlyPlaceholder()
    {
        var ancient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAncient.cs");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<LothaDeathReprievePower>()");
        AssertSourceContains(
            powers,
            "internal sealed class LothaDeathReprievePower",
            "PowerType.Buff",
            "PowerStackType.Single");
        AssertSourceContains(
            runHook,
            "DeathReprieveCards = 10",
            "DeathReprieveEnergy = 10",
            "ShouldDieLate(Creature creature)",
            "ShouldDie(Creature creature)",
            "AfterPreventingDeath(Creature creature)",
            "CreatureCmd.SetCurrentHp(creature, 1m)",
            "StartDeathReprieveTurn",
            "ResolveDeathReprieveTurnEnd",
            "TryModifyEnergyCostInCombat",
            "TryModifyStarCost",
            "modifiedCost = 0",
            "CardPileCmd.Draw(choiceContext, DeathReprieveCards, player)",
            "PlayerCmd.GainEnergy(DeathReprieveEnergy, player)",
            "CreatureCmd.Kill(player.Creature, force: true)");

        Assert.DoesNotContain("DeathReprieveHealPercent", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("25%", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("25%", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.description"], StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_death_reprieve.description"],
            "set HP to [blue]1[/blue]",
            "current player turn",
            "next player turn if this triggers outside your turn",
            "draw [blue]10[/blue] cards",
            "gain [blue]10[/blue] [gold]Energy[/gold]",
            "cards cost [blue]0[/blue]",
            "if any enemies remain, die");
    }

    [Fact]
    public void SingleSentenceCapIgnoresAutoplayClonesAndExtraExecutions()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");

        AssertSourceContains(
            runHook,
            "LothaExtraPlayCount = 2",
            "SingleSentenceRemainingPlayLimit = 4",
            "TryResolveSingleSentencePowerFallback",
            "SingleSentencePowerFallbackUsedThisTurn",
            "IsPowerReplacementCostZeroCard",
            "Lotha Single Sentence Power fallback cost 0, drew 1 card, and did not consume the sentence",
            "autoPlayType == AutoPlayType.None",
            "combatState.AutoPlayCardPendingModifier = card",
            "return true;",
            "TrackSingleSentenceRemainingPlays",
            "!cardPlay.IsFirstInSeries",
            "cardPlay.IsAutoPlay",
            "cardPlay.Card.IsClone",
            "ReferenceEquals(cardPlay.Card, combatState.SingleSentenceRulingCard)");

        Assert.DoesNotContain("TryAddGeneratedCardToCombat(copy", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("CardCmd.AutoPlay", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"],
            "first [gold]Attack[/gold] or [gold]Skill[/gold]",
            "plays [blue]2[/blue] extra times",
            "only [blue]4[/blue] more cards",
            "first card before the sentence is a [gold]Power[/gold]",
            "costs [blue]0[/blue] and draws [blue]1[/blue] instead");
        Assert.DoesNotContain("[gold]Power[/gold] instead grants [blue]1[/blue] [gold]Energy[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("gain [blue]10[/blue] [gold]Block[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("exactly one card", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SingleSentenceBranchesAreGuardedBeforeAndAfterTheRuling()
    {
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");

        var shouldPlay = SourceSlice(runHook, "public static bool ShouldPlay", "public static async Task AfterCardPlayed");
        var powerFallback = SourceSlice(runHook, "private static async Task TryResolveSingleSentencePowerFallback", "private static void TrackSingleSentenceRemainingPlays");
        var playTracker = SourceSlice(runHook, "private static void TrackSingleSentenceRemainingPlays", "private static void TrackClosedCourtDiscountUse");
        var powerEligibility = SourceSlice(runHook, "private static bool CanUseSingleSentencePowerReplacement", "private static bool IsEligibleCard");
        var eligibleCard = SourceSlice(runHook, "private static bool IsEligibleCard", "private static bool IsPowerCard");
        var powerCard = SourceSlice(runHook, "private static bool IsPowerCard", "private static bool IsDeferredVerdictConsumerCard");

        AssertSourceContains(
            powerFallback,
            "combatState.SingleSentenceUsedThisTurn ||",
            "combatState.SingleSentencePowerFallbackUsedThisTurn ||",
            "!cardPlay.IsFirstInSeries ||",
            "cardPlay.IsAutoPlay ||",
            "!CanUseSingleSentencePowerReplacement(cardPlay.Card, combatState)",
            "combatState.SingleSentencePowerFallbackUsedThisTurn = true",
            "await ApplyPowerReplacementBenefit(choiceContext, cardPlay.Card.Owner)");
        AssertBefore(powerFallback, "combatState.SingleSentenceUsedThisTurn ||", "await ApplyPowerReplacementBenefit(choiceContext, cardPlay.Card.Owner)");

        AssertSourceContains(
            powerEligibility,
            "!combatState.SingleSentenceUsedThisTurn",
            "!combatState.SingleSentencePowerFallbackUsedThisTurn",
            "IsPowerCard(card)");
        Assert.DoesNotContain("CardType.Attack", powerEligibility, StringComparison.Ordinal);
        Assert.DoesNotContain("CardType.Skill", powerEligibility, StringComparison.Ordinal);

        AssertSourceContains(
            eligibleCard,
            "card.Type is CardType.Attack or CardType.Skill",
            "!card.IsClone");
        Assert.DoesNotContain("CardType.Power", eligibleCard, StringComparison.Ordinal);
        Assert.DoesNotContain("CardType.Status", eligibleCard, StringComparison.Ordinal);
        Assert.DoesNotContain("CardType.Curse", eligibleCard, StringComparison.Ordinal);

        AssertSourceContains(
            powerCard,
            "card.Type == CardType.Power",
            "!card.IsClone");
        Assert.DoesNotContain("CardType.Attack", powerCard, StringComparison.Ordinal);
        Assert.DoesNotContain("CardType.Skill", powerCard, StringComparison.Ordinal);

        AssertSourceContains(
            playTracker,
            "!combatState.SingleSentenceUsedThisTurn ||",
            "!cardPlay.IsFirstInSeries ||",
            "cardPlay.IsAutoPlay ||",
            "cardPlay.Card.IsClone",
            "ReferenceEquals(cardPlay.Card, combatState.SingleSentenceRulingCard)",
            "combatState.SingleSentenceRulingCard = null",
            "combatState.SingleSentenceRemainingCardsPlayedThisTurn++");
        AssertBefore(playTracker, "ReferenceEquals(cardPlay.Card, combatState.SingleSentenceRulingCard)", "combatState.SingleSentenceRemainingCardsPlayedThisTurn++");

        AssertSourceContains(
            shouldPlay,
            "autoPlayType == AutoPlayType.None",
            "combatState.AutoPlayCardPendingModifier = card",
            "GetSelectedBlessing(player) != LothaBlessingIds.SingleSentence",
            "!combatState.SingleSentenceUsedThisTurn",
            "return combatState.SingleSentenceRemainingCardsPlayedThisTurn < SingleSentenceRemainingPlayLimit;");
        Assert.DoesNotContain("<= SingleSentenceRemainingPlayLimit", shouldPlay, StringComparison.Ordinal);

        var engText = engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"];
        var zhsText = zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"];
        AssertSourceContains(
            engText,
            "only [blue]4[/blue] more cards",
            "first card before the sentence is a [gold]Power[/gold]",
            "costs [blue]0[/blue]",
            "draws [blue]1[/blue] instead");
        AssertSourceContains(
            zhsText,
            "[blue]4[/blue]张牌",
            "若宣判前第一张牌是[gold]能力牌[/gold]",
            "[gold]能力牌[/gold]",
            "费用变为[blue]0[/blue]并抽[blue]1[/blue]张牌",
            "不消耗宣判");
        foreach (var text in new[] { engText, zhsText })
        {
            Assert.DoesNotContain("exactly one card", text, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Block", text, StringComparison.Ordinal);
            Assert.DoesNotContain("Strength", text, StringComparison.Ordinal);
            Assert.DoesNotContain("Energy refund", text, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("格挡", text, StringComparison.Ordinal);
            Assert.DoesNotContain("力量", text, StringComparison.Ordinal);
            Assert.DoesNotContain("获得[blue]1[/blue]点[gold]能量[/gold]", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void PublicEvidenceUsesNonDamageDebuffPolicyAndVisibleEnlightenment()
    {
        var ancient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAncient.cs");
        var runHook = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaRunHook.cs");
        var powers = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaPowers.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");
        var poison = ReadRepoText("source code", "src", "Core", "Models", "Powers", "PoisonPower.cs");
        var weak = ReadRepoText("source code", "src", "Core", "Models", "Powers", "WeakPower.cs");
        var vulnerable = ReadRepoText("source code", "src", "Core", "Models", "Powers", "VulnerablePower.cs");
        var frail = ReadRepoText("source code", "src", "Core", "Models", "Powers", "FrailPower.cs");
        var helper = SourceSlice(runHook, "private static bool IsPublicEvidenceDebuffApplication", "private static bool IsPublicEvidenceExcludedDamageDebuff");
        var excludedDamageDebuffs = SourceSlice(runHook, "private static bool IsPublicEvidenceExcludedDamageDebuff", "private static bool IsUnblockedEnemyAttackDamage");
        var givenHook = SourceSlice(runHook, "public static decimal ModifyPowerAmountGiven", "public static bool TryModifyPowerAmountReceived");
        var receivedHook = SourceSlice(runHook, "public static bool TryModifyPowerAmountReceived", "public static async Task AfterPowerAmountChanged");
        var changedHook = SourceSlice(runHook, "public static async Task AfterPowerAmountChanged", "public static bool ShouldDieLate");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<LothaEnlightenmentPower>()",
            "HoverTipFactory.Static(StaticHoverTip.Block)");
        AssertSourceContains(
            runHook,
            "ModifyPowerAmountGiven",
            "TryModifyPowerAmountReceived",
            "AfterPowerAmountChanged",
            "PublicEvidenceEnlightenmentGain = 1",
            "PublicEvidenceConsumeLimit = 3",
            "PublicEvidenceBlockPerEnlightenment = 4",
            "PublicEvidenceCardsPerEnlightenment = 1",
            "amount * 2m",
            "PowerCmd.Apply<LothaEnlightenmentPower>",
            "RemoveOnePublicEvidenceEnlightenment",
            "ConsumePublicEvidenceEnlightenmentAtTurnStart",
            "IsPublicEvidenceDebuffApplication(power, amount)",
            "IsPublicEvidenceDebuffApplication(canonicalPower, amount)",
            "power.GetTypeForAmount(amount) == PowerType.Debuff");
        AssertSourceContains(
            helper,
            "power.GetTypeForAmount(amount) == PowerType.Debuff",
            "!IsPublicEvidenceExcludedDamageDebuff(power)");
        AssertSourceContains(
            excludedDamageDebuffs,
            "power is PoisonPower",
            "or ConstrictPower",
            "or DemisePower",
            "or DisintegrationPower",
            "or DoomPower",
            "or MagicBombPower",
            "or StranglePower",
            "or TheGambitPower",
            "Core v0.105.0 models these as Debuffs");
        AssertSourceContains(
            powers,
            "internal sealed class LothaEnlightenmentPower",
            "PowerType.Buff",
            "PowerStackType.Counter");

        Assert.DoesNotContain("HasVisibleDebuff", runHook, StringComparison.Ordinal);
        Assert.DoesNotContain("PublicEvidenceUsedThisTurn", runHook, StringComparison.Ordinal);
        AssertSourceContains(
            givenHook,
            "target is not { IsEnemy: true }",
            "!giver.IsPlayer",
            "GetSelectedBlessing(player) != LothaBlessingIds.PublicEvidence");
        AssertSourceContains(
            receivedHook,
            "!target.IsPlayer",
            "applier is not { IsEnemy: true }",
            "GetSelectedBlessing(player) != LothaBlessingIds.PublicEvidence");
        AssertSourceContains(
            changedHook,
            "applier is { IsPlayer: true, Player: { } applyingPlayer }",
            "power.Owner.IsEnemy",
            "applier is { IsEnemy: true }",
            "power.Owner is { IsPlayer: true, Player: { } targetPlayer }");
        AssertSourceContains(
            poison,
            "public override PowerType Type => PowerType.Debuff",
            "AfterSideTurnStart",
            "CreatureCmd.Damage",
            "ValueProp.Unblockable | ValueProp.Unpowered");
        AssertSourceContains(
            weak,
            "public override PowerType Type => PowerType.Debuff",
            "ModifyDamageMultiplicative",
            "PowerCmd.TickDownDuration(this)");
        AssertSourceContains(
            vulnerable,
            "public override PowerType Type => PowerType.Debuff",
            "ModifyDamageMultiplicative",
            "PowerCmd.TickDownDuration(this)");
        AssertSourceContains(
            frail,
            "public override PowerType Type => PowerType.Debuff",
            "ModifyBlockMultiplicative",
            "PowerCmd.TickDownDuration(this)");
        Assert.DoesNotContain("CreatureCmd.Damage", weak, StringComparison.Ordinal);
        Assert.DoesNotContain("CreatureCmd.Damage", vulnerable, StringComparison.Ordinal);
        Assert.DoesNotContain("CreatureCmd.Damage", frail, StringComparison.Ordinal);

        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            "When you apply a non-damaging [gold]negative status[/gold] to an enemy, double its stacks",
            "gain [blue]1[/blue] [gold]Enlightenment[/gold]",
            "When an enemy applies a non-damaging [gold]negative status[/gold] to you, double its stacks",
            "[gold]Poison[/gold], damage-over-time, and countdown damage do not count",
            "consume up to [blue]3[/blue] [gold]Enlightenment[/gold]",
            "draw [blue]1[/blue] card and gain [blue]4[/blue] [gold]Block[/gold]");
        AssertSourceContains(
            zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            "非伤害类[gold]负面状态[/gold]",
            "[gold]中毒[/gold]、持续伤害和倒计时伤害不计",
            "[gold]开悟[/gold]",
            "[gold]格挡[/gold]");
        Assert.Equal(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            engRelics["EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.description"]);
        Assert.Equal(
            zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            zhsRelics["EZMICROBALANCE-LOTHA_PUBLIC_EVIDENCE_OPTION_RELIC.description"]);
    }

    [Fact]
    public void PublicEvidenceDocsCloseDebuffAmbiguityWithoutPoisonClaim()
    {
        var riskRegister = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "risk-register.md");
        var sourceDesign = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "source-design.md");
        var manualChecklist = ReadRepoText("docs", "features", "ancient-expansion-v2.2", "manual-test-checklist.md");
        var issue = ReadRepoText("docs", "issues", "ancient-expansion-v2.2.md");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");

        AssertSourceContains(
            riskRegister,
            "source-closed / live-pending",
            "WeakPower",
            "VulnerablePower",
            "FrailPower",
            "PoisonPower",
            "ConstrictPower",
            "DoomPower",
            "damage/kill Debuffs");
        Assert.DoesNotContain("Define exact source-backed debuff list and ownership.", riskRegister, StringComparison.Ordinal);

        foreach (var doc in new[] { sourceDesign, manualChecklist, issue })
        {
            AssertSourceContains(
                doc,
                "non-damaging negative",
                "Weak",
                "Vulnerable",
                "Frail",
                "Poison",
                "damage-over-time",
                "countdown damage");
        }

        Assert.DoesNotContain("Poison[/gold] to an enemy, double", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("[gold]中毒[/gold]时，其层数翻倍", zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"], StringComparison.Ordinal);
    }

    [Fact]
    public void LothaLocalizationHoverAndRichTextAreReadable()
    {
        var ancient = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaAncient.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");
        var zhsPowers = JsonStringMap("EZMicroBalance", "localization", "zhs", "powers.json");
        var engRelics = JsonStringMap("EZMicroBalance", "localization", "eng", "relics.json");
        var zhsRelics = JsonStringMap("EZMicroBalance", "localization", "zhs", "relics.json");

        AssertSourceContains(
            ancient,
            "HoverTipFactory.FromPower<LothaPresumptionPower>()",
            "HoverTipFactory.FromPower<LothaVerdictPower>()",
            "HoverTipFactory.FromPower<LothaDeathReprievePower>()",
            "HoverTipFactory.FromPower<LothaEnlightenmentPower>()",
            "HoverTipFactory.Static(StaticHoverTip.ReplayStatic)",
            "HoverTipFactory.Static(StaticHoverTip.Energy)",
            "HoverTipFactory.Static(StaticHoverTip.Block)");

        AssertLocalizedKeys(
            [
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.title",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.description",
                "EZMICROBALANCE-LOTHA_DEATH_REPRIEVE_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.title",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description",
                "EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.title",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.description",
                "EZMICROBALANCE-LOTHA_PRESUMPTION_POWER.smartDescription",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.title",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.description",
                "EZMICROBALANCE-LOTHA_VERDICT_POWER.smartDescription"
            ],
            engPowers,
            zhsPowers,
            "Lotha power localization");

        foreach (var (ancientKey, relicKey) in LothaDescriptionKeys)
        {
            Assert.Equal(engAncients[ancientKey], engRelics[relicKey]);
            Assert.Equal(zhsAncients[ancientKey], zhsRelics[relicKey]);
        }

        foreach (var value in LothaOptionValues(zhsAncients, zhsRelics, zhsPowers))
        {
            AssertNoMojibake(value);
            Assert.DoesNotContain("瀵偓", value, StringComparison.Ordinal);
            Assert.DoesNotContain("閺€", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]能力牌[/gold]改为获得[blue]1[/blue]点[gold]能量[/gold]并抽[blue]1[/blue]张牌", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]能力牌[/gold]改为获得[blue]2[/blue]点[gold]能量[/gold]并抽[blue]2[/blue]张牌", value, StringComparison.Ordinal);
        }

        foreach (var value in LothaOptionValues(engAncients, engRelics, engPowers))
        {
            Assert.DoesNotContain("option art marker", value, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Every third", value, StringComparison.Ordinal);
            Assert.DoesNotContain("replayed [blue]2[/blue] times as [gold]Exhaust[/gold] copies", value, StringComparison.Ordinal);
            Assert.DoesNotContain("25%", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]Power[/gold] cards instead gain [blue]1[/blue] [gold]Energy[/gold]", value, StringComparison.Ordinal);
            Assert.DoesNotContain("[gold]Power[/gold] instead grants [blue]1[/blue] [gold]Energy[/gold]", value, StringComparison.Ordinal);
            Assert.DoesNotContain("gain [blue]10[/blue] [gold]Block[/gold]", value, StringComparison.Ordinal);
            Assert.DoesNotContain("exactly one card", value, StringComparison.OrdinalIgnoreCase);
        }

        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"],
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "[gold]Energy[/gold]",
            "[blue]1[/blue]",
            "[blue]2[/blue]");
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_deferred_verdict.description"],
            "[gold]Verdict[/gold]",
            "[gold]Attack[/gold]",
            "[gold]Skill[/gold]",
            "[gold]Power[/gold]",
            "[gold]Energy[/gold]");
        AssertSourceContains(
            engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_public_evidence.description"],
            "[gold]negative status[/gold]",
            "[gold]Poison[/gold]",
            "[gold]Enlightenment[/gold]",
            "[gold]Block[/gold]",
            "[blue]3[/blue]");
        AssertSourceContains(
            zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_mirror_rebuttal.description"],
            "[gold]攻击牌[/gold]",
            "[gold]技能牌[/gold]",
            "[gold]能力牌[/gold]",
            "[gold]能量[/gold]");
        AssertSourceContains(
            zhsPowers["EZMICROBALANCE-LOTHA_VERDICT_POWER.description"],
            "[gold]裁决[/gold]",
            "[gold]攻击牌[/gold]",
            "[gold]技能牌[/gold]",
            "[gold]能力牌[/gold]");
        AssertSourceContains(
            zhsPowers["EZMICROBALANCE-LOTHA_ENLIGHTENMENT_POWER.description"],
            "[gold]开悟[/gold]",
            "[gold]格挡[/gold]");
    }
    private static IEnumerable<string> LothaOptionValues(params IReadOnlyDictionary<string, string>[] maps)
    {
        foreach (var map in maps)
        {
            foreach (var (key, value) in map)
            {
                if (key.Contains("LOTHA", StringComparison.Ordinal) || key.Contains("lotha", StringComparison.Ordinal))
                {
                    yield return value;
                }
            }
        }
    }

    private static void AssertNoMojibake(string value)
    {
        foreach (var fragment in new[]
        {
            "\uFFFD",
            "鐟佷礁",
            "瀵偓",
            "閺€",
            "閼",
            "閻",
            "鐏",
            "閸",
            "缂",
            "闁",
            "閵",
            "缁"
        })
        {
            Assert.DoesNotContain(fragment, value, StringComparison.Ordinal);
        }
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

    private static void AssertLocalizedKeys(
        IEnumerable<string> keys,
        IReadOnlyDictionary<string, string> eng,
        IReadOnlyDictionary<string, string> zhs,
        string context)
    {
        foreach (var key in keys)
        {
            Assert.True(eng.ContainsKey(key), $"Missing English {context}: {key}");
            Assert.True(zhs.ContainsKey(key), $"Missing zhs {context}: {key}");
        }
    }

    private static void AssertSourceContains(string source, params string[] snippets)
    {
        var missing = snippets
            .Where(snippet => !source.Contains(snippet, StringComparison.Ordinal))
            .ToArray();

        Assert.True(missing.Length == 0, "Missing source evidence:" + Environment.NewLine + string.Join(Environment.NewLine, missing));
    }

    private static void AssertBefore(string source, string first, string second)
    {
        var firstIndex = source.IndexOf(first, StringComparison.Ordinal);
        var secondIndex = source.IndexOf(second, StringComparison.Ordinal);

        Assert.True(firstIndex >= 0, $"Missing source evidence: {first}");
        Assert.True(secondIndex >= 0, $"Missing source evidence: {second}");
        Assert.True(firstIndex < secondIndex, $"Expected `{first}` before `{second}`.");
    }

    private static string SourceSlice(string source, string startMarker, string endMarker)
    {
        var start = source.IndexOf(startMarker, StringComparison.Ordinal);
        Assert.True(start >= 0, $"Missing source start marker: {startMarker}");

        var end = source.IndexOf(endMarker, start + startMarker.Length, StringComparison.Ordinal);
        Assert.True(end > start, $"Missing source end marker after {startMarker}: {endMarker}");

        return source[start..end];
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
