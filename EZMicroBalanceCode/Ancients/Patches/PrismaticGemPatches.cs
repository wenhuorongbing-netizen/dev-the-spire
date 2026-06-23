using STS2RitsuLib.Patching.Models;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients;

internal sealed partial class PrismaticGemRewardPatch : IPatchMethod
{
    static string IPatchMethod.PatchId => "prismatic-gem-reward-options";
    static bool IPatchMethod.IsCritical => false;
    static string IPatchMethod.Description => "Apply Prismatic Gem off-color replacement between Core's early and late reward modifiers";
    static ModPatchTarget[] IPatchMethod.GetTargets() =>
    [
        new ModPatchTarget(
            typeof(MegaCrit.Sts2.Core.Hooks.Hook),
            nameof(MegaCrit.Sts2.Core.Hooks.Hook.TryModifyCardRewardOptions),
            [
                typeof(IRunState),
                typeof(Player),
                typeof(List<CardCreationResult>),
                typeof(CardCreationOptions),
                typeof(List<AbstractModel>).MakeByRefType()
            ])
    ];

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
            var listenerModified = listener.TryModifyCardRewardOptions(player, cardRewardOptions, creationOptions);
            modified = listenerModified || modified;
            if (listenerModified)
            {
                modifiers.Add(listener);
            }
        }

        // Prismatic replacement sits between Core's early and late reward hooks.
        // Late model modifiers such as Eggs, Silver Crucible, and Silken Tress
        // then modify the off-color cards instead of being erased by a later swap.
        modified = TryReplaceNormalRewardScreen(prismaticGem, player, cardRewardOptions, creationOptions) || modified;

        foreach (var listener in runState.IterateHookListeners(null))
        {
            var listenerModified = listener.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions);
            modified = listenerModified || modified;
            if (listenerModified)
            {
                modifiers.Add(listener);
            }
        }

        CleanupSupersededPrismaticReplacements(cardRewardOptions);
        __result = modified;
        return false;
    }
}

