param(
    [string]$Repository = 'wenhuorongbing-netizen/dev-the-spire',
    [string]$WorkflowName = 'Full Local Validation',
    [string]$Branch = 'main',
    [ValidateRange(1, 100)]
    [int]$PerPage = 100,
    [string]$OutFile,
    [switch]$RequireSuccessfulRun
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 3.0

$encodedBranch = [System.Uri]::EscapeDataString($Branch)
$uri = "https://api.github.com/repos/$Repository/actions/runs?per_page=$PerPage&branch=$encodedBranch"
$headers = @{
    'User-Agent' = 'SpirePlus-Workflow-Audit'
    'Accept' = 'application/vnd.github+json'
}

$response = Invoke-RestMethod -Uri $uri -Headers $headers
$runs = @($response.workflow_runs)
$matchingRuns = @(
    $runs |
        Where-Object { $_.name -eq $WorkflowName } |
        Sort-Object { [datetime]$_.created_at } -Descending
)
$latest = $matchingRuns | Select-Object -First 1

$summary = [ordered]@{
    Repository = $Repository
    Branch = $Branch
    WorkflowName = $WorkflowName
    CheckedAt = (Get-Date).ToString('o')
    ApiUrl = $uri
    TotalRunsChecked = $runs.Count
    MatchingRunCount = $matchingRuns.Count
    LatestRun = if ($latest) {
        [ordered]@{
            Name = $latest.name
            Status = $latest.status
            Conclusion = $latest.conclusion
            CreatedAt = $latest.created_at
            UpdatedAt = $latest.updated_at
            Url = $latest.html_url
        }
    } else {
        $null
    }
    Runs = @(
        $matchingRuns | ForEach-Object {
            [ordered]@{
                Name = $_.name
                Status = $_.status
                Conclusion = $_.conclusion
                CreatedAt = $_.created_at
                UpdatedAt = $_.updated_at
                Url = $_.html_url
            }
        }
    )
}

$json = $summary | ConvertTo-Json -Depth 8

if ($OutFile) {
    $outPath = [System.IO.Path]::GetFullPath($OutFile)
    $outDir = [System.IO.Path]::GetDirectoryName($outPath)
    if ($outDir -and -not (Test-Path -LiteralPath $outDir)) {
        New-Item -ItemType Directory -Path $outDir | Out-Null
    }

    Set-Content -LiteralPath $outPath -Value $json -Encoding UTF8
}

$json

if ($RequireSuccessfulRun -and (-not $latest -or $latest.status -ne 'completed' -or $latest.conclusion -ne 'success')) {
    throw "No completed successful '$WorkflowName' run was found in the latest $($runs.Count) GitHub Actions runs for $Repository on $Branch."
}
