#!/bin/sh

# Pausar para darle tiempo a la base de datos de Render para inicializarse (30 segundos)
sleep 30

# Ejecutar la migración forzada para asegurar que las tablas existan (debería funcionar ahora que el servicio está estable)
dotnet ef database update --project Infrastructure --startup-project Api/Presentation.csproj

# Iniciar la aplicación
dotnet Presentation.dll