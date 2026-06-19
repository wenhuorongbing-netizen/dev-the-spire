namespace EZMicroBalance.Tests;

public sealed partial class ReleaseCoverageGuardTests
{
    private sealed record AncientSystemGuard(
        string ManualRow,
        string[] RelicKeys,
        string[] CardKeys,
        string[] RestSiteKeys,
        string[] SourceSnippets);

    private sealed record GatedAscensionSliceGuard(
        string ManualSectionStart,
        string ManualSectionEnd,
        string[] SourceSnippets,
        string[] ApiSnippets,
        string[] ManualSnippets);

    private static readonly AncientSystemGuard[] ImplementedAncientSystems =
    [
        new("Pael's Horn", ["PAELS_HORN.description"], [], [], ["PaelsHorn", "CreateCard<Relax>", "CardCmd.Upgrade(upgradedRelax)"]),
        new("Black Star", ["BLACK_STAR.description"], [], [], ["BlackStar", "RelicFactory.PullNextRelicFromFront"]),
        new("War Hammer", ["WAR_HAMMER.description"], [], [], ["WarHammer", "CardSelectCmd.FromDeckForUpgrade", "CardCmd.Upgrade(cards, CardPreviewStyle.HorizontalLayout)"]),
        new("Jewelry Box", ["JEWELRY_BOX.description"], [], [], ["JewelryBox", "CreateNonInnateApotheosis", "Apotheosis"]),
        new("Preserved Fog / Folly", ["PRESERVED_FOG.description"], ["FOLLY.title", "FOLLY.description"], [], ["PreservedFog", "Folly", "FollyKeywordsPatch"]),
        new("Vakuu's Sere Talon", ["SERE_TALON.description"], [], [], ["option.Relic is PreservedFog or SereTalon"]),
        new("Choices Paradox", ["CHOICES_PARADOX.description"], [], [], ["ChoicesParadox", "ChooseRareTemporaryCard", "CardKeyword.Retain"]),
        new("Jeweled Mask", ["JEWELED_MASK.description", "JEWELED_MASK.ezSelectionScreenPrompt"], [], [], ["JeweledMask", "JeweledMaskFreePower", "CardCmd.Enchant<JeweledMaskFreePower>"]),
        new("Prismatic Gem", ["PRISMATIC_GEM.description"], [], [], ["PrismaticGem", "RewardScreenState", "GetOffColorRewardPool"]),
        new("Distinguished Cape", ["DISTINGUISHED_CAPE.description", "DISTINGUISHED_CAPE.eventDescription"], [], [], ["DistinguishedCape", "CalculateMaxHpLoss", "CreateCard<Apparition>"]),
        new("Velvet Choker", ["VELVET_CHOKER.description"], [], [], ["VelvetChoker", "VelvetChokerSoftLimitTracker", "CardEnergyCost.GetWithModifiers"]),
        new("Pael's Tooth", ["PAELS_TOOTH.description"], [], [], ["PaelsTooth", "PaelsToothNonBossCombatCounter", "ChooseAndReturnStoredCard"]),
        new("Sovereign Blade / Forge", [], [], [], ["ForgeCmd", "SovereignBlade", "CreatedThroughForge"]),
        new("Seal of Gold / Debt", ["SEAL_OF_GOLD.description"], ["DEBT.title", "DEBT.description"], [], ["SealOfGold", "DebtCardPatch", "CreateCard<Debt>"]),
        new("Sozu", ["SOZU.description"], [], [], ["Sozu", "InitialPotionFillOwners", "PotionCmd.TryToProcure"]),
        new("Ectoplasm", ["ECTOPLASM.description"], [], [], ["Ectoplasm", "InitialGoldOwners", "PlayerCmd.GainGold"]),
        new("Fiddle", ["FIDDLE.description"], [], [], ["Fiddle", "FiddleHandLimit", "FiddleDrawCapPatch"]),
        new("Iron Club", ["IRON_CLUB.description"], [], [], ["IronClub", "IronClubVarsPatch", "new CardsVar(5)"]),
        new("Brilliant Scarf", ["BRILLIANT_SCARF.description"], [], [], ["BrilliantScarf", "BrilliantScarfVarsPatch", "new CardsVar(6)"]),
        new("Beautiful Bracelet", ["BEAUTIFUL_BRACELET.description"], [], [], ["BeautifulBracelet", "ModelDb.Enchantment<Swift>", "AddSwiftTwo"]),
        new("Music Box", ["MUSIC_BOX.description"], [], [], ["MusicBox", "MusicBoxStateTracker", "CreateClone"]),
        new("Crossbow", ["CROSSBOW.description"], [], [], ["Crossbow", "OfferTemporaryAttack", "CardSelectCmd.FromChooseACardScreen"]),
        new("Toasty Mittens", ["TOASTY_MITTENS.description"], [], [], ["ToastyMittens", "OfferTopCardExhaust", "StrengthPower"]),
        new("Whispering Earring", ["WHISPERING_EARRING.description"], [], [], ["WhisperingEarring", "AutoPlayOneHighestCostCard", "AutoPlayType.Default"]),
        new("Meat Cleaver", ["MEAT_CLEAVER.description"], [], ["OPTION_COOK.name", "OPTION_COOK.ezDescription", "OPTION_COOK.ezDescriptionDisabled"], ["MeatCleaver", "CookRestSiteOption", "CardsToRemove = 2"]),
        new("Blood-Soaked Rose / Enthralled", ["BLOOD_SOAKED_ROSE.description"], ["ENTHRALLED.title", "ENTHRALLED.description"], [], ["Enthralled", "PlayEnthralled", "GainBlock"])
    ];

