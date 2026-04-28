using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RugbyEngine.Api.Data.Attributes;
using RugbyEngine.Api.Data.Entities;

namespace RugbyEngine.Api.Data.Seeds
{
    [Seed]
    public class MenusSeed : IEntityTypeConfiguration<Menu>
    {
        private readonly DateTime seedDate = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder.HasData(
                // Nivel 0 — raíces
                new Menu { Id = 1, Titulo = "Entrenamientos", Lvl = 0, MenuPadreId = null, PermisoId = 8, Permiso = null!, Ruta = "Entrenamientos", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Menu { Id = 4, Titulo = "Tesoreria", Lvl = 0, MenuPadreId = null, PermisoId = 2, Permiso = null!, Ruta = "Tesoreria", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Menu { Id = 5, Titulo = "Administracion", Lvl = 0, MenuPadreId = null, PermisoId = 6, Permiso = null!, Ruta = "Admin", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                // Nivel 1
                new Menu { Id = 7, Titulo = "Registro", Lvl = 1, MenuPadreId = 5, PermisoId = 3, Permiso = null!, Ruta = "Registro", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Menu { Id = 11, Titulo = "3T", Lvl = 1, MenuPadreId = 4, PermisoId = 5, Permiso = null!, Ruta = "3T", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                // Nivel 2
                new Menu { Id = 10, Titulo = "Personas", Lvl = 2, MenuPadreId = 7, PermisoId = 4, Permiso = null!, Ruta = "Personas", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Menu { Id = 12, Titulo = "Pagos", Lvl = 2, MenuPadreId = 11, PermisoId = 5, Permiso = null!, Ruta = "Pagos", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Menu { Id = 14, Titulo = "Usuarios", Lvl = 2, MenuPadreId = 7, PermisoId = 1, Permiso = null!, Ruta = "Usuarios", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                // Nivel 1 bajo Entrenamientos
                new Menu { Id = 16, Titulo = "Categor\u00edas", Lvl = 1, MenuPadreId = 1, PermisoId = 10, Permiso = null!, Ruta = "Categorias", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Menu { Id = 17, Titulo = "Posiciones", Lvl = 1, MenuPadreId = 1, PermisoId = 11, Permiso = null!, Ruta = "Posiciones", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Menu { Id = 18, Titulo = "Jugadores", Lvl = 1, MenuPadreId = 1, PermisoId = 12, Permiso = null!, Ruta = "Jugadores", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! },
                new Menu { Id = 19, Titulo = "Entrenamientos", Lvl = 1, MenuPadreId = 1, PermisoId = 8, Permiso = null!, Ruta = "Entrenamientos", CreatedAt = seedDate, CreatedById = 1, CreatedBy = null! }
            );
        }
    }
}
