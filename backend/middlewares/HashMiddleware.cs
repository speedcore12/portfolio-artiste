using System.Security.Cryptography;
using System.Text;

namespace middlewares
{
    public static class HashMiddleware
    {
        private static string GetHashSecret()
        {
            return Environment.GetEnvironmentVariable("HASH_SECRET")
                   ?? throw new InvalidOperationException("La clé de hash est manquante.");
        }

        /// <summary>
        /// Génère un hash sécurisé à partir d'une entrée.
        /// </summary>
        public static string GenerateHash(string input)
        {
            var secretKey = GetHashSecret();

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Vérifie si une entrée correspond à un hash donné.
        /// </summary>
        public static bool VerifyHash(string input, string hash)
        {
            var generatedHash = GenerateHash(input);
            return hash == generatedHash;
        }
    }
}