using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RugbyEngine.Api.Data.Entities;

namespace RugbyEngine.Api.Data.Seeds
{
    public class PerfilesSeed : IEntityTypeConfiguration<Perfil>
    {
        private readonly DateTime seedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<Perfil> builder)
        {
            builder.HasData(
                new Perfil { Id = 1, Nombre = "Administrador", Descripcion = "Acceso Total",   CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Perfil { Id = 2, Nombre = "Entrenador",    Descripcion = "Entrenador",      CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Perfil { Id = 4, Nombre = "Jugador",       Descripcion = "Usuario General", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! }
            );

            builder.HasMany(p => p.Permisos)
                .WithMany(u => u.Perfiles)
                .UsingEntity(j => j.HasData(
                    // Administrador: todos los permisos
                    new { PerfilId = 1, PermisoId = 1 },
                    new { PerfilId = 1, PermisoId = 2 },
                    new { PerfilId = 1, PermisoId = 3 },
                    new { PerfilId = 1, PermisoId = 4 },
                    new { PerfilId = 1, PermisoId = 5 },
                    new { PerfilId = 1, PermisoId = 6 },
                    new { PerfilId = 1, PermisoId = 7 },
                    new { PerfilId = 1, PermisoId = 8 },
                    new { PerfilId = 1, PermisoId = 9 },
                    // Entrenador: Registros, Personas, Administracion, Entrenamientos, Asistencia
                    new { PerfilId = 2, PermisoId = 3 },
                    new { PerfilId = 2, PermisoId = 4 },
                    new { PerfilId = 2, PermisoId = 6 },
                    new { PerfilId = 2, PermisoId = 8 },
                    new { PerfilId = 2, PermisoId = 9 }
                ));
        }
    }
}
