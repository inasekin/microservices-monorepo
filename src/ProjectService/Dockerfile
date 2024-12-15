FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./src/BuildingBlocks/CommonContracts/CommonContracts.csproj ./BuildingBlocks/CommonContracts/
COPY ./src/BuildingBlocks/EventBus/EventBus.csproj ./BuildingBlocks/EventBus/
COPY ./src/ProjectService/ProjectService.Api/ProjectService.Api.csproj ./ProjectService/ProjectService.Api/
COPY ./src/ProjectService/ProjectService.Domain/ProjectService.Domain.csproj ./ProjectService/ProjectService.Domain/
COPY ./src/ProjectService/ProjectService.Infrastructure/ProjectService.Infrastructure.csproj ./ProjectService/ProjectService.Infrastructure/

RUN dotnet restore ./ProjectService/ProjectService.Api/ProjectService.Api.csproj

COPY ./src .
RUN dotnet build ./ProjectService/ProjectService.Api/ProjectService.Api.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "ProjectService.Api.dll"]
