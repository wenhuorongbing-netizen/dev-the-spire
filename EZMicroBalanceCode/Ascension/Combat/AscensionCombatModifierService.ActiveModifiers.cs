namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class AscensionCombatModifierService
{
    private static bool HasActiveFiremark(CombatState combatState, AscensionNodeMetadata metadata)
    {
        return metadata.Firemark.HasValue &&
            combatState.RunState.CurrentRoom?.RoomType == RoomType.Elite &&
            AscensionFeatureGate.IsFiremarkedEliteEnabled(combatState.RunState);
    }

    private static bool HasActiveBanner(CombatState combatState, AscensionNodeMetadata metadata)
    {
        return metadata.Banner.HasValue &&
            combatState.RunState.CurrentRoom?.RoomType == RoomType.Monster &&
            AscensionFeatureGate.IsBannerRoomEnabled(combatState.RunState);
    }

    private static bool HasActiveBossSeal(CombatState combatState, AscensionNodeMetadata metadata)
    {
        return metadata.BossSeal != null &&
            combatState.RunState.CurrentRoom?.RoomType == RoomType.Boss &&
            (metadata.IsBossBrand
                ? AscensionFeatureGate.IsBrandedFormSinglePlayerEnabled(combatState.RunState)
                : AscensionFeatureGate.IsBossSealsEnabled(combatState.RunState));
    }
}
