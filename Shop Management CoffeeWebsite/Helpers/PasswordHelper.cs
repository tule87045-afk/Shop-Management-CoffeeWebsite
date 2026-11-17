using System;
using System.Security.Cryptography;

namespace Shop_Management_CoffeeWebsite.Helpers
{
    public static class PasswordHelper
    {
        private const int Iterations = 10000;
        private const int SaltSize = 16;
        private const int HashSize = 20;

        public static string Hash(string password)
        {
            var salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            var hash = Pbkdf2(password, salt, Iterations);
            return Iterations + "." + Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(stored)) return false;
            var parts = stored.Split('.');
            if (parts.Length != 3) return false;

            int iterations;
            if (!int.TryParse(parts[0], out iterations)) return false;

            byte[] salt, expected;
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                expected = Convert.FromBase64String(parts[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            var actual = Pbkdf2(password, salt, iterations);
            if (actual.Length != expected.Length) return false;

            // So sánh thời gian cố định
            int diff = 0;
            for (int i = 0; i < actual.Length; i++) diff |= actual[i] ^ expected[i];
            return diff == 0;
        }

        private static byte[] Pbkdf2(string password, byte[] salt, int iterations)
        {
            using (var kdf = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return kdf.GetBytes(HashSize);
            }
        }
    }
}
