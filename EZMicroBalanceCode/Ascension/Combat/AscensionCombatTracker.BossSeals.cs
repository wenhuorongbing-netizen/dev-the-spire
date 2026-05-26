using MegaCrit.Sts2.Core.Entities.Cards;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class AscensionCombatTracker
{
    public bool BossRageRoundFiveApplied { get; set; }
    public bool BossRageRoundEightApplied { get; set; }
    public bool ChaosApplied { get; set; }

    public bool HolyDazeTriggered { get; set; }
    public bool InkReturnTriggered { get; set; }
    public bool InkReturnPending { get; set; }
    public int InkReturnLastObservedSlippery { get; set; }
    public int InkReturnRestoreAmount { get; set; }
    public bool StartledShellApplied { get; set; }
    public bool StartledShellWakeByPlayerDamagePending { get; set; }
    public bool StartledShellSoulSiphonTurn { get; set; }
    public bool SoulSiphonShellReduced { get; set; }
    public decimal PendingSoulTideBlock { get; set; }
    public int SoulTideBeckonSettlementRound { get; set; }
    public int LastSoulFyshIntangibleAmount { get; set; }
    public int LastSteamEruptionMilestone { get; set; }
    public bool BoilingExplosionFortified { get; set; }
    public int BoilingExplosionArtifactAdded { get; set; }
    public int BoilingExplosionVulnerabilityRound { get; set; }
    public int MartyrOathTriggers { get; set; }
    public int MartyrOathFollowerDeathsThisTurn { get; set; }
    public bool MartyrOathSameTurnArtifactGranted { get; set; }
    public Dictionary<Creature, int> MisalignedShellCalibration { get; } = [];
    public HashSet<Creature> MisalignedShellCalibrationUsed { get; } = [];
    public List<Creature> MisalignedShellClawsDiedThisTurn { get; } = [];
    public bool KnowledgeDemonCurseMoveActive { get; set; }
    public int MarginalDeepThoughtAddedThisRound { get; set; }
    public int MarginalDeepThoughtRound { get; set; }
    public int RoyalEscapesPlayed { get; set; }
    public HashSet<CardModel> StruggleBaitGeneratedEscapes { get; } = [];
    public int StruggleBaitVigorGainRound { get; set; }
    public bool SuppressStruggleBaitStrengthTrigger { get; set; }
    public bool StruggleBaitBaselineCaptured { get; set; }
    public int LastInsatiableStrengthAmount { get; set; }
    public Dictionary<Player, decimal> LastInsatiableSandpitByPlayer { get; } = [];
    public Dictionary<Player, CardModel> ChosenDecreeCardsByPlayer { get; } = [];
    public HashSet<Player> ChosenDecreePlayersWhoPlayedDecree { get; } = [];
    public HashSet<Player> ChosenDecreePlayersWhoPlayedAnyBound { get; } = [];
    public int ChosenDecreeRoundCapRound { get; set; }
    public int ChosenDecreeMajestyGainedThisRound { get; set; }
    public int ChosenDecreeAmalgamStrengthThisRound { get; set; }
    public bool AeonglassEbbMoveActive { get; set; }
    public bool AeonglassIncreasingIntensityMoveActive { get; set; }
    public int AeonglassTimeSand { get; set; }
    public int AeonglassExtraWitherFromSands { get; set; }
    public int AeonglassLaserEchoesUsed { get; set; }
    public int TestSubjectPhaseChanges { get; set; }
    public int TestSubjectAttackCardsThisPhase { get; set; }
    public int TestSubjectSkillCardsThisPhase { get; set; }
    public bool TestSubjectDebuffAppliedThisPhase { get; set; }
    public List<TestSubjectSampleKind> PendingTestSubjectSamples { get; } = [];
    public decimal PendingTestSubjectStrengthResidue { get; set; }
}

internal enum TestSubjectSampleKind
{
    StrengthResidue,
    SkillAdaptation,
    AttackAdaptation,
    AntibodySample,
    ContaminatedSample
}
