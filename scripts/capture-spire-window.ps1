param(
    [Parameter(Mandatory = $true)]
    [string]$OutFile,

    [switch]$RequireSpireForeground
)

$ErrorActionPreference = 'Stop'

Add-Type -AssemblyName System.Drawing

Add-Type @"
using System;
using System.Runtime.InteropServices;
using System.Text;

public static class SpireCaptureNative {
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

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

    [DllImport("user32.dll")]
    public static extern bool SetProcessDpiAwarenessContext(IntPtr dpiContext);
}
"@

# Windows returns logical window bounds to a DPI-unaware PowerShell process while
# CopyFromScreen reads physical pixels. Mark the helper as per-monitor DPI-aware
# before reading bounds so evidence screenshots include the full game window on
# scaled displays such as 150% laptop panels.
$dpiAwarenessPerMonitorV2 = [IntPtr](-4)
[void][SpireCaptureNative]::SetProcessDpiAwarenessContext($dpiAwarenessPerMonitorV2)

function Get-ForegroundProcessId {
    $handle = [SpireCaptureNative]::GetForegroundWindow()
    if ($handle -eq [IntPtr]::Zero) {
        return $null
    }

    [uint32]$processId = 0
    [void][SpireCaptureNative]::GetWindowThreadProcessId($handle, [ref]$processId)
    if ($processId -eq 0) {
        return $null
    }

    return [int]$processId
}

function Request-SpireForeground {
    param([Parameter(Mandatory = $true)][IntPtr]$WindowHandle)

    $foregroundWindow = [SpireCaptureNative]::GetForegroundWindow()
    [uint32]$foregroundProcessId = 0
    [uint32]$foregroundThreadId = 0
    if ($foregroundWindow -ne [IntPtr]::Zero) {
        $foregroundThreadId = [SpireCaptureNative]::GetWindowThreadProcessId($foregroundWindow, [ref]$foregroundProcessId)
    }

    [uint32]$targetProcessId = 0
    $targetThreadId = [SpireCaptureNative]::GetWindowThreadProcessId($WindowHandle, [ref]$targetProcessId)
    $currentThreadId = [SpireCaptureNative]::GetCurrentThreadId()
    $attachedForeground = $false
    $attachedTarget = $false
    try {
        if ($foregroundThreadId -ne 0 -and $foregroundThreadId -ne $currentThreadId) {
            $attachedForeground = [SpireCaptureNative]::AttachThreadInput($currentThreadId, $foregroundThreadId, $true)
        }
        if ($targetThreadId -ne 0 -and $targetThreadId -ne $currentThreadId) {
            $attachedTarget = [SpireCaptureNative]::AttachThreadInput($currentThreadId, $targetThreadId, $true)
        }

        [void][SpireCaptureNative]::ShowWindow($WindowHandle, 9)
        [void][SpireCaptureNative]::BringWindowToTop($WindowHandle)
        [void][SpireCaptureNative]::SetForegroundWindow($WindowHandle)
        [void][SpireCaptureNative]::SetActiveWindow($WindowHandle)
        [void][SpireCaptureNative]::SetFocus($WindowHandle)
    } finally {
        if ($attachedTarget) {
            [void][SpireCaptureNative]::AttachThreadInput($currentThreadId, $targetThreadId, $false)
        }
        if ($attachedForeground) {
            [void][SpireCaptureNative]::AttachThreadInput($currentThreadId, $foregroundThreadId, $false)
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

$process = Get-Process -Name SlayTheSpire2 -ErrorAction SilentlyContinue |
    Where-Object { $_.MainWindowHandle -ne 0 } |
    Select-Object -First 1

if (-not $process) {
    throw 'SlayTheSpire2 is not running with a visible main window.'
}

$foregroundProcessId = Get-ForegroundProcessId
$isForeground = $foregroundProcessId -eq $process.Id
if ($RequireSpireForeground -and -not $isForeground) {
    $isForeground = Wait-SpireForeground -WindowHandle $process.MainWindowHandle -ProcessId $process.Id
    $foregroundProcessId = Get-ForegroundProcessId
}
if ($RequireSpireForeground -and -not $isForeground) {
    Write-Error "SlayTheSpire2 is running but is not the foreground window. Foreground process id: $foregroundProcessId"
    exit 2
}

$rect = New-Object SpireCaptureNative+RECT
if (-not [SpireCaptureNative]::GetWindowRect($process.MainWindowHandle, [ref]$rect)) {
    throw 'Could not read SlayTheSpire2 window bounds.'
}

$width = $rect.Right - $rect.Left
$height = $rect.Bottom - $rect.Top
if ($width -le 0 -or $height -le 0) {
    throw "Invalid SlayTheSpire2 window bounds: $width x $height."
}

$parent = Split-Path -Parent $OutFile
if ($parent) {
    New-Item -ItemType Directory -Path $parent -Force | Out-Null
}

$bitmap = [System.Drawing.Bitmap]::new($width, $height)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
try {
    $graphics.CopyFromScreen($rect.Left, $rect.Top, 0, 0, [System.Drawing.Size]::new($width, $height))
    $bitmap.Save($OutFile, [System.Drawing.Imaging.ImageFormat]::Png)
} finally {
    $graphics.Dispose()
    $bitmap.Dispose()
}

[pscustomobject]@{
    CapturedAt = (Get-Date).ToString('o')
    OutFile = (Resolve-Path -LiteralPath $OutFile).Path
    ProcessId = $process.Id
    MainWindowTitle = $process.MainWindowTitle
    SpireForeground = $isForeground
    ForegroundProcessId = $foregroundProcessId
    Bounds = [pscustomobject]@{
        Left = $rect.Left
        Top = $rect.Top
        Right = $rect.Right
        Bottom = $rect.Bottom
        Width = $width
        Height = $height
    }
} | ConvertTo-Json -Depth 5
