using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Default")
         ?? "Host=userdb;Database=user_db;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(cs));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c => c.RouteTemplate = "swagger/user/{documentName}/swagger.json");
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/user/v1/swagger.json", "User Service API");
        c.RoutePrefix = "swagger/user";
    });
}

// Простой эндпоинт для проверки
app.MapGet("/profile/{id}", async (Guid id, [FromServices] UserProfileRepository repo) =>
{
    var profile = await repo.GetProfileByIdAsync(id);
    return profile == null ? Results.NotFound() : Results.Ok(profile);
});

app.Run();