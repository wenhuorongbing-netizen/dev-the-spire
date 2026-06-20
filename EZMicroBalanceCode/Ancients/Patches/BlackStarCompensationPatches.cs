using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed class BlackStarObtainPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "black-star-obtain";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Grant act 3+ compensation relic when BlackStar is obtained";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
        [new ModPatchTarget(typeof(RelicCmd), nameof(RelicCmd.Obtain),
            [typeof(RelicModel), typeof(Player), typeof(int)])];
    [HarmonyPostfix]
    private static void Postfix(ref Task<RelicModel> __result)
    {
        __result = GrantActThreeCompensationAfterObtain(__result);
    }

    private static async Task<RelicModel> GrantActThreeCompensationAfterObtain(Task<RelicModel> original)
    {
        var obtained = await original;
        if (obtained is not BlackStar blackStar)
        {
            return obtained;
        }

        if (blackStar.Owner.RunState.CurrentActIndex < 2)
        {
            MainFile.Logger.Info(
                $"[Spire Plus] BlackStar skipped: pickup compensation requires act 3+, currentActIndex={blackStar.Owner.RunState.CurrentActIndex}.");
            return obtained;
        }

        var relic = RelicFactory.PullNextRelicFromFront(blackStar.Owner).ToMutable();
        await RelicCmd.Obtain(relic, blackStar.Owner);
        MainFile.Logger.Info($"[Spire Plus] BlackStar applied: act 3+ immediate relic {relic.Id.Entry}.");
        return obtained;
    }
}
