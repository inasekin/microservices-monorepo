FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./src/BuildingBlocks/CommonContracts/CommonContracts.csproj ./BuildingBlocks/CommonContracts/
COPY ./src/BuildingBlocks/EventBus/EventBus.csproj ./BuildingBlocks/EventBus/
COPY ./src/Services/ProjectService/ProjectService.Api/ProjectService.Api.csproj ./Services/ProjectService/ProjectService.Api/
COPY ./src/Services/ProjectService/ProjectService.Domain/ProjectService.Domain.csproj ./Services/ProjectService/ProjectService.Domain/
COPY ./src/Services/ProjectService/ProjectService.Infrastructure/ProjectService.Infrastructure.csproj ./Services/ProjectService/ProjectService.Infrastructure/

RUN dotnet restore ./Services/ProjectService/ProjectService.Api/ProjectService.Api.csproj

COPY ./src .
RUN dotnet build ./Services/ProjectService/ProjectService.Api/ProjectService.Api.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "ProjectService.Api.dll"]
