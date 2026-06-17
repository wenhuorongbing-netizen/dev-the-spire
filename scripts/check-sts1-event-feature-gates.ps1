param(
    [string]$FeatureGatePath = 'EZMicroBalanceCode\Sts1Events\Runtime\Sts1EventFeatureGate.cs',
    [string]$RegistrationModePath = 'EZMicroBalanceCode\Sts1Events\Runtime\Sts1EventRegistrationMode.cs',
    [string]$RegistrationServicePath = 'EZMicroBalanceCode\Sts1Events\Runtime\Sts1EventRegistrationService.cs',
    [string]$FeatureModulePath = 'EZMicroBalanceCode\Sts1Events\Sts1EventsFeatureModule.cs',
    [string]$FeatureRegistryPath = 'EZMicroBalanceCode\Core\Features\SpirePlusFeatureRegistry.cs',
    [string]$ReplacementPrototypePath = 'EZMicroBalanceCode\Sts1Events\Runtime\Sts1ReplacementPrototype.cs',
    [string]$OutFile,
    [switch]$FailOnMismatch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$repoRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$checks = [System.Collections.Generic.List[object]]::new()
$mismatches = [System.Collections.Generic.List[string]]::new()

function Resolve-RepoPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return [System.IO.Path]::GetFullPath($Path)
    }

    return [System.IO.Path]::GetFullPath((Join-Path $repoRoot $Path))
}

function Read-RepoText {
    param([Parameter(Mandatory = $true)][string]$Path)

    $resolved = Resolve-RepoPath $Path
    if (-not (Test-Path -LiteralPath $resolved)) {
        Write-Error "Required file not found: $resolved"
        exit 1
    }

    return [System.IO.File]::ReadAllText($resolved)
}

function Add-Check {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][bool]$Passed,
        [Parameter(Mandatory = $true)][string]$Detail
    )

    $checks.Add([pscustomobject]@{
        Name = $Name
        Passed = $Passed
        Detail = $Detail
    }) | Out-Null

    if (-not $Passed) {
        $mismatches.Add("${Name}: $Detail") | Out-Null
    }
}

function Add-ContainsCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$Needle
    )

    Add-Check -Name $Name -Passed ($Text.Contains($Needle)) -Detail "requires '$Needle'"
}

function Add-NotContainsCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$Needle
    )

    Add-Check -Name $Name -Passed (-not $Text.Contains($Needle)) -Detail "must not contain '$Needle'"
}

function Add-RegexCheck {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][string]$Text,
        [Parameter(Mandatory = $true)][string]$Pattern
    )

    Add-Check -Name $Name -Passed ([regex]::IsMatch($Text, $Pattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)) -Detail "requires pattern '$Pattern'"
}

$featureGate = Read-RepoText $FeatureGatePath
$registrationMode = Read-RepoText $RegistrationModePath
$registrationService = Read-RepoText $RegistrationServicePath
$featureModule = Read-RepoText $FeatureModulePath
$featureRegistry = Read-RepoText $FeatureRegistryPath
$replacementPrototype = Read-RepoText $ReplacementPrototypePath

Add-ContainsCheck -Name 'mode_env_key_is_spireplus' -Text $featureGate -Needle 'private const string ModeEnvKey = "SPIREPLUS_STS1_EVENT_MODE"'
Add-ContainsCheck -Name 'unsafe_env_key_is_spireplus' -Text $featureGate -Needle 'private const string UnsafeModeEnvKey = "SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES"'
Add-ContainsCheck -Name 'blank_mode_defaults_off' -Text $featureGate -Needle 'string.IsNullOrWhiteSpace(envValue)'
Add-ContainsCheck -Name 'resolve_mode_returns_off' -Text $featureGate -Needle 'return Sts1EventRegistrationMode.Off;'
Add-ContainsCheck -Name 'invalid_mode_defaults_off' -Text $featureGate -Needle '? mode'
Add-ContainsCheck -Name 'tryparse_is_case_insensitive' -Text $featureGate -Needle 'Enum.TryParse<Sts1EventRegistrationMode>(envValue, ignoreCase: true, out var mode)'

Add-ContainsCheck -Name 'evaluate_off_disabled' -Text $featureGate -Needle 'Sts1EventRegistrationMode.Off => FeatureGateResult.Disabled'
Add-ContainsCheck -Name 'evaluate_canary_enabled' -Text $featureGate -Needle 'Sts1EventRegistrationMode.CanaryOnly => FeatureGateResult.Enabled'
Add-ContainsCheck -Name 'evaluate_batch1_enabled' -Text $featureGate -Needle 'Sts1EventRegistrationMode.AdditiveBatch1 => FeatureGateResult.Enabled'
Add-ContainsCheck -Name 'evaluate_all_draft_unsafe_gate' -Text $featureGate -Needle 'Sts1EventRegistrationMode.AdditiveAllDraft => EvaluateAdditiveAllDraftGate()'
Add-ContainsCheck -Name 'evaluate_replacement_gate' -Text $featureGate -Needle 'Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype => EvaluateReplacementPrototypeGate()'
Add-ContainsCheck -Name 'unknown_mode_disabled' -Text $featureGate -Needle "defaulting to Off"

