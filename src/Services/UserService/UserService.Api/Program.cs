using CommonContracts.Dto;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Models;
using UserService.Infrastructure;
using EventBus;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Default")
         ?? "Host=userdb;Database=user_db;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ApplicationDbContext>(opts => opts.UseNpgsql(cs));
builder.Services.AddScoped<UserRepository>();

builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api/v1/user/swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/v1/user/swagger/v1/swagger.json", "User Service API");
        c.RoutePrefix = "api/v1/user/swagger";
    });
}

app.MapGet("/api/v1/user", async (UserRepository repo) =>
{
    var list = await repo.GetAllAsync();
    return list.Select(u => new UserDto
    {
        Id = u.Id,
        UserName = u.UserName
    });
});

app.MapGet("/api/v1/user/{id:guid}", async (UserRepository repo, Guid id) =>
{
    var user = await repo.GetAsync(id);
    return user == null
        ? Results.NotFound()
        : Results.Ok(new UserDto { Id = user.Id, UserName = user.UserName });
});

app.MapPost("/api/v1/user", async (UserRepository repo, IEventBus eventBus, UserDto dto) =>
{
    var user = new User { Id = Guid.NewGuid(), UserName = dto.UserName };
    await repo.AddAsync(user);

    var ev = new UserCreatedIntegrationEvent(user.Id, user.UserName);
    eventBus.Publish(ev);

    return Results.Created($"/api/v1/user/{user.Id}", new { user.Id });
});

app.MapPut("/api/v1/user/{id:guid}", async (UserRepository repo, Guid id, UserDto dto) =>
{
    var user = await repo.GetAsync(id);
    if (user == null) return Results.NotFound();

    user.UserName = dto.UserName;
    await repo.UpdateAsync(user);
    return Results.NoContent();
});

app.MapDelete("/api/v1/user/{id:guid}", async (UserRepository repo, Guid id) =>
{
    await repo.DeleteAsync(id);
    return Results.NoContent();
});

app.Run();

public class UserCreatedIntegrationEvent(Guid UserId, string UserName) : IntegrationEvent
{
    public Guid UserId { get; init; } = UserId;
    public string UserName { get; init; } = UserName;

    public void Deconstruct(out Guid UserId, out string UserName)
    {
        UserId = this.UserId;
        UserName = this.UserName;
    }
}
