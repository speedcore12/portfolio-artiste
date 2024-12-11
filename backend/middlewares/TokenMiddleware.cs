namespace middlewares
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Data.SqlClient; // Utilisation de Microsoft.Data.SqlClient pour la connexion à la base de données
    using Schemas;
    using System.Text.Json;

    /// <summary>
    /// Middleware pour définir et gérer les routes de l'API.
    /// </summary>
    public static class RoutesMiddleware
    {
        /// <summary>
        /// Configure les routes de l'API.
        /// </summary>
        /// <param name="endpoints">L'interface permettant de mapper les routes dans l'application.</param>
        public static void MapRoutes(this IEndpointRouteBuilder endpoints)
        {
            // Groupe de routes API avec le préfixe "/api"
            var apiGroup = endpoints.MapGroup("/api");

            /// <summary>
            /// Route pour tester la connexion à la base de données.
            /// Vérifie si la connexion à la base de données est possible.
            /// </summary>
            apiGroup.MapGet("/test-db", async (IConfiguration config) =>
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                try
                {
                    // Tentative de connexion à la base de données
                    using var connection = new SqlConnection(connectionString);
                    await connection.OpenAsync();
                    return Results.Ok("Connexion réussie à la base de données !");
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"Erreur SQL : {sqlEx.Message}");
                    return Results.Problem($"Erreur SQL : {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur générale : {ex.Message}");
                    return Results.Problem($"Erreur générale : {ex.Message}");
                }
            });

            /// <summary>
            /// Route pour gérer le login.
            /// Vérifie les identifiants fournis et génère un jeton JWT si la connexion est réussie.
            /// </summary>
            apiGroup.MapPost("/login", async (HttpContext context) =>
            {
                try
                {
                    // Désérialisation du corps de la requête
                    var requestBody = await JsonSerializer.DeserializeAsync<LoginRequest>(context.Request.Body);

                    if (requestBody == null || string.IsNullOrEmpty(requestBody.Email) || string.IsNullOrEmpty(requestBody.Password))
                    {
                        return Results.BadRequest("Requête invalide : email et mot de passe requis.");
                    }

                    // Vérification des identifiants
                    if (requestBody.Email == "user@example.com" && requestBody.Password == "password123")
                    {
                        return Results.Ok(new { Message = "Connexion réussie", Token = "example-jwt-token" });
                    }

                    return Results.Json(new { Error = "Identifiants incorrects" }, statusCode: 401);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur générale : {ex.Message}");
                    return Results.Problem($"Erreur lors du traitement de la requête : {ex.Message}");
                }
            });
        }
    }
}
