using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RugbyEngine.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAsistenciaPresententeToEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Presente",
                table: "Asistencia");

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Asistencia",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Asistencia");

            migrationBuilder.AddColumn<bool>(
                name: "Presente",
                table: "Asistencia",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
