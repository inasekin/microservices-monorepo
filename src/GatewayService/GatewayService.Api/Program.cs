using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromMemory(
        new[]
        {
            new RouteConfig
            {
                RouteId = "auth_route",
                ClusterId = "auth_cluster",
                Match = new RouteMatch { Path = "/auth/{**catchall}" }
            },
            new RouteConfig
            {
                RouteId = "user_route",
                ClusterId = "user_cluster",
                Match = new RouteMatch { Path = "/user/{**catchall}" }
            },
            new RouteConfig
            {
                RouteId = "project_route",
                ClusterId = "project_cluster",
                Match = new RouteMatch { Path = "/project/{**catchall}" }
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
app.MapReverseProxy();
app.Run();