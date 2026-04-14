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

    $matches = [regex]::Matches($content, 'public\s+class\s+([A-Za-z_][A-Za-z0-9_]*)\s*:\s*FuseCP\.EnterpriseServer\.\1\s*,')
    if ($matches.Count -eq 0) {
        continue
    }

    foreach ($match in $matches) {
        $typeName = $match.Groups[1].Value
        $aliasName = "Base_$typeName"
        $aliasLine = "using $aliasName = FuseCP.EnterpriseServer.$typeName;"

        if ($content -notmatch [regex]::Escape($aliasLine)) {
            $content = [regex]::Replace(
                $content,
                'using\s+FuseCP\.EnterpriseServer;\r?\n',
                "using FuseCP.EnterpriseServer;`r`n$aliasLine`r`n",
                1
            )
            $updated = $true
        }

        $escapedTypeName = [regex]::Escape($typeName)
        $classPattern = "public\s+class\s+$escapedTypeName\s*:\s*FuseCP\.EnterpriseServer\.$escapedTypeName\s*,"
        $replacement = "public class $typeName : $aliasName,"
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
