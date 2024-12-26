FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./src/BuildingBlocks/CommonContracts/CommonContracts.csproj ./BuildingBlocks/CommonContracts/
COPY ./src/BuildingBlocks/EventBus/EventBus.csproj ./BuildingBlocks/EventBus/
COPY ./src/Services/AuthService/AuthService.Api/AuthService.Api.csproj ./Services/AuthService/AuthService.Api/
COPY ./src/Services/AuthService/AuthService.Domain/AuthService.Domain.csproj ./Services/AuthService/AuthService.Domain/
COPY ./src/Services/AuthService/AuthService.Infrastructure/AuthService.Infrastructure.csproj ./Services/AuthService/AuthService.Infrastructure/

RUN dotnet restore ./Services/AuthService/AuthService.Api/AuthService.Api.csproj

COPY ./src .
RUN dotnet build ./Services/AuthService/AuthService.Api/AuthService.Api.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "AuthService.Api.dll"]