    private static readonly GatedAscensionSliceGuard[] ImplementedGatedAscensionSlices =
    [
        new(
            "## A12 Firemarked Elite and Forge Token",
            "## A13 Fission Enchantment",
            [
                "FiremarkedEliteLevel = 12",
                "MarkFiremarkedElite",
                "FiremarkKind.Might",
                "FiremarkKind.Giant",
                "FiremarkKind.ForgeArmor",
                "FiremarkKind.ConstantHeal",
                "AscensionMapQuestMarker",
                "FiremarkedEliteMapQuestMarker",
                "ActOneFiremarkedEliteTargetCount = 2",
                "LaterActFiremarkedEliteTargetCount = 3",
                "PickFiremarkedElitesByAct",
                "FiremarkedEliteMapIconPatch",
                "MightMarkFiremarkPower",
                "GiantMarkFiremarkPower",
                "ForgeArmorMarkFiremarkPower",
                "ConstantHealMarkFiremarkPower",
                "FiremarkMightOverflowPower",
                "FiremarkedEliteRewardTargetOptionCount = 4",
                "ForgeTokenHeld",
                "ForgeTokenRelic",
                "ForgeTokenService.GrantAfterFiremarkedElite",
                "ForgeTokenService.HasToken",
                "CardCmd.Upgrade(extraCard)",
                "ApplyAfterRestSiteHeal",
                "ApplyAfterRestSiteSmith",
                "DuplicateTokenGoldAmount"
            ],
            [
                "Firemarked Elite and Forge Token are implemented for the A12 Ascension-level gate.",
                "One main enemy receives Might, Giant, Forge Armor, or Constant Heal",
                "Overflow affects at most one secondary enemy",
                "Special rest-site action payout is disabled"
            ],
            [
                "Gated implementation present; live testing pending.",
                "Firemarked elite is visible before route commitment.",
                "Defeating firemarked elite grants one visible Forge Token status relic with counter 1.",
                "Forge Token save/load behavior is stable."
            ]),
        new(
            "## A13 Fission Enchantment",
            "## A16 Banner Rooms",
            [
                "FissionLevel = 13",
                "FissionEnchantment",
                "ILocalizationProvider",
                "LocManager.Instance.Language == \"zhs\"",
                "CustomIconPath => AscensionAssetPaths.FissionEnchantmentIcon",
                "TryApplyFission",
                "CardCmd.Enchant<FissionEnchantment>",
                "IsFissionEligible",
                "!card.ExhaustOnNextPlay",
                "CardKeyword.Exhaust"
            ],
            [
                "Fission is implemented for the A13 Ascension-level gate.",
                "Fission reward mutation is source-patched; reward reroll, pickup, localization rendering, Exhaust payoff live behavior, and save/load are pending."
            ],
            [
                "Gated implementation present; live testing pending.",
                "Fission appears only on eligible reward cards.",
                "Tooltip/card text is correct in English and Simplified Chinese, uses energy-cost wording, does not show raw `{energyPrefix:energyIcons(...)}` templates, does not duplicate the added Exhaust line, and does not use the Chinese word \"\u8d39\u7528\" for Fission.",
                "Picked Fission cards save/load correctly."
            ]),
        new(
            "## A16 Banner Rooms",
            "## A17 Deep Branches",
            [
                "BannerRoomLevel = 16",
                "MarkBannerRooms",
                "BannerRoomMapQuestMarker",
                "BannerKind.Vanguard",
                "BannerKind.Shieldwall",
                "BannerKind.BloodPrize",
                "BannerKind.PressingLine",
                "BannerKind.LastStand",
                "ApplyBannerCombatStart",
                "HasActiveBanner"
            ],
            [
                "Banner Rooms are implemented for the A16 Ascension-level gate.",
                "Banner node marking and combat modifiers are source-patched; live route visibility, persistence, reward settlement, and combat cleanup are pending."
            ],
            [
                "Gated implementation present; live testing pending.",
                "Banner rooms are visible before route commitment.",
                "Banner modifiers apply only to the intended combat.",
                "Banner modifiers do not persist into later combats."
            ]),
        new(
            "## A19/A20 Boss Systems",
            "## Disable and Uninstall",
            [
                "BossSealsLevel = 19",
                "DoubleRoyalBrandLevel = 20",
                "MarkBossSeals",
                "BossSealDefinition",
                "BossSealCatalog",
                "BossSealImplementationStatus.SourceGuardedPendingLiveVerification",
                "HolyDaze",
                "HolyDazePower",
                "MarginalNote",
                "RoyalDecreeEnchantment",
                "AeonglassHourglass",
                "AeonglassLaserEchoIntentLabelPatch",
                "IsBossBrand",
                "AscensionA20GenerateRoomsPatch",
                "AscensionA20CourtyardProceedPatch",
                "A20Courtyard",
                "AscensionA20RewardScreenReadyPatch",
                "A20_INTERMISSION_HEADER",
                "BossMapPointHoverPatch",
                "BOSS_BRANDED_FORM",
                "TryAddBossSealRewardOption",
                "BossRewardTargetOptionCount = 4",
                "TryAddA20BossOneCardReward"
            ],
            [
                "`BossSealDefinition` / `BossSealCatalog` now map active boss encounters to the v4.1 dedicated Boss ability set",
                "source-guarded through supported hooks",
                "Boss 1 post-combat recovery",
                "fixed courtyard event",
                "vanilla double-boss map path"
            ],
            [
                "Gated implementation present as BossSeal definitions plus source-guarded runtime hooks; live testing pending.",
                "A19 boss-specific dedicated ability metadata is assigned at map generation.",
                "Boss card rewards improve as documented.",
                "Boss 1 reward screen opens the A20 courtyard event before the second Boss."
            ])
    ];
}
