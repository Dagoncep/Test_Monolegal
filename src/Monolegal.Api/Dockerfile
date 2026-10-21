# Se construye desde la raíz del repositorio: docker build -f src/Monolegal.Api/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restaurar primero solo con el .csproj para aprovechar la caché de capas
COPY src/Monolegal.Api/Monolegal.Api.csproj src/Monolegal.Api/
RUN dotnet restore src/Monolegal.Api/Monolegal.Api.csproj

COPY src/Monolegal.Api/ src/Monolegal.Api/
RUN dotnet publish src/Monolegal.Api/Monolegal.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Monolegal.Api.dll"]
