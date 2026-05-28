using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class JeweledMaskCombatStartPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "j-e-w-e-l-e-d-m-a-s-k-c-o-m-b-a-t-s-t-a-r-t-p-a-t-c-h";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Patch JeweledMask.BeforeHandDraw";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(JeweledMask), nameof(JeweledMask.BeforeHandDraw))];
{
    [HarmonyPrefix]
    private static bool Prefix(JeweledMask __instance, Player player, ICombatState combatState, ref Task __result)
    {
        if (player != __instance.Owner || combatState.RoundNumber > 1)
        {
            return true;
        }

        __result = PullMarkedPowerToHand(__instance, player);
        return false;
    }

    private static async Task PullMarkedPowerToHand(JeweledMask jeweledMask, Player player)
    {
        var drawPile = PileType.Draw.GetPile(player);
        var markedPower = drawPile.Cards.FirstOrDefault(AncientCardHelpers.IsJeweledMaskPower);
        if (markedPower != null)
        {
            jeweledMask.Flash();
            await CardPileCmd.Add(markedPower, PileType.Hand);
            MainFile.Logger.Info($"[Spire Plus] JeweledMask applied: moved marked power {markedPower.Id.Entry} from draw pile to hand.");
            return;
        }

        if (PileType.Hand.GetPile(player).Cards.Any(AncientCardHelpers.IsJeweledMaskPower))
        {
            MainFile.Logger.Info("[Spire Plus] JeweledMask skipped pull: marked power already in hand.");
            return;
        }

        MainFile.Logger.Info("[Spire Plus] JeweledMask skipped pull: no marked power in draw pile or hand.");
    }
}


