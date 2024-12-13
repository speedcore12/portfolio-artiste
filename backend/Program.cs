using Microsoft.EntityFrameworkCore;
using middlewares;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Charger les variables d'environnement
try
{
    // Charge les variables d'environnement à partir du fichier .env
    DotNetEnv.Env.Load();
}
catch (Exception ex)
{
    Console.WriteLine($"Erreur lors du chargement des variables d'environnement : {ex.Message}");
}

// Récupérer la chaîne de connexion depuis les variables d'environnement
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") 
                       ?? throw new InvalidOperationException("La chaîne de connexion est manquante.");

// Ajouter ApplicationDbContext avec la chaîne de connexion
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Ajouter les middlewares nécessaires
builder.Services.AddSwaggerMiddleware();
builder.Services.AddCorsPolicy();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerMiddleware();
}

// Activer les middlewares
app.UseCorsPolicy();
app.UseCustomExceptionHandler();
app.MapRoutes();

app.Run();
