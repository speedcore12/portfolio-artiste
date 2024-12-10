using Microsoft.EntityFrameworkCore;
using middlewares; 

var builder = WebApplication.CreateBuilder(args);

// Ajouter ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
