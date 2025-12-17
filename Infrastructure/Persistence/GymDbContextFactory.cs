using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql.EntityFrameworkCore.PostgreSQL;

// Asegúrate de que este namespace coincida con el de GymDbContext
namespace Infrastructure.Persistence
{
    // Esta clase instruye a la herramienta 'dotnet ef' cómo construir el DbContext
    // cuando no puede acceder a la configuración de la aplicación web (Program.cs).
    public class GymDbContextFactory : IDesignTimeDbContextFactory<GymDbContext>
    {
        public GymDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GymDbContext>();

            // Usamos una cadena de conexión ficticia para que el comando de migración pueda funcionar.
            // Esto NO afectará tu conexión en producción.
            const string dummyConnectionString =
                "Host=localhost;Database=MigrationDesignTimeDB;Username=user;Password=password";

            // Le decimos a la herramienta que use el proveedor PostgreSQL (Npgsql)
            optionsBuilder.UseNpgsql(dummyConnectionString);

            return new GymDbContext(optionsBuilder.Options);
        }
    }
}
