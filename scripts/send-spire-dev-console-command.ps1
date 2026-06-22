param(
    [Parameter(Mandatory = $true)]
    [string]$Command,

    [int]$InitialDelayMs = 500,

    [int]$AfterOpenDelayMs = 300,

    [int]$AfterEnterDelayMs = 3000,

    [switch]$AssumeConsoleOpen,

    [switch]$ClearExistingInput = $true
)

$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Windows.Forms

Add-Type @"
using System;
using System.Runtime.InteropServices;

public static class SpireConsoleCommandNative {
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr SetActiveWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();

    [DllImport("user32.dll")]
    public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
}
"@

function Assert-SendKeysSafe {
    param([Parameter(Mandatory = $true)][string]$Value)

    if ($Value.IndexOfAny(@('{'[0], '}'[0], '+'[0], '^'[0], '%'[0], '~'[0], '('[0], ')'[0], '['[0], ']'[0])) -ge 0) {
        throw 'Command contains SendKeys control characters. Use a simpler command or type it manually.'
    }
}

function Get-ForegroundProcessId {
    $handle = [SpireConsoleCommandNative]::GetForegroundWindow()
    if ($handle -eq [IntPtr]::Zero) {
        return $null
    }

    [uint32]$processId = 0
    [void][SpireConsoleCommandNative]::GetWindowThreadProcessId($handle, [ref]$processId)
    if ($processId -eq 0) {
        return $null
    }

    return [int]$processId
}

function Request-SpireForeground {
    param([Parameter(Mandatory = $true)][IntPtr]$WindowHandle)

    $foregroundWindow = [SpireConsoleCommandNative]::GetForegroundWindow()
    [uint32]$foregroundProcessId = 0
    [uint32]$foregroundThreadId = 0
    if ($foregroundWindow -ne [IntPtr]::Zero) {
        $foregroundThreadId = [SpireConsoleCommandNative]::GetWindowThreadProcessId($foregroundWindow, [ref]$foregroundProcessId)
    }

    [uint32]$targetProcessId = 0
    $targetThreadId = [SpireConsoleCommandNative]::GetWindowThreadProcessId($WindowHandle, [ref]$targetProcessId)
    $currentThreadId = [SpireConsoleCommandNative]::GetCurrentThreadId()
    $attachedForeground = $false
    $attachedTarget = $false
    try {
        if ($foregroundThreadId -ne 0 -and $foregroundThreadId -ne $currentThreadId) {
            $attachedForeground = [SpireConsoleCommandNative]::AttachThreadInput($currentThreadId, $foregroundThreadId, $true)
        }
        if ($targetThreadId -ne 0 -and $targetThreadId -ne $currentThreadId) {
            $attachedTarget = [SpireConsoleCommandNative]::AttachThreadInput($currentThreadId, $targetThreadId, $true)
        }

        [void][SpireConsoleCommandNative]::ShowWindow($WindowHandle, 9)
        [void][SpireConsoleCommandNative]::BringWindowToTop($WindowHandle)
        [void][SpireConsoleCommandNative]::SetForegroundWindow($WindowHandle)
        [void][SpireConsoleCommandNative]::SetActiveWindow($WindowHandle)
        [void][SpireConsoleCommandNative]::SetFocus($WindowHandle)
    } finally {
        if ($attachedTarget) {
            [void][SpireConsoleCommandNative]::AttachThreadInput($currentThreadId, $targetThreadId, $false)
        }
        if ($attachedForeground) {
            [void][SpireConsoleCommandNative]::AttachThreadInput($currentThreadId, $foregroundThreadId, $false)
        }
    }
}

function Wait-SpireForeground {
    param(
        [Parameter(Mandatory = $true)][IntPtr]$WindowHandle,
        [Parameter(Mandatory = $true)][int]$ProcessId,
        [int]$TimeoutMs = 3000
    )

    $deadline = (Get-Date).AddMilliseconds($TimeoutMs)
    do {
        Request-SpireForeground -WindowHandle $WindowHandle
        Start-Sleep -Milliseconds 100
        if ((Get-ForegroundProcessId) -eq $ProcessId) {
            return $true
        }
    } while ((Get-Date) -lt $deadline)

    return (Get-ForegroundProcessId) -eq $ProcessId
}

Assert-SendKeysSafe -Value $Command

$process = Get-Process -Name SlayTheSpire2 -ErrorAction SilentlyContinue |
    Where-Object { $_.MainWindowHandle -ne 0 } |
    Select-Object -First 1

if (-not $process) {
    throw 'SlayTheSpire2 is not running with a visible main window.'
}

$foregroundBefore = Get-ForegroundProcessId
$foregroundReady = Wait-SpireForeground -WindowHandle $process.MainWindowHandle -ProcessId $process.Id
$foregroundAfterFocus = Get-ForegroundProcessId
if (-not $foregroundReady) {
    throw "SlayTheSpire2 is running but could not become the foreground window. Foreground process id: $foregroundAfterFocus"
}
Start-Sleep -Milliseconds $InitialDelayMs

if (-not $AssumeConsoleOpen) {
    [System.Windows.Forms.SendKeys]::SendWait("'")
    Start-Sleep -Milliseconds $AfterOpenDelayMs
}

$clipboardHadText = $false
$previousClipboardText = ''
try {
    $clipboardHadText = [System.Windows.Forms.Clipboard]::ContainsText()
    if ($clipboardHadText) {
        $previousClipboardText = [System.Windows.Forms.Clipboard]::GetText()
    }
} catch {
    $clipboardHadText = $false
    $previousClipboardText = ''
}

try {
    if ($ClearExistingInput) {
        [System.Windows.Forms.SendKeys]::SendWait("^{a}")
        Start-Sleep -Milliseconds 50
        [System.Windows.Forms.SendKeys]::SendWait("{BACKSPACE}")
        Start-Sleep -Milliseconds 50
    }

    [System.Windows.Forms.Clipboard]::SetText($Command)
    [System.Windows.Forms.SendKeys]::SendWait("^v")
    Start-Sleep -Milliseconds 50
    [System.Windows.Forms.SendKeys]::SendWait("{ENTER}")
    Start-Sleep -Milliseconds $AfterEnterDelayMs
} finally {
    if ($clipboardHadText) {
        [System.Windows.Forms.Clipboard]::SetText($previousClipboardText)
    }
}

[pscustomobject]@{
    SentAt = (Get-Date).ToString('o')
    ProcessId = $process.Id
    MainWindowTitle = $process.MainWindowTitle
    Command = $Command
    AssumeConsoleOpen = [bool]$AssumeConsoleOpen
    ClearExistingInput = [bool]$ClearExistingInput
    UsedClipboardPaste = $true
    ClipboardHadTextBefore = [bool]$clipboardHadText
    ForegroundProcessIdBeforeFocus = $foregroundBefore
    ForegroundProcessIdAfterFocus = $foregroundAfterFocus
    ForegroundReady = [bool]$foregroundReady
} | ConvertTo-Json -Depth 4
