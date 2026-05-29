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
/// Registers StS1 events with RitsuLib's ModContentRegistry.
///
/// StS2 act mapping (verified from UrdaAct1AncientService, MorviAct2AncientService, LothaAct3AncientService):
///   StS1 Act 1 events → Overgrowth + Underdocks (both acts)
///   StS1 Act 2 events → Hive
///   StS1 Act 3 events → Glory
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
            case Sts1EventRegistrationMode.AdditiveAllDraft:
            case Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype:
                RegisterAll(modId);
                return;
        }
    }

    /// <summary>
    /// Registers exactly 4 canary events (all Shared): Big Fish, Golden Idol, Lab, Divine Fountain.
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

        content.SharedEvent<Sts1BigFish>();
        content.SharedEvent<Sts1GoldenIdol>();
        content.SharedEvent<Sts1TheLab>();
        content.SharedEvent<Sts1DivineFountain>();

        content.Apply();

        logger.Info("[StS1 Events] Canary events registered successfully.");
    }

    /// <summary>
    /// Registers all drafted StS1 events. Used by AdditiveAllDraft and ReplaceUnknownEventsPrototype modes.
    /// Sts1Duplicator excluded — uses CardSelectCmd/CardPileCmd APIs not yet available.
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

        // ── Shared events ──────────────────────────────────────────────
        content.SharedEvent<Sts1BigFish>();
        content.SharedEvent<Sts1GoldenIdol>();
        content.SharedEvent<Sts1TheCleric>();
        content.SharedEvent<Sts1GoldenWing>();
        content.SharedEvent<Sts1LivingWall>();
        content.SharedEvent<Sts1OldBeggar>();
        content.SharedEvent<Sts1BonfireSpirits>();
        content.SharedEvent<Sts1DivineFountain>();
        // Sts1Duplicator excluded from compilation — uses CardSelectCmd/CardPileCmd APIs not yet available.
        content.SharedEvent<Sts1FountainOfCleansing>();
        content.SharedEvent<Sts1TheLab>();
        content.SharedEvent<Sts1FaceTrader>();
        content.SharedEvent<Sts1TheMausoleum>();
        content.SharedEvent<Sts1Designer>();
        content.SharedEvent<Sts1TheWomanInBlue>();
        content.SharedEvent<Sts1WheelOfChange>();

        // ── StS1 Act 1 → StS2 Overgrowth + Underdocks ─────────────────
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

        // ── StS1 Act 2 → StS2 Hive ────────────────────────────────────
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

        // ── StS1 Act 3 → StS2 Glory ───────────────────────────────────
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
