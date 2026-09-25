# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY global.json Directory.Build.props Directory.Packages.props Korner.sln ./
COPY src/Korner.Api/Korner.Api.csproj src/Korner.Api/
RUN dotnet restore src/Korner.Api/Korner.Api.csproj

COPY src/Korner.Api/ src/Korner.Api/
RUN dotnet publish src/Korner.Api/Korner.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# curl is needed for the compose healthcheck; the base image ships neither curl nor wget.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app .

USER $APP_UID
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Korner.Api.dll"]
