using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RugbyEngine.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModuloEntrenamientos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventoCuentas_Cuenta_CuentaId",
                table: "EventoCuentas");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Cuenta_EnviaId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Cuenta_RecibeId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonaCuentas_Cuenta_CuentaId",
                table: "PersonaCuentas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cuenta",
                table: "Cuenta");

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.RenameTable(
                name: "Cuenta",
                newName: "Cuentas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cuentas",
                table: "Cuentas",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Abreviatura = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EdadMinima = table.Column<int>(type: "integer", nullable: false),
                    EdadMaxima = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedById = table.Column<int>(type: "integer", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BorradoLogico = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categoria_Usuario_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categoria_Usuario_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categoria_Usuario_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Posicion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Numero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedById = table.Column<int>(type: "integer", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BorradoLogico = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posicion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posicion_Usuario_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posicion_Usuario_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Posicion_Usuario_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Entrenamiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedById = table.Column<int>(type: "integer", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BorradoLogico = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entrenamiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entrenamiento_Categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entrenamiento_Usuario_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entrenamiento_Usuario_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entrenamiento_Usuario_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Jugador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonaId = table.Column<int>(type: "integer", nullable: false),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false),
                    PosicionPrincipalId = table.Column<int>(type: "integer", nullable: false),
                    PosicionSecundariaId = table.Column<int>(type: "integer", nullable: true),
                    PosicionTerciariaId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedById = table.Column<int>(type: "integer", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BorradoLogico = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jugador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jugador_Categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jugador_Persona_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Persona",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jugador_Posicion_PosicionPrincipalId",
                        column: x => x.PosicionPrincipalId,
                        principalTable: "Posicion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jugador_Posicion_PosicionSecundariaId",
                        column: x => x.PosicionSecundariaId,
                        principalTable: "Posicion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jugador_Posicion_PosicionTerciariaId",
                        column: x => x.PosicionTerciariaId,
                        principalTable: "Posicion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jugador_Usuario_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jugador_Usuario_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jugador_Usuario_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Asistencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntrenamientoId = table.Column<int>(type: "integer", nullable: false),
                    JugadorId = table.Column<int>(type: "integer", nullable: false),
                    Presente = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedById = table.Column<int>(type: "integer", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BorradoLogico = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Asistencia_Entrenamiento_EntrenamientoId",
                        column: x => x.EntrenamientoId,
                        principalTable: "Entrenamiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Asistencia_Jugador_JugadorId",
                        column: x => x.JugadorId,
                        principalTable: "Jugador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asistencia_Usuario_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asistencia_Usuario_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Asistencia_Usuario_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 1,
                column: "Ruta",
                value: "Entrenamientos");

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 4,
                column: "Ruta",
                value: "Tesoreria");

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 5,
                column: "Ruta",
                value: "Admin");

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 7,
                column: "Ruta",
                value: "Registro");

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 10,
                column: "Ruta",
                value: "Personas");

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 11,
                column: "Ruta",
                value: "3T");

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 12,
                column: "Ruta",
                value: "Pagos");

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 14,
                column: "Ruta",
                value: "Usuarios");

            migrationBuilder.InsertData(
                table: "Menu",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Icono", "Lvl", "MenuPadreId", "PermisoId", "Ruta", "Titulo", "ToolTip", "UpdatedAt", "UpdatedById" },
                values: new object[] { 19, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 1, 1, 8, "Entrenamientos", "Entrenamientos", null, null, null });

            migrationBuilder.InsertData(
                table: "Permisos",
                columns: new[] { "Id", "Codigo", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Descripcion", "Nombre", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 10, "categorias", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso a Categorias", "Categorias", null, null },
                    { 11, "posiciones", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso a Posiciones", "Posiciones", null, null },
                    { 12, "jugadores", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, "Acceso a Jugadores", "Jugadores", null, null }
                });

            migrationBuilder.InsertData(
                table: "Menu",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Icono", "Lvl", "MenuPadreId", "PermisoId", "Ruta", "Titulo", "ToolTip", "UpdatedAt", "UpdatedById" },
                values: new object[,]
                {
                    { 16, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 1, 1, 10, "Categorias", "Categorías", null, null, null },
                    { 17, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 1, 1, 11, "Posiciones", "Posiciones", null, null, null },
                    { 18, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 1, 1, 12, "Jugadores", "Jugadores", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "PerfilPermiso",
                columns: new[] { "PerfilId", "PermisoId" },
                values: new object[,]
                {
                    { 1, 10 },
                    { 1, 11 },
                    { 1, 12 },
                    { 2, 10 },
                    { 2, 11 },
                    { 2, 12 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_CreatedById",
                table: "Asistencia",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_DeletedById",
                table: "Asistencia",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_EntrenamientoId_JugadorId",
                table: "Asistencia",
                columns: new[] { "EntrenamientoId", "JugadorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_JugadorId",
                table: "Asistencia",
                column: "JugadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencia_UpdatedById",
                table: "Asistencia",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_CreatedById",
                table: "Categoria",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_DeletedById",
                table: "Categoria",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Categoria_UpdatedById",
                table: "Categoria",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Entrenamiento_CategoriaId",
                table: "Entrenamiento",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Entrenamiento_CreatedById",
                table: "Entrenamiento",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Entrenamiento_DeletedById",
                table: "Entrenamiento",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Entrenamiento_UpdatedById",
                table: "Entrenamiento",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Jugador_CategoriaId",
                table: "Jugador",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Jugador_CreatedById",
                table: "Jugador",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Jugador_DeletedById",
                table: "Jugador",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Jugador_PersonaId_CategoriaId",
                table: "Jugador",
                columns: new[] { "PersonaId", "CategoriaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jugador_PosicionPrincipalId",
                table: "Jugador",
                column: "PosicionPrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_Jugador_PosicionSecundariaId",
                table: "Jugador",
                column: "PosicionSecundariaId");

            migrationBuilder.CreateIndex(
                name: "IX_Jugador_PosicionTerciariaId",
                table: "Jugador",
                column: "PosicionTerciariaId");

            migrationBuilder.CreateIndex(
                name: "IX_Jugador_UpdatedById",
                table: "Jugador",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Posicion_CreatedById",
                table: "Posicion",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Posicion_DeletedById",
                table: "Posicion",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Posicion_UpdatedById",
                table: "Posicion",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_EventoCuentas_Cuentas_CuentaId",
                table: "EventoCuentas",
                column: "CuentaId",
                principalTable: "Cuentas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Cuentas_EnviaId",
                table: "Movimientos",
                column: "EnviaId",
                principalTable: "Cuentas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Cuentas_RecibeId",
                table: "Movimientos",
                column: "RecibeId",
                principalTable: "Cuentas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonaCuentas_Cuentas_CuentaId",
                table: "PersonaCuentas",
                column: "CuentaId",
                principalTable: "Cuentas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventoCuentas_Cuentas_CuentaId",
                table: "EventoCuentas");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Cuentas_EnviaId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_Movimientos_Cuentas_RecibeId",
                table: "Movimientos");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonaCuentas_Cuentas_CuentaId",
                table: "PersonaCuentas");

            migrationBuilder.DropTable(
                name: "Asistencia");

            migrationBuilder.DropTable(
                name: "Entrenamiento");

            migrationBuilder.DropTable(
                name: "Jugador");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Posicion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cuentas",
                table: "Cuentas");

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 10 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 11 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 1, 12 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 2, 10 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 2, 11 });

            migrationBuilder.DeleteData(
                table: "PerfilPermiso",
                keyColumns: new[] { "PerfilId", "PermisoId" },
                keyValues: new object[] { 2, 12 });

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Permisos",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.RenameTable(
                name: "Cuentas",
                newName: "Cuenta");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cuenta",
                table: "Cuenta",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 1,
                column: "Ruta",
                value: null);

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 4,
                column: "Ruta",
                value: null);

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 5,
                column: "Ruta",
                value: null);

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 7,
                column: "Ruta",
                value: null);

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 10,
                column: "Ruta",
                value: "/Admin/Registro/Personas");

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 11,
                column: "Ruta",
                value: "/Tesoreria/3T");

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 12,
                column: "Ruta",
                value: "/Tesoreria/3T/Pagos");

            migrationBuilder.UpdateData(
                table: "Menu",
                keyColumn: "Id",
                keyValue: 14,
                column: "Ruta",
                value: "/Admin/Registro/Usuarios");

            migrationBuilder.InsertData(
                table: "Menu",
                columns: new[] { "Id", "CreatedAt", "CreatedById", "DeletedAt", "DeletedById", "Icono", "Lvl", "MenuPadreId", "PermisoId", "Ruta", "Titulo", "ToolTip", "UpdatedAt", "UpdatedById" },
                values: new object[] { 15, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, null, null, 2, 1, 9, "/Entrenamientos/Asistencia", "Asistencia", null, null, null });

            migrationBuilder.AddForeignKey(
                name: "FK_EventoCuentas_Cuenta_CuentaId",
                table: "EventoCuentas",
                column: "CuentaId",
                principalTable: "Cuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Cuenta_EnviaId",
                table: "Movimientos",
                column: "EnviaId",
                principalTable: "Cuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Movimientos_Cuenta_RecibeId",
                table: "Movimientos",
                column: "RecibeId",
                principalTable: "Cuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonaCuentas_Cuenta_CuentaId",
                table: "PersonaCuentas",
                column: "CuentaId",
                principalTable: "Cuenta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
