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
}
"@

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

$process = Get-Process -Name SlayTheSpire2 -ErrorAction SilentlyContinue |
    Where-Object { $_.MainWindowHandle -ne 0 } |
    Select-Object -First 1

if (-not $process) {
    throw 'SlayTheSpire2 is not running with a visible main window.'
}

$foregroundProcessId = Get-ForegroundProcessId
$isForeground = $foregroundProcessId -eq $process.Id
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
    Bounds = [pscustomobject]@{
        Left = $rect.Left
        Top = $rect.Top
        Right = $rect.Right
        Bottom = $rect.Bottom
        Width = $width
        Height = $height
    }
} | ConvertTo-Json -Depth 5
