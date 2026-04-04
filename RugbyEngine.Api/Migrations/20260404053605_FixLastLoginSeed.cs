using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RugbyEngine.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixLastLoginSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastLogin",
                value: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastLogin",
                value: new DateTime(2026, 4, 4, 5, 32, 17, 792, DateTimeKind.Utc).AddTicks(4763));
        }
    }
}
