[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$AssetId,

    [string]$ManifestPath = "docs/features/ancient-expansion-v2.2/art-asset-manifest.json",
    [string]$PromptPackPath = "docs/features/ancient-expansion-v2.2/art-generation-prompts.md",
    [string]$Endpoint = $env:GPT4FREE_IMAGE_ENDPOINT,
    [string]$Provider = $(if ($env:GPT4FREE_PROVIDER) { $env:GPT4FREE_PROVIDER } else { "OpenaiChat" }),
    [string]$ApiModel = $(if ($env:GPT4FREE_IMAGE_MODEL) { $env:GPT4FREE_IMAGE_MODEL } else { "gpt-image" }),
    [string]$ApiKey = $env:GPT4FREE_API_KEY,
    [string]$OutDir = ".tools/art-generation/gpt4free",
    [switch]$DryRun,
    [switch]$DownloadToTarget,
    [switch]$Force
)

$ErrorActionPreference = "Stop"
$RequiredGenerationMode = "GPTimage2"

function Get-RepoRoot {
    return [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
}

function Resolve-RepoPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot,
        [Parameter(Mandatory = $true)]
        [string]$RelativePath,
        [switch]$MustExist
    )

    if ([System.IO.Path]::IsPathRooted($RelativePath)) {
        throw "Expected a repository-relative path, got absolute path: $RelativePath"
    }

    $root = [System.IO.Path]::GetFullPath($RepoRoot)
    $candidate = [System.IO.Path]::GetFullPath((Join-Path $root $RelativePath))
    if (!$candidate.StartsWith($root, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Path escapes repository root: $RelativePath"
    }

    if ($MustExist -and !(Test-Path -LiteralPath $candidate -PathType Leaf)) {
        throw "Missing required file: $RelativePath"
    }

    return $candidate
}

function Get-MarkdownSection {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Text,
        [Parameter(Mandatory = $true)]
        [string]$HeadingLine
    )

    $escapedHeading = [regex]::Escape($HeadingLine)
    $match = [regex]::Match(
        $Text,
        "(?ms)^$escapedHeading\s*\r?\n(?<body>.*?)(?=^#{1,3}\s+|\z)")
    if (!$match.Success) {
        throw "Could not find markdown section: $HeadingLine"
    }

    return $match.Groups["body"].Value.Trim()
}

function Get-CodeBlockAfter {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Text,
        [Parameter(Mandatory = $true)]
        [string]$Anchor
    )

    $escapedAnchor = [regex]::Escape($Anchor)
    $pattern = '(?ms)' + $escapedAnchor + '.*?```text\s*(?<body>.*?)```'
    $match = [regex]::Match($Text, $pattern)
    if (!$match.Success) {
        throw "Could not find text code block after: $Anchor"
    }

    return $match.Groups["body"].Value.Trim()
}

function Get-RoleTemplateHeading {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Role,
        [Parameter(Mandatory = $true)]
        [string]$PromptId
    )

    if ($PromptId -eq "lotha_event_background" -or $Role -eq "event_background") {
        return "Wide Event Background"
    }

    if ($Role -eq "card_portrait") {
        return "Card Portrait"
    }

    if ($Role -eq "power_art" -or $Role -eq "relic_art" -or $Role -eq "relic_art_outline" -or $Role -eq "option_relic") {
        return "Relic Or Option Item"
    }

    if ($Role -eq "map_icon" -or $Role -eq "map_icon_outline" -or $Role -eq "run_history_icon" -or $Role -eq "run_history_outline") {
        return "Relic Or Option Item"
    }

    return "Relic Or Option Item"
}

function Get-ConceptKey {
    param(
        [Parameter(Mandatory = $true)]
        [string]$AssetId,
        [Parameter(Mandatory = $true)]
        [string]$PromptId
    )

    if ($PromptId -eq "ancient_identity_icons") {
        if ($AssetId.StartsWith("urda_", [System.StringComparison]::Ordinal)) { return "urda_identity" }
        if ($AssetId.StartsWith("morvi_", [System.StringComparison]::Ordinal)) { return "morvi_identity" }
        if ($AssetId.StartsWith("lotha_", [System.StringComparison]::Ordinal)) { return "lotha_identity" }
    }

    if ($AssetId.EndsWith("_event_background", [System.StringComparison]::Ordinal)) {
        return ""
    }

    $key = $AssetId
    foreach ($suffix in @(
        "_option_relic",
        "_card_portrait_small",
        "_card_portrait_big",
        "_power",
        "_map_icon_outline",
        "_map_icon",
        "_run_history_icon_outline",
        "_run_history_icon",
        "_run_history_outline"
    )) {
        if ($key.EndsWith($suffix, [System.StringComparison]::Ordinal)) {
            return $key.Substring(0, $key.Length - $suffix.Length)
        }
    }

    return $key
}

