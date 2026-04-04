using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RugbyEngine.Api.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movimientos_EnviaId",
                table: "Movimientos");

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento_Unicidad",
                table: "Movimientos",
                columns: new[] { "EnviaId", "RecibeId", "TipoMovimientoId", "Fecha" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movimiento_Unicidad",
                table: "Movimientos");

            migrationBuilder.CreateIndex(
                name: "IX_Movimientos_EnviaId",
                table: "Movimientos",
                column: "EnviaId");
        }
    }
}
