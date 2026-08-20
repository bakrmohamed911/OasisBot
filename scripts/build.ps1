# Usage:
# `scripts\build.ps1
# -Clean[False, optional]
# -DoNotStart[False, optional]
# -Configuration[Debug, optional]`

param(
    [string]$Configuration = "Debug",
    [switch]$Clean,
    [switch]$DoNotStart
)

if (-not (Test-Path ".\SDUI\SDUI\SDUI.csproj")) {
    Write-Output "SDUI submodule is missing or incomplete. Initializing and updating submodules..."
    git submodule update --init --recursive
}

taskkill /F /IM OasisBot.exe
taskkill /F /IM sro_client.exe

if ($Clean) {
    Write-Output "Performing a clean build..."
    New-Item  -ItemType Directory ".\temp" -ErrorAction SilentlyContinue > $null
    Move-Item ".\Build\User" ".\temp" -ErrorAction SilentlyContinue > $null
    Remove-Item -Recurse -Force ".\Build" -ErrorAction SilentlyContinue > $null
}

Write-Output "Building with '$Configuration' configuration..."

$vsPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -property installationPath
$msBuildPath = Join-Path $vsPath "MSBuild\Current\Bin\MSBuild.exe"

Write-Output "Step 1: Building .NET projects with dotnet build..."
dotnet build OasisBot.sln /p:Configuration=$Configuration /p:Platform=x86 2>&1 | Tee-Object -FilePath build.log

Write-Output "Step 2: Building C++ loader with VS MSBuild..."
& $msBuildPath "Library\RSBot.Loader.Library\RSBot.Loader.Library.vcxproj" /p:Configuration=Release 2>&1 | Tee-Object -FilePath build.log -Append

$buildExitCode = $LASTEXITCODE

if ($Clean) {
    Move-Item ".\temp\User" ".\Build\User" -ErrorAction SilentlyContinue > $null
    Remove-Item -Recurse -Force ".\temp" -ErrorAction SilentlyContinue > $null
}

if ($buildExitCode -eq 0) {
    if (!$DoNotStart) {
        Write-Output "Starting OasisBot..."
        & ".\Build\OasisBot.exe"
    }
}
else {
    Write-Output "Build failed. Check build.log for details."
    exit $buildExitCode
}