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
using System.Security.Cryptography;
using System.Text;

namespace FuseCP.Providers.Common
{
    public class PasswdHelper
    {
        protected static readonly string MD5_MAGIC_PREFIX = "$apr1$";
        protected const int MD5_DIGESTSIZE = 16;
        protected static readonly string SHA_MAGIC_PREFIX = "{SHA}";

        private static readonly string itoa64 =         /* 0 ... 63 => ASCII - 64 */
            "./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        private static readonly Random _random;

        static PasswdHelper()
        {
            _random = new Random();

        }

        public static string to64(ulong v, int n)
        {
            StringBuilder sb = new StringBuilder();
            while (--n >= 0)
            {
                sb.Append(itoa64[(int)v & 0x3f]);
                v >>= 6;
            }
            return sb.ToString();
        }


        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in ba)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }


        public static byte[] getMD5HashHex(string s)
        {
            using MD5 md5 = MD5.Create();
            return md5.ComputeHash(Encoding.ASCII.GetBytes(s));
        }


        public static string MD5Encode(string pw, string salt)
        {
            // ��� �������� �������� �����:
            // $apr1$Vs5.....$iSQlpTkND9RjL7iAMTjDt.

            // ��� ������������� ������ �������� ��������� ������ - ����

            salt = NormalizeMd5Salt(salt);

            ByteVector s = new ByteVector();
            ByteVector s1 = new ByteVector();

            InitializeMd5Vectors(pw, salt, s, s1);
            byte[] final = s1.GetMD5Hash();

            AppendDigestChunks(pw, s, final);
            ClearBytes(final);
            AppendAlternatingSourceBytes(pw, s, final);

            final = s.GetMD5Hash();
            final = IterateMd5Rounds(pw, salt, s1, final);

            return BuildMd5Password(salt, final);
        }

        private static string NormalizeMd5Salt(string salt)
        {
            if (salt.StartsWith(MD5_MAGIC_PREFIX))
            {
                salt = salt.Substring(MD5_MAGIC_PREFIX.Length);
            }

            int sp = salt.IndexOf('$');
            if (sp < 0 || sp > 8)
            {
                sp = 8;
            }

            return salt.Substring(0, sp);
        }

        private static void InitializeMd5Vectors(string pw, string salt, ByteVector s, ByteVector s1)
        {
            s.Add(pw);
            s.Add(MD5_MAGIC_PREFIX);
            s.Add(salt);

            s1.Add(pw);
            s1.Add(salt);
            s1.Add(pw);
        }

        private static void AppendDigestChunks(string pw, ByteVector s, byte[] final)
        {
            for (int i = pw.Length; i > 0; i -= MD5_DIGESTSIZE)
            {
                s.Add(final, 0, (i > MD5_DIGESTSIZE) ? MD5_DIGESTSIZE : i);
            }
        }

        private static void ClearBytes(byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                value[i] = 0;
            }
        }

        private static void AppendAlternatingSourceBytes(string pw, ByteVector s, byte[] final)
        {
            for (int i = pw.Length; i != 0; i >>= 1)
            {
                if ((i & 0x01) == 1)
                {
                    s.Add(final, 0, 1);
                }
                else
                {
                    s.Add(pw.Substring(0, 1));
                }
            }
        }

        private static byte[] IterateMd5Rounds(string pw, string salt, ByteVector s1, byte[] final)
        {
            for (int i = 0; i < 1000; i++)
            {
                s1.Clear();

                if ((i & 1) != 0)
                {
                    s1.Add(pw);
                }
                else
                {
                    s1.Add(final);
                }

                if ((i % 3) != 0)
                {
                    s1.Add(salt);
                }

                if ((i % 7) != 0)
                {
                    s1.Add(pw);
                }

                if ((i & 1) != 0)
                {
                    s1.Add(final);
                }
                else
                {
                    s1.Add(pw);
                }

                final = s1.GetMD5Hash();
            }

            return final;
        }

        private static string BuildMd5Password(string salt, byte[] final)
        {
            var password = string.Empty;
            ulong l;

            l = ((ulong)final[0] << 16) | ((ulong)final[6] << 8) | ((ulong)final[12]);
            password += PasswdHelper.to64(l, 4);
            l = ((ulong)final[1] << 16) | ((ulong)final[7] << 8) | ((ulong)final[13]);
            password += PasswdHelper.to64(l, 4);
            l = ((ulong)final[2] << 16) | ((ulong)final[8] << 8) | ((ulong)final[14]);
            password += PasswdHelper.to64(l, 4);
            l = ((ulong)final[3] << 16) | ((ulong)final[9] << 8) | ((ulong)final[15]);
            password += PasswdHelper.to64(l, 4);
            l = ((ulong)final[4] << 16) | ((ulong)final[10] << 8) | ((ulong)final[5]);
            password += PasswdHelper.to64(l, 4);
            l = ((ulong)final[11]);
            password += PasswdHelper.to64(l, 2);

            return string.Format("{0}{1}${2}", MD5_MAGIC_PREFIX, salt, password);
        }


        public static string SHA1Encode(string clear)
        {
            if (clear.StartsWith(SHA_MAGIC_PREFIX))
            {
                clear = clear.Substring(SHA_MAGIC_PREFIX.Length);
            }

            using SHA1 sha = SHA1.Create();

            string cr = Convert.ToBase64String(
                sha.ComputeHash(Encoding.Default.GetBytes(clear))
                );
            return SHA_MAGIC_PREFIX + cr;
        }


        public static string GetRandomSalt()
        { 
            return to64((ulong)_random.Next(), 8);
        }


        public static string DigestEncode(string username, string realm, string passwd)
        {
            using MD5 md5 = MD5.Create();

            byte[] b = md5.ComputeHash(Encoding.ASCII.GetBytes(
                                           string.Format("{0}:{1}:{2}", username, realm, passwd)
                                           ));

            StringBuilder sb = new StringBuilder(b.Length*2);
            for (int i = 0; i < b.Length; ++i)
            {
                sb.Append( String.Format("{0:x2}", b[i]) );
            }

            return sb.ToString();
        }
    }
}
