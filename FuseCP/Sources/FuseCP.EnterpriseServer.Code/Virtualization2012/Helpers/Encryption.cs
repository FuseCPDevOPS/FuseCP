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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.IO;

namespace FuseCP.EnterpriseServer.Code.Virtualization2012.Helpers
{
    class Encryption
    {
        private const string ModernPayloadPrefix = "v2:";
        private const int ModernNonceSize = 12;
        private const int ModernTagSize = 16;

        public static string Encrypt(string prm_text_to_encrypt, string prm_key, string prm_iv)
        {
            var key = Convert.FromBase64String(prm_key);
            if (key.Length != 32)
                throw new CryptographicException("Guacamole key must be 32 bytes (Base64-encoded).");

            var iv = Convert.FromBase64String(prm_iv);
            var plain = Encoding.UTF8.GetBytes(prm_text_to_encrypt);
            var nonce = new byte[ModernNonceSize];
            RandomNumberGenerator.Fill(nonce);

            var cipher = new byte[plain.Length];
            var tag = new byte[ModernTagSize];

            using var aesGcm = new AesGcm(key);
            aesGcm.Encrypt(nonce, plain, cipher, tag, iv);

            var payload = new byte[nonce.Length + tag.Length + cipher.Length];
            Buffer.BlockCopy(nonce, 0, payload, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, payload, nonce.Length, tag.Length);
            Buffer.BlockCopy(cipher, 0, payload, nonce.Length + tag.Length, cipher.Length);

            return ModernPayloadPrefix + Convert.ToBase64String(payload);
        }

        public static string Decrypt(string prm_text_to_decrypt, string prm_key, string prm_iv)
        {
            var key = Convert.FromBase64String(prm_key);
            if (key.Length != 32)
                throw new CryptographicException("Guacamole key must be 32 bytes (Base64-encoded).");

            var iv = Convert.FromBase64String(prm_iv);

            if (!String.IsNullOrEmpty(prm_text_to_decrypt)
                && prm_text_to_decrypt.StartsWith(ModernPayloadPrefix, StringComparison.Ordinal))
            {
                var modernPayload = Convert.FromBase64String(prm_text_to_decrypt.Substring(ModernPayloadPrefix.Length));
                if (modernPayload.Length < ModernNonceSize + ModernTagSize)
                    throw new CryptographicException("Invalid Guacamole encrypted payload.");

                var nonce = new byte[ModernNonceSize];
                var tag = new byte[ModernTagSize];
                var cipherLen = modernPayload.Length - ModernNonceSize - ModernTagSize;
                var cipher = new byte[cipherLen];
                var plain = new byte[cipherLen];

                Buffer.BlockCopy(modernPayload, 0, nonce, 0, nonce.Length);
                Buffer.BlockCopy(modernPayload, nonce.Length, tag, 0, tag.Length);
                Buffer.BlockCopy(modernPayload, nonce.Length + tag.Length, cipher, 0, cipher.Length);

                using var aesGcm = new AesGcm(key);
                aesGcm.Decrypt(nonce, cipher, tag, plain, iv);
                return Encoding.UTF8.GetString(plain);
            }

            // Backward-compatible AES-CBC path.
            using var rj = Aes.Create();
            rj.Padding = PaddingMode.PKCS7;
            rj.Mode = CipherMode.CBC;
            rj.KeySize = 256;

            var decryptor = rj.CreateDecryptor(key, iv);

            var sEncrypted = Convert.FromBase64String(prm_text_to_decrypt);
            using var msDecrypt = new MemoryStream(sEncrypted);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var plainBuffer = new MemoryStream();
            csDecrypt.CopyTo(plainBuffer);

            return Encoding.UTF8.GetString(plainBuffer.ToArray());
        }

        public static void GenerateIV(out string IV)
        {
            using var rj = Aes.Create();
            rj.Padding = PaddingMode.PKCS7;
            rj.Mode = CipherMode.CBC;
            rj.KeySize = 256;
            //rj.GenerateKey();
            rj.GenerateIV();

            //key = Convert.ToBase64String(rj.Key);
            IV = Convert.ToBase64String(rj.IV);
        }
    }
}
