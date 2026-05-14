param(
    [switch]$RequireSpireForeground,

    [string]$OutFile
)

$ErrorActionPreference = 'Stop'

Add-Type @"
using System;
using System.Runtime.InteropServices;
using System.Text;

public static class SpireWindowPreflightNative {
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
}
"@

function Get-WindowTitle {
    param([IntPtr]$Handle)

    if ($Handle -eq [IntPtr]::Zero) {
        return ''
    }

    $builder = [System.Text.StringBuilder]::new(1024)
    [void][SpireWindowPreflightNative]::GetWindowText($Handle, $builder, $builder.Capacity)
    return $builder.ToString()
}

function Get-ForegroundProcess {
    $handle = [SpireWindowPreflightNative]::GetForegroundWindow()
    if ($handle -eq [IntPtr]::Zero) {
        return $null
    }

    [uint32]$processId = 0
    [void][SpireWindowPreflightNative]::GetWindowThreadProcessId($handle, [ref]$processId)
    if ($processId -eq 0) {
        return $null
    }

    $process = Get-Process -Id ([int]$processId) -ErrorAction SilentlyContinue
    if (-not $process) {
        return $null
    }

    return [pscustomobject]@{
        ProcessName = $process.ProcessName
        Id = $process.Id
        MainWindowTitle = $process.MainWindowTitle
        ForegroundWindowTitle = Get-WindowTitle -Handle $handle
    }
}

$foreground = Get-ForegroundProcess
$spireProcess = Get-Process -Name SlayTheSpire2 -ErrorAction SilentlyContinue |
    Where-Object { $_.MainWindowHandle -ne 0 } |
    Select-Object -First 1

$visibleWindows = @(Get-Process |
    Where-Object { $_.MainWindowHandle -ne 0 } |
    Sort-Object ProcessName, Id |
    ForEach-Object {
        [pscustomobject]@{
            ProcessName = $_.ProcessName
            Id = $_.Id
            MainWindowTitle = $_.MainWindowTitle
        }
    })

$spireForeground = $false
if ($foreground -and $spireProcess) {
    $spireForeground = $foreground.Id -eq $spireProcess.Id
}

$result = [ordered]@{
    CheckedAt = (Get-Date).ToString('o')
    SpireRunning = [bool]$spireProcess
    SpireForeground = $spireForeground
    SlayTheSpire2 = if ($spireProcess) {
        [pscustomobject]@{
            ProcessName = $spireProcess.ProcessName
            Id = $spireProcess.Id
            MainWindowTitle = $spireProcess.MainWindowTitle
        }
    } else {
        $null
    }
    Foreground = $foreground
    VisibleWindows = $visibleWindows
    CaptureGuidance = if ($spireForeground) {
        'Slay the Spire 2 is foreground; desktop screenshots can be captured.'
    } elseif ($spireProcess) {
        'Slay the Spire 2 is running but not foreground. Bring it to the front before taking gameplay screenshots.'
    } else {
        'Slay the Spire 2 is not running. Launch it before taking gameplay screenshots.'
    }
}

$json = $result | ConvertTo-Json -Depth 10
if ($OutFile) {
    $parent = Split-Path -Parent $OutFile
    if ($parent) {
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
    }

    $json | Set-Content -LiteralPath $OutFile -Encoding UTF8
}

$json

if ($RequireSpireForeground -and -not $spireForeground) {
    exit 2
}
