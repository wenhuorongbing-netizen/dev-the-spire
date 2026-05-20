using System.Globalization;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal sealed class EzmbVakuuTrialEncounter : CustomEncounterModel
{
    public const string VakuuSlot = "Vakuu";
    public const int MaxLocks = 3;
    public const int DamageLockThreshold = 40;
    public const int GoldPerBrokenLock = 50;

    private const string BrokenLocksKey = "BrokenLocks";
    private const string BloodDebtKey = "BloodDebt";
    private const string DamageRoundKey = "DamageRound";
    private const string DamageThisRoundKey = "DamageThisRound";
    private const string DamageLockRoundKey = "DamageLockRound";

    private int brokenLocks;
    private int bloodDebt;
    private int damageRound = -1;
    private decimal damageThisRound;
    private int damageLockRound = -1;

    public EzmbVakuuTrialEncounter()
        : base(RoomType.Monster, autoAdd: false)
    {
    }

    public override string? CustomScenePath => VakuuFightAssetPaths.EncounterScene;

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

    public int VictoryChoiceCount => Math.Clamp(BrokenLocks + 1, 1, MaxLocks);

    public decimal VictoryGold => BrokenLocks * GoldPerBrokenLock;

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
        [ModelDb.Monster<EzmbVakuuTrialMonster>()];

    public override bool IsValidForAct(ActModel act) => false;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        [(ModelDb.Monster<EzmbVakuuTrialMonster>().ToMutable(), VakuuSlot)];

    public override Dictionary<string, string> SaveCustomState()
    {
        return new Dictionary<string, string>
        {
            [BrokenLocksKey] = BrokenLocks.ToString(),
            [BloodDebtKey] = BloodDebt.ToString(),
            [DamageRoundKey] = DamageRound.ToString(),
            [DamageThisRoundKey] = DamageThisRound.ToString(CultureInfo.InvariantCulture),
            [DamageLockRoundKey] = DamageLockRound.ToString()
        };
    }

    public override void LoadCustomState(Dictionary<string, string> state)
    {
        BrokenLocks = ReadInt(state, BrokenLocksKey);
        BloodDebt = ReadInt(state, BloodDebtKey);
        DamageRound = ReadInt(state, DamageRoundKey, -1);
        DamageThisRound = ReadDecimal(state, DamageThisRoundKey);
        DamageLockRound = ReadInt(state, DamageLockRoundKey, -1);
    }

    private static int ReadInt(IReadOnlyDictionary<string, string> state, string key, int fallback = 0) =>
        state.TryGetValue(key, out var value) && int.TryParse(value, out var parsed)
            ? parsed
            : fallback;

    private static decimal ReadDecimal(IReadOnlyDictionary<string, string> state, string key) =>
        state.TryGetValue(key, out var value) &&
        decimal.TryParse(value, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : 0m;
}
