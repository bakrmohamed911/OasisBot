# Converts legacy training-place waypoint scripts (go(x,y) / go,x,y / delay(N) / wait /
# teleport / fly / buff / skill / kill / scriptversion=) into OasisBot's native script
# syntax ("move XOffset YOffset ZOffset XSector YSector", "wait Milliseconds").
#
# Coordinate formula matches RSBot.Core.Objects.Position's own global-coordinate
# constructor exactly (center sector 135/92, 192 units/sector, offset scaled x10):
#   xOffset = abs(x) % 192 * 10; if x < 0: xOffset = 1920 - xOffset
#   yOffset = abs(y) % 192 * 10; if y < 0: yOffset = 1920 - yOffset
#   regionX = round((x - xOffset/10) / 192 + 135)
#   regionY = round((y - yOffset/10) / 192 + 92)
#
# Commands with no safe automatic translation (teleport/fly/buff/skill/kill/
# scriptversion=) are kept as commented-out originals rather than guessed at.
# Files using the packed-hex "move(HEXBLOB)" format are skipped entirely - no known
# byte layout, and guessing wrong here would silently send the character to the
# wrong spot with no error, unlike a plain parse failure.

param(
    [string]$SourceRoot = "C:\Users\engba\Downloads\New folder",
    [string]$DestRoot = "D:\Games\OasisBot\Build\Data\Scripts\TrainingPlaces"
)

function Convert-GlobalToRegion {
    param([double]$x, [double]$y)

    $xOffset = ([Math]::Abs($x) % 192.0) * 10.0
    if ($x -lt 0) { $xOffset = 1920.0 - $xOffset }

    $yOffset = ([Math]::Abs($y) % 192.0) * 10.0
    if ($y -lt 0) { $yOffset = 1920.0 - $yOffset }

    # MathF.Round(float) in the original C# formula uses no explicit MidpointRounding,
    # which defaults to ToEven (banker's rounding) - match that exactly so a midpoint
    # coordinate can't land in an off-by-one sector.
    $regionX = [Math]::Round((($x - $xOffset / 10.0) / 192.0 + 135.0), [MidpointRounding]::ToEven)
    $regionY = [Math]::Round((($y - $yOffset / 10.0) / 192.0 + 92.0), [MidpointRounding]::ToEven)

    # Region.X/Y are bytes (0-255) - wrap the same way an unchecked byte cast would,
    # so a stray out-of-range value can't crash the conversion.
    $regionX = [byte]([int]$regionX -band 0xFF)
    $regionY = [byte]([int]$regionY -band 0xFF)

    return [PSCustomObject]@{
        XOffset = [Math]::Round($xOffset, 2)
        YOffset = [Math]::Round($yOffset, 2)
        RegionX = $regionX
        RegionY = $regionY
    }
}

$goParen = [regex]'^go\((-?\d+(?:\.\d+)?),\s*(-?\d+(?:\.\d+)?)\)'
$goComma = [regex]'^go,(-?\d+(?:\.\d+)?),(-?\d+(?:\.\d+)?)'
$delay = [regex]'^delay\((\d+)\)'
$hexMove = [regex]'^move\('

$files = Get-ChildItem -Path $SourceRoot -Filter "*.txt" -Recurse -File
$converted = 0
$skippedHex = @()
$totalUnknownLines = 0

foreach ($file in $files) {
    $lines = Get-Content -LiteralPath $file.FullName -ErrorAction SilentlyContinue
    if ($null -eq $lines) { continue }

    if ($lines -match '^move\(') {
        $skippedHex += $file.FullName
        continue
    }

    $outLines = New-Object System.Collections.Generic.List[string]
    $outLines.Add("# Converted from legacy training-place script: $($file.Name)")

    foreach ($raw in $lines) {
        $line = $raw.Trim()
        if ($line.Length -eq 0) { continue }

        $m = $goParen.Match($line)
        if ($m.Success) {
            $pos = Convert-GlobalToRegion -x ([double]$m.Groups[1].Value) -y ([double]$m.Groups[2].Value)
            $outLines.Add("move $($pos.XOffset) $($pos.YOffset) 0 $($pos.RegionX) $($pos.RegionY)")
            continue
        }

        $m = $goComma.Match($line)
        if ($m.Success) {
            $pos = Convert-GlobalToRegion -x ([double]$m.Groups[1].Value) -y ([double]$m.Groups[2].Value)
            $outLines.Add("move $($pos.XOffset) $($pos.YOffset) 0 $($pos.RegionX) $($pos.RegionY)")
            continue
        }

        $m = $delay.Match($line)
        if ($m.Success) {
            $outLines.Add("wait $($m.Groups[1].Value)")
            continue
        }

        if ($line -eq "wait") {
            $outLines.Add("wait 1000  # no explicit duration in original - defaulted to 1s")
            continue
        }

        if ($line -match '^(teleport|fly|buff|skill|kill|scriptversion)') {
            $outLines.Add("# UNTRANSLATED (needs manual mapping): $line")
            $totalUnknownLines++
            continue
        }

        if ($line.StartsWith("//") -or $line.StartsWith("#")) {
            $outLines.Add("# $line")
            continue
        }

        $outLines.Add("# UNRECOGNIZED LINE: $line")
        $totalUnknownLines++
    }

    $relative = $file.FullName.Substring($SourceRoot.Length).TrimStart('\')
    $destPath = Join-Path $DestRoot $relative
    $destDir = Split-Path $destPath -Parent
    if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Path $destDir -Force | Out-Null }

    Set-Content -LiteralPath $destPath -Value $outLines -Encoding utf8
    $converted++
}

Write-Output "Converted: $converted files"
Write-Output "Skipped (hex move format): $($skippedHex.Count) files"
foreach ($f in $skippedHex) { Write-Output "  - $f" }
Write-Output "Lines needing manual attention (teleport/fly/buff/skill/kill/unrecognized): $totalUnknownLines"
