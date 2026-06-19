using Xunit;

namespace EZMicroBalance.Tests;

public sealed partial class LothaPolishGuardTests
{
    [Fact]
    public void SingleSentenceCapIgnoresAutoplayClonesAndExtraExecutions()
    {
        var runHook = ReadLothaSource();
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var engPowers = JsonStringMap("EZMicroBalance", "localization", "eng", "powers.json");

        AssertSourceContains(
            runHook,
            "LothaExtraPlayCount = 2",
            "SingleSentenceRemainingPlayLimit = 4",
            "SingleSentenceReadyDisplayAmount = SingleSentenceRemainingPlayLimit + 1",
            "TryResolveSingleSentencePowerFallback",
            "SingleSentencePowerFallbackUsedThisTurn",
            "IsPowerReplacementCostZeroCard",
            "HydrateSingleSentenceFromPower",
            "EnsureSingleSentencePower",
            "SetSingleSentencePowerAmount",
            "LothaSingleSentencePower",
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
            "play up to [blue]4[/blue] more cards",
            "[gold]Power[/gold] cards do not count",
            "cost [blue]0[/blue]",
            "draw [blue]1[/blue]");
        AssertSourceContains(
            engPowers["EZMICROBALANCE-LOTHA_SINGLE_SENTENCE_POWER.description"],
            "ready",
            "counter becomes [blue]4[/blue]",
            "remaining card plays");
        Assert.DoesNotContain("[gold]Power[/gold] instead grants [blue]1[/blue] [gold]Energy[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("gain [blue]10[/blue] [gold]Block[/gold]", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.Ordinal);
        Assert.DoesNotContain("exactly one card", engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SingleSentenceBranchesAreGuardedBeforeAndAfterTheRuling()
    {
        var cardRules = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CardRules.cs");
        var cardPlayDispatch = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CardPlayDispatch.cs");
        var cardPlayCount = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CardPlayCount.cs");
        var cardEligibility = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.CardEligibility.cs");
        var powerReplacement = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.PowerReplacement.cs");
        var singleSentence = ReadRepoText("EZMicroBalanceCode", "Ancients", "Expansion", "Lotha", "LothaBlessingService.SingleSentence.cs");
        var engAncients = JsonStringMap("EZMicroBalance", "localization", "eng", "ancients.json");
        var zhsAncients = JsonStringMap("EZMicroBalance", "localization", "zhs", "ancients.json");

        var shouldPlay = SliceFrom(cardRules, "public static bool ShouldPlay");
        var powerFallback = SliceBetween(singleSentence, "private static async Task TryResolveSingleSentencePowerFallback", "private static void TrackSingleSentenceRemainingPlays");
        var playTracker = SliceBetween(singleSentence, "private static void TrackSingleSentenceRemainingPlays", "private static async Task EnsureSingleSentencePower");
        var powerEligibility = SliceFrom(singleSentence, "private static bool CanUseSingleSentencePowerReplacement");
        var eligibleCard = SliceBetween(cardEligibility, "private static bool IsEligibleCard", "private static bool IsDeferredVerdictConsumerCard");
        var powerCard = SliceFrom(powerReplacement, "private static bool IsPowerCard");

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
            "var canPlay = combatState.SingleSentenceRemainingCardsPlayedThisTurn < SingleSentenceRemainingPlayLimit",
            "SetSingleSentencePowerAmount(player, 0)",
            "return canPlay;");
        Assert.DoesNotContain("<= SingleSentenceRemainingPlayLimit", shouldPlay, StringComparison.Ordinal);
        Assert.DoesNotContain("public static int ModifyCardPlayCount", cardRules, StringComparison.Ordinal);
        Assert.DoesNotContain("public static async Task AfterCardPlayed", cardRules, StringComparison.Ordinal);
        AssertSourceContains(
            cardPlayDispatch,
            "public static async Task AfterCardPlayed",
            "TryResolveMirrorRebuttalPowerFallback(choiceContext, cardPlay, combatState)",
            "TryResolveMirrorHallEchoPowerFallback(choiceContext, cardPlay, combatState)",
            "TryResolveDeferredVerdictCard(choiceContext, cardPlay, combatState)",
            "TryResolveSingleSentencePowerFallback(choiceContext, cardPlay, combatState)",
            "TrackSingleSentenceRemainingPlays(cardPlay, combatState)");
        AssertSourceContains(
            cardPlayCount,
            "public static int ModifyCardPlayCount",
            "LothaExtraPlayCount = 2",
            "MirrorRebuttalExtraPlayCount = 1",
            "selectedBlessing == LothaBlessingIds.SingleSentence",
            "LogExtraPlayAttempt(");

        var engText = engAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"];
        var zhsText = zhsAncients["EZMB_LOTHA.pages.INITIAL.options.lotha_single_sentence.description"];
        AssertSourceContains(
            engText,
            "first [gold]Attack[/gold] or [gold]Skill[/gold]",
            "plays [blue]2[/blue] extra times",
            "play up to [blue]4[/blue] more cards",
            "[gold]Power[/gold] cards do not count",
            "cost [blue]0[/blue]",
            "draw [blue]1[/blue]");
        AssertSourceContains(
            zhsText,
            "[gold]",
            "[/gold]",
            "[blue]4[/blue]",
            "[blue]0[/blue]",
            "[blue]1[/blue]");
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
}
