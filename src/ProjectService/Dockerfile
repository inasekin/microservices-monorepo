FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj для ProjectService и BuildingBlocks
COPY ./src/ProjectService/ProjectService.Api/ProjectService.Api.csproj ProjectService.Api/
COPY ./src/ProjectService/ProjectService.Domain/ProjectService.Domain.csproj ProjectService.Domain/
COPY ./src/ProjectService/ProjectService.Infrastructure/ProjectService.Infrastructure.csproj ProjectService.Infrastructure/
COPY ./src/BuildingBlocks/CommonContracts/CommonContracts.csproj BuildingBlocks/CommonContracts/
COPY ./src/BuildingBlocks/EventBus/EventBus.csproj BuildingBlocks/EventBus/

RUN dotnet restore ProjectService.Api/ProjectService.Api.csproj

COPY ./src .

RUN dotnet build ProjectService.Api/ProjectService.Api.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "ProjectService.Api.dll"]
