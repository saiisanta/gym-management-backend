using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAllClasesAndRelated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Salas",
                columns: new[] { "Id", "Activa", "Capacidad", "Descripcion", "Nombre", "SucursalId", "Tipo" },
                values: new object[,]
                {
                    { 5, true, 22, "Sala de cardio con equipamiento variado", "Sala 5", 2, "Cardio" },
                    { 7, true, 18, "Estudio pequeño para sesiones especializadas", "Sala 7", 1, "Estudio" }
                });

            migrationBuilder.InsertData(
                table: "Sucursales",
                columns: new[] { "Id", "Activa", "Direccion", "Email", "Nombre", "Telefono" },
                values: new object[] { 3, true, "Av. Sur 789", "sur@gym.com", "Sucursal Sur", "555-0003" });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Apellido", "Direccion", "Discriminator", "Dni", "Email", "FailedLoginAttempts", "FechaNacimiento", "Genero", "Image", "LockoutEnd", "Nombre", "PasswordHash", "Role", "SucursalId", "Telefono" },
                values: new object[,]
                {
                    { 2, true, "González", null, "Profesor", "11111111", "maria.profe@gym.com", 0, new DateOnly(1985, 6, 12), null, null, null, "María", "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", "Profesor", null, "555-1002" },
                    { 3, true, "Pérez", null, "Profesor", "22222222", "carlos.profe@gym.com", 0, new DateOnly(1980, 3, 2), null, null, null, "Carlos", "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", "Profesor", null, "555-1003" },
                    { 4, true, "Ramírez", null, "Profesor", "33333333", "lucia.profe@gym.com", 0, new DateOnly(1990, 11, 5), null, null, null, "Lucía", "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", "Profesor", null, "555-1004" },
                    { 5, true, "Suárez", null, "Profesor", "44444444", "martin.profe@gym.com", 0, new DateOnly(1988, 8, 20), null, null, null, "Martín", "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", "Profesor", null, "555-1005" },
                    { 6, true, "López", null, "Profesor", "55555555", "sofia.profe@gym.com", 0, new DateOnly(1992, 2, 14), null, null, null, "Sofía", "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", "Profesor", null, "555-1006" },
                    { 7, true, "Fernández", null, "Profesor", "66666666", "diego.profe@gym.com", 0, new DateOnly(1983, 9, 30), null, null, null, "Diego", "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", "Profesor", null, "555-1007" }
                });

            migrationBuilder.InsertData(
                table: "Clases",
                columns: new[] { "Id", "Activa", "Capacidad", "Descripcion", "Dias", "DuracionMinutos", "Fecha", "HoraInicio", "Imagen", "MostrarEnHome", "Nombre", "ProfesorId", "SalaId", "SucursalId", "Tipo" },
                values: new object[,]
                {
                    { 1, true, 15, "Clase suave para activar el cuerpo y la mente. Ideal para empezar el día con energía.", "[\"Lunes\",\"Miércoles\",\"Viernes\"]", 60, new DateOnly(2025, 10, 11), new TimeOnly(0, 0, 0), "https://images.unsplash.com/photo-1506126613408-eca07ce68773?auto=format&fit=crop&w=800&q=80", true, "Yoga Matinal", 2, 1, 1, "general" },
                    { 2, true, 20, "Entrenamiento cardiovascular sobre bicicletas fijas, con música y mucha energía.", "[\"Martes\",\"Jueves\",\"Sábado\"]", 60, new DateOnly(2025, 10, 10), new TimeOnly(18, 0, 0), "https://images.pexels.com/photos/4853854/pexels-photo-4853854.jpeg", true, "Spin Intenso", 2, 2, 2, "general" },
                    { 3, true, 18, "Sesiones de alta intensidad con pesas, cardio y fuerza funcional.", "[\"Lunes\",\"Martes\",\"Jueves\"]", 60, new DateOnly(2025, 10, 11), new TimeOnly(17, 0, 0), "https://images.pexels.com/photos/7674492/pexels-photo-7674492.jpeg", true, "CrossFit Power", 3, 3, 3, "especializada" },
                    { 4, true, 12, "Fortalecé tu zona media con ejercicios controlados y precisos.", "[\"Miércoles\",\"Viernes\"]", 60, new DateOnly(2025, 10, 12), new TimeOnly(13, 0, 0), "https://images.pexels.com/photos/3775524/pexels-photo-3775524.jpeg", true, "Pilates Core", 4, 4, 1, "general" },
                    { 5, true, 16, "Entrenamiento completo en 45 minutos. Ideal para quienes tienen poco tiempo.", "[\"Martes\",\"Jueves\"]", 45, new DateOnly(2025, 10, 12), new TimeOnly(13, 0, 0), "https://images.pexels.com/photos/2294361/pexels-photo-2294361.jpeg", false, "Funcional Express", 5, 5, 2, "general" },
                    { 7, true, 14, "Ideal para la recuperación muscular y la relajación post entrenamiento.", "[\"Miércoles\",\"Viernes\",\"Domingo\"]", 60, new DateOnly(2025, 10, 13), new TimeOnly(20, 0, 0), "https://images.pexels.com/photos/3076509/pexels-photo-3076509.jpeg", false, "Stretch & Relax", 7, 7, 1, "general" }
                });

            migrationBuilder.InsertData(
                table: "Salas",
                columns: new[] { "Id", "Activa", "Capacidad", "Descripcion", "Nombre", "SucursalId", "Tipo" },
                values: new object[] { 6, true, 30, "Espacio amplio para clases de baile y zumba", "Sala 6", 3, "Baile" });

            migrationBuilder.InsertData(
                table: "Clases",
                columns: new[] { "Id", "Activa", "Capacidad", "Descripcion", "Dias", "DuracionMinutos", "Fecha", "HoraInicio", "Imagen", "MostrarEnHome", "Nombre", "ProfesorId", "SalaId", "SucursalId", "Tipo" },
                values: new object[] { 6, true, 25, "Baile, energía y diversión. Quemás calorías sin darte cuenta.", "[\"Lunes\",\"Miércoles\",\"Sábado\"]", 60, new DateOnly(2025, 10, 12), new TimeOnly(19, 0, 0), "https://images.pexels.com/photos/8436601/pexels-photo-8436601.jpeg", false, "Zumba Party", 6, 6, 3, "general" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clases",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clases",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clases",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clases",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Clases",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Clases",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Clases",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Salas",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Salas",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Salas",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
