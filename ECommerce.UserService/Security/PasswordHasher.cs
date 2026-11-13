using System.Security.Cryptography;

namespace ECommerce.UserService.Security
{
    public class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "Password cannot be empty.",
                    nameof(password));
            }

            var salt =
                RandomNumberGenerator.GetBytes(
                    SaltSize);

            var hash =
                Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    Iterations,
                    HashAlgorithmName.SHA256,
                    HashSize);

            return $"{Convert.ToBase64String(salt)}." +
                   $"{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(
            string password,
            string storedPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(storedPasswordHash))
            {
                return false;
            }

            var parts =
                storedPasswordHash.Split('.');

            if (parts.Length != 2)
            {
                return false;
            }

            try
            {
                var salt =
                    Convert.FromBase64String(
                        parts[0]);

                var storedHash =
                    Convert.FromBase64String(
                        parts[1]);

                var computedHash =
                    Rfc2898DeriveBytes.Pbkdf2(
                        password,
                        salt,
                        Iterations,
                        HashAlgorithmName.SHA256,
                        HashSize);

                return CryptographicOperations
                    .FixedTimeEquals(
                        storedHash,
                        computedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}