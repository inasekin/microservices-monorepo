using ProjectService.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Default")
         ?? "Host=projectdb;Database=project_db;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(cs));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/project/{id}", async (Guid id, ProjectRepository repo) =>
{
    var proj = await repo.GetProjectByIdAsync(id);
    return proj == null ? Results.NotFound() : Results.Ok(proj);
});

app.Run();