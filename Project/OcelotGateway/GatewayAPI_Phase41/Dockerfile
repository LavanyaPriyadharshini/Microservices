# ── STAGE 1: BUILD ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy project file first for layer caching
COPY GatewayAPI_Phase4.csproj .

# Restore NuGet packages including Ocelot
RUN dotnet restore GatewayAPI_Phase4.csproj

# Copy all source code including ocelot.json and ocelot.Docker.json
COPY . .

# Publish in Release mode
RUN dotnet publish GatewayAPI_Phase4.csproj -c Release -o /app/publish

# ── STAGE 2: RUNTIME ────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "GatewayAPI_Phase4.dll"]