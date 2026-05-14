using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Rooms;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Vakuu;

internal sealed class EzmbVakuuTrialEncounter : CustomEncounterModel
{
    public EzmbVakuuTrialEncounter()
        : base(RoomType.Event, autoAdd: false)
    {
    }

    public override bool ShouldGiveRewards => false;

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
        [ModelDb.Monster<OwlMagistrate>()];

    public override bool IsValidForAct(ActModel act) => false;

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
        [(ModelDb.Monster<OwlMagistrate>().ToMutable(), null)];
}
