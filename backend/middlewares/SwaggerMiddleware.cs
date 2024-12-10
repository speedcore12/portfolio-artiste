namespace middlewares
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public static class SwaggerMiddleware
    {
        public static void AddSwaggerMiddleware(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "Portfolio API",
                    Version = "v1",
                    Description = "API pour gérer les fonctionnalités du portfolio."
                });
            });
        }

        public static void UseSwaggerMiddleware(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Portfolio API v1");
                options.RoutePrefix = string.Empty; // Swagger sera disponible à la racine
            });
        }
    }
}
