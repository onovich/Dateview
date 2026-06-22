param(
    [string]$Version = "",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$Configuration = "Release",
    [switch]$SkipPublish
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-RepoRoot {
    $scriptDirectory = Split-Path -Parent $PSCommandPath
    return (Resolve-Path -LiteralPath (Join-Path $scriptDirectory "..")).Path
}

function Assert-UnderPath {
    param(
        [string]$Path,
        [string]$ParentPath
    )

    $resolvedParent = (Resolve-Path -LiteralPath $ParentPath).Path
    $fullPath = [System.IO.Path]::GetFullPath($Path)
    if (-not $fullPath.StartsWith($resolvedParent, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to operate outside repository root: $fullPath"
    }
}

function Copy-DirectoryContents {
    param(
        [string]$Source,
        [string]$Destination
    )

    if (-not (Test-Path -LiteralPath $Source)) {
        throw "Source directory does not exist: $Source"
    }

    New-Item -ItemType Directory -Path $Destination -Force | Out-Null
    Get-ChildItem -LiteralPath $Source -Force | ForEach-Object {
        Copy-Item -LiteralPath $_.FullName -Destination $Destination -Recurse -Force
    }
}

function Test-RequiredFile {
    param(
        [string]$Root,
        [string]$RelativePath
    )

    $path = Join-Path $Root $RelativePath
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Missing required release file: $RelativePath"
    }
}

function Get-ProjectVersion {
    param(
        [string]$ProjectPath
    )

    [xml]$project = Get-Content -Raw -LiteralPath $ProjectPath
    $versionNode = $project.Project.PropertyGroup | ForEach-Object { $_.Version } | Where-Object { $_ } | Select-Object -First 1
    if ([string]::IsNullOrWhiteSpace($versionNode)) {
        throw "Project Version property is missing in $ProjectPath"
    }

    return [string]$versionNode
}

function Get-GitCommit {
    param(
        [string]$RepositoryRoot
    )

    $commit = git -C $RepositoryRoot rev-parse --short HEAD
    if ($LASTEXITCODE -ne 0) {
        return $null
    }

    return $commit.Trim()
}

function Get-RelativeFileManifest {
    param(
        [string]$Root
    )

    $rootFullPath = [System.IO.Path]::GetFullPath($Root).TrimEnd([System.IO.Path]::DirectorySeparatorChar)
    Get-ChildItem -LiteralPath $Root -Recurse -File | Sort-Object FullName | ForEach-Object {
        $relativePath = [System.IO.Path]::GetFullPath($_.FullName).Substring($rootFullPath.Length).TrimStart(
            [System.IO.Path]::DirectorySeparatorChar,
            [System.IO.Path]::AltDirectorySeparatorChar) -replace "\\", "/"
        [pscustomobject]@{
            path = $relativePath
            bytes = $_.Length
            sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $_.FullName).Hash.ToLowerInvariant()
        }
    }
}

function Write-Utf8NoBom {
    param(
        [string]$Path,
        [string]$Content
    )

    [System.IO.File]::WriteAllText($Path, $Content, [System.Text.UTF8Encoding]::new($false))
}

$repoRoot = Resolve-RepoRoot
$projectPath = Join-Path $repoRoot "src\ChinaTrayCalendar.Desktop\ChinaTrayCalendar.Desktop.csproj"
if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = Get-ProjectVersion -ProjectPath $projectPath
}

$publishDir = Join-Path $repoRoot "src\ChinaTrayCalendar.Desktop\bin\$Configuration\net10.0-windows\$RuntimeIdentifier\publish"
$releaseRoot = Join-Path $repoRoot "artifacts\release"
$bundleName = "Dateview-$Version-$RuntimeIdentifier"
$stagingRoot = Join-Path $releaseRoot $bundleName
$appRoot = Join-Path $stagingRoot "Dateview"
$zipPath = Join-Path $releaseRoot "$bundleName.zip"
$zipHashPath = Join-Path $releaseRoot "$bundleName.sha256.txt"
$metadataPath = Join-Path $releaseRoot "$bundleName.release.json"

