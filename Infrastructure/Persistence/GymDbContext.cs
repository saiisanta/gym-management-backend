using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options)
            : base(options) { }

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

            modelBuilder
                .Entity<Usuario>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<Usuario>("Usuario")
                .HasValue<Alumno>("Alumno")
                .HasValue<Profesor>("Profesor");

            var adminPassword = "Admin123!";
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(
                    System.Text.Encoding.UTF8.GetBytes(adminPassword)
                );
                var adminHash = System.Convert.ToBase64String(hashedBytes);

                modelBuilder
                    .Entity<Usuario>()
                    .HasData(
                        new
                        {
                            Id = -9999,
                            Nombre = "Admin",
                            Apellido = "User",
                            Dni = "00000000",
                            Email = "admin@gym.com",
                            Telefono = "",
                            FechaNacimiento = DateOnly.FromDateTime(new DateTime(1990, 1, 1)),
                            Activo = true,
                            PasswordHash = adminHash,
                            Role = "Administrador",
                            FailedLoginAttempts = 0,
                        }
                    );
            }

            modelBuilder
                .Entity<Sucursal>()
                .HasData(
                    new Sucursal
                    {
                        Id = 1,
                        Nombre = "Sucursal Centro",
                        Direccion = "Av. Principal 123",
                        Telefono = "555-0001",
                        Email = "centro@gym.com",
                        Activa = true,
                    },
                    new Sucursal
                    {
                        Id = 2,
                        Nombre = "Sucursal Norte",
                        Direccion = "Calle Norte 456",
                        Telefono = "555-0002",
                        Email = "norte@gym.com",
                        Activa = true,
                    }
                );

            modelBuilder
                .Entity<Sala>()
                .HasData(
                    new
                    {
                        Id = 1,
                        Nombre = "Sala A",
                        Tipo = "Multiuso",
                        Capacidad = 25,
                        Descripcion = "Sala multiuso para yoga, pilates y actividades grupales",
                        SucursalId = 1,
                        Activa = true,
                    },
                    new
                    {
                        Id = 2,
                        Nombre = "Sala B",
                        Tipo = "Spinning",
                        Capacidad = 20,
                        Descripcion = "Sala equipada con 20 bicicletas estáticas profesionales",
                        SucursalId = 1,
                        Activa = true,
                    },
                    new
                    {
                        Id = 3,
                        Nombre = "Sala 1",
                        Tipo = "Funcional",
                        Capacidad = 30,
                        Descripcion = "Espacio amplio para entrenamiento funcional y crossfit",
                        SucursalId = 2,
                        Activa = true,
                    },
                    new
                    {
                        Id = 4,
                        Nombre = "Sala 2",
                        Tipo = "Pesas",
                        Capacidad = 40,
                        Descripcion = "Sala de musculación con equipamiento completo",
                        SucursalId = 2,
                        Activa = true,
                    }
                );

            // --- Planes (ya definidos) ---
            modelBuilder
                .Entity<Plan>()
                .HasData(
                    new
                    {
                        Id = 1,
                        Nombre = "Básico",
                        Descripcion = "Acceso a clases generales y recursos introductorios. Ideal para quienes recién comienzan.",
                        Precio = 25000m,
                        DuracionDias = 30,
                        MaxReservasPorMes = (int?)8,
                        TiposPermitidos = "[\"general\"]",
                        Activo = true,
                    },
                    new
                    {
                        Id = 2,
                        Nombre = "Avanzado",
                        Descripcion = "Incluye acceso a clases especializadas, soporte prioritario y material exclusivo.",
                        Precio = 40000m,
                        DuracionDias = 30,
                        MaxReservasPorMes = (int?)20,
                        TiposPermitidos = "[]",
                        Activo = true,
                    },
                    new
                    {
                        Id = 3,
                        Nombre = "Premium",
                        Descripcion = "Acceso ilimitado a todos los cursos, mentorías personalizadas y beneficios exclusivos.",
                        Precio = 55000m,
                        DuracionDias = 30,
                        MaxReservasPorMes = (int?)null,
                        TiposPermitidos = "[]",
                        Activo = true,
                    }
                );

            // --- Sucursal 3 (nuevo) ---
            modelBuilder
                .Entity<Sucursal>()
                .HasData(
                    new Sucursal
                    {
                        Id = 3,
                        Nombre = "Sucursal Sur",
                        Direccion = "Av. Sur 789",
                        Telefono = "555-0003",
                        Email = "sur@gym.com",
                        Activa = true,
                    }
                );

            // --- Salas faltantes (5,6,7) ---
            modelBuilder
                .Entity<Sala>()
                .HasData(
                    new
                    {
                        Id = 5,
                        Nombre = "Sala 5",
                        Tipo = "Cardio",
                        Capacidad = 22,
                        Descripcion = "Sala de cardio con equipamiento variado",
                        SucursalId = 2,
                        Activa = true,
                    },
                    new
                    {
                        Id = 6,
                        Nombre = "Sala 6",
                        Tipo = "Baile",
                        Capacidad = 30,
                        Descripcion = "Espacio amplio para clases de baile y zumba",
                        SucursalId = 3,
                        Activa = true,
                    },
                    new
                    {
                        Id = 7,
                        Nombre = "Sala 7",
                        Tipo = "Estudio",
                        Capacidad = 18,
                        Descripcion = "Estudio pequeño para sesiones especializadas",
                        SucursalId = 1,
                        Activa = true,
                    }
                );

            // --- Profesores (Usuarios con Discriminator = "Profesor") ---
            var profesorPassword = "Prof123!";
            string profesorHash;
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(
                    System.Text.Encoding.UTF8.GetBytes(profesorPassword)
                );
                profesorHash = System.Convert.ToBase64String(hashedBytes);
            }

            modelBuilder
                .Entity<Usuario>()
                .HasData(
                    new
                    {
                        Id = 2,
                        Nombre = "María",
                        Apellido = "González",
                        Dni = "11111111",
                        Email = "maria.profe@gym.com",
                        Telefono = "555-1002",
                        FechaNacimiento = DateOnly.FromDateTime(new DateTime(1985, 6, 12)),
                        Activo = true,
                        PasswordHash = profesorHash,
                        Role = "Profesor",
                        Discriminator = "Profesor",
                        FailedLoginAttempts = 0,
                    },
                    new
                    {
                        Id = 3,
                        Nombre = "Carlos",
                        Apellido = "Pérez",
                        Dni = "22222222",
                        Email = "carlos.profe@gym.com",
                        Telefono = "555-1003",
                        FechaNacimiento = DateOnly.FromDateTime(new DateTime(1980, 3, 2)),
                        Activo = true,
                        PasswordHash = profesorHash,
                        Role = "Profesor",
                        Discriminator = "Profesor",
                        FailedLoginAttempts = 0,
                    },
                    new
                    {
                        Id = 4,
                        Nombre = "Lucía",
                        Apellido = "Ramírez",
                        Dni = "33333333",
                        Email = "lucia.profe@gym.com",
                        Telefono = "555-1004",
                        FechaNacimiento = DateOnly.FromDateTime(new DateTime(1990, 11, 5)),
                        Activo = true,
                        PasswordHash = profesorHash,
                        Role = "Profesor",
                        Discriminator = "Profesor",
                        FailedLoginAttempts = 0,
                    },
                    new
                    {
                        Id = 5,
                        Nombre = "Martín",
                        Apellido = "Suárez",
                        Dni = "44444444",
                        Email = "martin.profe@gym.com",
                        Telefono = "555-1005",
                        FechaNacimiento = DateOnly.FromDateTime(new DateTime(1988, 8, 20)),
                        Activo = true,
                        PasswordHash = profesorHash,
                        Role = "Profesor",
                        Discriminator = "Profesor",
                        FailedLoginAttempts = 0,
                    },
                    new
                    {
                        Id = 6,
                        Nombre = "Sofía",
                        Apellido = "López",
                        Dni = "55555555",
                        Email = "sofia.profe@gym.com",
                        Telefono = "555-1006",
                        FechaNacimiento = DateOnly.FromDateTime(new DateTime(1992, 2, 14)),
                        Activo = true,
                        PasswordHash = profesorHash,
                        Role = "Profesor",
                        Discriminator = "Profesor",
                        FailedLoginAttempts = 0,
                    },
                    new
                    {
                        Id = 7,
                        Nombre = "Diego",
                        Apellido = "Fernández",
                        Dni = "66666666",
                        Email = "diego.profe@gym.com",
                        Telefono = "555-1007",
                        FechaNacimiento = DateOnly.FromDateTime(new DateTime(1983, 9, 30)),
                        Activo = true,
                        PasswordHash = profesorHash,
                        Role = "Profesor",
                        Discriminator = "Profesor",
                        FailedLoginAttempts = 0,
                    }
                );

            // --- Usuarios seed (clientes / admins que vienen del fake-backend) ---
            // Contraseñas hasheadas con PBKDF2 (salt 16 bytes + 10000 iteraciones, SHA256) para que AuthService.VerifyPassword funcione.
            modelBuilder
                .Entity<Usuario>()
                .HasData(
                    new
                    {
                        Id = 1,
                        Nombre = "Simón",
                        Apellido = "Santarelli",
                        Dni = "1",
                        Email = "simon@example.com",
                        Telefono = "3411234567",
                        FechaNacimiento = DateOnly.FromDateTime(new DateTime(2005, 3, 12)),
                        Activo = true,
                        // PasswordHash generado con PBKDF2 (salt + hash), base64
                        PasswordHash = "HfLlcoG+fBtUcKKRzzb3sTcc0Kk1P0qAGDSXV656AFKkzPJ5tGWmtiXdOVLmMjT7",
                        Role = "SuperAdministrador",
                        FailedLoginAttempts = 0,
                        Image = "https://placehold.co/120x120?text=User",
                        SucursalId = 1,
                        Discriminator = "Usuario",
                    },
                    new
                    {
                        Id = 8,
                        Nombre = "Francisco",
                        Apellido = "Cumini",
                        Dni = "39222001",
                        Email = "fran@highfit.com",
                        Telefono = "3412345678",
                        FechaNacimiento = DateOnly.FromDateTime(new DateTime(1995, 5, 20)),
                        Activo = true,
                        PasswordHash = "+yFOjDzWtkAR9lxanKPZsbY3Na3Kv5ZrRBOpG0nJfBt7YFf/LJurnWJrskA6QWT8",
                        Role = "Administrador",
                        FailedLoginAttempts = 0,
                        Image = "https://placehold.co/120x120?text=User",
                        SucursalId = 1,
                        Discriminator = "Usuario",
                    },
                    new
                    {
                        Id = 9,
                        Nombre = "Pablo",
                        Apellido = "Cardillo",
                        Dni = "39555111",
                        Email = "pablo@highfit.com",
                        Telefono = "3413456789",
                        FechaNacimiento = DateOnly.FromDateTime(new DateTime(1996, 10, 3)),
                        Activo = true,
                        PasswordHash = "hz5yIy4nK3ZmVcTPJki6YH0sTq3debyK804lrYE+bqI0Fgz7i/PXysORUKzjBtGH",
                        Role = "Recepcionista",
                        FailedLoginAttempts = 0,
                        Image = "https://placehold.co/120x120?text=User",
                        SucursalId = 1,
                        Discriminator = "Usuario",
                    },
                    new
                    {
                        Id = 10,
                        Nombre = "Cliente Demo",
                        Apellido = "Perez",
                        Dni = "40111222",
                        Email = "cliente@demo.com",
                        Telefono = "+5493415555555",
                        FechaNacimiento = DateOnly.FromDateTime(new DateTime(1997, 4, 1)),
                        Activo = true,
                        PasswordHash = "FNnfcvojVZDEjAzKc+6S3aD0QOHlc0vd6lIRLQ7Nt1pYxgP/TtcZvobB9F/gp+vO",
                        Role = "Alumno",
                        FailedLoginAttempts = 0,
                        Image = "https://placehold.co/120x120?text=User",
                        SucursalId = 1,
                        Discriminator = "Alumno",
                    }
                );

            // --- Clases (Ids 1..7) ---
            modelBuilder
                .Entity<Clase>()
                .HasData(
                    new
                    {
                        Id = 1,
                        ProfesorId = 2,
                        SalaId = 1,
                        SucursalId = 1,
                        Nombre = "Yoga Matinal",
                        Descripcion = "Clase suave para activar el cuerpo y la mente. Ideal para empezar el día con energía.",
                        Imagen = "https://images.unsplash.com/photo-1506126613408-eca07ce68773?auto=format&fit=crop&w=800&q=80",
                        Tipo = "general",
                        DuracionMinutos = 60,
                        HoraInicio = new TimeOnly(0, 0),
                        Fecha = DateOnly.FromDateTime(new DateTime(2025, 10, 11)),
                        Dias = "[\"Lunes\",\"Miércoles\",\"Viernes\"]",
                        Capacidad = 15,
                        MostrarEnHome = true,
                        Activa = true,
                    },
                    new
                    {
                        Id = 2,
                        ProfesorId = 2,
                        SalaId = 2,
                        SucursalId = 2,
                        Nombre = "Spin Intenso",
                        Descripcion = "Entrenamiento cardiovascular sobre bicicletas fijas, con música y mucha energía.",
                        Imagen = "https://images.pexels.com/photos/4853854/pexels-photo-4853854.jpeg",
                        Tipo = "general",
                        DuracionMinutos = 60,
                        HoraInicio = new TimeOnly(18, 0),
                        Fecha = DateOnly.FromDateTime(new DateTime(2025, 10, 10)),
                        Dias = "[\"Martes\",\"Jueves\",\"Sábado\"]",
                        Capacidad = 20,
                        MostrarEnHome = true,
                        Activa = true,
                    },
                    new
                    {
                        Id = 3,
                        ProfesorId = 3,
                        SalaId = 3,
                        SucursalId = 3,
                        Nombre = "CrossFit Power",
                        Descripcion = "Sesiones de alta intensidad con pesas, cardio y fuerza funcional.",
                        Imagen = "https://images.pexels.com/photos/7674492/pexels-photo-7674492.jpeg",
                        Tipo = "especializada",
                        DuracionMinutos = 60,
                        HoraInicio = new TimeOnly(17, 0),
                        Fecha = DateOnly.FromDateTime(new DateTime(2025, 10, 11)),
                        Dias = "[\"Lunes\",\"Martes\",\"Jueves\"]",
                        Capacidad = 18,
                        MostrarEnHome = true,
                        Activa = true,
                    },
                    new
                    {
                        Id = 4,
                        ProfesorId = 4,
                        SalaId = 4,
                        SucursalId = 1,
                        Nombre = "Pilates Core",
                        Descripcion = "Fortalecé tu zona media con ejercicios controlados y precisos.",
                        Imagen = "https://images.pexels.com/photos/3775524/pexels-photo-3775524.jpeg",
                        Tipo = "general",
                        DuracionMinutos = 60,
                        HoraInicio = new TimeOnly(13, 0),
                        Fecha = DateOnly.FromDateTime(new DateTime(2025, 10, 12)),
                        Dias = "[\"Miércoles\",\"Viernes\"]",
                        Capacidad = 12,
                        MostrarEnHome = true,
                        Activa = true,
                    },
                    new
                    {
                        Id = 5,
                        ProfesorId = 5,
                        SalaId = 5,
                        SucursalId = 2,
                        Nombre = "Funcional Express",
                        Descripcion = "Entrenamiento completo en 45 minutos. Ideal para quienes tienen poco tiempo.",
                        Imagen = "https://images.pexels.com/photos/2294361/pexels-photo-2294361.jpeg",
                        Tipo = "general",
                        DuracionMinutos = 45,
                        HoraInicio = new TimeOnly(13, 0),
                        Fecha = DateOnly.FromDateTime(new DateTime(2025, 10, 12)),
                        Dias = "[\"Martes\",\"Jueves\"]",
                        Capacidad = 16,
                        MostrarEnHome = false,
                        Activa = true,
                    },
                    new
                    {
                        Id = 6,
                        ProfesorId = 6,
                        SalaId = 6,
                        SucursalId = 3,
                        Nombre = "Zumba Party",
                        Descripcion = "Baile, energía y diversión. Quemás calorías sin darte cuenta.",
                        Imagen = "https://images.pexels.com/photos/8436601/pexels-photo-8436601.jpeg",
                        Tipo = "general",
                        DuracionMinutos = 60,
                        HoraInicio = new TimeOnly(19, 0),
                        Fecha = DateOnly.FromDateTime(new DateTime(2025, 10, 12)),
                        Dias = "[\"Lunes\",\"Miércoles\",\"Sábado\"]",
                        Capacidad = 25,
                        MostrarEnHome = false,
                        Activa = true,
                    },
                    new
                    {
                        Id = 7,
                        ProfesorId = 7,
                        SalaId = 7,
                        SucursalId = 1,
                        Nombre = "Stretch & Relax",
                        Descripcion = "Ideal para la recuperación muscular y la relajación post entrenamiento.",
                        Imagen = "https://images.pexels.com/photos/3076509/pexels-photo-3076509.jpeg",
                        Tipo = "general",
                        DuracionMinutos = 60,
                        HoraInicio = new TimeOnly(20, 0),
                        Fecha = DateOnly.FromDateTime(new DateTime(2025, 10, 13)),
                        Dias = "[\"Miércoles\",\"Viernes\",\"Domingo\"]",
                        Capacidad = 14,
                        MostrarEnHome = false,
                        Activa = true,
                    }
                );
        }
    }
}
