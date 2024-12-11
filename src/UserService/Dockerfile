FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем csproj для UserService и BuildingBlocks
COPY ./src/UserService/UserService.Api/UserService.Api.csproj UserService.Api/
COPY ./src/UserService/UserService.Domain/UserService.Domain.csproj UserService.Domain/
COPY ./src/UserService/UserService.Infrastructure/UserService.Infrastructure.csproj UserService.Infrastructure/
COPY ./src/BuildingBlocks/CommonContracts/CommonContracts.csproj BuildingBlocks/CommonContracts/
COPY ./src/BuildingBlocks/EventBus/EventBus.csproj BuildingBlocks/EventBus/

RUN dotnet restore UserService.Api/UserService.Api.csproj

COPY ./src .

RUN dotnet build UserService.Api/UserService.Api.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/build .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "UserService.Api.dll"]
