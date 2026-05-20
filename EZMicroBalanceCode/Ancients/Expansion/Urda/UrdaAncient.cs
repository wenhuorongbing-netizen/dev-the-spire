using System;
using System.Collections.Generic;

using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;

namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal sealed partial class EzmbUrda : CustomAncientModel
{
    public EzmbUrda()
        : base(autoAdd: false)
    {
    }

    protected override OptionPools MakeOptionPools => new(MakePool(Array.Empty<AncientOption>()));

    public override string? CustomScenePath => UrdaAssetPaths.BackgroundScene;

    public override string? CustomMapIconPath => UrdaAssetPaths.MapIcon;

    public override string? CustomMapIconOutlinePath => UrdaAssetPaths.MapIconOutline;

    public override string? CustomRunHistoryIconPath => UrdaAssetPaths.RunHistoryIcon;

    public override string? CustomRunHistoryIconOutlinePath => UrdaAssetPaths.RunHistoryIconOutline;

    protected override AncientDialogueSet DefineDialogues()
    {
        return new AncientDialogueSet
        {
            FirstVisitEverDialogue = new AncientDialogue(AncientDialogueLine.sfxFallbackPath),
            CharacterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>(),
            AgnosticDialogues = [new AncientDialogue(AncientDialogueLine.sfxFallbackPath)]
        };
    }
}
