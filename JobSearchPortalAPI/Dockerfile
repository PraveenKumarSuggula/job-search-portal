# Use official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JobSearchPortalAPI.csproj", "."]

RUN dotnet restore "./JobSearchPortalAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./JobSearchPortalAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish Stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JobSearchPortalAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final Production Stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render requires an explicit entry point
CMD ["dotnet", "JobSearchPortalAPI.dll"]
