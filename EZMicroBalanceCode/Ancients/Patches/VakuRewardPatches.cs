using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class IronClubVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "i-r-o-n-c-l-u-b-v-a-r-s-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch IronClub.get_CanonicalVars";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(IronClub), "get_CanonicalVars", HarmonyLib.MethodType.Getter)];
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(5) };
        return false;
    }
}

internal sealed class BrilliantScarfVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "b-r-i-l-l-i-a-n-t-s-c-a-r-f-v-a-r-s-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch BrilliantScarf.get_CanonicalVars";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(BrilliantScarf), "get_CanonicalVars", HarmonyLib.MethodType.Getter)];
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(6) };
        return false;
    }
}

internal sealed class BeautifulBraceletVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "b-e-a-u-t-i-f-u-l-b-r-a-c-e-l-e-t-v-a-r-s-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch BeautifulBracelet.get_CanonicalVars";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(BeautifulBracelet), "get_CanonicalVars", HarmonyLib.MethodType.Getter)];
{
    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(3), new DynamicVar("Swift", 2m) };
        return false;
    }
}

internal sealed class BeautifulBraceletPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "b-e-a-u-t-i-f-u-l-b-r-a-c-e-l-e-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch BeautifulBracelet.AfterObtained";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(BeautifulBracelet), nameof(BeautifulBracelet.AfterObtained))];
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

internal sealed class MusicBoxBeforeCardPlayedPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "m-u-s-i-c-b-o-x-b-e-f-o-r-e-c-a-r-d-p-l-a-y-e-d-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch MusicBox.BeforeCardPlayed";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(MusicBox), nameof(MusicBox.BeforeCardPlayed))];
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

internal sealed class MusicBoxAfterCardPlayedPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "m-u-s-i-c-b-o-x-a-f-t-e-r-c-a-r-d-p-l-a-y-e-d-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch MusicBox.AfterCardPlayed";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(MusicBox), nameof(MusicBox.AfterCardPlayed))];
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

internal sealed class MusicBoxTurnResetPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "m-u-s-i-c-b-o-x-t-u-r-n-r-e-s-e-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch MusicBox.BeforeSideTurnStart";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(MusicBox), nameof(MusicBox.BeforeSideTurnStart))];
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

internal sealed class MusicBoxCombatResetPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "m-u-s-i-c-b-o-x-c-o-m-b-a-t-r-e-s-e-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch MusicBox.AfterCombatEnd";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(MusicBox), nameof(MusicBox.AfterCombatEnd))];
{
    [HarmonyPostfix]
    private static void ResetAfterCombat(MusicBox __instance)
    {
        MusicBoxStateTracker.Reset(__instance);
    }
}



