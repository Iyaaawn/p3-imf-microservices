# ---- Stage 1: build ----
# The SDK image has the full .NET toolset (big, ~800 MB); it's used only to compile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy just the project file first and restore NuGet packages.
# Docker caches this step, so packages only re-download when the .csproj changes.
COPY *.csproj ./
RUN dotnet restore

# Now copy the rest of the code and publish a Release build
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# ---- Stage 2: run ----
# The ASP.NET image has only the runtime (much smaller); this is what actually ships
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

# curl lets Docker Compose call /health to check the container is healthy
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "RequestsApi.dll"]