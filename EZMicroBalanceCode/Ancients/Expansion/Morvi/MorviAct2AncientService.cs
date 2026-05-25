using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Unlocks;
using EZMicroBalance.EZMicroBalanceCode.Ascension;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Morvi;

internal static class MorviAct2AncientService
{
    public static void AddMorviToAct2(UnlockState unlockState, ref IEnumerable<AncientEventModel> unlockedAncients)
    {
        if (!MorviFeatureGate.IsMorviEnabled(unlockState))
        {
            return;
        }

        var runState = MultiplayerFeaturePolicy.CurrentRunStateOrNull();
        if (MultiplayerFeaturePolicy.ShouldDisableUnverifiedCoopGameplay(
                runState,
                "MorviAncientOffer",
                "Morvi Ancient reward selection mutates per-player reward state and is disabled in co-op until two-client proof exists."))
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(MorviFeatureGate.ForcedAncient) &&
            !MorviFeatureGate.ShouldForceMorvi)
        {
            return;
        }

        var morvi = ModelDb.AncientEvent<EzmbMorvi>();
        if (MorviFeatureGate.ShouldForceMorvi)
        {
            unlockedAncients = [morvi];
            MainFile.Logger.Info("[Spire Plus] Force Ancient gate selected Morvi as the Act 2 Ancient.");
            return;
        }

        var list = unlockedAncients.ToList();
        if (!list.Any(ancient => ancient.Id == morvi.Id))
        {
            list.Add(morvi);
            MainFile.Logger.Info("[Spire Plus] Morvi added to Act 2 unlocked ancients for private-beta testing.");
            unlockedAncients = list;
        }
    }
}

[HarmonyPatch(typeof(Hive), nameof(Hive.GetUnlockedAncients))]
internal static class MorviHivePatch
{
    [HarmonyPostfix]
    private static void Postfix(UnlockState unlockState, ref IEnumerable<AncientEventModel> __result) =>
        MorviAct2AncientService.AddMorviToAct2(unlockState, ref __result);
}
