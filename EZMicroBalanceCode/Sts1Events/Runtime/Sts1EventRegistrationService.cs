using STS2RitsuLib;
using STS2RitsuLib.Content;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act1;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

/// <summary>
/// Registers StS1 events through RitsuLib content packs.
///
/// Keep this service on RitsuLib APIs only. The feature is intentionally gated,
/// and new event batches should extend the mode-specific registrations below.
///
/// StS2 act mapping (verified from UrdaAct1AncientService, MorviAct2AncientService, LothaAct3AncientService):
///   StS1 Act 1 events -> Overgrowth + Underdocks (both acts)
///   StS1 Act 2 events -> Hive
///   StS1 Act 3 events -> Glory
/// </summary>
internal static class Sts1EventRegistrationService
{
    /// <summary>
    /// Registers events gated by mode. Called from Sts1EventsFeatureModule.Initialize().
    /// </summary>
    public static void RegisterGated(string modId, Sts1EventRegistrationMode mode)
    {
        switch (mode)
        {
            case Sts1EventRegistrationMode.Off:
                return;
            case Sts1EventRegistrationMode.CanaryOnly:
                RegisterCanaryOnly(modId);
                return;
            case Sts1EventRegistrationMode.AdditiveBatch1:
                RegisterAdditiveBatch1(modId);
                return;
            case Sts1EventRegistrationMode.AdditiveAllDraft:
                RegisterAll(modId);
                return;
            case Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype:
#if REPLACEMENT_PROTOTYPE_ENABLED
                RegisterAll(modId);
#else
                MainFile.Logger.Warn("[StS1 Events] ReplaceUnknownEventsPrototype requested, but REPLACEMENT_PROTOTYPE_ENABLED is not defined; no StS1 events registered.");
#endif
                return;
        }
    }

    /// <summary>
    /// Registers exactly 4 canary event types.
    /// </summary>
    public static void RegisterCanaryOnly(string modId)
    {
        if (!RitsuLibFramework.IsActive)
        {
            MainFile.Logger.Warn("[StS1 Events] RitsuLib not active; skipping canary event registration.");
            return;
        }

        var logger = RitsuLibFramework.CreateLogger(modId);
        logger.Info("[StS1 Events] Registering canary events (Big Fish, Golden Idol, Lab, Divine Fountain)...");

        var content = RitsuLibFramework.CreateContentPack(modId);

        content.ActEvent<Overgrowth, Sts1BigFish>();
        content.ActEvent<Underdocks, Sts1BigFish>();
        content.ActEvent<Overgrowth, Sts1GoldenIdol>();
        content.ActEvent<Underdocks, Sts1GoldenIdol>();
        content.SharedEvent<Sts1TheLab>();
        content.SharedEvent<Sts1DivineFountain>();

        content.Apply();

        logger.Info("[StS1 Events] Canary events registered successfully.");
    }

    /// <summary>
    /// Registers exactly 10 verified-scope prototype events: four canary events plus six simple batch events.
    /// </summary>
    public static void RegisterAdditiveBatch1(string modId)
    {
        if (!RitsuLibFramework.IsActive)
        {
            MainFile.Logger.Warn("[StS1 Events] RitsuLib not active; skipping AdditiveBatch1 event registration.");
            return;
        }

        var logger = RitsuLibFramework.CreateLogger(modId);
        logger.Info("[StS1 Events] Registering AdditiveBatch1 events (4 canary + 6 simple)...");

        var content = RitsuLibFramework.CreateContentPack(modId);

        content.ActEvent<Overgrowth, Sts1BigFish>();
        content.ActEvent<Underdocks, Sts1BigFish>();
        content.ActEvent<Overgrowth, Sts1GoldenIdol>();
        content.ActEvent<Underdocks, Sts1GoldenIdol>();
        content.SharedEvent<Sts1TheLab>();
        content.SharedEvent<Sts1DivineFountain>();
        content.SharedEvent<Sts1Purifier>();
        content.ActEvent<Glory, Sts1UpgradeShrine>();
        content.SharedEvent<Sts1GoldenShrine>();
        content.ActEvent<Overgrowth, Sts1TheCleric>();
        content.ActEvent<Underdocks, Sts1TheCleric>();
        content.SharedEvent<Sts1OldBeggar>();
        content.ActEvent<Overgrowth, Sts1ShiningLight>();
        content.ActEvent<Underdocks, Sts1ShiningLight>();

        content.Apply();

        logger.Info("[StS1 Events] AdditiveBatch1 events registered successfully.");
    }

