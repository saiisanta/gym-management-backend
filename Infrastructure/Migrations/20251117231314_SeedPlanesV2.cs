using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPlanesV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Planes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MaxReservasPorMes", "Precio", "TiposPermitidos" },
                values: new object[] { 8, 25000m, "[\"general\"]" });

            migrationBuilder.UpdateData(
                table: "Planes",
                keyColumn: "Id",
                keyValue: 2,
                column: "TiposPermitidos",
                value: "[]");

            migrationBuilder.UpdateData(
                table: "Planes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "MaxReservasPorMes", "Precio", "TiposPermitidos" },
                values: new object[] { null, 55000m, "[]" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Planes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MaxReservasPorMes", "Precio", "TiposPermitidos" },
                values: new object[] { 10, 20000m, "general" });

            migrationBuilder.UpdateData(
                table: "Planes",
                keyColumn: "Id",
                keyValue: 2,
                column: "TiposPermitidos",
                value: "general");

            migrationBuilder.UpdateData(
                table: "Planes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "MaxReservasPorMes", "Precio", "TiposPermitidos" },
                values: new object[] { 30, 50000m, "general" });
        }
    }
}
