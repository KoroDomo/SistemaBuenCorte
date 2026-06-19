using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaBuenCorte.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaHash",
                value: "$2a$11$ZiF34myITAxk5LYYp4vIS.aN.zFmicf1eoscUjqFlriTsEKQKWgq6");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "ContrasenaHash",
                value: "$2a$11$jEUie1ZnO7uOi.aj0Trki.QkINtTVDqa784n3C/aIiKM2dHPcGXsC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaHash",
                value: "PLACEHOLDER_CAMBIAR");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "ContrasenaHash",
                value: "PLACEHOLDER_CAMBIAR");
        }
    }
}
