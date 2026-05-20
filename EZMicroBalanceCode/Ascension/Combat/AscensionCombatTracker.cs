using MegaCrit.Sts2.Core.Entities.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class AscensionCombatTracker
{
    public bool Seeded { get; set; }
    public HashSet<RootBud> Buds { get; } = [];
    public HashSet<Player> DiedPlayers { get; } = [];

    public bool CombatModifiersInitialized { get; set; }
    public AscensionNodeMetadata? NodeMetadata { get; set; }
    public Creature? FiremarkHost { get; set; }
    public decimal FiremarkBaseAmount { get; set; }
    public decimal FiremarkOriginalMaxHp { get; set; }
    public decimal FiremarkDamageThisPlayerTurn { get; set; }
    public decimal FiremarkDamageThisEnemyCycle { get; set; }
    public int FiremarkDamageTrackingRound { get; set; }
    public bool FiremarkCoreExposed { get; set; }
    public bool FiremarkCoreResolved { get; set; }
    public decimal FiremarkCoreDamage { get; set; }
    public decimal FiremarkCoreDamageNeeded { get; set; }
    public bool FiremarkArmorSkippedNextTurn { get; set; }
    public bool FiremarkArmorGeneratedThisTurn { get; set; }
    public decimal FiremarkArmorBlockBaseline { get; set; }
    public decimal FiremarkArmorRemainingThisTurn { get; set; }
    public int FiremarkArmorBreaks { get; set; }
    public bool BannerRageApplied { get; set; }
    public bool VanguardStrengthRemoved { get; set; }
    public Creature? ShieldwallBearer { get; set; }
    public int ShieldwallLastBlockRound { get; set; }
    public bool ShieldwallDeathBlockApplied { get; set; }
    public Creature? BloodPrizeTarget { get; set; }
    public bool BloodPrizeKilledEarly { get; set; }
    public bool BloodPrizeExpired { get; set; }
    public bool BloodPrizeRewardAdded { get; set; }
    public bool LastStandTriggered { get; set; }
    public int PressingLineRound { get; set; }
    public Dictionary<Player, int> PressingLineCardsPlayed { get; } = [];
    public Dictionary<Player, int> PressingLineLayers { get; } = [];
    public int PressingLineLastResolvedRound { get; set; }
    public bool BossRageRoundFiveApplied { get; set; }
    public bool BossRageRoundEightApplied { get; set; }
    public bool ChaosApplied { get; set; }
    public bool ForgeTokenAwarded { get; set; }
    public HashSet<Creature> ThresholdShieldedEnemies { get; } = [];

    public bool HolyDazeTriggered { get; set; }
    public bool InkReturnTriggered { get; set; }
    public bool InkReturnPending { get; set; }
    public bool StartledShellApplied { get; set; }
    public bool StartledShellSoulSiphonTurn { get; set; }
    public bool SoulSiphonShellReduced { get; set; }
    public decimal PendingSoulTideBlock { get; set; }
    public int SoulTideBeckonSettlementRound { get; set; }
    public int LastSoulFyshIntangibleAmount { get; set; }
    public int LastSteamEruptionMilestone { get; set; }
    public bool BoilingExplosionBlockGranted { get; set; }
    public int MartyrOathTriggers { get; set; }
    public HashSet<Creature> MisalignedShellBlockedTargetsThisTurn { get; } = [];
    public List<Creature> MisalignedShellClawsDiedThisTurn { get; } = [];
    public bool MisalignedShellArtifactApplied { get; set; }
    public bool KnowledgeDemonCurseMoveActive { get; set; }
    public int FranticEscapesPlayed { get; set; }
    public Dictionary<CardModel, int> StruggleBaitBrandEscapeAges { get; } = [];
    public bool SuppressStruggleBaitStrengthTrigger { get; set; }
    public bool StruggleBaitBaselineCaptured { get; set; }
    public int LastInsatiableStrengthAmount { get; set; }
    public decimal LastInsatiableSandpitAmount { get; set; }
    public CardModel? ChosenDecreeCard { get; set; }
    public bool ChosenDecreePlayed { get; set; }
    public int TestSubjectPhaseChanges { get; set; }
}
