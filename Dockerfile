# ==================================
# ETAPA 1: BUILD (Compilación)
# ==================================
# Usa la imagen oficial de SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el archivo de la solución (.sln) y todos los archivos de proyecto (.csproj) 
# para que 'dotnet restore' pueda resolver las dependencias entre proyectos.
COPY *.sln .
COPY **/*.csproj ./

RUN dotnet restore

# Copia todos los archivos restantes de la solución
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
# Usa el nombre de la DLL del proyecto de la API (Presentation.dll)
ENTRYPOINT /bin/bash -c "dotnet ef database update --project Infrastructure --startup-project Presentation.csproj && dotnet Presentation.dll"