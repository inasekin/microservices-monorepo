using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:80");

var microservices = new[]
{
    new {
        Name = "UserService",
        Prefix = "user",
        Host = "userservice",
        Port = 80,
        SwaggerUrl = "http://userservice/api/v1/user/swagger/v1/swagger.json"
    },
    new {
        Name = "AuthService",
        Prefix = "auth",
        Host = "authservice",
        Port = 80,
        SwaggerUrl = "http://authservice/api/v1/auth/swagger/v1/swagger.json"
    },
    new {
        Name = "ProjectService",
        Prefix = "project",
        Host = "projectservice",
        Port = 80,
        SwaggerUrl = "http://projectservice/api/v1/project/swagger/v1/swagger.json"
    }
};

var routes = new List<object>();

foreach (var svc in microservices)
{
    // 1) CRUD: /api/v1/<prefix>/{everything}
    routes.Add(new {
        Priority = 1,
        UpstreamPathTemplate = $"/api/v1/{svc.Prefix}/{{everything}}",
        UpstreamHttpMethod = new[] { "Get","Post","Put","Delete","Patch","Options" },
        DownstreamPathTemplate = $"/api/v1/{svc.Prefix}/{{everything}}",
        DownstreamScheme = "http",
        DownstreamHostAndPorts = new[] {
            new { Host = svc.Host, Port = svc.Port }
        }
    });

    // 2) Swagger JSON
    routes.Add(new {
        Priority = 90,
        UpstreamPathTemplate = $"/api/v1/{svc.Prefix}/swagger/v1/swagger.json",
        UpstreamHttpMethod = new[] { "Get" },
        DownstreamPathTemplate = $"/api/v1/{svc.Prefix}/swagger/v1/swagger.json",
        DownstreamScheme = "http",
        DownstreamHostAndPorts = new[] {
            new { Host = svc.Host, Port = svc.Port }
        }
    });

    // 3) Swagger static
    routes.Add(new {
        Priority = 100,
        UpstreamPathTemplate = $"/api/v1/{svc.Prefix}/swagger/{{everything}}",
        UpstreamHttpMethod = new[] { "Get" },
        DownstreamPathTemplate = $"/api/v1/{svc.Prefix}/swagger/{{everything}}",
        DownstreamScheme = "http",
        DownstreamHostAndPorts = new[] {
            new { Host = svc.Host, Port = svc.Port }
        }
    });
}

var ocelotConfig = new {
    Routes = routes,
    GlobalConfiguration = new {
        BaseUrl = "http://localhost:5010"
    }
};

var jsonString = JsonConvert.SerializeObject(ocelotConfig, Formatting.Indented);
Console.WriteLine("=== FINAL ROUTES CONFIG ===");
Console.WriteLine(jsonString);

builder.Configuration.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(jsonString)));
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Gateway Single Swagger",
        Version = "v1"
    });
    opt.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = "swagger"; // => /swagger/index.html

        // Подключаем swagger.json каждого сервиса
        foreach (var svc in microservices)
        {
            // Upstream - /api/v1/<prefix>/swagger/v1/swagger.json
            c.SwaggerEndpoint(
                $"/api/v1/{svc.Prefix}/swagger/v1/swagger.json",
                svc.Name
            );
        }
    });
}

await app.UseOcelot();

app.Run();
