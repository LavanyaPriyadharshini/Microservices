FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ProductAPI_Phase1.csproj .

RUN dotnet restore ProductAPI_Phase1.csproj

COPY . .

RUN dotnet publish ProductAPI_Phase1.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ProductAPI_Phase1.dll"]