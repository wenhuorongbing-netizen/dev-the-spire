using STS2RitsuLib;
using STS2RitsuLib.Content;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Content;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act1;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act2;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Act3;
using EZMicroBalance.EZMicroBalanceCode.Sts1Events.Models.Shared;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

internal static partial class Sts1EventRegistrationService
{
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

        // Shared events are not act-bound in RitsuLib; the game can offer them
        // wherever the runtime event pool accepts shared event definitions.
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

        // Golden Idol's Take branch grants this custom relic (StS1 parity).
        // Event-only marker relic (never in pools/shops/Neow); registered here
        // so the all-draft mode matches CanaryOnly.
        content.Relic<SharedRelicPool, Sts1GoldenIdolRelic>();

        content.Apply();

        logger.Info("[StS1 Events] All StS1 events registered successfully.");
    }
}
