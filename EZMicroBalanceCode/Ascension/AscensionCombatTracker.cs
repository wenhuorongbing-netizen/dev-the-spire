using MegaCrit.Sts2.Core.Entities.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed class AscensionCombatTracker
{
    public bool Seeded { get; set; }
    public HashSet<RootBud> Buds { get; } = [];
    public HashSet<Player> DiedPlayers { get; } = [];

    public bool CombatModifiersInitialized { get; set; }
    public AscensionNodeMetadata? NodeMetadata { get; set; }
    public bool BannerRageApplied { get; set; }
    public bool VanguardStrengthRemoved { get; set; }
    public Creature? ShieldFormationBearer { get; set; }
    public bool ShieldFormationDeathBlockApplied { get; set; }
    public Creature? BountyTarget { get; set; }
    public bool BountyKilledEarly { get; set; }
    public bool BountyExpired { get; set; }
    public bool BountyRewardAdded { get; set; }
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
    public bool DoorWedgeTriggered { get; set; }
    public int DoorWedgeAttacksPlayed { get; set; }
    public CardModel? ChosenDecreeCard { get; set; }
    public bool ChosenDecreePlayed { get; set; }
    public int TestSubjectPhaseChanges { get; set; }
}
