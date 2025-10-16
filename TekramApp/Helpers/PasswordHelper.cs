using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace TekramApp.Helpers
{
    public class PasswordHelper
    {
        public interface IPasswordHasher
        {
            string HashPassword(string password);
            bool VerifyPassword(string hashedPassword, string providedPassword);
        }

        public class PasswordHasher : IPasswordHasher
        {
            public string HashPassword(string password)
            {
                byte[] salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                return $"{Convert.ToBase64String(salt)}.{hashed}";
            }

            public bool VerifyPassword(string hashedPassword, string providedPassword)
            {
                var parts = hashedPassword.Split('.');
                if (parts.Length != 2) return false;

                var salt = Convert.FromBase64String(parts[0]);
                var hash = parts[1];

                string providedHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: providedPassword,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));

                return hash == providedHash;
            }
        }
    }
}
