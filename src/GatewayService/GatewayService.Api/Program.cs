using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:80");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("gateway", new OpenApiInfo
    {
        Title = "Gateway API",
        Version = "v1",
        Description = "Aggregated APIs via Ocelot Gateway"
    });
});

builder.Services.AddOcelot(builder.Configuration);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Auth Service API");
        c.SwaggerEndpoint("/user/swagger/v1/swagger.json", "User Service API");
        c.SwaggerEndpoint("/project/swagger/v1/swagger.json", "Project Service API");
        c.RoutePrefix = "swagger";
    });
}

await app.UseOcelot();

app.Run();