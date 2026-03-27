$ErrorActionPreference = 'Stop'

$repoRoot = (Get-Location).Path
$targets = @(
    'FuseCP/Sources/FuseCP.WebPortal/tinymce/themes/inlite/scratch/inline/theme.js',
    'FuseCP/Sources/FuseCP.WebPortal/tinymce/themes/inlite/scratch/inline/theme.raw.js',
    'FuseCP/Sources/FuseCP.WebPortal/JavaScript/jquery.window.js',
    'FuseCP/Sources/FuseCP.WebPortal/App_Themes/Default/addons/fuelux/js/fuelux.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/jquery-ui-1-13.3.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/DataTables/buttons.flash.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/DataTables/buttons.print.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/DataTables/dataTables.autoFill.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/DataTables/dataTables.colReorder.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/DataTables/dataTables.fixedColumns.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/DataTables/dataTables.fixedHeader.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/DataTables/dataTables.keyTable.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/DataTables/dataTables.material.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/DataTables/dataTables.responsive.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/DataTables/dataTables.rowReorder.js',
    'FuseCP/Sources/FuseCP.WebDavPortal/Scripts/DataTables/jquery.dataTables.js',
    'tools/MSBuildCommunityTasks/Source/Source/MSBuild.Community.Tasks/swagger-codegen-2.1.6/modules/swagger-codegen/src/main/resources/swagger-static/assets/js/bootstrap.js',
    'tools/MSBuildCommunityTasks/Source/Source/MSBuild.Community.Tasks/swagger-codegen-2.1.6/modules/swagger-codegen/src/main/resources/swagger-static/assets/js/main.js'
)

$totalNormalized = 0
$filesChanged = 0

foreach ($rel in $targets) {
    $abs = Join-Path $repoRoot ($rel -replace '/', '\\')
    if (-not (Test-Path $abs)) { continue }

    $lines = [System.Collections.Generic.List[string]]([System.IO.File]::ReadAllLines($abs))
    $changed = $false

    for ($i = 1; $i -lt $lines.Count; $i++) {
        $curr = $lines[$i].Trim()
        $prev = $lines[$i - 1].Trim()

        if ($curr -match '^(?<name>[A-Za-z_$][A-Za-z0-9_$]*)\s*;\s*$') {
            $name = $Matches['name']

            if ($prev -match ('^void\s+' + [regex]::Escape($name) + '\s*;\s*$') -or $prev -match ('^' + [regex]::Escape($name) + '\s*;\s*$')) {
                $lines.RemoveAt($i)
                $i--
                $changed = $true
                $totalNormalized++
                continue
            }

            $indentLen = $lines[$i].Length - $lines[$i].TrimStart().Length
            $indent = $lines[$i].Substring(0, $indentLen)
            $lines[$i] = "${indent}void $name;"
            $changed = $true
            $totalNormalized++
            continue
        }
    }

    if ($changed) {
        [System.IO.File]::WriteAllLines($abs, $lines, [System.Text.UTF8Encoding]::new($false))
        $filesChanged++
    }
}

Write-Output "JS_MARKER_LINES_NORMALIZED=$totalNormalized"
Write-Output "JS_MARKER_FILES_CHANGED=$filesChanged"
