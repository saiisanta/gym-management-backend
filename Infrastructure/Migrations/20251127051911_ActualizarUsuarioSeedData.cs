using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarUsuarioSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlanId",
                table: "Usuarios",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: -9999,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "PlanId", "SucursalId", "Telefono" },
                values: new object[] { "Sede Central, Admin St 101", "No especificado", "https://placehold.co/120x120?text=Admin", "R7n4G/D8mUoF9g01/M8t4gH1cO4v5xT2zS0l+L4nI3q7kP9wJ0yT+uF0xV8aC2bF", null, 1, "555-9999" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Direccion", "Genero", "Image", "PlanId" },
                values: new object[] { "Av. Siempre Viva 742", "Masculino", "https://placehold.co/120x120?text=SuperAdmin", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "PlanId", "SucursalId" },
                values: new object[] { "Calle Falsa 123", "Femenino", "https://placehold.co/120x120?text=Maria", "s+LhM8wzY1QxK2n0tN7q5wX2yC4aA8bC9dE0fG3hJ6p8rT4uV7yZ2eB3eD5gH0iK", null, 1 });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "PlanId", "SucursalId" },
                values: new object[] { "Av. Rivadavia 800", "Masculino", "https://placehold.co/120x120?text=Carlos", "s+LhM8wzY1QxK2n0tN7q5wX2yC4aA8bC9dE0fG3hJ6p8rT4uV7yZ2eB3eD5gH0iK", null, 2 });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "PlanId", "SucursalId" },
                values: new object[] { "Calle 9 de Julio 50", "Femenino", "https://placehold.co/120x120?text=Lucia", "s+LhM8wzY1QxK2n0tN7q5wX2yC4aA8bC9dE0fG3hJ6p8rT4uV7yZ2eB3eD5gH0iK", null, 1 });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "PlanId", "SucursalId" },
                values: new object[] { "Boulevard Oroño 1500", "Masculino", "https://placehold.co/120x120?text=Martin", "s+LhM8wzY1QxK2n0tN7q5wX2yC4aA8bC9dE0fG3hJ6p8rT4uV7yZ2eB3eD5gH0iK", null, 2 });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "PlanId", "SucursalId" },
                values: new object[] { "Pasaje Zeballos 33", "Femenino", "https://placehold.co/120x120?text=Sofia", "s+LhM8wzY1QxK2n0tN7q5wX2yC4aA8bC9dE0fG3hJ6p8rT4uV7yZ2eB3eD5gH0iK", null, 3 });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "PlanId", "SucursalId" },
                values: new object[] { "Ruta 9 KM 10", "Masculino", "https://placehold.co/120x120?text=Diego", "s+LhM8wzY1QxK2n0tN7q5wX2yC4aA8bC9dE0fG3hJ6p8rT4uV7yZ2eB3eD5gH0iK", null, 1 });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Direccion", "Genero", "Image", "PlanId" },
                values: new object[] { "Calle San Martín 1500", "Masculino", "https://placehold.co/120x120?text=AdminFran", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Direccion", "Genero", "Image", "PlanId" },
                values: new object[] { "Av. Pellegrini 200", "Masculino", "https://placehold.co/120x120?text=Recep", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Direccion", "Genero", "Image", "PlanId" },
                values: new object[] { "Bv. 27 de Febrero 100", "Masculino", "https://placehold.co/120x120?text=Cliente", 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "Usuarios");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: -9999,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "SucursalId", "Telefono" },
                values: new object[] { null, null, null, "PrP+ZrMeO00Q+nC1ytSccRIpSvauTkdqHEBRVdRaoSE=", null, "" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Direccion", "Genero", "Image" },
                values: new object[] { null, null, "https://placehold.co/120x120?text=User" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "SucursalId" },
                values: new object[] { null, null, null, "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "SucursalId" },
                values: new object[] { null, null, null, "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "SucursalId" },
                values: new object[] { null, null, null, "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "SucursalId" },
                values: new object[] { null, null, null, "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "SucursalId" },
                values: new object[] { null, null, null, "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Direccion", "Genero", "Image", "PasswordHash", "SucursalId" },
                values: new object[] { null, null, null, "K3gLVAHR3OIvAHe3dyrKl2HIfEdJOYoutYwq0/5Vjrw=", null });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Direccion", "Genero", "Image" },
                values: new object[] { null, null, "https://placehold.co/120x120?text=User" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Direccion", "Genero", "Image" },
                values: new object[] { null, null, "https://placehold.co/120x120?text=User" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Direccion", "Genero", "Image" },
                values: new object[] { null, null, "https://placehold.co/120x120?text=User" });
        }
    }
}
