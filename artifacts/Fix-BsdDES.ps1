# Fix-BsdDES.ps1 - Refactor Init() in BsdDES.cs into helper methods

$file = 'c:\git\FuseCPDevOPS-FuseCP\FuseCP\Sources\FuseCP.Server.Utils\BsdDES.cs'
$lines = [System.IO.File]::ReadAllLines($file)

# Find the line index of "void Init()"
$startIdx = -1
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match '^\s+void Init\(\)$') {
        $startIdx = $i
        break
    }
}

if ($startIdx -lt 0) {
    Write-Host "Init() not found"
    exit 1
}

Write-Host "Found Init() at line $($startIdx + 1)"

# Find the closing } of Init() by counting braces
$depth = 0
$endIdx = -1
for ($i = $startIdx; $i -lt $lines.Count; $i++) {
    foreach ($ch in $lines[$i].ToCharArray()) {
        if ($ch -eq '{') { $depth++ }
        elseif ($ch -eq '}') { $depth-- }
    }
    if ($depth -eq 0 -and $i -gt $startIdx) {
        $endIdx = $i
        break
    }
}

Write-Host "Init() ends at line $($endIdx + 1)"

# Build replacement lines
$newLines = @(
    '        void Init()',
    '        {',
    '            old_rawkey0 = old_rawkey1 = 0;',
    '            saltbits = 0;',
    '            old_salt = 0;',
    '',
    '            InitializeSboxTables();',
    '            InitializePermutationTables();',
    '            InitializeMaskArrays();',
    '            InitializePsboxMasks();',
    '',
    '            des_initialised = true;',
    '        }',
    '',
    '        void InitializeSboxTables()',
    '        {',
    '            /*',
    '             * Invert the S-boxes, reordering the input bits.',
    '             */',
    '            for (int i = 0; i < 8; i++)',
    '                for (int j = 0; j < 64; j++)',
    '                {',
    '                    int b = (j & 0x20) | ((j & 1) << 4) | ((j >> 1) & 0xf);',
    '                    u_sbox[i, j] = sbox[i, b];',
    '                }',
    '',
    '            /*',
    '             * Convert the inverted S-boxes into 4 arrays of 8 bits.',
    '             * Each will handle 12 bits of the S-box input.',
    '             */',
    '            for (int b = 0; b < 4; b++)',
    '                for (int i = 0; i < 64; i++)',
    '                    for (int j = 0; j < 64; j++)',
    '                        m_sbox[b, (i << 6) | j] =',
    '                            (byte)((u_sbox[(b << 1), i] << 4) |',
    '                            u_sbox[(b << 1) + 1, j]);',
    '        }',
    '',
    '        void InitializePermutationTables()',
    '        {',
    '            /*',
    '             * Set up the initial & final permutations into a useful form, and',
    '             * initialise the inverted key permutation.',
    '             */',
    '            for (int i = 0; i < 64; i++)',
    '            {',
    '                init_perm[final_perm[i] = (byte)(IP[i] - 1)] = (byte)i;',
    '                inv_key_perm[i] = 255;',
    '            }',
    '',
    '            /*',
    '             * Invert the key permutation and initialise the inverted key',
    '             * compression permutation.',
    '             */',
    '            for (int i = 0; i < 56; i++)',
    '            {',
    '                inv_key_perm[key_perm[i] - 1] = (byte)i;',
    '                inv_comp_perm[i] = 255;',
    '            }',
    '',
    '            /*',
    '             * Invert the key compression permutation.',
    '             */',
    '            for (int i = 0; i < 48; i++)',
    '            {',
    '                inv_comp_perm[comp_perm[i] - 1] = (byte)i;',
    '            }',
    '        }',
    '',
    '        void InitializeMaskArrays()',
    '        {',
    '            const int bits28 = 4;',
    '            const int bits24 = 8;',
    '            int inbit, obit;',
    '',
    '            /*',
    '             * Set up the OR-mask arrays for the initial and final permutations,',
    '             * and for the key initial and compression permutations.',
    '             */',
    '            for (int k = 0; k < 8; k++)',
    '            {',
    '                for (int i = 0; i < 256; i++)',
    '                {',
    '                    ip_maskl[k, i] = 0;',
    '                    ip_maskr[k, i] = 0;',
    '                    fp_maskl[k, i] = 0;',
    '                    fp_maskr[k, i] = 0;',
    '                    for (int j = 0; j < 8; j++)',
    '                    {',
    '                        inbit = 8 * k + j;',
    '                        if ((i & bits8[j]) > 0)',
    '                        {',
    '                            if ((obit = init_perm[inbit]) < 32)',
    '                                ip_maskl[k, i] |= bits32[obit];',
    '                            else',
    '                                ip_maskr[k, i] |= bits32[obit - 32];',
    '                            if ((obit = final_perm[inbit]) < 32)',
    '                                fp_maskl[k, i] |= bits32[obit];',
    '                            else',
    '                                fp_maskr[k, i] |= bits32[obit - 32];',
    '                        }',
    '                    }',
    '                }',
    '                for (int i = 0; i < 128; i++)',
    '                {',
    '                    key_perm_maskl[k, i] = 0;',
    '                    key_perm_maskr[k, i] = 0;',
    '                    for (int j = 0; j < 7; j++)',
    '                    {',
    '                        inbit = 8 * k + j;',
    '                        if ((i & bits8[j + 1]) > 0)',
    '                        {',
    '                            if ((obit = inv_key_perm[inbit]) == 255)',
    '                                continue;',
    '                            if (obit < 28)',
    '                                key_perm_maskl[k, i] |= bits32[obit + bits28];',
    '                            else',
    '                                key_perm_maskr[k, i] |= bits32[obit - 28 + bits28];',
    '                        }',
    '                    }',
    '                    comp_maskl[k, i] = 0;',
    '                    comp_maskr[k, i] = 0;',
    '                    for (int j = 0; j < 7; j++)',
    '                    {',
    '                        inbit = 7 * k + j;',
    '                        if ((i & bits8[j + 1]) > 0)',
    '                        {',
    '                            if ((obit = inv_comp_perm[inbit]) == 255)',
    '                                continue;',
    '                            if (obit < 24)',
    '                                comp_maskl[k, i] |= bits32[obit + bits24];',
    '                            else',
    '                                comp_maskr[k, i] |= bits32[obit - 24 + bits24];',
    '                        }',
    '                    }',
    '                }',
    '            }',
    '        }',
    '',
    '        void InitializePsboxMasks()',
    '        {',
    '            /*',
    '             * Invert the P-box permutation, and convert into OR-masks for',
    '             * handling the output of the S-box arrays setup above.',
    '             */',
    '            for (int i = 0; i < 32; i++)',
    '                un_pbox[pbox[i] - 1] = (byte)i;',
    '',
    '            for (int b = 0; b < 4; b++)',
    '                for (int i = 0; i < 256; i++)',
    '                {',
    '                    psbox[b, i] = 0;',
    '                    for (int j = 0; j < 8; j++)',
    '                    {',
    '                        if ((i & bits8[j]) > 0)',
    '                            psbox[b, i] |= bits32[un_pbox[8 * b + j]];',
    '                    }',
    '                }',
    '        }'
)

# Build output: lines before startIdx + newLines + lines after endIdx
$before = $lines[0..($startIdx - 1)]
$after = $lines[($endIdx + 1)..($lines.Count - 1)]
$result = $before + $newLines + $after

[System.IO.File]::WriteAllLines($file, $result, [System.Text.Encoding]::UTF8)
Write-Host "Done. File now has $($result.Count) lines (was $($lines.Count))"
