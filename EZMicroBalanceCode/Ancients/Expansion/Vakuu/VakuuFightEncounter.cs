using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal sealed partial class EzmbVakuuTrialEncounter : ModEncounterTemplate
{
    public const string VakuuSlot = "Vakuu";
    public const int MaxLocks = 3;
    public const int DamageLockThreshold = 40;
    public const int GoldPerBrokenLock = 50;
    public const int GoldCostPerBloodDebt = 15;
    public const int HpLossPerDebtShortfall = 3;

    private int brokenLocks;
    private int bloodDebt;
    private int damageRound = -1;
    private decimal damageThisRound;
    private int damageLockRound = -1;
    private int cashOutOfferedLock;
    private bool cashedOut;

    public override RoomType RoomType => RoomType.Monster;

    public override string? CustomEncounterScenePath => VakuuFightAssetPaths.EncounterScene;

    public override bool HasScene => true;

    public override bool ShouldGiveRewards => false;

    public override IReadOnlyList<string> Slots => [VakuuSlot];

    public int BrokenLocks
    {
        get => brokenLocks;
        set
        {
            AssertMutable();
            brokenLocks = Math.Clamp(value, 0, MaxLocks);
        }
    }

    public int RemainingLocks => Math.Max(0, MaxLocks - BrokenLocks);

    public int BloodDebt
    {
        get => bloodDebt;
        set
        {
            AssertMutable();
            bloodDebt = Math.Max(0, value);
        }
    }

    public int DamageRound
    {
        get => damageRound;
        set
        {
            AssertMutable();
            damageRound = value;
        }
    }

    public decimal DamageThisRound
    {
        get => damageThisRound;
        set
        {
            AssertMutable();
            damageThisRound = Math.Max(0m, value);
        }
    }

    public int DamageLockRound
    {
        get => damageLockRound;
        set
        {
            AssertMutable();
            damageLockRound = value;
        }
    }

    public int CashOutOfferedLock
    {
        get => cashOutOfferedLock;
        set
        {
            AssertMutable();
            cashOutOfferedLock = Math.Clamp(value, 0, MaxLocks);
        }
    }

    public bool CashedOut
    {
        get => cashedOut;
        set
        {
            AssertMutable();
            cashedOut = value;
        }
    }

    public int VictoryChoiceCount => Math.Clamp(BrokenLocks + 1, 1, MaxLocks);

    public decimal VictoryLootGold => BrokenLocks * GoldPerBrokenLock;

    public decimal BloodDebtGoldCost => BloodDebt * GoldCostPerBloodDebt;

    public decimal VictoryGold => Math.Max(0m, VictoryLootGold - BloodDebtGoldCost);

    public decimal BloodDebtShortfall => Math.Max(0m, BloodDebtGoldCost - VictoryLootGold);

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
        [ModelDb.Monster<EzmbVakuuTrialMonster>()];

    public override bool IsValidForAct(ActModel act) => false;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        [(ModelDb.Monster<EzmbVakuuTrialMonster>().ToMutable(), VakuuSlot)];
}
