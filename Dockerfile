# ==================================
# ETAPA 1: BUILD (Compilación)
# ==================================
# Usa la imagen oficial de SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copia el archivo de la solución a la raíz
COPY GymManagement.sln .

# 2. Copia los archivos .csproj necesarios, manteniendo la estructura de carpetas
COPY Domain/*.csproj ./Domain/
COPY Application/*.csproj ./Application/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY Contract/*.csproj ./Contract/
COPY Api/*.csproj ./Api/

# 3. Ejecuta restore con el nombre de la solución
RUN dotnet restore GymManagement.sln

# 4. Copia el resto del código (los archivos .cs, etc.)
COPY . .

# Compila y publica SOLO el proyecto principal (Presentation.csproj), 
# que está en la carpeta 'Api'.
RUN dotnet publish "Api/Presentation.csproj" -c Release -o /app/publish

# ==================================
# ETAPA 2: RUNTIME (Ejecución)
# ==================================
# Usa la imagen oficial de ASP.NET Runtime (mucho más pequeña)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia los archivos publicados desde la etapa de build
COPY --from=build /app/publish .

# Define el puerto que la aplicación escuchará (importante para Render)
ENV ASPNETCORE_URLS=http://+:$PORT

# Comando para ejecutar la aplicación
# 🚨 AJUSTE DE RUTAS EN EL ENTRYPOINT
# Se agrega "Api/" a Presentation.csproj para reflejar la estructura del proyecto original.
ENTRYPOINT /bin/bash -c "dotnet ef database update --project Infrastructure --startup-project Api/Presentation.csproj && dotnet Presentation.dll"