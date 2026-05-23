using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Runs;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class RootRunHook : AbstractModel
{
    public RootRunHook()
    {
    }

    public override bool ShouldReceiveCombatHooks => false;

    public override Task AfterActEntered()
    {
        return HandleAfterActEntered();
    }

    public override Task BeforeRoomEntered(AbstractRoom room)
    {
        return HandleBeforeRoomEntered(room);
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        return HandleAfterRoomEntered();
    }

    public override ActMap ModifyGeneratedMap(IRunState runState, ActMap map, int actIndex)
    {
        return AscensionMapService.Apply(runState, map, actIndex);
    }

    public override ActMap ModifyGeneratedMapLate(IRunState runState, ActMap map, int actIndex)
    {
        return AscensionMapService.Apply(runState, map, actIndex);
    }

    public override bool TryModifyCardRewardOptionsLate(
        Player player,
        List<CardCreationResult> cardRewardOptions,
        CardCreationOptions creationOptions)
    {
        return AscensionRewardService.TryModifyCardRewardOptionsLate(player, cardRewardOptions, creationOptions);
    }

    public override bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom? room)
    {
        return AscensionRewardService.TryModifyRewardsLate(player, rewards, room);
    }

    public override Task AfterRestSiteHeal(Player player, bool isMimicked)
    {
        return HandleAfterRestSiteHeal(player, isMimicked);
    }

    public override Task AfterRestSiteSmith(Player player)
    {
        return ForgeTokenService.ApplyAfterRestSiteSmith(player);
    }

    public override Task BeforeCardRemoved(CardModel card)
    {
        return RootDeckService.BeforeCardRemoved(card);
    }

    public override IReadOnlyList<LocString> ModifyExtraRestSiteHealText(
        Player player,
        IReadOnlyList<LocString> currentExtraText)
    {
        return ForgeTokenService.ModifyExtraRestSiteHealText(player, currentExtraText);
    }

    private async Task HandleAfterActEntered()
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState != null)
        {
            AscensionDiagnostics.LogRunState(runState, "after act entered before root seed");
            await ForgeTokenService.SyncVisibleTokens(runState);

            if (AscensionFeatureGate.IsRootblightEnabled(runState))
            {
                await RootDeckService.EnsureStartingRoot(runState);
            }

            AscensionDiagnostics.LogRunState(runState, "after act entered after root seed");
        }
    }

    private static async Task HandleBeforeRoomEntered(AbstractRoom room)
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState == null)
        {
            return;
        }

        if (AscensionFeatureGate.IsRootblightEnabled(runState))
        {
            await RootDeckService.EnsureStartingRoot(runState);
        }
    }

    private static async Task HandleAfterRoomEntered()
    {
        var runState = RunManager.Instance.DebugOnlyGetState();
        if (runState != null)
        {
            await ForgeTokenService.SyncVisibleTokens(runState);
        }
    }

    private static async Task HandleAfterRestSiteHeal(Player player, bool isMimicked)
    {
        if (!isMimicked &&
            AscensionFeatureGate.IsRootblightEnabled(player.RunState))
        {
            await RootDeckService.RemoveHighestRootblight(player, "rest");
        }

        await ForgeTokenService.ApplyAfterRestSiteHeal(player);
    }
}