Assert-UnderPath -Path $releaseRoot -ParentPath $repoRoot
Assert-UnderPath -Path $stagingRoot -ParentPath $repoRoot
Assert-UnderPath -Path $zipPath -ParentPath $repoRoot
Assert-UnderPath -Path $zipHashPath -ParentPath $repoRoot
Assert-UnderPath -Path $metadataPath -ParentPath $repoRoot

if (-not $SkipPublish) {
    dotnet publish $projectPath -p:PublishProfile="$RuntimeIdentifier-folder" --configuration $Configuration
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet publish failed with exit code $LASTEXITCODE"
    }
}

if (Test-Path -LiteralPath $stagingRoot) {
    Assert-UnderPath -Path $stagingRoot -ParentPath $repoRoot
    Remove-Item -LiteralPath $stagingRoot -Recurse -Force
}

if (Test-Path -LiteralPath $zipPath) {
    Assert-UnderPath -Path $zipPath -ParentPath $repoRoot
    Remove-Item -LiteralPath $zipPath -Force
}

if (Test-Path -LiteralPath $zipHashPath) {
    Assert-UnderPath -Path $zipHashPath -ParentPath $repoRoot
    Remove-Item -LiteralPath $zipHashPath -Force
}

if (Test-Path -LiteralPath $metadataPath) {
    Assert-UnderPath -Path $metadataPath -ParentPath $repoRoot
    Remove-Item -LiteralPath $metadataPath -Force
}

Copy-DirectoryContents -Source $publishDir -Destination $appRoot

$requiredFiles = @(
    "ChinaTrayCalendar.Desktop.exe",
    "ChinaTrayCalendar.Desktop.dll",
    "ChinaTrayCalendar.Desktop.deps.json",
    "ChinaTrayCalendar.Desktop.runtimeconfig.json",
    "assets\holidays\cn\2025.json",
    "assets\holidays\cn\2026.json"
)

foreach ($relativePath in $requiredFiles) {
    Test-RequiredFile -Root $appRoot -RelativePath $relativePath
}

$manifestPath = Join-Path $appRoot "release-manifest.json"
$fileManifest = @(Get-RelativeFileManifest -Root $appRoot)
$manifest = [ordered]@{
    schemaVersion = 1
    product = "Dateview"
    version = $Version
    runtimeIdentifier = $RuntimeIdentifier
    configuration = $Configuration
    generatedAtUtc = [System.DateTimeOffset]::UtcNow.ToString("o")
    gitCommit = Get-GitCommit -RepositoryRoot $repoRoot
    files = $fileManifest
}
Write-Utf8NoBom -Path $manifestPath -Content ($manifest | ConvertTo-Json -Depth 6)

Compress-Archive -LiteralPath $appRoot -DestinationPath $zipPath -CompressionLevel Optimal

$zipItem = Get-Item -LiteralPath $zipPath
$zipHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $zipPath).Hash.ToLowerInvariant()
Write-Utf8NoBom -Path $zipHashPath -Content "$zipHash  $bundleName.zip`r`n"

$metadata = [ordered]@{
    schemaVersion = 1
    product = "Dateview"
    version = $Version
    runtimeIdentifier = $RuntimeIdentifier
    configuration = $Configuration
    bundleName = $bundleName
    zip = "$bundleName.zip"
    zipBytes = $zipItem.Length
    zipSha256 = $zipHash
    manifest = "$bundleName\Dateview\release-manifest.json"
    generatedAtUtc = [System.DateTimeOffset]::UtcNow.ToString("o")
    gitCommit = Get-GitCommit -RepositoryRoot $repoRoot
    notes = "SHA256 is an integrity check, not a code signature."
}
Write-Utf8NoBom -Path $metadataPath -Content ($metadata | ConvertTo-Json -Depth 5)

[pscustomobject]@{
    BundleName = $bundleName
    ReleaseRoot = $releaseRoot
    Folder = $stagingRoot
    AppFolder = $appRoot
    Zip = $zipPath
    ZipBytes = $zipItem.Length
    ZipSha256 = $zipHash
    ZipSha256File = $zipHashPath
    Metadata = $metadataPath
    Manifest = $manifestPath
} | ConvertTo-Json -Depth 3
