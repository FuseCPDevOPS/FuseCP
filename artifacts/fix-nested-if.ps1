$j = Get-Content "C:\git\FuseCPDevOPS-FuseCP\artifacts\codeql-open-alerts.json" | ConvertFrom-Json
$alerts = $j | Where-Object { $_.rule.id -eq "cs/nested-if-statements" }
Write-Host "Total: $($alerts.Count)"

$byFile = $alerts | Group-Object { $_.most_recent_instance.location.path }
$totalFixed = 0

foreach ($group in ($byFile | Sort-Object Name)) {
    $relPath = $group.Name
    $absPath = "C:\git\FuseCPDevOPS-FuseCP\" + $relPath.Replace('/', '\')
    if (-not (Test-Path $absPath)) { continue }
    $origLines = [System.IO.File]::ReadAllLines($absPath)
    $lines = [System.Collections.Generic.List[string]]($origLines)
    $changed = $false

    # Process in reverse order to preserve line numbers
    $sorted = $group.Group | Sort-Object { $_.most_recent_instance.location.start_line } -Descending

    foreach ($alert in $sorted) {
        $outerLn = $alert.most_recent_instance.location.start_line - 1
        if ($outerLn -lt 0 -or $outerLn -ge $lines.Count) { continue }

        $outerLine = $lines[$outerLn]
        $outerTrimmed = $outerLine.TrimStart()
        $indent = $outerLine.Length - $outerTrimmed.Length
        $indentStr = $outerLine.Substring(0, $indent)

        # Must be a simple single-line if condition
        if ($outerTrimmed -notmatch '^(?:else\s+)?if\s*\(') { continue }
        if ($outerTrimmed -notmatch '\)\s*$' -and $outerTrimmed -notmatch '\)\s*\{?\s*$') {
            # Multi-line condition - skip
            continue
        }

        # Extract outer condition
        $outerCond = if ($outerTrimmed -match '^(?:else\s+)?if\s*\((.+)\)\s*\{?\s*$') { $Matches[1] } else { continue }

        # Find the outer opening brace
        $outerOpenBraceIdx = -1
        for ($i = $outerLn; $i -le [Math]::Min($outerLn+2, $lines.Count-1); $i++) {
            if ($lines[$i].Contains('{')) { $outerOpenBraceIdx = $i; break }
        }
        if ($outerOpenBraceIdx -lt 0) {
            # BRACELESS OUTER IF: body is the very next non-empty code line (inner if)
            $innerLn = -1
            for ($i = $outerLn+1; $i -lt [Math]::Min($lines.Count, $outerLn+5); $i++) {
                $t2 = $lines[$i].Trim()
                if ($t2 -ne '' -and -not ($t2 -match '^//')) { $innerLn = $i; break }
            }
            if ($innerLn -lt 0) { continue }
            # Verify there's no else after this block
            $innerTrimmedBL = $lines[$innerLn].TrimStart()
            if ($innerTrimmedBL -notmatch '^if\s*\(') { continue }
            if ($innerTrimmedBL -notmatch '\)\s*$' -and $innerTrimmedBL -notmatch '\)\s*\{?\s*$') { continue }
            $innerCondBL = if ($innerTrimmedBL -match '^if\s*\((.+)\)\s*\{?\s*$') { $Matches[1] } else { continue }
            $hasBraceBL = $innerTrimmedBL -match '\{'
            if (-not $hasBraceBL) {
                # Braceless inner: body is next single statement
                $stmtLn = -1
                for ($i = $innerLn+1; $i -lt [Math]::Min($lines.Count, $innerLn+5); $i++) {
                    $t2 = $lines[$i].Trim()
                    if ($t2 -ne '' -and -not ($t2 -match '^//')) { $stmtLn = $i; break }
                }
                if ($stmtLn -lt 0) { continue }
                # No else after statement
                $afterLn = -1
                for ($i = $stmtLn+1; $i -lt [Math]::Min($lines.Count, $stmtLn+4); $i++) {
                    if ($lines[$i].Trim() -ne '') { $afterLn = $i; break }
                }
                if ($afterLn -ge 0 -and $lines[$afterLn].TrimStart() -match '^else\b') { continue }
                $ocBL = $outerCond.Trim(); $icBL = $innerCondBL.Trim()
                if ($ocBL -match '\|\|' -or ($ocBL -match '&&' -and -not ($ocBL -match '^\('))) { $ocBL = "($ocBL)" }
                if ($icBL -match '\|\|' -or ($icBL -match '&&' -and -not ($icBL -match '^\('))) { $icBL = "($icBL)" }
                # Remove outer if line and inner if line; insert merged if line
                $rangeCountBL = $innerLn - $outerLn + 1
                $lines.RemoveRange($outerLn, $rangeCountBL)
                $lines.Insert($outerLn, "${indentStr}if ($ocBL && $icBL)")
                $changed = $true; $totalFixed++
                Write-Host "MERGED(bl-bl): $($relPath.Split('/')[-1]) L$($alert.most_recent_instance.location.start_line) [$ocBL && $icBL]"
            } else {
                # Braced inner: find matching close
                $innerDepthBL = 0; $innerStartedBL = $false; $innerCloseBL = -1
                for ($i = $innerLn; $i -lt [Math]::Min($lines.Count, $innerLn+80); $i++) {
                    foreach ($c in $lines[$i].ToCharArray()) {
                        if ($c -eq '{') { $innerDepthBL++; $innerStartedBL = $true }
                        elseif ($c -eq '}') { $innerDepthBL-- }
                    }
                    if ($innerStartedBL -and $innerDepthBL -eq 0) { $innerCloseBL = $i; break }
                }
                if ($innerCloseBL -lt 0) { continue }
                $afterLn = -1
                for ($i = $innerCloseBL+1; $i -lt [Math]::Min($lines.Count, $innerCloseBL+4); $i++) {
                    if ($lines[$i].Trim() -ne '') { $afterLn = $i; break }
                }
                if ($afterLn -ge 0 -and $lines[$afterLn].TrimStart() -match '^else\b') { continue }
                $hasPreprocessorBL = $false
                for ($i = $innerLn; $i -le $innerCloseBL; $i++) {
                    if ($lines[$i].Trim() -match '^#(if|else|endif|pragma)') { $hasPreprocessorBL = $true; break }
                }
                if ($hasPreprocessorBL) { continue }
                $innerBodyBL = @()
                for ($i = $innerLn+1; $i -lt $innerCloseBL; $i++) {
                    $innerBodyBL += [pscustomobject]@{Idx=$i; Raw=$lines[$i]}
                }
                if ($innerBodyBL.Count -eq 0) { continue }
                $ocBL = $outerCond.Trim(); $icBL = $innerCondBL.Trim()
                if ($ocBL -match '\|\|' -or ($ocBL -match '&&' -and -not ($ocBL -match '^\('))) { $ocBL = "($ocBL)" }
                if ($icBL -match '\|\|' -or ($icBL -match '&&' -and -not ($icBL -match '^\('))) { $icBL = "($icBL)" }
                $newLinesBL = @("${indentStr}if ($ocBL && $icBL)", "${indentStr}{")
                foreach ($bl in $innerBodyBL) { $newLinesBL += $bl.Raw }
                $newLinesBL += "${indentStr}}"
                $rangeCountBL = $innerCloseBL - $outerLn + 1
                $lines.RemoveRange($outerLn, $rangeCountBL)
                for ($ri = $newLinesBL.Count - 1; $ri -ge 0; $ri--) { $lines.Insert($outerLn, $newLinesBL[$ri]) }
                $changed = $true; $totalFixed++
                Write-Host "MERGED(bl-br): $($relPath.Split('/')[-1]) L$($alert.most_recent_instance.location.start_line) [$ocBL && $icBL]"
            }
            continue  # handled in braceless path
        }

        # Find matching closing brace for outer if
        $depth = 0; $started = $false; $outerCloseBraceIdx = -1
        for ($i = $outerOpenBraceIdx; $i -lt [Math]::Min($lines.Count, $outerLn+80); $i++) {
            foreach ($c in $lines[$i].ToCharArray()) {
                if ($c -eq '{') { $depth++; $started = $true }
                elseif ($c -eq '}') { $depth-- }
            }
            if ($started -and $depth -eq 0) { $outerCloseBraceIdx = $i; break }
        }
        if ($outerCloseBraceIdx -lt 0) { continue }

        # Check no else after outer closing brace
        $nextNonEmptyIdx = -1
        for ($i = $outerCloseBraceIdx+1; $i -lt [Math]::Min($lines.Count, $outerCloseBraceIdx+4); $i++) {
            if ($lines[$i].Trim() -ne '') { $nextNonEmptyIdx = $i; break }
        }
        if ($nextNonEmptyIdx -ge 0 -and $lines[$nextNonEmptyIdx].TrimStart() -match '^else\b') { continue }

        # Scan inner lines (excluding the outer { and }) for the inner if
        $innerLines = @()
        for ($i = $outerOpenBraceIdx+1; $i -lt $outerCloseBraceIdx; $i++) {
            $t = $lines[$i].Trim()
            if ($t -ne '' -and -not ($t -match '^//')) { $innerLines += [pscustomobject]@{Idx=$i; Txt=$t; Raw=$lines[$i]} }
        }

        # Must have exactly ONE non-comment code line to start: an inner if
        # (may have more for the if body)
        if ($innerLines.Count -eq 0) { continue }

        # No preprocessor directives
        $hasPreprocessor = $false
        for ($i = $outerOpenBraceIdx+1; $i -lt $outerCloseBraceIdx; $i++) {
            if ($lines[$i].Trim() -match '^#(if|else|endif|pragma)') { $hasPreprocessor = $true; break }
        }
        if ($hasPreprocessor) { continue }

        # First code line should be the inner if
        $innerIfLine = $innerLines[0]
        if ($innerIfLine.Txt -notmatch '^if\s*\(') { continue }

        # Inner if must also be single-line condition
        if ($innerIfLine.Txt -notmatch '\)\s*$' -and $innerIfLine.Txt -notmatch '\)\s*\{?\s*$') { continue }

        $innerCond = if ($innerIfLine.Txt -match '^if\s*\((.+)\)\s*\{?\s*$') { $Matches[1] } else { continue }

        # Find inner if's block or single-statement
        $innerBodyLines = @()
        $hasBrace = $innerIfLine.Txt -match '\{'
        
        if (-not $hasBrace) {
            # Next line is the single statement
            $nextCode = $innerLines | Where-Object { $_.Idx -gt $innerIfLine.Idx } | Select-Object -First 1
            if ($nextCode) { $innerBodyLines = @($nextCode) }
        } else {
            # Find the inner { and }
            $innerDepth = 0; $innerStarted = $false; $innerCloseBrace = -1
            for ($i = $innerIfLine.Idx; $i -le $outerCloseBraceIdx; $i++) {
                foreach ($c in $lines[$i].ToCharArray()) {
                    if ($c -eq '{') { $innerDepth++; $innerStarted = $true }
                    elseif ($c -eq '}') { $innerDepth-- }
                }
                if ($innerStarted -and $innerDepth -eq 0) { $innerCloseBrace = $i; break }
            }
            if ($innerCloseBrace -lt 0) { continue }
            
            # Collect inner body lines
            for ($i = $innerIfLine.Idx+1; $i -lt $innerCloseBrace; $i++) {
                $innerBodyLines += [pscustomobject]@{Idx=$i; Raw=$lines[$i]}
            }
            
            # Check no else after inner close brace
            if ($innerCloseBrace -lt $outerCloseBraceIdx) {
                for ($i = $innerCloseBrace+1; $i -lt $outerCloseBraceIdx; $i++) {
                    $t = $lines[$i].Trim()
                    if ($t -ne '' -and $t -notmatch '^//') {
                        $innerBodyLines = @()  # Signal: there's extra code after inner if inside outer body
                        break
                    }
                }
            }
        }

        # If no body lines found or they're empty, skip
        if ($innerBodyLines.Count -eq 0) { continue }

        # Build merged if condition
        # Wrap in parens if contains || or &&
        $oc = $outerCond.Trim()
        $ic = $innerCond.Trim()
        if ($oc -match '\|\|' -or ($oc -match '&&' -and -not ($oc -match '^\('))) { $oc = "($oc)" }
        if ($ic -match '\|\|' -or ($ic -match '&&' -and -not ($ic -match '^\('))) { $ic = "($ic)" }

        $mergedCond = "$oc && $ic"

        # Build the replacement lines
        $newLines = @()
        $newLines += "${indentStr}if ($mergedCond)"
        $newLines += "${indentStr}{"
        foreach ($bl in $innerBodyLines) {
            $newLines += $bl.Raw
        }
        $newLines += "${indentStr}}"

        # Replace lines from outerLn to outerCloseBraceIdx
        $rangeCount = $outerCloseBraceIdx - $outerLn + 1
        $lines.RemoveRange($outerLn, $rangeCount)
        # Insert in reverse order so they end up in correct order
        for ($ri = $newLines.Count - 1; $ri -ge 0; $ri--) {
            $lines.Insert($outerLn, $newLines[$ri])
        }

        $changed = $true; $totalFixed++
        Write-Host "MERGED: $($relPath.Split('/')[-1]) L$($alert.most_recent_instance.location.start_line) [$oc && $ic]"
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($absPath, $lines, [System.Text.UTF8Encoding]::new($false))
    }
}

Write-Host "Total fixed: $totalFixed"