function Get-ConceptText {
    param(
        [Parameter(Mandatory = $true)]
        [string]$PromptBlock,
        [string]$ConceptKey
    )

    if ([string]::IsNullOrWhiteSpace($ConceptKey)) {
        return ""
    }

    $escapedKey = [regex]::Escape($ConceptKey)
    $pattern = '(?m)^\s*-\s*`' + $escapedKey + '`:\s*(?<concept>.+)$'
    $match = [regex]::Match($PromptBlock, $pattern)
    if ($match.Success) {
        return $match.Groups["concept"].Value.Trim()
    }

    return ""
}

function Get-FirstImageUrl {
    param([object]$Value)

    if ($null -eq $Value) {
        return $null
    }

    if ($Value -is [string]) {
        if ($Value -match '^https?://') {
            return $Value
        }

        if ($Value.StartsWith("/", [System.StringComparison]::Ordinal)) {
            return $Value
        }

        return $null
    }

    foreach ($name in @("imageUrl", "image_url", "url", "downloadUrl", "download_url")) {
        if ($Value.PSObject.Properties.Name -contains $name) {
            $candidate = [string]$Value.$name
            if ($candidate -match '^https?://') {
                return $candidate
            }
        }
    }

    if ($Value -is [System.Collections.IEnumerable]) {
        foreach ($item in $Value) {
            $nested = Get-FirstImageUrl -Value $item
            if ($nested) {
                return $nested
            }
        }
    }

    foreach ($property in $Value.PSObject.Properties) {
        $nested = Get-FirstImageUrl -Value $property.Value
        if ($nested) {
            return $nested
        }
    }

    return $null
}

function Get-FirstBase64Image {
    param([object]$Value)

    if ($null -eq $Value) {
        return $null
    }

    foreach ($name in @("b64_json", "base64", "imageBase64", "image_base64")) {
        if ($Value.PSObject.Properties.Name -contains $name) {
            $candidate = [string]$Value.$name
            if (![string]::IsNullOrWhiteSpace($candidate)) {
                return $candidate
            }
        }
    }

    if ($Value -is [System.Collections.IEnumerable] -and $Value -isnot [string]) {
        foreach ($item in $Value) {
            $nested = Get-FirstBase64Image -Value $item
            if ($nested) {
                return $nested
            }
        }
    }

    foreach ($property in $Value.PSObject.Properties) {
        $nested = Get-FirstBase64Image -Value $property.Value
        if ($nested) {
            return $nested
        }
    }

    return $null
}

$repoRoot = Get-RepoRoot
$manifestFullPath = Resolve-RepoPath -RepoRoot $repoRoot -RelativePath $ManifestPath -MustExist
$promptPackFullPath = Resolve-RepoPath -RepoRoot $repoRoot -RelativePath $PromptPackPath -MustExist
$manifest = Get-Content -Raw -LiteralPath $manifestFullPath | ConvertFrom-Json
$promptPack = Get-Content -Raw -LiteralPath $promptPackFullPath
$asset = @($manifest.assets | Where-Object { [string]$_.id -eq $AssetId })

if ($asset.Count -ne 1) {
    throw "Expected exactly one manifest asset with id '$AssetId', found $($asset.Count)."
}

$asset = $asset[0]
$promptId = [string]$asset.prompt_id
if ([string]::IsNullOrWhiteSpace($promptId)) {
    throw "Manifest asset '$AssetId' has no prompt_id. Add one before requesting generated art."
}

$targetPath = [string]$asset.target_path
$role = [string]$asset.role
$promptBlock = Get-MarkdownSection -Text $promptPack -HeadingLine "## Prompt Block: $promptId"
$roleTemplateHeading = Get-RoleTemplateHeading -Role $role -PromptId $promptId
$roleTemplate = Get-MarkdownSection -Text $promptPack -HeadingLine "### $roleTemplateHeading"
$coreStyleSuffix = Get-CodeBlockAfter -Text $promptPack -Anchor "Append this exact style suffix"
$conceptKey = Get-ConceptKey -AssetId $AssetId -PromptId $promptId
$conceptText = Get-ConceptText -PromptBlock $promptBlock -ConceptKey $conceptKey

if (![string]::IsNullOrWhiteSpace($conceptKey) -and [string]::IsNullOrWhiteSpace($conceptText) -and $promptBlock -match '(?m)^\s*-\s*`') {
    throw "Prompt block '$promptId' has multiple concept bullets, but concept key '$conceptKey' was not found."
}

$assetPromptBlock = if ([string]::IsNullOrWhiteSpace($conceptText)) {
    $promptBlock
}
else {
    "Use the '$promptId' prompt block concept for '$conceptKey': $conceptText"
}

$assembledPrompt = @"
Generate exactly one original Spire Plus image asset.

Required generation mode:
generation_mode: $RequiredGenerationMode
mode: $RequiredGenerationMode
semantic_model: $RequiredGenerationMode
gpt4free_transport_model: $ApiModel

Target asset:
- id: $AssetId
- role: $role
- target_path: $targetPath
- prompt_id: $promptId
- concept_key: $conceptKey

Use this canonical role template from ${PromptPackPath}:
$roleTemplate

Use this canonical asset concept from ${PromptPackPath}:
$assetPromptBlock

Asset-specific concept:
$conceptText

Append this exact core style suffix:
$coreStyleSuffix

