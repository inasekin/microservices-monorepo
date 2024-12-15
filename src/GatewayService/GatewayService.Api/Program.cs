using Swashbuckle.AspNetCore.SwaggerUI;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:80");

// Добавляем Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddReverseProxy()
    .LoadFromMemory(
        new[]
        {
            // Swagger JSON files
            new RouteConfig
            {
                RouteId = "auth_swagger_json",
                ClusterId = "auth_cluster",
                Match = new RouteMatch { Path = "/swagger/auth/v1/swagger.json" }
            },
            new RouteConfig
            {
                RouteId = "user_swagger_json",
                ClusterId = "user_cluster",
                Match = new RouteMatch { Path = "/swagger/user/v1/swagger.json" }
            },
            new RouteConfig
            {
                RouteId = "project_swagger_json",
                ClusterId = "project_cluster",
                Match = new RouteMatch { Path = "/swagger/project/v1/swagger.json" }
            },

            // Swagger UI
            new RouteConfig
            {
                RouteId = "auth_swagger_ui",
                ClusterId = "auth_cluster",
                Match = new RouteMatch { Path = "/swagger/auth/{**catchall}" }
            },
            new RouteConfig
            {
                RouteId = "user_swagger_ui",
                ClusterId = "user_cluster",
                Match = new RouteMatch { Path = "/swagger/user/{**catchall}" }
            },
            new RouteConfig
            {
                RouteId = "project_swagger_ui",
                ClusterId = "project_cluster",
                Match = new RouteMatch { Path = "/swagger/project/{**catchall}" }
            }
        },
        new[]
        {
            new ClusterConfig
            {
                ClusterId = "auth_cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["auth"] = new DestinationConfig { Address = "http://authservice:80" }
                }
            },
            new ClusterConfig
            {
                ClusterId = "user_cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["user"] = new DestinationConfig { Address = "http://userservice:80" }
                }
            },
            new ClusterConfig
            {
                ClusterId = "project_cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["project"] = new DestinationConfig { Address = "http://projectservice:80" }
                }
            }
        }
    );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/auth/v1/swagger.json", "Auth Service API");
        c.SwaggerEndpoint("/swagger/user/v1/swagger.json", "User Service API");
        c.SwaggerEndpoint("/swagger/project/v1/swagger.json", "Project Service API");
        c.RoutePrefix = "swagger";
    });
}

app.MapReverseProxy();
app.Run();
