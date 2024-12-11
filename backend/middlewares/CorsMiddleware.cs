namespace middlewares
{
    /// <summary>
    /// Middleware pour configurer et appliquer la politique CORS dans l'application.
    /// </summary>
    public static class CorsMiddleware
    {
        /// <summary>
        /// Configure une politique CORS permettant des requêtes provenant d'une origine spécifique.
        /// </summary>
        /// <param name="services">Collection de services à laquelle la politique CORS sera ajoutée.</param>
        public static void AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                // Définition de la politique "AllowAll"
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.WithOrigins("http://localhost:3000") // Autorise uniquement les requêtes provenant de cette origine.
                        .AllowAnyMethod()                        // Permet toutes les méthodes HTTP (GET, POST, etc.).
                        .AllowAnyHeader();                       // Permet tous les en-têtes HTTP.
                });
            });
        }

        /// <summary>
        /// Applique la politique CORS définie précédemment dans la pipeline de l'application.
        /// </summary>
        /// <param name="app">Instance de <see cref="IApplicationBuilder"/> pour configurer les middlewares.</param>
        public static void UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors("AllowAll"); // Applique la politique "AllowAll".
        }
    }
}
