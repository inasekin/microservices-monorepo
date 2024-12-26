using CommonContracts.Dto;
using Microsoft.EntityFrameworkCore;
using ProjectService.Domain.Models;
using ProjectService.Infrastructure;
using EventBus;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Default")
         ?? "Host=projectdb;Database=project_db;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ApplicationDbContext>(opts => opts.UseNpgsql(cs));
builder.Services.AddScoped<ProjectRepository>();

builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();

builder.Services.AddSingleton<IIntegrationEventHandler<UserCreatedIntegrationEvent>, UserCreatedIntegrationEventHandler>();

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
        c.RouteTemplate = "api/v1/project/swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/v1/project/swagger/v1/swagger.json", "Project Service API");
        c.RoutePrefix = "api/v1/project/swagger";
    });
}

app.Lifetime.ApplicationStarted.Register(() =>
{
    var eventBus = app.Services.GetRequiredService<IEventBus>();
    eventBus.Subscribe<UserCreatedIntegrationEvent, UserCreatedIntegrationEventHandler>();
});

app.MapGet("/api/v1/project", async (ProjectRepository repo) =>
{
    var list = await repo.GetAllAsync();
    return list.Select(p => new ProjectDto { Id = p.Id, Title = p.Title, OwnerId = p.OwnerId });
});

app.MapGet("/api/v1/project/{id:guid}", async (ProjectRepository repo, Guid id) =>
{
    var proj = await repo.GetAsync(id);
    return proj == null
        ? Results.NotFound()
        : Results.Ok(new ProjectDto { Id = proj.Id, Title = proj.Title, OwnerId = proj.OwnerId });
});

app.MapPost("/api/v1/project", async (ProjectRepository repo, ProjectDto dto) =>
{
    var project = new Project
    {
        Id = Guid.NewGuid(),
        Title = dto.Title,
        OwnerId = dto.OwnerId
    };
    await repo.AddAsync(project);
    
    return Results.Created($"/api/v1/project/{project.Id}", new { project.Id });
});

app.MapPut("/api/v1/project/{id:guid}", async (ProjectRepository repo, Guid id, ProjectDto dto) =>
{
    var proj = await repo.GetAsync(id);
    if (proj == null) return Results.NotFound();

    proj.Title = dto.Title;
    proj.OwnerId = dto.OwnerId;
    await repo.UpdateAsync(proj);
    return Results.NoContent();
});

app.MapDelete("/api/v1/project/{id:guid}", async (ProjectRepository repo, Guid id) =>
{
    await repo.DeleteAsync(id);
    return Results.NoContent();
});

app.MapGet("/api/v1/project/check-owner/{projectId}", async (ProjectRepository repo, Guid projectId) =>
{
    var proj = await repo.GetAsync(projectId);
    if (proj == null) return Results.NotFound();

    // Синхронный вызов UserService, если нужно
    using var client = new HttpClient { BaseAddress = new Uri("http://userservice") };
    var resp = await client.GetAsync($"/api/v1/user/{proj.OwnerId}");
    if (!resp.IsSuccessStatusCode)
        return Results.Ok($"Owner not found. StatusCode: {resp.StatusCode}");
    
    var userDto = await resp.Content.ReadFromJsonAsync<UserDto>();
    return Results.Ok(new { project = proj.Title, owner = userDto?.UserName });
});

app.Run();

public class UserCreatedIntegrationEventHandler : IIntegrationEventHandler<UserCreatedIntegrationEvent>
{
    public Task Handle(UserCreatedIntegrationEvent @event)
    {
        Console.WriteLine($"[ProjectService] Received UserCreatedIntegrationEvent: UserId={@event.UserId}, UserName={@event.UserName}");

        return Task.CompletedTask;
    }
}

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
