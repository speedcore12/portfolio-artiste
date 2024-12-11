namespace middlewares
{
    /// <summary>
    /// Middleware personnalisé pour gérer les exceptions non interceptées dans l'application.
    /// </summary>
    public static class ExceptionMiddleware
    {
        /// <summary>
        /// Ajoute un gestionnaire d'exceptions personnalisé à la pipeline d'exécution de l'application.
        /// Ce middleware capture les exceptions non gérées, les logge, et les ré-élève.
        /// </summary>
        /// <param name="app">L'objet <see cref="IApplicationBuilder"/> représentant la pipeline de traitement des requêtes.</param>
        public static void UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            // Ajout du middleware à la pipeline
            app.Use(async (context, next) =>
            {
                try
                {
                    // Exécution du middleware suivant
                    await next.Invoke();
                }
                catch (Exception ex)
                {
                    // Log de l'exception dans la console
                    Console.WriteLine($"Erreur non gérée : {ex.Message}");
                    
                    // Ré-élever l'exception pour une gestion ultérieure
                    throw;
                }
            });
        }
    }
}
