using ProjectService.Domain.Models;
using ProjectService.Infrastructure;
using CommonContracts;
using Microsoft.AspNetCore.Mvc;
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
    app.UseSwagger(c => c.RouteTemplate = "swagger/project/{documentName}/swagger.json");
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/project/v1/swagger.json", "Project Service API");
        c.RoutePrefix = "swagger/project";
    });
}

app.MapGet("/project/{id}", async (Guid id, [FromServices] ProjectRepository repo) =>
{
    var proj = await repo.GetProjectByIdAsync(id);
    return proj == null ? Results.NotFound() : Results.Ok(proj);
});

app.Run();