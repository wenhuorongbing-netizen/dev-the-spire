using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class IronClubVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "iron-club-vars";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Override Iron Club canonical vars for Spire Plus balance text";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(IronClub), "CanonicalVars", MethodType.Getter)];

    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(5) };
        return false;
    }
}

internal sealed class BrilliantScarfVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "brilliant-scarf-vars";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Override Brilliant Scarf canonical vars for Spire Plus balance text";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(BrilliantScarf), "CanonicalVars", MethodType.Getter)];

    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(6) };
        return false;
    }
}

internal sealed class BeautifulBraceletVarsPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "beautiful-bracelet-vars";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Override Beautiful Bracelet canonical vars for Spire Plus balance text";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(BeautifulBracelet), "CanonicalVars", MethodType.Getter)];

    [HarmonyPrefix]
    private static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[] { new CardsVar(3), new DynamicVar("Swift", 2m) };
        return false;
    }
}

internal sealed class BeautifulBraceletPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "beautiful-bracelet-after-obtained";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Apply Beautiful Bracelet's Spire Plus Swift enchantment pickup flow";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(BeautifulBracelet), nameof(BeautifulBracelet.AfterObtained))];

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
    static string IPatchMethod.PatchId => "music-box-before-card-played";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Suppress vanilla Music Box before-play behavior for Spire Plus replacement logic";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(MusicBox), nameof(MusicBox.BeforeCardPlayed))];

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
    static string IPatchMethod.PatchId => "music-box-after-card-played";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Create Music Box's once-per-turn temporary attack copy";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(MusicBox), nameof(MusicBox.AfterCardPlayed))];

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
    static string IPatchMethod.PatchId => "music-box-turn-reset";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reset Music Box's once-per-turn state at owner turn start";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(MusicBox), nameof(MusicBox.BeforeSideTurnStart))];

    [HarmonyPostfix]
    private static void Postfix(MusicBox __instance, CombatSide side)
    {
        if (side == __instance.Owner.Creature.Side)
        {
            MusicBoxStateTracker.Reset(__instance);
        }
    }
}

internal sealed class MusicBoxCombatResetPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "music-box-combat-reset";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Reset Music Box's once-per-turn state after combat";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(MusicBox), nameof(MusicBox.AfterCombatEnd))];

    [HarmonyPostfix]
    private static void Postfix(MusicBox __instance)
    {
        MusicBoxStateTracker.Reset(__instance);
    }
}
