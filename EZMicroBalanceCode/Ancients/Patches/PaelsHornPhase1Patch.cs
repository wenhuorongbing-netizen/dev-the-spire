using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(PaelsHorn), nameof(PaelsHorn.AfterObtained))]
internal static class PaelsHornPhase1Patch
{
    [HarmonyPrefix]
    private static bool Prefix(PaelsHorn __instance, ref Task __result)
    {
        __result = AddRelaxAndUpgradedRelax(__instance);
        return false;
    }

    private static async Task AddRelaxAndUpgradedRelax(PaelsHorn paelsHorn)
    {
        var owner = paelsHorn.Owner;
        var normalRelax = owner.RunState.CreateCard<Relax>(owner);
        var upgradedRelax = owner.RunState.CreateCard<Relax>(owner);
        CardCmd.Upgrade(upgradedRelax);

        var results = new List<CardPileAddResult>
        {
            await CardPileCmd.Add(normalRelax, PileType.Deck),
            await CardPileCmd.Add(upgradedRelax, PileType.Deck)
        };

        SpirePlusFeedback.PreviewDeckAdds(results, paelsHorn, 2f);
        MainFile.Logger.Info("[Spire Plus] PaelsHornPhase1 applied: added Relax and Relax+.");
    }
}
