# Imagen base de producción
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Imagen para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TFG_Cultivos.csproj", "."]
RUN dotnet restore "./TFG_Cultivos.csproj"
COPY . .
RUN dotnet build "./TFG_Cultivos.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "./TFG_Cultivos.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TFG_Cultivos.dll"]
