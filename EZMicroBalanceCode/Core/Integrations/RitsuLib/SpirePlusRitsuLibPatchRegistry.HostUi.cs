using EZMicroBalance.EZMicroBalanceCode.Ancients;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Ascension.Events;
using EZMicroBalance.EZMicroBalanceCode.Core.Localization;
using EZMicroBalance.EZMicroBalanceCode.Modding;
using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// RitsuLib registrations for host-screen UI patches such as rewards, mod info,
/// combat hand safety, rest-site buttons, and Ascension lobby selection.
/// </summary>
internal static partial class SpirePlusRitsuLibPatchRegistry
{
    private static void RegisterHostUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<PrismaticGemRewardScreenHintPatch>();
        patcher.RegisterPatch<AscensionA20RewardScreenReadyPatch>();
        patcher.RegisterPatch<AscensionA20RewardScreenStatePatch>();
        patcher.RegisterPatch<AscensionA20CourtyardProceedPatch>();
        patcher.RegisterPatch<AscensionA20CourtyardPortraitPatch>();
        patcher.RegisterPatch<ModInfoLocalizationPatches>();
        patcher.RegisterPatch<CombatHandInputSafetyPatch>();
        patcher.RegisterPatch<MeatCleaverCookIsEnabledPatch>();
        patcher.RegisterPatch<MeatCleaverCookDescriptionPatch>();
        patcher.RegisterPatch<MeatCleaverCookPatch>();
    }

    private static void RegisterAscensionSelectionUiPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<StartRunLobbySetSingleplayerAscensionPatch>();
        patcher.RegisterPatch<StartRunLobbyBeginRunLocallyPatch>();
        patcher.RegisterPatch<StartRunLobbyUpdateMaxMultiplayerAscensionPatch>();
        patcher.RegisterPatch<StartRunLobbyUpdatePreferredAscensionPatch>();
        patcher.RegisterPatch<StartRunLobbySyncAscensionChangeA20WarningPatch>();
        patcher.RegisterPatch<StartRunLobbyBeginRunForAllPlayersA20WarningPatch>();
    }
}
