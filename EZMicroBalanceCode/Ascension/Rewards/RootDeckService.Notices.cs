using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;

namespace EZMicroBalance.EZMicroBalanceCode.Ascension;

internal static partial class RootDeckService
{
    private const double RootblightNoticeSeconds = 5.0;

    private static void ShowRootSystemFull(Player player)
    {
        ShowLocalRootblightNotice(
            player,
            new LocString("ascension", "ROOT_SYSTEM_FULL"),
            "cap");
    }

    private static void ShowRootblightAdded(Player player, bool preferOverlayNotice)
    {
        ShowLocalRootblightNotice(
            player,
            new LocString("ascension", "ROOTBLIGHT_ADDED"),
            "add",
            preferOverlayNotice);
    }

    private static void ShowLocalRootblightNotice(
        Player player,
        LocString line,
        string noticeKind,
        bool preferOverlayNotice = false)
    {
        if (!LocalContext.IsMe(player))
        {
            return;
        }

        try
        {
            if (preferOverlayNotice && TryShowRunOverlayNotice(line))
            {
                return;
            }

            var creatureVfxContainer = player.Creature.GetVfxContainer();
            if (creatureVfxContainer != null)
            {
                ThinkCmd.Play(line, player.Creature, RootblightNoticeSeconds);
                return;
            }

            if (TryShowEventRoomNotice(line))
            {
                return;
            }

            TryShowRunOverlayNotice(line);
        }
        catch (Exception ex)
        {
            MainFile.Logger.Warn($"[Spire Plus] Ascension Rootblight {noticeKind} notice could not be displayed: {ex.Message}");
        }
    }

    private static bool TryShowEventRoomNotice(LocString line)
    {
        var container = NEventRoom.Instance?.VfxContainer;
        if (container == null)
        {
            return false;
        }

        var bubble = NThoughtBubbleVfx.Create(line.GetFormattedText(), DialogueSide.Left, RootblightNoticeSeconds);
        if (bubble == null)
        {
            return false;
        }

        container.AddChildSafely(bubble);
        PrepareOverlayNotice(bubble);
        bubble.GlobalPosition = container.GlobalPosition + new Vector2(220f, MathF.Max(180f, container.Size.Y * 0.55f));
        return true;
    }

    private static bool TryShowRunOverlayNotice(LocString line)
    {
        return TryShowTopLevelRunNotice(line) || TryShowGlobalRunNotice(line);
    }

    private static bool TryShowTopLevelRunNotice(LocString line)
    {
        var container = NGame.Instance;
        if (container == null)
        {
            return false;
        }

        var bubble = NThoughtBubbleVfx.Create(line.GetFormattedText(), DialogueSide.Left, RootblightNoticeSeconds);
        if (bubble == null)
        {
            return false;
        }

        container.AddChildSafely(bubble);
        PrepareOverlayNotice(bubble);
        bubble.GlobalPosition = new Vector2(110f, 90f);
        return true;
    }

    private static bool TryShowGlobalRunNotice(LocString line)
    {
        var container = NRun.Instance?.GlobalUi.AboveTopBarVfxContainer;
        if (container == null)
        {
            return false;
        }

        var bubble = NThoughtBubbleVfx.Create(line.GetFormattedText(), DialogueSide.Left, RootblightNoticeSeconds);
        if (bubble == null)
        {
            return false;
        }

        container.AddChildSafely(bubble);
        PrepareOverlayNotice(bubble);
        bubble.GlobalPosition = container.GlobalPosition + new Vector2(220f, 180f);
        return true;
    }

    private static void PrepareOverlayNotice(NThoughtBubbleVfx bubble)
    {
        bubble.MouseFilter = Control.MouseFilterEnum.Ignore;
        bubble.ZAsRelative = false;
        bubble.ZIndex = 4096;
    }
}
