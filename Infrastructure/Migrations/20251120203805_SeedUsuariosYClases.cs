using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsuariosYClases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Apellido", "Direccion", "Discriminator", "Dni", "Email", "FailedLoginAttempts", "FechaNacimiento", "Genero", "Image", "LockoutEnd", "Nombre", "PasswordHash", "Role", "SucursalId", "Telefono" },
                values: new object[,]
                {
                    { 1, true, "Santarelli", null, "Usuario", "1", "simon@example.com", 0, new DateOnly(2005, 3, 12), null, "https://placehold.co/120x120?text=User", null, "Simón", "HfLlcoG+fBtUcKKRzzb3sTcc0Kk1P0qAGDSXV656AFKkzPJ5tGWmtiXdOVLmMjT7", "SuperAdministrador", 1, "3411234567" },
                    { 8, true, "Cumini", null, "Usuario", "39222001", "fran@highfit.com", 0, new DateOnly(1995, 5, 20), null, "https://placehold.co/120x120?text=User", null, "Francisco", "+yFOjDzWtkAR9lxanKPZsbY3Na3Kv5ZrRBOpG0nJfBt7YFf/LJurnWJrskA6QWT8", "Administrador", 1, "3412345678" },
                    { 9, true, "Cardillo", null, "Usuario", "39555111", "pablo@highfit.com", 0, new DateOnly(1996, 10, 3), null, "https://placehold.co/120x120?text=User", null, "Pablo", "hz5yIy4nK3ZmVcTPJki6YH0sTq3debyK804lrYE+bqI0Fgz7i/PXysORUKzjBtGH", "Recepcionista", 1, "3413456789" },
                    { 10, true, "Perez", null, "Alumno", "40111222", "cliente@demo.com", 0, new DateOnly(1997, 4, 1), null, "https://placehold.co/120x120?text=User", null, "Cliente Demo", "FNnfcvojVZDEjAzKc+6S3aD0QOHlc0vd6lIRLQ7Nt1pYxgP/TtcZvobB9F/gp+vO", "Alumno", 1, "+5493415555555" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
