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
/// Registers all StS1 events with RitsuLib's ModContentRegistry.
/// StS2 acts: Underdocks=Act1, Overgrowth=Act2, Hive=Act3.
/// </summary>
internal static class Sts1EventRegistrationService
{
    public static void RegisterAll(string modId)
    {
        if (!RitsuLibFramework.IsActive)
        {
            MainFile.Logger.Warn("[StS1 Events] RitsuLib not active; skipping event registration.");
            return;
        }

        var logger = RitsuLibFramework.CreateLogger(modId);
        logger.Info("[StS1 Events] Registering StS1 events with RitsuLib...");

        var content = RitsuLibFramework.CreateContentPack(modId);

        // Phase 1: Canary — shared events
        content.SharedEvent<Sts1BigFish>();
        content.SharedEvent<Sts1GoldenIdol>();

        // Phase 2: Simple batch — shared
        content.SharedEvent<Sts1TheCleric>();
        content.SharedEvent<Sts1GoldenWing>();
        content.SharedEvent<Sts1LivingWall>();
        content.SharedEvent<Sts1OldBeggar>();
        content.SharedEvent<Sts1BonfireSpirits>();
        content.SharedEvent<Sts1DivineFountain>();
        content.SharedEvent<Sts1Duplicator>();
        content.SharedEvent<Sts1FountainOfCleansing>();
        content.SharedEvent<Sts1TheLab>();

        // Phase 2: Simple batch — Act 1 (Underdocks)
        content.ActEvent<Underdocks, Sts1ShiningLight>();
        content.ActEvent<Underdocks, Sts1Mushrooms>();

        // Phase 2: Simple batch — Act 2 (Overgrowth)
        content.ActEvent<Overgrowth, Sts1Altar>();
        content.ActEvent<Overgrowth, Sts1DrugDealer>();
        content.ActEvent<Overgrowth, Sts1TheLibrary>();
        content.ActEvent<Overgrowth, Sts1AncientWriting>();
        content.ActEvent<Overgrowth, Sts1Augmenter>();

        // Phase 2: Simple batch — Act 3 (Hive)
        content.ActEvent<Hive, Sts1SensoryStone>();
        content.ActEvent<Hive, Sts1MoaiHead>();
        content.ActEvent<Hive, Sts1Transmogrifier>();
        content.ActEvent<Hive, Sts1UpgradeShrine>();

        // Phase 3: Card service — shared
        content.SharedEvent<Sts1FaceTrader>();
        content.SharedEvent<Sts1TheMausoleum>();
        content.SharedEvent<Sts1Designer>();

        // Phase 3: Card service — Act 2 (Overgrowth)
        content.ActEvent<Overgrowth, Sts1CouncilOfGhosts>();
        content.ActEvent<Overgrowth, Sts1CursedTome>();
        content.ActEvent<Overgrowth, Sts1KnowingSkull>();
        content.ActEvent<Overgrowth, Sts1Nest>();
        content.ActEvent<Overgrowth, Sts1Vampires>();

        // Phase 3: Card service — Act 3 (Hive)
        content.ActEvent<Hive, Sts1Falling>();
        content.ActEvent<Hive, Sts1MindBloom>();

        // Phase 4: Combat — Act 1 (Underdocks)
        content.ActEvent<Underdocks, Sts1DeadAdventurer>();
        content.ActEvent<Underdocks, Sts1ScorpionNest>();
        content.ActEvent<Underdocks, Sts1TreasureOoze>();
        content.ActEvent<Underdocks, Sts1Joust>();
        content.ActEvent<Underdocks, Sts1TheSsssserpent>();

        // Phase 4: Combat — Act 2 (Overgrowth)
        content.ActEvent<Overgrowth, Sts1MaskedBandits>();

        // Phase 4: Combat — Act 3 (Hive)
        content.ActEvent<Hive, Sts1MysteriousSphere>();

        // Phase 5: Custom UI (simplified) — shared
        content.SharedEvent<Sts1TheWomanInBlue>();
        content.SharedEvent<Sts1WheelOfChange>();

        // Phase 5: Custom UI — Act 2 (Overgrowth)
        content.ActEvent<Overgrowth, Sts1ForgottenAltar>();
        content.ActEvent<Overgrowth, Sts1TheGhost>();
        content.ActEvent<Overgrowth, Sts1Nloth>();

        // Phase 5: Custom UI — Act 3 (Hive)
        content.ActEvent<Hive, Sts1TombOfLordRedMask>();
        content.ActEvent<Hive, Sts1WindingHalls>();

        content.Apply();

        logger.Info("[StS1 Events] All StS1 events registered successfully.");
    }
}