Final constraints:
No visible text, pseudo-text, letters, numbers, logos, watermarks, UI panels, card frames, release numbers, official Slay the Spire 2 assets, web-image copying, photorealism, anime, 3D render, glossy concept-art polish, or generic high-detail fantasy.
"@

$payload = [ordered]@{
    generation_mode = $RequiredGenerationMode
    mode = $RequiredGenerationMode
    semantic_model = $RequiredGenerationMode
    model = $ApiModel
    provider = $Provider
    prompt = $assembledPrompt
    response_format = "url"
    n = 1
    download_media = $true
    asset_id = $AssetId
    role = $role
    prompt_id = $promptId
    concept_key = $conceptKey
    target_path = $targetPath
    prompt_source_path = $PromptPackPath
    manifest_path = $ManifestPath
}

$outFullPath = Resolve-RepoPath -RepoRoot $repoRoot -RelativePath $OutDir
if (!(Test-Path -LiteralPath $outFullPath -PathType Container)) {
    New-Item -ItemType Directory -Path $outFullPath | Out-Null
}

$safeAssetId = $AssetId -replace '[^A-Za-z0-9_.-]', '_'
$requestPath = Join-Path $outFullPath "$safeAssetId.request.json"
$payloadJson = $payload | ConvertTo-Json -Depth 8
Set-Content -LiteralPath $requestPath -Value $payloadJson -Encoding UTF8

if ($DryRun -or [string]::IsNullOrWhiteSpace($Endpoint)) {
    if (!$DryRun) {
        Write-Warning "GPT4FREE_IMAGE_ENDPOINT is not set. Wrote a dry-run request only."
    }

    [ordered]@{
        dry_run = $true
        required_generation_mode = $RequiredGenerationMode
        request_path = $requestPath
        endpoint_required_env = "GPT4FREE_IMAGE_ENDPOINT"
        api_key_optional_env = "GPT4FREE_API_KEY"
        provider = $Provider
        api_model = $ApiModel
        target_path = $targetPath
    } | ConvertTo-Json -Depth 4
    exit 0
}

$headers = @{}
if (![string]::IsNullOrWhiteSpace($ApiKey)) {
    $headers["Authorization"] = "Bearer $ApiKey"
}

$requestUri = [UriBuilder]::new($Endpoint)
if (![string]::IsNullOrWhiteSpace($Provider) -and $requestUri.Path.EndsWith("/v1/images/generations", [System.StringComparison]::OrdinalIgnoreCase)) {
    $query = [System.Web.HttpUtility]::ParseQueryString($requestUri.Query)
    if ([string]::IsNullOrWhiteSpace($query["provider"])) {
        $query["provider"] = $Provider
        $requestUri.Query = $query.ToString()
    }
}

$response = Invoke-RestMethod -Method Post -Uri $requestUri.Uri.AbsoluteUri -ContentType "application/json" -Headers $headers -Body $payloadJson
$responsePath = Join-Path $outFullPath "$safeAssetId.response.json"
$response | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $responsePath -Encoding UTF8

$imageDownloadPath = $null
$imageUrl = Get-FirstImageUrl -Value $response
$base64Image = Get-FirstBase64Image -Value $response
if ($imageUrl) {
    if ($DownloadToTarget) {
        $imageDownloadPath = Resolve-RepoPath -RepoRoot $repoRoot -RelativePath $targetPath
        if ((Test-Path -LiteralPath $imageDownloadPath -PathType Leaf) -and !$Force) {
            throw "Target already exists. Pass -Force to overwrite after visual review intent is explicit: $targetPath"
        }
    }
    else {
        $imageDownloadPath = Join-Path $outFullPath "$safeAssetId.png"
    }

    if ($imageUrl.StartsWith("/", [System.StringComparison]::Ordinal)) {
        $endpointUri = [Uri]$requestUri.Uri.AbsoluteUri
        $imageUrl = ([Uri]::new($endpointUri, $imageUrl)).AbsoluteUri
    }

    Invoke-WebRequest -Uri $imageUrl -OutFile $imageDownloadPath
}
elseif ($base64Image) {
    if ($DownloadToTarget) {
        $imageDownloadPath = Resolve-RepoPath -RepoRoot $repoRoot -RelativePath $targetPath
        if ((Test-Path -LiteralPath $imageDownloadPath -PathType Leaf) -and !$Force) {
            throw "Target already exists. Pass -Force to overwrite after visual review intent is explicit: $targetPath"
        }
    }
    else {
        $imageDownloadPath = Join-Path $outFullPath "$safeAssetId.png"
    }

    $base64Image = $base64Image -replace '^data:image/[^;]+;base64,', ''
    [System.IO.File]::WriteAllBytes($imageDownloadPath, [Convert]::FromBase64String($base64Image))
}

[ordered]@{
    dry_run = $false
    required_generation_mode = $RequiredGenerationMode
    request_path = $requestPath
    response_path = $responsePath
    image_url = $imageUrl
    image_download_path = $imageDownloadPath
    provider = $Provider
    api_model = $ApiModel
    semantic_model = $RequiredGenerationMode
    target_path = $targetPath
} | ConvertTo-Json -Depth 4

exit 0
