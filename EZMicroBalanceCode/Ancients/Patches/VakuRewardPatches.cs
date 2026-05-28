namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(IronClub), "get_CanonicalVars")]
internal static class IronClubVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(5) };
        return false;
    }
}

[HarmonyPatch(typeof(BrilliantScarf), "get_CanonicalVars")]
internal static class BrilliantScarfVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(6) };
        return false;
    }
}

[HarmonyPatch(typeof(BeautifulBracelet), "get_CanonicalVars")]
internal static class BeautifulBraceletVarsPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(3), new DynamicVar("Swift", 2m) };
        return false;
    }
}

[HarmonyPatch(typeof(BeautifulBracelet), nameof(BeautifulBracelet.AfterObtained))]
internal static class BeautifulBraceletPatch
{
    [HarmonyPrefix]
    private static bool Prefix(BeautifulBracelet __instance, ref Task __result)
    {
        __result = AddSwiftTwo(__instance);
        return false;
    }

    private static async Task AddSwiftTwo(BeautifulBracelet bracelet)
    {
        var swift = ModelDb.Enchantment<Swift>();
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, bracelet.DynamicVars.Cards.IntValue);
        var cards = (await CardSelectCmd.FromDeckForEnchantment(bracelet.Owner, swift, 2, prefs)).ToList();
        foreach (var card in cards)
        {
            CardCmd.Enchant<Swift>(card, 2m);
        }

        MainFile.Logger.Info($"[Spire Plus] BeautifulBracelet applied: enchanted {cards.Count} card(s) with Swift 2.");
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeCardPlayed))]
internal static class MusicBoxBeforeCardPlayedPatch
{
    [HarmonyPrefix]
    private static bool Prefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }
}

internal static class MusicBoxStateTracker
{
    private sealed class State
    {
        public bool WasUsedThisTurn { get; set; }
    }

    private static readonly ConditionalWeakTable<MusicBox, State> States = new();

    public static bool WasUsedThisTurn(MusicBox musicBox)
    {
        return States.GetOrCreateValue(musicBox).WasUsedThisTurn;
    }

    public static void MarkUsed(MusicBox musicBox)
    {
        States.GetOrCreateValue(musicBox).WasUsedThisTurn = true;
    }

    public static void Reset(MusicBox musicBox)
    {
        States.GetOrCreateValue(musicBox).WasUsedThisTurn = false;
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCardPlayed))]
internal static class MusicBoxAfterCardPlayedPatch
{
    [HarmonyPrefix]
    private static bool Prefix(MusicBox __instance, CardPlay cardPlay, ref Task __result)
    {
        __result = AfterCardPlayed(__instance, cardPlay);
        return false;
    }

    private static async Task AfterCardPlayed(MusicBox musicBox, CardPlay cardPlay)
    {
        if (MusicBoxStateTracker.WasUsedThisTurn(musicBox) ||
            cardPlay.Card.Owner != musicBox.Owner ||
            cardPlay.Card.Type != CardType.Attack)
        {
            return;
        }

        musicBox.Flash();
        var copy = cardPlay.Card.CreateClone();
        AncientCardHelpers.ApplyTemporaryCostReduction(copy, 1);
        AncientCardHelpers.ApplyKeywords(copy, CardKeyword.Ethereal, CardKeyword.Exhaust);
        var addResult = await AncientCardHelpers.TryAddGeneratedCardToCombat(copy, PileType.Hand, musicBox.Owner);
        if (addResult is not { success: true })
        {
            MainFile.Logger.Warn("[Spire Plus] MusicBox skipped: generated attack copy could not be added to combat.");
            return;
        }

        MusicBoxStateTracker.MarkUsed(musicBox);
        MainFile.Logger.Info("[Spire Plus] MusicBox applied: created attack copy with -1 cost, Ethereal, and Exhaust.");
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.BeforeSideTurnStart))]
internal static class MusicBoxTurnResetPatch
{
    [HarmonyPostfix]
    private static void ResetOnTurnStart(MusicBox __instance, CombatSide side)
    {
        if (side == __instance.Owner.Creature.Side)
        {
            MusicBoxStateTracker.Reset(__instance);
        }
    }
}

[HarmonyPatch(typeof(MusicBox), nameof(MusicBox.AfterCombatEnd))]
internal static class MusicBoxCombatResetPatch
{
    [HarmonyPostfix]
    private static void ResetAfterCombat(MusicBox __instance)
    {
        MusicBoxStateTracker.Reset(__instance);
    }
}

