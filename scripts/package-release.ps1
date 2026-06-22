param(
    [string]$Version = "0.1.0-preview",
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

$repoRoot = Resolve-RepoRoot
$projectPath = Join-Path $repoRoot "src\ChinaTrayCalendar.Desktop\ChinaTrayCalendar.Desktop.csproj"
$publishDir = Join-Path $repoRoot "src\ChinaTrayCalendar.Desktop\bin\$Configuration\net10.0-windows\$RuntimeIdentifier\publish"
$releaseRoot = Join-Path $repoRoot "artifacts\release"
$bundleName = "Dateview-$Version-$RuntimeIdentifier"
$stagingRoot = Join-Path $releaseRoot $bundleName
$appRoot = Join-Path $stagingRoot "Dateview"
$zipPath = Join-Path $releaseRoot "$bundleName.zip"

Assert-UnderPath -Path $releaseRoot -ParentPath $repoRoot
Assert-UnderPath -Path $stagingRoot -ParentPath $repoRoot
Assert-UnderPath -Path $zipPath -ParentPath $repoRoot

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

Compress-Archive -LiteralPath $appRoot -DestinationPath $zipPath -CompressionLevel Optimal

$zipItem = Get-Item -LiteralPath $zipPath
[pscustomobject]@{
    BundleName = $bundleName
    ReleaseRoot = $releaseRoot
    Folder = $stagingRoot
    AppFolder = $appRoot
    Zip = $zipPath
    ZipBytes = $zipItem.Length
} | ConvertTo-Json -Depth 3
