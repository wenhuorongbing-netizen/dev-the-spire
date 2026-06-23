using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class PaelsHornPhase1Patch : IPatchMethod
{
    static string IPatchMethod.PatchId => "paels-horn-after-obtained";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Add one Relax and one Relax+ when Pael's Horn is obtained";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(PaelsHorn), nameof(PaelsHorn.AfterObtained))];

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
