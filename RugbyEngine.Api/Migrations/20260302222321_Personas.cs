using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RugbyEngine.Api.Migrations
{
    /// <inheritdoc />
    public partial class Personas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    ToolTip = table.Column<string>(type: "text", nullable: false),
                    Icono = table.Column<string>(type: "text", nullable: false),
                    Ruta = table.Column<string>(type: "text", nullable: true),
                    MenuPadreId = table.Column<int>(type: "integer", nullable: true),
                    Lvl = table.Column<int>(type: "integer", nullable: false),
                    PermisoId = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("PK_Menu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menu_Menu_MenuPadreId",
                        column: x => x.MenuPadreId,
                        principalTable: "Menu",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Perfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
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
                    table.PrimaryKey("PK_Perfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerfilPermiso",
                columns: table => new
                {
                    PerfilId = table.Column<int>(type: "integer", nullable: false),
                    PermisoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilPermiso", x => new { x.PerfilId, x.PermisoId });
                    table.ForeignKey(
                        name: "FK_PerfilPermiso_Perfiles_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
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
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persona",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombres = table.Column<string>(type: "text", nullable: false),
                    Apellidos = table.Column<string>(type: "text", nullable: false),
                    Documento = table.Column<string>(type: "text", nullable: false),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: false),
                    Cobertura = table.Column<string>(type: "text", nullable: true),
                    NroAfiliado = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Domicilio = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Persona", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PersonaId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_Persona_PersonaId",
                        column: x => x.PersonaId,
                        principalTable: "Persona",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Usuario_Usuario_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuario_Usuario_DeletedById",
                        column: x => x.DeletedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuario_Usuario_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioPerfil",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    PerfilId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioPerfil", x => new { x.UsuarioId, x.PerfilId });
                    table.ForeignKey(
                        name: "FK_UsuarioPerfil_Perfiles_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioPerfil_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Menu_CreatedById",
                table: "Menu",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_DeletedById",
                table: "Menu",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_MenuPadreId",
                table: "Menu",
                column: "MenuPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_PermisoId",
                table: "Menu",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_UpdatedById",
                table: "Menu",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_CreatedById",
                table: "Perfiles",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_DeletedById",
                table: "Perfiles",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_UpdatedById",
                table: "Perfiles",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilPermiso_PermisoId",
                table: "PerfilPermiso",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Codigo",
                table: "Permisos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_CreatedById",
                table: "Permisos",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_DeletedById",
                table: "Permisos",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_UpdatedById",
                table: "Permisos",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Persona_CreatedById",
                table: "Persona",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Persona_DeletedById",
                table: "Persona",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Persona_UpdatedById",
                table: "Persona",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_CreatedById",
                table: "Usuario",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_DeletedById",
                table: "Usuario",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_PersonaId",
                table: "Usuario",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_UpdatedById",
                table: "Usuario",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Username",
                table: "Usuario",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioPerfil_PerfilId",
                table: "UsuarioPerfil",
                column: "PerfilId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menu_Permisos_PermisoId",
                table: "Menu",
                column: "PermisoId",
                principalTable: "Permisos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Menu_Usuario_CreatedById",
                table: "Menu",
                column: "CreatedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Menu_Usuario_DeletedById",
                table: "Menu",
                column: "DeletedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Menu_Usuario_UpdatedById",
                table: "Menu",
                column: "UpdatedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Perfiles_Usuario_CreatedById",
                table: "Perfiles",
                column: "CreatedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Perfiles_Usuario_DeletedById",
                table: "Perfiles",
                column: "DeletedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Perfiles_Usuario_UpdatedById",
                table: "Perfiles",
                column: "UpdatedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfilPermiso_Permisos_PermisoId",
                table: "PerfilPermiso",
                column: "PermisoId",
                principalTable: "Permisos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permisos_Usuario_CreatedById",
                table: "Permisos",
                column: "CreatedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Permisos_Usuario_DeletedById",
                table: "Permisos",
                column: "DeletedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Permisos_Usuario_UpdatedById",
                table: "Permisos",
                column: "UpdatedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Persona_Usuario_CreatedById",
                table: "Persona",
                column: "CreatedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Persona_Usuario_DeletedById",
                table: "Persona",
                column: "DeletedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Persona_Usuario_UpdatedById",
                table: "Persona",
                column: "UpdatedById",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persona_Usuario_CreatedById",
                table: "Persona");

            migrationBuilder.DropForeignKey(
                name: "FK_Persona_Usuario_DeletedById",
                table: "Persona");

            migrationBuilder.DropForeignKey(
                name: "FK_Persona_Usuario_UpdatedById",
                table: "Persona");

            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "PerfilPermiso");

            migrationBuilder.DropTable(
                name: "UsuarioPerfil");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Perfiles");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Persona");
        }
    }
}
