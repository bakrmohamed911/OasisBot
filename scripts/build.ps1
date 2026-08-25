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
    Write-Output "Performing a clean build (wiping Build output plus every project's bin/obj)..."

    New-Item -ItemType Directory ".\temp" -ErrorAction SilentlyContinue > $null
    Move-Item ".\Build\User" ".\temp" -ErrorAction SilentlyContinue > $null
    # Recorded/imported training-place routes (Data\Scripts\TrainingPlaces\*.rbs + catalog.json)
    # only ever exist as runtime-written data under the gitignored Build\ tree - unlike
    # Data\Scripts\Towns, which is seeded from the source-controlled Dependencies\ folder on every
    # build, there's no copy of these anywhere else. Preserve them the same way Build\User already
    # is, or a clean build silently throws away every route ever recorded/imported.
    Move-Item ".\Build\Data\Scripts\TrainingPlaces" ".\temp" -ErrorAction SilentlyContinue > $null
    Remove-Item -Recurse -Force ".\Build" -ErrorAction SilentlyContinue > $null

    # Wiping just the Build output isn't a real clean build - each project's own bin/obj
    # still holds the previous build's compiled output and incremental-build state, so a
    # DLL that looks "up to date" by timestamp can actually be stale relative to source.
    # Remove those too so every build starts from nothing, like a fresh checkout.
    Get-ChildItem -Path . -Recurse -Directory -Force -ErrorAction SilentlyContinue |
        Where-Object { $_.Name -in @("bin", "obj") -and $_.FullName -notmatch '\\(Build|temp)\\' } |
        ForEach-Object {
            Write-Output "Removing $($_.FullName)"
            Remove-Item -Recurse -Force $_.FullName -ErrorAction SilentlyContinue
        }

    # The C++ loader isn't a bin/obj-style project - its intermediate object files and
    # output DLL live under their own folders that dotnet/MSBuild won't clean for us here.
    Remove-Item -Recurse -Force ".\Library\RSBot.Loader.Library\Build" -ErrorAction SilentlyContinue
    Get-ChildItem -Path ".\Library\RSBot.Loader.Library" -Directory -Filter "RSBot.Lo.*" -ErrorAction SilentlyContinue |
        ForEach-Object { Remove-Item -Recurse -Force $_.FullName -ErrorAction SilentlyContinue }
}

Write-Output "Building with '$Configuration' configuration..."

$vsPath = & "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -property installationPath
$msBuildPath = Join-Path $vsPath "MSBuild\Current\Bin\MSBuild.exe"

Write-Output "Step 1: Building .NET projects with dotnet build..."

# The .NET SDK's `dotnet build` cannot process the native RSBot.Loader.Library.vcxproj
# inside OasisBot.sln (it has no C++ toolset support -> MSB4019), even though that
# project isn't meant to be built here at all - it's built separately in Step 2 with
# real VS MSBuild. Building the .sln directly would always fail on that one project
# regardless of whether the actual .NET projects succeeded, so build a solution filter
# that includes every project except the vcxproj instead.
$dotnetProjects = dotnet sln OasisBot.sln list | Where-Object { $_ -like "*.csproj" }
$slnFilterPath = Join-Path $env:TEMP "OasisBot.dotnet-build.slnf"
$slnFilter = @{
    solution = @{
        path     = (Resolve-Path "OasisBot.sln").Path
        projects = @($dotnetProjects)
    }
} | ConvertTo-Json -Depth 5
Set-Content -Path $slnFilterPath -Value $slnFilter -Encoding utf8

dotnet build $slnFilterPath /p:Configuration=$Configuration /p:Platform=x86 2>&1 | Tee-Object -FilePath build.log
$dotnetExitCode = $LASTEXITCODE

Write-Output "Step 2: Building C++ loader with VS MSBuild..."
& $msBuildPath "Library\RSBot.Loader.Library\RSBot.Loader.Library.vcxproj" /p:Configuration=Release 2>&1 | Tee-Object -FilePath build.log -Append
$msBuildExitCode = $LASTEXITCODE

# The vcxproj's OutDir ("$(SolutionDir)Build\") only resolves correctly when MSBuild
# builds through the .sln - $(SolutionDir) is never defined when building this .vcxproj
# directly (which is why it's built this way at all - see the comment on Step 1). So the
# DLL actually lands in Library\RSBot.Loader.Library\Build\, never in .\Build\ where
# ClientManager.Start() looks for it to inject into the game client - injection then fails
# with no obvious error (the client still launches normally, it just never gets hooked).
if ($msBuildExitCode -eq 0) {
    $loaderDll = "Library\RSBot.Loader.Library\Build\Client.Library.dll"
    if (Test-Path $loaderDll) {
        Copy-Item $loaderDll ".\Build\Client.Library.dll" -Force
    }
    else {
        Write-Output "Warning: $loaderDll not found after a successful build - client injection will fail."
    }
}

# $LASTEXITCODE only reflects whichever native command ran most recently, so it must be
# captured right after each step - otherwise a failing dotnet build (Step 1) is masked by
# a succeeding MSBuild step (Step 2).
if ($dotnetExitCode -ne 0) {
    $buildExitCode = $dotnetExitCode
}
else {
    $buildExitCode = $msBuildExitCode
}

if ($Clean) {
    Move-Item ".\temp\User" ".\Build\User" -ErrorAction SilentlyContinue > $null
    # Data\Scripts\Towns already exists by now (CopyDependencies, an AfterTargets="Build" MSBuild
    # target, seeded it from source-controlled Dependencies\Scripts\Towns during the build above),
    # so Data\Scripts\ itself is already there - but create it defensively anyway in case that
    # target didn't run for some reason, so this restore doesn't silently no-op.
    New-Item -ItemType Directory ".\Build\Data\Scripts" -ErrorAction SilentlyContinue > $null
    Move-Item ".\temp\TrainingPlaces" ".\Build\Data\Scripts\TrainingPlaces" -ErrorAction SilentlyContinue > $null
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