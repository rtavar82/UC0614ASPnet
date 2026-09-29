using System;
using System.Security.Cryptography;

namespace UC0614_ExFinal
{
    public static class PasswordSecurity
    {
        public static string CreateHash(string password)
        {
            byte[] salt = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000))
            {
                return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(pbkdf2.GetBytes(32));
            }
        }

        public static bool Verify(string password, string storedHash)
        {
            if (String.IsNullOrWhiteSpace(storedHash) || !storedHash.Contains(":")) return false;
            string[] values = storedHash.Split(':');
            byte[] salt = Convert.FromBase64String(values[0]);
            byte[] expected = Convert.FromBase64String(values[1]);
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000))
            {
                byte[] actual = pbkdf2.GetBytes(32);
                if (actual.Length != expected.Length) return false;
                int difference = 0;
                for (int i = 0; i < actual.Length; i++) difference |= actual[i] ^ expected[i];
                return difference == 0;
            }
        }
    }
}
