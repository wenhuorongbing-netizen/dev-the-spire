using BaseLib.Abstracts;
using BaseLib.Extensions;
using EzDailyContent.EzDailyContentCode.Extensions;
using Godot;

namespace EzDailyContent.EzDailyContentCode.Powers;

public abstract class EzDailyContentPower : CustomPowerModel
{
    //Loads from EzDailyContent/images/powers/your_power.png
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}