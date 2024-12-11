FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем все .csproj файлы для AuthService и BuildingBlocks
COPY ./src/AuthService/AuthService.Api/AuthService.Api.csproj AuthService.Api/
COPY ./src/AuthService/AuthService.Domain/AuthService.Domain.csproj AuthService.Domain/
COPY ./src/AuthService/AuthService.Infrastructure/AuthService.Infrastructure.csproj AuthService.Infrastructure/
COPY ./src/BuildingBlocks/CommonContracts/CommonContracts.csproj BuildingBlocks/CommonContracts/
COPY ./src/BuildingBlocks/EventBus/EventBus.csproj BuildingBlocks/EventBus/

# Восстанавливаем зависимости
RUN dotnet restore AuthService.Api/AuthService.Api.csproj

# Копируем весь код
COPY ./src .

RUN dotnet build AuthService.Api/AuthService.Api.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "AuthService.Api.dll"]
