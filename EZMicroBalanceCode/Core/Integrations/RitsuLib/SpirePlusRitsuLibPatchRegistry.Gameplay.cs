using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;
using EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;
using EZMicroBalance.EZMicroBalanceCode.Ascension;
using EZMicroBalance.EZMicroBalanceCode.Ascension.Events;
using EZMicroBalance.EZMicroBalanceCode.Map;
using EZMicroBalance.EZMicroBalanceCode.Modding;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;
using STS2RitsuLib.Patching.Core;

namespace EZMicroBalance.EZMicroBalanceCode.Core.Integrations.RitsuLib;

/// <summary>
/// Gameplay and diagnostic registrations whose runtime behavior still needs live proof.
/// </summary>
internal static partial class SpirePlusRitsuLibPatchRegistry
{
    private static void RegisterUrdaBehaviorPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<UrdaRootSightRollRoomTypePatch>();
        patcher.RegisterPatch<UrdaRootSightCreateRoomPatch>();
        patcher.RegisterPatch<WitheredHuskTransformablePatch>();
        patcher.RegisterPatch<WitheredHuskTransformationOptionsPatch>();
        patcher.RegisterPatch<UrdaSeedbedAfterCardDrawnPatch>();
        patcher.RegisterPatch<UrdaSeedbedCardPileDrawPatch>();
    }

    private static void RegisterVakuuLifecyclePatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<VakuuFightPreFinishedSavePatch>();
        patcher.RegisterPatch<VakuuFightPreFinishedParentRestorePatch>();
        patcher.RegisterPatch<VakuuFightNoRewardRestorePatch>();
    }

    private static void RegisterAscensionBossFlowPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<AscensionA20GenerateRoomsPatch>();
    }

    private static void RegisterAscensionMapGenerationPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<AscensionActModelCreateMapPatch>();
    }

    private static void RegisterAscensionDiagnosticPatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<JoinFlowHandleInitialGameInfoMessageDiagPatch>();
        patcher.RegisterPatch<StartRunLobbyBeginRunForAllPlayersDiagPatch>();
        patcher.RegisterPatch<StartRunLobbyBeginRunLocallyDiagPatch>();
        patcher.RegisterPatch<StartRunLobbyUpdateMaxMultiplayerAscensionDiagPatch>();
        patcher.RegisterPatch<NGameStartNewMultiplayerRunDiagPatch>();
        patcher.RegisterPatch<RunManagerEnterActDiagPatch>();
        patcher.RegisterPatch<SaveQuitSaveRunDiagPatch>();
        patcher.RegisterPatch<SaveQuitReturnToMainMenuDiagPatch>();
        patcher.RegisterPatch<SaveQuitNGameQuitDiagPatch>();
        patcher.RegisterPatch<AncientEventModelBeforeEventStartedDiagPatch>();
    }

#if REPLACEMENT_PROTOTYPE_ENABLED
    private static void RegisterSts1ReplacementPrototypePatches(ModPatcher patcher)
    {
        patcher.RegisterPatch<Sts1ReplacementPrototype>();
    }
#endif
}
