namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

[HarmonyPatch(typeof(MegaCrit.Sts2.Core.Hooks.Hook), nameof(MegaCrit.Sts2.Core.Hooks.Hook.TryModifyCardRewardOptions))]
internal static partial class PrismaticGemRewardPatch
{
    [HarmonyPrefix]
    private static bool Prefix(
        IRunState runState,
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions,
        ref List<AbstractModel> modifiers,
        ref bool __result)
    {
        var prismaticGem = player.Relics.OfType<PrismaticGem>().FirstOrDefault(relic => !relic.IsMelted);
        if (prismaticGem == null)
        {
            return true;
        }

        var modified = false;
        modifiers = [];
        foreach (var listener in runState.IterateHookListeners(null))
        {
            modified = listener.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions) || modified;
            modifiers.Add(listener);
        }

        modified = TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions) || modified;

        foreach (var listener in runState.IterateHookListeners(null))
        {
            modified = listener.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions) || modified;
            modifiers.Add(listener);
        }

        CleanupSupersededPrismaticReplacements(cardRewardOptions);
        __result = modified;
        return false;
    }
}
