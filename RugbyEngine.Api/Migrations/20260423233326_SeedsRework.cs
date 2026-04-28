using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RugbyEngine.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedsRework : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categoria",
                columns: new[] { "Id", "Abreviatura", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "EdadMaxima", "EdadMinima", "Nombre", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1, "SUP", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 0, 20, "Superior", null, null },
                    { 2, "M19", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 19, 18, "M19", null, null },
                    { 3, "M17", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 17, 17, "M17", null, null },
                    { 4, "M16", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 16, 16, "M16", null, null },
                    { 5, "M15", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 15, 15, "M15", null, null },
                    { 6, "M14", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 14, 14, "M14", null, null },
                    { 7, "M13", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 13, 13, "M13", null, null },
                    { 8, "M10", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 10, 10, "M10", null, null },
                    { 9, "M9", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 9, 9, "M9", null, null },
                    { 10, "M8", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 8, 8, "M8", null, null },
                    { 11, "M7", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 7, 7, "M7", null, null },
                    { 12, "ESC", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, 6, 0, "Escuelita ", null, null }
                });

            migrationBuilder.InsertData(
                table: "Posicion",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Nombre", "Numero", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Pilar", 1, null, null },
                    { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Hocker", 2, null, null },
                    { 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Pilar", 3, null, null },
                    { 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Segunda linea", 4, null, null },
                    { 5, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Segunda linea", 5, null, null },
                    { 6, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Ala ciega", 6, null, null },
                    { 7, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Ala abierta", 7, null, null },
                    { 8, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Octavo", 8, null, null },
                    { 9, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Medio scrum", 9, null, null },
                    { 10, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Apertura", 10, null, null },
                    { 11, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Wing izquierdo", 11, null, null },
                    { 12, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Centro interno", 12, null, null },
                    { 13, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Centro externo", 13, null, null },
                    { 14, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Wing derecho", 14, null, null },
                    { 15, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "FullBack", 15, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Categoria",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Posicion",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}
