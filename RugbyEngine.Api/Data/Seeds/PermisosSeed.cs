using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RugbyEngine.Api.Data.Entities;

namespace RugbyEngine.Api.Data.Seeds
{
    public class PermisosSeed : IEntityTypeConfiguration<Permiso>
    {
        private readonly DateTime seedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<Permiso> builder)
        {
            builder.HasData(
                new Permiso { Id = 1, Nombre = "Administrador", Codigo = "admin", Descripcion = "Administrador del sistema", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Permiso { Id = 2, Nombre = "Tesoreria", Codigo = "tesoreria", Descripcion = "Acceso a Tesoreria", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Permiso { Id = 3, Nombre = "Registros", Codigo = "registros", Descripcion = "Acceso a Registros", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Permiso { Id = 4, Nombre = "Personas", Codigo = "personas", Descripcion = "Acceso a Personas", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Permiso { Id = 5, Nombre = "3T", Codigo = "3t", Descripcion = "Acceso a 3T", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Permiso { Id = 6, Nombre = "Administracion", Codigo = "administracion", Descripcion = "Acceso a Administracion", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Permiso { Id = 7, Nombre = "Usuarios", Codigo = "usuarios", Descripcion = "Acceso a Usuarios", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Permiso { Id = 8, Nombre = "Entrenamientos", Codigo = "entrenamientos", Descripcion = "Acceso a Entrenamientos", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Permiso { Id = 9, Nombre = "Asistencia", Codigo = "asistencia", Descripcion = "Acceso a Asistencia", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Permiso { Id = 10, Nombre = "Categorias", Codigo = "categorias", Descripcion = "Acceso a Categorias", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Permiso { Id = 11, Nombre = "Posiciones", Codigo = "posiciones", Descripcion = "Acceso a Posiciones", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Permiso { Id = 12, Nombre = "Jugadores", Codigo = "jugadores", Descripcion = "Acceso a Jugadores", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! }
            );
        }
    }
}
