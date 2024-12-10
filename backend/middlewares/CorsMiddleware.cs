namespace middlewares
{
    public static class CorsMiddleware
    {
        // Ajout de la politique CORS
        public static void AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.WithOrigins("http://localhost:3000") // Autorise uniquement cette origine
                        .AllowAnyMethod()                        // Autorise toutes les méthodes HTTP
                        .AllowAnyHeader();                       // Autorise tous les en-têtes
                });
            });
        }

        // Application de la politique CORS
        public static void UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors("AllowAll");
        }
    }
}
