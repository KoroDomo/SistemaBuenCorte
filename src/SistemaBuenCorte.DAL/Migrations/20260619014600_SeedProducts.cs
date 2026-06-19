using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaBuenCorte.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Categoria", "Descripcion", "FechaCreacion", "Nombre", "Precio", "Stock", "TipoVenta" },
                values: new object[,]
                {
                    { 3, true, "Embutidos", "Chorizo fresco, sazonado", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chorizo artesanal", 220.00m, 30.5m, "Peso" },
                    { 4, true, "Cerdo", "Costilla para asar, por kilo", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Costilla de cerdo", 300.00m, 18.75m, "Peso" },
                    { 5, true, "Embutidos", "Pack de 5 salchichas artesanales", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Salchichas mixtas (pack)", 120.00m, 50m, "Unidad" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
