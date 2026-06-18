namespace EZMicroBalance.EZMicroBalanceCode.Ancients.Expansion.Urda;

internal static partial class UrdaBlessingService
{
    private static bool TryGetRootSightPreviewTitle(RootSightPreview preview, out LocString title)
    {
        title = new LocString("ancients", "EZMB_URDA.root_sight.map_hover.title");
        try
        {
            var id = ModelId.Deserialize(preview.ModelId);
            if (preview.RoomType == RoomType.Event)
            {
                var eventModel = ModelDb.GetByIdOrNull<EventModel>(id);
                if (eventModel == null)
                {
                    return false;
                }

                title = eventModel.Title;
                return true;
            }

            var encounter = ModelDb.GetByIdOrNull<EncounterModel>(id);
            if (encounter == null)
            {
                return false;
            }

            title = encounter.Title;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryGetRootSightPreviewDescription(RootSightPreview preview, out LocString description)
    {
        description = new LocString("ancients", "EZMB_URDA.root_sight.map_hover.preview_description");
        if (preview.RoomType != RoomType.Event)
        {
            return false;
        }

        try
        {
            var id = ModelId.Deserialize(preview.ModelId);
            var eventModel = ModelDb.GetByIdOrNull<EventModel>(id);
            if (eventModel == null)
            {
                return false;
            }

            var options = eventModel.GameInfoOptions
                .Select(option => NormalizeRootSightEventOptionPreview(option.GetFormattedText()))
                .Where(option => !string.IsNullOrWhiteSpace(option))
                .Distinct(StringComparer.Ordinal)
                .Take(3)
                .ToList();
            if (options.Count == 0)
            {
                return false;
            }

            var optionPreview = string.Join(" / ", options);
            if (optionPreview.Length > 220)
            {
                optionPreview = optionPreview[..217] + "...";
            }

            description = new LocString("ancients", "EZMB_URDA.root_sight.map_hover.event_preview_description");
            description.Add("Options", optionPreview);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string NormalizeRootSightEventOptionPreview(string text)
    {
        return text
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal)
            .Trim();
    }
}
