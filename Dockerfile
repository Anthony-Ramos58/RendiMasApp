# Usa la imagen oficial de .NET SDK para compilar la app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo de solución y los proyectos
COPY ["Antohny/Antohny.csproj", "Antohny/"]
RUN dotnet restore "Antohny/Antohny.csproj"

COPY . .
WORKDIR "/src/Antohny"
RUN dotnet publish "Antohny.csproj" -c Release -o /app/publish

# Imagen de runtime para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Antohny.dll"]
