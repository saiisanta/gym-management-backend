// Infrastructure/Persistence/GymDbContext.cs

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options)
            : base(options) { }

        // Definiciones de DbSet
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Alumno> Alumnos { get; set; }
        public DbSet<Plan> Planes { get; set; }
        public DbSet<Membresia> Membresias { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Clase> Clases { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuracion de TPH (Table Per Hierarchy) para herencia
            modelBuilder
                .Entity<Usuario>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Usuario>("Usuario")
                .HasValue<Alumno>("Alumno")
                .HasValue<Profesor>("Profesor");

            // *** IMPORTANTE: Se han ELIMINADO todas las llamadas a .HasData() ***
            // La inserción de datos iniciales (Seed Data) se hará mediante el script SQL externo (import_data_final.sql)
            // Esto evita el error de System.InvalidCastException al generar el script de migración.
        }
    }
}