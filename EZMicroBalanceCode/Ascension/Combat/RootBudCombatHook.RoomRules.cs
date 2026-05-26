using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Rooms;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class RootBudCombatHook
{
    private static bool IsGameplayEnabledForCurrentRoom(CombatState state)
    {
        var requiredLevel = RequiredAscensionLevelForCurrentRoom(state);
        return requiredLevel switch
        {
            AscensionFeatureGate.BossRootBudLevel => AscensionFeatureGate.IsBossBlightSproutEnabled(state.RunState),
            AscensionFeatureGate.EliteRootBudLevel => AscensionFeatureGate.IsEliteBlightSproutEnabled(state.RunState),
            _ => false
        };
    }

    private static int GetRootBudCountForCurrentRoom(CombatState state)
    {
        return state.RunState.CurrentRoom?.RoomType == RoomType.Boss
            ? 2
            : 1;
    }

    private static int GetRootBudSproutRoundForCurrentRoom(CombatState state, int budIndex)
    {
        return state.RunState.CurrentRoom?.RoomType == RoomType.Boss && budIndex == 1
            ? RootBud.BossSecondSproutRound
            : RootBud.DefaultSproutRound;
    }

    private static void NormalizeExistingRootBudRounds(CombatState state, IReadOnlyList<RootBud> existingBuds)
    {
        var targetRounds = Enumerable.Range(0, existingBuds.Count)
            .Select(i => GetRootBudSproutRoundForCurrentRoom(state, i))
            .ToList();
        for (var i = 0; i < existingBuds.Count; i++)
        {
            existingBuds[i].SproutRound = targetRounds[i];
        }
    }

    private static int? RequiredAscensionLevelForCurrentRoom(CombatState state)
    {
        return state.RunState.CurrentRoom?.RoomType switch
        {
            RoomType.Boss when IsActTwoOrThree(state) => AscensionFeatureGate.BossRootBudLevel,
            RoomType.Elite when IsEligibleEliteSproutFight(state) => AscensionFeatureGate.EliteRootBudLevel,
            _ => null
        };
    }

    private static bool IsActTwoOrThree(CombatState state)
    {
        return state.RunState.CurrentActIndex is 1 or 2;
    }

    private static bool IsEligibleEliteSproutFight(CombatState state)
    {
        if (!IsActTwoOrThree(state))
        {
            return false;
        }

        var currentRow = state.RunState.CurrentMapPoint?.coord.row ?? state.RunState.ActFloor - 1;
        return currentRow >= 3;
    }
}
