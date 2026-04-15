param(
    [Parameter(Mandatory = $true)]
    [string]$RootPath
)

if (-not (Test-Path $RootPath)) {
    return
}

$files = Get-ChildItem -Path $RootPath -Filter *.g.cs -Recurse -File
foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw
    $updated = $false

    $registryContent = [regex]::Replace(
        $content,
        'typeof\(FuseCP\.Server\.Services\.([A-Za-z_][A-Za-z0-9_]*)\)',
        {
            param($registryMatch)

            $typeName = $registryMatch.Groups[1].Value
            if ($typeName.EndsWith('Service')) {
                return $registryMatch.Value
            }

            return "typeof(FuseCP.Server.Services.${typeName}Service)"
        })
    if ($registryContent -ne $content) {
        $content = $registryContent
        $updated = $true
    }

    $matches = [regex]::Matches($content, 'public\s+class\s+([A-Za-z_][A-Za-z0-9_]*)\s*:\s*FuseCP\.Server\.\1\s*,')
    if ($matches.Count -eq 0) {
        if ($updated) {
            Set-Content -Path $file.FullName -Value $content -NoNewline
        }
        continue
    }

    foreach ($match in $matches) {
        $typeName = $match.Groups[1].Value
        $serviceTypeName = "${typeName}Service"
        $aliasName = "Base_$typeName"
        $aliasLine = "using $aliasName = FuseCP.Server.$typeName;"

        if ($content -notmatch [regex]::Escape($aliasLine)) {
            $content = [regex]::Replace(
                $content,
                'using\s+FuseCP\.Server;\r?\n',
                "using FuseCP.Server;`r`n$aliasLine`r`n",
                1
            )
            $updated = $true
        }

        $escapedTypeName = [regex]::Escape($typeName)
        $classPattern = "public\s+class\s+$escapedTypeName\s*:\s*FuseCP\.Server\.$escapedTypeName\s*,"
        $replacement = "public class $serviceTypeName : $aliasName,"
        $newContent = [regex]::Replace($content, $classPattern, $replacement, 1)
        if ($newContent -ne $content) {
            $content = $newContent
            $updated = $true
        }
    }

    if ($updated) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
    }
}
