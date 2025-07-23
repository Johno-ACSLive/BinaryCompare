param (
    [string]$File1 = "C:\Path\To\File1.bin",
    [string]$File2 = "C:\Path\To\File2.bin",
    [int]$BufferSize = 64MB,
    [switch]$ContinueOnMismatch = $false
)

function Compare-BinaryFiles {
    param (
        [string]$Path1,
        [string]$Path2,
        [int]$BufferSize,
        [switch]$ContinueOnMismatch
    )

    if (!(Test-Path $Path1) -or !(Test-Path $Path2)) {
        Write-Error "❌ One or both files do not exist."
        return
    }

    $length1 = (Get-Item $Path1).Length
    $length2 = (Get-Item $Path2).Length

    if ($length1 -ne $length2) {
        Write-Host "❌ Files are different sizes: $length1 vs $length2"
        return
    }

    $fs1 = New-Object System.IO.BufferedStream ([System.IO.File]::OpenRead($Path1), $BufferSize)
    $fs2 = New-Object System.IO.BufferedStream ([System.IO.File]::OpenRead($Path2), $BufferSize)

    $buffer1 = New-Object byte[] $BufferSize
    $buffer2 = New-Object byte[] $BufferSize

    $totalRead = 0
    $startTime = Get-Date
    $lastProgressUpdate = 0
    $mismatchCount = 0

    try {
        while ($true) {
            $bytesRead1 = $fs1.Read($buffer1, 0, $BufferSize)
            $bytesRead2 = $fs2.Read($buffer2, 0, $BufferSize)

            if ($bytesRead1 -ne $bytesRead2) {
                Write-Warning "❌ File read mismatch at offset $totalRead."
                break
            }
            if ($bytesRead1 -eq 0) { break } # EOF

            # Copy valid bytes into new arrays for comparison
            $chunk1 = New-Object byte[] $bytesRead1
            $chunk2 = New-Object byte[] $bytesRead2
            [Array]::Copy($buffer1, 0, $chunk1, 0, $bytesRead1)
            [Array]::Copy($buffer2, 0, $chunk2, 0, $bytesRead2)

            if (-not [System.Linq.Enumerable]::SequenceEqual($chunk1, $chunk2)) {
                Write-Warning "❌ Mismatch found at byte offset: $totalRead"
                $mismatchCount++
                if (-not $ContinueOnMismatch) {
                    return
                }
            }

            $totalRead += $bytesRead1

            # Show progress every 10MB
            if (($totalRead - $lastProgressUpdate) -ge 10MB) {
                $percent = ($totalRead / $length1) * 100
                $elapsed = (Get-Date) - $startTime
                $rate = if ($elapsed.TotalSeconds -gt 0) { $totalRead / $elapsed.TotalSeconds } else { 0 }
                $remaining = if ($rate -gt 0) { ($length1 - $totalRead) / $rate } else { 0 }

                Write-Progress -Activity "Comparing binary files..." `
                               -Status ("{0:N2}% - ETA: {1}" -f $percent, [TimeSpan]::FromSeconds($remaining).ToString("hh\:mm\:ss")) `
                               -PercentComplete $percent

                $lastProgressUpdate = $totalRead
            }
        }

        if ($mismatchCount -eq 0) {
            Write-Host "✅ Files are identical." -ForegroundColor Green
        } else {
            Write-Warning "⚠️ $mismatchCount mismatches found."
        }

    } finally {
        $fs1.Close()
        $fs2.Close()
    }
}

Compare-BinaryFiles -Path1 $File1 -Path2 $File2 -BufferSize $BufferSize -ContinueOnMismatch:$ContinueOnMismatch
