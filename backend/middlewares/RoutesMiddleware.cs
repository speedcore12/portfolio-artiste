namespace middlewares
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Data.SqlClient; // Utilisation de Microsoft.Data.SqlClient
    using Schemas;
    using System.Text.Json;

    public static class RoutesMiddleware
    {
        public static void MapRoutes(this IEndpointRouteBuilder endpoints)
        {
            var apiGroup = endpoints.MapGroup("/api");

            // Route pour tester la connexion à la base de données
            apiGroup.MapGet("/test-db", async (IConfiguration config) =>
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                try
                {
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

            // Route pour le login
            apiGroup.MapPost("/login", async (HttpContext context) =>
            {
                try
                {
                    var requestBody = await JsonSerializer.DeserializeAsync<LoginRequest>(context.Request.Body);

                    if (requestBody == null || string.IsNullOrEmpty(requestBody.Email) || string.IsNullOrEmpty(requestBody.Password))
                    {
                        return Results.BadRequest("Requête invalide : email et mot de passe requis.");
                    }

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
