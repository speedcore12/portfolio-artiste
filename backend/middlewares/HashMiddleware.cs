using System.Security.Cryptography;
using System.Text;
using DotNetEnv;

namespace middlewares
{
    public static class HashMiddleware
    {
        /// <summary>
        /// Charge le secret pour le hash depuis les variables d'environnement.
        /// </summary>
        private static string GetHashSecret()
        {
            // Charger les variables d'environnement depuis le fichier .env
            DotNetEnv.Env.Load();

            return Environment.GetEnvironmentVariable("HASH_SECRET")
                   ?? throw new InvalidOperationException("La clé de hash est manquante.");
        }

        /// <summary>
        /// Génère un hash sécurisé à partir d'une entrée.
        /// </summary>
        /// <param name="input">Données à hasher.</param>
        /// <returns>Le hash en Base64.</returns>
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
        /// <param name="input">Entrée à vérifier.</param>
        /// <param name="hash">Hash attendu.</param>
        /// <returns>True si le hash correspond, sinon False.</returns>
        public static bool VerifyHash(string input, string hash)
        {
            var generatedHash = GenerateHash(input);
            return hash == generatedHash;
        }
    }
}
