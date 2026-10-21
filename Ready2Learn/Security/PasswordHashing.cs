using System;
using System.Security.Cryptography;

namespace Ready2Learn.Security
{
    public static class PasswordHashing
    {
        // Format: {iterations}.{saltBase64}.{hashBase64}
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32;  // 256 bit
        private const int Iterations = 100_000;

        public static string CreateHash(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(input, salt, Iterations, HashAlgorithmName.SHA256);
            var key = pbkdf2.GetBytes(KeySize);

            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public static bool VerifyHash(string hash, string input)
        {
            if (hash == null || input == null) return false;

            var parts = hash.Split('.');
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(input, salt, iterations, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(key.Length);

            return CryptographicOperations.FixedTimeEquals(computed, key);
        }
    }
}