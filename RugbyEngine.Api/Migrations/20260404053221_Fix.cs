using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RugbyEngine.Api.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Persona_PersonaId",
                table: "Usuario");

            migrationBuilder.AlterColumn<int>(
                name: "PersonaId",
                table: "Usuario",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Email", "LastLogin", "PasswordHash", "PersonaId", "UpdatedAt", "UpdatedById", "Username" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "agustingigena1704@gmail.com", new DateTime(2026, 4, 4, 5, 32, 17, 792, DateTimeKind.Utc).AddTicks(4763), "$2a$11$knIWvYSD.JF1fM.2GKgZ4eMdCFDV26H/hEVyj5A9upJwIPIVFXZI.", null, null, null, "agigena" });

            migrationBuilder.InsertData(
                table: "Perfiles",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Descripcion", "Nombre", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso Total", "Administrador", null, null },
                    { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Entrenador", "Entrenador", null, null },
                    { 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Usuario General", "Jugador", null, null }
                });

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "Codigo", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Descripcion", "Nombre", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1, "admin", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Administrador del sistema", "Administrador", null, null },
                    { 2, "tesoreria", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso a Tesoreria", "Tesoreria", null, null },
                    { 3, "registros", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso a Registros", "Registros", null, null },
                    { 4, "personas", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso a Personas", "Personas", null, null },
                    { 5, "3t", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso a 3T", "3T", null, null },
                    { 6, "administracion", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso a Administracion", "Administracion", null, null },
                    { 7, "usuarios", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso a Usuarios", "Usuarios", null, null },
                    { 8, "entrenamientos", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso a Entrenamientos", "Entrenamientos", null, null },
                    { 9, "asistencia", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso a Asistencia", "Asistencia", null, null }
                });

            migrationBuilder.InsertData(
                table: "Persona",
                columns: new[] { "Id", "Apellidos", "Cobertura", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Documento", "Domicilio", "Email", "FechaNacimiento", "Nombres", "NroAfiliado", "Telefono", "UpdatedAt", "UpdatedById" },
                values: new object[] { 1, "Gigena", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "44590912", null, "agustingigena1704@gmail.com", new DateOnly(2000, 1, 1), "Agustin", null, null, null, null });

            migrationBuilder.InsertData(
                table: "Menu",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Icono", "Lvl", "MenuPadreId", "PermisoId", "Ruta", "Titulo", "ToolTip", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 0, null, 8, null, "Entrenamientos", null, null, null },
                    { 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 0, null, 2, null, "Tesoreria", null, null, null },
                    { 5, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 0, null, 6, null, "Administracion", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "PerfilPermiso",
                columns: new[] { "PerfilId", "PermisoId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 1, 5 },
                    { 1, 6 },
                    { 1, 7 },
                    { 1, 8 },
                    { 1, 9 },
                    { 2, 3 },
                    { 2, 4 },
                    { 2, 6 },
                    { 2, 8 },
                    { 2, 9 }
                });

            migrationBuilder.InsertData(
                table: "Menu",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Icono", "Lvl", "MenuPadreId", "PermisoId", "Ruta", "Titulo", "ToolTip", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 7, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 1, 5, 3, null, "Registro", null, null, null },
                    { 11, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 1, 4, 5, "/Tesoreria/3T", "3T", null, null, null },
                    { 15, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 2, 1, 9, "/Entrenamientos/Asistencia", "Asistencia", null, null, null },
                    { 10, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 2, 7, 4, "/Admin/Registro/Personas", "Personas", null, null, null },
                    { 12, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 2, 11, 5, "/Tesoreria/3T/Pagos", "Pagos", null, null, null },
                    { 14, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 2, 7, 1, "/Admin/Registro/Usuarios", "Usuarios", null, null, null }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Persona_PersonaId",
                table: "Usuario",
                column: "PersonaId",
                principalTable: "Persona",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Persona_PersonaId",
                table: "Usuario");

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 6 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 7 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 8 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 9 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 2, 6 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 2, 8 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 2, 9 });

            migrationBuilder.DeleteData(
                table: "Perfiles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Persona",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Perfiles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Perfiles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Usuario",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "PersonaId",
                table: "Usuario",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Persona_PersonaId",
                table: "Usuario",
                column: "PersonaId",
                principalTable: "Persona",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
