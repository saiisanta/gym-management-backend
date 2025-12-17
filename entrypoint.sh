#!/bin/bash
# 1. Ejecutar la migración
dotnet ef database update --project Infrastructure --startup-project Api/Presentation.csproj

# 2. Iniciar la aplicación
dotnet Presentation.dll