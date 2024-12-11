using AuthService.Domain.Models;
using AuthService.Infrastructure;
using CommonContracts;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var cs = builder.Configuration.GetConnectionString("Default")
                 ?? "Host=authdb;Database=auth_db;Username=postgres;Password=postgres";

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(cs));
        builder.Services.AddScoped<UserRepository>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapPost("/user", async (UserDto dto, UserRepository repo) =>
        {
            var user = new User { Id = Guid.NewGuid(), UserName = dto.UserName };
            await repo.AddUserAsync(user);
            return Results.Created($"/user/{user.Id}", user.Id);
        });

        app.MapGet("/user/{id:guid}", async (Guid id, UserRepository repo) =>
        {
            var user = await repo.GetUserByIdAsync(id);
            return user == null
                ? Results.NotFound()
                : Results.Ok(new UserDto { Id = user.Id, UserName = user.UserName });
        });

        app.Run();
    }
}