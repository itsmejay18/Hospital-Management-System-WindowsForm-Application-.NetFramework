param(
    [string]$ReleaseDir = (Join-Path $PSScriptRoot '..\..\Hospital Management System\bin\Release'),
    [string]$OutputPath = (Join-Path $env:USERPROFILE 'Downloads\HospitalManagementSystemSetup.exe')
)

$ErrorActionPreference = 'Stop'

function Resolve-MSBuildPath {
    $candidates = @(
        'C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\17\Professional\MSBuild\Current\Bin\MSBuild.exe',
        'C:\Program Files\Microsoft Visual Studio\17\Community\MSBuild\Current\Bin\MSBuild.exe'
    )

    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    throw 'MSBuild.exe was not found.'
}

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$releasePath = [System.IO.Path]::GetFullPath($ReleaseDir)
$outputFile = [System.IO.Path]::GetFullPath($OutputPath)
$bootstrapperProject = Join-Path $PSScriptRoot 'Bootstrapper\HospitalManagementSystemSetup.csproj'
$payloadDir = Join-Path $PSScriptRoot 'Bootstrapper\Payload'
$payloadZip = Join-Path $payloadDir 'AppPayload.zip'
$stageDir = Join-Path $PSScriptRoot 'stage'
$msbuild = Resolve-MSBuildPath

& $msbuild (Join-Path $repoRoot 'Hospital Management System\Hospital Management System.csproj') /t:Build /p:Configuration=Release | Out-Null
if ($LASTEXITCODE -ne 0) {
    throw "Application build failed with exit code $LASTEXITCODE."
}

if (-not (Test-Path $releasePath)) {
    throw "Release output not found: $releasePath"
}

if (Test-Path $stageDir) {
    Remove-Item $stageDir -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $stageDir | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $stageDir 'Assets') | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $stageDir 'Schema') | Out-Null
New-Item -ItemType Directory -Force -Path $payloadDir | Out-Null

$filesToCopy = @(
    'Hospital Management System.exe',
    'Hospital Management System.exe.config',
    'ClosedXML.dll',
    'Dapper.dll',
    'itextsharp.dll',
    'MySql.Data.dll',
    'MySql.Data.EntityFramework.dll',
    'System.Threading.Tasks.Extensions.dll'
)

foreach ($fileName in $filesToCopy) {
    Copy-Item (Join-Path $releasePath $fileName) (Join-Path $stageDir $fileName) -Force
}

Copy-Item (Join-Path $releasePath 'Assets\branding-logo.ico') (Join-Path $stageDir 'Assets\branding-logo.ico') -Force
Copy-Item (Join-Path $releasePath 'Assets\branding-logo.png') (Join-Path $stageDir 'Assets\branding-logo.png') -Force
Copy-Item (Join-Path $releasePath 'Schema\hospitalmanagementsystem.sql') (Join-Path $stageDir 'Schema\hospitalmanagementsystem.sql') -Force

if (Test-Path $payloadZip) {
    Remove-Item $payloadZip -Force
}

Compress-Archive -Path (Join-Path $stageDir '*') -DestinationPath $payloadZip -CompressionLevel Optimal -Force

& $msbuild $bootstrapperProject /t:Build /p:Configuration=Release | Out-Null
if ($LASTEXITCODE -ne 0) {
    throw "Bootstrapper build failed with exit code $LASTEXITCODE."
}

$bootstrapperOutput = Join-Path $PSScriptRoot 'Bootstrapper\bin\Release\HospitalManagementSystemSetup.exe'
if (-not (Test-Path $bootstrapperOutput)) {
    throw "Bootstrapper output was not created: $bootstrapperOutput"
}

$outputDirectory = Split-Path -Parent $outputFile
if (-not [string]::IsNullOrWhiteSpace($outputDirectory)) {
    New-Item -ItemType Directory -Force -Path $outputDirectory | Out-Null
}

Copy-Item $bootstrapperOutput $outputFile -Force

if (Test-Path $stageDir) {
    Remove-Item $stageDir -Recurse -Force
}

if (Test-Path $payloadZip) {
    Remove-Item $payloadZip -Force
}

Write-Host $outputFile
