namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal sealed partial class AscensionCombatTracker
{
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
}
