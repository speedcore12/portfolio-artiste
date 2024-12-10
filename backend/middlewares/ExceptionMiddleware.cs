namespace middlewares {
    public static class ExceptionMiddleware
    {
        public static void UseCustomExceptionHandler(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                try
                {
                    await next.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur non gérée : {ex.Message}");
                    throw;
                }
            });
        }
    }
}