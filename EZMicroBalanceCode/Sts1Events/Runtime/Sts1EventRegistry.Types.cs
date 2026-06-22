namespace EZMicroBalance.EZMicroBalanceCode.Sts1Events.Runtime;

internal sealed record Sts1EventEntry(
    string Id,
    string DisplayName,
    Sts1EventPhase Phase,
    Sts1EventAct Act);

internal enum Sts1EventPhase
{
    Canary,
    Simple,
    CardService,
    Combat,
    CustomUi,
    Special
}

internal enum Sts1EventAct
{
    Shared,
    Act1,
    Act2,
    Act3
}