Add-RegexCheck -Name 'all_draft_requires_unsafe_override' -Text $featureGate -Pattern 'EvaluateAdditiveAllDraftGate\(\)\s*=>\s*IsUnsafeModeAllowed\(\)\s*\?'
Add-ContainsCheck -Name 'all_draft_disabled_message_mentions_unsafe_env' -Text $featureGate -Needle 'SPIREPLUS_ALLOW_UNSAFE_STS1_EVENT_MODES=1 to enable'
Add-ContainsCheck -Name 'replacement_compile_symbol_gate' -Text $featureGate -Needle '#if REPLACEMENT_PROTOTYPE_ENABLED'
Add-ContainsCheck -Name 'replacement_has_else_disabled_path' -Text $featureGate -Needle '#else'
Add-ContainsCheck -Name 'replacement_disabled_without_symbol' -Text $featureGate -Needle 'REPLACEMENT_PROTOTYPE_ENABLED is not defined; no StS1 events registered.'
Add-RegexCheck -Name 'replacement_requires_unsafe_override_when_compiled' -Text $featureGate -Pattern 'REPLACEMENT_PROTOTYPE_ENABLED[\s\S]*IsUnsafeModeAllowed\(\)\s*\?'
Add-ContainsCheck -Name 'unsafe_value_rejects_zero' -Text $featureGate -Needle 'string.Equals(value, "0", StringComparison.OrdinalIgnoreCase)'
Add-ContainsCheck -Name 'unsafe_value_rejects_false' -Text $featureGate -Needle 'string.Equals(value, "false", StringComparison.OrdinalIgnoreCase)'
Add-ContainsCheck -Name 'unsafe_value_rejects_off' -Text $featureGate -Needle 'string.Equals(value, "off", StringComparison.OrdinalIgnoreCase)'
Add-ContainsCheck -Name 'unsafe_value_rejects_no' -Text $featureGate -Needle 'string.Equals(value, "no", StringComparison.OrdinalIgnoreCase)'

Add-ContainsCheck -Name 'enum_off_zero' -Text $registrationMode -Needle 'Off = 0,'
Add-ContainsCheck -Name 'enum_canary_one' -Text $registrationMode -Needle 'CanaryOnly = 1,'
Add-ContainsCheck -Name 'enum_batch1_two' -Text $registrationMode -Needle 'AdditiveBatch1 = 2,'
Add-ContainsCheck -Name 'enum_all_draft_three' -Text $registrationMode -Needle 'AdditiveAllDraft = 3,'
Add-ContainsCheck -Name 'enum_replacement_four' -Text $registrationMode -Needle 'ReplaceUnknownEventsPrototype = 4,'

Add-RegexCheck -Name 'register_gated_off_returns_without_registration' -Text $registrationService -Pattern 'case\s+Sts1EventRegistrationMode\.Off:\s*return;'
Add-RegexCheck -Name 'register_gated_canary_routes_to_canary' -Text $registrationService -Pattern 'case\s+Sts1EventRegistrationMode\.CanaryOnly:\s*RegisterCanaryOnly\(modId\);\s*return;'
Add-RegexCheck -Name 'register_gated_batch1_routes_to_batch1' -Text $registrationService -Pattern 'case\s+Sts1EventRegistrationMode\.AdditiveBatch1:\s*RegisterAdditiveBatch1\(modId\);\s*return;'
Add-RegexCheck -Name 'register_gated_all_draft_routes_to_register_all' -Text $registrationService -Pattern 'case\s+Sts1EventRegistrationMode\.AdditiveAllDraft:\s*RegisterAll\(modId\);\s*return;'
Add-RegexCheck -Name 'register_gated_replacement_compile_gated' -Text $registrationService -Pattern 'case\s+Sts1EventRegistrationMode\.ReplaceUnknownEventsPrototype:[\s\S]*#if REPLACEMENT_PROTOTYPE_ENABLED[\s\S]*RegisterAll\(modId\);[\s\S]*#else[\s\S]*no StS1 events registered'

Add-ContainsCheck -Name 'feature_module_uses_gate_evaluation' -Text $featureModule -Needle 'public FeatureGateResult EvaluateGate() => Sts1EventFeatureGate.EvaluateGate();'
Add-ContainsCheck -Name 'feature_module_registers_gated_mode' -Text $featureModule -Needle 'Sts1EventRegistrationService.RegisterGated(MainFile.ModId, mode);'
Add-NotContainsCheck -Name 'feature_module_has_no_disable_env_override' -Text $featureModule -Needle 'DisableEnvKeys'
Add-ContainsCheck -Name 'feature_registry_registers_sts1_module' -Text $featureRegistry -Needle '.Register(new Sts1EventsFeatureModule())'

Add-ContainsCheck -Name 'replacement_patch_source_compile_gated' -Text $replacementPrototype -Needle '#if REPLACEMENT_PROTOTYPE_ENABLED'
Add-ContainsCheck -Name 'replacement_patch_harmony_patch_declared' -Text $replacementPrototype -Needle '[HarmonyPatch(typeof(ActModel), nameof(ActModel.GenerateRooms))]'
Add-ContainsCheck -Name 'replacement_patch_checks_mode_at_runtime' -Text $replacementPrototype -Needle 'Sts1EventFeatureGate.ResolveMode() == Sts1EventRegistrationMode.ReplaceUnknownEventsPrototype'

$report = [pscustomobject]@{
    Checks = $checks
    Mismatches = $mismatches
}

foreach ($check in $checks) {
    $status = if ($check.Passed) { 'pass' } else { 'fail' }
    Write-Output "$($check.Name) status=$status"
}

Write-Output "checks=$($checks.Count)"
Write-Output "mismatches=$($mismatches.Count)"

foreach ($mismatch in $mismatches) {
    Write-Output "mismatch $mismatch"
}

if ($OutFile) {
    $resolvedOutFile = Resolve-RepoPath $OutFile
    $outDir = [System.IO.Path]::GetDirectoryName($resolvedOutFile)
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        [void][System.IO.Directory]::CreateDirectory($outDir)
    }

    $report | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $resolvedOutFile -Encoding UTF8
}

if ($FailOnMismatch -and $mismatches.Count -gt 0) {
    exit 1
}
