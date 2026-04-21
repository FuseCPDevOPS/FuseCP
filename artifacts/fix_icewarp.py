import sys

file = r'c:\git\FuseCPDevOPS-FuseCP\FuseCP\Sources\FuseCP.Providers.Mail.IceWarp\IceWarp.cs'
with open(file, 'r', encoding='utf-8') as f:
    content = f.read()

old = '''        private static string GeneratePassword(int minLength, int nonAlphaNumCount, int digitCount, int alphaCount)
        {
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var chars = new List<char>();

            for (int i = 0; i < Math.Max(0, alphaCount); i++)
                chars.Add(letters[RandomNumberGenerator.GetInt32(letters.Length)]);

            for (int i = 0; i < Math.Max(0, digitCount); i++)
                chars.Add(digits[RandomNumberGenerator.GetInt32(digits.Length)]);

            for (int i = 0; i < Math.Max(0, nonAlphaNumCount); i++)
                chars.Add(symbols[RandomNumberGenerator.GetInt32(symbols.Length)]);

            while (chars.Count < Math.Max(0, minLength))
                chars.Add(letters[RandomNumberGenerator.GetInt32(letters.Length)]);

            for (int i = chars.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            return new string(chars.ToArray());
        }'''

new = '''        private static string GeneratePassword(int minLength, int nonAlphaNumCount, int digitCount, int alphaCount)
        {
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var chars = new List<char>();
            AddRandomChars(chars, letters, alphaCount);
            AddRandomChars(chars, digits, digitCount);
            AddRandomChars(chars, symbols, nonAlphaNumCount);

            while (chars.Count < Math.Max(0, minLength))
                chars.Add(letters[RandomNumberGenerator.GetInt32(letters.Length)]);

            ShuffleChars(chars);
            return new string(chars.ToArray());
        }

        private static void AddRandomChars(List<char> chars, string pool, int count)
        {
            for (int i = 0; i < Math.Max(0, count); i++)
                chars.Add(pool[RandomNumberGenerator.GetInt32(pool.Length)]);
        }

        private static void ShuffleChars(List<char> chars)
        {
            for (int i = chars.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }
        }'''

if old in content:
    content = content.replace(old, new)
    with open(file, 'w', encoding='utf-8', newline='') as f:
        f.write(content)
    print('Replacement successful')
else:
    print('Pattern not found - trying CRLF version')
    old_crlf = old.replace('\n', '\r\n')
    if old_crlf in content:
        new_crlf = new.replace('\n', '\r\n')
        content = content.replace(old_crlf, new_crlf)
        with open(file, 'w', encoding='utf-8') as f:
            f.write(content)
        print('CRLF replacement successful')
    else:
        print('Pattern not found with either LF or CRLF')
        lines = content.split('\n')
        for i, line in enumerate(lines):
            if 'GeneratePassword' in line:
                print(f'Line {i+1}: {repr(line)}')
