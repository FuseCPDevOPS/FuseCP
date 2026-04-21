# Fix-IceWarp.ps1 - Refactor GeneratePassword in IceWarp.cs

$file = 'c:\git\FuseCPDevOPS-FuseCP\FuseCP\Sources\FuseCP.Providers.Mail.IceWarp\IceWarp.cs'
$lines = [System.IO.File]::ReadAllLines($file)

# Find the line index of "private static string GeneratePassword"
$startIdx = -1
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match 'private static string GeneratePassword') {
        $startIdx = $i
        break
    }
}

if ($startIdx -lt 0) {
    Write-Host "GeneratePassword not found"
    exit 1
}

Write-Host "Found GeneratePassword at line $($startIdx + 1)"

# Find the closing } of GeneratePassword by counting braces
$depth = 0
$endIdx = -1
for ($i = $startIdx; $i -lt $lines.Count; $i++) {
    $depth += ($lines[$i].ToCharArray() | Where-Object { $_ -eq '{' }).Count
    $depth -= ($lines[$i].ToCharArray() | Where-Object { $_ -eq '}' }).Count
    if ($depth -eq 0 -and $i -gt $startIdx) {
        $endIdx = $i
        break
    }
}

Write-Host "Method ends at line $($endIdx + 1)"

# Build new lines
$newLines = @(
    '        private static string GeneratePassword(int minLength, int nonAlphaNumCount, int digitCount, int alphaCount)',
    '        {',
    '            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";',
    '            const string digits = "0123456789";',
    '            const string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";',
    '',
    '            var chars = new List<char>();',
    '            AddRandomChars(chars, letters, alphaCount);',
    '            AddRandomChars(chars, digits, digitCount);',
    '            AddRandomChars(chars, symbols, nonAlphaNumCount);',
    '',
    '            while (chars.Count < Math.Max(0, minLength))',
    '                chars.Add(letters[RandomNumberGenerator.GetInt32(letters.Length)]);',
    '',
    '            ShuffleChars(chars);',
    '            return new string(chars.ToArray());',
    '        }',
    '',
    '        private static void AddRandomChars(List<char> chars, string pool, int count)',
    '        {',
    '            for (int i = 0; i < Math.Max(0, count); i++)',
    '                chars.Add(pool[RandomNumberGenerator.GetInt32(pool.Length)]);',
    '        }',
    '',
    '        private static void ShuffleChars(List<char> chars)',
    '        {',
    '            for (int i = chars.Count - 1; i > 0; i--)',
    '            {',
    '                int j = RandomNumberGenerator.GetInt32(i + 1);',
    '                (chars[i], chars[j]) = (chars[j], chars[i]);',
    '            }',
    '        }'
)

# Build output: lines before startIdx + newLines + lines after endIdx
$before = $lines[0..($startIdx - 1)]
$after = $lines[($endIdx + 1)..($lines.Count - 1)]
$result = $before + $newLines + $after

[System.IO.File]::WriteAllLines($file, $result, [System.Text.Encoding]::UTF8)
Write-Host "Replacement done. New file has $($result.Count) lines (was $($lines.Count))"