    /// <summary>
    /// Registers all drafted StS1 events. Used by AdditiveAllDraft and ReplaceUnknownEventsPrototype modes.
    /// Sts1Duplicator is excluded because the needed CardSelectCmd/CardPileCmd APIs are not yet available.
    /// </summary>
    public static void RegisterAll(string modId)
    {
        if (!RitsuLibFramework.IsActive)
        {
            MainFile.Logger.Warn("[StS1 Events] RitsuLib not active; skipping event registration.");
            return;
        }

        var logger = RitsuLibFramework.CreateLogger(modId);
        logger.Info("[StS1 Events] Registering all StS1 events with RitsuLib...");

        var content = RitsuLibFramework.CreateContentPack(modId);

        // Shared events
        content.SharedEvent<Sts1GoldenWing>();
        content.SharedEvent<Sts1LivingWall>();
        content.SharedEvent<Sts1OldBeggar>();
        content.SharedEvent<Sts1Purifier>();
        content.SharedEvent<Sts1GoldenShrine>();
        content.SharedEvent<Sts1BonfireSpirits>();
        content.SharedEvent<Sts1DivineFountain>();
        // Sts1Duplicator is excluded from compilation until the needed card-selection APIs are available.
        content.SharedEvent<Sts1FountainOfCleansing>();
        content.SharedEvent<Sts1TheLab>();
        content.SharedEvent<Sts1FaceTrader>();
        content.SharedEvent<Sts1TheMausoleum>();
        content.SharedEvent<Sts1Designer>();
        content.SharedEvent<Sts1TheWomanInBlue>();
        content.SharedEvent<Sts1WheelOfChange>();

        // StS1 Act 1 -> StS2 Overgrowth + Underdocks
        content.ActEvent<Overgrowth, Sts1BigFish>();
        content.ActEvent<Underdocks, Sts1BigFish>();
        content.ActEvent<Overgrowth, Sts1GoldenIdol>();
        content.ActEvent<Underdocks, Sts1GoldenIdol>();
        content.ActEvent<Overgrowth, Sts1TheCleric>();
        content.ActEvent<Underdocks, Sts1TheCleric>();
        content.ActEvent<Overgrowth, Sts1ShiningLight>();
        content.ActEvent<Underdocks, Sts1ShiningLight>();
        content.ActEvent<Overgrowth, Sts1Mushrooms>();
        content.ActEvent<Underdocks, Sts1Mushrooms>();
        content.ActEvent<Overgrowth, Sts1DeadAdventurer>();
        content.ActEvent<Underdocks, Sts1DeadAdventurer>();
        content.ActEvent<Overgrowth, Sts1ScorpionNest>();
        content.ActEvent<Underdocks, Sts1ScorpionNest>();
        content.ActEvent<Overgrowth, Sts1TreasureOoze>();
        content.ActEvent<Underdocks, Sts1TreasureOoze>();
        content.ActEvent<Overgrowth, Sts1Joust>();
        content.ActEvent<Underdocks, Sts1Joust>();
        content.ActEvent<Overgrowth, Sts1TheSsssserpent>();
        content.ActEvent<Underdocks, Sts1TheSsssserpent>();

        // StS1 Act 2 -> StS2 Hive
        content.ActEvent<Hive, Sts1Altar>();
        content.ActEvent<Hive, Sts1DrugDealer>();
        content.ActEvent<Hive, Sts1TheLibrary>();
        content.ActEvent<Hive, Sts1AncientWriting>();
        content.ActEvent<Hive, Sts1Augmenter>();
        content.ActEvent<Hive, Sts1CouncilOfGhosts>();
        content.ActEvent<Hive, Sts1CursedTome>();
        content.ActEvent<Hive, Sts1KnowingSkull>();
        content.ActEvent<Hive, Sts1Nest>();
        content.ActEvent<Hive, Sts1Vampires>();
        content.ActEvent<Hive, Sts1MaskedBandits>();
        content.ActEvent<Hive, Sts1ForgottenAltar>();
        content.ActEvent<Hive, Sts1TheGhost>();
        content.ActEvent<Hive, Sts1Nloth>();

        // StS1 Act 3 -> StS2 Glory
        content.ActEvent<Glory, Sts1SensoryStone>();
        content.ActEvent<Glory, Sts1MoaiHead>();
        content.ActEvent<Glory, Sts1Transmogrifier>();
        content.ActEvent<Glory, Sts1UpgradeShrine>();
        content.ActEvent<Glory, Sts1Falling>();
        content.ActEvent<Glory, Sts1MindBloom>();
        content.ActEvent<Glory, Sts1MysteriousSphere>();
        content.ActEvent<Glory, Sts1TombOfLordRedMask>();
        content.ActEvent<Glory, Sts1WindingHalls>();

        content.Apply();

        logger.Info("[StS1 Events] All StS1 events registered successfully.");
    }
}
