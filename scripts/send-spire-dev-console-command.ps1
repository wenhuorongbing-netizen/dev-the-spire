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
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
"@

function Assert-SendKeysSafe {
    param([Parameter(Mandatory = $true)][string]$Value)

    if ($Value.IndexOfAny(@('{'[0], '}'[0], '+'[0], '^'[0], '%'[0], '~'[0], '('[0], ')'[0], '['[0], ']'[0])) -ge 0) {
        throw 'Command contains SendKeys control characters. Use a simpler command or type it manually.'
    }
}

function ConvertTo-SendKeysLiteral {
    param([Parameter(Mandatory = $true)][string]$Value)

    $builder = [System.Text.StringBuilder]::new()
    foreach ($char in $Value.ToCharArray()) {
        switch ($char) {
            ' ' { [void]$builder.Append(' ') }
            default {
                if ($char -match '^[A-Za-z0-9_.:-]$') {
                    [void]$builder.Append($char)
                    continue
                }

                throw "Unsupported SendKeys character '$char'. Use a simpler DevConsole command or type it manually."
            }
        }
    }

    $builder.ToString()
}

Assert-SendKeysSafe -Value $Command

$process = Get-Process -Name SlayTheSpire2 -ErrorAction SilentlyContinue |
    Where-Object { $_.MainWindowHandle -ne 0 } |
    Select-Object -First 1

if (-not $process) {
    throw 'SlayTheSpire2 is not running with a visible main window.'
}

[void][SpireConsoleCommandNative]::ShowWindow($process.MainWindowHandle, 9)
[void][SpireConsoleCommandNative]::SetForegroundWindow($process.MainWindowHandle)
Start-Sleep -Milliseconds $InitialDelayMs

if (-not $AssumeConsoleOpen) {
    [System.Windows.Forms.SendKeys]::SendWait("'")
    Start-Sleep -Milliseconds $AfterOpenDelayMs
}

if ($ClearExistingInput) {
    [System.Windows.Forms.SendKeys]::SendWait("^{a}")
    Start-Sleep -Milliseconds 50
    [System.Windows.Forms.SendKeys]::SendWait("{BACKSPACE}")
    Start-Sleep -Milliseconds 50
}

[System.Windows.Forms.SendKeys]::SendWait((ConvertTo-SendKeysLiteral -Value $Command))
[System.Windows.Forms.SendKeys]::SendWait("~")
Start-Sleep -Milliseconds $AfterEnterDelayMs

[pscustomobject]@{
    SentAt = (Get-Date).ToString('o')
    ProcessId = $process.Id
    MainWindowTitle = $process.MainWindowTitle
    Command = $Command
    AssumeConsoleOpen = [bool]$AssumeConsoleOpen
    ClearExistingInput = [bool]$ClearExistingInput
} | ConvertTo-Json -Depth 4
