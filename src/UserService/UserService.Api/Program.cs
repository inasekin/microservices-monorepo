using UserService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using CommonContracts;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Default")
         ?? "Host=userdb;Database=user_db;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(cs));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Простой эндпоинт для проверки
app.MapGet("/profile/{id}", async (Guid id, UserProfileRepository repo) =>
{
    var profile = await repo.GetProfileByIdAsync(id);
    return profile == null ? Results.NotFound() : Results.Ok(profile);
});

app.Run();