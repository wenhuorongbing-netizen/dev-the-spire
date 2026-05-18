namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Obtain), typeof(RelicModel), typeof(Player), typeof(int))]
internal static class BlackStarObtainPatch
{
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
                $"[EZMicroBalance] BlackStar skipped: pickup compensation requires act 3+, currentActIndex={blackStar.Owner.RunState.CurrentActIndex}.");
            return obtained;
        }

        var relic = RelicFactory.PullNextRelicFromFront(blackStar.Owner).ToMutable();
        await RelicCmd.Obtain(relic, blackStar.Owner);
        MainFile.Logger.Info($"[EZMicroBalance] BlackStar applied: act 3+ immediate relic {relic.Id.Entry}.");
        return obtained;
    }
}
