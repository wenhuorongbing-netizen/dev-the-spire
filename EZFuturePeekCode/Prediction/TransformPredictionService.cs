using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace EZFuturePeek.EZFuturePeekCode.Prediction;

internal static class TransformPredictionService
{
    public static CardModel? PredictReplacementModel(CardTransformation transformation, Rng rng)
    {
        if (transformation.Replacement != null)
        {
            return transformation.Replacement;
        }

        if (!transformation.Original.IsTransformable)
        {
            return null;
        }

        var options = transformation.ReplacementOptions == null
            ? CardFactory.GetDefaultTransformationOptions(transformation.Original, transformation.IsInCombat)
            : FilterLikeVanilla(transformation.Original, transformation.ReplacementOptions, transformation.IsInCombat);

        var optionArray = options.ToArray();
        return optionArray.Length == 0 ? null : rng.NextItem(optionArray);
    }

    private static IEnumerable<CardModel> FilterLikeVanilla(
        CardModel original,
        IEnumerable<CardModel> originalOptions,
        bool isInCombat)
    {
        var source = originalOptions;
        var rarity = original.Rarity;
        if ((uint)(rarity - 8) > 1u)
        {
            source = source.Where(card =>
            {
                var optionRarity = card.Rarity;
                return (uint)(optionRarity - 2) <= 2u;
            });
        }

        if (isInCombat)
        {
            source = source.Where(card => card.CanBeGeneratedInCombat);
        }

        source = source.Where(card => card.Id != original.Id).ToList();

        return original.Owner.RunState.Players.Count > 1
            ? source.Where(card => card.MultiplayerConstraint != CardMultiplayerConstraint.SingleplayerOnly)
            : source.Where(card => card.MultiplayerConstraint != CardMultiplayerConstraint.MultiplayerOnly);
    }
}
