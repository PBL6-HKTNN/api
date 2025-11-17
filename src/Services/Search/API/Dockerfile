# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Search/API/Codemy.Search.API.csproj", "src/Services/Search/API/"]
COPY ["src/BuildingBlocks/Codemy.BuildingBlocks.EventBus/Codemy.BuildingBlocks.EventBus.csproj", "src/BuildingBlocks/Codemy.BuildingBlocks.EventBus/"]
COPY ["src/Services/Search/Application/Codemy.Search.Application.csproj", "src/Services/Search/Application/"]
COPY ["src/Services/Search/Infrastructure/Codemy.Search.Infrastructure.csproj", "src/Services/Search/Infrastructure/"]
COPY ["src/BuildingBlocks/Codemy.BuildingBlocks.Infrastructure/Codemy.BuildingBlocks.Infrastructure.csproj", "src/BuildingBlocks/Codemy.BuildingBlocks.Infrastructure/"]
COPY ["src/BuildingBlocks/Codemy.BuildingBlocks.Domain/Codemy.BuildingBlocks.Domain.csproj", "src/BuildingBlocks/Codemy.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/Core/Codemy.BuildingBlocks.Core.csproj", "src/BuildingBlocks/Core/"]
RUN dotnet restore "./src/Services/Search/API/Codemy.Search.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Search/API"
RUN dotnet build "./Codemy.Search.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Codemy.Search.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Codemy.Search.API.dll"]