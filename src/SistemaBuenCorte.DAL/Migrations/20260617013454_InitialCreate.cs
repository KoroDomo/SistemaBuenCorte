using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SistemaBuenCorte.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TipoVenta = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Stock = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContrasenaHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Categoria", "Descripcion", "FechaCreacion", "Nombre", "Precio", "Stock", "TipoVenta" },
                values: new object[,]
                {
                    { 1, true, "Res", "Carne molida fresca, 80/20", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Carne molida de res", 280.00m, 25.500m, "Peso" },
                    { 2, true, "Pollo", "Bandeja de pechuga deshuesada", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pechuga de pollo (bandeja)", 350.00m, 40m, "Unidad" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "ContrasenaHash", "FechaCreacion", "NombreCompleto", "NombreUsuario", "Rol" },
                values: new object[,]
                {
                    { 1, true, "PLACEHOLDER_CAMBIAR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Administrador del Sistema", "admin", "Administrador" },
                    { 2, true, "PLACEHOLDER_CAMBIAR", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cajero de Prueba", "cajero", "Cajero" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
