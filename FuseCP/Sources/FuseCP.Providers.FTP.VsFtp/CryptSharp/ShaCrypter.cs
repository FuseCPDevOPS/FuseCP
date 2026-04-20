// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

#region License
/*
CryptSharp
Copyright (c) 2013 James F. Bellinger <http://www.zer7.com/software/cryptsharp>

Permission to use, copy, modify, and/or distribute this software for any
purpose with or without fee is hereby granted, provided that the above
copyright notice and this permission notice appear in all copies.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/
#endregion

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CryptSharp.Internal;
using CryptSharp.Utility;

namespace CryptSharp
{
    // See http://www.akkadia.org/drepper/SHA-crypt.txt for algorithm details.
    /// <summary>
    /// Base class for Sha256Crypter and Sha512Crypter. 
    /// </summary>
    public abstract class ShaCrypter : Crypter
    {
        const int MinRounds = 1000;
        const int MaxRounds = 999999999;

        static readonly CrypterOptions _properties = new CrypterOptions()
        {
            { CrypterProperty.MinRounds, MinRounds },
            { CrypterProperty.MaxRounds, MaxRounds }
        }.MakeReadOnly();

        /// <inheritdoc />
        public override string GenerateSalt(CrypterOptions options)
        {
            Check.Null("options", options);

            int? rounds = options.GetValue<int?>(CrypterOption.Rounds);
            if (rounds != null)
            {
                Check.Range("CrypterOption.Rounds", (int)rounds, MinRounds, MaxRounds);
            }

            return CryptPrefix
                + (rounds != null ? string.Format("rounds={0}$", rounds) : "")
                + Base64Encoding.UnixMD5.GetString(Security.GenerateRandomBytes(12));
        }

        /// <inheritdoc />
        public override bool CanCrypt(string salt)
        {
            Check.Null("salt", salt);

            return salt.StartsWith(CryptPrefix);
        }

        /// <inheritdoc />
        public override string Crypt(byte[] password, string salt)
        {
            Check.Null("password", password);
            Check.Null("salt", salt);

            Match match = GetRegex().Match(salt);
            if (!match.Success) { throw Exceptions.Argument("salt", "Invalid salt."); }

            string roundsString = match.Groups["rounds"].Value;
            bool roundsStringPresent = roundsString.Length != 0;
            int rounds = roundsStringPresent ? int.Parse(roundsString) : 5000;
            //int requestedRounds = rounds; // PHP tests indicate the rounds string is NOT preserved if the count is outside spec.
            if (rounds < MinRounds) { rounds = MinRounds; }
            if (rounds > MaxRounds) { rounds = MaxRounds; }

            byte[] saltBytes = null, formattedKey = null, truncatedSalt = null, crypt = null;
            try
            {
                string saltString = match.Groups["salt"].Value;
                saltBytes = Encoding.ASCII.GetBytes(saltString);

                formattedKey = FormatKey(password);
                truncatedSalt = ByteArray.TruncateAndCopy(saltBytes, 16);
                crypt = Crypt(formattedKey, truncatedSalt, rounds, CreateHashAlgorithm());

                string result = CryptPrefix
                    + (roundsStringPresent ? string.Format("rounds={0}$", rounds) : "")
                    + Encoding.ASCII.GetString(truncatedSalt) + '$'
                    + Base64Encoding.UnixMD5.GetString(crypt);
                return result;
            }
            finally
            {
                Security.Clear(saltBytes);
                Security.Clear(formattedKey);
                Security.Clear(truncatedSalt);
                Security.Clear(crypt);
            }
        }

        byte[] Crypt(byte[] key, byte[] salt, int rounds, HashAlgorithm A)
        {
            byte[] P = null, S = null, H = null, I = null;

            try
            {
                A.Initialize();
                AddToDigest(A, key);
                AddToDigest(A, salt);
                AddToDigest(A, key);
                FinishDigest(A);

                I = (byte[])A.Hash.Clone();

                H = ComputeInitialHash(A, key, salt, I);
                P = ComputeRepeatedKeyDigest(A, key);
                S = ComputeRepeatedSaltDigest(A, salt, H[0]);
                ApplyRounds(A, P, S, H, rounds);

                return PermuteHash(H, GetCryptPermutation());
            }
            finally
            {
                A.Clear();
                Security.Clear(P);
                Security.Clear(S);
                Security.Clear(H);
                Security.Clear(I);
            }
        }

        static byte[] ComputeInitialHash(HashAlgorithm algorithm, byte[] key, byte[] salt, byte[] initialHash)
        {
            algorithm.Initialize();
            AddToDigest(algorithm, key);
            AddToDigest(algorithm, salt);

            AddToDigestRolling(algorithm, initialHash, 0, initialHash.Length, key.Length);

            int length = key.Length;
            for (int i = 0; i < 31 && length != 0; i++)
            {
                AddToDigest(algorithm, (length & (1 << i)) != 0 ? initialHash : key);
                length &= ~(1 << i);
            }
            FinishDigest(algorithm);

            return (byte[])algorithm.Hash.Clone();
        }

        static byte[] ComputeRepeatedKeyDigest(HashAlgorithm algorithm, byte[] key)
        {
            algorithm.Initialize();
            for (int i = 0; i < key.Length; i++)
            {
                AddToDigest(algorithm, key);
            }
            FinishDigest(algorithm);

            byte[] result = new byte[key.Length];
            CopyRolling(algorithm.Hash, 0, algorithm.Hash.Length, result);
            return result;
        }

        static byte[] ComputeRepeatedSaltDigest(HashAlgorithm algorithm, byte[] salt, byte firstHashByte)
        {
            algorithm.Initialize();
            for (int i = 0; i < 16 + firstHashByte; i++)
            {
                AddToDigest(algorithm, salt);
            }
            FinishDigest(algorithm);

            byte[] result = new byte[salt.Length];
            CopyRolling(algorithm.Hash, 0, algorithm.Hash.Length, result);
            return result;
        }

        static void ApplyRounds(HashAlgorithm algorithm, byte[] keyDigest, byte[] saltDigest, byte[] hash, int rounds)
        {
            for (int i = 0; i < rounds; i++)
            {
                algorithm.Initialize();
                if ((i & 1) != 0) { AddToDigest(algorithm, keyDigest); }
                if ((i & 1) == 0) { AddToDigest(algorithm, hash); }
                if ((i % 3) != 0) { AddToDigest(algorithm, saltDigest); }
                if ((i % 7) != 0) { AddToDigest(algorithm, keyDigest); }
                if ((i & 1) != 0) { AddToDigest(algorithm, hash); }
                if ((i & 1) == 0) { AddToDigest(algorithm, keyDigest); }
                FinishDigest(algorithm);

                Array.Copy(algorithm.Hash, hash, hash.Length);
            }
        }

        static byte[] PermuteHash(byte[] hash, int[] permutation)
        {
            byte[] crypt = new byte[hash.Length];
            for (int i = 0; i < crypt.Length; i++)
            {
                crypt[i] = hash[permutation[i]];
            }

            return crypt;
        }

        protected abstract HashAlgorithm CreateHashAlgorithm();

        protected abstract int[] GetCryptPermutation();

        protected abstract Regex GetRegex();

        protected static Regex CreateDefaultRegex(string cryptPrefix, int keyCharacters)
        {
            Check.Null("cryptPrefix", cryptPrefix);
            Check.Range("keyCharacters", keyCharacters, 0, int.MaxValue);

            string regex = @"\A"
                + Regex.Escape(cryptPrefix)
                + @"(rounds=(?<rounds>[0-9]{1,9})\$)?(?<salt>[A-Za-z0-9./]{1,99})(\$(?<crypt>[A-Za-z0-9./]{"
                + keyCharacters
                + @"}))?\z";
            return new Regex(regex, RegexOptions.CultureInvariant);
        }

        static void AddToDigest(HashAlgorithm algorithm, byte[] buffer)
        {
            AddToDigest(algorithm, buffer, 0, buffer.Length);
        }

        static void AddToDigest(HashAlgorithm algorithm, byte[] buffer, int offset, int count)
        {
            algorithm.TransformBlock(buffer, offset, count, buffer, offset);
        }

        static void AddToDigestRolling(HashAlgorithm algorithm, byte[] buffer, int offset, int inputCount, int outputCount)
        {
            int count;
            for (count = 0; count < outputCount; count += inputCount)
            {
                AddToDigest(algorithm, buffer, offset, Math.Min(outputCount - count, inputCount));
            }
        }

        static void CopyRolling(byte[] buffer, int offset, int inputCount, byte[] output)
        {
            int count;
            for (count = 0; count < output.Length; count += inputCount)
            {
                Array.Copy(buffer, offset, output, count, Math.Min(output.Length - count, inputCount));
            }
        }

        static void FinishDigest(HashAlgorithm algorithm)
        {
            algorithm.TransformFinalBlock(new byte[0], 0, 0);
        }

        byte[] FormatKey(byte[] key)
        {
            int length = ByteArray.NullTerminatedLength(key, key.Length);
            return ByteArray.TruncateAndCopy(key, length);
        }

        protected abstract string CryptPrefix
        {
            get;
        }

        /// <inheritdoc />
        public override CrypterOptions Properties
        {
            get { return _properties; }
        }
    }
}
