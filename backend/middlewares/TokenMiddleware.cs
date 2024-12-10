using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace middlewares
{
    public static class TokenMiddleware
    {
        private static string GetSecretKey()
        {
            return Environment.GetEnvironmentVariable("JWT_SECRET")
                   ?? throw new InvalidOperationException("La clé JWT est manquante.");
        }

        /// <summary>
        /// Vérifie si un token JWT est valide pour un utilisateur donné.
        /// </summary>
        public static bool IsTokenValid(string token, string userId)
        {
            try
            {
                var secretKey = GetSecretKey();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                // Valider le token
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Vérifier si le token contient le même utilisateur
                var jwtToken = validatedToken as JwtSecurityToken;
                var claimUserId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return userId == claimUserId;
            }
            catch
            {
                return false; // Retourne false si le token est invalide ou si une erreur survient
            }
        }

        /// <summary>
        /// Génère un token JWT pour un utilisateur donné.
        /// </summary>
        public static string GenerateToken(string userId, string login)
        {
            var secretKey = GetSecretKey();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Ajout des claims pour l'utilisateur
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, login)
            };

            // Créer le token
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12), // Durée de validité : 12 heures
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
