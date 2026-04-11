using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RugbyEngine.Api.Migrations
{
    /// <inheritdoc />
    public partial class PosicionNumeroInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE \"Posicion\" ALTER COLUMN \"Numero\" TYPE integer USING \"Numero\"::integer;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Numero",
                table: "Posicion",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
