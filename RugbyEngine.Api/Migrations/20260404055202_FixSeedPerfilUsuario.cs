using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RugbyEngine.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedPerfilUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UsuarioPerfil",
                columns: new[] { "PerfilId", "UsuarioId" },
                values: new object[] { 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UsuarioPerfil",
                keyColumns: new[] { "PerfilId", "UsuarioId" },
                keyValues: new object[] { 1, 1 });
        }
    }
}
