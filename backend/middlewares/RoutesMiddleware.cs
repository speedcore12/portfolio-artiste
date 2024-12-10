namespace middlewares
{
    using Microsoft.Data.SqlClient;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;

    public static class RoutesMiddleware
    {
        public static void MapRoutes(this IEndpointRouteBuilder endpoints)
        {
            // Crée un groupe de routes avec le préfixe /api
            var apiGroup = endpoints.MapGroup("/api");

            // DB testing
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
        }
    }
}
