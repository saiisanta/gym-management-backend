using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPlanes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Planes",
                columns: new[] { "Id", "Activo", "Descripcion", "DuracionDias", "MaxReservasPorMes", "Nombre", "Precio", "TiposPermitidos" },
                values: new object[,]
                {
                    { 1, true, "Acceso a clases generales y recursos introductorios. Ideal para quienes recién comienzan.", 30, 10, "Básico", 20000m, "general" },
                    { 2, true, "Incluye acceso a clases especializadas, soporte prioritario y material exclusivo.", 30, 20, "Avanzado", 40000m, "general" },
                    { 3, true, "Acceso ilimitado a todos los cursos, mentorías personalizadas y beneficios exclusivos.", 30, 30, "Premium", 50000m, "general" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Planes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Planes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Planes",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
