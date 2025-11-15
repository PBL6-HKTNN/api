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
COPY ["src/Services/Notification/API/Codemy.Notification.API.csproj", "src/Services/Notification/API/"]
COPY ["src/BuildingBlocks/Core/Codemy.BuildingBlocks.Core.csproj", "src/BuildingBlocks/Core/"]
COPY ["src/Services/Notification/Application/Codemy.Notification.Application.csproj", "src/Services/Notification/Application/"]
COPY ["src/Services/Notification/Infrastructure/Codemy.Notification.Infrastructure.csproj", "src/Services/Notification/Infrastructure/"]
RUN dotnet restore "./src/Services/Notification/API/Codemy.Notification.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Notification/API"
RUN dotnet build "./Codemy.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Codemy.Notification.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Codemy.Notification.API.dll"]