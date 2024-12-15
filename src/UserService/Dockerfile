FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./src/BuildingBlocks/CommonContracts/CommonContracts.csproj ./BuildingBlocks/CommonContracts/
COPY ./src/BuildingBlocks/EventBus/EventBus.csproj ./BuildingBlocks/EventBus/
COPY ./src/UserService/UserService.Api/UserService.Api.csproj ./UserService/UserService.Api/
COPY ./src/UserService/UserService.Domain/UserService.Domain.csproj ./UserService/UserService.Domain/
COPY ./src/UserService/UserService.Infrastructure/UserService.Infrastructure.csproj ./UserService/UserService.Infrastructure/

RUN dotnet restore ./UserService/UserService.Api/UserService.Api.csproj

COPY ./src .
RUN dotnet build ./UserService/UserService.Api/UserService.Api.csproj -c Release -o /app/build

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "UserService.Api.dll"]
