param(
    [string]$Configuration = "Release",
    [switch]$InstallInnoSetup
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$solutionPath = Join-Path $repoRoot "Hospital Management System.sln"
$projectOutDir = Join-Path $repoRoot "Hospital Management System\bin\$Configuration"
$exePath = Join-Path $projectOutDir "Hospital Management System.exe"
$issPath = Join-Path $repoRoot "Installer\HospitalManagementSystem.iss"
$distDir = Join-Path $repoRoot "dist\installer"
$downloadsDir = Join-Path $env:USERPROFILE "Downloads"
$downloadsSetup = Join-Path $downloadsDir "HospitalManagementSystemSetup.exe"

function Find-MsBuild {
    $vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $vswhere) {
        $found = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
        if ($found -and (Test-Path $found)) {
            return $found
        }
    }

    $candidates = @(
        "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
    )

    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    throw "MSBuild.exe not found. Install Visual Studio Build Tools with .NET Framework support."
}

function Find-Iscc {
    $candidates = @(
        "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
        "C:\Program Files\Inno Setup 6\ISCC.exe",
        (Join-Path $env:LOCALAPPDATA "Programs\Inno Setup 6\ISCC.exe")
    )

    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    $fromPath = Get-Command iscc -ErrorAction SilentlyContinue
    if ($fromPath) {
        return $fromPath.Source
    }

    return $null
}

$msbuild = Find-MsBuild
Write-Host "Building solution using: $msbuild"
& $msbuild $solutionPath /t:Build /p:Configuration=$Configuration /p:Platform="Any CPU" /v:m

if (-not (Test-Path $exePath)) {
    throw "Build completed but executable not found at: $exePath"
}

$iscc = Find-Iscc
if (-not $iscc -and $InstallInnoSetup) {
    Write-Host "Inno Setup not found. Installing via winget..."
    winget install --id JRSoftware.InnoSetup -e --silent --accept-package-agreements --accept-source-agreements
    $iscc = Find-Iscc
}

if (-not $iscc) {
    throw "Inno Setup compiler (ISCC.exe) not found. Install Inno Setup 6, then rerun this script."
}

if (-not (Test-Path $distDir)) {
    New-Item -ItemType Directory -Path $distDir -Force | Out-Null
}

if (-not (Test-Path $downloadsDir)) {
    New-Item -ItemType Directory -Path $downloadsDir -Force | Out-Null
}

Write-Host "Building installer using: $iscc"
& $iscc $issPath

$setup = Get-ChildItem -Path $distDir -Filter "HospitalManagementSystemSetup.exe" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $setup) {
    throw "Installer build did not produce HospitalManagementSystemSetup.exe in $distDir"
}

Copy-Item -Path $setup.FullName -Destination $downloadsSetup -Force
Write-Host "Installer created: $($setup.FullName)"
Write-Host "Copied to: $downloadsSetup"
